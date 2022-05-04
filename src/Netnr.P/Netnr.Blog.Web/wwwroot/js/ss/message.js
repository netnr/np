nr.onReady = function () {
    page.init();
    page.list();
}

var page = {
    pageNumber: 1,
    pageSize: 999,
    init: function () {
        AV.init({
            appId: "86m0YudK96J2iAcOSjbOMhFt-gzGzoHsz",
            appKey: "nea9RxxRgGVNd1xAgOf4biXa",
            serverURL: "https://lc.netnr.com"
        });

        nr.domTxtNickname.value = nr.lsStr("nickname");
        nr.domBtnReply.addEventListener('click', page.reply);
    },
    list: function (toBottom) {
        ss.loading(true);

        var query = new AV.Query("NetnrMessage");
        query.limit(page.pageSize);
        query.skip((page.pageNumber - 1) * page.pageSize);
        query.ascending('createdAt');
        query.find().then(res => {
            ss.loading(false);
            nr.domCardBox.classList.remove('invisible');

            var htm = [];
            for (let index = 0; index < res.length; index++) {
                var row = res[index].attributes;

                var id = 'mi' + (index + 1);
                var nickname = nr.htmlEncode(row.nickname == "" ? "guest" : row.nickname);
                var context = '<p><em title="该信息已被屏蔽">Block</em></p>'
                if (!row.block) {
                    context = netnrmd.render(netnrmd.spacing(row.message)).replace(/@\S+/g, function (n) {
                        return '<a class="text-decoration-none">' + n + '</a>'
                    }).replace(/#\d+/g, function (n) {
                        return '<a href="' + n.replace("#", "#mi") + '" class="text-warning">' + n + '</a>'
                    });
                }
                var itemtmp = `<div class="d-flex mb-2" id="${id}">
                    <div class="mt-1">${iisvg({ value: nickname, size: 42 }).outerHTML}</div>
                    <div class="ms-2">
                        <a class="text-decoration-none" role="button" onclick="page.refTA(this)">${nickname}</a>
                        <small class="opacity-75 mx-2">${row.created.toLocaleString()}</small>
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
        }).catch(ex => {
            console.debug(ex);
            ss.loading(false);
            nr.alert("查询失败");
        });
    },
    reply: function () {
        var obj = {
            nickname: nr.domTxtNickname.value,
            message: nr.domTxtMessage.value
        }

        if (obj.message.trim() == "" || netnrmd.render(obj.message) == "") {
            nr.alert("请输入内容");
        } else {
            ss.loading(true);
            nr.domBtnReply.loading = true;

            const NetnrMessage = AV.Object.extend('NetnrMessage');
            const tableObj = new NetnrMessage();
            tableObj.set('block', 0);
            tableObj.set('created', new Date());
            for (var i in obj) {
                tableObj.set(i, obj[i]);
            }
            tableObj.save().then(res => {
                ss.loading(false);
                nr.domBtnReply.loading = false;
                nr.alert("保存成功");
                nr.domTxtMessage.value = "";
                page.list(true);

                nr.ls["nickname"] = obj.nickname;
                nr.lsSave();
            }).catch(ex => {
                console.debug(ex);
                ss.loading(false);
                nr.domBtnReply.loading = false;
                nr.alert("保存失败");
            });
        }
    },
    refTA: function (that) {
        nr.domTxtMessage.value = `@${that.innerText} ${nr.domTxtMessage.value}`;
    }
}