using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Collections.Concurrent;

namespace Netnr.DataX.Application
{
    /// <summary>
    /// Network
    /// </summary>
    public partial class MenuItemService
    {
        [Display(Name = "TCPing", Description = "TCP 端口探测", GroupName = "Network",
            ShortName = "tcping [host] [port]", Prompt = "ndx tcping zme.ink 443 -t")]
        public static void TCPing()
        {
            var host = DXService.VarString(0, "host");
            var port = DXService.VarString(1, "port(default: 80)");
            _ = int.TryParse(port, out int portNumber);
            if (portNumber < 1)
            {
                portNumber = 80;
            }

            var keep = DXService.VarBool("持续 PING", "-t");
            int count = keep ? short.MaxValue : 4;

            if (!string.IsNullOrWhiteSpace(host))
            {
                Console.WriteLine("");
                var index = 0;
                while (index++ < count)
                {
                    var client = new TcpClient();

                    var sw = Stopwatch.StartNew();
                    var result = client.ConnectAsync(host, portNumber).Wait(2001);
                    var time = sw.ElapsedMilliseconds;

                    DXService.Log($"Probing {host}:{portNumber}/tcp - {(result ? "Port is open" : "No response")} - time={time}ms");

                    if (time < 1000)
                    {
                        Thread.Sleep(Convert.ToInt32(1000 - time));
                    }
                }
            }
        }

