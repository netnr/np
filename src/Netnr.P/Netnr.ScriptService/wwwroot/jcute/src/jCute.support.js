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
