//文件格式化
function col_custom_txtfile(value) {
    //第一种：直接显示图片
    //if (value) {
    //    return '<img src="' + value + '" style="max-height:33px" />'
    //}

    //第二种：显示图标，点击图标显示（推荐，避免加载所有的图片）
    if (value) {
        return '<i class="fa fa-2x text-muted fa-image" onclick="viewimg()" style="cursor:pointer" />'
    }

    //也可以结合两种模式：直接显示原始图片，限制高度，点击后显示大图片
}

//预览图片
function viewimg() {
    setTimeout(function () {
        var rowData = gd1.func("getSelected");
        if (rowData) {
            var ao = art('<div style="text-align:center"><img src="/images/loading.gif" style="max-width:100%" /></div>');
            ao.modal.find('.modal-dialog').addClass('modal-lg modal-full');
            ao.modal.find('.modal-title').html('<i class="fa fa-eye blue"></i><span>查看<span>');
            ao.modal.find('.modal-body').children().removeClass('alert alert-info');
            ao.modal.find('.modal-footer').addClass('hidden');

            var img = new Image();
            img.src = rowData.txtFile;
            img.onload = function () {
                ao.modal.find('img').attr('src', img.src);
                z.FormAutoHeight();
            }
        }
    }, 100)
}

//载入
var gd1 = z.Grid();
gd1.columns = [
    [
        {
            "field": "SbBtnText",
            "title": "按钮文本",
            "width": 550,
            "halign": "center",
            "align": "left"
        },
        {
            "field": "SbPid",
            "title": "上级按钮",
            "width": 100,
            "hidden": true,
            "halign": "center",
            "align": "left"
        },
        {
            "field": "txtFile",
            "title": "文件上传",
            "width": 100,
            "hidden": false,
            "halign": "center",
            "align": "center",
            "FormUrl": "col_custom_",
            formatter: function (value) {
                return col_custom_txtfile(value);
            }
        },
        {
            "field": "SbBtnId",
            "title": "按钮ID",
            "width": 100,
            "hidden": false,
            "halign": "center",
            "align": "left"
        },
        {
            "field": "SbBtnOrder",
            "title": "按钮排序",
            "width": 80,
            "hidden": false,
            "halign": "center",
            "align": "center"
        }
    ]
];
gd1.data = [{ "SbId": "EFE021E2-30FE-4500-9BF6-52611F1AAA4A", "SbPid": "00000000-0000-0000-0000-000000000000", "SbBtnText": "测试时，请打开控制台，Network 选项卡限制网速查看进度条，如设置 Fast 3G", "SbBtnId": "m_Query", "SbBtnClass": "btn btn-sm  btn-success", "SbBtnIcon": "fa fa-search", "SbBtnOrder": 1, "SbStatus": 1, "SbDescribe": "", "SbBtnGroup": 1, "SbBtnHide": null, "state": "open" }, { "SbId": "90ED8666-0961-426D-B582-E08C43EEE9E1", "SbPid": "00000000-0000-0000-0000-000000000000", "SbBtnText": "此为本地数据，非操作服务器，刷新会丢失，上传文件是临时目录，会定时清理", "SbBtnId": "m_Add", "SbBtnClass": "btn btn-sm btn-primary", "SbBtnIcon": "fa fa-plus", "SbBtnOrder": 2, "SbStatus": 1, "SbDescribe": null, "SbBtnGroup": 1, "SbBtnHide": null, "state": "open" }];
gd1.pagination = false;
gd1.onDblClickRow = function (index, row) {
    //双击行模拟点编辑
    z.buttonClick('edit');
}
gd1.bind();

//刷新
z.button('reload', function () {
    gd1.bind();
});

//新增
z.button('add', function () {
    //表单标题
    z.FormTitle({
        icon: 0,
        title: '新增' + document.title
    });
    $('#hid_txtFile').val('');
    $('#fv_modal_1').modal();
});

//查看
z.button('see', function () {
    //获取选中行
    var rowData = gd1.func("getSelected");
    if (rowData) {
        //选中行回填表单
        z.FormEdit(rowData);
        //表单标题
        z.FormTitle({
            icon: 2,
            title: '查看' + document.title,
            required: false
        });
        //禁用
        z.FormDisabled(true);
        //显示模态框
        $('#fv_modal_1').modal();
    } else {
        art("select");
    }
});
//关闭模态框后
$('#fv_modal_1').on('hidden.bs.modal', function () {
    //是查看时，解除禁用
    if (z.btnTrigger == "see") {
        z.FormDisabled(false);
    }
});

