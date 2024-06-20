#if Full || ConsoleX

using System.Runtime.Versioning;
using System.Text.RegularExpressions;
using Hardware.Info;

namespace Netnr;

/// <summary>
/// Tool
/// </summary>
public partial class MenuItemService
{
    [Display(Name = "System Info", Description = "系统信息", GroupName = "Tool",
        ShortName = "sinfo [-j]|[--send url --address 0.0.0.0]", Prompt = "sinfo\r\nsinfo -j\r\nsinfo --send http://zme.ink")]
    public static async Task SystemInfo()
    {
        var wItems = "view,json,send".Split(',');
        var wType = ConsoleXTo.VarIndex(0, "输出为", wItems);

        var ss = new SystemStatusTo();

        switch (wType)
        {
            case "--json":
            case "json":
            case "-j":
                await ss.RefreshAll();
                ConsoleTo.LogColor(ss.ToJson(true));
                break;
            case "--send":
            case "send":
            case "-s":
                {
                    var url = ConsoleXTo.VarName("--send", "Send URL");
                    if (string.IsNullOrWhiteSpace(url))
                    {
                        ConsoleTo.LogColor("URL 无效", ConsoleColor.Red);
                    }
                    else
                    {
                        await ss.RefreshAll(true);

                        var address = ConsoleXTo.VarName("--address", $"指定地址(default: {ss.AddressInterNetwork})");
                        if (!string.IsNullOrWhiteSpace(address) && IPAddress.TryParse(address, out _))
                        {
                            ss.AddressInterNetwork = address;
                        }

                        ss.RefreshCPU();

                        var paramsJson = ss.ToJson();
                        var postData = $"paramsJson={paramsJson.ToUrlEncode()}";
                        ConsoleTo.LogColor($"curl -X POST {url} -d '{postData}'");

                        //https://stackoverflow.com/questions/12553277
                        var handler = new HttpClientHandler
                        {
                            ClientCertificateOptions = ClientCertificateOption.Manual,
                            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                        };
                        using var hc = new HttpClient(handler);

                        var content = new FormUrlEncodedContent(new Dictionary<string, string> { { "paramsJson", paramsJson } });
                        try
                        {
                            var resp = await hc.PostAsync(url, content);

                            if (resp.IsSuccessStatusCode)
                            {
                                var read = await resp.Content.ReadAsStringAsync();
                                ConsoleTo.LogColor(read);
                            }
                            else
                            {
                                ConsoleTo.LogColor($"Failed to send, StatusCode: {resp.StatusCode}");
                            }
                        }
                        catch (Exception ex)
                        {
                            ConsoleTo.LogError(ex);
                        }
                    }
                }
                break;
            default:
                await ss.RefreshAll();
                ConsoleTo.LogColor(ss.ToView());
                break;
        }
    }

    [Display(Name = "System Monitor", Description = "系统监控", GroupName = "Tool",
        ShortName = "sming", Prompt = "sming")]
    public static void SystemMonitor()
    {
        ConsoleTo.LogColor("Starting System Monitor ...");

        var IsWorking = true;
        var thread = new Thread(async () =>
        {
            var ss = new SystemStatusTo();
            await ss.RefreshAll();

            var hop = 0;
            var lastStats = SystemStatusTo.GetNetworkInterfaceStats();
            while (IsWorking)
            {
                var result = SystemStatusTo.GetNetworkInterfaceCalc(ref lastStats);
                Console.Clear();

                var sumReceived = result.Sum(x => x.BytesReceived);
                var sumSent = result.Sum(x => x.BytesSent);

                var outinfo = new string[]
                {
                    $"\r\n Received Speed: {ParsingTo.FormatByte(sumReceived)}/s",
                    $"\r\n\r\n Sent Speed: {ParsingTo.FormatByte(sumSent)}/s",
                    ss.ToView()
                };
                Console.WriteLine(string.Join("", outinfo));

                if (hop == 0)
                {
                    _ = ss.RefreshAll();
                }
                else
                {
                    ss.RefreshCPU();
                }

                if (hop++ > 2)
                {
                    hop = 0;
                }
            }
        });
        thread.Start();

        while (Console.ReadLine() != "exit")
        {
            Console.WriteLine($"Enter \"exit\" stop");
        }
        IsWorking = false;
    }

    [Display(Name = "Hardware Info", Description = "硬件信息", GroupName = "Tool",
        ShortName = "hinfo", Prompt = "hinfo")]
    public static void HardwareInfo()
    {
        var hinfo = new HardwareInfo();
        hinfo.RefreshAll();
        ConsoleTo.LogColor(hinfo.ToJson(true));
    }

