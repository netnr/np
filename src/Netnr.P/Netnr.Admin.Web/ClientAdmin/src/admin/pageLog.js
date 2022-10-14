import { ag } from "../js/ag";

var pageLog = {
    init: () => {
        document.title = "日志管理";

        pageLog.list();
    },

    list: () => {
        var domGrid = document.createElement("div");
        domGrid.classList.add("ag-theme-alpine");
        domGrid.style.height = "70vh";
        document.querySelectorAll('sl-tab-panel')[2].appendChild(domGrid);

        let gridOptions = {
            localeText: ag.localeText, //语言
            defaultColDef: {
                filter: 'agTextColumnFilter', floatingFilter: true,
                sortable: true, resizable: true, width: 200
            },
            getRowId: event => event.data.LogId,
            columnDefs: [
                ag.numberCol({ headerCheckboxSelection: false }),
                { field: "LogId", headerName: "标识ID", filter: 'agNumberColumnFilter' },
                ag.agDateColumn({ field: "CreateTime", headerName: "创建时间", }),
                { field: "LogUser", headerName: "账号", },
                { field: "LogNickname", headerName: "昵称", },
                ag.agSetColumn({ field: "LogType", headerName: "分类" }, { "-1": "错误", "1": "默认", "2": "登录" }),
                { field: "LogAction", headerName: "动作", },
                { field: "LogUrl", headerName: "URL", },
                { field: "LogContent", headerName: "内容", },
                { field: "LogSql", headerName: "SQL", },
                { field: "LogTimeCost", headerName: "耗时", filter: 'agNumberColumnFilter' },
                { field: "LogIp", headerName: "IP", },
                { field: "LogUserAgent", headerName: "代理", },
                { field: "LogBrowser", headerName: "浏览器", },
                { field: "LogSystem", headerName: "系统", },
                { field: "LogRemark", headerName: "备注", },
            ],
            suppressRowClickSelection: true,
            rowSelection: 'multiple',
            pagination: true, //分页
            paginationPageSize: 100,
            cacheBlockSize: 30,
            enableRangeSelection: true, //范围选择
            animateRows: true, //动画
            rowModelType: 'infinite', //无限行模式
            //数据源
            datasource: {
                getRows: params => {
                    //默认排序
                    if (params.sortModel.length == 0) {
                        params.sortModel.push({ colId: "CreateTime", sort: "desc" });
                    }

                    var url = `https://localhost:8005/Admin/LogGet?paramsJson=${encodeURIComponent(JSON.stringify(params))}`;
                    fetch(url).then(resp => resp.json()).then(res => {
                        if (res.code == 200) {
                            params.successCallback(res.data.RowsThisBlock, res.data.LastRow)
                        } else {
                            params.failCallback();
                            console.debug(res.msg);
                            alert(res.msg)
                        }
                    }).catch(err => {
                        console.log(err);
                        params.failCallback();
                    })
                }
            },
            onCellDoubleClicked: event => {

            },
            onGridReady: () => {

            },
            getContextMenuItems: (params) => {
                var result = [
                    'autoSizeAll',
                    'resetColumns',
                    'separator',
                    'copy',
                    'copyWithHeaders'
                ];

                return result;
            },
        };

        var grid = new agGrid.Grid(domGrid, gridOptions).gridOptions;
    }
}

export { pageLog };