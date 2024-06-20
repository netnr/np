using MiniExcelLibs;
using System.IO.Compression;
using System.Collections.Concurrent;

namespace Netnr;

/// <summary>
/// Data
/// </summary>
public partial class MenuItemService
{
    [Display(Name = "Work", Description = "作业, 以 Work 开头", GroupName = "Data", AutoGenerateFilter = true,
        ShortName = "work [Work_Name] [Work_2]", Prompt = "work Work_Demo")]
    public static async Task DataWork()
    {
        //配置
        var ci = new ConfigInit();
        var cc = ci.DXConfig;

        var worksName = cc.Works.AsObject().Select(x => x.Key).ToList();
        var working = new List<string>();

        if (worksName.Count == 0)
        {
            ConsoleTo.LogColor("没找到作业配置");
        }
        else if (BaseTo.IsCmdArgs)
        {
            working = ConsoleXTo.CmdArgs;
        }
        else
        {
            var workIndex = ConsoleXTo.ConsoleReadItem("选择作业", worksName);
            var workName = worksName[workIndex - 1];
            working.Add(workName);
        }

        ConsoleTo.LogColor($"\r\nWorks({working.Count}) : {string.Join(" , ", working)}\r\n");
        for (int ti = 0; ti < working.Count; ti++)
        {
            var workName = working[ti];

            if (cc.Works.AsObject().ContainsKey(workName))
            {
                try
                {
                    var taskJson = cc.Works[workName].AsObject();
                    // 动作
                    var actions = taskJson.Select(p => p.Key).ToList();

                    ConsoleTo.LogColor($"--- {workName}, work-progress: {ti + 1}/{working.Count}\n");

                    for (int actionIndex = 0; actionIndex < actions.Count; actionIndex++)
                    {
                        var actionName = actions[actionIndex];
                        ConsoleTo.LogColor($"----- {actionName}，action-progress: {actionIndex + 1}/{actions.Count}\n");

                        //参数
                        var parameters = taskJson[actionName];

                        if (actionName.StartsWith("ExportDatabase", StringComparison.OrdinalIgnoreCase))
                        {
                            var mo = parameters.ToJson().DeJson<DataKitTransfer.ExportDatabase>();
                            mo.PackagePath = ConsoleXTo.ParsePathVar(mo.PackagePath);

                            //引用连接
                            if (mo.ReadConnectionInfo == null && !string.IsNullOrWhiteSpace(mo.RefReadConnectionInfo))
                            {
                                var refConn = cc.ListConnectionInfo.FirstOrDefault(x => x.ConnectionRemark == mo.RefReadConnectionInfo);
                                mo.ReadConnectionInfo = new DbKitConnectionOption().ToDeepCopy(refConn);
                                if (!string.IsNullOrWhiteSpace(mo.ReadDatabaseName))
                                {
                                    mo.ReadConnectionInfo.DatabaseName = mo.ReadDatabaseName;
                                    mo.ReadConnectionInfo.DeepCopyNewInstance = true;
                                }
                            }

                            await DataKitTo.ExportDataTable(await mo.AsExportDataTable(), le => ConsoleTo.LogColor(le.NewItems[0].ToString()));
                        }
                        else if (actionName.StartsWith("ExportDataTable", StringComparison.OrdinalIgnoreCase))
                        {
                            var mo = parameters.ToJson().DeJson<DataKitTransfer.ExportDataTable>();
                            mo.PackagePath = ConsoleXTo.ParsePathVar(mo.PackagePath);

                            //引用连接
                            if (mo.ReadConnectionInfo == null && !string.IsNullOrWhiteSpace(mo.RefReadConnectionInfo))
                            {
                                var refConn = cc.ListConnectionInfo.FirstOrDefault(x => x.ConnectionRemark == mo.RefReadConnectionInfo);
                                mo.ReadConnectionInfo = new DbKitConnectionOption().ToDeepCopy(refConn);
                                if (!string.IsNullOrWhiteSpace(mo.ReadDatabaseName))
                                {
                                    mo.ReadConnectionInfo.DatabaseName = mo.ReadDatabaseName;
                                    mo.ReadConnectionInfo.DeepCopyNewInstance = true;
                                }
                            }

                            await DataKitTo.ExportDataTable(mo, le => ConsoleTo.LogColor(le.NewItems[0].ToString()));
                        }
                        else if (actionName.StartsWith("MigrateDatabase", StringComparison.OrdinalIgnoreCase))
                        {
                            var mo = parameters.ToJson().DeJson<DataKitTransfer.MigrateDatabase>();

                            //引用连接
                            if (mo.ReadConnectionInfo == null && !string.IsNullOrWhiteSpace(mo.RefReadConnectionInfo))
                            {
                                var refConn = cc.ListConnectionInfo.FirstOrDefault(x => x.ConnectionRemark == mo.RefReadConnectionInfo);
                                mo.ReadConnectionInfo = new DbKitConnectionOption().ToDeepCopy(refConn);
                                if (!string.IsNullOrWhiteSpace(mo.ReadDatabaseName))
                                {
                                    mo.ReadConnectionInfo.DatabaseName = mo.ReadDatabaseName;
                                    mo.ReadConnectionInfo.DeepCopyNewInstance = true;
                                }
                            }
                            if (mo.WriteConnectionInfo == null && !string.IsNullOrWhiteSpace(mo.RefWriteConnectionInfo))
                            {
                                var refConn = cc.ListConnectionInfo.FirstOrDefault(x => x.ConnectionRemark == mo.RefWriteConnectionInfo);
                                mo.WriteConnectionInfo = new DbKitConnectionOption().ToDeepCopy(refConn);
                                if (!string.IsNullOrWhiteSpace(mo.WriteDatabaseName))
                                {
                                    mo.WriteConnectionInfo.DatabaseName = mo.WriteDatabaseName;
                                    mo.WriteConnectionInfo.DeepCopyNewInstance = true;
                                }
                            }

                            await DataKitTo.MigrateDataTable(await mo.AsMigrateDataTable(cc.MapingMatchPattern != "Same"), le => ConsoleTo.LogColor(le.NewItems[0].ToString()));
                        }
                        else if (actionName.StartsWith("MigrateDataTable", StringComparison.OrdinalIgnoreCase))
                        {
                            var mo = parameters.ToJson().DeJson<DataKitTransfer.MigrateDataTable>();

                            //引用连接
                            if (mo.ReadConnectionInfo == null && !string.IsNullOrWhiteSpace(mo.RefReadConnectionInfo))
                            {
                                var refConn = cc.ListConnectionInfo.FirstOrDefault(x => x.ConnectionRemark == mo.RefReadConnectionInfo);
                                mo.ReadConnectionInfo = new DbKitConnectionOption().ToDeepCopy(refConn);
                                if (!string.IsNullOrWhiteSpace(mo.ReadDatabaseName))
                                {
                                    mo.ReadConnectionInfo.DatabaseName = mo.ReadDatabaseName;
                                    mo.ReadConnectionInfo.DeepCopyNewInstance = true;
                                }
                            }
                            if (mo.WriteConnectionInfo == null && !string.IsNullOrWhiteSpace(mo.RefWriteConnectionInfo))
                            {
                                var refConn = cc.ListConnectionInfo.FirstOrDefault(x => x.ConnectionRemark == mo.RefWriteConnectionInfo);
                                mo.WriteConnectionInfo = new DbKitConnectionOption().ToDeepCopy(refConn);
                                if (!string.IsNullOrWhiteSpace(mo.WriteDatabaseName))
                                {
                                    mo.WriteConnectionInfo.DatabaseName = mo.WriteDatabaseName;
                                    mo.WriteConnectionInfo.DeepCopyNewInstance = true;
                                }
                            }

                            await DataKitTo.MigrateDataTable(mo, le => ConsoleTo.LogColor(le.NewItems[0].ToString()));
                        }
                        else if (actionName.StartsWith("ImportDatabase", StringComparison.OrdinalIgnoreCase))
                        {
                            var mo = parameters.ToJson().DeJson<DataKitTransfer.ImportDatabase>();
                            mo.PackagePath = ConsoleXTo.ParsePathVar(mo.PackagePath);

                            //引用连接
                            if (mo.WriteConnectionInfo == null && !string.IsNullOrWhiteSpace(mo.RefWriteConnectionInfo))
                            {
                                var refConn = cc.ListConnectionInfo.FirstOrDefault(x => x.ConnectionRemark == mo.RefWriteConnectionInfo);
                                mo.WriteConnectionInfo = new DbKitConnectionOption().ToDeepCopy(refConn);
                                if (!string.IsNullOrWhiteSpace(mo.WriteDatabaseName))
                                {
                                    mo.WriteConnectionInfo.DatabaseName = mo.WriteDatabaseName;
                                    mo.WriteConnectionInfo.DeepCopyNewInstance = true;
                                }
                            }

                            await DataKitTo.ImportDatabase(mo, le => ConsoleTo.LogColor(le.NewItems[0].ToString()));
                        }
                        else if (actionName.StartsWith("SyncDatabase", StringComparison.OrdinalIgnoreCase))
                        {
                            var mo = parameters.ToJson().DeJson<DataKitTransfer.SyncDatabase>();

                            //引用连接
                            if (mo.ReadConnectionInfo == null && !string.IsNullOrWhiteSpace(mo.RefReadConnectionInfo))
                            {
                                var refConn = cc.ListConnectionInfo.FirstOrDefault(x => x.ConnectionRemark == mo.RefReadConnectionInfo);
                                mo.ReadConnectionInfo = new DbKitConnectionOption().ToDeepCopy(refConn);
                                if (!string.IsNullOrWhiteSpace(mo.ReadDatabaseName))
                                {
                                    mo.ReadConnectionInfo.DatabaseName = mo.ReadDatabaseName;
                                    mo.ReadConnectionInfo.DeepCopyNewInstance = true;
                                }
                            }
                            if (mo.WriteConnectionInfo == null && !string.IsNullOrWhiteSpace(mo.RefWriteConnectionInfo))
                            {
                                var refConn = cc.ListConnectionInfo.FirstOrDefault(x => x.ConnectionRemark == mo.RefWriteConnectionInfo);
                                mo.WriteConnectionInfo = new DbKitConnectionOption().ToDeepCopy(refConn);
                                if (!string.IsNullOrWhiteSpace(mo.WriteDatabaseName))
                                {
                                    mo.WriteConnectionInfo.DatabaseName = mo.WriteDatabaseName;
                                    mo.WriteConnectionInfo.DeepCopyNewInstance = true;
                                }
                            }

                            await DataKitTo.SyncDataTable(await mo.AsSyncDataTable(), le => ConsoleTo.LogColor(le.NewItems[0].ToString()));
                        }
                        else if (actionName.StartsWith("SyncDataTable", StringComparison.OrdinalIgnoreCase))
                        {
                            var mo = parameters.ToJson().DeJson<DataKitTransfer.SyncDataTable>();

                            //引用连接
                            if (mo.ReadConnectionInfo == null && !string.IsNullOrWhiteSpace(mo.RefReadConnectionInfo))
                            {
                                var refConn = cc.ListConnectionInfo.FirstOrDefault(x => x.ConnectionRemark == mo.RefReadConnectionInfo);
                                mo.ReadConnectionInfo = new DbKitConnectionOption().ToDeepCopy(refConn);
                                if (!string.IsNullOrWhiteSpace(mo.ReadDatabaseName))
                                {
                                    mo.ReadConnectionInfo.DatabaseName = mo.ReadDatabaseName;
                                    mo.ReadConnectionInfo.DeepCopyNewInstance = true;
                                }
                            }
                            if (mo.WriteConnectionInfo == null && !string.IsNullOrWhiteSpace(mo.RefWriteConnectionInfo))
                            {
                                var refConn = cc.ListConnectionInfo.FirstOrDefault(x => x.ConnectionRemark == mo.RefWriteConnectionInfo);
                                mo.WriteConnectionInfo = new DbKitConnectionOption().ToDeepCopy(refConn);
                                if (!string.IsNullOrWhiteSpace(mo.WriteDatabaseName))
                                {
                                    mo.WriteConnectionInfo.DatabaseName = mo.WriteDatabaseName;
                                    mo.WriteConnectionInfo.DeepCopyNewInstance = true;
                                }
                            }

                            await DataKitTo.SyncDataTable(mo, le => ConsoleTo.LogColor(le.NewItems[0].ToString()));
                        }
                        else
                        {
                            ConsoleTo.LogColor($"Not support {actionName}\n", ConsoleColor.Red);
                        }
                    }

                    ConsoleTo.LogColor($"\r\nDone {workName}\r\n");
                }
                catch (Exception ex)
                {
                    ConsoleTo.LogError(ex, nameof(DataWork));
                }
            }
            else
            {
                ConsoleTo.LogColor($"invalid: {workName}", ConsoleColor.Red);
            }
        }

        ConsoleTo.LogColor($"\r\nDone Works\r\n");
    }

