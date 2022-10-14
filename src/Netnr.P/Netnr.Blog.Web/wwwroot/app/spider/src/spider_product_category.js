var spc = {

    //版本号
    version: "1.0.0",
    date: "2010-06-17",

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
            var aa = tr.getElementsByTagName('a');
            if (aa.length) {
                data.push({ id: aa[0].innerHTML.trim(), txt: aa[1].innerHTML.trim(), pid: item.id, num: oi++, url: item.suburl, suburl: nrSpider.parsingAttr(aa[0], 'href'), deep: item.deep + 1 })
            } else {
                //中间级别无下级 http://www.stats.gov.cn/tjsj/tjbz/tjypflml/2010/15.html
                var tds = tr.getElementsByTagName('td');
                data.push({ id: tds[0].innerHTML.trim(), txt: tds[1].innerHTML.trim(), pid: item.id, num: oi++, url: item.suburl, deep: item.deep + 1 })
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
     * @param {any} list
     */
    grabIndex: function (list) {
        var parr = [];

        list.forEach(url => {
            parr.push(new Promise((resolve) => {

                nrSpider.requestCache(url, 'utf-8', spc.config.fileName).then(html => {

                    var data = [];
                    //解析
                    var vdom = nrSpider.parsingHtml(html, url), oi = 1;
                    vdom.querySelectorAll('.cont_tit').forEach(a => {
                        var afont = a.getElementsByTagName('font')[0].innerHTML.split('-');
                        //填充数据
                        data.push({ id: afont[0].trim(), txt: afont[1].trim(), pid: "0", num: oi++, url: url, suburl: nrSpider.parsingAttr(a.parentElement, 'href'), deep: 1 })
                    })

                    spc.dataIndex = spc.dataIndex.concat(data);
                    resolve(data);

                })
            }))
        });

        Promise.all(parr).then(() => {
            nrSpider.noticeVoice("目录解析完成");

            //修复目录排序
            spc.dataIndex.sort(function (a, b) { return a.id - b.id });
            for (var i = 0; i < spc.dataIndex.length; i++) {
                spc.dataIndex[i].num = i + 1;
            }

            //过滤数据加入队列
            var fdata = spc.filterData(spc.dataIndex, spc.config.filterCode);
            nrSpider.queueAdd(fdata);

            //运行队列
            spc.queueDo();
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

        nrSpider.requestCache(item.suburl, 'GBK', spc.config.fileName).then(res => {
            //空内容
            if (res == "") {
                spc.dataCatch.push({ item, error: "is empty" });
                return;
            }

            var data = spc.matchResult(res, item);

            //得到数据
            spc.dataResult = spc.dataResult.concat(data);

            //加入队列（过滤数据）
            if (data.length && item.deep < spc.config.maxDeep - 1) {
                var fdata = spc.filterData(data, spc.config.filterCode);
                nrSpider.queueAdd(fdata);
            }
        }).catch(err => {
            spc.dataCatch.push({ item, error: err + "" });
        });
    },

    //异常记录重新抓取
    grabCatch: function () {
        spc.dataCatch.forEach(cdi => {
            spc.grab(cdi.item);
        })
    },

    //重新抓取异常后刷新
    refreshCatch: function () {
        for (var i = 0; i < spc.dataCatch.length; i++) {
            var item = spc.dataCatch[i].item;

            if (spc.dataResult.filter(x => x.id == item.id).length) {
                spc.dataCatch.splice(i, 1)
            }
        }
    },

    /**
     * 下载
     * @param {boolean} distinct 去重
     */
    zip: function (distinct) {
        nrSpider.noticeVoice("开始下载，构建数据中...")

        var zip = new JSZip();

        //目录、子数据、总数据
        var indexs = [], sdata = {}, data = [];
        spc.dataIndex.forEach(di => {
            var obj = { id: di.id, txt: di.txt, pid: di.pid, num: di.num, deep: di.deep };
            indexs.push(obj);
            sdata[di.id] = [];
        })
        //填充总数据、子数据
        data = data.concat(indexs);
        spc.dataResult.forEach(di => {
            var obj = { id: di.id, txt: di.txt, pid: di.pid, num: di.num, deep: di.deep };
            data.push(obj);

            var sk = obj.id.substring(0, 2);
            sdata[sk].push(obj);
        });

        //去除重复的编码
        if (distinct) {
            nrSpider.noticeVoice("正在数据去重")

            if (spc.config.maxDeep > 2) {
                for (var i = data.length - 1; i >= 0; i--) {
                    var di = data[i];
                    if (di.deep > 2 && data.filter(x => x.id == di.id && x.deep < di.deep).length) {
                        data.splice(i, 1)
                    }
                }
                for (var s in sdata) {
                    var sitem = sdata[s];
                    for (var i = sitem.length - 1; i >= 0; i--) {
                        var di = sitem[i];
                        if (di.deep > 2 && sitem.filter(x => x.id == di.id && x.deep < di.deep).length) {
                            sitem.splice(i, 1)
                        }
                    }
                }
            }
        }

        nrSpider.noticeVoice("正在写入 JSON")

        //导出总数据
        zip.file(`${spc.config.fileName}-${spc.config.maxDeep}.json`, JSON.stringify(data));

        //导出目录
        zip.file(`0.json`, JSON.stringify(indexs));

        //导出子数据
        for (var i in sdata) {
            zip.file(`${i}.json`, JSON.stringify(sdata[i]));
        }

        //导出异常
        if (spc.dataCatch.length) {
            zip.file(`catch-${spc.config.maxDeep}.json`, JSON.stringify(spc.dataCatch));
        }

        nrSpider.noticeVoice("正在生成 Excel")
        nrSpider.arrayToExcel(data, { id: 100, txt: 400, pid: 100, num: 50, deep: 50 }).then(blobExcel => {
            zip.file(`${spc.config.fileName}-${spc.config.maxDeep}.xlsx`, blobExcel);

            nrSpider.noticeVoice("正在生成 SQL")
            nrSpider.arrayToSQL(data, spc.config.fileName.replaceAll('-', '_'), { id: "text primary key", txt: "text not null", pid: "text not null", num: "int not null", deep: "int" }).then(esql => {
                zip.file(`${spc.config.fileName}-${spc.config.maxDeep}.sql`, esql);

                nrSpider.noticeVoice("正在打包 ZIP")
                zip.generateAsync({ type: "blob" }).then(function (content) {
                    saveAs(content, `${spc.config.fileName}-${spc.config.maxDeep}.zip`);
                });
            });
        });
    },

    //运行
    queueDo: function () {
        //执行任务
        nrSpider.taskFuse(() => {
            var task = nrSpider.queueUse();
            if (task) {
                task.forEach(x => spc.grab(x));
                nrSpider.consoleFuse(`Rows: ${spc.dataResult.length} ，Queue: ${nrSpider.queueList.length} ，Request: ${nrSpider.countRequest} ，Catch: ${spc.dataCatch.length}`, 'process');
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
                    start: new Date(spc.startTime).toLocaleString(),
                    now: new Date().toLocaleString()
                });

                spc.zip(1);
            }
        }, 5000);
    },

    //开始
    start: function () {
        spc.startTime = new Date().valueOf();

        nrSpider.init().then(() => {
            //抓取写入队列
            spc.grabIndex(spc.config.indexList);
        });
    }
};

//参数配置
spc.config = {
    //下载名称：统计用产品分类目录 http://www.stats.gov.cn/tjsj/tjbz/tjypflml/index.html
    fileName: "stats-product-category",

    //索引目录
    indexList: [
        "http://www.stats.gov.cn/tjsj/tjbz/tjypflml/index.html",
        "http://www.stats.gov.cn/tjsj/tjbz/tjypflml/index_1.html",
        "http://www.stats.gov.cn/tjsj/tjbz/tjypflml/index_2.html",
        "http://www.stats.gov.cn/tjsj/tjbz/tjypflml/index_3.html",
        "http://www.stats.gov.cn/tjsj/tjbz/tjypflml/index_4.html"
    ],
    //最大深度（1-5）
    maxDeep: 3,
    //抓指定编码，为空时抓所有
    filterCode: ""
};

//spc.zip(1); //去重下载zip，抓取完成后
//spc.grabCatch() //抓取异常
//spc.refreshCatch() //抓取异常后刷新
//localforage.clear() //清空本地数据库（不再用时删除）

spc.start(); //开始