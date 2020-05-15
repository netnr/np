//载入
var gd1 = z.Grid();
gd1.url = "/Setting/QuerySysLog?tableName=" + z.TableName;
gd1.sortName = "LogCreateTime";
gd1.sortOrder = "desc";
gd1.load();

//查询
z.button('query', function () {
    gd1.QueryOpen();
});

//刷新
z.button('reload', function () {
    gd1.load();
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
            title: '查看日志',
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

//导出回调
function ExportCallBack(data) {
    var sq = gd1.QueryWhere();
    data.wheres = sq.stringify();
}