    [Display(Name = "Migrate Data", Description = "迁移数据", GroupName = "Data", AutoGenerateFilter = true)]
    public static async Task MigrateData()
    {
        //配置
        var ci = new ConfigInit();
        var cc = ci.DXConfig;

        ConsoleTo.LogColor("\r\n注意：参数模式（静默执行）可配置多表、指定列映射\r\n", ConsoleColor.Cyan);

        var mdt = new DataKitTransfer.MigrateDataTable
        {
            ReadConnectionInfo = await ConsoleXTo.ConsoleReadDatabase(cc, "读取库"),
            WriteConnectionInfo = await ConsoleXTo.ConsoleReadDatabase(cc, "写入库")
        };
        ConsoleXTo.ViewConnectionOption(mdt.ReadConnectionInfo, mdt.WriteConnectionInfo);

        if (ConsoleXTo.ConsoleReadBool("数据库全表"))
        {
            var mdb = new DataKitTransfer.MigrateDatabase
            {
                ReadConnectionInfo = mdt.ReadConnectionInfo,
                WriteConnectionInfo = mdt.WriteConnectionInfo,
                ListIgnoreTableName = ConsoleXTo.ConsoleReadJoin<string>("忽略表名，多个逗号分隔"),
                StartTableName = ConsoleXTo.ConsoleReadString("指定读取源开始表名"),
                WriteDeleteData = ConsoleXTo.ConsoleReadBool("写入前清空表数据")
            };
            mdt = await mdb.AsMigrateDataTable(cc.MapingMatchPattern != "Same");
        }
        else
        {
            var rwi = new DataKitTransfer.ReadWriteItem();

            Console.Write("读取表数据 SQL: ");
            rwi.ReadDataSQL = Console.ReadLine();

            Console.Write("写入表名: ");
            rwi.WriteTableName = Console.ReadLine();

            var clearTableSql = DbKitExtensions.SqlClearTable(rwi.WriteTableName, mdt.WriteConnectionInfo.ConnectionType);
            Console.Write($"清空写入表(可选, {clearTableSql}): ");
            rwi.WriteDeleteSQL = Console.ReadLine();

            mdt.ListReadWrite = [rwi];
        }

        var vm = new ResultVM();
        await ConsoleXTo.TryAgain(async () =>
        {
            vm = await DataKitTo.MigrateDataTable(mdt, le => ConsoleTo.LogColor(le.NewItems[0].ToString()));
        });
    }

