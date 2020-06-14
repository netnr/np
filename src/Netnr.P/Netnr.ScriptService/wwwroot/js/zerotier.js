var zt = {
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
                    $('#table').bootstrapTable({
                        filterControl: true,
                        columns: [
                            {
                                field: "online", title: "在线状态", sortable: true, filterControl: "select", formatter: function (value) {
                                    if (value == true) {
                                        return '✔'
                                    } else {
                                        return '✖'
                                    }
                                }
                            },
                            { field: "nodeId", title: "节点ID", sortable: true, filterControl: "input" },
                            {
                                field: "name", title: "名称", sortable: true, filterControl: "input", formatter: function (value, row) {
                                    if (row.description != "") {
                                        value += "（" + row.description + "）";
                                    }
                                    return value;
                                }
                            },
                            {
                                field: "managedIp", title: "托管IP", filterControl: "input", formatter: function (_value, row) {
                                    return row.config.ipAssignments.join('<br/>');
                                }
                            },
                            {
                                field: "noAutoAssignIps", title: "自动分配IP", filterControl: "select", formatter: function (_value, row) {
                                    return !row.config.noAutoAssignIps;
                                }
                            },
                            { field: "physicalAddress", title: "公网IP", sortable: true, filterControl: "input" },
                            {
                                field: "creationTime", title: "创建时间", filterControl: "input", formatter: function (_value, row) {
                                    return new Date(row.config.creationTime + 8 * 3600 * 1000).toISOString().replace("T", " ").substr(0, 19);
                                }
                            },
                            {
                                field: "lastOnline", title: "最后在线时间", filterControl: "input", formatter: function (value) {
                                    if (value != 0) {
                                        return new Date(value + 8 * 3600 * 1000).toISOString().replace("T", " ").substr(0, 19);
                                    }
                                    return "";
                                }
                            },
                            {
                                field: "authorized", title: "授权", sortable: true, filterControl: "select", formatter: function (_value, row) {
                                    return row.config.authorized;
                                }
                            },
                            { field: "clientVersion", title: "客户端版本", sortable: true, filterControl: "select" }
                        ],
                        data: data,
                        onLoadSuccess: function () {
                            zt.resize();
                        }
                    });
                },
                error: function () {
                    jz.msg("载入失败")
                },
                complete: function () {
                    ss.loading(0);
                }
            }, 0)
        }
    },

    resize: function () {
        $('#table').bootstrapTable('resetView', {
            height: $(window).height() - $('#PGrid').offset().top - 15
        });
    }
}

zt.init();