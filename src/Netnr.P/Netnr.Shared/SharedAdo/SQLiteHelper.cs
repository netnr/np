#if Full || AdoFull || AdoSQLite

using System;
using System.Data;
using System.Data.SQLite;

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
        /// <param name="dataAdapter">执行前修改（命令行脚本、超时等信息）</param>
        /// <param name="openTransaction">开启事务，默认开启</param>
        /// <returns></returns>
        public int BulkBatchSQLite(DataTable dt, string table, Action<SQLiteDataAdapter, DataTable> dataAdapter = null, bool openTransaction = true)
        {
            return SafeConn(() =>
            {
                dt.TableName = table;

                var connection = (SQLiteConnection)Connection;
                SQLiteTransaction transaction = openTransaction ? (SQLiteTransaction)(Transaction = connection.BeginTransaction()) : null;

                var cb = new SQLiteCommandBuilder();

                cb.DataAdapter = new SQLiteDataAdapter
                {
                    SelectCommand = new SQLiteCommand($"select * from {cb.QuotePrefix}{table}{cb.QuoteSuffix} where 0=1", connection, transaction)
                };
                cb.ConflictOption = ConflictOption.OverwriteChanges;

                var da = new SQLiteDataAdapter
                {
                    InsertCommand = cb.GetInsertCommand(true),
                    UpdateCommand = cb.GetUpdateCommand(true)
                };
                da.InsertCommand.CommandTimeout = 300;
                da.UpdateCommand.CommandTimeout = 300;

                //执行前修改
                dataAdapter?.Invoke(da, dt);

                var num = da.Update(dt);

                transaction?.Commit();

                return num;
            });
        }
    }
}

#endif