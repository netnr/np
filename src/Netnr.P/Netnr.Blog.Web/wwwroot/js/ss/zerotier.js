nr.onChangeSize = function (ch) {
    var vh = ch - nr.domGrid.getBoundingClientRect().top - 30;
    nr.domGrid.style.height = vh + "px";
}

nr.onReady = function () {
    nr.domTxtToken.value = nr.lsStr("token");
    nr.domTxtNetworkid.value = nr.lsStr("networkId");

    page.getNetworkMember();

    nr.domTxtToken.addEventListener("input", function () {
        page.getNetworkMember();
    });

    nr.domTxtNetworkid.addEventListener("input", function () {
        page.getNetworkMember();
    });

    nr.domTxtFilter.addEventListener("input", function () {
        if (page.grid) {
            page.grid.api.setQuickFilter(this.value);
        }
    });
}

var page = {
    grid: null,

    /** 获取您至少具有读取权限的网络的所有成员 */
    getNetworkMember: function () {
        var token = nr.domTxtToken.value.trim();
        var networkId = nr.domTxtNetworkid.value.trim();

        nr.ls["token"] = token;
        nr.ls["networkId"] = networkId;
        nr.lsSave();

        if (token != "" && networkId != "") {
            ss.loading(true);

            ss.fetch({
                url: `https://my.zerotier.com/api/network/${networkId}/member`,
                headers: {
                    Authorization: 'Bearer ' + token
                }
            }).then(res => {
                ss.loading(false);
                res = JSON.parse(res);
                page.view(res);
            }).catch(ex => {
                console.debug(ex);
                ss.loading(false);
                nr.alert("网络错误");
            })
        }
    },

    view: function (data) {
        console.debug(data);

        var gridOptions = ag.optionDef({
            columnDefs: [
                {
                    field: "online", headerName: "在线状态", enableRowGroup: true, valueFormatter: (params) => {
                        if (params.data) {
                            return params.value ? '✅' : '⛔'
                        }
                    }
                },
                { field: "nodeId", headerName: "节点ID", },
                {
                    field: "name", headerName: "名称", width: 220, cellRenderer: function (params) {
                        if (params.data) {
                            var val = params.value;
                            if (params.data.description != "") {
                                val += "（" + params.data.description + "）";
                            }
                            return val;
                        }
                    }
                },
                { field: "config.ipAssignments", headerName: "托管IP" },
                {
                    field: "config.noAutoAssignIps", headerName: "自动分配IP", valueFormatter: (params) => {
                        if (params.data) {
                            return params.value ? '✅' : '⛔'
                        }
                    }
                },
                { field: "physicalAddress", headerName: "公网IP" },
                {
                    field: "config.creationTime", headerName: "创建时间", width: 220, valueFormatter: (params) => {
                        if (params.data) {
                            return new Date(params.data.config.creationTime + 8 * 3600 * 1000).toISOString().replace("T", " ").substring(0, 19);
                        }
                    }
                },
                {
                    field: "lastOnline", headerName: "最后在线时间", width: 220, valueFormatter: (params) => {
                        if (params.data) {
                            if (params.value != 0) {
                                return new Date(params.value + 8 * 3600 * 1000).toISOString().replace("T", " ").substring(0, 19);
                            }
                            return '';
                        }
                    }
                },
                {
                    field: "config.authorized", headerName: "授权", valueFormatter: (params) => {
                        if (params.data) {
                            return params.value ? '✅' : '⛔'
                        }
                    }
                },
                { field: "clientVersion", headerName: "客户端版本", enableRowGroup: true, }
            ],
            rowData: data,
        });

        page.grid = new agGrid.Grid(nr.domGrid, gridOptions).gridOptions;
    }
}