#if Full || AdoFull || AdoOracle

using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;

namespace Netnr.SharedAdo
{
    /// <summary>
    /// Oracle操作类
    /// </summary>
    public partial class DbHelper
    {
        /// <summary>
        /// 表批量写入
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="table">数据库表名</param>
        /// <param name="bulkCopy">设置表复制对象</param>
        /// <returns></returns>
        public int BulkCopyOracle(DataTable dt, string table, Action<OracleBulkCopy> bulkCopy = null)
        {
            return SafeConn(() =>
            {
                var connection = (OracleConnection)Connection;
                using var bulk = new OracleBulkCopy(connection)
                {
                    DestinationTableName = table,
                    BatchSize = dt.Rows.Count,
                    BulkCopyTimeout = 3600
                };

                bulkCopy?.Invoke(bulk);

                foreach (DataColumn dc in dt.Columns)
                {
                    bulk.ColumnMappings.Add(dc.ColumnName, dc.ColumnName);
                }

                bulk.WriteToServer(dt);

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
        public int BulkBatchOracle(DataTable dt, string table, Action<OracleDataAdapter, DataTable> dataAdapter = null, bool openTransaction = true)
        {
            return SafeConn(() =>
            {
                dt.TableName = table;

                var connection = (OracleConnection)Connection;
                OracleTransaction transaction = openTransaction ? (OracleTransaction)(Transaction = connection.BeginTransaction()) : null;

                var cb = new OracleCommandBuilder();
                var quoteTable = $"{cb.QuotePrefix}{table}{cb.QuoteSuffix}";
                cb.DataAdapter = new OracleDataAdapter
                {
                    SelectCommand = new OracleCommand($"select * from {quoteTable} where 0=1", connection)
                };
                cb.ConflictOption = ConflictOption.OverwriteChanges;

                var da = new OracleDataAdapter
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

        /// <summary>
        /// 执行SQL语句，返回打印信息
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">带参</param>
        /// <returns>返回打印信息</returns>
        public List<string> SqlPrintOracle(string sql, DbParameter[] parameters = null)
        {
            return SafeConn(() =>
            {
                var connection = (OracleConnection)Connection;

                var listPrint = new List<string>();
                connection.InfoMessage += (s, e) => listPrint.Add(e.Message);

                var cmd = GetCommand(sql, parameters);
                cmd.ExecuteNonQuery();

                return listPrint;
            });
        }
    }
}

#endif