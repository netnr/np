nr.onReady = function () {
    sp.bindSpeedServer();

    nr.domBtnStart.addEventListener('click', function () {
        var sps = [];
        //过程
        sp.progress = function (speed) {
            var dom = document.createElement('div');
            dom.innerHTML = `${nr.formatByteSize(speed)}/s`;
            nr.domCardResult.appendChild(dom);
            sps.push(speed);
        };
        //完成
        sp.complete = function () {
            var sum = 0;
            sps.map(x => sum += x * 1);
            var avg = (sum / sps.length);

            var dom = document.createElement('div');
            dom.innerHTML = `<h2>平均：${nr.formatByteSize(avg)}/s</h2>`;
            nr.domCardResult.insertBefore(dom, nr.domCardResult.children[0]);

            nr.domBtnStart.loading = false;
        };
        sp.run();
    });
}

var sp = {
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
    //压缩率（根据开发环境的测试结果计算所得）
    cr: function () {
        var ss = nr.domSeServer.value, cr = 1;
        var cr = sp.ss.filter(x => x.link == ss)[0].cr;
        return cr;
    },
    //测速节点
    ss: [
        {
            name: "百度 CDN",
            cr: 1331 / 10578,
            link: "https://npm.elemecdn.com/sql.js@1.2.2/dist/sql-asm-debug.js"
        },
        {
            name: "七牛 CDN",
            cr: 1126 / 10578,
            link: "https://cdn.staticfile.org/sql.js/1.2.2/dist/sql-asm-debug.js"
        },
        {
            name: "字节跳动 CDN",
            cr: 1126 / 10578,
            link: "https://s1.pstatp.com/cdn/expire-1-M/sql.js/1.2.2/dist/sql-asm-debug.js"
        }
    ],
    //绑定节点
    bindSpeedServer: function () {
        var htm = [];
        sp.ss.forEach(si => {
            htm.push(`<sl-menu-item value="${si.link}">${si.name}</sl-menu-item>`);
        })
        nr.domSeServer.innerHTML = htm.join('');
        nr.domSeServer.value = sp.ss[0].link;
    },
    //获取请求的源
    geturi: function () {
        return nr.domSeServer.value + "?" + Math.random();
    },
    addconn: function () {
        var si1 = setInterval(function () {
            if (sp.conn < sp.maxconn) {
                sp.conn++;
                var xhr = new XMLHttpRequest();
                xhr.open('GET', sp.geturi());
                var urid = sp.urid++;
                xhr.onprogress = function (event) {
                    sp.result[urid] = event.loaded;
                };
                xhr.onreadystatechange = function () {
                    if (xhr.readyState == 4) {
                        sp.conn--;
                    }
                }
                window.setTimeout(function () {
                    xhr.abort()
                }, sp.time);
                xhr.send();
            }
        }, 50);

        //统计
        var si2 = setInterval(function () {
            var now = new Date().valueOf(), ds = 0;
            for (var j = 0; j < sp.result.length; j++) {
                var ri = sp.result[j] * 1;
                ds += (ri || 0);
            }
            sp.downsize = ds;
            var size = sp.downsize * sp.cr();
            var time = (now - sp.start) / 1000;
            var speed = (size / time).toFixed(2);
            sp.data.push(speed);

            //回调
            sp.progress && sp.progress(speed);

            //结束
            if (now - sp.start > sp.time) {
                window.clearInterval(si1);
                window.clearInterval(si2);
                //完成
                sp.complete && sp.complete();
            }
        }, 900)
    },
    run: function () {
        sp.downsize = 0;
        sp.urindex = -1;
        sp.urid = 0;
        sp.conn = 0;
        sp.result = [];
        sp.start = new Date().valueOf();

        nr.domCardResult.innerHTML = "";
        nr.domBtnStart.loading = true;

        sp.addconn();
    }
}