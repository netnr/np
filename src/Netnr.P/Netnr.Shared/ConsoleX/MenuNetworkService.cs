#if Full || ConsoleX

using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Collections.Concurrent;

namespace Netnr;

/// <summary>
/// Network
/// </summary>
public partial class MenuItemService
{
    [Display(Name = "TCPing", Description = "TCP 端口探测", GroupName = "Network",
        ShortName = "tcping [host] [port]", Prompt = "tcping zme.ink 443 -t")]
    public static async Task TCPing()
    {
        var host = ConsoleXTo.VarIndex(0, "host");
        var port = ConsoleXTo.VarIndex(1, "port(default: 80)");
        _ = int.TryParse(port, out int portNumber);
        if (portNumber < 1)
        {
            portNumber = 80;
        }

        var keep = ConsoleXTo.VarBool("持续 PING", "-t");
        int count = keep ? short.MaxValue : 4;

        if (!string.IsNullOrWhiteSpace(host))
        {
            if (!IPAddress.TryParse(host, out IPAddress _))
            {
                var listIps = await Dns.GetHostAddressesAsync(host);
                if (ConsoleXTo.CmdArgs.Any(x => x == "-6"))
                {
                    host = listIps.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetworkV6)?.ToString();
                }
                else if (ConsoleXTo.CmdArgs.Any(x => x == "-4"))
                {
                    host = listIps.FirstOrDefault(x => x.AddressFamily != AddressFamily.InterNetworkV6)?.ToString();
                }
            }

            if (string.IsNullOrWhiteSpace(host))
            {
                ConsoleTo.LogColor("get host is empty", ConsoleColor.Red);
            }
            else
            {
                Console.WriteLine("");
                var index = 0;
                bool? lastResult = null;

                var sw = Stopwatch.StartNew();

                while (index++ < count)
                {
                    var client = new TcpClient();

                    bool result = false;
                    try
                    {
                        result = client.ConnectAsync(host, portNumber).Wait(2001);
                    }
                    catch (Exception)
                    {
                        Thread.Sleep(Convert.ToInt32(2000 - sw.ElapsedMilliseconds));
                    }

                    var time = sw.ElapsedMilliseconds;
                    var openNotify = index > 1 && count > 4 && lastResult.Value == false && result;
                    if (openNotify)
                    {
                        ConsoleXTo.Notify(nameof(TCPing), $"{host}:{portNumber}/tcp Port is open");
                    }
                    lastResult = result;

                    ConsoleTo.LogColor($"Probing {host}:{portNumber}/tcp - {(result ? "Port is open" : "No response")} - time={time}ms", openNotify ? ConsoleColor.Green : null);
                    if (time < 1000)
                    {
                        Thread.Sleep(Convert.ToInt32(1000 - time));
                    }
                    sw.Restart();
                }
            }
        }
    }

    [Display(Name = "TCP Scan", Description = "TCP端口扫描（1-65535）", GroupName = "Network", AutoGenerateFilter = true,
        ShortName = "tcpscan [host] [ports]", Prompt = "tcpscan zme.ink 21,22,53,80,443,9000-9100")]
    public static void TCPScan()
    {
        var dvhost = "127.0.0.1";
        var host = ConsoleXTo.VarIndex(0, $"host(default: {dvhost})");
        if (string.IsNullOrWhiteSpace(host))
        {
            host = dvhost;
        }

        var dvports = "21,22,53,80,443,9000-9100";
        var ports = ConsoleXTo.VarIndex(1, $"ports(default: {dvports})");
        if (string.IsNullOrWhiteSpace(ports))
        {
            ports = dvports;
        }

        var portList = ConsoleXTo.RangeToList(ports, 1, 65535);
        var portQueue = new ConcurrentQueue<int>();
        foreach (var portItem in portList)
        {
            portQueue.Enqueue(portItem);
        }

        if (portList.Count != 0)
        {
            ConsoleTo.LogColor($"扫描 {host} 端口(共 {portList.Count} 个)\r\n");
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
                                    Console.ResetColor();
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

            ConsoleTo.LogColor($"\r\n扫描完成, 耗时 {sw.Elapsed}, 发现 {result.Count} 个端口", ConsoleColor.Cyan);
            ConsoleTo.LogColor($"{string.Join(" ", result.ToList())}", ConsoleColor.Green);
        }
    }

    [Display(Name = "Device Scan", Description = "设备扫描", GroupName = "Network",
        ShortName = "devicescan [ipRange]", Prompt = "devicescan 192.168.1.1-254")]
    public static void DeviceScan()
    {
        string ipRange = ConsoleXTo.VarIndex(0, "ip range(demo: 192.168.1.1-254)");
        if (string.IsNullOrWhiteSpace(ipRange))
        {
            ConsoleTo.LogColor("IP cannot be empty", ConsoleColor.Red);
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
                        ConsoleTo.LogColor($"{e.UserState}");
                    }

                    if (result.Count == listAdds.Count)
                    {
                        ConsoleTo.LogColor("Done!");
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

    [Display(Name = "Trace Route", Description = "路由追踪", GroupName = "Network",
        ShortName = "traceroute [host]", Prompt = "traceoute zme.ink")]
    public static async Task TraceRoute()
    {
        string host = ConsoleXTo.VarIndex(0, "host");

        int timeout = 5000;
        int maxTTL = 30;
        int bufferSize = 32;
        int finalTimeout = 0;
        var ipCache = new Dictionary<IPAddress, string>();

        byte[] buffer = new byte[bufferSize];
        new Random().NextBytes(buffer);

        using var pinger = new Ping();
        var address = (await Dns.GetHostAddressesAsync(host)).First();
        ConsoleTo.LogColor($"traceroute to {host} ({address}), {maxTTL} hops max, {bufferSize} byte packets\r\n", ConsoleColor.Cyan);
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

            if (ipCache.TryAdd(reply.Address, "Query failed"))
            {
                var result = await ConsoleXTo.DomainOrIPInfo(reply.Address.ToString());
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
                }
            }

            var ms = reply.RoundtripTime == 0 ? $"{"0",5}   " : $"{reply.RoundtripTime,5} ms";
            ConsoleColor? cc = reply.Status == IPStatus.Success ? ConsoleColor.Green : reply.Status == IPStatus.TimedOut ? ConsoleColor.Red : null;
            ConsoleTo.LogColor($"{ttl,3} {ms} {reply.Status,15} {reply.Address,30}  {ipCache[reply.Address]}", cc);

            if (reply.Status != IPStatus.TtlExpired && reply.Status != IPStatus.TimedOut)
                break;
        }
    }

    [Display(Name = "Wake On LAN", Description = "局域网唤醒", GroupName = "Network", AutoGenerateFilter = true,
        ShortName = "wol [mac]", Prompt = "wol 04-D4-C4-20-03-AC")]
    public static void WakeOnLAN()
    {
        var macAddress = ConsoleXTo.VarIndex(0, $"MAC");
        if (!string.IsNullOrWhiteSpace(macAddress))
        {
            macAddress = macAddress.Replace(":", "").Replace("-", "").ToUpper();
            if (macAddress.Length != 12)
            {
                ConsoleTo.LogColor("无效的MAC地址！", ConsoleColor.Red);
            }
            else
            {
                // 将MAC地址转换为字节数组
                byte[] macBytes = new byte[6];
                for (int i = 0; i < 6; i++)
                {
                    macBytes[i] = Convert.ToByte(macAddress.Substring(i * 2, 2), 16);
                }

                // 组装唤醒包数据（首先是6个字节的0xFF，即全为1的二进制值，紧接着是目标计算机的MAC地址重复16次）
                byte[] packet = new byte[102];
                for (int i = 0; i < 6; i++)
                {
                    packet[i] = 0xFF;
                }
                for (int i = 6; i < 102; i += 6)
                {
                    Array.Copy(macBytes, 0, packet, i, 6);
                }

                // 使用UDP发送唤醒包到广播地址
                using (var client = new UdpClient())
                {
                    client.Connect(IPAddress.Broadcast, 9); // 设置广播地址和端口
                    client.Send(packet, packet.Length);
                }

                ConsoleTo.LogColor("唤醒包已发送！", ConsoleColor.Cyan);
            }
        }
        else
        {
            ConsoleTo.LogColor("无效的MAC地址！", ConsoleColor.Red);
        }
    }

    [Display(Name = "Whois", Description = "Whois查询", GroupName = "Network",
        ShortName = "whois [domain]", Prompt = "whois zme.ink")]
    public static async Task Whois()
    {
        string domain = ConsoleXTo.VarIndex(0, "domain");
        if (!string.IsNullOrWhiteSpace(domain))
        {
            await ConsoleXTo.QueryWhois(domain);
        }
        else
        {
            ConsoleTo.LogColor("请输入域名", ConsoleColor.Red);
        }
    }

    [Display(Name = "DNS Resolve", Description = "DNS解析", GroupName = "Network",
        ShortName = "dns [host]", Prompt = "dns zme.ink")]
    public static async Task DNSResolve()
    {
        string host = ConsoleXTo.VarIndex(0, "host");

        try
        {
            var addresses = await Dns.GetHostAddressesAsync(host);
            foreach (var item in addresses)
            {
                ConsoleTo.LogColor($"{item}");
            }
        }
        catch (Exception ex)
        {
            ConsoleTo.LogError(ex);
        }

        if (string.IsNullOrWhiteSpace(host))
        {
            try
            {
                var endPoint = await SystemStatusTo.GetAddressInterNetwork();
                ConsoleTo.LogColor($"\r\nLAN IP: {endPoint.Address}", ConsoleColor.Cyan);
            }
            catch (Exception ex)
            {
                ConsoleTo.LogError(ex);
            }
        }
    }

    [Display(Name = "IP", Description = "IP查询", GroupName = "Network",
        ShortName = "ip [ipOrDomain]", Prompt = "ip\r\nip 1.1.1.1\r\nip zme.ink")]
    public static async Task IP()
    {
        string ipOrDomain = ConsoleXTo.VarIndex(0, "ip or domain");
        var result = await ConsoleXTo.DomainOrIPInfo(ipOrDomain);
        foreach (var key in result.Keys)
        {
            ConsoleTo.LogColor($"{key} {result[key]}");
        }
    }

    [Display(Name = "ICP", Description = "ICP查询", GroupName = "Network",
        ShortName = "icp [domain]", Prompt = "icp qq.com")]
    public static async Task ICP()
    {
        string domain = ConsoleXTo.VarIndex(0, "domain");

        if (domain.Contains("://") && Uri.TryCreate(domain, UriKind.Absolute, out Uri uri))
        {
            domain = uri.Host;
        }
        if (!string.IsNullOrWhiteSpace(domain))
        {
            if (!await ConsoleXTo.QueryICP(domain))
            {
                if (domain.Split('.').Length > 2 && domain.StartsWith("www."))
                {
                    var tryFixDomain = domain[4..];
                    ConsoleTo.LogColor($"尝试查询 {tryFixDomain}", ConsoleColor.Cyan);
                    await ConsoleXTo.QueryICP(tryFixDomain);
                }
            }
        }
        else
        {
            ConsoleTo.LogColor("请输入域名");
        }
    }

    [Display(Name = "SSL", Description = "证书信息", GroupName = "Network",
        ShortName = "ssl [hostname:port|path]", Prompt = "ssl ss.netnr.com\r\nssl zme.ink:443\r\nssl ./zme.ink.crt")]
    public static void SSL()
    {
        string hostnameAndPort = ConsoleXTo.VarIndex(0, "host:port or path");

        try
        {
            //证书文件
            if (File.Exists(hostnameAndPort))
            {
                ConsoleXTo.CheckSSL(filePath: hostnameAndPort);
            }
            else
            {
                var hnp = ConsoleXTo.ParseHostnameAndPorts(hostnameAndPort);
                var hostname = hnp.Item1;
                var port = hnp.Item2.Count == 0 ? 443 : hnp.Item2.First();

                var uri = new UriBuilder("https", hostname, port);
                ConsoleXTo.CheckSSL(uri.Uri);
            }
        }
        catch (Exception ex)
        {
            ConsoleTo.LogError(ex);
        }
    }

    [Display(Name = "Domain Name Information", GroupName = "Network", AutoGenerateFilter = true,
        Description = "域名信息查询（合集）", ShortName = "dni [domain]", Prompt = "dni qq.com")]
    public static async Task DomainNameInformation()
    {
        string domain = ConsoleXTo.VarIndex(0, "domain");

        var isRestore = false;
        if (BaseTo.IsCmdArgs == false)
        {
            isRestore = true;
            ConsoleXTo.CmdArgs.Clear();
            ConsoleXTo.CmdArgs.Add(domain);
            BaseTo.IsCmdArgs = true;
        }

        ConsoleTo.LogCard("Whois");
        await Whois();
        ConsoleTo.LogCard("DNS");
        await DNSResolve();
        ConsoleTo.LogCard("IP");
        await IP();
        ConsoleTo.LogCard("ICP");
        await ICP();
        ConsoleTo.LogCard("SSL/TLS");
        SSL();

        if (isRestore)
        {
            ConsoleXTo.CmdArgs.Clear();
            BaseTo.IsCmdArgs = false;
        }
    }

    [Display(Name = "Serve", Description = "启动服务", GroupName = "Network",
        ShortName = "serve --urls [urls] --root [root] --index [index] --404 [404] --suffix [suffix] --charset [charset] --readonly [readonly] --auth [auth] --headers [headers]",
        Prompt = "serve --urls http://*:713;http://*:81 --root ./ --index index.html --404 404.html --suffix .html --charset utf-8 --readonly true --auth user:pass --headers access-control-allow-origin:*||x-server:Netnr\r\nserve")]
    public static async Task Serve() => await ServeTo.QuickStart();
}

#endif