//字典分类初始化
var cbtype = z.Combo();
cbtype.id = "#query_sdtype";
cbtype.icons = false;
cbtype.reversed = true;
cbtype.onChange = function (newValue, oldValue) {
    if (oldValue) {
        gd1.load();
    }
}
//在此配置所有可维护的字典分类，推荐格式：表名:列名 作为分类唯一标识
cbtype.data = [
    { text: "示例字典一", value: "SysDictionary:SdType", selected: true },
    { text: "示例字典二", value: "SysTableConfig:ColAlign" },
    { text: "示例字典三", value: "SysTableConfig:FormSpan" }
];
cbtype.bind();

//状态
var cbstatus = z.Combo();
cbstatus.id = "#query_sdstatus";
cbstatus.icons = false;
cbstatus.editable = false;
cbstatus.onChange = function (newValue, oldValue) {
    if (oldValue) {
        gd1.load();
    }
}
cbstatus.panelHeight = 140;
cbstatus.data = [
    { text: "正常", value: 1, selected: true },
    { text: "停用", value: 2 },
    { text: "删除", value: -1 },
    { text: "全部", value: 10 }
];
cbstatus.bind();

//字典分类状态格式化
function col_custom_sdstatus(value) {
    var labcolor;
    $.each(cbstatus.data, function () {
        if (this.value == value) {
            switch (this.value) {
                case 1:
                    labcolor = 'info';
                    break;
                case 2:
                    labcolor = 'warning';
                    break;
                case -1:
                    labcolor = 'danger';
                    break;
            }
            value = this.text;
            return;
        }
    })
    return '<span class="label label-' + labcolor + '">' + value + '</span>';
}

//表单字典分类
z.DC["dataurl_sdtype"] = {
    data: cbtype.data
}

//状态
z.DC["dataurl_sdstatus"] = {
    data: [
        { text: "正常", value: 1, selected: true },
        { text: "停用", value: 2 },
        { text: "删除", value: -1 }
    ]
}

//载入
var gd1 = z.Grid();
gd1.url = "/Setting/QuerySysDictionary?tableName=" + z.TableName;
gd1.sortName = "SdOrder";
gd1.onDblClickRow = function (index, row) {
    //双击行模拟点编辑
    z.buttonClick('edit');
}
//从表载入前，填充查询条件，且需要指定gd1.QueryOk方法
gd1.onBeforeLoad = function (row, param) {
    var sq = gd1.QueryWhere();

    //注意：快捷查询条件 与 查询面板 不能同时存在，查询面板需要关闭快捷查询已有的项，避免冲突

    //字典分类
    sq.wheres.push({ field: "SdType", relation: "Equal", value: cbtype.func('getValue') });
    //字典状态
    var cbsv = cbstatus.func('getValue');
    if (cbsv != 10) {
        sq.wheres.push({ field: "SdStatus", relation: "Equal", value: cbsv });
    }

    param.wheres = sq.stringify();
}
//从表查询条件面板确定事件，如果不写该事件，则调用默认的查询面板条件（我们需要添加主表的条件项）
gd1.QueryOk = function () {
    gd1.pageNumber = 1;
    gd1.load();
}
gd1.load();

//查询
z.button('query', function () {
    gd1.QueryOpen();
});

//刷新
z.button('reload', function () {
    gd1.load();
});

//新增
z.button('add', function () {
    //表单标题
    z.FormTitle({
        icon: 0,
        title: '新增数据字典'
    });
    $('#SdType').combobox('setValue', cbtype.func('getValue'));
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
            title: '查看数据字典',
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
        //选中行回填表单
        z.FormEdit(rowData);
        //表单标题
        z.FormTitle({
            icon: 1,
            title: '修改数据字典'
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
        $.ajax({
            url: "/Setting/SaveSysDictionary?savetype=" + z.btnTrigger,
            type: "post",
            data: $("#fv_form_1").serialize(),
            dataType: 'json',
            success: function (data) {
                if (data.code == 200) {
                    gd1.load();
                    $('#fv_modal_1').modal('hide');
                } else {
                    art(data.msg);
                }
            },
            error: function () {
                art('error');
            }
        });

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
        $.ajax({
            url: "/Setting/DelSysDictionary?id=" + rowData.SdId,
            type: "post",
            dataType: 'json',
            success: function (data) {
                if (data.code == 200) {
                    gd1.load();
                } else {
                    art(data.msg);
                }
            }
        })
    });
});

//导出回调
function ExportCallBack(data) {
    //文件名
    data.title = document.title + "[" + cbtype.func('getText') + "]";
    //排序
    data.sort = gd1.sortName;
    data.order = gd1.sortOrder;

    var sq = gd1.QueryWhere();

    //字典分类
    sq.wheres.push({ field: "SdType", relation: "Equal", value: cbtype.func('getValue') });
    //字典状态
    var cbsv = cbstatus.func('getValue');
    if (cbsv != 10) {
        sq.wheres.push({ field: "SdStatus", relation: "Equal", value: cbsv });
    }

    data.wheres = sq.stringify();
}