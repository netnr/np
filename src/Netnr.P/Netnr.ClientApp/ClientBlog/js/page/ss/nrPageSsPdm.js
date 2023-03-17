import { nrcFile } from "../../../../frame/nrcFile";
import { nrcBase } from "../../../../frame/nrcBase";
import { nrcUpstream } from "../../../../frame/nrcUpstream";
import { nrGrid } from "../../../../frame/nrGrid";
import { nrVary } from "../../nrVary";
import { nrApp } from "../../../../frame/Bootstrap/nrApp";

let nrPage = {
    pathname: "/ss/pdm",

    init: async () => {

        nrcBase.setHeightFromBottom(nrVary.domGrid);
        nrPage.bindEvent();
        nrcBase.dispatchEvent('hashchange');
    },

    bindEvent: () => {
        //过滤
        nrVary.domTxtFilter.addEventListener('input', async function () {
            if (nrApp.tsGrid) {
                nrApp.tsGrid.api.setQuickFilter(this.value);
            }
        });

        window.addEventListener('hashchange', async function () {
            await nrPage.openUrl();
        });

        //demo
        nrVary.domBtnDemo.addEventListener('click', function () {
            location.hash = 'https://gs.zme.ink/2021/07/02/125049306.pdm'
        });

        //接收文件
        nrcFile.init(files => nrPage.parse(files), nrVary.domTxtFile);
    },

    parse: async (files) => {
        await nrcBase.importScript('/file/ss-pdm.js');

        for (const file of files) {
            if (file.name.split('.').pop().toLowerCase() == "pdm") {
                let text = await nrcFile.reader(file);
                let result = pdm.parse(text);

                pdm.dataArray.push(result);
            }
        }

        await nrPage.view(pdm.gridData());
        nrVary.domGrid.classList.add('border');
    },

    view: async (rowData) => {
        if (nrApp.tsGrid) {
            nrApp.tsGrid.api.setRowData(nrApp.tsGrid.rowData = rowData)
        } else {
            let gridOptions = nrGrid.gridOptionsClient({
                columnDefs: [
                    { field: "ModelCode", headerName: "Model", rowGroup: true, hide: true },
                    { field: "Users", headerName: "Users", hide: true },
                    { field: "Tables", headerName: "Tables", hide: true },
                    { field: "TableName", headerName: "Table Name", rowGroup: true, hide: true },
                    { field: "TableCode", headerName: "Code", hide: true },
                    { field: "TableComment", headerName: "Table Comment", width: 300, tooltipField: "TableComment", hide: true },
                    { field: "ColumnName", headerName: "Name", hide: true },
                    { field: "ColumnCode", headerName: "Code" },
                    { field: "ColumnComment", headerName: "Comment", width: 300, tooltipField: "ColumnComment" },
                    { field: "ColumnDataType", headerName: "Data Type" },
                    nrGrid.newColumnNumber({ field: "ColumnLength", headerName: "Length", width: 120 }),
                    {
                        field: "ColumnPrimaryKey", headerName: "P", width: 150, cellRenderer: function (item) {
                            if (item.value) {
                                let vr = `<b title="Primary Key">🔑</b>`;
                                if (item.data.ColumnClusterObject) {
                                    vr += `<b class="ml-2" title="Cluster Object">📏</b>`;
                                }
                                return vr;
                            }
                        }
                    },
                    {
                        field: "ColumnMandatory", headerName: "M", width: 120, cellRenderer: function (item) {
                            return item.value ? `<b title="NOT NULL">✍</b>` : '';
                        }
                    },
                    {
                        field: "ColumnDefaultValue", headerName: "Default Value", cellRenderer: function (item) {
                            return item.value;
                        }
                    }
                ],
                autoGroupColumnDef: nrGrid.autoGroupColumnDef({
                    menuTabs: ['generalMenuTab', 'columnsMenuTab'],
                    field: 'ColumnName', width: 550, resizable: true, cellRendererParams: {
                        suppressCount: true, suppressEnterExpand: true, suppressDoubleClickExpand: true,
                        innerRenderer: function (item) {
                            let grow = nrApp.tsGrid.rowData.filter(x => x[item.node.field] == item.value);
                            let row = grow[0];
                            switch (item.node.field) {
                                case "ModelCode":
                                    {
                                        let subg = nrcBase.groupBy(grow, gi => gi.TableName);
                                        return `<b>${item.value}</b> (${subg.length}) <span class="badge text-bg-success mx-2">${row.PowerDesignerTarget}</span> <span class="badge text-bg-warning">v${row.PowerDesignerVersion}</span>`
                                    }
                                case "TableName":
                                    {
                                        let subg = nrcBase.groupBy(grow, gi => gi.ColumnCode);
                                        return `<b>${row.TableCode}</b> (${subg.length}) <span class="badge text-bg-info">${row.TableComment || ""}</span>`
                                    }
                                default:
                                    return `<b>${item.value}</b>`
                            }
                        }
                    }
                }),
                rowData: rowData
            });

            nrGrid.buildDom(nrVary.domGrid);
            nrApp.tsGrid = await nrGrid.viewGrid(nrVary.domGrid, gridOptions);

            //搜索
            nrVary.domTxtFilter.addEventListener("input", function () {
                nrApp.tsGrid.api.setQuickFilter(this.value.trim());
            });

            //展开节点
            setTimeout(function () {
                nrApp.tsGrid.api.forEachNode(function (node) {
                    if (node.group && node.level == 0) {
                        node.setExpanded(true);
                    }
                });
            }, 200)
        }
    },

    /**
     * 打开链接
     * @param {*} url 
     */
    openUrl: async (url) => {
        try {
            nrApp.setLoading(nrVary.domBtnDemo);

            if (!url) {
                url = location.hash.substring(1);
            }
            if (url.length > 4) {
                let text;
                if (url.includes("gs.zme.ink")) {
                    let resp = await fetch(url);
                    text = await resp.text();
                } else {
                    text = await nrcUpstream.fetch(url);
                }

                await nrcBase.importScript('/file/ss-pdm.js');
                pdm.dataArray.push(pdm.parse(text));

                await nrPage.view(pdm.gridData());
            }
        } catch (error) {
            nrApp.logError(error, '打开失败');
        }

        nrApp.setLoading(nrVary.domBtnDemo, true);
    },
}

export { nrPage };