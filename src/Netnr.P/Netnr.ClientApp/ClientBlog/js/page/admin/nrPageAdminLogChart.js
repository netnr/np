import { nrWeb } from "../../nrWeb";
import { nrVary } from "../../nrVary";
import { nrECharts } from "../../../../frame/nrECharts";

let nrPage = {
    pathname: '/admin/logchart',

    init: async () => {
        nrVary.domPVUV = document.createElement('div');
        nrVary.domPVUV.className = 'nrg-chart col-lg-12 border-top py-5';
        nrVary.domPVUV.style.height = "25em";
        nrVary.domStats.appendChild(nrVary.domPVUV);

        nrPage.fields.forEach((field, index) => {
            let dom = document.createElement('div');
            if (index < 2) {
                //图表
                dom.className = 'nrg-chart col-lg-6 border-top py-5';
                dom.style.height = "30em";
            } else {
                dom.className = 'col-lg-6 border-top py-5';
            }
            nrVary.domStats.appendChild(dom);
            nrVary[`dom${field}`] = dom;
        });

        nrPage.bindEvent();

        await nrPage.load();
    },

    bindEvent: () => {
        nrVary.domSeDate.addEventListener('input', nrPage.load);
        nrVary.domSeGroup.addEventListener('input', nrPage.load);
    },

    load: async () => {
        await nrPage.viewChart();
        nrPage.fields.forEach(field => {
            nrPage.viewTop(field);
        });
    },

    fields: ["LogSystemName", "LogBrowserName", "LogAction", "LogReferer"],

    viewChart: async () => {
        let url = `/Admin/QueryLogStatsPVUV?days=${nrVary.domSeDate.value}&LogGroup=${nrVary.domSeGroup.value}`
        let result = await nrWeb.reqServer(url);

        let data = result.RowData || [];
        //按时间升序
        data.sort((a, b) => a.time.localeCompare(b.time));

        let totalPV = 0;
        let totalUV = 0;
        data.forEach(item => {
            totalPV += item.pv;
            totalUV += item.uv;
        })

        const option = {
            title: { left: 'center', text: `PV(${totalPV})  UV(${totalUV})` },
            tooltip: { trigger: 'axis', show: true, },
            legend: { bottom: 0, data: ['PV', 'UV'] },
            xAxis: { type: 'category', boundaryGap: false, data: data.map(x => x.time) },
            yAxis: { type: 'value' },
            series: [
                {
                    name: 'PV', type: 'line', smooth: true,
                    data: data.map(x => x.pv)
                },
                {
                    name: 'UV', type: 'line', smooth: true,
                    data: data.map(x => x.uv)
                }
            ]
        };
        await nrECharts.bind(nrVary.domPVUV, option);
    },

    viewTop: async (field) => {
        let url = `/Admin/QueryLogStatsTop?days=${nrVary.domSeDate.value}&LogGroup=${nrVary.domSeGroup.value}&field=${field}`
        let result = await nrWeb.reqServer(url);

        let data = result.RowData || [];
        let sum = 0;
        data.forEach(item => sum += item.total);
        data.forEach(item => {
            item.p = `${(item.total / sum * 100).toFixed(2)}%`;
        });

        switch (field) {
            case "LogSystemName":
            case "LogBrowserName":
                {
                    data.length = 5;
                    const option = {
                        title: { left: 'center', text: `${field}(${sum})` },
                        tooltip: { trigger: 'item' },
                        legend: { bottom: 0 },
                        series: [
                            {
                                type: 'pie',
                                radius: ['40%', '60%'],
                                avoidLabelOverlap: false,
                                itemStyle: {
                                    borderRadius: 10,
                                },
                                data: data.map(x => ({ value: x.total, name: x.field }))
                            }
                        ]
                    };
                    await nrECharts.bind(nrVary[`dom${field}`], option);
                }
                break;
            case "LogAction":
            case "LogReferer":
                {
                    data.length = 20;
                    let htm = [];
                    htm.push(`<h4 class='text-center'>${field}(${sum})</h4>`);
                    htm.push('<ol class="list-group list-group-numbered">');

                    data.forEach(item => {
                        let href = item.field;
                        let link = 'unknown'
                        if (field == "LogAction") {
                            href = location.origin + "/" + item.field;
                        }
                        if (href != "") {
                            link = `<a class="text-break" target="_blank" href="${href}">${href}</a>`;
                        }

                        htm.push(`
<li class="list-group-item d-flex justify-content-between align-items-start">
<div class="ms-2 me-auto">${link}</div>
<span class="badge bg-success rounded-pill mx-2">${item.total}</span>
<span class="badge bg-primary">${item.p}</span>
</li>
                        `)
                    })
                    htm.push('</ol>');

                    nrVary[`dom${field}`].innerHTML = htm.join('');
                }
                break;
        }
    }
}

export { nrPage };