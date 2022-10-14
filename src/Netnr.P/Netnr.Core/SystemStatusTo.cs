using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

namespace Netnr;

/// <summary>
/// 系统状态
/// </summary>
public class SystemStatusTo
{
    /// <summary>
    /// 确定当前操作系统是否为64位操作系统
    /// </summary>
    public bool Is64BitOperatingSystem { get; set; } = Environment.Is64BitOperatingSystem;
    /// <summary>
    /// 获取此本地计算机的NetBIOS名称
    /// </summary>
    public string MachineName { get; set; } = Environment.MachineName;
    /// <summary>
    /// 获取当前平台标识符和版本号
    /// </summary>
    public OperatingSystem OSVersion { get; set; } = Environment.OSVersion;
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
    /// 获取系统目录的标准路径
    /// </summary>
    public string SystemDirectory { get; set; } = Environment.SystemDirectory;
    /// <summary>
    /// 获取操作系统的内存页面中的字节数
    /// </summary>
    public int SystemPageSize { get; set; } = Environment.SystemPageSize;
    /// <summary>
    /// 获取自系统启动以来经过的毫秒数
    /// </summary>
    public long TickCount { get; set; } = Environment.TickCount;
    /// <summary>
    /// 获取与当前用户关联的网络域名
    /// </summary>
    public string UserDomainName { get; set; } = Environment.UserDomainName;
    /// <summary>
    /// 获取当前登录到操作系统的用户的用户名
    /// </summary>
    public string UserName { get; set; } = Environment.UserName;
    /// <summary>
    /// 获取公共语言运行时的主要，次要，内部和修订版本号
    /// </summary>
    public Version Version { get; set; } = Environment.Version;
    /// <summary>
    /// 获取运行应用程序的.NET安装的名称
    /// </summary>
    public string FrameworkDescription { get; set; } = RuntimeInformation.FrameworkDescription;
    /// <summary>
    /// 获取描述应用程序正在运行的操作系统的字符串
    /// </summary>
    public string OSDescription { get; set; } = RuntimeInformation.OSDescription;
    /// <summary>
    /// 代表操作系统平台
    /// </summary>
    public string OS { get; set; }
    /// <summary>
    /// 总物理内存 B
    /// </summary>
    public long TotalPhysicalMemory { get; set; } = 0;
    /// <summary>
    /// 可用物理内存 B
    /// </summary>
    public long FreePhysicalMemory { get; set; } = 0;
    /// <summary>
    /// 使用物理内存
    /// </summary>
    public long UseMemory { get; set; } = Environment.WorkingSet;
    /// <summary>
    /// 总交换空间（Linux）B
    /// </summary>
    public long SwapTotal { get; set; } = 0;
    /// <summary>
    /// 可用交换空间（Linux）B
    /// </summary>
    public long SwapFree { get; set; } = 0;
    /// <summary>
    /// 物理内存明细
    /// </summary>
    public List<Dictionary<string, object>> PhysicalMemory { get; set; } = new List<Dictionary<string, object>>();
    /// <summary>
    /// 逻辑磁盘 B
    /// </summary>
    public List<Dictionary<string, object>> LogicalDisk { get; set; } = new List<Dictionary<string, object>>();
    /// <summary>
    /// 型号
    /// </summary>
    public string Model { get; set; }
    /// <summary>
    /// 操作系统
    /// </summary>
    public string OperatingSystem { get; set; }

