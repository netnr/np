using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using ClickHouse.Client.ADO;
using ClickHouse.Client.Copy;
using System.Text.RegularExpressions;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.InteropServices;

namespace Netnr.DataX.Application
{
    /// <summary>
    /// 辅助类
    /// </summary>
    public partial class DXService
    {
        /// <summary>
        /// 菜单项类型
        /// </summary>
        public static Type MethodType { get; set; } = typeof(MenuItemService);

        private static List<MethodModel> allMethod;
        /// <summary>
        /// 方法对象
        /// </summary>
        public static List<MethodModel> AllMethod
        {
            get
            {
                if (allMethod == null)
                {
                    allMethod = new List<MethodModel>();

                    var cms = MethodType.GetMethods().ToList();
                    var mm = cms.First().Module;
                    cms = cms.Where(x => x.Module == mm).ToList();

                    for (int i = 0; i < cms.Count; i++)
                    {
                        var mi = cms[i];
                        var nameArgs = mi.CustomAttributes.LastOrDefault()?.NamedArguments;
                        if (nameArgs != null)
                        {
                            var model = new MethodModel
                            {
                                GroupName = nameArgs.FirstOrDefault(x => x.MemberName == "GroupName").TypedValue.Value?.ToString(),
                                Name = nameArgs.FirstOrDefault(x => x.MemberName == "Name").TypedValue.Value?.ToString(),
                                ShortName = nameArgs.FirstOrDefault(x => x.MemberName == "ShortName").TypedValue.Value?.ToString(),
                                Description = nameArgs.FirstOrDefault(x => x.MemberName == "Description").TypedValue.Value?.ToString(),
                                Prompt = nameArgs.FirstOrDefault(x => x.MemberName == "Prompt").TypedValue.Value?.ToString(),
                                Method = mi
                            };

                            var autoGenerateFilter = nameArgs.FirstOrDefault(x => x.MemberName == "AutoGenerateFilter").TypedValue.Value;
                            if (autoGenerateFilter != null && (bool)autoGenerateFilter == true)
                            {
                                model.NewLine = "\r\n";
                            }
                            if (!string.IsNullOrWhiteSpace(model.ShortName))
                            {
                                model.Action = model.ShortName.Split(' ')[0];
                            }

                            allMethod.Add(model);
                        }
                    }

                    //每组末尾换行
                    allMethod.GroupBy(x => x.GroupName).ToList().ForEach(ig =>
                    {
                        ig.Last().NewLine = "\r\n";
                    });
                }
                return allMethod;
            }
        }

        /// <summary>
        /// 启动参数
        /// </summary>
        public static List<string> Args { get; set; } = BaseTo.CommandLineArgs;

        /// <summary>
        /// 获取变量名
        /// </summary>
        /// <param name="name">名称（带横线），支持逗号分割多个</param>
        /// <param name="tip">输入提示</param>
        /// <returns></returns>
        public static string VarName(string name, string tip, IList<string> items = null, int? dv = 1)
        {
            string result = null;
            if (BaseTo.IsWithArgs)
            {
                var nameArray = name.Split(',');
                foreach (var nameItem in nameArray)
                {
                    if (!string.IsNullOrWhiteSpace(nameItem))
                    {
                        var keyIndex = Args.IndexOf(nameItem);
                        if (keyIndex != -1 && ++keyIndex < Args.Count)
                        {
                            var val = Args[keyIndex];
                            if (!val.StartsWith("-"))
                            {
                                result = val;
                                break;
                            }
                        }
                    }
                }
            }
            else if (items != null)
            {
                var itemIndex = ConsoleReadItem(tip, items, dv);
                result = items[itemIndex - 1];
            }
            else
            {
                Console.Write(TipSymbol(tip));
                result = Console.ReadLine();
            }

            return result;
        }

        /// <summary>
        /// 获取变量
        /// </summary>
        /// <param name="index">静默带参索引，从 0 开始</param>
        /// <param name="tip">输入提示</param>
        /// <returns></returns>
        public static string VarString(int index, string tip)
        {
            string val;
            if (BaseTo.IsWithArgs)
            {
                val = index < Args.Count ? Args[index] : "";
            }
            else
            {
                Console.Write(TipSymbol(tip));
                val = Console.ReadLine();
            }
            return val.Trim();
        }

        /// <summary>
        /// 获取变量
        /// </summary>
        /// <param name="index">静默带参索引，从 0 开始</param>
        /// <param name="tip">输入提示</param>
        /// <param name="items">选项</param>
        /// <param name="dv"></param>
        /// <returns></returns>
        public static string VarString(int index, string tip, IList<string> items, int? dv = 1)
        {
            string val;
            if (BaseTo.IsWithArgs)
            {
                val = index < Args.Count ? Args[index] : "";
            }
            else
            {
                var itemIndex = ConsoleReadItem(tip, items, dv);
                val = items[itemIndex - 1];
            }
            return val.Trim();
        }