        [Display(Name = "TCP Scan", Description = "TCP端口扫描（1-65535）", GroupName = "Network", AutoGenerateFilter = true,
            ShortName = "tcpscan [host] [ports]", Prompt = "ndx tcpscan zme.ink 21,22,53,80,443,9000-9100")]
        public static void TCPScan()
        {
            var dvhost = "127.0.0.1";
            var host = DXService.VarString(0, $"host(default: {dvhost})");
            if (string.IsNullOrWhiteSpace(host))
            {
                host = dvhost;
            }

            var dvports = "21,22,53,80,443,9000-9100";
            var ports = DXService.VarString(1, $"ports(default: {dvports})");
            if (string.IsNullOrWhiteSpace(ports))
            {
                ports = dvports;
            }

            var portList = DXService.RangeToList(ports, 1, 65535);
            var portQueue = new ConcurrentQueue<int>();
            foreach (var portItem in portList)
            {
                portQueue.Enqueue(portItem);
            }

            if (portList.Any())
            {
                DXService.Log($"扫描 {host} 端口(共 {portList.Count} 个)\r\n");
                var result = new ConcurrentBag<int>();

                var sw = Stopwatch.StartNew();

                var scanMax = 20;
                var scanCurr = 0;

                do
                {
                    if (scanCurr < scanMax)
                    {
                        //+1
                        Interlocked.Increment(ref scanCurr);
                        Task.Run(() =>
                        {
                            if (portQueue.TryDequeue(out int port))
                            {
                                try
                                {
                                    var client = new TcpClient();
                                    if (client.ConnectAsync(host, port).Wait(2000))
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
                            }

                            //-1
                            Interlocked.Decrement(ref scanCurr);
                        });
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                } while (!portQueue.IsEmpty || scanCurr != 0);

                DXService.Log($"\r\n扫描完成, 耗时 {sw.Elapsed}, 发现 {result.Count} 个端口", ConsoleColor.Cyan);
                DXService.Log($"{string.Join(" ", result.ToList())}", ConsoleColor.Green);
            }
        }

        [Display(Name = "Device Scan", Description = "设备扫描", GroupName = "Network",
            ShortName = "devicescan [ipRange]", Prompt = "ndx devicescan 192.168.1.1-254")]
        public static void DeviceScan()
        {
            string ipRange = DXService.VarString(0, "ip range(demo: 192.168.1.1-254)");
            if (string.IsNullOrWhiteSpace(ipRange))
            {
                DXService.Log("IP cannot be empty", ConsoleColor.Red);
            }
            else
            {
                var listAdds = new List<string>();
                var ips = ipRange.Split('.').ToList();

                var ranges = ips.Last();
                ips.RemoveAt(ips.Count - 1);
                var ipPrefix = string.Join('.', ips);

                int num1 = 1;
                int num2 = 254;
                if (ranges.Contains('-'))
                {
                    var ipArr = ranges.Split('-');
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
                            DXService.Log("Done!");
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

        [Display(Name = "Trace Route", Description = "路由追踪", GroupName = "Network", AutoGenerateFilter = true,
            ShortName = "traceroute [host]", Prompt = "ndx traceoute zme.ink")]
        public static async Task TraceRoute()
        {
            string host = DXService.VarString(0, "host");

            int timeout = 5000;
            int maxTTL = 30;
            int bufferSize = 32;
            int finalTimeout = 0;
            var ipCache = new Dictionary<IPAddress, string>();

            byte[] buffer = new byte[bufferSize];
            new Random().NextBytes(buffer);

            using var pinger = new Ping();
            var address = (await Dns.GetHostAddressesAsync(host)).First();
            DXService.Log($"traceroute to {host} ({address}), {maxTTL} hops max, {bufferSize} byte packets\r\n");
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
                    var result = await DXService.DomainOrIPInfo(reply.Address.ToString());
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

        [Display(Name = "Whois", Description = "Whois查询", GroupName = "Network",
            ShortName = "whois [domain]", Prompt = "ndx whois zme.ink")]
        public static async Task Whois()
        {
            string domain = DXService.VarString(0, "domain");
            if (!string.IsNullOrWhiteSpace(domain))
            {
                await DXService.QueryWhois(domain);
            }
            else
            {
                DXService.Log("请输入域名", ConsoleColor.Red);
            }
        }

        [Display(Name = "DNS Resolve", Description = "DNS解析", GroupName = "Network",
            ShortName = "dns [host]", Prompt = "ndx dns zme.ink")]
        public static async Task DNSResolve()
        {
            string host = DXService.VarString(0, "host");

            try
            {
                var addresses = await Dns.GetHostAddressesAsync(host);
                foreach (var item in addresses)
                {
                    DXService.Log($"{item}");
                }
            }
            catch (Exception ex)
            {
                DXService.Log(ex);
            }

            if (string.IsNullOrWhiteSpace(host))
            {
                try
                {
                    var endPoint = await SystemStatusTo.GetAddressInterNetwork();
                    DXService.Log($"\r\nLAN IP: {endPoint.Address}", ConsoleColor.Cyan);
                }
                catch (Exception ex)
                {
                    DXService.Log(ex);
                }
            }
        }

        [Display(Name = "IP", Description = "IP查询", GroupName = "Network",
            ShortName = "ip [ipOrDomain]", Prompt = "ndx ip\r\nndx ip 1.1.1.1\r\nndx ip zme.ink")]
        public static async Task IP()
        {
            string ipOrDomain = DXService.VarString(0, "ip or domain");
            var result = await DXService.DomainOrIPInfo(ipOrDomain);
            foreach (var key in result.Keys)
            {
                DXService.Log($"{key} {result[key]}");
            }
        }

        [Display(Name = "ICP", Description = "ICP查询", GroupName = "Network",
            ShortName = "icp [domain]", Prompt = "ndx icp qq.com")]
        public static async Task ICP()
        {
            string domain = DXService.VarString(0, "domain");

            if (domain.Contains("://") && Uri.TryCreate(domain, UriKind.Absolute, out Uri uri))
            {
                domain = uri.Host;
            }
            if (!string.IsNullOrWhiteSpace(domain))
            {
                if (!await DXService.QueryICP(domain))
                {
                    if (domain.Split('.').Length > 2 && domain.StartsWith("www."))
                    {
                        var tryFixDomain = domain[4..];
                        DXService.Log($"尝试查询 {tryFixDomain}", ConsoleColor.Cyan);
                        await DXService.QueryICP(tryFixDomain);
                    }
                }
            }
            else
            {
                DXService.Log("请输入域名");
            }
        }

        [Display(Name = "SSL", Description = "证书信息", GroupName = "Network",
            ShortName = "ssl [hostname:port|path]", Prompt = "ndx ssl ss.netnr.com\r\nndx ssl zme.ink:443\r\nndx ssl ./zme.ink.crt")]
        public static void SSL()
        {
            string hostnameAndPort = DXService.VarString(0, "host:port or path");

            try
            {
                //证书文件
                if (File.Exists(hostnameAndPort))
                {
                    DXService.CheckSSL(filePath: hostnameAndPort);
                }
                else
                {
                    var hnp = DXService.ParseHostnameAndPorts(hostnameAndPort);
                    var hostname = hnp.Item1;
                    var port = hnp.Item2.Count == 0 ? 443 : hnp.Item2.First();

                    var uri = new UriBuilder("https", hostname, port);
                    DXService.CheckSSL(uri.Uri);
                }
            }
            catch (Exception ex)
            {
                DXService.Log(ex);
            }
        }

        [Display(Name = "Domain Name Information", GroupName = "Network", AutoGenerateFilter = true,
            Description = "域名信息查询（合集）", ShortName = "dni [domain]", Prompt = "ndx dni qq.com")]
        public static async Task DomainNameInformation()
        {
            string domain = DXService.VarString(0, "domain");

            var isRestore = false;
            if (BaseTo.IsWithArgs == false)
            {
                isRestore = true;
                DXService.Args.Clear();
                DXService.Args.Add(domain);
                BaseTo.IsWithArgs = true;
            }

            DXService.OutputTitle("Whois");
            await Whois();
            DXService.OutputTitle("DNS");
            await DNSResolve();
            DXService.OutputTitle("IP");
            await IP();
            DXService.OutputTitle("ICP");
            await ICP();
            DXService.OutputTitle("SSL/TLS");
            SSL();

            if (isRestore)
            {
                DXService.Args.Clear();
                BaseTo.IsWithArgs = false;
            }
        }

        [Display(Name = "Serve", Description = "启动服务", GroupName = "Network",
            ShortName = "serve --urls [urls] --root [root] --index [index] --404 [404] --suffix [suffix] --charset [charset] --headers [headers] --auth [auth]",
            Prompt = "ndx serve --urls http://*:713;http://*:81 --root ./ --index index.html --404 404.html --suffix .html --charset utf-8 --headers access-control-allow-headers:*||access-control-allow-origin:* --auth user:pass\r\nndx serve")]
        public static void Serve() => ServeTo.FastStart();
    }
}