    [Display(Name = "Export Data", Description = "导出数据", GroupName = "Data")]
    public static async Task ExportData()
    {
        //配置
        var ci = new ConfigInit();
        var cc = ci.DXConfig;

        var edt = new DataKitTransfer.ExportDataTable()
        {
            ReadConnectionInfo = await ConsoleXTo.ConsoleReadDatabase(cc)
        };

        var etypeIndex = ConsoleXTo.ConsoleReadItem("选择导出类型", "仅表数据,结构及数据".Split(','), 2);

        if (ConsoleXTo.ConsoleReadBool("数据库全表"))
        {
            var edb = new DataKitTransfer.ExportDatabase()
            {
                ReadConnectionInfo = edt.ReadConnectionInfo,
                ListIgnoreTableName = ConsoleXTo.ConsoleReadJoin<string>("忽略表名，多个逗号分隔")
            };
            ConsoleTo.LogColor("读取表脚本构建");
            edt = await edb.AsExportDataTable();
        }
        else
        {
            //读取表
            edt.ListReadDataSQL = ConsoleXTo.ConsoleReadJoin<string>("读取表(SELECT * FROM dbo.Table1; SELECT * FROM Table2)", ";");
        }

        edt.ExportType = "dataOnly,all".Split(',')[etypeIndex - 1];
        edt.PackagePath = Path.Combine(ci.DXHub, ConsoleXTo.NewFileName(edt.ReadConnectionInfo.ConnectionType, ".zip"));
        edt.PackagePath = ConsoleXTo.ConsoleReadPath("导出完整路径", 1, edt.PackagePath, false);

        var vm = new ResultVM();
        await ConsoleXTo.TryAgain(async () =>
        {
            vm = await DataKitTo.ExportDataTable(edt, le => ConsoleTo.LogColor($"{le.NewItems[0]}"));
        });
    }

    [Display(Name = "Import Data", Description = "导入数据", GroupName = "Data")]
    public static async Task ImportData()
    {
        //配置
        var ci = new ConfigInit();
        var cc = ci.DXConfig;

        var idb = new DataKitTransfer.ImportDatabase()
        {
            WriteConnectionInfo = await ConsoleXTo.ConsoleReadDatabase(cc),
            PackagePath = ConsoleXTo.ConsoleReadPath("导入包(zip)", 1)
        };
        //没有 sql 文件，提示是否清空数据
        using var zipRead = ZipFile.OpenRead(idb.PackagePath);
        if (!zipRead.Entries.Any(x => x.Name.EndsWith(".sql")))
        {
            idb.WriteDeleteData = ConsoleXTo.ConsoleReadBool("写入前清空表数据");
        }

        var vm = new ResultVM();
        await ConsoleXTo.TryAgain(async () =>
        {
            vm = await DataKitTo.ImportDatabase(idb, cce =>
            {
                ConsoleTo.LogColor($"{cce.NewItems[0]}");
            });
        });
    }

