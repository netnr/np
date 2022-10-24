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
                    vh.Add($"<!--\r\nhttps://github.com/{adminGitHub}\r\n{DateTime.Now:yyyy-MM}\r\n-->");
                    break;

                case "loading":
                    vh.Add("<div id='LoadingMask' style='position:fixed;top:0;left:0;bottom:0;right:0;background-color:white;z-index:19999;background-image:url(\"/images/loading.svg\");background-repeat:no-repeat;background-position:48% 45%'></div>");
                    break;

                case "favicon":
                    vh.Add("<link rel='shortcut icon' href='/favicon.ico' type='image/x-icon' />");
                    break;

                case "netnrmd.css":
                    vh.Add("<link href='/app/md/dist/netnrmd.css?v4-20220819' rel='stylesheet' />");
                    break;
                case "netnrmd-ace.js":
                    vh.Add("<script src='/app/md/dist/ace.js?v4-20220819'></script>");
                    break;
                case "netnrmd.js":
                    vh.Add("<script src='/app/md/dist/netnrmd.js?v4-20220819'></script>");
                    break;

                case "fa4.css":
                    vh.Add("<link href='https://ss.netnr.com/font-awesome@4.7.0/css/font-awesome.min.css' rel='stylesheet' async />");
                    break;

                case "jquery3.js":
                    vh.Add("<script src='https://ss.netnr.com/jquery@3.6.1/dist/jquery.min.js'></script>");
                    break;

                case "bootstrap3.css":
                    vh.Add("<link href='https://ss.netnr.com/bootstrap@3.4.1/dist/css/bootstrap.min.css' rel='stylesheet' />");
                    break;
                case "bootstrap3.js":
                    vh.Add("<script src='https://ss.netnr.com/bootstrap@3.4.1/dist/js/bootstrap.min.js'></script>");
                    break;

                case "bootstrap5.css":
                    vh.Add("<link href='https://ss.netnr.com/bootstrap@5.2.2/dist/css/bootstrap.min.css' rel='stylesheet' async />");
                    break;
                case "bootstrap5.js":
                    vh.Add("<script src='https://ss.netnr.com/bootstrap@5.2.2/dist/js/bootstrap.bundle.min.js'></script>");
                    break;

                case "bpmn-js":
                    vh.Add("<link href='https://ss.netnr.com/bpmn-js@10.2.1/dist/assets/diagram-js.css' rel='stylesheet' async />");
                    vh.Add("<link href='https://ss.netnr.com/bpmn-js@10.2.1/dist/assets/bpmn-js.css' rel='stylesheet' async />");
                    vh.Add("<link href='https://ss.netnr.com/bpmn-js@10.2.1/dist/assets/bpmn-font/css/bpmn.css' rel='stylesheet' async />");
                    vh.Add("<script src='https://ss.netnr.com/bpmn-js@10.2.1/dist/bpmn-modeler.production.min.js'></script>");
                    break;

                case "jstree":
                    vh.Add("<link href='https://ss.netnr.com/jstree@3.3.12/dist/themes/default/style.min.css' rel='stylesheet' />");
                    vh.Add("<link href='https://ss.netnr.com/jstree@3.3.12/dist/themes/default-dark/style.min.css' rel='stylesheet' />");
                    vh.Add("<script src='https://ss.netnr.com/jstree@3.3.12/dist/jstree.min.js'></script>");
                    break;

                case "xm-select.js":
                    vh.Add("<script src='https://ss.netnr.com/xm-select@1.2.4/dist/xm-select.js'></script>");
                    break;

                case "viewer.css":
                    vh.Add("<link href='https://ss.netnr.com/viewerjs@1.11.0/dist/viewer.min.css' rel='stylesheet' />");
                    break;
                case "viewer.js":
                    vh.Add("<script src='https://ss.netnr.com/viewerjs@1.11.0/dist/viewer.min.js'></script>");
                    break;

                case "qiniu.js":
                    vh.Add("<script src='https://ss.netnr.com/qiniu-js@3.4.1/dist/qiniu.min.js'></script>");
                    break;

                case "cos-js-sdk-v5.js":
                    vh.Add("<script src='https://ss.netnr.com/cos-js-sdk-v5@1.4.10/dist/cos-js-sdk-v5.min.js'></script>");
                    break;

                case "ag-grid-enterprise.js":
                    vh.Add("<script src='https://ss.netnr.com/ag-grid-enterprise@28.2.0/dist/ag-grid-enterprise.min.js'></script>");
                    vh.Add("<script>agGrid.LicenseManager.prototype.outputMissingLicenseKey = _ => { }</script>");
                    break;

                case "ckeditor.js":
                    vh.Add("<script src='https://ss.netnr.com/ckeditor@4.12.1/ckeditor.js'></script>");
                    break;

                case "crypto.js":
                    vh.Add("<script src='https://ss.netnr.com/crypto-js@4.1.1/crypto-js.js'></script>");
                    break;

                case "md5.js":
                    vh.Add("<script src='https://ss.netnr.com/blueimp-md5@2.19.0/js/md5.min.js'></script>");
                    break;

                //生成二维码
                case "qrcode.js":
                    vh.Add("<script src='https://ss.netnr.com/qrcode@1.5.1/build/qrcode.js'></script>");
                    break;

                //解析二维码
                case "jsqr.js":
                    vh.Add("<script src='https://ss.netnr.com/jsqr@1.4.0/dist/jsQR.js'></script>");
                    break;

                case "sql-formatter.js":
                    vh.Add("<script src='https://ss.netnr.com/sql-formatter@11.0.2/dist/sql-formatter.min.js'></script>");
                    break;

                case "highcharts.js":
                    vh.Add("<script src='https://ss.netnr.com/highcharts@10.2.1/highcharts.js'></script>");
                    break;

                case "hls.js":
                    vh.Add("<script src='https://ss.netnr.com/hls.js@1.2.4/dist/hls.min.js'></script>");
                    break;

                case "watermark.js":
                    vh.Add("<script src='https://ss.netnr.com/watermarkjs@2.1.1/dist/watermark.min.js'></script>");
                    break;

                case "nsfwjs":
                    vh.Add("<script src='https://ss.netnr.com/@tensorflow/tfjs@2.8.6/dist/tf.min.js'></script>");
                    vh.Add("<script src='https://ss.netnr.com/nsfwjs@2.4.2/dist/nsfwjs.min.js'></script>");
                    break;

                case "cropperjs":
                    vh.Add("<link href='https://ss.netnr.com/cropperjs@1.5.12/dist/cropper.css' rel='stylesheet' />");
                    vh.Add("<script src='https://ss.netnr.com/cropperjs@1.5.12/dist/cropper.min.js'></script>");
                    break;

                case "terser.js":
                    vh.Add("<script src='https://ss.netnr.com/terser@5.15.1/dist/bundle.min.js'></script>");
                    break;

                case "html2canvas.js":
                    vh.Add("<script src='https://ss.netnr.com/html2canvas@1.4.1/dist/html2canvas.min.js'></script>");
                    break;

                case "asciinema-player":
                    vh.Add($"<link href='https://ss.netnr.com/asciinema-player@3.0.0-rc.3/dist/bundle/asciinema-player.css' rel='stylesheet' />");
                    vh.Add("<script src='https://ss.netnr.com/asciinema-player@3.0.0-rc.3/dist/bundle/asciinema-player.min.js'></script>");
                    break;

                case "api-spec-converter.js":
                    vh.Add("<script src='https://ss.netnr.com/api-spec-converter@2.12.0/dist/api-spec-converter.js'></script>");
                    break;

                case "swagger-ui-dist.css":
                    vh.Add("<link href='https://ss.netnr.com/swagger-ui-dist@4.15.0/swagger-ui.css' rel='stylesheet' />");
                    break;
                case "swagger-ui-dist.js":
                    vh.Add("<script src='https://ss.netnr.com/swagger-ui-dist@4.15.0/swagger-ui-bundle.js'></script>");
                    vh.Add("<script src='https://ss.netnr.com/swagger-ui-dist@4.15.0/swagger-ui-standalone-preset.js'></script>");
                    break;

                case "js-beautify":
                    vh.Add("<script src='https://ss.netnr.com/js-beautify@1.14.7/js/lib/beautifier.min.js'></script>");
                    break;

                case "jdenticon.js":
                    vh.Add("<script src='https://ss.netnr.com/jdenticon@3.2.0/dist/jdenticon.min.js'></script>");
                    break;

                case "jszip.js":
                    vh.Add("<script src='https://ss.netnr.com/jszip@3.10.1/dist/jszip.min.js'></script>");
                    break;

                case "pdf.js":
                    vh.Add("<script src='https://ss.netnr.com/pdfjs-dist@2.16.105/legacy/build/pdf.min.js'></script>");
                    break;

                case "nginxbeautifier":
                    vh.Add("<script src='https://ss.netnr.com/nginxbeautifier@1.0.19/nginxbeautifier.js'></script>");
                    break;

                case "monaco-editor":
                    vh.Add("<script src='https://ss.netnr.com/monaco-editor@0.34.1/min/vs/loader.js'></script>");
                    vh.Add(@"
                            <script>
                                var meRequire = require;

                                require.config({
                                    paths: {
                                        vs: 'https://ss.netnr.com/monaco-editor@0.34.1/min/vs'
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