using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Net.Security;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Collections.Concurrent;
using System.Security.Cryptography.X509Certificates;

namespace Netnr.DataX.Menu
{
    /// <summary>
    /// 静默
    /// </summary>
    public class MenuSilenceService
    {
        /// <summary>
        /// 启动参数
        /// </summary>
        static List<string> Args { get; set; } = GlobalTo.StartArgs;

        [Display(Name = "Back to menu", Description = "返回菜单", GroupName = "\r\n")]
        public static void BackToMenu() => DXService.InvokeMenu(typeof(MenuMainService));

        /// <summary>
        /// 静默执行
        /// </summary>
        public static void Run()
        {
            Args.RemoveAt(0);
            var action = Args[0].ToLower();
            Args.RemoveAt(0);

            var ctype = typeof(MenuSilenceService);
            var cms = ctype.GetMethods().ToList();
            var mm = cms.First().Module;
            cms = cms.Where(x => x.Module == mm).ToList();

            var dicSilent = new Dictionary<string, MethodInfo>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < cms.Count; i++)
            {
                var mi = cms[i];
                var attrs = mi.CustomAttributes.LastOrDefault()?.NamedArguments;
                if (attrs?.Count > 0)
                {
                    var attrShortName = attrs.FirstOrDefault(x => x.MemberName == "ShortName").TypedValue.Value?.ToString();
                    if (!string.IsNullOrWhiteSpace(attrShortName))
                    {
                        dicSilent.Add(attrShortName.Split(' ')[0], mi);
                    }
                }
            }

            if (dicSilent.TryGetValue(action, out MethodInfo value))
            {
                var method = value;
                Console.WriteLine("");
                method.Invoke(ctype, null);
            }
            else
            {
                //help
                Console.WriteLine($"\r\nndx(v{ConfigInit.Version})\r\n");

                for (int i = 0; i < cms.Count; i++)
                {
                    var mi = cms[i];
                    var attrs = mi.CustomAttributes.LastOrDefault()?.NamedArguments;
                    if (attrs?.Count > 0)
                    {
                        var attrShortName = attrs.FirstOrDefault(x => x.MemberName == "ShortName").TypedValue.Value?.ToString();
                        var attrDescription = attrs.FirstOrDefault(x => x.MemberName == "Description").TypedValue.Value?.ToString();
                        var attrPrompt = attrs.FirstOrDefault(x => x.MemberName == "Prompt").TypedValue.Value?.ToString();
                        if (!string.IsNullOrWhiteSpace(attrShortName))
                        {
                            DXService.Log($"ndx {attrShortName} {(attrDescription.Length > 50 ? "\r\n" : "")}## {attrDescription}\r\ndemo: {string.Join("\r\ndemo: ", attrPrompt.Split(Environment.NewLine))}\r\n");
                        }
                    }
                }
            }
        }

