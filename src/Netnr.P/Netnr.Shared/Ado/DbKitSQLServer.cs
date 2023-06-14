#if Full || AdoAll || AdoSQLServer

using Microsoft.Data.SqlClient;

namespace Netnr;

/// <summary>
/// SQLServer操作类
/// </summary>
public partial class DbKit
{
    /// <summary>
    /// 表批量写入
    /// </summary>
    /// <param name="dt">数据表（Namespace=SchemaName，TableName=TableName）</param>
    /// <param name="bulkCopy">设置表复制对象</param>
    /// <param name="openTransaction">开启事务，默认 True</param>
    /// <returns></returns>
    public async Task<int> BulkCopySQLServer(DataTable dt, Action<SqlBulkCopy> bulkCopy = null, bool openTransaction = true)
    {
        return await SafeConn(async () =>
        {
            var connection = (SqlConnection)ConnOption.Connection;
            var transaction = openTransaction ? (SqlTransaction)await connection.BeginTransactionAsync() : null;

            using var bulk = new SqlBulkCopy(connection, SqlBulkCopyOptions.KeepIdentity, transaction)
            {
                DestinationTableName = DbKitExtensions.SqlSNTN(dt.TableName, dt.Namespace, EnumTo.TypeDB.SQLServer),
                BatchSize = dt.Rows.Count,
                BulkCopyTimeout = ConnOption.Timeout * 10
            };

            bulkCopy?.Invoke(bulk);

            foreach (DataColumn dc in dt.Columns)
            {
                bulk.ColumnMappings.Add(dc.ColumnName, dc.ColumnName);
            }

            await bulk.WriteToServerAsync(dt);
            if (transaction != null)
            {
                await transaction.CommitAsync();
            }

            return dt.Rows.Count;
        });
    }

    /// <summary>
    /// 表批量写入
    /// 根据行数据 RowState 状态新增、修改
    /// </summary>
    /// <param name="dt">数据表（Namespace=SchemaName，TableName=TableName）</param>
    /// <param name="sqlEmpty">查询空表脚本，默认*，可选列，会影响数据更新的列</param>
    /// <param name="dataAdapter">执行前修改（命令行脚本、超时等信息）</param>
    /// <param name="openTransaction">开启事务，默认开启</param>
    /// <returns></returns>
    public async Task<int> BulkBatchSQLServer(DataTable dt, string sqlEmpty = null, Action<SqlDataAdapter> dataAdapter = null, bool openTransaction = true)
    {
        return await SafeConn(async () =>
        {
            var connection = (SqlConnection)ConnOption.Connection;
            var transaction = openTransaction ? (SqlTransaction)await connection.BeginTransactionAsync() : null;

            var cb = new SqlCommandBuilder();
            if (string.IsNullOrWhiteSpace(sqlEmpty))
            {
                var sntn = DbKitExtensions.SqlSNTN(dt.TableName, dt.Namespace, EnumTo.TypeDB.SQLServer);
                sqlEmpty = DbKitExtensions.SqlEmpty(sntn);
            }

            cb.DataAdapter = new SqlDataAdapter
            {
                SelectCommand = new SqlCommand(sqlEmpty, connection, transaction)
            };
            cb.ConflictOption = ConflictOption.OverwriteChanges;

            var da = new SqlDataAdapter
            {
                InsertCommand = cb.GetInsertCommand(true),
                UpdateCommand = cb.GetUpdateCommand(true)
            };
            da.InsertCommand.CommandTimeout = ConnOption.Timeout * 10;
            da.UpdateCommand.CommandTimeout = ConnOption.Timeout * 10;

            //执行前修改
            dataAdapter?.Invoke(da);

            var num = da.Update(dt);
            if (transaction != null)
            {
                await transaction.CommitAsync();
            }

            return num;
        });
    }
}

#endif