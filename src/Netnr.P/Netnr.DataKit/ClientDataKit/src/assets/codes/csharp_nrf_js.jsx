// @namespace   netnr
// @name        csharp_nrf_js
// @date        2022-10-16
// @version     1.0.0
// @description NRF 生成 JS

async () => {
    // 输出结果
    let result = { language: "javascript", files: [] };

    // 遍历表列
    ndkGenerateCode.echoTableColumn(tableObj => {
        console.debug(tableObj)
        var codes = [];

        // 类信息
        let classInfo = {
            className: tableObj.sntn.split('.').pop(), // 类名
            classComment: tableObj.tableColumns[0].TableComment || "", //类注释
        }

        //构建代码
        codes.push('//载入');
        codes.push('var gd1 = z.Grid();');
        codes.push(`gd1.url = "/${classInfo.className}/Query${classInfo.className}?tableName=" + z.TableName;`);
        codes.push('gd1.onDblClickRow = function (index, row) {');
        codes.push('\t//双击行模拟点编辑');
        codes.push('\tz.buttonClick(\'edit\');');
        codes.push('}');
        codes.push('gd1.load();');
        codes.push('');

        codes.push('//查询');
        codes.push('z.button(\'query\', function () {');
        codes.push('\tgd1.QueryOpen();');
        codes.push('});');
        codes.push('');

        codes.push('//刷新');
        codes.push(`z.button('reload', function () {`);
        codes.push('\tgd1.load();');
        codes.push('});');
        codes.push('');

        codes.push('//新增');
        codes.push(`z.button('add', function () {`);
        codes.push('\t//表单标题');
        codes.push('\tz.FormTitle({');
        codes.push('\t\ticon: 0,');
        codes.push(`\t\ttitle: '新增${classInfo.classComment}'`);
        codes.push('\t});');
        codes.push('');
        codes.push(`\t$('#fv_modal_1').modal();`);
        codes.push('});');
        codes.push('');

        codes.push('//查看');
        codes.push(`z.button('see', function () {`);
        codes.push('\t//获取选中行');
        codes.push('\tvar rowData = gd1.func("getSelected");');
        codes.push('\tif (rowData) {');
        codes.push('\t\t//选中行回填表单');
        codes.push('\t\tz.FormEdit(rowData);');
        codes.push('\t\t//表单标题');
        codes.push('\t\tz.FormTitle({');
        codes.push('\t\t\ticon: 2,');
        codes.push(`\t\t\ttitle: '查看${classInfo.classComment}',`);
        codes.push('\t\t\trequired: false');
        codes.push('\t\t});');
        codes.push('\t\t//禁用');
        codes.push('\t\tz.FormDisabled(true);');
        codes.push('\t\t//显示模态框');
        codes.push(`\t\t$('#fv_modal_1').modal();`);
        codes.push('\t} else {');
        codes.push('\t\tart("select");');
        codes.push('\t}');
        codes.push('});');
        codes.push('');
        codes.push('//关闭模态框后');
        codes.push(`$('#fv_modal_1').on('hidden.bs.modal', function () {`);
        codes.push('\t//是查看时，解除禁用');
        codes.push('\tif (z.btnTrigger == "see") {');
        codes.push('\t\tz.FormDisabled(false);');
        codes.push('\t}');
        codes.push('});');
        codes.push('');

        codes.push('//修改');
        codes.push(`z.button('edit', function () {`);
        codes.push('\t//获取选中行');
        codes.push('\tvar rowData = gd1.func("getSelected");');
        codes.push('\tif (rowData) {');
        codes.push('\t\t//选中行回填表单');
        codes.push('\t\tz.FormEdit(rowData);');
        codes.push('\t\t//表单标题');
        codes.push('\t\tz.FormTitle({');
        codes.push('\t\t\ticon: 1,');
        codes.push(`\t\t\ttitle: '修改${classInfo.classComment}'`);
        codes.push('\t\t});');
        codes.push('\t\t//显示模态框');
        codes.push(`\t\t$('#fv_modal_1').modal();`);
        codes.push('\t} else {');
        codes.push('\t    art("select");');
        codes.push('\t}');
        codes.push('});');
        codes.push('');

        codes.push('//保存');
        codes.push(`$('#fv_save_1').click(function () {`);
        codes.push('\t//检测必填项');
        codes.push(`\tif (z.FormRequired('red')) {`);
        codes.push(`\t\t$('#fv_save_1')[0].disabled = true;`);
        codes.push('\t\t$.ajax({');
        codes.push(`\t\t\turl: "/${classInfo.className}/Save${classInfo.className}?savetype=" + z.btnTrigger,`);
        codes.push('\t\t\ttype: "post",');
        codes.push('\t\t\tdata: $("#fv_form_1").serialize(),');
        codes.push(`\t\t\tdataType: 'json',`);
        codes.push('\t\t\tsuccess: function (data) {');
        codes.push('\t\t\t\tif (data.code == 200) {');
        codes.push('\t\t\t\t\tgd1.load();');
        codes.push(`\t\t\t\t\t$('#fv_modal_1').modal('hide');`);
        codes.push('\t\t\t\t} else {');
        codes.push('\t\t\t\t\tart(data.msg);');
        codes.push('\t\t\t\t}');
        codes.push('\t\t\t},');
        codes.push('\t\t\terror: function () {');
        codes.push(`\t\t\t    art('error');`);
        codes.push('\t\t\t}');
        codes.push('\t\t});');
        codes.push(`\t\t$('#fv_save_1')[0].disabled = false;`);
        codes.push('\t}');
        codes.push('});');
        codes.push('');

        codes.push('//删除');
        codes.push(`z.button('del', function () {`);
        codes.push('\tvar rowData = gd1.func("getSelected");');
        codes.push('\tif (!rowData) {');
        codes.push(`\t\tart('select');`);
        codes.push('\t\treturn false;');
        codes.push('\t}');
        codes.push(`\tart('确定删除选中的行', function () {`);
        codes.push('\t\t$.ajax({');
        codes.push(`\t\t\turl: "/${classInfo.className}/Del${classInfo.className}?id=" + rowData.PK,`);
        codes.push(`\t\t\tdataType: 'json',`);
        codes.push('\t\t\tsuccess: function (data) {');
        codes.push('\t\t\t\tif (data.code == 200) {');
        codes.push('\t\t\t\t\tgd1.load();');
        codes.push('\t\t\t\t} else {');
        codes.push('\t\t\t\t\tart(data.msg);');
        codes.push('\t\t\t\t}');
        codes.push('\t\t\t}');
        codes.push('\t\t})');
        codes.push('\t});');
        codes.push('});');

        // 添加文件项
        result.files.push({
            fullName: `wwwroot/js/${classInfo.className}/${classInfo.className.toLowerCase()}.js`,
            content: codes.join("\r\n")
        })
    });

    // 输出
    return result;
}