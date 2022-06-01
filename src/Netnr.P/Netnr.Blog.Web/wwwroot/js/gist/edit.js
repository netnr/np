nr.onChangeSize = function (ch) {
    nr.domEditor.style.height = (ch - nr.domEditor.getBoundingClientRect().top - 80) + "px";
}

nr.onReady = function () {
    page.initEditor();

    //保存
    nr.domBtnSave.addEventListener('click', function () {
        page.save();
    });
}

var page = {
    initEditor: function () {
        me.init().then(() => {
            var modesIds = monaco.languages.getLanguages().map(lang => lang.id).sort();
            modesIds = modesIds.filter(x => !x.includes('.'));

            //语言列表
            modesIds.forEach(language => {
                var domItem = document.createElement("sl-menu-item");
                domItem.value = language;
                domItem.innerHTML = language;
                nr.domSeLanguage.appendChild(domItem);
            });

            //新增，恢复
            if (location.pathname == "/gist") {
                nr.domHidContent.value = nr.lsStr("content");
            }

            page.editor = me.create(nr.domEditor, {
                value: nr.domHidContent.value,
                language: nr.domSeLanguage.value,
                scrollbar: {
                    verticalScrollbarSize: 0,
                    horizontalScrollbarSize: 12
                },
                minimap: {
                    enabled: true
                }
            });

            //新增，自动保存
            if (location.pathname == "/gist") {
                page.editor.onDidChangeModelContent(function (e) {
                    clearTimeout(window.defer1);
                    window.defer1 = setTimeout(function () {
                        nr.ls["content"] = page.editor.getValue();
                        nr.lsSave();
                    }, 1000 * 1)
                });
            }

            nr.domBtnSave.classList.remove('d-none');
            nr.changeSize();
            nr.changeTheme();

            //语言切换
            nr.domSeLanguage.addEventListener('sl-change', function () {
                me.setLanguage(page.editor, this.value);
            });

            //快捷键
            page.editor.addCommand(monaco.KeyMod.CtrlCmd | monaco.KeyCode.KeyS, function () {
                nr.domBtnSave.click();
            })
        });
    },
    save: function () {
        var code = page.editor.getValue(), arrv = code.split('\n'), row = arrv.length, msg = [];
        var obj = {
            GistCode: nr.domHidCode.value,
            GistRemark: nr.domTxtRemark.value,
            GistFilename: nr.domTxtFilename.value,
            GistLanguage: nr.domSeLanguage.value,
            GistTheme: nr.isDark() ? "vs-dark" : "vs",
            GistContent: code,
            GistContentPreview: arrv.slice(0, 10).join('\n'),
            GistRow: row,
            GistOpen: 1
        };

        var errMsg = [];
        if (obj.GistRemark.trim() == "") {
            errMsg.push('Gist description');
        }
        if (obj.GistLanguage == "") {
            errMsg.push('Gist language');
        }
        if (obj.GistFilename.trim() == "") {
            errMsg.push('Filename including extension');
        }
        if (obj.GistContent.trim() == "") {
            errMsg.push('Gist content');
        }

        if (obj.GistContent.length > 10000 * 50) {
            errMsg.push('Gist content is too long ( less than 500000 )');
        }

        if (errMsg.length) {
            nr.alert(errMsg.join('<br/>'));
        } else {
            nr.domBtnSave.loading = true;
            fetch('/Gist/Save', {
                method: 'POST',
                redirect: 'manual',
                body: nr.toFormData(obj)
            }).then(resp => {
                nr.domBtnSave.loading = false;
                if (resp.type == "opaqueredirect") {
                    throw new Error("please login first");
                } else {
                    return resp.json()
                }
            }).then(res => {
                if (res.code == 200) {
                    nr.ls["content"] = "";
                    nr.lsSave();

                    location.href = "/gist/code/" + res.data;
                } else {
                    nr.alert(res.msg);
                }
            }).catch(ex => {
                console.debug(ex)
                nr.alert(ex);
            })
        }
    }
}