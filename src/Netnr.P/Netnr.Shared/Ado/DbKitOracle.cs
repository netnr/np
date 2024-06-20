#if Full || AdoAll || AdoOracle

using Oracle.ManagedDataAccess.Client;

namespace Netnr;

/// <summary>
/// Oracle操作类
/// </summary>
public partial class DbKit
{
    /// <summary>
    /// 表批量写入
    /// </summary>
    /// <param name="dt">数据表</param>
    /// <param name="bulkCopyAction">设置表复制对象</param>
    /// <param name="batchSize">每批行数，默认全部</param>
    /// <returns></returns>
    public async Task<int> BulkCopyOracle(DataTable dt, Action<OracleBulkCopy> bulkCopyAction = null, int batchSize = 0)
    {
        return await SafeConn(async () =>
        {
            var connection = (OracleConnection)ConnOption.Connection;

            using var bulkCopy = new OracleBulkCopy(connection)
            {
                DestinationTableName = dt.TableName,
                BatchSize = batchSize > 0 ? batchSize : dt.Rows.Count,
                BulkCopyTimeout = ConnOption.Timeout * 10
            };

            bulkCopyAction?.Invoke(bulkCopy);

            foreach (DataColumn dc in dt.Columns)
            {
                bulkCopy.ColumnMappings.Add(dc.ColumnName, dc.ColumnName);
            }

            await Task.Run(() => bulkCopy.WriteToServer(dt));

            return dt.Rows.Count;
        });
    }

    /// <summary>
    /// 表批量写入
    /// 根据行数据 RowState 状态新增、修改
    /// </summary>
    /// <param name="dt">数据表</param>
    /// <param name="sqlEmpty">查询空表脚本，默认*，可选列，会影响数据更新的列</param>
    /// <param name="dataAdapter">执行前修改（命令行脚本、超时等信息）</param>
    /// <param name="openTransaction">开启事务，默认 True</param>
    /// <returns></returns>
    public async Task<int> BulkBatchOracle(DataTable dt, string sqlEmpty = null, Action<OracleDataAdapter> dataAdapter = null, bool openTransaction = true)
    {
        return await SafeConn(async () =>
        {
            var connection = (OracleConnection)ConnOption.Connection;
            var transaction = openTransaction ? await connection.BeginTransactionAsync() : null;

            var cb = new OracleCommandBuilder();
            if (string.IsNullOrWhiteSpace(sqlEmpty))
            {
                var sntn = DbKitExtensions.SqlSNTN(dt.TableName, dt.Namespace, DBTypes.Oracle);
                sqlEmpty = DbKitExtensions.SqlEmpty(sntn);
            }

            cb.DataAdapter = new OracleDataAdapter
            {
                SelectCommand = new OracleCommand(sqlEmpty, connection)
            };
            cb.ConflictOption = ConflictOption.OverwriteChanges;

            var da = new OracleDataAdapter
            {
                InsertCommand = cb.GetInsertCommand(true),
                UpdateCommand = cb.GetUpdateCommand(true)
            };
            da.InsertCommand.CommandTimeout = ConnOption.Timeout * 10;
            da.UpdateCommand.CommandTimeout = ConnOption.Timeout * 10;

            //执行前修改
            dataAdapter?.Invoke(da);

            int num = da.Update(dt);
            if (transaction != null)
            {
                await transaction.CommitAsync();
            }

            return num;
        });
    }
}

#endif