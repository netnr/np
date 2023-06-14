#if Full || AdoAll || AdoSQLite

namespace Netnr;

/// <summary>
/// SQLite操作类
/// </summary>
public partial class DbKit
{
    /// <summary>
    /// 表批量写入
    /// 根据行数据 RowState 状态新增、修改
    /// </summary>
    /// <param name="dt">数据表</param>
    /// <param name="sqlEmpty">查询空表脚本，默认*，可选列，会影响数据更新的列</param>
    /// <param name="openTransaction">开启事务，默认 True</param>
    /// <returns></returns>
    public async Task<int> BulkBatchSQLite(DataTable dt, string sqlEmpty = null, bool openTransaction = true)
    {
        var sntn = $"[{dt.TableName}]";

        if (string.IsNullOrWhiteSpace(sqlEmpty))
        {
            sqlEmpty = DbKitExtensions.SqlEmpty(sntn);
        }

        //空表（新增、更新的模板）
        var dtEmpty = (await SqlExecuteDataSet(sqlEmpty)).Datas.Tables[0];

        var listCols = dtEmpty.Columns.Cast<DataColumn>().ToList();

        var sqlAdd = $"INSERT INTO {sntn}([{string.Join("], [", listCols)}]) VALUES (@{string.Join(", @", listCols)})";

        var colKey = dtEmpty.PrimaryKey.ToList();
        if (colKey.Count == 0)
        {
            colKey.Add(dtEmpty.Columns[0]);
        }
        var colMod = listCols.Where(x => !colKey.Contains(x));
        var sqlMod = $"UPDATE {sntn} SET {string.Join(", ", colMod.Select(x => $"[{x.ColumnName}] = @{x.ColumnName}"))} WHERE {string.Join(", ", colKey.Select(x => $"[{x.ColumnName}] = @{x.ColumnName}"))}";

        return await SafeConn(async () =>
        {
            var cmdAdd = ConnOption.Connection.CreateCommand();
            cmdAdd.CommandText = sqlAdd;

            var cmdMod = ConnOption.Connection.CreateCommand();
            cmdMod.CommandText = sqlMod;

            var transaction = openTransaction ? await ConnOption.Connection.BeginTransactionAsync() : null;
            if (openTransaction)
            {
                cmdAdd.Transaction = transaction;
                cmdMod.Transaction = transaction;
            }

            var parametersAdd = new List<DbParameter>();
            var parametersMod = new List<DbParameter>();
            listCols.ForEach(col =>
            {
                var parameterAdd = cmdAdd.CreateParameter();
                var parameterMod = cmdMod.CreateParameter();

                parameterAdd.ParameterName = $"@{col.ColumnName}";
                parameterMod.ParameterName = $"@{col.ColumnName}";

                cmdAdd.Parameters.Add(parameterAdd);
                cmdMod.Parameters.Add(parameterMod);

                parametersAdd.Add(parameterAdd);
                parametersMod.Add(parameterMod);
            });

            var num = 0;
            foreach (DataRow dr in dt.Rows)
            {
                if (dr.RowState == DataRowState.Added)
                {
                    for (int i = 0; i < listCols.Count; i++)
                    {
                        var cell = dr[listCols[i].ColumnName];
                        if (cell == DBNull.Value)
                        {
                            parametersAdd[i].Value = DBNull.Value;
                        }
                        else
                        {
                            parametersAdd[i].Value = cell;
                        }
                    }

                    num += await cmdAdd.ExecuteNonQueryAsync();
                }
                else if (dr.RowState == DataRowState.Modified)
                {
                    for (int i = 0; i < listCols.Count; i++)
                    {
                        var cell = dr[listCols[i].ColumnName];
                        if (cell == DBNull.Value)
                        {
                            parametersMod[i].Value = DBNull.Value;
                        }
                        else
                        {
                            parametersMod[i].Value = cell;
                        }
                    }

                    num += await cmdMod.ExecuteNonQueryAsync();
                }
            }

            if (transaction != null)
            {
                await transaction.CommitAsync();
            }

            return num;
        });
    }
}

#endif