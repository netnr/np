// @namespace   netnr
// @name        csharp_nrf_cv
// @date        2022-10-16
// @version     1.0.0
// @description NRF 生成 Controllers Views

async () => {
    // 输出结果
    let result = { language: "csharp", files: [] };

    // 遍历表列
    ndkGenerateCode.echoTableColumn(tableObj => {
        console.debug(tableObj)
        var codes = [];

        // 类信息
        let classInfo = {
            namespace: "Netnr.ResponseFramework.Web.Controllers",  //类空间
            className: tableObj.sntn.split('.').pop(),  //类名
            classComment: tableObj.tableColumns[0].TableComment || "", //类注释
        }

        // 构建代码 Controllers
        codes.push(`namespace ${classInfo.namespace}`);
        codes.push('{');
        codes.push('\t/// <summary>');
        codes.push(`\t/// ${classInfo.classComment}`);
        codes.push('\t/// </summary>');
        codes.push('\t[Authorize]');
        codes.push('\t[Route("[controller]/[action]")]');
        codes.push(`\tpublic class ${classInfo.className}Controller : Controller`);
        codes.push('\t{');

        codes.push('\t\tpublic ContextBase db;');
        codes.push('');
        codes.push(`\t\tpublic ${classInfo.className}Controller(ContextBase cb)`);
        codes.push('\t\t{');
        codes.push('\t\t\tdb = cb;');
        codes.push('\t\t}');
        codes.push('');

        codes.push('\t\t/// <summary>');
        codes.push(`\t\t/// ${classInfo.classComment} 页面`);
        codes.push('\t\t/// </summary>');
        codes.push('\t\t/// <returns></returns>');
        codes.push('\t\t[ApiExplorerSettings(IgnoreApi = true)]');
        codes.push(`\t\tpublic IActionResult ${classInfo.className}()`);
        codes.push('\t\t{');
        codes.push('\t\t\treturn View();');
        codes.push('\t\t}');
        codes.push('');

        codes.push('\t\t/// <summary>');
        codes.push(`\t\t/// ${classInfo.classComment} 查询`);
        codes.push('\t\t/// </summary>');
        codes.push('\t\t/// <param name="ivm"></param>');
        codes.push('\t\t/// <returns></returns>');
        codes.push('\t\t[HttpGet]');
        codes.push('\t\t[HttpPost]');
        codes.push(`\t\tpublic QueryDataOutputVM Query${classInfo.className}(QueryDataInputVM ivm)`);
        codes.push('\t\t{');
        codes.push('\t\t\tvar ovm = new QueryDataOutputVM();');
        codes.push('');
        codes.push(`\t\t\tvar query = db.${classInfo.className};`);
        codes.push('\t\t\tCommonService.QueryJoin(query, ivm, db, ref ovm);');
        codes.push('');
        codes.push('\t\t\treturn ovm;');
        codes.push('\t\t}');
        codes.push('');

        codes.push('\t\t/// <summary>');
        codes.push(`\t\t/// ${classInfo.classComment} 保存`);
        codes.push('\t\t/// </summary>');
        codes.push('\t\t/// <param name="mo"></param>');
        codes.push('\t\t/// <param name="savetype">保存类型</param>');
        codes.push('\t\t/// <returns></returns>');
        codes.push('\t\t[HttpPost]');
        codes.push(`\t\tpublic ResultVM Save${classInfo.className}(${classInfo.className} mo, string savetype)`);
        codes.push('\t\t{');
        codes.push('\t\t\tvar vm = new ResultVM();');
        codes.push('');
        codes.push('\t\t\tif (savetype == "add")');
        codes.push('\t\t\t{');
        codes.push(`\t\t\t\tdb.${classInfo.className}.Add(mo);`);
        codes.push('\t\t\t}');
        codes.push('\t\t\telse');
        codes.push('\t\t\t{');
        codes.push(`\t\t\t\tdb.${classInfo.className}.Update(mo);`);
        codes.push('\t\t\t}');
        codes.push('');
        codes.push('\t\t\tint num = db.SaveChanges();');
        codes.push('');
        codes.push('\t\t\tvm.Set(num > 0);');
        codes.push('');
        codes.push('\t\t\treturn vm;');
        codes.push('\t\t}');
        codes.push('');

        codes.push('\t\t/// <summary>');
        codes.push(`\t\t/// ${classInfo.classComment} 删除`);
        codes.push('\t\t/// </summary>');
        codes.push('\t\t/// <param name="id"></param>');
        codes.push('\t\t/// <returns></returns>');
        codes.push('\t\t[HttpGet]');
        codes.push(`\t\tpublic ResultVM Del${classInfo.className}(string id)`);
        codes.push('\t\t{');
        codes.push('\t\t\tvar vm = new ResultVM();');
        codes.push('');
        codes.push(`\t\t\tvar mo = db.${classInfo.className}.Find(id);`);
        codes.push('\t\t\tif (mo != null)');
        codes.push('\t\t\t{');
        codes.push(`\t\t\t\tdb.${classInfo.className}.Remove(mo);`);
        codes.push('\t\t\t\tint num = db.SaveChanges();');
        codes.push('');
        codes.push('\t\t\t\tvm.Set(num > 0);');
        codes.push('\t\t\t}');
        codes.push('\t\t\telse');
        codes.push('\t\t\t{');
        codes.push('\t\t\t\tvm.Set(ARTag.invalid);');
        codes.push('\t\t\t}');
        codes.push('');
        codes.push('\t\t\treturn vm;');
        codes.push('\t\t}');
        codes.push('\t}');
        codes.push('}');

        // 添加文件项
        result.files.push({
            fullName: `Controllers/${classInfo.className}Controller.cs`,
            content: codes.join("\r\n")
        })


        // 构建代码 Views
        codes = [];
        codes.push('@{');
        codes.push(`\tViewData["Title"] = "${classInfo.classComment}";`);
        codes.push('}');
        codes.push('');
        codes.push('@await Component.InvokeAsync("Form", new InvokeFormVM()');
        codes.push('{');
        codes.push(`\tTableName = "${classInfo.className}"`);
        codes.push('})');
        codes.push('');
        codes.push('<partial name="_BaseViewPartial" />');

        // 添加文件项
        result.files.push({
            fullName: `Views/${classInfo.className}/${classInfo.className}.cshtml`,
            content: codes.join("\r\n")
        })
    });

    // 输出
    return result;
}