var sic = {

    //版本号
    version: "1.0.0",
    date: "2019-07-16",

    //抓取数据
    dataResult: [],

    /**
     * 匹配结果
     * @param {any} html 内容
     * @param {any} item
     */
    matchResult: function (html, item) {
        var data = [];

        return data;
    },

    /**
     * 下载
     * @param {boolean} distinct 去重
     */
    zip: function (distinct) {        
        nrSpider.noticeVoice("开始下载，构建数据中...")

        var zip = new JSZip();

        nrSpider.noticeVoice("正在写入 JSON")

        //导出总数据
        zip.file(`${spc.config.fileName}-${spc.config.maxDeep}.json`, JSON.stringify(data));

        //导出目录
        zip.file(`0.json`, JSON.stringify(indexs));

        //导出子数据
        for (var i in sdata) {
            zip.file(`${i}.json`, JSON.stringify(sdata[i]));
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

    //开始
    start: function () {
        nrSpider.init().then(() => {
            nrSpider.pdfDocumentGet("http://www.stats.gov.cn/tjsj/tjbz/hyflbz/201905/P020190716349644060705.pdf").then(pdfDocument => {
                var pageCount = pdfDocument.numPages;
                pageCount = 10;
                for (let pageIndex = 10; pageIndex <= pageCount; pageIndex++) {
                    nrSpider.pdfPageGet(pdfDocument, pageIndex).then(pdfPage => {
                        nrSpider.pdfPageContentRows(pdfPage).then(rows => {
                            console.debug(rows.join(''));
                        })

                        nrSpider.pdfPageContent(pdfPage).then(items => {
                            console.debug(items);
                        })
                    })
                }
            })
        });
    }
};

//参数配置
sic.config = {
    //下载名称：国民经济行业分类 http://www.stats.gov.cn/tjsj/tjbz/hyflbz/
    fileName: "stats-industry-category"
};

//sic.zip(); //下载zip

sic.start(); //开始（未完成，解析 PDF 文件内容）