        /// <summary>
        /// 获取变量
        /// </summary>
        /// <param name="tip"></param>
        /// <param name="has">多个支持逗号分割</param>
        /// <returns></returns>
        public static bool VarBool(string tip, string has = "-y")
        {
            bool val;
            if (BaseTo.IsWithArgs)
            {
                val = has.Split(',').Any(x => !string.IsNullOrWhiteSpace(x) && Args.Contains(x));
            }
            else
            {
                val = ConsoleReadBool(tip);
            }
            return val;
        }

        /// <summary>
        /// 控制台运行
        /// </summary>
        /// <param name="groupName">分组，为空时显示所有，为斜杠时 / 显示分组，为 /Data 时显示某一组</param>
        /// <param name="isAgain"></param>
        /// <returns></returns>
        public static async Task RunOfConsole(string groupName = "/", bool isAgain = true)
        {
            if (groupName == "/")
            {
                //分组

                var groupKeys = AllMethod.Select(x => x.GroupName).Distinct().ToList();
                groupKeys.Insert(0, "All\r\n");

                var itemIndex = ConsoleReadItem("Please choose", groupKeys, 1);
                if (itemIndex == 1)
                {
                    //所有
                    await RunOfConsole("");
                }
                else
                {
                    var groupKey = groupKeys[itemIndex - 1];
                    await RunOfConsole($"/{groupKey}");
                }
            }
            else if (groupName.StartsWith("/"))
            {
                //一组

                groupName = groupName.TrimStart('/');
                var groupMethod = AllMethod.Where(x => x.GroupName == groupName).ToList();
                groupMethod.Insert(0, new MethodModel
                {
                    Name = "Back to main menu",
                    Description = "返回主菜单",
                    NewLine = "\r\n"
                });
                var methodKeys = groupMethod.Select(x => x.GetViewName()).ToList();

                var itemIndex = ConsoleReadItem("Please choose", methodKeys, 1);
                if (itemIndex == 1)
                {
                    //返回主菜单
                    await RunOfConsole();
                }
                else
                {
                    var method = groupMethod[itemIndex - 1].Method;
                    if (method.Invoke(MethodType, null) is Task mr)
                    {
                        await mr;
                    }
                }
            }
            else
            {
                //所有

                var methodKeys = AllMethod.Select(x => x.GetViewName()).ToList();
                methodKeys.Insert(0, "Back to main menu 返回主菜单");

                var itemIndex = ConsoleReadItem("Please choose", methodKeys, 1);
                if (itemIndex == 1)
                {
                    //返回主菜单
                    await RunOfConsole();
                }
                else
                {
                    var method = allMethod[itemIndex - 2].Method;
                    if (method.Invoke(MethodType, null) is Task mr)
                    {
                        await mr;
                    }
                }
            }

            if (isAgain)
            {
                Thread.Sleep(1500);
                await RunOfConsole("/", isAgain);
            }
        }

        /// <summary>
        /// 静默运行
        /// </summary>
        /// <returns></returns>
        public static async Task RunOfSilence()
        {
            //仅保留参数
            Args.RemoveAt(0);
            var action = Args[0];
            Args.RemoveAt(0);

            if (action.StartsWith("/") && Args.Count == 0)
            {
                //help group
                var groupMethod = AllMethod.Where(x => x.GroupName == action.TrimStart('/')).ToList();
                if (groupMethod.Count == 0)
                {
                    Log($"{action} 分组无效");
                }
                else
                {
                    foreach (var mo in groupMethod)
                    {
                        mo.GetRunPrompt();
                    }
                }
            }
            else
            {
                var mo = AllMethod.FirstOrDefault(x => x.Action == action);
                if (mo != null)
                {
                    if (Args.Count == 1 && new string[] { "?", "/?", "-?", "-h", "/help", "-help", "--help" }.Contains(Args[0]))
                    {
                        //help on
                        mo.GetRunPrompt();
                    }
                    else if (mo.Method.Invoke(MethodType, null) is Task mr)
                    {
                        //run one
                        await mr;
                    }
                }
                else
                {
                    //help all
                    AllMethod.ForEach(mo =>
                    {
                        if (!string.IsNullOrWhiteSpace(mo.Action))
                        {
                            mo.GetRunPrompt();
                        }
                    });
                }
            }
        }

        /// <summary>
        /// 异常
        /// </summary>
        /// <param name="ex"></param>
        public static void Log(Exception ex)
        {
            Log(ex.ToJson(true), ConsoleColor.Red);
        }

