nr.onReady = function () {
    if (nr.isDark()) {
        var si = nr.findScript('highcharts.js');
        if (si) {
            var dom = document.createElement("script");
            dom.src = si.src.replace("highcharts.js", "themes/dark-unica.js");
            dom.onload = function () {
                page.init();
            }
            document.body.appendChild(dom);
        }
    } else {
        page.init();
    }
}

var page = {
    domStats: document.querySelector('.nr-stats'),
    domSeDate: document.querySelector('.nr-se-date'),
    domSeGroup: document.querySelector('.nr-se-group'),
    fields: ["LogSystemName", "LogBrowserName", "LogAction", "LogReferer"],
    init: () => {
        page.domPVUV = document.createElement('div');
        page.domPVUV.className = 'col-12 mb-3';
        page.domStats.appendChild(page.domPVUV);

        page.fields.forEach(field => {
            var dom = document.createElement('div');
            dom.className = 'col-md-6 mb-3';
            page.domStats.appendChild(dom);
            page[`dom${field}`] = dom;
        });

        page.domSeDate.addEventListener('sl-change', function () {
            page.loadPVUV();
            page.loadTop();
        }, false);

        page.domSeGroup.addEventListener('sl-change', function () {
            page.loadPVUV();
            page.loadTop();
        }, false);

        page.loadPVUV();

        page.fields.forEach(field => {
            page.loadTop(field);
        });
    },
    loadPVUV: () => {
        var days = page.domSeDate.value;
        var group = page.domSeGroup.value;

        fetch(`/Admin/QueryLogStatsPVUV?days=${days}&LogGroup=${group}`).then(x => x.json()).then(res => {
            var categories = [], pv = [], uv = [], spv = 0, suv = 0;

            (res.RowData || []).forEach(item => {
                spv += item.pv;
                suv += item.ip;
                categories.push(item.time);
                pv.push(item.pv);
                uv.push(item.ip);
            });

            series = [{ name: "PV", data: pv }, { name: "UV", data: uv }];

            Highcharts.chart(page.domPVUV, {
                chart: { type: 'line' },
                title: { text: 'PV(' + spv + ')/ UV(' + suv + ')' },
                xAxis: { categories: categories },
                yAxis: { title: { text: 'Total' } },
                plotOptions: {
                    line: { dataLabels: { enabled: true }, enableMouseTracking: false }
                },
                tooltip: { shared: true, crosshairs: true, dateTimeLabelFormats: { day: '%Y-%m-%d' } },
                series: series, credits: { enabled: false }
            });
        });
    },
    loadTop: (field) => {
        var days = page.domSeDate.value;
        var group = page.domSeGroup.value;

        fetch(`/Admin/QueryLogStatsTop?days=${days}&field=${field}&LogGroup=${group}`).then(x => x.json()).then(res => {
            var data = res.RowData || [], arr = [], total = 0;

            data.forEach(item => total += item.total);
            data.forEach(item => {
                item.y = (item.total / total * 100).toFixed(2) * 1;
                item.name = item.field;
                item.p = item.y + "%";
            });

            switch (field) {
                case "LogAction":
                    {
                        arr.push('<ul>');

                        data.forEach(item => {
                            arr.push('<li>');
                            var url = location.origin + "/" + item.field;
                            arr.push('<a class="text-break" target="_blank" href="' + url + '">' + url + '</a>');
                            arr.push(' &nbsp; ' + item.total);
                            arr.push(' &nbsp; ' + item.p);
                            arr.push('</li>');
                        })
                        arr.push('</ul>');
                    }
                    break;
                case "LogReferer":
                    {
                        arr.push('<ul>');

                        data.forEach(item => {
                            arr.push('<li>');
                            if (item.field == "") {
                                arr.push('unknown');
                            } else {
                                arr.push('<a class="text-break" target="_blank" href="' + item.field + '">' + item.field + '</a>');
                            }
                            arr.push(' &nbsp; ' + item.total);
                            arr.push(' &nbsp; ' + item.p);
                            arr.push('</li>');
                        });
                        arr.push('</ul>');
                    }
                    break;
                case "LogSystemName":
                case "LogBrowserName":
                    {
                        Highcharts.chart(page[`dom${field}`], {
                            chart: { plotBackgroundColor: null, plotBorderWidth: null, plotShadow: false, type: 'pie' },
                            title: { text: field + ' Total(' + total + ')' },
                            tooltip: { pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>' },
                            plotOptions: {
                                pie: {
                                    allowPointSelect: true,
                                    cursor: 'pointer',
                                    dataLabels: {
                                        enabled: true,
                                        format: '<b>{point.name}</b>: {point.percentage:.1f} %',
                                        style: {
                                            color: (Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black'
                                        }
                                    }
                                }
                            },
                            series: [{ name: 'Brands', colorByPoint: true, data: data }],
                            credits: { enabled: false }
                        });
                    }
            }

            if (arr.length) {
                var msg = "<h4 class='text-center'>" + field + "(" + total + ")</h4>" + arr.join('');
                page[`dom${field}`].innerHTML = msg;
            }
        })
    }
}