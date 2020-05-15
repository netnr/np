function loadPVUV(type, group) {
    $.getJSON('/Admin/QueryLogStatsPVUV?type=' + type + '&LogGroup=' + group, null, function (res) {
        var categories = [], pv = [], uv = [], spv = 0, suv = 0;
        (res.Data || []).forEach(x => {
            spv += x.pv;
            suv += x.ip;
            categories.push(x.time);
            pv.push(x.pv);
            uv.push(x.ip);
        });
        series = [{ name: "PV", data: pv }, { name: "UV", data: uv }];
        Highcharts.chart('chart1', {
            chart: {
                type: 'line'
            },
            title: {
                text: 'PV（' + spv + '） / UV（' + suv + '）'
            },
            xAxis: {
                categories: categories
            },
            yAxis: {
                title: {
                    text: 'Total'
                }
            },
            plotOptions: {
                line: {
                    dataLabels: {
                        // 开启数据标签
                        enabled: true
                    },
                    // 关闭鼠标跟踪，对应的提示框、点击事件会失效
                    enableMouseTracking: false
                }
            },
            series: series,
            credits: {
                enabled: false
            }
        });
    })
}

function loadTop(type, field, group) {
    $.getJSON('/Admin/QueryLogReportTop?type=' + type + '&field=' + field + '&LogGroup=' + group, null, function (res) {
        var data = res.Data || [], arr = [], total = 0;
        data.forEach(x => total += x.total);
        data.forEach(x => {
            x.y = (x.total / total * 100).toFixed(2) * 1;
            x.name = x.field;
            x.p = x.y + "%";
        });

        switch (field) {
            case "LogUrl":
                {
                    arr.push('<ul>');

                    data.forEach(function (x) {
                        arr.push('<li>');
                        var url = location.origin + x.field;
                        arr.push('<a target="_blank" href="' + url + '">' + url + '</a>');
                        arr.push(' &nbsp; ' + x.total);
                        arr.push(' &nbsp; ' + x.p);
                        arr.push('</li>');
                    });
                    arr.push('</ul>');
                }
                break;
            case "LogReferer":
                {
                    arr.push('<ul>');

                    data.forEach(function (x) {
                        arr.push('<li>');
                        if (x.field == "") {
                            arr.push('unknown');
                        } else {
                            arr.push('<a target="_blank" href="' + x.field + '">' + x.field + '</a>');
                        }
                        arr.push(' &nbsp; ' + x.total);
                        arr.push(' &nbsp; ' + x.p);
                        arr.push('</li>');
                    });
                    arr.push('</ul>');
                }
                break;
            case "LogSystemName":
            case "LogBrowserName":
                {
                    Highcharts.chart('field' + field, {
                        chart: {
                            plotBackgroundColor: null,
                            plotBorderWidth: null,
                            plotShadow: false,
                            type: 'pie'
                        },
                        title: {
                            text: '【' + field + '】Total：' + total
                        },
                        tooltip: {
                            pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
                        },
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
                        series: [{
                            name: 'Brands',
                            colorByPoint: true,
                            data: data
                        }],
                        credits: {
                            enabled: false
                        }
                    });
                }
        }

        if (arr.length) {
            var msg = "<h4 class='text-center'>【" + field + "】Total：" + total + "</h4>" + arr.join('');
            $('#field' + field).html(msg);
        }

    });
}

function init() {
    var st = $('#setime').val(), sg = $('#segroup').val();

    loadPVUV(st, sg);
    loadTop(st, 'LogUrl', sg);
    loadTop(st, 'LogReferer', sg);
    loadTop(st, 'LogSystemName', sg);
    loadTop(st, 'LogBrowserName', sg);
}
init();

$('#setime').change(function () {
    init();
})
$('#segroup').change(function () {
    init();
})