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
    isgif: false,
    model: null,

    init: function () {
        //接收文件
        nr.receiveFiles(function (files) {
            var isImg = false;
            for (var i = 0; i < files.length; i++) {
                var file = files[i];
                page.isgif = file.type == "image/gif";
                if (file.type.indexOf("image") != -1) {
                    isImg = true;
                    page.viewImage(file);
                    break;
                }
            }

            if (!isImg) {
                nr.alert('不是图片哦');
            }
        }, nr.domTxtFile);

        page.modelPath();
    },

    viewImage: function (file) {
        var img = new Image();
        img.classList.add('mw-100', 'rounded');
        img.style.maxHeight = '18em';
        img.onload = function () {
            page.scan();
        }
        img.src = URL.createObjectURL(file);
        page.img = img;

        nr.domCardView.style.height = '20em';
        nr.domCardView.innerHTML = "";
        nr.domCardView.appendChild(img);
        nr.domCardView.classList.add('p-3', 'border', 'rounded');
    },

    modelPath: function () {
        if (!page.modelVersion) {
            document.scripts.forEach(si => {
                if (si.src.includes('/nsfwjs@')) {
                    page.modelVersion = /nsfwjs(@\d+.\d+.\d+)/.exec(si.src)[0];
                }
            })
            page.modelVersion = 'nsfwjs@2.4.0';
        }
        return 'https://cdn.staticaly.com/gh/infinitered/nsfwjs/master/example/nsfw_demo/public/quant_nsfw_mobilenet/';
    },

    scan: () => new Promise(() => {
        nr.domLoading.classList.remove('d-none');
        nr.domTxtResult.value = "正在加载引擎...";
        nr.domCardChart.innerHTML = "";
        if (page.model) {
            page.classify();
        } else {
            nsfwjs.load(page.modelPath()).then(function (model) {
                page.model = model;
                page.classify();
            })
        }
    }),

    classify: function () {
        nr.domTxtResult.value = "正在识别...";

        var mf = page.isgif ? "classifyGif" : "classify";
        page.model[mf](page.img).then(function (predictions) {
            console.debug(predictions);
            nr.domLoading.classList.add('d-none');
            nr.domTxtResult.value = JSON.stringify(predictions, null, 2);
            nr.domTxtResult.parentElement.classList.remove('invisible');

            if (!page.isgif) {
                predictions = [predictions];
            }

            page.chart(predictions);
        })
    },

    chart: function (results) {
        var categories = {
            Hentai: "Hentai（变态）",
            Porn: "Porn（色情）",
            Sexy: "Sexy（性感）",
            Neutral: "Neutral（中立）",
            Drawing: "Drawing（绘画）",
        }, data = [], cats = [];

        for (let index = 0; index < results.length; index++) {
            cats.push(`${index + 1} 帧`);
            var items = results[index], ci = 0;
            for (const key in categories) {
                var col = items.filter(item => item.className == key)[0];
                var obj = data[ci] || { name: categories[key], data: [] };
                obj.data.push(col.probability.toFixed(6) * 1);
                data[ci++] = obj;
            }
        }

        nr.domCardChart.style.height = (results.length * 2 + 10) + 'em'
        Highcharts.chart(nr.domCardChart, {
            chart: { type: 'bar' },
            title: { text: '识别结果' },
            xAxis: { categories: cats },
            yAxis: { min: 0, title: { text: '识别指数' } },
            legend: { reversed: true },
            colors: ['#28a745', '#17a2b8', '#ffc107', '#dc3545', '#343a40'].reverse(),
            plotOptions: { series: { stacking: 'normal' } },
            series: data
        });
    }
}