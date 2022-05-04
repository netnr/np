
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
