#if Full || AdoFull || AdoMySQL

using System;
using System.Linq;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using MySqlConnector;

namespace Netnr.SharedAdo
{
    /// <summary>
    /// MySQL操作类
    /// </summary>
    public partial class DbHelper
    {
        /// <summary>
        /// 表批量写入
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="table">数据库表名</param>
        /// <param name="bulkCopy">设置表复制对象</param>
        /// <param name="openTransaction">开启事务，默认不开启</param>
        /// <returns></returns>
        public int BulkCopyMySQL(DataTable dt, string table, Action<MySqlBulkCopy> bulkCopy = null, bool openTransaction = true)
        {
            return SafeConn(() =>
            {
                var connection = (MySqlConnection)Connection;
                MySqlTransaction transaction = openTransaction ? (MySqlTransaction)(Transaction = connection.BeginTransaction()) : null;

                var bulk = new MySqlBulkCopy(connection, transaction)
                {
                    DestinationTableName = table,
                    BulkCopyTimeout = 3600
                };

                bulkCopy?.Invoke(bulk);

                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    bulk.ColumnMappings.Add(new MySqlBulkCopyColumnMapping(i, dt.Columns[i].ColumnName));
                }

                bulk.WriteToServer(dt);

                transaction?.Commit();

                return dt.Rows.Count;
            });
        }

        /// <summary>
        /// 表批量写入
        /// 根据行数据 RowState 状态新增、修改
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="table">数据库表名</param>
        /// <param name="dataAdapter">执行前修改（命令行脚本、超时等信息）</param>
        /// <param name="openTransaction">开启事务，默认开启</param>
        /// <returns></returns>
        public int BulkBatchMySQL(DataTable dt, string table, Action<MySqlDataAdapter, DataTable> dataAdapter = null, bool openTransaction = true)
        {
            return SafeConn(() =>
            {
                dt.TableName = table;

                var connection = (MySqlConnection)Connection;
                MySqlTransaction transaction = openTransaction ? (MySqlTransaction)(Transaction = connection.BeginTransaction()) : null;

                var cb = new MySqlCommandBuilder();
                var quoteTable = $"{cb.QuotePrefix}{table}{cb.QuoteSuffix}";
                cb.DataAdapter = new MySqlDataAdapter
                {
                    SelectCommand = new MySqlCommand($"select * from {quoteTable} where 0=1", connection, transaction)
                };
                cb.ConflictOption = ConflictOption.OverwriteChanges;

                var da = new MySqlDataAdapter
                {
                    InsertCommand = (MySqlCommand)cb.GetInsertCommand(true),
                    UpdateCommand = (MySqlCommand)cb.GetUpdateCommand(true)
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

        /// <summary>
        /// 执行SQL语句，返回打印信息
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">带参</param>
        /// <param name="action"></param>
        /// <returns>返回打印信息</returns>
        public int SqlPrintMySQL(string sql, DbParameter[] parameters = null, Action<IReadOnlyList<MySqlError>> action = null)
        {
            return SafeConn(() =>
            {
                var connection = (MySqlConnection)Connection;

                var listPrint = new List<string>();
                connection.InfoMessage += (s, e) => action?.Invoke(e.Errors);

                var cmd = GetCommand(sql, parameters);
                var num = cmd.ExecuteNonQuery();

                return num;
            });
        }
    }
}

#endif