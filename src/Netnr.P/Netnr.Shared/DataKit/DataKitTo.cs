#if Full || DataKit

using System.IO.Compression;
using Npgsql;
using MySqlConnector;
using Microsoft.Data.Sqlite;
using Microsoft.Data.SqlClient;
using Oracle.ManagedDataAccess.Client;

namespace Netnr;

/// <summary>
/// 数据交互
/// </summary>
public partial class DataKitTo
{
    #region 基础

    /// <summary>
    /// 数据库上下文
    /// </summary>
    public DbHelper db;

    /// <summary>
    /// 数据库类型
    /// </summary>
    public EnumTo.TypeDB tdb;

    /// <summary>
    /// 构造
    /// </summary>
    /// <param name="typeDB">类型</param>
    /// <param name="dbConnection">连接</param>
    public DataKitTo(EnumTo.TypeDB typeDB, DbConnection dbConnection)
    {
        tdb = typeDB;
        db = new DbHelper(dbConnection);
    }

    /// <summary>
    /// 默认库名
    /// </summary>
    /// <returns></returns>
    public string DefaultDatabaseName()
    {
        var databaseName = db.Connection.Database;

        if (string.IsNullOrWhiteSpace(databaseName))
        {
            if (db.Connection is OracleConnection connection)
            {
                var sb = new OracleConnectionStringBuilder(connection.ConnectionString);
                databaseName = sb.UserID;
            }
        }

        return databaseName;
    }

