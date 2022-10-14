import { ag } from "../js/ag";

var pageMenu = {
    init: () => {
        document.title = "菜单管理";

        pageMenu.list();
    },

    list: () => {
        var domGrid = document.createElement("div");
        domGrid.classList.add("ag-theme-alpine");
        domGrid.style.height = "50vh";
        document.querySelector('sl-tab-panel').appendChild(domGrid);

        let gridOptions = {
            localeText: ag.localeText, //语言
            defaultColDef: {
                filter: 'agTextColumnFilter', floatingFilter: true,
                sortable: true, resizable: true, width: 200
            },
            getRowId: event => event.data.MenuId,
            columnDefs: [
                ag.numberCol({ headerCheckboxSelection: false }),
                { field: "MenuId", headerName: "节点ID", },
                { field: "CreateTime", headerName: "创建时间", },
                { field: "Status", headerName: "状态", },
                { field: "MenuPid", headerName: "上级", },
                { field: "MenuName", headerName: "名称", },
                { field: "MenuUrl", headerName: "链接", },
                { field: "MenuIcon", headerName: "图标", },
                { field: "MenuOrder", headerName: "排序", },
                { field: "MenuGroup", headerName: "分组", },
                { field: "MenuRemark", headerName: "备注", },
            ],
            suppressRowClickSelection: true,
            rowSelection: 'multiple',
            cacheBlockSize: 30, //请求行数
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

                    var url = `https://localhost:8005/Admin/MenuGet?paramsJson=${encodeURIComponent(JSON.stringify(params))}`;
                    fetch(url).then(resp => resp.json()).then(res => {
                        if (res.code == 200) {
                            params.successCallback(res.data.RowsThisBlock, res.data.LastRow)
                        } else {
                            params.failCallback();
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

export { pageMenu };