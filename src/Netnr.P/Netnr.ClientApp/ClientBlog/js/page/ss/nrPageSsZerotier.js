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
                { field: "nodeId", headerName: "节点ID", width: 150, },
                { field: "name", headerName: "名称" },
                { field: "description", headerName: "备注", },
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
                    field: "lastOnline", headerName: "最后在线时间", width: 220,
                    valueFormatter: (params) => {
                        if (params.value > 0) {
                            return nrcBase.formatDateTime('datetime', params.value);
                        }
                    },
                    cellStyle: (params) => {
                        if (params.value != null) {
                            let pastTime = new Date() - new Date(params.value);

                            if (pastTime > 1000 * 120) {
                                //超 2 分钟
                                return { 'color': 'var(--bs-info)' };
                            } else if (pastTime > 1000 * 3600) {
                                //超 1 小时
                                return { 'color': 'var(--bs-danger)' };
                            }
                        }
                    }
                },
                {
                    field: "config.creationTime", headerName: "创建时间", width: 220,
                    valueFormatter: (params) => {
                        if (params.value > 0) {
                            return nrcBase.formatDateTime('datetime', params.value);
                        }
                    }
                },
                {
                    field: "config.authorized", headerName: "授权", width: 150, valueFormatter: (params) => {
                        if (params.data) {
                            return params.value ? '🟩' : '⛔'
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