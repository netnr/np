#if Full || AdoAll || AdoMySQL

using Microsoft.Data.SqlClient;
using MySqlConnector;

namespace Netnr;

/// <summary>
/// MySQL操作类
/// </summary>
public partial class DbKit
{
    /// <summary>
    /// 表批量写入
    /// </summary>
    /// <param name="dt">数据表</param>
    /// <param name="bulkCopyAction">设置表复制对象</param>
    /// <param name="openTransaction">开启事务，默认 True</param>
    /// <param name="batchSize">每批行数，默认全部</param>
    /// <returns></returns>
    public async Task<int> BulkCopyMySQL(DataTable dt, Action<MySqlBulkCopy> bulkCopyAction = null, bool openTransaction = true, int batchSize = 0)
    {
        return await SafeConn(async () =>
        {
            var connection = (MySqlConnection)ConnOption.Connection;
            var transaction = openTransaction ? await connection.BeginTransactionAsync() : null;

            var bulkCopy = new MySqlBulkCopy(connection, transaction)
            {
                DestinationTableName = dt.TableName,
                BulkCopyTimeout = ConnOption.Timeout * 10
            };
            bulkCopyAction?.Invoke(bulkCopy);

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                bulkCopy.ColumnMappings.Add(new MySqlBulkCopyColumnMapping(i, dt.Columns[i].ColumnName));
            }

            if (batchSize > 0 && batchSize < dt.Rows.Count)
            {
                #region 不支持 BatchSize 手动分行
                var drCollection = dt.AsEnumerable();

                var num = 0;
                for (int i = 0; i < dt.Rows.Count; i += batchSize)
                {
                    var dataRows = drCollection.Skip(i).Take(batchSize);

                    var result = await bulkCopy.WriteToServerAsync(dataRows, dt.Columns.Count);
                    num += result.RowsInserted;
                }
                if (transaction != null)
                {
                    await transaction.CommitAsync();
                }
                return num;

                #endregion
            }
            else
            {
                var result = await bulkCopy.WriteToServerAsync(dt);
                if (transaction != null)
                {
                    await transaction.CommitAsync();
                }

                return result.RowsInserted;
            }
        });
    }

    /// <summary>
    /// 表批量写入，根据行数据 RowState 状态新增、修改
    /// </summary>
    /// <param name="dt">数据表</param>
    /// <param name="sqlEmpty">查询空表脚本，默认*，可选列，会影响数据更新的列</param>
    /// <param name="dataAdapter">执行前修改（命令行脚本、超时等信息）</param>
    /// <param name="openTransaction">开启事务，默认 True</param>
    /// <returns></returns>
    public async Task<int> BulkBatchMySQL(DataTable dt, string sqlEmpty = null, Action<MySqlDataAdapter> dataAdapter = null, bool openTransaction = true)
    {
        return await SafeConn(async () =>
        {
            var connection = (MySqlConnection)ConnOption.Connection;
            var transaction = openTransaction ? await connection.BeginTransactionAsync() : null;

            var cb = new MySqlCommandBuilder();
            if (string.IsNullOrWhiteSpace(sqlEmpty))
            {
                var sntn = DbKitExtensions.SqlSNTN(dt.TableName, dt.Namespace, DBTypes.MySQL);
                sqlEmpty = DbKitExtensions.SqlEmpty(sntn);
            }

            cb.DataAdapter = new MySqlDataAdapter
            {
                SelectCommand = new MySqlCommand(sqlEmpty, connection, transaction)
            };
            cb.ConflictOption = ConflictOption.OverwriteChanges;

            var da = new MySqlDataAdapter
            {
                InsertCommand = (MySqlCommand)cb.GetInsertCommand(true),
                UpdateCommand = (MySqlCommand)cb.GetUpdateCommand(true)
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