        /// <summary>
        /// 日志
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="cc"></param>
        public static void Log(string msg, ConsoleColor? cc = null)
        {
            if (cc != null)
            {
                Console.ForegroundColor = cc.Value;
                Console.WriteLine(msg);
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                Console.WriteLine(msg);
            }

            ConsoleTo.Log(msg);
        }

        /// <summary>
        /// 输出标题
        /// </summary>
        /// <param name="title">标题</param>
        /// <returns></returns>
        public static void OutputTitle(string title) => Log($"\r\n------- {title}", ConsoleColor.Cyan);

        /// <summary>
        /// 提示符号
        /// </summary>
        /// <param name="tip"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static string TipSymbol(string tip, string symbol = ": ")
        {
            return $"\r\n{tip.TrimEnd('：').TrimEnd(':')}{symbol}";
        }

        /// <summary>
        /// 新建文件名
        /// </summary>
        /// <param name="prefix">前缀</param>
        /// <param name="ext">后缀，如 .zip</param>
        /// <returns></returns>
        public static string NewFileName(object prefix, string ext)
        {
            return $"{prefix}_{DateTime.Now:yyyyMMdd_HHmmss}{ext}";
        }

        /// <summary>
        /// 解析路径变量
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ParsePathVar(string str)
        {
            string pattern = @"({\w+})";
            var now = DateTime.Now;

            var ci = new ConfigInit();

            var path = new Regex(pattern).Replace(str, o =>
            {
                var format = o.Groups[1].Value[1..^1];
                return DateTime.Now.ToString(format);
            }).Replace("~", ci.DXHub);

            return path;
        }

        /// <summary>
        /// 重试
        /// </summary>
        /// <param name="action"></param>
        public static async Task TryAgain(Func<Task> action)
        {
            try
            {
                await action();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Log(ex.Message);

                if (ConsoleReadBool("\r\nTry Again"))
                {
                    await TryAgain(action);
                }
            }
        }

        /// <summary>
        /// [Remark](MySQL://Conn)
        /// </summary>
        /// <returns></returns>
        [GeneratedRegex(@"^\[(.*?)\]\((.*?):\/\/(.*?)\)$", RegexOptions.Compiled | RegexOptions.Singleline)]
        private static partial Regex MatchConnUriFull();

        /// <summary>
        /// MySQL://Conn
        /// </summary>
        /// <returns></returns>
        [GeneratedRegex(@"^(.*?):\/\/(.*?)$", RegexOptions.Compiled | RegexOptions.Singleline)]
        private static partial Regex MatchConnUriShort();

        /// <summary>
        /// 输入数据库
        /// </summary>
        /// <param name="configOption"></param>
        public static async Task<DbKitConnectionOption> ConsoleReadDatabase(ConfigOption configOption, string tip = "请选择数据库连接")
        {
            var connOption = new DbKitConnectionOption();

            await ConsoleTo.ReadRetry(async () =>
            {
                var allDbConns = configOption.ListConnectionInfo;

                var ckey = "Database-Conns";
                var tmpDbConns = CacheTo.Get<List<DbKitConnectionOption>>(ckey);
                if (tmpDbConns != null)
                {
                    allDbConns = configOption.ListConnectionInfo.Concat(tmpDbConns).ToList();
                }
                else
                {
                    tmpDbConns = new List<DbKitConnectionOption>();
                }

                Console.WriteLine($"\n{0,5}. 输入数据库连接信息");
                for (int i = 0; i < allDbConns.Count; i++)
                {
                    var obj = allDbConns[i];
                    Console.WriteLine($"{i + 1,5}. [{obj.ConnectionRemark}]({obj.ConnectionType}://{obj.ConnectionString})");
                }
                Console.Write(TipSymbol(tip));

                //读取选择的连接序号
                var connIndex = Convert.ToInt32(Console.ReadLine().Trim());

                //输入新的连接
                if (connIndex == 0)
                {
                    await ConsoleTo.ReadRetry(() =>
                    {
                        Console.Write(TipSymbol("连接格式 [Remark](MySQL://Conn)"));
                        var readConnUri = Console.ReadLine().Trim();

                        var mr = MatchConnUriFull().Match(readConnUri);
                        if (mr.Success || (mr = MatchConnUriShort().Match(readConnUri)).Success)
                        {
                            if (mr.Groups.Count == 3)
                            {
                                connOption.ConnectionRemark = $"TMP_{RandomTo.NewNumber()}";
                                connOption.ConnectionType = mr.Groups[1].ToString().DeEnum<EnumTo.TypeDB>();
                                connOption.ConnectionString = mr.Groups[2].ToString();
                            }
                            else
                            {
                                connOption.ConnectionRemark = mr.Groups[1].ToString();
                                connOption.ConnectionType = mr.Groups[2].ToString().DeEnum<EnumTo.TypeDB>();
                                connOption.ConnectionString = mr.Groups[3].ToString();
                            }

                            //缓存
                            tmpDbConns.Add(connOption);
                            CacheTo.Set(ckey, tmpDbConns);
                        }

                        return Task.FromResult(mr.Success);
                    });
                }
                else
                {
                    //选择连接序号
                    connOption = allDbConns[connIndex - 1];
                }

                connOption.DeepCopyNewInstance = true;

                //选择数据库名
                switch (connOption.ConnectionType)
                {
                    case EnumTo.TypeDB.MySQL:
                    case EnumTo.TypeDB.MariaDB:
                    case EnumTo.TypeDB.SQLServer:
                    case EnumTo.TypeDB.PostgreSQL:
                        {
                            var dk = DataKitTo.CreateDkInstance(connOption);
                            var listDatabaseName = await dk.GetDatabaseName();

                            var dv = 1;
                            if (!string.IsNullOrWhiteSpace(connOption.DatabaseName))
                            {
                                dv = listDatabaseName.IndexOf(connOption.DatabaseName) + 1;
                            }

                            var cri = ConsoleReadItem(TipSymbol($"选择数据库名"), listDatabaseName, dv);
                            connOption.DatabaseName = listDatabaseName[cri - 1];
                            connOption.SetConnDatabaseName(connOption.DatabaseName);
                        }
                        break;
                }

                Log($"\n已选择 [{connOption.ConnectionRemark}]({connOption.ConnectionType}://{connOption.ConnectionString})\n", ConsoleColor.Cyan);

                return true;
            });

            return connOption;
        }

