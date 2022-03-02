//载入资源
const nd = {
    baseServer: ["localhost", "ss.netnr.com"].includes(location.hostname) ? "https://s1.netnr.com/libs/mxgraph" : "https://s1.netnr.eu.org/libs/mxgraph",
    sourceList: [
        "/grapheditor/styles/grapheditor.css",
        "/grapheditor/deflate/pako.min.js",
        "/grapheditor/deflate/base64.js",
        "/grapheditor/jscolor/jscolor.js",
        "/grapheditor/sanitizer/sanitizer.min.js",
        "/mxClient.min.js",
        "/grapheditor/js/EditorUi.js",
        "/grapheditor/js/Editor.js",
        "/grapheditor/js/Sidebar.js",
        "/grapheditor/js/Graph.js",
        "/grapheditor/js/Format.js",
        "/grapheditor/js/Shapes.js",
        "/grapheditor/js/Actions.js",
        "/grapheditor/js/Menus.js",
        "/grapheditor/js/Toolbar.js",
        "/grapheditor/js/Dialogs.js"
    ],
    createNode: function (nn, html) {
        let em = document.createElement(nn);
        em.innerHTML = html;
        return em;
    },
    init: function (callback) {
        let pas = [];
        nd.sourceList.forEach(u => {
            pas.push(fetch(nd.baseServer + u).then(x => x.text()));
        });

        Promise.all(pas).then(res => {

            var head = document.getElementsByTagName("HEAD")[0];

            for (let i = 0; i < res.length; i++) {
                let suri = nd.sourceList[i];
                let text = res[i];
                head.appendChild(nd.createNode(suri.endsWith('.js') ? "SCRIPT" : "STYLE", text));
            }

            callback();
        })
    }
};

// Parses URL parameters. Supported parameters are:
// - lang=xy: Specifies the language of the user interface.
// - touch=1: Enables a touch-style user interface.
// - storage=local: Enables HTML5 local storage.
// - chrome=0: Chromeless mode.
var urlParams = (function (url) {
    var result = new Object();
    var idx = url.lastIndexOf('?');

    if (idx > 0) {
        var params = url.substring(idx + 1).split('&');

        for (var i = 0; i < params.length; i++) {
            idx = params[i].indexOf('=');

            if (idx > 0) {
                result[params[i].substring(0, idx)] = params[i].substring(idx + 1);
            }
        }
    }

    return result;
})(window.location.href);

//编辑器对象、服务源
var eui, baseServer = nd.baseServer;

//初始化容器样式
document.body.classList.add("geEditor");

// Default resources are included in grapheditor resources
mxLoadResources = false;

// Public global variables
window.MAX_REQUEST_SIZE = 10485760;
window.MAX_AREA = 15000 * 15000;

window.mxBasePath = `${baseServer}/src`;
window.mxLanguage = urlParams['lang'] || "zh";
window.mxLanguages = ['zh'];

window.RESOURCES_PATH = `${baseServer}/grapheditor/resources`;
window.RESOURCE_BASE = `${baseServer}/grapheditor/resources/grapheditor`;
window.STENCIL_PATH = `${baseServer}/grapheditor/stencils`;
window.IMAGE_PATH = `${baseServer}/grapheditor/images`;
window.STYLE_PATH = `${baseServer}/grapheditor/styles`;
window.CSS_PATH = `${baseServer}/grapheditor/styles`;

