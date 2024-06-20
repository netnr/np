import { nrEditor } from "./nrEditor";
import { nrcBase } from "./nrcBase";

let nrcRely = {
    /**
     * 远程资源
     * @param {any} name
     */
    remote: async (name) => {
        switch (name) {
            case "bootstrap5.js":
                await nrcBase.importScript('https://npmcdn.com/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js');
                break;
            case "bootstrap5.css":
                await nrcBase.importStyle('https://npmcdn.com/bootstrap@5.3.3/dist/css/bootstrap.min.css');
                break;
            case "agGrid":
                await nrcBase.importScript('https://npmcdn.com/ag-grid-enterprise@31.3.2/dist/ag-grid-enterprise.min.js');
                break;
            case "echarts":
                await nrcBase.importScript('https://npmcdn.com/echarts@5.5.0/dist/echarts.min.js');
                break;
            case "netnrmdEditor":
                // 打包后实践，部分页面使用 await nrcRely.remote("netnrmdEditor") 引入出错
                // 使用线上方式代替 await nrEditor.rely()
                await nrEditor.rely();
                // await nrcBase.importStyle('/file/md/monaco.css?4.1.0');
                // await nrcBase.importScript('/file/md/monaco.js?4.1.0');
                break;
            case "netnrmd":
                await nrcBase.importStyle('/file/md/netnrmd.css?4.1.0');
                await nrcBase.importScript('/file/md/netnrmd.js?4.1.0');
                break;
            case "asciinema-player":
                await nrcBase.importStyle('https://npmcdn.com/asciinema-player@3.7.1/dist/bundle/asciinema-player.css');
                await nrcBase.importScript('https://npmcdn.com/asciinema-player@3.7.1/dist/bundle/asciinema-player.min.js');
                break;
            case "choices":
                await nrcBase.importStyle('https://npmcdn.com/choices.js@10.2.0/public/assets/styles/choices.min.css');
                await nrcBase.importScript('https://npmcdn.com/choices.js@10.2.0/public/assets/scripts/choices.min.js');
                break;
            case "md5.js":
                await nrcBase.importScript('https://npmcdn.com/blueimp-md5@2.19.0/js/md5.min.js');
                break;
            case "hls.js":
                await nrcBase.importScript('https://npmcdn.com/hls.js@1.5.8/dist/hls.min.js');
                break;
            //生成二维码（保持 1.5.1 勿升级，新版本没有 build https://github.com/soldair/node-qrcode/issues/346）
            case "qrcode.js":
                await nrcBase.importScript('https://npmcdn.com/qrcode@1.5.1/build/qrcode.js');
                break;
            //解析二维码
            case "jsqr.js":
                await nrcBase.importScript('https://npmcdn.com/jsqr@1.4.0/dist/jsQR.js');
                break;
            case "terser.js":
                await nrcBase.importScript('https://npmcdn.com/terser@5.31.0/dist/bundle.min.js');
                break;
            case "editor-nginx":
                await nrcBase.importScript('/file/ss-editor-nginx.js?202303');
                await nrcBase.importScript('https://npmcdn.com/nginxbeautifier@1.0.19/nginxbeautifier.js');
                break;
            case "js-beautify.js":
                await nrcBase.importScript('https://npmcdn.com/js-beautify@1.15.1/js/lib/beautifier.min.js');
                await nrcBase.require(['beautifier'], 'beautifier');
                // require(['beautifier'], function (beautifier) {
                //     console.debug(beautifier)
                // });
                break;
            case "sql-formatter.js":
                await nrcBase.require(['https://npmcdn.com/sql-formatter@15.3.1/dist/sql-formatter.min.js'], 'sqlFormatter');
                break;
            case "html2canvas.js":
                await nrcBase.importScript('https://npmcdn.com/html2canvas@1.4.1/dist/html2canvas.min.js');
                break;
            case "identicon":
                await nrcBase.importScript('/file/identicon/identicon.min.js?202303');
                await nrcBase.importScript('https://npmcdn.com/jdenticon@3.3.0/dist/jdenticon.min.js');
                break;
            case "svgo.js":
                if (!window["svgo"]) {
                    Object.assign(window, { svgo: await import(/* webpackIgnore:true */ 'https://npmcdn.com/svgo@3.3.2/dist/svgo.browser.js') })
                }
                break;
            case "nsfwjs":
                await nrcBase.importScript('https://npmcdn.com/@tensorflow/tfjs@2.8.6/dist/tf.min.js');
                await nrcBase.importScript('https://npmcdn.com/nsfwjs@2.4.2/dist/nsfwjs.min.js');
                break;
            case "segmentit.js":
                await nrcBase.importScript('https://npmcdn.com/segmentit@2.0.3/dist/umd/segmentit.js');
                break;
            case "cropperjs":
                await nrcBase.importStyle('https://npmcdn.com/cropperjs@1.6.2/dist/cropper.min.css');
                await nrcBase.importScript('https://npmcdn.com/cropperjs@1.6.2/dist/cropper.min.js');
                break;
            //https://github.com/exceljs/exceljs
            //只支持 xlsx
            case "exceljs":
                await nrcBase.importScript('https://npmcdn.com/exceljs@4.4.0/dist/exceljs.min.js');
                break;
            //https://github.com/SheetJS/sheetjs
            //社区版 不支持设置样式
            case "XLSX":
                await nrcBase.importScript('https://npmcdn.com/xlsx@0.18.5/dist/xlsx.full.min.js');
                break;
            //使用版本3
            case "pdf.js":
                await nrcBase.importScript('https://npmcdn.com/pdfjs-dist@3.11.174/build/pdf.min.js');
                break;
            case "jszip.js":
                await nrcBase.importScript('https://npmcdn.com/jszip@3.10.1/dist/jszip.min.js');
                break;
            case "bowser.js":
                await nrcBase.importScript('https://npmcdn.com/bowser@2.11.0/es5.js');
                break;
            case "swaggerui":
                await nrcBase.importStyle('https://npmcdn.com/swagger-ui-dist@5.17.10/swagger-ui.css');
                await nrcBase.require([
                    'https://npmcdn.com/swagger-ui-dist@5.17.10/swagger-ui-bundle.js',
                    'https://npmcdn.com/swagger-ui-dist@5.17.10/swagger-ui-standalone-preset.js'
                ], 'SwaggerUIBundle, SwaggerUIStandalonePreset')
                break;
            case "crypto.js":
                await nrcBase.importScript('https://npmcdn.com/crypto-js@4.2.0/crypto-js.js');
                break;
            case "jstree":
                await nrcBase.importScript('https://npmcdn.com/jquery@3.7.1/dist/jquery.min.js');
                await nrcBase.importStyle('https://npmcdn.com/jstree@3.3.16/dist/themes/default/style.min.css');
                await nrcBase.importStyle('https://npmcdn.com/jstree@3.3.16/dist/themes/default-dark/style.min.css');
                await nrcBase.importScript('https://npmcdn.com/jstree@3.3.16/dist/jstree.min.js');
                break;
            case "pinyin-pro.js":
                await nrcBase.importScript('https://npmcdn.com/pinyin-pro@3.20.4/dist/index.js');
                break;
        }
    }
}

Object.assign(window, { nrcRely });
export { nrcRely }