nr.onChangeSize = function (ch) {
    if (document.documentElement.clientWidth < 768) {
        nr.domCard0.style.height = (document.documentElement.clientHeight * 1.5) + "px";
    } else {
        nr.domCard0.style.height = (ch - nr.domCard0.offsetTop) + 'px';
    }
    page.cardSize();
}

nr.onReady = function () {
    nr.domCard0.classList.remove('invisible')

    //调整上下
    nr.domCard0.addEventListener('sl-reposition', function (e) {
        if (e.target == nr.domCard0) {
            page.cardSize()
        }
    });
    nr.domCard0.addEventListener('mousedown', function (e) {
        document.body.addEventListener('mousemove', page.moveStart, false);
        document.body.addEventListener('mouseup', page.moveEnd, false);
    });

    //初始化编辑器
    page.initEditor();

    //运行
    nr.domBtnRun.addEventListener('click', function () {
        page.preview();
    });

    //保存
    nr.domBtnSave.addEventListener('click', function () {
        page.save();
    });

    //删除
    nr.domBtnDelete.addEventListener('click', function () {
        var dialog = nr.dialog(`<h5>Confirm to delete?</h5>
        <sl-button href="/run/code/${nr.domHidCode.value}/delete" variant="danger">Confirm</sl-button>`);
        dialog.addEventListener('sl-after-hide', function () {
            dialog.remove();
        });
    });

    //搜索库
    slib.init();
}

