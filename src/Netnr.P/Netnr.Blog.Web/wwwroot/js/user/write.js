var page = {
    init: () => {
        page.load();

        //编辑
        nr.domBtnEdit.addEventListener("click", function () {
            var srow = page.gridOps.api.getSelectedRows();
            if (srow.length == 0) {
                nr.alert("请选择一条记录");
            } else {
                page.editGrid(srow[0].UwId);
            }
        }, false);

        //删除
        nr.domBtnDel.addEventListener("click", function () {
            var srow = page.gridOps.api.getSelectedRows();
            if (srow.length) {
                if (confirm("确定要删除吗？")) {
                    fetch(`/User/WriteDel?ids=${srow.map(x => x.UwId).join(',')}`).then(x => x.json()).then(res => {
                        if (res.code == 200) {
                            page.load();
                        } else {
                            nr.alert(res.msg);
                        }
                    }).catch(x => {
                        nr.alert(x);
                    });
                }
            } else {
                nr.alert("请选择一行再操作");
            }
        }, false);
    },
    load: () => {
        let gridOptions = {
            localeText: ag.localeText, //语言
            defaultColDef: {
                filter: 'agTextColumnFilter', floatingFilter: true,
                sortable: true, resizable: true, width: 200
            },
            getRowId: event => event.data.UwId,
            columnDefs: [
                ag.numberCol({ headerCheckboxSelection: false }),
                { headerName: "Id", field: "UwId", filter: 'agNumberColumnFilter', },
                {
                    headerName: "标题", field: "UwTitle", width: 640, cellRenderer: (params) => {
                        if (params.data) {
                            return `<a href="/home/list/${params.data.UwId}">${params.value}</a>`;
                        }
                    }
                },
                { headerName: "创建时间", field: "UwCreateTime", filter: 'agDateColumnFilter', },
                { headerName: "修改时间", field: "UwUpdateTime", filter: 'agDateColumnFilter', },
                { headerName: "回复", field: "UwReplyNum", filter: 'agNumberColumnFilter', },
                { headerName: "浏览", field: "UwReadNum", filter: 'agNumberColumnFilter', },
                { headerName: "点赞", field: "UwLaud", filter: 'agNumberColumnFilter', },
                { headerName: "收藏", field: "UwMark", filter: 'agNumberColumnFilter', },
                 ag.agSetColumn({ headerName: "公开", field: "UwOpen", }, { "1": "✔", "2": "✘" }),
                 ag.agSetColumn({ headerName: "状态", field: "UwStatus", }, { "1": "✔", "2": "Block", "-1": "Lock" }),
                //{
                //    headerName: "公开", field: "UwOpen", filter: 'agNumberColumnFilter', cellRenderer: params => {
                //        switch (params.value) {
                //            case 1: return "✔"; break;
                //            case 2: return "✘"; break;
                //        }
                //    }
                //},
                //{
                //    headerName: "状态", field: "UwStatus", filter: 'agNumberColumnFilter', cellRenderer: function (params) {
                //        switch (params.value) {
                //            case 1: return "✔"; break;
                //            case 2: return "Block"; break;
                //            case -1: return "Lock"; break;
                //        }
                //    }
                //}
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
                        params.sortModel.push({ colId: "UwCreateTime", sort: "desc" });
                    }

                    fetch(`/User/WriteList?grp=${encodeURIComponent(JSON.stringify(params))}`).then(x => x.json()).then(res => {
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
                page.editGrid(event.data.UwId);
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
    editGrid: (id) => {
        nr.keyId = id;

        nr.domTxtTitle.value = "";
        nr.nmd.setmd("Loading...");
        nr.domSelect.setValue([]);

        nr.domDialogForm.show()

        fetch(`/User/WriteOne?id=${id}`).then(x => x.json()).then(res => {
            if (res.code == 200) {
                var item = res.data.item;
                nr.domTxtTitle.value = item.UwTitle;
                nr.domSelect.setValue(res.data.tags.map(x => x.TagId));
                nr.nmd.setmd(item.UwContentMd);
            } else {
                console.debug(res);
            }
        });
    }
};