    /// <summary>
    /// 构造
    /// </summary>
    public SystemStatusTo()
    {
        OS = GetOSPlatform();

        if (GlobalTo.IsWindows)
        {
            LogicalDisk.AddRange(GetLogicalDisk());

            var separator = "-=-=-";

            var cmds = string.Join($" & echo {separator} & ", new List<string>
            {
                //0 系统名称、开机时间、可用物理内存、总内存
                $"wmic os get Caption,LastBootUpTime,FreePhysicalMemory,TotalVisibleMemorySize /value",

                //1 CPU name
                "wmic cpu get Name /value",

                //2 CPU used
                "PowerShell \"Get-Counter '\\Processor(_Total)\\% Processor Time'\"",

                //3 Memory Info
                "wmic MemoryChip get Capacity,Speed",

                //4 Model
                $"wmic csproduct get name /value"
            });

            try
            {
                var cr = CmdTo.Execute(cmds);
                var outGroup = cr.CrOutput.Split(separator).ToList();

                var osResult = outGroup[0].Split('\n');
                foreach (var item in osResult)
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

                var cpuResult = outGroup[1].Split('\n');
                ProcessorName = cpuResult.FirstOrDefault(x => x.StartsWith("Name=")).Split('=').LastOrDefault().Trim();

                var cpuuResult = outGroup[2].Trim().Split(Environment.NewLine);
                ProcessorUsed = Math.Ceiling(Convert.ToDecimal(cpuuResult.LastOrDefault().ToString().Trim()));

                var memoryChipResult = outGroup[3].Split('\n');
                foreach (var item in memoryChipResult)
                {
                    if (!item.Contains("Capacity") && !string.IsNullOrWhiteSpace(item))
                    {
                        var cols = item.Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
                        var dicItem = new Dictionary<string, object>
                        {
                            { "Capacity", Convert.ToInt64(cols[0]) },
                            { "Speed", Convert.ToInt64(cols[1]) },
                        };
                        PhysicalMemory.Add(dicItem);
                    }
                }

                var csproductResult = outGroup[4].Split('\n');
                Model = csproductResult.FirstOrDefault(x => x.StartsWith("Name=")).Split('=').LastOrDefault().Trim();
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

            //logical disk
            try
            {
                var items = CmdTo.Execute("df").CrOutput.Split('\n').ToList();
                foreach (var item in items)
                {
                    if (item.StartsWith("/dev/"))
                    {
                        var dis = item.Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
                        var size = long.Parse(dis[1]) * 1024;
                        //大于 1GB
                        if (size > 1073741824)
                        {
                            LogicalDisk.Add(new Dictionary<string, object>
                            {
                                { "Directory", dis.Last() },
                                { "VolumeLabel", dis.First() },
                                { "TotalSize", size },
                                { "TotalFreeSpace", long.Parse(dis[3]) * 1024 }
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("df");
                Console.WriteLine(ex);
            }

            //内存
            try
            {
                var items = File.ReadAllText("/proc/meminfo").Split('\n').ToList();
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
            }
            catch (Exception ex)
            {
                Console.WriteLine("/proc/meminfo");
                Console.WriteLine(ex);
            }

            //cpu name
            try
            {
                //lscpu
                var items = File.ReadAllText("/proc/cpuinfo").Split('\n').ToList();
                var val = items.FirstOrDefault(x => x.StartsWith("model name")).Split(':').LastOrDefault().Trim();
                ProcessorName = val;
            }
            catch (Exception ex)
            {
                Console.WriteLine("/proc/cpuinfo");
                Console.WriteLine(ex);
            }

            //cpu used
            try
            {
                var br = CmdTo.Execute("vmstat 1 2").CrOutput;
                var cpuitems = br.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).LastOrDefault().Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
                var usi = cpuitems.Count - 5;
                var val = decimal.Parse(cpuitems[usi]);
                ProcessorUsed = val;
            }
            catch (Exception ex)
            {
                Console.WriteLine("vmstat 1 2");
                Console.WriteLine(ex);
            }

            //memory info
            try
            {
                var meminfo = CmdTo.Execute("lshw -short -C memory").CrOutput;
                var items = meminfo.Split('\n');
                foreach (var item in items)
                {
                    if (item.Contains("iB") && !item.Contains("System Memory"))
                    {
                        var dicItem = new Dictionary<string, object>
                        {
                            { "Capacity", item.Split(' ').Last() }
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("lshw -short -quiet -C memory");
                Console.WriteLine(ex);
            }

            //uptime
            try
            {
                var uptime = File.ReadAllText("/proc/uptime");
                var val = Convert.ToInt64(Convert.ToDouble(uptime.Split(' ')[0]) * 1000);
                TickCount = val;
            }
            catch (Exception ex)
            {
                Console.WriteLine("/proc/uptime");
                Console.WriteLine(ex);
            }

            //model name
            try
            {
                var val = File.ReadAllText("/sys/class/dmi/id/product_name")?.Trim();
                Model = val;
            }
            catch (Exception ex)
            {
                Console.WriteLine("/sys/class/dmi/id/product_name");
                Console.WriteLine(ex);
            }

            //os name
            try
            {
                if (File.Exists("/etc/os-release"))
                {
                    var items = File.ReadAllText("/etc/os-release").Split('\n').ToList();
                    OperatingSystem = items.FirstOrDefault(x => x.Contains("PRETTY_NAME")).Split('"')[1];
                }
                else if (File.Exists("/etc/redhat-release"))
                {
                    OperatingSystem = File.ReadAllText("/etc/redhat-release");
                }
                else if (File.Exists("/etc/lsb-release"))
                {
                    var items = File.ReadAllText("/etc/lsb-release").Split('\n').ToList();
                    OperatingSystem = items.FirstOrDefault(x => x.Contains("DESCRIPTION")).Split('"')[1];
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }

    /// <summary>
    /// 获取平台
    /// </summary>
    /// <returns></returns>
    public static string GetOSPlatform()
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
                    { "TotalFreeSpace", di.TotalFreeSpace }
                };
                list.Add(dicItem);
            }
        }

        return list;
    }

    /// <summary>
    /// 系统监控
    /// </summary>
    /// <param name="isStop">是否停止</param>
    public static void SystemMonitor(ref bool isStop)
    {
        var nis = NetworkInterface.GetAllNetworkInterfaces();

        //last
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

        var sw = new Stopwatch();
        var index = 0;

        while (!isStop)
        {
            index++;
            sw.Restart();
            Thread.Sleep(100);
            var elapsed = sw.Elapsed.TotalSeconds;

            foreach (var ni in nis)
            {
                var stats = ni.GetIPStatistics();

                var br = stats.BytesReceived;
                var bs = stats.BytesSent;

                var brLocal = (br - brLast[ni]) / elapsed;
                var bsLocal = (bs - bsLast[ni]) / elapsed;

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
            if (index % 10 == 0)
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

                var ss = new SystemStatusTo();
                Console.Clear();
                Console.Write($"\r\n Received Speed: {ParsingTo.FormatByteSize(outSpeedBr)}/s  ");
                Console.Write($"Sent Speed: {ParsingTo.FormatByteSize(outSpeedBr)}/s  ");
                Console.WriteLine(ss.ToView(true));
            }
        }
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
    /// 可视化输出
    /// </summary>
    /// <param name="isRealTime">实时</param>
    /// <returns></returns>
    public string ToView(bool isRealTime = false)
    {
        var dic = new Dictionary<int, string>
        {
            { 0, "" },
            { 1, $" Framework: {FrameworkDescription}" },
            { 2, $" Memory: {ParsingTo.FormatByteSize(size:UseMemory)}" },
            { 3, $" System: {(string.IsNullOrWhiteSpace(OperatingSystem) ? OS : OperatingSystem)}{(Is64BitOperatingSystem ? " , 64Bit" : "")}" },
            { 4, $" OSVersion: {OSVersion.VersionString}" },
            { 5, $" User: {UserName}" },
            { 6, $" Uptime: {Math.Round(TickCount*1.0/1000/24/3600,2)} Days" },
            { 7, $" CPU: {ProcessorName} , {ProcessorCount} Core{ProgressBar(Convert.ToInt64(ProcessorUsed*100), 10000, false)}" },
            { 8, $" RAM: {ProgressBar(TotalPhysicalMemory-FreePhysicalMemory,TotalPhysicalMemory)}" }
        };
        if (SwapTotal > 0)
        {
            dic.Add(9, $" Swap: {ProgressBar(SwapTotal - SwapFree, SwapTotal)}");
        }

        var listlgd = new List<string>();
        LogicalDisk.ForEach(item =>
        {
            var totalSize = (long)item["TotalSize"];
            var totalFreeSpace = (long)item["TotalFreeSpace"];
            var dir = item["Directory"].ToString();
            listlgd.Add(ProgressBar(totalSize - totalFreeSpace, totalSize, true, dir));
        });
        dic.Add(10, $" Disk: {string.Join("", listlgd)}");

        if (isRealTime)
        {
            new[] { 1, 2, 3, 4, 5 }.ToList().ForEach(key => dic.Remove(key));
        }

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
            text = $"\n{text}";
        }
        var per = " " + (v2 == 0 ? 0 : Math.Round((v1 / v2) * 100, 0)) + "%";
        text = per + text;
        listpb.Add(text);

        return string.Join("", listpb);
    }
}