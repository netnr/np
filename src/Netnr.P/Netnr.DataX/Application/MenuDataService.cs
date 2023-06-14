using MiniExcelLibs;
using System.IO.Compression;

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
            DXService.Log("没找到作业配置");
        }
        else if (BaseTo.IsWithArgs)
        {
            working = DXService.Args;
        }
        else
        {
            var workIndex = DXService.ConsoleReadItem("选择作业", worksName);
            var workName = worksName[workIndex - 1];
            working.Add(workName);
        }

        DXService.Log($"\n开始作业({working.Count} 个): {string.Join(" ", working)}\n");
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

                    DXService.Log($"开始 {workName} 作业，进度 {ti + 1}/{working.Count}\n");

                    for (int mi = 0; mi < methods.Count; mi++)
                    {
                        var methodName = methods[mi];
                        DXService.Log($"开始 {methodName} 方法，进度 {mi + 1}/{methods.Count}\n");

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
                                        mo.ReadConnectionInfo = cc.ListConnectionInfo.FirstOrDefault(x => x.ConnectionRemark == mo.RefReadConnectionInfo);
                                        if (!string.IsNullOrWhiteSpace(mo.ReadDatabaseName))
                                        {
                                            mo.ReadConnectionInfo.DatabaseName = mo.ReadDatabaseName;
                                            mo.ReadConnectionInfo.DeepCopyNewInstance = true;
                                        }
                                    }

                                    await DataKitTo.ExportDatabase(mo, le => DXService.Log(le.NewItems[0].ToString()));
                                }
                                break;
                            case "ExportDataTable":
                                {
                                    var mo = parameters.ToJson().DeJson<DataKitTransfer.ExportDataTable>();
                                    mo.PackagePath = DXService.ParsePathVar(mo.PackagePath);

                                    //引用连接
                                    if (mo.ReadConnectionInfo == null && !string.IsNullOrWhiteSpace(mo.RefReadConnectionInfo))
                                    {
                                        mo.ReadConnectionInfo = cc.ListConnectionInfo.FirstOrDefault(x => x.ConnectionRemark == mo.RefReadConnectionInfo);
                                        if (!string.IsNullOrWhiteSpace(mo.ReadDatabaseName))
                                        {
                                            mo.ReadConnectionInfo.DatabaseName = mo.ReadDatabaseName;
                                            mo.ReadConnectionInfo.DeepCopyNewInstance = true;
                                        }
                                    }

                                    await DataKitTo.ExportDataTable(mo, le => DXService.Log(le.NewItems[0].ToString()));
                                }
                                break;
                            case "MigrateDatabase":
                                {
                                    var mo = parameters.ToJson().DeJson<DataKitTransfer.MigrateDatabase>();

                                    //引用连接
                                    if (mo.ReadConnectionInfo == null && !string.IsNullOrWhiteSpace(mo.RefReadConnectionInfo))
                                    {
                                        mo.ReadConnectionInfo = cc.ListConnectionInfo.FirstOrDefault(x => x.ConnectionRemark == mo.RefReadConnectionInfo);
                                        if (!string.IsNullOrWhiteSpace(mo.ReadDatabaseName))
                                        {
                                            mo.ReadConnectionInfo.DatabaseName = mo.ReadDatabaseName;
                                            mo.ReadConnectionInfo.DeepCopyNewInstance = true;
                                        }
                                    }
                                    if (mo.WriteConnectionInfo == null && !string.IsNullOrWhiteSpace(mo.RefWriteConnectionInfo))
                                    {
                                        mo.WriteConnectionInfo = cc.ListConnectionInfo.FirstOrDefault(x => x.ConnectionRemark == mo.RefWriteConnectionInfo);
                                        if (!string.IsNullOrWhiteSpace(mo.WriteDatabaseName))
                                        {
                                            mo.WriteConnectionInfo.DatabaseName = mo.WriteDatabaseName;
                                            mo.WriteConnectionInfo.DeepCopyNewInstance = true;
                                        }
                                    }

                                    await DataKitTo.MigrateDataTable(await mo.AsMigrateDataTable(cc.MapingMatchPattern != "Same"), le => DXService.Log(le.NewItems[0].ToString()));
                                }
                                break;
                            case "MigrateDataTable":
                                {
                                    var mo = parameters.ToJson().DeJson<DataKitTransfer.MigrateDataTable>();

                                    //引用连接
                                    if (mo.ReadConnectionInfo == null && !string.IsNullOrWhiteSpace(mo.RefReadConnectionInfo))
                                    {
                                        mo.ReadConnectionInfo = cc.ListConnectionInfo.FirstOrDefault(x => x.ConnectionRemark == mo.RefReadConnectionInfo);
                                        if (!string.IsNullOrWhiteSpace(mo.ReadDatabaseName))
                                        {
                                            mo.ReadConnectionInfo.DatabaseName = mo.ReadDatabaseName;
                                            mo.ReadConnectionInfo.DeepCopyNewInstance = true;
                                        }
                                    }
                                    if (mo.WriteConnectionInfo == null && !string.IsNullOrWhiteSpace(mo.RefWriteConnectionInfo))
                                    {
                                        mo.WriteConnectionInfo = cc.ListConnectionInfo.FirstOrDefault(x => x.ConnectionRemark == mo.RefWriteConnectionInfo);
                                        if (!string.IsNullOrWhiteSpace(mo.WriteDatabaseName))
                                        {
                                            mo.WriteConnectionInfo.DatabaseName = mo.WriteDatabaseName;
                                            mo.WriteConnectionInfo.DeepCopyNewInstance = true;
                                        }
                                    }

                                    await DataKitTo.MigrateDataTable(mo, le => DXService.Log(le.NewItems[0].ToString()));
                                }
                                break;
                            case "ImportDatabase":
                                {
                                    var mo = parameters.ToJson().DeJson<DataKitTransfer.ImportDatabase>();
                                    mo.PackagePath = DXService.ParsePathVar(mo.PackagePath);

                                    //引用连接
                                    if (mo.WriteConnectionInfo == null && !string.IsNullOrWhiteSpace(mo.RefWriteConnectionInfo))
                                    {
                                        mo.WriteConnectionInfo = cc.ListConnectionInfo.FirstOrDefault(x => x.ConnectionRemark == mo.RefWriteConnectionInfo);
                                        if (!string.IsNullOrWhiteSpace(mo.WriteDatabaseName))
                                        {
                                            mo.WriteConnectionInfo.DatabaseName = mo.WriteDatabaseName;
                                            mo.WriteConnectionInfo.DeepCopyNewInstance = true;
                                        }
                                    }

                                    await DataKitTo.ImportDatabase(mo, le => DXService.Log(le.NewItems[0].ToString()));
                                }
                                break;
                            default:
                                DXService.Log($"Not support {methodName}\n", ConsoleColor.Red);
                                break;
                        }
                    }

                    DXService.Log($"\n完成 {workName} 作业\n");
                }
                catch (Exception ex)
                {
                    DXService.Log($"作业（{workName}）出错", ConsoleColor.Red);
                    DXService.Log(ex);
                }
            }
            else
            {
                DXService.Log($"无效作业（{workName}）", ConsoleColor.Red);
            }
        }

        DXService.Log($"\n作业全部完成\n");
    }

    [Display(Name = "Migrate Data", Description = "迁移数据", GroupName = "Data", AutoGenerateFilter = true)]
    public static async Task MigrateData()
    {
        //配置
        var ci = new ConfigInit();
        var cc = ci.DXConfig;

        DXService.Log("\n注意：参数模式（静默执行）可配置多表、指定列映射\n");

        var mdt = new DataKitTransfer.MigrateDataTable
        {
            ReadConnectionInfo = await DXService.ConsoleReadDatabase(cc, "读取库"),
            WriteConnectionInfo = await DXService.ConsoleReadDatabase(cc, "写入库")
        };

        if (DXService.ConsoleReadBool("数据库全表"))
        {
            var mdb = new DataKitTransfer.MigrateDatabase
            {
                ReadConnectionInfo = mdt.ReadConnectionInfo,
                WriteConnectionInfo = mdt.WriteConnectionInfo,
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

            var clearTableSql = DbKitExtensions.SqlClearTable(mdt.WriteConnectionInfo.ConnectionType, rwi.WriteTableName);
            Console.Write($"清空写入表(可选, {clearTableSql}): ");
            rwi.WriteDeleteSQL = Console.ReadLine();

            mdt.ListReadWrite = new List<DataKitTransfer.ReadWriteItem>() { rwi };
        }

        var vm = new ResultVM();
        await DXService.TryAgain(async () =>
        {
            vm = await DataKitTo.MigrateDataTable(mdt, le => DXService.Log(le.NewItems[0].ToString()));
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
                ReadConnectionInfo = edt.ReadConnectionInfo
            };
            DXService.Log("读取表脚本构建");
            edt = await DataKitTo.ConvertTransferVM(edb);
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

        edt.ExportType = "dataOnly,all".Split(',')[etypeIndex - 1];
        edt.PackagePath = Path.Combine(ci.DXHub, DXService.NewFileName(edt.ReadConnectionInfo.ConnectionType, ".zip"));
        edt.PackagePath = DXService.ConsoleReadPath("导出完整路径", 1, edt.PackagePath, false);

        var vm = new ResultVM();
        await DXService.TryAgain(async () =>
        {
            vm = await DataKitTo.ExportDataTable(edt, le => DXService.Log($"{le.NewItems[0]}"));
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
                DXService.Log($"{cce.NewItems[0]}");
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
                ReadConnectionInfo = edt.ReadConnectionInfo
            };
            edt = await DataKitTo.ConvertTransferVM(edb);
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

        await DXService.TryAgain(async () =>
        {
            edt.ReadConnectionInfo.AutoClose = false;
            var dbk = edt.ReadConnectionInfo.CreateInstance();
            await dbk.ConnOption.Open();

            var sheets = new Dictionary<string, object>();
            for (int i = 0; i < edt.ListReadDataSQL.Count; i++)
            {
                var sql = edt.ListReadDataSQL[i];

                var cmdOption = dbk.CommandCreate(sql);
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
            DXService.Log("正在导出 ...");
            MiniExcel.SaveAs(edt.PackagePath, sheets, overwriteFile: true);
            DXService.Log($"导出完成，耗时：{sw.Elapsed}");

            await dbk.ConnOption.Close();
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

        Console.Write("指定表名(逗号分隔,默认工作薄名称): ");
        var tns = Console.ReadLine();
        var listTableName = new List<string>();
        if (!string.IsNullOrWhiteSpace(tns))
        {
            listTableName.AddRange(tns.Split(','));
        }

        var vm = new ResultVM();
        await DXService.TryAgain(async () =>
        {
            var dk = DataKitTo.CreateDkInstance(idb.WriteConnectionInfo);
            var tables = await dk.GetTable(databaseName: idb.WriteDatabaseName);

            var isCopy = await dk.DbInstance.PreExecute() != -1; //预检通过

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
                    var sqlEmpty = DbKitExtensions.SqlEmpty(sntn, idb.WriteConnectionInfo.ConnectionType);
                    var dtWrite = (await dk.DbInstance.SqlExecuteDataSet(sqlEmpty)).Datas.Tables[0];
                    dtWrite.Namespace = table.SchemaName;
                    dtWrite.TableName = table.TableName;

                    //清空表
                    if (idb.WriteDeleteData)
                    {
                        vm.PartTime();
                        var clearTableSql = DbKitExtensions.SqlClearTable(idb.WriteConnectionInfo.ConnectionType, sntn);

                        DXService.Log($"清理写入表：{clearTableSql}");
                        var num = await dk.DbInstance.SqlExecuteNonQuery(clearTableSql);
                        DXService.Log($"返回受影响行数：{num}，耗时：{vm.PartTimeFormat()}");
                    }

                    //读取数据
                    var batchIndex = 0;
                    var batchRows = 0;
                    var rows = MiniExcel.Query(idb.PackagePath, sheetName: sheetName, useHeaderRow: true);
                    foreach (var row in rows)
                    {
                        //填充行
                        var drWrite = dtWrite.NewRow();
                        foreach (DataColumn dc in dtWrite.Columns)
                        {
                            if (row.TryGetValue(dc.ColumnName, out object value))
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

                            await dk.DbInstance.BulkCopy(dtWrite, isCopy);
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

                        await dk.DbInstance.BulkCopy(dtWrite, isCopy);
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

    [Display(Name = "Generate Table Mapping", Description = "生成 表映射(读=>写)", GroupName = "Data")]
    public static async Task GenerateTableMapping()
    {
        //配置
        var ci = new ConfigInit();
        var cc = ci.DXConfig;

        var ciRead = await DXService.ConsoleReadDatabase(cc, "读取库");
        var ciWrite = await DXService.ConsoleReadDatabase(cc, "写入库");

        DXService.Log($"{ciRead.ConnectionType}://{ciWrite.ConnectionType}");
        DXService.Log($"正在读取表信息");

        var tableRead = await DataKitTo.CreateDkInstance(ciRead).GetTable();
        var tableWrite = await DataKitTo.CreateDkInstance(ciWrite).GetTable();

        DXService.Log($"读取库 {tableRead.Count} 张表，写入库 {tableWrite.Count} 张表");

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
                DXService.Log($"{cc.MapingMatchPattern} Mapping {mo.ReadTableName} => {mo.WriteTableName}");
            }
            //相似
            else if (cc.MapingMatchPattern == "Similar")
            {
                foreach (var itemWrite in tableWrite)
                {
                    if (DataKitTo.SimilarMatch(itemWrite.TableName, itemRead.TableName))
                    {
                        mo.WriteTableName = DbKitExtensions.SqlSNTN(itemWrite.TableName, itemWrite.SchemaName);
                        DXService.Log($"{cc.MapingMatchPattern} Mapping {mo.ReadTableName} => {mo.WriteTableName}");
                        break;
                    }
                }
            }

            mo.ReadDataSQL = $"SELECT * FROM {DbKitExtensions.SqlSNTN(itemRead.TableName, itemRead.SchemaName, ciRead.ConnectionType)}";
            if (string.IsNullOrEmpty(mo.WriteTableName))
            {
                DXService.Log($"No Mapping {itemRead.TableName}");
            }
            else
            {
                mo.WriteDeleteSQL = DbKitExtensions.SqlClearTable(ciWrite.ConnectionType, mo.WriteTableName);
            }

            rws.Add(mo);
        }

        //表映射文件名
        var MappingTableName = DXService.NewFileName("MappingTable", ".json");
        var MappingTablePath = Path.Combine(ci.DXHub, MappingTableName);

        DXService.Log($"写入表映射: {MappingTablePath}");
        FileTo.WriteText(rws.ToJson(true), MappingTablePath, false);
    }

    [Display(Name = "Generate Column Mapping", Description = "生成 列映射(读=>写)", GroupName = "Data")]
    public static async Task GenerateColumnMapping()
    {
        //配置
        var ci = new ConfigInit();
        var cc = ci.DXConfig;

        var ciRead = await DXService.ConsoleReadDatabase(cc, "读取库");
        var ciWrite = await DXService.ConsoleReadDatabase(cc, "写入库");

        DXService.Log($"{ciRead.ConnectionType}://{ciWrite.ConnectionType}");

        var tableMapPath = DXService.ConsoleReadPath("表映射文件");
        var rws = File.ReadAllText(tableMapPath).DeJson<DataKitTransfer.ReadWriteItem[]>();

        DXService.Log($"正在读取列信息");

        var columnRead = await DataKitTo.CreateDkInstance(ciRead).GetColumn();
        var columnWrite = await DataKitTo.CreateDkInstance(ciWrite).GetColumn();

        DXService.Log($"读取库 {columnRead.Count} 列, 写入库 {columnWrite.Count} 列");

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
                    DXService.Log($"Same {itemRead.TableName}.{itemRead.ColumnName}");
                }
                //相似
                else if (cc.MapingMatchPattern == "Similar")
                {
                    foreach (var itemWrite in igWrite)
                    {
                        if (DataKitTo.SimilarMatch(itemWrite.ColumnName, itemRead.ColumnName))
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

    [Display(Name = "Generate Table DDL", Description = "生成 DDL", GroupName = "Data", AutoGenerateFilter = true)]
    public static async Task GenerateTableDDL()
    {
        //配置
        var ci = new ConfigInit();
        var cc = ci.DXConfig;

        //选择库
        var cdb = await DXService.ConsoleReadDatabase(cc);
        var dk = DataKitTo.CreateDkInstance(cdb);

        Console.Write($"表名, 逗号分隔(default: *): ");
        var tableNames = Console.ReadLine().Trim();

        var listTable = new List<DataKitTablResult>();
        var allTables = await dk.GetTable();
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

    [Display(Name = "Connection Test", Description = "连接测试", GroupName = "Data",
        ShortName = "conntest", Prompt = "ndx conntest")]
    public static async Task ConnectionTest()
    {
        //配置
        var ci = new ConfigInit();
        var cc = ci.DXConfig;

        if (cc.ListConnectionInfo.Count > 0)
        {
            var badConn = 0;
            await Parallel.ForEachAsync(cc.ListConnectionInfo, async (connOption, token) =>
            {
                var isBad = false;
                var listLog = new List<string>();

                var dbk = connOption.CreateInstance();
                try
                {
                    listLog.Add($"\r\n\r\n[{connOption.ConnectionRemark}]{connOption.ConnectionType}://{connOption.ConnectionString}");
                    await dbk.ConnOption.Open();

                    listLog.Add("successfully connected");

                    var qv = string.Empty;
                    switch (connOption.ConnectionType)
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
                        var versionInfo = await dbk.SqlExecuteScalar(qv);
                        listLog.Add(versionInfo.ToString());
                    }
                }
                catch (Exception ex)
                {
                    Interlocked.Increment(ref badConn);
                    isBad = true;

                    listLog.Add("failed to connect");
                    listLog.Add(ex.ToJson(true));
                }
                listLog.Add($"Drive: {dbk.ConnOption.Connection.GetType().Assembly.FullName}");

                DXService.Log(string.Join(Environment.NewLine, listLog), isBad ? ConsoleColor.Red : ConsoleColor.Green);
            });

            DXService.Log($"\r\nsuccessfully: {cc.ListConnectionInfo.Count - badConn}, failed: {badConn}");
        }
        else
        {
            DXService.Log($"Not Found", ConsoleColor.Red);
        }
    }

    [Display(Name = "Parameter Optimization (SQLite MySQL)", Description = "参数优化", GroupName = "Data")]
    public static async Task ParameterOptimization()
    {
        //配置
        var ci = new ConfigInit();
        var cc = ci.DXConfig;

        //选择库
        var cdb = await DXService.ConsoleReadDatabase(cc);
        var dbk = cdb.CreateInstance();

        switch (cdb.ConnectionType)
        {
            case EnumTo.TypeDB.SQLite:
                {
                    DXService.Log($"磁盘空间释放：VACUUM");
                    await dbk.SqlExecuteNonQuery("VACUUM");
                }
                break;
            case EnumTo.TypeDB.MySQL:
            case EnumTo.TypeDB.MariaDB:
                {
                    if (await dbk.PreExecute() == 0)
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

    [Display(Name = "Execute SQL", Description = "执行 SQL", GroupName = "Data")]
    public static async Task ExecuteNonQuery()
    {
        //配置
        var ci = new ConfigInit();
        var cc = ci.DXConfig;

        //选择库
        var cdb = await DXService.ConsoleReadDatabase(cc);

        Console.Write("脚本路径或SQL: ");
        var sqlOrPath = Console.ReadLine();
        if (File.Exists(sqlOrPath))
        {
            sqlOrPath = File.ReadAllText(sqlOrPath);
        }

        //跑表
        var sw = Stopwatch.StartNew();

        var dbk = cdb.CreateInstance();
        try
        {
            var num = await dbk.SqlExecuteNonQuery(sqlOrPath, cmdCall: async cmdOption =>
            {
                if (DXService.ConsoleReadBool("开启事务"))
                {
                    await cmdOption.OpenTransactionAsync();
                }
            });
            DXService.Log($"执行结束，受影响行数：{num}，耗时：{sw.Elapsed}");
        }
        catch (Exception ex)
        {
            DXService.Log(ex);
        }
    }

    [Display(Name = "Full Text Search", Description = "全文检索", GroupName = "Data")]
    public static async Task FullTextSearch()
    {
        //配置
        var ci = new ConfigInit();
        var cc = ci.DXConfig;

        //选择库
        var cdb = await DXService.ConsoleReadDatabase(cc);

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
        var sw = Stopwatch.StartNew();

        var dk = DataKitTo.CreateDkInstance(cdb);
        dk.DbInstance.ConnOption.AutoClose = false;
        var listTables = await dk.GetTable();
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
        var savePath = Path.Combine(ci.DXHub, DXService.NewFileName($"{nameof(FullTextSearch)}_{cdb.ConnectionType}", ".log"));
        //找到数量
        var findCount = 0;

        for (int i = 0; i < listTables.Count; i++)
        {
            var itemTable = listTables[i];
            var sntn = DbKitExtensions.SqlSNTN(itemTable.TableName, itemTable.SchemaName, cdb.ConnectionType);
            var sql = $"select * from {sntn}";
            DXService.Log($"开始读取: {sql} , 进度: {i + 1}/{listTables.Count}");

            //表查找结果
            var tableResult = new StringBuilder();
            var findRows = 0;
            var tableRows = 0;
            await dk.DbInstance.SqlExecuteDataRow(sql, readRow: row =>
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

                return Task.CompletedTask;
            });

            //写入表结果
            var tableShow = $"表（{sntn}）检索 {tableRows} 条, 找到 {findRows} 条, \r\n";
            DXService.Log(tableShow);
            tableResult.Insert(0, tableShow);
            FileTo.WriteText(tableResult.ToString(), savePath);
        }
        await dk.DbInstance.ConnOption.Close();

        var dbShow = $"总共找到 {findCount} 条, 查看详情: {savePath}\r\n";
        DXService.Log(dbShow);
        FileTo.WriteText(dbShow, savePath);

        sw.Stop();
        DXService.Log($"执行结束，共耗时：{sw.Elapsed}");
    }
}