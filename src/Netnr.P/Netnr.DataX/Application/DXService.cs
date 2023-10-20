using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.InteropServices;
using System.Reflection;

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
                        var dispAttr = mi.GetCustomAttribute<DisplayAttribute>();
                        if (dispAttr != null)
                        {
                            var model = new MethodModel
                            {
                                GroupName = dispAttr.GroupName,
                                Name = dispAttr.Name,
                                ShortName = dispAttr.ShortName,
                                Description = dispAttr.Description,
                                Prompt = dispAttr.Prompt,
                                Method = mi
                            };

                            if (dispAttr.GetAutoGenerateFilter() == true)
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
                    allMethod.GroupBy(x => x.GroupName).ForEach(ig =>
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
        public static List<string> CmdArgs { get; set; } = BaseTo.CommandLineArgs;

        /// <summary>
        /// 参数转键值对数组
        /// </summary>
        /// <param name="args">可指定，默认自动获取</param>
        /// <returns></returns>
        public static List<KeyValuePair<string, string>> GetArgsKeyValue(List<string> args = null)
        {
            var list = new List<KeyValuePair<string, string>>();

            args ??= CmdArgs;
            for (int i = 0; i < args.Count; i++)
            {
                var key = args[i];
                if (key.StartsWith("-"))
                {
                    var val = i + 1 < args.Count ? args[i + 1] : "";
                    list.Add(new KeyValuePair<string, string>(key, val.StartsWith("-") ? "" : val));
                }
            }

            return list;
        }

        /// <summary>
        /// 获取变量名
        /// </summary>
        /// <param name="name">名称（带横线），支持逗号分割多个</param>
        /// <param name="tip">输入提示</param>
        /// <returns></returns>
        public static string VarName(string name, string tip, IList<string> items = null, int? dv = 1)
        {
            var result = "";
            if (BaseTo.IsCmdArgs)
            {
                var nameArray = name.Split(',');
                var filter = GetArgsKeyValue().Where(x => nameArray.Contains(x.Key)).ToList();
                if (filter.Count > 0)
                {
                    result = filter.First().Value;
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

            return result.Trim();
        }

        /// <summary>
        /// 获取变量
        /// </summary>
        /// <param name="index">静默带参索引，从 0 开始</param>
        /// <param name="tip">输入提示</param>
        /// <param name="items">选项</param>
        /// <param name="dv"></param>
        /// <returns></returns>
        public static string VarIndex(int index, string tip, IList<string> items = null, int? dv = 1)
        {
            string result;
            if (BaseTo.IsCmdArgs)
            {
                result = index < CmdArgs.Count ? CmdArgs[index] : "";
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
            return result.Trim();
        }

        /// <summary>
        /// 获取变量
        /// </summary>
        /// <param name="tip"></param>
        /// <param name="has">多个支持逗号分割</param>
        /// <returns></returns>
        public static bool VarBool(string tip, string has = "-y")
        {
            bool result;
            if (BaseTo.IsCmdArgs)
            {
                result = has.Split(',').Any(x => !string.IsNullOrWhiteSpace(x) && CmdArgs.Contains(x));
            }
            else
            {
                result = ConsoleReadBool(tip);
            }
            return result;
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
                await Task.Delay(1500);
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
            CmdArgs.RemoveAt(0);
            var action = CmdArgs[0];
            CmdArgs.RemoveAt(0);

            if (action.StartsWith("/") && CmdArgs.Count == 0)
            {
                //help group
                var groupMethod = AllMethod.Where(x => x.GroupName == action.TrimStart('/')).ToList();
                if (groupMethod.Count == 0)
                {
                    ConsoleTo.LogColor($"{action} 分组无效");
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
                    if (CmdArgs.Count == 1 && new string[] { "?", "/?", "-?", "-h", "/help", "-help", "--help" }.Contains(CmdArgs[0]))
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
        /// 提示符号
        /// </summary>
        /// <param name="tip"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static string TipSymbol(string tip, string symbol = ": ")
        {
            return $"\r\n{tip.TrimEnd('：').TrimEnd(':').TrimEnd('?').TrimEnd('？')}{symbol}";
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
            var ci = new ConfigInit();
            var now = DateTime.Now;

            var pattern = @"({\w+})";
            var path = new Regex(pattern).Replace(str, o =>
            {
                var format = o.Groups[1].Value[1..^1];
                return now.ToString(format);
            }).Replace("~", ci.DXHub);

            return path;
        }

        /// <summary>
        /// 弹窗通知
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content"></param>
        public static void Notify(string title, string content)
        {
            if (CmdTo.IsWindows)
            {
                var cmd = $@"
Add-Type -AssemblyName System.Drawing; # get icon
Add-Type -AssemblyName System.Windows.Forms;

$ni = New-Object System.Windows.Forms.NotifyIcon;
$ni.Icon = [System.Drawing.Icon]::ExtractAssociatedIcon(""{Environment.ProcessPath}""); # get icon
$ni.BalloonTipIcon = ""Info"";
$ni.BalloonTipTitle = ""{title.Replace("\"", "\"\"").Replace("\r\n", "`n")}"";
$ni.BalloonTipText = ""{content.Replace("\"", "\"\"").Replace("\r\n", "`n")}"";
$ni.Visible = $true;
$ni.ShowBalloonTip(1000);
";
                var psFile = Path.Combine(Path.GetTempPath(), $"{ConfigInit.ShortName}_{BaseTo.StartTime.ToTimestamp()}.ps1");
                File.WriteAllText(psFile, cmd);
                CmdTo.Execute($"-ExecutionPolicy unrestricted -File \"{psFile}\"", "powershell");
            }
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
                ConsoleTo.LogColor(ex.Message);

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

                Console.WriteLine($"\r\n{0,5}. 输入数据库连接信息");
                for (int i = 0; i < allDbConns.Count; i++)
                {
                    var obj = allDbConns[i];
                    Console.WriteLine($"{i + 1,5}. [{obj.ConnectionRemark}]({obj.ConnectionType}://{obj.GetSafeConnectionString()})");
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
                                connOption.ConnectionType = mr.Groups[1].ToString().DeEnum<DBTypes>();
                                connOption.ConnectionString = mr.Groups[2].ToString();
                            }
                            else
                            {
                                connOption.ConnectionRemark = mr.Groups[1].ToString();
                                connOption.ConnectionType = mr.Groups[2].ToString().DeEnum<DBTypes>();
                                connOption.ConnectionString = mr.Groups[3].ToString();
                            }
                            //解密
                            connOption.ConnectionString = DbKitExtensions.SqlConnEncryptOrDecrypt(connOption.ConnectionString, "");

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

                //深拷贝构建新实例
                connOption.DeepCopyNewInstance = true;

                //选择数据库名
                switch (connOption.ConnectionType)
                {
                    case DBTypes.MySQL:
                    case DBTypes.MariaDB:
                    case DBTypes.SQLServer:
                    case DBTypes.PostgreSQL:
                        {
                            var dataKit = DataKitTo.CreateDataKitInstance(connOption);
                            var listDatabaseName = await dataKit.GetDatabaseName();

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

                ConsoleTo.LogColor($"\r\n[{connOption.ConnectionRemark}]({connOption.ConnectionType}://{connOption.GetSafeConnectionString()})\r\n", ConsoleColor.Cyan);
                return true;
            });

            //复制一个新对象返回
            var newOption = new DbKitConnectionOption();
            newOption.ToDeepCopy(connOption);

            return newOption;
        }

        /// <summary>
        /// 显示读写数据库配置
        /// </summary>
        /// <param name="connOptionRead">读取源数据库</param>
        /// <param name="connOptionWrite">写入目标数据库</param>
        public static void ViewConnectionOption(DbKitConnectionOption connOptionRead, DbKitConnectionOption connOptionWrite)
        {
            ConsoleTo.LogColor($"\r\n[读取]: {connOptionRead.ConnectionType}://{connOptionRead.GetSafeConnectionString()}", ConsoleColor.Green);
            ConsoleTo.LogColor($"[写入]: {connOptionWrite.ConnectionType}://{connOptionWrite.GetSafeConnectionString()}\r\n", ConsoleColor.Yellow);
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
                    ConsoleTo.LogColor($"{path} 无效文件（夹）");
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
                    ConsoleTo.LogColor($"\r\nChosen {dv}. {items[dv.Value - 1].Trim()}", ConsoleColor.Cyan);
                    itemIndex = dv.Value;
                }
                else
                {
                    var si = Convert.ToInt32(ii);
                    if (si > 0 && si <= items.Count)
                    {
                        ConsoleTo.LogColor($"\r\nChosen {si}. {items[si - 1].Trim()}", ConsoleColor.Cyan);
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
            Console.Write($"{TipSymbol(tip, "?")} [y(1)/N(default)]: ");
            return new[] { "y", "1" }.Contains(Console.ReadLine().ToLower().Trim());
        }

        /// <summary>
        /// 输入多个 按指定分隔
        /// </summary>
        /// <param name="tip">提示文字</param>
        /// <param name="delimiter">分隔符，默认逗号</param>
        public static List<T> ConsoleReadJoin<T>(string tip, string delimiter = ",")
        {
            Console.Write($"{tip}: ");
            var values = Console.ReadLine().Split(delimiter);

            var result = values.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.ToConvert<T>()).ToList();
            ConsoleTo.LogColor($"\r\n已输入 {result.Count} 个对象\r\n", ConsoleColor.Cyan);
            return result;
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
                var allDirs = rootPath.EnumerateDirectories();

                var searchFiles = listSearch.Select(rootPath.GetFiles);
                var mergeFiles = new List<FileInfo>();
                foreach (var fileGroup in searchFiles)
                {
                    mergeFiles.AddRange(fileGroup);
                }
                mergeFiles = mergeFiles.Distinct().ToList();

                var searchDirs = listSearch.Select(rootPath.EnumerateDirectories);
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
                        ConsoleTo.LogColor($"{deleteFlag}删除文件: {fileItem.FullName}", isReallyDelete ? ConsoleColor.Red : null);
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
                            ConsoleTo.LogColor($"{deleteFlag}删除文件夹: {subDir.FullName}", isReallyDelete ? ConsoleColor.Red : null);
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
                            var url = $"https://ip.zxinc.org/api.php?type=json&ip={addr}";
                            var res = await hc.GetStringAsync(url);
                            var data = res.DeJson();

                            result.Add($"{addr}", data.GetProperty("data").GetValue("location").Replace("\t", " ").Replace("CZ88.NET", "").Trim());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ConsoleTo.LogColor(ex.Message);
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
                    ConsoleTo.LogColor($"{item}\r\n", ConsoleColor.Red);
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
                        ConsoleTo.LogColor("");
                    }

                    ConsoleTo.LogColor($"颁发给: {sslModel.Subject}");
                    ConsoleTo.LogColor($"颁发着: {sslModel.Issuer}");
                    ConsoleTo.LogColor($"有效期: {sslModel.NotBefore} 至 {sslModel.NotAfter} (剩余 {sslModel.AvailableDay} 天)");

                    var isRevoked = sslModel.ChainStatus.Any(x => x.Status.HasFlag(X509ChainStatusFlags.Revoked))
                        ? string.Join(", ", sslModel.ChainStatus.Select(x => x.Status)) : "正常";
                    ConsoleTo.LogColor($"吊销状态: {isRevoked}");
                    if (i < 1 && !string.IsNullOrWhiteSpace(sslModel.AlternativeName))
                    {
                        ConsoleTo.LogColor($"备用名称: {sslModel.AlternativeName}");
                    }

                    // 是根证书
                    if (sslModel.Subject == sslModel.Issuer)
                    {
                        ConsoleTo.LogColor($"根证书: 是");
                    }

                    ConsoleTo.LogColor($"加密算法: {sslModel.AlgorithmType} {sslModel.AlgorithmSize} bits");
                    ConsoleTo.LogColor($"签名算法: {sslModel.SignatureAlgorithm}");
                    ConsoleTo.LogColor($"SHA1指纹: {sslModel.Thumbprint}");
                    ConsoleTo.LogColor($"SHA256指纹: {sslModel.Thumbprint256}");

                    ConsoleTo.LogColor($"序列号: {sslModel.SerialNumber}");
                    ConsoleTo.LogColor($"版本: {sslModel.Version}");
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
                        val.Split(',').ForEach(vi =>
                        {
                            ConsoleTo.LogColor($"{keyMap[key]}: {vi}");
                        });
                    }
                    else
                    {
                        ConsoleTo.LogColor($"{keyMap[key]}: {val}");
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ConsoleTo.LogError(ex);
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
                var url = $"https://micp.chinaz.com/{domain.ToUrlEncode()}";

                var hc = new HttpClient();
                hc.DefaultRequestHeaders.UserAgent.TryParseAdd("Netnr");
                var result = await hc.GetStringAsync(url);

                if (result.Contains("主办单位"))
                {
                    var s1 = result.IndexOf("<table");
                    var s2 = result.IndexOf("</table>");
                    result = result.Substring(s1, s2 - s1 + 8);

                    var headers = "主办单位,单位性质,备案号,网站名称,审核时间".Split(',');

                    var lines = result.Split('\n');
                    var pattern = @">(.*)<\/td";
                    for (int i = 0; i < lines.Length; i++)
                    {
                        var line = lines[i];
                        var header = headers.FirstOrDefault(x => line.Contains($"{x}："));
                        if (header != null)
                        {
                            var val = lines[i + 1].Trim();
                            // 使用正则表达式提取内容
                            var mr = Regex.Match(val, pattern);
                            if (mr.Success)
                            {
                                val = mr.Groups[1].Value;
                            }
                            ConsoleTo.LogColor($"{header}: {val}");
                        }
                    }

                    return true;
                }
                else
                {
                    ConsoleTo.LogColor($"Not found {domain}", ConsoleColor.Red);
                }
            }
            catch (Exception ex)
            {
                ConsoleTo.LogError(ex);
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
        /// 消耗 Network
        /// </summary>
        /// <param name="url">下载资源链接</param>
        /// <param name="working">结束、暂停</param>
        /// <param name="speed">网速</param>
        public static async Task ConsumeNetwork(string url, Func<ValueTuple<bool, bool>> working, int speed = 1024 * 1024 * 2)
        {
            var handler = new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };
            var httpClient = new HttpClient(handler);

            while (working.Invoke().Item1)
            {
                try
                {
                    //暂停
                    if (working.Invoke().Item2 == false)
                    {
                        await Task.Delay(1000);
                    }
                    else
                    {
                        using var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                        if (response.IsSuccessStatusCode)
                        {
                            using var download = await response.Content.ReadAsStreamAsync();

                            int readLength;
                            long receiveLength = 0;
                            var buffer = new byte[1024];

                            var sw = Stopwatch.StartNew();
                            while (working.Invoke().Item1 && (readLength = await download.ReadAsync(buffer)) != 0)
                            {
                                //暂停
                                if (working.Invoke().Item2 == false)
                                {
                                    break;
                                }

                                receiveLength += readLength;
                                if (receiveLength >= speed * 0.1)
                                {
                                    receiveLength = 0;

                                    var wait = 100 - (int)sw.ElapsedMilliseconds;
                                    if (wait > 0)
                                    {
                                        await Task.Delay(wait);
                                    }
                                    sw.Restart();
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    await Task.Delay(1000);
                }
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
                var lockObject = new object();

                var dte = dt.AsEnumerable();
                var po = new ParallelOptions
                {
                    MaxDegreeOfParallelism = Math.Max(1, Environment.ProcessorCount / 2)
                };
                if (dt.Rows.Count > 100)
                {
                    ConsoleTo.LogColor($"正在并行（{po.MaxDegreeOfParallelism}）解析 User-Agent，共 {dt.Rows.Count} 条");
                }
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

                if (dt.Rows.Count > 100)
                {
                    ConsoleTo.LogColor($"解析完成，解析耗时 {st.Elapsed}");
                }
            }

            st.Restart();

            if (dt.Rows.Count > 100)
            {
                ConsoleTo.LogColor($"开始写入 {dt.Rows.Count} 条");
            }

            var connOption = new DbKitConnectionOption
            {
                ConnectionType = DBTypes.ClickHouse,
                ConnectionString = conn
            };
            var dbKit = connOption.CreateDbInstance();
            var num = await dbKit.BulkCopy(dt);

            ConsoleTo.LogColor($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] 已写入 {num} 条，当前写入耗时 {st.Elapsed}");

            return num;
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
