#if Full || Web

using System.Text.RegularExpressions;

namespace Netnr;

/// <summary>
/// 资源引用
/// </summary>
public partial class QuoteTo
{
    /// <summary>
    /// 面板容器样式1
    /// </summary>
    public static string PanelClass1 { get; set; } = "container-fluid p-lg-5 pb-lg-0 py-4 pb-0";

    /// <summary>
    /// 面板容器样式2
    /// </summary>
    public static string PanelClass2 { get; set; } = "container-fluid p-lg-5 py-4";

    /// <summary>
    /// 面板容器样式3
    /// </summary>
    public static string PanelClass3 { get; set; } = "container-fluid px-lg-5";

    /// <summary>
    /// 得到html字符串
    /// </summary>
    /// <param name="quotes">引用项，逗号分割，按顺序</param>
    /// <returns></returns>
    public static string Html(string quotes)
    {
        var vh = new List<string>();

        var listQuote = quotes.Split(',');
        foreach (var item in listQuote)
        {
            switch (item)
            {
                case "the":
                    vh.Add($"<!-- https://github.com/{AppTo.GetValue("ProgramParameters:AdminGitHub")}  {DateTime.Now:yyyy-MM} -->");
                    break;

                case "loading":
                    vh.Add("<div id='LoadingMask' style='position:fixed;top:0;left:0;bottom:0;right:0;background-color:white;z-index:19999;background-image:url(\"/images/loading.svg\");background-repeat:no-repeat;background-position:48% 45%'></div>");
                    break;

                case "favicon":
                    vh.Add("<link rel='icon' href='/favicon.ico'>");
                    break;

                case "fa4.css":
                    vh.Add("<link href='https://npmcdn.com/font-awesome@4.7.0/css/font-awesome.min.css' rel='stylesheet'/>");
                    break;

                case "jquery3.js":
                    vh.Add("<script src='https://npmcdn.com/jquery@3.7.1/dist/jquery.min.js'></script>");
                    break;

                case "bootstrap3.css":
                    vh.Add("<link href='https://npmcdn.com/bootstrap@3.4.1/dist/css/bootstrap.min.css' rel='stylesheet' />");
                    break;
                case "bootstrap3.js":
                    vh.Add("<script src='https://npmcdn.com/bootstrap@3.4.1/dist/js/bootstrap.min.js'></script>");
                    break;
                case "bootstrap5.css":
                    vh.Add("<link href='https://npmcdn.com/bootstrap@5.3.3/dist/css/bootstrap.min.css' rel='stylesheet' />");
                    break;
                case "bootstrap5.js":
                    vh.Add("<script src='https://npmcdn.com/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js' async ></script>");
                    break;

                case "bpmn-js":
                    vh.Add("<link href='https://npmcdn.com/bpmn-js@11.5.0/dist/assets/diagram-js.css' rel='stylesheet'/>");
                    vh.Add("<link href='https://npmcdn.com/bpmn-js@11.5.0/dist/assets/bpmn-js.css' rel='stylesheet'/>");
                    vh.Add("<link href='https://npmcdn.com/bpmn-js@11.5.0/dist/assets/bpmn-font/css/bpmn.css' rel='stylesheet'/>");
                    vh.Add("<script src='https://npmcdn.com/bpmn-js@11.5.0/dist/bpmn-modeler.production.min.js'></script>");
                    break;
            }
        }

        return string.Join("\r\n", vh.Select(MirrorNPM));
    }

    /// <summary>
    /// NPM 镜像
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public static string MirrorNPM(string url)
    {
        // "https://registry.npmmirror.com/$2/$3/files/$4.$5"
        var replacement = "https://ss.netnr.com/$2@$3/$4.$5";
        url = MatchMirrorNPM().Replace(url, replacement);
        return url;
    }

    [GeneratedRegex(@"(https?:\/\/[\w.-]+)\/(.*)@([\d.]+)\/(.*)\.(\w+)")]
    private static partial Regex MatchMirrorNPM();
}
#endif