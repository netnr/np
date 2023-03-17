import { nrcBase } from "../../../../frame/nrcBase";
import { nrApp } from "../../../../frame/Bootstrap/nrApp";
import { nrcRely } from "../../../../frame/nrcRely";
import { nrWeb } from "../../nrWeb";
import { nrVary } from "../../nrVary";

let nrPage = {
    pathname: '/doc/catalog/*',

    init: async () => {
        nrcBase.setHeightFromBottom(nrVary.domTree);
        
        await nrPage.viewTree();
        nrPage.bindEvent();
    },

    rowId: location.pathname.split('/').pop(),

    bindEvent: () => {
        let domForm = nrVary.domModalForm.querySelector('form');

        //新建
        nrVary.domBtnAdd.addEventListener('click', function () {
            if (nrPage.modalForm == null) {
                nrPage.modalForm = new bootstrap.Modal(nrVary.domModalForm, { keyboard: false, });
            }
            nrPage.modalForm.show();
            domForm.reset();
        })

        //新加确定
        domForm.addEventListener('submit', async function (event) {
            event.preventDefault();

            let obj = nrcBase.getFormJson(this);

            let newData = {
                id: nrcBase.snow(),
                text: obj.DsdTitle,
                type: obj.DsdType == "file" ? "file" : "menu",
            }
            newData["data"] = {
                DsdId: newData.id,
                DsdTitle: newData.text,
                DsdPid: obj.DsdPid,
                IsCatalog: obj.DsdType != "file",
            }

            //添加到根节点
            nrPage.tree.create_node(null, newData);
            nrPage.modalForm.hide();
        })

        //保存
        nrVary.domBtnSave.addEventListener("click", async function () {
            nrApp.setLoading(this);

            let allNode = nrPage.tree.get_json(null, { flat: true });
            let rows = [];
            let order = 0;
            allNode.forEach(function (node) {
                rows.push({
                    DsdId: node.id,
                    DsdTitle: node.text,
                    DsdPid: node.parent == "#" ? nrVary.flagGuidEmpty : node.parent,
                    IsCatalog: node.type != "file",
                    DsdOrder: order += 5
                })
            });

            let fd = new FormData();
            fd.append("rows", JSON.stringify(rows));

            let url = `/Doc/SaveCatalog/${nrPage.rowId}`;
            let result = await nrWeb.reqServer(url, { method: "POST", body: fd });
            nrApp.setLoading(this, true);

            if (result.code == 200) {
                nrApp.toast('保存成功')
            } else {
                nrApp.alert(result.msg);
            }
        })
    },

    /**
     * 显示树
     */
    viewTree: async () => {
        nrApp.setLoading(nrVary.domBtnSave);
        nrVary.domTree.innerHTML = nrApp.tsLoadingHtml;

        let result = await nrWeb.reqServer(`/Doc/MenuTree/${nrPage.rowId}/parent`);
        await nrcRely.remote('jstree');

        nrApp.setLoading(nrVary.domBtnSave, true);
        nrVary.domTree.innerHTML = '';

        if (result.code == 200) {
            //树 数组
            let treeData = [];
            result.data.forEach(item => {
                treeData.push({
                    id: item.DsdId,
                    text: item.DsdTitle,
                    type: item.IsCatalog ? "menu" : "file",
                    parent: item.DsdPid == nrVary.flagGuidEmpty ? "#" : item.DsdPid,
                    data: item,
                    state: {
                        opened: true
                    }
                });
            })

            //树 构建
            $('.nrg-tree').jstree({
                core: {
                    themes: {
                        name: nrcBase.isDark() ? "default-dark" : "default",
                    },
                    data: treeData,
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
                                        nrApp.toast('请选择目录');
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
                        let obj = {
                            Rename: {
                                "separator_before": false,
                                "separator_after": false,
                                "label": "重命名",
                                "action": function () {
                                    nrPage.tree.edit(data);
                                }
                            },
                            Remove: {
                                "separator_before": false,
                                "separator_after": false,
                                "label": "删除",
                                "action": function () {
                                    nrPage.tree.delete_node(data);
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
                                        let newData = {
                                            id: nrcBase.snow(),
                                            text: "新目录",
                                            type: "menu"
                                        }
                                        newData["data"] = {
                                            DsdId: newData.id,
                                            DsdTitle: newData.text,
                                            DsdPid: data.id,
                                            IsCatalog: true
                                        }
                                        let en = nrPage.tree.create_node(data, newData);
                                        nrPage.tree.edit(en);
                                    }
                                },
                                CreateFile: {
                                    separator_before: false,
                                    separator_after: false,
                                    label: "新建页面",
                                    action: function () {
                                        let newData = {
                                            id: nrcBase.snow(),
                                            text: "新页面",
                                            type: "file"
                                        }
                                        newData["data"] = {
                                            DsdId: newData.id,
                                            DsdTitle: newData.text,
                                            DsdPid: data.id,
                                            IsCatalog: false
                                        }
                                        let en = nrPage.tree.create_node(data, newData);
                                        nrPage.tree.edit(en);
                                    }
                                }
                            });
                        }

                        return obj;
                    }
                },
                plugins: ["contextmenu", "dnd", "types"]
            });

            nrPage.tree = $('.nrg-tree').jstree(true);
            nrVary.domTree.tree = nrPage.tree;
        } else {
            nrApp.alert(result.msg);
        }
    },
}

export { nrPage };