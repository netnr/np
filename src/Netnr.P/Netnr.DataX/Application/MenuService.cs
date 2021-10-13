using System.Reflection;
using Newtonsoft.Json.Linq;
using Netnr.Core;
using Netnr.SharedAdo;
using Netnr.DataX.Domain;
using Netnr.SharedDataKit;
using System.ComponentModel.DataAnnotations;

namespace Netnr.DataX.Application;

/// <summary>
/// 菜单
/// </summary>
public partial class MenuService
{
    [Display(Name = "退出", GroupName = "0")]
    public static void Exit()
    {
        Environment.Exit(0);
    }

    #region 11

    [Display(Name = "系统状态", GroupName = "11")]
    public static void SystemStatus()
    {
        //输出头
        var mi = MethodBase.GetCurrentMethod();
        DXService.ShowTitleInfo(mi);

        var ss = new SystemStatusTo();
        DXService.Log(ss.ToView());
    }

    [Display(Name = "打开 Hub 文件夹", GroupName = "11")]
    public static void OpenHubFolder()
    {
        //输出头
        var mi = MethodBase.GetCurrentMethod();
        DXService.ShowTitleInfo(mi);

        //配置
        var co = new ConfigObj();
        if (CmdTo.IsWindows)
        {
            CmdTo.Execute($"start {co.DXHub}");
        }
        else
        {
            DXService.Log("仅支持 Windows 系统");
        }
    }

    #endregion

    #region 44

    [Display(Name = "数据库信息", GroupName = "44")]
    public static bool DatabaseInfo()
    {
        //输出头
        var mi = MethodBase.GetCurrentMethod();
        DXService.ShowTitleInfo(mi);

        //配置
        var co = new ConfigObj();

        //查询项
        var qi = DXService.ConsoleReadItem("选择查询项：", "环境信息,库信息,表信息".Split(','), 1);

        //选择库
        var cdb = DXService.ConsoleReadDatabase(co);

        DXService.Log("正在查询，请稍等...\n");
        switch (qi)
        {
            case 1:
                {
                    var vm = DataKitTo.GetDEI(cdb.TDB, cdb.Conn);
                    if (vm.Code == 200)
                    {
                        DXService.Log(vm.Data.ToJson(true));
                    }
                    else
                    {
                        DXService.Log($"{vm.Msg}");
                    }
                }
                break;
            case 2:
                {
                    var vm = DataKitTo.GetDatabase(cdb.TDB, cdb.Conn);
                    if (vm.Code == 200)
                    {
                        DXService.Log($"{vm.Data.ToJson(true)}");
                    }
                    else
                    {
                        DXService.Log($"{vm.Msg}");
                    }
                }
                break;
            case 3:
                {
                    var vm = DataKitTo.GetTable(cdb.TDB, cdb.Conn);
                    if (vm.Code == 200)
                    {
                        DXService.Log($"{vm.Data.ToJson(true)}");
                    }
                    else
                    {
                        DXService.Log($"{vm.Msg}");
                    }
                }
                break;
        }

        return true;
    }

