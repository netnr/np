var sz = {

    //版本号
    version: "2.2021.0",
    date: "2021-12-30",

    //抓取目录
    dataIndex: [],
    //抓取数据
    dataResult: [],
    //抓取异常记录
    dataCatch: [],

    /**
     * 匹配结果
     * @param {any} html 内容
     * @param {any} item
     */
    matchResult: function (html, item) {
        var data = [];

        //解析
        var vdom = nrSpider.parsingHtml(html, item.suburl);

        //有子集
        var trs = vdom.querySelectorAll('.citytr,.countytr,.towntr,.villagetr'), oi = 1;
        trs.forEach(tr => {
            //五级
            if (tr.classList.contains("villagetr")) {
                var tds = tr.getElementsByTagName('td');
                var sid = tds[0].innerText.trim();
                data.push({ id: sid, txt: tds[2].innerText.trim(), pid: item.id, sid: sid, spid: item.sid, ct: tds[1].innerText.trim(), num: oi++, url: item.suburl, leaf: 1, deep: item.deep + 1 })
            } else {
                var aa = tr.getElementsByTagName('a');
                if (aa.length) {
                    var href = nrSpider.parsingAttr(aa[0], 'href');
                    data.push({ id: aa[0].innerText.trim(), txt: aa[1].innerText.trim(), pid: item.id, sid: href.split('/').pop().split('.')[0], spid: item.sid || item.id, num: oi++, url: item.suburl, suburl: nrSpider.parsingAttr(aa[0], 'href'), leaf: 0, deep: item.deep + 1 })
                } else {
                    //中间级别无下级 http://www.stats.gov.cn/tjsj/tjbz/tjyqhdmhcxhfdm/2021/65/6501.html
                    var tds = tr.getElementsByTagName('td');
                    var sid = sz.trimEndZero(tds[0].innerText.trim());
                    data.push({ id: tds[0].innerText.trim(), txt: tds[1].innerText.trim(), pid: item.id, sid: sid, spid: item.sid, num: oi++, url: item.suburl, leaf: 1, deep: item.deep + 1 })
                }
            }
        })
        return data;
    },

    /**
     * 数据过滤
     * @param {any} arr
     * @param {any} code
     */
    filterData: function (arr, code) {
        if (code != null && code.length) {
            return arr.filter(x => code.indexOf(x.id) == 0 || x.id.indexOf(code) == 0)
        }
        return arr;
    },

    /**
     * 抓取目录
     * @param {any} url
     */
    grabIndex: function (url) {
        nrSpider.requestCache(url, null, sz.config.fileName).then(html => {

            var data = [];
            //解析
            var vdom = nrSpider.parsingHtml(html, url), oi = 1;
            vdom.querySelectorAll('.provincetr a').forEach(a => {
                var href = nrSpider.parsingAttr(a, 'href');
                var sid = href.split('/').pop().split('.')[0];
                //填充数据
                data.push({ id: sid, txt: a.innerText.trim(), pid: "0", sid: sid, spid: "0", num: oi++, url: url, suburl: href, leaf: 0, deep: 1 })
            })

            sz.dataIndex = sz.dataIndex.concat(data);

            nrSpider.noticeVoice('目录解析完成');

            //过滤数据加入队列
            var fdata = sz.filterData(sz.dataIndex, sz.config.filterCode);
            nrSpider.queueAdd(fdata);

            //运行队列
            sz.queueDo();
        })
    },

    /**
     * 抓取列表
     * @param {any} item
     */
    grab: function (item) {
        if (!item.suburl) {
            return
        }

        nrSpider.requestCache(item.suburl, null, sz.config.fileName).then(res => {
            //空内容
            if (res == "") {
                sz.dataCatch.push({ item, error: "is empty" });
                return;
            }

            var data = sz.matchResult(res, item);

            //得到数据
            sz.dataResult = sz.dataResult.concat(data);

            //加入队列（过滤数据）
            if (data.length && item.deep < sz.config.maxDeep - 1) {
                var fdata = sz.filterData(data, sz.config.filterCode);
                nrSpider.queueAdd(fdata);
            }
        }).catch(err => {
            sz.dataCatch.push({ item, error: err + "" });
        });
    },

    /**
     * 去除末尾0
     * @param {any} code
     */
    trimEndZero: function (code) {
        while (code[code.length - 1] == "0") {
            code = code.substring(0, code.length - 2);
        }
        return code;
    },

    //抓取境外（港澳台） https://lbs.qq.com/service/webService/webServiceGuide/webServiceDistrict
    //获取全部行政区划数据另存为线上的 districtJson 链接
    // CORS ERROR: The request client is not a secure context and the resource is in more-private address space local
    // chrome://flags => Block insecure private network requests => default => Disabled
    grabOutland: function () {
        nrSpider.noticeVoice('境外（港澳台）处理中');
        return new Promise((resolve) => {
            var districtJson = "https://raw.githubusercontent.com/netnr/test/main/tmp/district-20211103.json";
            fetch(districtJson).then(x => x.json()).then(res => {
                console.debug(res);

                var okey = ["71", "81", "82"], ris0 = [], ris1 = [], ris2 = [];

                //删除已添加的数据
                for (var i = sz.dataIndex.length - 1; i >= 0; i--) {
                    if (okey.indexOf(sz.dataIndex[i].id) >= 0) {
                        sz.dataIndex.splice(i, 1);
                    }
                }
                for (var i = sz.dataResult.length - 1; i >= 0; i--) {
                    if (okey.indexOf(sz.dataResult[i].id.substring(0, 2)) >= 0) {
                        sz.dataResult.splice(i, 1);
                    }
                }

                ris0 = res.result[0].filter(x => okey.indexOf((x.id + "").substring(0, 2)) >= 0);
                var oi = sz.dataIndex.length;
                ris0.forEach(x => {
                    var sid = (x.id + "").substring(0, 2)
                    sz.dataIndex.push({ id: sid, txt: x.fullname, pid: "0", sid: sid, spid: "0", num: oi++, leaf: 0, deep: 1 });
                });

                if (sz.config.maxDeep >= 2) {
                    ris1 = res.result[1].filter(x => okey.indexOf((x.id + "").substring(0, 2)) >= 0);
                    oi = 1;
                    ris1.forEach(x => {
                        var spid = (x.id + "").substring(0, 2);
                        var sid = (x.id + "").substring(0, spid == "71" ? 4 : 6);
                        sz.dataResult.push({ id: sid.padEnd(12, '0'), txt: x.fullname, pid: spid, sid: sid, spid: spid, num: oi++, leaf: spid == "71" ? 0 : 1, deep: 2 })
                    });
                }
                if (sz.config.maxDeep >= 3) {
                    ris2 = res.result[2].filter(x => (x.id + "").substring(0, 2) == "71");
                    oi = 1;
                    ris2.forEach(x => {
                        var sid = x.id + "";
                        var spid = sid.substring(0, 4);
                        sz.dataResult.push({ id: sid.padEnd(12, '0'), txt: x.fullname, pid: spid.padEnd(12, '0'), sid: sid, spid: spid, num: oi++, leaf: 1, deep: 3 })
                    });
                }

                nrSpider.noticeVoice("境外（港澳台）处理完成");
                resolve();
            })
        })
    },

    //异常记录重新抓取
    grabCatch: function () {
        sz.dataCatch.forEach(cdi => {
            sz.grab(cdi.item);
        })
    },

    //重新抓取异常后刷新
    refreshCatch: function () {
        for (var i = 0; i < sz.dataCatch.length; i++) {
            var item = sz.dataCatch[i].item;

            if (sz.dataResult.filter(x => x.id == item.id).length) {
                sz.dataCatch.splice(i, 1)
            }
        }

        nrSpider.noticeVoice(`刷新异常数据完成，还剩 ${sz.dataCatch.length} 条`);
    },

    //构建打包数据
    zipReady: function () {
        nrSpider.noticeVoice("开始构建打包数据")

        //目录、子数据、总数据
        var indexs = [], sdata = {}, data = [];
        sz.dataIndex.forEach(di => {
            var obj = { id: di.id, txt: di.txt, pid: di.pid, sid: di.sid, spid: di.spid, num: di.num, leaf: di.leaf, deep: di.deep };
            indexs.push(obj);
        })
        //填充总数据、子数据
        data = data.concat(indexs);
        sz.dataResult.forEach(di => {
            var obj = { id: di.id, txt: di.txt, pid: di.pid, sid: di.sid, spid: di.spid, num: di.num, leaf: di.leaf, deep: di.deep };
            if (di.ct) {
                obj.ct = di.ct;
            }
            data.push(obj);

            if (di.deep <= 3) {
                indexs.push(obj);
            }
            if (di.deep == 2) {
                sdata[di.sid] = [];
            }
        });
        for (var i in sdata) {
            sdata[i] = data.filter(x => x.deep > 3 && x.id.startsWith(i));
        }

        sz.zipData = { indexs, sdata, data };

        nrSpider.noticeVoice("构建数据完成")
    },

    /**
     * 下载
     */
    zip: function (type) {
        var zip = new JSZip();

        nrSpider.noticeVoice(`下载 ${type}，构建中...`)

        switch (type) {
            case "json-all":
                {
                    //导出总数据
                    zip.file(`${sz.config.fileName}-${sz.config.maxDeep}.json`, JSON.stringify(sz.zipData.data));

                    zip.generateAsync({ type: "blob" }).then(function (content) {
                        saveAs(content, `${sz.config.fileName}-${type}-${sz.config.maxDeep}.zip`);
                    });
                }
                break;
            case "json-split":
                {
                    //导出目录
                    zip.file(`0.json`, JSON.stringify(sz.zipData.indexs));

                    //导出子数据
                    for (var i in sz.zipData.sdata) {
                        var idata = sz.zipData.sdata[i];
                        if (idata.length) {
                            zip.file(`${i}.json`, JSON.stringify(idata));
                        }
                    }

                    //导出异常
                    if (sz.dataCatch.length) {
                        zip.file(`catch-${sz.config.maxDeep}.json`, JSON.stringify(sz.dataCatch));
                    }

                    zip.generateAsync({ type: "blob" }).then(function (content) {
                        saveAs(content, `${sz.config.fileName}-${type}-${sz.config.maxDeep}.zip`);
                    });
                }
                break;
            case "sql":
                nrSpider.arrayToSQL(sz.zipData.data, sz.config.fileName.replaceAll('-', '_'), { id: "text primary key", txt: "text not null", pid: "text not null", sid: "text unique not null", spid: "text not null", ct: "text", num: "int not null", leaf: "int not null", deep: "int not null" }).then(esql => {
                    zip.file(`${sz.config.fileName}-${sz.config.maxDeep}.sql`, esql);

                    zip.generateAsync({ type: "blob" }).then(function (content) {
                        saveAs(content, `${sz.config.fileName}-${type}-${sz.config.maxDeep}.zip`);
                    });
                });
                break;
            case "excel":
                if (sz.config.maxDeep == 5) {
                    nrSpider.noticeVoice("5 级数量近 70万，导出 Excel 浏览器可能会崩溃，建议通过 SQLite 导出 Excel 文件");
                }
                nrSpider.arrayToExcel(sz.zipData.data).then(function (content) {
                    saveAs(content, `${sz.config.fileName}-${sz.config.maxDeep}.xlsx`);
                });
                break;
            default:
                nrSpider.noticeVoice("type Optional: sql、json-split、json-all")
        }
    },

    //运行
    queueDo: function () {
        //执行任务
        nrSpider.taskFuse(() => {
            var task = nrSpider.queueUse();
            if (task) {
                task.forEach(x => sz.grab(x));
                nrSpider.consoleFuse(`Rows: ${sz.dataResult.length} ，Queue: ${nrSpider.queueList.length} ，Request: ${nrSpider.countRequest} ，Catch: ${sz.dataCatch.length}`, 'process');
            }
        });

        //结束
        clearInterval(nrSpider.defer.endQueue);
        nrSpider.defer.endQueue = setInterval(() => {
            if (nrSpider.queueList.length == 0) {
                clearInterval(nrSpider.defer.task);
                clearInterval(nrSpider.defer.endQueue);

                nrSpider.noticeVoice("队列为空，已自动停止任务");
                console.table({
                    start: new Date(sz.startTime).toLocaleString(),
                    now: new Date().toLocaleString()
                });

                sz.grabOutland().then(() => {
                    sz.zipReady();
                    sz.zip('json-all')
                })
            }
        }, 5000);
    },

    //开始
    start: function () {
        sz.startTime = new Date().valueOf();

        nrSpider.init().then(() => {
            //抓取写入队列
            sz.grabIndex(sz.config.indexList);
        });
    }
};

//参数配置
sz.config = {
    //下载名称：统计用区划和城乡划分代码 http://www.stats.gov.cn/tjsj/tjbz/tjyqhdmhcxhfdm/
    fileName: "stats-zoning",

    //索引目录
    indexList: "http://www.stats.gov.cn/tjsj/tjbz/tjyqhdmhcxhfdm/2021/index.html",
    //最大深度（1-5）
    maxDeep: 5,
    //抓指定编码，为空时抓所有
    filterCode: "", //
};

//下载zip，抓取完成后
//sz.zip('json-split')
//sz.zip('json-all')
//sz.zip('excel')
//sz.zip('sql')

//sz.grabCatch() //抓取异常
//sz.refreshCatch() //抓取异常后刷新
//localforage.clear() //清空本地数据库（不再用时删除）

sz.start(); //开始