    [Display(Name = "Process Info", Description = "程序信息", GroupName = "Tool",
        ShortName = "pinfo --pid [pid]", Prompt = "pinfo --pid 4")]
    public static void ProcessInfo()
    {
        var pid = ConsoleXTo.VarName("--pid", "输入 PID");
        var full = ConsoleXTo.VarBool("完整的信息（默认精简）", "--full,-f");

        Process process;
        if (string.IsNullOrWhiteSpace(pid))
        {
            process = Process.GetCurrentProcess();
        }
        else
        {
            try
            {
                process = Process.GetProcessById(Convert.ToInt32(pid));
            }
            catch (Exception ex)
            {
                ConsoleTo.LogColor(ex.Message, ConsoleColor.Red);

                process = Process.GetCurrentProcess();
            }
        }

        var dict = process.ToProps(full);
        var removes = dict.Keys.Where(x => x.EndsWith("64")).Select(x => x[..^2]);
        removes.ForEach(x => dict.Remove(x));
        ConsoleTo.LogColor(dict.ToJson(true));
    }

    [Display(Name = "Consume", Description = "消耗", GroupName = "Tool", AutoGenerateFilter = true,
        ShortName = "consume --cpu [1-100] --ram [1-100] --idle [1-7,20-23] --downloadurl [url] --downloadspeed [1024-2097152] --interval [5-20] --time [9]",
        Prompt = "consume\r\nconsume --cpu 1-40 --ram 1-50 --idle 1-7,20-23 --time 9")]
    public static void Consume()
    {
        var cpuValue = ConsoleXTo.VarName("--cpu", $"CPU 消耗百分比(1-100 默认1-30随机)");
        var cpuRange = ConsoleXTo.RandomRange(cpuValue, 1, 40);
        var cpuRandom = cpuRange[0] != cpuRange[1];
        var cpuNumber = cpuRandom ? RandomTo.Instance.Next(cpuRange[0], cpuRange[1]) : cpuRange[0];

        var ramValue = ConsoleXTo.VarName("--ram", $"RAM 消耗百分比(1-100 默认1-50随机)");
        var ramRange = ConsoleXTo.RandomRange(ramValue, 1, 50);
        var ramRandom = ramRange[0] != ramRange[1];
        var ramNumber = ramRandom ? RandomTo.Instance.Next(ramRange[0], ramRange[1]) : ramRange[0];

        var idleValue = ConsoleXTo.VarName("--idle", $"闲时除以1-3(0-23 如0-7,20-22,23)");
        var idleHour = ConsoleXTo.RangeToList(idleValue, 0, 23);
        if (string.IsNullOrWhiteSpace(idleValue))
        {
            idleValue = "none";
        }

        var downloadUrl = ConsoleXTo.VarName("--downloadurl", $"URL 网络下载链接");
        var enableDownload = downloadUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase);

        var downloadSpeedValue = ConsoleXTo.VarName("--downloadspeed", $"URL 网络下载限速(单位 B/s 默认1024-2097152随机)");
        var downloadSpeedRange = ConsoleXTo.RandomRange(downloadSpeedValue, 1024, 2097152);
        var downloadSpeedRandom = downloadSpeedRange[0] != downloadSpeedRange[1];
        var downloadSpeedNumber = downloadSpeedRandom ? RandomTo.Instance.Next(downloadSpeedRange[0], downloadSpeedRange[1]) : downloadSpeedRange[0];

        var intervalValue = ConsoleXTo.VarName("--interval", $"间隔变化时间(单位秒 默认5-15随机)");
        var intervalRange = ConsoleXTo.RandomRange(intervalValue, 5, 15);
        var intervalRandom = intervalRange[0] != intervalRange[1];
        var intervalNumber = intervalRandom ? RandomTo.Instance.Next(intervalRange[0], intervalRange[1]) : intervalRange[0];

        var timeValue = ConsoleXTo.VarName("--time", $"倒计时自动结束(单位秒 默认永久)");
        _ = int.TryParse(timeValue, out int timeNumber);

        //闲时降低
        if (idleHour.Contains(DateTime.Now.Hour))
        {
            var reduce = RandomTo.Instance.Next(10, 30) * 0.1;
            cpuNumber = (int)Math.Ceiling(cpuNumber / reduce);
            ramNumber = (int)Math.Ceiling(ramNumber / reduce);
            downloadSpeedNumber = (int)Math.Ceiling(downloadSpeedNumber / reduce);
        }

