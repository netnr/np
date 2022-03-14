ss.bmobInit();
var mg = {
    page: 1,
    pageNumber: 999,
    bquery: Bmob.Query("NetnrMessage"),
    messageObjectSave: function () { return new mg.MessageObject() },
    list: function () {
        ss.loading();
        var query = mg.bquery;
        query.limit(mg.pageNumber);
        query.skip((mg.page - 1) * mg.pageNumber);
        query.find().then(res => {
            ss.loading(0);
            var htm = [];
            $(res).each(function (i) {
                var id = 'mi' + (i + 1);
                var nickname = ss.htmlEncode(this.UserNickname == "" ? "guest" : this.UserNickname);
                var context = '<p><em class="badge bg-secondary" title="该信息已被屏蔽">block</em></p>'
                if (!this.IsBlock) {
                    context = netnrmd.render(netnrmd.spacing(this.Content)).replace(/@\S+/g, function (n) {
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
                                        <small class="text-muted mx-3">${this.createdAt}</small>
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
    objv.UserNickname = un.val().trim();
    objv.Content = uc.val().substring(0, 9999);
    if (objv.Content == "" || netnrmd.render(objv.Content) == "") {
        bs.msg("<h4>请输入内容</h4>");
    } else {
        var query = mg.bquery;
        for (var i in objv) {
            query.set(i, objv[i]);
        }
        query.save().then(res => {
            bs.msg("<h4>操作成功</h4>");
            uc.val('');
            mg.list();
            ss.ls["nickname"] = objv.UserNickname;
            ss.lsSave();
        }).catch(err => {
            bs.alert("<h4>查询失败</h4>");
            console.log(err);
            ss.loading(0);
        });
    }
});

$('input[name="UserNickname"]').val(ss.lsStr("nickname"));