        [Display(Name = "Work", Description = "作业, 以 Work_ 开头",
            ShortName = "work [Work_Name] [Work_2]", Prompt = "ndx task Task_Demo", GroupName = "\r\n")]
        public static void Work()
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
            else if (GlobalTo.IsStartWithArgs)
            {
                working = Args;
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
                                        var mo = parameters.ToJson().DeJson<DataKitTransferVM.ExportDatabase>();
                                        mo.PackagePath = DXService.ParsePathVar(mo.PackagePath);

                                        //引用连接
                                        if (mo.ReadConnectionInfo == null && !string.IsNullOrWhiteSpace(mo.RefReadConnectionInfo))
                                        {
                                            mo.ReadConnectionInfo = cc.ListConnectionInfo.FirstOrDefault(x => x.ConnectionRemark == mo.RefReadConnectionInfo);
                                            if (!string.IsNullOrWhiteSpace(mo.ReadDatabaseName))
                                            {
                                                mo.ReadConnectionInfo.DatabaseName = mo.ReadDatabaseName;
                                            }
                                        }

                                        DataKitTo.ExportDatabase(mo, le => DXService.Log(le.NewItems[0].ToString()));
                                    }
                                    break;
                                case "ExportDataTable":
                                    {
                                        var mo = parameters.ToJson().DeJson<DataKitTransferVM.ExportDataTable>();
                                        mo.PackagePath = DXService.ParsePathVar(mo.PackagePath);

                                        //引用连接
                                        if (mo.ReadConnectionInfo == null && !string.IsNullOrWhiteSpace(mo.RefReadConnectionInfo))
                                        {
                                            mo.ReadConnectionInfo = cc.ListConnectionInfo.FirstOrDefault(x => x.ConnectionRemark == mo.RefReadConnectionInfo);
                                            if (!string.IsNullOrWhiteSpace(mo.ReadDatabaseName))
                                            {
                                                mo.ReadConnectionInfo.DatabaseName = mo.ReadDatabaseName;
                                            }
                                        }

                                        DataKitTo.ExportDataTable(mo, le => DXService.Log(le.NewItems[0].ToString()));
                                    }
                                    break;
                                case "MigrateDatabase":
                                    {
                                        var mo = parameters.ToJson().DeJson<DataKitTransferVM.MigrateDatabase>();

                                        //引用连接
                                        if (mo.ReadConnectionInfo == null && !string.IsNullOrWhiteSpace(mo.RefReadConnectionInfo))
                                        {
                                            mo.ReadConnectionInfo = cc.ListConnectionInfo.FirstOrDefault(x => x.ConnectionRemark == mo.RefReadConnectionInfo);
                                            if (!string.IsNullOrWhiteSpace(mo.ReadDatabaseName))
                                            {
                                                mo.ReadConnectionInfo.DatabaseName = mo.ReadDatabaseName;
                                            }
                                        }
                                        if (mo.WriteConnectionInfo == null && !string.IsNullOrWhiteSpace(mo.RefWriteConnectionInfo))
                                        {
                                            mo.WriteConnectionInfo = cc.ListConnectionInfo.FirstOrDefault(x => x.ConnectionRemark == mo.RefWriteConnectionInfo);
                                            if (!string.IsNullOrWhiteSpace(mo.WriteDatabaseName))
                                            {
                                                mo.WriteConnectionInfo.DatabaseName = mo.WriteDatabaseName;
                                            }
                                        }

                                        DataKitTo.MigrateDataTable(mo.AsMigrateDataTable(cc.MapingMatchPattern != "Same"), le => DXService.Log(le.NewItems[0].ToString()));
                                    }
                                    break;
                                case "MigrateDataTable":
                                    {
                                        var mo = parameters.ToJson().DeJson<DataKitTransferVM.MigrateDataTable>();

                                        //引用连接
                                        if (mo.ReadConnectionInfo == null && !string.IsNullOrWhiteSpace(mo.RefReadConnectionInfo))
                                        {
                                            mo.ReadConnectionInfo = cc.ListConnectionInfo.FirstOrDefault(x => x.ConnectionRemark == mo.RefReadConnectionInfo);
                                            if (!string.IsNullOrWhiteSpace(mo.ReadDatabaseName))
                                            {
                                                mo.ReadConnectionInfo.DatabaseName = mo.ReadDatabaseName;
                                            }
                                        }
                                        if (mo.WriteConnectionInfo == null && !string.IsNullOrWhiteSpace(mo.RefWriteConnectionInfo))
                                        {
                                            mo.WriteConnectionInfo = cc.ListConnectionInfo.FirstOrDefault(x => x.ConnectionRemark == mo.RefWriteConnectionInfo);
                                            if (!string.IsNullOrWhiteSpace(mo.WriteDatabaseName))
                                            {
                                                mo.WriteConnectionInfo.DatabaseName = mo.WriteDatabaseName;
                                            }
                                        }

                                        DataKitTo.MigrateDataTable(mo, le => DXService.Log(le.NewItems[0].ToString()));
                                    }
                                    break;
                                case "ImportDatabase":
                                    {
                                        var mo = parameters.ToJson().DeJson<DataKitTransferVM.ImportDatabase>();
                                        mo.PackagePath = DXService.ParsePathVar(mo.PackagePath);

                                        //引用连接
                                        if (mo.WriteConnectionInfo == null && !string.IsNullOrWhiteSpace(mo.RefWriteConnectionInfo))
                                        {
                                            mo.WriteConnectionInfo = cc.ListConnectionInfo.FirstOrDefault(x => x.ConnectionRemark == mo.RefWriteConnectionInfo);
                                            if (!string.IsNullOrWhiteSpace(mo.WriteDatabaseName))
                                            {
                                                mo.WriteConnectionInfo.DatabaseName = mo.WriteDatabaseName;
                                            }
                                        }

                                        DataKitTo.ImportDatabase(mo, le => DXService.Log(le.NewItems[0].ToString()));
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

        [Display(Name = "TCP Port Probing", Description = "TCP端口探测",
            ShortName = "tcping [hostname] [port]", Prompt = "ndx tcping zme.ink 443 -t")]
        public static void TCPortProbing()
        {
            int count = 4;
            string hostnameAndPort;
            if (GlobalTo.IsStartWithArgs)
            {
                if (Args.Contains("-t"))
                {
                    count = short.MaxValue;
                    Args.Remove("-t");
                }

                hostnameAndPort = string.Join(" ", Args).Trim();
            }
            else
            {
                Console.Write("hostname port: ");
                hostnameAndPort = Console.ReadLine().Trim();
                Console.WriteLine("");
            }

            var hnp = DXService.ParseHostnameAndPorts(hostnameAndPort);
            var address = Dns.GetHostAddresses(hnp.Item1).First();
            var port = hnp.Item2.Count == 0 ? 80 : hnp.Item2.First();

            var index = 0;
            while (index++ < count)
            {
                var client = new TcpClient();

                var sw = Stopwatch.StartNew();
                var result = client.ConnectAsync(address, port).Wait(2001);
                var time = sw.ElapsedMilliseconds;

                DXService.Log($"Probing {address}:{port}/tcp - {(result ? "Port is open" : "No response")} - time={time}ms");

                if (time < 1000)
                {
                    Thread.Sleep(Convert.ToInt32(1000 - time));
                }
            }
        }

        [Display(Name = "TCP Port Scan", Description = "TCP端口扫描（1-65535）",
            ShortName = "tcpscan [hostname] [ports]", Prompt = "ndx tcpscan zme.ink 80,443,9000-9100")]
        public static void TCPortScan()
        {
            var dvhost = "127.0.0.1";
            var dvports = "22,80,443,3306,9000-9100";
            string hostnameAndPorts;

            if (GlobalTo.IsStartWithArgs)
            {
                hostnameAndPorts = string.Join(" ", Args).Trim();
            }
            else
            {
                Console.Write($"hostname ports(default: {dvhost} {dvports}): ");
                hostnameAndPorts = Console.ReadLine().Trim();
                if (string.IsNullOrWhiteSpace(hostnameAndPorts))
                {
                    hostnameAndPorts = $"{dvhost} {dvports}";
                }
                Console.WriteLine("");
            }

            var hap = DXService.ParseHostnameAndPorts(hostnameAndPorts);
            var address = Dns.GetHostAddresses(hap.Item1).First();

            var listPort = new ConcurrentQueue<int>();
            hap.Item2.ForEach(listPort.Enqueue);

            if (!listPort.IsEmpty)
            {
                DXService.Log($"扫描 {address} 端口(共 {listPort.Count} 个)");
                var result = new ConcurrentBag<int>();

                var sw = Stopwatch.StartNew();

                int scanMax = 20;
                int scanCount = 0;

                while (!listPort.IsEmpty)
                {
                    if (scanCount == scanMax)
                    {
                        Thread.Sleep(50);
                    }
                    else
                    {
                        if (listPort.TryDequeue(out int port))
                        {
                            //+1
                            Interlocked.Increment(ref scanCount);

                            Task.Run(() =>
                            {
                                try
                                {
                                    var client = new TcpClient();
                                    if (client.ConnectAsync(address, port).Wait(1500))
                                    {
                                        result.Add(port);

                                        Console.ForegroundColor = ConsoleColor.Green;
                                        Console.Write($"{port} ");
                                        Console.ForegroundColor = ConsoleColor.White;
                                    }
                                    else
                                    {
                                        Console.Write($"{port} ");
                                    }
                                }
                                catch (Exception) { }

                                //-1
                                Interlocked.Decrement(ref scanCount);
                            });
                        }
                    }
                }

                for (int i = 0; i < 10; i++)
                {
                    if (listPort.IsEmpty && scanCount == 0)
                    {
                        DXService.Log($"\r\n扫描完成, 耗时 {sw.Elapsed}, 发现 {result.Count} 个端口");
                        DXService.Log($"{string.Join(" ", result.ToList())}", ConsoleColor.Green);
                        break;
                    }
                    else
                    {
                        Thread.Sleep(200);
                    }
                }
            }
        }

        [Display(Name = "Port Info", Description = "端口占用信息",
            ShortName = "portinfo", Prompt = "ndx portinfo")]
        public static void PortInfo()
        {
            var listPortInfo = CmdTo.GetPortInfo();
            listPortInfo.ForEach(item =>
            {
                DXService.Log(item.ToJson());
            });
        }

        [Display(Name = "Kill Port", Description = "端口杀进程",
            ShortName = "killport [ports]", Prompt = "ndx killport 80,443")]
        public static void KillPort()
        {
            string ports;
            if (GlobalTo.IsStartWithArgs)
            {
                ports = string.Join(',', Args);
            }
            else
            {
                Console.Write("查杀端口号: ");
                ports = Console.ReadLine();
            }
            if (!string.IsNullOrWhiteSpace(ports))
            {
                var listPortInfo = CmdTo.GetPortInfo();
                ports.Replace(' ', ',').Split(',').ToList().ForEach(port =>
                {
                    if (!string.IsNullOrWhiteSpace(port))
                    {
                        var p = Convert.ToInt32(port);
                        var pinfo = listPortInfo.Where(x => x.Port == p).ToList();
                        if (pinfo.Count == 0)
                        {
                            DXService.Log($"{port} 端口未找到进程");
                        }
                        else
                        {
                            pinfo = pinfo.DistinctBy(x => x.ProcessId).ToList();
                            DXService.Log($"{port} 端口找到 {pinfo.Count} 个进程");
                            pinfo.ForEach(x =>
                            {
                                var proc = Process.GetProcessById(x.ProcessId);
                                try
                                {
                                    proc.Kill();
                                    DXService.Log($"成功: 已终止进程 \"{proc.ProcessName}\"，其 PID 为 {x.ProcessId}。");
                                }
                                catch (Exception ex)
                                {
                                    DXService.Log($"错误: {ex.Message}（进程 \"{proc.ProcessName}\"，其 PID 为 {x.ProcessId}）");
                                }
                            });
                        }
                    }
                });
            }
        }

        [Display(Name = "Device Scan", Description = "设备扫描",
            ShortName = "devicescan [ipRange]", Prompt = "ndx devicescan 192.168.1.1-254")]
        public static void DeviceScan()
        {
            string scanIps = string.Empty;
            if (GlobalTo.IsStartWithArgs)
            {
                scanIps = string.Join(" ", Args).Trim();
            }
            else
            {
                Console.Write($"ip range（如：192.168.1.1-254）: ");
                scanIps = Console.ReadLine().Trim();
                Console.WriteLine("");
            }

            if (string.IsNullOrWhiteSpace(scanIps))
            {
                DXService.Log("IP cannot be empty");
            }
            else
            {
                var listAdds = new List<string>();
                var ips = scanIps.Split('.').ToList();
                var ipRange = ips.Last();
                ips.RemoveAt(ips.Count - 1);
                var ipPrefix = string.Join('.', ips);

                int num1 = 1;
                int num2 = 254;
                if (ipRange.Contains('-'))
                {
                    var ipArr = ipRange.Split('-');
                    num1 = Convert.ToInt32(ipArr[0]);
                    num2 = Convert.ToInt32(ipArr[1]);
                }
                for (int i = num1; i <= num2; i++)
                {
                    listAdds.Add($"{ipPrefix}.{i}");
                }

                var result = new ConcurrentBag<ValueTuple<string, IPStatus>>();
                var data = Encoding.ASCII.GetBytes("abababababababababababababababab");

                foreach (var item in listAdds)
                {
                    var p = new Ping();
                    p.PingCompleted += (s, e) =>
                    {
                        result.Add(new(e.UserState.ToString(), e.Reply.Status));
                        if (e.Reply.Status == IPStatus.Success)
                        {
                            DXService.Log($"{e.UserState}");
                        }

                        if (result.Count == listAdds.Count)
                        {
                            DXService.Log("Done");
                        }
                    };
                    p.SendAsync(item, 2001, data, item);
                }

                while (result.Count != listAdds.Count)
                {
                    Thread.Sleep(10);
                }
            }
        }

        [Display(Name = "Trace Route", Description = "路由追踪",
            ShortName = "traceroute [hostname]", Prompt = "ndx traceoute zme.ink")]
        public static void TraceRoute()
        {
            string hostname;
            if (GlobalTo.IsStartWithArgs)
            {
                hostname = string.Join("", Args);
            }
            else
            {
                Console.Write("hostname: ");
                hostname = Console.ReadLine();
                Console.WriteLine("");
            }

            int timeout = 5000;
            int maxTTL = 30;
            int bufferSize = 32;
            int finalTimeout = 0;
            var ipCache = new Dictionary<IPAddress, string>();

            byte[] buffer = new byte[bufferSize];
            new Random().NextBytes(buffer);

            using var pinger = new Ping();
            var address = Dns.GetHostAddresses(hostname).First();
            DXService.Log($"traceroute to {hostname} ({address}), {maxTTL} hops max, {bufferSize} byte packets\r\n");
            for (int ttl = 1; ttl <= maxTTL; ttl++)
            {
                var options = new PingOptions(ttl, true);
                var reply = pinger.Send(address, timeout, buffer, options);
                if (reply.Status == IPStatus.TimedOut && reply.Address.Equals(address))
                {
                    finalTimeout++;
                }
                if (finalTimeout > 20)
                {
                    break;
                }

                if (!ipCache.ContainsKey(reply.Address))
                {
                    var result = DXService.DomainOrIPInfo(reply.Address.ToString());
                    if (result.Count > 0)
                    {
                        foreach (var key in result.Keys)
                        {
                            ipCache.Add(reply.Address, result[key]);
                            break;
                        }
                    }
                    else
                    {
                        ipCache.Add(reply.Address, "Query failed");
                    }
                }

                var itemInfo = $"{reply.Address,20} {reply.Status,20} {reply.RoundtripTime,5}   {ipCache[reply.Address]}".TrimEnd();
                DXService.Log(itemInfo);

                if (reply.Status != IPStatus.TtlExpired && reply.Status != IPStatus.TimedOut)
                    break;
            }
        }

        [Display(Name = "DNS Resolve", Description = "DNS解析",
            ShortName = "dns [hostname]", Prompt = "ndx dns zme.ink")]
        public static void DNSResolve()
        {
            string hostname;
            if (GlobalTo.IsStartWithArgs)
            {
                hostname = string.Join("", Args);
            }
            else
            {
                Console.Write("hostname: ");
                hostname = Console.ReadLine();
                Console.WriteLine("");
            }

            try
            {
                var addresses = Dns.GetHostAddresses(hostname);
                foreach (var item in addresses)
                {
                    DXService.Log($"{item}");
                }
            }
            catch (Exception ex)
            {
                DXService.Log(ex);
            }

            if (string.IsNullOrWhiteSpace(hostname))
            {
                try
                {
                    using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0);
                    socket.Connect("114.114.114.114", 65530);
                    IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                    DXService.Log($"\r\nLAN IP: {endPoint.Address}", ConsoleColor.Cyan);
                }
                catch (Exception ex)
                {
                    DXService.Log(ex);
                }
            }
        }

