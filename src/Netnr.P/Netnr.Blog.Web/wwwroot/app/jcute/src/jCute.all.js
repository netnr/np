/*!
 * jCute JavaScript Library
 *
 * Date: 2018-05
 * Author: netnr
 */

var jCute = function (selector) { return new jCute.fn.init(selector); };

jCute.fn = jCute.prototype = {
    init: function (selector) {
        //TO DO

        return this;
    }
};

jCute.fn.init.prototype = jCute.fn;

window.jCute = window.cu = jCute;

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

    //设置header,非FormData
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

/**
 * Unicode编码
 * @param {string} str 字符串
 */
jCute.toUnicode = function (str) {
    var val = "", i = 0, c, len = str.length;
    for (; i < len; i++) {
        c = str.charCodeAt(i).toString(16);
        while (c.length < 4) {
            c = '0' + c;
        }
        val += '\\u' + c
    }
    return val
};

/**
 * Unicode解码
 * @param {string} str 字符串
 */
jCute.toUnicodeUn = function (str) {
    return eval("'" + str + "'");/*return unescape(str.replace(/\u/g, "%u"))*/
};


/**
 * Ascii编码
 * @param {string} str 字符串
 */
jCute.toAscii = function (str) {
    var val = "", i = 0, len = str.length;
    for (; i < len; i++) {
        val += "&#" + str[i].charCodeAt() + ";";
    }
    return val
};

/**
 * Ascii解码
 * @param {string} str 字符串
 */
jCute.toAsciiUn = function (str) {
    var val = "", strs = str.match(/&#(\d+);/g);
    if (strs != null) {
        for (var i = 0, len = strs.length; i < len; i++) {
            val += String.fromCharCode(strs[i].replace(/[&#;]/g, ''));
        }
    }
    return val
};

﻿
/**
 * Cookie获取、设置、删除
 * @param {string} key 键
 * @param {string} value 值
 * @param {number} time 过期时间（默认不指定过期时间），单位：毫秒，小于0删除
 */
jCute.cookie = function (key, value, time) {
    if (arguments.length == 1) {
        var arr, reg = new RegExp("(^| )" + key + "=([^;]*)(;|$)");
        if (arr = document.cookie.match(reg)) {
            return arr[2];
        }
        return "";
    } else {
        var kv = key + "=" + value + ";path=/;";
        if (time != undefined) {
            var d = new Date();
            d.setTime(d.getTime() + time);
            kv += "expires=" + d.toGMTString()
        }
        document.cookie = kv;
    }
};
/**
 * 载入js脚本，并回调
 * @param {string} src js脚本路径
 * @param {function} success 载入成功回调方法
 */
jCute.getScript = function (src, success) {
    var ele = document.createElement("SCRIPT");
    ele.src = src;
    ele.type = "text/javascript";
    document.getElementsByTagName("HEAD")[0].appendChild(ele);
    //加载完成回调
    if (success != undefined) {
        ele.onload = ele.onreadystatechange = function () {
            if (!this.readyState || this.readyState == "loaded" || this.readyState == "complete") { success(); }
        }        
    }
};

/**
 * 载入css样式
 * @param {string} href css样式路径
 */
jCute.getStyle = function (href) {
    var ele = document.createElement("LINK");
    ele.href = href;
    ele.rel = "stylesheet";
    document.getElementsByTagName("HEAD")[0].appendChild(ele);
};

/** 
 * IE8- 
 */
jCute.oldIE = function () {
    return typeof document.createElement == "object" || false
};

/**
 * 移除空字符
 * @param {string} str 字符串
 */
jCute.trim = function (str) {
    return this.replace(/(^\s+)|(\s+$)/g, "")
};

/**
 * event
 * @param {event} e 事件流
 */
jCute.event = function (e) {
    return e || window.event
};

/**
 * target
 * @param {event} e 事件流
 */
jCute.target = function (e) {
    e = e || window.event;
    return e.target || e.srcElement;
};

/**
 * 阻止事件冒泡
 * @param {event} e 事件流
 */
jCute.stopEvent = function (e) {
    if (e && e.stopPropagation) {
        e.stopPropagation()
    } else {
        window.event.cancelBubble = true
    }
};

/**
 * 阻止浏览器默认行为
 * @param {event} e 事件流
 */
jCute.stopDefault = function (e) {
    if (e && e.preventDefault) {
        e.preventDefault()
    } else {
        window.event.returnValue = false
    }
};

/**
 * 按键ASCII值
 * @param {event} e 事件流
 */
jCute.key = function (e) {
    e = e || window.event;
    return e.keyCode || e.which || e.charCode;
};

/**
 * 检测类型
 * @param {any} obj 对象
 */
jCute.type = function (obj) {
    var tv = {}.toString.call(obj);
    return tv.split(' ')[1].replace(']', '').toLowerCase();
}
/**
 * 解析字符串为xml
 * @param {string} data 字符串
 */
jCute.parseXML = function (data) {
    var xmldom = null;
    if (typeof DOMParser != "undefined") {
        xmldom = new DOMParser();
        xmldom = xmldom.parseFromString(data, "text/xml");
        var errors = xmldom.getElementsByTagName("parsererror");
        if (errors.length) { throw new Error(xmldom.parseError.reason); }
    } else if (typeof ActiveXObject != "undefined") {
        var versions = ["MSXML2.DOMDocument.6.0", "MSXML2.DOMDocument.3.0", "MSXML2.DOMDocument"];
        for (var i = 0; i < versions.length; i++) {
            try {
                new ActiveXObject(versions[i]);
                arguments.callee.activeXString = versions[i]; break;
            } catch (e) { }
        }
        xmldom = new ActiveXObject(arguments.callee.activeXString);
        xmldom.loadXML(data);
        if (xmldom.parseError != 0) { throw new Error(xmldom.parseError.reason); }
    } else { throw new Error("XML parse error"); }
    return xmldom;
}

/**
 * XML转字符串
 * @param {object} xmlDoc XML对象
 */
jCute.XMLSerializer = function (xmlDoc) {
    if (typeof XMLSerializer != "undefined") {
        return (new XMLSerializer()).serializeToString(xmlDoc);
    } else if (typeof xmlDoc.xml != "undefined") {
        return xmlDoc.xml;
    } else {
        throw new Error("XML serialize error");
    }
}

