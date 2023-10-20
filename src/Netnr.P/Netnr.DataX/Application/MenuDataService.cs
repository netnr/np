using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using MiniExcelLibs;
using System.Collections.Concurrent;
using System.IO.Compression;
using System.Threading.Tasks.Dataflow;

namespace Netnr.DataX.Application;

/// <summary>
/// Data
/// </summary>
public partial class MenuItemService
{
    [Display(Name = "Work", Description = "作业, 以 Work_ 开头", GroupName = "Data", AutoGenerateFilter = true,
        ShortName = "work [Work_Name] [Work_2]", Prompt = "ndx task Task_Demo")]
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
            working = DXService.CmdArgs;
        }
        else
        {
            var workIndex = DXService.ConsoleReadItem("选择作业", worksName);
            var workName = worksName[workIndex - 1];
            working.Add(workName);
        }

        ConsoleTo.LogColor($"\r\n开始作业({working.Count} 个): {string.Join(" ", working)}\r\n");
        for (int ti = 0; ti < working.Count; ti++)
        {
            var workName = working[ti];

            if (cc.Works.AsObject().ContainsKey(workName))
            {
                try
                {
                    var taskJson = cc.Works[workName].AsObject();
                    //方法
                    var methods = taskJson.Select(p => p.Key).ToList();

                    ConsoleTo.LogColor($"开始 {workName} 作业，进度 {ti + 1}/{working.Count}\n");

                    for (int mi = 0; mi < methods.Count; mi++)
                    {
                        var methodName = methods[mi];
                        ConsoleTo.LogColor($"开始 {methodName} 方法，进度 {mi + 1}/{methods.Count}\n");

                        //参数
                        var parameters = taskJson[methodName];

                        switch (methodName)
                        {
                            case "ExportDatabase":
                                {
                                    var mo = parameters.ToJson().DeJson<DataKitTransfer.ExportDatabase>();
                                    mo.PackagePath = DXService.ParsePathVar(mo.PackagePath);

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
                                break;
                            case "ExportDataTable":
                                {
                                    var mo = parameters.ToJson().DeJson<DataKitTransfer.ExportDataTable>();
                                    mo.PackagePath = DXService.ParsePathVar(mo.PackagePath);

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
                                break;
                            case "MigrateDatabase":
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
                                break;
                            case "MigrateDataTable":
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
                                break;
                            case "ImportDatabase":
                                {
                                    var mo = parameters.ToJson().DeJson<DataKitTransfer.ImportDatabase>();
                                    mo.PackagePath = DXService.ParsePathVar(mo.PackagePath);

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
                                break;
                            default:
                                ConsoleTo.LogColor($"Not support {methodName}\n", ConsoleColor.Red);
                                break;
                        }
                    }

                    ConsoleTo.LogColor($"\r\n完成 {workName} 作业\r\n");
                }
                catch (Exception ex)
                {
                    ConsoleTo.LogError(ex, $"{nameof(DataWork)} 作业出错");
                }
            }
            else
            {
                ConsoleTo.LogColor($"无效作业（{workName}）", ConsoleColor.Red);
            }
        }

        ConsoleTo.LogColor($"\r\n作业全部完成\r\n");
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
            ReadConnectionInfo = await DXService.ConsoleReadDatabase(cc, "读取库"),
            WriteConnectionInfo = await DXService.ConsoleReadDatabase(cc, "写入库")
        };
        DXService.ViewConnectionOption(mdt.ReadConnectionInfo, mdt.WriteConnectionInfo);

        if (DXService.ConsoleReadBool("数据库全表"))
        {
            var mdb = new DataKitTransfer.MigrateDatabase
            {
                ReadConnectionInfo = mdt.ReadConnectionInfo,
                WriteConnectionInfo = mdt.WriteConnectionInfo,
                ListIgnoreTableName = DXService.ConsoleReadJoin<string>("忽略表名，多个逗号分隔"),
                WriteDeleteData = DXService.ConsoleReadBool("写入前清空表数据")
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

            mdt.ListReadWrite = new List<DataKitTransfer.ReadWriteItem>() { rwi };
        }

        var vm = new ResultVM();
        await DXService.TryAgain(async () =>
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
            ReadConnectionInfo = await DXService.ConsoleReadDatabase(cc)
        };

        var etypeIndex = DXService.ConsoleReadItem("选择导出类型", new string[] { "仅表数据", "结构及数据" }, 2);

        if (DXService.ConsoleReadBool("数据库全表"))
        {
            var edb = new DataKitTransfer.ExportDatabase()
            {
                ReadConnectionInfo = edt.ReadConnectionInfo,
                ListIgnoreTableName = DXService.ConsoleReadJoin<string>("忽略表名，多个逗号分隔")
            };
            ConsoleTo.LogColor("读取表脚本构建");
            edt = await edb.AsExportDataTable();
        }
        else
        {
            //读取表
            edt.ListReadDataSQL = DXService.ConsoleReadJoin<string>("读取表(SELECT * FROM dbo.Table1; SELECT * FROM Table2)", ";");
        }

        edt.ExportType = "dataOnly,all".Split(',')[etypeIndex - 1];
        edt.PackagePath = Path.Combine(ci.DXHub, DXService.NewFileName(edt.ReadConnectionInfo.ConnectionType, ".zip"));
        edt.PackagePath = DXService.ConsoleReadPath("导出完整路径", 1, edt.PackagePath, false);

        var vm = new ResultVM();
        await DXService.TryAgain(async () =>
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
            WriteConnectionInfo = await DXService.ConsoleReadDatabase(cc),
            PackagePath = DXService.ConsoleReadPath("导入包(zip)", 1)
        };
        //没有 sql 文件，提示是否清空数据
        using var zipRead = ZipFile.OpenRead(idb.PackagePath);
        if (!zipRead.Entries.Any(x => x.Name.EndsWith(".sql")))
        {
            idb.WriteDeleteData = DXService.ConsoleReadBool("写入前清空表数据");
        }

        var vm = new ResultVM();
        await DXService.TryAgain(async () =>
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
            ReadConnectionInfo = await DXService.ConsoleReadDatabase(cc)
        };

        if (DXService.ConsoleReadBool("数据库全表"))
        {
            var edb = new DataKitTransfer.ExportDatabase()
            {
                ReadConnectionInfo = edt.ReadConnectionInfo,
                ListIgnoreTableName = DXService.ConsoleReadJoin<string>("忽略表名，多个逗号分隔")
            };
            edt = await edb.AsExportDataTable();
        }
        else
        {
            //读取表
            edt.ListReadDataSQL = DXService.ConsoleReadJoin<string>("读取表(SELECT * FROM dbo.Table1; SELECT * FROM Table2)", ";");
        }

        edt.PackagePath = Path.Combine(ci.DXHub, DXService.NewFileName(edt.ReadConnectionInfo.ConnectionType, ".xlsx"));
        edt.PackagePath = DXService.ConsoleReadPath("导出完整路径", 1, edt.PackagePath, false);

        await DXService.TryAgain(async () =>
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
            WriteConnectionInfo = await DXService.ConsoleReadDatabase(cc),
            PackagePath = DXService.ConsoleReadPath("导入包(xlsx)", 1),
            WriteDeleteData = DXService.ConsoleReadBool("写入前清空表数据")
        };

        var listTableName = DXService.ConsoleReadJoin<string>("指定每个工作薄表名，按逗号分隔，默认工作薄名称)");

        var vm = new ResultVM();
        await DXService.TryAgain(async () =>
        {
            var dataKit = DataKitTo.CreateDataKitInstance(idb.WriteConnectionInfo);
            var tables = await dataKit.GetTable(databaseName: idb.WriteDatabaseName);

            var isCopy = await dataKit.DbInstance.PreExecute() != -1; //预检通过

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

                            await dataKit.DbInstance.BulkCopy(dtWrite, isCopy);
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

                        await dataKit.DbInstance.BulkCopy(dtWrite, isCopy);
                        dtWrite.Clear();

                        ConsoleTo.LogColor($"导入表（{sntn}）分片成功，耗时：{vm.PartTimeFormat()}\n");
                    }
                }
                else
                {
                    ConsoleTo.LogColor($"表（{sntn}）不存在", ConsoleColor.Red);
                }
            }

            ConsoleTo.LogColor($"导入完成,总耗时：{vm.UseTimeFormat}");
        });
    }

    [Display(Name = "Generate Table Mapping", Description = "生成 表映射(读=>写)", GroupName = "Data")]
    public static async Task GenerateTableMapping()
    {
        //配置
        var ci = new ConfigInit();
        var cc = ci.DXConfig;

        var connOptionRead = await DXService.ConsoleReadDatabase(cc, "读取来源");
        var connOptionWrite = await DXService.ConsoleReadDatabase(cc, "写入目标");

        DXService.ViewConnectionOption(connOptionRead, connOptionWrite);

        ConsoleTo.LogTag("读取表信息");

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
        var MappingTableName = DXService.NewFileName("MappingTable", ".json");
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

        var connOptionRead = await DXService.ConsoleReadDatabase(cc, "读取来源");
        var connOptionWrite = await DXService.ConsoleReadDatabase(cc, "写入目标");

        DXService.ViewConnectionOption(connOptionRead, connOptionWrite);

        var tableMapPath = DXService.ConsoleReadPath("表映射文件");
        var rws = File.ReadAllText(tableMapPath).DeJson<DataKitTransfer.ReadWriteItem[]>();

        ConsoleTo.LogTag("读取列信息");

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
        var connOption = await DXService.ConsoleReadDatabase(cc);
        connOption.AutoClose = false;
        var dataKit = DataKitTo.CreateDataKitInstance(connOption);

        var tableNames = DXService.ConsoleReadJoin<string>("指定表名，多个逗号分隔，默认全表");

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

        var saveFile = Path.Combine(ci.DXHub, DXService.NewFileName("TableDDL", ".sql"));
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
        var connOption = await DXService.ConsoleReadDatabase(cc);
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
                    if (await dbKit.PreExecute() == 0)
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
        var connOption = await DXService.ConsoleReadDatabase(cc);

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
                if (DXService.ConsoleReadBool("开启事务"))
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

    [Display(Name = "Full Text Search", Description = "全文检索", GroupName = "Data")]
    public static async Task FullTextSearch()
    {
        //配置
        var ci = new ConfigInit();
        var cc = ci.DXConfig;

        //选择库
        var connOption = await DXService.ConsoleReadDatabase(cc);

        //选择表
        var tableMode = DXService.ConsoleReadItem("选择表", "所有表,指定表,排除表".Split(','));
        var includeTable = new List<string>(); //指定表
        var excludeTable = new List<string>(); //排除表
        if (tableMode == 2)
        {
            includeTable = DXService.ConsoleReadJoin<string>("指定表名，多个逗号分隔");
        }
        else if (tableMode == 3)
        {
            excludeTable = DXService.ConsoleReadJoin<string>("排除表名，多个逗号分隔");
        }

        var keys = DXService.ConsoleReadJoin<string>("关键词，多个逗号分隔");
        if (keys.Count > 0)
        {
            //跑表
            var sw = Stopwatch.StartNew();

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
            ConsoleTo.LogTag($"开始检索 {listTables.Count} 张表");

            //结果输出
            var savePath = Path.Combine(ci.DXHub, DXService.NewFileName($"{nameof(FullTextSearch)}_{connOption.ConnectionType}_{connOption.DatabaseName}", ".log"));
            //找到数量
            var findCount = 0;

            for (int i = 0; i < listTables.Count; i++)
            {
                var itemTable = listTables[i];
                var sntn = DbKitExtensions.SqlSNTN(itemTable.TableName, itemTable.SchemaName, connOption.ConnectionType);
                var sql = $"select * from {sntn}";
                ConsoleTo.LogColor($"read SQL: {sql} , progress: {i + 1}/{listTables.Count}");

                //表查找结果
                var tableResult = new StringBuilder();
                var findRows = 0;
                var tableRows = 0;
                await dataKit.DbInstance.SqlExecuteDataRow(sql, readRow: row =>
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
                        ConsoleTo.LogColor($"已找到 {findCount} 条");
                    }

                    return Task.CompletedTask;
                });

                //写入表结果
                var tableShow = $"检索表（{sntn}）{tableRows} 行, 找到 {findRows} 条\r\n";
                ConsoleTo.LogColor(tableShow, ConsoleColor.Cyan);
                tableResult.Insert(0, tableShow);
                FileTo.WriteText(tableResult.ToString(), savePath);
            }
            await dataKit.Close();

            var dbShow = $"总共找到 {findCount} 条, 详情: {savePath}\r\n";
            ConsoleTo.LogColor(dbShow, ConsoleColor.Cyan);
            FileTo.WriteText(dbShow, savePath);

            sw.Stop();
            ConsoleTo.LogColor($"执行结束，共耗时：{sw.Elapsed}");
        }
    }

}