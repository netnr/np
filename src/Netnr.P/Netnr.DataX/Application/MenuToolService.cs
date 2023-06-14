using System.Net;
using System.Net.Http;
using Hardware.Info;
using System.Runtime.InteropServices;

namespace Netnr.DataX.Application
{
    /// <summary>
    /// Tool
    /// </summary>
    public partial class MenuItemService
    {
        [Display(Name = "System Info", Description = "系统信息", GroupName = "Tool",
            ShortName = "sinfo [-j]|[--send url --address 0.0.0.0]", Prompt = "ndx sinfo\r\nndx sinfo -j\r\nndx sinfo --send http://zme.ink")]
        public static async Task SystemInfo()
        {
            var wItems = "view,json,send".Split(',');
            var wType = DXService.VarString(0, "输出为", wItems);

            var ss = new SystemStatusTo();

            switch (wType)
            {
                case "--json":
                case "json":
                case "-j":
                    await ss.RefreshAll();
                    DXService.Log(ss.ToJson(true));
                    break;
                case "--send":
                case "send":
                case "-s":
                    {
                        var url = DXService.VarName("--send", "Send URL");
                        if (string.IsNullOrWhiteSpace(url))
                        {
                            DXService.Log("URL 无效", ConsoleColor.Red);
                        }
                        else
                        {
                            await ss.RefreshAll(true);

                            var address = DXService.VarName("--address", $"指定地址(default: {ss.AddressInterNetwork})");
                            if (!string.IsNullOrWhiteSpace(address))
                            {
                                ss.AddressInterNetwork = IPAddress.Parse(address);
                            }

                            ss.RefreshCPU();

                            var paramsJson = ss.ToJson();
                            var postData = $"paramsJson={paramsJson.ToUrlEncode()}";
                            DXService.Log($"curl -X POST {url} -d '{postData}'");

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
                                    DXService.Log(read);
                                }
                                else
                                {
                                    DXService.Log($"Failed to send, StatusCode: {resp.StatusCode}");
                                }
                            }
                            catch (Exception ex)
                            {
                                DXService.Log(ex);
                            }
                        }
                    }
                    break;
                default:
                    await ss.RefreshAll();
                    DXService.Log(ss.ToView());
                    break;
            }
        }

        [Display(Name = "System Monitor", Description = "系统监控", GroupName = "Tool",
            ShortName = "sming", Prompt = "ndx sming")]
        public static void SystemMonitor()
        {
            DXService.Log("Starting System Monitor ...");

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
                        $"\r\n Received Speed: {ParsingTo.FormatByteSize(sumReceived)}/s",
                        $"\r\n\r\n Sent Speed: {ParsingTo.FormatByteSize(sumSent)}/s",
                        ss.ToView()
                    };
                    Console.WriteLine(string.Join("", outinfo));

                    if (hop % 3 == 0)
                    {
                        _ = ss.RefreshAll();
                        hop = 0;
                    }
                    else
                    {
                        ss.RefreshCPU();
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
            ShortName = "hinfo", Prompt = "ndx hinfo")]
        public static void HardwareInfo()
        {
            var hinfo = new HardwareInfo();
            hinfo.RefreshAll();
            DXService.Log(hinfo.ToJson(true));
        }

        [Display(Name = "Process Info", Description = "程序信息", GroupName = "Tool",
            ShortName = "pinfo --pid [pid]", Prompt = "ndx pinfo --pid 4")]
        public static void ProcessInfo()
        {
            var pid = DXService.VarName("--pid", "输入 PID");
            var full = DXService.VarBool("完整的信息（默认精简）", "--full,-f");

            Process process;
            try
            {
                process = Process.GetProcessById(Convert.ToInt32(pid));
            }
            catch (Exception ex)
            {
                DXService.Log($"{ex.Message}\r\n获取 PID 失败，已返回自身信息", ConsoleColor.Red);

                process = Process.GetCurrentProcess();
            }

            var dict = process.ToProps(full);
            DXService.Log(dict.ToJson(true));
        }

        [Display(Name = "Consume", Description = "消耗", GroupName = "Tool", AutoGenerateFilter = true,
            ShortName = "consume --cpu [1-100] --ram [1-100] --idle [1-7,20-23] --time [9]", Prompt = "ndx consume\r\nndx consume --cpu 1-40 --ram 1-50 --idle 1-7,20-23 --time 9")]
        public static void Consume()
        {
            var cpuValue = DXService.VarName("--cpu", $"CPU 消耗百分比(1-100 默认1-40随机)");
            var cpuRange = DXService.RandomRange(cpuValue, 1, 40);
            var cpuRandom = cpuRange[0] != cpuRange[1];
            var cpuNumber = cpuRandom ? RandomTo.Instance.Next(cpuRange[0], cpuRange[1]) : cpuRange[0];

            var ramValue = DXService.VarName("--ram", $"RAM 消耗百分比(1-100 默认1-50随机)");
            var ramRange = DXService.RandomRange(ramValue, 1, 50);
            var ramRandom = ramRange[0] != ramRange[1];
            var ramNumber = ramRandom ? RandomTo.Instance.Next(ramRange[0], ramRange[1]) : ramRange[0];

            var idleValue = DXService.VarName("--idle", $"闲时减半(0-23 如0-7,20-22,23)");
            var idleHour = DXService.RangeToList(idleValue, 0, 23);
            if (string.IsNullOrWhiteSpace(idleValue))
            {
                idleValue = "none";
            }

            var timeValue = DXService.VarName("--time", $"倒计时(单位秒 默认永久)");
            _ = int.TryParse(timeValue, out int timeNumber);

            //闲时减半
            if (idleHour.Contains(DateTime.Now.Hour))
            {
                cpuNumber = (int)Math.Ceiling(cpuNumber / 2.0);
                ramNumber = (int)Math.Ceiling(ramNumber / 2.0);
            }

            //随机间隔
            var randomInterval = 10;

            DXService.Log($"\r\nCPU({string.Join('-', cpuRange)}) {cpuNumber}% , RAM({string.Join('-', ramRange)}) {ramNumber}% , IdleHour {idleValue} , Interval {randomInterval}s");

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
            IntPtr[] fillResult = null;
            DXService.ConsumeRAM(ramNumber, ref fillResult);

            var taskTime = Stopwatch.StartNew();

            //随机
            var taskCancel = new CancellationTokenSource();
            var randomWatch = Stopwatch.StartNew();
            while (!taskCancel.IsCancellationRequested)
            {
                //随机时长
                if (randomWatch.Elapsed.TotalSeconds > randomInterval)
                {
                    //随机时，可不活跃
                    if (ramRandom && cpuRandom && taskTime.ElapsedMilliseconds % 2 == 0)
                    {
                        randomInterval = RandomTo.Instance.Next(10, 90);
                        cpuNumber = 1;
                        ramNumber = 1;
                    }
                    else
                    {
                        //活跃
                        randomInterval = RandomTo.Instance.Next(10, 30);

                        cpuNumber = cpuRandom ? RandomTo.Instance.Next(cpuRange[0], cpuRange[1]) : cpuRange[0];
                        ramNumber = ramRandom ? RandomTo.Instance.Next(ramRange[0], ramRange[1]) : ramRange[0];

                        //闲时减半
                        if (idleHour.Contains(DateTime.Now.Hour))
                        {
                            cpuNumber = (int)Math.Ceiling(cpuNumber / 2.0);
                            ramNumber = (int)Math.Ceiling(ramNumber / 2.0);
                        }
                    }

                    DXService.Log($"\r\nCPU({string.Join('-', cpuRange)}) {cpuNumber}% , RAM({string.Join('-', ramRange)}) {ramNumber}% , IdleHour {idleValue} , Interval {randomInterval}s , {taskTime.Elapsed}");

                    //随机时，重新分配
                    if (ramRandom)
                    {
                        //释放
                        foreach (var item in fillResult)
                        {
                            Marshal.FreeHGlobal(item);
                        }
                        DXService.ConsumeRAM(ramNumber, ref fillResult);
                    }
                    randomWatch.Restart();
                }

                Thread.Sleep(1000);

                //自动停止
                if (timeNumber > 0 && taskTime.Elapsed.TotalSeconds > timeNumber)
                {
                    //释放
                    ctsCPU.Cancel();
                    taskCancel.Cancel();

                    DXService.Log($"\r\n倒计时 {timeNumber}s 结束, {taskTime.Elapsed}");
                }
            }
        }

        [Display(Name = "Clear Memory", Description = "清理内存（仅限 Windows）", GroupName = "Tool",
            ShortName = "clearmemory -y", Prompt = "ndx clearmemory -y")]
        public static void ClearMemory()
        {
            if (DXService.VarBool("清理有风险，是否继续"))
            {
                ClearMemoryService.CleanUp();
            }
            else
            {
                DXService.Log("已放弃", ConsoleColor.Cyan);
            }
        }

        [Display(Name = "Environment variables", Description = "环境变量", GroupName = "Tool",
            ShortName = "env [-g] --set [key=val,key2=val2,path=dir,path=dir2] --del [key,path=dir]", Prompt = "ndx env\r\nndx env --get $PATH\r\nndx env --set key1=val1,key2=val2\r\nndx env --del key1\r\nndx env --set path=D:\\software\\single\r\nndx env --del path=D:\\software\\single")]
        public static void EnvironmentVariables()
        {
            var processPath = Path.GetDirectoryName(Environment.ProcessPath);

            var mItems = "get,set,del".Split(',');
            var mType = DXService.VarString(0, "管理", mItems);

            if (CmdTo.IsWindows)
            {
                var evTarget = DXService.VarBool("全局 Machine（默认 用户 User）", "-g,--global");
                var evt = evTarget ? EnvironmentVariableTarget.Machine : EnvironmentVariableTarget.User;

                switch (mType)
                {
                    case "set":
                    case "--set":
                        {
                            var mVal = DXService.VarName($"--set", "设置环境变量（key=val 或 path=dir）");
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
                            var mVal = DXService.VarName($"--del", "删除环境变量（key 或 path=dir）");
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
                                    else if (Environment.GetEnvironmentVariable(valItem, evt) != null)
                                    {
                                        Environment.SetEnvironmentVariable(valItem, null, evt);
                                    }
                                }
                            }
                        }
                        break;
                    default:
                        {
                            var varyName = DXService.VarName("--get", "变量名称（默认全部）");
                            if (string.IsNullOrWhiteSpace(varyName))
                            {
                                var dictEnv = Environment.GetEnvironmentVariables(evt);
                                foreach (var key in dictEnv.Keys)
                                {
                                    DXService.Log($"{key}={dictEnv[key]}");
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
                                                DXService.Log(pathItem);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    DXService.Log(envVal);
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
                    DXService.Log($"env # 查看环境变量列表\r\necho $PATH # 查看环境变量路径\r\nexport KEY=val # 设置环境变量\r\nexport PATH=$PATH:/new/path # 设置环境变量路径\r\nvi ~/.bashrc # 编辑用户配置文件加入 export 再执行 source ~/.bashrc 生效（新开 Shell）\r\n");
                    AppContext.SetData("Netnr.NotFirstEnv", true);
                }

                var bashrcPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".bashrc");

                switch (mType)
                {
                    case "set":
                    case "--set":
                        {
                            DXService.Log($"尝试写入用户配置文件 {bashrcPath}");

                            var contents = File.ReadAllLines(bashrcPath).ToList();

                            var mVal = DXService.VarName($"--set", "设置环境变量（key=val 或 path=dir）");
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
                                DXService.Log(refreshEnv);
                                CmdTo.Execute(refreshEnv);
                            }
                        }
                        break;
                    case "del":
                    case "--del":
                        {
                            DXService.Log($"尝试从用户配置文件中删除 {bashrcPath}");

                            var contents = File.ReadAllLines(bashrcPath).ToList();
                            var isChange = false;

                            var mVal = DXService.VarName($"--del", "删除环境变量（key 或 path=dir）");
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
                                DXService.Log(refreshEnv);
                                CmdTo.Execute(refreshEnv);
                            }
                        }
                        break;
                    default:
                        {
                            var varyName = DXService.VarName("--get", "变量名称（默认全部）");
                            if (string.IsNullOrWhiteSpace(varyName))
                            {
                                DXService.Log(CmdTo.Execute("env").CrOutput);
                            }
                            else
                            {
                                DXService.Log(CmdTo.Execute($"env | grep {varyName}=").CrOutput);
                            }
                        }
                        break;
                }
            }
        }

        [Display(Name = "Generate UUID", Description = "生成UUID", GroupName = "Tool",
            ShortName = "uuid [count]", Prompt = "ndx uuid\r\nndx uuid -9")]
        public static void GenerateUUID()
        {
            var count = DXService.VarString(0, "生成个数");
            _ = int.TryParse(count, out int countNumber);
            countNumber = Math.Max(1, Math.Abs(countNumber));

            for (int i = 0; i < countNumber; i++)
            {
                DXService.Log(Guid.NewGuid().ToString());
            }
        }

        [Display(Name = "Generate Snowflake", Description = "生成雪花ID", GroupName = "Tool",
            ShortName = "snow [count]", Prompt = "ndx snow\r\nndx snow -9")]
        public static void GenerateSnowflake()
        {
            var count = DXService.VarString(0, "生成个数");
            _ = int.TryParse(count, out int countNumber);
            countNumber = Math.Max(1, Math.Abs(countNumber));

            for (int i = 0; i < countNumber; i++)
            {
                DXService.Log(Snowflake53To.Id().ToString());
            }
        }

        [Display(Name = "Tail", Description = "读取文件最新内容", GroupName = "Tool",
            ShortName = "tail [file]", Prompt = "ndx tail access.log")]
        public static void Tail()
        {
            string filePath = DXService.VarString(0, "文件路径");
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
                DXService.Log("file not found", ConsoleColor.Red);
            }
        }

        [Display(Name = "Text Mining", Description = "文本挖掘", GroupName = "Tool",
            ShortName = "textmining [file] --top [50]", Prompt = "ndx textmining text.txt\r\nndx textmining text.txt --top 999")]
        public static void TextMining()
        {
            string filePath = DXService.VarString(0, "文件路径或内容");

            var dvTop = 50;
            string topRow = DXService.VarString(0, $"输出前 N 项(default {dvTop})");
            if (int.TryParse(topRow, out int topRowNumber))
            {
                dvTop = Math.Max(topRowNumber, 1);
            }

            var tm = new TextMiningTo();
            if (File.Exists(filePath))
            {
                tm.FromFile(filePath);
                DXService.Log(tm.TopItems(dvTop).ToJson());
            }
            else if (filePath.Length > 9)
            {
                tm.FromString(new string[] { filePath });
                DXService.Log(tm.TopItems(dvTop).ToJson());
            }
            else
            {
                DXService.Log("内容太少了", ConsoleColor.Red);
            }
        }

        [Display(Name = "deep delete", Description = "深度删除匹配的文件（夹）", GroupName = "Tool",
            ShortName = "ddel [dir] [matchName] [ignoreDir] [-y]", Prompt = "ndx ddel ./ bin,*.md,file?.txt -y")]
        public static void DeepDelete()
        {
            var dir = DXService.VarString(0, $"根目录(default: {Environment.CurrentDirectory})");
            if (string.IsNullOrWhiteSpace(dir) || !Directory.Exists(dir))
            {
                dir = Environment.CurrentDirectory;
            }

            var matchName = DXService.VarString(1, "要删除的文件（夹），如 bin,*.md,file?.txt 逗号分割");
            var ignoreDir = DXService.VarString(2, "要忽略的目录名称，逗号分割");

            if (!string.IsNullOrWhiteSpace(matchName))
            {
                var isDelete = DXService.VarBool("真删除");

                var listSearch = matchName.Split(',').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

                var listIgnore = new List<string>();
                if (!string.IsNullOrWhiteSpace(ignoreDir))
                {
                    listIgnore = ignoreDir.Split(',').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
                }

                DXService.EachSearchRemove(new DirectoryInfo(dir), listSearch, listIgnore, isDelete);
            }
        }

        [Display(Name = "Git Pull", Description = "批量拉取", GroupName = "Tool",
            ShortName = "gitpull [dir]", Prompt = "ndx gitpull ./")]
        public static void GitPull()
        {
            var dir = DXService.VarString(0, "请输入目录");
            if (!Directory.Exists(dir))
            {
                dir = Environment.CurrentDirectory;
            }

            var di = new DirectoryInfo(dir);
            var sdis = di.GetDirectories().ToList();
            if (Directory.Exists(Path.Combine(di.FullName, ".git")))
            {
                sdis.Insert(0, di);
            }

            DXService.Log($"\r\n从 {dir} 目录找到 {sdis.Count} 个项目\r\n", ConsoleColor.Cyan);

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
    }
}