//修改
z.button('edit', function () {
    //获取选中行
    var rowData = gd1.func("getSelected");
    if (rowData) {
        $('#hid_txtFile').val('');
        //选中行回填表单
        z.FormEdit(rowData);
        //表单标题
        z.FormTitle({
            icon: 1,
            title: '修改' + document.title
        });
        //显示模态框
        $('#fv_modal_1').modal();
    } else {
        art("select");
    }
});

//保存
$('#fv_save_1').click(function () {
    //检测必填项
    if (z.FormRequired('red')) {
        $('#fv_save_1')[0].disabled = true;

        //新增
        if (z.btnTrigger == "add") {
            gd1.data.push(z.FormToJson());
            gd1.bind();
        } else {
            //编辑
            gd1.func("updateRow", {
                index: gd1.func('getRowIndex', gd1.func('getSelected')),
                row: z.FormToJson()
            });
        }

        $('#fv_modal_1').modal('hide');

        $('#fv_save_1')[0].disabled = false;
    }
});

//删除
z.button('del', function () {
    var rowData = gd1.func("getSelected");
    if (!rowData) {
        art('select');
        return false;
    }
    art('确定删除选中的行', function () {
        gd1.func('deleteRow', gd1.func('getRowIndex', gd1.func('getSelected')))
    });
});

//选择文件
$("#txtFile").change(function () {
    var file = $('#txtFile')[0].files[0];
    if (file) {
        var err = [];
        var ext = file.name.substr(file.name.lastIndexOf('.'));
        var exts = ".jpg .jpeg .png .gif";
        if (exts.indexOf(ext) == -1) {
            err.push('格式文件不对，仅支持 ' + exts);
            //清空文件
            this.value = '';
        }
        //文件大小限制，单位M
        var maxsize = 2;
        if (file.size / 1024 / 1024 > maxsize) {
            err.push("文件大小不能超过 " + maxsize + "M")
        }

        if (err.length) {
            art(err.join('</br>'));
        } else {
            UploadFile(file);
        }
    }
})

//上传
function UploadFile(file) {
    var formData = new FormData();
    formData.append("file1", file);

    //Common/Upload?temp=1 上传到临时文件夹，用于导入等，文件会被定时清理
    //Common/Upload?path=doc 上传自定义文件夹路径，永久路径，在 upload 文件夹下 创建 doc 文件夹

    $.ajax({
        url: "/Common/Upload?temp=1",
        type: "POST",
        data: formData,
        contentType: false,
        processData: false,
        dataType: 'json',
        xhr: function () {
            xhr = $.ajaxSettings.xhr();
            xhr.upload.addEventListener('progress', function (e) {
                var rate = ((e.loaded / e.total) * 100).toFixed();
                UploadProgress('#txtFile', rate);
            })
            return xhr;
        },
        success: function (data) {
            console.log(data);
            //上传成功，把上传返回的路径赋值 隐藏的 hidden
            //hidden 的id等于 hid_加文件输入框的ID，固定格式
            //表单的新增、修改，上传成功后更新 hidden 的文件路径
            //表单的验证，发现是file类型，自动检测 "#hide_"+file输入框的ID，不会去验证file的值
            //file的name标签需要清空，应赋值到hidden的name标签上
            $('#hid_txtFile').val(data.data);
        },
        error: function () {
            art("上传出错");
        }
    });
}

//进度条
function UploadProgress(input, rate) {
    var ipt = $(input);
    var css = 'position:absolute;height:10px;width:' + ipt.outerWidth() + 'px;z-index:2888;left:' + ipt.offset().left + 'px;top:' + (ipt.offset().top + ipt.outerHeight()) + 'px';
    if (!ipt.data('progress')) {
        var htm = [];
        htm.push('<div class="progress">');
        htm.push('<div class="progress-bar progress-bar-success progress-bar-striped active" role="progressbar">');
        htm.push('</div>');
        var node = $(htm.join(''));
        node.appendTo(document.body);
        ipt.data('progress', node);
    }
    if (rate < 100) {
        ipt.data('progress')[0].style.cssText = css;
        ipt.data('progress').children().width(rate + "%");
    } else {
        ipt.data('progress').remove();
    }
}