        /// <summary>
        /// 输入文件（夹）
        /// </summary>
        /// <param name="tip">提示文字</param>
        /// <param name="type">默认（0：都可以；1：文件；2：文件夹）</param>
        /// <param name="dv">默认文件（夹）</param>
        /// <param name="mustExist">是否必须存在</param>
        public static string ConsoleReadPath(string tip, int type = 1, string dv = null, bool mustExist = true)
        {
        Flag1:
            var dtip = string.IsNullOrWhiteSpace(dv) ? TipSymbol(tip) : TipSymbol(tip, $"(default: {dv}): ");
            Console.Write(dtip);
            var path = Console.ReadLine().Trim();
            if (!string.IsNullOrWhiteSpace(dv) && string.IsNullOrWhiteSpace(path))
            {
                path = dv;
            }
            else if (mustExist)
            {
                if ((type == 1 && !File.Exists(path)) || (type == 2 && !Directory.Exists(path)) || (type == 0 && !File.Exists(path) && !Directory.Exists(path)))
                {
                    Log($"{path} 无效文件（夹）");
                    goto Flag1;
                }
            }

            return path;
        }

        /// <summary>
        /// 输入选择项，从 1 开始
        /// </summary>
        /// <param name="tip">提示文字</param>
        /// <param name="items">项</param>
        /// <param name="dv">默认 1</param>
        public static int ConsoleReadItem(string tip, IList<string> items, int? dv = 1)
        {
            var itemIndex = -1;
            ConsoleTo.ReadRetry(() =>
            {
                Console.WriteLine("");
                for (int j = 0; j < items.Count; j++)
                {
                    if (j == items.Count - 1)
                    {
                        Console.WriteLine($"{j + 1,5}. {items[j].Trim()}");
                    }
                    else
                    {
                        Console.WriteLine($"{j + 1,5}. {items[j]}");
                    }
                }
                Console.Write(TipSymbol(tip, dv.HasValue ? $"(default: {dv}): " : ": "));

                var ii = Console.ReadLine()?.Trim();
                //默认项
                if (dv.HasValue && string.IsNullOrWhiteSpace(ii))
                {
                    Log($"\r\nChosen {dv}. {items[dv.Value - 1].Trim()}", ConsoleColor.Cyan);
                    itemIndex = dv.Value;
                }
                else
                {
                    var si = Convert.ToInt32(ii);
                    if (si > 0 && si <= items.Count)
                    {
                        Log($"\r\nChosen {si}. {items[si - 1].Trim()}", ConsoleColor.Cyan);
                        itemIndex = si;
                    }
                }

                return Task.FromResult(itemIndex != -1);
            }).GetAwaiter().GetResult();

            return itemIndex;
        }

        /// <summary>
        /// 输入是否
        /// </summary>
        /// <param name="tip">提示文字</param>
        public static bool ConsoleReadBool(string tip)
        {
            Console.Write($"{tip}? [y(1)/N(default)]: ");
            return new[] { "y", "1" }.Contains(Console.ReadLine().ToLower().Trim());
        }

