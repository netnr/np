import { nrcBase } from "../../../../frame/nrcBase";
import { nrWeb } from "../../nrWeb";
import { nrGrid } from "../../../../frame/nrGrid";
import { nrVary } from "../../nrVary";
import { nrApp } from "../../../../frame/Bootstrap/nrApp";

let nrPage = {
    pathname: '/admin/reply',

    init: async () => {
        await nrPage.viewGrid1();

        nrPage.bindEvent();
    },

    tableKey: "UrId",

    bindEvent: () => {
        //过滤
        nrVary.domTxtFilter.addEventListener('input', function () {
            if (nrPage.grid1) {
                nrPage.grid1.api.setQuickFilter(this.value);
            }
        })
    },

    viewGrid1: async () => {
        //表格加载中
        if (nrPage.grid1) {
            nrGrid.setGridLoading(nrPage.grid1);
        } else {
            nrVary.domGrid.innerHTML = nrApp.tsLoadingHtml;
        }

        let result = await nrWeb.reqServer('/Admin/ReplyList');
        if (result.code == 200) {
            //grid 列
            let colDefs = [
                nrGrid.newColumnLineNumber(),
                nrGrid.newColumnNumber({ field: "UserId", width: 120 }),
                { field: "Nickname", headerName: "昵称", },
                { field: "UrAnonymousName", headerName: "🤏匿名昵称", editable: true },
                { field: "UrAnonymousMail", headerName: "🤏匿名邮箱", editable: true },
                { field: "UrAnonymousLink", headerName: "🤏匿名链接", editable: true },
                { field: "UrId", headerName: "回复Key" },
                { field: "UrTargetType", headerName: "回复分类" },
                { field: "UrTargetId", headerName: "回复对象ID" },
                nrGrid.newColumnTextarea({
                    field: "UrContent", headerName: "🤏回复内容(HTML)", cellRenderer: params => {
                        if (params.value != null) {
                            return nrcBase.htmlEncode(params.value);
                        }
                        return params.value
                    }, editable: true,
                }),
                nrGrid.newColumnTextarea({
                    field: "UrContentMd", headerName: "🤏回复内容(MD)", cellRenderer: params => {
                        if (params.value != null) {
                            return nrcBase.htmlEncode(params.value);
                        }
                        return params.value
                    }, editable: true,
                }),
                nrGrid.newColumnDate({ headerName: "创建时间", field: "UrCreateTime", }),
                nrGrid.newColumnSet({ headerName: "🤏状态", field: "UrStatus", width: 120, editable: true, }, [{ value: 1, text: "✔" }, { value: 2, text: "Block" }]),
            ];

            //grid 配置
            let gridOptions = nrGrid.gridOptionsClient({
                rowData: result.data,
                columnDefs: colDefs,
                rowGroupPanelShow: "never",
                getRowId: params => params.data[nrPage.tableKey],
                // 单元格变动
                onCellValueChanged: async function (params) {
                    let data = params.data;
                    let fd = nrcBase.jsonToFormData(data);

                    let result = await nrWeb.reqServer('/Admin/ReplySave', { method: "POST", body: fd });

                    if (result.code == 200) {
                        nrPage.grid1.api.ensureIndexVisible(params.rowIndex); //滚动到行显示
                        nrPage.grid1.api.flashCells({ rowNodes: [params.node], columns: [params.column.colId] }); //闪烁单元格
                    } else {
                        nrApp.alert(result.msg);
                    }
                },
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
}

export { nrPage };