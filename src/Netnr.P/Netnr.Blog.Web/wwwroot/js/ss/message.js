var baas = {
    baseUrl: "https://lc.netnr.com/1.1",
    appId: "86m0YudK96J2iAcOSjbOMhFt-gzGzoHsz",
    appKey: "nea9RxxRgGVNd1xAgOf4biXa",
    fetch: async (url, ops) => {
        Object.assign(ops, {
            headers: {
                "X-LC-Id": baas.appId,
                "X-LC-Key": baas.appKey,
                "Content-Type": "application/json"
            }
        });
        ops.method = ops.method || "POST";
        var result = await fetch(url, ops).then(resp => resp.json());
        return result;
    },
    /**
     * 对象操作
     * @param {any} requests 数据，一个或数组
     */
    objBatch: async (requests) => {
        var requests = requests;
        if (baas.type(requests) != "Array") {
            requests = [requests];
        }

        var result = await baas.fetch(`${baas.baseUrl}/batch`, {
            body: JSON.stringify({ requests })
        });
        return result;
    },
    /**
     * 对象查询
     * @param {any} tableName 表名
     * @param {any} jsonArgs 参数
     */
    objQuery: async (tableName, jsonArgs) => {
        var url = `${baas.baseUrl}/classes/${tableName}?${new URLSearchParams(jsonArgs)}`;
        var result = await baas.fetch(url, { method: "GET" });
        return result.results ? result.results : result;
    },
    type: (obj) => {
        var tv = {}.toString.call(obj);
        return tv.split(' ')[1].replace(']', '');
    },
    /**
     * 日期包裹
     * @param {any} date
     */
    wrapDate: (date) => {
        if (date && date.iso) {
            return new Date(date.iso).toLocaleString()
        } else {
            return { __type: "Date", iso: date || new Date() }
        }
    },
    /** 新ID */
    getId: () => `${Date.now()}${Math.random().toString().slice(-4)}` * 1,
    /**
     * 通知
     * @param {any} content
     * @param {any} title
     */
    notice: (content, title) => {
        var fd = new FormData();
        fd.append("title", title || "留言（SS）");
        fd.append("content", content);
        fetch("https://www.netnr.com/api/v1/Push", { method: 'POST', body: fd });
    },
    testObj: async () => {
        var result = baas.objBatch([
            {
                method: "POST",
                path: "/1.1/classes/tableName",
                body: {
                    nr_id: Date.now(),
                    nr_create: baas.wrapDate(),
                    nr_status: 2
                }
            }
        ]);
        return result;
    }
}

nr.onReady = function () {
    nr.domTxtNickname.value = nr.lsStr("nickname");
    nr.domBtnReply.addEventListener('click', page.reply);

    page.list();
}

var page = {
    list: async (toBottom) => {
        try {
            ss.loading(true);
            var data = await baas.objQuery("netnr_message", { order: "nr_create", limit: 1000 });
            ss.loading(false);
            nr.domCardBox.classList.remove('invisible');

            var htm = [];
            for (let index = 0; index < data.length; index++) {
                var row = data[index];

                var id = 'mi' + (index + 1);
                var nickname = nr.htmlEncode(row.nr_name == "" ? "guest" : row.nr_name);
                var context = '<p><em title="该信息已被屏蔽">Block</em></p>'
                if (row.nr_status == 1) {
                    context = netnrmd.render(netnrmd.pangu.spacing(row.nr_content)).replace(/@\S+/g, function (n) {
                        return '<a class="text-decoration-none">' + n + '</a>'
                    }).replace(/#\d+/g, function (n) {
                        return '<a href="' + n.replace("#", "#mi") + '" class="text-warning">' + n + '</a>'
                    });
                }
                var itemtmp = `<div class="d-flex mb-2" id="${id}">
                    <div class="mt-1">${iisvg({ value: nickname, size: 42 }).outerHTML}</div>
                    <div class="ms-2">
                        <a class="text-decoration-none" role="button" onclick="page.refTA(this)">${nickname}</a>
                        <small class="opacity-75 mx-2">${baas.wrapDate(row.nr_create)}</small>
                        <a class="text-warning" href="#${id}" role="button">#${index + 1}</a>
                        <div class="text-break mt-2">${context}</div>
                    </div>
                </div>`;

                htm.push(itemtmp);
            }

            if (htm.length) {
                nr.domCardMessage.innerHTML = htm.join("");
                if (toBottom) {
                    window.scrollTo(0, nr.domCardMessage.scrollHeight);
                }
            } else {
                nr.domCardMessage.innerHTML = htm.join("no message");
            }
        } catch (ex) {
            console.debug(ex);
            ss.loading(false);
            nr.alert("查询失败");
        }
    },
    reply: async () => {
        try {
            var nickname = nr.domTxtNickname.value.trim(), content = nr.domTxtMessage.value;

            if (content.trim() == "" || netnrmd.render(nickname) == "") {
                nr.alert("请输入内容");
            } else {
                ss.loading(true);
                nr.domBtnReply.loading = true;

                var result = await baas.objBatch([
                    {
                        method: "POST",
                        path: "/1.1/classes/netnr_message",
                        body: {
                            nr_id: baas.getId(),
                            nr_name: nickname,
                            nr_content: content,
                            nr_create: baas.wrapDate(),
                            nr_status: 1
                        }
                    }
                ]);

                ss.loading(false);
                nr.domBtnReply.loading = false;

                if (result.code) {
                    nr.alert("保存失败");
                } else {
                    nr.alert("保存成功");
                    baas.notice(content);
                    nr.domTxtMessage.value = "";

                    page.list();
                }
            }
        } catch (ex) {
            console.debug(ex);
            ss.loading(false);
            nr.alert("保存失败");
            nr.domBtnReply.loading = false;
        }
    },
    refTA: function (that) {
        nr.domTxtMessage.value = `@${that.innerText} ${nr.domTxtMessage.value}`;
    },
}