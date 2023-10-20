import { nrcBase } from "../../../../frame/nrcBase";
import { nrWeb } from "../../nrWeb";
import { nrGrid } from "../../../../frame/nrGrid";
import { nrVary } from "../../nrVary";
import { nrApp } from "../../../../frame/Bootstrap/nrApp";

let nrPage = {
    pathname: '/admin/write',

    init: async () => {
        await nrPage.viewGrid1();
        nrPage.bindEvent();
    },

    tableKey: "UwId",

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

        let result = await nrWeb.reqServer('/Admin/WriteList');
        if (result.code == 200) {
            //grid 列
            let colDefs = [
                nrGrid.newColumnLineNumber(),
                nrGrid.newColumnNumber({ field: "UserId", width: 120 }),
                { field: "Nickname", headerName: "昵称", },
                nrGrid.newColumnNumber({ headerName: "文章ID", field: nrPage.tableKey, width: 120 }),
                nrGrid.newColumnTextarea({
                    headerName: "🤏标题", field: "UwTitle", width: 520, cellRenderer: (params) => {
                        if (params.data) {
                            return `<a href="/home/list/${params.data[nrPage.tableKey]}">${params.value}</a>`;
                        }
                    }, editable: true,
                }, 200),
                nrGrid.newColumnDate({ headerName: "创建时间", field: "UwCreateTime", }),
                nrGrid.newColumnDate({ headerName: "修改时间", field: "UwUpdateTime", }),
                nrGrid.newColumnNumber({ headerName: "🤏回复", field: "UwReplyNum", width: 120, editable: true, }),
                nrGrid.newColumnNumber({ headerName: "🤏浏览", field: "UwReadNum", width: 120, editable: true, }),
                nrGrid.newColumnNumber({ headerName: "🤏点赞", field: "UwLaud", width: 120, editable: true, }),
                nrGrid.newColumnNumber({ headerName: "🤏收藏", field: "UwMark", width: 120, editable: true, }),
                nrGrid.newColumnSet({ headerName: "🤏公开", field: "UwOpen", width: 120, editable: true, }, [{ value: 1, text: "✔" }, { value: 2, text: "✘" }]),
                nrGrid.newColumnSet({ headerName: "🤏状态", field: "UwStatus", width: 120, editable: true, }, [{ value: 1, text: "✔" }, { value: 2, text: "Block" }, { value: -1, text: "Lock" }]),
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
                    let fd = nrcBase.fromKeyToFormData(data);

                    let result = await nrWeb.reqServer('/Admin/WriteSave', { method: "POST", body: fd });

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