    [Display(Name = "Export Excel", Description = "导出 Excel", GroupName = "Data")]
    public static async Task ExportExcel()
    {
        //配置
        var ci = new ConfigInit();
        var cc = ci.DXConfig;

        var edt = new DataKitTransfer.ExportDataTable()
        {
            ReadConnectionInfo = await ConsoleXTo.ConsoleReadDatabase(cc)
        };

        if (ConsoleXTo.ConsoleReadBool("数据库全表"))
        {
            var edb = new DataKitTransfer.ExportDatabase()
            {
                ReadConnectionInfo = edt.ReadConnectionInfo,
                ListIgnoreTableName = ConsoleXTo.ConsoleReadJoin<string>("忽略表名，多个逗号分隔")
            };
            edt = await edb.AsExportDataTable();
        }
        else
        {
            //读取表
            edt.ListReadDataSQL = ConsoleXTo.ConsoleReadJoin<string>("读取表(SELECT * FROM dbo.Table1; SELECT * FROM Table2)", ";");
        }

        edt.PackagePath = Path.Combine(ci.DXHub, ConsoleXTo.NewFileName(edt.ReadConnectionInfo.ConnectionType, ".xlsx"));
        edt.PackagePath = ConsoleXTo.ConsoleReadPath("导出完整路径", 1, edt.PackagePath, false);

        await ConsoleXTo.TryAgain(async () =>
        {
            edt.ReadConnectionInfo.AutoClose = false;
            var dbKit = edt.ReadConnectionInfo.CreateDbInstance();
            await dbKit.Open();

            var sheets = new Dictionary<string, object>();
            for (int i = 0; i < edt.ListReadDataSQL.Count; i++)
            {
                var sql = edt.ListReadDataSQL[i];

                var cmdOption = dbKit.CommandCreate(sql);
                var reader = await cmdOption.Command.ExecuteReaderAsync(CommandBehavior.KeyInfo);

                var schemaModel = await reader.ReaderSchemaAsync();
                var sntn = DbKitExtensions.SqlSNTN(schemaModel.Table.TableName, schemaModel.Table.Namespace);
                if (sheets.ContainsKey(sntn))
                {
                    sntn = $"Sheet{i + 1}";
                }

                sheets.Add(sntn, reader);
            }

            var sw = Stopwatch.StartNew();
            ConsoleTo.LogColor("正在导出 ...");
            MiniExcel.SaveAs(edt.PackagePath, sheets, overwriteFile: true);
            ConsoleTo.LogColor($"导出完成，耗时：{sw.Elapsed}");

            await dbKit.Close();
        });
    }

