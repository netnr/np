// @namespace   netnr
// @name        csharp_admin_js
// @date        2024-04-27
// @version     1.0.0
// @description Netnr.Admin 生成 js

async () => {
    // 输出结果
    let result = { language: "javascript", files: [] };

    // 遍历表列
    ndkGenerateCode.echoTableColumn(tableObj => {
        console.debug(tableObj)
        var codes = [];

        let pkCol = tableObj.tableColumns.find(col => col.PrimaryKey == 1);
        if (pkCol == null) {
            tableObj.tableColumns[0];
        }

        // 类信息
        let dt = {
            name: tableObj.sntn.split('.').pop(), // 名称
            comment: tableObj.tableColumns[0].TableComment || "", //注释
            keyName: ndkFunction.handleClassName(pkCol.ColumnName), //主键列名
        }
        let nameArray = dt.name.split('_');
        let pageController = nameArray[0];
        nameArray.shift();
        let pageAction = nameArray.join('_');
        let pageUrlPrefix = `api/${pageController}/${ndkFunction.handleClassName(dt.name)}`;

        //构建代码
        codes.push('import { nrcBase } from "../../../../frame/nrcBase";');
        codes.push('import { nrGrid } from "../../../../frame/nrGrid";');
        codes.push('import { nrApp } from "../../../../frame/Shoelace/nrApp";');
        codes.push('import { nrVary } from "../../nrVary";');
        codes.push('import { nrWeb } from "../../nrWeb";');
        codes.push('');
        codes.push('let nrPage = {');
        codes.push(`\tpathname: "/${pageController}/${pageAction}",`);
        codes.push('');
        codes.push('\trender: async () => {');
        codes.push('');
        codes.push(`\t//渲染`);
        codes.push('\tnrVary.domPanel.innerHTML = `');
        codes.push('<div class="${nrVary.flagPanelClass}">');
        codes.push('\t<div class="nrp-head row">');
        codes.push('\t\t<div class="nrp-buttons col-auto d-none"></div>');
        codes.push('\t\t<div class="col-auto">');
        codes.push('\t\t\t<sl-input class="nrp-txt-filter mb-2" placeholder="过滤" style="width:12em" clearable size="small">');
        codes.push('\t\t\t\t<sl-icon name="search" slot="prefix"></sl-icon>');
        codes.push('\t\t\t</sl-input>');
        codes.push('\t\t</div>');
        codes.push('');
        codes.push('\t</div>');
        codes.push('\t<div class="row">');
        codes.push('\t\t<div class="nrp-grid1 col-12"></div>');
        codes.push('\t</div>');
        codes.push('</div>');
        codes.push('`;');
        codes.push('\t\t//构建节点对象');
        codes.push('\t\tnrcBase.readDOM(nrVary.domPanel, "nrp", nrPage);');
        codes.push('\t\t//显示按钮');
        codes.push('\t\tawait nrWeb.viewButton(nrPage);');
        codes.push('');
        codes.push('\t\tawait nrPage.viewGrid1();');
        codes.push('');
        codes.push('\t\tawait nrPage.viewDialogForm();');
        codes.push('');
        codes.push('\t\tnrPage.bindEvent();');
        codes.push(`\t},`);
        codes.push('');

        codes.push(`\ttableTitle: "${dt.comment}", //表标题`);
        codes.push(`\ttableKey: "${dt.keyName}", //表主键列`);
        codes.push('');

        codes.push('\t/**');
        codes.push('\t * 绑定事件');
        codes.push('\t */');
        codes.push('\tbindEvent: () => {');
        codes.push('\t\t//点击按钮');
        codes.push('\t\tnrPage.domHead.addEventListener("click", async function (event) {');
        codes.push('\t\t\tlet target = event.target;');
        codes.push('\t\t\tlet btnClass = nrWeb.getButtonCommand(target);');
        codes.push('');
        codes.push('\t\t\tif (btnClass == "btn-add") {');
        codes.push('\t\t\t\tawait nrPage.domDialogForm.show();');
        codes.push('\t\t\t\tnrPage.domDialogForm.label = `${target.innerText} ${nrPage.tableTitle}`;');
        codes.push('\t\t\t\tnrWeb.setFormDisabled(nrPage.domDialogForm, false);');
        codes.push('\t\t\t} else if (btnClass == "btn-update") {');
        codes.push('\t\t\t\tlet srows = nrPage.grid1.getSelectedRows();');
        codes.push('\t\t\t\tif (srows.length == 1) {');
        codes.push('\t\t\t\t\tawait nrPage.domDialogForm.show();');
        codes.push('\t\t\t\t\tnrPage.domDialogForm.label = `${target.innerText} ${nrPage.tableTitle}`;');
        codes.push('\t\t\t\t\tnrWeb.setFormValue(nrPage.domDialogForm, srows[0]);');
        codes.push('\t\t\t\t} else {');
        codes.push('\t\t\t\t\tnrApp.alert("请选择一行");');
        codes.push('\t\t\t\t}');
        codes.push('\t\t\t} else if (btnClass == "btn-delete") {');
        codes.push('\t\t\t\tlet srows = nrPage.grid1.getSelectedRows();');
        codes.push('\t\t\t\tif (srows.length) {');
        codes.push('\t\t\t\t\tlet msg = srows.map(x => htmlraw`${x.' + dt.keyName + '}`).join(nrApp.tsHrHtml);');
        codes.push('\t\t\t\t\tif (await nrApp.confirm(msg, "确定删除")) {');
        codes.push('\t\t\t\t\t\tfor (const row of srows) {');
        codes.push('\t\t\t\t\t\t\tlet url = `${nrVary.apiHost}/' + pageUrlPrefix + 'Delete?${nrcBase.fromKeyToURLParams({ id: row[nrPage.tableKey] })}`;');
        codes.push('\t\t\t\t\t\t\tlet result = await nrWeb.reqServer(url)');
        codes.push('\t\t\t\t\t\t\tif (result.code == 200) {');
        codes.push('\t\t\t\t\t\t\t\tnrApp.toast("删除成功");');
        codes.push('\t\t\t\t\t\t\t} else {');
        codes.push('\t\t\t\t\t\t\t\tnrApp.toast(result.msg);');
        codes.push('\t\t\t\t\t\t\t}');
        codes.push('\t\t\t\t\t\t}');
        codes.push('\t\t\t\t\t\tawait nrPage.viewGrid1(); //刷新');
        codes.push('\t\t\t\t\t}');
        codes.push('\t\t\t\t} else {');
        codes.push('\t\t\t\t\tnrApp.alert("请选择行");');
        codes.push('\t\t\t\t}');
        codes.push('\t\t\t}');
        codes.push('\t\t});');
        codes.push('');
        codes.push('\t\t//过滤');
        codes.push('\t\tnrApp.setQuickFilter(nrPage.domTxtFilter, nrPage.grid1);');
        codes.push('\t},');
        codes.push('');

        codes.push('\t/**');
        codes.push('\t * 表格（Client）');
        codes.push('\t */');
        codes.push('\tviewGrid1: async () => {');
        codes.push('\t\tnrApp.setLoading(nrPage.domHead, true);');
        codes.push('');
        codes.push('\t\t//表格加载中');
        codes.push('\t\tif (nrPage.grid1) {');
        codes.push('\t\t\tnrGrid.setGridLoading(nrPage.grid1);');
        codes.push('\t\t}');
        codes.push('');
        codes.push('\t\tlet url = `${nrVary.apiHost}/' + pageUrlPrefix + 'Get`;');
        codes.push('\t\tlet result = await nrWeb.reqServer(url);');
        codes.push('\t\tif (result.code == 200) {');
        codes.push('\t\t\t//grid 列');
        codes.push('\t\t\tlet colDefs = [');
        codes.push('\t\t\t\tnrGrid.newColumnLineNumber(),');
        for (let col of tableObj.tableColumns) {
            if (col.ColumnName != pkCol.ColumnName) {
                if (col.DataType == "datetime") {
                    codes.push(`\t\t\t\tnrGrid.newColumnDate({ field: "${ndkFunction.handleClassName(col.ColumnName)}", headerName: "${col.ColumnComment}" }),`);
                } else if (col.DataType == "int" || col.DataType == "bigint") {
                    codes.push(`\t\t\t\tnrGrid.newColumnNumber({ field: "${ndkFunction.handleClassName(col.ColumnName)}", headerName: "${col.ColumnComment}", width: 120 }),`);
                } else {
                    codes.push(`\t\t\t\t{ field: "${ndkFunction.handleClassName(col.ColumnName)}", headerName: "${col.ColumnComment}", editable: true },`);
                }
            }
        }
        codes.push('\t\t\t];');
        codes.push('');
        codes.push('\t\t\t//grid 配置');
        codes.push('\t\t\tlet gridOptions = nrGrid.gridOptionsClient({');
        codes.push('\t\t\t\trowData: result.data,');
        codes.push('\t\t\t\tcolumnDefs: colDefs,');
        codes.push('\t\t\t\tgetRowId: event => event.data[nrPage.tableKey],');
        codes.push('\t\t\t\t//编辑');
        codes.push('\t\t\t\treadOnlyEdit: nrPage.domBtnUpdate == null, //是否只读');
        codes.push('\t\t\t\tonCellValueChanged: async function (event) {');
        codes.push('\t\t\t\t\tlet fd = nrcBase.fromKeyToFormData(event.data);');
        codes.push('\t\t\t\t\tlet url = `${nrVary.apiHost}/' + pageUrlPrefix + 'Put`;');
        codes.push('\t\t\t\t\tlet result = await nrWeb.reqServer(url, { method: "POST", body: fd });');
        codes.push('\t\t\t\t\tif (result.code == 200) {');
        codes.push('\t\t\t\t\t\tnrApp.toast("修改成功");');
        codes.push('\t\t\t\t\t\tevent.api.redrawRows([event.node])');
        codes.push('\t\t\t\t\t} else {');
        codes.push('\t\t\t\t\t\tnrApp.alert(result.msg);');
        codes.push('\t\t\t\t\t\tawait nrPage.viewGrid1();');
        codes.push('\t\t\t\t\t}');
        codes.push('\t\t\t\t},');
        codes.push('\t\t\t\t//双击');
        codes.push('\t\t\t\tonCellDoubleClicked: async function (event) {');
        codes.push('\t\t\t\t\tif (event.column.colId == "#line_number" && nrPage.domBtnUpdate) {');
        codes.push('\t\t\t\t\t\tnrPage.domBtnUpdate.click();');
        codes.push('\t\t\t\t\t}');
        codes.push('\t\t\t\t}');
        codes.push('\t\t\t});');
        codes.push('');
        codes.push('\t\t\t//grid dom');
        codes.push('\t\t\tnrGrid.buildDom(nrPage.domGrid1);');
        codes.push('\t\t\tnrcBase.setHeightFromBottom(nrPage.domGrid1);');
        codes.push('');
        codes.push('\t\t\t//grid 显示');
        codes.push('\t\t\tnrPage.grid1 = await nrGrid.createGrid(nrPage.domGrid1, gridOptions);');
        codes.push('\t\t} else {');
        codes.push('\t\t\tnrPage.domGrid1.innerHTML = nrApp.tsFailHtml;');
        codes.push('\t\t}');
        codes.push('');
        codes.push('\t\tnrApp.setLoading(nrPage.domHead, false);');
        codes.push('\t},');
        codes.push('');

        codes.push('\t/**');
        codes.push('\t * 表格（Infinite）');
        codes.push('\t */');
        codes.push('\tviewGrid1: async () => {');
        codes.push('\t\ttry {');
        codes.push('\t\t\t//grid 列');
        codes.push('\t\t\tlet colDefs = [');
        codes.push('\t\t\t\tnrGrid.newColumnLineNumber({ headerCheckboxSelection: false }),');
        for (let col of tableObj.tableColumns) {
            if (col.ColumnName != pkCol.ColumnName) {
                if (col.DataType == "datetime") {
                    codes.push(`\t\t\t\tnrGrid.newColumnDate({ field: "${ndkFunction.handleClassName(col.ColumnName)}", headerName: "${col.ColumnComment}" }),`);
                } else if (col.DataType == "int" || col.DataType == "bigint") {
                    codes.push(`\t\t\t\tnrGrid.newColumnNumber({ field: "${ndkFunction.handleClassName(col.ColumnName)}", headerName: "${col.ColumnComment}", width: 120 }),`);
                } else {
                    codes.push(`\t\t\t\t{ field: "${ndkFunction.handleClassName(col.ColumnName)}", headerName: "${col.ColumnComment}", editable: true },`);
                }
            }
        }
        codes.push('\t\t\t];');
        codes.push('\t\t\tnrApp.setGridColumnLoading(colDefs);');
        codes.push('');
        codes.push('\t\t\t//grid 配置');
        codes.push('\t\t\tlet gridOptions = await nrGrid.gridOptionsInfinite({');
        codes.push('\t\t\t\tgetRowId: event => event.data[nrPage.tableKey],');
        codes.push('\t\t\t\tcolumnDefs: colDefs,');
        codes.push('');
        codes.push('\t\t\t\t//编辑');
        codes.push('\t\t\t\treadOnlyEdit: nrPage.domBtnUpdate == null, //是否只读');
        codes.push('\t\t\t\tonCellValueChanged: async function (event) {');
        codes.push('\t\t\t\t\tlet fd = nrcBase.fromKeyToFormData(event.data);');
        codes.push('\t\t\t\t\tlet url = `${nrVary.apiHost}/' + pageUrlPrefix + 'Put`;');
        codes.push('\t\t\t\t\tlet result = await nrWeb.reqServer(url, { method: "POST", body: fd });');
        codes.push('\t\t\t\t\tif (result.code == 200) {');
        codes.push('\t\t\t\t\t\tnrApp.toast("修改成功");');
        codes.push('\t\t\t\t\t} else {');
        codes.push('\t\t\t\t\t\tnrApp.alert(result.msg);');
        codes.push('\t\t\t\t\t\tevent.api.refreshInfiniteCache(); //刷新');
        codes.push('\t\t\t\t\t}');
        codes.push('\t\t\t\t},');
        codes.push('\t\t\t\t//双击');
        codes.push('\t\t\t\tonCellDoubleClicked: async function (event) {');
        codes.push('\t\t\t\t\tif (event.column.colId == "#line_number" && nrPage.domBtnUpdate) {');
        codes.push('\t\t\t\t\t\tnrPage.domBtnUpdate.click();');
        codes.push('\t\t\t\t\t}');
        codes.push('\t\t\t\t}');
        codes.push('\t\t\t}, (params) => {');
        codes.push('\t\t\t\tnrApp.setLoading(nrPage.domHead, true);');
        codes.push('');
        codes.push('\t\t\t\tlet kvObj = { paramsJson: JSON.stringify(params) }');
        codes.push('\t\t\t\tlet url = `/' + pageUrlPrefix + 'Get?${nrcBase.fromKeyToURLParams(kvObj)}`;');
        codes.push('\t\t\t\treturn url;');
        codes.push('\t\t\t}, () => {');
        codes.push('\t\t\t\tnrApp.setLoading(nrPage.domHead, false);');
        codes.push('\t\t\t});');
        codes.push('');
        codes.push('\t\t\t//grid dom');
        codes.push('\t\t\tnrGrid.buildDom(nrPage.domGrid1);');
        codes.push('\t\t\tnrcBase.setHeightFromBottom(nrPage.domGrid1);');
        codes.push('');
        codes.push('\t\t\t//grid 显示');
        codes.push('\t\t\tnrPage.grid1 = await nrGrid.createGrid(nrPage.domGrid1, gridOptions);');
        codes.push('\t\t} catch (error) {');
        codes.push('\t\t\tnrApp.logError(error);');
        codes.push('\t\t\tnrPage.domGrid1.innerHTML = nrApp.tsFailHtml;');
        codes.push('\t\t}');
        codes.push('\t},');
        codes.push('');

        codes.push('\t/**');
        codes.push('\t * Form');
        codes.push('\t */');
        codes.push('\tviewDialogForm: async () => {');
        codes.push('\t\tif (nrPage.domDialogForm == null) {');
        codes.push('\t\t\tlet domDialog = nrPage.domDialogForm = document.createElement("sl-dialog");');
        codes.push('\t\t\tdomDialog.className = `nrg-dialog-form`;');
        codes.push('\t\t\tdomDialog.style = "--width:85em"');
        codes.push('\t\t\tdomDialog.innerHTML = `<sl-button slot="header-actions" data-name="save" variant="primary" outline size="small">保存</sl-button>');
        codes.push('<form class="row">');
        codes.push('\t<input style="display:none" name="${nrPage.tableKey}" />');
        codes.push('');
        for (let col of tableObj.tableColumns) {
            if (col.ColumnName != pkCol.ColumnName && col.ColumnName != "create_time") {
                codes.push('\t<div class="col-md-6 mb-3">');
                if (col.DataType == "datetime") {
                    codes.push(`\t\t<sl-input name="${ndkFunction.handleClassName(col.ColumnName)}" label="${col.ColumnComment}" type="datetime-local" ${col.IsNullable == 1 ? "" : "required"}></sl-input>`);
                }
                else if (col.ColumnName.includes("status")) {
                    codes.push(`\t\t<sl-select name="${ndkFunction.handleClassName(col.ColumnName)}" label="${col.ColumnComment}" ${col.IsNullable == 1 ? "" : "required"}></sl-select>`);
                } else {
                    codes.push(`\t\t<sl-input name="${ndkFunction.handleClassName(col.ColumnName)}" label="${col.ColumnComment}" maxlength="${col.DataLength}" ${col.IsNullable == 1 ? "" : "required"}></sl-input>`);
                }
                codes.push('\t</div>');
            }
        }
        codes.push('</form>');
        codes.push('`;');
        codes.push('\t\t\tnrVary.domPanel.appendChild(domDialog);');
        codes.push('');
        codes.push('\t\t\t//阻止遮罩层关闭');
        codes.push('\t\t\tnrApp.dialogSuppressHide(domDialog);');
        codes.push('');
        let bindCols = tableObj.tableColumns.filter(x => x.ColumnName.includes("status"));
        if (bindCols.length) {
            codes.push('\t\t\t//下拉列表');
            bindCols.forEach(col => {
                codes.push(`\t\t\tnrWeb.bindSelect(domDialog.querySelector('[name="${ndkFunction.handleClassName(col.ColumnName)}"]'), nrVary.vtDisabled);`);
            })
            codes.push('');
        }
        codes.push('\t\t\t//显示时');
        codes.push('\t\t\tdomDialog.addEventListener("sl-show", async function (event) {');
        codes.push('\t\t\t\tif (event.target == this) {');
        codes.push('\t\t\t\t\tthis.querySelector("form").reset(); //重置');
        codes.push('\t\t\t\t}');
        codes.push('\t\t\t});');
        codes.push('');
        codes.push('\t\t\t//提交');
        codes.push('\t\t\tdomDialog.querySelector("form").addEventListener("submit", async function (event) {');
        codes.push('\t\t\t\tevent.preventDefault();');
        codes.push('');
        codes.push('\t\t\t\tlet fd = new FormData(this);');
        codes.push('\t\t\t\tlet pkey = fd.get(nrPage.tableKey);');
        codes.push('');
        codes.push('\t\t\t\tlet url = pkey == ""');
        codes.push('\t\t\t\t\t? `${nrVary.apiHost}/' + pageUrlPrefix + 'Post`');
        codes.push('\t\t\t\t\t: `${nrVary.apiHost}/' + pageUrlPrefix + 'Put`;');
        codes.push('');
        codes.push('\t\t\t\tlet domSave = domDialog.querySelector(\'[data-name="save"]\');');
        codes.push('\t\t\t\tnrApp.setLoading(domSave, true);');
        codes.push('\t\t\t\tlet result = await nrWeb.reqServer(url, { method: "POST", body: fd });');
        codes.push('\t\t\t\tnrApp.setLoading(domSave, false);');
        codes.push('');
        codes.push('\t\t\t\tif (result.code == 200) {');
        codes.push('\t\t\t\t\tnrApp.toast("保存成功");');
        codes.push('\t\t\t\t\tthis.parentNode.hide();');
        codes.push('\t\t\t\t\t//!! 刷新二选一');
        codes.push('\t\t\t\t\tnrPage.grid1.refreshInfiniteCache();//刷新');
        codes.push('\t\t\t\t\tawait nrPage.viewGrid1();//刷新');
        codes.push('\t\t\t\t} else {');
        codes.push('\t\t\t\t\tnrApp.alert(result.msg, "保存失败");');
        codes.push('\t\t\t\t}');
        codes.push('');
        codes.push('\t\t\t});');
        codes.push('\t\t}');
        codes.push('\t},');

        codes.push('}');
        codes.push('');
        codes.push('export { nrPage };');

        // 添加文件项
        result.files.push({
            fullName: `${pageController}/nrPage${ndkFunction.handleClassName(dt.name)}.js`,
            content: codes.join("\r\n")
        })
    });

    // 输出
    return result;
}