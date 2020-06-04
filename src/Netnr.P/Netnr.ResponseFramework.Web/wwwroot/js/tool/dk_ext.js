if (window.dk) {
    /**
     * 加载已生成虚拟表
     */
    dk.loadHasTableConfig = function () {
        $('.dkn-loadTableConfigWait').addClass('fa-spin');
        var url = dk.api + dk.apiPath["QueryTableConfig"];
        $.ajax({
            url: url,
            dataType: 'json',
            headers: dk.apiHeaders,
            success: function (data) {
                dk.dc.hasTableConfig = data.code == 200 ? data.data : [];
                $('#btnQueryTable')[0].click();
            },
            error: function () {
                $('#btnQueryTable')[0].click();
            },
            complete: function () {
                $('.dkn-loadTableConfigWait').removeClass('fa-spin');
            }
        })
    }
    dk.loadHasTableConfig();

    /**
     * 生成表配置
     * @@param {any} t 追加或覆盖
     */
    dk.buildTableConfig = function (t) {
        var rowDatas = $('#Grid2').datagrid('getSelections');
        if (rowDatas.length) {
            var tcrows = [];
            $.each(rowDatas, function (i, row) {
                var tcrow = {
                    "Id": null,
                    "TableName": row.TableName,
                    "ColField": row.FieldName,
                    "DvTitle": row.TableComment || row.FieldName,
                    "ColTitle": row.TableComment || row.FieldName,
                    "ColWidth": 100,
                    "ColAlign": 1,
                    "ColHide": 0,
                    "ColOrder": row.FieldOrder,
                    "ColFrozen": 0,
                    "ColFormat": "0",
                    "ColSort": 0,
                    "ColExport": 1,
                    "ColQuery": 0,
                    "ColRelation": "",
                    "FormArea": 1,
                    "FormUrl": null,
                    "FormType": "text",
                    "FormSpan": 1,
                    "FormHide": 0,
                    "FormOrder": row.FieldOrder,
                    "FormRequired": 0,
                    "FormPlaceholder": null,
                    "FormValue": null,
                    "FormText": null,
                    "FormMaxlength": row.DataLength
                };
                tcrows.push(tcrow);
            });

            //保存表配置
            var url = dk.api + dk.apiPath['SaveTableConfig'];
            $.ajax({
                url: url,
                data: {
                    rows: JSON.stringify(tcrows),
                    buildType: t
                },
                type: "post",
                dataType: 'json',
                headers: dk.apiHeaders,
                success: function (data) {
                    //渲染表信息
                    if (data.code == 200) {
                        dk.loadHasTableConfig();
                    }
                    dk.msg(data.msg);
                },
                error: function (err) {
                    console.log(err);
                    dk.msgError(url);
                }
            })
        } else {
            dk.msg('请选择列')
        }
    }

    //表渲染前回调
    dk.viewTableBefore = function (dgo) {
        dgo.columns[0].splice(2, 0, { field: "HasTableConfig", title: "表配置", align: "center", formatter: function (value) { return value ? "✔" : "" } });
        $.each(dgo.data, function () {
            this.HasTableConfig = dk.dc.hasTableConfig.indexOf(this.TableName) >= 0;
        });
    }

    //添加刷新表配置按钮
    $('<button class="btn btn-sm btn-primary"><i class="fa fa-fw fa-refresh dkn-loadTableConfigWait"></i>刷新</button>').insertAfter($('#btnQueryTable')).click(function () {
        dk.loadHasTableConfig();
    });
    //添加生成表配置按钮
    $(['<div class="btn-group btn-group-sm">',
        '<button class="btn btn-sm btn-primary" data-toggle="dropdown">生成表配置<i class="fa fa-fw fa-caret-down"></i></button>',
        '<div class="dropdown-menu">',
        '<a class="dropdown-item p-2" href="javascript:void(0);" id="btnBuildTableConfigForAdd"><i class="fa fa-fw fa-plus"></i>追加不存在的字段</a>',
        '<a class="dropdown-item p-2" href="javascript:void(0);" id="btnBuildTableConfigForCover"><i class="fa fa-fw fa-eraser"></i>覆盖更新</a>',
        '</div>',
        '</div>'].join('')).insertAfter($('#btnQueryColumn'));
    $('#btnBuildTableConfigForAdd').click(function () {
        dk.buildTableConfig(1);
    });
    $('#btnBuildTableConfigForCover').click(function () {
        dk.buildTableConfig(2);
    });


    //重构
    dk.build.language = {
        csharp: {},
        "csharp-comment": "（netnrf 生成通用代码）",
        javascript: {},
        "javascript-comment": "（netnrf 生成通用代码）",
    }

    //构建控制器
    dk.build.language.csharp["controller"] = function am(pa) {
        //参数默认值
        pa.project = pa.project || "Netnr.ResponseFramework.Web";
        pa.namespace = pa.namespace || "Netnr.ResponseFramework.Web";
        pa.ext = pa.ext || ".cs";

        var modelNamespace = "Netnr.ResponseFramework.Domain",
            dataNamespace = "Netnr.ResponseFramework.Data";

        //按表名分组
        var rg = dk.build.rowsGroup(pa.rows);

        //遍历表再遍历列构建代码【主要修改以下代码】
        for (var gn in rg) {
            var ts = [], rs = rg[gn];
            var pkcolumn, tableComment = rs[0].TableComment.split('\n').join('  ') || gn;

            for (var i = 0; i < rs.length; i++) {
                var ri = rs[i];
                if (ri.PrimaryKey == "YES") {
                    pkcolumn = ri;
                }
            }
            //无主键列，默认第一个字段
            if (!pkcolumn) {
                pkcolumn = rs[0];
            }

            //构建代码
            ts.push('using Microsoft.AspNetCore.Authorization;');
            ts.push('using Microsoft.AspNetCore.Mvc;');
            ts.push('using ' + modelNamespace + ';');
            ts.push('using ' + dataNamespace + ';');
            ts.push('using System;');
            ts.push('');
            ts.push('namespace ' + pa.namespace + '.Controllers');
            ts.push('{');
            ts.push('\t' + '/// <summary>');
            ts.push('\t' + '/// ' + tableComment);
            ts.push('\t' + '/// </summary>');
            ts.push('\t' + '[Authorize]');
            ts.push('\t' + '[Route("[controller]/[action]")]');
            ts.push('\t' + 'public class ' + gn + 'Controller : Controller');
            ts.push('\t' + '{');
            ts.push('\t\t' + 'public ContextBase db;');
            ts.push('\t\t' + 'public ' + gn + 'Controller(ContextBase cb)');
            ts.push('\t\t' + '{');
            ts.push('\t\t\t' + 'db = cb;');
            ts.push('\t\t' + '}');
            ts.push('');
            ts.push('\t\t' + '/// <summary>');
            ts.push('\t\t' + '/// ' + tableComment + '页面');
            ts.push('\t\t' + '/// </summary>');
            ts.push('\t\t' + '/// <returns></returns>');
            ts.push('\t\t' + '[ApiExplorerSettings(IgnoreApi = true)]');
            ts.push('\t\t' + 'public IActionResult ' + gn + '()');
            ts.push('\t\t' + '{');
            ts.push('\t\t\t' + 'return View();');
            ts.push('\t\t' + '}');
            ts.push('');
            ts.push('\t\t' + '/// <summary>');
            ts.push('\t\t' + '/// 查询' + tableComment);
            ts.push('\t\t' + '/// </summary>');
            ts.push('\t\t' + '/// <param name="ivm"></param>');
            ts.push('\t\t' + '/// <returns></returns>');
            ts.push('\t\t' + '[HttpGet]');
            ts.push('\t\t' + '[HttpPost]');
            ts.push('\t\t' + 'public QueryDataOutputVM Query' + gn + '(QueryDataInputVM ivm)');
            ts.push('\t\t' + '{');
            ts.push('\t\t\t' + 'var ovm = new QueryDataOutputVM();');
            ts.push('');
            ts.push('\t\t\t' + 'var query = db.' + gn + ';');
            ts.push('\t\t\t' + 'Application.Common.QueryJoin(query, ivm, db, ref ovm);');
            ts.push('');
            ts.push('\t\t\t' + 'return ovm;');
            ts.push('\t\t' + '}');
            ts.push('');
            ts.push('\t\t' + '/// <summary>');
            ts.push('\t\t' + '/// 保存' + tableComment);
            ts.push('\t\t' + '/// </summary>');
            ts.push('\t\t' + '/// <param name="mo"></param>');
            ts.push('\t\t' + '/// <param name="savetype">保存类型</param>');
            ts.push('\t\t' + '/// <returns></returns>');
            ts.push('\t\t' + '[HttpPost]');
            ts.push('\t\t' + 'public ActionResultVM Save' + gn + '(' + gn + ' mo, string savetype)');
            ts.push('\t\t' + '{');
            ts.push('\t\t\t' + 'var vm = new ActionResultVM();');
            ts.push('');
            ts.push('\t\t\t' + 'if (savetype == "add")');
            ts.push('\t\t\t' + '{');
            ts.push('\t\t\t\t' + 'mo.' + pkcolumn.FieldName + ' = Guid.NewGuid().ToString();');
            ts.push('');
            ts.push('\t\t\t\t' + 'db.' + gn + '.Add(mo);');
            ts.push('\t\t\t' + '}');
            ts.push('\t\t\t' + 'else');
            ts.push('\t\t\t' + '{');
            ts.push('\t\t\t\t' + 'db.' + gn + '.Update(mo);');
            ts.push('\t\t\t' + '}');
            ts.push('');
            ts.push('\t\t\t' + 'int num = db.SaveChanges();');
            ts.push('');
            ts.push('\t\t\t' + 'vm.Set(num > 0);');
            ts.push('');
            ts.push('\t\t\t' + 'return vm;');
            ts.push('\t\t' + '}');
            ts.push('');
            ts.push('\t\t' + '/// <summary>');
            ts.push('\t\t' + '/// 删除' + tableComment);
            ts.push('\t\t' + '/// </summary>');
            ts.push('\t\t' + '/// <param name="id"></param>');
            ts.push('\t\t' + '/// <returns></returns>');
            ts.push('\t\t' + '[HttpGet]');
            ts.push('\t\t' + 'public ActionResultVM Del' + gn + '(string id)');
            ts.push('\t\t' + '{');
            ts.push('\t\t\t' + 'var vm = new ActionResultVM();');
            ts.push('');
            ts.push('\t\t\t' + 'var mo = db.' + gn + '.Find(id);');
            ts.push('\t\t\t' + 'if (mo != null)');
            ts.push('\t\t\t' + '{');
            ts.push('\t\t\t\t' + 'db.' + gn + '.Remove(mo);');
            ts.push('\t\t\t\t' + 'int num = db.SaveChanges();');
            ts.push('');
            ts.push('\t\t\t\t' + 'vm.Set(num > 0);');
            ts.push('\t\t\t' + '}');
            ts.push('\t\t\t' + 'else');
            ts.push('\t\t\t' + '{');
            ts.push('\t\t\t\t' + 'vm.Set(ARTag.invalid);');
            ts.push('\t\t\t' + '}');
            ts.push('');
            ts.push('\t\t\t' + 'return vm;');
            ts.push('\t\t' + '}');
            ts.push('\t' + '}');
            ts.push('}');

            pa.output['Controllers/' + gn + "Controller" + pa.ext] = ts;
        }
    }

    //构建视图
    dk.build.language.csharp["view"] = function am(pa) {
        //参数默认值
        pa.project = pa.project || "Netnr.ResponseFramework.Web";
        pa.namespace = pa.namespace || "Netnr.ResponseFramework.Web";
        pa.ext = pa.ext || ".cshtml";

        //按表名分组
        var rg = dk.build.rowsGroup(pa.rows);

        //遍历表再遍历列构建代码【主要修改以下代码】
        for (var gn in rg) {
            var ts = [], rs = rg[gn];
            var tableComment = rs[0].TableComment.split('\n').join('  ').split('"').join("'") || gn;

            //构建代码
            ts.push('@@{');
            ts.push('\t' + 'ViewData["Title"] = "' + tableComment + '";');
            ts.push('}');
            ts.push('');
            ts.push('@@await Component.InvokeAsync("Form", new InvokeFormVM()');
            ts.push('{');
            ts.push('\t' + 'TableName = "' + gn + '"');
            ts.push('})');
            ts.push('');
            ts.push('<partial name="_BaseViewPartial" />');

            pa.output['Views/' + gn + '/' + gn + pa.ext] = ts;
        }
    }

    //构建js
    dk.build.language.javascript["js"] = function am(pa) {
        //参数默认值
        pa.project = pa.project || "Netnr.ResponseFramework.Web";
        pa.namespace = pa.namespace || "Netnr.ResponseFramework.Web";
        pa.ext = pa.ext || ".js";

        //按表名分组
        var rg = dk.build.rowsGroup(pa.rows);

        //遍历表再遍历列构建代码【主要修改以下代码】
        for (var gn in rg) {
            var ts = [], rs = rg[gn];
            var pkcolumn, tableComment = rs[0].TableComment.split('\n').join('  ');

            for (var i = 0; i < rs.length; i++) {
                var ri = rs[i];
                if (ri.PrimaryKey == "YES") {
                    pkcolumn = ri;
                }
            }
            //无主键列，默认第一个字段
            if (!pkcolumn) {
                pkcolumn = rs[0];
            }

            //构建代码
            ts.push('//载入');
            ts.push('var gd1 = z.Grid();');
            ts.push('gd1.url = "/' + gn + '/Query' + gn + '?tableName=" + z.TableName;');
            ts.push('gd1.onDblClickRow = function (index, row) {');
            ts.push('\t' + '//双击行模拟点编辑');
            ts.push('\t' + 'z.buttonClick(\'edit\');');
            ts.push('}');
            ts.push('gd1.load();');
            ts.push('');
            ts.push('//刷新');
            ts.push('z.button(\'reload\', function () {');
            ts.push('\t' + 'gd1.load();');
            ts.push('});');
            ts.push('');
            ts.push('//新增');
            ts.push('z.button(\'add\', function () {');
            ts.push('\t' + '//表单标题');
            ts.push('\t' + 'z.FormTitle({');
            ts.push('\t\t' + 'icon: 0,');
            ts.push('\t\t' + 'title: \'新增' + tableComment + '\'');
            ts.push('\t' + '});');
            ts.push('');
            ts.push('\t' + '$(\'#fv_modal_1\').modal();');
            ts.push('});');
            ts.push('');
            ts.push('//查看');
            ts.push('z.button(\'see\', function () {');
            ts.push('\t' + '//获取选中行');
            ts.push('\t' + 'var rowData = gd1.func("getSelected");');
            ts.push('\t' + 'if (rowData) {');
            ts.push('\t\t' + '//选中行回填表单');
            ts.push('\t\t' + 'z.FormEdit(rowData);');
            ts.push('\t\t' + '//表单标题');
            ts.push('\t\t' + 'z.FormTitle({');
            ts.push('\t\t\t' + 'icon: 2,');
            ts.push('\t\t\t' + 'title: \'查看' + tableComment + '\',');
            ts.push('\t\t\t' + 'required: false');
            ts.push('\t\t' + '});');
            ts.push('\t\t' + '//禁用');
            ts.push('\t\t' + 'z.FormDisabled(true);');
            ts.push('\t\t' + '//显示模态框');
            ts.push('\t\t' + '$(\'#fv_modal_1\').modal();');
            ts.push('\t' + '} else {');
            ts.push('\t\t' + 'art("select");');
            ts.push('\t' + '}');
            ts.push('});');
            ts.push('');
            ts.push('//关闭模态框后');
            ts.push('$(\'#fv_modal_1\').on(\'hidden.bs.modal\', function () {');
            ts.push('\t' + '//是查看时，解除禁用');
            ts.push('\t' + 'if (z.btnTrigger == "see") {');
            ts.push('\t\t' + 'z.FormDisabled(false);');
            ts.push('\t' + '}');
            ts.push('});');
            ts.push('');
            ts.push('//修改');
            ts.push('z.button(\'edit\', function () {');
            ts.push('\t' + '//获取选中行');
            ts.push('\t' + 'var rowData = gd1.func("getSelected");');
            ts.push('\t' + 'if (rowData) {');
            ts.push('\t\t' + '//选中行回填表单');
            ts.push('\t\t' + 'z.FormEdit(rowData);');
            ts.push('\t\t' + '//表单标题');
            ts.push('\t\t' + 'z.FormTitle({');
            ts.push('\t\t\t' + 'icon: 1,');
            ts.push('\t\t\t' + 'title: \'修改' + tableComment + '\'');
            ts.push('\t\t' + '});');
            ts.push('\t\t' + '//显示模态框');
            ts.push('\t\t' + '$(\'#fv_modal_1\').modal();');
            ts.push('\t' + '} else {');
            ts.push('\t' + '    art("select");');
            ts.push('\t' + '}');
            ts.push('});');
            ts.push('');
            ts.push('//保存');
            ts.push('$(\'#fv_save_1\').click(function () {');
            ts.push('\t' + '//检测必填项');
            ts.push('\t' + 'if (z.FormRequired(\'red\')) {');
            ts.push('\t\t' + '$(\'#fv_save_1\')[0].disabled = true;');
            ts.push('\t\t' + '$.ajax({');
            ts.push('\t\t\t' + 'url: "/' + gn + '/Save' + gn + '?savetype=" + z.btnTrigger,');
            ts.push('\t\t\t' + 'type: "post",');
            ts.push('\t\t\t' + 'data: $("#fv_form_1").serialize(),');
            ts.push('\t\t\t' + 'dataType: \'json\',');
            ts.push('\t\t\t' + 'success: function (data) {');
            ts.push('\t\t\t\t' + 'if (data.code == 200) {');
            ts.push('\t\t\t\t\t' + 'gd1.load();');
            ts.push('\t\t\t\t\t' + '$(\'#fv_modal_1\').modal(\'hide\');');
            ts.push('\t\t\t\t' + '} else {');
            ts.push('\t\t\t\t\t' + 'art(data.msg);');
            ts.push('\t\t\t\t' + '}');
            ts.push('\t\t\t' + '},');
            ts.push('\t\t\t' + 'error: function () {');
            ts.push('\t\t\t' + '    art(\'error\');');
            ts.push('\t\t\t' + '}');
            ts.push('\t\t' + '});');
            ts.push('\t\t' + '$(\'#fv_save_1\')[0].disabled = false;');
            ts.push('\t' + '}');
            ts.push('});');
            ts.push('');
            ts.push('//删除');
            ts.push('z.button(\'del\', function () {');
            ts.push('\t' + 'var rowData = gd1.func("getSelected");');
            ts.push('\t' + 'if (!rowData) {');
            ts.push('\t\t' + 'art(\'select\');');
            ts.push('\t\t' + 'return false;');
            ts.push('\t' + '}');
            ts.push('\t' + 'art(\'确定删除选中的行\', function () {');
            ts.push('\t\t' + '$.ajax({');
            ts.push('\t\t\t' + 'url: "/' + gn + '/Del' + gn + '?id=" + rowData.' + pkcolumn.FieldName + ',');
            ts.push('\t\t\t' + 'dataType: \'json\',');
            ts.push('\t\t\t' + 'success: function (data) {');
            ts.push('\t\t\t\t' + 'if (data.code == 200) {');
            ts.push('\t\t\t\t\t' + 'gd1.load();');
            ts.push('\t\t\t\t' + '} else {');
            ts.push('\t\t\t\t\t' + 'art(data.msg);');
            ts.push('\t\t\t\t' + '}');
            ts.push('\t\t\t' + '}');
            ts.push('\t\t' + '})');
            ts.push('\t' + '});');
            ts.push('});');

            pa.output[('wwwroot/js/' + gn + '/' + gn + pa.ext).toLowerCase()] = ts;
        }
    }

    //重新渲染构建
    dk.viewBuild();

    //关闭加载提示
    if (dk.dc.loadingMask) {
        dk.dc.loadingMask.modal('hide');
    }
}