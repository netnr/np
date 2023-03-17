using System.Net.Http;
using System.Text.RegularExpressions;

namespace Netnr.DataX.Menu;

/// <summary>
/// 混合
/// </summary>
public partial class MenuMixService
{
    [Display(Name = "Back to main menu", Description = "返回主菜单", GroupName = "\r\n")]
    public static void BackToMainMenu() => DXService.InvokeMenu(typeof(MenuMainService));

    [Display(Name = "View version", Description = "查看版本")]
    public static void ViewVersion() => DXService.Log(ConfigInit.Version);

    [Display(Name = "Check for updates", Description = "检查更新")]
    public static void CheckForUpdates()
    {
        var curi = "https://gitee.com/api/v5/repos/netnr/np/releases/latest";
        var puri = "https://gitee.com/netnr/np/releases";
        try
        {
            DXService.Log($"connect to the server: {curi}");

            //ndx-0.2.0-win-x64.zip
            var osPlatform = GlobalTo.IsWindows ? "win-x64" : "linux-x64";
            string pattern = @$"ndx-(\d+.\d+.\d+)-{osPlatform}.zip";

            var hc = new HttpClient();
            hc.DefaultRequestHeaders.UserAgent.TryParseAdd("Netnr");
            var result = hc.GetStringAsync(curi).ToResult();
            var assets = result.DeJson().GetProperty("assets").EnumerateArray();
            var isFound = false;

            foreach (var item in assets)
            {
                var name = item.GetValue("name");

                var match = Regex.Match(name, pattern);
                if (match.Success)
                {
                    isFound = true;
                    var latestVersion = match.Groups[1].ToString();
                    var vc = new Version(latestVersion).CompareTo(new Version(ConfigInit.Version));
                    if (vc > 0)
                    {
                        DXService.Log($"new version {ConfigInit.Version} => {latestVersion}", ConsoleColor.Green);

                        var durl = item.GetValue("browser_download_url");
                        DXService.Log($"downloading {durl}");

                        var saveZip = Path.Combine(ReadyTo.ProjectRootPath, name);
                        HttpTo.DownloadSave(durl, saveZip);

                        DXService.Log($"Save to {saveZip}");
                        DXService.Log($"Downloaded, please unzip it manually");
                    }
                    else
                    {
                        DXService.Log($"already the latest version v{ConfigInit.Version}", ConsoleColor.Cyan);
                    }
                    break;
                }
            }
            if (!isFound)
            {
                throw new Exception("No release found");
            }
        }
        catch (Exception ex)
        {
            DXService.Log($"Not supported, please visit {puri}", ConsoleColor.Red);
            DXService.Log(ex);
        }
    }

    [Display(Name = "Console encoding", Description = "控制台编码")]
    public static void ConsoleEncoding()
    {
        DXService.Log($"Current: {Console.OutputEncoding.EncodingName}");

        //选择编码
        var cri2 = DXService.ConsoleReadItem("Choose an encoding", "Unicode,UTF-8".Split(','), 1);
        switch (cri2)
        {
            case 1: Console.OutputEncoding = Encoding.Unicode; break;
            case 2: Console.OutputEncoding = Encoding.UTF8; break;
        }
    }

    [Display(Name = "GC", Description = "清理", GroupName = "\r\n")]
    public static void GarbageCleanup()
    {
        DXService.Log($"Use Physical Memory: {ParsingTo.FormatByteSize(Environment.WorkingSet)}");
        GC.Collect();
        Thread.Sleep(2000);
        DXService.Log($"Use Physical Memory: {ParsingTo.FormatByteSize(Environment.WorkingSet)}");
    }

    [Display(Name = "Open the hub directory", Description = "打开 hub 目录")]
    public static void OpenTheHubDirectory()
    {
        //配置
        var ci = new ConfigInit();

        if (GlobalTo.IsWindows)
        {
            CmdTo.Execute($"start {ci.DXHub}");
        }
        else
        {
            var cr = CmdTo.Execute($"cd {ci.DXHub} && pwd && ls -lh");
            DXService.Log(cr.CrOutput);
        }
    }

    [Display(Name = "Set environment variables", Description = "设置环境变量")]
    public static void SetEnvironmentVariables()
    {
        var processPath = Path.GetDirectoryName(Environment.ProcessPath);
        if (DXService.ConsoleReadBool($"配置环境变量({processPath})"))
        {
            if (GlobalTo.IsWindows)
            {
                var listPath = Environment.GetEnvironmentVariable("PATH").Split(Path.PathSeparator).ToList();
                listPath.Add(processPath);
                Environment.SetEnvironmentVariable("PATH", string.Join(Path.PathSeparator, listPath), EnvironmentVariableTarget.User);
            }
            else
            {
                var bashrcPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".bashrc");
                Console.WriteLine(bashrcPath);
                var contents = File.ReadAllLines(bashrcPath);
                var newPath = $"export PATH=$PATH:{processPath}";
                if (!contents.Contains(newPath))
                {
                    File.WriteAllLines(bashrcPath, contents.Add(newPath)); //添加到最后
                    CmdTo.Execute($"source {bashrcPath}"); //生效
                }
            }

            DXService.Log("Done!");
        }
    }
}