using MySqlConnector;
using System.Data.SQLite;
using Microsoft.Data.SqlClient;
using Oracle.ManagedDataAccess.Client;
using Npgsql;
using System;
using System.IO;
using System.Linq;
using System.Data;
using System.Reflection;
using System.Data.Common;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.Loader;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;
using Netnr.Core;
using Netnr.SharedAdo;
using Netnr.DataX.Domain;
using Netnr.SharedDataKit;

namespace Netnr.DataX.Application
{
    /// <summary>
    /// 菜单
    /// </summary>
    public partial class MenuService
    {
        [Description("退出")]
        public static void Exit()
        {
            Environment.Exit(0);
        }

        [Description("设置配置文件")]
        public static void SetConfig()
        {
            var listConfig = DXService.GetConfigList();
            if (listConfig.Count == 0)
            {
                DXService.Log($"未找到配置文件 config*.json");
            }
            else
            {
                DXService.Log($"配置文件列表：{Environment.NewLine}");
                for (int i = 0; i < listConfig.Count; i++)
                {
                    DXService.Log($"    {i + 1}、{listConfig[i]}{Environment.NewLine}");
                }
            Flag1:
                Console.Write("请输入数字设置配置：");
                var cnNum = Console.ReadLine();
                if (!(int.TryParse(cnNum, out int ki) && ki > 0 && ki <= listConfig.Count))
                {
                    DXService.Log($"{cnNum} 无效");
                    goto Flag1;
                }
                else
                {
                    var cnName = listConfig[ki - 1];
                    DXService.ConfigName = cnName;
                    DXService.Log($"已选择 {cnName}");
                }
            }
        }

        [Description("查询配置文件")]
        public static void GetConfig()
        {
            if (string.IsNullOrWhiteSpace(DXService.ConfigName))
            {
                DXService.Log($"{Environment.NewLine}【未设置配置文件，请先设置配置文件】");
            }
            else
            {
                var co = new ConfigObj();
                DXService.Log($"{Environment.NewLine}{co.Init.ToJson(true)}{Environment.NewLine}{Environment.NewLine}{co.Config.ToJson(true)}");
                DXService.Log($"{Environment.NewLine}已选择配置文件：【{DXService.ConfigName}】{Environment.NewLine}{co.OdTypeDB} => {co.NdTypeDB}，dx 路径：{co.DXPath}");
            }
        }

