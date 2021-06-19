#if Full || App

using System;
using System.Linq;
using System.Collections.Generic;

namespace Netnr.SharedApp
{
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
            var vh = new List<string>();

            List<string> listQuote = quotes.Split(',').ToList();
            foreach (var item in listQuote)
            {
                switch (item)
                {
                    case "the":
                        vh.Add(@"
                                <!--
                                https://github.com/netnr
                                https://gitee.com/netnr
                                https://www.netnr.com
                                https://zme.ink
                                -->
                                ");
                        break;

                    case "favicon":
                        vh.Add("<link rel='shortcut icon' href='/favicon.ico' type='image/x-icon' />");
                        break;

                    case "blog-seo":
                        vh.Add("<meta name='keywords' content='NET牛人, Netnr, Gist, Run, Doc, Draw' />");
                        vh.Add("<meta name='description' content='NET牛人, Netnr, Gist, Run, Doc, Draw' />");
                        break;

                    case "guff-seo":
                        vh.Add("<meta name='keywords' content='Guff,尬服,尬服乐天地' />");
                        vh.Add("<meta name='description' content='Guff,尬服,尬服乐天地' />");
                        break;

                    case "fa.css":
                        vh.Add("<link href='https://cdn.jsdelivr.net/npm/font-awesome@4.7.0/css/font-awesome.min.css' rel='stylesheet' />");
                        break;
                        
                    case "jquery1.js":
                        vh.Add("<script src='https://cdn.jsdelivr.net/npm/jquery@1.12.4/dist/jquery.min.js'></script>");
                        break;

                    case "jquery.js":
                        vh.Add("<script src='https://cdn.jsdelivr.net/npm/jquery@3.6.0/dist/jquery.min.js'></script>");
                        break;
                        
                    case "bootstrap3.css":
                        vh.Add("<link href='https://cdn.jsdelivr.net/npm/bootstrap@3.4.1/dist/css/bootstrap.min.css' rel='stylesheet' />");
                        break;

                    case "bootstrap3.js":
                        vh.Add("<script src='https://cdn.jsdelivr.net/npm/bootstrap@3.4.1/dist/js/bootstrap.min.js'></script>");
                        break;

                    case "bootstrap5.css":
                        vh.Add("<link href='https://cdn.jsdelivr.net/npm/bootstrap@5.0.1/dist/css/bootstrap.min.css' rel='stylesheet' />");
                        break;

                    case "bootstrap5.js":
                        vh.Add("<script src='https://cdn.jsdelivr.net/npm/bootstrap@5.0.1/dist/js/bootstrap.bundle.min.js'></script>");
                        break;

                    case "bootstrap.css":
                    case "bootstrap4.css":
                        vh.Add("<link href='https://cdn.jsdelivr.net/npm/bootstrap@4.6.0/dist/css/bootstrap.min.css' rel='stylesheet' />");
                        break;

                    case "bootstrap.js":
                    case "bootstrap4.js":
                        vh.Add("<script src='https://cdn.jsdelivr.net/npm/bootstrap@4.6.0/dist/js/bootstrap.bundle.min.js'></script>");
                        break;
                        
                    case "swiper.css":
                        vh.Add("<link href='https://cdn.jsdelivr.net/npm/swiper@6.7.0/swiper-bundle.min.css' rel='stylesheet' />");
                        break;

                    case "swiper.js":
                        vh.Add("<script src='https://cdn.jsdelivr.net/npm/swiper@6.7.0/swiper-bundle.min.js'></script>");
                        break;

                    case "jz.js":
                        vh.Add("<script src='https://cdn.jsdelivr.net/npm/jzjs@2.0.2/2.0.2/jz.min.js'></script>");
                        break;

                    case "netnrmd.css":
                        vh.Add("<link href='https://cdn.jsdelivr.net/npm/netnrmd@3.0.0/src/netnrmd.css' rel='stylesheet' />");
                        break;

                    case "netnrmd.js":
                        vh.Add("<script src='https://cdn.jsdelivr.net/npm/netnrmd@3.0.0/src/netnrmd.bundle.js'></script>");
                        break;

                    case "netnrnav.js":
                        vh.Add("<script src='https://cdn.jsdelivr.net/gh/netnr/cdn/libs/netnr-nav/1.1.3/netnrnav.bundle.min.js' ></script>");
                        break;
                        
                    case "tocbot.js":
                        vh.Add("<script src='https://cdn.jsdelivr.net/npm/tocbot@4.12.3/dist/tocbot.min.js'></script>");
                        break;

                    case "selectpage":
                        vh.Add("<link href='https://cdn.jsdelivr.net/npm/selectpage@2.19.0/selectpage.css' rel='stylesheet' />");
                        vh.Add("<script src='https://cdn.jsdelivr.net/npm/selectpage@2.19.0/selectpage.min.js'></script>");
                        break;

                    case "ace.css":
                        vh.Add("<link href='https://cdn.jsdelivr.net/gh/netnr/cdn/libs/acenav/20200108/ace.min.css' rel='stylesheet' />");
                        vh.Add("<link href='https://cdn.jsdelivr.net/gh/netnr/cdn/libs/acenav/20200108/ace-skins.min.css' rel='stylesheet' async />");
                        break;

                    case "ace.js":
                        vh.Add("<script src='https://cdn.jsdelivr.net/gh/netnr/cdn/libs/acenav/20200108/ace.min.js'></script>");
                        break;

                    case "easyui":
                        vh.Add("<link href='https://cdn.jsdelivr.net/gh/netnr/cdn/libs/jquery-easyui/1.9.0/themes/metro/easyui.min.css' rel='stylesheet' />");
                        vh.Add("<script src='https://cdn.jsdelivr.net/gh/netnr/cdn/libs/jquery-easyui/1.9.0/jquery.easyui.min.min.js'></script>");
                        break;

                    case "bootstrap-table.css":
                        vh.Add("<link href='https://cdn.jsdelivr.net/npm/bootstrap-table@1.18.3/dist/bootstrap-table.min.css' rel='stylesheet' />");
                        break;

                    case "bootstrap-table.js":
                        vh.Add("<script src='https://cdn.jsdelivr.net/npm/bootstrap-table@1.18.3/dist/bootstrap-table.min.js'></script>");
                        vh.Add("<script src='https://cdn.jsdelivr.net/npm/bootstrap-table@1.18.3/dist/extensions/filter-control/bootstrap-table-filter-control.min.js'></script>");
                        break;

                    case "canvas-datagrid.js":
                        vh.Add("<script src='https://cdn.jsdelivr.net/npm/canvas-datagrid@0.3.4/dist/canvas-datagrid.min.js'></script>");
                        break;

                    case "ag-grid-community.js":
                        vh.Add("<script src='https://cdn.jsdelivr.net/npm/ag-grid-community@25.3.0/dist/ag-grid-community.min.js'></script>");
                        break;

                    case "ag-grid-enterprise.js":
                        vh.Add("<script src='https://cdn.jsdelivr.net/npm/ag-grid-enterprise@25.3.0/dist/ag-grid-enterprise.min.js'></script>");
                        break;

                    case "webuploader.js":
                        vh.Add("<script src='https://cdn.jsdelivr.net/npm/webuploader@0.1.8/dist/webuploader.html5only.min.js'></script>");
                        break;

                    case "ckeditor.js":
                        vh.Add("<script src='https://cdn.jsdelivr.net/npm/ckeditor@4.12.1/ckeditor.js'></script>");
                        break;

                    case "crypto.js":
                        vh.Add("<script src='https://cdn.jsdelivr.net/npm/crypto-js@4.0.0/crypto-js.min.js'></script>");
                        break;

                    case "md5.js":
                        vh.Add("<script src='https://cdn.jsdelivr.net/npm/blueimp-md5@2.18.0/js/md5.min.js'></script>");
                        break;

                    case "uuid4.js":
                        vh.Add("<script src='https://cdn.jsdelivr.net/npm/uuid@8.3.2/dist/umd/uuidv4.min.js'></script>");
                        break;

                    case "qrcode.js":
                        vh.Add("<script src='https://cdn.jsdelivr.net/npm/qrcode@1.4.4/build/qrcode.min.js'></script>");
                        break;

                    case "jsqr.js":
                        vh.Add("<script src='https://cdn.jsdelivr.net/npm/jsqr@1.4.0/dist/jsQR.min.js'></script>");
                        break;

                    case "bmob.js":
                        vh.Add("<script src='https://cdn.jsdelivr.net/gh/netnr/cdn/libs/bmob/1.7.0/bmob.min.js?170'></script>");
                        break;

                    case "xml2json.js":
                        vh.Add("<script src='https://cdn.jsdelivr.net/gh/abdolence/x2js@1.2.0/xml2json.min.js'></script>");
                        break;

                    case "sql-formatter.js":
                        vh.Add("<script src='https://cdn.jsdelivr.net/npm/sql-formatter@4.0.2/dist/sql-formatter.min.js'></script>");
                        break;

                    case "highcharts.js":
                        vh.Add("<script src='https://cdn.jsdelivr.net/npm/highcharts@9.1.1/highcharts.js'></script>");
                        break;

                    case "hls.js":
                        vh.Add("<script src='https://cdn.jsdelivr.net/npm/hls.js@1.0.6/dist/hls.min.js'></script>");
                        break;

                    case "watermark.js":
                        vh.Add("<script src='https://cdn.jsdelivr.net/npm/watermarkjs@2.1.1/dist/watermark.min.js'></script>");
                        break;

                    case "nsfwjs":
                        vh.Add("<script src='https://cdn.jsdelivr.net/npm/@tensorflow/tfjs@3.7.0/dist/tf.min.js'></script>");
                        vh.Add("<script src='https://cdn.jsdelivr.net/npm/nsfwjs@2.4.0/dist/nsfwjs.min.js'></script>");
                        break;

                    case "cropperjs":
                        vh.Add("<link href='https://cdn.jsdelivr.net/npm/cropperjs@1.5.11/dist/cropper.css' rel='stylesheet' />");
                        vh.Add("<script src='https://cdn.jsdelivr.net/npm/cropperjs@1.5.11/dist/cropper.min.js'></script>");
                        break;

                    case "terser.js":
                        vh.Add("<script src='https://cdn.jsdelivr.net/npm/terser@5.7.0/dist/bundle.min.js'></script>");
                        break;

                    case "cleancss.js":
                        vh.Add("<script src='https://cdn.jsdelivr.net/gh/jakubpawlowicz/clean-css-builds/v5.1.1.js'></script>");
                        break;

                    case "svgo.js":
                        vh.Add("<script src='https://cdn.jsdelivr.net/gh/netnr/cdn/libs/svgo/2.3.0/svgo.min.js'></script>");
                        break;

                    case "html2canvas.js":
                        vh.Add("<script src='https://cdn.jsdelivr.net/npm/html2canvas@1.0.0-rc.7/dist/html2canvas.min.js'></script>");
                        break;

                    case "asciinema-player.css":
                        vh.Add("<link href='https://cdn.jsdelivr.net/gh/netnr/cdn/libs/asciinema-player/2.6.1/asciinema-player.css' rel='stylesheet' />");
                        break;
                    case "asciinema-player.js":
                        vh.Add("<script src='https://cdn.jsdelivr.net/gh/netnr/cdn/libs/asciinema-player/2.6.1/asciinema-player.js'></script>");
                        break;

                    case "esprima.js":
                        vh.Add("<script src='https://cdn.jsdelivr.net/npm/esprima@4.0.1/dist/esprima.min.js'></script>");
                        break;
                        
                    case "js-yaml.js":
                        vh.Add("<script src='https://cdn.jsdelivr.net/npm/js-yaml@3.14.1/dist/js-yaml.min.js'></script>");
                        break;

                    case "api-spec-converter.js":
                        vh.Add("<script src='https://cdn.jsdelivr.net/npm/api-spec-converter@2.12.0/dist/api-spec-converter.min.js'></script>");
                        break;

                    case "swagger-ui-dist.css":
                        vh.Add("<link href='https://cdn.jsdelivr.net/npm/swagger-ui-dist@3.50.0/swagger-ui.css' rel='stylesheet' />");
                        break;
                    case "swagger-ui-dist.js":
                        vh.Add("<script src='https://cdn.jsdelivr.net/npm/swagger-ui-dist@3.50.0/swagger-ui-bundle.js'></script>");
                        vh.Add("<script src='https://cdn.jsdelivr.net/npm/swagger-ui-dist@3.50.0/swagger-ui-standalone-preset.js'></script>");
                        break;
                        
                    case "prettier-css":
                        vh.Add("<script src='https://cdn.jsdelivr.net/npm/prettier@2.3.1/standalone.min.js'></script>");;
                        vh.Add("<script src='https://cdn.jsdelivr.net/npm/prettier@2.3.1/parser-postcss.js'></script>");
                        break;

                    case "prettier":
                        vh.Add("<script src='https://cdn.jsdelivr.net/npm/prettier@2.3.1/standalone.min.js'></script>");
                        vh.Add("<script src='https://cdn.jsdelivr.net/npm/prettier@2.3.1/parser-angular.js'></script>");
                        vh.Add("<script src='https://cdn.jsdelivr.net/npm/prettier@2.3.1/parser-babel.js'></script>");
                        vh.Add("<script src='https://cdn.jsdelivr.net/npm/prettier@2.3.1/parser-flow.js'></script>");
                        vh.Add("<script src='https://cdn.jsdelivr.net/npm/prettier@2.3.1/parser-glimmer.js'></script>");
                        vh.Add("<script src='https://cdn.jsdelivr.net/npm/prettier@2.3.1/parser-graphql.js'></script>");
                        vh.Add("<script src='https://cdn.jsdelivr.net/npm/prettier@2.3.1/parser-html.js'></script>");
                        vh.Add("<script src='https://cdn.jsdelivr.net/npm/prettier@2.3.1/parser-markdown.js'></script>");
                        vh.Add("<script src='https://cdn.jsdelivr.net/npm/prettier@2.3.1/parser-postcss.js'></script>");
                        vh.Add("<script src='https://cdn.jsdelivr.net/npm/prettier@2.3.1/parser-typescript.js'></script>");
                        vh.Add("<script src='https://cdn.jsdelivr.net/npm/prettier@2.3.1/parser-yaml.js'></script>");
                        break;

                    case "lrz.js":
                        vh.Add("<script src='https://cdn.jsdelivr.net/npm/lrz@4.9.41/dist/lrz.all.bundle.min.js'></script>");
                        break;

                    case "identicon.js":
                        vh.Add("<script src='https://cdn.jsdelivr.net/gh/netnr/cdn/libs/identicon/2.3.3/identicon.js'></script>");
                        break;

                    case "jdenticon.js":
                        vh.Add("<script src='https://cdn.jsdelivr.net/npm/jdenticon@3.1.0/dist/jdenticon.min.js'></script>");
                        break;

                    case "jszip.js":
                        vh.Add("<script src='https://cdn.jsdelivr.net/npm/jszip@3.6.0/dist/jszip.min.js'></script>");
                        break;

                    case "tti.js":
                        vh.Add("<script src='https://cdn.jsdelivr.net/gh/netnr/cdn/libs/text-to-image/20201119/tti.js'></script>");
                        break;

                    case "nginxbeautifier":
                        vh.Add("<script src='https://cdn.jsdelivr.net/npm/nginxbeautifier@1.0.19/nginxbeautifier.min.js'></script>");
                        break;

                    case "codemirror":
                        vh.Add("<link href='https://cdn.jsdelivr.net/npm/codemirror@5.61.1/lib/codemirror.min.css' rel='stylesheet' />");
                        vh.Add("<script src='https://cdn.jsdelivr.net/npm/codemirror@5.61.1/lib/codemirror.min.js'></script>");
                        break;

                    case "codemirror-nginx":
                        vh.Add("<script src='https://cdn.jsdelivr.net/npm/codemirror@5.61.1/mode/nginx/nginx.min.js'></script>");
                        break;

                    case "monaco-editor":
                        vh.Add("<script src='https://cdn.jsdelivr.net/npm/monaco-editor@0.25.0/min/vs/loader.js'></script>");
                        vh.Add(@"
                                <script>
                                    function htmlDecode(html) {
                                        var a = document.createElement('a');
                                        a.innerHTML = html;
                                        return a.innerText;
                                    }

                                    require.config({
                                        paths: {
                                            vs: 'https://cdn.jsdelivr.net/npm/monaco-editor@0.25.0/min/vs'
                                        },
                                        'vs/nls': { availableLanguages: { '*': 'zh-cn' } }
                                    });
                                </script>
                        ");
                        break;

                    case "loading":
                        vh.Add("<div id='LoadingMask' style='position:fixed;top:0;left:0;bottom:0;right:0;background-color:white;z-index:19999;background-image:url(\"/images/loading.svg\");background-repeat:no-repeat;background-position:48% 45%'></div>");
                        break;
                }
            }

            return (string.Join(Environment.NewLine, vh) + Environment.NewLine).Replace(@"                                ", "");
        }
    }
}

#endif