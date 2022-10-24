using MiniExcelLibs;
using System.IO.Compression;

namespace Netnr.DataX.Menu;

/// <summary>
/// 数据
/// </summary>
public partial class MenuDataService
{
    [Display(Name = "Back to menu", Description = "返回菜单")]
    public static void BackToMenu() => DXService.InvokeMenu(typeof(MenuMainService));

    [Display(Name = "Connection Test", Description = "连接测试", GroupName = "\r\n")]
    public static void ConnectionTest()
    {
        //配置
        var ci = new ConfigInit();
        var cc = ci.DXConfig;

        if (cc.ListConnectionInfo.Count > 0)
        {
            var badConn = 0;
            cc.ListConnectionInfo.AsParallel().ForAll(ci =>
            {
                var connInfo = new List<string> { $"{ci.ConnectionRemark} -> {ci.ConnectionType} => {ci.ConnectionString}" };
                var isSuccess = false;
                using var dbConn = DbHelper.DbConn(ci.ConnectionType, ci.ConnectionString);
                try
                {
                    dbConn.Open();

                    isSuccess = true;
                    connInfo.Insert(0, $"\nTest connection is successful ({ci.ConnectionType} {dbConn.ServerVersion})");

                    var qv = string.Empty;
                    switch (ci.ConnectionType)
                    {
                        case EnumTo.TypeDB.Oracle:
                            qv = "select * from v$version where rownum=1";
                            break;
                        case EnumTo.TypeDB.SQLServer:
                            qv = "select @@version";
                            break;
                        case EnumTo.TypeDB.PostgreSQL:
                            qv = "select version()";
                            break;
                    }
                    if (!string.IsNullOrWhiteSpace(qv))
                    {
                        var db = new DbHelper(dbConn);
                        connInfo.Add(db.SqlExecuteScalar(qv).ToString());
                    }

                    if (dbConn.State != ConnectionState.Closed)
                    {
                        dbConn.Close();
                    }
                }
                catch (Exception ex)
                {
                    badConn++;
                    connInfo.Insert(0, "\nTest connection failed");
                    connInfo.Add(ex.ToJson(true));
                }

                connInfo.Add($"Drive: {dbConn.GetType().Assembly.FullName}");

                DXService.Log(string.Join(Environment.NewLine, connInfo), isSuccess ? ConsoleColor.Green : ConsoleColor.Red);
            });

            DXService.Log($"\nSuccessful: {cc.ListConnectionInfo.Count - badConn}, Failed: {badConn}");
        }
        else
        {
            DXService.Log($"Not Found", ConsoleColor.Red);
        }
    }

    [Display(Name = "Parameter Optimization (SQLite MySQL)", Description = "参数优化")]
    public static void ParameterOptimization()
    {
        //配置
        var ci = new ConfigInit();
        var cc = ci.DXConfig;

        //选择库
        var cdb = DXService.ConsoleReadDatabase(cc);
        var db = cdb.NewDbHelper();

        switch (cdb.ConnectionType)
        {
            case EnumTo.TypeDB.SQLite:
                {
                    DXService.Log($"磁盘空间释放：VACUUM");
                    db.SqlExecuteNonQuery("VACUUM");
                }
                break;
            case EnumTo.TypeDB.MySQL:
            case EnumTo.TypeDB.MariaDB:
                {
                    if (db.PreCheck() == 0)
                    {
                        DXService.Log($"\n没有需要优化的参数", ConsoleColor.Cyan);
                    }
                }
                break;
            case EnumTo.TypeDB.Oracle:
                break;
            case EnumTo.TypeDB.SQLServer:
                break;
            case EnumTo.TypeDB.PostgreSQL:
                break;
        }
    }

