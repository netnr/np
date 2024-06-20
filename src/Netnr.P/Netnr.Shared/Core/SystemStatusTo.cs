#if Full || Core

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
public partial class SystemStatusTo
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
    public int ProcessorUsage { get; set; } = 0;

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
    public List<LogicalDiskModel> LogicalDiskList { get; set; } = [];

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
    public string AddressInterNetwork { get; set; }

    /// <summary>
    /// 更多
    /// </summary>
    public Dictionary<string, string> More { get; set; } = [];

    /// <summary>
    /// 刷新全部
    /// </summary>
    /// <param name="ignoreSlow">忽略慢的项，如 CPU</param>
    /// <returns></returns>
    public async Task RefreshAll(bool ignoreSlow = false)
    {
        OSType = GetOSType();

        LogicalDiskList = GetLogicalDisk();

        AddressList = await GetAddressList();

        AddressInterNetwork = (await GetAddressInterNetwork()).Address.ToString();

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
                    $"wmic csproduct get name /value",
                    "wmic baseboard get Manufacturer,Product /value" //3 型号为空则获取主板型号
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
                                if (Model == "System Product Name")
                                {
                                    var baseboard = outGroup[3].Trim().Split('\n').Where(x => x.Contains('=')).Select(x => x.Split('=').Last().Trim());
                                    Model = string.Join(" ", baseboard);
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
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                var listCmd = new List<string>
                {
                    //0 os
                    "sw_vers",

                    //1 memory total
                    "sysctl -n hw.memsize",
                    //2 memory free
                    "getconf PAGE_SIZE && vm_stat | grep 'Pages free'",
                    //3 swap
                    "sysctl -n vm.swapusage",

                    //4 uptime
                    "sysctl -n kern.boottime",

                    //5 Model name
                    "sysctl -n hw.model",

                    //6 CPU name
                    "sysctl -n machdep.cpu.brand_string"
                };
                var cmds = string.Join($" && echo {separator} && ", listCmd);

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
                                    OperatingSystem = items.FirstOrDefault(x => x.Contains("ProductName")).Split(':')[1].Trim();
                                }
                                break;
                            case 1:
                                {
                                    TotalPhysicalMemory = Convert.ToInt64(outItem.Trim());
                                }
                                break;
                            case 2:
                                {
                                    var items = outItem.Split('\n');
                                    var pageSize = Convert.ToInt64(items[0].Trim());
                                    var freePages = Convert.ToInt64(items[1].Split(':')[1].Trim().TrimEnd('.'));
                                    FreePhysicalMemory = pageSize * freePages;

                                    UsedPhysicalMemory = TotalPhysicalMemory - FreePhysicalMemory;
                                }
                                break;
                            case 3:
                                {
                                    var items = outItem.Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

                                    var valIndex = items.FindIndex(x => x.Contains("total")) + 2;
                                    if (valIndex < items.Count)
                                    {
                                        var itemTotal = items[valIndex].Trim();
                                        var itemUnit = itemTotal.Last();
                                        var n = itemTotal.Last() == 'G' ? 1024 * 1024 * 1024 : 1024 * 1024;
                                        SwapTotal = Convert.ToInt64(Convert.ToDouble(itemTotal.TrimEnd('M').TrimEnd('G')) * n);
                                    }

                                    valIndex = items.FindIndex(x => x.Contains("used")) + 2;
                                    if (valIndex < items.Count)
                                    {
                                        var itemTotal = items[valIndex].Trim();
                                        var itemUnit = itemTotal.Last();
                                        var n = itemTotal.Last() == 'G' ? 1024 * 1024 * 1024 : 1024 * 1024;
                                        SwapUsed = Convert.ToInt64(Convert.ToDouble(itemTotal.TrimEnd('M').TrimEnd('G')) * n);
                                    }

                                    SwapFree = SwapTotal - SwapUsed;
                                }
                                break;
                            case 4:
                                {
                                    var items = outItem.Split('}');
                                    DateTime result = DateTime.ParseExact(items.Last().Trim(), "ddd MMM dd HH:mm:ss yyyy", CultureInfo.InvariantCulture);
                                    TickCount = Convert.ToInt64((DateTime.Now - result).TotalMilliseconds);
                                }
                                break;
                            case 5:
                                Model = outItem.Trim();
                                break;
                            case 6:
                                ProcessorName = outItem.Trim();
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
                var osFile = new FileInfo("/etc/os-release");
                if (osFile.Exists)
                {
                    var osText = File.ReadAllText(osFile.FullName);
                    OperatingSystem = osText.Split('\n').FirstOrDefault(x => x.Contains("PRETTY_NAME")).Split('"')[1];
                }

                var meminfoFile = new FileInfo("/proc/meminfo");
                if (meminfoFile.Exists)
                {
                    var meminfoText = File.ReadAllText(meminfoFile.FullName);
                    var items = meminfoText.Split('\n');
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

                var uptimeFile = new FileInfo("/proc/uptime");
                if (uptimeFile.Exists)
                {
                    var uptimeText = File.ReadAllText(uptimeFile.FullName);
                    TickCount = Convert.ToInt64(Convert.ToDouble(uptimeText.Split(' ')[0]) * 1000);
                }

                var productNameFile = new FileInfo("/sys/class/dmi/id/product_name");
                if (productNameFile.Exists)
                {
                    var productNameText = File.ReadAllText(productNameFile.FullName);
                    Model = productNameText.Trim();
                }

                var cpuinfoFile = new FileInfo("/proc/cpuinfo");
                if (cpuinfoFile.Exists)
                {
                    var cpuinfoText = File.ReadAllText(cpuinfoFile.FullName);
                    ProcessorName = cpuinfoText.Split('\n').FirstOrDefault(x => x.StartsWith("model name", StringComparison.OrdinalIgnoreCase))?.Split(':').Last().Trim();
                }
                // ARM 不兼容的用 lscpu
                if (string.IsNullOrWhiteSpace(ProcessorName))
                {
                    var cmds = $"export LC_ALL=C && lscpu && unset LC_ALL";
                    var cr = CmdTo.Execute(cmds);

                    var items = cr.CrOutput.Split('\n');
                    ProcessorName = items.FirstOrDefault(x => x.StartsWith("Model name:", StringComparison.OrdinalIgnoreCase)).Split(':').Last().Trim();
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
                //【调用 kernel32.dll 方式】
                ProcessorUsage = WindowsLibrary.GetCPUUsage();

                //【PerformanceCounter 性能计数器方式】
                //using var cpuCounter = new PerformanceCounter("Processor Information", "% Processor Utility", "_Total");
                //cpuCounter.NextValue(); // 第一次调用 NextValue() 会返回 0，因此需要先调用一次
                //Thread.Sleep(200);
                //ProcessorUsage = (int)Math.Ceiling(cpuCounter.NextValue());

                //【CMD 方式】https://stackoverflow.com/questions/9097067
                //var cmd = "PowerShell \"Get-WmiObject Win32_PerfFormattedData_PerfOS_Processor | Select Name, PercentProcessorTime\"";
                //var cr = CmdTo.Execute(cmd).CrOutput.Trim();
                //ProcessorUsage = Convert.ToInt32(cr.Split(' ').Last().Trim());
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                var cmd = "top -l 1 | awk '/CPU usage:/ {print $3}'";
                var cr = CmdTo.Execute(cmd).CrOutput.Trim().Trim('%');
                ProcessorUsage = (int)Math.Ceiling(Convert.ToDouble(cr));
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
    /// Windows
    /// </summary>
    class WindowsLibrary
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetSystemTimes(out long lpIdleTime, out long lpKernelTime, out long lpUserTime);

        /// <summary>
        /// CPU 使用率
        /// </summary>
        /// <param name="intervalMilliseconds"></param>
        /// <returns></returns>
        public static int GetCPUUsage(int intervalMilliseconds = 300)
        {
            GetSystemTimes(out var idleTime1, out var kernelTime1, out var userTime1);
            Thread.Sleep(intervalMilliseconds);
            GetSystemTimes(out var idleTime2, out var kernelTime2, out var userTime2);

            long idleTimeDiff = idleTime2 - idleTime1;
            long kernelTimeDiff = kernelTime2 - kernelTime1;
            long userTimeDiff = userTime2 - userTime1;

            long totalTime = kernelTimeDiff + userTimeDiff;
            var cpuUsage = (int)Math.Ceiling((totalTime - idleTimeDiff) * 100f / totalTime);

            return cpuUsage;
        }

        /// <summary>
        /// 系统启动以来经过的毫秒数
        /// </summary>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        public static extern ulong GetTickCount64();
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
    /// <param name="filterLessThan">过滤空间小于多少的磁盘，默认 1G</param>
    /// <returns></returns>
    public static List<LogicalDiskModel> GetLogicalDisk(int filterLessThan = 1024 * 1024 * 1024)
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
                        //路径：文件系统
                        dictMount.Add(cols[1], cols[0]);
                    }
                }
            }
            catch (Exception) { }
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            var cmd = "mount";
            var lines = CmdTo.Execute(cmd).CrOutput.Split('\n');
            foreach (var line in lines)
            {
                if (line.StartsWith("/dev"))
                {
                    var cols = line.Split(" on ");
                    //路径：文件系统
                    dictMount.Add(cols[1].Split(' ')[0], cols[0]);
                }
            }
        }

        var allDrives = DriveInfo.GetDrives();
        foreach (var di in allDrives)
        {
            if (di.IsReady && di.TotalSize >= filterLessThan && di.DriveType != DriveType.CDRom)
            {
                var model = new LogicalDiskModel
                {
                    Directory = di.RootDirectory.FullName,
                    VolumeLabel = di.VolumeLabel,
                    TotalSize = di.TotalSize,
                    TotalFreeSpace = di.TotalFreeSpace,
                    TotalUsedSpace = di.TotalSize - di.AvailableFreeSpace,

                    DriveFormat = di.DriveFormat,
                    DriveType = di.DriveType.ToString(),
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

        // mac 尝试按 /dev/diskNs + 大小进行分组去重取路径最短的一条
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) && !result.Any(x => !x.VolumeLabel.StartsWith("/dev/disk")))
        {
            try
            {
                result = result.GroupBy(x => new
                {
                    x.TotalSize,
                    Label = x.VolumeLabel.Substring(0, 11)
                }).Select(x => x.OrderBy(y => y.Directory.Length).First()).ToList();
            }
            catch (Exception) { }
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
            { 2, $" Memory: {ParsingTo.FormatByte(size:WorkingSet)}" },
            { 3, $" OSName: {OperatingSystem} , {OSArchitecture}" },
            { 4, $" OSVersion: {OSVersion}" },
            { 5, $" User: {UserName}" },
            { 6, $" Uptime: {TimeSpan.FromMilliseconds(TickCount):d' Days 'hh':'mm':'ss}" },
            { 7, $" CPU: {ProcessorName} , {ProcessorCount} Core , {ProcessArchitecture}{ProgressBar(Convert.ToInt64(ProcessorUsage*100), 10000, false)}" },
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
            desc = desc.Substring(0, 20) + " ...";
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

        /// <summary>
        /// 格式
        /// </summary>
        public string DriveFormat { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string DriveType { get; set; }
    }
}

#endif