import { nrcBase } from "../../../../frame/nrcBase";
import { nrVary } from "../../nrVary";
import { nrApp } from "../../../../frame/Bootstrap/nrApp";

let nrPage = {
    pathname: "/ss/speedtest",

    init: async () => {
        nrPage.speedSource.forEach(si => {
            let domItem = document.createElement("option");
            domItem.value = si.link;
            domItem.innerHTML = si.name;

            nrVary.domSeServer.appendChild(domItem);
        });
        nrVary.domSeServer.classList.remove('d-none');

        nrPage.bindEvent();
    },

    //测试时长
    time: 1000 * 12,
    //开始时间
    start: null,
    //下载大小
    downsize: 0,
    //源索引
    urindex: -1,
    //源标识
    urid: 0,
    //连接数.
    conn: 0,
    //允许连接数
    maxconn: 10,
    //结果
    result: [],
    //数据项
    data: [],
    speeds: [],

    //测速节点
    speedSource: [
        {
            name: "七牛 CDN",
            ratio: 1126 / 10578,
            link: "https://cdn.staticfile.org/sql.js/1.2.2/dist/sql-asm-debug.js"
        },
        {
            name: "字节跳动 CDN",
            ratio: 1128 / 11201,
            link: "https://lf3-cdn-tos.bytecdntp.com/cdn/expire-1-y/sql.js/1.6.1/sql-asm-debug.js"
        },
        {
            name: "百度 CDN",
            ratio: 1536 / 10578,
            link: "https://code.bdstatic.com/npm/sql.js@1.2.2/dist/sql-asm-debug.js"
        }
    ],

    bindEvent: () => {
        //开始
        nrVary.domBtnStart.addEventListener('click', function () {
            nrApp.setLoading(this);

            nrPage.speeds = [];

            nrPage.downsize = 0;
            nrPage.urindex = -1;
            nrPage.urid = 0;
            nrPage.conn = 0;
            nrPage.result = [];
            nrPage.start = new Date().valueOf();

            nrVary.domCardResult.innerHTML = "";
            nrVary.domBtnStart.loading = true;

            nrPage.addConnection();
        });
    },

    //压缩率（根据开发环境的测试结果计算所得）
    ratio: function () {
        let ss = nrVary.domSeServer.value;
        let cr = nrPage.speedSource.filter(x => x.link == ss)[0].ratio;
        return cr;
    },

    /**
     * 获取请求的源
     * @returns 
     */
    getUri: () => `${nrVary.domSeServer.value}?${nrcBase.random()}`,

    addConnection: function () {
        let si1 = setInterval(function () {
            if (nrPage.conn < nrPage.maxconn) {
                nrPage.conn++;
                let xhr = new XMLHttpRequest();
                xhr.open('GET', nrPage.getUri());
                let urid = nrPage.urid++;
                xhr.onprogress = function (event) {
                    nrPage.result[urid] = event.loaded;
                };
                xhr.onreadystatechange = function () {
                    if (xhr.readyState == 4) {
                        nrPage.conn--;
                    }
                }
                window.setTimeout(function () {
                    xhr.abort()
                }, nrPage.time);
                xhr.send();
            }
        }, 50);

        //统计
        let si2 = setInterval(function () {
            let now = new Date().valueOf(), ds = 0;
            for (let j = 0; j < nrPage.result.length; j++) {
                let ri = nrPage.result[j] * 1;
                ds += (ri || 0);
            }
            nrPage.downsize = ds;
            let size = nrPage.downsize * nrPage.ratio();
            let time = (now - nrPage.start) / 1000;
            let speed = (size / time).toFixed(2);
            nrPage.data.push(speed);

            //过程处理
            let dom = document.createElement('div');
            dom.innerHTML = `${nrcBase.formatByteSize(speed)}/s`;
            nrVary.domCardResult.appendChild(dom);
            nrPage.speeds.push(speed);

            //结束
            if (now - nrPage.start > nrPage.time) {
                window.clearInterval(si1);
                window.clearInterval(si2);

                //完成
                let skip3 = nrPage.speeds.length > 4 ? nrPage.speeds.slice(nrPage.speeds.length - 3) : nrPage.speeds;

                let sum = 0;
                skip3.map(x => sum += x * 1);
                let avg = (sum / skip3.length);

                let dom = document.createElement('div');
                dom.innerHTML = `<h2>平均：${nrcBase.formatByteSize(avg)}/s</h2>`;
                nrVary.domCardResult.insertBefore(dom, nrVary.domCardResult.children[0]);

                nrApp.setLoading(nrVary.domBtnStart, true);
            }
        }, 900)
    }
}

export { nrPage };