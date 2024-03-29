/*
 * mxg 为 mxgraph 调用封装
 * by netnr
 * 
 */

//初始化容器样式
document.body.classList.add("geEditor");

//打开主键、编辑器对象、服务源
var rowId = location.pathname.split('/').pop(), eui, mxBaseServer = "/file/mxgraph";

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

// Default resources are included in grapheditor resources
mxLoadResources = false;


/**
 * Init.js 配置信息
 */

// Public global variables
window.MAX_REQUEST_SIZE = 10485760;
window.MAX_AREA = 15000 * 15000;

// URLs for save and export
window.SAVE_URL = `/draw/EditorSave/${rowId}`;
window.OPEN_URL = `/draw/EditorOpen/${rowId}`;
window.RESOURCES_PATH = `${mxBaseServer}/grapheditor/resources`;
window.RESOURCE_BASE = `${mxBaseServer}/grapheditor/resources/grapheditor`;
window.STENCIL_PATH = `${mxBaseServer}/grapheditor/stencils`;
window.IMAGE_PATH = `${mxBaseServer}/grapheditor/images`;
window.STYLE_PATH = `${mxBaseServer}/grapheditor/styles`;
window.CSS_PATH = `${mxBaseServer}/grapheditor/styles`;
window.OPEN_FORM = `${mxBaseServer}/grapheditor/open.html`;

window.mxBasePath = `${mxBaseServer}/src`;
window.mxLanguage = urlParams['lang'] || "zh";
window.mxLanguages = ['zh'];



