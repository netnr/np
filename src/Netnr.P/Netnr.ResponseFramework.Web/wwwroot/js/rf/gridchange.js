//主表
var gd1 = z.Grid();
gd1.url = "/RF/QueryGridChange1?tableName=SysRole"
gd1.multiSort = true;
gd1.title = "角色列表";
gd1.sortName = "SrCreateTime";
gd1.autosizePid = "#PGrid1";
gd1.onClickRow = function () {
    gd2.pageNumber = 1;
    gd2.load();
}
gd1.load();

//从表
var gd2 = z.Grid();
gd2.url = "/RF/QueryGridChange2?tableName=SysUser"
gd2.multiSort = true;
gd2.title = "角色对应的用户";
gd2.sortName = "SuCreateTime";
gd2.id = "#Grid2";
gd2.autosizePid = "#PGrid2";
//从表载入前，填充查询条件，且需要指定gd2.QueryOk方法
gd2.onBeforeLoad = function (row, param) {
    var sq = gd2.QueryWhere();

    //从表根据主表选择的行查询
    //根据角色ID查询对应的用户
    var rowData = gd1.func('getSelected');
    sq.wheres.push({
        field: "SrId",
        relation: "Equal",
        value: rowData.SrId
    })
    param.wheres = sq.stringify();
}
//从表查询条件面板确定事件，如果不写该事件，则调用默认的查询面板条件（我们需要添加主表的条件项）
gd2.QueryOk = function () {
    gd2.pageNumber = 1;
    gd2.load();

    //模态框隐藏
    gd2.query.md.modal.modal('hide');

    //gd2.query 查询面板整个对象
    //gd2.query.md  查询面板 z.Modal 返回的对象
    //gd2.query.md.modal    模态框对象（jQuery对象）
}

//刷新
z.button('reload', function () {
    location.reload()
})