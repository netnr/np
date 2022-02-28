var zt = {
    grid: null,
    gbox: document.querySelector('.grid'),
    init: function () {
        $('#txtToken').on('input', function () {
            ss.ls.token = this.value;
            ss.lsSave();
            zt.getNetworkMember();
        }).val(ss.ls.token || "")

        $('#txtNetworkId').on('input', function () {
            if (this.value.length == 16 || this.value == "") {
                ss.ls.networkId = this.value;
                ss.lsSave();
            }
            zt.getNetworkMember();
        }).val(ss.ls.networkId || "")

        zt.getNetworkMember();

        $(window).resize(function () {
            zt.resize();
        })
    },

    /** 获取您至少具有读取权限的网络的所有成员 */
    getNetworkMember: function () {
        if (ss.ls.token && ss.ls.networkId && ss.ls.networkId.length == 16) {
            ss.loading();

            ss.ajax({
                headers: {
                    Authorization: "bearer " + ss.ls.token
                },
                url: "https://my.zerotier.com/api/network/" + ss.ls.networkId + "/member",
                dataType: 'json',
                success: function (data) {
                    console.log(data);
                    zt.view(data);
                },
                error: function () {
                    bs.msg("<h4>载入失败</h4>");
                },
                complete: function () {
                    ss.loading(0);
                }
            }, 0)
        }
    },

    view: function (data) {
        var gridOptions = {
            columnDefs: [
                {
                    field: "online", headerName: "在线状态", width: 150,
                    cellRenderer: function (item) {
                        if (item.value) {
                            return '🔵'
                        } else {
                            return '🔴'
                        }
                    }
                },
                {
                    field: "nodeId", headerName: "节点ID", width: 150
                },
                {
                    field: "name", headerName: "名称",
                    cellRenderer: function (item) {
                        var val = item.value;
                        if (item.data.description != "") {
                            val += "（" + item.data.description + "）";
                        }
                        return val;
                    }
                },
                {
                    field: "managedIp", headerName: "托管IP",
                    cellRenderer: function (item) {
                        return item.data.config.ipAssignments.join('<br/>');
                    }
                },
                {
                    field: "noAutoAssignIps", headerName: "自动分配IP", width: 150,
                    cellRenderer: function (item) {
                        if (!item.data.config.noAutoAssignIps) {
                            return '🔵'
                        } else {
                            return '🔴'
                        }
                    }
                },
                {
                    field: "physicalAddress", headerName: "公网IP"
                },
                {
                    field: "creationTime", headerName: "创建时间",
                    cellRenderer: function (item) {
                        return new Date(item.data.config.creationTime + 8 * 3600 * 1000).toISOString().replace("T", " ").substr(0, 19);
                    }
                },
                {
                    field: "lastOnline", headerName: "最后在线时间",
                    cellRenderer: function (item) {
                        if (item.value != 0) {
                            return new Date(item.value + 8 * 3600 * 1000).toISOString().replace("T", " ").substr(0, 19);
                        }
                        return '';
                    }
                },
                {
                    field: "authorized", headerName: "授权", width: 100,
                    cellRenderer: function (item) {
                        if (item.data.config.authorized) {
                            return '🔵'
                        } else {
                            return '🔴'
                        }
                    }
                },
                {
                    field: "clientVersion", title: "客户端版本"
                }
            ],
            animateRows: true,
            rowSelection: 'multiple',
            groupSelectsChildren: true,
            rowData: data,
            enableRangeSelection: true,
            defaultColDef: ss.agg.defaultColDef,
            localeText: ss.agg.localeText,
            components: {

            },
            onGridReady: function () {
                //表格创建完成后执行的事件
                //gridOptions.api.sizeColumnsToFit();//调整表格大小自适应
            }
        };

        zt.grid = new agGrid.Grid(zt.gbox, gridOptions);

        zt.resize();
    },

    resize: function () {
        var h = $(window).height() - $(zt.gbox).offset().top - 15;
        zt.gbox.style.height = h + "px";
    }
}

zt.init();