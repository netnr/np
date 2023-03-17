import { nrcBase } from "./nrcBase";

let nrcRely = {
    /**
     * 远程资源
     * @param {any} name
     */
    remote: async (name) => {
        switch (name) {
            case "bootstrap5.css":
                await nrcBase.importStyle('https://npmcdn.com/bootstrap@5.3.0-alpha1/dist/css/bootstrap.min.css');
                break;
            case "agGrid":
                await nrcBase.importScript('https://npmcdn.com/ag-grid-enterprise@29.1.0/dist/ag-grid-enterprise.min.js');
                agGrid.LicenseManager.prototype.outputMissingLicenseKey = _ => { }
                break;
            case "echarts":
                await nrcBase.importScript('https://npmcdn.com/echarts@5.4.1/dist/echarts.min.js');
                break;
            case "netnrmdAce.js":
                await nrcBase.importScript('/file/md/ace.js?4.0.1');
                break;
            case "netnrmd":
                await nrcBase.importStyle('/file/md/netnrmd.css?4.0.1');
                await nrcBase.importScript('/file/md/netnrmd.js?4.0.1');
                break;
            case "asciinema-player":
                await nrcBase.importStyle('https://npmcdn.com/asciinema-player@3.1.2/dist/bundle/asciinema-player.css');
                await nrcBase.importScript('https://npmcdn.com/asciinema-player@3.1.2/dist/bundle/asciinema-player.min.js');
                break;
            case "choices":
                await nrcBase.importStyle('https://npmcdn.com/choices.js@10.2.0/public/assets/styles/choices.min.css');
                await nrcBase.importScript('https://npmcdn.com/choices.js@10.2.0/public/assets/scripts/choices.min.js');
                break;
            case "md5.js":
                await nrcBase.importScript('https://npmcdn.com/blueimp-md5@2.19.0/js/md5.min.js');
                break;
            case "hls.js":
                await nrcBase.importScript('https://npmcdn.com/hls.js@1.3.4/dist/hls.min.js');
                break;
            //生成二维码
            case "qrcode.js":
                await nrcBase.importScript('https://npmcdn.com/qrcode@1.5.1/build/qrcode.js');
                break;
            //解析二维码
            case "jsqr.js":
                await nrcBase.importScript('https://npmcdn.com/jsqr@1.4.0/dist/jsQR.js');
                break;
            case "terser.js":
                await nrcBase.importScript('https://npmcdn.com/terser@5.16.5/dist/bundle.min.js');
                break;
            case "editor-nginx":
                await nrcBase.importScript('/file/ss-editor-nginx.js?202303');
                await nrcBase.importScript('https://npmcdn.com/nginxbeautifier@1.0.19/nginxbeautifier.js');
                break;
            case "js-beautify.js":
                await nrcBase.importScript('https://npmcdn.com/js-beautify@1.14.7/js/lib/beautifier.min.js');
                await nrcBase.require(['beautifier'], 'beautifier');
                // require(['beautifier'], function (beautifier) {
                //     console.debug(beautifier)
                // });
                break;
            case "sql-formatter.js":
                await nrcBase.require(['https://npmcdn.com/sql-formatter@12.1.3/dist/sql-formatter.min.js'], 'sqlFormatter');
                break;
            case "html2canvas.js":
                await nrcBase.importScript('https://npmcdn.com/html2canvas@1.4.1/dist/html2canvas.min.js');
                break;
            case "identicon":
                await nrcBase.importScript('/file/identicon/identicon.min.js?202303');
                await nrcBase.importScript('https://npmcdn.com/jdenticon@3.2.0/dist/jdenticon.min.js');
                break;
            case "svgo.js":
                if (!window["svgo"]) {
                    Object.assign(window, { svgo: await import(/* webpackIgnore:true */ 'https://npmcdn.com/svgo@3.0.2/dist/svgo.browser.js') })
                }
                break;
            case "nsfwjs":
                await nrcBase.importScript('https://npmcdn.com/@tensorflow/tfjs@2.8.6/dist/tf.min.js');
                await nrcBase.importScript('https://npmcdn.com/nsfwjs@2.4.2/dist/nsfwjs.min.js');
                break;
            case "cropperjs":
                await nrcBase.importStyle('https://npmcdn.com/cropperjs@1.5.13/dist/cropper.min.css');
                await nrcBase.importScript('https://npmcdn.com/cropperjs@1.5.13/dist/cropper.min.js');
                break;
            case "pdf.js":
                await nrcBase.importScript('https://npmcdn.com/pdfjs-dist@3.4.120/build/pdf.min.js');
                break;
            case "jszip.js":
                await nrcBase.importScript('https://npmcdn.com/jszip@3.10.1/dist/jszip.min.js');
                break;
            case "bowser.js":
                await nrcBase.importScript('https://npmcdn.com/bowser@2.11.0/es5.js');
                break;
            case "swaggerui":
                await nrcBase.importStyle('https://npmcdn.com/swagger-ui-dist@4.18.0/swagger-ui.css');
                await nrcBase.require([
                    'https://npmcdn.com/swagger-ui-dist@4.18.0/swagger-ui-bundle.js',
                    'https://npmcdn.com/swagger-ui-dist@4.18.0/swagger-ui-standalone-preset.js'
                ], 'SwaggerUIBundle, SwaggerUIStandalonePreset')
                break;
            case "crypto.js":
                await nrcBase.importScript('https://npmcdn.com/crypto-js@4.1.1/crypto-js.js');
                break;
            case "jstree":
                await nrcBase.importScript('https://npmcdn.com/jquery@3.6.3/dist/jquery.min.js');
                await nrcBase.importStyle('https://npmcdn.com/jstree@3.3.15/dist/themes/default/style.min.css');
                await nrcBase.importStyle('https://npmcdn.com/jstree@3.3.15/dist/themes/default-dark/style.min.css');
                await nrcBase.importScript('https://npmcdn.com/jstree@3.3.15/dist/jstree.min.js');
                break;
        }
    }
}

export { nrcRely }