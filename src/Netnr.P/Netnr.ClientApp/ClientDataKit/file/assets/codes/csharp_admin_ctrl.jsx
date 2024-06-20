// @namespace   netnr
// @name        csharp_admin_ctrl
// @date        2024-04-27
// @version     1.0.0
// @description Netnr.Admin 生成 Controllers

async () => {
    // 输出结果
    let result = { language: "csharp", files: [] };

    // 遍历表列
    ndkGenerateCode.echoTableColumn(tableObj => {
        console.debug(tableObj)
        let codes = [];

        // 类信息
        let classInfo = {
            namespace: "Netnr.Admin.Web.Controllers",  //类空间
            className: ndkFunction.handleClassName(tableObj.sntn.split('.').pop()),  //类名
            classComment: tableObj.tableColumns[0].TableComment || "", //类注释
        }

        // 构建代码 Controllers
        codes.push(`namespace ${classInfo.namespace}`);
        codes.push('{');
        codes.push('\t/// <summary>');
        codes.push(`\t/// ${classInfo.classComment}`);
        codes.push('\t/// </summary>');
        codes.push('\t[Authorize]');
        codes.push(`\tpublic class ${classInfo.className}Controller(ContextBase cb) : WebController`);
        codes.push('\t{');

        codes.push('\t\tpublic ContextBase db = cb;');
        codes.push('');

        codes.push('\t\t/// <summary>');
        codes.push(`\t\t/// 查询${classInfo.classComment}列表`);
        codes.push('\t\t/// </summary>');
        codes.push('\t\t/// <returns></returns>');
        codes.push('\t\t[HttpGet]');
        codes.push(`\t\tpublic async Task<ResultVM> ${classInfo.className}Get()`);
        codes.push('\t\t{');
        codes.push('\t\t\tvar vm = new ResultVM');
        codes.push('\t\t\t{');
        codes.push(`\t\t\t\tData = await db.${classInfo.className}.ToListAsync()`);
        codes.push('\t\t\t};');
        codes.push('\t\t\tvm.Set(RCodeTypes.success);');
        codes.push('');
        codes.push('\t\t\treturn vm;');
        codes.push('\t\t}');
        codes.push('');

        codes.push('\t\t/// <summary>');
        codes.push(`\t\t/// 查询${classInfo.classComment}列表`);
        codes.push('\t\t/// </summary>');
        codes.push('\t\t/// <param name="paramsJson"></param>');
        codes.push('\t\t/// <returns></returns>');
        codes.push('\t\t[HttpGet]');
        codes.push(`\t\tpublic async Task<ResultVM> ${classInfo.className}Get(string paramsJson)`);
        codes.push('\t\t{');
        codes.push(`\t\t\tvar vm = await GetQuery(db.${classInfo.className}, paramsJson);`);
        codes.push('\t\t\treturn vm;');
        codes.push('\t\t}');
        codes.push('');

        let pkCol = tableObj.tableColumns.find(col => col.PrimaryKey == 1);
        if (pkCol == null) {
            tableObj.tableColumns[0];
        }
        let createTimeCol = tableObj.tableColumns.find(col => col.ColumnName == "create_time");

        codes.push('\t\t/// <summary>');
        codes.push(`\t\t/// 新增${classInfo.classComment}`);
        codes.push('\t\t/// </summary>');
        codes.push('\t\t/// <param name="model"></param>');
        codes.push('\t\t/// <returns></returns>');
        codes.push('\t\t[HttpPost]');
        codes.push(`\t\tpublic async Task<ResultVM> ${classInfo.className}Post([FromForm] ${classInfo.className} model)`);
        codes.push('\t\t{');
        codes.push('\t\t\tvar vm = new ResultVM();');
        codes.push('');
        codes.push(`\t\t\tmodel.${ndkFunction.handleClassName(pkCol.ColumnName)} = Snowflake53To.Id();`);
        if (createTimeCol) {
            codes.push(`\t\t\tmodel.${ndkFunction.handleClassName(createTimeCol.ColumnName)} = DateTime.Now;`);
        }
        codes.push('');
        codes.push('\t\t\tawait db.BaseButton.AddAsync(model);');
        codes.push('\t\t\tvar num = await db.SaveChangesAsync();');
        codes.push('\t\t\tvm.Data = num;');
        codes.push('\t\t\tvm.Set(num > 0);');
        codes.push('');
        codes.push('\t\t\treturn vm;');
        codes.push('\t\t}');
        codes.push('');

        codes.push('\t\t/// <summary>');
        codes.push(`\t\t/// 修改${classInfo.classComment}`);
        codes.push('\t\t/// </summary>');
        codes.push('\t\t/// <param name="model"></param>');
        codes.push('\t\t/// <returns></returns>');
        codes.push('\t\t[HttpPost]');
        codes.push(`\t\tpublic async Task<ResultVM> ${classInfo.className}Put([FromForm] ${classInfo.className} model)`);
        codes.push('\t\t{');
        codes.push('\t\t\tvar vm = new ResultVM();');
        codes.push('');
        codes.push(`\t\t\tvar num = await db.${classInfo.className}.Where(x => x.${ndkFunction.handleClassName(pkCol.ColumnName)} == model.${ndkFunction.handleClassName(pkCol.ColumnName)}).ExecuteUpdateAsync(x => x`);
        for (let col of tableObj.tableColumns) {
            if (col.ColumnName != pkCol.ColumnName && col.ColumnName != "create_time" && col.ColumnName != "user_id") {
                codes.push(`\t\t\t.SetProperty(p => p.${ndkFunction.handleClassName(col.ColumnName)}, model.${ndkFunction.handleClassName(col.ColumnName)})`);
            }
        }
        codes.push(codes.pop() + ');');
        codes.push('\t\t\tvm.Data = num;');
        codes.push('\t\t\tvm.Set(num > 0);');
        codes.push('');
        codes.push('\t\t\treturn vm;');
        codes.push('\t\t}');
        codes.push('');

        codes.push('\t\t/// <summary>');
        codes.push(`\t\t/// 删除${classInfo.classComment}`);
        codes.push('\t\t/// </summary>');
        codes.push('\t\t/// <param name="id"></param>');
        codes.push('\t\t/// <returns></returns>');
        codes.push('\t\t[HttpGet]');
        codes.push(`\t\tpublic async Task<ResultVM> ${classInfo.className}Delete(long id)`);
        codes.push('\t\t{');
        codes.push('\t\t\tvar vm = new ResultVM();');
        codes.push('');
        codes.push(`\t\t\tvar num = await db.${classInfo.className}.Where(x => x.${ndkFunction.handleClassName(pkCol.ColumnName)} == id).ExecuteDeleteAsync();`);
        codes.push('\t\t\tvm.Data = num;');
        codes.push('\t\t\tvm.Set(num > 0);');
        codes.push('');
        codes.push('\t\t\treturn vm;');
        codes.push('\t\t}');
        codes.push('\t}');
        codes.push('}');
        codes.push('');

        // 添加文件项
        result.files.push({
            fullName: `Controllers/${classInfo.className}Controller.cs`,
            content: codes.join("\r\n")
        })
    });

    // 输出
    return result;
}