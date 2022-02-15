using System.Reflection;
using Newtonsoft.Json.Linq;
using Netnr.Core;
using Netnr.SharedAdo;
using Netnr.DataX.Domain;
using Netnr.SharedDataKit;
using System.ComponentModel.DataAnnotations;
using System.Collections.Concurrent;

namespace Netnr.DataX.Application;

/// <summary>
/// 菜单
/// </summary>
public partial class MenuService
{
    [Display(Name = "Console Exit", GroupName = "0")]
    public static void Exit()
    {
        Environment.Exit(0);
    }

    [Display(Name = "Console Config (Encoding)", GroupName = "0")]
    public static void ConsoleConfig()
    {
        DXService.Log($"Current: {Console.OutputEncoding}");

        //选择编码
        var cri = DXService.ConsoleReadItem("Choose an encoding", "Unicode,UTF-8".Split(','), 1);
        switch (cri)
        {
            case 1: Console.OutputEncoding = Encoding.Unicode; break;
            case 2: Console.OutputEncoding = Encoding.UTF8; break;
        }
    }


    [Display(Name = "System Status Info", GroupName = "11")]
    public static void SystemStatus()
    {
        var ss = new SystemStatusTo();
        DXService.Log(ss.ToView());
    }

    [Display(Name = "Open Hub Folder", GroupName = "11")]
    public static void OpenHubFolder()
    {
        //配置
        var ci = new ConfigInit();
        if (CmdTo.IsWindows)
        {
            CmdTo.Execute($"start {ci.DXHub}");
        }
        else
        {
            var cr = CmdTo.Execute($"cd {ci.DXHub} && pwd && ls -lh");
            DXService.Log(cr.CrOutput);
        }
    }