        ConsoleTo.LogCard(string.Join(" - ", [
            $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}",
            $"CPU({string.Join('-', cpuRange)}) {cpuNumber}%",
            $"RAM({string.Join('-', ramRange)}) {ramNumber}%",
            $"IdleHour {idleValue}",
            $"Interval({string.Join('-', intervalRange)}) {intervalNumber}s",
            enableDownload ? $"Download({string.Join('-', downloadSpeedRange)}) {ParsingTo.FormatByte(downloadSpeedNumber)}/s" : "Download none"
        ]));

        //消耗 网络
        var downloadTask = new ConsoleXTo.ConsumeNetworkTask(downloadUrl, downloadSpeedNumber);
        if (enableDownload)
        {
            var threadNetwork = new Thread(async () =>
            {
                await downloadTask.StartAsync();
            })
            {
                IsBackground = true
            };
            threadNetwork.Start();
            GC.KeepAlive(threadNetwork);
        }

        //消耗 CPU
        var ctsCPU = new CancellationTokenSource();
        for (int i = 0; i < Environment.ProcessorCount; i++)
        {
            var thread = new Thread(() =>
            {
                var watch = Stopwatch.StartNew();
                //按 100 毫秒空循环需要消耗的百分比毫秒并等待剩余的毫秒
                while (!ctsCPU.IsCancellationRequested)
                {
                    if (watch.ElapsedMilliseconds > cpuNumber)
                    {
                        Thread.Sleep(100 - cpuNumber);
                        watch.Restart();
                    }
                }
            })
            {
                IsBackground = true
            };
            thread.Start();
            GC.KeepAlive(thread);
        }

        //消耗 RAM
        ConsoleXTo.ConsumeRAMTask.ConsumeRAM(ramNumber);

        var taskTime = Stopwatch.StartNew();

        //随机
        var taskCancel = new CancellationTokenSource();
        var randomWatch = Stopwatch.StartNew();
        while (!taskCancel.IsCancellationRequested)
        {
            //随机时长
            if (randomWatch.Elapsed.TotalSeconds > intervalNumber)
            {
                intervalNumber = intervalRandom ? RandomTo.Instance.Next(intervalRange[0], intervalRange[1]) : intervalRange[0];

                //随机时，CPU 可不活跃
                if (cpuRandom && taskTime.ElapsedMilliseconds % 7 == 0)
                {
                    cpuNumber = 1;
                    downloadSpeedNumber = 1024; //1k
                }
                else
                {
                    //活跃
                    cpuNumber = cpuRandom ? RandomTo.Instance.Next(cpuRange[0], cpuRange[1]) : cpuRange[0];
                    ramNumber = ramRandom ? RandomTo.Instance.Next(ramRange[0], ramRange[1]) : ramRange[0];
                    downloadSpeedNumber = downloadSpeedRandom ? RandomTo.Instance.Next(downloadSpeedRange[0], downloadSpeedRange[1]) : downloadSpeedRange[0];

                    //闲时降低
                    if (idleHour.Contains(DateTime.Now.Hour))
                    {
                        var reduce = RandomTo.Instance.Next(10, 30) * 0.1;
                        cpuNumber = (int)Math.Ceiling(cpuNumber / reduce);
                        ramNumber = (int)Math.Ceiling(ramNumber / reduce);
                        downloadSpeedNumber = (int)Math.Ceiling(downloadSpeedNumber / reduce);
                    }
                }

                ConsoleTo.LogCard(string.Join(" - ", [
                    $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}",
                    $"CPU({string.Join('-', cpuRange)}) {cpuNumber}%",
                    $"RAM({string.Join('-', ramRange)}) {ramNumber}%",
                    $"IdleHour {idleValue}",
                    $"Interval({string.Join('-', intervalRange)}) {intervalNumber}s",
                    enableDownload ? $"Download({string.Join('-', downloadSpeedRange)}) {ParsingTo.FormatByte(downloadSpeedNumber)}/s" : "Download none"
                ]));

                //重新分配
                if (ramRandom)
                {
                    ConsoleXTo.ConsumeRAMTask.ConsumeRAM(ramNumber);
                }
                if (downloadSpeedRandom)
                {
                    downloadTask.ByteSpeed = downloadSpeedNumber;
                }

                randomWatch.Restart();
            }

            Thread.Sleep(1000);

            //自动停止
            if (timeNumber > 0 && taskTime.Elapsed.TotalSeconds > timeNumber)
            {
                //释放
                ctsCPU.Cancel();
                ConsoleXTo.ConsumeRAMTask.ReleaseAll();
                downloadTask.Stop();

                taskCancel.Cancel();

                ConsoleTo.LogColor($"\r\n倒计时 {timeNumber}s 结束, {taskTime.Elapsed}");
            }
        }
    }

    [Display(Name = "Clear Memory", Description = "清理内存（仅限 Windows）", GroupName = "Tool",
        ShortName = "clearmemory -y", Prompt = "clearmemory -y")]
    public static void ClearMemory()
    {
        if (ConsoleXTo.VarBool("清理有风险，是否继续"))
        {
            CleanUpService.CleanUp();
        }
        else
        {
            ConsoleTo.LogColor("已放弃", ConsoleColor.Cyan);
        }
    }

    [Display(Name = "Pipeline", Description = "管道工具", GroupName = "Tool",
        ShortName = "pipe --source [source] --json [xpath] --formabyte --trim --split [char[0]] --regex [regex[1]] --debug",
        Prompt = @"pipe --json .key --formatbyte --trim --split ';[0]' --regex '(\d+).(\d+).(\d+).(\d+)[0]' --debug")]
    public static async Task Pipe()
    {
        string oo = null;

        if (BaseTo.IsCmdArgs)
        {
            //管道输入
            if (Console.IsInputRedirected)
            {
                oo = await Console.In.ReadToEndAsync();
            }
            else
            {
                var source = ConsoleXTo.VarName("--source", "content or file");
                if (File.Exists(source) || File.Exists(source = ParsingTo.Combine(Environment.CurrentDirectory, source)))
                {
                    oo = await File.ReadAllTextAsync(source);
                }
            }

            if (string.IsNullOrWhiteSpace(oo))
            {
                ConsoleTo.LogColor("Get source is empty", ConsoleColor.Red);
            }
            else
            {
                var isDebug = ConsoleXTo.VarBool("debug", "--debug");

                var listStep = ConsoleXTo.GetArgsKeyValue();
                foreach (var step in listStep)
                {
                    switch (step.Key)
                    {
                        case "--json":
                            {
                                if (string.IsNullOrWhiteSpace(step.Value))
                                {
                                    oo = oo.DeJson().ToJson(true);
                                }
                                else
                                {
                                    if (oo.StartsWith('['))
                                    {
                                        oo = oo.DeJArray().SelectToken(step.Value).ToNJson(true);
                                    }
                                    else
                                    {
                                        oo = oo.DeJObject().SelectToken(step.Value).ToNJson(true);
                                    }
                                }
                            }
                            break;
                        case "--formatbyte":
                            {
                                if (!string.IsNullOrWhiteSpace(oo) && double.TryParse(oo, out double sizeNumber))
                                {
                                    oo = ParsingTo.FormatByte(sizeNumber);
                                }
                            }
                            break;
                        case "--trim":
                            {
                                oo = string.IsNullOrWhiteSpace(step.Value) ? oo.Trim() : oo.Trim(step.Value[0]);
                            }
                            break;
                        case "--split":
                            {
                                var vi = step.Value.LastIndexOf('[');
                                if (vi == -1)
                                {
                                    oo = oo.Split(step.Value).ToNJson(true);
                                }
                                else
                                {
                                    var index = step.Value[(vi + 1)..].TrimEnd(']');
                                    oo = oo.Split(step.Value[..vi])[Convert.ToInt32(index)];
                                }
                            }
                            break;
                        case "--regex":
                            {
                                var vi = step.Value.LastIndexOf('[');
                                if (vi == -1)
                                {
                                    var mr = new Regex(step.Value).Match(oo);
                                    if (mr.Success)
                                    {
                                        oo = (mr.Groups as IEnumerable<Group>).Select(x => x.Value).ToNJson(true);
                                    }
                                }
                                else
                                {
                                    var mr = new Regex(step.Value[..vi]).Match(oo);
                                    if (mr.Success)
                                    {
                                        var index = step.Value[(vi + 1)..].TrimEnd(']');
                                        oo = mr.Groups[Convert.ToInt32(index)].Value;
                                    }
                                }
                            }
                            break;
                    }
                    if (isDebug && step.Key != "--debug")
                    {
                        ConsoleTo.LogCard($"{step.Key} {step.Value}");
                        ConsoleTo.LogColor(oo);
                    }
                }

                if (!isDebug)
                {
                    ConsoleTo.LogColor(oo);
                }
            }
        }
        else
        {
            ConsoleTo.LogColor("Only command line arguments are supported", ConsoleColor.Red);
        }
    }

    [Display(Name = "Environment variables", Description = "环境变量", GroupName = "Tool",
        ShortName = "env [-g] --set [key=val,key2=val2,path=dir,path=dir2] --del [key,path=dir]",
        Prompt = "env\r\nenv --get $PATH\r\nenv --set key1=val1,key2=val2\r\nenv --set \"NLS_LANG=SIMPLIFIED CHINESE_CHINA.ZHS16GBK\"\r\nenv --del key1\r\nenv --set path=D:\\software\\single\r\nenv --del path=D:\\software\\single")]
    public static void EnvironmentVariables()
    {
        var processPath = Path.GetDirectoryName(Environment.ProcessPath);

        var mItems = "get,set,del".Split(',');
        var mType = ConsoleXTo.VarIndex(0, "管理", mItems);

        if (CmdTo.IsWindows)
        {
            var evTarget = ConsoleXTo.VarBool("全局 Machine（默认 用户 User）", "-g,--global");
            var evt = evTarget ? EnvironmentVariableTarget.Machine : EnvironmentVariableTarget.User;

            switch (mType)
            {
                case "set":
                case "--set":
                    {
                        var mVal = ConsoleXTo.VarName($"--set", "设置环境变量（key=val 或 path=dir）");
                        var valArray = mVal.Split(',');
                        foreach (var valItem in valArray)
                        {
                            if (!string.IsNullOrWhiteSpace(valItem))
                            {
                                var isPath = valItem.StartsWith("path=", StringComparison.OrdinalIgnoreCase);
                                if (isPath)
                                {
                                    var pathArray = new List<string>();
                                    var currentPath = Environment.GetEnvironmentVariable("PATH", evt);
                                    if (currentPath != null)
                                    {
                                        pathArray = currentPath.Split(Path.PathSeparator).ToList();
                                    }
                                    pathArray.AddRange(valItem[5..].Split(Path.PathSeparator).Where(x => !string.IsNullOrWhiteSpace(x)));
                                    Environment.SetEnvironmentVariable("PATH", string.Join(Path.PathSeparator, pathArray.Distinct()), evt);
                                }
                                else if (valItem.Contains('='))
                                {
                                    var vv = valItem.Split('=');
                                    if (!string.IsNullOrWhiteSpace(vv[1]))
                                    {
                                        Environment.SetEnvironmentVariable(vv[0], vv[1], evt);
                                    }
                                }
                            }
                        }
                    }
                    break;
                case "del":
                case "--del":
                    {
                        var mVal = ConsoleXTo.VarName($"--del", "删除环境变量（key 或 path=dir）");
                        var valArray = mVal.Split(',');
                        foreach (var valItem in valArray)
                        {
                            if (!string.IsNullOrWhiteSpace(valItem))
                            {
                                var isPath = valItem.StartsWith("path=", StringComparison.OrdinalIgnoreCase);
                                if (isPath)
                                {
                                    var pathArray = Environment.GetEnvironmentVariable("PATH", evt).Split(Path.PathSeparator);
                                    var delPath = valItem[5..].Split(Path.PathSeparator).Where(x => !string.IsNullOrWhiteSpace(x));
                                    Environment.SetEnvironmentVariable("PATH", string.Join(Path.PathSeparator, pathArray.Where(x => !delPath.Contains(x))), evt);
                                }
                                else if (!valItem.Equals("path", StringComparison.OrdinalIgnoreCase) && Environment.GetEnvironmentVariable(valItem, evt) != null)
                                {
                                    Environment.SetEnvironmentVariable(valItem, null, evt);
                                }
                            }
                        }
                    }
                    break;
                default:
                    {
                        var varyName = ConsoleXTo.VarName("--get", "变量名称（默认全部）");
                        if (string.IsNullOrWhiteSpace(varyName))
                        {
                            var dictEnv = Environment.GetEnvironmentVariables(evt);
                            foreach (var key in dictEnv.Keys)
                            {
                                ConsoleTo.LogColor($"{key}={dictEnv[key]}");
                            }
                        }
                        else
                        {
                            varyName = varyName.TrimStart('$').ToUpper();
                            var isPath = varyName == "PATH";
                            var envVal = Environment.GetEnvironmentVariable(varyName, evt);
                            if (isPath)
                            {
                                if (envVal != null)
                                {
                                    var pathArray = envVal.Split(Path.PathSeparator);
                                    foreach (var pathItem in pathArray)
                                    {
                                        if (!string.IsNullOrWhiteSpace(pathItem))
                                        {
                                            ConsoleTo.LogColor(pathItem);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                ConsoleTo.LogColor(envVal);
                            }
                        }
                    }
                    break;
            }
        }
        else
        {
            if (AppContext.GetData("Netnr.NotFirstEnv") is not true)
            {
                ConsoleTo.LogColor($"env # 查看环境变量列表\r\necho $PATH # 查看环境变量路径\r\nexport KEY=val # 设置环境变量\r\nexport PATH=$PATH:/new/path # 设置环境变量路径\r\nvi ~/.bashrc # 编辑用户配置文件加入 export 再执行 source ~/.bashrc 生效（新开 Shell）\r\n");
                AppContext.SetData("Netnr.NotFirstEnv", true);
            }

            var bashrcPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".bashrc");

            switch (mType)
            {
                case "set":
                case "--set":
                    {
                        ConsoleTo.LogColor($"尝试写入用户配置文件 {bashrcPath}");

                        var contents = File.ReadAllLines(bashrcPath).ToList();

                        var mVal = ConsoleXTo.VarName($"--set", "设置环境变量（key=val 或 path=dir）");
                        var valArray = mVal.Split(',');

                        var isChange = false;
                        foreach (var valItem in valArray)
                        {
                            if (!string.IsNullOrWhiteSpace(valItem))
                            {
                                isChange = true;

                                var isPath = valItem.StartsWith("path=", StringComparison.OrdinalIgnoreCase);
                                if (isPath)
                                {
                                    var newItem = $"export PATH=$PATH:{valItem[5..]}";
                                    contents.Add(newItem);
                                }
                                else if (valItem.Contains('='))
                                {
                                    var vv = valItem.Split('=');
                                    if (!string.IsNullOrWhiteSpace(vv[1]))
                                    {
                                        var newItem = $"export {vv[0]}={vv[1]}";
                                        contents.Add(newItem);
                                    }
                                }
                            }
                        }

                        if (isChange)
                        {
                            File.WriteAllText(bashrcPath, string.Join(Environment.NewLine, contents.Distinct()) + Environment.NewLine);
                            var refreshEnv = $"source {bashrcPath}";
                            ConsoleTo.LogColor(refreshEnv);
                            CmdTo.Execute(refreshEnv);
                        }
                    }
                    break;
                case "del":
                case "--del":
                    {
                        ConsoleTo.LogColor($"尝试从用户配置文件中删除 {bashrcPath}");

                        var contents = File.ReadAllLines(bashrcPath).ToList();
                        var isChange = false;

                        var mVal = ConsoleXTo.VarName($"--del", "删除环境变量（key 或 path=dir）");
                        var valArray = mVal.Split(',');
                        foreach (var valItem in valArray)
                        {
                            if (!string.IsNullOrWhiteSpace(valItem))
                            {
                                isChange = true;

                                var isPath = valItem.StartsWith("path=", StringComparison.OrdinalIgnoreCase);
                                if (isPath)
                                {
                                    var delItem = new string[] { $"export PATH=$PATH:{valItem[5..]}", $"export PATH={valItem[5..]}:$PATH" };
                                    contents = contents.Where(x => !delItem.Contains(x)).ToList();
                                }
                                else
                                {
                                    contents = contents.Where(x => !x.StartsWith($"export {valItem}=")).ToList();
                                }
                            }
                        }

                        if (isChange)
                        {
                            File.WriteAllText(bashrcPath, string.Join(Environment.NewLine, contents.Distinct()) + Environment.NewLine);
                            var refreshEnv = $"source {bashrcPath}";
                            ConsoleTo.LogColor(refreshEnv);
                            CmdTo.Execute(refreshEnv);
                        }
                    }
                    break;
                default:
                    {
                        var varyName = ConsoleXTo.VarName("--get", "变量名称（默认全部）");
                        if (string.IsNullOrWhiteSpace(varyName))
                        {
                            ConsoleTo.LogColor(CmdTo.Execute("env").CrOutput);
                        }
                        else
                        {
                            ConsoleTo.LogColor(CmdTo.Execute($"env | grep {varyName}=").CrOutput);
                        }
                    }
                    break;
            }
        }
    }

    [Display(Name = ".NET Framework", Description = "已安装的 .NET Framework", GroupName = "Tool",
        ShortName = "dotnetframework", Prompt = "dotnetframework")]
    [SupportedOSPlatform("windows")]
    public static void DOTNETFramework()
    {
        var result1 = WinTo.Get45Less();
        result1.ForEach(item =>
        {
            ConsoleTo.LogColor($"{item.Version}\t{item.ServicePack}");
        });

        var result2 = WinTo.Get45Plus();
        result2.ForEach(item =>
        {
            ConsoleTo.LogColor($"{item.Version}\t{item.ServicePack}");
        });
    }

    [Display(Name = "Generate UUID", Description = "生成UUID", GroupName = "Tool",
        ShortName = "uuid [count]", Prompt = "uuid\r\nuuid -9")]
    public static void GenerateUUID()
    {
        var count = ConsoleXTo.VarIndex(0, "生成个数");
        _ = int.TryParse(count, out int countNumber);
        countNumber = Math.Max(1, Math.Abs(countNumber));

        for (int i = 0; i < countNumber; i++)
        {
            ConsoleTo.LogColor(Guid.NewGuid().ToString());
        }
    }

    [Display(Name = "Generate Snowflake", Description = "雪花ID", GroupName = "Tool",
        ShortName = "snow [count|id|time]", Prompt = "snow\r\nsnow 9\r\nsnow 154689429516288\r\nsnow \"2023-07-28 15:50:45.943\"")]
    public static void GenerateSnowflake()
    {
        var count = ConsoleXTo.VarIndex(0, "生成数量 或 时间和ID转换");
        if (DateTime.TryParse(count, out DateTime time))
        {
            ConsoleTo.LogColor(Snowflake53To.GetId(time).ToString());
        }
        else
        {
            _ = long.TryParse(count, out long countNumber);
            countNumber = Math.Max(1, Math.Abs(countNumber));

            if (countNumber > 1000000)
            {
                time = Snowflake53To.Parse(countNumber).Value;
                ConsoleTo.LogColor(time.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            }
            else
            {
                for (long i = 0; i < countNumber; i++)
                {
                    ConsoleTo.LogColor(Snowflake53To.Id().ToString());
                }
            }
        }
    }

    [Display(Name = "Tail", Description = "读取文件最新内容", GroupName = "Tool",
        ShortName = "tail [file]", Prompt = "tail access.log")]
    public static void Tail()
    {
        string filePath = ConsoleXTo.VarIndex(0, "文件路径");
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
                var lastContent = sr.ReadToEnd().Split('\n').TakeLast(10);
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
            ConsoleTo.LogColor("file not found", ConsoleColor.Red);
        }
    }

    [Display(Name = "Wget", Description = "下载文件", GroupName = "Tool",
        ShortName = "wget [url] -O [savePath] --limit-rate [B]", Prompt = "wget https://zme.ink/favicon.ico")]
    public static async Task Wget()
    {
        var uriString = ConsoleXTo.VarIndex(0, "url");
        var uri = new Uri(uriString);

        var fileName = uri.AbsolutePath.Split('/').LastOrDefault();
        if (string.IsNullOrEmpty(fileName))
        {
            fileName = DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        var defaultSavePath = Path.Combine(Environment.CurrentDirectory, fileName);
        var savePath = ConsoleXTo.VarName("-O", $"保存为（默认：{defaultSavePath}）");
        if (string.IsNullOrWhiteSpace(savePath))
        {
            savePath = defaultSavePath;
        }
        if (File.Exists(savePath))
        {
            var no = 1;
            var newSavePath = savePath + "." + no++;
            while (Path.Exists(newSavePath))
            {
                newSavePath = savePath + "." + no++;
            }
            savePath = newSavePath;
        }

        var limitRate = ConsoleXTo.VarName("--limit-rate", $"限速（单位 B）");
        _ = long.TryParse(limitRate, out var limitRateNumber);

        await Console.Out.WriteLineAsync($"--{DateTime.Now:yyyy-MM-dd HH:mm:ss}--  {uri}");
        await Console.Out.WriteAsync($"HTTP request sent, awaiting response... ");

        var client = HttpTo.BuildClient();
        client.Timeout = TimeSpan.FromSeconds(30);

        HttpResponseMessage response = null;

        try
        {
            response = await client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);
        }
        catch (Exception ex)
        {
            await Console.Out.WriteAsync(ex.Message);
            ConsoleTo.LogError(ex);
        }

        if (response != null)
        {
            await Console.Out.WriteAsync($"{(int)response.StatusCode} {response.ReasonPhrase}\r\n");
            if (response.IsSuccessStatusCode)
            {
                // 获取文件大小
                long fileSize = response.Content.Headers.ContentLength ?? 0;

                Console.WriteLine($"Length: {fileSize} ({ParsingTo.FormatByte(fileSize)}) [{response.Content.Headers.ContentType}]");
                Console.WriteLine($"Saving to: '{savePath}'\r\n");

                // 创建文件流
                using var fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write, FileShare.None);
                // 创建缓冲区
                byte[] buffer = new byte[8192];
                int bytesRead;
                long totalBytesRead = 0;
                var startTime = DateTime.Now;
                var st = Stopwatch.StartNew();

                // 循环读取响应流并写入文件流
                using Stream contentStream = await response.Content.ReadAsStreamAsync();
                while ((bytesRead = await contentStream.ReadAsync(buffer)) > 0)
                {
                    // 限制下载速度
                    if (limitRateNumber > 0)
                    {
                        double elapsedSeconds = (DateTime.Now - startTime).TotalSeconds;
                        int maxBytes = (int)(limitRateNumber * elapsedSeconds);
                        if (totalBytesRead >= maxBytes)
                        {
                            int sleepTime = (int)((totalBytesRead - maxBytes) * 1000 / limitRateNumber);
                            await Task.Delay(sleepTime);
                        }
                    }

                    await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead));
                    totalBytesRead += bytesRead;

                    // 计算并显示下载进度
                    if (st.ElapsedMilliseconds > 1000)
                    {
                        var progress = Math.Round((double)totalBytesRead / fileSize * 100, 2) + "%";
                        await Console.Out.WriteLineAsync($"{progress,8}  {ParsingTo.FormatByte(totalBytesRead),10}");

                        st.Restart();
                    }
                }
                Console.WriteLine($"\r\n{DateTime.Now:yyyy-MM-dd HH:mm:ss} - '{savePath}' saved");
            }
            await Console.Out.WriteLineAsync("");
        }
    }

    [Display(Name = "Text Mining", Description = "文本挖掘", GroupName = "Tool",
        ShortName = "textmining [file] --top [50]", Prompt = "textmining text.txt\r\ntextmining text.txt --top 999")]
    public static void TextMining()
    {
        string filePath = ConsoleXTo.VarIndex(0, "文件路径或内容");

        var dvTop = 50;
        string topRow = ConsoleXTo.VarIndex(0, $"输出前 N 项(default {dvTop})");
        if (int.TryParse(topRow, out int topRowNumber))
        {
            dvTop = Math.Max(topRowNumber, 1);
        }

        var tm = new TextMiningTo();
        if (File.Exists(filePath))
        {
            tm.FromFile(filePath);
            ConsoleTo.LogColor(tm.TopItems(dvTop).ToJson());
        }
        else if (filePath.Length > 9)
        {
            tm.FromString(new string[] { filePath });
            ConsoleTo.LogColor(tm.TopItems(dvTop).ToJson());
        }
        else
        {
            ConsoleTo.LogColor("内容太少了", ConsoleColor.Red);
        }
    }

    [Display(Name = "deep delete", Description = "深度删除匹配的文件（夹）", GroupName = "Tool",
        ShortName = "ddel [dir] [matchName] [ignoreDir] [-y]", Prompt = "ddel ./ bin,*.md,file?.txt -y")]
    public static void DeepDelete()
    {
        var dir = ConsoleXTo.VarIndex(0, $"根目录(default: {Environment.CurrentDirectory})");
        if (string.IsNullOrWhiteSpace(dir) || !Directory.Exists(dir))
        {
            dir = Environment.CurrentDirectory;
        }

        var matchName = ConsoleXTo.VarIndex(1, "要删除的文件（夹），如 bin,*.md,file?.txt 逗号分割");
        var ignoreDir = ConsoleXTo.VarIndex(2, "要忽略的目录名称，逗号分割");

        if (!string.IsNullOrWhiteSpace(matchName))
        {
            var isDelete = ConsoleXTo.VarBool("真删除");

            var listSearch = matchName.Split(',').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

            var listIgnore = new List<string>();
            if (!string.IsNullOrWhiteSpace(ignoreDir))
            {
                listIgnore = ignoreDir.Split(',').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            }

            ConsoleXTo.EachSearchRemove(new DirectoryInfo(dir), listSearch, listIgnore, isDelete);
        }
    }

    [Display(Name = "Directory Time", Description = "目录时间", GroupName = "Tool",
        ShortName = "directorytime --dir [dir] --daterange [daterange]", Prompt = "directorytime --dir ./ --daterange 2023-06-01,2023-08-31")]
    public static void DirectoryTime()
    {
        var dir = ConsoleXTo.VarName("--dir", "请输入目录");
        dir = ConsoleXTo.ConsoleAsPath(dir);
        ConsoleTo.LogColor("已选择目录：" + dir);

        var daterange = ConsoleXTo.VarName("--daterange", "请输入日期范围（如：2023-06-01,2023-08-31）").Split(',');
        var date1 = Convert.ToDateTime(daterange[0]);
        var date2 = Convert.ToDateTime(daterange[1]);

        var dirCount = 0;
        var fileCount = 0;

        FileTo.EachDirectory(dir, (listDir, listFile) =>
        {
            listDir.ForEach(dir =>
            {
                dirCount++;

                var workTime1 = ConsoleXTo.GenerateWorkingTime(date1, date2);
                var workTime2 = ConsoleXTo.GenerateWorkingTime(date1, date2);
                if (workTime1.HasValue && workTime2.HasValue)
                {
                    dir.CreationTime = workTime1 < workTime2 ? workTime1.Value : workTime2.Value;
                    dir.LastWriteTime = workTime1 > workTime2 ? workTime1.Value : workTime2.Value;
                }
                else
                {
                    throw new Exception("生成日期失败");
                }
            });

            listFile.ForEach(file =>
            {
                fileCount++;

                var workTime1 = ConsoleXTo.GenerateWorkingTime(date1, date2);
                var workTime2 = ConsoleXTo.GenerateWorkingTime(date1, date2);
                if (workTime1.HasValue && workTime2.HasValue)
                {
                    file.CreationTime = workTime1 < workTime2 ? workTime1.Value : workTime2.Value;
                    file.LastWriteTime = workTime1 > workTime2 ? workTime1.Value : workTime2.Value;
                }
                else
                {
                    throw new Exception("生成日期失败");
                }
            });
        });

        //根目录
        var dirTime1 = ConsoleXTo.GenerateWorkingTime(date1, date2);
        var dirTime2 = ConsoleXTo.GenerateWorkingTime(date1, date2);
        _ = new DirectoryInfo(dir)
        {
            CreationTime = dirTime1 < dirTime2 ? dirTime1.Value : dirTime2.Value,
            LastWriteTime = dirTime1 > dirTime2 ? dirTime1.Value : dirTime2.Value
        };
        dirCount++;

        ConsoleTo.LogColor($"Done! File: {fileCount}, Directory: {dirCount}", ConsoleColor.Cyan);
    }

    [Display(Name = "Git Pull", Description = "批量拉取", GroupName = "Tool",
        ShortName = "gitpull [dir]", Prompt = "gitpull ./")]
    public static void GitPull()
    {
        var dir = ConsoleXTo.VarIndex(0, "请输入目录");
        if (!Directory.Exists(dir))
        {
            dir = Environment.CurrentDirectory;
        }

        var di = new DirectoryInfo(dir);
        var sdis = di.EnumerateDirectories();
        if (Directory.Exists(Path.Combine(di.FullName, ".git")))
        {
            sdis.Add(di);
        }

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
                    ConsoleTo.LogColor($"add safe.directory {sdi.FullName}, please try again");
                }
                else
                {
                    var rt = cr.CrOutput + cr.CrError;
                    ConsoleTo.LogColor($"【 {sdi.Name} 】\n{rt}");
                    c1++;
                }
            }
            else
            {
                ConsoleTo.LogColor($"已跳过 \"{sdi.FullName}\", 未找到 .git\n");
                c2++;
            }
        });
        ConsoleTo.LogColor($"Done!  Pull: {c1}, Skip: {c2}");

        if (listUnsafe.Count > 0)
        {
            Console.WriteLine(Environment.NewLine);
            ConsoleTo.LogColor(string.Join("\r\n", listUnsafe));
        }
    }

    [Display(Name = "AES Conn", Description = "连接字符串加密解密", GroupName = "Tool",
        ShortName = "aesconn --pwd [pwd] --encrypt [encrypt] --decrypt [decrypt]", Prompt = "aesconn --pwd 123 --encrypt conn")]
    public static void AesConn()
    {
        var txtPwd = ConsoleXTo.VarName("--password,--pwd", "请输入密码");
        var txtEncrypt = ConsoleXTo.VarName("--encrypt", "要加密的连接字符串");
        var txtDecrypt = ConsoleXTo.VarName("--decrypt", "要解密的连接字符串");

        if (!string.IsNullOrWhiteSpace(txtEncrypt))
        {
            ConsoleTo.LogCard("加密后的连接字符串");
            ConsoleTo.LogColor(CalcTo.AESEncrypt(txtEncrypt, txtPwd));
        }

        if (!string.IsNullOrWhiteSpace(txtDecrypt))
        {
            ConsoleTo.LogCard("解密后的连接字符串");
            ConsoleTo.LogColor(CalcTo.AESDecrypt(txtDecrypt, txtPwd));
        }
    }
}

#endif