    [Display(Name = "Data Export", Description = "数据导出", GroupName = "\r\n")]
    public static void DataExport()
    {
        //配置
        var ci = new ConfigInit();
        var cc = ci.DXConfig;

        var edt = new DataKitTransferVM.ExportDataTable()
        {
            ReadConnectionInfo = DXService.ConsoleReadDatabase(cc)
        };

        var etypeIndex = DXService.ConsoleReadItem("选择导出类型", new string[] { "仅表数据", "结构及数据" }, 2);

        if (DXService.ConsoleReadBool("数据库全表"))
        {
            var edb = new DataKitTransferVM.ExportDatabase()
            {
                ReadConnectionInfo = edt.ReadConnectionInfo
            };
            DXService.Log("读取表脚本构建");
            edt = DataKitTo.ConvertTransferVM(edb);
        }
        else
        {
            //读取表
            Console.Write("读取表(SELECT * FROM dbo.Table1; SELECT * FROM Table2): ");
            var tns = Console.ReadLine().Trim();
            if (!string.IsNullOrWhiteSpace(tns))
            {
                edt.ListReadDataSQL.AddRange(tns.Split(';'));
            }
            edt.ListReadDataSQL = edt.ListReadDataSQL.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
        }

        edt.Type = "dataOnly,all".Split(',')[etypeIndex - 1];
        edt.PackagePath = Path.Combine(ci.DXHub, DXService.NewFileName(edt.ReadConnectionInfo.ConnectionType, ".zip"));
        edt.PackagePath = DXService.ConsoleReadPath("导出完整路径", 1, edt.PackagePath, false);

        var vm = new ResultVM();
        DXService.TryAgain(() =>
        {
            vm = DataKitTo.ExportDataTable(edt, le => DXService.Log($"{le.NewItems[0]}"));
        });
    }

    [Display(Name = "Data Import", Description = "数据导入")]
    public static void DataImport()
    {
        //配置
        var ci = new ConfigInit();
        var cc = ci.DXConfig;

        var idb = new DataKitTransferVM.ImportDatabase()
        {
            WriteConnectionInfo = DXService.ConsoleReadDatabase(cc),
            PackagePath = DXService.ConsoleReadPath("导入包(zip)", 1)
        };
        //没有 sql 文件，提示是否清空数据
        using var zipRead = ZipFile.OpenRead(idb.PackagePath);
        if (!zipRead.Entries.Any(x => x.Name.EndsWith(".sql")))
        {
            idb.WriteDeleteData = DXService.ConsoleReadBool("写入前清空表数据");
        }

        var vm = new ResultVM();
        DXService.TryAgain(() =>
        {
            vm = DataKitTo.ImportDatabase(idb, cce =>
            {
                DXService.Log($"{cce.NewItems[0]}");
            });
        });
    }

    [Display(Name = "Data Migrate", Description = "数据迁移")]
    public static void DataMigrate()
    {
        //配置
        var ci = new ConfigInit();
        var cc = ci.DXConfig;

        DXService.Log("\n注意：参数模式（静默执行）可配置多表、指定列映射\n");

        var mdt = new DataKitTransferVM.MigrateDataTable
        {
            ReadConnectionInfo = DXService.ConsoleReadDatabase(cc, "读取库"),
            WriteConnectionInfo = DXService.ConsoleReadDatabase(cc, "写入库")
        };

        if (DXService.ConsoleReadBool("数据库全表"))
        {
            var mdb = new DataKitTransferVM.MigrateDatabase
            {
                ReadConnectionInfo = mdt.ReadConnectionInfo,
                WriteConnectionInfo = mdt.WriteConnectionInfo,
                WriteDeleteData = DXService.ConsoleReadBool("写入前清空表数据")
            };
            mdt = mdb.AsMigrateDataTable();
        }
        else
        {
            var rwi = new DataKitTransferVM.ReadWriteItem();

            Console.Write("读取表数据 SQL: ");
            rwi.ReadDataSQL = Console.ReadLine();

            Console.Write("写入表名: ");
            rwi.WriteTableName = Console.ReadLine();

            var clearTableSql = DbHelper.SqlClearTable(mdt.WriteConnectionInfo.ConnectionType, rwi.WriteTableName);
            Console.Write($"清空写入表(可选, {clearTableSql}): ");
            rwi.WriteDeleteSQL = Console.ReadLine();

            mdt.ListReadWrite = new List<DataKitTransferVM.ReadWriteItem>() { rwi };
        }

        var vm = new ResultVM();
        DXService.TryAgain(() =>
        {
            vm = DataKitTo.MigrateDataTable(mdt, le => DXService.Log(le.NewItems[0].ToString()));
        });
    }

