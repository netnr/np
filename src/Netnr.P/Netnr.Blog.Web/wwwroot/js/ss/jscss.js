nr.onChangeSize = function (ch) {
    var vh = ch - nr.domEditor1.getBoundingClientRect().top - 50;
    nr.domEditor1.style.height = vh + "px";
    nr.domEditor2.style.height = vh + "px";
}

var editor1, editor2;

nr.onReady = function () {
    ss.loading(true);
    me.init().then(() => {
        editor1 = me.create(nr.domEditor1, {
            language: "javascript",
            value: "// 粘贴或拖拽 JS、CSS 代码"
        });

        editor2 = me.create(nr.domEditor2, {
            value: "",
            language: "text/plain",
            wordWrap: "on",
        });

        nr.domEditor1.classList.add('border');
        nr.domEditor2.classList.add('border');
        nr.domCardBox.classList.remove('invisible');
        ss.loading(false);

        editor1.onDidChangeModelContent(function (e) {
            clearTimeout(window.defer1);
            window.defer1 = setTimeout(function () {
                nr.domCardInfo1.innerHTML = `大小：<b>${nr.formatByteSize(editor1.getValue().length)}</b>`;
            }, 1000 * 1)
        });

        nr.domBtnMinifyJs.addEventListener("click", function () {
            var code = editor1.getValue();
            if (code.trim() != "") {

                me.setLanguage(editor1, "javascript");
                me.setLanguage(editor2, "javascript");
                nr.domBtnDownload.parentElement.classList.remove('invisible');

                Terser.minify(code, page.options.js).then(res => {
                    me.keepSetValue(editor2, res.code);
                    nr.domCardInfo2.innerHTML = `压缩后：<b>${nr.formatByteSize(res.code.length)}</b>，
                    压缩率：<b>${(res.code.length / code.length * 100).toFixed(2)}%</b>`;
                }).catch(e => {
                    console.debug(e);
                    me.keepSetValue(editor2, e + "");
                })
            }
        });

        nr.domBtnMinifyCss.addEventListener("click", function () {
            var code = editor1.getValue();
            if (code.trim() != "") {
                me.setLanguage(editor1, "css");
                me.setLanguage(editor2, "css");
                nr.domBtnDownload.parentElement.classList.remove('invisible');

                var cout = new CleanCSS(page.options.cleancss).minify(code);
                if (cout.errors.length) {
                    me.keepSetValue(editor2, cout.errors.join("\r\n"));
                } else {
                    me.keepSetValue(editor2, cout.styles);
                    nr.domCardInfo2.innerHTML = `压缩后：<b>${nr.formatByteSize(cout.styles.length)}</b>，
                    压缩率：<b>${(cout.styles.length / code.length * 100).toFixed(2)}%</b>`;
                }
            }
        });

        nr.domBtnDownload.addEventListener("click", function () {
            var filename = nr.domTxtFilename.value.trim();
            if (filename == "") {
                filename = me.getLanguage(editor2) == "javascript" ? "code.js" : "style.css";
            }
            nr.download(editor2.getValue(), filename);
        });

        //接收文件
        nr.receiveFiles(function (files) {
            var file = files[0];
            var reader = new FileReader();
            reader.onload = function (e) {
                me.keepSetValue(editor1, e.target.result);

                var ext = file.name.split('.').pop().toLowerCase();
                if (ext == "js") {
                    me.setLanguage(editor1, "javascript");
                } else if (ext == "css") {
                    me.setLanguage(editor1, "css");
                }
                
                nr.domTxtFile.value = "";
            };
            reader.readAsText(file);
        }, nr.domTxtFile);
    })
}

var page = {
    options: {
        js: {},
        cleancss: {
            "compatibility": "",
            "format": false,
            "inline": [
                "local"
            ],
            "rebase": false,
            "level": {
                "0": true,
                "1": {
                    "cleanupCharsets": true,
                    "normalizeUrls": true,
                    "optimizeBackground": true,
                    "optimizeBorderRadius": true,
                    "optimizeFilter": true,
                    "optimizeFontWeight": true,
                    "optimizeOutline": true,
                    "removeEmpty": true,
                    "removeNegativePaddings": true,
                    "removeQuotes": true,
                    "removeWhitespace": true,
                    "replaceMultipleZeros": true,
                    "replaceTimeUnits": true,
                    "replaceZeroUnits": true,
                    "roundingPrecision": "",
                    "selectorsSortingMethod": "standard",
                    "specialComments": "all",
                    "tidyAtRules": true,
                    "tidyBlockScopes": true,
                    "tidySelectors": true
                },
                "2": false
            },
            "sourceMap": false
        }
    }
}