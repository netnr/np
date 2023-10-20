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
    public static void ViewVersion() => ConsoleTo.LogColor(BaseTo.Version);

    [Display(Name = "Check for update", Description = "检查更新", GroupName = "About")]
    public static async Task CheckForUpdate()
    {
        var curi = "https://gitee.com/api/v5/repos/netnr/np/releases/latest";
        var puri = "https://gitee.com/netnr/np/releases";
        try
        {
            ConsoleTo.LogColor($"connect to the server: {curi}");

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
                    if (vc <= 0)
                    {
                        ConsoleTo.LogColor($"new version {BaseTo.Version} => {latestVersion}", ConsoleColor.Green);

                        var durl = item.GetValue("browser_download_url");
                        ConsoleTo.LogColor($"downloading {durl}");

                        var saveZip = Path.Combine(BaseTo.ProjectRootPath, name);
                        var client = new HttpClient
                        {
                            Timeout = TimeSpan.FromMinutes(5)
                        };

                        var st = Stopwatch.StartNew();
                        await client.DownloadAsync(durl, saveZip, (receive, total) =>
                        {
                            if (st.ElapsedMilliseconds > 500)
                            {
                                if (total.HasValue)
                                {
                                    ConsoleTo.LogColor($"{receive * 1m / total * 100:0.00}% downloaded, {ParsingTo.FormatByte(receive)}/{ParsingTo.FormatByte(Convert.ToInt64(total))}");
                                }
                                else
                                {
                                    ConsoleTo.LogColor($"{ParsingTo.FormatByte(receive)} downloaded");
                                }

                                st.Restart();
                            }
                        });

                        ConsoleTo.LogColor($"Save to {saveZip}", ConsoleColor.Green);
                        ConsoleTo.LogColor($"Downloaded, please unzip it manually", ConsoleColor.Cyan);
                    }
                    else
                    {
                        ConsoleTo.LogColor($"already the latest version {BaseTo.Version}>={latestVersion}", ConsoleColor.Cyan);
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
            ConsoleTo.LogColor($"Check for updates failed, please visit {puri}", ConsoleColor.Red);
            ConsoleTo.LogError(ex);
        }
    }

    [Display(Name = "GC", Description = "清理", GroupName = "About")]
    public static async Task GarbageCleanup()
    {
        ConsoleTo.LogColor($"Use Physical Memory: {ParsingTo.FormatByte(Environment.WorkingSet)}");
        GC.Collect();
        await Task.Delay(1000);
        GC.Collect();
        ConsoleTo.LogColor($"Use Physical Memory: {ParsingTo.FormatByte(Environment.WorkingSet)}");
    }

    [Display(Name = "Console encoding", Description = "控制台编码", GroupName = "About")]
    public static void ConsoleEncoding()
    {
        ConsoleTo.LogColor($"Current: {Console.OutputEncoding.EncodingName}");

        //选择编码
        var cri2 = DXService.ConsoleReadItem("Choose an encoding", "Unicode,UTF-8".Split(','), 1);
        switch (cri2)
        {
            case 1: Console.OutputEncoding = Encoding.Unicode; break;
            case 2: Console.OutputEncoding = Encoding.UTF8; break;
        }
    }

    [Display(Name = "Open directory for hub", Description = "打开 hub 目录", GroupName = "About", AutoGenerateFilter = true,
        ShortName = "hub", Prompt = "ndx hub")]
    public static void OpenDirectoryForHub()
    {
        //配置
        var ci = new ConfigInit();

        if (CmdTo.IsWindows)
        {
            CmdTo.Execute($"start {ci.DXHub}");
        }
        else
        {
            var cr = CmdTo.Execute($"ls {ci.DXHub} -lh");
            ConsoleTo.LogColor(cr.CrOutput);
        }
    }
}