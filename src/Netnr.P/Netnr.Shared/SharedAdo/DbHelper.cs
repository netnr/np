#if Full || Ado

using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;

namespace Netnr.SharedAdo
{
    /// <summary>
    /// Db帮助类
    /// </summary>
    public partial class DbHelper
    {
        /// <summary>
        /// 连接对象
        /// </summary>
        public DbConnection Connection { get; }

        /// <summary>
        /// 事务
        /// </summary>
        public DbTransaction Transaction { get; set; }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="dbConnection">连接对象</param>
        public DbHelper(DbConnection dbConnection)
        {
            Connection = dbConnection;
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sql">SQL语句，支持多条</param>
        /// <param name="parameters">带参</param>
        /// <param name="action">回调</param>
        /// <returns>返回数据集</returns>
        public DataSet SqlQuery(string sql, DbParameter[] parameters = null, Action<DbCommand> action = null)
        {
            return SafeConn(() =>
            {
                var cname = Connection.GetType().FullName.ToLower();
                var isOracle = cname.Contains("oracle");

                var listSql = new List<string>() { sql };
                if (isOracle)
                {
                    listSql = sql.Split(';').ToList();
                }

                var ds = new DataSet();

                foreach (var txt in listSql)
                {
                    var dbc = GetCommand(txt, parameters);
                    action?.Invoke(dbc);
                    var tds = dbc.ExecuteDataSet();
                    while (tds.Tables.Count > 0)
                    {
                        var dt = tds.Tables[0];
                        dt.TableName = "table" + (ds.Tables.Count + 1).ToString();
                        tds.Tables.RemoveAt(0);
                        ds.Tables.Add(dt);
                    }
                }

                return ds;
            });
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">带参</param>
        /// <returns>返回受影响行数</returns>
        public object SqlScalar(string sql, DbParameter[] parameters = null)
        {
            return SafeConn(() =>
            {
                var obj = GetCommand(sql, parameters).ExecuteScalar();
                return obj;
            });
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">带参</param>
        /// <returns>返回受影响行数</returns>
        public int SqlExecute(string sql, DbParameter[] parameters = null)
        {
            return SafeConn(() =>
            {
                var num = GetCommand(sql, parameters).ExecuteNonQuery();
                return num;
            });
        }

        /// <summary>
        /// 执行（批量、事务）
        /// </summary>
        /// <param name="listSql">SQL语句</param>
        /// <param name="sqlBatchSize">脚本分批大小，单位：字节（byte），默认：1024 * 100 = 100KB</param>
        /// <returns>返回受影响行数</returns>
        public int SqlExecute(List<string> listSql, int sqlBatchSize = 1024 * 100)
        {
            return SafeConn(() =>
            {
                Transaction = Connection.BeginTransaction();
                var num = 0;

                var listBatchSql = new List<string>();
                StringBuilder sbsql = new();
                var currSqlSize = 0;
                foreach (var sql in listSql)
                {
                    currSqlSize += Encoding.Default.GetBytes(sql).Length;
                    sbsql.AppendLine(sql + ";");
                    if (currSqlSize > sqlBatchSize)
                    {
                        listBatchSql.Add(sbsql.ToString());
                        sbsql.Clear();
                        currSqlSize = 0;
                    }
                }
                if (currSqlSize != 0)
                {
                    listBatchSql.Add(sbsql.ToString());
                }

                foreach (var bs in listBatchSql)
                {
                    var cmd = GetCommand(bs);
                    cmd.Transaction = Transaction;
                    num += cmd.ExecuteNonQuery();
                }

                Transaction.Commit();
                return num;
            });
        }

        /// <summary>
        /// 拿到 DbCommand
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">带参</param>
        /// <param name="commandType">类型</param>
        /// <returns></returns>
        public DbCommand GetCommand(string sql, DbParameter[] parameters = null, CommandType commandType = CommandType.Text)
        {
            var cmd = Connection.CreateCommand();
            cmd.CommandType = commandType;
            cmd.CommandText = sql;

            if (parameters != null)
            {
                cmd.Parameters.AddRange(parameters);
            }

            return cmd;
        }

        /// <summary>
        /// 连接包装
        /// </summary>
        /// <param name="action"></param>
        public void SafeConn(Action action)
        {
            SafeConn(() => { action(); return 0; });
        }

        /// <summary>
        /// 连接包装
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        public T SafeConn<T>(Func<T> action)
        {
            try
            {
                if (Connection.State == ConnectionState.Closed)
                {
                    Connection.Open();
                }

                return action();
            }
            catch (Exception ex)
            {
                Transaction?.Rollback();
                Console.WriteLine(ex);
                throw;
            }
            finally
            {
                Transaction?.Dispose();
                if (Connection.State == ConnectionState.Open)
                {
                    Connection.Close();
                }
            }
        }
    }

    /// <summary>
    /// 扩展
    /// </summary>
    public static class DbHelperExtend
    {
        /// <summary>
        /// 查询返回数据集
        /// </summary>
        /// <param name="dbCommand"></param>
        /// <returns></returns>
        public static DataSet ExecuteDataSet(this DbCommand dbCommand)
        {
            var ds = new DataSet();
            var reader = dbCommand.ExecuteReader();

            var isSQLite = dbCommand.Connection.GetType().FullName.ToLower().Contains("sqlite");
            // https://github.com/dotnet/efcore/issues/23490
            if (isSQLite)
            {
                do
                {
                    var table = new DataTable
                    {
                        TableName = "table" + (ds.Tables.Count + 1).ToString()
                    };

                    var readerSt = reader.GetSchemaTable();
                    var ctype = typeof(object);
                    for (int i = 0; i < readerSt.Rows.Count; i++)
                    {
                        table.Columns.Add(new DataColumn(readerSt.Rows[i]["ColumnName"].ToString(), ctype));
                    }

                    while (reader.Read())
                    {
                        var dr = table.NewRow();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            dr[i] = reader.GetValue(i);
                        }
                        table.Rows.Add(dr.ItemArray);
                    }
                    ds.Tables.Add(table);

                } while (reader.NextResult());
            }
            else
            {
                do
                {
                    var table = new DataTable
                    {
                        TableName = "table" + (ds.Tables.Count + 1).ToString()
                    };
                    table.Load(reader);
                    ds.Tables.Add(table);
                } while (!reader.IsClosed);
            }

            return ds;
        }
    }
}

#endif