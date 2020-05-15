z.DC["dataurl_formtype"] = {
    //绑定数据
    data: [
        { value: "text", text: "text 文本" },
        { value: "textarea", text: "textarea 文本域" },
        { value: "date", text: "yyyy-MM-dd 日期" },
        { value: "time", text: "mm:ss 时间" },
        { value: "datetime", text: "yyyy-MM-dd HH:mm:ss 日期时间" },
        { value: "calc", text: "calc 计算器" },
        { value: "combobox", text: "combobox 下拉列表框" },
        { value: "combotree", text: "combotree 下拉列表树" },
        { value: "modal", text: "modal 模态弹出（浏览）" },
        { value: "checkbox", text: "checkbox 复选框" },
        { value: "password", text: "password 密码框" }
    ],
    //绑定数据前回调
    init: function (obj) {
        //this和obj 都是 z.Combo构造的对象（obj参数可以不要，直接用this）
        //允许编辑，具体配置项参考 EasyUI文档
        this.editable = true; //obj.editable = true;
    }
};
z.DC["/common/querymenu?custom=m"] = {
    init: function () {
        this.panelHeight = 300;
    }
}
z.DC["dataurl_colformat"] = {
    data: [
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
z.DC["dataurl_formarea"] = {
    data: [
        { value: 1, text: "值一" },
        { value: 2, text: "值二" }
    ],
    init: function () {
        this.multiple = true;
        this.panelHeight = 100;
    }
};

//modal 浏览示例
z.DC["/setting/sysuser"] = {
    init: function () {
        //允许输入
        //this.editable = true;

        //确定回调事件
        this.okClick = function () {
            var wd = this.modal.find('iframe')[0].contentWindow;
            var rowData = wd.gd1.func('getSelected');

            //由于新增、编辑、公共查询都调用一个模态框，需要对确定事件做不同处理
            //this.guide 指向触发模态框的节点
            //console.log(this.guide);

            //this.guidetype 触发类型，form 表单触发，table 表格编辑触发
            switch (this.guidetype) {
                //表单输入，即新增、编辑
                case "form":
                    {
                        //人员姓名
                        $('#FormPlaceholder').val(rowData ? rowData.SuNickname : '');
                        //人员账号
                        $('#ColWidth').val(rowData ? rowData.SuName : '');
                    }
                    break;
                //表格编辑，公共查询
                case "table":
                    {
                        //当前行
                        var queryRow = z.queryin.grid.func('getSelected');
                        //当前行索引
                        var queryIndex = z.queryin.grid.func('getRowIndex', queryRow);
                        //当前行对应的字段(小写)
                        var field = z.queryin.grid.data[queryIndex].field.toLowerCase();

                        switch (field) {
                            //人员姓名
                            case "formplaceholder":
                                queryRow.value1 = rowData ? rowData.SuNickname : '';
                                break;
                            //人员账号
                            case "colwidth":
                                queryRow.value1 = rowData ? rowData.SuName : '';
                                break;
                        }
                        //更新行
                        z.queryin.grid.func('updateRow', { index: queryIndex, row: queryRow });
                        //刷新行
                        z.queryin.grid.func('refreshRow', queryIndex);
                    }
                    break;
            }

            this.hide();
        };
    }
};

//查询面板格式化，col_custom_query_开头加列字段小写
//可选，有就调用
//注意：比如说查询人员，在选择人员姓名的时候，我实际上想拿到人员账号，即 key value的形式
//      那么就应该需要格式化，格式化又有个问题，我不知道会选择那个人员，怎么格式化？
//      1.把数据库的所有人员的账号、姓名查询出来（量少可以，量大不科学）
//      2.在浏览时选择一行，把这个数据追加到这个格式化中，选一个追加一个，
//        所谓的追加：重写格式化方法 或者 申明一个数组，查询方法引用数组，把新的值push进去
//        但是需要二次刷新查询条件表格，因为在选择的时候
function col_custom_query_colwidth(value, row) {
    var text = value;
    //格式化 key value 对象；当然也可以简化,如：{admin:"管理员",test:"测试"}
    var arr = [
        { value: "admin", text: "管理员" }
    ];
    $.each(arr, function () {
        if (this.value == value) {
            text = this.text;
            return false;
        }
    })
    return text;
}

//modal 浏览示例
z.DC["/setting/sysrole"] = {
    init: function () {
        //配置项看 z.Modal组件
        this.title = '<i class="fa fa-search blue"></i><span>选择角色<span>';
        //模态框大小 3最大 2普通 1小
        this.size = 2;
        //设置iframe最大高度
        this.heightIframe = 800;
        //OK按钮文本
        this.okText = '确定';
        //不显示右上角关闭按钮
        this.showClose = false;
        //不显示取消按钮
        this.showCancel = false;

        //确定回调
        this.okClick = function () {
            //根据模态框 this.modal 对象 找到 iframe
            var wd = this.modal.find('iframe')[0].contentWindow;
            //获取iframe里面的选中行数据
            var rowData = wd.gd1.func('getSelected');

            //由于新增、编辑、公共查询都调用一个模态框，需要对确定事件做不同处理
            //this.guide 指向触发模态框的节点
            //this.guidetype 触发类型，form 表单触发，table 表格编辑触发
            switch (this.guidetype) {
                //表单输入，即新增、编辑
                case "form":
                    this.guide.val(rowData ? rowData.SrName : '');
                    break;
                //表格编辑，公共查询
                case "table":
                    {

                    }
                    break;
            }

            this.hide();
        };
    }
};



//载入
var gd1 = z.Grid();
gd1.url = "/RF/QueryTempExample?tableName=" + z.TableName;
gd1.multiSort = true;
gd1.sortName = "TableName,ColOrder";
gd1.sortOrder = "asc,asc";
gd1.onDblClickRow = function (index, row) {
    //双击行模拟点编辑
    z.buttonClick('edit');
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
        title: '新增表单'
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
            title: '查看表单',
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
            title: '修改表单'
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

    });
});

setTimeout(function () {
    //模拟点击新增
    z.buttonClick('add');
}, 1000 * 1.5);