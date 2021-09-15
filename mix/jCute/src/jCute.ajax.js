/**
 * ajax请求
 * @param {JQuery.AjaxSettings} settings 类似于jQuery传参
 * 
 * @param {string} url 请求地址
 * @param {string} type 请求方式，默认：GET
 * @param {boolean} async 异步请求，默认：true
 * @param {string} data 发送内容，字符串 | 键值对 | FormData
 * @param {string} contentType 内容编码类型，默认：application/x-www-form-urlencoded
 * @param {string} dataType 返回类型：json/xml/text，默认：text
 * @param {string} headers 消息头，键值对
 * @param {string} timeout 超时，单位：毫秒，默认设置超时
 * @param {string} progress function(p){ }，FormData上传进度回调方法 p：0-100
 * @param {string} success function(data,status,xhr){ }，成功回调方法，data：返回数据，
 * @param {string} error function(xhr,status){ }，错误回调方法
 * @param {string} complete:function(xhr,status){ }，完成回调
 */
jCute.ajax = function (settings) {
    settings.xhr = (window.XMLHttpRequest) ? (new XMLHttpRequest()) : (new ActiveXObject("Microsoft.XMLHTTP"));
    settings.type = (settings.type || "GET").toUpperCase();
    settings.async = settings.async == undefined ? true : settings.async;
    settings.data = settings.data || {};
    settings.headers = settings.headers || {};
    settings.contentType = settings.contentType || "application/x-www-form-urlencoded";
    settings.dataType = (settings.dataType || "text").toLowerCase();

    var darr = [], isfd = false;
    if (typeof settings.data == "object") {
        //FormData
        if (settings.data.__proto__.constructor.name == "FormData") {
            isfd = true;
            settings.type = "POST";
            settings.contentType = false;
            //进度监听
            settings.xhr.upload.onprogress = function (e) {
                if (e.lengthComputable) {
                    typeof settings.progress == "function" && settings.progress(((e.loaded / e.total) * 100).toFixed(0));
                }
            };
        } else {
            //键值对序列化
            for (var i in settings.data) {
                darr.push('&' + encodeURIComponent(i) + '=' + encodeURIComponent(settings.data[i]));
            }
        }
    }

    //GET请求，参数拼接
    if (settings.type == "GET" && darr.length) {
        settings.url += (settings.url.indexOf('?') >= 0 ? '&' : '?') + darr.join('').substr(1);
    }

    var ts = null;
    settings.xhr.onreadystatechange = function () {
        if (settings.xhr.readyState == 4) {
            //成功
            if (settings.xhr.status == 200) {
                //返回结果
                var data = settings.xhr.responseText;
                //成功回调
                if (typeof settings.success == "function") {
                    switch (settings.dataType) {
                        case 'json':
                            //解析json
                            var pj = true;
                            try {
                                if (typeof JSON == "function") {
                                    data = JSON.parse(data);
                                } else {
                                    data = (new Function("return " + data))();
                                }
                            }
                            catch (e) {
                                //解析失败
                                pj = false;
                                typeof settings.error == "function" && settings.error(settings.xhr, "parsererror");
                            }
                            pj && settings.success.call(settings, data, "success", settings.xhr);
                            break;
                        case 'xml':
                            settings.success.call(settings, settings.xhr.responseXML, "success", settings.xhr);
                            break;
                        default:
                            settings.success.call(settings, data, "success", settings.xhr);
                            break;
                    }
                }
            }
            else {
                //错误回调
                typeof settings.error == "function" && settings.error.call(settings, settings.xhr, ts || settings.xhr.statusText);
            }
            //完成回调
            typeof settings.complete == "function" && settings.complete.call(settings, settings.xhr, ts || settings.xhr.statusText);
        }
    };

    //监听异步超时
    if (settings.async && settings.timeout > 0) {
        setTimeout(function () {
            if (settings.xhr.readyState != 4) {
                ts = "timeout";
                //停止请求
                settings.xhr.abort();
            }
        }, settings.timeout)
    }

    //open
    settings.xhr.open(settings.type, settings.url, settings.async);

    //设置header,非FromData
    if (!isfd) {
        settings.xhr.setRequestHeader("X-Requested-With", "XMLHttpRequest");
        settings.contentType && settings.xhr.setRequestHeader("Content-Type", settings.contentType);
    }
    //追加headers
    for (var i in settings.headers) {
        settings.xhr.setRequestHeader(i, settings.headers[i]);
    }

    //发送前回调
    if (typeof settings.beforeSend == "function") {
        if (settings.beforeSend.call(settings, settings) == false) {
            return false;
        }
    }

    //send
    settings.xhr.send(settings.data);
};

