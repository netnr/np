//格式化
z.DC["dataurl_colformat"] = {
    data: [
        { value: "0", text: "无格式化" },
        { value: "col_custom_", text: "col_custom_+小写字段名（自定义）" },
        { value: "11", text: "yyyy-MM" },
        { value: "12", text: "HH:mm:ss" },
        { value: "13", text: "yyyy-MM-dd HH:mm:ss" },
        { value: "14", text: "yyyy-MM-dd" },
        { value: "15", text: "精确两位" },
        { value: "16", text: "确两位 带￥" },
        { value: "17", text: "1√ 0×" },
        { value: "18", text: "1× 0√" },
        { value: "19", text: "1正常 0停用" }
    ]
};
//“格式化”格式化
function col_custom_colformat(value) {
    $(z.DC["dataurl_colformat"].data).each(function () {
        if (this.value == value) {
            value = this.text;
            return false;
        }
    });
    return value;
}
//输入类型
z.DC["dataurl_formtype"] = {
    //绑定数据
    data: [
        { value: "text", text: "text 文本" },
        { value: "textarea", text: "textarea 文本域" },
        { value: "combobox", text: "combobox 下拉列表框" },
        { value: "combotree", text: "combotree 下拉列表树" },
        { value: "checkbox", text: "checkbox 复选框" },
        { value: "password", text: "password 密码框" },
        { value: "modal", text: "modal 模态弹出（浏览）" },
        { value: "datetime", text: "yyyy-MM-dd HH:mm:ss 日期时间" },
        { value: "date", text: "yyyy-MM-dd 日期" },
        { value: "time", text: "HH:mm:ss 时间" }
    ],
    //绑定数据前回调
    init: function (obj) {
        //this和obj 都是 z.Combo构造的对象（obj参数可以不要，直接用this）
        //允许编辑
        //this.editable = true;
        //obj.editable = true;
    }
};
//格式化输入类型
function col_custom_formtype(value) {
    $(z.DC["dataurl_formtype"].data).each(function () {
        if (this.value == value) {
            value = this.text;
            return false;
        }
    });
    return value;
}
//跨列
z.DC["dataurl_formspan"] = {
    data: [
        { value: 1, text: "半排", selected: true },
        { value: 2, text: "整排" }
    ],
    init: function () {
        this.panelHeight = 100;
    }
};
//跨列格式化
function col_custom_formspan(value) {
    $(z.DC["dataurl_formspan"].data).each(function () {
        if (this.value == value) {
            value = this.text;
            return false;
        }
    });
    return value;
}
//对齐方式
z.DC["dataurl_colalign"] = {
    data: [
        { value: 1, text: "居左" },
        { value: 2, text: "居中" },
        { value: 3, text: "居右" }
    ],
    init: function () {
        this.panelHeight = 100;
    }
}
//对齐方式格式化
function col_custom_colalign(value) {
    $(z.DC["dataurl_colalign"].data).each(function () {
        if (this.value == value) {
            value = this.text;
            return false;
        }
    });
    return value;
}
//区域
z.DC["dataurl_formarea"] = {
    data: [
        { value: 1, text: "表格配置" },
        { value: 2, text: "表单配置" }
    ],
    init: function () {
        this.panelHeight = 100;
    }
};
//区域格式化
function col_custom_formarea(value) {
    $(z.DC["dataurl_formarea"].data).each(function () {
        if (this.value == value) {
            value = this.text;
            return false;
        }
    });
    return value;
}

//显示/隐藏
var hideobj = function () {
    return {
        data: [
            { value: 0, text: "显示" },
            { value: 1, text: "隐藏" },
            { value: 2, text: "系统级隐藏" }
        ],
        init: function () {
            this.panelHeight = 100;
        }
    }
};
z.DC["dataurl_formhide"] = hideobj();
z.DC["dataurl_colhide"] = hideobj();
var col_custom_colhide = col_custom_formhide = function (value) {
    var obj = hideobj().data, text = '显示';
    $.each(obj, function () {
        if (this.value == value) {
            text = this.text;
            return false;
        }
    })
    return text;
}


//载入
var gd1 = z.Grid();
gd1.url = "/RF/QuerySysTableConfig?tableName=SysTableConfig"
gd1.multiSort = true;
gd1.sortName = "TableName,ColOrder";
gd1.sortOrder = "asc,asc";

//从表载入前，填充查询条件，且需要指定gd1.QueryOk方法
gd1.onBeforeLoad = function (row, param) {
    var sq = gd1.QueryWhere();

    //注意：快捷查询条件 与 查询面板 不能同时存在，查询面板需要关闭快捷查询已有的项，避免冲突
    //此处的例子是 引用的配置表，所以查询面板的条件没有关闭

    //虚表条件
    var qtb = $('#query_tablename').val();
    if (qtb != "") {
        sq.wheres.push({ field: "TableName", relation: "Equal", value: qtb });
    }

    //模糊搜索表字段
    var qcf = $('#query_colfield').val();
    if (qcf != "") {
        sq.wheres.push({ field: "ColField", relation: "Contains", value: qcf });
    }

    param.wheres = sq.stringify();
}
//从表查询条件面板确定事件，如果不写该事件，则调用默认的查询面板条件（我们需要添加主表的条件项）
gd1.QueryOk = function () {
    gd1.pageNumber = 1;
    gd1.load();

    //gd2.query 查询面板整个对象
    //gd2.query.md  查询面板 z.Modal 返回的对象
    //gd2.query.md.modal    模态框对象（jQuery对象）
}
$('#query_tablename').change(function () {
    gd1.load();
})
$('#btnSearch').click(function () {
    gd1.load();
})
$('#query_colfield').keydown(function (e) {
    e = e || window.event;
    if (e.keyCode == 13) {
        gd1.load();
    }
})

gd1.load();

//查询
z.button('query', function () {
    gd1.QueryOpen();
});

//刷新
z.button('reload', function () {
    gd1.load();
});

//批处理
z.button('batch', function () {
    gd1.singleSelect = false;
    gd1.checkbox = true;
    gd1.load();
})

//关闭批处理
z.button('batch_close', function () {
    gd1.singleSelect = true;
    gd1.checkbox = false;
    gd1.bind();
})

//批量回调调用
function BatchCallBack() {
    var rowDatas = gd1.func("getSelections");
    if (rowDatas.length) {
        var msg = "";
        switch (z.btnTrigger) {
            case "batch_start":
                msg = "启用";
                break;
            case "batch_stop":
                msg = "停用";
                break;
            case "batch_edit":
                msg = "修改";
                break;
            case "batch_del":
                msg = "删除";
                break;
        }
        art("确定批量" + msg + "已选择的 " + rowDatas.length + " 条记录？（演示非真" + msg + "）", function () {
            art("success")
            gd1.load();
        })
    } else {
        art('select');
    }
}

//批量启用
z.button('batch_start', function () {
    BatchCallBack()
})

//批量停用
z.button('batch_stop', function () {
    BatchCallBack()
})

//批量修改
z.button('batch_edit', function () {
    BatchCallBack()
})

//批量删除
z.button('batch_del', function () {
    BatchCallBack()
})