        [Display(Name = "Whois", Description = "Whois查询",
            ShortName = "whois [domain]", Prompt = "ndx whois zme.ink")]
        public static void Whois()
        {
            string domain;
            if (GlobalTo.IsStartWithArgs)
            {
                domain = string.Join("", Args);
            }
            else
            {
                Console.Write($"domain: ");
                domain = Console.ReadLine().Trim();
                Console.WriteLine("");
            }

            if (!string.IsNullOrWhiteSpace(domain))
            {
                try
                {
                    var hc = new HttpClient();
                    hc.DefaultRequestHeaders.UserAgent.TryParseAdd("Netnr");
                    hc.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");

                    var url = $"http://m.west.cn/web/whois/whoisinfo?domain={domain}&server=&refresh=0";
                    var stream = hc.GetStreamAsync(url).ToResult();
                    var html = new StreamReader(stream, Encoding.GetEncoding("gbk")).ReadToEnd();
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

                        if (key == "nameserver")
                        {
                            val.Split(',').ToList().ForEach(ns =>
                            {
                                DXService.Log($"{keyMap[key]}: {ns}");
                            });
                        }
                        else
                        {
                            DXService.Log($"{keyMap[key]}: {val}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    DXService.Log(ex);
                }
            }
            else
            {
                DXService.Log("请输入域名");
            }
        }

        [Display(Name = "IP", Description = "IP查询",
            ShortName = "ip [ip|domain]", Prompt = "ndx ip\r\nndx ip 1.1.1.1\r\nndx ip zme.ink")]
        public static void IP()
        {
            string domainOrIP;
            if (GlobalTo.IsStartWithArgs)
            {
                domainOrIP = string.Join("", Args);
            }
            else
            {
                Console.Write($"ip or domain: ");
                domainOrIP = Console.ReadLine().Trim();
                Console.WriteLine("");
            }

            var result = DXService.DomainOrIPInfo(domainOrIP);
            foreach (var key in result.Keys)
            {
                DXService.Log($"{key} {result[key]}");
            }
        }

        [Display(Name = "ICP", Description = "ICP查询",
            ShortName = "icp [domain]", Prompt = "ndx icp qq.com")]
        public static void ICP()
        {
            string domain;
            if (GlobalTo.IsStartWithArgs)
            {
                domain = string.Join("", Args);
            }
            else
            {
                Console.Write($"domain: ");
                domain = Console.ReadLine().Trim();
                Console.WriteLine("");
            }

            if (domain.Contains("://") && Uri.TryCreate(domain, UriKind.Absolute, out Uri uri))
            {
                domain = uri.Host;
            }
            if (!string.IsNullOrWhiteSpace(domain))
            {
                try
                {
                    var url = $"https://micp.chinaz.com/Handle/AjaxHandler.ashx?action=GetPermit&callback=fn&query={domain.ToUrlEncode()}&type=host&_={DateTime.Now.ToTimestamp()}";

                    var hc = new HttpClient();
                    hc.DefaultRequestHeaders.UserAgent.TryParseAdd("Netnr");
                    var html = hc.GetStringAsync(url).ToResult();
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
                            DXService.Log($"{keyMap[key]}: {val}");
                        }
                    }
                    else
                    {
                        DXService.Log("Not found", ConsoleColor.Red);
                    }
                }
                catch (Exception ex)
                {
                    DXService.Log(ex);
                }
            }
            else
            {
                DXService.Log("请输入域名");
            }
        }

