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
        /// 项目根目录
        /// </summary>
        public static string ProjectRoot { get; set; } = new DirectoryInfo(ReadyTo.ProjectRootPath).Parent.FullName;

        [Fact]
        public void ReleaseAll()
        {
            NetnrDataKit_Client_Online();

            NetnrBlogBuildSS();
            NetnrBlog();

            NetnrResponseFramework();

            NetnrFileServer();

            NetnrDataKit();

            NetnrDataX();

            NetnrServe();

            NetnrDataKit_Client_Online();
        }

        [Fact]
        public void NetnrBlogBuildSS()
        {
            var projectName = "Netnr.Blog.Web";
            var projectDir = Path.Combine(ProjectRoot, projectName);
            var launchSettings = File.ReadAllText(Path.Combine(projectDir, "Properties/launchSettings.json"));
            var uri = launchSettings.DeJson().GetProperty("profiles").GetProperty(projectName).GetValue("applicationUrl").Split(';').FirstOrDefault(x => x.StartsWith("https"));

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
                                process.Kill();
                            }
                        }
                    };

                    process.Start();//启动线程
                    process.BeginOutputReadLine();//开始异步读取
                    process.WaitForExit();//阻塞等待进程结束
                    process.Close();//关闭进程
                    process.Dispose();
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
                        Thread.Sleep(3000);
                        var bresult = HttpTo.Get($"{uri}/ss/build");
                        Debug.WriteLine(bresult);
                        Assert.True(bresult.DeJson<ResultVM>().Code == 200);

                        break;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
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
            var versionContent = File.ReadAllText(Path.Combine(ProjectRoot, projectName, "appsettings.json"));
            var version = versionContent.DeJson().GetValue("Version");

            var shortName = "nfs";
            var platform = "win-x64";
            var zipFile = $"{shortName}-{version}-{platform}.zip";
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
            zipFile = $"{shortName}-{version}-{platform}.zip";
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
        public void NetnrDataKit_Client_Online()
        {
            var projectName = "Netnr.DataKit";
            var projectDir = Path.Combine(ProjectRoot, projectName);
            var clientDir = new DirectoryInfo(projectDir).GetDirectories("*Client*").First().FullName;
            var wwwroot = Path.Combine(projectDir, "wwwroot");
            var blogdk = Path.Combine(ProjectRoot, "Netnr.Blog.Web/wwwroot/app/dk");
            var batPath = Path.Combine(Path.GetTempPath(), "_build.bat");

            Debug.WriteLine("构建在线资源版到 Netnr.Blog.Web/wwwroot/app/dk");
            File.WriteAllText(batPath, $"npm run prod_online --prefix {clientDir} && exit");
            Debug.WriteLine(CmdTo.Execute($"start {batPath}").CrOutput);
            if (Directory.Exists(blogdk))
            {
                Directory.Delete(blogdk, true);
                FileTo.CopyDirectory(wwwroot, blogdk);
            }
        }

        [Fact]
        public void NetnrDataKit()
        {
            var projectName = "Netnr.DataKit";
            var shortName = "ndk";
            var projectDir = Path.Combine(ProjectRoot, projectName);
            var clientDir = new DirectoryInfo(projectDir).GetDirectories("*Client*").First().FullName;
            var versionContent = File.ReadAllText(Path.Combine(clientDir, "src/js/ndkVary.js"));
            var version = versionContent.Split("\r\n").ToList().FirstOrDefault(x => x.Contains("version:")).Split('"')[1];

            //npm build
            Debug.WriteLine("构建本地资源版");
            var batPath = Path.Combine(Path.GetTempPath(), "_build.bat");
            File.WriteAllText(batPath, $"npm run prod --prefix {clientDir} && exit");
            Debug.WriteLine(CmdTo.Execute($"start {batPath}").CrOutput);

            var platform = "linux-x64";
            var zipFile = $"{shortName}-{version}-{platform}.zip";
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
            zipFile = $"{shortName}-{version}-{platform}.zip";
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

            var projectDir = Path.Combine(ProjectRoot, projectName);
            var versionContent = File.ReadAllText(Path.Combine(projectDir, "Domain/ConfigInit.cs"));
            var version = versionContent.Split("\r\n").ToList().FirstOrDefault(x => x.Contains(" Version ")).Split('"')[1];

            var platform = "win-x64";
            var zipFile = $"{shortName}-{version}-{platform}.zip";
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
            zipFile = $"{shortName}-{version}-{platform}.zip";
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
            var versionContent = File.ReadAllText(Path.Combine(ProjectRoot, "Netnr.Shared/Serve/ServeTo.cs"));
            var version = versionContent.Split("\r\n").ToList().FirstOrDefault(x => x.Contains(" Version ")).Split('"')[1];

            var platform = "win-x64";
            var zipFile = $"{shortName}-{version}-{platform}.zip";
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
            zipFile = $"{shortName}-{version}-{platform}.zip";
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
            var model = new ReleaseService
            {
                ProjectDir = Path.Combine(ProjectRoot, projectName),
                ProjectShortName = shortName,
                OutputDir = Path.Combine(ReleaseRoot, shortName, platform),
                PackagePath = Path.Combine(ReleaseRoot, shortName, zipFile),
                Platform = platform
            };
            ReleaseService.ReleasePackage(model);
        }
    }

    public class ReleaseService
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
        public List<string> PackageExclusion = new() { "appsettings.json" };

        /// <summary>
        /// 发布打包
        /// </summary>
        /// <param name="model"></param>
        public static void ReleasePackage(ReleaseService model)
        {
            Debug.WriteLine($"Release {model.ProjectShortName}");
            var csproj = new DirectoryInfo(model.ProjectDir).GetFiles("*.csproj", SearchOption.TopDirectoryOnly).First();

            var cddir = Path.GetDirectoryName(csproj.FullName);
            var run = $"cd {cddir} && dotnet publish {csproj.Name} -p:PublishSingleFile={model.PublishSingleFile} -p:PublishTrimmed={model.PublishTrimmed} -c Release -r {model.Platform} --self-contained {model.SelfContained} -o {model.OutputDir}";
            Debug.WriteLine(run);

            Debug.WriteLine($"重建输出目录 {model.OutputDir}");
            if (Directory.Exists(model.OutputDir))
            {
                Directory.Delete(model.OutputDir, true);
            }
            Directory.CreateDirectory(model.OutputDir);

            var cer = CmdTo.Execute(run);

            Debug.WriteLine(cer.CrOutput);
            Debug.WriteLine(cer.CrError);

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
