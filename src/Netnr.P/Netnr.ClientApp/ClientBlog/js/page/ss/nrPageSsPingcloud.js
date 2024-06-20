import { nrApp } from "../../../../frame/Bootstrap/nrApp";
import { nrcBase } from "../../../../frame/nrcBase";
import { nrGrid } from "../../../../frame/nrGrid";
import { nrVary } from "../../nrVary";

let nrPage = {
    pathname: "/ss/pingcloud",

    init: async () => {
        await nrPage.view();

        nrcBase.setHeightFromBottom(nrVary.domGrid);
        nrPage.bindEvent();
    },

    bindEvent: () => {
        //过滤
        nrApp.setQuickFilter(nrVary.domTxtFilter, nrApp.tsGrid);
    },

    view: async () => {
        await nrcBase.importScript('/file/data-cloud-node.js');

        let data = [];
        dataCloudNode.forEach(p => {
            p.list.forEach(item => {
                data.push({
                    vender: p.name,
                    venderAlias: p.alias,
                    area: item.area,
                    alias: item.alias,
                    endpoint: item.endpoint
                })
            });
        })

        let gridOptions = nrGrid.gridOptionsClient({
            columnDefs: [
                { field: "vender", rowGroup: true, hide: true },
                { field: "venderAlias", hide: true },
                { field: "area", width: 240 },
                { field: "alias", width: 240 },
                {
                    field: "ping", cellRenderer: params => {
                        if (params.node.group) {
                            return `<span class="d-flex" role="button">Ping Vender</span>`
                        } else if (params.value == null) {
                            return `<span class="d-flex" role="button">Ping</span>`
                        } else if (typeof params.value == "number") {
                            let fc = 'limegreen', ms = params.value;
                            if (ms > 100) {
                                fc = 'teal';
                            }
                            if (ms > 200) {
                                fc = 'orange';
                            }
                            if (ms > 300) {
                                fc = 'orangered';
                            }
                            if (ms > 500) {
                                fc = 'red';
                            }
                            if (ms == nrPage.pingTimeOut) {
                                ms = 'timeout';
                            }
                            return '<b style="color:' + fc + '">' + ms + '</b>';
                        }
                        return params.value
                    }
                },
                { field: "endpoint", flex: 1 },
            ],
            rowData: data,
            getRowId: event => event.data.endpoint,
            onCellClicked: function (params) {
                if (params.column.colId == "ping") {
                    if (params.node.group) {
                        //展开
                        params.node.setExpanded(true);

                        nrPage.queue.list = []; //清空队列

                        //加入队列
                        params.node.childrenAfterFilter.forEach(c => {
                            nrPage.queue.list.push(c.data)
                        })

                        //开始测速
                        nrPage.task();
                    } else {
                        let row = params.data;
                        row.ping = "Pinging...";
                        nrPage.setValue(row);

                        nrPage.ping(params.data.endpoint, function (useTime) {
                            row.ping = useTime;
                            nrPage.setValue(row);
                            nrPage.ping(params.data.endpoint, function (useTime) {
                                row.ping = useTime;
                                nrPage.setValue(row);
                            });
                        });
                    }
                }
            }
        });

        nrGrid.buildDom(nrVary.domGrid);
        nrApp.tsGrid = await nrGrid.createGrid(nrVary.domGrid, gridOptions);
    },
    setValue: (row) => {
        nrApp.tsGrid.applyTransaction({ update: [row] });
    },

    //超时
    pingTimeOut: 1000 * 3,

    //Ping
    ping: function (url, cb) {
        let img = new Image(), start = new Date().valueOf(), st = setTimeout(function () { cb(nrPage.pingTimeOut) }, nrPage.pingTimeOut);
        img.onload = img.onerror = function () {
            let end = new Date().valueOf(), useTime = end - start;
            if (useTime < nrPage.pingTimeOut) {
                clearTimeout(st);
                cb(useTime, start, end)
            }
        }
        img.src = url;
    },

    //任务
    task: function (row) {
        row = row || nrPage.queue.use();
        nrPage.queue.status = 1;

        row.ping = "Pinging...";
        nrPage.setValue(row);
        nrPage.ping(row.endpoint, function (useTime) {
            row.ping = useTime;
            nrPage.setValue(row);
            nrPage.ping(row.endpoint, function (useTime) {
                row.ping = useTime;
                nrPage.setValue(row);

                //可消费
                let newi = nrPage.queue.use();
                if (newi) {
                    nrPage.task(newi);
                } else {
                    nrPage.queue.status = 0;
                }
            })
        })
    },

    //队列
    queue: {
        list: [],
        use: function () {
            if (nrPage.queue.list.length) {
                return nrPage.queue.list.splice(0, 1)[0];
            }
        }
    },
}

export { nrPage };