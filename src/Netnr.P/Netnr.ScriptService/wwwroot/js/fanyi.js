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

var defer;
$("#txt").input(function () {
    clearTimeout(defer);
    defer = setTimeout(function () { transl(); }, 1000);
}).keydown(function () {
    if (this.value.length > 200) { this.value = this.value.substr(0, 200); }
})[0].focus();

$(document).keydown(function (e) {
    e = e || window.event;
    var keyCode = e.keyCode || e.which || e.charCode;
    if (e.ctrlKey && keyCode == 13) { transl(); }
})
$("#btnTransl").click(transl);
function transl() {
    if ($.trim($("#txt").val()) != "") {
        loading();
        ss.ajax({
            url: "http://fanyi.youdao.com/openapi.do?keyfrom=NET-BBS&key=1734763581&type=data&doctype=json&version=1.1&q=" + encodeURIComponent($("#txt").val()),
            dataType: "json",
            success: function (data) {
                data = ss.datalocation(data);
                if (data.errorCode == "0") {
                    var basic = '';
                    if ('basic' in data) {
                        if ('phonetic' in data.basic) {
                            basic += ('<p><span>发音：</span>[' + data.basic.phonetic + ']<p>');
                        }

                        basic += '<p><span>基本词义：</span>';
                        if ($.isArray(data.basic.explains)) {
                            basic += data.basic.explains.join('，');
                        } else {
                            basic += data.basic.explains;
                        }
                    }

                    $("#divTranslResult").html('<p>' + data.translation + '</p><p>' + basic + '</p>');
                }
            },
            error: function () {
                loading(0);
                $("#divTranslResult").html('<span style="color:red">网络错误</span>');
            },
            complete: function () {
                loading(0);
            }
        });
    } else { $("#divTranslResult").html('') }
}