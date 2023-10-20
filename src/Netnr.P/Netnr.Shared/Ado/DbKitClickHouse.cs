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
    /// <param name="bulkCopy">设置表复制对象</param>
    /// <returns></returns>
    public async Task<int> BulkCopyClickHouse(DataTable dt, Action<ClickHouseBulkCopy> bulkCopy = null)
    {
        return await SafeConn(async () =>
        {
            var connection = (ClickHouseConnection)ConnOption.Connection;

            using var bulk = new ClickHouseBulkCopy(connection)
            {
                DestinationTableName = dt.TableName,
                BatchSize = dt.Rows.Count
            };

            bulkCopy?.Invoke(bulk);

            var cts = new CancellationTokenSource();
            await bulk.WriteToServerAsync(dt, cts.Token);

            return (int)bulk.RowsWritten;
        });
    }
}

#endif