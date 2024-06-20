#if Full || AdoAll || AdoDm

using Dm;
using System.Text.RegularExpressions;

namespace Netnr;

/// <summary>
/// Dm操作类
/// </summary>
public partial class DbKit
{
    /// <summary>
    /// 表批量写入
    /// 初步测试报错：The faseloading dll not loading
    /// </summary>
    /// <param name="dt">数据表</param>
    /// <param name="bulkCopyAction">设置表复制对象</param>
    /// <param name="openTransaction">开启事务，默认 True</param>
    /// <param name="batchSize">每批行数，默认全部</param>
    /// <returns></returns>
    public async Task<int> BulkCopyDm(DataTable dt, Action<DmBulkCopy> bulkCopyAction = null, bool openTransaction = true, int batchSize = 0)
    {
        return await SafeConn(async () =>
        {
            var connection = (DmConnection)ConnOption.Connection;
            var transaction = openTransaction ? (DmTransaction)(await connection.BeginTransactionAsync()) : null;

            var bulkCopy = new DmBulkCopy(connection, DmBulkCopyOptions.Default, transaction)
            {
                DestinationTableName = dt.TableName,
                BulkCopyTimeout = ConnOption.Timeout * 10,
                BatchSize = batchSize > 0 ? batchSize : dt.Rows.Count
            };
            bulkCopyAction?.Invoke(bulkCopy);

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                bulkCopy.ColumnMappings.Add(new DmBulkCopyColumnMapping(i, dt.Columns[i].ColumnName));
            }

            bulkCopy.WriteToServer(dt);
            if (transaction != null)
            {
                await transaction.CommitAsync();
            }

            return dt.Rows.Count;
        });
    }

    /// <summary>
    /// 表批量写入，根据行数据 RowState 状态新增、修改
    /// 需要开启连接池，https://eco.dameng.com/community/question/3f1acf7922e5fd27bed66f0edeb28c1f
    /// </summary>
    /// <param name="dt">数据表</param>
    /// <param name="sqlEmpty">查询空表脚本，默认*，可选列，会影响数据更新的列</param>
    /// <param name="dataAdapter">执行前修改（命令行脚本、超时等信息）</param>
    /// <param name="openTransaction">开启事务，默认 True</param>
    /// <returns></returns>
    public async Task<int> BulkBatchDm(DataTable dt, string sqlEmpty = null, Action<DmDataAdapter> dataAdapter = null, bool openTransaction = true)
    {
        return await SafeConn(async () =>
        {
            var connection = (DmConnection)ConnOption.Connection;

            //强制使用连接池
            if (!connection.ConnectionString.Contains("conn_pooling=True", StringComparison.OrdinalIgnoreCase))
            {
                await Console.Out.WriteLineAsync("Reconnect using connection pool");
                var csb = new DmConnectionStringBuilder(connection.ConnectionString)
                {
                    ConnPooling = true
                };
                connection = new DmConnection(csb.ConnectionString);
                await connection.OpenAsync();
                ConnOption.Connection = connection;
            }

            var transaction = openTransaction ? (DmTransaction)(await connection.BeginTransactionAsync()) : null;

            var cb = new DmCommandBuilder();
            if (string.IsNullOrWhiteSpace(sqlEmpty))
            {
                var sntn = DbKitExtensions.SqlSNTN(dt.TableName, dt.Namespace, DBTypes.Dm);
                sqlEmpty = DbKitExtensions.SqlEmpty(sntn);
            }

            cb.DataAdapter = new DmDataAdapter
            {
                SelectCommand = new DmCommand(sqlEmpty, connection, transaction)
            };
            cb.ConflictOption = ConflictOption.OverwriteChanges;

            var da = new DmDataAdapter
            {
                InsertCommand = (DmCommand)cb.GetInsertCommand(true),
                UpdateCommand = (DmCommand)cb.GetUpdateCommand(true)
            };

            // fix :TYPE => :"TYPE"
            da.InsertCommand.CommandText = MatchVarKeyword().Replace(da.InsertCommand.CommandText, match => ":\"" + match.Groups[1].Value + "\"");
            da.UpdateCommand.CommandText = MatchVarKeyword().Replace(da.UpdateCommand.CommandText, match => ":\"" + match.Groups[1].Value + "\"");

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

    [GeneratedRegex(@":(\w+)", RegexOptions.Multiline)]
    private static partial Regex MatchVarKeyword();
}

#endif