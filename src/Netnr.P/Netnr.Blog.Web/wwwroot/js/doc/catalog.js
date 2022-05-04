nr.onChangeSize = function (ch) {
    nr.domTree.style.height = (ch - nr.domTree.getBoundingClientRect().top - 25) + "px";
}

nr.onChangeTheme = function () {
    if (page.tree) {
        page.tree.set_theme(nr.isDark() ? "default-dark" : "default");
    }
}

nr.onReady = function () {
    page.load();

    //tree 新增
    nr.domBtnAdd.addEventListener("click", function () {
        nr.domDialogForm.querySelectorAll('sl-input,sl-select').forEach(function (input) {
            input.value = '';
        });
        page.sdata = null;

        nr.domSePid.innerHTML = `<sl-menu-item value="#">根目录</sl-menu-item>`;
        nr.domSePid.value = "#";

        var snode = page.tree.get_selected();
        if (snode.length) {
            var sdata = page.tree.get_node(snode[0]);
            if (sdata.data.IsCatalog) {
                page.sdata = sdata;
                nr.domSePid.innerHTML = `<sl-menu-item value="#">根目录</sl-menu-item><sl-menu-item value="${sdata.id}"><i class="fa fa-folder-open text-warning mx-2"></i>${sdata.text}</sl-menu-item>`;
                nr.domSePid.value = sdata.id;
            }
        }

        nr.domDialogForm.show();
    });

    //tree 确定
    nr.domBtnConfirm.addEventListener("click", function () {
        var type = nr.domSeType.value;
        var pid = nr.domSePid.value;
        var newData = {
            id: page.newLong(),
            text: nr.domTxtTitle.value.trim(),
            type: type == "file" ? "file" : "menu",
        }
        newData["data"] = {
            DsdId: newData.id,
            DsdTitle: newData.text,
            DsdPid: pid == "#" ? "#" : pid,
            IsCatalog: type != "file",
        }

        if (newData.text == "") {
            nr.alert("请输入名称");
        } else {
            page.tree.create_node(pid == "#" ? null : page.sdata, newData);
            nr.domDialogForm.hide();
        }
    });

    //保存
    nr.domBtnSave.addEventListener("click", function () {
        var allNode = page.tree.get_json(null, { flat: true });
        var rows = [], order = 0;
        allNode.forEach(function (node) {
            rows.push({
                DsdId: node.id,
                DsdTitle: node.text,
                DsdPid: node.parent == "#" ? "00000000-0000-0000-0000-000000000000" : node.parent,
                IsCatalog: node.type != "file",
                DsdOrder: order += 5
            })
        });

        nr.domBtnSave.loading = true;
        fetch(`/Doc/SaveCatalog/${nr.domHidCode.value}`, {
            method: 'POST',
            body: nr.toFormData({
                rows: JSON.stringify(rows)
            })
        }).then(resp => {
            nr.domBtnSave.loading = false;
            return resp.json();
        }).then(function (res) {
            nr.alert(res.msg);
        }).catch(ex => {
            console.debug(ex);
            nr.alert(ex);
        })
    })
}

var page = {
    load: function () {
        var code = location.pathname.split('/').pop();
        fetch(`/Doc/MenuTree/${code}/parent`).then(resp => resp.json()).then(res => {
            if (res.code == 200) {
                var json = [];
                res.data.forEach(item => {
                    json.push({
                        id: item.DsdId,
                        text: item.DsdTitle,
                        type: item.IsCatalog ? "menu" : "file",
                        parent: item.DsdPid == "00000000-0000-0000-0000-000000000000" ? "#" : item.DsdPid,
                        data: item,
                        state: {
                            opened: true
                        }
                    });
                })

                page.viewTree(json);
            }
        })
    },
    viewTree: function (tree) {
        $('.nr-tree').jstree({
            core: {
                themes: {
                    name: nr.isDark() ? "default-dark" : "default",
                },
                data: tree,
                check_callback: function (operation, node, node_parent) {
                    switch (operation) {
                        case "move_node":
                            if (node_parent.data && !node_parent.data.IsCatalog) {
                                return false
                            }
                            break;
                        case "create_node":
                            if (node_parent.data) {
                                if (!node_parent.data.IsCatalog) {
                                    nr.alert('请选择目录');
                                }
                                return node_parent.data.IsCatalog;
                            }
                            break;
                    }
                    return true;
                }
            },
            types: {
                menu: {
                    icon: "fa fa-folder-open text-warning"
                },
                file: {
                    "icon": "fa fa-file-text text-secondary"
                }
            },
            contextmenu: {
                items: function (data) {
                    var obj = {
                        Rename: {
                            "separator_before": false,
                            "separator_after": false,
                            "label": "重命名",
                            "action": function () {
                                page.tree.edit(data);
                            }
                        },
                        Remove: {
                            "separator_before": false,
                            "separator_after": false,
                            "label": "删除",
                            "action": function () {
                                page.tree.delete_node(data);
                            }
                        }
                    };

                    if (data.data.IsCatalog) {
                        Object.assign(obj, {
                            CreateMenu: {
                                separator_before: false,
                                separator_after: false,
                                label: "新建子目录",
                                action: function () {
                                    var newData = {
                                        id: page.newLong(),
                                        text: "新目录",
                                        type: "menu"
                                    }
                                    newData["data"] = {
                                        DsdId: newData.id,
                                        DsdTitle: newData.text,
                                        DsdPid: data.id,
                                        IsCatalog: true
                                    }
                                    var en = page.tree.create_node(data, newData);
                                    page.tree.edit(en);
                                }
                            },
                            CreateFile: {
                                separator_before: false,
                                separator_after: false,
                                label: "新建文件",
                                action: function () {
                                    var newData = {
                                        id: page.newLong(),
                                        text: "新文件",
                                        type: "file"
                                    }
                                    newData["data"] = {
                                        DsdId: newData.id,
                                        DsdTitle: newData.text,
                                        DsdPid: data.id,
                                        IsCatalog: false
                                    }
                                    var en = page.tree.create_node(data, newData);
                                    page.tree.edit(en);
                                }
                            }
                        });
                    }

                    return obj;
                }
            },
            plugins: ["contextmenu", "dnd", "types"]
        });

        page.tree = $('.nr-tree').jstree(true);
    },
    newLong: function () {
        return "30000" + Math.random().toString().substring(2, 16);
    }
}