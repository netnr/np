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
        DbInstance = connOption.CreateDbInstance();
    }

    /// <summary>
    /// 手动关闭连接
    /// </summary>
    /// <returns></returns>
    public async Task Close()
    {
        var conn = DbInstance.ConnOption.Connection;
        if (conn?.State == ConnectionState.Open)
        {
            await conn.CloseAsync();
        }
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
            if (DbInstance.ConnOption.ConnectionType == DBTypes.SQLite)
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

        if (DbInstance.ConnOption.ConnectionType == DBTypes.SQLite)
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
    public async Task<List<DataKitTableResult>> GetTable(string schemaName = null, string databaseName = null)
    {
        var result = new List<DataKitTableResult>();

        databaseName ??= DbInstance.ConnOption.DatabaseName;

        var sql = DataKitScript.GetTable(DbInstance.ConnOption.ConnectionType, databaseName, schemaName);
        var edo = await DbInstance.SqlExecuteDataOnly(sql);

        result = edo.Datas.Tables[0].ToModel<DataKitTableResult>();

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
            case DBTypes.SQLite:
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
            case DBTypes.MySQL:
            case DBTypes.MariaDB:
                {
                    var edo = await DbInstance.SqlExecuteDataOnly(sql);

                    var rows = edo.Datas.Tables[0].Rows;
                    var ddl = new List<string> { $"DROP TABLE IF EXISTS `{rows[0][0]}`" };

                    var script = rows[0][1].ToString();
                    ddl.Add(script);

                    result = string.Join(";\r\n", ddl) + ";";
                }
                break;
            case DBTypes.Oracle:
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
            case DBTypes.SQLServer:
                {
                    var edo = await DbInstance.SqlExecuteDataOnly(sql);

                    result = edo.Datas.Tables[0].Rows[0][1].ToString();
                }
                break;
            case DBTypes.PostgreSQL:
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
    /// 获取视图
    /// </summary>
    /// <param name="schemaName">模式名</param>
    /// <param name="databaseName">数据库名</param>
    /// <returns></returns>
    public async Task<List<DataKitTableResult>> GetView(string schemaName = null, string databaseName = null)
    {
        var result = new List<DataKitTableResult>();

        databaseName ??= DbInstance.ConnOption.DatabaseName;

        var sql = DataKitScript.GetView(DbInstance.ConnOption.ConnectionType, databaseName, schemaName);
        var edo = await DbInstance.SqlExecuteDataOnly(sql);

        result = edo.Datas.Tables[0].ToModel<DataKitTableResult>();

        return result;
    }

    /// <summary>
    /// 视图DDL
    /// </summary>
    /// <param name="tableName">表名</param>
    /// <param name="schemaName">模式名</param>
    /// <param name="databaseName">数据库名</param>
    /// <returns></returns>
    public async Task<string> GetViewDDL(string tableName, string schemaName = null, string databaseName = null)
    {
        string result = string.Empty;

        databaseName ??= DbInstance.ConnOption.DatabaseName;

        var sql = DataKitScript.GetViewDDL(DbInstance.ConnOption.ConnectionType, databaseName, schemaName, tableName);
        var ddl = await DbInstance.SqlExecuteScalar(sql);
        result = ddl.ToString();

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
            filterSchemaNameTableName.Replace("'", "").Split(',').ForEach(item =>
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

        if (DbInstance.ConnOption.ConnectionType == DBTypes.SQLite)
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
        if (DbInstance.ConnOption.ConnectionType != DBTypes.SQLite)
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
        if (DbInstance.ConnOption.ConnectionType != DBTypes.SQLite)
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
        var listDBTypes = DBTypes.PostgreSQL | DBTypes.SQLServer;
        if (listDBTypes.HasFlag(DbInstance.ConnOption.ConnectionType) && listKey.Any(sqlUpper.Contains))
        {
            openTransaction = false;
        }
        if (DbInstance.ConnOption.ConnectionType == DBTypes.SQLite && sqlUpper == "VACUUM")
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
                case DBTypes.MySQL:
                case DBTypes.MariaDB:
                    {
                        var dbConn = (MySqlConnection)cmdOption.Command.Connection;
                        dbConn.InfoMessage += (s, e) =>
                        {
                            listInfo.Add(e.Errors[0].Message);
                        };
                    }
                    break;
                case DBTypes.Oracle:
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
                case DBTypes.SQLServer:
                    {
                        var dbConn = (SqlConnection)cmdOption.Command.Connection;
                        dbConn.InfoMessage += (s, e) =>
                        {
                            listInfo.Add(e.Message);
                        };
                    }
                    break;
                case DBTypes.PostgreSQL:
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
    public static DataKitTo CreateDataKitInstance(DbKitConnectionOption connOption)
    {
        //额外处理 SQLite
        if (connOption.ConnectionType == DBTypes.SQLite && !string.IsNullOrWhiteSpace(connOption.ConnectionString) && connOption.ConnectionString.Length > 12)
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
                                throw new Exception($"{ds} Size exceeds limit(max {ParsingTo.FormatByte(maxLength)})");
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

        var dataKit = new DataKitTo(connOption);
        return dataKit;
    }

    /// <summary>
    /// 分批最大行数
    /// </summary>
    public static int BatchMaxRows { get; set; } = 10000;

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

        vm.Log.Add($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {nameof(ExportDataTable)}");
        vm.Log.Add("");

        //数据库
        edt.ReadConnectionInfo.SetConnDatabaseName(edt.ReadDatabaseName);
        edt.ReadConnectionInfo.AutoClose = false;

        var dataKit = CreateDataKitInstance(edt.ReadConnectionInfo);
        DataKitTo dataKit2 = null;

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

            vm.Log.Add($"read table: {sql}");

            var rowCount = 0;
            var batchNo = 0;

            var dt = new DataTable();
            //模式名.表名
            var sntn = string.Empty;

            var sw = new Stopwatch();
            sw.Start();

            //读取行
            await dataKit.DbInstance.SqlExecuteDataRow(sql, readRow: row =>
            {
                rowCount++;

                dt.Rows.Add(row);

                if (sw.Elapsed.TotalMilliseconds > 5000)
                {
                    vm.Log.Add($"read line: {dt.Rows.Count}/{rowCount}");
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

                    vm.Log.Add($"export table({sntn}) part {batchNo}, rows: {dt.Rows.Count}/{rowCount}, time: {vm.PartTimeFormat()}");
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
                    vm.Log.Add($"get table({sntn}) DDL");

                    var tableDesc = "-- --------------------";
                    string ddl;

                    // fix MySqlConnection is already in use. See https://fl.vu/mysql-conn-reuse
                    if (edt.ReadConnectionInfo.ConnectionType == DBTypes.MySQL)
                    {
                        dataKit2 ??= CreateDataKitInstance(edt.ReadConnectionInfo);
                        ddl = await dataKit2.GetTableDDL(dt.TableName, dt.Namespace);
                    }
                    else
                    {
                        ddl = await dataKit.GetTableDDL(dt.TableName, dt.Namespace);
                    }
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

                    vm.Log.Add($"export table({sntn}) DDL, time: {vm.PartTimeFormat()}");
                    vm.Log.Add("");
                }
            });

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

                vm.Log.Add($"export table({sntn}) part {batchNo}, rows: {dt.Rows.Count}/{rowCount}, time: {vm.PartTimeFormat()}");
            }

            vm.Log.Add($"export table({sntn}) complete, rows: {rowCount}, progress: {i + 1}/{edt.ListReadDataSQL.Count}");
            vm.Log.Add("");

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
        await dataKit.Close();
        if (dataKit2 != null)
        {
            await dataKit2.Close();
        }

        vm.Log.Add($"Done! Save to: {edt.PackagePath}, size: {ParsingTo.FormatByte(new FileInfo(edt.PackagePath).Length)}, total time: {vm.UseTimeFormat}");
        vm.Log.Add("");
        vm.Set(RCodeTypes.success);
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

        vm.Log.Add($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {nameof(MigrateDataTable)}");
        vm.Log.Add("");

        //读取数据库
        var dbKitRead = mdt.ReadConnectionInfo.CreateDbInstance();
        //写入数据库
        var dbKitWrite = mdt.WriteConnectionInfo.CreateDbInstance();
        //预检通过
        var isCopy = await dbKitWrite.PreExecute() != -1;

        //遍历
        for (int i = 0; i < mdt.ListReadWrite.Count; i++)
        {
            var rw = mdt.ListReadWrite[i];

            vm.Log.Add($"get write table({rw.WriteTableName}) meta");
            var edoWrite = await dbKitWrite.SqlExecuteDataSet(DbKitExtensions.SqlEmpty(rw.WriteTableName, tdb: mdt.WriteConnectionInfo.ConnectionType));
            var dtWrite = edoWrite.Datas.Tables[0];
            dtWrite.TableName = rw.WriteTableName.Split('.').Last();
            var dtWriteColumnName = dtWrite.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToList();

            //读取表的列 => 读取数据行索引,写入表的列
            var rwMap = new Dictionary<string, KeyValuePair<int, string>>();

            vm.Log.Add($"read table data SQL: {rw.ReadDataSQL}");

            var rowCount = 0;
            var batchNo = 0;
            var catchCount = 0;

            //读取行
            await dbKitRead.SqlExecuteDataRow(rw.ReadDataSQL, readRow: async row =>
            {
                rowCount++;

                //构建一行 写入表
                var drWriteNew = dtWrite.NewRow();
                //根据读取表列映射写入表列填充单元格数据
                foreach (var columnNameRead in rwMap.Keys)
                {
                    var colIndexAndMap = rwMap[columnNameRead];

                    //读取表列值
                    var valueRead = row[colIndexAndMap.Key];
                    if (valueRead is not DBNull)
                    {
                        //读取表列值 转换类型为 写入表列值
                        try
                        {
                            //写入表列
                            var columnNameWrite = colIndexAndMap.Value;
                            //写入表列类型
                            var typeWrite = dtWrite.Columns[columnNameWrite].DataType;

                            //读取表列值 赋予 写入表列
                            drWriteNew[columnNameWrite] = Convert.ChangeType(valueRead, typeWrite);
                        }
                        catch (Exception ex)
                        {
                            catchCount++;
                            vm.Log.Add($"value conversion ERROR");
                            vm.Log.Add(ex.ToJson());
                            var rowJson = row.ToJson();
                            vm.Log.Add(rowJson.Length > 500 ? rowJson[..500] : rowJson);
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
                            vm.Log.Add($"delete write table data SQL: {rw.WriteDeleteSQL}");
                            var num = await dbKitWrite.SqlExecuteNonQuery(rw.WriteDeleteSQL);
                            vm.Log.Add($"return the number of affected rows: {num}, time: {vm.PartTimeFormat()}");
                        }
                    }

                    //分批写入
                    vm.Log.Add($"write table({rw.WriteTableName}) part: {batchNo} (rows: {dtWrite.Rows.Count}/{rowCount})");
                    await dbKitWrite.BulkCopy(dtWrite, isCopy);

                    //清理
                    dtWrite.Clear();
                }
            }, emptyTable: etable =>
            {
                var index = 0;
                //构建列映射
                foreach (DataColumn dcRead in etable.Columns)
                {
                    //指定映射
                    if (rw.ReadWriteColumnMap.TryGetValue(dcRead.ColumnName, out string value))
                    {
                        rwMap.Add(dcRead.ColumnName, new KeyValuePair<int, string>(index, value));
                    }
                    else
                    {
                        //自动映射
                        var columnNameWrite = dtWriteColumnName.FirstOrDefault(x => x.Equals(dcRead.ColumnName, StringComparison.OrdinalIgnoreCase));
                        if (columnNameWrite != null)
                        {
                            rwMap.Add(dcRead.ColumnName, new KeyValuePair<int, string>(index, columnNameWrite));
                        }
                    }
                    index++;
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
                        vm.Log.Add($"delete write table data SQL: {rw.WriteDeleteSQL}");
                        var num = await dbKitWrite.SqlExecuteNonQuery(rw.WriteDeleteSQL);
                        vm.Log.Add($"return the number of affected rows: {num}, time: {vm.PartTimeFormat()}");
                    }
                }

                //分批写入
                vm.Log.Add($"write table({rw.WriteTableName}) part: {batchNo} (rows: {dtWrite.Rows.Count}/{rowCount})");
                await dbKitWrite.BulkCopy(dtWrite, isCopy);

                //清理
                dtWrite.Clear();
            }

            if (catchCount > 0)
            {
                vm.Log.Add($"value conversion ERROR: {catchCount}");
            }
            vm.Log.Add($"write table({rw.WriteTableName}) done! time: {vm.PartTimeFormat()}, progress: {i + 1}/{mdt.ListReadWrite.Count}");
            vm.Log.Add("");
        }

        vm.Log.Add($"Done! Total time：{vm.UseTimeFormat}");
        vm.Log.Add("");

        vm.Set(RCodeTypes.success);
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

        vm.Log.Add($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {nameof(ImportDatabase)}");
        vm.Log.Add("");

        var dbKit = idb.WriteConnectionInfo.CreateDbInstance();
        var isCopy = await dbKit.PreExecute() != -1; //预检通过

        vm.Log.Add($"read data source: {idb.PackagePath}");
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
        var writeTables = new List<DataKitTableResult>();
        if (!zipRead.Entries.Any(x => x.Name.EndsWith(".sql")))
        {
            vm.Log.Add("get write table information");
            var dataKit = CreateDataKitInstance(idb.WriteConnectionInfo);
            writeTables = await dataKit.GetTable();
        }

        var tableIndex = 0;
        foreach (var tableGroup in tableGroups)
        {
            tableIndex++;
            vm.Log.Add($"read table({tableGroup.Key}), progress: {tableIndex}/{tableGroups.Count()}");

            var sqlFile = tableGroup.FirstOrDefault(x => x.Name.EndsWith(".sql"));

            //仅数据时，支持表、列映射配置，导入前清空表数据，不同类型数据库（主要用于数据迁移）
            //含 SQL 文件时，视为同类型数据库，重建表，直接写入数据（主要用于备份还原）
            var dataOnly = sqlFile == null;
            if (!dataOnly)
            {
                var sqlContent = sqlFile.Open().ToText();

                vm.Log.Add($"create table, execute script: {sqlFile.Name}");
                await dbKit.SqlExecuteNonQuery(sqlContent);
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
                            vm.Log.Add($"write table({dt.TableName}) not found, skipped: {item.Name}");
                            continue;
                        }

                        //模式名.表名
                        sntn = DbKitExtensions.SqlSNTN(dt.TableName, dt.Namespace);

                        //清空表
                        if (hsName.Add(sntn) && idb.WriteDeleteData)
                        {
                            var deleteTableSql = DbKitExtensions.SqlClearTable(sntn, idb.WriteConnectionInfo.ConnectionType);

                            vm.Log.Add($"delete write table data SQL: {deleteTableSql}");
                            var num = await dbKit.SqlExecuteNonQuery(deleteTableSql);
                            vm.Log.Add($"return the number of affected rows: {num}, time: {vm.PartTimeFormat()}");
                        }
                    }

                    vm.Log.Add($"importing({sntn}) part: {item.Name} (rows: {dt.Rows.Count}, size: {ParsingTo.FormatByte(item.Length)})");
                    await dbKit.BulkCopy(dt, isCopy);
                    vm.Log.Add($"import({sntn}) part complete, time: {vm.PartTimeFormat()}");
                    vm.Log.Add("");
                }
                else
                {
                    vm.Log.Add($"skip empty part: {item.Name}");
                }
            }
            vm.Log.Add($"import table({tableGroup.Key}) done!");
            vm.Log.Add("");
        }

        vm.Log.Add($"Done! Total time：{vm.UseTimeFormat}");
        vm.Log.Add("");

        vm.Set(RCodeTypes.success);
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