//mxg
var mxg = {
    //临时存储key
    storageKey: "/graph/content",
    //eui回调
    euiInit: function () {
        //重写新建事件
        eui.actions.actions.new.funct = function () {
            if (confirm("确定删除当前记录？")) {
                localStorage.setItem(mxg.storageKey, "");
                eui.editor.setGraphXml()
            }
        }

        //重写打开导入
        eui.actions.actions.open.funct = eui.actions.actions.import.funct = eui.actions.actions.editDiagram.funct;

        //重写保存事件
        eui.actions.actions.save.funct = eui.actions.actions.saveAs.funct = function () {
            var name = eui.editor.filename;
            if (eui.editor.graph.isEditing()) {
                eui.editor.graph.stopEditing();
            }

            var xml = mxUtils.getXml(eui.editor.getGraphXml());

            try {
                mxUtils.popup(xml);

                eui.editor.setModified(false);
                eui.editor.setFilename(name);
                eui.updateDocumentTitle();
            }
            catch (e) {
                eui.editor.setStatus(mxUtils.htmlEntities(mxResources.get('errorSavingFile')));
            }
        }

        //重写帮助事件
        eui.actions.actions.help.funct = function () {
            eui.editor.graph.openLink("https://mxgraph.netnr.eu.org");
        }

        //重写导出边框
        ExportDialog.lastBorderValue = 15;

        //导出重写
        ExportDialog.exportFile = function (editorUi, name, format, bg, s, b) {
            var graph = editorUi.editor.graph, data;

            if (format == "pdf") {
                mxUtils.alert("暂不支持该格式导出");
                return
            }
            if (format == 'xml') {
                data = mxUtils.getXml(editorUi.editor.getGraphXml());
            }
            else {
                data = mxUtils.getXml(graph.getSvg(bg, s, b));
            }

            if (data.length < MAX_REQUEST_SIZE) {
                editorUi.hideDialog();

                if (format == "svg" || format == "xml") {
                    var blob = new Blob([data]);
                    mxg.down(blob, name);
                } else {
                    var svgbase64 = 'data:image/svg+xml;base64,' + btoa(unescape(encodeURIComponent(data)));
                    var canvas = document.createElement('canvas');
                    var img = new Image();
                    var ctx = canvas.getContext('2d');
                    img.onload = function () {
                        canvas.width = img.width;
                        canvas.height = img.height;
                        ctx.drawImage(img, 0, 0);
                        var url = canvas.toDataURL();

                        //下载
                        var blob = mxg.base64ToBlob(url);
                        mxg.down(blob, name);
                    }
                    img.src = svgbase64;
                }
            }
            else {
                mxUtils.alert(mxResources.get('drawingTooLarge'));
                mxUtils.popup(xml);
            }
        };

        window.onbeforeunload = null;

        //恢复本地记录
        var txt = localStorage.getItem(mxg.storageKey);
        try {
            eui.editor.setGraphXml(mxUtils.parseXml(txt).firstChild);
        } catch (e) { }

        setTimeout(function () {
            //实时本地存储
            eui.editor.graphChangeListener = function () {
                var txt = mxUtils.getXml(eui.editor.getGraphXml());
                localStorage.setItem(mxg.storageKey, txt);
            }
        }, 1000);

        //页脚
        mxg.footerInit();
    },
    //页脚面板
    footerInit: function () {
        mxg.footer = document.querySelector('.geFooterContainer');
        if (mxg.footer) {
            var ct = mxg.footer.style.cssText;
            mxg.footer.outerHTML = '<div class="geFooterContainer" style="' + ct + ';text-align:center"></div>';
            mxg.footer = document.querySelector('.geFooterContainer');

            //导出
            var fni = document.createElement("BUTTON");
            fni.onclick = function () {
                eui.actions.actions.export.funct();
            }
            fni.className = "geBtn";
            fni.innerHTML = mxResources.get('export');
            mxg.footer.appendChild(fni);

            //全屏切换
            var fni = document.createElement("BUTTON");
            fni.onclick = function () {
                if (document.body.className.indexOf('viewFull') >= 0) {
                    document.body.className = document.body.className.replace("viewFull", "").trim();
                } else {
                    document.body.className += " viewFull";
                }
            }
            fni.className = "geBtn";
            fni.innerHTML = "全屏切换";
            mxg.footer.appendChild(fni);
        }
    },
    //下载
    down: function (blob, fileName) {
        var aEle = document.createElement("a");
        aEle.download = fileName;
        aEle.href = URL.createObjectURL(blob);
        document.body.appendChild(aEle);
        aEle.click();
        document.body.removeChild(aEle);
    },
    base64ToBlob(code) {
        let parts = code.split(';base64,');
        let contentType = parts[0].split(':')[1];
        let raw = window.atob(parts[1]);
        let rawLength = raw.length;

        let uInt8Array = new Uint8Array(rawLength);

        for (let i = 0; i < rawLength; ++i) {
            uInt8Array[i] = raw.charCodeAt(i);
        }
        return new Blob([uInt8Array], { type: contentType });
    }
}


/*
 * 初始化
 */
nd.init(function () {

    var editorUiInit = EditorUi.prototype.init;

    EditorUi.prototype.init = function () {
        editorUiInit.apply(this, arguments);
    };

    // Adds required resources (disables loading of fallback properties, this can only
    // be used if we know that all keys are defined in the language specific file)
    mxResources.loadDefaultBundle = false;
    var bundle = mxResources.getDefaultBundle(RESOURCE_BASE, mxLanguage) ||
        mxResources.getSpecialBundle(RESOURCE_BASE, mxLanguage);

    // Fixes possible asynchronous requests
    mxUtils.getAll([bundle, STYLE_PATH + '/default.xml'], function (xhr) {
        // Adds bundle text to resources
        mxResources.parse(xhr[0].getText());

        // Configures the default graph theme
        var themes = new Object();
        themes[Graph.prototype.defaultThemeName] = xhr[1].getDocumentElement();

        // Main
        eui = new EditorUi(new Editor(false, themes));

        //回调
        mxg.euiInit.call(this, xhr);
    }, function () {
        document.body.innerHTML = '<center style="margin-top:10%;">Error loading resource files. Please check browser console.</center>';
    });
});