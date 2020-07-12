var t1 = $('#txtCode1'), t2 = $('#txtCode2'), k1 = $('#txtKey1'), k2 = $('#txtKey2'), box = $('#codetoolbox');

//按钮点击事件
box.click(function (e) {
    e = e || window.event;
    var target = e.target || e.srcElement;
    if (target.nodeName == "BUTTON" || target.nodeName == "I") {
        var html = target.innerHTML;
        if (html == "" || html.indexOf("fa-rotate-left") >= 0) {
            html = "<I>"; target = target.nodeName == "I" ? $(target).parent() : $(target);
        } else {
            target = $(target);
        }

        switch (html) {
            //反向
            case "<I>":
                var prevhtml = target.prev().html();
                switch (prevhtml) {
                    case "AES":
                    case "DES":
                    case "RC4":
                    case "Rabbit":
                    case "TripleDES":
                        try {
                            t1.val(CryptoJS[prevhtml].decrypt(t2.val(), k1.val()).toString(CryptoJS.enc.Utf8));
                        } catch (e) {
                            t1.val('');
                        }
                        break;
                    case "encodeURI":
                        t1.val(decodeURI(t2.val()));
                        break;
                    case "encodeURIComponent":
                        t1.val(decodeURIComponent(t2.val()));
                        break;
                    case "Base64":
                        try {
                            t1.val(CryptoJS.enc.Base64.parse(t2.val()).toString(CryptoJS.enc.Utf8));
                        } catch (e) {
                            t1.val('')
                        }
                        break;
                    case "Unicode":
                        try { t1.val(UnConvertUnicode(t2.val())); } catch (e) { t1.val('') }
                        break;
                    case "ASCII":
                        try { t1.val(UnConvertAscii(t2.val())); } catch (e) { t1.val('') }
                        break;
                }
                break;

            case "AES":
            case "DES":
            case "RC4":
            case "Rabbit":
            case "TripleDES":
                try { t2.val(CryptoJS[html].encrypt(t1.val(), k1.val())); } catch (e) { t2.val(''); }
                break;

            case "MD5":
                var s1 = t1.val(), s2 = CryptoJS.MD5(t1.val()).toString(), s3 = s2.substr(8, 16);
                t2.val("源字符串：" + s1 + "\n16位小写：" + s3 + "\n16位大写：" + s3.toUpperCase() + "\n32位小写：" + s2 + "\n32位大写：" + s2.toUpperCase());
                break;
            case "SHA1":
            case "HmacSHA1":
            case "224":
            case "256":
            case "384":
            case "512":
            case "HmacMD5":
                var cmdN = html;
                if (target.parent().children().first().html() == "SHA1") {
                    cmdN.length == 3 && (cmdN = "SHA" + cmdN);
                    try { t2.val(CryptoJS[cmdN](t1.val())); } catch (e) { t2.val(''); }
                } else {
                    cmdN.length == 3 && (cmdN = "HmacSHA" + cmdN);
                    try { t2.val(CryptoJS[cmdN](t1.val(), k2.val())); } catch (e) { t2.val(''); }
                }
                break;

            case "encodeURI":
                t2.val(encodeURI(t1.val()));
                break;
            case "encodeURIComponent":
                t2.val(encodeURIComponent(t1.val()));
                break;
            case "Base64":
                try { t2.val(CryptoJS.enc.Base64.stringify(CryptoJS.enc.Utf8.parse(t1.val()))); } catch (e) { t2.val('') }
                break;
            case "Unicode":
                t2.val(ConvertUnicode(t1.val()));
                break;
            case "ASCII":
                t2.val(ConvertAscii(t1.val()));
                break;

            case "UUID":
                $.ajax({
                    url: "https://api.zme.ink/uuid/9",
                    dataType: 'json',
                    success: function (data) {
                        t2.val(data.join('\r\n'));
                    },
                    error: function () {
                        t2.val('生成UUID失败');
                    }
                });
                break;
        }
    }
});


$("#btnHtmlToJs").click(function () {
    $("#txtCode2").val("var tmp='" + $("#txtCode1").val().replace(/\'/g, "\\'").replace(/\n/g, '').replace(/\s/g, '') + "'");
});


//转码 To Unicode
function ConvertUnicode(s) {
    var val = "", i = 0, c, len = s.length;
    for (; i < len; i++) {
        c = s.charCodeAt(i).toString(16);
        while (c.length < 4) { c = '0' + c; } val += '\\u' + c
    } return val
};

//转码 Unicode To STR-CN
function UnConvertUnicode(s) { return eval("'" + s + "'"); };/*return unescape(str.replace(/\u/g, "%u"))*/

//转码 To ASCII
function ConvertAscii(s) {
    var val = "", i = 0, len = s.length;
    for (; i < len; i++) { val += "&#" + s[i].charCodeAt() + ";"; }
    return val
};

//转码 ASCII To STR-CN
function UnConvertAscii(s) {
    var val = "", strs = s.match(/&#(\d+);/g);
    if (strs != null) {
        for (var i = 0, len = strs.length; i < len; i++) {
            val += String.fromCharCode(strs[i].replace(/[&#;]/g, ''));
        }
    } return val
};