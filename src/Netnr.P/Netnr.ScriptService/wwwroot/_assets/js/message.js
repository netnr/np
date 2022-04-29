AV.init({
    appId: "86m0YudK96J2iAcOSjbOMhFt-gzGzoHsz",
    appKey: "nea9RxxRgGVNd1xAgOf4biXa",
    serverURL: "https://lc.netnr.com"
});

var mg = {
    page: 1,
    pageNumber: 999,
    messageObjectSave: function () { return new mg.MessageObject() },
    list: function () {
        ss.loading();

        var query = new AV.Query("NetnrMessage");
        query.limit(mg.pageNumber);
        query.skip((mg.page - 1) * mg.pageNumber);
        query.find().then(res => {
            ss.loading(0);
            var htm = [];

            $(res.map(x => x.attributes)).each(function (i) {
                var id = 'mi' + (i + 1);
                var nickname = ss.htmlEncode(this.nickname == "" ? "guest" : this.nickname);
                var context = '<p><em class="badge bg-secondary" title="该信息已被屏蔽">block</em></p>'
                if (!this.block) {
                    context = netnrmd.render(netnrmd.spacing(this.message)).replace(/@\S+/g, function (n) {
                        return '<span class="text-primary">' + n + '</span>'

                    }).replace(/#\d+/g, function (n) {
                        return '<a href="' + n.replace("#", "#mi") + '" class="text-warning">' + n + '</a>'
                    });
                }

                var itemtmp = `
                                <div class="d-flex mb-2" id="${id}">
                                    <div class="flex-shrink-0 pt-2">
                                        ${iisvg({ value: nickname, size: 42 }).outerHTML}
                                    </div>
                                    <div class="flex-grow-1 ms-2">
                                        <a class="text-primary text-decoration-none" role="button" onclick="mg.refTA(this)">${nickname}</a>
                                        <small class="text-muted mx-3">${new Date(this.created).toLocaleString()}</small>
                                        <a class="text-warning" href="#${id}" role="button">#${i + 1}</a>
                                        <div class="text-break mt-2">
                                            ${context}
                                        </div>
                                    </div>
                                </div>`;

                htm.push(itemtmp);
            });
            if (htm.length) {
                $('.messagebox').html(htm.join(''));
            } else {
                $('#messagebox').html('no message');
            }
        }).catch(err => {
            bs.alert("<h4>查询失败</h4>");
            console.log(err);
            ss.loading(0);
        });
    },
    refTA: function (that) {
        var txt = $('textarea[name="Content"]');
        txt.val("@" + that.innerHTML + " " + txt.val());
    }
}

mg.list();
$('textarea[name="Content"]').keydown(function (e) {
    e = e || window.event;
    var keys = e.keyCode || e.which || e.charCode;
    if (keys == 13 && e.ctrlKey) {
        $('#btnSave')[0].click();
    }
});
$('#btnSave').click(function () {
    var un = $('input[name="UserNickname"]'), uc = $('textarea[name="Content"]'), objv = {};
    objv.nickname = un.val().trim();
    objv.message = uc.val().substring(0, 9999);
    if (objv.message == "" || netnrmd.render(objv.message) == "") {
        bs.msg("<h4>请输入内容</h4>");
    } else {
        const NetnrMessage = AV.Object.extend('NetnrMessage');
        const tableObj = new NetnrMessage();
        tableObj.set('block', 0);
        tableObj.set('created', new Date());
        for (var i in objv) {
            tableObj.set(i, objv[i]);
        }
        tableObj.save().then(res => {
            bs.msg("<h4>操作成功</h4>");
            uc.val('');
            mg.list();
            ss.ls["nickname"] = objv.nickname;
            ss.lsSave();
        }).catch(err => {
            bs.alert("<h4>查询失败</h4>");
            console.log(err);
            ss.loading(0);
        });
    }
});

$('input[name="UserNickname"]').val(ss.lsStr("nickname"));