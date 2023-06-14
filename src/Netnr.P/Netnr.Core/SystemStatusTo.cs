using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Diagnostics;

namespace Netnr;

/// <summary>
/// 系统状态
/// </summary>
public class SystemStatusTo
{
    /// <summary>
    /// 时间
    /// </summary>
    public DateTime Now { get; set; } = DateTime.Now;

    /// <summary>
    /// 操作系统
    /// </summary>
    public string OperatingSystem { get; set; }

    /// <summary>
    /// 代表操作系统平台
    /// </summary>
    public string OSType { get; set; }

    /// <summary>
    /// 系统架构
    /// </summary>
    public Architecture OSArchitecture { get; set; } = RuntimeInformation.OSArchitecture;

    /// <summary>
    /// 系统版本
    /// </summary>
    public Version OSVersion { get; set; } = Environment.OSVersion.Version;

    /// <summary>
    /// 获取描述应用程序正在运行的操作系统的字符串
    /// </summary>
    public string OSDescription { get; set; } = RuntimeInformation.OSDescription;

    /// <summary>
    /// 获取与当前用户关联的网络域名
    /// </summary>
    public string UserDomainName { get; set; } = Environment.UserDomainName;

    /// <summary>
    /// 获取当前登录到操作系统的用户的用户名
    /// </summary>
    public string UserName { get; set; } = Environment.UserName;

    /// <summary>
    /// 获取自系统启动以来经过的毫秒数
    /// </summary>
    public long TickCount { get; set; } = Environment.TickCount;

    /// <summary>
    /// 型号
    /// </summary>
    public string Model { get; set; }

    /// <summary>
    /// 处理器名称
    /// </summary>
    public string ProcessorName { get; set; }

    /// <summary>
    /// 处理器架构
    /// </summary>
    public Architecture ProcessArchitecture { get; set; } = RuntimeInformation.ProcessArchitecture;

    /// <summary>
    /// 获取当前计算机上的处理器数量
    /// </summary>
    public int ProcessorCount { get; set; } = Environment.ProcessorCount;

    /// <summary>
    /// 处理器使用率（0-100）
    /// </summary>
    public decimal ProcessorUsage { get; set; } = 0;

    /// <summary>
    /// 总物理内存 B
    /// </summary>
    public long TotalPhysicalMemory { get; set; } = 0;

    /// <summary>
    /// 可用物理内存 B
    /// </summary>
    public long FreePhysicalMemory { get; set; } = 0;

    /// <summary>
    /// 已用物理内存 B
    /// </summary>
    public long UsedPhysicalMemory { get; set; } = 0;

    /// <summary>
    /// 总交换空间（Linux）B
    /// </summary>
    public long SwapTotal { get; set; } = 0;

    /// <summary>
    /// 可用交换空间（Linux）B
    /// </summary>
    public long SwapFree { get; set; } = 0;

    /// <summary>
    /// 已用交换空间（Linux）B
    /// </summary>
    public long SwapUsed { get; set; } = 0;

    /// <summary>
    /// 逻辑磁盘
    /// </summary>
    public List<LogicalDiskModel> LogicalDiskList { get; set; } = new List<LogicalDiskModel>();

    /// <summary>
    /// 获取运行应用程序的.NET安装的名称
    /// </summary>
    public string FrameworkDescription { get; set; } = RuntimeInformation.FrameworkDescription;

    /// <summary>
    /// 是否为64位进程
    /// </summary>
    public bool Is64BitProcess { get; set; } = Environment.Is64BitProcess;

    /// <summary>
    /// 使用物理内存
    /// </summary>
    public long WorkingSet { get; set; } = Environment.WorkingSet;

    /// <summary>
    /// 所有IP地址
    /// </summary>
    public IEnumerable<IPAddress> AddressList { get; set; }

    /// <summary>
    /// 可联网IP地址
    /// </summary>
    public IPAddress AddressInterNetwork { get; set; }

