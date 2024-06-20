using Microsoft.CodeAnalysis;
using System.IO.Compression;
using System.Xml;
using Xunit;

namespace Netnr.Test
{
    /// <summary>
    /// 发布项目
    /// </summary>
    public class Publish
    {
        /// <summary>
        /// 发布根目录
        /// </summary>
        public static string ReleaseRoot { get; set; } = @"E:\package\release";

        /// <summary>
        /// 批处理
        /// </summary>
        public static string BatPath { get; set; } = Path.Combine(Path.GetTempPath(), "_build.bat");

        /// <summary>
        /// 项目根目录
        /// </summary>
        public static string ProjectRoot { get; set; } = new DirectoryInfo(BaseTo.ProjectPath).Parent.FullName;

        /// <summary>
        /// 拷贝项目
        /// </summary>
        [Fact]
        public void ProjectCopy()
        {
            //源目录
            var fromSource = @"D:\site\npp";
            //新目录
            var toTarget = @"D:\site\np";
            //忽略文件夹
            var ignoreForder = "bin,obj,PublishProfiles,node_modules,packages,.git,.github,.svg,.vs,.config,.vercel,regexes";
            ignoreForder += ",Netnr.Admin.Web,Netnr.Admin.Domain,Netnr.Admin.Application,Netnr.Admin.XOpsClient,Netnr.Admin.Cqbn,Netnr.Cqbncrane,Netnr.Observer,ClientAdmin";

            //删除旧文件夹
            "docs,src".Split(',').ForEach(f =>
            {
                var df = Path.Combine(toTarget, f);
                if (Directory.Exists(df))
                {
                    Directory.Delete(df, true);
                }
            });

            FileTo.CopyDirectory(fromSource, toTarget, ignoreForder.Split(','));
            Debug.WriteLine("Copy completed!");

            //需要处理的项目名称
            var listEp = "Netnr.Blog.Web".ToLower().Split(",");

            var filesPath = Directory.EnumerateFiles(toTarget, "appsettings.json", SearchOption.AllDirectories);
            foreach (var filePath in filesPath)
            {
                if (!listEp.Any(x => filePath.Contains(x, StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }

                var jo = File.ReadAllText(filePath).DeJsonNode();
                ClearJsonObject(jo);

                FileTo.WriteText(jo.ToJson(true), filePath, false);
            }

            Debug.WriteLine("Done!");
        }

        [Fact]
        public void ProjectRelease()
        {
            string[] ProjectArray = [
                //"Netnr.ToolX",
                //"Netnr.DataX",

                //"Netnr.Admin.Web",
                //"Netnr.Admin.XOpsClient",
                //"Netnr.Admin.Cqbn",
                //"Netnr.Cqbncrane",
                //"Netnr.Observer",

                //"Netnr.Blog.Web",
                //"Netnr.ResponseFramework.Web",

                //"Netnr.DataKit",
                //"Netnr.FileServer",
                //"Netnr.Garnet",
                //"Netnr.Serve",

                //"Netnr.PaddleOCR",
                //"Netnr.PaddleSeg",

                //"Netnr.Demo"
            ];

            ProjectArray.ForEach(p =>
            {
                //项目环境版本
                var csprojFile = new DirectoryInfo(Path.Combine(ProjectRoot, p)).GetFiles("*.csproj", SearchOption.TopDirectoryOnly).First();
                var csprojXml = new XmlDocument();
                csprojXml.LoadXml(File.ReadAllText(csprojFile.FullName));
                var targetFramework = csprojXml.SelectSingleNode("Project/PropertyGroup/TargetFramework").InnerText.Replace("net", "");
                var targetVersion = Convert.ToInt32(targetFramework.Split('.')[0]);

                //发布环境版本
                var cr = CmdTo.Execute("dotnet --list-runtimes").CrOutput;
                var releaseVersion = cr.Split('\n').Where(line => line.StartsWith($"Microsoft.AspNetCore.App {targetVersion}."))
                .Select(x => new Version(x.Split(' ')[1].Trim())).Max().ToString();

                var option = new ReleaseOption
                {
                    ProjectDir = Path.Combine(ProjectRoot, p),
                    ProjectShortName = p,
                    PackageExclusion = [],
                    CompressLevel = ProjectArray.Length > 1 ? CompressionLevel.SmallestSize : CompressionLevel.NoCompression
                };
                option.DictProps["PublishTrimmed"] = "false";

                var win_x64Option = new ReleaseOption().ToDeepCopy(option);
                win_x64Option.Platform = "win-x64";
                win_x64Option.OutputDir = Path.Combine(ReleaseRoot, p, win_x64Option.Platform);
                win_x64Option.PackagePath = Path.Combine(ReleaseRoot, p, "assets", $"{p}-{releaseVersion}-{win_x64Option.Platform}.zip");

                var linux_x64Option = new ReleaseOption().ToDeepCopy(option);
                linux_x64Option.Platform = "linux-x64";
                linux_x64Option.OutputDir = Path.Combine(ReleaseRoot, p, linux_x64Option.Platform);
                linux_x64Option.PackagePath = Path.Combine(ReleaseRoot, p, "assets", $"{p}-{releaseVersion}-{linux_x64Option.Platform}.zip");

                var linux_arm64Option = new ReleaseOption().ToDeepCopy(option);
                linux_arm64Option.Platform = "linux-arm64";
                linux_arm64Option.OutputDir = Path.Combine(ReleaseRoot, p, linux_arm64Option.Platform);
                linux_arm64Option.PackagePath = Path.Combine(ReleaseRoot, p, "assets", $"{p}-{releaseVersion}-{linux_arm64Option.Platform}.zip");

                switch (p)
                {
                    case "Netnr.PaddleOCR":
                    case "Netnr.PaddleSeg":
                        {
                            win_x64Option.DictProps["PublishTrimmed"] = "false";

                            ReleaseOption.ReleasePackage(win_x64Option);
                        }
                        break;
                    case "Netnr.Demo":
                    case "Netnr.Blog.Web":
                    case "Netnr.ResponseFramework.Web":
                    case "Netnr.Admin.Web":
                        {
                            ReleaseOption.ReleasePackage(linux_x64Option);
                            ReleaseOption.ReleasePackage(win_x64Option);
                            ReleaseOption.ReleasePackage(linux_arm64Option);
                        }
                        break;
                    case "Netnr.Admin.Cqbn":
                        {
                            win_x64Option.DictProps["PublishTrimmed"] = "false";
                            win_x64Option.DictProps["ApplicationIcon"] = "./favicon-cqbn.ico";
                            win_x64Option.DictProps["Copyright"] = $"cqhg.com.cn {DateTime.Now:yyyy-MM-dd HH:mm:ss}";

                            ReleaseOption.ReleasePackage(win_x64Option);
                        }
                        break;
                    case "Netnr.Serve":
                    case "Netnr.Admin.XOpsClient":
                        {
                            //winOption.DictProps["PublishTrimmed"] = "true";
                            //linuxOption.DictProps["PublishTrimmed"] = "true";

                            //ReleaseOption.ReleasePackage(linuxOption);
                            //ReleaseOption.ReleasePackage(winOption);

                            // aot
                            if (targetVersion >= 8)
                            {
                                var aotWinOption = new ReleaseOption().ToDeepCopy(win_x64Option);
                                aotWinOption.DictProps["PublishAot"] = "true";
                                ReleaseOption.ReleasePackage(aotWinOption);

                                //var aotLinuxOption = new ReleaseOption().ToDeepCopy(linuxOption);
                                //aotLinuxOption.DictProps["PublishAot"] = "true";
                                //ReleaseOption.ReleasePackage(aotLinuxOption);
                            }
                        }
                        break;
                    case "Netnr.Observer":
                        {
                            var aotWinOption = new ReleaseOption().ToDeepCopy(win_x64Option);
                            aotWinOption.DictProps["PublishAot"] = "true";

                            ReleaseOption.ReleasePackage(aotWinOption);
                        }
                        break;
                    default:
                        {
                            win_x64Option.DictProps["PublishTrimmed"] = "true";
                            linux_x64Option.DictProps["PublishTrimmed"] = "true";
                            linux_arm64Option.DictProps["PublishTrimmed"] = "true";

                            ReleaseOption.ReleasePackage(linux_x64Option);
                            ReleaseOption.ReleasePackage(win_x64Option);
                            ReleaseOption.ReleasePackage(linux_arm64Option);
                        }
                        break;
                }
            });

            Debug.WriteLine("All done!");
        }

        [Fact]
        public void BuildSS()
        {
            var projectName = "Netnr.Blog.Web";
            var projectDir = Path.Combine(ProjectRoot, projectName);
            var launchSettings = File.ReadAllText(Path.Combine(projectDir, "Properties/launchSettings.json"));
            var uri = launchSettings.DeJson().GetProperty("profiles").GetProperty(projectName).GetValue("applicationUrl").Split(';').FirstOrDefault();

            //带参数 模式启动，用于授权
            Task.Run(() =>
            {
                var run = $"cd {projectDir} && dotnet run --urls {uri} --admin";
                var proc = CmdTo.BuildProcess(run);
                proc.OutputDataReceived += (sender, output) =>
                {
                    Debug.WriteLine(output.Data);
                };

                proc.Start();//启动线程
                proc.BeginOutputReadLine();//开始异步读取
                proc.WaitForExit();//阻塞等待进程结束
            });

            for (int i = 0; i < 9; i++)
            {
                Debug.WriteLine("等待服务启动 ...");
                Thread.Sleep(1000 * 3);

                var client = HttpTo.BuildClient();
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(uri),
                    Method = HttpMethod.Get
                };

                try
                {
                    var resp = client.Send(request);
                    if (resp.StatusCode == HttpStatusCode.OK)
                    {
                        Debug.WriteLine("服务已启动成功，准备执行生成");
                        Thread.Sleep(1000);

                        var result = client.GetStringAsync($"{uri}/ss/Build").ToResult();
                        Debug.WriteLine(result);
                        Assert.Equal(200, result.DeJson<ResultVM>().Code);

                        _ = client.GetAsync($"{uri}/ss/BuildDone");
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }

        /// <summary>
        /// 遍历清除值
        /// </summary>
        /// <param name="jo"></param>
        private static void ClearJsonObject(JsonNode node, JsonElement? ele = null)
        {
            if (!ele.HasValue)
            {
                ele = node.ToJson().DeJson();
            }

            node.AsObject().Select(x => x.Key).ToList().ForEach(key =>
            {
                var jv = ele.Value.GetProperty(key);
                switch (jv.ValueKind)
                {
                    case JsonValueKind.Object:
                        ClearJsonObject(node[key], jv);
                        break;
                    case JsonValueKind.Array:
                        ClearJsonArray(node[key], jv);
                        break;
                    case JsonValueKind.True:
                        node[key] = false;
                        break;
                    case JsonValueKind.String:
                        node[key] = "";
                        break;
                    case JsonValueKind.Number:
                        node[key] = 10;
                        break;
                }
            });
        }

        /// <summary>
        /// 遍历清除值
        /// </summary>
        /// <param name="ja"></param>
        private static void ClearJsonArray(JsonNode node, JsonElement? ele = null)
        {
            if (!ele.HasValue)
            {
                ele = node.ToJson().DeJson();
            }

            var nodeArr = node.AsArray();
            var eleArr = ele.Value.EnumerateArray();
            var ei = 0;
            foreach (var item in eleArr)
            {
                switch (item.ValueKind)
                {
                    case JsonValueKind.Object:
                        ClearJsonObject(nodeArr[ei], item);
                        break;
                    case JsonValueKind.Array:
                        ClearJsonArray(nodeArr[ei], item);
                        break;
                }
                ei++;
            }
        }
    }

    public class ReleaseOption
    {
        /// <summary>
        /// 项目目录
        /// </summary>
        public string ProjectDir { get; set; }
        /// <summary>
        /// 项目简称
        /// </summary>
        public string ProjectShortName { get; set; }
        /// <summary>
        /// 输出目录
        /// </summary>
        public string OutputDir { get; set; }
        /// <summary>
        /// 打包路径
        /// </summary>
        public string PackagePath { get; set; }
        /// <summary>
        /// 属性字典
        /// </summary>
        public Dictionary<string, string> DictProps { get; set; } = new Dictionary<string, string>
        {
            {"PublishSingleFile","true"},
            {"PublishTrimmed","true"},
            {"PublishAot","false"},

            {"ApplicationIcon","../Netnr.ClientApp/file/favicon.ico"},
            {"Authors", "netnr"},
            {"Copyright", $"netnr.com {DateTime.Now:yyyy-MM-dd HH:mm:ss}"},
            {"Version",BaseTo.Version},
            {"FileVersion",BaseTo.Version},
            {"AssemblyVersion",BaseTo.Version},
        };
        /// <summary>
        /// 是否包含运行时
        /// </summary>
        public string SelfContained { get; set; } = "true";
        /// <summary>
        /// 平台，win-64
        /// </summary>
        public string Platform { get; set; } = "linux-x64";
        /// <summary>
        /// 打包排除
        /// </summary>
        public List<string> PackageExclusion { get; set; } = ["appsettings.json"];
        /// <summary>
        /// 打包压缩等级
        /// </summary>
        public CompressionLevel CompressLevel { get; set; } = CompressionLevel.SmallestSize;

        /// <summary>
        /// 发布打包
        /// </summary>
        /// <param name="model"></param>
        public static void ReleasePackage(ReleaseOption model)
        {
            Debug.WriteLine($"\r\n===== Start publishing {model.ProjectShortName}");
            var csproj = new DirectoryInfo(model.ProjectDir).EnumerateFiles("*.csproj", SearchOption.TopDirectoryOnly).First();
            var cddir = Path.GetDirectoryName(csproj.FullName);

            var listProps = new List<string>();
            var isAOT = model.DictProps.TryGetValue("PublishAot", out string PublishAot) && PublishAot == "true";
            var aotIgnore = "PublishSingleFile,PublishTrimmed".Split(',');
            foreach (var kv in model.DictProps)
            {
                if (isAOT && aotIgnore.Contains(kv.Key))
                {
                    continue;
                }
                listProps.Add($"-p:{kv.Key}={kv.Value.ToUrlEncode()}");
            }

            var listRun = new List<string>
            {
                $"cd {cddir} &&",
                $"dotnet publish",
                $"{csproj.Name}",
                $"-c Release",
                $"--self-contained {model.SelfContained}",
                $"-r {model.Platform}",
                $"-o {model.OutputDir}"
            };
            listRun.AddRange(listProps);
            var run = string.Join(" ", listRun);

            Debug.WriteLine($"{run}\r\n重建输出目录 {model.OutputDir}");
            FileTo.ClearDirectory(model.OutputDir);

            var proc = CmdTo.BuildProcess(run);
            proc.OutputDataReceived += (sender, output) =>
            {
                if (output.Data != null)
                {
                    Debug.WriteLine(output.Data);
                }
            };

            proc.Start();//启动线程

            proc.BeginOutputReadLine();//开始异步读取
            proc.WaitForExit();//阻塞等待进程结束

            proc.Close();//关闭进程

            Debug.WriteLine($"===== Published {model.ProjectShortName}");

            //创建资产目录
            var ppdir = new DirectoryInfo(Path.GetDirectoryName(model.PackagePath));
            if (!ppdir.Exists)
            {
                ppdir.Create();
            }

            if (isAOT)
            {
                if (model.Platform.Contains("win"))
                {
                    var winAotName = $"{model.ProjectShortName}.exe";

                    File.Copy(Path.Combine(model.OutputDir, winAotName), Path.Combine(Path.GetDirectoryName(model.PackagePath), winAotName.Replace(".exe", $"-{BaseTo.Version}.exe")), true);
                }
            }
            else if (!string.IsNullOrWhiteSpace(model.PackagePath))
            {
                Debug.WriteLine($"压缩打包 {model.PackagePath}");

                if (File.Exists(model.PackagePath))
                {
                    File.Delete(model.PackagePath);
                }
                ZipFile.CreateFromDirectory(model.OutputDir, model.PackagePath, model.CompressLevel, false);

                if (model.PackageExclusion.Count > 0)
                {
                    Debug.WriteLine($"压缩打包排除 {model.PackageExclusion.Count}");
                    using var zip = ZipFile.Open(model.PackagePath, ZipArchiveMode.Update);
                    for (int i = 0; i < zip.Entries.Count; i++)
                    {
                        var entry = zip.Entries[i];
                        if (model.PackageExclusion.Contains(entry.Name))
                        {
                            entry.Delete();
                        }
                    }
                }

                Debug.WriteLine("===== Packaging completed !!!");
            }
        }
    }
}
