import { nrcFile } from "../../../../frame/nrcFile";
import { nrcBase } from "../../../../frame/nrcBase";
import { nrVary } from "../../nrVary";
import { nrApp } from "../../../../frame/Bootstrap/nrApp";
import { nrcRely } from "../../../../frame/nrcRely";

let nrPage = {
    pathname: "/ss/code",

    init: async () => {
        await nrcRely.remote('crypto.js')

        nrPage.bindEvent();
    },

    bindEvent: () => {
        //click
        document.body.addEventListener('click', function (e) {
            let target = e.target;
            if (target.nodeName == "BUTTON") {

                let code1 = nrVary.domTxtCode1.value;
                let code2 = nrVary.domTxtCode2.value;
                let action = target.innerText;

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
                                            let key2 = nrVary.domTxtKey2.value;
                                            let result = CryptoJS[action].decrypt(code2, key2).toString(CryptoJS.enc.Utf8);
                                            nrVary.domTxtCode1.value = result;
                                        }
                                        break;

                                    case "encodeURI":
                                        nrVary.domTxtCode1.value = decodeURI(code2);
                                        break;
                                    case "encodeURIComponent":
                                        nrVary.domTxtCode1.value = decodeURIComponent(code2);
                                        break;
                                    case "Base64":
                                        {
                                            let result = CryptoJS.enc.Base64.parse(code2).toString(CryptoJS.enc.Utf8)
                                            nrVary.domTxtCode1.value = result;
                                        }
                                        break;
                                    case "Unicode":
                                        nrVary.domTxtCode1.value = nrPage.deUnicode(code2);
                                        break;
                                    case "ASCII":
                                        nrVary.domTxtCode1.value = nrPage.deASCII(code2);
                                        break;
                                    case "HTMLEncode":
                                        nrVary.domTxtCode1.value = nrcBase.htmlDecode(code2);
                                        break;
                                }
                            }
                            break;

                        case "MD5":
                            let result = CryptoJS.MD5(code1).toString();
                            nrVary.domTxtCode2.value = `源字符串：${code1}\n32位小写：${result}\n32位大写：${result.toUpperCase()}`;
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
                                let cmdName = action;
                                if (target.parentElement.children[0].innerText == "SHA1") {
                                    if (cmdName.length == 3) {
                                        cmdName = `SHA${cmdName}`;
                                    }
                                    let result = CryptoJS[cmdName](code1);
                                    nrVary.domTxtCode2.value = result
                                } else {
                                    if (cmdName.length == 3) {
                                        cmdName = `HmacSHA${cmdName}`;
                                    }
                                    let key1 = nrVary.domTxtKey1.value;
                                    let result = CryptoJS[cmdName](code1, key1);
                                    nrVary.domTxtCode2.value = result
                                }
                            }
                            break;

                        case "AES":
                        case "DES":
                        case "RC4":
                        case "Rabbit":
                        case "TripleDES":
                            {
                                let key2 = nrVary.domTxtKey2.value;
                                let result = CryptoJS[action].encrypt(code1, key2);
                                nrVary.domTxtCode2.value = result;
                            }
                            break;

                        case "encodeURI":
                            nrVary.domTxtCode2.value = encodeURI(code1);
                            break;
                        case "encodeURIComponent":
                            nrVary.domTxtCode2.value = encodeURIComponent(code1);
                            break;
                        case "Base64":
                            {
                                let result = CryptoJS.enc.Base64.stringify(CryptoJS.enc.Utf8.parse(code1));
                                nrVary.domTxtCode2.value = result;
                            }
                            break;
                        case "Unicode":
                            nrVary.domTxtCode2.value = nrPage.toUnicode(code1);
                            break;
                        case "ASCII":
                            nrVary.domTxtCode2.value = nrPage.toASCII(code1);
                            break;
                        case "HTMLEncode":
                            nrVary.domTxtCode2.value = nrcBase.htmlEncode(code1);
                            break;

                        case "UUID":
                            {
                                let arr = [];
                                for (let i = 0; i < 9; i++) {
                                    arr.push(nrcBase.UUID());
                                }
                                nrVary.domTxtCode2.value = arr.join("\r\n");
                            }
                            break;
                    }
                } catch (ex) {
                    nrApp.logError(ex, '处理错误');
                }
            }
        });

        //接收文件
        nrcFile.init(async (files) => {
            let file = files[0];
            if (file) {
                let finfo = `文件名：${file.name}，大小：${nrcBase.formatByteSize(file.size)}`;
                let startDate = new Date();

                nrVary.domTxtCode2.value = `${finfo}\n读取中 ...`;

                let algoKeys = ['MD5', 'SHA1', 'SHA256'], algsObj = {};
                algoKeys.forEach(key => algsObj[key] = CryptoJS.algo[key].create());

                nrPage.readLargeFile({
                    file: file,
                    work: obj => {
                        let parseResult = CryptoJS.enc.Latin1.parse(obj.chunkResult);
                        algoKeys.forEach(key => algsObj[key].update(parseResult));
                        nrVary.domTxtCode2.value = `${finfo}\n进度：${((obj.chunkIndex / obj.chunkTotal) * 100).toFixed(2)}%\n耗时：${((new Date() - startDate) / 1000).toFixed(2)}s`;
                    },
                    done: obj => {
                        let result = [`文件名：${file.name}，大小：${nrcBase.formatByteSize(file.size)}，耗时：${((new Date() - startDate) / 1000).toFixed(2)}s\n`];
                        algoKeys.forEach(key => {
                            result.push(`${key}：${algsObj[key].finalize()}`);
                        });
                        nrVary.domTxtCode2.value = result.join("\n");
                    },
                })
            }

            nrVary.domTxtFile.value = "";
        }, nrVary.domTxtFile);

        // 按键 ASCII
        nrVary.domTxtAscii.addEventListener('keydown', function (e) {
            console.debug(e);
            nrVary.domTxtAscii.value = `${e.keyCode}  ${e.code}${e.ctrlKey ? "  Ctrl  " : ""}${e.shiftKey ? "  Shift  " : ""}${e.altKey ? "  Alt  " : ""}${e.metaKey ? "  Meta  " : ""}`;

            e.stopPropagation();
            e.preventDefault();
            return false;
        });

        //color
        nrVary.domTxtColor.addEventListener('input', function () {
            nrVary.domTxtColorValue.value = this.value;
        })
    },

    /**
     * 读取大文件
     * @param {*} obj
     */
    readLargeFile: function (obj) {
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

        let reader = new FileReader();

        function readChunk(offset) {
            obj.chunkIndex++;
            offset = offset || 0;
            let end = Math.min(offset + obj.chunkSize, obj.file.size);
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
    },

    //Content To Unicode
    toUnicode: function (s) {
        let val = "", i = 0, c, len = s.length;
        for (; i < len; i++) {
            c = s.charCodeAt(i).toString(16);
            while (c.length < 4) { c = '0' + c; } val += '\\u' + c
        } return val
    },

    //Unicode To Content
    deUnicode: function (s) {
        return unescape(s.replace(/\\u/g, "%u"))
    },

    //Content To ASCII
    toASCII: function (s) {
        let val = "", i = 0, len = s.length;
        for (; i < len; i++) { val += "&#" + s[i].charCodeAt() + ";"; }
        return val
    },

    //ASCII To Content
    deASCII: function (s) {
        let val = "", strs = s.match(/&#(\d+);/g);
        if (strs != null) {
            for (let i = 0, len = strs.length; i < len; i++) {
                val += String.fromCharCode(strs[i].replace(/[&#;]/g, ''));
            }
        } return val
    }

}

export { nrPage };