//载入
var gd1 = z.Grid();
gd1.url = "/Setting/QuerySysUser?tableName=" + z.TableName;
gd1.sortName = "SuCreateTime";
gd1.onDblClickRow = function (index, row) {
    //双击行模拟点编辑
    z.buttonClick('edit');
}

gd1.load();

//查询
z.button('query', function () {
    gd1.QueryOpen();
});

//角色格式化
function col_custom_srid(value, row, v) {
    //初始化第一次
    if (gd1.isinit) {
        value = row.SrName;
    } else {
        $.each(z.DC["/common/queryrole"].data, function () {
            if (this.value == value) {
                value = this.text;
                return false;
            }
        });
    }
    return value;
}

//刷新
z.button('reload', function () {
    gd1.load();
});

//新增
z.button('add', function () {
    //表单标题
    z.FormTitle({
        icon: 0,
        title: '新增系统用户'
    });
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
            title: '查看系统用户',
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
            title: '修改系统用户'
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
            url: "/Setting/SaveSysUser?savetype=" + z.btnTrigger,
            type: "post",
            data: $("#fv_form_1").serialize(),
            dataType: 'json',
            success: function (data) {
                if (data.code == 97) {
                    art('账号已经存在');
                } else if (data.code == 200) {
                    //新增成功，重新载入
                    if (z.btnTrigger == "add") {
                        gd1.load();
                    } else {
                        //编辑成功，修改行
                        gd1.func("updateRow", {
                            index: gd1.func('getRowIndex', gd1.func('getSelected')),
                            row: z.FormToJson()
                        });
                    }
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
            url: "/Setting/DelSysUser?id=" + rowData.SuId,
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
    var sq = gd1.QueryWhere();
    data.wheres = sq.stringify();
}