    [Display(Name = "数据库优化", GroupName = "44")]
    public static bool DatabaseBetter()
    {
        //输出头
        var mi = MethodBase.GetCurrentMethod();
        DXService.ShowTitleInfo(mi);

        //配置
        var co = new ConfigObj();

        //选择库
        var cdb = DXService.ConsoleReadDatabase(co);

        //新数据库
        var db = new DbHelper(DataKitAidTo.DbConn(cdb.TDB, cdb.Conn));

        switch (cdb.TDB)
        {
            case SharedEnum.TypeDB.SQLite:
                break;
            case SharedEnum.TypeDB.MySQL:
            case SharedEnum.TypeDB.MariaDB:
                {
                    var drs = db.SqlQuery("SHOW VARIABLES").Tables[0].Select();

                    var dicVar1 = new Dictionary<string, string>
                        {
                            { "local_infile","是否允许加载本地数据，BulkCopy 需要开启"},
                            { "innodb_lock_wait_timeout","innodb 的 dml 操作的行级锁的等待时间，事务等待获取资源等待的最长时间，BulkCopy 量大超时设置，单位：秒"},

                            { "max_allowed_packet","传输的 packet 大小限制，最大 1G，单位：B"},

                            { "information_schema_stats","缓存中统计信息过期时间，要直接从存储引擎获取统计信息，将其设置为 0，单位：秒"},
                            { "information_schema_stats_expiry","MySQL8，缓存中统计信息过期时间，要直接从存储引擎获取统计信息，将其设置为 0，单位：秒"}
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
                                case "information_schema_stats":
                                case "information_schema_stats_expiry":
                                    if (val != "0")
                                    {
                                        //缓存统计信息实时
                                        listBetterSql.Add($"SET GLOBAL {key} = 0");
                                    }
                                    break;
                            }

                            DXService.Log($"\n{key} -> {val} （{dicVar1[key]}）");
                        }
                    }

                    if (listBetterSql.Count > 0)
                    {
                        DXService.Log($"\n执行优化脚本：\n{string.Join(Environment.NewLine, listBetterSql)}");
                        db.SqlExecute(listBetterSql);
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

        return true;
    }

    [Display(Name = "执行 SQL", GroupName = "44")]
    public static bool Execute()
    {
        //输出头
        var mi = MethodBase.GetCurrentMethod();
        DXService.ShowTitleInfo(mi);

        //配置
        var co = new ConfigObj();

        //选择库
        var cdb = DXService.ConsoleReadDatabase(co);

        var sqlPath = DXService.ConsoleReadPath("脚本路径（sql）：", 1);
        DXService.Log($"开始执行脚本 {sqlPath}");

        //跑表
        var st = new SharedTimingVM();

        var db = new DbHelper(DataKitAidTo.DbConn(cdb.TDB, cdb.Conn));
        var num = db.SqlExecute(FileTo.ReadText(sqlPath));

        DXService.Log($"执行结束，受影响行数：{num}，耗时：{st.PartTimeFormat()}");

        return true;
    }

    [Display(Name = "导入数据库", GroupName = "4411")]
    public static bool ImportDatabase()
    {
        //输出头
        var mi = MethodBase.GetCurrentMethod();
        DXService.ShowTitleInfo(mi);

        //配置
        var co = new ConfigObj();

        //选择库
        var cdb = DXService.ConsoleReadDatabase(co);

        var db = new DbHelper(DataKitAidTo.DbConn(cdb.TDB, cdb.Conn));

        var zipPath = DXService.ConsoleReadPath("导入源（zip）：", 1);

        var clearTable = DXService.ConsoleReadItem("导入前删除表数据？", "不清空表数据,清空表数据".Split(','), 1) == 2;

        var vm = DataKitAidTo.ImportDatabase(cdb.TDB, cdb.Conn, zipPath, clearTable, cce =>
        {
            DXService.Log($"{cce.NewItems[0]}");
        });

        return vm.Code == 200;
    }

    [Display(Name = "导出数据库", GroupName = "4411")]
    public static bool ExportDatabase()
    {
        //输出头
        var mi = MethodBase.GetCurrentMethod();
        DXService.ShowTitleInfo(mi);

        //配置
        var co = new ConfigObj();

        //选择库
        var cdb = DXService.ConsoleReadDatabase(co);

        var tables = new List<string>();
        //指定表
        Console.Write("指定表（默认所有表，多个表逗号分隔）：");
        var tns = Console.ReadLine().Trim();
        if (!string.IsNullOrWhiteSpace(tns))
        {
            tables.AddRange(tns.Split(','));
        }

        var outPath = PathTo.Combine(co.DXHub, $"{cdb.TDB}_{mi.Name}_{DateTime.Now:yyyyMMdd_HHmmss}.zip");

        var vm = DataKitAidTo.ExportDatabase(cdb.TDB, cdb.Conn, outPath, tables, cce =>
        {
            DXService.Log($"{cce.NewItems[0]}");
        });

        return vm.Code == 200;
    }

    [Display(Name = "导出表", GroupName = "4411")]
    public static bool ExportDataTable()
    {
        //输出头
        var mi = MethodBase.GetCurrentMethod();
        DXService.ShowTitleInfo(mi);

        //配置
        var co = new ConfigObj();

        //选择库
        var cdb = DXService.ConsoleReadDatabase(co);

        var sqls = new List<string>();
        //指定表
        Console.Write("查询表脚本（SELECT * FROM table1; SELECT * FROM table2）：");
        var tns = Console.ReadLine().Trim();
        if (!string.IsNullOrWhiteSpace(tns))
        {
            sqls.AddRange(tns.Split(';'));
        }
        sqls = sqls.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

        var outPath = PathTo.Combine(co.DXHub, $"{cdb.TDB}_{mi.Name}_{DateTime.Now:yyyyMMdd_HHmmss}.zip");

        var vm = DataKitAidTo.ExportDataTable(cdb.TDB, cdb.Conn, outPath, sqls, cce =>
        {
            DXService.Log($"{cce.NewItems[0]}");
        });

        return vm.Code == 200;
    }

    [Display(Name = "生成 DDL", GroupName = "44")]
    public static bool BuildTableDDL()
    {
        //输出头
        var mi = MethodBase.GetCurrentMethod();
        DXService.ShowTitleInfo(mi);

        //配置
        var co = new ConfigObj();

        //选择库
        var cdb = DXService.ConsoleReadDatabase(co);

        var vm = DataKitTo.GetTableDDL(cdb.TDB, cdb.Conn);
        if (vm.Code == 200)
        {
            var dic = vm.Data as Dictionary<string, string>;

            DXService.Log($"生成成功，共 {dic.Keys.Count} 张表");

            var sbSql = new StringBuilder();
            foreach (var key in dic.Keys)
            {
                var sql = dic[key];
                sbSql.AppendLine($"-- {key}\n");
                sbSql.Append(sql);
                sbSql.AppendLine(";\n");
            }

            var outPath = PathTo.Combine(co.DXHub, DXService.NewFileName($"{cdb.TDB}_{mi.Name}", ".sql"));

            FileTo.WriteText(sbSql.ToString(), outPath, false);
            DXService.Log($"已写入 {outPath}");
        }

        return true;
    }

    [Display(Name = "生成 Drop Table SQL", GroupName = "44")]
    public static bool BuildDropTable()
    {
        //输出头
        var mi = MethodBase.GetCurrentMethod();
        DXService.ShowTitleInfo(mi);

        //配置
        var co = new ConfigObj();

        //选择库
        var cdb = DXService.ConsoleReadDatabase(co);

        var vm = DataKitTo.GetTable(cdb.TDB, cdb.Conn);
        if (vm.Code == 200)
        {
            var tables = vm.Data as List<TableVM>;

            var listSql = new List<string>();
            foreach (var item in tables)
            {
                var sql = "DROP TABLE";
                listSql.Add($"{sql} {DbHelper.SqlQuote(cdb.TDB, item.TableName)}");
            }
            var sqls = string.Join(";\n", listSql);

            DXService.Log($"生成成功：\n{sqls}");

            var outPath = PathTo.Combine(co.DXHub, DXService.NewFileName($"{cdb.TDB}_{mi.Name}", ".sql"));
            FileTo.WriteText(sqls, outPath, false);

            DXService.Log($"已写入 {outPath}");
        }

        return true;
    }

    [Display(Name = "生成 Truncate Table SQL", GroupName = "44")]
    public static bool BuildTruncateTable()
    {
        //输出头
        var mi = MethodBase.GetCurrentMethod();
        DXService.ShowTitleInfo(mi);

        //配置
        var co = new ConfigObj();

        //选择库
        var cdb = DXService.ConsoleReadDatabase(co);

        var vm = DataKitTo.GetTable(cdb.TDB, cdb.Conn);
        if (vm.Code == 200)
        {
            var tables = vm.Data as List<TableVM>;

            var listSql = new List<string>();
            foreach (var item in tables)
            {
                var sql = cdb.TDB == SharedEnum.TypeDB.SQLite ? "DELETE FROM" : "TRUNCATE TABLE";
                listSql.Add($"{sql} {DbHelper.SqlQuote(cdb.TDB, item.TableName)}");
            }
            var sqls = string.Join(";\n", listSql);

            DXService.Log($"生成成功：\n{sqls}");

            var outPath = PathTo.Combine(co.DXHub, DXService.NewFileName($"{cdb.TDB}_{mi.Name}", ".sql"));
            FileTo.WriteText(sqls, outPath, false);

            DXService.Log($"已写入 {outPath}");
        }

        return true;
    }

    [Display(Name = "生成 表映射（原=>新）", GroupName = "4466")]
    public static bool BuildMappingTable()
    {
        //输出头
        var mi = MethodBase.GetCurrentMethod();
        DXService.ShowTitleInfo(mi);

        //配置
        var co = new ConfigObj();

        //选择原库
        var odb = DXService.ConsoleReadDatabase(co, "设置原库：");

        //选择新库
        var ndb = DXService.ConsoleReadDatabase(co, "设置新库：");

        DXService.Log($"{odb.TDB} => {ndb.TDB}");

        DXService.Log($"读取两个库的表名");

        var odvm = DataKitTo.GetTable(odb.TDB, odb.Conn);
        var ndvm = DataKitTo.GetTable(ndb.TDB, ndb.Conn);

        if (odvm.Code == 200 && ndvm.Code == 200)
        {
            var odTable = odvm.Data as List<TableVM>;
            var ndTable = ndvm.Data as List<TableVM>;

            DXService.Log($"原库 {odTable.Count} 张表，新库 {ndTable.Count} 张表");
            DXService.Log($"开始遍历原库表名");

            var mapping = new Dictionary<string, string>();
            foreach (var odItem in odTable)
            {
                var newTable = string.Empty;
                foreach (var ndItem in ndTable)
                {
                    //完整匹配
                    if (co.MappingTableFullMatch && ndItem.TableName == odItem.TableName)
                    {
                        newTable = ndItem.TableName;
                        DXService.Log($"{odItem.TableName} 完整匹配到 {newTable}");
                        break;
                    }
                    //模糊匹配
                    else if (DXService.FuzzyMatch(ndItem.TableName, odItem.TableName))
                    {
                        newTable = ndItem.TableName;
                        DXService.Log($"{odItem.TableName} 模糊匹配到 {newTable}");
                        break;
                    }
                }

                if (string.IsNullOrEmpty(newTable))
                {
                    DXService.Log($"{odItem.TableName} 未匹配到");
                }

                mapping.Add(odItem.TableName, newTable);
            }

            //表映射文件名
            var MappingTableName = DXService.NewFileName("MappingTable", ".json");
            var MappingTablePath = PathTo.Combine(co.DXHub, MappingTableName);
            DXService.Log($"写入表映射：{MappingTablePath}");
            FileTo.WriteText(mapping.ToJson(true), MappingTablePath, false);
        }

        return true;
    }

    [Display(Name = "生成 列映射（原=>新）", GroupName = "4466")]
    public static bool BuildMappingColumn()
    {
        //输出头
        var mi = MethodBase.GetCurrentMethod();
        DXService.ShowTitleInfo(mi);

        //配置
        var co = new ConfigObj();

        //表映射
        var mtPath = DXService.ConsoleReadPath("表映射配置文件路径：");
        JObject MappingTable = FileTo.ReadText(mtPath).ToJObject();

        //选择原库
        var odb = DXService.ConsoleReadDatabase(co, "设置原库：");

        //选择新库
        var ndb = DXService.ConsoleReadDatabase(co, "设置新库：");

        DXService.Log($"{odb.TDB} => {ndb.TDB}");

        DXService.Log($"读取两个库的所有列");

        var odvm = DataKitTo.GetColumn(odb.TDB, odb.Conn);
        var ndvm = DataKitTo.GetColumn(ndb.TDB, ndb.Conn);

        if (odvm.Code == 200 && ndvm.Code == 200)
        {
            var odColumn = odvm.Data as List<ColumnVM>;
            var ndColumn = ndvm.Data as List<ColumnVM>;

            DXService.Log($"原库 {odColumn.Count} 列，新库 {ndColumn.Count} 列");
            DXService.Log($"开始按原库表名分组遍历列");

            var mapping = new Dictionary<string, Dictionary<string, string>>();

            var odIgs = odColumn.GroupBy(x => x.TableName);
            foreach (var odIg in odIgs)
            {
                //根据表映射配置筛选列
                var ndIgs = ndColumn.Where(x => x.TableName == MappingTable[odIg.Key]?.ToString());

                var mappingOne = new Dictionary<string, string>();
                foreach (var odItem in odIg)
                {
                    var newField = string.Empty;
                    foreach (var ndItem in ndIgs)
                    {
                        //完整匹配
                        if (co.MappingColumnFullMatch && ndItem.ColumnName == odItem.ColumnName)
                        {
                            newField = ndItem.ColumnName;
                            DXService.Log($"{odItem.TableName}.{odItem.ColumnName} 完整匹配到 {ndItem.TableName}.{newField}");
                            break;
                        }
                        //模糊匹配
                        else if (DXService.FuzzyMatch(ndItem.ColumnName, odItem.ColumnName))
                        {
                            newField = ndItem.ColumnName;
                            DXService.Log($"{odItem.TableName}.{odItem.ColumnName} 模糊匹配到 {ndItem.TableName}.{newField}");
                            break;
                        }
                    }

                    if (string.IsNullOrEmpty(newField))
                    {
                        DXService.Log($"{odItem.TableName}.{odItem.ColumnName} 未匹配到");
                    }

                    //添加表的一列
                    mappingOne.Add(odItem.ColumnName, newField);
                }

                //添加一个表
                mapping.Add(odIg.Key, mappingOne);
            }

            var dicmt = new Dictionary<string, string>();
            foreach (var item in MappingTable)
            {
                dicmt.Add(item.Key, item.Value.ToString());
            }
            mapping.Add("__ON_MappingTable", dicmt);

            //列映射文件名
            var MappingColumnName = DXService.NewFileName("MappingColumn", ".json");
            var MappingColumnPath = PathTo.Combine(co.DXHub, MappingColumnName);
            DXService.Log($"写入列映射：{MappingColumnPath}");
            FileTo.WriteText(mapping.ToJson(true), MappingColumnPath, false);
        }

        return true;
    }

    [Display(Name = "表数据转换（原=>新）", GroupName = "4466")]
    public static bool Conversion()
    {
        //输出头
        var mi = MethodBase.GetCurrentMethod();
        DXService.ShowTitleInfo(mi);

        //配置
        var co = new ConfigObj();

        //选择原库
        var odb = DXService.ConsoleReadDatabase(co, "设置原库：");

        //选择新库
        var ndb = DXService.ConsoleReadDatabase(co, "设置新库：");

        DXService.Log($"{odb.TDB} => {ndb.TDB}");

        //列映射
        var mcPath = DXService.ConsoleReadPath("列映射配置文件路径：");
        JObject MappingColumn = FileTo.ReadText(mcPath).ToJObject();

        //表映射
        JToken MappingTable = MappingColumn["__ON_MappingTable"];

        //转换变量
        var cv = new ConversionObj()
        {
            //原库表名
            OdTableName = "",
            //原库表数据查询SQL
            OdQuerySql = "",
            //新库表名
            NdTableName = "",
            //新库表数据清理（为空不清理）
            NdClearTableSql = ""
        };

    Flag2:
        Console.Write("输入原库表名：");
        //原数据库表名
        cv.OdTableName = Console.ReadLine();
        if (!MappingColumn.ContainsKey(cv.OdTableName))
        {
            DXService.Log($"{cv.OdTableName} 原库表名无效");
            goto Flag2;
        }

        //原表数据查询
        var odQuerySql = $"SELECT * FROM {DbHelper.SqlQuote(odb.TDB, cv.OdTableName)}";
        Console.Write($"原表数据查询 SQL（默认 {odQuerySql}）：");
        cv.OdQuerySql = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(cv.OdQuerySql))
        {
            cv.OdQuerySql = odQuerySql;
        }

        //新表名
        if (string.IsNullOrWhiteSpace(cv.NdTableName) && !string.IsNullOrWhiteSpace(cv.OdTableName))
        {
            cv.NdTableName = MappingTable[cv.OdTableName].ToString();
        }

        //新表清空数据脚本（为空时不清空）
        var ndClearTableSql = DbHelper.SqlQuote(ndb.TDB, cv.NdTableName);
        ndClearTableSql = ndb.TDB == SharedEnum.TypeDB.SQLite ? $"DELETE FROM {ndClearTableSql}" : $"TRUNCATE TABLE {ndClearTableSql}";
        Console.Write($"新表清空数据脚本 SQL（如 {ndClearTableSql}）：");
        cv.NdClearTableSql = Console.ReadLine();

        //原表列映射新表列
        JToken mappingColumn = MappingColumn[cv.OdTableName];

        //跑表
        var st = new SharedTimingVM();

        DbConnection odc = DataKitAidTo.DbConn(odb.TDB, odb.Conn);
        DbConnection ndc = DataKitAidTo.DbConn(ndb.TDB, ndb.Conn);
        //原数据库
        var odDB = new DbHelper(odc);
        //新数据库
        var ndDB = new DbHelper(ndc);

        DXService.Log($"开始查询原表，查询语句：{cv.OdQuerySql}");
        //原表数据集
        var odDs = odDB.SqlQuery(cv.OdQuerySql);
        var odDt = odDs.Tables[0];

        DXService.Log($"原表数据共：{odDt.Rows.Count} 行，查询耗时：{st.PartTimeFormat()}");

        //构建新表空数据
        var ndDt = ndDB.SqlEmptyTable($"{DbHelper.SqlQuote(ndb.TDB, cv.NdTableName)}");

        //遍历原表数据 填充到 新表
        foreach (DataRow odDr in odDt.Rows)
        {
            //构建新表一行
            var ndNewRow = ndDt.NewRow();

            //根据原表列映射新表列填充单元格数据
            foreach (DataColumn odDc in odDt.Columns)
            {
                //原表列值
                var odColValue = odDr[odDc.ColumnName];
                if (odColValue is not DBNull)
                {
                    //当前原表列映射的新表列
                    var ndColName = mappingColumn[odDc.ColumnName]?.ToString();
                    if (!string.IsNullOrWhiteSpace(ndColName))
                    {
                        //原表列值 转换类型为 新表列值
                        try
                        {
                            //新表列类型
                            var ndColType = ndDt.Columns[ndColName].DataType;

                            //赋值原表列值
                            ndNewRow[ndColName] = Convert.ChangeType(odColValue, ndColType);
                        }
                        catch (Exception ex)
                        {
                            DXService.Log($"列值转换失败");
                            DXService.Log(ex.ToJson());
                        }
                    }
                }

                //处理未映射的列
                //TO DO
                //示例：某一列设置为当前时间
                //if (odDc.ColumnName == "inputedate")
                //{
                //    ndNewRow[odDc.ColumnName] = DateTime.Now;
                //}
            }

            //新行添加到新表
            ndDt.Rows.Add(ndNewRow.ItemArray);
        }

        DXService.Log($"原表数据填充到新表耗时：{st.PartTimeFormat()}");

        if (!string.IsNullOrWhiteSpace(cv.NdClearTableSql))
        {
            DXService.Log($"开始清空新表，执行脚本：{cv.NdClearTableSql}");
            var num = ndDB.SqlExecute(cv.NdClearTableSql);

            DXService.Log($"返回受影响行数：{num}，执行耗时：{st.PartTimeFormat()}");
        }

        DXService.Log($"开始写入新表，共：{ndDt.Rows.Count} 行");
        switch (ndb.TDB)
        {
            case SharedEnum.TypeDB.SQLite:
                ndDB.BulkBatchSQLite(ndDt, cv.NdTableName);
                break;
            case SharedEnum.TypeDB.MySQL:
            case SharedEnum.TypeDB.MariaDB:
                ndDB.BulkCopyMySQL(ndDt, cv.NdTableName);
                break;
            case SharedEnum.TypeDB.Oracle:
                ndDB.BulkCopyOracle(ndDt, cv.NdTableName);
                break;
            case SharedEnum.TypeDB.SQLServer:
                ndDB.BulkCopySQLServer(ndDt, cv.NdTableName);
                break;
            case SharedEnum.TypeDB.PostgreSQL:
                ndDB.BulkCopyPostgreSQL(ndDt, cv.NdTableName);
                break;
        }

        DXService.Log($"已写入新表，耗时：{st.PartTimeFormat()}");
        DXService.Log($"总共耗时：{st.TotalTimeFormat()}");

        return true;
    }

