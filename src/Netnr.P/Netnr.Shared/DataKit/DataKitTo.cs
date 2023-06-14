#if Full || DataKit

using System.Net.Http;
using System.IO.Compression;
using Oracle.ManagedDataAccess.Client;
using Microsoft.Data.SqlClient;
using MySqlConnector;
using Npgsql;

namespace Netnr;

/// <summary>
/// 数据交互
/// </summary>
public partial class DataKitTo
{
    #region 基础

    /// <summary>
    /// 数据库
    /// </summary>
    public DbKit DbInstance { get; set; }

    /// <summary>
    /// 构造
    /// </summary>
    /// <param name="connOption"></param>
    public DataKitTo(DbKitConnectionOption connOption)
    {
        DbInstance = connOption.CreateInstance();
    }

    /// <summary>
    /// 获取库名
    /// </summary>
    /// <returns></returns>
    public async Task<List<string>> GetDatabaseName()
    {
        var result = new List<string>();

        var sql = DataKitScript.GetDatabaseName(DbInstance.ConnOption.ConnectionType);
        var edo = await DbInstance.SqlExecuteDataOnly(sql);

        var dt = edo.Datas.Tables[0];
        foreach (DataRow dr in dt.Rows)
        {
            if (DbInstance.ConnOption.ConnectionType == EnumTo.TypeDB.SQLite)
            {
                result.Add(dr["name"].ToString());
            }
            else
            {
                result.Add(dr[0].ToString());
            }
        }

        return result;
    }

    /// <summary>
    /// 获取库
    /// </summary>
    /// <param name="filterDatabaseName">过滤数据库名，逗号分隔</param>
    /// <returns></returns>
    public async Task<List<DataKitDatabaseResult>> GetDatabase(string filterDatabaseName = null)
    {
        var result = new List<DataKitDatabaseResult>();

        var listDatabaseName = string.IsNullOrWhiteSpace(filterDatabaseName)
            ? null : filterDatabaseName.Replace("'", "").Split(',');

        var sql = DataKitScript.GetDatabase(DbInstance.ConnOption.ConnectionType, listDatabaseName);
        var edo = await DbInstance.SqlExecuteDataOnly(sql);

        if (DbInstance.ConnOption.ConnectionType == EnumTo.TypeDB.SQLite)
        {
            var dt1 = edo.Datas.Tables[0];
            var charset = edo.Datas.Tables[1].Rows[0][0].ToString();
            foreach (DataRow dr in dt1.Rows)
            {
                var name = dr["name"].ToString();
                var file = dr["file"].ToString();
                var fi = new FileInfo(file);

                result.Add(new DataKitDatabaseResult
                {
                    DatabaseName = name,
                    DatabaseCharset = charset,
                    DatabasePath = file,
                    DatabaseDataLength = fi.Length,
                    DatabaseCreateTime = fi.CreationTime
                });
            }
        }
        else
        {
            result = edo.Datas.Tables[0].ToModel<DataKitDatabaseResult>();
        }

        return result;
    }

    /// <summary>
    /// 获取表
    /// </summary>
    /// <param name="schemaName">模式名</param>
    /// <param name="databaseName">数据库名</param>
    /// <returns></returns>
    public async Task<List<DataKitTablResult>> GetTable(string schemaName = null, string databaseName = null)
    {
        var result = new List<DataKitTablResult>();

        databaseName ??= DbInstance.ConnOption.DatabaseName;

        var sql = DataKitScript.GetTable(DbInstance.ConnOption.ConnectionType, databaseName, schemaName);
        var edo = await DbInstance.SqlExecuteDataOnly(sql);

        result = edo.Datas.Tables[0].ToModel<DataKitTablResult>();

        return result;
    }

