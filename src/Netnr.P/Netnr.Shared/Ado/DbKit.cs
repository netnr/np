#if Full || Ado || AdoAll

namespace Netnr;

/// <summary>
/// Database
/// </summary>
public partial class DbKit
{
    /// <summary>
    /// 构造（DbConnection 对象不能为空）
    /// </summary>
    public DbKit(DbKitConnectionOption connOption)
    {
        if (connOption.Connection == null)
        {
            throw new Exception($"{nameof(connOption.Connection)} 对象不能为空");
        }
        ConnOption = connOption;
    }

    /// <summary>
    /// 配置
    /// </summary>
    public DbKitConnectionOption ConnOption { get; set; }

    /// <summary>
    /// 执行列表
    /// </summary>
    public List<DbKitCommandOption> CommandList { get; set; } = new();

    /// <summary>
    /// 执行终止
    /// </summary>
    /// <param name="Id">可选，默认当前</param>
    /// <returns></returns>
    public bool CommandAbort(string Id = null)
    {
        var isAbort = false;

        if (string.IsNullOrWhiteSpace(Id))
        {
            Id = Activity.Current?.Id;
        }

        for (int i = CommandList.Count - 1; i >= 0; i--)
        {
            var cmdModel = CommandList[i];
            if (cmdModel.Id.StartsWith(Id))
            {
                cmdModel.Command.Cancel();
                CommandList.RemoveAt(i);

                isAbort = true;
            }
        }

        return isAbort;
    }

    /// <summary>
    /// 执行删除
    /// </summary>
    /// <param name="cmdOption"></param>
    /// <param name="force">强制删除，默认否，仅删除非事务执行</param>
    /// <returns></returns>
    public bool CommandRemove(DbKitCommandOption cmdOption, bool force = false)
    {
        var isRemove = false;

        if (cmdOption.Command.Transaction == null || force)
        {
            isRemove = CommandList.Remove(cmdOption);
        }

        return isRemove;
    }

    /// <summary>
    /// 执行创建
    /// </summary>
    /// <param name="sql">SQL语句</param>
    /// <param name="parameters">带参</param>
    /// <returns></returns>
    public DbKitCommandOption CommandCreate(string sql = null, DbParameter[] parameters = null)
    {
        var cmd = ConnOption.Connection.CreateCommand();

        // 修复：避免内存泄露
        // ref: https://stackoverflow.com/questions/3699143
        // ref: https://support.oracle.com/knowledge/Oracle%20Database%20Products/1050515_1.html
        if (ConnOption.ConnectionType == EnumTo.TypeDB.Oracle)
        {
            var gtCmd = cmd.GetType();

            var gpLob = gtCmd.GetProperty("InitialLOBFetchSize");
            gpLob.SetValue(cmd, -1);

            var gpLong = gtCmd.GetProperty("InitialLONGFetchSize");
            gpLong.SetValue(cmd, -1);
        }

        //参数
        cmd.CommandTimeout = ConnOption.Timeout;
        cmd.CommandType = CommandType.Text;
        if (!string.IsNullOrWhiteSpace(sql))
        {
            cmd.CommandText = sql;
        }

        if (parameters != null)
        {
            cmd.Parameters.AddRange(parameters);
        }

        //记录添加
        var cmdOption = new DbKitCommandOption
        {
            Id = Activity.Current?.Id ?? Guid.NewGuid().ToString("N"),
            Command = cmd
        };
        CommandList.Add(cmdOption);

        return cmdOption;
    }

    /// <summary>
    /// 读取 首行首列
    /// </summary>
    /// <param name="sql">SQL语句</param>
    /// <param name="parameters">带参</param>
    /// <param name="cmdCall">回调</param>
    /// <returns></returns>
    public async Task<object> SqlExecuteScalar(string sql, DbParameter[] parameters = null, Func<DbKitCommandOption, Task> cmdCall = null)
    {
        return await SafeConn(async () =>
        {
            var cmdOption = CommandCreate(sql, parameters);
            if (cmdCall != null)
            {
                await cmdCall.Invoke(cmdOption);
            }

            var obj = await cmdOption.Command.ExecuteScalarAsync();

            if (cmdOption.AutoCommit)
            {
                await cmdOption.CommitAsync();
            }
            CommandRemove(cmdOption);

            return obj;
        });
    }

