import { nrcBase } from "../../../../frame/nrcBase";
import { nrWeb } from "../../nrWeb";
import { nrGrid } from "../../../../frame/nrGrid";
import { nrVary } from "../../nrVary";
import { nrApp } from "../../../../frame/Bootstrap/nrApp";
import { nrcRely } from "../../../../frame/nrcRely";

let nrPage = {
    pathname: '/note/index',

    init: async () => {
        await nrPage.viewGrid1();

        await nrPage.viewForm();

        nrPage.bindEvent();
    },

    event_resize: (ch) => {
        if (nrApp.tsMd) {
            let vh = ch - nrApp.tsMd.domContainer.getBoundingClientRect().top - nrcBase.tsBottomKeepHeight;
            nrApp.tsMd.height(Math.max(200, vh));
        }
    },

    tableKey: "NoteId",
    rowId: 0,

    bindEvent: () => {
        //过滤
        nrApp.setQuickFilter(nrVary.domTxtFilter, nrPage.grid1);

        //新增
        nrVary.domBtnAdd.addEventListener('click', function () {
            nrPage.grid1.deselectAll()
            nrPage.grid1.clearRangeSelection();
            nrPage.viewAdd();
        });

        //删除
        nrVary.domBtnDel.addEventListener('click', async function () {
            let srows = nrPage.grid1.getSelectedRows();
            if (srows.length) {
                if (await nrApp.confirm(srows.map(x => `${x["NoteTitle"]}`).join('<hr/>'), `确定删除（${srows.length}）`)) {
                    nrApp.setLoading(this)

                    let url = `/Note/DelNote?ids=${srows.map(x => x[nrPage.tableKey]).join()}`;
                    let result = await nrWeb.reqServer(url);

                    nrApp.setLoading(this, true)

                    if (result.code == 200) {
                        nrPage.viewGrid1();

                        nrPage.viewAdd();
                    } else {
                        nrApp.alert(result.msg);
                    }
                }
            } else {
                nrApp.alert('请选择行');
            }
        })

        //保存
        nrVary.domBtnSave.addEventListener("click", async function () {

            let title = nrVary.domTxtTitle.value.trim();
            let md = nrApp.tsMd.getmd();

            let errMsg = [];
            if (title == "") {
                errMsg.push("请输入 标题");
            }
            if (md.length < 2) {
                errMsg.push("多写一点内容哦");
            }

            if (errMsg.length > 0) {
                nrApp.alert(errMsg.join('<hr/>'));
            } else {

                let fd = new FormData();
                fd.append("NoteId", nrPage.rowId);
                fd.append("NoteTitle", title);
                fd.append("NoteContent", md);

                nrApp.setLoading(nrVary.domBtnSave);

                let result = await nrWeb.reqServer('/Note/SaveNote', { method: 'POST', body: fd });
                nrApp.setLoading(nrVary.domBtnSave, true);

                if (result.code == 200) {
                    nrApp.toast('保存成功');

                    //新增
                    if (nrPage.rowId == 0) {
                        await nrPage.viewGrid1();
                        nrPage.grid1.forEachNode((rowNode) => {
                            if (rowNode.data[nrPage.tableKey] == result.data) {
                                rowNode.setSelected(true)
                                nrPage.viewEdit(rowNode.data);
                            }
                        })
                    } else {
                        let srow = nrPage.grid1.getSelectedRows().find(x => x[nrPage.tableKey] == nrPage.rowId);
                        if (srow) {
                            srow["NoteTitle"] = title;
                            nrPage.grid1.applyTransactionAsync({ update: [srow] })
                        }
                    }
                } else {
                    nrApp.alert(result.msg);
                }
            }
        });
    },

    viewGrid1: async () => {
        //表格加载中
        if (nrPage.grid1) {
            nrGrid.setGridLoading(nrPage.grid1);
        } else {
            nrVary.domGrid.innerHTML = nrApp.tsLoadingHtml;
        }

        let result = await nrWeb.reqServer('/Note/NoteList');
        if (result.code == 200) {
            //grid 列
            let colDefs = [
                { field: "NoteTitle", headerName: "标题", flex: 1 },
                nrGrid.newColumnLineNumber({ hide: true }),
            ];

            //grid 配置
            let gridOptions = nrGrid.gridOptionsClient({
                rowData: result.data,
                columnDefs: colDefs,
                rowGroupPanelShow: "never",
                getRowId: params => params.data[nrPage.tableKey],
                //点击
                onCellClicked: async function (params) {
                    if (params.data) {
                        await nrPage.viewEdit(params.data);
                    }
                }
            });

            //grid dom
            nrGrid.buildDom(nrVary.domGrid);
            nrcBase.setHeightFromBottom(nrVary.domGrid);

            //grid 显示
            nrPage.grid1 = await nrGrid.createGrid(nrVary.domGrid, gridOptions);
        } else {
            nrVary.domGrid.innerHTML = nrApp.tsFailHtml;
        }
    },

    //form
    viewForm: async () => {
        //编辑器
        await nrcRely.remote("netnrmdEditor");
        await nrcRely.remote("netnrmd");
        nrApp.tsMd = netnrmd.init('.nrg-editor', {
            theme: nrcBase.isDark() ? "dark" : "light",
            autosave: false,
            input: function () {
                nrVary.domWordCount.querySelector('b').innerHTML = this.getmd().length;
            },
        });
        //快捷键
        nrApp.tsMd.addCommand("Ctrl+S", () => nrVary.domBtnSave.click());

        //高度
        let vh = document.documentElement.clientHeight - nrApp.tsMd.domContainer.getBoundingClientRect().top - 26;
        nrApp.tsMd.height(Math.max(200, vh));
    },

    viewAdd: () => {
        nrVary.domCardForm.classList.remove('invisible');
        nrPage.rowId = 0;
        nrVary.domTxtTitle.value = "";

        if (nrApp.tsMd) {
            nrApp.tsMd.setmd("");
        }
        nrVary.domTimeInfo.classList.add('d-none');
    },

    viewEdit: async (data) => {
        nrPage.viewAdd(); //先清空

        nrPage.rowId = data[nrPage.tableKey];
        nrVary.domTxtTitle.value = data["NoteTitle"];

        nrVary.domTimeInfo.classList.remove('d-none');
        nrVary.domTimeInfo.innerHTML = `创建时间：${nrcBase.formatDateTime('datetime', data["NoteCreateTime"])} ，修改时间：${nrcBase.formatDateTime('datetime', data["NoteUpdateTime"])}`

        if (nrApp.tsMd) {
            nrApp.tsMd.setmd("");
        }

        nrApp.setLoading(nrVary.domBtnSave);
        let result = await nrWeb.reqServer(`/Note/QueryNoteOne?id=${nrPage.rowId}`);
        nrApp.setLoading(nrVary.domBtnSave, true);
        if (result.code == 200 && result.data) {
            if (nrApp.tsMd) {
                nrApp.tsMd.setmd(result.data["NoteContent"]);
                nrVary.domTimeInfo.innerHTML = `创建时间：${nrcBase.formatDateTime('datetime', result.data["NoteCreateTime"])} ，修改时间：${nrcBase.formatDateTime('datetime', result.data["NoteUpdateTime"])}`
            }
        }
    }
}

export { nrPage };