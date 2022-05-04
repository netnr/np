nr.onReady = function () {
    
    document.body.addEventListener('click', function (e) {
        var target = e.target;
        if (target.nodeName == "SL-BUTTON") {

            var code1 = nr.domTxtCode1.value;
            var code2 = nr.domTxtCode2.value;
            var action = target.innerText;

            try {
                switch (action) {
                    //反向
                    case "↺":
                        {
                            action = target.previousElementSibling.innerText;
                            switch (action) {
                                case "AES":
                                case "DES":
                                case "RC4":
                                case "Rabbit":
                                case "TripleDES":
                                    {
                                        var key2 = nr.domTxtKey2.value;
                                        var result = CryptoJS[action].decrypt(code2, key2).toString(CryptoJS.enc.Utf8);
                                        nr.domTxtCode1.value = result;
                                    }
                                    break;

                                case "encodeURI":
                                    nr.domTxtCode1.value = decodeURI(code2);
                                    break;
                                case "encodeURIComponent":
                                    nr.domTxtCode1.value = decodeURIComponent(code2);
                                    break;
                                case "Base64":
                                    {
                                        var result = CryptoJS.enc.Base64.parse(code2).toString(CryptoJS.enc.Utf8)
                                        nr.domTxtCode1.value = result;
                                    }
                                    break;
                                case "Unicode":
                                    nr.domTxtCode1.value = UnicodeToContent(code2);
                                    break;
                                case "ASCII":
                                    nr.domTxtCode1.value = ASCIIToContent(code2);
                                    break;
                                case "HTMLEncode":
                                    nr.domTxtCode1.value = nr.htmlDecode(code2);
                                    break;
                            }
                        }
                        break;

                    case "MD5":
                        var result = CryptoJS.MD5(code1).toString();
                        nr.domTxtCode2.value = `源字符串：${code1}\n32位小写：${result}\n32位大写：${result.toUpperCase()}`;
                        break;

                    case "SHA1":
                    case "SHA3":
                    case "HmacSHA1":
                    case "224":
                    case "256":
                    case "384":
                    case "512":
                    case "HmacMD5":
                        {
                            var cmdName = action;
                            if (target.parentElement.children[0].innerText == "SHA1") {
                                if (cmdName.length == 3) {
                                    cmdName = `SHA${cmdName}`;
                                }
                                var result = CryptoJS[cmdName](code1);
                                nr.domTxtCode2.value = result
                            } else {
                                if (cmdName.length == 3) {
                                    cmdName = `HmacSHA${cmdName}`;
                                }
                                var key1 = nr.domTxtKey1.value;
                                var result = CryptoJS[cmdName](code1, key1);
                                nr.domTxtCode2.value = result
                            }
                        }
                        break;

                    case "AES":
                    case "DES":
                    case "RC4":
                    case "Rabbit":
                    case "TripleDES":
                        {
                            var key2 = nr.domTxtKey2.value;
                            var result = CryptoJS[action].encrypt(code1, key2);
                            nr.domTxtCode2.value = result;
                        }
                        break;

                    case "encodeURI":
                        nr.domTxtCode2.value = encodeURI(code1);
                        break;
                    case "encodeURIComponent":
                        nr.domTxtCode2.value = encodeURIComponent(code1);
                        break;
                    case "Base64":
                        {
                            var result = CryptoJS.enc.Base64.stringify(CryptoJS.enc.Utf8.parse(code1));
                            nr.domTxtCode2.value = result;
                        }
                        break;
                    case "Unicode":
                        nr.domTxtCode2.value = ContentToUnicode(code1);
                        break;
                    case "ASCII":
                        nr.domTxtCode2.value = ContentToASCII(code1);
                        break;
                    case "HTMLEncode":
                        nr.domTxtCode2.value = nr.htmlEncode(code1);
                        break;

                    case "UUID":
                        {
                            var arr = [];
                            for (var i = 0; i < 9; i++) {
                                arr.push(UUID());
                            }
                            nr.domTxtCode2.value = arr.join("\r\n");
                        }
                        break;
                }
            } catch (ex) {
                console.debug(ex);
                var atip = target.innerText;
                if (action != atip) {
                    atip += " " + action;
                }
                nr.alert(`${atip} 出错`)
            }
        }
    });

    //接收文件
    nr.receiveFiles(function (files) {
        var file = files[0];
        if (file) {
            var finfo = `文件名：${file.name}，大小：${nr.formatByteSize(file.size)}`, startDate = new Date();
            nr.domTxtCode2.value = `${finfo}\n读取中 ...`;

            var algoKeys = ['MD5', 'SHA1', 'SHA256'], algsObj = {};
            algoKeys.forEach(key => algsObj[key] = CryptoJS.algo[key].create());

            readLargeFile({
                file: file,
                work: obj => {
                    var parseResult = CryptoJS.enc.Latin1.parse(obj.chunkResult);
                    algoKeys.forEach(key => algsObj[key].update(parseResult));
                    nr.domTxtCode2.value = `${finfo}\n进度：${((obj.chunkIndex / obj.chunkTotal) * 100).toFixed(2)}%\n耗时：${((new Date() - startDate) / 1000).toFixed(2)}s`;
                },
                done: obj => {
                    var result = [`文件名：${file.name}，大小：${nr.formatByteSize(file.size)}，耗时：${((new Date() - startDate) / 1000).toFixed(2)}s\n`];
                    algoKeys.forEach(key => {
                        result.push(`${key}：${algsObj[key].finalize()}`);
                    });
                    nr.domTxtCode2.value = result.join("\n");
                },
            })
        }

        nr.domTxtFile.value = "";
    }, nr.domTxtFile);

    // 按键 ASCII
    nr.domTxtAscii.addEventListener('keydown', function (e) {
        console.debug(e);
        nr.domTxtAscii.value = `${e.keyCode}  ${e.code}${e.ctrlKey ? "  Ctrl  " : ""}${e.shiftKey ? "  Shift  " : ""}${e.altKey ? "  Alt  " : ""}${e.metaKey ? "  Meta  " : ""}`;

        e.stopPropagation();
        e.preventDefault();
        return false;
    });
}

