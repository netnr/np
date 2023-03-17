import { nrcBase } from "../../../../frame/nrcBase";
import { nrcUpstream } from "../../../../frame/nrcUpstream";
import { nrGrid } from "../../../../frame/nrGrid";
import { nrStorage } from "../../../../frame/nrStorage";
import { nrVary } from "../../nrVary";
import { nrApp } from "../../../../frame/Bootstrap/nrApp";

let nrPage = {
    pathname: "/ss/zerotier",

    ckeyToken: "/ss/zerotier/token",
    ckeyNetworkId: "/ss/zerotier/networkid",

    init: async () => {
        let token = await nrStorage.getItem(nrPage.ckeyToken);
        let networkId = await nrStorage.getItem(nrPage.ckeyNetworkId);
        if (token) {
            nrVary.domTxtToken.value = token;
        }
        if (networkId) {
            nrVary.domTxtNetworkid.value = networkId;
        }

        await nrPage.getNetworkMember();

        nrcBase.setHeightFromBottom(nrVary.domGrid);
        nrPage.bindEvent();
    },

    bindEvent: () => {
        //过滤
        nrVary.domTxtFilter.addEventListener('input', async function () {
            if (nrApp.tsGrid) {
                nrApp.tsGrid.api.setQuickFilter(this.value);
            }
        });

        [nrVary.domTxtToken, nrVary.domTxtNetworkid].forEach(dom => {
            dom.addEventListener('input', async function () {
                await nrPage.getNetworkMember();
            })
        })
    },

    /** 获取您至少具有读取权限的网络的所有成员 */
    getNetworkMember: async () => {
        let token = nrVary.domTxtToken.value.trim();
        let networkId = nrVary.domTxtNetworkid.value.trim();

        if (token && token.length > 10 && networkId && networkId.length > 10) {
            nrVary.domGrid.innerHTML = nrApp.tsLoadingHtml;

            await nrStorage.setItem(nrPage.ckeyToken, token);
            await nrStorage.setItem(nrPage.ckeyNetworkId, networkId);

            let url = `https://my.zerotier.com/api/network/${networkId}/member`;

            try {
                let result = await nrcUpstream.fetch(url, { headers: { Authorization: `Bearer ${token}` } });
                let rowData = JSON.parse(result);
                await nrPage.view(rowData);
            } catch (error) {
                nrApp.logError(error, '加载失败');
            }
        }
    },

    view: async (rowData) => {
        let gridOptions = nrGrid.gridOptionsClient({
            suppressFieldDotNotation: false,
            columnDefs: [
                {
                    field: "online", headerName: "在线状态", width: 150, enableRowGroup: true, valueFormatter: (params) => {
                        if (params.data) {
                            return params.value ? '✅' : '⛔'
                        }
                    }
                },
                { field: "nodeId", headerName: "节点ID", width: 150, },
                {
                    field: "name", headerName: "名称", width: 220, cellRenderer: function (params) {
                        if (params.data) {
                            let val = params.value;
                            if (params.data.description != "") {
                                val += "（" + params.data.description + "）";
                            }
                            return val;
                        }
                    }
                },
                { field: "config.ipAssignments", headerName: "托管IP" },
                {
                    field: "config.noAutoAssignIps", headerName: "自动分配IP", width: 150, valueFormatter: (params) => {
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
                    field: "config.authorized", headerName: "授权", width: 150, valueFormatter: (params) => {
                        if (params.data) {
                            return params.value ? '✅' : '⛔'
                        }
                    }
                },
                { field: "clientVersion", headerName: "客户端版本", width: 150, enableRowGroup: true, }
            ],
            rowData: rowData,
        });

        nrGrid.buildDom(nrVary.domGrid);
        nrApp.tsGrid = await nrGrid.viewGrid(nrVary.domGrid, gridOptions);
    }
}

export { nrPage };