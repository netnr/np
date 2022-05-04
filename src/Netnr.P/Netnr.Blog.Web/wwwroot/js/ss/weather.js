nr.onReady = function () {
    nr.domTxtQuery.addEventListener('sl-focus', function () {
        page.autoWeatherCode(this.value);
    });
    nr.domTxtQuery.addEventListener('sl-input', function () {
        page.autoWeatherCode(this.value);
    });
    document.addEventListener('click', function (e) {
        var target = e.target;
        if (nr.domCardWeatherCode.contains(target)) {
            if (target.nodeName == "A") {
                page.queryWeather(target.dataset.code);
                nr.domCardWeatherCode.classList.add('d-none');
            }
        } else if (!nr.domTxtQuery.contains(target)) {
            nr.domCardWeatherCode.classList.add('d-none');
        }
    });

    page.queryWeather('101040100');
}

var page = {
    autoWeatherCode: function (value) {
        Object.assign(nr.domCardWeatherCode.style, {
            top: 0,
            zIndex: 2,
            maxHeight: '60vh',
            backgroundColor: 'var(--sl-panel-background-color)',
        })

        var htmls = [];
        WeatherCode.forEach(item1 => {
            var children = [];
            item1.citys.forEach(item2 => {
                if (item2.name.includes(value) || item2.py.includes(value.toUpperCase())) {
                    children.push(`<a href="javascript:void(0)" class="me-3 text-nowrap" data-code="${item2.code}">${item2.name}</a>`);
                }
            });
            if (children.length) {
                htmls.push(`<div class="m-3">${item1.name}：${children.join('')}</div>`);
            }
        });

        nr.domCardWeatherCode.classList.remove('d-none');
        if (htmls.length) {
            nr.domCardWeatherCode.innerHTML = htmls.join('');
        } else {
            nr.domCardWeatherCode.innerHTML = '<div class="m-3">( ⊙ o ⊙ ) ， 没有相关城市信息</div>';
        }
    },
    queryWeather: function (code) {
        ss.loading(true);
        
        ss.fetch({
            url: `http://wthrcdn.etouch.cn/weather_mini?citykey=${code}`
        }).then(res => {
            ss.loading(false);

            res = JSON.parse(res);
            if (res.desc == "OK") {
                var htm = `<div class="col-12">
                    <b class="me-3">${res.data.city}</b><b>${res.data.wendu}℃</b>
                </div>`, index = 0;

                res.data.forecast.forEach(item => {
                    htm += `<div class="col-12 mt-3">
                        <b class="me-3">${index == 0 ? "今日天气" : item.date}</b>
                        <span class="me-3">${item.type}</span>
                        <span class="me-3">${item.low}</span>
                        <span class="me-3">${item.high}</span>
                        <span>${item.fengxiang}</span>
                    </div>`;
                    index++;
                });

                var yesterday = res.data.yesterday;
                htm += `<div class="col-12 mt-3 opacity-75">
                    <p>${res.data.ganmao}</p>
                    <b class="me-3">昨日天气</b>
                    <span class="me-3">${yesterday.type}</span>
                    <span class="me-3">${yesterday.low}</span>
                    <span class="me-3">${yesterday.high}</span>
                    <span>${nr.htmlDecode(yesterday.fl)}</span>
                </div>`;

                nr.domCardResult.innerHTML = htm;
            } else {
                nr.alert("查询失败")
            }
        }).catch(ex => {
            console.debug(ex);
            ss.loading(false);
            nr.alert("网络错误")
        })
    }
}