/**
 * 读取大文件
 * @param {*} obj
 */
function readLargeFile(obj) {
    obj = Object.assign({
        file: null,
        work: function (obj) { },
        done: function (obj) { },
        chunkSize: 1024 * 512,
        chunkIndex: 0,
        chunk: null,
        chunkResult: null,
        result: null,
    }, obj);
    obj.chunkTotal = Math.ceil(obj.file.size / obj.chunkSize);

    var reader = new FileReader();

    function readChunk(offset) {
        obj.chunkIndex++;
        offset = offset || 0;
        var end = Math.min(offset + obj.chunkSize, obj.file.size);
        obj.chunk = obj.file.slice(offset, end);
        reader.onload = function (e) {
            obj.chunkResult = e.target.result;
            if (obj.result == null) {
                obj.result = obj.chunkResult;
            } else {
                obj.result += obj.chunkResult;
            }

            obj.work(obj);
            //当前进度
            if (end < obj.file.size) {
                readChunk(end);
            } else {
                obj.done(obj); //完成
            }
        }
        reader.readAsBinaryString(obj.chunk);
    }
    readChunk();
}

//Content To Unicode
function ContentToUnicode(s) {
    var val = "", i = 0, c, len = s.length;
    for (; i < len; i++) {
        c = s.charCodeAt(i).toString(16);
        while (c.length < 4) { c = '0' + c; } val += '\\u' + c
    } return val
};

//Unicode To Content
function UnicodeToContent(s) {
    return unescape(s.replace(/\\u/g, "%u"))
};

//Content To ASCII
function ContentToASCII(s) {
    var val = "", i = 0, len = s.length;
    for (; i < len; i++) { val += "&#" + s[i].charCodeAt() + ";"; }
    return val
};

//ASCII To Content
function ASCIIToContent(s) {
    var val = "", strs = s.match(/&#(\d+);/g);
    if (strs != null) {
        for (var i = 0, len = strs.length; i < len; i++) {
            val += String.fromCharCode(strs[i].replace(/[&#;]/g, ''));
        }
    } return val
};

function UUID() {
    if (window["crypto"]) {
        return crypto.randomUUID();
    } else {
        return URL.createObjectURL(new Blob([])).split('/').pop();
    }
}