var page = {
    cardSize: function () {
        var startH = nr.domCard0.clientHeight * (nr.domCard0.position / 100);
        if (document.body.clientWidth < 768) {
            startH = document.documentElement.clientHeight * 0.8;
        }

        nr.domCard1.style.height = startH + 'px';
        nr.domCard2.style.height = startH + 'px';

        nr.domPreHtml.style.height = startH + 'px';
        nr.domPreCss.style.height = startH + 'px';
        nr.domPreJs.style.height = startH + 'px';
        nr.domPreview.style.height = (nr.domCard0.clientHeight - startH) + 'px';
    },
    initEditor: function () {
        me.init().then(() => {
            //CSS格式化
            monaco.languages.registerDocumentFormattingEditProvider('css', {
                provideDocumentFormattingEdits: function (model, options, _token) {
                    return [{
                        text: cssFormatter(model.getValue(), options.tabSize),
                        range: model.getFullModelRange()
                    }];
                }
            });

            nr.domCard0.classList.remove('nr-pre-init');
            [nr.domPreHtml, nr.domPreCss, nr.domPreJs].forEach(function (dom) {
                let code = dom.innerText;
                let lang = dom.getAttribute('data-lang');
                switch (lang) {
                    case "html":
                        dom.innerHTML = `<sl-badge class="position-absolute end-0 top-0" style="z-index:1" variant="danger">HTML</sl-badge>`;
                        break;
                    case "css":
                        dom.innerHTML = `<sl-badge class="position-absolute end-0 top-0" style="z-index:1" variant="primary">CSS</sl-badge>`;
                        break;
                    case "javascript":
                        dom.innerHTML = `<sl-badge class="position-absolute end-0 top-0" style="z-index:1" variant="warning">JS</sl-badge>`;
                        break;
                }

                dom.editor = me.create(dom, {
                    value: code,
                    language: lang,
                    roundedSelection: true,
                    scrollBeyondLastLine: true,
                    scrollbar: {
                        verticalScrollbarSize: 0,
                        horizontalScrollbarSize: 12
                    },
                    minimap: {
                        enabled: false
                    }
                })

                //快捷键
                dom.editor.addCommand(monaco.KeyMod.CtrlCmd | monaco.KeyCode.KeyS, function () {
                    page.preview();
                })
                dom.editor.addCommand(monaco.KeyCode.PauseBreak, function () {
                    page.preview();
                })

                //载入最后一次运行
                dom.editor.addAction({
                    id: "meid-run-last",
                    label: "还原为上次运行记录",
                    keybindings: [monaco.KeyMod.CtrlCmd | monaco.KeyMod.Alt | monaco.KeyCode.KeyR],
                    contextMenuGroupId: "me-01",
                    run: function () {
                        try {
                            var json = localStorage.getItem("run_last")
                            if (json) {
                                json = JSON.parse(json);

                                nr.domPreHtml.editor.setValue(json.html);
                                nr.domPreCss.editor.setValue(json.css);
                                nr.domPreJs.editor.setValue(json.javascript);
                            }
                        } catch (ex) {
                            console.debug(ex)
                            nr.alert("error")
                        }
                    }
                });
            });

            nr.changeTheme();
            page.preview(1);
        })
    },

    preview: function (isInit) {
        nr.domPreview.innerHTML = "";

        let codeHtml = nr.domPreHtml.editor.getValue();
        let codeCss = nr.domPreCss.editor.getValue();
        let codeJs = nr.domPreJs.editor.getValue();

        //存储运行
        if (!isInit) {
            localStorage.setItem("run_last", JSON.stringify({ html: codeHtml, javascript: codeJs, css: codeCss }))
        }

        let iframe = document.createElement('iframe');
        iframe.className = "w-100 h-100";
        iframe.name = "Run preview";
        nr.domPreview.appendChild(iframe);
        if (codeHtml.includes("</head>") && codeHtml.includes("</body>")) {
            if (codeCss != "") {
                codeHtml = codeHtml.replace("</head>", `<style>${codeCss}</style></head>`);
            }
            if (codeJs != "") {
                codeHtml = codeHtml.replace("</body>", `<script>${codeJs}</script></body>`);
            }

            iframe.onload = function () {
                //提取 title
                if (!isInit && nr.domTxtDescription.value.trim() == "") {
                    var title = iframe.contentWindow.document.title;
                    nr.domTxtDescription.value = title;
                }
            }
        } else {
            iframe.onload = function () {
                if (codeCss != "") {
                    var style = document.createElement("STYLE");
                    style.innerHTML = codeCss;
                    iframe.contentWindow.document.head.appendChild(style);
                }

                if (codeJs != "") {
                    var script = document.createElement("SCRIPT");
                    script.innerHTML = codeJs;
                    iframe.contentWindow.document.body.appendChild(script);
                }

                //提取 title
                if (!isInit && nr.domTxtDescription.value.trim() == "") {
                    var title = iframe.contentWindow.document.title;
                    nr.domTxtDescription.value = title;
                }
            }
        }
        iframe.contentWindow.document.open();
        iframe.contentWindow.document.write(codeHtml);
        iframe.contentWindow.document.close();
    },

    //保存
    save: function () {
        var errMsg = [];

        var post = {
            RunCode: nr.domHidCode.value,
            RunRemark: nr.domTxtDescription.value,
            RunTheme: nr.isDark() ? "vs-dark" : "vs",
            RunContent1: nr.domPreHtml.editor.getValue(),
            RunContent2: nr.domPreJs.editor.getValue(),
            RunContent3: nr.domPreCss.editor.getValue()
        };

        if (post.RunRemark.trim() == "") {
            errMsg.push('Description is empty');
        }
        if (post.RunRemark.trim().length > 150) {
            errMsg.push('Description content is too long ( less than 150 )');
        }
        var rclen = post.RunContent1 + post.RunContent2 + post.RunContent3;
        if (rclen.length == 0) {
            errMsg.push('Code is empty');
        }

        if (rclen > 10000 * 50) {
            errMsg.push('Code content is too long ( less than 500000 )');
        }

        if (errMsg.length) {
            nr.alert(errMsg.join('<br/>'));
        } else {
            nr.domBtnSave.loading = true;
            fetch('/Run/Save', {
                method: 'POST',
                redirect: 'manual',
                body: nr.toFormData(post),
            }).then(resp => {
                nr.domBtnSave.loading = false;
                if (resp.type == "opaqueredirect") {
                    throw new Error("please login first");
                } else {
                    return resp.json()
                }
            }).then(res => {
                if (res.code == 200) {
                    nr.alert(res.msg);
                    location.href = "/run/code/" + res.data;
                } else if (res.code == 403) {
                    throw new Error("it's not belongs to you");
                } else {
                    nr.alert(res.msg);
                }
            }).catch(ex => {
                console.debug(ex);
                nr.alert(ex);
            })
        }
    },

    moveStart: function () {
        nr.domPreview.classList.add("pe-none");
    },
    moveEnd: function () {
        nr.domPreview.classList.remove("pe-none");

        document.body.removeEventListener("mousemove", page.moveStart);
        document.body.removeEventListener("mouseup", page.moveEnd);
    }
}