        [Display(Name = "SSL", Description = "证书信息",
            ShortName = "ssl [hostname:port|path]", Prompt = "ndx ssl ss.netnr.com\r\nndx ssl zme.ink:443\r\nndx ssl ./zme.ink.crt")]
        public static void SSL()
        {
            string hostnameAndPort;
            if (GlobalTo.IsStartWithArgs)
            {
                hostnameAndPort = string.Join(":", Args);
            }
            else
            {
                Console.Write($"hostname port or path: ");
                hostnameAndPort = Console.ReadLine().Trim();
                Console.WriteLine("");
            }

            //证书文件
            if (File.Exists(hostnameAndPort))
            {
                try
                {
                    DXService.CertificateInformation(new X509Certificate2(hostnameAndPort));
                }
                catch (Exception ex)
                {
                    DXService.Log(ex);
                }
            }
            else
            {
                try
                {
                    var hnp = DXService.ParseHostnameAndPorts(hostnameAndPort);
                    var hostname = hnp.Item1;
                    var port = hnp.Item2.Count == 0 ? 443 : hnp.Item2.First();

                    var client = new TcpClient(hostname, port);
                    var sslStream = new SslStream(client.GetStream(), false, new RemoteCertificateValidationCallback((sender, certificate, chain, sslPolicyErrors) =>
                    {
                        if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateNotAvailable)
                        {
                            DXService.Log($"{sslPolicyErrors} 证书不可用\r\n", ConsoleColor.Red);
                        }
                        else if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateNameMismatch)
                        {
                            DXService.Log($"{sslPolicyErrors} 证书名称不匹配\r\n", ConsoleColor.Red);
                        }
                        else if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors)
                        {
                            DXService.Log($"{sslPolicyErrors}\r\n", ConsoleColor.Red);
                        }

                        DXService.CertificateInformation((X509Certificate2)certificate, chain);

                        return true;
                    }), null);

