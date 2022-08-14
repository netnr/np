#if Full || Web

namespace Netnr;

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
        var adminGitHub = AppTo.GetValue("Common:AdminGitHub");

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

                case "netnrmd.css":
                    vh.Add("<link href='/app/md/dist/netnrmd.css?v4-20220714' rel='stylesheet' />");
                    break;
                case "netnrmd-ace.js":
                    vh.Add("<script src='/app/md/dist/ace.js?v4'></script>");
                    break;
                case "netnrmd.js":
                    vh.Add("<script src='/app/md/dist/netnrmd.js?v4-20220723'></script>");
                    break;

                case "fa4.css":
                    vh.Add("<link href='https://fastly.jsdelivr.net/npm/font-awesome@4.7.0/css/font-awesome.min.css' rel='stylesheet' async />");
                    break;

                case "jquery3.js":
                    vh.Add("<script src='https://fastly.jsdelivr.net/npm/jquery@3.6.0/dist/jquery.min.js'></script>");
                    break;

                case "bootstrap3.css":
                    vh.Add("<link href='https://fastly.jsdelivr.net/npm/bootstrap@3.4.1/dist/css/bootstrap.min.css' rel='stylesheet' />");
                    break;
                case "bootstrap3.js":
                    vh.Add("<script src='https://fastly.jsdelivr.net/npm/bootstrap@3.4.1/dist/js/bootstrap.min.js'></script>");
                    break;

                case "bootstrap5.css":
                    vh.Add("<link href='https://fastly.jsdelivr.net/npm/bootstrap@5.2.0/dist/css/bootstrap.min.css' rel='stylesheet' async />");
                    break;
                case "bootstrap5.js":
                    vh.Add("<script src='https://fastly.jsdelivr.net/npm/bootstrap@5.2.0/dist/js/bootstrap.bundle.min.js'></script>");
                    break;

                case "bpmn-js":
                    vh.Add("<link href='https://fastly.jsdelivr.net/npm/bpmn-js@9.3.2/dist/assets/diagram-js.css' rel='stylesheet' async />");
                    vh.Add("<link href='https://fastly.jsdelivr.net/npm/bpmn-js@9.3.2/dist/assets/bpmn-js.css' rel='stylesheet' async />");
                    vh.Add("<link href='https://fastly.jsdelivr.net/npm/bpmn-js@9.3.2/dist/assets/bpmn-font/css/bpmn.css' rel='stylesheet' async />");
                    vh.Add("<script src='https://fastly.jsdelivr.net/npm/bpmn-js@9.3.2/dist/bpmn-modeler.production.min.js'></script>");
                    break;

                case "leancloud-storage.js":
                    vh.Add("<script src='https://fastly.jsdelivr.net/npm/leancloud-storage@4.13.1/dist/av-min.js'></script>");
                    break;

                case "jstree":
                    vh.Add("<link href='https://fastly.jsdelivr.net/npm/jstree@3.3.12/dist/themes/default/style.min.css' rel='stylesheet' />");
                    vh.Add("<link href='https://fastly.jsdelivr.net/npm/jstree@3.3.12/dist/themes/default-dark/style.min.css' rel='stylesheet' />");
                    vh.Add("<script src='https://fastly.jsdelivr.net/npm/jstree@3.3.12/dist/jstree.min.js'></script>");
                    break;

                case "xm-select.js":
                    vh.Add("<script src='https://fastly.jsdelivr.net/npm/xm-select@1.2.4/dist/xm-select.js'></script>");
                    break;

                case "viewer.css":
                    vh.Add("<link href='https://fastly.jsdelivr.net/npm/viewerjs@1.10.5/dist/viewer.min.css' rel='stylesheet' />");
                    break;
                case "viewer.js":
                    vh.Add("<script src='https://fastly.jsdelivr.net/npm/viewerjs@1.10.5/dist/viewer.min.js'></script>");
                    break;

                case "qiniu.js":
                    vh.Add("<script src='https://fastly.jsdelivr.net/npm/qiniu-js@3.4.1/dist/qiniu.min.js'></script>");
                    break;

                case "cos-js-sdk-v5.js":
                    vh.Add("<script src='https://fastly.jsdelivr.net/npm/cos-js-sdk-v5@1.3.10/dist/cos-js-sdk-v5.min.js'></script>");
                    break;

                case "ag-grid-enterprise.js":
                    vh.Add("<script src='https://fastly.jsdelivr.net/npm/ag-grid-enterprise@28.1.0/dist/ag-grid-enterprise.min.js'></script>");
                    vh.Add("<script>agGrid.LicenseManager.prototype.outputMissingLicenseKey = _ => { }</script>");
                    break;

                case "ckeditor.js":
                    vh.Add("<script src='https://fastly.jsdelivr.net/npm/ckeditor@4.12.1/ckeditor.js'></script>");
                    break;

                case "crypto.js":
                    vh.Add("<script src='https://fastly.jsdelivr.net/npm/crypto-js@4.1.1/crypto-js.js'></script>");
                    break;

                case "md5.js":
                    vh.Add("<script src='https://fastly.jsdelivr.net/npm/blueimp-md5@2.19.0/js/md5.min.js'></script>");
                    break;

                //生成二维码
                case "qrcode.js":
                    vh.Add("<script src='https://fastly.jsdelivr.net/npm/qrcode@1.5.1/build/qrcode.js'></script>");
                    break;

                //解析二维码
                case "jsqr.js":
                    vh.Add("<script src='https://fastly.jsdelivr.net/npm/jsqr@1.4.0/dist/jsQR.js'></script>");
                    break;

                case "sql-formatter.js":
                    vh.Add("<script src='https://fastly.jsdelivr.net/npm/sql-formatter@9.0.0/dist/sql-formatter.min.js'></script>");
                    break;

                case "highcharts.js":
                    vh.Add("<script src='https://fastly.jsdelivr.net/npm/highcharts@10.2.0/highcharts.js'></script>");
                    break;

                case "hls.js":
                    vh.Add("<script src='https://fastly.jsdelivr.net/npm/hls.js@1.2.0/dist/hls.min.js'></script>");
                    break;

                case "watermark.js":
                    vh.Add("<script src='https://fastly.jsdelivr.net/npm/watermarkjs@2.1.1/dist/watermark.min.js'></script>");
                    break;

                case "nsfwjs":
                    vh.Add("<script src='https://fastly.jsdelivr.net/npm/@tensorflow/tfjs@2.8.6/dist/tf.min.js'></script>");
                    vh.Add("<script src='https://fastly.jsdelivr.net/npm/nsfwjs@2.4.1/dist/nsfwjs.min.js'></script>");
                    break;

                case "cropperjs":
                    vh.Add("<link href='https://fastly.jsdelivr.net/npm/cropperjs@1.5.12/dist/cropper.css' rel='stylesheet' />");
                    vh.Add("<script src='https://fastly.jsdelivr.net/npm/cropperjs@1.5.12/dist/cropper.min.js'></script>");
                    break;

                case "terser.js":
                    vh.Add("<script src='https://fastly.jsdelivr.net/npm/terser@5.14.2/dist/bundle.min.js'></script>");
                    break;

                case "html2canvas.js":
                    vh.Add("<script src='https://fastly.jsdelivr.net/npm/html2canvas@1.4.1/dist/html2canvas.min.js'></script>");
                    break;

                case "asciinema-player":
                    vh.Add($"<link href='https://fastly.jsdelivr.net/npm/asciinema-player@3.0.0-rc.3/dist/bundle/asciinema-player.css' rel='stylesheet' />");
                    vh.Add("<script src='https://fastly.jsdelivr.net/npm/asciinema-player@3.0.0-rc.3/dist/bundle/asciinema-player.min.js'></script>");
                    break;

                case "api-spec-converter.js":
                    vh.Add("<script src='https://fastly.jsdelivr.net/npm/api-spec-converter@2.12.0/dist/api-spec-converter.js'></script>");
                    break;

                case "swagger-ui-dist.css":
                    vh.Add("<link href='https://fastly.jsdelivr.net/npm/swagger-ui-dist@4.13.2/swagger-ui.css' rel='stylesheet' />");
                    break;
                case "swagger-ui-dist.js":
                    vh.Add("<script src='https://fastly.jsdelivr.net/npm/swagger-ui-dist@4.13.2/swagger-ui-bundle.js'></script>");
                    vh.Add("<script src='https://fastly.jsdelivr.net/npm/swagger-ui-dist@4.13.2/swagger-ui-standalone-preset.js'></script>");
                    break;

                case "js-beautify":
                    vh.Add("<script src='https://fastly.jsdelivr.net/npm/js-beautify@1.14.5/js/lib/beautifier.min.js'></script>");
                    break;

                case "jdenticon.js":
                    vh.Add("<script src='https://fastly.jsdelivr.net/npm/jdenticon@3.2.0/dist/jdenticon.min.js'></script>");
                    break;

                case "jszip.js":
                    vh.Add("<script src='https://fastly.jsdelivr.net/npm/jszip@3.10.1/dist/jszip.min.js'></script>");
                    break;

                case "pdf.js":
                    vh.Add("<script src='https://fastly.jsdelivr.net/npm/pdfjs-dist@2.15.349/legacy/build/pdf.min.js'></script>");
                    break;

                case "nginxbeautifier":
                    vh.Add("<script src='https://fastly.jsdelivr.net/npm/nginxbeautifier@1.0.19/nginxbeautifier.js'></script>");
                    break;

                case "monaco-editor":
                    vh.Add("<script src='https://fastly.jsdelivr.net/npm/monaco-editor@0.34.0/min/vs/loader.js'></script>");
                    vh.Add(@"
                            <script>
                                var meRequire = require;

                                require.config({
                                    paths: {
                                        vs: 'https://fastly.jsdelivr.net/npm/monaco-editor@0.34.0/min/vs'
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