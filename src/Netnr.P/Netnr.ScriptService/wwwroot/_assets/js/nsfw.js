var pg = {
    isgif: false,
    model: null,
    rtorder: [],
    init: function () {
        $('.nrResultType').find('input').each(function () {
            pg.rtorder.push({
                key: this.value.split('（')[0],
                name: this.value
            });
        });

        //接收文件
        ss.receiveFiles(function (files) {
            var isImg = false;
            for (var i = 0; i < files.length; i++) {
                var file = files[i];
                pg.isgif = file.type == "image/gif";
                if (file.type.indexOf("image") != -1) {
                    isImg = true;
                    pg.iv.onload = function () {
                        pg.scan();
                    }
                    pg.iv.src = URL.createObjectURL(file);
                    break;
                }
            }
            if (!isImg) {
                bs.alert('<h4>不是图片哦</h4>');
                pg.iv.src = `${ss.apiServer}/v1/SVGPIG/400*200`;
            }
        }, "#txtFile");

        pg.modelPath();
    },
    iv: document.getElementById('imgview'),

    modelPath: function () {
        if (!pg.modelVersion) {
            var bhtm = document.body.innerHTML;
            pg.modelVersion = bhtm.substr(bhtm.indexOf("/nsfwjs"), 20).substring(8).split('/')[0];
        }
        return `https://cdn.jsdelivr.net/gh/infinitered/nsfwjs@${pg.modelVersion}/example/nsfw_demo/public/quant_nsfw_mobilenet/`
    },

    scan: function () {
        $('.sbox-scanner').removeClass('d-none');
        $('#txtResult').val("正在加载引擎...")
        if (pg.model) {
            pg.classify();
        } else {
            nsfwjs.load(pg.modelPath()).then(function (model) {
                pg.model = model;
                pg.classify();
            })
        }
    },

    classify: function () {
        $('#txtResult').val("正在识别...")

        var mf = pg.isgif ? "classifyGif" : "classify";
        pg.model[mf](pg.iv).then(function (predictions) {
            console.log(predictions);

            if (pg.isgif) {
                var oarr = [];
                predictions.forEach(parr => {
                    var ro = {};
                    parr.forEach(p => {
                        ro[p.className] = p.probability.toFixed(20);
                    });
                    oarr.push(ro);
                });
                $('#txtResult').val(JSON.stringify(oarr, null, 4))
                pg.chart(oarr);
            } else {
                var ro = {};
                predictions.forEach(p => {
                    ro[p.className] = p.probability.toFixed(20);
                });
                $('#txtResult').val(JSON.stringify(ro, null, 4))
                pg.chart([ro]);
            }

            $('.sbox-scanner').addClass('d-none');
        })
    },

    chart: function (results) {
        var nrc = $('.nrChart').empty()
        for (var i = 0; i < results.length; i++) {
            var ro = results[i], data = [];
            for (var j = 0; j < pg.rtorder.length; j++) {
                var o = pg.rtorder[j];
                data.push({
                    name: o.name,
                    y: ro[o.key] * 1
                });
            }

            var colsize = results.length > 1 ? "col-md-6 col-xl-4" : "col-md-12";

            $('<div style="height:360px;" class="' + colsize + ' px-3 mb-3"></div>').appendTo(nrc).highcharts({
                chart: {
                    plotBackgroundColor: null,
                    plotBorderWidth: null,
                    plotShadow: false
                },
                title: {
                    text: '识别结果占比' + (results.length > 1 ? '（' + (i + 1) + ' 帧）' : '')
                },
                colors: ['#28a745', '#17a2b8', '#ffc107', '#dc3545', '#343a40'],
                tooltip: {
                    headerFormat: '{series.name}<br>',
                    pointFormat: '{point.name}: <b>{point.percentage:.1f}%</b>'
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
                    type: 'pie',
                    name: '识别结果占比',
                    data: data
                }]
            });
        }

    }
}

pg.init();