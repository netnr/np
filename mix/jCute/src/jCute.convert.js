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

