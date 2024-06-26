let nrcBase = {
    /**版本号 */
    version: require("../package.json").version,

    lastFetchDate: null, //上次请求时间（需配置 Access-Control-Expose-Headers 含 date）

    /**
     * 读写 cookie
     * @param {*} key 
     * @param {*} value 
     * @param {*} ms -1 删除
     * @returns 
     */
    cookie: function (key, value, ms) {
        if (arguments.length == 1) {
            let arr = document.cookie.match(new RegExp("(^| )" + key + "=([^;]*)(;|$)"));
            if (arr != null) {
                return arr[2];
            }
            return null;
        } else {
            let kv = `${key}=${value};Path=/`;
            if (ms) {
                let d = new Date();
                d.setTime(d.getTime() + ms);
                kv = `${kv};Expires=${d.toGMTString()}`;
            }
            document.cookie = kv;
        }
    },

    /** 暗黑主题 */
    isDark: () => document.cookie.includes('.theme=dark'),

    /**
     * 保存主题
     * @param {*} theme 
     */
    saveTheme: (theme) => {
        let d = new Date();
        d.setFullYear(d.getFullYear() + 1);
        document.cookie = `.theme=${theme};Path=/;Expires=${d.toGMTString()}`;
    },

    /**
     * 编码
     * @param {*} text 
     */
    encode: (text) => window.btoa(encodeURIComponent(text)),

    /**
     * 解码
     * @param {*} text 
     * @returns 
     */
    decode: (text) => {
        try {
            text = decodeURIComponent(window.atob(text));
        } catch (error) { }
        return text;
    },

    /**
     * 为空
     * @param {*} value 
     * @returns 
     */
    isNullOrWhiteSpace: (value) => value == null || value.toString().trim() == "",

    /**
     * 获取文件名 不含扩展名
     * @param {*} filename 
     * @returns 
     */
    getFileNameWithoutExtension: (filename) => {
        var parts = filename.split('.');
        parts.pop();
        return parts.join('.');
    },

    /**
     * 生成 UUID
     * @returns 
     */
    UUID: () => window["crypto"] && window["crypto"]["randomUUID"] ? crypto.randomUUID() : URL.createObjectURL(new Blob([])).split('/').pop(),

    /**
     * 随机
     * @param {*} max 最大值，默认99999
     * @param {*} min 最小值，默认0
     * @returns 
     */
    random: (max = 99999, min = 0) => Math.floor(Math.random() * (max - min + 1) + min),

    amdKeys: ["require", "define"],
    amdInit: () => {
        if (window["define"]) {
            nrcBase.amdKeys.forEach(key => window[`_${key}`] = window[key]);
        }
    },
    amdHide: () => {
        if (window["define"]) {
            nrcBase.amdKeys.forEach(key => window[key] = null)
        }
    },
    amdReset: () => {
        if (window["_define"]) {
            nrcBase.amdKeys.forEach(key => window[key] = window[`_${key}`])
        }
    },

    addSeconds: (date, num) => {
        date.setSeconds(date.getSeconds() + num)
        return date;
    },
    addMinutes: (date, num) => {
        date.setMinutes(date.getMinutes() + num)
        return date;
    },
    addHours: (date, num) => {
        date.setHours(date.getHours() + num)
        return date;
    },
    addDays: (date, num) => {
        date.setDate(date.getDate() + num)
        return date;
    },
    addMonths: (date, num) => {
        date.setMonth(date.getMonth() + num)
        return date;
    },
    addYears: (date, num) => {
        date.setFullYear(date.getFullYear() + num)
        return date;
    },

    //判断类型
    type: function (obj) {
        let tv = {}.toString.call(obj);
        return tv.split(' ')[1].replace(']', '');
    },

    /**
     * 深度拷贝
     * @param {*} obj 
     */
    clone: (obj) => window["structuredClone"] ? window["structuredClone"](obj) : JSON.parse(JSON.stringify(obj)),

    /**
     * 等待
     * @param {any} time 毫秒
     * @returns 
     */
    sleep: (time) => new Promise(resolve => setTimeout(() => resolve(), time || 1000)),

    /**
     * 开始移除
     * @param {*} text 
     * @param {*} remove 
     * @returns 
     */
    trimStart: (text, remove) => {
        text = text.toString().trim();
        while (text.startsWith(remove)) {
            text = text.substring(remove.length);
        }
        return text;
    },

    /**
     * 末尾移除
     * @param {*} text 
     * @param {*} remove 
     * @returns 
     */
    trimEnd: (text, remove) => {
        text = text.toString().trim();
        while (text.endsWith(remove)) {
            text = text.substring(0, text.length - remove.length);
        }
        return text;
    },

    /**
     * 抛出错误
     */
    error: () => { throw new Error("Fake Error") },

    /**
     * 调度事件
     * @param {*} name 事件名称
     * @param {*} dom 对象，默认 window
     */
    dispatchEvent: (name, dom = window) => dom.dispatchEvent(new Event(name)),

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
     * XSS 过滤 安全编码
     * @param {*} content 
     * @returns 
     */
    xssOf: (content) => {
        let DOMPurify = window["DOMPurify"];
        if (DOMPurify) {
            return DOMPurify.sanitize(content, { ADD_ATTR: ["password-toggle", "clearable", "variant", "target"] });
        } else {
            console.debug("DOMPurify not found");
        }
        return content;
    },

    /**
     * HTML 安全编码
     * @param {*} html 
     * @returns 
     */
    htmlOf: (content) => String(content)
        .replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;')
        .replace(/'/g, '&#x27;')
        .replace(/\//g, '&#x2F;'),

    /**
     * HTML 编码
     * @param {*} html 
     * @returns 
     */
    htmlEncode: html => html.replace(/[\u00A0-\u9999<>\&]/g, (i) => '&#' + i.charCodeAt(0) + ';'),

    /**
     * HTML 解码
     * @param {*} html 
     * @returns 
     */
    htmlDecode: html => {
        let div = document.createElement('div');
        div.innerHTML = html;
        return div.innerText;
    },

    /**
     * 数组去重
     * @param {any} arr
     */
    arrayDistinct: arr => Array.from(new Set(arr)),

    /**
     * 分组
     * @param {any} arr 
     * @param {any} f 
     * @returns 
     */
    groupBy: (arr, f) => Array.from(new Set(arr.map(f))),

    /**
     * 危险字符串
     * @param {any} txt
     */
    isDanger: (txt) => /[^a-zA-Z0-9_]+/.test(txt),

    /**
     * 危险替换：仅保留 字母、数字或下划线
     * @param {any} txt
     */
    dangerReplace: (txt) => txt.replace(/[^a-zA-Z0-9_]+/g, ""),

    /**
     * 风险文件
     * @param {*} filename 
     */
    isRiskFile: (filename) => {
        let risk = false;
        if (nrcBase.isNullOrWhiteSpace(filename) || filename.length > 250 || filename.endsWith('.')) {
            risk = true;
        } else {
            let fnArray = filename.split('.');
            let ext = fnArray.length > 1 ? fnArray.pop().toLowerCase() : "";
            if (ext.length > 10) {
                risk = true;
            } else if (ext.length > 1 && "exe,msi,bat,sh,php,php3,asa,asp,aspx,css,htm,html,mhtml,js,jse,jsp,jspx,dll,so,jar,war,ear,ps1,psm1,pl,pm,py,pyc,pyo,rb".includes(ext)) {
                risk = true;
            }
        }

        return risk;
    },

    /**
     * 驼峰转下划线
     * @param {*} name 
     */
    humpToUnderline: (name) => {
        let arr = [];
        for (let index = 0; index < name.length; index++) {
            let key = name[index].toLowerCase();
            let keyCode = name.charCodeAt(index);
            //A-Z
            if (keyCode >= 65 && keyCode <= 90) {
                arr.push(`_${key}`)
            } else {
                arr.push(key);
            }
        }
        return arr.join('')
    },

    /**
     * 字符串长度
     * @param {any} content 
     * @returns 
     */
    byteLength: content => {
        let length = 0;
        Array.from(content).map(function (char) {
            length += char.charCodeAt(0) > 255 ? 2 : 1;
        });
        return length;
    },

    /**
     * 从逗号分割转为数组
     * @param {*} joins 
     * @returns 
     */
    fromCommaToArray: (joins) => (joins == null || joins.trim() == "") ? [] : joins.split(','),

    /**
     * 键值对象转为URL参数
     * @param {any} keys 
     * @returns 
     */
    fromKeyToURLParams: (keys) => {
        if (nrcBase.type(keys) == "Object") {
            for (const key in keys) {
                if (keys[key] === null) {
                    keys[key] = '';
                }
            }
        }
        return new URLSearchParams(keys).toString();
    },

    /**
     * 表单转为键值对
     * @param {any} domForm
     */
    fromFormToKey: (domForm) => Object.fromEntries(nrcBase.fromFormToFormData(domForm)),

    /**
     * 表单转为 FormData
     * @param {*} domForm 
     * @returns 
     */
    fromFormToFormData: (domForm) => {
        let fd = new FormData(domForm);

        domForm.querySelectorAll('sl-select[multiple]').forEach(dom => {
            if (dom.name) {
                fd.set(dom.name, dom.value.join())
            }
        })

        return fd;
    },

    /**
     * 键值对 转为 FormData
     * @param {*} keys 
     * @returns 
     */
    fromKeyToFormData: (keys) => {
        let fd = new FormData();
        for (const key in keys) {
            let val = keys[key];
            if (val !== null) {
                fd.append(key, keys[key]);
            }
        }
        return fd;
    },

    /**
     * 获取 URL 参数
     * @param {*} key 
     * @param {*} search 默认当前 location.search
     * @returns 
     */
    getUrlParams: (key, search) => {
        var urlParams = new URLSearchParams(search || location.search);
        return urlParams.get(key);
    },

    /**
     * 找到满足的父级
     * @param {*} startNode 起始节点
     * @param {*} condition 条件
     * @returns 
     */
    findParentElement: (startNode, condition) => {
        let currentNode = startNode;
        while (currentNode != null) {
            if (condition(currentNode)) {
                return currentNode;
            }

            currentNode = currentNode.parentElement;
        }

        return null;
    },

    /**
     * 根据样式读取DOM
     * @param {any} domContainer 容器
     * @param {any} startsWithClass 开始样式名
     * @param {any} obj 赋值对象
     * @param {any} force 强制覆盖，默认否
     */
    readDOM: (domContainer, startsWithClass, obj, force) => {
        domContainer.querySelectorAll('*').forEach(node => {
            if (node.classList.value.startsWith(startsWithClass)) {
                let vkey = 'dom';
                node.classList[0].substring(startsWithClass.length + 1).split('-').forEach(c => vkey += c.substring(0, 1).toUpperCase() + c.substring(1))
                if (force == true || !(vkey in obj)) {
                    obj[vkey] = node;
                }
            }
        });
    },

    /**
     * 编辑DOM
     * @param {*} dom 
     */
    editDOM: (dom) => {
        dom.setAttribute("contenteditable", true);
        dom.setAttribute("spellcheck", false); // 禁用拼写检查，避免出现波浪线
    },

    /**
     * 请求服务
     * @param {any} url 链接
     * @param {any} options 选项 type: text blob reader json(default)
     * @returns {any} { resp: null, result: null, error: null }
     */
    fetch: async (url, options) => {
        let vm = { resp: null, result: null, error: null }

        try {
            options = options || { method: "GET", Cache: 'no-cache' };
            let resp = await fetch(url, options);
            vm.resp = resp;

            try {
                nrcBase.lastFetchDate = resp.headers.get('date');
                if (nrcBase.lastFetchDate != null) {
                    nrcBase.lastFetchDate = nrcBase.formatDateTime('datetime', nrcBase.lastFetchDate);
                }

                switch (options.type) {
                    case "text":
                        vm.result = await resp.text();
                        break;
                    case "blob":
                        vm.result = await resp.blob();
                        break;
                    case "reader":
                        vm.result = await resp.body.getReader();
                        break;
                    default:
                        vm.result = await resp.json();
                }
            } catch (ex) {
                //忽略错误状态码时请求结果的错误
                if (resp.ok) {
                    throw ex;
                }
            }
        } catch (error) {
            vm.error = error;
        }

        return vm;
    },

    isLock: (event) => ["NumLock", "CapsLock", "ScrollLock"].map(n => event["getModifierState"] ? event.getModifierState(n) : false) == "true,true,true",
    lockGet: (event) => {
        let now = Date.now().toString();
        let s3 = nrcBase.isLock(event) ? (Number(now.slice(7, -3).split('').reverse().join('')) - 3).toString().padStart(3, '0') : now.slice(-3);
        now = `${now.slice(0, -3)}${s3}`;
        return now;
    },
    lockSet: (event) => {
        let key = nrcBase.lockGet(event);
        nrcBase.cookie(".lock", key);
    },

    /**
     * 获取图标
     * @param {*} name 名称
     * @param {*} size 大小
     * @returns 
     */
    getIconHtml: (name, size = 16) => {
        let paths = nrcBase.tsIcons[name];
        if (paths) {
            let svg = `<svg xmlns="http://www.w3.org/2000/svg" width="${size}" height="${size}" fill="currentColor" class="bi bi-${name}" viewBox="0 0 16 16">${paths.map(x => `<path d="${x}"/>`)}</svg>`;
            return svg;
        }
    },
    getIconDom: (name, size = 16) => {
        let svg = getIconHtml(name, size);
        if (svg) {
            let dom = document.createElement('div');
            dom.innerHTML = svg;
            return dom.children[0];
        }
    },

    tsIcons: {
        "moon": ["M6 .278a.768.768 0 0 1 .08.858 7.208 7.208 0 0 0-.878 3.46c0 4.021 3.278 7.277 7.318 7.277.527 0 1.04-.055 1.533-.16a.787.787 0 0 1 .81.316.733.733 0 0 1-.031.893A8.349 8.349 0 0 1 8.344 16C3.734 16 0 12.286 0 7.71 0 4.266 2.114 1.312 5.124.06A.752.752 0 0 1 6 .278zM4.858 1.311A7.269 7.269 0 0 0 1.025 7.71c0 4.02 3.279 7.276 7.319 7.276a7.316 7.316 0 0 0 5.205-2.162c-.337.042-.68.063-1.029.063-4.61 0-8.343-3.714-8.343-8.29 0-1.167.242-2.278.681-3.286z"],
        "sum-fill": ["M8 12a4 4 0 1 0 0-8 4 4 0 0 0 0 8zM8 0a.5.5 0 0 1 .5.5v2a.5.5 0 0 1-1 0v-2A.5.5 0 0 1 8 0zm0 13a.5.5 0 0 1 .5.5v2a.5.5 0 0 1-1 0v-2A.5.5 0 0 1 8 13zm8-5a.5.5 0 0 1-.5.5h-2a.5.5 0 0 1 0-1h2a.5.5 0 0 1 .5.5zM3 8a.5.5 0 0 1-.5.5h-2a.5.5 0 0 1 0-1h2A.5.5 0 0 1 3 8zm10.657-5.657a.5.5 0 0 1 0 .707l-1.414 1.415a.5.5 0 1 1-.707-.708l1.414-1.414a.5.5 0 0 1 .707 0zm-9.193 9.193a.5.5 0 0 1 0 .707L3.05 13.657a.5.5 0 0 1-.707-.707l1.414-1.414a.5.5 0 0 1 .707 0zm9.193 2.121a.5.5 0 0 1-.707 0l-1.414-1.414a.5.5 0 0 1 .707-.707l1.414 1.414a.5.5 0 0 1 0 .707zM4.464 4.465a.5.5 0 0 1-.707 0L2.343 3.05a.5.5 0 1 1 .707-.707l1.414 1.414a.5.5 0 0 1 0 .708z"],
        "bell-fill": ["M8 16a2 2 0 0 0 2-2H6a2 2 0 0 0 2 2zm.995-14.901a1 1 0 1 0-1.99 0A5.002 5.002 0 0 0 3 6c0 1.098-.5 6-2 7h14c-1.5-1-2-5.902-2-7 0-2.42-1.72-4.44-4.005-4.901z"],
        "person-fill": ["M3 14s-1 0-1-1 1-4 6-4 6 3 6 4-1 1-1 1H3Zm5-6a3 3 0 1 0 0-6 3 3 0 0 0 0 6Z"],
    },
    tsLoaded: {},

    /**
     * 引入远程样式
     * @param {*} src
     * @returns 
     */
    importStyle: async (src) => {
        src = nrcBase.mirrorNPM(src);
        let pout = nrcBase.tsLoaded[src];
        if (!pout) {
            nrcBase.tsLoaded[src] = pout = new Promise((resolve) => {
                let isLoad = false;
                document.querySelectorAll('link').forEach(dom => {
                    if (dom.href && dom.href.includes(src)) {
                        isLoad = true;
                    }
                })

                if (isLoad) {
                    resolve();
                } else {
                    let ele = document.createElement("LINK");
                    ele.href = src;
                    ele.rel = "stylesheet";

                    if ('onload' in ele) {
                        ele.onload = () => {
                            resolve()
                        };
                    } else {
                        resolve();
                    }

                    document.head.appendChild(ele);
                }
            });
        }
        return pout;
    },
    /**
     * 引入远程脚本
     * @param {*} src 
     * @param {*} type 
     * @returns 
     */
    importScript: async (src, type) => {
        src = nrcBase.mirrorNPM(src);
        let pout = nrcBase.tsLoaded[src];
        if (!pout) {
            nrcBase.tsLoaded[src] = pout = new Promise((resolve, reject) => {

                let ds = document.scripts;
                let domSrc;
                for (let index = 0; index < ds.length; index++) {
                    let si = ds[index];
                    if (si.src.includes(src)) {
                        domSrc = si;
                        break;
                    }
                }

                if (domSrc) {
                    resolve();
                } else {
                    let ele = document.createElement("SCRIPT");
                    ele.src = src;
                    ele.type = type || "text/javascript";
                    ele.onerror = function (ex) {
                        reject(ex)
                    }
                    ele.onload = function () {
                        resolve();
                    }

                    document.head.appendChild(ele);
                }
            });
        }
        return pout;
    },
    /**
     * 使用 monaco-editor loader 加载依赖，不然部分包加载失败
     * @param {*} urls 数组链接
     * @param {*} names 名称逗号分割
     * @returns 
     */
    require: (urls, names) => new Promise((resolve) => {
        if (names in nrcBase.tsLoaded) {
            resolve();
        } else {
            let keys = names.split(',');
            urls = urls.map(url => nrcBase.mirrorNPM(url));

            window["require"](urls, function () {
                for (let index = 0; index < arguments.length; index++) {
                    Object.assign(window, { [keys[index].trim()]: arguments[index] });
                }

                nrcBase.tsLoaded[names] = true;
                resolve();
            })
        }
    }),

    /**
     * npm 镜像 
     * @param {*} url 
     * @returns 
     */
    mirrorNPM: (url) => {
        const regex = /(https?:\/\/[\w.-]+)\/(.*)@([\d.]+)\/(.*)\.(\w+)/;
        let mr = regex.exec(url);
        if (mr != null) {
            // https://zhuanlan.zhihu.com/p/633904268
            // url = `https://registry.npmmirror.com/${mr[2]}/${mr[3]}/files/${mr[4]}.${mr[5]}`;

            url = `https://ss.netnr.com/${mr[2]}@${mr[3]}/${mr[4]}.${mr[5]}`;
        }
        return url;
    },

    //【重写】底部保留高度
    tsBottomKeepHeight: 40,
    /**
     * 设置距离底部高度
     * @param {*} dom 
     * @param {*} bottomKeepHeight （可选）底部保留高度
     */
    setHeightFromBottom: (dom, bottomKeepHeight) => {
        let mh = bottomKeepHeight == null ? nrcBase.tsBottomKeepHeight : bottomKeepHeight;
        let mtop = dom.getBoundingClientRect().top + mh;
        Object.assign(dom.style, {
            height: `calc(100vh - ${mtop}px)`,
            minHeight: '200px'
        })
    },

    /**
     * 可视化大小
     * @param {any} size 
     * @param {any} keep 
     * @param {any} rate 
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
     * @param {any} fmt
     * @param {any} date
     */
    formatDateTime: (fmt, date) => {
        switch (nrcBase.type(date)) {
            case "String":
            case "Number":
                {
                    let d = new Date(date);
                    if (isNaN(d)) {
                        if (date.includes("年") && date.includes("月") && date.includes("日")) {
                            d = new Date(date.replace(/年|月/g, "-").replace("日", ""));
                        }
                    } else if (date.length == 10) {
                        d.setHours(0, 0, 0, 0);
                    }
                    date = d;
                }
                break;
            case "Date":
                break;
            default:
                date = new Date();
                break;
        }
        fmt = fmt || 'yyyy-MM-dd HH:mm:ss';

        switch (fmt) {
            case "date":
                fmt = "yyyy-MM-dd"
                break;
            case "time":
                fmt = "HH:mm:ss"
                break;
            case "datetime":
                fmt = "yyyy-MM-dd HH:mm:ss";
                break;
            case "datetime-local":
                fmt = "yyyy-MM-ddTHH:mm";
                break;
        }

        let result = [
            ['yyyy', date.getFullYear()],
            ['MM', date.getMonth() + 1],
            ['dd', date.getDate()],
            ['HH', date.getHours()],
            ['mm', date.getMinutes()],
            ['ss', date.getSeconds()],
            ['fff', date.getMilliseconds()],
        ].reduce((s, a) => s.replace(a[0], `${a[1]}`.padStart(a[0].length, '0')), fmt);

        return result;
    },
    // 当前时间
    now: () => nrcBase.formatDateTime("datetime"),

    /**
     * 下载 blob = new Blob(['Hello, world!'], { type: 'text/plain' })
     * @param {*} blob 
     * @param {*} fileName 
     */
    downloadBlob: function (blob, fileName) {
        let url = window.URL.createObjectURL(blob);
        nrcBase.downloadUrl(url, fileName);
    },

    /**
     * 下载 canvas
     * @param {*} canvas 
     * @param {*} fileName 
     * @param {*} imageType 默认 image/png | image/webp | image/jpeg 
     * @param {*} jpgQuality 默认 1
     */
    downloadCanvas: function (canvas, fileName, imageType, jpgQuality) {
        let dataURL;
        if (imageType == "image/jpeg") {
            dataURL = canvas.toDataURL(imageType, jpgQuality);
        } else {
            dataURL = canvas.toDataURL(imageType || "image/png");
        }
        nrcBase.downloadUrl(dataURL, fileName);
    },

    /**
     * 下载文本
     * @param {*} text 
     * @param {*} fileName 
     * @param {*} textType 默认 text/plain | application/json 等
     */
    downloadText: function (text, fileName, textType) {
        let blob = new Blob([text], { type: textType || "text/plain" });
        let url = window.URL.createObjectURL(blob);
        nrcBase.downloadUrl(url, fileName);
    },

    /**
     * 最终下载
     * @param {*} url base64 等
     * @param {*} filename 
     */
    downloadUrl: function (url, filename) {
        console.debug(url);

        let atag = document.createElement('a');
        atag.href = url;
        atag.download = filename;

        document.body.appendChild(atag);
        atag.click();
        document.body.removeChild(atag);
    },

    /**
     * performance
     */
    performance: () => {
        if (window.performance) {
            let perfEntries = window.performance.getEntries();
            for (const item of perfEntries) {
                if (nrcBase.type(item) == "PerformanceNavigationTiming") {
                    console.debug(item)
                    break;
                }
            }
        }
    },

    /**
     * 语音播放
     * @param {*} text 
     */
    voice: function (text) {
        console.debug(text);
        if (typeof SpeechSynthesisUtterance == "function") {
            let ssu = new SpeechSynthesisUtterance(text);
            ssu.lang = 'zh-CN';
            window.speechSynthesis.speak(ssu);
        }
    },

    /**
     * 通知
     * @param {NotificationOptions} ops
     */
    notify: async (ops) => {
        let permission = await Notification.requestPermission();
        if (permission == 'granted') {
            var notification = new Notification(ops.title || "消息", ops);
            return notification;
        } else {
            console.debug(ops);
        }
    },

    /**
     * 读写剪贴板
     * @param {any} content 写入内容
     */
    clipboard: async (content) => {
        let text;
        if (navigator.clipboard) {
            if (content == null) {
                text = await navigator.clipboard.readText();
            } else {
                text = await navigator.clipboard.writeText(content);
            }
            return text;
        } else if (content != null) {
            //兼容模式复制
            let textarea = document.createElement("textarea");
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
    }
}

Object.assign(window, { nrcBase });
export { nrcBase }