    /// <summary>
    /// 表DDL
    /// </summary>
    /// <param name="tableName">表名</param>
    /// <param name="schemaName">模式名</param>
    /// <param name="databaseName">数据库名</param>
    /// <returns></returns>
    public async Task<string> GetTableDDL(string tableName, string schemaName = null, string databaseName = null)
    {
        string result = string.Empty;

        databaseName ??= DbInstance.ConnOption.DatabaseName;

        var sql = DataKitScript.GetTableDDL(DbInstance.ConnOption.ConnectionType, databaseName, schemaName, tableName);
        switch (DbInstance.ConnOption.ConnectionType)
        {
            case EnumTo.TypeDB.SQLite:
                {
                    var edo = await DbInstance.SqlExecuteDataOnly(sql);

                    var rows = edo.Datas.Tables[0].Rows;
                    var ddl = new List<string> { $"DROP TABLE IF EXISTS [{rows[0]["tbl_name"]}]" };
                    foreach (DataRow dr in rows)
                    {
                        var script = dr["sql"]?.ToString();
                        if (!string.IsNullOrWhiteSpace(script))
                        {
                            ddl.Add(script);
                        }
                    }

                    result = string.Join(";\r\n", ddl) + ";";
                }
                break;
            case EnumTo.TypeDB.MySQL:
            case EnumTo.TypeDB.MariaDB:
                {
                    var edo = await DbInstance.SqlExecuteDataOnly(sql);

                    var rows = edo.Datas.Tables[0].Rows;
                    var ddl = new List<string> { $"DROP TABLE IF EXISTS `{rows[0][0]}`" };

                    var script = rows[0][1].ToString();
                    ddl.Add(script);

                    result = string.Join(";\r\n", ddl) + ";";
                }
                break;
            case EnumTo.TypeDB.Oracle:
                {
                    //是 SYSDBA 用户，提示换普通用户连接
                    if (DbInstance.ConnOption.ConnectionString.Contains("DBA Privilege=SYSDBA", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new Exception("SYSDBA not support, use normal user");
                    }

                    var edo = await DbInstance.SqlExecuteDataOnly(sql, cmdCall: cmdOption =>
                    {
                        var ocmd = (OracleCommand)cmdOption.Command;

                        //begin ... end;
                        if (DbKitExtensions.SqlParserBeginEnd(sql))
                        {
                            //open:name for
                            var cursors = DbKitExtensions.SqlParserCursors(sql);
                            foreach (var cursor in cursors)
                            {
                                ocmd.Parameters.Add(cursor, OracleDbType.RefCursor, DBNull.Value, ParameterDirection.Output);
                            }
                        }

                        return Task.CompletedTask;
                    });

                    var ddlTable = edo.Datas.Tables[0].Rows[0][0].ToString().Trim();
                    var ddlIndex = edo.Datas.Tables[1].Rows.Cast<DataRow>().Select(x => x[0].ToString().Trim() + ";");
                    var ddlCheck = edo.Datas.Tables[2].Rows.Cast<DataRow>().Select(x => x[0].ToString().Trim() + ";");
                    var ddlTableComment = edo.Datas.Tables[3].Rows[0][0].ToString().Trim();
                    var ddlColumnComment = edo.Datas.Tables[4];

                    var fullTableName = $"\"{databaseName}\".\"{tableName}\"";

                    var ddl = new List<string> { $"DROP TABLE {fullTableName};", $"{ddlTable};" };
                    if (ddlIndex.Any())
                    {
                        ddl.Add("");
                        ddl.AddRange(ddlIndex);
                    }
                    if (ddlCheck.Any())
                    {
                        ddl.Add("");
                        ddl.AddRange(ddlCheck);
                    }
                    ddl.Add("");
                    ddl.Add($"COMMENT ON TABLE {fullTableName} IS '{ddlTableComment.Replace("'", "''")}';");
                    ddl.Add("");
                    foreach (DataRow dr in ddlColumnComment.Rows)
                    {
                        ddl.Add($"COMMENT ON COLUMN {fullTableName}.\"{dr[0]}\" IS '{dr[1].ToString().Trim().Replace("'", "''")}';");
                    }

                    result = string.Join("\r\n", ddl);
                }
                break;
            case EnumTo.TypeDB.SQLServer:
                {
                    var edo = await DbInstance.SqlExecuteDataOnly(sql);

                    result = edo.Datas.Tables[0].Rows[0][1].ToString();
                }
                break;
            case EnumTo.TypeDB.PostgreSQL:
                {
                    //消息
                    var listInfo = new List<string>();
                    var dbConn = (NpgsqlConnection)DbInstance.ConnOption.Connection;
                    dbConn.Notice += (s, e) =>
                    {
                        listInfo.Add(e.Notice.MessageText);
                    };

                    await DbInstance.SqlExecuteDataOnly(sql);

                    result = string.Join("\r\n", listInfo);
                }
                break;
        }

        return result;
    }

    /// <summary>
    /// 获取列
    /// </summary>
    /// <param name="filterSchemaNameTableName">过滤模式表名，逗号分隔</param>
    /// <param name="databaseName">数据库名</param>
    /// <returns></returns>
    public async Task<List<DataKitColumnResult>> GetColumn(string filterSchemaNameTableName = null, string databaseName = null)
    {
        var result = new List<DataKitColumnResult>();

        databaseName ??= DbInstance.ConnOption.DatabaseName;

        var listSchemaNameTableName = new List<ValueTuple<string, string>>();
        if (!string.IsNullOrWhiteSpace(filterSchemaNameTableName))
        {
            filterSchemaNameTableName.Replace("'", "").Split(',').ToList().ForEach(item =>
            {
                var schemaName = string.Empty;
                var tableName = string.Empty;
                var listST = item.Split(".");
                if (listST.Length == 2)
                {
                    schemaName = listST[0];
                    tableName = listST[1];
                }
                else
                {
                    tableName = listST[0];
                }

                listSchemaNameTableName.Add(new ValueTuple<string, string>(schemaName, tableName));
            });
        }

        var sql = DataKitScript.GetColumn(DbInstance.ConnOption.ConnectionType, databaseName, listSchemaNameTableName);
        var edo = await DbInstance.SqlExecuteDataOnly(sql);

        if (DbInstance.ConnOption.ConnectionType == EnumTo.TypeDB.SQLite)
        {
            edo.Datas.Tables[0].Rows.RemoveAt(0);
            var ds2 = edo.Datas.Tables[1].Select();

            var aakey = "AUTOINCREMENT";
            foreach (DataRow dr in edo.Datas.Tables[0].Rows)
            {
                var csql = ds2.FirstOrDefault(x => x["name"].ToString().Equals(dr["TableName"].ToString(), StringComparison.OrdinalIgnoreCase))[1].ToString().ToUpper();
                if (csql.Contains(aakey))
                {
                    var isaa = csql.Split(',').Any(x => x.Contains(aakey) && x.Contains(dr["ColumnName"].ToString().ToUpper()));
                    if (isaa)
                    {
                        dr["AutoIncr"] = 1;
                    }
                }
            }
        }

        result = edo.Datas.Tables[0].ToModel<DataKitColumnResult>();

        return result;
    }

    /// <summary>
    /// 设置表注释
    /// </summary>
    /// <param name="tableName">表名</param>
    /// <param name="tableComment">表注释</param>
    /// <param name="schemaName">模式名</param>
    /// <param name="databaseName">数据库名</param>
    /// <returns></returns>
    public async Task<bool> SetTableComment(string tableName, string tableComment, string schemaName = null, string databaseName = null)
    {
        if (DbInstance.ConnOption.ConnectionType != EnumTo.TypeDB.SQLite)
        {
            databaseName ??= DbInstance.ConnOption.DatabaseName;

            var sql = DataKitScript.SetTableComment(DbInstance.ConnOption.ConnectionType, databaseName, schemaName, tableName, tableComment);
            await DbInstance.SqlExecuteNonQuery(sql);
        }

        return true;
    }

    /// <summary>
    /// 设置列注释
    /// </summary>
    /// <param name="tableName">表名</param>
    /// <param name="columnName">列名</param>
    /// <param name="columnComment">列注释</param>
    /// <param name="schemaName">模式名</param>
    /// <param name="databaseName">数据库名</param>
    /// <returns></returns>
    public async Task<bool> SetColumnComment(string tableName, string columnName, string columnComment, string schemaName = null, string databaseName = null)
    {
        if (DbInstance.ConnOption.ConnectionType != EnumTo.TypeDB.SQLite)
        {
            databaseName ??= DbInstance.ConnOption.DatabaseName;

            var sql = DataKitScript.SetColumnComment(DbInstance.ConnOption.ConnectionType, databaseName, schemaName, tableName, columnName, columnComment);
            await DbInstance.SqlExecuteNonQuery(sql);
        }

        return true;
    }

    /// <summary>
    /// 执行脚本
    /// </summary>
    /// <param name="sql">脚本</param>
    /// <returns></returns>
    public async Task<ValueTuple<DataSet, DataSet, object>> ExecuteSql(string sql)
    {
        var sw = Stopwatch.StartNew();

        //开启事务
        var openTransaction = true;
        var sqlUpper = sql.ToUpper();
        var listKey = "DROP DATABASE,ALTER DATABASE,CREATE DATABASE".Split(',');
        var listTypeDB = EnumTo.TypeDB.PostgreSQL | EnumTo.TypeDB.SQLServer;
        if (listTypeDB.HasFlag(DbInstance.ConnOption.ConnectionType) && listKey.Any(sqlUpper.Contains))
        {
            openTransaction = false;
        }
        if (DbInstance.ConnOption.ConnectionType == EnumTo.TypeDB.SQLite && sqlUpper == "VACUUM")
        {
            openTransaction = false;
        }

        //消息
        var listInfo = new List<string>();

        var eds = await DbInstance.SqlExecuteDataSet(sql, cmdCall: async cmdOption =>
        {
            if (openTransaction)
            {
                await cmdOption.OpenTransactionAsync();
            }

            switch (DbInstance.ConnOption.ConnectionType)
            {
                case EnumTo.TypeDB.MySQL:
                case EnumTo.TypeDB.MariaDB:
                    {
                        var dbConn = (MySqlConnection)cmdOption.Command.Connection;
                        dbConn.InfoMessage += (s, e) =>
                        {
                            listInfo.Add(e.Errors[0].Message);
                        };
                    }
                    break;
                case EnumTo.TypeDB.Oracle:
                    {
                        var dbCmd = (OracleCommand)cmdOption.Command;
                        var dbConn = dbCmd.Connection;
                        dbConn.InfoMessage += (s, e) =>
                        {
                            listInfo.Add(e.Message);
                        };

                        //begin ... end;
                        if (DbKitExtensions.SqlParserBeginEnd(sql))
                        {
                            //open:name for
                            var cursors = DbKitExtensions.SqlParserCursors(sql);
                            foreach (var cursor in cursors)
                            {
                                dbCmd.Parameters.Add(cursor, OracleDbType.RefCursor, DBNull.Value, ParameterDirection.Output);
                            }
                        }
                    }
                    break;
                case EnumTo.TypeDB.SQLServer:
                    {
                        var dbConn = (SqlConnection)cmdOption.Command.Connection;
                        dbConn.InfoMessage += (s, e) =>
                        {
                            listInfo.Add(e.Message);
                        };
                    }
                    break;
                case EnumTo.TypeDB.PostgreSQL:
                    {
                        var dbConn = (NpgsqlConnection)cmdOption.Command.Connection;
                        dbConn.Notice += (s, e) =>
                        {
                            listInfo.Add(e.Notice.MessageText);
                        };
                    }
                    break;
            }
        });

        listInfo.Add($"耗时: {sw.Elapsed}");

        if (eds.RecordsAffected != -1)
        {
            listInfo.Insert(0, $"({eds.RecordsAffected} 行受影响)");
        }

        var dtInfo = new DataTable();
        dtInfo.Columns.Add(new DataColumn("message"));
        listInfo.ForEach(info =>
        {
            var drInfo = dtInfo.NewRow();
            drInfo[0] = info;
            dtInfo.Rows.Add(drInfo.ItemArray);
        });

        return new ValueTuple<DataSet, DataSet, object>(eds.Datas, eds.Schemas, new { info = dtInfo });
    }

    #endregion

    #region 方法

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="connOption"></param>
    /// <returns></returns>
    public static DataKitTo CreateDkInstance(DbKitConnectionOption connOption)
    {
        //额外处理 SQLite
        if (connOption.ConnectionType == EnumTo.TypeDB.SQLite && !string.IsNullOrWhiteSpace(connOption.ConnectionString) && connOption.ConnectionString.Length > 12)
        {
            //下载 SQLite 文件
            var ds = connOption.ConnectionString[12..].TrimEnd(';');
            //路径
            var dspath = Path.GetTempPath();
            //文件名
            var dsname = Path.GetFileName(ds);
            var fullPath = Path.Combine(dspath, dsname);

            //网络路径
            if (ds.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                //不存在则下载
                if (!File.Exists(fullPath))
                {
                    //下载
                    var maxLength = 1024 * 1024 * 10; //最大下载

                    Task.Run(async () =>
                    {
                        var client = new HttpClient
                        {
                            Timeout = TimeSpan.FromMinutes(1)
                        };
                        await client.DownloadAsync(ds, fullPath, (rlen, total) =>
                        {
                            if (total > maxLength || rlen > maxLength)
                            {
                                throw new Exception($"{ds} Size exceeds limit(max {ParsingTo.FormatByteSize(maxLength)})");
                            }
                        });
                    }).GetAwaiter().GetResult();
                }

                connOption.ConnectionString = "Data Source=" + fullPath;
            }
            else
            {
                connOption.ConnectionString = "Data Source=" + ds;
            }
        }

        var dk = new DataKitTo(connOption);
        return dk;
    }

    /// <summary>
    /// 分批最大行数
    /// </summary>
    public static int BatchMaxRows { get; set; } = 10000;

    /// <summary>
    /// 转换，导出数据库对象转换为导出数据表对象
    /// </summary>
    /// <param name="edb"></param>
    /// <returns></returns>
    public static async Task<DataKitTransfer.ExportDataTable> ConvertTransferVM(DataKitTransfer.ExportDatabase edb)
    {
        if (edb.ListReadTableName.Count == 0)
        {
            var dk = CreateDkInstance(edb.ReadConnectionInfo);
            var listTable = await dk.GetTable();
            edb.ListReadTableName = listTable.Select(x => DbKitExtensions.SqlSNTN(x.TableName, x.SchemaName, edb.ReadConnectionInfo.ConnectionType)).ToList();
        }

        var edt = new DataKitTransfer.ExportDataTable().ToDeepCopy(edb);
        edt.ListReadDataSQL = new List<string>();

        foreach (var table in edb.ListReadTableName)
        {
            var sql = $"SELECT * FROM {table}";
            edt.ListReadDataSQL.Add(sql);
        }

        return edt;
    }

    /// <summary>
    /// 导出数据库
    /// </summary>
    /// <param name="edb"></param>
    /// <param name="le">日志事件</param>
    /// <returns></returns>
    public static async Task<ResultVM> ExportDatabase(DataKitTransfer.ExportDatabase edb, Action<NotifyCollectionChangedEventArgs> le = null)
    {
        var edt = await ConvertTransferVM(edb);
        return await ExportDataTable(edt, le);
    }

    /// <summary>
    /// 导出表
    /// </summary>
    /// <param name="edt"></param>
    /// <param name="le">日志事件</param>
    /// <returns></returns>
    public static async Task<ResultVM> ExportDataTable(DataKitTransfer.ExportDataTable edt, Action<NotifyCollectionChangedEventArgs> le = null)
    {
        var vm = new ResultVM();
        vm.LogEvent(le);

        vm.Log.Add($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} 导出数据");

        //数据库
        edt.ReadConnectionInfo.SetConnDatabaseName(edt.ReadDatabaseName);
        edt.ReadConnectionInfo.AutoClose = false;

        var dk = CreateDkInstance(edt.ReadConnectionInfo);

        //打包
        var zipFolder = Path.GetDirectoryName(edt.PackagePath);
        if (!Directory.Exists(zipFolder))
        {
            Directory.CreateDirectory(zipFolder);
        }
        var isOldZip = File.Exists(edt.PackagePath);
        using ZipArchive zip = ZipFile.Open(edt.PackagePath, isOldZip ? ZipArchiveMode.Update : ZipArchiveMode.Create);

        for (int i = 0; i < edt.ListReadDataSQL.Count; i++)
        {
            var sql = edt.ListReadDataSQL[i];

            vm.Log.Add($"读取表：{sql}");

            var rowCount = 0;
            var batchNo = 0;

            var dt = new DataTable();
            //模式名.表名
            var sntn = string.Empty;

            var sw = new Stopwatch();
            sw.Start();

            //读取行
            await dk.DbInstance.SqlExecuteDataRow(sql, readRow: row =>
            {
                rowCount++;

                dt.Rows.Add(row);

                if (sw.Elapsed.TotalMilliseconds > 5000)
                {
                    vm.Log.Add($"当前读取行：{dt.Rows.Count}/{rowCount}");
                    sw.Restart();
                }

                if (dt.Rows.Count >= BatchMaxRows)
                {
                    batchNo++;

                    var xmlName = $"{sntn}_{batchNo.ToString().PadLeft(7, '0')}.xml";

                    //xml 写入 zip
                    var zae = isOldZip
                    ? zip.GetEntry(xmlName) ?? zip.CreateEntry(xmlName)
                    : zip.CreateEntry(xmlName);

                    //覆盖写入
                    var ceStream = zae.Open();
                    if (ceStream.CanSeek)
                    {
                        ceStream.SetLength(0);
                        ceStream.Seek(0, SeekOrigin.Begin);
                    }
                    dt.WriteXml(ceStream, XmlWriteMode.WriteSchema);
                    ceStream.Close();

                    vm.Log.Add($"导出表（{dt.TableName}）第 {batchNo} 批（行：{dt.Rows.Count}/{rowCount}），耗时：{vm.PartTimeFormat()}");
                    sw.Restart();

                    dt.Clear();
                }

                return Task.CompletedTask;
            }, emptyTable: async etable =>
            {
                dt = etable.Clone();
                dt.MinimumCapacity = BatchMaxRows;
                sntn = DbKitExtensions.SqlSNTN(dt.TableName, dt.Namespace);

                //表结构 DDL
                if (edt.ExportType == "all")
                {
                    var tableDesc = "-- --------------------";
                    var ddl = await dk.GetTableDDL(dt.TableName, dt.Namespace);
                    var bytes = string.Join("\r\n", new string[] { tableDesc, $"-- {sntn}", tableDesc, ddl }).ToByte();

                    var sqlName = $"{sntn}.sql";

                    //sql 写入 zip
                    var zae = isOldZip
                    ? zip.GetEntry(sqlName) ?? zip.CreateEntry(sqlName)
                    : zip.CreateEntry(sqlName);

                    //覆盖写入
                    var ceStream = zae.Open();
                    if (ceStream.CanSeek)
                    {
                        ceStream.SetLength(0);
                        ceStream.Seek(0, SeekOrigin.Begin);
                    }
                    await ceStream.WriteAsync(bytes);
                    ceStream.Close();

                    vm.Log.Add($"导出表（{sntn}）DDL结构，耗时：{vm.PartTimeFormat()}");
                }
            });

            //手动关闭连接
            await dk.DbInstance.ConnOption.Close();

            //最后一批
            if (dt.Rows.Count > 0)
            {
                batchNo++;

                var xmlName = $"{sntn}_{batchNo.ToString().PadLeft(7, '0')}.xml";

                //xml 写入 zip
                var zae = isOldZip
                ? zip.GetEntry(xmlName) ?? zip.CreateEntry(xmlName)
                : zip.CreateEntry(xmlName);

                //写入
                var ceStream = zae.Open();
                if (ceStream.CanSeek)
                {
                    ceStream.SetLength(0);
                    ceStream.Seek(0, SeekOrigin.Begin);
                }
                dt.WriteXml(ceStream, XmlWriteMode.WriteSchema);
                ceStream.Close();

                vm.Log.Add($"导出表（{sntn}）第 {batchNo} 批（行：{dt.Rows.Count}/{rowCount}），耗时：{vm.PartTimeFormat()}");
            }

            vm.Log.Add($"导出表（{sntn}）完成（行：{rowCount}），导表进度：{i + 1}/{edt.ListReadDataSQL.Count}");

            //清理包历史分片
            if (isOldZip)
            {
                var hasOldData = false;
                do
                {
                    var xmlName = $"{sntn}_{(++batchNo).ToString().PadLeft(7, '0')}.xml";
                    var zae = zip.GetEntry(xmlName);
                    if (hasOldData = (zae != null))
                    {
                        zae.Delete();
                    }
                } while (hasOldData);
            }
        }

        vm.Log.Add($"导出完成：{edt.PackagePath}，共耗时：{vm.UseTimeFormat}");
        GC.Collect();

        vm.Set(EnumTo.RTag.success);
        return vm;
    }

    /// <summary>
    /// 迁移数据表
    /// </summary>
    /// <param name="mdt"></param>
    /// <param name="le">实时日志</param>
    /// <returns></returns>
    public static async Task<ResultVM> MigrateDataTable(DataKitTransfer.MigrateDataTable mdt, Action<NotifyCollectionChangedEventArgs> le = null)
    {
        var vm = new ResultVM();
        vm.LogEvent(le);

        vm.Log.Add($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} 迁移表数据");

        //读取数据库
        var dbRead = mdt.ReadConnectionInfo.CreateInstance();
        //写入数据库
        var dbWrite = mdt.WriteConnectionInfo.CreateInstance();
        //预检通过
        var isCopy = await dbWrite.PreExecute() != -1;

        //遍历
        for (int i = 0; i < mdt.ListReadWrite.Count; i++)
        {
            var rw = mdt.ListReadWrite[i];

            vm.Log.Add($"获取写入表（{rw.WriteTableName}）结构");
            var edoWrite = await dbWrite.SqlExecuteDataSet(DbKitExtensions.SqlEmpty(rw.WriteTableName, tdb: mdt.WriteConnectionInfo.ConnectionType));
            var dtWrite = edoWrite.Datas.Tables[0];
            dtWrite.TableName = rw.WriteTableName.Split('.').Last();
            var dtWriteColumnName = dtWrite.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToList();

            //读取表的列 => 写入表的列
            var rwMap = new Dictionary<string, string>();

            vm.Log.Add($"获取读取表数据 SQL：{rw.ReadDataSQL}");

            var rowCount = 0;
            var batchNo = 0;
            var catchCount = 0;

            //读取行
            await dbRead.SqlExecuteDataRow(rw.ReadDataSQL, readRow: async row =>
            {
                rowCount++;

                //构建一行 写入表
                var drWriteNew = dtWrite.NewRow();
                //根据读取表列映射写入表列填充单元格数据
                var columnIndex = 0;
                foreach (var columnNameRead in rwMap.Keys)
                {
                    //读取表列值
                    var valueRead = row[columnIndex++];
                    if (valueRead is not DBNull)
                    {
                        //读取表列值 转换类型为 写入表列值
                        try
                        {
                            //写入表列
                            var columnNameWrite = rwMap[columnNameRead];
                            //写入表列类型
                            var typeWrite = dtWrite.Columns[columnNameWrite].DataType;

                            //读取表列值 赋予 写入表列
                            drWriteNew[columnNameWrite] = Convert.ChangeType(valueRead, typeWrite);
                        }
                        catch (Exception ex)
                        {
                            catchCount++;
                            vm.Log.Add($"列值转换失败：");
                            vm.Log.Add(ex.ToJson());
                        }
                    }
                }
                //添加新行
                dtWrite.Rows.Add(drWriteNew.ItemArray);

                if (dtWrite.Rows.Count >= BatchMaxRows)
                {
                    batchNo++;

                    if (batchNo == 1)
                    {
                        //第一批写入前清理表
                        if (!string.IsNullOrWhiteSpace(rw.WriteDeleteSQL))
                        {
                            vm.Log.Add($"清理写入表：{rw.WriteDeleteSQL}");
                            var num = await dbWrite.SqlExecuteNonQuery(rw.WriteDeleteSQL);
                            vm.Log.Add($"返回受影响行数：{num}，耗时：{vm.PartTimeFormat()}");
                        }
                    }

                    //分批写入
                    vm.Log.Add($"写入表（{rw.WriteTableName}）第 {batchNo} 批（行：{dtWrite.Rows.Count}/{rowCount}）");

                    await dbWrite.BulkCopy(dtWrite, isCopy);

                    //清理
                    dtWrite.Clear();
                }
            }, emptyTable: etable =>
            {
                //构建列映射
                foreach (DataColumn dcRead in etable.Columns)
                {
                    //指定映射
                    if (rw.ReadWriteColumnMap.TryGetValue(dcRead.ColumnName, out string value))
                    {
                        rwMap.Add(dcRead.ColumnName, value);
                    }
                    else
                    {
                        //自动映射
                        var columnNameWrite = dtWriteColumnName.FirstOrDefault(x => x.Equals(dcRead.ColumnName, StringComparison.OrdinalIgnoreCase));
                        if (columnNameWrite != null)
                        {
                            rwMap.Add(dcRead.ColumnName, columnNameWrite);
                        }
                    }
                }

                return Task.CompletedTask;
            });

            //最后一批
            if (dtWrite.Rows.Count > 0)
            {
                batchNo++;

                if (batchNo == 1)
                {
                    //第一批写入前清理表
                    if (!string.IsNullOrWhiteSpace(rw.WriteDeleteSQL))
                    {
                        vm.Log.Add($"清理写入表：{rw.WriteDeleteSQL}");
                        var num = await dbWrite.SqlExecuteNonQuery(rw.WriteDeleteSQL);
                        vm.Log.Add($"返回受影响行数：{num}，耗时：{vm.PartTimeFormat()}");
                    }
                }

                //分批写入
                vm.Log.Add($"写入表（{rw.WriteTableName}）第 {batchNo} 批（行：{dtWrite.Rows.Count}/{rowCount}）");

                await dbWrite.BulkCopy(dtWrite, isCopy);

                //清理
                dtWrite.Clear();
            }

            if (catchCount > 0)
            {
                vm.Log.Add($"列值转换失败：{catchCount} 次");
            }
            vm.Log.Add($"写入表（{rw.WriteTableName}）完成，耗时：{vm.PartTimeFormat()}，写表进度：{i + 1}/{mdt.ListReadWrite.Count}");
        }

        vm.Log.Add($"总共耗时：{vm.UseTimeFormat}");
        GC.Collect();

        vm.Set(EnumTo.RTag.success);
        return vm;
    }

    /// <summary>
    /// 导入数据
    /// </summary>
    /// <param name="idb"></param>
    /// <param name="le">日志事件</param>
    /// <returns></returns>
    public static async Task<ResultVM> ImportDatabase(DataKitTransfer.ImportDatabase idb, Action<NotifyCollectionChangedEventArgs> le = null)
    {
        var vm = new ResultVM();
        vm.LogEvent(le);

        vm.Log.Add($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} 导入数据");
        vm.Log.Add($"读取数据源：{idb.PackagePath}\r\n");

        var dbk = idb.WriteConnectionInfo.CreateInstance();
        var isCopy = await dbk.PreExecute() != -1; //预检通过

        using var zipRead = ZipFile.OpenRead(idb.PackagePath);
        //表名分组
        var tableGroups = zipRead.Entries.GroupBy(x =>
        {
            if (x.Name.EndsWith(".sql"))
            {
                return Path.GetFileNameWithoutExtension(x.Name);
            }
            else
            {
                return x.Name[..x.Name.LastIndexOf('_')];
            }
        });

        var hsName = new HashSet<string>(); // 已导入的表记录

        //写入库表信息
        var writeTables = new List<DataKitTablResult>();
        if (!zipRead.Entries.Any(x => x.Name.EndsWith(".sql")))
        {
            vm.Log.Add($"读取写入库表信息");
            var dk = CreateDkInstance(idb.WriteConnectionInfo);
            writeTables = await dk.GetTable();
        }

        var tableIndex = 0;
        foreach (var tableGroup in tableGroups)
        {
            tableIndex++;
            vm.Log.Add($"读取表（{tableGroup.Key}），进度：{tableIndex}/{tableGroups.Count()}");

            var sqlFile = tableGroup.FirstOrDefault(x => x.Name.EndsWith(".sql"));

            //仅数据时，支持表、列映射配置，导入前清空表数据，不同类型数据库（主要用于数据迁移）
            //含 SQL 文件时，视为同类型数据库，重建表，直接写入数据（主要用于备份还原）
            var dataOnly = sqlFile == null;
            if (!dataOnly)
            {
                var sqlContent = sqlFile.Open().ToText();

                vm.Log.Add($"创建表结构，执行脚本：{sqlFile.Name}");
                await dbk.SqlExecuteNonQuery(sqlContent);
            }

            //当前表分片
            foreach (var item in tableGroup)
            {
                if (item.Name.EndsWith(".sql"))
                {
                    continue;
                }

                //读取分片为数据表
                var dt = new DataTable();
                dt.ReadXml(item.Open());

                var sntn = tableGroup.Key;

                //有数据
                if (dt.Rows.Count > 0)
                {
                    //表列映射，是否先清空表数据等判断
                    if (dataOnly)
                    {
                        //写入表名映射
                        var writeTableMap = string.Empty;
                        foreach (var key in idb.ReadWriteTableMap.Keys)
                        {
                            var val = idb.ReadWriteTableMap[key];
                            var sntnArray = key.Split('.');
                            if (sntnArray.Length == 2)
                            {
                                if (dt.Namespace == sntnArray[0] && dt.TableName == sntnArray[1])
                                {
                                    writeTableMap = val;
                                    break;
                                }
                            }
                            else if (dt.TableName == key)
                            {
                                writeTableMap = val;
                                break;
                            }
                        }

                        //指定映射 且在 写入库
                        if (!string.IsNullOrWhiteSpace(writeTableMap) && writeTables.Any(x => DbKitExtensions.SqlEqualSNTN(writeTableMap, x.TableName, x.SchemaName)))
                        {
                            var sntnArray = writeTableMap.Split('.');
                            if (sntnArray.Length == 2)
                            {
                                dt.Namespace = sntnArray[0];
                                dt.TableName = sntnArray[1];
                            }
                            else
                            {
                                dt.Namespace = "";
                                dt.TableName = sntnArray[0];
                            }
                        }
                        //有模式名 但不在 写入库 清除模式名
                        else if (!string.IsNullOrWhiteSpace(dt.Namespace) && !writeTables.Any(x => x.SchemaName == dt.Namespace))
                        {
                            dt.Namespace = "";
                        }

                        if (!writeTables.Any(x => x.TableName == dt.TableName))
                        {
                            vm.Log.Add($"写入库未找到表（{dt.TableName}），已跳过分片：{item.Name}");
                            continue;
                        }

                        //模式名.表名
                        sntn = DbKitExtensions.SqlSNTN(dt.TableName, dt.Namespace);

                        //清空表
                        if (hsName.Add(sntn) && idb.WriteDeleteData)
                        {
                            var clearTableSql = DbKitExtensions.SqlClearTable(idb.WriteConnectionInfo.ConnectionType, sntn);

                            vm.Log.Add($"清理写入表：{clearTableSql}");
                            var num = await dbk.SqlExecuteNonQuery(clearTableSql);
                            vm.Log.Add($"返回受影响行数：{num}，耗时：{vm.PartTimeFormat()}");
                        }
                    }

                    vm.Log.Add($"正在导入（{sntn}）分片：{item.Name}（行：{dt.Rows.Count}，大小：{ParsingTo.FormatByteSize(item.Length)}）");
                    await dbk.BulkCopy(dt, isCopy);
                    vm.Log.Add($"导入表（{sntn}）分片成功，耗时：{vm.PartTimeFormat()}");
                }
                else
                {
                    vm.Log.Add($"跳过空分片：{item.Name}");
                }
            }
            vm.Log.Add($"导入表（{tableGroup.Key}）完成\r\n");
        }

        vm.Log.Add($"导入完成，共耗时：{vm.UseTimeFormat}\r\n");

        vm.Set(EnumTo.RTag.success);
        return vm;
    }

    /// <summary>
    /// 相似匹配
    /// </summary>
    /// <param name="s1"></param>
    /// <param name="s2"></param>
    /// <returns></returns>
    public static bool SimilarMatch(string s1, string s2)
    {
        s1 = s1.Replace("-", "").Replace("_", "").Replace(" ", "").ToLower();
        s2 = s2.Replace("-", "").Replace("_", "").Replace(" ", "").ToLower();
        return s1 == s2;
    }

    #endregion
}

#endif