        /// <summary>
        /// 输入数字
        /// </summary>
        /// <param name="tip">提示文字</param>
        /// <param name="dv">默认 1 </param>
        public static int ConsoleReadNumber(string tip, int dv = 1)
        {
            Console.Write(TipSymbol(tip, $"(default: {dv}): "));

            var ii = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(ii))
            {
                Log($"\nNumber {dv}\n", ConsoleColor.Cyan);
                return dv;
            }
            else
            {
                _ = int.TryParse(ii, out int i);
                Log($"\nNumber {i}\n", ConsoleColor.Cyan);
                return i;
            }
        }

        /// <summary>
        /// 搜索删除
        /// </summary>
        /// <param name="rootPath">根目录</param>
        /// <param name="listSearch">搜索</param>
        /// <param name="listIgnore">忽略的文件夹名称</param>
        /// <param name="isReallyDelete">真删出，默认 False</param>
        /// 
        public static void EachSearchRemove(DirectoryInfo rootPath, List<string> listSearch, List<string> listIgnore, bool isReallyDelete = false)
        {
            if (rootPath.Exists)
            {
                var allDirs = rootPath.GetDirectories();

                var searchFiles = listSearch.Select(rootPath.GetFiles);
                var mergeFiles = new List<FileInfo>();
                foreach (var fileGroup in searchFiles)
                {
                    mergeFiles.AddRange(fileGroup);
                }
                mergeFiles = mergeFiles.Distinct().ToList();

                var searchDirs = listSearch.Select(rootPath.GetDirectories);
                var mergeDirs = new List<DirectoryInfo>();
                foreach (var dirGroup in searchDirs)
                {
                    mergeDirs.AddRange(dirGroup);
                }
                mergeDirs = mergeDirs.Distinct().ToList();

                var deleteFlag = isReallyDelete ? "" : "【假】";

                //删除文件
                foreach (var fileItem in mergeFiles)
                {
                    if (fileItem.Exists)
                    {
                        if (isReallyDelete)
                        {
                            fileItem.Delete();
                        }
                        Log($"{deleteFlag}删除文件: {fileItem.FullName}", isReallyDelete ? ConsoleColor.Red : null);
                    }
                }

                foreach (var subDir in allDirs)
                {
                    //忽略
                    if (listIgnore.Contains(subDir.Name))
                    {
                        continue;
                    }
                    else if (mergeDirs.Any(x => x.FullName == subDir.FullName))
                    {
                        //删除文件夹
                        if (subDir.Exists)
                        {
                            if (isReallyDelete)
                            {
                                subDir.Delete(true);
                            }
                            Log($"{deleteFlag}删除文件夹: {subDir.FullName}", isReallyDelete ? ConsoleColor.Red : null);
                        }
                    }
                    else
                    {
                        EachSearchRemove(subDir, listSearch, listIgnore, isReallyDelete);
                    }
                }
            }
        }

        /// <summary>
        /// 解析主机名、端口
        /// </summary>
        /// <param name="hostnameAndPorts"></param>
        /// <returns></returns>
        public static ValueTuple<string, HashSet<int>> ParseHostnameAndPorts(string hostnameAndPorts)
        {
            string hostname = hostnameAndPorts.Trim();
            var hsPort = new HashSet<int>();

            var hnp = hostname.Split(hostname.Contains(' ') ? ' ' : ':');
            hostname = hnp[0];
            if (hnp.Length > 1)
            {
                hsPort = RangeToList(hnp[1], 1, 65535);
            }

            return new ValueTuple<string, HashSet<int>>(hostname, hsPort);
        }