    /// <summary>
    /// 执行 返回受影响行数
    /// </summary>
    /// <param name="sql">SQL语句</param>
    /// <param name="parameters">带参</param>
    /// <param name="cmdCall">回调</param>
    /// <returns></returns>
    public async Task<int> SqlExecuteNonQuery(string sql, DbParameter[] parameters = null, Func<DbKitCommandOption, Task> cmdCall = null)
    {
        return await SafeConn(async () =>
        {
            var cmdOption = CommandCreate(sql, parameters);
            if (cmdCall != null)
            {
                await cmdCall.Invoke(cmdOption);
            }

            var num = await cmdOption.Command.ExecuteNonQueryAsync();

            if (cmdOption.AutoCommit)
            {
                await cmdOption.CommitAsync();
            }
            CommandRemove(cmdOption);

            return num;
        });
    }

    /// <summary>
    /// 执行大量脚本（分批，默认开启事务）
    /// </summary>
    /// <param name="listSql">SQL语句</param>
    /// <param name="sqlBatchSize">脚本分批大小，单位：字节（byte），默认：1024 * 100 = 100KB</param>
    /// <param name="cmdCall">回调</param>
    /// <returns>返回受影响行数</returns>
    public async Task<int> SqlExecuteNonQuery(IEnumerable<string> listSql, int sqlBatchSize = 1024 * 100, Func<DbKitCommandOption, Task> cmdCall = null)
    {
        return await SafeConn(async () =>
        {
            //拆分
            var listBatchSql = new List<string>();
            StringBuilder sbsql = new();
            var currSqlSize = 0;
            foreach (var sql in listSql)
            {
                currSqlSize += Encoding.Default.GetBytes(sql).Length;
                sbsql.AppendLine(sql.TrimEnd(';') + ";");
                if (currSqlSize > sqlBatchSize)
                {
                    listBatchSql.Add(sbsql.ToString());
                    sbsql.Clear();
                    currSqlSize = 0;
                }
            }
            if (currSqlSize != 0)
            {
                listBatchSql.Add(sbsql.ToString());
            }

            //执行
            var cmdOption = CommandCreate();
            //事务
            await cmdOption.OpenTransactionAsync();
            if (cmdCall != null)
            {
                await cmdCall.Invoke(cmdOption);
            }

            //分批执行
            var num = 0;
            foreach (var bs in listBatchSql)
            {
                cmdOption.Command.CommandText = bs;
                num += await cmdOption.Command.ExecuteNonQueryAsync();
            }

            if (cmdOption.AutoCommit)
            {
                await cmdOption.CommitAsync();
            }
            CommandRemove(cmdOption);

            return num;
        });
    }