    [Display(Name = "Excel Export", Description = "Excel 导出")]
    public static void ExcelExport()
    {
        //配置
        var ci = new ConfigInit();
        var cc = ci.DXConfig;

        var edt = new DataKitTransferVM.ExportDataTable()
        {
            ReadConnectionInfo = DXService.ConsoleReadDatabase(cc)
        };

        if (DXService.ConsoleReadBool("数据库全表"))
        {
            var edb = new DataKitTransferVM.ExportDatabase()
            {
                ReadConnectionInfo = edt.ReadConnectionInfo
            };
            edt = DataKitTo.ConvertTransferVM(edb);
        }
        else
        {
            //读取表
            Console.Write("读取表(SELECT * FROM dbo.Table1; SELECT * FROM Table2): ");
            var tns = Console.ReadLine().Trim();
            if (!string.IsNullOrWhiteSpace(tns))
            {
                edt.ListReadDataSQL.AddRange(tns.Split(';'));
            }
            edt.ListReadDataSQL = edt.ListReadDataSQL.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
        }

        edt.PackagePath = Path.Combine(ci.DXHub, DXService.NewFileName(edt.ReadConnectionInfo.ConnectionType, ".xlsx"));
        edt.PackagePath = DXService.ConsoleReadPath("导出完整路径", 1, edt.PackagePath, false);

        DXService.TryAgain(() =>
        {
            var sheets = new Dictionary<string, object>();
            for (int i = 0; i < edt.ListReadDataSQL.Count; i++)
            {
                var sql = edt.ListReadDataSQL[i];

                var db = edt.ReadConnectionInfo.NewDbHelper();
                db.Connection.Open();

                var dbc = db.GetCommand(sql);
                var reader = dbc.ExecuteReader(CommandBehavior.KeyInfo | CommandBehavior.CloseConnection);

                var rts = DbHelper.ReaderTableSchema(reader);
                var sntn = DbHelper.SqlSNTN(rts.Item1.TableName, rts.Item2.Namespace);
                if (sheets.ContainsKey(sntn))
                {
                    sntn = $"Sheet{i + 1}";
                }

                sheets.Add(sntn, reader);
            }

            var st = new TimingVM();
            DXService.Log("正在导出 ...");
            MiniExcel.SaveAs(edt.PackagePath, sheets, overwriteFile: true);
            DXService.Log($"导出完成，耗时：{st.PartTimeFormat()}");
        });
    }