        /// <summary>
        /// 查询域名、IP对应的地址信息
        /// </summary>
        /// <param name="domainOrIP"></param>
        /// <returns></returns>
        public static async Task<Dictionary<string, string>> DomainOrIPInfo(string domainOrIP)
        {
            var result = new Dictionary<string, string>();

            try
            {
                var hc = new HttpClient();
                hc.DefaultRequestHeaders.UserAgent.TryParseAdd("Netnr");

                //空，获取本地公网出口IP
                if (string.IsNullOrWhiteSpace(domainOrIP))
                {
                    var res = await hc.GetStringAsync("https://api.bilibili.com/x/web-interface/zone");
                    var data = res.DeJson().GetProperty("data");
                    result.Add(data.GetValue("addr"), $"{data.GetValue("country")} {data.GetValue("province")} {data.GetValue("isp")}");
                }
                //域名
                else
                {
                    var listIp = new List<string>();
                    if (!IPAddress.TryParse(domainOrIP, out _))
                    {
                        var addresses = await Dns.GetHostAddressesAsync(domainOrIP);
                        foreach (var item in addresses)
                        {
                            listIp.Add($"{item}");
                        }
                    }
                    else
                    {
                        listIp.Add(domainOrIP);
                    }

                    foreach (var item in listIp)
                    {
                        if (IPAddress.TryParse(item, out var addr))
                        {
                            if (addr.AddressFamily == AddressFamily.InterNetworkV6)
                            {
                                var url = $"https://ip.useragentinfo.com/ipv6/{addr}";
                                var res = await hc.GetStringAsync(url);
                                var data = res.DeJson();
                                result.Add(data.GetValue("ipv6"), $"{data.GetValue("country")} {data.GetValue("region")} {data.GetValue("city")}");
                            }
                            else
                            {
                                var url = $"https://opendata.baidu.com/api.php?query={item}&resource_id=6006&oe=utf8";
                                var res = await hc.GetStringAsync(url);
                                var data = res.DeJson().GetProperty("data").EnumerateArray().First();
                                result.Add(data.GetValue("origipquery"), data.GetValue("location"));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// 检查 SSL
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="filePath"></param>
        public static void CheckSSL(Uri uri = null, string filePath = null)
        {
            List<MonitorTo.MonitorSSLModel> listSSLModel = null;

            //从文件分析
            if (!string.IsNullOrWhiteSpace(filePath))
            {
                var cert = new X509Certificate2(filePath);
                listSSLModel = MonitorTo.CertificateInformation(cert);
            }
            else
            {
                var result = MonitorTo.SSL(uri);
                result.Logs.ForEach(item =>
                {
                    Log($"{item}\r\n", ConsoleColor.Red);
                });
                listSSLModel = result.Data as List<MonitorTo.MonitorSSLModel>;
            }

            if (listSSLModel != null)
            {
                for (int i = 0; i < listSSLModel.Count; i++)
                {
                    var sslModel = listSSLModel[i];
                    if (i > 0)
                    {
                        Log("");
                    }

                    Log($"颁发给: {sslModel.Subject}");
                    Log($"颁发着: {sslModel.Issuer}");
                    Log($"有效期: {sslModel.NotBefore} 至 {sslModel.NotAfter} (剩余 {sslModel.AvailableDay} 天)");

                    var isRevoked = sslModel.ChainStatus.Any(x => x.Status.HasFlag(X509ChainStatusFlags.Revoked))
                        ? string.Join(", ", sslModel.ChainStatus.Select(x => x.Status)) : "正常";
                    Log($"吊销状态: {isRevoked}");
                    if (i < 1)
                    {
                        Log($"备用名称: {sslModel.AlternativeName}");
                    }

                    Log($"加密算法: {sslModel.AlgorithmType} {sslModel.AlgorithmSize} bits");
                    Log($"签名算法: {sslModel.SignatureAlgorithm}");
                    Log($"SHA1指纹: {sslModel.Thumbprint}");
                    Log($"SHA256指纹: {sslModel.Thumbprint256}");

                    Log($"序列号: {sslModel.SerialNumber}");
                    Log($"版本: {sslModel.Version}");
                }
            }
        }

        /// <summary>
        /// 查询 Whois
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static async Task<bool> QueryWhois(string domain)
        {
            try
            {
                var hc = new HttpClient();
                hc.DefaultRequestHeaders.UserAgent.TryParseAdd("Netnr");
                hc.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");

                var url = $"http://m.west.cn/web/whois/whoisinfo?domain={domain}&server=&refresh=0";
                var stream = await hc.GetStreamAsync(url);
                var html = await new StreamReader(stream, Encoding.GetEncoding("gbk")).ReadToEndAsync();
                var jsonData = html.DeJson();

                var keyMap = new Dictionary<string, string>
                {
                    {"registrer","注 册 商" },
                    {"regdate","注册时间" },
                    {"expdate","到期时间" },
                    {"status","域名状态" },
                    {"nameserver","Name Server" }
                };

                foreach (var key in keyMap.Keys)
                {
                    var val = jsonData.GetValue(key);

                    if (new string[] { "status", "nameserver" }.Contains(key))
                    {
                        val.Split(',').ToList().ForEach(vi =>
                        {
                            Log($"{keyMap[key]}: {vi}");
                        });
                    }
                    else
                    {
                        Log($"{keyMap[key]}: {val}");
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Log(ex);
            }

            return false;
        }

        /// <summary>
        /// 查询 ICP
        /// </summary>
        /// <param name="domain"></param>
        public static async Task<bool> QueryICP(string domain)
        {
            try
            {
                var url = $"https://micp.chinaz.com/Handle/AjaxHandler.ashx?action=GetPermit&callback=fn&query={domain.ToUrlEncode()}&type=host&_={DateTime.Now.ToTimestamp()}";

                var hc = new HttpClient();
                hc.DefaultRequestHeaders.UserAgent.TryParseAdd("Netnr");
                var html = await hc.GetStringAsync(url);
                if (html.Length > 10)
                {
                    html = html[4..^3];
                    var items = html.Split(',').ToList();
                    var keyMap = new Dictionary<string, string>
                    {
                        {"ComName","主办单位名称" },
                        {"Typ","主办单位性质" },
                        {"Permit","网站备案号" },
                        {"WebName","网站名称" },
                    };

                    foreach (var key in keyMap.Keys)
                    {
                        var val = items.FirstOrDefault(x => x.Contains(key)).Split(':').Last().Trim('"').Trim();
                        Log($"{keyMap[key]}: {val}");
                    }

                    return true;
                }
                else
                {
                    Log($"Not found {domain}", ConsoleColor.Red);
                }
            }
            catch (Exception ex)
            {
                Log(ex);
            }

            return false;
        }

        /// <summary>
        /// 随机范围
        /// </summary>
        /// <param name="value"></param>
        /// <param name="dvStart"></param>
        /// <param name="dvEnd"></param>
        /// <returns></returns>
        public static List<int> RandomRange(string value, int dvStart = 0, int dvEnd = 30)
        {
            if (value?.Contains('-') == true)
            {
                var range = value.Split('-');
                if (int.TryParse(range[0], out int getStart))
                {
                    dvStart = getStart;
                }

                if (int.TryParse(range[1], out int getEnd))
                {
                    dvEnd = getEnd;
                }
            }
            else if (int.TryParse(value, out int getVal))
            {
                dvStart = dvEnd = getVal;
            }

            return new List<int> { dvStart, dvEnd };
        }

        /// <summary>
        /// 范围转列表，如 20-80,443,3306,3389,8800-8999
        /// </summary>
        /// <param name="value"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static HashSet<int> RangeToList(string value, int? minValue = null, int? maxValue = null)
        {
            var result = new HashSet<int>();

            if (!string.IsNullOrWhiteSpace(value))
            {
                var items = value.Split(',');
                foreach (var item in items)
                {
                    if (item.Contains('-'))
                    {
                        var itemRange = item.Split('-');
                        var r1 = Convert.ToInt32(itemRange.First());
                        if (minValue.HasValue)
                        {
                            r1 = Math.Max(r1, minValue.Value);
                        }

                        var r2 = Convert.ToInt32(itemRange.Last());
                        if (maxValue.HasValue)
                        {
                            r2 = Math.Min(r2, maxValue.Value);
                        }

                        for (int i = r1; i <= r2; i++)
                        {
                            result.Add(i);
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(item))
                    {
                        var r = Convert.ToInt32(item);
                        if ((minValue == null && maxValue == null) || (minValue.HasValue && r >= minValue) || (maxValue.HasValue && r <= maxValue))
                        {
                            result.Add(r);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 消耗 CPU（单核）
        /// https://stackoverflow.com/questions/2514544
        /// </summary>
        /// <param name="percentage">百分比 1-100 </param>
        /// <param name="isRuning">初始设置 True，设置 False 停止</param>
        public static void ConsumeCPU(int percentage, ref bool isRuning)
        {
            var watch = Stopwatch.StartNew();
            while (isRuning)
            {
                // Make the loop go on for "percentage" milliseconds then sleep the 
                // remaining percentage milliseconds. So 40% utilization means work 40ms and sleep 60ms
                if (watch.ElapsedMilliseconds > percentage)
                {
                    Thread.Sleep(100 - percentage);
                    watch.Restart();
                }
            }
        }

        /// <summary>
        /// 消耗 RAM
        /// </summary>
        /// <param name="percentage"></param>
        /// <param name="fillResult"></param>
        public static void ConsumeRAM(int percentage, ref IntPtr[] fillResult)
        {
            var ss = new SystemStatusTo();
            ss.RefreshAll(true).GetAwaiter().GetResult();

            //需要追加的内存
            var totalSize = Convert.ToInt64(percentage / 100.0 * ss.TotalPhysicalMemory - ss.UsedPhysicalMemory);
            if (totalSize < 10240)
            {
                totalSize = 10240;
            }

            var blockSize = int.MaxValue / 2;
            if (blockSize > totalSize)
            {
                blockSize = (int)totalSize;
            }

            var blockCount = (int)(totalSize / blockSize);
            var lastBlockSize = (int)(totalSize - blockSize * blockCount);

            //组对应大小
            var groupMapSize = new Dictionary<int, int>();
            for (int i = 0; i < blockCount; i++)
            {
                groupMapSize.Add(i, blockSize);
            }
            if (lastBlockSize > 0)
            {
                groupMapSize.Add(blockCount, lastBlockSize);
            }

            //填充
            fillResult = new IntPtr[blockCount + 1];
            int[] fillArray = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };
            //10组重复再转为数组
            fillArray = Enumerable.Repeat(fillArray, 10).SelectMany(x => x).ToArray();

            foreach (var index in groupMapSize.Keys)
            {
                var size = groupMapSize[index];
                var pnt = Marshal.AllocHGlobal(size);

                // 复制 fillArray 到 pnt
                int offset = 0;
                while (offset < size && offset + fillArray.Length * sizeof(int) <= size)
                {
                    Marshal.Copy(fillArray, 0, pnt + offset, fillArray.Length);
                    offset += fillArray.Length * sizeof(int);
                }

                fillResult[index] = pnt;
            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="conn"></param>
        /// <param name="parserUA"></param>
        /// <returns></returns>
        public static async Task<int> NginxLogWriteTable(DataTable dt, string conn, bool parserUA = false)
        {
            var st = Stopwatch.StartNew();

            if (parserUA && dt.Columns.Contains("http_user_agent"))
            {
                Log($"解析 User-Agent");

                var lockObject = new object();

                var dte = dt.AsEnumerable();
                var po = new ParallelOptions
                {
                    MaxDegreeOfParallelism = Math.Max(2, Environment.ProcessorCount / 2)
                };
                Parallel.ForEach(dte, po, dr =>
                {
                    var http_user_agent = dr["http_user_agent"].ToString();
                    if (http_user_agent.Length > 1)
                    {
                        var uap = new UAParsers(http_user_agent);
                        var botModel = uap.GetBot();
                        if (botModel != null)
                        {
                            lock (lockObject)
                            {
                                dr["ua_bot"] = botModel.Name;
                            }
                        }
                        else
                        {
                            lock (lockObject)
                            {
                                dr["ua_bot"] = "user";
                            }

                            var clientModel = uap.GetClient();
                            if (clientModel != null)
                            {
                                lock (lockObject)
                                {
                                    dr["ua_browser_name"] = clientModel.Name;
                                    dr["ua_browser_version"] = clientModel.Version;
                                }
                            }

                            var osModel = uap.GetOS();
                            if (osModel != null)
                            {
                                lock (lockObject)
                                {
                                    dr["ua_system_name"] = osModel.Name;
                                    dr["ua_system_version"] = osModel.Version;
                                }
                            }
                        }
                    }
                    else
                    {
                        lock (lockObject)
                        {
                            dr["ua_bot"] = "bot";
                        }
                    }
                });
                Log($"解析完成，解析耗时 {st.Elapsed}");
            }

            st.Restart();
            Log($"分批写 {dt.Rows.Count} 条");

            var dbConn = new ClickHouseConnection(conn);

            using var bulk = new ClickHouseBulkCopy(dbConn)
            {
                DestinationTableName = dt.TableName,
                BatchSize = dt.Rows.Count
            };

            var cts = new CancellationTokenSource();
            await bulk.WriteToServerAsync(dt, cts.Token);

            Log($"已写入 {bulk.RowsWritten} 条，当前写入耗时 {st.Elapsed}");

            return (int)bulk.RowsWritten;
        }

        /// <summary>
        /// 构建空表
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="isMore"></param>
        /// <returns></returns>
        public static DataTable NginxLogEmptyTable(string tableName = "access_more", bool isMore = false)
        {
            var dict = new Dictionary<string, Type>
            {
                { "time_local", typeof(DateTime) },
                { "remote_addr", typeof(string) },
                { "host", typeof(string) },
                { "path_name", typeof(string) },
                { "url", typeof(string) },
                { "http_method", typeof(string) },
                { "http_protocol", typeof(string) },
                { "status", typeof(int) },
                { "body_bytes_sent", typeof(long) },
                { "request_body", typeof(string) },
                { "http_referer", typeof(string) },
                { "http_user_agent", typeof(string) }
            };
            if (isMore)
            {
                dict.Add("ip_country", typeof(string));
                dict.Add("ip_city1", typeof(string));
                dict.Add("ip_city2", typeof(string));
                dict.Add("ip_isp", typeof(string));
                dict.Add("ua_bot", typeof(string));
                dict.Add("ua_browser_name", typeof(string));
                dict.Add("ua_browser_version", typeof(string));
                dict.Add("ua_system_name", typeof(string));
                dict.Add("ua_system_version", typeof(string));
            }

            var dt = new DataTable();
            foreach (var key in dict.Keys)
            {
                dt.Columns.Add(key, dict[key]);
            }
            dt.TableName = tableName;

            return dt;
        }
    }
}
