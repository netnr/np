#if Full || App

namespace Netnr.SharedApp;

/// <summary>
/// 资源引用
/// </summary>
public class QuoteTo
{
    /// <summary>
    /// 得到html字符串
    /// </summary>
    /// <param name="quotes">引用项，逗号分割，按顺序</param>
    /// <returns></returns>
    public static string Html(string quotes)
    {
        var adminGitHub = SharedFast.GlobalTo.GetValue("Common:AdminGitHub");

        var vh = new List<string>();

        List<string> listQuote = quotes.Split(',').ToList();
        foreach (var item in listQuote)
        {
            switch (item)
            {
                case "the":
                    vh.Add($"<!--\r\nhttps://github.com/{adminGitHub}\r\n{DateTime.Now:yyyy-MM-dd HH:mm:ss}\r\n-->");
                    break;

                case "loading":
                    vh.Add("<div id='LoadingMask' style='position:fixed;top:0;left:0;bottom:0;right:0;background-color:white;z-index:19999;background-image:url(\"/images/loading.svg\");background-repeat:no-repeat;background-position:48% 45%'></div>");
                    break;

                case "favicon":
                    vh.Add("<link rel='shortcut icon' href='/favicon.ico' type='image/x-icon' />");
                    break;

                case "fa4.css":
                    vh.Add("<link href='https://npm.elemecdn.com/font-awesome@4.7.0/css/font-awesome.min.css' rel='stylesheet' async />");
                    break;

                case "jquery3.js":
                    vh.Add("<script src='https://npm.elemecdn.com/jquery@3.6.0/dist/jquery.min.js'></script>");
                    break;

                case "bootstrap3.css":
                    vh.Add("<link href='https://npm.elemecdn.com/bootstrap@3.4.1/dist/css/bootstrap.min.css' rel='stylesheet' />");
                    break;
                case "bootstrap3.js":
                    vh.Add("<script src='https://npm.elemecdn.com/bootstrap@3.4.1/dist/js/bootstrap.min.js'></script>");
                    break;

                case "bootstrap5.css":
                    vh.Add("<link href='https://npm.elemecdn.com/bootstrap@5.1.3/dist/css/bootstrap.min.css' rel='stylesheet' async />");
                    break;
                case "bootstrap5.js":
                    vh.Add("<script src='https://npm.elemecdn.com/bootstrap@5.1.3/dist/js/bootstrap.bundle.min.js'></script>");
                    break;

                case "bpmn-js":
                    vh.Add("<link href='https://npm.elemecdn.com/bpmn-js@9.2.2/dist/assets/diagram-js.css' rel='stylesheet' async />");
                    vh.Add("<link href='https://npm.elemecdn.com/bpmn-js@9.2.2/dist/assets/bpmn-js.css' rel='stylesheet' async />");
                    vh.Add("<link href='https://npm.elemecdn.com/bpmn-js@9.2.2/dist/assets/bpmn-font/css/bpmn.css' rel='stylesheet' async />");
                    vh.Add("<script src='https://npm.elemecdn.com/bpmn-js@9.2.2/dist/bpmn-modeler.production.min.js'></script>");
                    break;

                case "leancloud-storage.js":
                    vh.Add("<script src='https://npm.elemecdn.com/leancloud-storage@4.12.3/dist/av-min.js'></script>");
                    break;

                case "shoelace":
                    vh.Add("<link href='https://npm.elemecdn.com/@shoelace-style/shoelace@2.0.0-beta.73/dist/themes/light.css' rel='stylesheet' async />");
                    vh.Add("<link href='https://npm.elemecdn.com/@shoelace-style/shoelace@2.0.0-beta.73/dist/themes/dark.css' rel='stylesheet' async />");
                    vh.Add("<script type='module' src='https://npm.elemecdn.com/@shoelace-style/shoelace@2.0.0-beta.73/dist/shoelace.js'></script>");
                    break;

                case "jstree":
                    vh.Add("<link href='https://npm.elemecdn.com/jstree@3.3.12/dist/themes/default/style.min.css' rel='stylesheet' />");
                    vh.Add("<link href='https://npm.elemecdn.com/jstree@3.3.12/dist/themes/default-dark/style.min.css' rel='stylesheet' />");
                    vh.Add("<script src='https://npm.elemecdn.com/jstree@3.3.12/dist/jstree.min.js'></script>");
                    break;

                case "netnrmd.css":
                    vh.Add("<link href='https://npm.elemecdn.com/netnrmd@3.0.3/src/netnrmd.css' rel='stylesheet' />");
                    break;
                case "netnrmd.js":
                    vh.Add("<script src='https://npm.elemecdn.com/netnrmd@3.0.3/src/netnrmd.bundle.js'></script>");
                    break;

                case "tocbot.js":
                    vh.Add("<script src='https://npm.elemecdn.com/tocbot@4.18.2/dist/tocbot.min.js'></script>");
                    break;

                case "xm-select.js":
                    vh.Add("<script src='https://npm.elemecdn.com/xm-select@1.2.4/dist/xm-select.js'></script>");
                    break;

                case "viewer.css":
                    vh.Add("<link href='https://npm.elemecdn.com/viewerjs@1.10.5/dist/viewer.min.css' rel='stylesheet' />");
                    break;
                case "viewer.js":
                    vh.Add("<script src='https://npm.elemecdn.com/viewerjs@1.10.5/dist/viewer.min.js'></script>");
                    break;

                case "qiniu.js":
                    vh.Add("<script src='https://npm.elemecdn.com/qiniu-js@3.4.1/dist/qiniu.min.js'></script>");
                    break;

                case "cos-js-sdk-v5.js":
                    vh.Add("<script src='https://npm.elemecdn.com/cos-js-sdk-v5@1.3.9/dist/cos-js-sdk-v5.min.js'></script>");
                    break;

                case "ag-grid-community.js":
                    vh.Add("<script src='https://npm.elemecdn.com/ag-grid-community@27.3.0/dist/ag-grid-community.min.js'></script>");
                    break;

                case "ag-grid-enterprise.js":
                    vh.Add("<script src='https://npm.elemecdn.com/ag-grid-enterprise@27.3.0/dist/ag-grid-enterprise.min.js'></script>");
                    vh.Add("<script>agGrid.LicenseManager.prototype.outputMissingLicenseKey = _ => { }</script>");
                    break;

                case "ckeditor.js":
                    vh.Add("<script src='https://npm.elemecdn.com/ckeditor@4.12.1/ckeditor.js'></script>");
                    break;

                case "crypto.js":
                    vh.Add("<script src='https://npm.elemecdn.com/crypto-js@4.1.1/crypto-js.js'></script>");
                    break;

                case "md5.js":
                    vh.Add("<script src='https://npm.elemecdn.com/blueimp-md5@2.19.0/js/md5.min.js'></script>");
                    break;

                //生成二维码
                case "qrcode.js":
                    vh.Add("<script src='https://npm.elemecdn.com/qrcode@1.5.0/build/qrcode.js'></script>");
                    break;

                //解析二维码
                case "jsqr.js":
                    vh.Add("<script src='https://npm.elemecdn.com/jsqr@1.4.0/dist/jsQR.js'></script>");
                    break;

                case "sql-formatter.js":
                    vh.Add("<script src='https://npm.elemecdn.com/sql-formatter@7.0.2/dist/sql-formatter.min.js'></script>");
                    break;

                case "highcharts.js":
                    vh.Add("<script src='https://npm.elemecdn.com/highcharts@10.1.0/highcharts.js'></script>");
                    break;

                case "hls.js":
                    vh.Add("<script src='https://npm.elemecdn.com/hls.js@1.1.5/dist/hls.min.js'></script>");
                    break;

                case "watermark.js":
                    vh.Add("<script src='https://npm.elemecdn.com/watermarkjs@2.1.1/dist/watermark.min.js'></script>");
                    break;

                case "nsfwjs":
                    vh.Add("<script src='https://npm.elemecdn.com/@tensorflow/tfjs@2.8.6/dist/tf.min.js'></script>");
                    vh.Add("<script src='https://npm.elemecdn.com/nsfwjs@2.4.1/dist/nsfwjs.min.js'></script>");
                    break;

                case "cropperjs":
                    vh.Add("<link href='https://npm.elemecdn.com/cropperjs@1.5.12/dist/cropper.css' rel='stylesheet' />");
                    vh.Add("<script src='https://npm.elemecdn.com/cropperjs@1.5.12/dist/cropper.min.js'></script>");
                    break;

                case "terser.js":
                    vh.Add("<script src='https://npm.elemecdn.com/terser@5.14.1/dist/bundle.min.js'></script>");
                    break;

                case "html2canvas.js":
                    vh.Add("<script src='https://npm.elemecdn.com/html2canvas@1.4.1/dist/html2canvas.min.js'></script>");
                    break;

                case "asciinema-player":
                    vh.Add($"<link href='https://npm.elemecdn.com/asciinema-player@3.0.0-rc.3/dist/bundle/asciinema-player.css' rel='stylesheet' />");
                    vh.Add("<script src='https://npm.elemecdn.com/asciinema-player@3.0.0-rc.3/dist/bundle/asciinema-player.min.js'></script>");
                    break;

                case "api-spec-converter.js":
                    vh.Add("<script src='https://npm.elemecdn.com/api-spec-converter@2.12.0/dist/api-spec-converter.js'></script>");
                    break;

                case "swagger-ui-dist.css":
                    vh.Add("<link href='https://npm.elemecdn.com/swagger-ui-dist@4.12.0/swagger-ui.css' rel='stylesheet' />");
                    break;
                case "swagger-ui-dist.js":
                    vh.Add("<script src='https://npm.elemecdn.com/swagger-ui-dist@4.12.0/swagger-ui-bundle.js'></script>");
                    vh.Add("<script src='https://npm.elemecdn.com/swagger-ui-dist@4.12.0/swagger-ui-standalone-preset.js'></script>");
                    break;

                case "js-beautify":
                    vh.Add("<script src='https://npm.elemecdn.com/js-beautify@1.14.4/js/lib/beautifier.min.js'></script>");
                    break;

                case "jdenticon.js":
                    vh.Add("<script src='https://npm.elemecdn.com/jdenticon@3.1.1/dist/jdenticon.min.js'></script>");
                    break;

                case "jszip.js":
                    vh.Add("<script src='https://npm.elemecdn.com/jszip@3.10.0/dist/jszip.min.js'></script>");
                    break;

                case "pdf.js":
                    vh.Add("<script src='https://npm.elemecdn.com/pdfjs-dist@2.14.305/legacy/build/pdf.min.js'></script>");
                    break;

                case "nginxbeautifier":
                    vh.Add("<script src='https://npm.elemecdn.com/nginxbeautifier@1.0.19/nginxbeautifier.js'></script>");
                    break;

                case "monaco-editor":
                    vh.Add("<script src='https://npm.elemecdn.com/monaco-editor@0.33.0/min/vs/loader.js'></script>");
                    vh.Add(@"
                            <script>
                                var meRequire = require;

                                require.config({
                                    paths: {
                                        vs: 'https://npm.elemecdn.com/monaco-editor@0.33.0/min/vs'
                                    },
                                    'vs/nls': { availableLanguages: { '*': 'zh-cn' } }
                                });
                            </script>
                        ");
                    break;
            }
        }

        return (string.Join(Environment.NewLine, vh) + Environment.NewLine).Replace(@"                            ", "");
    }
}
#endif