#if Full || AdoFull || AdoSQLServer

using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace Netnr.SharedAdo
{
    /// <summary>
    /// SQLServer操作类
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
        public int BulkCopySQLServer(DataTable dt, string table, Action<SqlBulkCopy> bulkCopy = null)
        {
            return SafeConn(() =>
            {
                var connection = (SqlConnection)Connection;

                using var bulk = new SqlBulkCopy(connection, SqlBulkCopyOptions.KeepIdentity, null)
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
        public int BulkBatchSQLServer(DataTable dt, string table, Action<SqlDataAdapter> dataAdapter = null, bool openTransaction = true)
        {
            return SafeConn(() =>
            {
                dt.TableName = table;

                var connection = (SqlConnection)Connection;
                SqlTransaction transaction = openTransaction ? (SqlTransaction)(Transaction = connection.BeginTransaction()) : null;

                var cb = new SqlCommandBuilder
                {
                    ConflictOption = ConflictOption.OverwriteChanges,
                    DataAdapter = new SqlDataAdapter
                    {
                        SelectCommand = new SqlCommand($"select * from {table} where 0=1", connection, transaction)
                    }
                };

                var da = new SqlDataAdapter
                {
                    InsertCommand = cb.GetInsertCommand(true),
                    UpdateCommand = cb.GetUpdateCommand(true)
                };
                da.InsertCommand.CommandTimeout = 300;
                da.UpdateCommand.CommandTimeout = 300;

                //执行前修改
                dataAdapter?.Invoke(da);

                var num = da.Update(dt);

                transaction?.Commit();

                return num;
            });
        }
    }
}

#endif