nr.onChangeSize = function (ch) {
    if (nr.nmd) {
        var vh = ch - nr.nmd.obj.container.getBoundingClientRect().top - 30;
        nr.nmd.height(Math.max(100, vh));
    }
}

nr.onReady = function () {
    page.initEditor();
    page.initMenuTree(nr.domHidCode.value);

    nr.domBtnSave.addEventListener("click", function () {
        page.save();
    });

    nr.domBtnTemplateApi.addEventListener("click", function () {
        page.insertTemplate('api')
    });
    nr.domBtnTemplateDic.addEventListener("click", function () {
        page.insertTemplate('dic')
    });
}

var page = {
    initEditor: function () {
        require(['vs/editor/editor.main'], function () {
            var code = nr.domEditor.getAttribute("data-value");
            nr.nmd = new netnrmd('.nr-editor', { autosave: false });

            nr.changeTheme();
            nr.changeSize();

            nr.nmd.setmd(code);

            //快捷键
            nr.nmd.obj.me.addCommand(monaco.KeyMod.CtrlCmd | monaco.KeyCode.KeyS, function () {
                nr.domBtnSave.click();
            })
        });
    },
    initMenuTree: function (code) {
        fetch(`/Doc/MenuTree/${code}`).then(resp => resp.json()).then(res => {
            if (res.code == 200) {

                var tree = (function (json, deep, ptitle) {
                    var arr = [], deep = deep || 0, ptitle = ptitle || [];
                    for (var i = 0; i < json.length; i++) {
                        var ji = json[i], child = ji.children;
                        if (!ji.IsCatalog) {
                            continue
                        }
                        var obj = {};
                        ptitle.push(ji.DsdTitle);
                        obj[ji.DsdId] = ptitle.join(' / ');
                        arr.push(obj);
                        if (child) {
                            deep += 1;
                            var arrc = arguments.callee(child, deep, ptitle);
                            if (arrc.length) {
                                arr = arr.concat(arrc);
                            }
                            deep -= 1;
                        }
                        ptitle.length = deep;
                    }
                    return arr;
                })(res.data) || [];

                var dmarr = [];
                dmarr.push('<sl-menu-item value="">(none)</sl-menu-item><sl-divider></sl-divider>');
                tree.forEach(function (item) {
                    for (let key in item) {
                        dmarr.push(`<sl-menu-item value="${key}">${item[key]}</sl-menu-item>`);
                    }
                })
                nr.domSePid.innerHTML = dmarr.join('');
            }
        })
    },
    save: function () {
        var obj = {
            DsdContentMd: nr.nmd.getmd(),
            DsdContentHtml: nr.nmd.gethtml(),
        }

        document.querySelectorAll('input,sl-input').forEach(dom => {
            if (dom.name) {
                obj[dom.name] = dom.value.trim();
            }
        })

        var errMsg = [];
        if (obj.DsdTitle == "") {
            errMsg.push('标题 必填')
        }
        if (obj.DsdContentMd == "") {
            errMsg.push("内容 必填");
        }
        if (errMsg.length) {
            nr.alert(errMsg.join('</br>'));
        } else {
            nr.domBtnSave.loading = true;
            fetch('/Doc/ItemSave', {
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
            }).then(function (res) {
                nr.alert(res.msg)
                if (res.code == 200) {
                    nr.domHidId.value = res.data;
                }
            }).catch(ex => {
                console.debug(ex);
                nr.alert(ex);
            })
        }
    },
    insertTemplate: function (name) {
        fetch(`/file/template/doc_${name}.md`).then(resp => resp.text()).then(res => {
            if (nr.nmd) {
                netnrmd.insertAfterText(nr.nmd.obj.me, res);
            }
        })
    }
}