    /// <summary>
    /// 执行读取（查询、新增、修改、删除等）
    /// </summary>
    /// <param name="sql">SQL语句，支持多条</param>
    /// <param name="parameters">带参</param>
    /// <param name="cmdCall">回调</param>
    /// <returns>返回 表数据、受影响行数、表结构</returns>
    public async Task<DbKitDataSetResult> SqlExecuteDataSet(string sql, DbParameter[] parameters = null, Func<DbKitCommandOption, Task> cmdCall = null)
    {
        return await SafeConn(async () =>
        {
            var model = new DbKitDataSetResult();

            if (ConnOption.ConnectionType == EnumTo.TypeDB.Oracle && !DbKitExtensions.SqlParserBeginEnd(sql))
            {
                model.Datas = new DataSet();
                model.Schemas = new DataSet();

                var cmdOption = CommandCreate();

                var listSql = sql.Split(';').ToList();
                foreach (var txt in listSql)
                {
                    if (!string.IsNullOrWhiteSpace(txt))
                    {
                        cmdOption.Command.CommandText = txt;
                        if (cmdCall != null)
                        {
                            await cmdCall.Invoke(cmdOption);
                        }

                        //单条结果
                        var dsResult = await cmdOption.Command.ReaderDataSetAsync(tableLoad: false);

                        //合并单条结果
                        while (dsResult.Datas.Tables.Count > 0)
                        {
                            var dt = dsResult.Datas.Tables[0];
                            dt.TableName = $"table{model.Datas.Tables.Count + 1}";

                            dsResult.Datas.Tables.RemoveAt(0);
                            model.Datas.Tables.Add(dt);
                        }

                        if (dsResult.RecordsAffected != -1)
                        {
                            if (model.RecordsAffected == -1)
                            {
                                model.RecordsAffected = dsResult.RecordsAffected;
                            }
                            else
                            {
                                model.RecordsAffected += dsResult.RecordsAffected;
                            }
                        }

                        while (dsResult.Schemas.Tables.Count > 0)
                        {
                            var dt = dsResult.Schemas.Tables[0];
                            dt.TableName = $"table{dsResult.Schemas.Tables.Count + 1}";

                            dsResult.Schemas.Tables.RemoveAt(0);
                            model.Schemas.Tables.Add(dt);
                        }
                    }
                }

                if (cmdOption.AutoCommit)
                {
                    await cmdOption.CommitAsync();
                }
                CommandRemove(cmdOption);
            }
            else
            {
                var cmdOption = CommandCreate(sql, parameters);
                if (cmdCall != null)
                {
                    await cmdCall.Invoke(cmdOption);
                }

                model = await cmdOption.Command.ReaderDataSetAsync(tableLoad: false);

                if (cmdOption.AutoCommit)
                {
                    await cmdOption.CommitAsync();
                }
                CommandRemove(cmdOption);
            }

            return model;
        });
    }

    /// <summary>
    /// 执行读取（查询、新增、修改、删除等）
    /// </summary>
    /// <param name="sql">SQL语句，支持多条</param>
    /// <param name="parameters">带参</param>
    /// <param name="cmdCall">回调</param>
    /// <returns>返回 表数据、受影响行数、表结构</returns>
    public async Task<DbKitDataOnlyResult> SqlExecuteDataOnly(string sql, DbParameter[] parameters = null, Func<DbKitCommandOption, Task> cmdCall = null)
    {
        return await SafeConn(async () =>
        {
            var cmdOption = CommandCreate(sql, parameters);
            if (cmdCall != null)
            {
                await cmdCall.Invoke(cmdOption);
            }

            var model = await cmdOption.Command.ReaderDataOnlyAsync();

            if (cmdOption.AutoCommit)
            {
                await cmdOption.CommitAsync();
            }
            CommandRemove(cmdOption);

            return model;
        });
    }

    /// <summary>
    /// 查询 读取行
    /// </summary>
    /// <param name="sql">SQL</param>
    /// <param name="parameters">参数</param>
    /// <param name="cmdCall">回调</param>
    /// <param name="readRow">行数据</param>
    /// <param name="emptyTable">空表</param>
    public async Task SqlExecuteDataRow(string sql, DbParameter[] parameters = null, Func<DbKitCommandOption, Task> cmdCall = null, Func<object[], Task> readRow = null, Func<DataTable, Task> emptyTable = null)
    {
        await SafeConn(async () =>
        {
            var cmdOption = CommandCreate(sql, parameters);
            if (cmdCall != null)
            {
                await cmdCall.Invoke(cmdOption);
            }

            await cmdOption.Command.ReaderDataRowAsync(readRow, emptyTable);

            if (cmdOption.AutoCommit)
            {
                await cmdOption.CommitAsync();
            }
            CommandRemove(cmdOption);

            return true;
        });
    }

    /// <summary>
    /// 连接包装
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="action"></param>
    /// <returns></returns>
    public async Task<T> SafeConn<T>(Func<Task<T>> action)
    {
        try
        {
            if (ConnOption.Connection.State == ConnectionState.Closed)
            {
                await ConnOption.Connection.OpenAsync();
            }

            var result = await action.Invoke();
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
        finally
        {
            if (ConnOption.AutoClose)
            {
                if (ConnOption.Connection.State == ConnectionState.Open)
                {
                    await ConnOption.Connection.CloseAsync();
                }
            }
        }
    }
}

#endif