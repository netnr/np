import { nrcAgGridQuery } from "../../../../frame/nrcAgGridQuery";
import { nrApp } from "../../../../frame/Bootstrap/nrApp";
import { nrcBase } from "../../../../frame/nrcBase";
import { nrGrid } from "../../../../frame/nrGrid";
import { nrVary } from "../../nrVary";

let nrPage = {
    pathname: '/admin/log',

    init: async () => {
        await nrPage.viewGrid1();
    },

    tableKey: 'LogId',

    viewGrid1: async () => {
        //表格加载中
        if (nrPage.grid1) {
            nrGrid.setGridLoading(nrPage.grid1);
        } else {
            nrVary.domGrid.innerHTML = nrApp.tsLoadingHtml;
        }

        try {
            //grid 列
            let colDefs = [
                nrGrid.newColumnLineNumber({ headerCheckboxSelection: false }),
                nrGrid.newColumnNumber({ field: "LogUid", }),
                { field: "LogNickname", },
                {
                    field: "LogAction", cellRenderer: params => {
                        if (!params.node.group) {
                            try {
                                return decodeURIComponent(params.value)
                            } catch (e) { }
                        }
                        return params.value;
                    }
                },
                { field: "LogContent", },
                {
                    field: "LogUrl", cellRenderer: params => {
                        if (!params.node.group) {
                            try {
                                return decodeURIComponent(params.value)
                            } catch (e) { }
                        }
                        return params.value;
                    },
                },
                { field: "LogIp", },
                { field: "LogReferer", width: 300, },
                nrGrid.newColumnDate({
                    field: "LogCreateTime", valueGetter: params => {
                        if (params.data) {
                            return new Date((params.data["LogCreateTime"] - 621355968000000000) / 10000).toISOString().replace("T", " ").replace("Z", "");
                        }
                    },
                }),
                nrGrid.newColumnSet({ field: "LogGroup", enableRowGroup: true }, [{ value: 1, text: "用户" }, { value: 2, text: "爬虫" }, { value: -1, text: "异常" }, { value: 9, text: "记录" }]),
                nrGrid.newColumnSet({ field: "LogLevel", enableRowGroup: true }, [{ value: 'F', text: "Fatal" }, { value: 'W', text: "Warning" }, { value: 'I', text: "Info" }, { value: 'D', text: "Debug" }, { value: 'T', text: "Trace" }, { value: 'A', text: "All" }]),
                { field: "LogBrowserName", width: 250, },
                { field: "LogSystemName", },
                { field: "LogUserAgent", width: 500, },
                { field: "LogRemark" }
            ];

            //grid 配置
            let gridOptions = await nrGrid.gridOptionsServer({
                getRowId: params => params.data[nrPage.tableKey],
                columnDefs: colDefs,
                //数据源
                serverSideDatasource: {
                    getRows: params => {
                        let req = params.request; //请求参数
                        //默认排序
                        if (req.sortModel.length == 0) {
                            req.sortModel = [{ colId: 'LogCreateTime', sort: 'desc' }];
                        }

                        let sqls = nrcAgGridQuery.buildSql(req);
                        let url = `/Admin/QueryLog?${nrcBase.fromKeyToURLParams(sqls)}`;
                        nrcBase.fetch(url).then(vm => {
                            if (!vm.error && vm.resp.ok) {
                                params.success({ rowData: vm.result.RowData, rowCount: vm.result.RowCount });
                            } else {
                                params.fail();
                                nrApp.logError(vm.error);
                            }
                        })
                    }
                }
            });

            //grid dom
            nrGrid.buildDom(nrVary.domGrid);
            nrcBase.setHeightFromBottom(nrVary.domGrid);

            //grid 显示
            nrPage.grid1 = await nrGrid.createGrid(nrVary.domGrid, gridOptions);
        } catch (error) {
            nrApp.logError(error);
            nrVary.domGrid.innerHTML = nrApp.tsFailHtml;
        }
    },
}

export { nrPage };