nr.onChangeSize = function (ch) {
    if (page.gridOps) {
        var vh = ch - nr.domGrid.getBoundingClientRect().top - 15;
        nr.domGrid.style.height = vh + "px";
    }
}

nr.onReady = function () {
    page.load();
}

var page = {
    load: () => {
        let gridOptions = {
            localeText: ag.localeText, //语言
            defaultColDef: {
                filter: 'agTextColumnFilter', floatingFilter: true,
                sortable: true, resizable: true, width: 200,
                menuTabs: ['generalMenuTab', 'filterMenuTab', 'columnsMenuTab']
            },
            getRowId: event => event.data.UrId, //指定行标识列
            //列
            columnDefs: [
                ag.numberCol({ checkboxSelection: false, headerCheckboxSelection: false }),
                { field: "UserId", filter: 'agNumberColumnFilter', },
                { field: "Nickname", },
                { field: "UrAnonymousName", headerName: "💡匿名昵称", editable: true },
                { field: "UrAnonymousMail", headerName: "💡匿名邮箱", editable: true },
                { field: "UrAnonymousLink", headerName: "💡匿名链接", editable: true },
                { field: "UrId", headerName: "回复Key" },
                { field: "UrTargetType", headerName: "回复分类" },
                { field: "UrTargetId", headerName: "回复对象ID" },
                {
                    field: "UrContent", headerName: "💡回复内容(HTML)", cellRenderer: params => {
                        if (params.value != null) {
                            return nr.htmlEncode(params.value);
                        }
                        return params.value
                    }, editable: true, cellEditor: 'agLargeTextCellEditor', cellEditorParams: { maxLength: 99999 }
                },
                {
                    field: "UrContentMd", headerName: "💡回复内容(MD)", cellRenderer: params => {
                        if (params.value != null) {
                            return nr.htmlEncode(params.value);
                        }
                        return params.value
                    }, editable: true, cellEditor: 'agLargeTextCellEditor', cellEditorParams: { maxLength: 99999 }
                },
                {
                    field: "UrCreateTime", filter: 'agDateColumnFilter',
                },
                {
                    field: "UrStatus", headerName: "💡状态", filter: 'agNumberColumnFilter', cellRenderer: params => {
                        switch (Number(params.value)) {
                            case 1: return "✔";
                            case 2: return "Block";
                        }
                        return params.value
                    }, editable: true, cellEditor: 'agRichSelectCellEditor', cellEditorParams: {
                        values: [1, 2], formatValue: fv => {
                            var km =
                            {
                                "1": "✔",
                                "2": "Block"
                            };
                            return km[fv];
                        }
                    }
                }
            ],
            cacheBlockSize: 30, //请求行数
            enableRangeSelection: true, //范围选择
            animateRows: true, //动画
            rowModelType: 'infinite', //无限行模式
            //数据源
            datasource: {
                getRows: params => {

                    //默认排序
                    if (params.sortModel.length == 0) {
                        params.sortModel.push({ colId: "UrCreateTime", sort: "desc" });
                    }

                    fetch(`/Admin/ReplyList?grp=${encodeURIComponent(JSON.stringify(params))}`).then(x => x.json()).then(res => {
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

                fetch('/Admin/ReplySave', {
                    method: 'POST',
                    body: fd
                }).then(x => x.json()).then(res => {
                    if (res.code == 200) {
                        page.gridOps.api.ensureIndexVisible(event.rowIndex); //滚动到行显示
                        page.gridOps.api.flashCells({ rowNodes: [event.node], columns: [event.column.colId] }); //闪烁单元格
                    } else {
                        alert(res.msg);
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
    }
}