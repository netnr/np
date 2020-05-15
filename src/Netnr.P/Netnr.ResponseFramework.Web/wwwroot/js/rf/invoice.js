//门店
z.DC["/common/querymenu"] = {
    init: function (cb) {
        cb.onBeforeSelect = function (node) {
            if (node.children) {
                art('请选择 子节点');
                return false;
            }
        }
    }
};

//采购类型
z.DC["dataurl_timtype"] = {
    data: [
        { value: 1, text: "普通采购" },
        { value: 2, text: "促销采购" },
        { value: 3, text: "批量采购" }
    ]
};

//供应商
z.DC["dataurl_timsupplier"] = {
    data: [
        { value: 1, text: "1688" },
        { value: 2, text: "亚马逊" }
    ]
};


//页面对象（PageObject）
z.po = {
    //表单ID
    FormId1: "#invoiceForm_1",
    //表单只读
    FormReadonly: false
}

//载入
var gd1 = z.Grid();
gd1.url = "/RF/QueryInvoiceMain?tableName=" + z.TableName;
gd1.multiSort = true;
gd1.sortName = "TimCreateTime";
gd1.sortOrder = "asc";
gd1.onDblClickRow = function (index, row) {
    z.buttonClick('edit');
}
gd1.load();

//查询
z.button('query', function () {
    gd1.QueryOpen();
});

//刷新
z.button('reload', function () {
    z.invoiceViewSwitch(false);
    gd1.load();
});

//新增
z.button('add', function () {
    //切换视图
    z.invoiceViewSwitch(true);
    //切换按钮状态
    z.buttonViewSwitch(['save', 'cancel'], true);

    //禁用表单
    z.FormDisabled(false, z.po.FormId1);
    z.po.FormReadonly = false;

    //清空表单
    z.FormClear(z.po.FormId1);

    //重置颜色
    z.FormRequired("reset", z.po.FormId1);

    //第一次加载，后面重新绑定
    ig1.isinit ? ig1.load() : ig1.bind();
});

//切换
z.button('switch', function () {
    viewInvoice()
})

//修改
z.button('edit', function () {
    viewInvoice();
});

//显示单据
function viewInvoice() {

    //获取选中行
    var rowData = gd1.func("getSelected");
    if (rowData) {
        //清空表单
        z.FormClear(z.po.FormId1);

        //重置颜色
        z.FormRequired("reset", z.po.FormId1);

        //切换按钮显示状态
        switch (z.btnTrigger) {
            case "edit":
                z.invoiceViewSwitch(true);
                z.buttonViewSwitch(['save', 'cancel'], true)

                //禁用表单
                z.FormDisabled(false, z.po.FormId1);
                z.po.FormReadonly = false;
                break;
            case "switch":
                z.invoiceViewSwitch();
                z.buttonViewSwitch(['save', 'cancel'], false);

                //禁用表单
                z.FormDisabled(z.invoiceControlStatus, z.po.FormId1);
                z.po.FormReadonly = z.invoiceControlStatus;
                break;
        }

        //回填主表
        z.FormEdit(rowData);

        //加载从表
        ig1.load();
    } else {
        art("select");
    }
}

//保存
z.button('save', function () {
    //结束编辑
    if (ig1.ei != null) {
        ig1.func('endEdit', ig1.ei);
        ig1.ei = null;
    }

    //表单验证
    if (z.FormRequired("red", z.po.FormId1)) {
        //明细项验证
        var robj = z.invoiceGridExtend.requiredRow(ig1);
        if (robj.lack.length) {
            art(robj.lack.join('<br/>'));
        } else {

            //禁用按钮
            var btn = z.buttonForTrigger();
            $(btn).addClass('disabled');

            //保存
            $.ajax({
                url: "/RF/SaveInvoiceForm",
                type: 'post',
                data: $(z.po.FormId1).serialize() + "&rows=" + encodeURIComponent(JSON.stringify(robj.rows)),
                success: function (data) {
                    if (data.code == 200) {
                        //模拟点击取消
                        z.buttonClick('cancel');

                        gd1.load();
                    } else {
                        art(data.msg);
                    }
                },
                error: function () {
                    art('error')
                },
                complete: function () {
                    //启用按钮
                    $(btn).removeClass('disabled');
                }
            })
        }
    }
});

//取消
z.button('cancel', function () {
    z.invoiceViewSwitch();
    z.buttonViewSwitch(['save', 'cancel'], false);
});
//初始化按钮状态（与取消事件保持一直）
z.buttonViewSwitch(['save', 'cancel'], false);





//单据明细表
var ig1 = z.Grid();
ig1.id = "#invoiceGrid_1";
ig1.pid = "#invoicePGrid_1";
ig1.url = "/RF/QueryInvoiceDetail?tableName=TempInvoiceDetail";
ig1.pagination = false;
ig1.sortName = "TidOrder";
ig1.autosize = "x";
ig1.onClickCell = function (index, field, value) {
    //排除不编辑的列，非只读
    if (["GoodsCode", "GoodsType", "GoodsModel"].indexOf(field) == -1 && !z.po.FormReadonly) {
        z.GridEditor(ig1, index, field);
    }
}
//载入前，填充查询条件
ig1.onBeforeLoad = function (row, param) {
    var rowData = gd1.func("getSelected");
    if (rowData) {
        //主表ID
        param.pe1 = rowData.TimId;
    }
}
//绑定前
ig1.onBeforeBind = function (gd) {
    //第一次初始化
    if (gd.isinit) {
        //添加操作按钮
        var cc = {
            field: "CtrlCol", title: "操作", width: 80, align: "center", halign: "center", formatter: function (value, row, index) {
                var htm = [], igkey = "ig1";
                htm.push('<i onclick="z.invoiceGridExtend.addRow(' + igkey + ')" class="fa fa-lg fa-plus blue" title="新增一行" style="cursor:pointer;user-select:none;"></i> &nbsp; ')
                htm.push('<i onclick="z.invoiceGridExtend.delRow(' + igkey + ')" class="fa fa-lg fa-remove red" title="删除当前行" style="cursor:pointer;user-select:none;"></i>')
                return htm.join('');
            }
        };
        gd.frozenColumns[0].splice(0, 0, cc);

        //必填标题突出
        $.each(gd.columns[0], function () {
            if (this.FormRequired) {
                this.title = '<span class="required">' + this.title + '</span>';
            }
        })
    }

    switch (z.btnTrigger) {
        case "add":
            gd.data = [{}];
            break;
        case "edit":
        case "switch":
            if (!gd.data.length) {
                gd.data = [{}];
            }
            break;
    }

    //是否显示操作列
    $.each(gd.frozenColumns[0], function () {
        if (this.field == "CtrlCol") {
            this.hidden = z.btnTrigger == "switch";
            return false;
        }
    })
}