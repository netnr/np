import { nrcBase } from "../../../../frame/nrcBase";
import { nrWeb } from "../../nrWeb";
import { nrGrid } from "../../../../frame/nrGrid";
import { nrVary } from "../../nrVary";
import { nrApp } from "../../../../frame/Bootstrap/nrApp";
import { nrcRely } from "../../../../frame/nrcRely";

let nrPage = {
    pathname: '/user/write',

    init: async () => {
        await nrPage.viewGrid1();

        await nrPage.viewModalForm();

        nrPage.bindEvent();
    },

    event_resize: (ch) => {
        if (nrApp.tsMd) {
            let vh = ch - nrApp.tsMd.domContainer.getBoundingClientRect().top - 40;
            nrApp.tsMd.height(Math.max(200, vh));
        }
    },

    tableKey: "UwId",
    rowId: null,

    bindEvent: () => {
        //过滤
        nrVary.domTxtFilter.addEventListener('input', function () {
            if (nrPage.grid1) {
                nrPage.grid1.api.setQuickFilter(this.value);
            }
        })

        //编辑
        nrVary.domBtnEdit.addEventListener('click', async function () {
            let srows = nrPage.grid1.api.getSelectedRows();
            if (srows.length == 1) {
                if (nrPage.modalForm == null) {
                    nrPage.modalForm = new bootstrap.Modal(nrVary.domModalForm, { keyboard: false, });
                    nrVary.domModalForm.addEventListener('shown.bs.modal', function () {
                        nrApp.tsMd.height(document.documentElement.clientHeight - nrVary.domEditor.getBoundingClientRect().top - 25);
                    })
                }
                let row = srows[0];
                nrPage.rowId = row[nrPage.tableKey];

                nrApp.setLoading(this)

                let result = await nrWeb.reqServer(`/User/WriteOne?id=${nrPage.rowId}`);
                nrApp.setLoading(this, true);

                if (result.code == 200) {
                    let item = result.data.item;
                    nrVary.domTxtTitle.value = item.UwTitle;
                    nrApp.tsMd.setmd(item.UwContentMd);

                    let tags = result.data.tags.map(x => ({ value: x.TagId, label: x.TagName }))
                    nrVary.tag1.removeActiveItems();
                    nrVary.tag1.setValue(tags);
                }

                nrPage.modalForm.show();
            } else {
                nrApp.alert('请选择一行');
            }
        });

        //删除
        nrVary.domBtnDel.addEventListener('click', async function () {
            let srows = nrPage.grid1.api.getSelectedRows();
            if (srows.length) {
                if (await nrApp.confirm(srows.map(x => `${x[nrPage.tableKey]}.${x["UwTitle"]}`).join('<hr/>'), `确定删除（${srows.length}）`)) {
                    nrApp.setLoading(this)

                    let url = `/User/WriteDel?ids=${srows.map(x => x[nrPage.tableKey]).join()}`;
                    let result = await nrWeb.reqServer(url);

                    nrApp.setLoading(this, true)

                    if (result.code == 200) {
                        nrPage.viewGrid1();
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
            let obj = {
                UwId: nrPage.rowId,
                UwTitle: nrVary.domTxtTitle.value,
                UwCategory: 0,
                UwContent: nrApp.tsMd.gethtml(),
                UwContentMd: nrApp.tsMd.getmd(),
                TagIds: nrVary.tag1.getValue(true).join()
            }

            let errMsg = [];
            if (obj.UwTitle == "") {
                errMsg.push("请输入 标题");
            }
            if (obj.TagIds == "") {
                errMsg.push("请选择 标签");
            }
            if (obj.UwContentMd.length < 20) {
                errMsg.push("多写一点内容哦");
            }

            if (errMsg.length > 0) {
                nrApp.alert(errMsg.join('<hr/>'));
            } else {
                nrApp.setLoading(nrVary.domBtnSave);

                let fd = nrcBase.jsonToFormData(obj);

                let result = await nrWeb.reqServer('/User/WriteSave', { method: 'POST', body: fd });
                nrApp.setLoading(nrVary.domBtnSave, true);

                if (result.code == 200) {
                    nrApp.toast('保存成功');
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

        let result = await nrWeb.reqServer('/User/WriteList');
        if (result.code == 200) {
            //grid 列
            let colDefs = [
                nrGrid.newColumnLineNumber(),
                nrGrid.newColumnNumber({ headerName: "文章ID", field: nrPage.tableKey, width: 120 }),
                {
                    headerName: "标题", field: "UwTitle", width: 520, cellRenderer: (params) => {
                        if (params.data) {
                            return `<a href="/home/list/${params.data[nrPage.tableKey]}">${params.value}</a>`;
                        }
                    }
                },
                nrGrid.newColumnDate({ headerName: "创建时间", field: "UwCreateTime", }),
                nrGrid.newColumnDate({ headerName: "修改时间", field: "UwUpdateTime", }),
                nrGrid.newColumnNumber({ headerName: "回复", field: "UwReplyNum", width: 110 }),
                nrGrid.newColumnNumber({ headerName: "浏览", field: "UwReadNum", width: 110 }),
                nrGrid.newColumnNumber({ headerName: "点赞", field: "UwLaud", width: 110 }),
                nrGrid.newColumnNumber({ headerName: "收藏", field: "UwMark", width: 110 }),
                nrGrid.newColumnSet({ headerName: "公开", field: "UwOpen", width: 110 }, [{ value: 1, text: "✔" }, { value: 2, text: "✘" }]),
                nrGrid.newColumnSet({ headerName: "状态", field: "UwStatus", width: 110 }, [{ value: 1, text: "✔" }, { value: 2, text: "Block" }, { value: -1, text: "Lock" }]),
            ];

            //grid 配置
            let gridOptions = nrGrid.gridOptionsClient({
                rowData: result.data,
                columnDefs: colDefs,
                rowGroupPanelShow: "never",
                getRowId: params => params.data[nrPage.tableKey],
                //双击
                onCellDoubleClicked: async function () {
                    nrVary.domBtnEdit.click();
                }
            });

            //grid dom
            nrGrid.buildDom(nrVary.domGrid);
            nrcBase.setHeightFromBottom(nrVary.domGrid);

            //grid 显示
            nrPage.grid1 = await nrGrid.viewGrid(nrVary.domGrid, gridOptions);
        } else {
            nrVary.domGrid.innerHTML = nrApp.tsFailHtml;
        }
    },

    //form
    viewModalForm: async () => {
        //标签
        let result = await nrWeb.reqServer('/Home/TagSelect');
        if (result.code == 200 && result.data) {
            await nrcRely.remote('choices');

            nrVary.domSeTags.multiple = true;
            nrVary.tag1 = new Choices(nrVary.domSeTags, {
                allowHTML: true,
                maxItemCount: 3,
                removeItemButton: true,
                noResultsText: '咣',
                noChoicesText: '（空）',
                itemSelectText: '点击选择',
                maxItemText: (maxItemCount) => `最多选择 ${maxItemCount} 项`,
            });
            nrVary.tag1.dropdown.element.classList.add('rounded');
            nrVary.tag1.containerInner.element.classList.add('rounded');
            nrVary.tag1.setChoices(() => {
                return result.data.map(item => ({ value: item.TagId, label: item.TagName }))
            })
        }

        //编辑器
        await nrcRely.remote("netnrmdAce.js");
        await nrcRely.remote("netnrmd");
        nrApp.tsMd = netnrmd.init('.nrg-editor', {
            theme: nrcBase.isDark() ? "dark" : "light"
        });

        //快捷键
        nrApp.tsMd.addCommand("Ctrl+S", () => nrVary.domBtnSave.click());

        //高度
        nrVary.domModalForm.addEventListener('shown.bs.modal', function () {
            let vh = document.documentElement.clientHeight - nrApp.tsMd.domContainer.getBoundingClientRect().top - 30;
            nrApp.tsMd.height(vh);
        });
    }
}

export { nrPage };