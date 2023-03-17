let nrSpider = {

    parsingKey: "nr", //解析前缀

    countRequest: 0, //请求量
    currentRequest: 0, //当前请求量
    lastRequestTime: Date.now(), //最后请求时间
    queueList: [], //队列记录

    inited: false, //初始化

    defer: {}, //延迟

    enableVoice: true, //是否开启语音提示

    /**
     * 队列新增
     * @param {any} task
     */
    queueAdd: function (task) {
        if (nrSpider.type(task) == "Array") {
            nrSpider.queueList = nrSpider.queueList.concat(task);
        } else {
            nrSpider.queueList.push(task);
        }
    },

    /**
     * 队列消费
     * @param {any} n
     */
    queueUse: function (n) {
        var len = nrSpider.queueList.length;
        if (len) {
            return nrSpider.queueList.splice(0, Math.min(n || 1, len));
        }
        return null;
    },

    /**
     * 输出熔断
     * @param {any} msg 消息
     * @param {any} type 分类
     */
    consoleFuse: function (msg, type) {
        var dkey = `console${type}`, ltkey = `lastTime${type}`, now = Date.now();
        if (nrSpider.defer[ltkey] == null || now - nrSpider.defer[ltkey] > 3000) {
            nrSpider.defer[ltkey] = now;
            console.debug(msg)
        } else {
            clearTimeout(nrSpider.defer[dkey]);
            nrSpider.defer[dkey] = setTimeout(() => {
                console.debug(msg)
            }, 800);
        }
    },

    /**
     * 任务熔断
     * @param {any} task 任务
     * @param {any} gap 间隔时间，默认1000毫秒
     */
    taskFuse: function (task, gap) {
        gap = gap || 1000;

        clearInterval(nrSpider.defer.task);
        nrSpider.defer.task = setInterval(() => {
            var notGap = Date.now() - nrSpider.lastRequestTime < gap, isLimit = nrSpider.currentRequest > 0;
            if (notGap || isLimit) {
                if (Date.now() - nrSpider.lastRequestTime > 1000 * 60) {
                    nrSpider.currentRequest--;
                    nrSpider.consoleFuse('Task timeout skipped...', 'task');
                } else {
                    nrSpider.consoleFuse('Task limit, waiting...', 'task');
                }
            } else {
                task();
            }
        }, 2);

        document.onkeydown = (e) => {
            //Pause/Break
            if (e.keyCode == 19) {
                clearInterval(nrSpider.defer.task);
                nrSpider.defer.taskStatus = 0;
                nrSpider.noticeVoice("任务已暂停");
            }
        }
    },

    /**
     * 载入 js
     * @param {any} src
     */
    getScript: function (src) {
        return new Promise((resolve) => {
            var isLoad = false;
            for (var i = 0; i < document.scripts.length; i++) {
                var si = document.scripts[i];
                if (si.src == src) {
                    isLoad = true;
                    break;
                }
            }
            if (isLoad) {
                resolve();
            } else {
                var ele = document.createElement("SCRIPT");
                ele.src = src;
                ele.type = "text/javascript";
                document.head.appendChild(ele);
                ele.onload = ele.onreadystatechange = function () {
                    if (!this.readyState || this.readyState == "loaded" || this.readyState == "complete") {
                        resolve();
                    }
                }
            }
        })
    },

    /**
     * 载入JS
     * @param {any} srcs
     */
    getScripts: function (srcs) {
        var parr = [];
        srcs.forEach(src => parr.push(nrSpider.getScript(src)));
        return Promise.all(parr)
    },

    /**
     * 判断类型
     * @param {any} obj
     */
    type: function (obj) {
        var tv = {}.toString.call(obj);
        return tv.split(' ')[1].replace(']', '');
    },

    /**
     * base64 => Uint8Array
     * @param {any} base64
     */
    base64ToUint8Array: function (base64) {
        base64 = base64.split(',').pop();
        var raw = atob(base64);
        var uint8Array = new Uint8Array(raw.length);
        for (var i = 0; i < raw.length; i++) {
            uint8Array[i] = raw.charCodeAt(i);
        }
        return uint8Array;
    },

    /**
     * 解析HTML
     * @param {any} html
     * @param {any} link
     */
    parsingHtml: function (html, link) {
        link = link || location.href;

        var vdom = document.createElement('div'), skey = ["src", "href"];
        skey.forEach(key => html = html.replaceAll(`${key}=`, `${nrSpider.parsingKey}${key}=`));
        vdom.innerHTML = html;

        skey.forEach(key => {
            var nrkey = nrSpider.parsingKey + key;
            vdom.querySelectorAll(`[${nrkey}]`).forEach(node => {
                var paths = link.split('/');
                paths.pop();

                var attr = node.getAttribute(`${nrkey}`);
                if (attr != null && !attr.startsWith("http") && !attr.startsWith("javascript:")) {
                    var newattr;
                    if (attr.startsWith("/")) {
                        newattr = new URL(link).origin + attr;
                    } else {
                        while (attr.startsWith('../')) {
                            attr = attr.substring(3);
                            paths.pop();
                        }
                        newattr = paths.join('/') + '/' + attr;
                    }
                    node.setAttribute(`${nrkey}`, newattr);
                }
            })
        })
        return vdom;
    },

    /**
     * 获取属性
     * @param {any} node
     * @param {any} name
     */
    parsingAttr: (node, name) => node.getAttribute(nrSpider.parsingKey + name),

    /**
     * 请求链接
     * @param {any} url
     * @param {any} encoding GBK 或 utf-8
     */
    requestLink: function (url, encoding) {
        return new Promise((resolve, reject) => {
            //请求数
            nrSpider.countRequest++;
            nrSpider.currentRequest++;
            nrSpider.lastRequestTime = Date.now();

            fetch(url).then(res => {
                nrSpider.currentRequest--;

                if (res.status == 200) {
                    return res.blob();
                } else {
                    reject(res.status + " " + res.statusText);
                }
            }).then(blob => {
                var reader = new FileReader();
                reader.onload = function (e) {
                    var res = e.target.result;
                    resolve(res);
                }
                reader.readAsText(blob, encoding || 'utf-8');
            }).catch(err => {
                nrSpider.currentRequest--;

                reject(err)
            })
        })
    },

    /**
     * 请求（自动缓存）
     * @param {any} url
     * @param {any} encoding 编码
     * @param {any} cachePrefix 缓存前缀
     */
    requestCache: function (url, encoding, cachePrefix) {
        return new Promise((resolve, reject) => {
            var ckey = `${cachePrefix}:${url}`
            nrSpider.getItem(ckey).then(res => {
                if (res == null || res.includes("请开启JavaScript并刷新该页")) {
                    nrSpider.requestLink(url, encoding).then(html => {
                        if (html.includes("请开启JavaScript并刷新该页")) {
                            reject(err);
                        } else {
                            nrSpider.setItem(ckey, html);
                            resolve(html);
                        }
                    }).catch(err => {
                        reject(err);
                    })
                } else {
                    resolve(res);
                }
            })
        })
    },

    /**
     * blob 构建 文本
     * @param {any} data
     */
    blobCreateText: data => new Blob([data], { type: "text/plain;charset=utf-8" }),

    /**
     * blob 构建
     * @param {any} data
     * @param {any} type
     */
    blobCreate: (data, type) => new Blob([data], { type: type }),

    /**
     * blob 下载
     * @param {any} blob
     * @param {any} filename
     */
    blobDownload: (blob, filename) => saveAs(blob, filename),

    /**
     * blob 转 url
     * @param {any} blob
     */
    blobAsUrl: blob => window.URL.createObjectURL(blob),

    /**
     * blob 请求
     * @param {any} src
     */
    blobGet: function (src) {
        return fetch(src, {
            method: 'get',
            responseType: 'blob'
        }).then(res => {
            return res.blob();
        });
    },

    /**
     * 语音播放
     * @param {*} text 
     */
    noticeVoice: function (text) {
        console.debug(text);
        if (nrSpider.enableVoice) {
            var msg = new SpeechSynthesisUtterance(text);
            msg.lang = 'zh-CN';
            window.speechSynthesis.speak(msg);
        }
    },

    /**
     * 获取 PDF 文档对象
     * @param {any} url
     */
    pdfDocumentGet: function (url) {
        return new Promise((resolve, reject) => {
            (new Promise((resolve1) => {
                switch (nrSpider.type(url)) {
                    case "File":
                        {
                            var fr = new FileReader();
                            fr.onload = function (e) {
                                var u8a = nrSpider.base64ToUint8Array(e.target.result)
                                resolve1(u8a);
                            }
                            fr.readAsDataURL(url)
                        }
                        break;
                    case "String":
                    default:
                        resolve1(url);
                }
            })).then(url => {
                pdfjsLib.getDocument(url).promise.then(pdfDocument => {
                    //console.log(pdfDocument.numPages); //总页数
                    resolve(pdfDocument);
                }).catch(err => {
                    reject(err);
                })
            })
        })
    },

    /**
     * 获取 PDF 页对象
     * @param {any} pdfDocument
     * @param {any} pageIndex
     */
    pdfPageGet: (pdfDocument, pageIndex) => pdfDocument.getPage(pageIndex),

    /**
     * 渲染 PDF 页
     * @param {any} pdfPage
     * @param {any} canvas
     */
    pdfPageRender: function (pdfPage, canvas) {
        var viewport = pdfPage.getViewport({ scale: 1 });
        canvas.width = viewport.width;
        canvas.height = viewport.height;

        var ctx = canvas.getContext("2d");
        var renderTask = pdfPage.render({
            canvasContext: ctx,
            viewport
        });
        return renderTask.promise
    },

    /**
     * 获取 PDF 页内容
     * @param {any} pdfPage
     */
    pdfPageContent: pdfPage => pdfPage.getTextContent(),

    /**
     * 获取 PDF 页内容（按行简单处理）
     * @param {any} pdfPage
     */
    pdfPageContentRows: function (pdfPage) {
        return new Promise((resolve) => {
            var rows = [], line = 0;
            pdfPage.getTextContent().then(textContent => {
                line = textContent.items[0].transform[5];
                textContent.items.forEach(item => {
                    if (item.fontName != "Times") {
                        if (line != item.transform[5]) {
                            rows.push('\r\n');
                            line = item.transform[5];
                        }
                        rows.push(item.str)
                    }
                })
                resolve(rows);
            })
        })
    },

    /**
     * array => excel
     * @param {any} arr
     * @param {any} heads 头宽度（可选） {id:70,txt:150}
     */
    arrayToExcel: function (arr, heads) {
        return new Promise((resolve) => {
            var workbook = new ExcelJS.Workbook();

            workbook.creator = 'netnr';
            workbook.lastModifiedBy = 'netnr';
            workbook.created = new Date();
            workbook.modified = workbook.created;
            workbook.lastPrinted = workbook.created;

            var worksheet = workbook.addWorksheet("Sheet1");
            worksheet.views = [
                { state: 'frozen', xSplit: 0, ySplit: 1 }
            ];

            //填充头部
            var headers = [];
            if (heads == null) {
                heads = {};
                for (var i in arr[0]) {
                    heads[i] = 120;
                }
            }
            for (var i in heads) {
                headers.push({ header: i, key: i, width: heads[i] / 7, style: { alignment: { vertical: 'middle', horizontal: 'left' } } })
            }
            worksheet.columns = headers;

            //头部样式
            var firstRow = worksheet.getRow(1);
            firstRow.height = 20;
            firstRow.font = { size: 12, bold: true };
            for (var i = 1; i <= headers.length; i++) {
                var cell = firstRow.getCell(i);
                cell.border = {
                    top: { style: 'thin' },
                    left: { style: 'thin' },
                    bottom: { style: 'thin' },
                    right: { style: 'thin' }
                };
            }

            //填充数据行
            var rows = [];
            arr.forEach(obj => {
                var orow = Object.values(obj);
                if (orow.length != heads.length) {
                    orow = [];
                    for (var i in heads) {
                        if (i in obj) {
                            orow.push(obj[i])
                        } else {
                            orow.push(null);
                        }
                    }
                }
                rows.push(orow);
            });
            worksheet.addRows(rows);

            //保存
            workbook.xlsx.writeBuffer().then(function (data) {
                var blob = nrSpider.blobCreate(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                resolve(blob);
            });
        })
    },

    /**
     * array => SQL
     * @param {any} arr
     * @param {any} tableName 表名
     * @param {any} cols 列 {id:"text primary key not null",txt:"text",deep:"int not null"}
     */
    arrayToSQL: function (arr, tableName, cols) {
        return new Promise((resolve) => {
            var esql = [];

            //创建数据库
            var fiels = [];
            for (var i in cols) {
                fiels.push(`[${i}] ${cols[i]}`);
            }
            esql.push(`DROP TABLE IF EXISTS [${tableName}]`);
            esql.push(`CREATE TABLE [${tableName}](${fiels.join(',')})`);

            //写入数据
            arr.forEach(obj => {
                var values = [];
                for (var i in cols) {
                    var ci = obj[i];
                    if (ci == null) {
                        values.push('null');
                    } else {
                        values.push(cols[i].indexOf("int") >= 0 ? ci : `'${ci}'`);
                    }
                }
                esql.push(`INSERT INTO [${tableName}] VALUES (${values.join(',')})`)
            });

            resolve(esql.join(';\n'));
        })
    },

    //初始化
    init: function () {
        return new Promise((resolve) => {
            if (nrSpider.inited) {
                resolve();
            } else {
                nrSpider.getScripts([
                    "https://npmcdn.com/localforage@1.10.0/dist/localforage.min.js",
                    "https://npmcdn.com/jszip@3.10.1/dist/jszip.min.js",
                    "https://npmcdn.com/file-saver@2.0.5/dist/FileSaver.min.js",
                    "https://npmcdn.com/exceljs@4.3.0/dist/exceljs.min.js",
                    "https://npmcdn.com/pdfjs-dist@3.4.120/legacy/build/pdf.min.js"
                ]).then(() => {
                    nrSpider.noticeVoice("组件初始化完成");
                    nrSpider.noticeVoice("如需关闭语音提示，请设置 nrSpider.noticeVoice = false");

                    nrSpider.setItem = localforage.setItem;
                    nrSpider.getItem = localforage.getItem;

                    nrSpider.inited = true;
                    resolve();
                });
            }
        })
    }
}

nrSpider.init();