#if Full || ConsoleX

using System.Text.RegularExpressions;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.InteropServices;
using System.Reflection;

namespace Netnr;

/// <summary>
/// 辅助类
/// </summary>
public partial class ConsoleXTo
{
    /// <summary>
    /// 启动
    /// </summary>
    /// <param name="programType"></param>
    /// <returns></returns>
    public static async Task ProgramMain(Type programType)
    {
        BaseTo.ReadyEncoding();
        BaseTo.ReadyLegacyTimestamp();

        //参数模式（静默）
        if (BaseTo.IsCmdArgs)
        {
            await RunOfSilence();
        }
        else
        {
            Console.Title = $"{programType.Namespace} v{BaseTo.Version}";
            await RunOfConsole();
        }
    }

    /// <summary>
    /// 菜单项类型
    /// </summary>
    public static Type MethodType { get; set; } = typeof(MenuItemService);

    /// <summary>
    /// 方法对象
    /// </summary>
    public static List<ConsoleXMethodModel> AllMethod { get; set; } = GetAllMethods();

    private static List<ConsoleXMethodModel> GetAllMethods()
    {
        var result = new List<ConsoleXMethodModel>();

        var cms = MethodType.GetMethods().ToList();
        var mm = cms.First().Module;
        cms = cms.Where(x => x.Module == mm).ToList();

        for (int i = 0; i < cms.Count; i++)
        {
            var mi = cms[i];
            var dispAttr = mi.GetCustomAttribute<DisplayAttribute>();
            if (dispAttr != null)
            {
                var model = new ConsoleXMethodModel
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

                result.Add(model);
            }
        }

        //每组末尾换行
        result.GroupBy(x => x.GroupName).ForEach(ig =>
        {
            ig.Last().NewLine = "\r\n";
        });

        return result;
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
            if (key.StartsWith('-'))
            {
                var val = i + 1 < args.Count ? args[i + 1] : "";
                list.Add(new KeyValuePair<string, string>(key, val.StartsWith('-') ? "" : val));
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

            var groupKeys = AllMethod.Select(x => x.GroupName).Distinct().OrderBy(p => p).ToList();
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
        else if (groupName.StartsWith('/'))
        {
            //一组

            groupName = groupName.TrimStart('/');
            var groupMethod = AllMethod.Where(x => x.GroupName == groupName).ToList();
            groupMethod.Insert(0, new ConsoleXMethodModel
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
                var method = AllMethod[itemIndex - 2].Method;
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

        if (action.StartsWith('/') && CmdArgs.Count == 0)
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
                var help = new string[] { "?", "/?", "-?", "-h", "/help", "-help", "--help" };
                if (CmdArgs.Count == 1 && help.Contains(CmdArgs[0]))
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
            var psFile = Path.Combine(Path.GetTempPath(), $"netnr_{BaseTo.StartTime.ToTimestamp()}.ps1");
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
    /// 输入选择项，从 1 开始，返回也是
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
        var rb = Console.ReadLine().ToLower().Trim();
        return rb == "y" || rb == "1";
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
    /// 输入字符串
    /// </summary>
    /// <param name="tip">提示文字</param>
    public static string ConsoleReadString(string tip)
    {
        Console.Write($"{tip}: ");
        var result = Console.ReadLine();
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
    /// 获取路径
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string ConsoleAsPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return Environment.CurrentDirectory;
        }
        else if (path.StartsWith('.'))
        {
            Path.Combine(Environment.CurrentDirectory, path);
        }
        return path;
    }

    /// <summary>
    /// 生成工作日随机时间（周一到周五，8-18点）
    /// </summary>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <returns></returns>
    public static DateTime? GenerateWorkingTime(DateTime startDate, DateTime endDate)
    {
        int range = (endDate - startDate).Days;
        DateTime? workTime = null;
        for (int i = 0; i < short.MaxValue; i++)
        {
            var randomDate = startDate.AddDays(RandomTo.Instance.Next(range));
            if (randomDate.DayOfWeek == DayOfWeek.Sunday || randomDate.DayOfWeek == DayOfWeek.Saturday)
            {
                continue;
            }
            else
            {
                workTime = randomDate.Date.AddSeconds(RandomTo.Instance.Next(3600 * 8, 3600 * 18));
                break;
            }
        }

        return workTime;
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

                if (key == "status" || key == "nameserver")
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

                var headers = "主办单位,单位性质,备案号,网站名称,网站首页,审核时间".Split(',');

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

        return [dvStart, dvEnd];
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

    public static class ConsumeRAMTask
    {
        private class MemoryBlock
        {
            public int Size { get; set; }
            public IntPtr Pointer { get; set; }

            /// <summary>
            /// 构造 分配指定大小的内存块
            /// </summary>
            /// <param name="size"></param>
            public MemoryBlock(int size)
            {
                Size = size;
                Pointer = Marshal.AllocHGlobal(size);

                int[] fillArray = [1, 2, 3, 4, 5, 6, 7, 8, 9, 0];
                fillArray = Enumerable.Repeat(fillArray, 10).SelectMany(x => x).ToArray();

                // Copy fillArray to Pointer
                int offset = 0;
                while (offset < size && offset + fillArray.Length * sizeof(int) <= size)
                {
                    Marshal.Copy(fillArray, 0, Pointer + offset, fillArray.Length);
                    offset += fillArray.Length * sizeof(int);
                }
            }
        }

        private static readonly Dictionary<long, MemoryBlock> allocatedMemory = [];
        private static long totalAllocatedSize = 0;
        private static long idCounter = 0;

        public static void ConsumeRAM(int percentage)
        {
            var ss = new SystemStatusTo();
            ss.RefreshAll(true).GetAwaiter().GetResult();

            long targetSize = Convert.ToInt64(percentage / 100.0 * ss.TotalPhysicalMemory - (ss.UsedPhysicalMemory - totalAllocatedSize));
            if (targetSize < 10240)
            {
                targetSize = 10240;
            }

            if (targetSize < totalAllocatedSize)
            {
                // Release some memory
                ReleaseMemory(targetSize);
            }
            else if (targetSize > totalAllocatedSize)
            {
                // Allocate more memory
                AllocateMemory(targetSize);
            }
        }

        private static void ReleaseMemory(long targetSize)
        {
            foreach (var key in allocatedMemory.Keys)
            {
                var block = allocatedMemory[key];

                Marshal.FreeHGlobal(block.Pointer);
                allocatedMemory.Remove(key);
                totalAllocatedSize -= block.Size;

                // 先释放一个块后再判断
                if (totalAllocatedSize <= targetSize)
                {
                    break;
                }
            }
        }

        private static void AllocateMemory(long targetSize)
        {
            //大块分配
            var blockSize = 1024 * 1024 * 50; //50MiB
            while (totalAllocatedSize + blockSize <= targetSize)
            {
                var block = new MemoryBlock(blockSize);
                allocatedMemory.Add(idCounter++, block);
                totalAllocatedSize += blockSize;
            }

            //剩余分配
            int lastBlockSize = (int)(targetSize - totalAllocatedSize);
            if (lastBlockSize > 0)
            {
                var block = new MemoryBlock(lastBlockSize);
                allocatedMemory.Add(idCounter++, block);
                totalAllocatedSize += lastBlockSize;
            }
        }

        public static void ReleaseAll()
        {
            foreach (var key in allocatedMemory.Keys)
            {
                Marshal.FreeHGlobal(allocatedMemory[key].Pointer);
                allocatedMemory.Remove(key);
            }

            totalAllocatedSize = 0;
        }
    }

    public class ConsumeNetworkTask(string url, long byteSpeed)
    {
        private readonly string _url = url;
        private long _byteSpeed = byteSpeed;
        private CancellationTokenSource _cancellationTokenSource;
        private Task _currentTask;

        public long ByteSpeed
        {
            get => _byteSpeed;
            set
            {
                _byteSpeed = Math.Max(1, value);
            }
        }

        public async Task StartAsync()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _currentTask = DownloadAsync();
            await _currentTask;
        }

        private async Task DownloadAsync()
        {
            while (true)
            {
                if (_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    break;
                }

                try
                {
                    using var client = HttpTo.BuildClient();
                    using var response = await client.GetAsync(_url, HttpCompletionOption.ResponseHeadersRead);
                    if (response.IsSuccessStatusCode)
                    {
                        using var stream = await response.Content.ReadAsStreamAsync();

                        var buffer = new byte[4096];
                        var stopwatch = new Stopwatch();

                        int bytesRead;
                        long totalRead = 0;

                        while ((bytesRead = await stream.ReadAsync(buffer, _cancellationTokenSource.Token)) > 0)
                        {
                            totalRead += bytesRead;
                            if (totalRead > _byteSpeed * 0.1)
                            {
                                var delay = 100 - stopwatch.ElapsedMilliseconds;
                                if (delay > 0)
                                {
                                    await Task.Delay((int)delay, _cancellationTokenSource.Token);
                                }

                                stopwatch.Restart();
                                totalRead = 0;
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    ConsoleTo.LogError(ex, nameof(ConsumeNetworkTask));
                    break;
                }
            }
        }

        public void Stop()
        {
            _cancellationTokenSource?.Cancel();
        }
    }
}

#endif