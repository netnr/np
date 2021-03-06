require(['vs/editor/editor.main'], function () {

    window.nmd = new netnrmd('#mdeditor', {
        //执行命令前回调
        cmdcallback: function (cmd) {
            if (cmd == "full") {
                if (nmd.obj.editor.classList.contains('netnrmd-fullscreen')) {
                    $('#ModalWrite').addClass('modal');
                } else {
                    $('#ModalWrite').removeClass('modal');
                }
            }
        }
    });

    if (location.pathname == "/home/write") {
        //高度沉底
        $(window).on('load resize', function () {
            var vh = $(window).height() - nmd.obj.container.getBoundingClientRect().top - 20;
            nmd.height(Math.max(100, vh));
        })
    }

    //快捷键
    nmd.obj.me.addCommand(monaco.KeyMod.CtrlCmd | monaco.KeyCode.KEY_S, function () {
        if (document.getElementById("btnSave")) {
            $('#btnSave')[0].click();
        } else {
            $('#btnSaveEdit')[0].click();
        }
    })
});

//标签
$.ajax({
    url: "/Home/TagSelect",
    dataType: 'json',
    success: function (data) {
        $('#tags').selectPage({
            showField: 'TagName',
            keyField: 'TagId',
            data: data,
            eSelect: function () {
                $('#tags').selectPageRefresh()
            },
            formatItem: function (node) {
                var fi = node.TagName;
                if (node.TagIcon) {
                    fi = '<img style="height:18px;margin-right:5px" onerror="this.src=\'/favicon.svg\';" src="/gs/static/tag/' + node.TagIcon + '" />' + fi;
                }
                return fi;
            },
            multiple: true,
            maxSelectLimit: 3,
            selectToCloseList: false
        });
    },
    error: function () {
        jz.alert("初始化标签失败");
    }
})

//保存（新增）
$('#btnSave').click(function () {
    var wtitle = $.trim($('#wtitle').val());
    var wcategory = $('#wcategory').val();
    var wcontentMd = nmd.getmd();
    var wcontent = nmd.gethtml();
    var tagids = $('#tags').val();
    var errmsg = [];
    if (wtitle == "") {
        errmsg.push("请输入 标题");
    }
    if (wcategory == "") {
        errmsg.push("请选择 分类");
    }
    if (tagids == "") {
        errmsg.push("请选择 标签");
    }
    if (wcontentMd.length < 20) {
        errmsg.push("多写一点内容哦");
    }
    if (errmsg.length > 0) {
        jz.alert(errmsg.join('<br/>'));
        return false;
    }

    $('#btnSave')[0].disabled = true;

    $.ajax({
        url: "/home/writesave",
        type: "post",
        data: {
            UwTitle: wtitle,
            UwCategory: wcategory,
            UwContent: wcontent,
            UwContentMd: wcontentMd,
            TagIds: tagids
        },
        dataType: 'json',
        success: function (data) {
            if (data.code == 200) {
                nmd.setmd('');
                location.href = "/home/list/" + data.data;
            } else {
                jz.msg(data.msg);
            }
        },
        error: function (ex) {
            if (ex.status == 401) {
                jz.msg("请登录");
            } else {
                jz.msg("网络错误");
            }
        },
        complete: function () {
            $('#btnSave')[0].disabled = false;
        }
    });
});


//保存（编辑）
$('#btnSaveEdit').click(function () {
    var wid = $(this).attr('data-id');
    var wtitle = $('#wtitle').val();
    var wcategory = $('#wcategory').val();
    var wcontentMd = nmd.getmd();
    var wcontent = nmd.gethtml();
    var tagids = $('#tags').val();

    var errmsg = [];
    if (wtitle == "") {
        errmsg.push("请输入 标题");
    }
    if (wcategory == "") {
        errmsg.push("请选择 分类");
    }
    if (tagids == "") {
        errmsg.push("请选择 标签");
    }
    if (wcontentMd.length < 20) {
        errmsg.push("多写一点内容哦");
    }
    if (errmsg.length > 0) {
        jz.confirm({
            content: errmsg.join('<hr />'),
            time: 12,
            mask: true
        });
        return false;
    }

    $('#btnSaveEdit')[0].disabled = true;

    $.ajax({
        url: "/user/writeeditsave",
        type: "post",
        data: {
            UwId: wid,
            UwTitle: wtitle,
            UwCategory: wcategory,
            UwContent: wcontent,
            UwContentMd: wcontentMd,
            TagIds: tagids,
        },
        dataType: 'json',
        success: function (data) {
            if (data.code == 200) {
                nmd.setmd('');
                $('#ModalWrite').modal("hide");
                gd1.load();
            } else {
                jz.msg(data.msg);
            }
        },
        error: function (ex) {
            if (ex.status == 401) {
                jz.msg("请登录");
            } else {
                jz.msg("网络错误");
            }
        },
        complete: function () {
            $('#btnSaveEdit')[0].disabled = false;
        }
    });
});