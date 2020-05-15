//富文本教程：https://ckeditor.com/docs/ckeditor4/latest/guide/dev_readonly.html

//初始化富文本
CKEDITOR.replace('txtRich', {
    toolbarCanCollapse: true,
    image_previewText: '',
    removeDialogTabs: 'image:advanced;image:Link',
    //粘贴、拖拽上传
    uploadUrl: "/Common/UploadRich",
    //图片选择上传
    filebrowserImageUploadUrl: "/Common/UploadRich",
    //自定义工具栏
    toolbar: [
        ['Undo', 'Redo', 'Styles', 'Format', 'FontSize'],
        ['Bold', 'Italic', 'Strike', 'RemoveFormat'],
        ['NumberedList', 'BulletedList', '-', 'Blockquote'],
        ['Link', 'Unlink', 'Anchor'],
        ['Image', 'Table', 'HorizontalRule', 'SpecialChar'],
        ['Source']
    ]
});

//载入
var gd1 = z.Grid();
gd1.columns = [
    [
        {
            "field": "SbBtnId",
            "title": "标题",
            "width": 100,
            "hidden": false,
            "halign": "center",
            "align": "left"
        },
        {
            "field": "txtRich",
            "title": "内容",
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
        }
    ]
];
gd1.data = [{ "SbId": "EFE021E2-30FE-4500-9BF6-52611F1AAA4A", "SbPid": "00000000-0000-0000-0000-000000000000", "txtRich": "<em><b>富文本基于CKEditor，可拖拽粘贴图片</b></em>", "SbBtnId": "标题一", "SbBtnClass": "btn btn-sm  btn-success", "SbBtnIcon": "fa fa-search", "SbBtnOrder": 1, "SbStatus": 1, "SbDescribe": "", "SbBtnGroup": 1, "SbBtnHide": null, "state": "open" }, { "SbId": "90ED8666-0961-426D-B582-E08C43EEE9E1", "SbPid": "00000000-0000-0000-0000-000000000000", "txtRich": "此为本地数据，非操作服务器，刷新会丢失，上传文件是临时目录，会定时清理", "SbBtnId": "标题二", "SbBtnClass": "btn btn-sm btn-primary", "SbBtnIcon": "fa fa-plus", "SbBtnOrder": 2, "SbStatus": 1, "SbDescribe": null, "SbBtnGroup": 1, "SbBtnHide": null, "state": "open" }];
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
    //清空富文本
    CKEDITOR.instances.txtRich.setData("");
    $('#fv_modal_1').modal();
});

//查看
z.button('see', function () {
    //获取选中行
    var rowData = gd1.func("getSelected");
    if (rowData) {
        //选中行回填表单
        z.FormEdit(rowData);
        //赋值富文本
        CKEDITOR.instances.txtRich.setData(rowData.txtRich);
        //富文本只读
        CKEDITOR.instances.txtRich.setReadOnly();
        //表单标题
        z.FormTitle({
            icon: 2,
            title: '查看' + document.title,
            required: false
        });
        //禁用
        z.FormDisabled(true);
        //修复禁用造成的问题
        $('#txtRich').next().show();
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
        //取消富文本只读
        CKEDITOR.instances.txtRich.setReadOnly(false);
    }
});

//修改
z.button('edit', function () {
    //获取选中行
    var rowData = gd1.func("getSelected");
    if (rowData) {
        //选中行回填表单
        z.FormEdit(rowData);
        //赋值富文本
        CKEDITOR.instances.txtRich.setData(rowData.txtRich);
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
    //富文本的值
    var txthtm = CKEDITOR.instances.txtRich.getData();
    //把富文本的值赋予文本域，触发验证
    $('#txtRich').val(txthtm);

    //检测必填项
    if (z.FormRequired('red')) {

        $('#fv_save_1')[0].disabled = true;

        //表单数据
        var newdata = z.FormToJson();
        newdata.txtRich = txthtm;

        //新增
        if (z.btnTrigger == "add") {
            gd1.data.push(newdata);
            gd1.bind();
        } else {
            //编辑
            gd1.func("updateRow", {
                index: gd1.func('getRowIndex', gd1.func('getSelected')),
                row: newdata
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