                    sslStream.AuthenticateAsClient(hostname);
                    client.Close();
                }
                catch (Exception ex)
                {
                    DXService.Log(ex);
                }
            }
        }

        [Display(Name = "Domain Name Information",
            Description = "域名信息查询（合集）", ShortName = "dni [domain]", Prompt = "ndx dni qq.com")]
        public static void DomainNameInformation()
        {
            var isRestore = false;
            if (GlobalTo.IsStartWithArgs == false)
            {
                Console.Write($"domain: ");
                var domain = Console.ReadLine().Trim();

                isRestore = true;
                Args.Clear();
                Args.Add(domain);
                GlobalTo.IsStartWithArgs = true;
            }

            DXService.OutputTitle("DNS");
            DNSResolve();
            DXService.OutputTitle("Whois");
            Whois();
            DXService.OutputTitle("ICP");
            ICP();
            DXService.OutputTitle("IP");
            IP();
            SSL();

            if (isRestore)
            {
                Args.Clear();
                GlobalTo.IsStartWithArgs = false;
            }
        }

        [Display(Name = "System Status", Description = "系统状态",
            ShortName = "ss [-j]|[-s url]", Prompt = "ndx ss\r\nndx ss -j\r\nndx ss -s http://zme.ink")]
        public static void SystemStatus()
        {
            var wType = "view";
            var uri = string.Empty;
            if (GlobalTo.IsStartWithArgs)
            {
                if (Args.Any(x => x == "-j"))
                {
                    wType = "json";
                }
                else if (Args.Any(x => x == "-s" || x == "--send"))
                {
                    wType = "send";
                    uri = Args.FirstOrDefault(x => x.StartsWith("http"));
                }
            }
            else
            {
                var wItems = "view,json,send".Split(',');
                var wNum = DXService.ConsoleReadItem("输出为", wItems);
                wType = wItems[wNum - 1];
                if (wType == "send")
                {
                    Console.Write("Uri: ");
                    uri = Console.ReadLine();
                }
            }

            var ss = new SystemStatusTo();
            ss.Init().ToResult();
            switch (wType)
            {
                case "json":
                    DXService.Log(ss.ToJson(true));
                    break;
                case "send":
                    {
                        DXService.Log($"curl -X POST '{uri}' -H 'Content-Type: multipart/form-data' -F 'paramsJson={{}}'");

                        //https://stackoverflow.com/questions/12553277
                        var handler = new HttpClientHandler
                        {
                            ClientCertificateOptions = ClientCertificateOption.Manual,
                            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                        };
                        using var client = new HttpClient(handler);

                        var req = new HttpRequestMessage(HttpMethod.Post, uri);
                        req.Headers.Add("UserAgent", ConfigInit.ShortName);
                        req.Content = new MultipartFormDataContent
                        {
                            { new StringContent(ss.ToJson()), "paramsJson" }
                        };
                        var resp = client.Send(req);

                        DXService.Log($"StatusCode: {resp.StatusCode}");
                        var result = resp.Content.ReadAsStringAsync().ToResult();
                        DXService.Log($"Result: {result}");
                    }
                    break;
                default:
                    DXService.Log(ss.ToView().ToResult());
                    break;
            }
        }

        [Display(Name = "System Monitor", Description = "系统监控",
            ShortName = "sming", Prompt = "ndx sming", GroupName = "\r\n")]
        public static void SystemMonitor()
        {
            DXService.Log("Starting System Monitor ...");

            var ss = new SystemStatusTo();
            var mw = new SystemStatusTo.MonitorWork();
            ThreadPool.QueueUserWorkItem(_ =>
            {
                string line;
                while ((line = Console.ReadLine()) != "exit")
                {
                    Console.WriteLine($"Enter \"exit\" stop");
                }

                mw.Stop();
            });

            mw.Start((elapsed, receivedSpeed, sentSpeed) =>
            {
                Console.Clear();
                Console.WriteLine($"\r\n Received Speed: {ParsingTo.FormatByteSize(receivedSpeed)}/s");
                Console.Write($"\r\n Sent Speed: {ParsingTo.FormatByteSize(sentSpeed)}/s");
                if (elapsed % 5000 == 0)
                {
                    _ = ss.Init(true);
                }
                Console.WriteLine(ss.ToView().ToResult());
            });
        }

        [Display(Name = "Serve", Description = "启动服务",
            ShortName = "serve --urls [urls] --root [root] --index [index] --404 [404] --suffix [suffix] --charset [charset] --headers [headers] --auth [auth]",
            Prompt = "ndx serve --urls http://*:713;http://*:81 --root ./ --index index.html --404 404.html --suffix .html --charset utf-8 --headers access-control-allow-headers:*||access-control-allow-origin:* --auth user:pass\r\nndx serve",
            GroupName = "\r\n")]
        public static void Serve() => ServeTo.FastStart();

        [Display(Name = "Generate UUID", Description = "生成UUID",
            ShortName = "uuid [count]", Prompt = "ndx uuid\r\nndx uuid 9")]
        public static void GenerateUUID()
        {
            int count = 1;
            if (GlobalTo.IsStartWithArgs)
            {
                if (int.TryParse(Args.FirstOrDefault(), out int val))
                {
                    count = val;
                }
            }
            else
            {
                count = DXService.ConsoleReadNumber("生成个数");
            }

            count = Math.Max(1, Math.Abs(count));

            for (int i = 0; i < count; i++)
            {
                DXService.Log(Guid.NewGuid().ToString());
            }
        }

        [Display(Name = "Generate Snowflake", Description = "生成雪花ID",
            ShortName = "snow [count]", Prompt = "ndx snow\r\nndx snow 9")]
        public static void GenerateSnowflake()
        {
            int count = 1;
            if (GlobalTo.IsStartWithArgs)
            {
                if (int.TryParse(Args.FirstOrDefault(), out int val))
                {
                    count = val;
                }
            }
            else
            {
                count = DXService.ConsoleReadNumber("生成个数");
            }

            count = Math.Max(1, Math.Abs(count));

            for (int i = 0; i < count; i++)
            {
                DXService.Log(Snowflake53To.Id().ToString());
            }
        }

        [Display(Name = "Tail", Description = "读取文件最新内容",
            ShortName = "tail [file]", Prompt = "ndx tail access.log")]
        public static void Tail()
        {
            string filePath;
            if (GlobalTo.IsStartWithArgs)
            {
                filePath = Args.FirstOrDefault();
            }
            else
            {
                filePath = DXService.ConsoleReadPath("文件路径", 1);
            }

            if (File.Exists(filePath))
            {
                using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var sr = new StreamReader(fs);
                //初次读取末尾少量内容
                if (fs.Length > 0)
                {
                    Console.WriteLine(Environment.NewLine);

                    var startIndex = Math.Max(0, fs.Length - 9999);
                    fs.Position = startIndex;
                    var lastContent = sr.ReadToEnd().Split('\n').TakeLast(15);
                    foreach (var row in lastContent)
                    {
                        Console.WriteLine(row);
                    }
                }

                sr.BaseStream.Seek(fs.Length, SeekOrigin.Begin);

                while (true)
                {
                    //小于等于全文
                    if (fs.Length < fs.Position)
                    {
                        fs.Position = fs.Length;
                    }

                    // 实时读取文件内容
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        Console.WriteLine(line);
                    }

                    Thread.Sleep(500);
                }
            }
            else
            {
                DXService.Log("file not found");
            }
        }

        [Display(Name = "Text encoding conversion", Description = "文本编码转换",
            ShortName = "tec [path|file] [-utf8|-gb2312]", Prompt = "ndx tec ./ndx")]
        public static void TextEncodingConversion()
        {
            TextEncodingConversionService.Run();
        }

        [Display(Name = "deep delete", Description = "深度删除指定文件",
            ShortName = "ddel [filter] [dir]", Prompt = "ndx ddel bin,demo,modules,package.json,-node.js,.,.md,.config.js ./")]
        public static void DeepDelete()
        {
            string ddFilter;
            string ddDir = string.Empty;
            if (GlobalTo.IsStartWithArgs)
            {
                ddFilter = Args[0];
                if (Args.Count > 1)
                {
                    ddDir = Args[1];
                }
            }
            else
            {
                Console.Write("请输入清理对象(如: bin,.md,.json): ");
                ddFilter = Console.ReadLine();

                ddDir = DXService.ConsoleReadPath("根目录", 2, Environment.CurrentDirectory);
            }

            if (!string.IsNullOrWhiteSpace(ddFilter) && Directory.Exists(ddDir))
            {
                var listFilter = ddFilter.Split(',').ToList();
                FileTo.EachDirectory(ddDir, (dirs, files) =>
                {
                    foreach (var item in dirs)
                    {
                        listFilter.ForEach(filter =>
                        {
                            if (filter == ".")
                            {
                                if (item.Name.StartsWith(filter))
                                {
                                    DXService.Log($"delete directory: {item.FullName}", ConsoleColor.Red);
                                    item.Delete(true);
                                }
                            }
                            else if (filter.StartsWith('.') || filter.StartsWith('-'))
                            {
                                if (item.Name.EndsWith(filter))
                                {
                                    DXService.Log($"delete directory: {item.FullName}", ConsoleColor.Red);
                                    item.Delete(true);
                                }
                            }
                            else if (filter.EndsWith('.'))
                            {
                                if (item.Name.StartsWith(filter))
                                {
                                    DXService.Log($"delete directory: {item.FullName}", ConsoleColor.Red);
                                    item.Delete(true);
                                }
                            }
                            else
                            {
                                if (item.Name.Trim() == filter.Trim())
                                {
                                    DXService.Log($"delete directory: {item.FullName}", ConsoleColor.Red);
                                    item.Delete(true);
                                }
                            }
                        });
                    }
                    foreach (var item in files)
                    {
                        listFilter.ForEach(filter =>
                        {
                            if (filter == ".")
                            {
                                if (item.Name.StartsWith(filter))
                                {
                                    DXService.Log($"delete file: {item.FullName}", ConsoleColor.Red);
                                    item.Delete();
                                }
                            }
                            else if (filter.StartsWith('.') || filter.StartsWith('-'))
                            {
                                if (item.Name.EndsWith(filter))
                                {
                                    DXService.Log($"delete file: {item.FullName}", ConsoleColor.Red);
                                    item.Delete();
                                }
                            }
                            else if (filter.EndsWith('.'))
                            {
                                if (item.Name.StartsWith(filter))
                                {
                                    DXService.Log($"delete file: {item.FullName}", ConsoleColor.Red);
                                    item.Delete();
                                }
                            }
                            else
                            {
                                if (item.Name == filter)
                                {
                                    DXService.Log($"delete file: {item.FullName}", ConsoleColor.Red);
                                    item.Delete();
                                }
                            }
                        });
                    }
                });
            }
        }

        [Display(Name = "Clear Memory", Description = "清理内存（仅限 Windows）",
            ShortName = "clearmemory", Prompt = "ndx clearmemory")]
        public static void ClearMemory()
        {
            if (DXService.ConsoleReadBool("清理有风险，是否继续"))
            {
                ClearMemoryService.CleanUp();
            }
        }

        [Display(Name = "Git Pull", Description = "批量拉取",
            ShortName = "gitpull [path]", Prompt = "ndx gitpull ./")]
        public static void GitPull()
        {
            string path;
            if (GlobalTo.IsStartWithArgs)
            {
                path = string.Join("", Args);
            }
            else
            {
                path = DXService.ConsoleReadPath("请输入目录", 2, Environment.CurrentDirectory);
            }

            if (!Directory.Exists(path))
            {
                path = Environment.CurrentDirectory;
            }

            var di = new DirectoryInfo(path);
            var sdis = di.GetDirectories().ToList();
            if (Directory.Exists(Path.Combine(di.FullName, ".git")))
            {
                sdis.Insert(0, di);
            }

            DXService.Log($"\n{sdis.Count} 个项目\n");

            int c1 = 0;
            int c2 = 0;
            var listUnsafe = new List<string>();
            sdis.AsParallel().ForAll(sdi =>
            {
                if (Directory.Exists(sdi.FullName + "/.git"))
                {
                    var arg = $"git -C \"{sdi.FullName}\" pull --all";
                    var cr = CmdTo.Execute(arg);
                    if (cr.CrError.Contains("unsafe repository"))
                    {
                        DXService.Log($"add safe.directory {sdi.FullName}, please try again");
                    }
                    else
                    {
                        var rt = cr.CrOutput + cr.CrError;
                        DXService.Log($"【 {sdi.Name} 】\n{rt}");
                        c1++;
                    }
                }
                else
                {
                    DXService.Log($"已跳过 \"{sdi.FullName}\", 未找到 .git\n");
                    c2++;
                }
            });
            DXService.Log($"Done!  Pull: {c1}, Skip: {c2}");

            if (listUnsafe.Count > 0)
            {
                Console.WriteLine(Environment.NewLine);
                DXService.Log(string.Join("\r\n", listUnsafe));
            }
        }

        [Display(Name = "Safe Copy", Description = "安全拷贝",
            ShortName = "scopy [sourcePath] [targetPath] [ignoreForder]", Prompt = @"ndx pcopy D:\\site\\npp D:\\site\\np bin,obj,PublishProfiles,node_modules,packages,.git,.svg,.vs,.config,.vercel")]
        public static void SafeCopy()
        {
            string sourcePath;
            string targetPath;
            string ignoreForder = string.Empty;
            if (GlobalTo.IsStartWithArgs)
            {
                sourcePath = Args[0];
                targetPath = Args[1];
                if (Args.Count == 3)
                {
                    ignoreForder = Args[2];
                }
            }
            else
            {
                var sourceDv = Path.Combine(Environment.CurrentDirectory, "npp");
                sourcePath = DXService.ConsoleReadPath("请输入源目录", 2, sourceDv);

                var targetDv = Path.Combine(Environment.CurrentDirectory, "np");
                targetPath = DXService.ConsoleReadPath("请输入目标目录", 2, targetDv);

                Console.Write("\r\n请输入忽略的目录: ");
                ignoreForder = Console.ReadLine();
            }

            DXService.Log($"Ignored folders\n{ignoreForder}");

            FileTo.CopyDirectory(sourcePath, targetPath, ignoreForder.Split(','));

            DXService.Log($"Copy completed!");
        }
    }
}
