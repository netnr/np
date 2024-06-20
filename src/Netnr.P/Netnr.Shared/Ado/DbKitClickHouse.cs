#if Full || AdoAll || AdoClickHouse

using ClickHouse.Client.ADO;
using ClickHouse.Client.Copy;

namespace Netnr;

/// <summary>
/// ClickHouse操作类
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
    public async Task<int> BulkCopyClickHouse(DataTable dt, Action<ClickHouseBulkCopy> bulkCopyAction = null, int batchSize = 0)
    {
        return await SafeConn(async () =>
        {
            var connection = (ClickHouseConnection)ConnOption.Connection;

            using var bulkCopy = new ClickHouseBulkCopy(connection)
            {
                DestinationTableName = dt.TableName,
                ColumnNames = dt.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToArray(),
                BatchSize = batchSize > 0 ? batchSize : dt.Rows.Count,
            };

            bulkCopyAction?.Invoke(bulkCopy);

            await bulkCopy.InitAsync();
            var cts = new CancellationTokenSource();
            await bulkCopy.WriteToServerAsync(dt, cts.Token);

            return (int)bulkCopy.RowsWritten;
        });
    }
}

#endif