    [Display(Name = "Excel Import", Description = "Excel 导入")]
    public static void ExcelImport()
    {
        //配置
        var ci = new ConfigInit();
        var cc = ci.DXConfig;

        var idb = new DataKitTransferVM.ImportDatabase()
        {
            WriteConnectionInfo = DXService.ConsoleReadDatabase(cc),
            PackagePath = DXService.ConsoleReadPath("导入包(xlsx)", 1),
            WriteDeleteData = DXService.ConsoleReadBool("写入前清空表数据")
        };

        Console.Write("指定表名(逗号分隔,默认工作薄名称): ");
        var tns = Console.ReadLine();
        var listTableName = new List<string>();
        if (!string.IsNullOrWhiteSpace(tns))
        {
            listTableName.AddRange(tns.Split(','));
        }

        var vm = new ResultVM();
        DXService.TryAgain(() =>
        {
            var dk = DataKitTo.Init(idb.WriteConnectionInfo);
            var tables = dk.GetTable(databaseName: idb.WriteDatabaseName);

            var isCopy = dk.db.PreCheck() != -1; //预检通过

            var sheetNames = MiniExcel.GetSheetNames(idb.PackagePath);
            for (int i = 0; i < sheetNames.Count; i++)
            {
                var sheetName = sheetNames[i];
                var sntn = i < listTableName.Count ? listTableName[i] : sheetName;
                var listSNTN = sntn.Split('.');
                string sn = listSNTN.Length > 1 ? listSNTN[0] : null;
                string tn = listSNTN.Length > 1 ? listSNTN[1] : sntn;

                //检测表名是否存在
                var table = tables.FirstOrDefault(x => x.TableName == tn && (sn == null || x.SchemaName == sn));
                if (table != null)
                {
                    //空表结构
                    var sqlEmpty = DbHelper.SqlEmpty(sntn, idb.WriteConnectionInfo.ConnectionType);
                    var dtWrite = dk.db.SqlExecuteReader(sqlEmpty).Item1.Tables[0];
                    dtWrite.Namespace = table.SchemaName;
                    dtWrite.TableName = table.TableName;

                    //清空表
                    if (idb.WriteDeleteData)
                    {
                        vm.PartTime();
                        var clearTableSql = DbHelper.SqlClearTable(idb.WriteConnectionInfo.ConnectionType, sntn);

                        DXService.Log($"清理写入表：{clearTableSql}");
                        var num = dk.db.SqlExecuteNonQuery(clearTableSql);
                        DXService.Log($"返回受影响行数：{num}，耗时：{vm.PartTimeFormat()}");
                    }

                    //读取数据
                    var batchIndex = 0;
                    var batchRows = 0;
                    var rows = MiniExcel.Query(idb.PackagePath, sheetName: sheetName, useHeaderRow: true);
                    foreach (IDictionary<string, object> row in rows)
                    {
                        //填充行
                        var drWrite = dtWrite.NewRow();
                        foreach (DataColumn dc in dtWrite.Columns)
                        {
                            if (row.ContainsKey(dc.ColumnName))
                            {
                                var val = row[dc.ColumnName]?.ToString();
                                try
                                {
                                    if (dc.DataType == typeof(string))
                                    {
                                        drWrite[dc.ColumnName] = val;
                                    }
                                    else if (!string.IsNullOrEmpty(val))
                                    {
                                        drWrite[dc.ColumnName] = val.ToConvert(dc.DataType);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    DXService.Log($"值转换失败: {dc.ColumnName}({dc.DataType.Name}) -> {val}");
                                    DXService.Log(ex);
                                }
                            }
                        }
                        dtWrite.Rows.Add(drWrite.ItemArray);

                        //分批写入
                        if (dtWrite.Rows.Count >= DataKitTo.BatchMaxRows)
                        {
                            batchIndex++;
                            batchRows += dtWrite.Rows.Count;
                            vm.PartTime();
                            DXService.Log($"导入表（{sntn}）分片：{batchIndex}（分片行：{dtWrite.Rows.Count}, 总行：{batchRows}）");

                            dk.db.BulkCopy(idb.WriteConnectionInfo.ConnectionType, dtWrite, isCopy);
                            dtWrite.Clear();

                            DXService.Log($"导入表（{sntn}）分片成功，耗时：{vm.PartTimeFormat()}\n");
                        }
                    }

                    //写入表
                    if (dtWrite.Rows.Count > 0)
                    {
                        batchIndex++;
                        batchRows += dtWrite.Rows.Count;
                        vm.PartTime();
                        DXService.Log($"导入表（{sntn}）分片：{batchIndex}（分片行：{dtWrite.Rows.Count}, 总行：{batchRows}）");

                        dk.db.BulkCopy(idb.WriteConnectionInfo.ConnectionType, dtWrite, isCopy);
                        dtWrite.Clear();

                        DXService.Log($"导入表（{sntn}）分片成功，耗时：{vm.PartTimeFormat()}\n");
                    }
                }
                else
                {
                    DXService.Log($"表（{sntn}）不存在", ConsoleColor.Red);
                }
            }

            DXService.Log($"导入完成,总耗时：{vm.UseTimeFormat}");
        });
    }

    [Display(Name = "Generate Table Mapping", Description = "生成 表映射(读=>写)", GroupName = "\r\n")]
    public static void GenerateTableMapping()
    {
        //配置
        var ci = new ConfigInit();
        var cc = ci.DXConfig;

        var ciRead = DXService.ConsoleReadDatabase(cc, "读取库");
        var ciWrite = DXService.ConsoleReadDatabase(cc, "写入库");

        DXService.Log($"{ciRead.ConnectionType} => {ciWrite.ConnectionType}");
        DXService.Log($"正在读取表信息");

        var tableRead = DataKitTo.Init(ciRead).GetTable();
        var tableWrite = DataKitTo.Init(ciWrite).GetTable();

        DXService.Log($"读取库 {tableRead.Count} 张表，写入库 {tableWrite.Count} 张表");

        var rws = new List<DataKitTransferVM.ReadWriteItem>();

        foreach (var itemRead in tableRead)
        {
            var mo = new DataKitTransferVM.ReadWriteItem()
            {
                ReadTableName = DbHelper.SqlSNTN(itemRead.TableName, itemRead.SchemaName)
            };

            var listWrite = tableWrite.Where(x => x.TableName == itemRead.TableName).ToList();
            //相同
            if (listWrite.Count > 0)
            {
                //模式相同
                var vmWrite = listWrite.FirstOrDefault(x => x.SchemaName == itemRead.SchemaName);
                if (vmWrite == null)
                {
                    vmWrite = listWrite[0];
                }
                mo.WriteTableName = DbHelper.SqlSNTN(vmWrite.TableName, vmWrite.SchemaName);
                DXService.Log($"{cc.MapingMatchPattern} Mapping {mo.ReadTableName} => {mo.WriteTableName}");
            }
            //相似
            else if (cc.MapingMatchPattern == "Similar")
            {
                foreach (var itemWrite in tableWrite)
                {
                    if (DXService.SimilarMatch(itemWrite.TableName, itemRead.TableName))
                    {
                        mo.WriteTableName = DbHelper.SqlSNTN(itemWrite.TableName, itemWrite.SchemaName);
                        DXService.Log($"{cc.MapingMatchPattern} Mapping {mo.ReadTableName} => {mo.WriteTableName}");
                        break;
                    }
                }
            }

            mo.ReadDataSQL = $"SELECT * FROM {DbHelper.SqlSNTN(itemRead.TableName, itemRead.SchemaName, ciRead.ConnectionType)}";
            if (string.IsNullOrEmpty(mo.WriteTableName))
            {
                DXService.Log($"No Mapping {itemRead.TableName}");
            }
            else
            {
                mo.WriteDeleteSQL = DbHelper.SqlClearTable(ciWrite.ConnectionType, mo.WriteTableName);
            }

            rws.Add(mo);
        }

        //表映射文件名
        var MappingTableName = DXService.NewFileName("MappingTable", ".json");
        var MappingTablePath = Path.Combine(ci.DXHub, MappingTableName);

        DXService.Log($"写入表映射: {MappingTablePath}");
        FileTo.WriteText(rws.ToJson(true), MappingTablePath, false);
    }

    [Display(Name = "Generate Column Mapping", Description = "生成 列映射(读=>写)")]
    public static void GenerateColumnMapping()
    {
        //配置
        var ci = new ConfigInit();
        var cc = ci.DXConfig;

        var ciRead = DXService.ConsoleReadDatabase(cc, "读取库");
        var ciWrite = DXService.ConsoleReadDatabase(cc, "写入库");

        DXService.Log($"{ciRead.ConnectionType} => {ciWrite.ConnectionType}");

        var tableMapPath = DXService.ConsoleReadPath("表映射文件");
        var rws = File.ReadAllText(tableMapPath).DeJson<DataKitTransferVM.ReadWriteItem[]>();

        DXService.Log($"正在读取列信息");

        var columnRead = DataKitTo.Init(ciRead).GetColumn();
        var columnWrite = DataKitTo.Init(ciWrite).GetColumn();

        DXService.Log($"读取库 {columnRead.Count} 列, 写入库 {columnWrite.Count} 列");

        foreach (var rw in rws)
        {
            rw.ReadWriteColumnMap.Clear();
            var igRead = columnRead.Where(x => DbHelper.SqlEqualSNTN(rw.ReadTableName, x.TableName, x.SchemaName));
            var igWrite = columnWrite.Where(x => DbHelper.SqlEqualSNTN(rw.WriteTableName, x.TableName, x.SchemaName));

            foreach (var itemRead in igRead)
            {
                var newField = string.Empty;

                //相同
                if (igWrite.Any(x => x.ColumnName == itemRead.ColumnName))
                {
                    newField = itemRead.ColumnName;
                    DXService.Log($"Same {itemRead.TableName}.{itemRead.ColumnName}");
                }
                //相似
                else if (cc.MapingMatchPattern == "Similar")
                {
                    foreach (var itemWrite in igWrite)
                    {
                        if (DXService.SimilarMatch(itemWrite.ColumnName, itemRead.ColumnName))
                        {
                            newField = itemWrite.ColumnName;
                            DXService.Log($"{cc.MapingMatchPattern} Mapping {itemRead.TableName}.{itemRead.ColumnName}:{itemWrite.TableName}.{newField}");
                            break;
                        }
                    }
                }

                if (string.IsNullOrEmpty(newField))
                {
                    DXService.Log($"No Mapping {itemRead.TableName}.{itemRead.ColumnName}");
                }

                rw.ReadWriteColumnMap.Add(itemRead.ColumnName, newField);
            }
        }

        //更新列映射
        DXService.Log($"写入列映射: {tableMapPath}");
        FileTo.WriteText(rws.ToJson(true), tableMapPath, false);
    }

    [Display(Name = "Generate Table DDL", Description = "生成 DDL")]
    public static void GenerateTableDDL()
    {
        //配置
        var ci = new ConfigInit();
        var cc = ci.DXConfig;

        //选择库
        var cdb = DXService.ConsoleReadDatabase(cc);
        var dk = DataKitTo.Init(cdb);

        Console.Write($"表名, 逗号分隔(default: *): ");
        var tableNames = Console.ReadLine().Trim();

        var listTable = new List<DataKitTableVM>();
        var allTables = dk.GetTable();
        if (!string.IsNullOrWhiteSpace(tableNames) && tableNames.Trim() != "*")
        {
            tableNames.Split(',').ToList().ForEach(item =>
            {
                var sntn = item.Split('.');
                var tableName = sntn.Last();
                var schemaName = sntn.Length > 1 ? sntn.First() : null;

                var searchTables = allTables.Where(x => x.TableName == tableName);
                if (!searchTables.Any())
                {
                    searchTables = allTables.Where(x => x.TableName.ToLower() == tableName.ToLower());
                }
                if (!string.IsNullOrWhiteSpace(schemaName))
                {
                    var table = searchTables.FirstOrDefault(x => x.SchemaName.ToLower() == schemaName.ToLower());
                    listTable.Add(table);
                }
                else if (searchTables.Count() == 1)
                {
                    listTable.Add(searchTables.First());
                }
            });
        }
        else
        {
            listTable = allTables;
        }

        var now = DateTime.Now;
        var vm = new ResultVM();
        vm.LogEvent(le =>
        {
            DXService.Log(le.NewItems[0].ToString());
        });
        var tableDesc = "-- --------------------";
        vm.Log.Add($"-- {cdb.ConnectionType}");
        vm.Log.Add($"-- Database: {cdb.DatabaseName}");
        vm.Log.Add($"-- {DateTime.Now:yyyy/MM/dd HH:mm:ss}");
        listTable.ForEach(table =>
        {
            var sntn = (new[] { EnumTo.TypeDB.SQLServer, EnumTo.TypeDB.PostgreSQL }).Contains(cdb.ConnectionType)
            ? $"{table.SchemaName}.{table.TableName}".Trim('.') : table.TableName;
            vm.Log.Add($"\r\n{tableDesc}\r\n-- {sntn}\r\n{tableDesc}");

            var ddl = dk.GetTableDDL(table.TableName, table.SchemaName);
            vm.Log.Add(ddl);
        });

        var saveFile = Path.Combine(ci.DXHub, DXService.NewFileName("TableDDL", ".sql"));
        File.WriteAllText(saveFile, string.Join("\r\n", vm.Log));
        DXService.Log($"\r\nDone! Saved to {saveFile}");
    }

    [Display(Name = "Execute SQL", Description = "执行 SQL", GroupName = "\r\n")]
    public static void ExecuteSQL()
    {
        //配置
        var ci = new ConfigInit();
        var cc = ci.DXConfig;

        //选择库
        var cdb = DXService.ConsoleReadDatabase(cc);

        Console.Write("脚本路径或SQL: ");
        var sqlOrPath = Console.ReadLine();
        if (File.Exists(sqlOrPath))
        {
            sqlOrPath = File.ReadAllText(sqlOrPath);
        }

        //跑表
        var st = new TimingVM();

        var db = cdb.NewDbHelper();
        try
        {
            var er = db.SqlExecuteReader(sqlOrPath, openTransaction: DXService.ConsoleReadBool("开启事务"));
            DXService.Log($"执行结束，受影响行数：{er.Item2}，耗时：{st.PartTimeFormat()}");
        }
        catch (Exception ex)
        {
            DXService.Log(ex);
        }
    }

    [Display(Name = "Full Text Search", Description = "全文检索")]
    public static void FullTextSearch()
    {
        //配置
        var ci = new ConfigInit();
        var cc = ci.DXConfig;

        //选择库
        var cdb = DXService.ConsoleReadDatabase(cc);

        //选择表
        var tableMode = DXService.ConsoleReadItem("选择表", "所有表,指定表,排除表".Split(','));
        var includeTable = new List<string>(); //指定表
        var excludeTable = new List<string>(); //排除表
        if (tableMode == 2)
        {
            Console.Write("输入指定表名，多个逗号分隔: ");
            includeTable = Console.ReadLine().Split(',').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
        }
        else if (tableMode == 3)
        {
            Console.Write("输入排除表名，多个逗号分隔: ");
            excludeTable = Console.ReadLine().Split(',').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
        }

        Console.Write("输入关键词，多个逗号分隔: ");
        var keys = Console.ReadLine().Split(',').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
        DXService.Log($"输入 {keys.Count} 个关键词，开始检索数据库");

        //跑表
        var st = new TimingVM();

        var dk = DataKitTo.Init(cdb);
        var listTables = dk.GetTable();
        if (tableMode == 2)
        {
            listTables = listTables.Where(x => includeTable.Contains(x.TableName)).ToList();
        }
        else if (tableMode == 3)
        {
            listTables = listTables.Where(x => !excludeTable.Contains(x.TableName)).ToList();
        }
        DXService.Log($"找到 {listTables.Count} 张表");

        //结果输出
        var savePath = Path.Combine(ci.DXHub, DXService.NewFileName($"FullTextSearch_{cdb.ConnectionType}", ".log"));
        //找到数量
        var findCount = 0;

        for (int i = 0; i < listTables.Count; i++)
        {
            var itemTable = listTables[i];
            var sntn = DbHelper.SqlSNTN(itemTable.TableName, itemTable.SchemaName, cdb.ConnectionType);
            var sql = $"select * from {sntn}";
            DXService.Log($"开始读取: {sql} , 进度: {i + 1}/{listTables.Count}");

            //表查找结果
            var tableResult = new StringBuilder();
            var findRows = 0;
            var tableRows = 0;
            dk.db.SqlExecuteDataRow(sql, row =>
            {
                tableRows++;

                var rowContent = row.ToJson();

                //搜索关键字
                var hasKeys = new List<string>();
                foreach (var key in keys)
                {
                    if (rowContent.Contains(key))
                    {
                        hasKeys.Add(key);
                    }
                }
                if (hasKeys.Count > 0)
                {
                    tableResult.AppendLine($"{string.Join(',', hasKeys)} => {sntn} => {rowContent}\r\n");
                    findRows++;
                    findCount++;
                    DXService.Log($"已找到 {findCount} 条");
                }
            });

            //写入表结果
            var tableShow = $"表（{sntn}）检索 {tableRows} 条, 找到 {findRows} 条,\r\n";
            DXService.Log(tableShow);
            tableResult.Insert(0, tableShow);
            FileTo.WriteText(tableResult.ToString(), savePath);
        }

        var dbShow = $"总共找到 {findCount} 条, 查看详情: {savePath}\r\n";
        DXService.Log(dbShow);
        FileTo.WriteText(dbShow, savePath);

        DXService.Log($"执行结束，共耗时：{st.TotalTimeFormat()}");
    }
}