    /// <summary>
    /// 刷新全部
    /// </summary>
    /// <param name="ignoreSlow">忽略慢的项，如 CPU</param>
    /// <returns></returns>
    public async Task RefreshAll(bool ignoreSlow = false)
    {
        var st = Stopwatch.StartNew();

        OSType = GetOSType();

        LogicalDiskList = GetLogicalDisk();

        AddressList = await GetAddressList();

        AddressInterNetwork = (await GetAddressInterNetwork()).Address;

        if (!ignoreSlow)
        {
            RefreshCPU();
        }

        var separator = "-=-=-";

        try
        {
            if (CmdTo.IsWindows)
            {
                var listCmd = new List<string>
                {
                    //0 系统名称、开机时间、可用物理内存、总内存
                    $"wmic os get Caption,LastBootUpTime,FreePhysicalMemory,TotalVisibleMemorySize /value",

                    //1 CPU name
                    "wmic cpu get Name /value",

                    //2 Model
                    $"wmic csproduct get name /value"
                };
                var cmds = string.Join($" & echo {separator} & ", listCmd);

                var cr = CmdTo.Execute(cmds);
                var outGroup = cr.CrOutput.Split(separator).ToList();

                for (int i = 0; i < outGroup.Count; i++)
                {
                    var items = outGroup[i].Trim().Split('\n');

                    try
                    {
                        switch (i)
                        {
                            case 0:
                                {
                                    foreach (var item in items)
                                    {
                                        if (item.StartsWith("Caption="))
                                        {
                                            var val = item.Split('=')[1].Trim();
                                            OperatingSystem = val;
                                        }
                                        else if (item.StartsWith("LastBootUpTime"))
                                        {
                                            var val = item.Split('=')[1].Split('.')[0];
                                            if (DateTime.TryParseExact(val, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime time))
                                            {
                                                TickCount = Convert.ToInt64((DateTime.Now - time).TotalMilliseconds);
                                            }
                                        }
                                        else if (item.StartsWith("FreePhysicalMemory"))
                                        {
                                            var val = item.Split('=')[1].Trim();
                                            FreePhysicalMemory = 1024 * long.Parse(val);
                                        }
                                        else if (item.StartsWith("TotalVisibleMemorySize"))
                                        {
                                            var val = item.Split('=')[1].Trim();
                                            TotalPhysicalMemory = 1024 * long.Parse(val);
                                        }
                                    }
                                    UsedPhysicalMemory = TotalPhysicalMemory - FreePhysicalMemory;
                                }
                                break;
                            case 1:
                                ProcessorName = items.FirstOrDefault(x => x.StartsWith("Name=")).Split('=').LastOrDefault().Trim();
                                break;
                            case 2:
                                Model = items.FirstOrDefault(x => x.StartsWith("Name=")).Split('=').LastOrDefault().Trim();
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(listCmd[i]);
                        Console.WriteLine(ex);
                    }
                }
            }
            else
            {
                var listCmd = new List<string>
                {
                    //0 os，或 cat /etc/redhat-release 或 cat /etc/lsb-release
                    "cat /etc/os-release",

                    //1 memory
                    "cat /proc/meminfo",

                    //2 uptime
                    "cat /proc/uptime",

                    //3 Model name
                    "cat /sys/class/dmi/id/product_name",

                    //4 CPU name, 使用 cat /proc/cpuinfo 对 ARM 不兼容
                    "lscpu"
                };
                var cmds = string.Join($" && echo {separator} && ", listCmd);
                //取消本地化
                cmds = $"export LC_ALL=C && {cmds} && unset LC_ALL";

                var cr = CmdTo.Execute(cmds);
                var outGroup = cr.CrOutput.Split(separator).ToList();

                for (int i = 0; i < outGroup.Count; i++)
                {
                    var outItem = outGroup[i].Trim();
                    try
                    {
                        switch (i)
                        {
                            case 0:
                                {
                                    var items = outItem.Split('\n');
                                    OperatingSystem = items.FirstOrDefault(x => x.Contains("PRETTY_NAME")).Split('"')[1];
                                }
                                break;
                            case 1:
                                {
                                    var items = outItem.Split('\n');
                                    foreach (var item in items)
                                    {
                                        if (item.StartsWith("MemTotal:"))
                                        {
                                            var val = 1024 * long.Parse(item.ToLower().Replace("kb", "").Split(':')[1].Trim());
                                            TotalPhysicalMemory = val;
                                        }
                                        else if (item.StartsWith("MemAvailable:"))
                                        {
                                            var val = 1024 * long.Parse(item.ToLower().Replace("kb", "").Split(':')[1].Trim());
                                            FreePhysicalMemory = val;
                                        }
                                        else if (item.StartsWith("SwapFree:"))
                                        {
                                            var val = 1024 * long.Parse(item.ToLower().Replace("kb", "").Split(':')[1].Trim());
                                            SwapFree = val;
                                        }
                                        else if (item.StartsWith("SwapTotal:"))
                                        {
                                            var val = 1024 * long.Parse(item.ToLower().Replace("kb", "").Split(':')[1].Trim());
                                            SwapTotal = val;
                                        }
                                    }
                                    UsedPhysicalMemory = TotalPhysicalMemory - FreePhysicalMemory;
                                    SwapUsed = SwapTotal - SwapFree;
                                }
                                break;
                            case 2:
                                TickCount = Convert.ToInt64(Convert.ToDouble(outItem.Split(' ')[0]) * 1000);
                                break;
                            case 3:
                                Model = outItem;
                                break;
                            case 4:
                                {
                                    var items = outItem.Split('\n');
                                    ProcessorName = items.FirstOrDefault(x => x.StartsWith("Model name:")).Substring(11).Trim();
                                }
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(listCmd[i]);
                        Console.WriteLine(ex);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    /// <summary>
    /// 刷新 CPU
    /// </summary>
    public void RefreshCPU()
    {
        try
        {
            if (CmdTo.IsWindows)
            {
                // https://stackoverflow.com/questions/9097067

                var cmd = "PowerShell \"Get-WmiObject Win32_PerfFormattedData_PerfOS_Processor | Select Name, PercentProcessorTime\"";
                var cr = CmdTo.Execute(cmd).CrOutput.Trim();

                ProcessorUsage = Convert.ToInt32(cr.Split(' ').Last().Trim());
            }
            else
            {
                // https://stackoverflow.com/questions/23367857

                // 打开 /proc/stat 文件
                using var stream0 = new StreamReader("/proc/stat");

                // 读取第一行，即总的 CPU 使用情况
                var parts = stream0.ReadLine().Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).Skip(1).ToArray();

                //idle + iowait
                var idle0 = long.Parse(parts[3]) + long.Parse(parts[4]);
                //user + nice + system + irq + softirq + steal
                var nonIdle0 = long.Parse(parts[0]) + long.Parse(parts[1]) + long.Parse(parts[2]) + long.Parse(parts[5]) + long.Parse(parts[6]) + long.Parse(parts[7]);
                //Idle + NonIdle
                var total0 = idle0 + nonIdle0;

                // 等待一段时间
                Thread.Sleep(100);

                // 重新读取文件，并计算 CPU 使用率的变化
                using var stream1 = new StreamReader("/proc/stat");
                parts = stream1.ReadLine().Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).Skip(1).ToArray();

                //idle + iowait
                var idle1 = long.Parse(parts[3]) + long.Parse(parts[4]);
                //user + nice + system + irq + softirq + steal
                var nonIdle1 = long.Parse(parts[0]) + long.Parse(parts[1]) + long.Parse(parts[2]) + long.Parse(parts[5]) + long.Parse(parts[6]) + long.Parse(parts[7]);
                //Idle + NonIdle
                var total1 = idle1 + nonIdle1;

                // 计算 CPU 使用率
                var total = total1 - total0;
                var idle = idle1 - idle0;

                ProcessorUsage = (int)Math.Ceiling((total - idle) * 1m / total * 100);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to get CPU usage\r\n{ex.Message}");
        }
    }

    /// <summary>
    /// 获取OS名称
    /// </summary>
    /// <returns></returns>
    public static string GetOSType()
    {
        string osp = string.Empty;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            osp = "Windows";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            osp = "Linux";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            osp = "OSX";
        }

        return osp;
    }

    /// <summary>
    /// 获取磁盘信息
    /// </summary>
    /// <param name="filterMoreThan">过滤空间大于多少的磁盘，默认 1G</param>
    /// <returns></returns>
    public static List<LogicalDiskModel> GetLogicalDisk(int filterMoreThan = 1024 * 1024 * 1024)
    {
        var result = new List<LogicalDiskModel>();

        var dictMount = new Dictionary<string, string>();
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            try
            {
                var lines = File.ReadAllLines("/proc/mounts");
                foreach (var line in lines)
                {
                    if (line.StartsWith("/dev"))
                    {
                        var cols = line.Split(' ');
                        dictMount.Add(cols[1], cols[0]);
                    }
                }
            }
            catch (Exception) { }
        }

        var allDrives = DriveInfo.GetDrives();
        foreach (var di in allDrives)
        {
            if (di.IsReady && di.TotalSize >= filterMoreThan)
            {
                var model = new LogicalDiskModel
                {
                    Directory = di.RootDirectory.FullName,
                    VolumeLabel = di.VolumeLabel,
                    TotalSize = di.TotalSize,
                    TotalFreeSpace = di.TotalFreeSpace,
                    TotalUsedSpace = di.TotalSize - di.AvailableFreeSpace
                };
                if (dictMount.Count > 0)
                {
                    if (!result.Any(x => x.Directory == model.Directory) && dictMount.TryGetValue(model.Directory, out string volumeLabel))
                    {
                        model.VolumeLabel = volumeLabel;
                        result.Add(model);
                    }
                }
                else
                {
                    result.Add(model);
                }
            }
        }

        return result;
    }

    /// <summary>
    /// 获取地址列表
    /// </summary>
    /// <returns></returns>
    public static async Task<IEnumerable<IPAddress>> GetAddressList()
    {
        var host = await Dns.GetHostEntryAsync(Dns.GetHostName()).ConfigureAwait(false);
        return host.AddressList.Where(x => x.AddressFamily == AddressFamily.InterNetwork || x.AddressFamily == AddressFamily.InterNetworkV6);
    }

    /// <summary>
    /// 获取联网地址
    /// </summary>
    /// <returns></returns>
    public static async Task<IPEndPoint> GetAddressInterNetwork()
    {
        using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0);
        await socket.ConnectAsync("114.114.114.114", 65530).ConfigureAwait(false);
        var endPoint = socket.LocalEndPoint as IPEndPoint;
        return endPoint;
    }

    /// <summary>
    /// 可视化输出
    /// </summary>
    /// <returns></returns>
    public string ToView()
    {
        var dic = new Dictionary<int, string>
        {
            { 0, "" },
            { 1, $" Framework: {FrameworkDescription}" },
            { 2, $" Memory: {ParsingTo.FormatByteSize(size:WorkingSet)}" },
            { 3, $" OSName: {OperatingSystem} , {OSArchitecture}" },
            { 4, $" OSVersion: {OSVersion}" },
            { 5, $" User: {UserName}" },
            { 6, $" Uptime: {Math.Round(TickCount*1.0/1000/24/3600,2)} Days" },
            { 7, $" CPU: {ProcessorName} , {ProcessorCount} Core{ProgressBar(Convert.ToInt64(ProcessorUsage*100), 10000, false)}" },
            { 8, $" RAM: {ProgressBar(UsedPhysicalMemory, TotalPhysicalMemory)}" }
        };
        if (SwapTotal > 0)
        {
            dic.Add(9, $" Swap: {ProgressBar(SwapUsed, SwapTotal)}");
        }

        var listDisk = LogicalDiskList.Select(item => ProgressBar(item.TotalUsedSpace, item.TotalSize, true, item.Directory));
        dic.Add(10, $" Disk: {string.Join("", listDisk)}");

        //排序
        var list = dic.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value).Values.ToList();

        return string.Join("\r\n\r\n", list);
    }

    /// <summary>
    /// 进度条
    /// </summary>
    /// <param name="m">分子</param>
    /// <param name="d">分母</param>
    /// <param name="isc">是否转换</param>
    /// <param name="desc">说明</param>
    /// <returns></returns>
    private static string ProgressBar(long m, long d, bool isc = true, string desc = "")
    {
        var vt = 24;
        var v1 = m * 1.0;
        var v2 = d * 1.0;
        var v3 = Math.Round(vt * (v1 / v2));
        var unit = string.Empty;
        if (isc)
        {
            v1 = Math.Round(m * 1.0 / 1024 / 1024 / 1024, 2);
            v2 = Math.Round(d * 1.0 / 1024 / 1024 / 1024, 2);
            unit = $" {v1}/{v2}GiB ";
        }

        if (!string.IsNullOrWhiteSpace(desc) && desc.Length > 20)
        {
            desc = desc.Substring(0, 20) + "...";
        }

        var listpb = new List<string>()
        {
            "\r\n "
        };
        while (--v3 > 0)
        {
            listpb.Add("#");
        }
        while (listpb.Count < vt)
        {
            listpb.Add(".");
        }
        var text = unit + desc;
        if (!string.IsNullOrEmpty(text))
        {
            text = $"\r\n{text}";
        }
        var per = " " + (v2 == 0 ? 0 : Math.Round((v1 / v2) * 100, 0)) + "%";
        text = per + text;
        listpb.Add(text);

        return string.Join("", listpb);
    }

    /// <summary>
    /// 获取网卡接收、发送字节
    /// </summary>
    /// <returns></returns>
    public static List<NetworkInterfaceModel> GetNetworkInterfaceStats()
    {
        var nis = NetworkInterface.GetAllNetworkInterfaces();

        var list = new List<NetworkInterfaceModel>();
        foreach (var ni in nis)
        {
            var stats = ni.GetIPStatistics();
            list.Add(new NetworkInterfaceModel
            {
                Id = ni.Id,
                Ni = ni,
                BytesReceived = stats.BytesReceived,
                BytesSent = stats.BytesSent
            });
        }
        return list;
    }

    /// <summary>
    /// 计算速度
    /// </summary>
    /// <param name="lastStats"></param>
    /// <param name="elapsed">过去，间隔时间，单位毫秒，默认 1000</param>
    public static List<NetworkInterfaceModel> GetNetworkInterfaceCalc(ref List<NetworkInterfaceModel> lastStats, int elapsed = 1000)
    {
        var list = new List<NetworkInterfaceModel>();
        Thread.Sleep(elapsed);
        var currStats = GetNetworkInterfaceStats();

        foreach (var lastItem in lastStats)
        {
            var currItem = currStats.First(x => x.Id == lastItem.Id);

            var diffReceived = currItem.BytesReceived - lastItem.BytesReceived;
            var diffSent = currItem.BytesSent - lastItem.BytesSent;

            list.Add(new NetworkInterfaceModel
            {
                Id = lastItem.Id,
                Ni = lastItem.Ni,
                BytesReceived = diffReceived,
                BytesSent = diffSent
            });
        }

        lastStats = currStats;
        return list;
    }

    /// <summary>
    /// 网卡
    /// </summary>
    public class NetworkInterfaceModel
    {
        /// <summary>
        /// 标识
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 网卡
        /// </summary>
        public NetworkInterface Ni { get; set; }

        /// <summary>
        /// 接收 B
        /// </summary>
        public long BytesReceived { get; set; }

        /// <summary>
        /// 发送 B
        /// </summary>
        public long BytesSent { get; set; }
    }

    /// <summary>
    /// 逻辑磁盘
    /// </summary>
    public class LogicalDiskModel
    {
        /// <summary>
        /// 路径
        /// </summary>
        public string Directory { get; set; }
        /// <summary>
        /// 盘符
        /// </summary>
        public string VolumeLabel { get; set; }
        /// <summary>
        /// 总大小 B
        /// </summary>
        public long TotalSize { get; set; }
        /// <summary>
        /// 可用大小 B
        /// </summary>
        public long TotalFreeSpace { get; set; }
        /// <summary>
        /// 已用大小 B
        /// </summary>
        public long TotalUsedSpace { get; set; }
    }
}