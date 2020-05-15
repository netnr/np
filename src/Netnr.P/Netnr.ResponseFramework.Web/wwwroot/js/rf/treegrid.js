//载入
var gd1 = z.Grid();
gd1.url = "/RF/QuerySysMenu?tableName=SysMenu";
gd1.sortName = "SmOrder";
gd1.type = "treegrid";
gd1.idField = "SmId";
gd1.treeField = "SmName";
gd1.onBeforeLoad = function (row, param) {
    //条件
    var sq = z.SqlQuery();

    sq.wheres.push({
        field: "SmPid",
        relation: "Equal",
        value: "00000000-0000-0000-0000-000000000000"
    });

    param.wheres = sq.stringify();
}
//绑定前回调
gd1.onBeforeBind = function (obj) {
    //把所有节点状态设置为未展开，state属性应该在后台查询
    $(obj.data).each(function (i) {
        i && (this.state = "closed");
    });
}
//展开时
gd1.onBeforeExpand = function (row) {
    //设置请求子节点数据
    //子节点还有子节点请设置state属性为closed
    gd1.func('options').url = "/RF/TreeGrid?id=" + row.SrId;

    //注意：
    //此处的请求由EasyUI内部实现，非z.js通过ajax得到数据来绑定
    //而z.js虽然设置url，但已将url转为_url，仅作为兼容API使用
    //由于设置请求子节点数据导致url变化，所以在完成时清空异步载入的url，不然刷新会出问题
    //展开节点设置url，请求结束时，要清空url，onLoadSuccess事件里面清空url，为什么要清空，url变化treegrid整个刷新会出问题
    //以上描述如有问题，请进群交流，有其他办法做异步载入也可以交流
};
//载入成功时
gd1.onLoadSuccess = function () {
    //清空设置的请求子节点数据的url
    //此处清空url不影响gd1.url="XXX"的值，因为已处理为_url，为什么要处理_url，是为了不触发EasyUI提供的URL载入，而是查询得到数据手动绑定
    gd1.func('options').url = null;
}
gd1.load();

//刷新
z.button('reload', function () {
    gd1.load();
});