        [Description("生成表映射")]
        public static bool MappingTable()
        {
            //已设置配置文件
            if (string.IsNullOrWhiteSpace(DXService.ConfigName))
            {
                GetConfig();
                return false;
            }

            //配置
            var co = new ConfigObj();
            DXService.Log($"{DateTime.Now:F} 生成表映射（{co.OdTypeDB} => {co.NdTypeDB}）");

            DXService.Log($"读取两个库的表名");
            var odvm = DataKitTo.GetTable(co.OdTypeDB, co.OdConn);
            var ndvm = DataKitTo.GetTable(co.NdTypeDB, co.NdConn);

            if (odvm.Code == 200 && ndvm.Code == 200)
            {
                var odTable = odvm.Data as List<TableNameVM>;
                var ndTable = ndvm.Data as List<TableNameVM>;

                DXService.Log($"原库 {odTable.Count} 张表，新库 {ndTable.Count} 张表");
                DXService.Log($"开始遍历原库表名");

                var mapping = new Dictionary<string, string>();
                foreach (var odItem in odTable)
                {
                    var newTable = string.Empty;
                    foreach (var ndItem in ndTable)
                    {
                        //完整匹配
                        if (co.MappingFullMatchTable && ndItem.TableName == odItem.TableName)
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

                DXService.Log($"写入表映射");
                FileTo.WriteText(mapping.ToJson(true), PathTo.Combine(co.DXPath, co.ConfigFolder, co.MappingTableName), false);
            }

            return true;
        }

        [Description("生成列映射")]
        public static bool MappingColumn()
        {
            //已设置配置文件
            if (string.IsNullOrWhiteSpace(DXService.ConfigName))
            {
                GetConfig();
                return false;
            }

            //配置
            var co = new ConfigObj();
            DXService.Log($"{DateTime.Now:F} 生成列映射（{co.OdTypeDB} => {co.NdTypeDB}）");

            DXService.Log($"读取两个库的所有列");
            var odvm = DataKitTo.GetColumn(co.OdTypeDB, co.OdConn);
            var ndvm = DataKitTo.GetColumn(co.NdTypeDB, co.NdConn);
            if (odvm.Code == 200 && ndvm.Code == 200)
            {
                var odColumn = odvm.Data as List<TableColumnVM>;
                var ndColumn = ndvm.Data as List<TableColumnVM>;

                DXService.Log($"原库 {odColumn.Count} 列，新库 {ndColumn.Count} 列");
                DXService.Log($"开始按原库表名分组遍历列");

                var mapping = new Dictionary<string, Dictionary<string, string>>();

                var odIgs = odColumn.GroupBy(x => x.TableName);
                foreach (var odIg in odIgs)
                {
                    //根据表映射配置筛选列
                    var ndIgs = ndColumn.Where(x => x.TableName == co.MappingTable[odIg.Key]?.ToString());
                    DXService.Log($"原库 {odIg.Key} 表从 {co.MappingTableName} 表映射配置取得新库 {ndIgs.Count()} 列");

                    var mappingOne = new Dictionary<string, string>();
                    foreach (var odItem in odIg)
                    {
                        var newField = string.Empty;
                        foreach (var ndItem in ndIgs)
                        {
                            //完整匹配
                            if (co.MappingFullMatchColumn && ndItem.FieldName == odItem.FieldName)
                            {
                                newField = ndItem.FieldName;
                                DXService.Log($"{odItem.TableName}.{odItem.FieldName} 完整匹配到 {ndItem.TableName}.{newField}");
                                break;
                            }
                            //模糊匹配
                            else if (DXService.FuzzyMatch(ndItem.FieldName, odItem.FieldName))
                            {
                                newField = ndItem.FieldName;
                                DXService.Log($"{odItem.TableName}.{odItem.FieldName} 模糊匹配到 {ndItem.TableName}.{newField}");
                                break;
                            }
                        }

                        if (string.IsNullOrEmpty(newField))
                        {
                            DXService.Log($"{odItem.TableName}.{odItem.FieldName} 未匹配到");
                        }

                        //添加表的一列
                        mappingOne.Add(odItem.FieldName, newField);
                    }

                    //添加一个表
                    mapping.Add(odIg.Key, mappingOne);
                }

                DXService.Log($"写入列映射");
                FileTo.WriteText(mapping.ToJson(true), PathTo.Combine(co.DXPath, co.ConfigFolder, co.MappingColumnName), false);
            }

            return true;
        }

        /// <summary>
        /// 表数据转换
        /// </summary>
        [Description("（原=>新）表数据转换")]
        public static bool Conversion()
        {
            //已设置配置文件
            if (string.IsNullOrWhiteSpace(DXService.ConfigName))
            {
                GetConfig();
                return false;
            }

            //配置
            var co = new ConfigObj();
            DXService.Log($"{DateTime.Now:F} 表数据转换（{co.OdTypeDB} => {co.NdTypeDB}）");

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
            Console.Write("输入原数据库表名：");
            //原数据库表名
            cv.OdTableName = Console.ReadLine();
            if (!co.MappingTable.ContainsKey(cv.OdTableName))
            {
                DXService.Log($"{cv.OdTableName} 表名无效");
                goto Flag2;
            }

            //原表数据查询
            var odQuerySql = $"select * from {cv.OdTableName}";
            Console.Write($"原表数据查询 SQL（默认 {odQuerySql}）：");
            cv.OdQuerySql = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(cv.OdQuerySql))
            {
                cv.OdQuerySql = odQuerySql;
            }

            //新表名
            if (string.IsNullOrWhiteSpace(cv.NdTableName) && !string.IsNullOrWhiteSpace(cv.OdTableName))
            {
                cv.NdTableName = co.MappingTable[cv.OdTableName].ToString();
            }

            //新表清空数据脚本（为空时不清空）
            var ndClearTableSql = DataKitAidTo.SqlQuote(co.NdTypeDB, cv.NdTableName);
            ndClearTableSql = co.NdTypeDB == SharedEnum.TypeDB.SQLite ? $"delete from {ndClearTableSql}" : $"truncate table {ndClearTableSql}";
            Console.Write($"新表清空数据脚本 SQL（如 {ndClearTableSql}）：");
            cv.NdClearTableSql = Console.ReadLine();

            //原表列映射新表列
            JToken mapping = co.MappingColumn[cv.OdTableName];

            //跑表
            var sw = new Stopwatch();
            double et = 0;
            double pt = 0;
            sw.Start();

            DXService.Log($"原数据库连接：{co.OdConn}");
            DXService.Log($"新数据库连接：{co.NdConn}");

            DbConnection odc = DataKitAidTo.SqlConn(co.OdTypeDB, co.OdConn);
            DbConnection ndc = DataKitAidTo.SqlConn(co.NdTypeDB, co.NdConn);
            //原数据库
            var odDB = new DbHelper(odc);
            //新数据库
            var ndDB = new DbHelper(ndc);

            //检查是否允许本地加载数据
            if (co.NdTypeDB == SharedEnum.TypeDB.MySQL)
            {
                DXService.Log("检查 MySQL 是否允许加载本地数据");
                var isAllow = ndDB.SqlQuery("SHOW VARIABLES LIKE 'local_infile'").Tables[0].Rows[0][1].ToString() == "ON";
                if (!isAllow)
                {
                    DXService.Log("MySQL 未允许加载本地数据，设置允许命令如下：");
                    DXService.Log("SET GLOBAL local_infile=1;  -- 1表示开启，0表示关闭");

                    //尝试开启
                    Console.Write("尝试设置未允许加载本地数据（默认 Y）：");
                    var rv = Console.ReadLine().ToLower().Trim();
                    if (rv == "" || rv.StartsWith("y"))
                    {
                        var num = ndDB.SqlExecute("SET GLOBAL local_infile=1");
                        DXService.Log($"已执行，返回受影响行数：{num}");
                    }
                    else
                    {
                        DXService.Log("已忽略设置加载本地数据");
                    }
                }
                else
                {
                    DXService.Log("MySQL 已允许加载本地数据");
                }
            }

            DXService.Log($"开始查询原表，查询语句：{cv.OdQuerySql}");
            //原表数据集
            var odDs = odDB.SqlQuery(cv.OdQuerySql);
            var odDt = odDs.Tables[0];

            pt = sw.Elapsed.TotalMilliseconds - et;
            et = sw.Elapsed.TotalMilliseconds;
            DXService.Log($"原表数据共：{odDt.Rows.Count} 行，查询耗时：{ pt} ms");

            //构建新表空数据
            var ndDt = ndDB.SqlEmptyTable($"{DataKitAidTo.SqlQuote(co.NdTypeDB, cv.NdTableName)}");

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
                        var ndColName = mapping[odDc.ColumnName]?.ToString();
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

            pt = sw.Elapsed.TotalMilliseconds - et;
            et = sw.Elapsed.TotalMilliseconds;
            DXService.Log($"原表数据填充到新表耗时：{pt} ms");

            if (!string.IsNullOrWhiteSpace(cv.NdClearTableSql))
            {
                DXService.Log($"开始清空新表，执行脚本：{cv.NdClearTableSql}");
                var num = ndDB.SqlExecute(cv.NdClearTableSql);

                pt = sw.Elapsed.TotalMilliseconds - et;
                et = sw.Elapsed.TotalMilliseconds;
                DXService.Log($"返回受影响行数：{num}，执行耗时：{pt} ms");
            }

            DXService.Log($"开始写入新表，共：{ndDt.Rows.Count} 行");
            switch (co.NdTypeDB)
            {
                case SharedEnum.TypeDB.SQLite:
                    ndDB.BulkBatchSQLite(ndDt, cv.NdTableName);
                    break;
                case SharedEnum.TypeDB.MySQL:
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

            pt = sw.Elapsed.TotalMilliseconds - et;
            et = sw.Elapsed.TotalMilliseconds;
            DXService.Log($"已写入新表，耗时：{pt} ms");
            DXService.Log($"总共耗时：{et} ms");

            return true;
        }

        [Description("动态编译运行")]
        public static bool CompileRun()
        {
            //已设置配置文件
            if (string.IsNullOrWhiteSpace(DXService.ConfigName))
            {
                GetConfig();
                return false;
            }

            //配置
            var co = new ConfigObj();
            DXService.Log($"{DateTime.Now:F} 动态编译运行（{co.OdTypeDB} => {co.NdTypeDB}）");

            //编译配置
            var cp = co.Init["compile"];
            var listUsing = (cp["using"] as JArray).ToList().Select(x => x.ToString()).ToList();
            var className = cp["className"].ToString();
            var methodName = cp["methodName"].ToString();
            var code = "";

            var keys = co.Compile.Keys.ToList();
            if (keys.Count == 0)
            {
                DXService.Log($"未找到动态编译文件 *.cs");
                return false;
            }
            else
            {
                DXService.Log($"动态编译列表：{Environment.NewLine}");
                for (int i = 0; i < keys.Count; i++)
                {
                    DXService.Log($"    {i + 1}、{keys[i]}{Environment.NewLine}");
                }
            Flag1:
                Console.Write("请输入数字动态编译并运行：");
                var csNum = Console.ReadLine();
                if (!(int.TryParse(csNum, out int ki) && ki > 0 && ki <= keys.Count))
                {
                    DXService.Log($"{csNum} 无效");
                    goto Flag1;
                }
                else
                {
                    var csName = keys[ki - 1];
                    code = co.Compile[csName];
                    DXService.Log($"已选择 {csName}");
                }
            }

            // 动态编译

            var listUseObj = new List<Type>()
            {
                typeof(object),
                typeof(Enumerable),
                typeof(Console),
                typeof(JObject),
                typeof(DataSet),
                typeof(ConsoleTo),
                typeof(TypeConverter),
                typeof(MySqlConnection),
                typeof(SQLiteConnection),
                typeof(OracleConnection),
                typeof(NpgsqlConnection),
                typeof(SqlConnection),
                MethodBase.GetCurrentMethod().DeclaringType
            };

            //引用对象
            var references = new List<MetadataReference>();
            listUseObj.ForEach(uo =>
            {
                references.Add(MetadataReference.CreateFromFile(uo.Assembly.Location));
            });

            //载入引用
            var sdkPath = Path.GetDirectoryName(listUseObj.First().Assembly.Location);
            var defaultUsing = "System.Runtime.dll,System.Collections.dll,netstandard.dll".Split(',');
            listUsing.AddRange(defaultUsing);
            listUsing.ForEach(us =>
            {
                if (!string.IsNullOrWhiteSpace(us))
                {
                    var ffPath = Path.Combine(sdkPath, us);
                    if (!File.Exists(ffPath))
                    {
                        ffPath = Path.Combine(AppContext.BaseDirectory, us);
                        if (!File.Exists(ffPath))
                        {
                            ffPath = null;
                        }
                    }
                    if (ffPath != null && references.Any(x => x.Display != ffPath))
                    {
                        references.Add(MetadataReference.CreateFromFile(ffPath));
                    }
                }
            });

            DXService.Log($"载入引用：{Environment.NewLine}{references.Select(x => Path.GetFileName(x.Display)).ToJson(true)}");

            string assemblyName = Path.GetRandomFileName();
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);
            CSharpCompilation compilation = CSharpCompilation.Create(assemblyName, syntaxTrees: new[] { syntaxTree }, references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using var ms = new MemoryStream();
            DXService.Log($"编译...");
            EmitResult result = compilation.Emit(ms);

            // Compilation result
            if (!result.Success)
            {
                DXService.Log($"编译失败");
                IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                    diagnostic.IsWarningAsError ||
                    diagnostic.Severity == DiagnosticSeverity.Error);

                foreach (Diagnostic diagnostic in failures)
                {
                    DXService.Log($"{diagnostic.Id}：{diagnostic.GetMessage()}");
                }
            }
            else
            {
                DXService.Log($"编译成功");
                ms.Seek(0, SeekOrigin.Begin);

                Assembly assembly = AssemblyLoadContext.Default.LoadFromStream(ms);
                var type = assembly.GetType(className);
                var instance = assembly.CreateInstance(className);
                var meth = type.GetMember(methodName).First() as MethodInfo;
                DXService.Log($"运行已编译方法 => {className}.{methodName}(){Environment.NewLine}");
                meth.Invoke(instance, null);
            }

            return true;
        }
    }
}