    [Display(Name = "Import Excel", Description = "导入 Excel", GroupName = "Data", AutoGenerateFilter = true)]
    public static async Task ImportExcel()
    {
        //配置
        var ci = new ConfigInit();
        var cc = ci.DXConfig;

        var idb = new DataKitTransfer.ImportDatabase()
        {
            WriteConnectionInfo = await ConsoleXTo.ConsoleReadDatabase(cc),
            PackagePath = ConsoleXTo.ConsoleReadPath("导入包(xlsx)", 1),
            WriteDeleteData = ConsoleXTo.ConsoleReadBool("写入前清空表数据")
        };

        var listTableName = ConsoleXTo.ConsoleReadJoin<string>("指定每个工作薄表名，按逗号分隔，默认工作薄名称)");

        var vm = new ResultVM();
        await ConsoleXTo.TryAgain(async () =>
        {
            idb.WriteConnectionInfo.DeepCopyNewInstance = true;
            var dataKit = DataKitTo.CreateDataKitInstance(idb.WriteConnectionInfo);
            //写入-批量
            var dbKitBulk = idb.WriteConnectionInfo.CreateDbInstance();
            dbKitBulk.ConnOption.AutoClose = false;

            var tables = await dataKit.GetTable(databaseName: idb.WriteDatabaseName);

            var sheetNames = MiniExcel.GetSheetNames(idb.PackagePath);
            for (int i = 0; i < sheetNames.Count; i++)
            {
                var sheetName = sheetNames[i];
                var sntn = i < listTableName.Count ? listTableName[i] : sheetName;
                var listSNTN = sntn.Split('.');
                string sn = listSNTN.Length > 1 ? listSNTN[0] : null;
                string tn = listSNTN.Length > 1 ? listSNTN[1] : sntn;

                //检测表名是否存在
                var table = tables.FirstOrDefault(x => x.TableName == tn && (sn == null || x.SchemaName == sn))
                ?? tables.FirstOrDefault(x => x.TableName == tn);

                if (table != null)
                {
                    sntn = DbKitExtensions.SqlSNTN(table.TableName, table.SchemaName, idb.WriteConnectionInfo.ConnectionType);

                    //空表结构
                    var sqlEmpty = DbKitExtensions.SqlEmpty(sntn, idb.WriteConnectionInfo.ConnectionType);
                    var dtWrite = (await dataKit.DbInstance.SqlExecuteDataSet(sqlEmpty)).Datas.Tables[0];
                    dtWrite.Namespace = table.SchemaName;
                    dtWrite.TableName = table.TableName;

                    //清空表
                    if (idb.WriteDeleteData)
                    {
                        vm.PartTime();
                        var clearTableSql = DbKitExtensions.SqlClearTable(sntn, idb.WriteConnectionInfo.ConnectionType);

                        ConsoleTo.LogColor($"清理写入表：{clearTableSql}");
                        var num = await dataKit.DbInstance.SqlExecuteNonQuery(clearTableSql);
                        ConsoleTo.LogColor($"返回受影响行数：{num}，耗时：{vm.PartTimeFormat()}");
                    }

                    //读取数据
                    var batchIndex = 0;
                    var batchRows = 0;
                    var rows = MiniExcel.Query(idb.PackagePath, sheetName: sheetName, useHeaderRow: true);
                    foreach (var row in rows)
                    {
                        var dictRow = (IDictionary<string, object>)row;

                        //填充行
                        var drWrite = dtWrite.NewRow();
                        foreach (DataColumn dc in dtWrite.Columns)
                        {
                            if (dictRow.TryGetValue(dc.ColumnName, out object value))
                            {
                                var val = value?.ToString();
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
                                    ConsoleTo.LogColor($"值转换失败: {dc.ColumnName}({dc.DataType.Name}) -> {val}");
                                    ConsoleTo.LogError(ex);
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
                            ConsoleTo.LogColor($"导入表（{table}）分片：{batchIndex}（分片行：{dtWrite.Rows.Count}, 总行：{batchRows}）");

                            await dbKitBulk.BulkCopy(dtWrite);
                            dtWrite.Clear();

                            ConsoleTo.LogColor($"导入表（{sntn}）分片成功，耗时：{vm.PartTimeFormat()}\n");
                        }
                    }

                    //写入表
                    if (dtWrite.Rows.Count > 0)
                    {
                        batchIndex++;
                        batchRows += dtWrite.Rows.Count;
                        vm.PartTime();
                        ConsoleTo.LogColor($"导入表（{sntn}）分片：{batchIndex}（分片行：{dtWrite.Rows.Count}, 总行：{batchRows}）");

                        await dbKitBulk.BulkCopy(dtWrite);
                        dtWrite.Clear();

                        ConsoleTo.LogColor($"导入表（{sntn}）分片成功，耗时：{vm.PartTimeFormat()}\n");
                    }
                }
                else
                {
                    ConsoleTo.LogColor($"表（{sntn}）不存在", ConsoleColor.Red);
                }
            }

            await dbKitBulk.Close();

            ConsoleTo.LogColor($"导入完成,总耗时：{vm.UseTimeFormat}");
        });
    }

    [Display(Name = "Generate Table Mapping", Description = "生成 表映射(读=>写)", GroupName = "Data")]
    public static async Task GenerateTableMapping()
    {
        //配置
        var ci = new ConfigInit();
        var cc = ci.DXConfig;

        var connOptionRead = await ConsoleXTo.ConsoleReadDatabase(cc, "读取来源");
        var connOptionWrite = await ConsoleXTo.ConsoleReadDatabase(cc, "写入目标");

        ConsoleXTo.ViewConnectionOption(connOptionRead, connOptionWrite);

        ConsoleTo.LogCard("读取表信息");

        var tableRead = await DataKitTo.CreateDataKitInstance(connOptionRead).GetTable();
        var tableWrite = await DataKitTo.CreateDataKitInstance(connOptionWrite).GetTable();

        ConsoleTo.LogColor($"读取来源 {tableRead.Count} 张表，写入目标 {tableWrite.Count} 张表");

        var rws = new List<DataKitTransfer.ReadWriteItem>();

        foreach (var itemRead in tableRead)
        {
            var mo = new DataKitTransfer.ReadWriteItem()
            {
                ReadTableName = DbKitExtensions.SqlSNTN(itemRead.TableName, itemRead.SchemaName)
            };

            var listWrite = tableWrite.Where(x => x.TableName == itemRead.TableName).ToList();
            //相同
            if (listWrite.Count > 0)
            {
                //模式相同
                var vmWrite = listWrite.FirstOrDefault(x => x.SchemaName == itemRead.SchemaName);
                vmWrite ??= listWrite[0];
                mo.WriteTableName = DbKitExtensions.SqlSNTN(vmWrite.TableName, vmWrite.SchemaName);
                ConsoleTo.LogColor($"{cc.MapingMatchPattern} Mapping {mo.ReadTableName} => {mo.WriteTableName}");
            }
            //相似
            else if (cc.MapingMatchPattern == "Similar")
            {
                foreach (var itemWrite in tableWrite)
                {
                    if (DataKitTo.SimilarMatch(itemWrite.TableName, itemRead.TableName))
                    {
                        mo.WriteTableName = DbKitExtensions.SqlSNTN(itemWrite.TableName, itemWrite.SchemaName);
                        ConsoleTo.LogColor($"{cc.MapingMatchPattern} Mapping {mo.ReadTableName} => {mo.WriteTableName}");
                        break;
                    }
                }
            }

            mo.ReadDataSQL = $"SELECT * FROM {DbKitExtensions.SqlSNTN(itemRead.TableName, itemRead.SchemaName, connOptionRead.ConnectionType)}";
            if (string.IsNullOrEmpty(mo.WriteTableName))
            {
                ConsoleTo.LogColor($"No Mapping {itemRead.TableName}");
            }
            else
            {
                mo.WriteDeleteSQL = DbKitExtensions.SqlClearTable(mo.WriteTableName, connOptionWrite.ConnectionType);
            }

            rws.Add(mo);
        }

        //表映射文件名
        var MappingTableName = ConsoleXTo.NewFileName("MappingTable", ".json");
        var MappingTablePath = Path.Combine(ci.DXHub, MappingTableName);

        ConsoleTo.LogColor($"写入表映射: {MappingTablePath}", ConsoleColor.Cyan);
        FileTo.WriteText(rws.ToJson(true), MappingTablePath, false);
    }

    [Display(Name = "Generate Column Mapping", Description = "生成 列映射(读=>写)", GroupName = "Data")]
    public static async Task GenerateColumnMapping()
    {
        //配置
        var ci = new ConfigInit();
        var cc = ci.DXConfig;

        var connOptionRead = await ConsoleXTo.ConsoleReadDatabase(cc, "读取来源");
        var connOptionWrite = await ConsoleXTo.ConsoleReadDatabase(cc, "写入目标");

        ConsoleXTo.ViewConnectionOption(connOptionRead, connOptionWrite);

        var tableMapPath = ConsoleXTo.ConsoleReadPath("表映射文件");
        var rws = File.ReadAllText(tableMapPath).DeJson<DataKitTransfer.ReadWriteItem[]>();

        ConsoleTo.LogCard("读取列信息");

        var columnRead = await DataKitTo.CreateDataKitInstance(connOptionRead).GetColumn();
        var columnWrite = await DataKitTo.CreateDataKitInstance(connOptionWrite).GetColumn();

        ConsoleTo.LogColor($"读取来源 {columnRead.Count} 列, 写入目标 {columnWrite.Count} 列");

        foreach (var rw in rws)
        {
            rw.ReadWriteColumnMap.Clear();
            var igRead = columnRead.Where(x => DbKitExtensions.SqlEqualSNTN(rw.ReadTableName, x.TableName, x.SchemaName));
            var igWrite = columnWrite.Where(x => DbKitExtensions.SqlEqualSNTN(rw.WriteTableName, x.TableName, x.SchemaName));

            foreach (var itemRead in igRead)
            {
                var newField = string.Empty;

                //相同
                if (igWrite.Any(x => x.ColumnName == itemRead.ColumnName))
                {
                    newField = itemRead.ColumnName;
                    ConsoleTo.LogColor($"Same {itemRead.TableName}.{itemRead.ColumnName}");
                }
                //相似
                else if (cc.MapingMatchPattern == "Similar")
                {
                    foreach (var itemWrite in igWrite)
                    {
                        if (DataKitTo.SimilarMatch(itemWrite.ColumnName, itemRead.ColumnName))
                        {
                            newField = itemWrite.ColumnName;
                            ConsoleTo.LogColor($"{cc.MapingMatchPattern} Mapping {itemRead.TableName}.{itemRead.ColumnName}:{itemWrite.TableName}.{newField}");
                            break;
                        }
                    }
                }

                if (string.IsNullOrEmpty(newField))
                {
                    ConsoleTo.LogColor($"No Mapping {itemRead.TableName}.{itemRead.ColumnName}");
                }

                rw.ReadWriteColumnMap.Add(itemRead.ColumnName, newField);
            }
        }

        //更新列映射
        ConsoleTo.LogColor($"写入列映射: {tableMapPath}", ConsoleColor.Cyan);
        FileTo.WriteText(rws.ToJson(true), tableMapPath, false);
    }

    [Display(Name = "Generate Table DDL", Description = "生成 DDL", GroupName = "Data", AutoGenerateFilter = true)]
    public static async Task GenerateTableDDL()
    {
        //配置
        var ci = new ConfigInit();
        var cc = ci.DXConfig;

        //选择库
        var connOption = await ConsoleXTo.ConsoleReadDatabase(cc);
        connOption.AutoClose = false;
        var dataKit = DataKitTo.CreateDataKitInstance(connOption);

        var tableNames = ConsoleXTo.ConsoleReadJoin<string>("指定表名，多个逗号分隔，默认全表");

        var listTable = new List<DataKitTableResult>();
        var allTables = await dataKit.GetTable();
        if (tableNames.Count > 0)
        {
            tableNames.ForEach(item =>
            {
                var sntn = item.Split('.');
                var tableName = sntn.Last();
                var schemaName = sntn.Length > 1 ? sntn.First() : null;

                var searchTables = allTables.Where(x => x.TableName == tableName);
                if (!searchTables.Any())
                {
                    searchTables = allTables.Where(x => x.TableName.Equals(tableName, StringComparison.OrdinalIgnoreCase));
                }
                if (!string.IsNullOrWhiteSpace(schemaName))
                {
                    var table = searchTables.FirstOrDefault(x => x.SchemaName.Equals(schemaName, StringComparison.OrdinalIgnoreCase));
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

        var vm = new ResultVM();
        vm.LogEvent(le =>
        {
            ConsoleTo.LogColor(le.NewItems[0].ToString());
        });
        var tableDesc = "-- --------------------";
        vm.Log.Add($"-- {connOption.ConnectionType}");
        vm.Log.Add($"-- Database: {connOption.DatabaseName}");
        vm.Log.Add($"-- {DateTime.Now:yyyy/MM/dd HH:mm:ss}");
        foreach (var table in listTable)
        {
            var sntn = (new[] { DBTypes.SQLServer, DBTypes.PostgreSQL }).Contains(connOption.ConnectionType)
            ? $"{table.SchemaName}.{table.TableName}".Trim('.') : table.TableName;
            vm.Log.Add("");
            vm.Log.Add(tableDesc);
            vm.Log.Add($"-- {sntn}");
            vm.Log.Add(tableDesc);

            var ddl = await dataKit.GetTableDDL(table.TableName, table.SchemaName);
            vm.Log.Add(ddl);
        }
        await dataKit.Close();

        var saveFile = Path.Combine(ci.DXHub, ConsoleXTo.NewFileName("TableDDL", ".sql"));
        File.WriteAllText(saveFile, string.Join("\r\n", vm.Log));
        ConsoleTo.LogColor($"\r\nDone! Saved to {saveFile}", ConsoleColor.Cyan);
    }

    [Display(Name = "Connection Test", Description = "连接测试", GroupName = "Data",
        ShortName = "conntest", Prompt = "ndx conntest")]
    public static async Task ConnectionTest()
    {
        //配置
        var ci = new ConfigInit();
        var cc = ci.DXConfig;

        if (cc.ListConnectionInfo.Count > 0)
        {
            var listQueue = new ConcurrentQueue<ValueTuple<DbKitConnectionOption, bool, string, string>>();
            var queueCount = 0;
            var badCount = 0;
            var thread = new Thread(() =>
            {
                do
                {
                    if (listQueue.TryDequeue(out var model))
                    {
                        queueCount++;
                        var color = model.Item2 ? ConsoleColor.Green : ConsoleColor.Red;
                        ConsoleTo.LogColor("");
                        ConsoleTo.LogColor(model.Item1.GetSafeConnectionOption(), color);
                        ConsoleTo.LogColor(model.Item2 ? $"Info: {model.Item3}" : model.Item3);
                        ConsoleTo.LogColor($"Drive: {model.Item4}");

                        if (queueCount == cc.ListConnectionInfo.Count)
                        {
                            ConsoleTo.LogColor($"\r\nDone! connected: {cc.ListConnectionInfo.Count - badCount}, failed: {badCount}", ConsoleColor.Cyan);
                        }
                    }
                    Thread.Sleep(100);
                } while (queueCount < cc.ListConnectionInfo.Count);
            });
            thread.Start();

            var listTask = cc.ListConnectionInfo.Select(connOption => Task.Run(async () =>
            {
                var isOk = false;
                var dbKit = connOption.CreateDbInstance();
                var result = string.Empty;
                try
                {
                    await dbKit.Open();
                    isOk = true;

                    var queryInfo = string.Empty;
                    switch (connOption.ConnectionType)
                    {
                        case DBTypes.MySQL:
                        case DBTypes.MariaDB:
                            queryInfo = "select concat(@@version_comment,' - ',@@version)";
                            break;
                        case DBTypes.Oracle:
                            queryInfo = "SELECT * FROM V$VERSION WHERE BANNER LIKE 'Oracle %'";
                            break;
                        case DBTypes.SQLServer:
                            queryInfo = "select @@VERSION";
                            break;
                        case DBTypes.PostgreSQL:
                            queryInfo = "select VERSION()";
                            break;
                        case DBTypes.ClickHouse:
                            queryInfo = "select value from system.build_options where name = 'VERSION_FULL'";
                            break;
                    }
                    if (string.IsNullOrWhiteSpace(queryInfo))
                    {
                        result = dbKit.ConnOption.Connection.ServerVersion;
                    }
                    else
                    {
                        var versionInfo = await dbKit.SqlExecuteScalar(queryInfo);
                        result = versionInfo.ToString();
                    }

                    await dbKit.Close();
                }
                catch (Exception ex)
                {
                    result = ex.ToJson(true);
                }

                //加入结果队列
                listQueue.Enqueue(new(connOption, isOk, result, dbKit.ConnOption.Connection.GetType().Assembly.FullName));
            }));
            await Task.WhenAll(listTask);
        }
        else
        {
            ConsoleTo.LogColor($"Not Found", ConsoleColor.Red);
        }
    }

    [Display(Name = "Parameter Optimization (SQLite MySQL)", Description = "参数优化", GroupName = "Data")]
    public static async Task ParameterOptimization()
    {
        //配置
        var ci = new ConfigInit();
        var cc = ci.DXConfig;

        //选择库
        var connOption = await ConsoleXTo.ConsoleReadDatabase(cc);
        var dbKit = connOption.CreateDbInstance();

        switch (connOption.ConnectionType)
        {
            case DBTypes.SQLite:
                {
                    ConsoleTo.LogColor($"磁盘空间释放：VACUUM");
                    await dbKit.SqlExecuteNonQuery("VACUUM");
                }
                break;
            case DBTypes.MySQL:
            case DBTypes.MariaDB:
                {
                    if (await dbKit.PreBulkCopy() == 0)
                    {
                        ConsoleTo.LogColor($"\n没有需要优化的参数", ConsoleColor.Cyan);
                    }
                }
                break;
            case DBTypes.Oracle:
                break;
            case DBTypes.SQLServer:
                break;
            case DBTypes.PostgreSQL:
                break;
        }
    }

    [Display(Name = "Execute SQL", Description = "执行 SQL", GroupName = "Data")]
    public static async Task ExecuteNonQuery()
    {
        //配置
        var ci = new ConfigInit();
        var cc = ci.DXConfig;

        //选择库
        var connOption = await ConsoleXTo.ConsoleReadDatabase(cc);

        Console.Write("脚本路径或SQL: ");
        var sqlOrPath = Console.ReadLine();
        if (File.Exists(sqlOrPath))
        {
            sqlOrPath = File.ReadAllText(sqlOrPath);
        }

        //跑表
        var sw = Stopwatch.StartNew();

        var dbKit = connOption.CreateDbInstance();
        try
        {
            var num = await dbKit.SqlExecuteNonQuery(sqlOrPath, cmdCall: async cmdOption =>
            {
                if (ConsoleXTo.ConsoleReadBool("开启事务"))
                {
                    await cmdOption.OpenTransactionAsync();
                }
            });
            ConsoleTo.LogColor($"执行结束，受影响行数：{num}，耗时：{sw.Elapsed}");
        }
        catch (Exception ex)
        {
            ConsoleTo.LogError(ex);
        }
    }

    [Display(Name = "Full Text Search", Description = "全文检索", GroupName = "Data", AutoGenerateFilter = true)]
    public static async Task FullTextSearch()
    {
        //配置
        var ci = new ConfigInit();
        var cc = ci.DXConfig;

        //选择连接
        var connOption = await ConsoleXTo.ConsoleReadDatabase(cc, isSelectDatabase: false);

        //选择库
        var includeDatabase = new List<string>();
        if (connOption.ConnectionType != DBTypes.Oracle)
        {
            do
            {
                var dataKit = DataKitTo.CreateDataKitInstance(connOption);
                var listDatabaseName = await dataKit.GetDatabaseNameOnly();

                Console.WriteLine("");
                listDatabaseName.ForEach(Console.WriteLine);
                Console.WriteLine("");

                includeDatabase = ConsoleXTo.ConsoleReadJoin<string>("指定库名，多个逗号分隔");

                //有效库名
                var validDatabase = includeDatabase.Intersect(listDatabaseName).ToList();
                if (validDatabase.Count != includeDatabase.Count)
                {
                    ConsoleTo.LogColor($"有效库名 {validDatabase.Count} 个\r\n", ConsoleColor.Green);
                    includeDatabase = validDatabase;
                }
            } while (includeDatabase.Count == 0);
        }
        else
        {
            includeDatabase.Add(connOption.DatabaseName);
        }

        //选择表
        var tableMode = ConsoleXTo.ConsoleReadItem("选择表", "所有表,指定表,排除表".Split(','));
        var includeTable = new List<string>(); //指定表
        var excludeTable = new List<string>(); //排除表
        if (tableMode == 2)
        {
            includeTable = ConsoleXTo.ConsoleReadJoin<string>("指定表名，多个逗号分隔");
        }
        else if (tableMode == 3)
        {
            excludeTable = ConsoleXTo.ConsoleReadJoin<string>("排除表名，多个逗号分隔");
        }

        var keys = ConsoleXTo.ConsoleReadJoin<string>("关键词，多个逗号分隔");
        if (keys.Count > 0)
        {
            foreach (var dbName in includeDatabase)
            {
                try
                {
                    //跑表
                    var sw = Stopwatch.StartNew();

                    ConsoleTo.LogCard($"开始检索库 {dbName}");
                    if (connOption.ConnectionType != DBTypes.Oracle)
                    {
                        connOption.DatabaseName = dbName;
                        connOption.SetConnDatabaseName(dbName);
                    }

                    var dataKit = DataKitTo.CreateDataKitInstance(connOption);
                    dataKit.DbInstance.ConnOption.AutoClose = false;
                    var listTables = await dataKit.GetTable();
                    if (tableMode == 2)
                    {
                        listTables = listTables.Where(x => includeTable.Any(y =>
                        DbKitExtensions.SqlEqualSNTN(y, DbKitExtensions.SqlSNTN(x.TableName, x.SchemaName, connOption.ConnectionType))
                        )).ToList();
                    }
                    else if (tableMode == 3)
                    {
                        listTables = listTables.Where(x => !excludeTable.Any(y =>
                        DbKitExtensions.SqlEqualSNTN(y, DbKitExtensions.SqlSNTN(x.TableName, x.SchemaName, connOption.ConnectionType))
                        )).ToList();
                    }
                    ConsoleTo.LogCard($"该库共计表 {listTables.Count} 张");
                    if (listTables.Count == 0)
                    {
                        continue;
                    }

                    //结果输出
                    var savePath = Path.Combine(ci.DXHub, ConsoleXTo.NewFileName($"{nameof(FullTextSearch)}_{connOption.ConnectionType}_{connOption.DatabaseName}", ".sql"));
                    //找到数量
                    var findCount = 0;

                    for (int i = 0; i < listTables.Count; i++)
                    {
                        var itemTable = listTables[i];
                        var sntn = DbKitExtensions.SqlSNTN(itemTable.TableName, itemTable.SchemaName, connOption.ConnectionType);
                        var sql = $"select * from {sntn}";
                        ConsoleTo.LogColor($"read SQL: {sql} , progress: {i + 1}/{listTables.Count}");
                        FileTo.WriteText($"----  {sntn}", savePath);

                        //表查找结果
                        var tableResult = new StringBuilder();
                        var findRows = 0;
                        var tableRows = 0;
                        DataTable tableSchema = null;
                        var tablePrimaryKey = new HashSet<string>();
                        await dataKit.DbInstance.SqlExecuteDataRow(sql, readRow: row =>
                        {
                            tableRows++;

                            //搜索关键字
                            string rowResultSQL;
                            var rowResultWhere = new Dictionary<string, string>();
                            var rowResultColumns = new HashSet<string>();
                            var rowResultColumnKeyContext = new HashSet<ValueTuple<string, string, string>>();

                            for (int i = 0; i < tableSchema.Columns.Count; i++)
                            {
                                var col = tableSchema.Columns[i];
                                if (tablePrimaryKey.Contains(col.ColumnName))
                                {
                                    rowResultWhere[col.ColumnName] = row[i]?.ToString();
                                }

                                if (col.DataType == typeof(string))
                                {
                                    var val = row[i]?.ToString();
                                    if (!string.IsNullOrWhiteSpace(val))
                                    {
                                        foreach (var key in keys)
                                        {
                                            int searchIndex = -1;
                                            do
                                            {
                                                searchIndex = val.IndexOf(key, searchIndex + 1, StringComparison.OrdinalIgnoreCase);
                                                if (searchIndex >= 0)
                                                {
                                                    rowResultColumns.Add(col.ColumnName);

                                                    var truncationLength = 30;
                                                    int startIndex = Math.Max(0, searchIndex - truncationLength);
                                                    int endIndex = Math.Min(val.Length, searchIndex + key.Length + truncationLength);
                                                    var truncationContext = val[startIndex..endIndex].Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ");
                                                    rowResultColumnKeyContext.Add(new(col.ColumnName, key, truncationContext));
                                                }
                                            } while (searchIndex >= 0);
                                        }
                                    }
                                }
                            }

                            if (rowResultColumns.Count > 0)
                            {
                                rowResultSQL = $"select {string.Join(',', tablePrimaryKey.Concat(rowResultColumns).Distinct())} from {sntn} where {string.Join(" and ", rowResultWhere.Select(x => $"{x.Key}='{x.Value}'"))}";
                                tableResult.AppendLine($"\r\n{rowResultSQL}");
                                rowResultColumnKeyContext.ForEach(x =>
                                {
                                    tableResult.AppendLine($"-- column-key-context: {x.Item1} - {x.Item2} - {x.Item3}");
                                });

                                findRows++;
                                findCount++;
                                if (findRows <= 99)
                                {
                                    ConsoleTo.LogColor($"已找到 {findRows} 条");
                                }
                                else if (findRows == 100)
                                {
                                    ConsoleTo.LogColor($"已找到 99+ 条");
                                }

                                //1M
                                if (tableResult.Length > 1048576)
                                {
                                    FileTo.WriteText(tableResult.ToString(), savePath);
                                    tableResult.Clear();
                                }
                            }

                            return Task.CompletedTask;
                        }, schemaResult: schemaModel =>
                        {
                            tableSchema = schemaModel.Table.Clone();

                            //取主键列或前两列为查询条件
                            if (schemaModel.KeyColumns.Count > 0)
                            {
                                schemaModel.KeyColumns.ForEach(x =>
                                {
                                    tablePrimaryKey.Add(x.ColumnName);
                                });
                            }
                            else
                            {
                                for (int i = 0; i < tableSchema.Columns.Count; i++)
                                {
                                    if (i < 3)
                                    {
                                        tablePrimaryKey.Add(tableSchema.Columns[i].ColumnName);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }

                            return Task.CompletedTask;
                        });

                        //写入表结果
                        var tableShow = $"----  done  {sntn} {tableRows} rows , found {findRows}\r\n";
                        ConsoleTo.LogColor(tableShow);
                        tableResult.AppendLine(tableShow);
                        FileTo.WriteText(tableResult.ToString(), savePath);
                    }
                    await dataKit.Close();

                    var dbShow = $"----  总共找到 {findCount} 条, 详情: {savePath}\r\n";
                    ConsoleTo.LogColor(dbShow);
                    FileTo.WriteText(dbShow, savePath);

                    sw.Stop();
                    ConsoleTo.LogColor($"执行结束，共耗时：{sw.Elapsed}");
                }
                catch (Exception ex)
                {
                    ConsoleTo.LogError(ex);
                }
            }
        }
    }

    [Display(Name = "Generate CreateTable In ClickHouse", Description = "生成创建表", GroupName = "Data")]
    public static async Task GenerateCreateTableInClickHouse()
    {
        var ci = new ConfigInit();
        var cc = ci.DXConfig;

        var connOption = await ConsoleXTo.ConsoleReadDatabase(cc, "读取库");
        Console.Write("表名，多个逗号分隔: ");
        var readTableName = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(readTableName))
        {
            var listReadSQL = readTableName.Split(',').Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => $"SELECT * FROM {x} WHERE 1=2");

            Console.Write("表名加前缀: ");
            var tableNamePrefix = Console.ReadLine().Trim();

            List<string> caseItem = ["LowerCase", "Same", "UpperCase"];
            var caseIndex = ConsoleXTo.ConsoleReadItem("转换大小写", caseItem, 2);
            var lowerCase = caseItem[caseIndex - 1];

            var allowDBNull = ConsoleXTo.ConsoleReadBool("允许列值为 NULL");

            var dbKit = connOption.CreateDbInstance();
            var eds = await dbKit.SqlExecuteDataSet(string.Join(';', listReadSQL));

            for (int i = 0; i < eds.Datas.Tables.Count; i++)
            {
                var dt = eds.Datas.Tables[i];
                var schema = eds.Schemas.Tables[i];

                var tableName = dt.TableName;
                if (schema.Columns.Contains("BaseTableName"))
                {
                    tableName = schema.Rows[0]["BaseTableName"].ToString();
                }
                tableName = DbKitExtensions.CaseMapping(tableName, lowerCase);
                tableName = $"{tableNamePrefix}{tableName}";

                var createSQL = DataKitTo.ToClickHouseCreateSQL(dt, tableName, lowerCase: lowerCase, allowDBNull: allowDBNull);
                ConsoleTo.LogCard($"Create table {tableName}", createSQL);
            }
        }
    }
}