var slib = {
    init: function () {
        nr.domTxtSearchLibrary.addEventListener("keydown", function (e) {
            if (e.keyCode == 13) {
                slib.search(this.value.trim());
            }
        });

        nr.domDdLibrary.addEventListener('sl-select', event => {
            const selectedItem = event.detail.item;
            slib.selected(selectedItem);
            nr.domDdLibrary.show();
        });
    },
    key: null,
    buildItem: function (name, dir, type) {
        type = type ? type : "file";
        dir = dir ? dir : "";
        var empty = [], pn = dir == "" ? 0 : dir.split('/').length - 1;
        while (pn--) {
            empty.push("-")
        }
        if (empty.length) {
            empty = empty.join(' ') + " ";
        } else {
            empty = "";
        }
        if (["file", "version"].indexOf(type) == -1) {
            return `<sl-menu-label>${empty}${name}</sl-menu-label>`
        } else {
            return `<sl-menu-item data-type="${type}" data-dir="${dir + name}">${empty}${name}</sl-menu-item>`;
        }
    },
    filesEach: function (files, directoryName) {
        directoryName = directoryName || "";

        var parr = [];
        files.forEach(item => {
            if (item.type == "file") {
                parr.push(slib.buildItem(item.name, directoryName));
            } else if (item.type == "directory") {
                parr.push(slib.buildItem(item.name, directoryName, item.type));
                parr = parr.concat(arguments.callee(item.files, directoryName + item.name + "/"));
            }
        })
        return parr;
    },
    search: function (key) {
        slib.key = key;
        if (key == "") {
            nr.domSeLibrary.innerHTML = slib.buildItem("empty", "", "empty");
            return;
        }

        nr.domBtnLibrary.loading = true;
        fetch("https://data.jsdelivr.com/v1/package/npm/" + key).then(resp => resp.json()).then(res => {
            console.log(res);
            nr.domBtnLibrary.loading = false;
            var htm = [];
            if (res.versions) {
                res.versions.forEach(item => {
                    htm.push(slib.buildItem(item, "", "version"));
                })
            } else if (res.files) {
                htm = slib.filesEach(res.files);

                if (res.default) {
                    if (res.default[0] == "/") {
                        res.default = res.default.substr(1);
                    }
                    htm.splice(0, 0, slib.buildItem(res.default));
                }
            } else {
                htm.push(slib.buildItem("empty", "", "empty"));
            }
            nr.domSeLibrary.innerHTML = htm.join("");
        }).catch(err => {
            console.log(err);
            nr.domBtnLibrary.loading = false;
            nr.domSeLibrary.innerHTML = slib.buildItem("empty", "", "empty");
        })
    },
    selected: function (item) {
        switch (item.getAttribute('data-type')) {
            case "file":
                {
                    var dir = item.getAttribute('data-dir');
                    var text = `https://unpkg.com/${slib.key}/${dir}`;
                    switch (dir.split('.').pop()) {
                        case "js":
                            text = `<script src="${text}"></script>`
                            break;
                        case "css":
                            text = `<link rel="stylesheet" href="${text}" />`
                            break;
                    }
                    text += "\n";

                    var gse = nr.domPreHtml.editor.getSelection();
                    var range = new monaco.Range(gse.startLineNumber, gse.startColumn, gse.endLineNumber, gse.endColumn);
                    var op = { identifier: { major: 1, minor: 1 }, range: range, text: text, forceMoveMarkers: true };
                    nr.domPreHtml.editor.executeEdits("", [op]);
                }
                break;
            case "version":
                nr.domTxtSearchLibrary.value = slib.key + "@" + item.getAttribute('data-dir');
                slib.search(nr.domTxtSearchLibrary.value);
                break;
        }
    }
}

function cssFormatter(css, tabSize) {
    try {
        return beautifier.css(css, {
            indent_size: tabSize || 4,
            "max-preserve-newlines": 2
        });
    } catch (e) {
        console.log(e);
        return null;
    }
}