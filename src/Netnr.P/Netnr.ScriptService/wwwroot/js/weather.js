$.prototype.init.prototype["input"] = function (callback) {
    if (!-[1,]) {
        $.each(this, function () {
            if (this.nodeType == 1) {
                var that = this;
                this.attachEvent("onpropertychange", function () {
                    if (event.propertyName.toLowerCase() == "value") {
                        callback.apply(that, arguments);
                    }
                });
            }
        });
    } else {
        $.each(this, function () {
            this.nodeType == 1 && $(this).on("input", callback);
        });
    }
    return this;
}

//请求天气预报信息
QueryWeather(101040100);
function QueryWeather(cityCode) {
    ss.loading();
    ss.ajax({
        url: "http://wthrcdn.etouch.cn/weather_mini?citykey=" + cityCode,
        dataType: "json",
        success: function (data) {
            data = ss.datalocation(data);
            if (data.desc == "OK") {
                var jsons = data.data.forecast,
                    htm = '<div class="col-sm-12"><em>' + data.data.city + '</em>实时温度：<b>' + data.data.wendu + '℃</b></div>';
                for (var i in jsons) {
                    var lx = jsons[i].type,
                        fx = jsons[i].fengxiang,
                        fl = jsons[i].fengli,
                        gw = jsons[i].high,
                        dw = jsons[i].low,
                        rq = jsons[i].date;
                    i == 0 && (rq = '今日天气');

                    htm += '<div class="col-md-4 col-sm-6">'
                        + '<b>' + rq + '</b>'
                        + '<strong>' + lx + '</strong><br/>'
                        + '<span>' + fl + '</span>'
                        + '<span>' + dw + '</span>'
                        + '<span>' + gw + '</span>'
                        + '</div>';
                }
                htm += '<div class="col-md-4 col-sm-6">' + data.data.ganmao + '</div>';
                $("#divweather").html(htm);
            } else {
                jz.alert("查询失败")
            }
        },
        error: function () {
            ss.loading(0);
            jz.msg("网络错误");
        },
        complete: function () {
            ss.loading(0);
        }
    });
}

//城市联动查询
$('#txtSearch').input(function () {
    AutoLoadWeatherCode(this.value);
}).focus(function () {
    $('#divWeatherCode').show();
    AutoLoadWeatherCode(this.value);
});
function AutoLoadWeatherCode(value) {
    var htmls = [];
    if (value == "") {
        $(WeatherCode).each(function () {
            htmls.push('<div class="card mb-3">');
            htmls.push('<div class="card-body">');
            htmls.push('<h5 class="card-title">' + this.city + '</h4>');
            $(this.citys).each(function () {
                htmls.push('<a href="javascript:void(0)" data-code="' + this.code + '" >' + this.name + '</a>');
            });
            htmls.push('</div></div>');
        });
    } else {
        $(WeatherCode).each(function () {
            var children = [];
            $(this.citys).each(function () {
                if (this.name.indexOf(value) >= 0 || this.py.indexOf(value.toUpperCase()) >= 0) {
                    children.push('<a href="javascript:void(0)" data-code="' + this.code + '" >' + this.name + '</a>');
                }
            });
            if (children.length) {
                htmls.push('<div class="card mb-3">');
                htmls.push('<div class="card-body">');
                htmls.push('<h5 class="card-title">' + this.city + '</h4>');
                htmls.push(children.join(''));
                htmls.push('</div></div>');
            }
        });
    }
    if (htmls.length) {
        $('#divWeatherCode').html(htmls.join(''));
    } else {
        $('#divWeatherCode').html('( ⊙ o ⊙ ) ， 没有相关城市信息');
    }
}
//点击城市发起查询
$('#divWeatherCode').click(function (e) {
    e = e || window.event;
    var target = e.target || e.srcElement;
    if (target.nodeName == "A") {
        var code = target.getAttribute('data-code');
        QueryWeather(code, target.innerHTML);
        $('#divWeatherCode').hide();
    }
}).css('max-height', $(window).height() - $('#txtSearch')[0].getBoundingClientRect().top - 80 + "px");
//自适应高度
$(window).resize(function () {
    $('#divWeatherCode').css('max-height', $(window).height() - $('#txtSearch')[0].getBoundingClientRect().top - 80 + "px");
});

//点击空白关闭城市浮动层
$(document).click(function (e) {
    e = e || window.event;
    var target = e.target || e.srcElement;
    if (!$('#divWeatherCode')[0].contains(target) && target.id != "txtSearch") {
        $('#divWeatherCode').hide();
    }
});