import { nrcBase } from "../../../../frame/nrcBase";
import { nrGrid } from "../../../../frame/nrGrid";
import { nrStorage } from "../../../../frame/nrStorage";
import { nrApp } from "../../../../frame/Bootstrap/nrApp";
import { nrVary } from "../../nrVary";
import { nrWeb } from "../../nrWeb";

let nrPage = {
    pathname: "/ss/datadict",
    ckeyType: "/ss/datadict/type",

    init: async () => {
        let defaultType = await nrStorage.getItem(nrPage.ckeyType);
        if (defaultType) {
            nrVary.domSeType.value = defaultType;
        }
        await nrPage.bindGrid(nrVary.domSeType.value);

        nrcBase.setHeightFromBottom(nrVary.domGrid);

        nrPage.bindEvent();
    },

    bindEvent: () => {
        //切换
        nrVary.domSeType.addEventListener('input', async function () {
            await nrStorage.setItem(nrPage.ckeyType, this.value);
            nrPage.bindGrid(this.value);
        })

        //过滤
        nrVary.domTxtFilter.addEventListener('input', function () {
            if (nrApp.tsGrid) {
                nrApp.tsGrid.api.setQuickFilter(this.value);
            }
        })
    },

    bindGrid: async (type) => {
        nrVary.domGrid.innerHTML = nrApp.tsLoadingHtml;
        let gridOps;

        nrVary.domCardRegex.classList.add('d-none');
        nrVary.domTxtFilter.value = "";
        nrVary.domTxtFilter.readOnly = false;
        switch (type) {
            case "regex":
                {
                    await nrcBase.importScript('/file/data-regex.js');

                    gridOps = nrGrid.gridOptionsClient({
                        columnDefs: [
                            { field: "type", headerName: "分类", width: 150, enableRowGroup: true },
                            { field: "regex", headerName: "正则", width: 200 },
                            { field: "remark", headerName: "备注", width: 500, flex: 1 },
                        ],
                        rowData: dataRegex,
                    });

                    nrVary.domCardRegex.classList.remove('d-none');
                }
                break;
            case "dns":
                {
                    await nrcBase.importScript('/file/data-dns.js');

                    gridOps = nrGrid.gridOptionsClient({
                        columnDefs: [
                            { field: "type", headerName: "分类", width: 150, enableRowGroup: true },
                            { field: "name", headerName: "名称", width: 200, enableRowGroup: true },
                            { field: "ip", headerName: "IP", flex: 1 },
                        ],
                        rowData: dataDNS,
                    });

                    nrVary.domCardRegex.classList.remove('d-none');
                }
                break;
            case "purine":
                {
                    await nrcBase.importScript('/file/data-purine.js');

                    dataPurine.forEach(d => {
                        d.rank = "低";
                        d.class = "var(--bs-success)";
                        if (d.number > 25) {
                            d.rank = "中";
                            d.class = "var(--bs-warning)";
                        }
                        if (d.number > 150) {
                            d.rank = "高";
                            d.class = "var(--bs-danger)";
                        }
                        d.rank += "嘌呤";
                    });

                    gridOps = nrGrid.gridOptionsClient({
                        columnDefs: [
                            nrGrid.newColumnLineNumber({ checkboxSelection: false, headerCheckboxSelection: false }),
                            { field: "type", headerName: "分类", enableRowGroup: true },
                            { field: "food", headerName: "食物" },
                            nrGrid.newColumnNumber({ field: "number", headerName: "含量" }),
                            { field: "rank", headerName: "等级", enableRowGroup: true }
                        ],
                        rowData: dataPurine,
                        getRowStyle: function (item) {
                            if (item.data) {
                                return {
                                    "color": item.data.class
                                }
                            }
                        }
                    });
                }
                break;
            case "gc":
                {
                    await nrcBase.importScript('/file/data-gc.js');

                    dataGc.forEach(d => {
                        d.cname = "可回收";
                        d.class = "var(--bs-success)";
                        if (d.category == 2) {
                            d.cname = "有害";
                            d.class = "var(--bs-danger)";
                        }
                        if (d.category == 4) {
                            d.cname = "湿/厨余";
                            d.class = "var(--bs-primary)";
                        }
                        if (d.category == 8) {
                            d.cname = "干/其它";
                            d.class = "var(--bs-secondary)";
                        }
                        if (d.category == 16) {
                            d.cname = "大件";
                            d.class = "var(--bs-secondary)";
                        }
                    });

                    gridOps = nrGrid.gridOptionsClient({
                        columnDefs: [
                            nrGrid.newColumnLineNumber({ checkboxSelection: false, headerCheckboxSelection: false }),
                            { field: "name", headerName: "名称" },
                            { field: "cname", headerName: "分类", enableRowGroup: true }
                        ],
                        rowData: dataGc,
                        getRowStyle: function (item) {
                            if (item.data) {
                                return {
                                    "color": item.data.class
                                }
                            }
                        },
                    });
                }
                break;
            case "stats-product-category":
                {
                    gridOps = await nrGrid.gridOptionsServer({
                        //列
                        columnDefs: [
                            { field: 'id', headerName: "代码", sortable: true },
                            { field: 'txt', headerName: "名称", },
                            { field: 'pid', headerName: "上级代码", },
                            nrGrid.newColumnNumber({ field: 'num', sortable: true, headerName: "同级排序", }),
                            nrGrid.newColumnNumber({ field: 'deep', headerName: "层级", }),
                        ],
                        //分组
                        autoGroupColumnDef: {
                            width: 520, field: 'id', headerName: '分组',
                            cellRendererParams: {
                                //组名称
                                innerRenderer: function (params) {
                                    var gcount = params.data.group == true || params.data.group == 0 ? "" : `（${params.data.group}）`;
                                    return `${params.data.txt} - ${params.data.id} ${gcount}`;
                                },
                            },
                        },
                        treeData: true, //树
                        isServerSideGroup: dataItem => dataItem.group > 0, //有子节点
                        getServerSideGroupKey: dataItem => dataItem.id, //分组项
                        //数据源
                        serverSideDatasource: {
                            getRows: async params => {
                                var pr = params.request; //请求参数
                                var gkey = pr.groupKeys.slice(-1).pop(); //分组Key
                                if (gkey) {
                                    let url = `https://npmcdn.com/stats-product-category@1.0.0/${gkey.substring(0, 2)}.json`;
                                    url = nrcBase.mirrorNPM(url);
                                    let result = await nrWeb.reqServer(url);
                                    if (result) {
                                        var allRows = result.filter(x => x.pid == gkey);
                                        allRows.forEach(item => {
                                            item.group = result.filter(x => x.pid == item.id).length
                                        });

                                        if (pr.sortModel.length) {
                                            var sm = pr.sortModel.slice(-1).pop();
                                            if (sm.sort == "desc") {
                                                allRows.sort((a, b) => b[sm.colId] - a[sm.colId]);
                                            } else {
                                                allRows.sort((a, b) => a[sm.colId] - b[sm.colId]);
                                            }
                                        }

                                        var rows = pr.startRow != null && pr.endRow != null ? allRows.slice(pr.startRow, pr.endRow) : allRows;
                                        params.success({ rowData: rows, rowCount: allRows.length });
                                    } else {
                                        params.fail();
                                    }
                                } else {
                                    let url = 'https://npmcdn.com/stats-product-category@1.0.0/0.json';
                                    url = nrcBase.mirrorNPM(url);
                                    let allRows = await nrWeb.reqServer(url);
                                    if (allRows) {
                                        allRows.forEach(item => item.group = true);

                                        if (pr.sortModel.length) {
                                            var sm = pr.sortModel.slice(-1).pop();
                                            if (sm.sort == "desc") {
                                                allRows.sort((a, b) => b[sm.colId] - a[sm.colId]);
                                            } else {
                                                allRows.sort((a, b) => a[sm.colId] - b[sm.colId]);
                                            }
                                        }

                                        var rows = pr.startRow != null && pr.endRow != null ? allRows.slice(pr.startRow, pr.endRow) : allRows;
                                        params.success({ rowData: rows, rowCount: allRows.length });
                                    } else {
                                        params.fail();
                                    }
                                }
                            }
                        }
                    });

                    nrVary.domTxtFilter.readOnly = true;
                }
                break;
        }

        nrGrid.buildDom(nrVary.domGrid);
        nrApp.tsGrid = await nrGrid.viewGrid(nrVary.domGrid, gridOps);
    }
}

export { nrPage };