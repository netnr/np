import { ndkVary } from './ndkVary';
import { ndkI18n } from "./ndkI18n";

// 方法
var ndkFunction = {

    /**
     * 大驼峰
     * @param {*} c 
     * @returns 
     */
    hump: c => c.substring(0, 1).toUpperCase() + c.substring(1),

    /**
     * html 编码 SO#18749591
     * @param {*} html 
     * @returns 
     */
    htmlEncode: html => html.replace(/[\u00A0-\u9999<>\&]/g, (i) => '&#' + i.charCodeAt(0) + ';'),

    /**
     * html 标签剔除，只保留文本
     * @param {*} html 
     * @returns 
     */
    htmlCull: html => {
        var div = document.createElement("div");
        div.innerHTML = html;
        return div.innerText;
    },

    /**
     * 类名处理
     * @param {*} name 名称
     * @param {*} smallHump 小驼峰，默认大驼峰 
     */
    handleClassName: (name, smallHump) => {
        var items = name.split("_");
        items = items.map(item => item[0].toUpperCase() + item.slice(1));
        var varName = items.join('');
        if (smallHump) {
            varName = varName[0].toLowerCase() + varName.slice(1);
        }
        return varName;
    },

    /**
     * 获取或设置CSS变量
     * @param {any} dom
     * @param {any} k
     * @param {any} v
     */
    cssvar: (dom, k, v) => {
        if (v == null) {
            return getComputedStyle(dom).getPropertyValue(k);
        } else {
            dom.style.setProperty(k, v);
        }
    },

    /**
     * 类型判断
     * @param {*} obj 
     * @returns 
     */
    type: function (obj) {
        var tv = {}.toString.call(obj);
        return tv.split(' ')[1].replace(']', '');
    },

    /**
     * 运行代码
     * @param {*} code 
     * @returns 
     */
    runCode: async (code) => {
        //执行代码
        var ecode = eval('(' + code + ')');
        var etype = ndkFunction.type(ecode);
        switch (etype) {
            case "AsyncFunction":
            case "Promise":
                ecode = await ecode();
                break;
            case "Function":
                ecode = ecode();
                break;
        }
        return ecode;
    },

    /**
     * 数组去重
     * @param {any} arr
     */
    arrayDistinct: arr => Array.from(new Set(arr)),

    /**
     * 分组
     * @param {*} arr 
     * @param {*} f 
     * @returns 
     */
    groupBy: (arr, f) => Array.from(new Set(arr.map(f))),

    /**
     * 字符串长度
     * @param {*} content 
     * @returns 
     */
    byteLength: content => {
        var length = 0;
        Array.from(content).map(function (char) {
            length += char.charCodeAt(0) > 255 ? 2 : 1;
        });
        return length;
    },

    /**
     * 获取目录
     */
    getFullPath: (path) => {
        var delimiter = path.includes('/') ? '/' : '\\';
        var result = path.substring(0, path.lastIndexOf(delimiter) + 1);
        return result;
    },

    /**
     * 可视化大小
     * @param {*} size 
     * @param {*} keep 
     * @param {*} rate 
     * @returns 
     */
    formatByteSize: function (size, keep = 2, rate = 1024) {
        if (Math.abs(size) < rate) {
            return size + ' B';
        }

        const units = rate == 1000 ? ['KB', 'MB', 'GB', 'TB', 'PB', 'EB', 'ZB', 'YB'] : ['KiB', 'MiB', 'GiB', 'TiB', 'PiB', 'EiB', 'ZiB', 'YiB'];
        let u = -1;
        const r = 10 ** keep;

        do {
            size /= rate;
            ++u;
        } while (Math.round(Math.abs(size) * r) / r >= rate && u < units.length - 1);

        return (size.toFixed(keep) * 1).toString() + ' ' + units[u];
    },

    /**
     * 时间格式化
     * @param {any} fdt
     * @param {any} date
     */
    formatDateTime: (fdt, date) => {
        date = date || Date.now();

        var rops = Intl.DateTimeFormat().resolvedOptions();
        var options = {
            hour12: false, timeZone: rops.timeZone
        };
        switch (fdt) {
            case "date":
                options = Object.assign({
                    year: 'numeric', month: '2-digit', day: '2-digit'
                }, options)
                break;
            case "time":
                options = Object.assign({
                    hour: 'numeric', minute: '2-digit', second: '2-digit'
                }, options)
                break;
            case "year":
                return new Date(date).getFullYear();
            case "datetime":
            default:
                options = Object.assign({
                    year: 'numeric', month: '2-digit', day: '2-digit',
                    hour: 'numeric', minute: '2-digit', second: '2-digit'
                }, options)
                break;
        }
        return new Intl.DateTimeFormat(rops.locale, options).format(date);
    },
    // 当前时间
    now: () => ndkFunction.formatDateTime("datetime"),

    // null 值处理
    formatNull: () => `<em style="color:var(--sl-color-neutral-400)">(Null)</em>`,

    /* 浏览器通知 */
    notify: async (title, options) => {
        if (window.Notification) {
            let permission = Notification.permission;
            if (permission != 'granted' && permission != 'denied') {
                permission = await Notification.requestPermission();
            }

            if (permission == 'granted') {
                var notification = new Notification(title, options);
                
                notification.onclick = function () {
                    window.focus();
                    notification.close();
                };
            }
        }
    },

    /**
     * 消息
     * @param {*} message 
     * @param {*} type 
     * @param {*} icon 
     * @param {*} duration 
     * @returns 
     */
    toast: function (message, type = 'primary', icon = 'info-circle', duration = 8000) {
        const alert = Object.assign(document.createElement('sl-alert'), {
            type: type,
            closable: true,
            duration: duration,
            innerHTML: `
          <sl-icon name="${icon}" slot="icon"></sl-icon>
          ${message}
        `
        });

        document.body.append(alert);
        return alert.toast();
    },

    /**
     * 提示
     * @param {*} content 内容
     * @param {*} title 标题
     * @param {*} width
     */
    alert: (content, title, width = "60vw") => {
        if (ndkVary.domAlert == null) {
            ndkVary.domAlert = document.createElement('sl-dialog');
            document.body.appendChild(ndkVary.domAlert);

            //关闭
            ndkVary.domAlert.addEventListener('click', function (e) {
                if (e.target.getAttribute('variant') == "primary") {
                    ndkVary.domAlert.hide();
                }
            }, false)
        }
        ndkVary.domAlert.label = title || "";

        if (width == "full") {
            width = "98vw";
        }
        ndkVary.domAlert.setAttribute('style', `--width:${width}`);
        ndkVary.domAlert.innerHTML = `${content}<sl-button slot="footer" variant="primary">${ndkI18n.lg.close}</sl-button>`;
        ndkVary.domAlert.show();
    },

    /**
     * 输出
     * @param {*} content 内容
     */
    output: (content) => {
        ndkVary.domOutputBody.style.display = "block";

        clearTimeout(ndkVary.defer.outputAutoHide);
        ndkVary.defer.outputAutoHide = setTimeout(() => {
            ndkVary.domOutputBody.style.display = "none";
        }, 1000 * 6);

        try {
            if (typeof content != "string") {
                content = JSON.stringify(content);
            }
        } catch (error) { }

        var rowData = [{ content: content }];

        ndkVary.gridOpsOutput.rowData.unshift(rowData[0]);
        ndkVary.gridOpsOutput.api.applyTransaction({
            addIndex: 0,
            add: rowData
        });

        //闪烁行
        var rowNode = ndkVary.gridOpsOutput.api.getDisplayedRowAtIndex(0);
        ndkVary.gridOpsOutput.api.flashCells({ rowNodes: [rowNode] });
    },

    /**
     * markdown 编码
     * @param {*} content 
     * @returns 
     */
    markdownEncode: content => {
        if (content != null) {
            content = content.toString().replace(/\n/g, "<br/>").replace(/\r/g, "").replace(/\[/g, '\\[').replace(/\(/g, '\\(')
        }
        return content;
    },

    /**
     * 数据行转换为 markdown
     * @param {*} rowDatas 
     * @param {*} headers 列名 [{field: "", headerName: ""}]
     * @param {*} options 选项 
     * @returns 
     */
    rowsToMarkdown: (rowDatas, headers, options) => {
        if (headers == null) {
            Object.keys(rowDatas[0]).forEach(key => {
                headers.push({
                    headerName: headers[key] || key,
                    field: key
                })
            })
        }

        if (options == null) {
            options = {};
        }

        var rows = [];
        rowDatas.forEach(rowData => {
            var row = [];
            headers.forEach(header => {
                var val = rowData[header.field];
                if (header.cellRenderer != null) {
                    val = header.cellRenderer({ value: val, data: rowData });
                }
                val = ndkFunction.markdownEncode(val);
                if (options.htmlCull) {
                    val = ndkFunction.htmlCull(val);
                }
                row.push(val);
            });
            rows.push(`| ${row.join(" | ")} |`);
        });

        return `| ${headers.map(header => header.headerName).join(` | `)} |\n| ${headers.map(_header => `---`).join(` | `)} |\n${rows.join(`\n`)}`;
    },

    /**
     * 新的连接ID
     * @param {*} s
     * @param {*} e 
     * @returns 
     */
    random: (s = 20000, e = 99999) => Math.floor(Math.random() * (e - s + 1) + s),

    /**
     * 生成 UUID
     * @returns 
     */
    UUID: () => URL.createObjectURL(new Blob([])).split('/').pop(),

    /**
     * AES 256 GCM 加密
     * @param {*} plaintext 纯文本
     * @param {*} password 密钥
     * @returns 
     */
    AESEncrypt: async (plaintext, password) => {
        let pwUtf8 = new TextEncoder().encode(password);
        let pwHash = await window.crypto.subtle.digest('SHA-256', pwUtf8);

        let iv = window.crypto.getRandomValues(new Uint8Array(12));
        let ivStr = Array.from(iv).map(b => String.fromCharCode(b)).join('');

        let alg = { name: 'AES-GCM', iv: iv };
        let key = await window.crypto.subtle.importKey('raw', pwHash, alg, false, ['encrypt']);
        let ptUint8 = new TextEncoder().encode(plaintext);

        let ctBuffer = await window.crypto.subtle.encrypt(alg, key, ptUint8);
        let ctArray = Array.from(new Uint8Array(ctBuffer));
        let ctStr = ctArray.map(byte => String.fromCharCode(byte)).join('');
        let result = window.btoa(ivStr + ctStr);
        return result;
    },

    /**
     * AES 256 GCM 解密
     * @param {*} ciphertext 密文
     * @param {*} password 密钥
     * @returns 
     */
    AESDecrypt: async (ciphertext, password) => {
        let pwUtf8 = new TextEncoder().encode(password);
        let pwHash = await window.crypto.subtle.digest('SHA-256', pwUtf8);

        let ivStr = window.atob(ciphertext).slice(0, 12);
        let iv = new Uint8Array(Array.from(ivStr).map(ch => ch.charCodeAt(0)));

        let alg = { name: 'AES-GCM', iv: iv };
        let key = await window.crypto.subtle.importKey('raw', pwHash, alg, false, ['decrypt']);
        let ctStr = window.atob(ciphertext).slice(12);
        let ctUint8 = new Uint8Array(Array.from(ctStr).map(ch => ch.charCodeAt(0)));

        let plainBuffer = await window.crypto.subtle.decrypt(alg, key, ctUint8);
        let result = new TextDecoder().decode(plainBuffer);
        return result;
    },

    /**
     * 下载
     * @param {any} content
     * @param {any} filename
     */
    download: function (content, filename) {
        var aTag = document.createElement('a');
        var blob = new Blob([content]);
        aTag.download = filename;
        aTag.href = URL.createObjectURL(blob);
        document.body.appendChild(aTag);
        aTag.click();
        URL.revokeObjectURL(blob);
        aTag.remove();
    },

    /**
     * 读取文件内容（可指定编码）
     * @param {any} file
     * @param {any} encoding GBK 或 utf-8
     */
    readFileContent: (file, encoding = "utf-8") => new Promise((resolve, reject) => {
        const reader = new FileReader();
        reader.onloadend = () => resolve(reader.result);
        reader.onerror = () => reject();
        reader.readAsText(file, encoding);
    }),

    /**
     * base64 MIME检测
     * @param {*} code 
     * @returns 
     */
    base64Detect: (code) => {
        try {
            var parts = code.split(';base64,');
            var base64 = parts.pop();

            var bin = window.atob(base64);
            var len = bin.length;
            var arr = new Uint8Array(len);
            for (var i = 0; i < len; i++) {
                arr[i] = bin.charCodeAt(i);
            }
            var ftinfo = magicBytes.filetypeinfo(arr);
            return ftinfo;
        } catch (e) {
            return [];
        }
    },

    /**
     * 读写剪贴板
     * @param {*} content 写入内容
     */
    clipboard: async (content) => {
        var text;
        if (navigator.clipboard) {
            if (content == null) {
                text = await navigator.clipboard.readText();
            } else {
                text = await navigator.clipboard.writeText(content);
            }
            return text;
        } else if (content != null) {
            //兼容模式复制
            var textarea = document.createElement("textarea");
            textarea.value = content;
            textarea.style.position = "fixed";
            textarea.style.opacity = 0;
            document.body.appendChild(textarea);
            textarea.select();
            window.document.execCommand("Copy");
            textarea.remove();

            return content;
        } else {
            return ndkI18n.lg.unsupported;
        }
    },

}


export { ndkFunction };