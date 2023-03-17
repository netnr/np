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
    /// 时间（UTC）
    /// </summary>
    public DateTime UtcNow { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 操作系统
    /// </summary>
    public string OperatingSystem { get; set; }

    /// <summary>
    /// 型号
    /// </summary>
    public string Model { get; set; }

    /// <summary>
    /// 确定当前操作系统是否为64位操作系统
    /// </summary>
    public bool Is64BitOperatingSystem { get; set; } = Environment.Is64BitOperatingSystem;

    /// <summary>
    /// 是否为64位进程
    /// </summary>
    public bool Is64BitProcess { get; set; } = Environment.Is64BitProcess;

    /// <summary>
    /// 系统目录
    /// </summary>
    public string SystemDirectory { get; set; } = Environment.SystemDirectory;

    /// <summary>
    /// NetBIOS名称
    /// </summary>
    public string MachineName { get; set; } = Environment.MachineName;

    /// <summary>
    /// 代表操作系统平台
    /// </summary>
    public string OSName { get; set; }

    /// <summary>
    /// 系统平台
    /// </summary>
    public PlatformID OSVersionPlatform { get; set; } = Environment.OSVersion.Platform;

    /// <summary>
    /// 系统版本
    /// </summary>
    public Version OSVersion { get; set; } = Environment.OSVersion.Version;

    /// <summary>
    /// 系统版本字符串
    /// </summary>
    public string OSVersionString { get; set; } = Environment.OSVersion.VersionString;

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
    /// 获取公共语言运行时的主要，次要，内部和修订版本号
    /// </summary>
    public Version Version { get; set; } = Environment.Version;

    /// <summary>
    /// 获取运行应用程序的.NET安装的名称
    /// </summary>
    public string FrameworkDescription { get; set; } = RuntimeInformation.FrameworkDescription;

    /// <summary>
    /// 使用物理内存
    /// </summary>
    public long WorkingSet { get; set; } = Environment.WorkingSet;

    /// <summary>
    /// 获取当前计算机上的处理器数量
    /// </summary>
    public int ProcessorCount { get; set; } = Environment.ProcessorCount;

    /// <summary>
    /// 处理器名称
    /// </summary>
    public string ProcessorName { get; set; }

    /// <summary>
    /// 处理器使用率
    /// </summary>
    public decimal ProcessorUsed { get; set; } = 0;

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
    /// 物理内存明细
    /// </summary>
    public List<Dictionary<string, object>> PhysicalMemoryList { get; set; } = new List<Dictionary<string, object>>();

    /// <summary>
    /// 逻辑磁盘 B
    /// </summary>
    public List<Dictionary<string, object>> LogicalDiskList { get; set; } = new List<Dictionary<string, object>>();

    /// <summary>
    /// 所有IP地址
    /// </summary>
    public IEnumerable<IPAddress> AddressList { get; set; }

    /// <summary>
    /// 可联网IP地址
    /// </summary>
    public IPAddress AddressInterNetwork { get; set; }

    private bool IsInit = false;
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="force">强制刷新</param>
    /// <returns></returns>
    public async Task<bool> Init(bool force = false)
    {
        if (force || IsInit == false)
        {
            IsInit = true;

            OSName = GetOSName();

            AddressList = await GetAddressList();
            AddressInterNetwork = (await GetAddressInterNetwork()).Address;

            var separator = "-=-=-";

            if (GlobalTo.IsWindows)
            {
                LogicalDiskList = GetLogicalDisk();

                var listCmd = new List<string>
                {
                    //0 系统名称、开机时间、可用物理内存、总内存
                    $"wmic os get Caption,LastBootUpTime,FreePhysicalMemory,TotalVisibleMemorySize /value",

                    //1 CPU name
                    "wmic cpu get Name /value",

                    //2 CPU used
                    // https://www.delftstack.com/howto/powershell/find-the-cpu-and-ram-usage-using-powershell/
                    //"PowerShell \"Get-Counter '\\Processor(_Total)\\% Processor Time'\"",
                    "PowerShell \"Get-WmiObject -Class Win32_Processor | Select LoadPercentage\"",

                    //3 Memory info
                    "wmic MemoryChip get Capacity,Speed",

                    //4 Model
                    $"wmic csproduct get name /value"
                };
                var cmds = string.Join($" & echo {separator} & ", listCmd);

                try
                {
                    var cr = await Task.Run(() => CmdTo.Execute(cmds));
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
                                    ProcessorUsed = Math.Ceiling(Convert.ToDecimal(items.LastOrDefault().ToString().Trim()));
                                    break;
                                case 3:
                                    {
                                        PhysicalMemoryList.Clear();

                                        foreach (var item in items)
                                        {
                                            if (!item.Contains("Capacity") && !string.IsNullOrWhiteSpace(item))
                                            {
                                                var cols = item.Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
                                                var dicItem = new Dictionary<string, object>
                                                {
                                                    { "Capacity", Convert.ToInt64(cols[0]) },
                                                    { "Speed", cols.Count > 1 ? Convert.ToInt64(cols[1]) : null },
                                                };
                                                PhysicalMemoryList.Add(dicItem);
                                            }
                                        }
                                    }
                                    break;
                                case 4:
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
                catch (Exception ex)
                {
                    Console.WriteLine(cmds);
                    Console.WriteLine(ex);
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(SystemDirectory))
                {
                    SystemDirectory = "/boot";
                }

                var listCmd = new List<string>
                {
                    //0 logical disk
                    $"df",

                    //1 os，或 cat /etc/redhat-release 或 cat /etc/lsb-release
                    "cat /etc/os-release",

                    //2 memory
                    "cat /proc/meminfo",

                    //3 uptime
                    "cat /proc/uptime",

                    //4 Model name
                    "cat /sys/class/dmi/id/product_name",

                    //5 CPU name, 使用 cat /proc/cpuinfo 对 ARM 不兼容
                    "lscpu",
                    //6 CPU used
                    "vmstat 1 2",

                    //7 Memory Info
                    "lshw -short -C memory"
                };
                var cmds = string.Join($" && echo {separator} && ", listCmd);
                //取消本地化
                cmds = $"export LC_ALL=C && {cmds} && unset LC_ALL";

                try
                {
                    var cr = await Task.Run(() => CmdTo.Execute(cmds));
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
                                        LogicalDiskList.Clear();

                                        var items = outItem.Split('\n');
                                        foreach (var item in items)
                                        {
                                            if (item.StartsWith("/dev/"))
                                            {
                                                var dis = item.Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
                                                var size = long.Parse(dis[1]) * 1024;
                                                //大于 1GB
                                                if (size > 1073741824)
                                                {
                                                    LogicalDiskList.Add(new Dictionary<string, object>
                                                    {
                                                        { "Directory", dis.Last() },
                                                        { "VolumeLabel", dis.First() },
                                                        { "TotalSize", size },
                                                        { "TotalFreeSpace", long.Parse(dis[3]) * 1024 },
                                                        { "TotalUsedSpace", size - long.Parse(dis[3]) * 1024 }
                                                    });
                                                }
                                            }
                                        }
                                    }
                                    break;
                                case 1:
                                    {
                                        var items = outItem.Split('\n');
                                        OperatingSystem = items.FirstOrDefault(x => x.Contains("PRETTY_NAME")).Split('"')[1];
                                    }
                                    break;
                                case 2:
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
                                case 3:
                                    TickCount = Convert.ToInt64(Convert.ToDouble(outItem.Split(' ')[0]) * 1000);
                                    break;
                                case 4:
                                    Model = outItem;
                                    break;
                                case 5:
                                    {
                                        var items = outItem.Split('\n');
                                        ProcessorName = items.FirstOrDefault(x => x.StartsWith("Model name:")).Substring(11).Trim();
                                    }
                                    break;
                                case 6:
                                    {
                                        var cpuitems = outItem.Split('\n').LastOrDefault().Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
                                        var usi = cpuitems.Count - 5;
                                        ProcessorUsed = decimal.Parse(cpuitems[usi]);
                                    }
                                    break;
                                case 7:
                                    {
                                        PhysicalMemoryList.Clear();

                                        var items = outItem.Split('\n');
                                        foreach (var item in items)
                                        {
                                            if (item.Contains("iB") && !item.Contains("System Memory") && !item.Contains("KiB"))
                                            {
                                                var dicItem = new Dictionary<string, object>
                                                {
                                                    { "Capacity", item.Split(' ').FirstOrDefault(x=>x.EndsWith("iB")) }
                                                };
                                                PhysicalMemoryList.Add(dicItem);
                                            }
                                        }
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
                catch (Exception ex)
                {
                    Console.WriteLine(cmds);
                    Console.WriteLine(ex);
                }
            }
        }

        return IsInit;
    }

    /// <summary>
    /// 可视化输出
    /// </summary>
    /// <returns></returns>
    public async Task<string> ToView()
    {
        await Init();

        var dic = new Dictionary<int, string>
        {
            { 0, "" },
            { 1, $" Framework: {FrameworkDescription}" },
            { 2, $" Memory: {ParsingTo.FormatByteSize(size:WorkingSet)}" },
            { 3, $" System: {(string.IsNullOrWhiteSpace(OperatingSystem) ? OSName : OperatingSystem)}{(Is64BitOperatingSystem ? " , 64Bit" : "")}" },
            { 4, $" OSVersion: {OSVersionString}" },
            { 5, $" User: {UserName}" },
            { 6, $" Uptime: {Math.Round(TickCount*1.0/1000/24/3600,2)} Days" },
            { 7, $" CPU: {ProcessorName} , {ProcessorCount} Core{ProgressBar(Convert.ToInt64(ProcessorUsed*100), 10000, false)}" },
            { 8, $" RAM: {ProgressBar(UsedPhysicalMemory, TotalPhysicalMemory)}" }
        };
        if (SwapTotal > 0)
        {
            dic.Add(9, $" Swap: {ProgressBar(SwapUsed, SwapTotal)}");
        }

        var listlgd = new List<string>();
        LogicalDiskList.ForEach(item =>
        {
            var totalSize = (long)item["TotalSize"];
            var totalUsedSpace = (long)item["TotalUsedSpace"];
            var dir = item["Directory"].ToString();
            listlgd.Add(ProgressBar(totalUsedSpace, totalSize, true, dir));
        });
        dic.Add(10, $" Disk: {string.Join("", listlgd)}");

        //排序
        var list = dic.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value).Values.ToList();

        return string.Join("\r\n\r\n", list);
    }

    /// <summary>
    /// 获取OS名称
    /// </summary>
    /// <returns></returns>
    public static string GetOSName()
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
    /// 获取磁盘信息（Linux建议使用df命令）
    /// </summary>
    /// <returns></returns>
    public static List<Dictionary<string, object>> GetLogicalDisk()
    {
        var list = new List<Dictionary<string, object>>();

        var allDrives = DriveInfo.GetDrives();
        foreach (var di in allDrives)
        {
            if (di.IsReady)
            {
                var dicItem = new Dictionary<string, object>
                {
                    { "Directory", di.RootDirectory.FullName },
                    { "VolumeLabel", di.VolumeLabel },
                    { "TotalSize", di.TotalSize },
                    { "TotalFreeSpace", di.TotalFreeSpace },
                    { "TotalUsedSpace", di.TotalSize - di.TotalFreeSpace }
                };
                list.Add(dicItem);
            }
        }

        return list;
    }

    /// <summary>
    /// 获取地址列表
    /// </summary>
    /// <returns></returns>
    public static async Task<IEnumerable<IPAddress>> GetAddressList()
    {
        var host = await Dns.GetHostEntryAsync(Dns.GetHostName());
        return host.AddressList.Where(x => x.AddressFamily == AddressFamily.InterNetwork || x.AddressFamily == AddressFamily.InterNetworkV6);
    }

    /// <summary>
    /// 获取联网地址
    /// </summary>
    /// <returns></returns>
    public static async Task<IPEndPoint> GetAddressInterNetwork()
    {
        using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0);
        await socket.ConnectAsync("114.114.114.114", 65530);
        var endPoint = socket.LocalEndPoint as IPEndPoint;
        return endPoint;
    }

    /// <summary>
    /// 清空当前行
    /// </summary>
    public static void ClearCurrentConsoleLine()
    {
        int currentLineCursor = Console.CursorTop;
        Console.SetCursorPosition(0, Console.CursorTop);
        Console.Write(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(0, currentLineCursor);
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
            text = $"\n{text}";
        }
        var per = " " + (v2 == 0 ? 0 : Math.Round((v1 / v2) * 100, 0)) + "%";
        text = per + text;
        listpb.Add(text);

        return string.Join("", listpb);
    }

    /// <summary>
    /// 监听作业
    /// </summary>
    public class MonitorWork
    {
        /// <summary>
        /// 跳计数
        /// </summary>
        private int HopCount { get; set; } = 0;

        /// <summary>
        /// 作业标识
        /// </summary>
        private bool IsWorking { get; set; } = false;

        /// <summary>
        /// 启动
        /// </summary>
        /// <param name="callback">计时 elapsed、总接收 receivedSpeed、总发送 sentSpeed</param>
        /// <param name="callbackInterval">回调间隔，单位：每100毫秒，如传 10 表示 10*100=1000毫秒 调一次</param>
        /// <returns></returns>
        public void Start(Action<int, double, double> callback, int callbackInterval = 10)
        {
            IsWorking = true;

            var nis = NetworkInterface.GetAllNetworkInterfaces();

            //last bytes
            var brLast = new Dictionary<NetworkInterface, double>();
            var bsLast = new Dictionary<NetworkInterface, double>();
            foreach (var ni in nis)
            {
                var stats = ni.GetIPStatistics();

                var br = stats.BytesReceived;
                var bs = stats.BytesSent;

                brLast.Add(ni, br);
                bsLast.Add(ni, bs);
            }
            var brRead = new Dictionary<NetworkInterface, IEnumerable<double>>();
            var bsRead = new Dictionary<NetworkInterface, IEnumerable<double>>();

            while (IsWorking)
            {
                HopCount++;
                var hopInterval = 100;
                Thread.Sleep(hopInterval);

                foreach (var ni in nis)
                {
                    var stats = ni.GetIPStatistics();

                    var br = stats.BytesReceived;
                    var bs = stats.BytesSent;

                    //(current-last)/elapsed
                    var brLocal = (br - brLast[ni]) / 0.102;
                    var bsLocal = (bs - bsLast[ni]) / 0.102;

                    //last=current
                    brLast[ni] = br;
                    bsLast[ni] = bs;

                    // Keep last 20
                    var brReads = brRead.ContainsKey(ni) ? brRead[ni] : Enumerable.Empty<double>();
                    brReads = new[] { brLocal }.Concat(brReads).Take(20);
                    brRead[ni] = brReads;

                    var bsReads = bsRead.ContainsKey(ni) ? bsRead[ni] : Enumerable.Empty<double>();
                    bsReads = new[] { bsLocal }.Concat(bsReads).Take(20);
                    bsRead[ni] = bsReads;
                }

                //total
                if (HopCount % callbackInterval == 0)
                {
                    var outSpeedBr = 0.0; //总接收
                    var outSpeedBs = 0.0; //总发送
                    var outSpeedDicBr = new Dictionary<NetworkInterface, double>(); //接收明细
                    var outSpeedDicBs = new Dictionary<NetworkInterface, double>(); //发送明细

                    foreach (var key in brRead.Keys)
                    {
                        var reads = brRead[key];
                        var speed = reads.Sum() / reads.Count();
                        outSpeedBr += speed;
                        outSpeedDicBr[key] = speed;
                    }

                    foreach (var key in bsRead.Keys)
                    {
                        var reads = bsRead[key];
                        var speed = reads.Sum() / reads.Count();
                        outSpeedBs += speed;
                        outSpeedDicBs[key] = speed;
                    }

                    //callback
                    callback.Invoke(HopCount * hopInterval, outSpeedBr, outSpeedBs);
                }
            }
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            IsWorking = false;
        }
    }
}