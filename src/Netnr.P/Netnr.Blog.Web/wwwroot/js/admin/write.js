nr.onChangeSize = function (ch) {
    if (page.gridOps) {
        var vh = ch - nr.domGrid.getBoundingClientRect().top - 15;
        nr.domGrid.style.height = vh + "px";
    }
}

nr.onReady = function () {
    page.load()
}

var page = {
    load: () => {
        let gridOptions = {
            localeText: ag.localeText, //语言
            defaultColDef: {
                filter: 'agTextColumnFilter', floatingFilter: true,
                sortable: true, resizable: true, width: 200
            },
            getRowId: event => event.data.UwId,
            columnDefs: [
                ag.numberCol({ checkboxSelection: false, headerCheckboxSelection: false }),
                { field: "UserId", filter: 'agNumberColumnFilter', },
                { headerName: "昵称", field: "Nickname", },
                { headerName: "文章ID", field: "UwId", filter: 'agNumberColumnFilter', },
                {
                    headerName: "💡标题", field: "UwTitle", width: 400, cellRenderer: (params) => {
                        if (params.data != null) {
                            return `<a href="/home/list/${params.data.UwId}">${params.value}</a>`;
                        }
                    }, editable: true, cellEditor: 'agLargeTextCellEditor', cellEditorParams: { maxLength: 999 }
                },
                { headerName: "创建时间", field: "UwCreateTime", filter: 'agDateColumnFilter', },
                { headerName: "修改时间", field: "UwUpdateTime", filter: 'agDateColumnFilter', },
                { headerName: "💡回复", field: "UwReplyNum", filter: 'agNumberColumnFilter', editable: true },
                { headerName: "💡浏览", field: "UwReadNum", filter: 'agNumberColumnFilter', editable: true },
                { headerName: "💡点赞", field: "UwLaud", filter: 'agNumberColumnFilter', editable: true },
                { headerName: "💡收藏", field: "UwMark", filter: 'agNumberColumnFilter', editable: true },
                {
                    headerName: "💡公开", field: "UwOpen", filter: 'agNumberColumnFilter', cellRenderer: params => params.value == 1 ? "✔" : "✘",
                    editable: true, cellEditor: 'agRichSelectCellEditor', cellEditorParams: {
                        values: [1, 0], formatValue: fv => fv == 1 ? "✔" : "✘"
                    }
                },
                {
                    headerName: "💡状态", field: "UwStatus", filter: 'agNumberColumnFilter', cellRenderer: function (params) {
                        var km =
                        {
                            "1": "✔",
                            "2": "Block",
                            "-1": "Lock"
                        };
                        return km[params.value];
                    }, editable: true, cellEditor: 'agRichSelectCellEditor', cellEditorParams: {
                        values: [1, 2, -1], formatValue: fv => {
                            var km =
                            {
                                "1": "✔",
                                "2": "Block",
                                "-1": "Lock"
                            };
                            return km[fv];
                        }
                    }
                }
            ],
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
                        params.sortModel.push({ colId: "UwCreateTime", sort: "desc" });
                    }

                    fetch(`/Admin/WriteList?grp=${encodeURIComponent(JSON.stringify(params))}`).then(x => x.json()).then(res => {
                        params.successCallback(res.RowsThisBlock, res.LastRow)
                    }).catch(err => {
                        console.log(err);
                        params.failCallback();
                    })
                }
            },
            // 单元格变动
            onCellValueChanged: function (event) {
                let data = event.data;

                var fd = new FormData();
                for (var i in data) {
                    fd.append(i, data[i]);
                }

                fetch('/Admin/WriteSave', {
                    method: 'POST',
                    body: fd
                }).then(x => x.json()).then(res => {
                    if (res.code == 200) {
                        page.gridOps.api.ensureIndexVisible(event.rowIndex); //滚动到行显示
                        page.gridOps.api.flashCells({ rowNodes: [event.node], columns: [event.column.colId] }); //闪烁单元格
                    } else {
                        nr.alert(res.msg);
                    }
                });
            },
            onGridReady: () => {
                //自适应
                nr.changeSize();
            },
            getContextMenuItems: (params) => {
                var result = [
                    { name: "Reload", icon: "🔄", action: page.load },
                    'autoSizeAll',
                    'resetColumns',
                    'separator',
                    'copy',
                    'copyWithHeaders'
                ];

                return result;
            },
        };

        if (page.gridOps) {
            page.gridOps.api.destroy();
        }
        page.gridOps = new agGrid.Grid(nr.domGrid, gridOptions).gridOptions;
    },
}