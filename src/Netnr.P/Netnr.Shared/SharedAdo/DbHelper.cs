#if Full || Ado || AdoFull

using System.Text;
using System.Data;
using System.Data.Common;
using System.ComponentModel;

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
        /// 记录
        /// </summary>
        public Dictionary<string, DbCommand> DicCommand { get; set; } = new Dictionary<string, DbCommand>();

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="dbConnection">连接对象</param>
        public DbHelper(DbConnection dbConnection)
        {
            Connection = dbConnection;
        }

        /// <summary>
        /// 执行（查询、新增、修改、删除等）
        /// </summary>
        /// <param name="sql">SQL语句，支持多条</param>
        /// <param name="parameters">带参</param>
        /// <param name="func">回调</param>
        /// <param name="includeSchemaTable">包含表结构</param>
        /// <returns>返回 表数据、受影响行数、表结构</returns>
        public Tuple<DataSet, int, DataSet> SqlExecuteReader(string sql, DbParameter[] parameters = null, Func<DbCommand, DbCommand> func = null, bool includeSchemaTable = false)
        {
            return SafeConn(() =>
            {
                var ds = new DataSet();
                var dsSchema = new DataSet();
                int recordsAffected = -1;

                var isOracle = Connection.GetType().FullName.ToLower().Contains("oracle");

                if (!isOracle)
                {
                    var dbc = GetCommand(sql, parameters);
                    if (func != null)
                    {
                        dbc = func(dbc);
                    }
                    var eds = dbc.ExecuteDataSet(includeSchemaTable);
                    ds = eds.Item1;
                    recordsAffected = eds.Item2;
                    dsSchema = eds.Item3;

                    if (DicCommand.ContainsKey(dbc.Site.Name))
                    {
                        DicCommand.Remove(dbc.Site.Name);
                    }
                }
                else
                {
                    var listSql = sql.Split(';').ToList();
                    var hasRa = false;
                    var ra = 0;

                    foreach (var txt in listSql)
                    {
                        var dbc = GetCommand(txt, parameters);
                        if (func != null)
                        {
                            dbc = func(dbc);
                        }
                        var eds = dbc.ExecuteDataSet(includeSchemaTable);

                        if (DicCommand.ContainsKey(dbc.Site.Name))
                        {
                            DicCommand.Remove(dbc.Site.Name);
                        }

                        while (eds.Item1.Tables.Count > 0)
                        {
                            var dt = eds.Item1.Tables[0];
                            dt.TableName = $"table{ds.Tables.Count + 1}";
                            eds.Item1.Tables.RemoveAt(0);
                            ds.Tables.Add(dt);
                        }

                        if (eds.Item2 != -1)
                        {
                            hasRa = true;
                            ra += eds.Item2;
                        }

                        while (eds.Item3.Tables.Count > 0)
                        {
                            var dt = eds.Item3.Tables[0];
                            dt.TableName = $"table{dsSchema.Tables.Count + 1}";
                            eds.Item3.Tables.RemoveAt(0);
                            dsSchema.Tables.Add(dt);
                        }
                    }

                    if (hasRa)
                    {
                        recordsAffected = ra;
                    }
                }

                return new Tuple<DataSet, int, DataSet>(ds, recordsAffected, dsSchema);
            });
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">带参</param>
        /// <returns>返回受影响行数</returns>
        public object SqlExecuteScalar(string sql, DbParameter[] parameters = null)
        {
            return SafeConn(() =>
            {
                var dbc = GetCommand(sql, parameters);
                var obj = dbc.ExecuteScalar();

                if (DicCommand.ContainsKey(dbc.Site.Name))
                {
                    DicCommand.Remove(dbc.Site.Name);
                }
                return obj;
            });
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">带参</param>
        /// <returns>返回受影响行数</returns>
        public int SqlExecuteNonQuery(string sql, DbParameter[] parameters = null)
        {
            return SafeConn(() =>
            {
                var dbc = GetCommand(sql, parameters);
                var num = dbc.ExecuteNonQuery();

                if (DicCommand.ContainsKey(dbc.Site.Name))
                {
                    DicCommand.Remove(dbc.Site.Name);
                }
                return num;
            });
        }

        /// <summary>
        /// 执行（批量、事务）
        /// </summary>
        /// <param name="listSql">SQL语句</param>
        /// <param name="sqlBatchSize">脚本分批大小，单位：字节（byte），默认：1024 * 100 = 100KB</param>
        /// <returns>返回受影响行数</returns>
        public int SqlExecuteNonQuery(List<string> listSql, int sqlBatchSize = 1024 * 100)
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
                    sbsql.AppendLine(sql.TrimEnd(';') + ";");
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
                    var dbc = GetCommand(bs);
                    dbc.Transaction = Transaction;
                    num += dbc.ExecuteNonQuery();

                    if (DicCommand.ContainsKey(dbc.Site.Name))
                    {
                        DicCommand.Remove(dbc.Site.Name);
                    }
                }

                Transaction.Commit();
                return num;
            });
        }

        /// <summary>
        /// 返回空表格
        /// </summary>
        /// <param name="table">数据库表名</param>
        /// <param name="cb">构建对象，取引用符号</param>
        /// <returns></returns>
        public DataTable SqlEmptyTable(string table, DbCommandBuilder cb = null)
        {
            var dt = SqlExecuteReader($"select * from {cb?.QuotePrefix}{table}{cb?.QuoteSuffix} where 0=1").Item1.Tables[0];
            return dt;
        }

        /// <summary>
        /// 拿到 DbCommand
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">带参</param>
        /// <param name="timeout">超时，默认 300 秒</param>
        /// <param name="commandType">类型</param>
        /// <returns></returns>
        public DbCommand GetCommand(string sql, DbParameter[] parameters = null, int timeout = 300, CommandType commandType = CommandType.Text)
        {
            var cmd = Connection.CreateCommand();
            cmd.Site = new DBCSite();
            cmd.CommandTimeout = timeout;
            cmd.CommandType = commandType;
            cmd.CommandText = sql;

            if (parameters != null)
            {
                cmd.Parameters.AddRange(parameters);
            }

            DicCommand.Add(cmd.Site.Name, cmd);

            return cmd;
        }

        class DBCSite : ISite
        {
            public DBCSite(string name = null)
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    name = Guid.NewGuid().ToString("N");
                }
                Name = name;
            }

            public IComponent Component => null;

            public IContainer Container => null;

            public bool DesignMode => false;

            public string Name { get; set; }

            public object GetService(Type serviceType)
            {
                return null;
            }
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
                Console.WriteLine(ex);
                Transaction?.Rollback();
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
}

#endif