//mxg对象拓展
var mxg = {
    //eui回调
    euiInit: function () {
        //重写新建事件
        eui.actions.actions.new.funct = function () {
            eui.editor.graph.openLink("/draw");
        }

        //重写保存事件
        eui.actions.actions.save.funct = eui.actions.actions.saveAs.funct = function () {
            var name = eui.editor.filename;
            if (name != null && name != "") {
                if (eui.editor.graph.isEditing()) {
                    eui.editor.graph.stopEditing();
                }

                var xml = mxUtils.getXml(eui.editor.getGraphXml());

                try {
                    if (xml.length < MAX_REQUEST_SIZE) {

                        if (!mxg.RequestActive) {
                            mxg.RequestActive = "save";

                            var req = new mxXmlRequest(SAVE_URL, 'filename=' + encodeURIComponent(name) +
                                '&xml=' + encodeURIComponent(xml) + '&DrType=draw');
                            req.send(function (mx) {
                                var data = JSON.parse(mx.request.responseText);
                                if (data.code == 200 && data.data) {
                                    location.href = "/draw/code/" + data.data;
                                } else {
                                    mxUtils.alert(data.msg);
                                }
                                if (data.code == 200) {
                                    window.onbeforeunload = null;
                                }
                                mxg.RequestActive = null;
                            }, function () {
                                mxg.RequestActive = null;
                            });
                        } else {
                            console.log('Operating too fast: ' + mxg.RequestActive);
                        }

                    }
                    else {
                        mxUtils.alert(mxResources.get('drawingTooLarge'));
                        mxUtils.popup(xml);

                        return;
                    }

                    eui.editor.setModified(false);
                    eui.editor.setFilename(name);
                    eui.updateDocumentTitle();
                }
                catch (e) {
                    eui.editor.setStatus(mxUtils.htmlEntities(mxResources.get('errorSavingFile')));
                }
            } else {
                mxUtils.alert("请输入标题名称");
            }
        }

        //重写帮助事件
        eui.actions.actions.help.funct = function () {
            eui.editor.graph.openLink("https://jgraph.github.io/mxgraph/");
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

        //菜单
        mxg.MenuInit();

        //图形
        mxg.opGrap();

        //页脚
        mxg.FooterInit();
    },
    //open回调
    openInit: function (req) {
        //一条记录
        var result = req.request.responseText;
        if (result != "") {
            result = JSON.parse(result);
            if (result.code == 200) {
                let data = result.data;
                try {
                    this.editor.setFilename(data.DrName);
                    mxg.titleNode.value = data.DrName;

                    this.editor.setGraphXml(mxUtils.parseXml(data.DrContent).firstChild);
                    this.actions.editorUi.updateDocumentTitle();
                } catch (e) { console.log(e) }
            } else {
                alert(result.msg);
            }
        }
    },
    //菜单调整
    MenuInit: function () {
        var fm2 = eui.menubar.addMenu('Netnr', function () {
            location.href = "/";
        })
        fm2.href = "/";
        var fm1 = eui.menubar.addMenu('Discover', function () {
            location.href = "/draw/discover";
        })
        fm1.href = "/draw/discover";
        fm1.style.cssText = fm2.style.cssText = 'float:right;color:blue;font-weight:600;margin-right:0.5em;';

    },
    //页脚面板
    FooterInit: function () {
        mxg.footer = document.querySelector('.geFooterContainer');
        var ct = mxg.footer.style.cssText;
        mxg.footer.outerHTML = '<div class="geFooterContainer" style="' + ct + ';text-align:center"></div>';
        mxg.footer = document.querySelector('.geFooterContainer');

        //标题
        var fni = document.createElement("INPUT");
        mxg.titleNode = fni;
        fni.maxlength = 30;
        fni.placeholder = "标题名称";
        fni.oninput = function () {
            eui.editor.setFilename(this.value);
            eui.actions.editorUi.updateDocumentTitle();
        }
        fni.value = eui.editor.filename;
        fni.className = "btns";
        fni.style.width = "28vw";
        mxg.footer.appendChild(fni);

        //保存
        var fni = document.createElement("BUTTON");
        fni.onclick = function () {
            eui.actions.actions.save.funct();
        }
        fni.className = "btns";
        fni.innerHTML = "保存";
        mxg.footer.appendChild(fni);

        //导出
        var fni = document.createElement("BUTTON");
        fni.onclick = function () {
            eui.actions.actions.export.funct();
        }
        fni.className = "btns";
        fni.innerHTML = "导出";
        mxg.footer.appendChild(fni);

        //全屏切换
        var fni = document.createElement("BUTTON");
        fni.onclick = function () {
            document.body.classList.toggle('viewFull')
        }
        fni.className = "btns";
        fni.innerHTML = "全屏切换";
        mxg.footer.appendChild(fni);
    },
    //图形操作
    opGrap: function () {
        eui.sidebar.palettes.flowchart[0].innerHTML = "流程图";
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
    },
    //剪切图片
    cutImg: function (src, callback, w, h) {
        var canvas = document.createElement("canvas"),
            ctx = canvas.getContext("2d"),
            im = new Image();
        w = w || 0, h = h || 0;
        im.onload = function () {
            //为传入缩放尺寸用原尺寸
            !w && (w = this.width);
            !h && (h = this.height);
            //以长宽最大值作为最终生成图片的依据
            if (w !== this.width || h !== this.height) {
                var ratio;
                if (w > h) {
                    ratio = this.width / w;
                    h = this.height / ratio;
                } else if (w === h) {
                    if (this.width > this.height) {
                        ratio = this.width / w;
                        h = this.height / ratio;
                    } else {
                        ratio = this.height / h;
                        w = this.width / ratio;
                    }
                } else {
                    ratio = this.height / h;
                    w = this.width / ratio;
                }
            }
            //以传入的长宽作为最终生成图片的尺寸
            if (w > h) {
                var offset = (w - h) / 2;
                canvas.width = canvas.height = w;
                ctx.drawImage(im, 0, offset, w, h);
            } else if (w < h) {
                var offset = (h - w) / 2;
                canvas.width = canvas.height = h;
                ctx.drawImage(im, offset, 0, w, h);
            } else {
                canvas.width = canvas.height = h;
                ctx.drawImage(im, 0, 0, w, h);
            }
            callback(canvas.toDataURL("image/png"));
        }
        im.src = src;
    }
}

/*
 * 初始化
 */
window.addEventListener('DOMContentLoaded', function () {

    var editorUiInit = EditorUi.prototype.init;

    EditorUi.prototype.init = function () {
        editorUiInit.apply(this, arguments);
        this.actions.get('export').setEnabled(false);

        // Updates action states which require a backend
        if (!Editor.useLocalStorage) {
            mxUtils.post(OPEN_URL, '', mxUtils.bind(this, function (req) {
                var enabled = req.getStatus() != 404;
                this.actions.get('open').setEnabled(enabled || Graph.fileSupport);
                this.actions.get('import').setEnabled(enabled || Graph.fileSupport);
                this.actions.get('save').setEnabled(enabled);
                this.actions.get('saveAs').setEnabled(enabled);
                this.actions.get('export').setEnabled(enabled);

                //回调
                mxg.openInit.call(this, req);
            }));
        }
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
        eui = new EditorUi(new Editor(urlParams['chrome'] == '0', themes));

        //回调
        mxg.euiInit.call(this, xhr);
    }, function () {
        document.body.innerHTML = '<center style="margin-top:10%;">Error loading resource files. Please check browser console.</center>';
    });

    document.getElementById("LoadingMask").style.display = "none";
}, false);