    #endregion

    #region 66

    [Display(Name = "git pull（有 .git 文件夹）", GroupName = "66")]
    public static void GitPull()
    {
        //输出头
        var mi = MethodBase.GetCurrentMethod();
        DXService.ShowTitleInfo(mi);

        try
        {
            var dp = Environment.CurrentDirectory.TrimEnd('/').TrimEnd('\\');
            var rootPath = DXService.ConsoleReadPath("请输入目录：", 2, dp);

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
                    DXService.Log($"已跳过 \"{sdi.FullName}\" ，未找到 .git\n");
                    c2++;
                }
            });

            DXService.Log($"Done!  Pull：{c1} ，Skip：{c2}");
        }
        catch (Exception ex)
        {
            DXService.Log($"ERROR：{ex.Message}");
        }
    }

    [Display(Name = "AES 加密解密（数据库连接字符串）", GroupName = "66")]
    public static void AESEncryptDecrypt()
    {
        //输出头
        var mi = MethodBase.GetCurrentMethod();
        DXService.ShowTitleInfo(mi);

        var ed = DXService.ConsoleReadItem("请选择：", "Encrypt 加密,Decrypt 解密".Split(','), 1);

        Console.Write($"请输入内容：");
        var content = Console.ReadLine();
        Console.Write($"请输入密码：");
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
            DXService.Log($"ERROR：{ex.Message}");
        }
    }

    [Display(Name = "文本编码转换（请先备份）", GroupName = "66")]
    public static void TextEncodingConversion()
    {
        //输出头
        var mi = MethodBase.GetCurrentMethod();
        DXService.ShowTitleInfo(mi);

        TextEncodingConversionService.Run();
    }

    [Display(Name = "项目清理（删除 bin、obj）", GroupName = "66")]
    public static void ProjectCleanup()
    {
        //输出头
        var mi = MethodBase.GetCurrentMethod();
        DXService.ShowTitleInfo(mi);

        ProjectCleanupService.Run();
    }

    [Display(Name = "项目安全拷贝（替换 appsettings.json 密钥）", GroupName = "66")]
    public static void ProjectSafeCopy()
    {
        //输出头
        var mi = MethodBase.GetCurrentMethod();
        DXService.ShowTitleInfo(mi);

        ProjectSafeCopyService.Run();
    }

    #endregion

    #region 99

    [Display(Name = "ZD 提取数据库文件到磁盘", GroupName = "99")]
    public static void ZDSaveAsFile()
    {
        //输出头
        var mi = MethodBase.GetCurrentMethod();
        DXService.ShowTitleInfo(mi);

        //配置
        var co = new ConfigObj();

        var st = new SharedTimingVM();

        //选择库
        var cdb = DXService.ConsoleReadDatabase(co);

        //数据库
        var db = new DbHelper(DataKitAidTo.DbConn(cdb.TDB, cdb.Conn));

        //新表
        var dt = db.SqlQuery("select top 0 * from cqzd2020.dbo.ANNEX_INFO").Tables[0];

        //附件目录
        Console.Write($"附件存储目录（如 {co.DXHub} ）：");
        var annexPath = Console.ReadLine();
        if (!Directory.Exists(annexPath))
        {
            Directory.CreateDirectory(annexPath);
        }

        //查询脚本
        Console.Write("查询脚本（如 select top 10 * from cqzd2016Annex.dbo.ANNEX_INFO）：");
        var querySql = Console.ReadLine();
        var note = querySql.Contains("cqzd2016Annex") ? "cqzd2016Annex" : "cqzd2020Annex";

        DXService.Log($"执行查询脚本: {querySql}");

        int wi = 0;
        db.SafeConn(() =>
        {
            var cmd = db.Connection.CreateCommand();
            cmd.CommandTimeout = 300;
            cmd.CommandText = querySql;

            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var dr = dt.NewRow();
                byte[] bin = null;
                for (int f = 0; f < reader.FieldCount; f++)
                {
                    //image
                    if (f == 5)
                    {
                        bin = (byte[])reader[f];
                    }
                    else
                    {
                        dr[f] = reader[f];
                    }
                }
                var id = dr["ID"].ToString();
                var suff = dr["SUFFIX_NAME"].ToString().TrimStart('.');
                dr["SUFFIX_NAME"] = suff;
                dr["NOTE"] = note;

                var vpath = id + "." + suff;
                dr["ANNEX_PATH"] = vpath;
                var path = Path.Combine(annexPath, vpath);
                File.WriteAllBytes(path, bin);
                DXService.Log($"写入文件（{++wi}）：{path}");

                dt.Rows.Add(dr.ItemArray);
            }
        });

        DXService.Log($"导出文件（{wi}）耗时: {st.PartTimeFormat()}");

        DXService.Log($"回写文件路径");

        var csb = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(cdb.Conn);
        csb.InitialCatalog = "cqzd2020";
        var dbmain = new DbHelper(DataKitAidTo.DbConn(cdb.TDB, csb.ConnectionString));
        dbmain.BulkCopySQLServer(dt, "ANNEX_INFO");
        DXService.Log($"写入文件路径耗时: {st.PartTimeFormat()}");

        DXService.Log($"共耗时: {st.TotalTimeFormat()}");
    }

    #endregion
}