    /// <summary>
    /// 获取库名
    /// </summary>
    /// <returns></returns>
    public List<string> GetDatabaseName()
    {
        var result = new List<string>();

        var sql = DataKitScript.GetDatabaseName(tdb);
        var dt = db.SqlExecuteReader(sql).Item1.Tables[0];

        foreach (DataRow dr in dt.Rows)
        {
            if (tdb == EnumTo.TypeDB.SQLite)
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
    public List<DataKitDatabaseVM> GetDatabase(string filterDatabaseName = null)
    {
        var result = new List<DataKitDatabaseVM>();

        var listDatabaseName = string.IsNullOrWhiteSpace(filterDatabaseName)
            ? null : filterDatabaseName.Replace("'", "").Split(',');

        var sql = DataKitScript.GetDatabase(tdb, listDatabaseName);
        var ds = db.SqlExecuteReader(sql);

        if (tdb == EnumTo.TypeDB.SQLite)
        {
            var dt1 = ds.Item1.Tables[0];
            var charset = ds.Item1.Tables[1].Rows[0][0].ToString();
            foreach (DataRow dr in dt1.Rows)
            {
                var name = dr["name"].ToString();
                var file = dr["file"].ToString();
                var fi = new FileInfo(file);

                result.Add(new DataKitDatabaseVM
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
            result = ds.Item1.Tables[0].ToModel<DataKitDatabaseVM>();
        }

        return result;
    }

    /// <summary>
    /// 获取表
    /// </summary>
    /// <param name="schemaName">模式名</param>
    /// <param name="databaseName">数据库名</param>
    /// <returns></returns>
    public List<DataKitTableVM> GetTable(string schemaName = null, string databaseName = null)
    {
        var result = new List<DataKitTableVM>();

        if (string.IsNullOrWhiteSpace(databaseName))
        {
            databaseName = DefaultDatabaseName();
        }

        var sql = DataKitScript.GetTable(tdb, databaseName, schemaName);
        var ds = db.SqlExecuteReader(sql);
        result = ds.Item1.Tables[0].ToModel<DataKitTableVM>();

        if (tdb == EnumTo.TypeDB.SQLite)
        {
            //计算表行 https://stackoverflow.com/questions/4474873
            var listsql = new List<string> { "SELECT '' AS TableName, 0 AS TableRows" };
            result.ForEach(t =>
            {
                var tableName = DbHelper.SqlQuote(EnumTo.TypeDB.SQLite, t.TableName);
                var sqlrows = $"SELECT '{t.TableName}' AS TableName, max(RowId) - min(RowId) + 1 AS TableRows FROM {tableName}";
                listsql.Add(sqlrows);
            });
            var sqls = string.Join("\nUNION ALL\n", listsql);

            var dsrows = db.SqlExecuteReader(sqls).Item1.Tables[0].Rows.Cast<DataRow>();
            result.ForEach(item =>
            {
                var trow = dsrows.FirstOrDefault(x => x[0].ToString().ToLower() == item.TableName.ToLower());
                if (trow != null && trow[1].ToString() != "")
                {
                    item.TableRows = Convert.ToInt64(trow[1]);
                }
            });
        }

        return result;
    }

    /// <summary>
    /// 表DDL
    /// </summary>
    /// <param name="tableName">表名</param>
    /// <param name="schemaName">模式名</param>
    /// <param name="databaseName">数据库名</param>
    /// <returns></returns>
    public string GetTableDDL(string tableName, string schemaName = null, string databaseName = null)
    {
        string result = string.Empty;

        if (string.IsNullOrWhiteSpace(databaseName))
        {
            databaseName = DefaultDatabaseName();
        }

        var sql = DataKitScript.GetTableDDL(tdb, databaseName, schemaName, tableName);
        switch (tdb)
        {
            case EnumTo.TypeDB.SQLite:
                {
                    var ds = db.SqlExecuteReader(sql);

                    var rows = ds.Item1.Tables[0].Rows;
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
                    var ds = db.SqlExecuteReader(sql);

                    var rows = ds.Item1.Tables[0].Rows;
                    var ddl = new List<string> { $"DROP TABLE IF EXISTS `{rows[0][0]}`" };

                    var script = rows[0][1].ToString();
                    ddl.Add(script);

                    result = string.Join(";\r\n", ddl) + ";";
                }
                break;
            case EnumTo.TypeDB.Oracle:
                {
                    var ds = db.SqlExecuteReader(sql, func: cmd =>
                    {
                        var ocmd = (OracleCommand)cmd;

                        //begin ... end;
                        if (DbHelper.SqlParserBeginEnd(sql))
                        {
                            //open:name for
                            var cursors = DbHelper.SqlParserCursors(sql);
                            foreach (var cursor in cursors)
                            {
                                ocmd.Parameters.Add(cursor, OracleDbType.RefCursor, DBNull.Value, ParameterDirection.Output);
                            }
                        }

                        return cmd;
                    });

                    var ddlTable = ds.Item1.Tables[0].Rows[0][0].ToString().Trim();
                    var ddlIndex = ds.Item1.Tables[1].Rows.Cast<DataRow>().Select(x => x[0].ToString().Trim() + ";");
                    var ddlCheck = ds.Item1.Tables[2].Rows.Cast<DataRow>().Select(x => x[0].ToString().Trim() + ";");
                    var ddlTableComment = ds.Item1.Tables[3].Rows[0][0].ToString().Trim();
                    var ddlColumnComment = ds.Item1.Tables[4];

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
                    var ds = db.SqlExecuteReader(sql);

                    result = ds.Item1.Tables[0].Rows[0][1].ToString();
                }
                break;
            case EnumTo.TypeDB.PostgreSQL:
                {
                    //消息
                    var listInfo = new List<string>();
                    var dbConn = (NpgsqlConnection)db.Connection;
                    dbConn.Notice += (s, e) =>
                    {
                        listInfo.Add(e.Notice.MessageText);
                    };

                    db.SqlExecuteReader(sql);

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
    public List<DataKitColumnVM> GetColumn(string filterSchemaNameTableName = null, string databaseName = null)
    {
        var result = new List<DataKitColumnVM>();

        if (string.IsNullOrWhiteSpace(databaseName))
        {
            databaseName = DefaultDatabaseName();
        }

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

        var sql = DataKitScript.GetColumn(tdb, databaseName, listSchemaNameTableName);
        var ds = db.SqlExecuteReader(sql);

        if (tdb == EnumTo.TypeDB.SQLite)
        {
            ds.Item1.Tables[0].Rows.RemoveAt(0);
            var ds2 = ds.Item1.Tables[1].Select();

            var aakey = "AUTOINCREMENT";
            foreach (DataRow dr in ds.Item1.Tables[0].Rows)
            {
                var csql = ds2.FirstOrDefault(x => x["name"].ToString().ToLower() == dr["TableName"].ToString().ToLower())[1].ToString().ToUpper();
                if (csql.Contains(aakey))
                {
                    var isaa = csql.Split(',').Any(x => x.Contains(aakey) && x.Contains(dr["ColumnName"].ToString().ToUpper()));
                    if (isaa)
                    {
                        dr["AutoAdd"] = "YES";
                    }
                }
            }
        }

        result = ds.Item1.Tables[0].ToModel<DataKitColumnVM>();

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
    public bool SetTableComment(string tableName, string tableComment, string schemaName = null, string databaseName = null)
    {
        if (tdb != EnumTo.TypeDB.SQLite)
        {
            if (string.IsNullOrWhiteSpace(databaseName))
            {
                databaseName = DefaultDatabaseName();
            }

            var sql = DataKitScript.SetTableComment(tdb, databaseName, schemaName, tableName, tableComment);
            db.SqlExecuteNonQuery(sql);
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
    public bool SetColumnComment(string tableName, string columnName, string columnComment, string schemaName = null, string databaseName = null)
    {
        if (tdb != EnumTo.TypeDB.SQLite)
        {
            if (string.IsNullOrWhiteSpace(databaseName))
            {
                databaseName = DefaultDatabaseName();
            }

            var sql = DataKitScript.SetColumnComment(tdb, databaseName, schemaName, tableName, columnName, columnComment);
            db.SqlExecuteNonQuery(sql);
        }

        return true;
    }

    /// <summary>
    /// 执行脚本
    /// </summary>
    /// <param name="sql">脚本</param>
    /// <returns></returns>
    public ValueTuple<DataSet, DataSet, object> ExecuteSql(string sql)
    {
        var sw = Stopwatch.StartNew();

        //开启事务
        var openTransaction = true;
        var sqlUpper = sql.ToUpper();
        var listKey = "DROP DATABASE,ALTER DATABASE,CREATE DATABASE".Split(',');
        var listTypeDB = EnumTo.TypeDB.PostgreSQL | EnumTo.TypeDB.SQLServer;
        if (listTypeDB.HasFlag(tdb) && listKey.Any(sqlUpper.Contains))
        {
            openTransaction = false;
        }
        if (tdb == EnumTo.TypeDB.SQLite && sqlUpper == "VACUUM")
        {
            openTransaction = false; 
        }

        //消息
        var listInfo = new List<string>();

        var er = db.SqlExecuteReader(sql, func: cmd =>
        {
            switch (tdb)
            {
                case EnumTo.TypeDB.MySQL:
                case EnumTo.TypeDB.MariaDB:
                    {
                        var dbConn = (MySqlConnection)cmd.Connection;
                        dbConn.InfoMessage += (s, e) =>
                        {
                            listInfo.Add(e.Errors[0].Message);
                        };
                    }
                    break;
                case EnumTo.TypeDB.Oracle:
                    {
                        var dbCmd = (OracleCommand)cmd;
                        var dbConn = dbCmd.Connection;
                        dbConn.InfoMessage += (s, e) =>
                        {
                            listInfo.Add(e.Message);
                        };

                        //begin ... end;
                        if (DbHelper.SqlParserBeginEnd(sql))
                        {
                            //open:name for
                            var cursors = DbHelper.SqlParserCursors(sql);
                            foreach (var cursor in cursors)
                            {
                                dbCmd.Parameters.Add(cursor, OracleDbType.RefCursor, DBNull.Value, ParameterDirection.Output);
                            }
                        }
                    }
                    break;
                case EnumTo.TypeDB.SQLServer:
                    {
                        var dbConn = (SqlConnection)cmd.Connection;
                        dbConn.InfoMessage += (s, e) =>
                        {
                            listInfo.Add(e.Message);
                        };
                    }
                    break;
                case EnumTo.TypeDB.PostgreSQL:
                    {
                        var dbConn = (NpgsqlConnection)cmd.Connection;
                        dbConn.Notice += (s, e) =>
                        {
                            listInfo.Add(e.Notice.MessageText);
                        };
                    }
                    break;
            }

            return cmd;
        }, openTransaction: openTransaction);

        listInfo.Add($"耗时: {sw.Elapsed}");

        if (er.Item2 != -1)
        {
            listInfo.Insert(0, $"({er.Item2} 行受影响)");
        }

        var dtInfo = new DataTable();
        dtInfo.Columns.Add(new DataColumn("message"));
        listInfo.ForEach(info =>
        {
            var drInfo = dtInfo.NewRow();
            drInfo[0] = info;
            dtInfo.Rows.Add(drInfo.ItemArray);
        });

        return new ValueTuple<DataSet, DataSet, object>(er.Item1, er.Item3, new { info = dtInfo });
    }

    #endregion

    #region 方法

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="tdb"></param>
    /// <param name="conn"></param>
    /// <param name="databaseName"></param>
    /// <returns></returns>
    public static DataKitTo Init(EnumTo.TypeDB tdb, string conn, string databaseName = null)
    {
        DataKitTo dk = null;

        try
        {
            //连接信息
            DbConnection dbConnection = null;

            //额外处理 SQLite
            if (tdb == EnumTo.TypeDB.SQLite)
            {
                //下载 SQLite 文件
                var ds = conn[12..].TrimEnd(';');
                //路径
                var dspath = Path.GetTempPath();
                //文件名
                var dsname = Path.GetFileName(ds);
                var fullPath = Path.Combine(dspath, dsname);

                //网络路径
                if (ds.ToLower().StartsWith("http"))
                {
                    //不存在则下载
                    if (!File.Exists(fullPath))
                    {
                        //下载
                        HttpTo.DownloadSave(ds, fullPath);
                    }

                    conn = "Data Source=" + fullPath;
                }
                else
                {
                    conn = "Data Source=" + ds;
                }
            }

            conn = DbHelper.SqlConnPreCheck(tdb, conn);
            switch (tdb)
            {
                case EnumTo.TypeDB.SQLite:
                    dbConnection = new SqliteConnection(conn);
                    break;
                case EnumTo.TypeDB.MySQL:
                case EnumTo.TypeDB.MariaDB:
                    {
                        var csb = new MySqlConnectionStringBuilder(conn);
                        if (!string.IsNullOrWhiteSpace(databaseName))
                        {
                            csb.Database = databaseName;
                        }
                        dbConnection = new MySqlConnection(csb.ConnectionString);
                    }
                    break;
                case EnumTo.TypeDB.Oracle:
                    {
                        dbConnection = new OracleConnection(conn);
                    }
                    break;
                case EnumTo.TypeDB.SQLServer:
                    {
                        var csb = new SqlConnectionStringBuilder(conn);
                        if (!string.IsNullOrWhiteSpace(databaseName))
                        {
                            csb.InitialCatalog = databaseName;
                        }
                        dbConnection = new SqlConnection(csb.ConnectionString);
                    }
                    break;
                case EnumTo.TypeDB.PostgreSQL:
                    {
                        var csb = new NpgsqlConnectionStringBuilder(conn);
                        if (!string.IsNullOrWhiteSpace(databaseName))
                        {
                            csb.Database = databaseName;
                        }
                        dbConnection = new NpgsqlConnection(csb.ConnectionString);
                    }
                    break;
            }

            dk = new DataKitTo(tdb, dbConnection);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

        return dk;
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="connectionInfo">连接信息</param>
    /// <returns></returns>
    public static DataKitTo Init(DataKitTransferVM.ConnectionInfo connectionInfo)
    {
        DataKitTo dk = Init(connectionInfo.ConnectionType, connectionInfo.ConnectionString, connectionInfo.DatabaseName);
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
    public static DataKitTransferVM.ExportDataTable ConvertTransferVM(DataKitTransferVM.ExportDatabase edb)
    {
        if (edb.ListReadTableName.Count == 0)
        {
            var dk = Init(edb.ReadConnectionInfo);
            edb.ListReadTableName = dk.GetTable().Select(x => DbHelper.SqlSNTN(x.TableName, x.SchemaName, edb.ReadConnectionInfo.ConnectionType)).ToList();
        }

        var edt = new DataKitTransferVM.ExportDataTable().ToCopy(edb);
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
    public static ResultVM ExportDatabase(DataKitTransferVM.ExportDatabase edb, Action<NotifyCollectionChangedEventArgs> le = null)
    {
        var edt = ConvertTransferVM(edb);
        return ExportDataTable(edt, le);
    }

    /// <summary>
    /// 导出表
    /// </summary>
    /// <param name="edt"></param>
    /// <param name="le">日志事件</param>
    /// <returns></returns>
    public static ResultVM ExportDataTable(DataKitTransferVM.ExportDataTable edt, Action<NotifyCollectionChangedEventArgs> le = null)
    {
        var vm = new ResultVM();
        vm.LogEvent(le);

        vm.Log.Add($"\n{DateTime.Now:yyyy-MM-dd HH:mm:ss} 导出数据\n");

        //数据库
        var db = edt.ReadConnectionInfo.NewDbHelper();

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
            db.SqlExecuteDataRow(sql, row =>
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

                    var ceStream = zae.Open();
                    dt.WriteXml(ceStream, XmlWriteMode.WriteSchema);
                    ceStream.Close();

                    vm.Log.Add($"导出表（{dt.TableName}）第 {batchNo} 批（行：{dt.Rows.Count}/{rowCount}），耗时：{vm.PartTimeFormat()}");
                    sw.Restart();

                    dt.Clear();
                }
            }, emptyTable =>
            {
                dt = emptyTable.Clone();
                dt.MinimumCapacity = BatchMaxRows;
                sntn = DbHelper.SqlSNTN(dt.TableName, dt.Namespace);

                //表结构 DDL
                if (edt.Type == "all")
                {
                    var dk = edt.ReadConnectionInfo.NewDataKit();

                    var tableDesc = "-- --------------------";
                    var ddl = dk.GetTableDDL(dt.TableName, dt.Namespace);
                    var bytes = string.Join("\r\n", new string[] { tableDesc, $"-- {sntn}", tableDesc, ddl }).ToByte();

                    var sqlName = $"{sntn}.sql";

                    //sql 写入 zip
                    var zae = isOldZip
                    ? zip.GetEntry(sqlName) ?? zip.CreateEntry(sqlName)
                    : zip.CreateEntry(sqlName);

                    var ceStream = zae.Open();
                    ceStream.Write(bytes);
                    ceStream.Close();

                    vm.Log.Add($"导出表（{sntn}）DDL结构，耗时：{vm.PartTimeFormat()}");
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

                var ceStream = zae.Open();
                dt.WriteXml(ceStream, XmlWriteMode.WriteSchema);
                ceStream.Close();

                vm.Log.Add($"导出表（{sntn}）第 {batchNo} 批（行：{dt.Rows.Count}/{rowCount}），耗时：{vm.PartTimeFormat()}");
            }

            vm.Log.Add($"导出表（{sntn}）完成（行：{rowCount}），导表进度：{i + 1}/{edt.ListReadDataSQL.Count}\n");

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

        vm.Log.Add($"导出完成：{edt.PackagePath}，共耗时：{vm.UseTimeFormat}\n");
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
    public static ResultVM MigrateDataTable(DataKitTransferVM.MigrateDataTable mdt, Action<NotifyCollectionChangedEventArgs> le = null)
    {
        var vm = new ResultVM();
        vm.LogEvent(le);

        vm.Log.Add($"\n{DateTime.Now:yyyy-MM-dd HH:mm:ss} 迁移表数据\n");

        //读取数据库
        var dbRead = mdt.ReadConnectionInfo.NewDbHelper();
        //写入数据库
        var dbWrite = mdt.WriteConnectionInfo.NewDbHelper();
        //预检通过
        var isCopy = dbWrite.PreCheck() != -1;

        //遍历
        for (int i = 0; i < mdt.ListReadWrite.Count; i++)
        {
            var rw = mdt.ListReadWrite[i];

            vm.Log.Add($"获取写入表（{rw.WriteTableName}）结构");
            var dtWrite = dbWrite.SqlExecuteReader(DbHelper.SqlEmpty(rw.WriteTableName, tdb: mdt.WriteConnectionInfo.ConnectionType)).Item1.Tables[0];
            dtWrite.TableName = rw.WriteTableName.Split('.').Last();
            var dtWriteColumnName = dtWrite.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToList();

            //读取表的列 => 写入表的列
            var rwMap = new Dictionary<string, string>();

            vm.Log.Add($"获取读取表数据 SQL：{rw.ReadDataSQL}");

            var rowCount = 0;
            var batchNo = 0;
            var catchCount = 0;

            //读取行
            dbRead.SqlExecuteDataRow(rw.ReadDataSQL, rowRead =>
            {
                rowCount++;

                //构建一行 写入表
                var drWriteNew = dtWrite.NewRow();
                //根据读取表列映射写入表列填充单元格数据
                var columnIndex = 0;
                foreach (var columnNameRead in rwMap.Keys)
                {
                    //读取表列值
                    var valueRead = rowRead[columnIndex++];
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
                            var num = dbWrite.SqlExecuteReader(rw.WriteDeleteSQL).Item2;
                            vm.Log.Add($"返回受影响行数：{num}，耗时：{vm.PartTimeFormat()}");
                        }
                    }

                    //分批写入
                    vm.Log.Add($"写入表（{rw.WriteTableName}）第 {batchNo} 批（行：{dtWrite.Rows.Count}/{rowCount}）");

                    dbWrite.BulkCopy(mdt.WriteConnectionInfo.ConnectionType, dtWrite, isCopy);

                    //清理
                    dtWrite.Clear();
                }
            }, emptyTable =>
            {
                //构建列映射
                foreach (DataColumn dcRead in emptyTable.Columns)
                {
                    //指定映射
                    if (rw.ReadWriteColumnMap.TryGetValue(dcRead.ColumnName, out string value))
                    {
                        rwMap.Add(dcRead.ColumnName, value);
                    }
                    else
                    {
                        //自动映射
                        var columnNameWrite = dtWriteColumnName.FirstOrDefault(x => x.ToLower() == dcRead.ColumnName.ToLower());
                        if (columnNameWrite != null)
                        {
                            rwMap.Add(dcRead.ColumnName, columnNameWrite);
                        }
                    }
                }
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
                        var num = dbWrite.SqlExecuteReader(rw.WriteDeleteSQL).Item2;
                        vm.Log.Add($"返回受影响行数：{num}，耗时：{vm.PartTimeFormat()}");
                    }
                }

                //分批写入
                vm.Log.Add($"写入表（{rw.WriteTableName}）第 {batchNo} 批（行：{dtWrite.Rows.Count}/{rowCount}）");

                dbWrite.BulkCopy(mdt.WriteConnectionInfo.ConnectionType, dtWrite, isCopy);

                //清理
                dtWrite.Clear();
            }

            if (catchCount > 0)
            {
                vm.Log.Add($"列值转换失败：{catchCount} 次");
            }
            vm.Log.Add($"写入表（{rw.WriteTableName}）完成，耗时：{vm.PartTimeFormat()}，写表进度：{i + 1}/{mdt.ListReadWrite.Count}\n");
        }

        vm.Log.Add($"总共耗时：{vm.UseTimeFormat}\n");
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
    public static ResultVM ImportDatabase(DataKitTransferVM.ImportDatabase idb, Action<NotifyCollectionChangedEventArgs> le = null)
    {
        var vm = new ResultVM();
        vm.LogEvent(le);

        vm.Log.Add($"\n{DateTime.Now:yyyy-MM-dd HH:mm:ss} 导入数据\r\n");
        vm.Log.Add($"读取数据源：{idb.PackagePath}\r\n");

        var db = idb.WriteConnectionInfo.NewDbHelper();
        var isCopy = db.PreCheck() != -1; //预检通过

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
        var writeTables = new List<DataKitTableVM>();
        if (!zipRead.Entries.Any(x => x.Name.EndsWith(".sql")))
        {
            vm.Log.Add($"读取写入库表信息");
            var dk = Init(idb.WriteConnectionInfo);
            writeTables = dk.GetTable();
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
                db.SqlExecuteNonQuery(sqlContent);
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
                        if (!string.IsNullOrWhiteSpace(writeTableMap) && writeTables.Any(x => DbHelper.SqlEqualSNTN(writeTableMap, x.TableName, x.SchemaName)))
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
                        sntn = DbHelper.SqlSNTN(dt.TableName, dt.Namespace);

                        //清空表
                        if (hsName.Add(sntn) && idb.WriteDeleteData)
                        {
                            var clearTableSql = DbHelper.SqlClearTable(idb.WriteConnectionInfo.ConnectionType, sntn);

                            vm.Log.Add($"清理写入表：{clearTableSql}");
                            var num = db.SqlExecuteReader(clearTableSql).Item2;
                            vm.Log.Add($"返回受影响行数：{num}，耗时：{vm.PartTimeFormat()}");
                        }
                    }

                    vm.Log.Add($"正在导入（{sntn}）分片：{item.Name}（行：{dt.Rows.Count}，大小：{ParsingTo.FormatByteSize(item.Length)}）");
                    db.BulkCopy(idb.WriteConnectionInfo.ConnectionType, dt, isCopy);
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