﻿#if Full || AdoAll || AdoPostgreSQL

using Npgsql;
using NpgsqlTypes;

namespace Netnr;

/// <summary>
/// PostgreSQL操作类
/// </summary>
public partial class DbKit
{
    /// <summary>
    /// 表批量写入（排除自增列）
    /// https://www.npgsql.org/doc/copy.html
    /// </summary>
    /// <param name="dt">数据表（Namespace=SchemaName，TableName=TableName）</param>
    /// <returns></returns>
    public async Task<int> BulkCopyPostgreSQL(DataTable dt)
    {
        return await SafeConn(async () =>
        {
            var connection = (NpgsqlConnection)ConnOption.Connection;

            //提取表列类型与数据库类型
            var cb = new NpgsqlCommandBuilder();
            var sntn = DbKitExtensions.SqlSNTN(dt.TableName, dt.Namespace, DBTypes.PostgreSQL);
            cb.DataAdapter = new NpgsqlDataAdapter
            {
                SelectCommand = new NpgsqlCommand($"select * from {sntn} where 0=1", connection)
            };

            //获取列类型
            var pars = cb.GetInsertCommand(true).Parameters;
            var colDbType = new Dictionary<string, NpgsqlDbType>();
            pars.ForEach(x => colDbType.Add(x.SourceColumn, x.NpgsqlDbType));

            //获取自增
            var dtSchema = new DataTable();
            cb.DataAdapter.FillSchema(dtSchema, SchemaType.Source);
            var autoIncrCol = dtSchema.Columns.Cast<DataColumn>().Where(x => x.AutoIncrement == true).Select(x => x.ColumnName).ToList();

            //自增列是主键时，清空主键属性
            var pkCol = dt.PrimaryKey.Select(x => x.ColumnName);
            if (autoIncrCol.Any(x => pkCol.Contains(x)))
            {
                dt.PrimaryKey = null;
            }

            //排除自增
            var columns = dt.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToList();
            columns = columns.Except(autoIncrCol).ToList();
            string copyString = $"COPY {sntn}(\"" + string.Join("\",\"", columns) + "\") FROM STDIN (FORMAT BINARY)";
            autoIncrCol.ForEach(x => dt.Columns.Remove(x));

            var num = 0;
            using (var writer = await connection.BeginBinaryImportAsync(copyString))
            {
                writer.Timeout = TimeSpan.FromSeconds(ConnOption.Timeout * 10);

                foreach (DataRow dr in dt.Rows)
                {
                    await writer.StartRowAsync();
                    foreach (DataColumn dc in dt.Columns)
                    {
                        var val = dr[dc.ColumnName];
                        if (val is not DBNull)
                        {
                            //列对应数据库类型
                            var dbType = colDbType[dc.ColumnName];
                            if (dc.DataType.FullName == "System.String")
                            {
                                await writer.WriteAsync(val.ToString().Replace("\0", ""), dbType);
                            }
                            else
                            {
                                await writer.WriteAsync(val, dbType);
                            }
                        }
                        else
                        {
                            await writer.WriteNullAsync();
                        }
                    }
                }

                num = (int)await writer.CompleteAsync();
            }

            return num;
        });
    }

    /// <summary>
    /// 表批量写入（须手动处理自增列SQL）
    /// 根据行数据 RowState 状态新增、修改
    /// </summary>
    /// <param name="dt">数据表（Namespace=SchemaName，TableName=TableName）</param>
    /// <param name="sqlEmpty">查询空表脚本，默认*，可选列，会影响数据更新的列</param>
    /// <param name="dataAdapter">执行前修改（命令行脚本、超时等信息）</param>
    /// <param name="openTransaction">开启事务，默认 True</param>
    /// <returns></returns>
    public async Task<int> BulkBatchPostgreSQL(DataTable dt, string sqlEmpty = null, Action<NpgsqlDataAdapter> dataAdapter = null, bool openTransaction = true)
    {
        return await SafeConn(async () =>
        {
            var connection = (NpgsqlConnection)ConnOption.Connection;
            var transaction = openTransaction ? await connection.BeginTransactionAsync() : null;

            var cb = new NpgsqlCommandBuilder();
            if (string.IsNullOrWhiteSpace(sqlEmpty))
            {
                var sntn = DbKitExtensions.SqlSNTN(dt.TableName, dt.Namespace, DBTypes.PostgreSQL);
                sqlEmpty = DbKitExtensions.SqlEmpty(sntn);
            }

            cb.DataAdapter = new NpgsqlDataAdapter
            {
                SelectCommand = new NpgsqlCommand(sqlEmpty, connection, transaction)
            };
            cb.ConflictOption = ConflictOption.OverwriteChanges;

            var da = new NpgsqlDataAdapter
            {
                InsertCommand = cb.GetInsertCommand(true),
                UpdateCommand = cb.GetUpdateCommand(true)
            };
            da.InsertCommand.CommandTimeout = ConnOption.Timeout * 10;
            da.UpdateCommand.CommandTimeout = ConnOption.Timeout * 10;

            //处理：无效的 "UTF8" 编码字节顺序: 0x00
            var listColName = dt.Columns.Cast<DataColumn>().Where(x => x.DataType == typeof(string)).Select(x => x.ColumnName).ToList();
            foreach (DataRow dr in dt.Rows)
            {
                listColName.ForEach(colName =>
                {
                    var val = dr[colName];
                    if (val is not DBNull)
                    {
                        dr[colName] = val.ToString().Replace("\0", "");
                    }
                });
            }

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

    /// <summary>
    /// 表批量写入（保留自增值）
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    public async Task<int> BulkKeepIdentityPostgreSQL(DataTable dt)
    {
        //自增列（使用SQL并指定自增列值写入）
        var autoIncrCol = dt.Columns.Cast<DataColumn>().Where(x => x.AutoIncrement == true).ToList();
        if (autoIncrCol.Count > 0)
        {
            return await BulkBatchPostgreSQL(dt, dataAdapter: dataAdapter =>
            {
                var sqlquote = "\"";
                var ct = dataAdapter.InsertCommand.CommandText;
                autoIncrCol = autoIncrCol.Where(x => !ct.Contains($"{sqlquote + x.ColumnName + sqlquote}")).ToList();
                if (autoIncrCol.Count > 0)
                {
                    var fields = string.Empty;
                    var values = string.Empty;

                    for (int i = 0; i < autoIncrCol.Count; i++)
                    {
                        var col = autoIncrCol[i];
                        fields += $"{sqlquote + col.ColumnName + sqlquote}, ";
                        values += $"@{col.ColumnName}, ";

                        //新增参数
                        var parameter = new NpgsqlParameter(col.ColumnName, col.DataType)
                        {
                            SourceColumn = col.ColumnName
                        };
                        dataAdapter.InsertCommand.Parameters.Add(parameter);
                    }

                    ct = ct.Replace("(\"", $"({fields}\"").Replace("(@", $"({values}@");
                    dataAdapter.InsertCommand.CommandText = ct;
                }
            });
        }
        else
        {
            return await BulkCopyPostgreSQL(dt);
        }
    }
}

#endif