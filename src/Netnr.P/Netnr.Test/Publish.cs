using System.IO.Compression;
using System.Net;
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
        public static string ReleaseRoot { get; set; } = @"D:\tmp\release";

        /// <summary>
        /// 批处理
        /// </summary>
        public static string BatPath { get; set; } = Path.Combine(Path.GetTempPath(), "_build.bat");

        /// <summary>
        /// 项目根目录
        /// </summary>
        public static string ProjectRoot { get; set; } = new DirectoryInfo(BaseTo.ProjectRootPath).Parent.FullName;

        /// <summary>
        /// 拷贝项目
        /// </summary>
        [Fact]
        public void NetnrProjectCopy()
        {
            //源目录
            var sourcePath = @"D:\site\npp";
            //目标目录
            var targetPath = @"D:\site\np";
            //忽略文件夹
            var ignoreForder = "bin,obj,PublishProfiles,node_modules,packages,.git,.svg,.vs,.config,.vercel,regexes,Netnr.Admin.Web,Netnr.Admin.Domain,Netnr.Admin.Application,Netnr.Admin.XOps,ClientAdmin,Netnr.SMS";

            //删除旧文件夹
            var docsFolder = Path.Combine(targetPath, "docs");
            if (Directory.Exists(docsFolder))
            {
                Directory.Delete(docsFolder, true);
            }
            var srcFolder = Path.Combine(targetPath, "src");
            if (Directory.Exists(srcFolder))
            {
                Directory.Delete(srcFolder, true);
            }

            FileTo.CopyDirectory(sourcePath, targetPath, ignoreForder.Split(','));
            Debug.WriteLine("Copy completed!");

            //需要处理的项目名称
            var listEp = "Netnr.Blog.Web".ToLower().Split(",").ToList();

            var filesPath = Directory.GetFiles(targetPath, "appsettings.json", SearchOption.AllDirectories);
            for (int i = 0; i < filesPath.Length; i++)
            {
                var filePath = filesPath[i];
                if (!listEp.Any(x => filePath.ToLower().Contains(x)))
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
        public void ReleaseAll()
        {
            NetnrDataX();
            NetnrServe();
            NetnrFileServer();

            NetnrDataKit();

            NetnrBlogBuildSS();
            NetnrBlog();

            NetnrResponseFramework();
        }

        [Fact]
        public void NetnrBlogBuildSS()
        {
            var projectName = "Netnr.Blog.Web";
            var projectDir = Path.Combine(ProjectRoot, projectName);
            var launchSettings = File.ReadAllText(Path.Combine(projectDir, "Properties/launchSettings.json"));
            var uri = launchSettings.DeJson().GetProperty("profiles").GetProperty(projectName).GetValue("applicationUrl").Split(';').FirstOrDefault();

            //带参数 模式启动，用于授权
            Task.Run(() =>
            {
                CmdTo.Execute(CmdTo.PSInfo($"cd {projectDir} && dotnet run --urls {uri} --admin"), (process, cr) =>
                {
                    process.OutputDataReceived += (sender, output) =>
                    {
                        if (output.Data != null)
                        {
                            Debug.WriteLine(output.Data);
                            if (output.Data == "Done!")
                            {
                                process.Close();//关闭进程
                                process.Dispose();
                            }
                        }
                    };

                    process.Start();//启动线程
                    process.BeginOutputReadLine();//开始异步读取
                    process.WaitForExit();//阻塞等待进程结束

                    //process.Close();//关闭进程
                    //process.Dispose();
                });
            });

            for (int i = 0; i < 9; i++)
            {
                Debug.WriteLine("等待服务启动 ...");
                Thread.Sleep(1000 * 3);

                var client = new HttpClient();
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
                        var bresult = HttpTo.Get($"{uri}/ss/build");
                        Debug.WriteLine(bresult);
                        Assert.True(bresult.DeJson<ResultVM>().Code == 200);

                        break;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }

            Thread.Sleep(2000);
            CmdTo.KillProcess(new int[] { Convert.ToInt32(uri.Split(':').Last()) });
        }

        [Fact]
        public void NetnrBlog()
        {
            Release("Netnr.Blog.Web", "blog");
        }

        [Fact]
        public void NetnrResponseFramework()
        {
            Release("Netnr.ResponseFramework.Web", "nrf");
        }

        [Fact]
        public void NetnrFileServer()
        {
            var projectName = "Netnr.FileServer";

            var shortName = "nfs";
            var platform = "win-x64";
            var zipFile = $"{shortName}-{BaseTo.Version}-{platform}.zip";
            var zipPath = Path.Combine(ReleaseRoot, shortName, zipFile);

            Release(projectName, shortName, platform, zipFile);
            if (File.Exists(zipPath))
            {
                using var zip = ZipFile.Open(zipPath, ZipArchiveMode.Update);

                var stream1 = zip.CreateEntry("start.bat", CompressionLevel.SmallestSize).Open();
                stream1.Write($"start {projectName}.exe --urls http://+:9998".ToByte());
                stream1.Close();
            }

            platform = "linux-x64";
            zipFile = $"{shortName}-{BaseTo.Version}-{platform}.zip";
            zipPath = Path.Combine(ReleaseRoot, shortName, zipFile);

            Release(projectName, shortName, platform, zipFile);
            if (File.Exists(zipPath))
            {
                using var zip = ZipFile.Open(zipPath, ZipArchiveMode.Update);

                var stream1 = zip.CreateEntry("start.sh", CompressionLevel.SmallestSize).Open();
                stream1.Write($"chmod +x {projectName}\r\nnohup ./{projectName} --urls http://+:9998 &".ToByte());
                stream1.Close();

                var stream2 = zip.CreateEntry("stop.sh", CompressionLevel.SmallestSize).Open();
                stream2.Write("kill -9 $(netstat -nlp | grep :9998 | awk '{print $7}' | awk -F\"/\" '{ print $1 }') ".ToByte());
                stream2.Close();
            }
        }

        [Fact]
        public void NetnrDataKit()
        {
            var projectName = "Netnr.DataKit";
            var shortName = "ndk";

            var platform = "linux-x64";
            var zipFile = $"{shortName}-{BaseTo.Version}-{platform}.zip";
            var zipPath = Path.Combine(ReleaseRoot, shortName, zipFile);
            Release(projectName, shortName, platform, zipFile);
            if (File.Exists(zipPath))
            {
                using var zip = ZipFile.Open(zipPath, ZipArchiveMode.Update);

                var stream1 = zip.CreateEntry("start.sh", CompressionLevel.SmallestSize).Open();
                stream1.Write($"chmod +x {projectName}\r\nnohup ./{projectName} --urls http://+:9999 &".ToByte());
                stream1.Close();

                var stream2 = zip.CreateEntry("stop.sh", CompressionLevel.SmallestSize).Open();
                stream2.Write("kill -9 $(netstat -nlp | grep :9999 | awk '{print $7}' | awk -F\"/\" '{ print $1 }') ".ToByte());
                stream2.Close();
            }

            platform = "win-x64";
            zipFile = $"{shortName}-{BaseTo.Version}-{platform}.zip";
            zipPath = Path.Combine(ReleaseRoot, shortName, zipFile);
            Release(projectName, shortName, platform, zipFile);
            if (File.Exists(zipPath))
            {
                using var zip = ZipFile.Open(zipPath, ZipArchiveMode.Update);

                var stream1 = zip.CreateEntry("start.bat", CompressionLevel.SmallestSize).Open();
                stream1.Write($"start {projectName}.exe --urls http://+:9999".ToByte());
                stream1.Close();
            }
        }

        [Fact]
        public void NetnrDataX()
        {
            var projectName = "Netnr.DataX";
            var shortName = "ndx";

            var platform = "win-x64";
            var zipFile = $"{shortName}-{BaseTo.Version}-{platform}.zip";
            var zipPath = Path.Combine(ReleaseRoot, shortName, zipFile);

            Release(projectName, shortName, platform, zipFile);
            if (File.Exists(zipPath))
            {
                using var zip = ZipFile.Open(zipPath, ZipArchiveMode.Update);
                for (int i = zip.Entries.Count - 1; i >= 0; i--)
                {
                    var entry = zip.Entries[i];
                    if (entry.Name.EndsWith(".pdb") || entry.Name.EndsWith(".xml"))
                    {
                        entry.Delete();
                    }
                    else if (entry.Name == $"{projectName}.exe")
                    {
                        var newEntry = zip.CreateEntry($"{shortName}.exe", CompressionLevel.SmallestSize);

                        using var oldStream = entry.Open();
                        using var newStream = newEntry.Open();
                        oldStream.CopyTo(newStream);
                        oldStream.Close();
                        entry.Delete();
                    }
                    else if (entry.Name == "config.json")
                    {
                        using var stream = entry.Open();
                        var reader = new StreamReader(stream);
                        var content = reader.ReadToEnd().Replace("CQSME", "NETNR").Replace("EE.Oracle.Docker", "orcl");
                        var buffer = content.ToByte();
                        stream.SetLength(0);
                        stream.Write(buffer, 0, buffer.Length);
                        stream.Dispose();
                    }
                }
            }

            platform = "linux-x64";
            zipFile = $"{shortName}-{BaseTo.Version}-{platform}.zip";
            zipPath = Path.Combine(ReleaseRoot, shortName, zipFile);

            Release(projectName, shortName, platform, zipFile);
            if (File.Exists(zipPath))
            {
                using var zip = ZipFile.Open(zipPath, ZipArchiveMode.Update);
                for (int i = zip.Entries.Count - 1; i >= 0; i--)
                {
                    var entry = zip.Entries[i];
                    if (entry.Name.EndsWith(".pdb") || entry.Name.EndsWith(".xml"))
                    {
                        entry.Delete();
                    }
                    else if (entry.Name == projectName)
                    {
                        var newEntry = zip.CreateEntry($"{shortName}", CompressionLevel.SmallestSize);

                        using var oldStream = entry.Open();
                        using var newStream = newEntry.Open();
                        oldStream.CopyTo(newStream);
                        oldStream.Close();
                        entry.Delete();
                    }
                    else if (entry.Name == "config.json")
                    {
                        using var stream = entry.Open();
                        var reader = new StreamReader(stream);
                        var content = reader.ReadToEnd().Replace("CQSME", "NETNR").Replace("EE.Oracle.Docker", "orcl").Replace(@"D:\\tmp\\res\\tmp.db", "/tmp/tmp.db");
                        var buffer = content.ToByte();
                        stream.SetLength(0);
                        stream.Write(buffer, 0, buffer.Length);
                        stream.Dispose();
                    }
                }
            }
        }

        [Fact]
        public void NetnrServe()
        {
            var projectName = "Netnr.Serve";
            var shortName = "ns";

            var platform = "win-x64";
            var zipFile = $"{shortName}-{BaseTo.Version}-{platform}.zip";
            var zipPath = Path.Combine(ReleaseRoot, shortName, zipFile);

            Release(projectName, shortName, platform, zipFile);
            if (File.Exists(zipPath))
            {
                using var zip = ZipFile.Open(zipPath, ZipArchiveMode.Update);
                for (int i = zip.Entries.Count - 1; i >= 0; i--)
                {
                    var entry = zip.Entries[i];
                    if (entry.Name.EndsWith(".pdb") || entry.Name.EndsWith(".xml"))
                    {
                        entry.Delete();
                    }
                    else if (entry.Name == $"{projectName}.exe")
                    {
                        var newEntry = zip.CreateEntry($"{shortName}.exe", CompressionLevel.SmallestSize);

                        using var oldStream = entry.Open();
                        using var newStream = newEntry.Open();
                        oldStream.CopyTo(newStream);
                        oldStream.Close();
                        entry.Delete();
                    }
                }
            }

            platform = "linux-x64";
            zipFile = $"{shortName}-{BaseTo.Version}-{platform}.zip";
            zipPath = Path.Combine(ReleaseRoot, shortName, zipFile);

            Release(projectName, shortName, platform, zipFile);
            if (File.Exists(zipPath))
            {
                using var zip = ZipFile.Open(zipPath, ZipArchiveMode.Update);
                for (int i = zip.Entries.Count - 1; i >= 0; i--)
                {
                    var entry = zip.Entries[i];
                    if (entry.Name.EndsWith(".pdb") || entry.Name.EndsWith(".xml"))
                    {
                        entry.Delete();
                    }
                    else if (entry.Name == projectName)
                    {
                        var newEntry = zip.CreateEntry($"{shortName}", CompressionLevel.SmallestSize);

                        using var oldStream = entry.Open();
                        using var newStream = newEntry.Open();
                        oldStream.CopyTo(newStream);
                        oldStream.Close();
                        entry.Delete();
                    }
                }
            }
        }

        /// <summary>
        /// 发布 WEB
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="shortName"></param>
        private static void Release(string projectName, string shortName, string platform = "linux-x64", string zipFile = "publish.zip")
        {
            var model = new ReleaseOption
            {
                ProjectDir = Path.Combine(ProjectRoot, projectName),
                ProjectShortName = shortName,
                OutputDir = Path.Combine(ReleaseRoot, shortName, platform),
                PackagePath = Path.Combine(ReleaseRoot, shortName, zipFile),
                Platform = platform
            };
            ReleaseOption.ReleasePackage(model);
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
        /// 是否单文件
        /// </summary>
        public string PublishSingleFile { get; set; } = "true";
        /// <summary>
        /// 是否剪裁
        /// </summary>
        public string PublishTrimmed { get; set; } = "true";
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
        public List<string> PackageExclusion { get; set; } = new() { "appsettings.json" };

        /// <summary>
        /// 发布打包
        /// </summary>
        /// <param name="model"></param>
        public static void ReleasePackage(ReleaseOption model)
        {
            Debug.WriteLine($"Release {model.ProjectShortName}");
            var csproj = new DirectoryInfo(model.ProjectDir).GetFiles("*.csproj", SearchOption.TopDirectoryOnly).First();

            var cddir = Path.GetDirectoryName(csproj.FullName);
            var run = $"cd {cddir} && dotnet publish {csproj.Name} -p:PublishSingleFile={model.PublishSingleFile} -p:PublishTrimmed={model.PublishTrimmed} -c Release -r {model.Platform} --self-contained {model.SelfContained} -o {model.OutputDir}";

            Debug.WriteLine($"重建输出目录 {model.OutputDir}");
            if (Directory.Exists(model.OutputDir))
            {
                Directory.Delete(model.OutputDir, true);
            }
            Directory.CreateDirectory(model.OutputDir);

            File.WriteAllText(Publish.BatPath, $"{run} && exit");
            CmdTo.Execute($"start {Publish.BatPath}");

            Debug.WriteLine($"打包文件 {model.PackagePath}");
            if (File.Exists(model.PackagePath))
            {
                File.Delete(model.PackagePath);
            }
            ZipFile.CreateFromDirectory(model.OutputDir, model.PackagePath, CompressionLevel.SmallestSize, false);

            if (model.PackageExclusion.Count > 0)
            {
                Debug.WriteLine($"打包排除 {model.PackageExclusion.Count}");
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

            Debug.WriteLine("打包完成!");
        }
    }
}
