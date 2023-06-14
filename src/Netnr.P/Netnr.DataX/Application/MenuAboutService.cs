using System.Net.Http;
using System.Text.RegularExpressions;

namespace Netnr.DataX.Application;

/// <summary>
/// About
/// </summary>
public partial class MenuItemService
{
    [Display(Name = "Exit", Description = "退出", GroupName = "About", AutoGenerateFilter = true)]
    public static void Exit() => Environment.Exit(0);

    [Display(Name = "View version", Description = "查看版本", GroupName = "About",
        ShortName = "version", Prompt = "ndx version")]
    public static void ViewVersion() => DXService.Log(BaseTo.Version);

    [Display(Name = "Check for updates", Description = "检查更新", GroupName = "About")]
    public static async Task CheckForUpdates()
    {
        var curi = "https://gitee.com/api/v5/repos/netnr/np/releases/latest";
        var puri = "https://gitee.com/netnr/np/releases";
        try
        {
            DXService.Log($"connect to the server: {curi}");

            //ndx-0.2.0-win-x64.zip
            var osPlatform = CmdTo.IsWindows ? "win-x64" : "linux-x64";
            string pattern = @$"ndx-(\d+.\d+.\d+)-{osPlatform}.zip";

            var hc = new HttpClient();
            hc.DefaultRequestHeaders.UserAgent.TryParseAdd("Netnr");
            var result = await hc.GetStringAsync(curi);
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
                    var vc = new Version(latestVersion).CompareTo(new Version(BaseTo.Version));
                    if (vc > 0)
                    {
                        DXService.Log($"new version {BaseTo.Version} => {latestVersion}", ConsoleColor.Green);

                        var durl = item.GetValue("browser_download_url");
                        DXService.Log($"downloading {durl}");

                        var saveZip = Path.Combine(BaseTo.ProjectRootPath, name);
                        var client = new HttpClient
                        {
                            Timeout = TimeSpan.FromMinutes(5)
                        };
                        await client.DownloadAsync(durl, saveZip);

                        DXService.Log($"Save to {saveZip}");
                        DXService.Log($"Downloaded, please unzip it manually");
                    }
                    else
                    {
                        DXService.Log($"already the latest version v{BaseTo.Version}", ConsoleColor.Cyan);
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

    [Display(Name = "Console encoding", Description = "控制台编码", GroupName = "About")]
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

    [Display(Name = "GC", Description = "清理", GroupName = "About", AutoGenerateFilter = true)]
    public static void GarbageCleanup()
    {
        DXService.Log($"Use Physical Memory: {ParsingTo.FormatByteSize(Environment.WorkingSet)}");
        GC.Collect();
        Thread.Sleep(200);
        GC.Collect();
        DXService.Log($"Use Physical Memory: {ParsingTo.FormatByteSize(Environment.WorkingSet)}");
    }

    [Display(Name = "Open the hub directory", Description = "打开 hub 目录", GroupName = "About",
        ShortName = "hub", Prompt = "ndx hub")]
    public static void OpenTheHubDirectory()
    {
        //配置
        var ci = new ConfigInit();

        if (CmdTo.IsWindows)
        {
            CmdTo.Execute($"start {ci.DXHub}");
        }
        else
        {
            var cr = CmdTo.Execute($"cd {ci.DXHub} && pwd && ls -lh");
            DXService.Log(cr.CrOutput);
        }
    }
}