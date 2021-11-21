#if Full || AdoFull || AdoSQLite

using System;
using System.Data;
using Microsoft.Data.Sqlite;

namespace Netnr.SharedAdo
{
    /// <summary>
    /// SQLite操作类
    /// </summary>
    public partial class DbHelper
    {
        /// <summary>
        /// 表批量写入
        /// 根据行数据 RowState 状态新增、修改
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="table">数据库表名</param>
        /// <param name="openTransaction">开启事务</param>
        /// <returns></returns>
        public int BulkBatchSQLite(DataTable dt, string table, bool openTransaction = true)
        {
            return SafeConn(() =>
            {
                dt.TableName = table;

                var connection = (SqliteConnection)Connection;
                SqliteTransaction transaction = openTransaction ? (SqliteTransaction)(Transaction = connection.BeginTransaction()) : null;

                var listCols = dt.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToList();
                var sql = $"INSERT INTO [{table}]([{string.Join("],[", listCols)}]) VALUES (${string.Join(", $", listCols)})";

                var cmd = connection.CreateCommand();
                cmd.CommandText = sql;

                var parameters = new List<SqliteParameter>();
                listCols.ForEach(col =>
                {
                    var parameter = cmd.CreateParameter();
                    parameter.ParameterName = $"${col}";
                    cmd.Parameters.Add(parameter);

                    parameters.Add(parameter);
                });

                var nums = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    for (int i = 0; i < listCols.Count; i++)
                    {
                        var cell = dr[listCols[i]];
                        if (cell == DBNull.Value)
                        {
                            parameters[i].Value = DBNull.Value;
                        }
                        else
                        {
                            parameters[i].Value = cell;
                        }
                    }

                    var num = cmd.ExecuteNonQuery();
                    nums += num;
                }

                transaction?.Commit();

                return nums;
            });
        }

        //public int BulkBatchSQLite(DataTable dt, string table, Action<SQLiteDataAdapter> dataAdapter = null, bool openTransaction = true)
        //{
        //    return SafeConn(() =>
        //    {
        //        dt.TableName = table;

        //        var connection = (SQLiteConnection)Connection;
        //        SQLiteTransaction transaction = openTransaction ? (SQLiteTransaction)(Transaction = connection.BeginTransaction()) : null;

        //        var cb = new SQLiteCommandBuilder();

        //        cb.DataAdapter = new SQLiteDataAdapter
        //        {
        //            SelectCommand = new SQLiteCommand($"select * from {cb.QuotePrefix}{table}{cb.QuoteSuffix} where 0=1", connection, transaction)
        //        };
        //        cb.ConflictOption = ConflictOption.OverwriteChanges;

        //        var da = new SQLiteDataAdapter
        //        {
        //            InsertCommand = cb.GetInsertCommand(true),
        //            UpdateCommand = cb.GetUpdateCommand(true)
        //        };
        //        da.InsertCommand.CommandTimeout = 300;
        //        da.UpdateCommand.CommandTimeout = 300;

        //        //执行前修改
        //        dataAdapter?.Invoke(da);

        //        var num = da.Update(dt);

        //        transaction?.Commit();

        //        return num;
        //    });
        //}

    }
}

#endif