    [Display(Name = "Ready", GroupName = "11")]
    public static bool Ready()
    {
        //配置
        var ci = new ConfigInit();
        var co = ci.ConfigObj;

        var cri = DXService.ConsoleReadItem("请选择", "Test connection,GC,数据库参数优化（MySQL）".Split(','));

        switch (cri)
        {
            //测试配置的连接
            case 1:
                {
                    if (co.ListConnectionInfo.Count > 0)
                    {
                        var badConn = 0;
                        Parallel.ForEach(co.ListConnectionInfo, ci =>
                        {
                            var connInfo = new List<string> { $"{ci.ConnectionRemark} -> {ci.ConnectionType} => {ci.ConnectionString}" };

                            using var dbConn = DataKit.DbConn(ci.ConnectionType, ci.ConnectionString);
                            try
                            {
                                dbConn.Open();

                                connInfo.Insert(0, $"\nTest connection is successful ({ci.ConnectionType} {dbConn.ServerVersion})");

                                var qv = string.Empty;
                                switch (ci.ConnectionType)
                                {
                                    case SharedEnum.TypeDB.Oracle:
                                        qv = "select * from v$version where rownum=1";
                                        break;
                                    case SharedEnum.TypeDB.SQLServer:
                                        qv = "select @@version";
                                        break;
                                    case SharedEnum.TypeDB.PostgreSQL:
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
                                connInfo.Add(ex.Message);
                            }

                            connInfo.Add($"Drive: {dbConn.GetType().Assembly.FullName}");

                            DXService.Log(string.Join(Environment.NewLine, connInfo));
                        });

                        DXService.Log($"\nSuccessful: {co.ListConnectionInfo.Count - badConn}, Failed: {badConn}");
                    }
                    else
                    {
                        DXService.Log($"Not Found");
                    }
                }
                break;
            case 2:
                {
                    DXService.Log($"Use Physical Memory: {ParsingTo.FormatByteSize(Environment.WorkingSet)}");
                    GC.Collect();
                    Thread.Sleep(1000);
                    DXService.Log($"Use Physical Memory: {ParsingTo.FormatByteSize(Environment.WorkingSet)}");
                }
                break;
            //数据库参数优化
            case 3:
                {
                    //选择库
                    var cdb = DXService.ConsoleReadDatabase(co);
                    var db = cdb.NewDbHelper();

                    switch (cdb.ConnectionType)
                    {
                        case SharedEnum.TypeDB.SQLite:
                            break;
                        case SharedEnum.TypeDB.MySQL:
                        case SharedEnum.TypeDB.MariaDB:
                            {
                                var drs = db.SqlExecuteReader("SHOW VARIABLES").Item1.Tables[0].Select();

                                var dicVar1 = new Dictionary<string, string>
                                {
                                    { "local_infile","是否允许加载本地数据，BulkCopy 需要开启"},
                                    { "innodb_lock_wait_timeout","innodb 的 dml 操作的行级锁的等待时间，事务等待获取资源等待的最长时间，BulkCopy 量大超时设置，单位：秒"},
                                    { "max_allowed_packet","传输的 packet 大小限制，最大 1G，单位：B"}
                                };

                                var listBetterSql = new List<string>();
                                foreach (var key in dicVar1.Keys)
                                {
                                    var dr = drs.FirstOrDefault(x => x[0].ToString() == key);
                                    if (dr != null)
                                    {
                                        var val = dr[1]?.ToString();
                                        switch (key)
                                        {
                                            case "local_infile":
                                                if (val != "ON")
                                                {
                                                    //ON 开启，OFF 关闭
                                                    listBetterSql.Add("SET GLOBAL local_infile = ON");
                                                }
                                                break;
                                            case "innodb_lock_wait_timeout":
                                                if (Convert.ToInt32(val) < 600)
                                                {
                                                    //10 分钟超时
                                                    listBetterSql.Add("SET GLOBAL innodb_lock_wait_timeout = 600");
                                                }
                                                break;
                                            case "max_allowed_packet":
                                                if (Convert.ToInt32(val) != 1073741824)
                                                {
                                                    //传输的 packet 大小 1G
                                                    listBetterSql.Add("SET GLOBAL max_allowed_packet = 1073741824");
                                                }
                                                break;
                                        }

                                        DXService.Log($"\n{key} -> {val} （{dicVar1[key]}）");
                                    }
                                }

                                if (listBetterSql.Count > 0)
                                {
                                    DXService.Log($"\n执行优化脚本：\n{string.Join(Environment.NewLine, listBetterSql)}");
                                    db.SqlExecuteNonQuery(listBetterSql);
                                }
                                else
                                {
                                    DXService.Log($"\n没有需要优化的参数");
                                }
                            }
                            break;
                        case SharedEnum.TypeDB.Oracle:
                            break;
                        case SharedEnum.TypeDB.SQLServer:
                            break;
                        case SharedEnum.TypeDB.PostgreSQL:
                            break;
                    }
                }
                break;
            default: DXService.Log("not support"); break;
        }

        return true;
    }


    [Display(Name = "静默任务", GroupName = "30")]
    public static bool SilenceTask()
    {
        //配置
        var ci = new ConfigInit();
        var co = ci.ConfigObj;

        var tasks = ci.Silence.Properties().Select(x => x.Name).ToList();
        if (tasks.Count == 0)
        {
            DXService.Log("没找到静默任务");
        }
        else
        {
            var taskIndex = DXService.ConsoleReadItem("选择执行任务", tasks);
            var taskName = tasks[taskIndex - 1];
            SilenceService.Run(new List<string> { taskName });
        }

        return true;
    }


    [Display(Name = "迁移数据表", GroupName = "31")]
    public static bool MigrateDataTable()
    {
        //配置
        var ci = new ConfigInit();
        var co = ci.ConfigObj;

        DXService.Log("\n注意：参数模式（静默执行）可配置多表、指定列映射\n");

        var mdt = new TransferVM.MigrateDataTable
        {
            ReadConnectionInfo = DXService.ConsoleReadDatabase(co, "读取库"),
            WriteConnectionInfo = DXService.ConsoleReadDatabase(co, "写入库")
        };
        var rwi = new TransferVM.ReadWriteItem();

        Console.Write("读取表 SQL: ");
        rwi.ReadSQL = Console.ReadLine();

        Console.Write("写入表名: ");
        rwi.WriteTableName = Console.ReadLine();

        var clearTableSql = DbHelper.SqlClearTable(mdt.WriteConnectionInfo.ConnectionType, rwi.WriteTableName);
        Console.Write($"清空写入表(可选, {clearTableSql}): ");
        rwi.WriteDeleteSQL = Console.ReadLine();

        mdt.ListReadWrite = new List<TransferVM.ReadWriteItem>() { rwi };

        var vm = DataKit.MigrateDataTable(mdt, le => DXService.Log(le.NewItems[0].ToString()));

        return vm.Code == 200;
    }

    [Display(Name = "迁移数据库", GroupName = "31")]
    public static bool MigrateDatabase()
    {
        //配置
        var ci = new ConfigInit();
        var co = ci.ConfigObj;

        var mdb = new TransferVM.MigrateDatabase
        {
            ReadConnectionInfo = DXService.ConsoleReadDatabase(co, "读取库："),
            WriteConnectionInfo = DXService.ConsoleReadDatabase(co, "写入库："),
            WriteDeleteData = DXService.ConsoleReadItem("写入前清空表数据", "保留,清空".Split(',')) == 2
        };

        var mdt = mdb.AsMigrateDataTable();
        var vm = DataKit.MigrateDataTable(mdt, le => DXService.Log(le.NewItems[0].ToString()));

        return vm.Code == 200;
    }


    [Display(Name = "导出数据表", GroupName = "32")]
    public static bool ExportDataTable()
    {
        //配置
        var ci = new ConfigInit();
        var co = ci.ConfigObj;

        var et = new TransferVM.ExportDataTable()
        {
            ReadConnectionInfo = DXService.ConsoleReadDatabase(co)
        };

        //读取表
        Console.Write("读取表(SELECT * FROM Table1; SELECT * FROM Table2): ");
        var tns = Console.ReadLine().Trim();
        if (!string.IsNullOrWhiteSpace(tns))
        {
            et.ListReadSQL.AddRange(tns.Split(';'));
        }
        et.ListReadSQL = et.ListReadSQL.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

        et.ZipPath = PathTo.Combine(ci.DXHub, DXService.NewFileName(et.ReadConnectionInfo.ConnectionType, ".zip"));
        et.ZipPath = DXService.ConsoleReadPath("导出完整路径", 1, et.ZipPath, false);

        var vm = DataKit.ExportDataTable(et, le => DXService.Log($"{le.NewItems[0]}"));

        return vm.Code == 200;
    }

    [Display(Name = "导出数据库", GroupName = "32")]
    public static bool ExportDatabase()
    {
        //配置
        var ci = new ConfigInit();
        var co = ci.ConfigObj;

        var ed = new TransferVM.ExportDatabase()
        {
            ReadConnectionInfo = DXService.ConsoleReadDatabase(co)
        };

        //指定表名
        Console.Write("指定表名(默认所有表, 多个表逗号分隔): ");
        var tns = Console.ReadLine().Trim();
        if (!string.IsNullOrWhiteSpace(tns))
        {
            ed.ListReadTableName.AddRange(tns.Split(','));
        }

        ed.ZipPath = PathTo.Combine(ci.DXHub, DXService.NewFileName(ed.ReadConnectionInfo.ConnectionType, ".zip"));
        ed.ZipPath = DXService.ConsoleReadPath("导出完整路径", 1, ed.ZipPath, false);

        var vm = DataKit.ExportDatabase(ed, le => DXService.Log($"{le.NewItems[0]}"));

        return vm.Code == 200;
    }


    [Display(Name = "导入数据库", GroupName = "33")]
    public static bool ImportDatabase()
    {
        //配置
        var ci = new ConfigInit();
        var co = ci.ConfigObj;

        var idb = new TransferVM.ImportDatabase()
        {
            WriteConnectionInfo = DXService.ConsoleReadDatabase(co),
            ZipPath = DXService.ConsoleReadPath("导入包(zip): ", 1),
            WriteDeleteData = DXService.ConsoleReadItem("写入前清空表数据", "保留,清空".Split(',')) == 2
        };

        var vm = DataKit.ImportDatabase(idb, cce =>
        {
            DXService.Log($"{cce.NewItems[0]}");
        });

        return vm.Code == 200;
    }


    [Display(Name = "生成 表映射(读=>写)", GroupName = "34")]
    public static bool GenerateTableMapping()
    {
        //配置
        var ci = new ConfigInit();
        var co = ci.ConfigObj;

        var ciRead = DXService.ConsoleReadDatabase(co, "读取库");
        var ciWrite = DXService.ConsoleReadDatabase(co, "写入库");

        DXService.Log($"{ciRead.ConnectionType} => {ciWrite.ConnectionType}");
        DXService.Log($"正在读取表信息");

        var vmRead = DataKit.GetTable(ciRead.ConnectionType, ciRead.ConnectionString);
        var vmWrite = DataKit.GetTable(ciWrite.ConnectionType, ciWrite.ConnectionString);

        if (vmRead.Code == 200 && vmWrite.Code == 200)
        {
            var tableRead = vmRead.Data as List<TableVM>;
            var tableWrite = vmWrite.Data as List<TableVM>;

            DXService.Log($"读取库 {tableRead.Count} 张表，写入库 {tableWrite.Count} 张表");

            var rws = new List<TransferVM.ReadWriteItem>();

            foreach (var itemRead in tableRead)
            {
                var mo = new TransferVM.ReadWriteItem()
                {
                    ReadTableName = itemRead.TableName
                };

                //相同
                if (tableWrite.Any(x => x.TableName == itemRead.TableName))
                {
                    mo.WriteTableName = itemRead.TableName;
                    DXService.Log($"Same Mapping {itemRead.TableName}");
                }
                //相似
                else if (co.MapingMatchPattern == "Similar")
                {
                    foreach (var itemWrite in tableWrite)
                    {
                        if (DXService.SimilarMatch(itemWrite.TableName, itemRead.TableName))
                        {
                            mo.WriteTableName = itemWrite.TableName;
                            DXService.Log($"{co.MapingMatchPattern} Mapping {itemRead.TableName}:{mo.WriteTableName}");
                            break;
                        }
                    }
                }

                mo.ReadSQL = $"SELECT * FROM {DbHelper.SqlQuote(ciRead.ConnectionType, itemRead.TableName)}";
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
            var MappingTablePath = PathTo.Combine(ci.DXHub, MappingTableName);

            DXService.Log($"写入表映射: {MappingTablePath}");
            FileTo.WriteText(rws.ToJson(true), MappingTablePath, false);
        }
        else
        {
            DXService.Log($"未获取到表信息");
        }

        return true;
    }

    [Display(Name = "生成 列映射(读=>写)", GroupName = "34")]
    public static bool GenerateColumnMapping()
    {
        //配置
        var ci = new ConfigInit();
        var co = ci.ConfigObj;

        var ciRead = DXService.ConsoleReadDatabase(co, "读取库");
        var ciWrite = DXService.ConsoleReadDatabase(co, "写入库");

        DXService.Log($"{ciRead.ConnectionType} => {ciWrite.ConnectionType}");

        var tableMapPath = DXService.ConsoleReadPath("表映射文件");
        var rws = File.ReadAllText(tableMapPath).ToEntitys<TransferVM.ReadWriteItem>();

        DXService.Log($"正在读取列信息");
        var vmRead = DataKit.GetColumn(ciRead.ConnectionType, ciRead.ConnectionString);
        var vmWrite = DataKit.GetColumn(ciWrite.ConnectionType, ciWrite.ConnectionString);

        if (vmRead.Code == 200 && vmWrite.Code == 200)
        {
            var columnRead = vmRead.Data as List<ColumnVM>;
            var columnWrite = vmWrite.Data as List<ColumnVM>;

            DXService.Log($"读取库 {columnRead.Count} 列, 写入库 {columnWrite.Count} 列");

            foreach (var rw in rws)
            {
                rw.ReadWriteColumnMap.Clear();
                var igRead = columnRead.Where(x => x.TableName == rw.ReadTableName);
                var igWrite = columnWrite.Where(x => x.TableName == rw.WriteTableName);

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
                    else if (co.MapingMatchPattern == "Similar")
                    {
                        foreach (var itemWrite in igWrite)
                        {
                            if (DXService.SimilarMatch(itemWrite.ColumnName, itemRead.ColumnName))
                            {
                                newField = itemWrite.ColumnName;
                                DXService.Log($"{co.MapingMatchPattern} Mapping {itemRead.TableName}.{itemRead.ColumnName}:{itemWrite.TableName}.{newField}");
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
        else
        {
            DXService.Log($"未获取到表信息");
        }

        return true;
    }


    [Display(Name = "执行 SQL", GroupName = "35")]
    public static bool ExecuteSQL()
    {
        //配置
        var ci = new ConfigInit();
        var co = ci.ConfigObj;

        //选择库
        var cdb = DXService.ConsoleReadDatabase(co);

        var sqlPath = DXService.ConsoleReadPath("脚本路径(sql): ", 1);
        DXService.Log($"开始执行脚本 {sqlPath}");

        //跑表
        var st = new SharedTimingVM();

        var db = cdb.NewDbHelper();
        var er = db.SqlExecuteReader(FileTo.ReadText(sqlPath));

        DXService.Log($"执行结束，受影响行数：{er.Item2}，耗时：{st.PartTimeFormat()}");

        return true;
    }


    [Display(Name = "转成 UTF8(请先备份)", GroupName = "66")]
    public static void TextEncodingConversion()
    {
        TextEncodingConversionService.Run();
    }

    [Display(Name = "AES 加密解密(数据库连接字符串)", GroupName = "66")]
    public static void AESEncryptDecrypt()
    {
        var ed = DXService.ConsoleReadItem("请选择", "Encrypt 加密,Decrypt 解密".Split(','), 1);

        Console.Write($"请输入内容: ");
        var content = Console.ReadLine();
        Console.Write($"请输入密码: ");
        var password = Console.ReadLine();

        try
        {
            var result = (ed) switch
            {
                2 => CalcTo.AESDecrypt(content, password),
                _ => CalcTo.AESEncrypt(content, password)
            };

            DXService.Log(Environment.NewLine + result);
        }
        catch (Exception ex)
        {
            DXService.Log($"ERROR: {ex.Message}");
        }
    }

    [Display(Name = "Git Pull(有 .git 文件夹)", GroupName = "66")]
    public static void GitPull()
    {
        try
        {
            var dp = Environment.CurrentDirectory.TrimEnd('/').TrimEnd('\\');
            var rootPath = DXService.ConsoleReadPath("请输入目录", 2, dp);

            var dis = new DirectoryInfo(rootPath);
            var sdis = dis.GetDirectories().ToList();

            DXService.Log($"\n{sdis.Count} 个项目\n");

            int c1 = 0;
            int c2 = 0;
            Parallel.ForEach(sdis, sdi =>
            {
                if (Directory.Exists(sdi.FullName + "/.git"))
                {
                    var arg = $"git -C \"{sdi.FullName}\" pull --all";
                    var cr = CmdTo.Execute(arg);
                    var rt = cr.CrOutput + cr.CrError;
                    DXService.Log($"【 {sdi.Name} 】\n{rt}");
                    c1++;
                }
                else
                {
                    DXService.Log($"已跳过 \"{sdi.FullName}\", 未找到 .git\n");
                    c2++;
                }
            });

            DXService.Log($"Done!  Pull: {c1}, Skip: {c2}");
        }
        catch (Exception ex)
        {
            DXService.Log($"ERROR: {ex.Message}");
        }
    }

    [Display(Name = "项目拷贝(替换 appsettings.json 密钥)", GroupName = "66")]
    public static void ProjectSafeCopy()
    {
        ProjectSafeCopyService.Run();
    }
}