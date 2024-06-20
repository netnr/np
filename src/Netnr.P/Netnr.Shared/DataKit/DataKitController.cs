#if Full || DKController

namespace Netnr;

/// <summary>
/// Netnr.DataKit API
/// </summary>
public class DataKitController : Controller
{
    /// <summary>
    /// 入口
    /// </summary>
    /// <param name="dbt"></param>
    /// <param name="conn"></param>
    /// <param name="databaseName"></param>
    /// <param name="dkCall"></param>
    /// <returns></returns>
    private async Task<ResultVM> Entry(DBTypes dbt, string conn, string databaseName, Func<DataKitTo, Task<object>> dkCall)
    {
        var vm = new ResultVM();

        try
        {
            var connOption = new DbKitConnectionOption
            {
                ConnectionType = dbt,
                ConnectionString = conn,
                DatabaseName = databaseName
            };

            var dataKit = DataKitTo.CreateDataKitInstance(connOption);

            //终止请求、终止执行
            HttpContext.RequestAborted.Register(() =>
            {
                dataKit.DbInstance.CommandAbort();
                ConsoleTo.WriteCard("Cancellation Requested");
            });

            vm.Data = await dkCall.Invoke(dataKit);
            vm.Set(RCodeTypes.success);
        }
        catch (Exception ex)
        {
            vm.Set(ex);
        }

        return vm;
    }

    /// <summary>
    /// 服务状态 204
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public IActionResult ServiceStatusGet() => NoContent();

    /// <summary>
    /// 获取库名
    /// </summary>
    /// <param name="dbt">数据库类型</param>
    /// <param name="conn">连接字符串</param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ResultVM> DatabaseNameOnlyGet(DBTypes dbt, string conn)
    {
        return await Entry(dbt, conn, null, async dk => await dk.GetDatabaseNameOnly());
    }

    /// <summary>
    /// 获取库
    /// </summary>
    /// <param name="dbt">数据库类型</param>
    /// <param name="conn">连接字符串</param>
    /// <param name="filterDatabaseName">数据库名</param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ResultVM> DatabaseGet(DBTypes dbt, string conn, string filterDatabaseName = null)
    {
        return await Entry(dbt, conn, null, async dk => await dk.GetDatabase(filterDatabaseName));
    }

    /// <summary>
    /// 获取表
    /// </summary>
    /// <param name="dbt">数据库类型</param>
    /// <param name="conn">连接字符串</param>
    /// <param name="schemaName">模式名</param>
    /// <param name="databaseName">数据库名</param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ResultVM> TableGet(DBTypes dbt, string conn, string schemaName = null, string databaseName = null)
    {
        return await Entry(dbt, conn, databaseName, async dk => await dk.GetTable(schemaName, databaseName, false));
    }

    /// <summary>
    /// 表DDL
    /// </summary>
    /// <param name="dbt">数据库类型</param>
    /// <param name="conn">连接字符串</param>
    /// <param name="tableName">表名</param>
    /// <param name="schemaName">模式名</param>
    /// <param name="databaseName">数据库名</param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ResultVM> TableDDLGet(DBTypes dbt, string conn, string tableName, string schemaName = null, string databaseName = null)
    {
        return await Entry(dbt, conn, databaseName, async dk => await dk.GetTableDDL(tableName, schemaName, databaseName));
    }

    /// <summary>
    /// 获取列
    /// </summary>
    /// <param name="dbt">数据库类型</param>
    /// <param name="conn">连接字符串</param>
    /// <param name="filterSchemaNameTableName">过滤模式表名，逗号分隔</param>
    /// <param name="databaseName">数据库名</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ResultVM> ColumnPost([FromForm] DBTypes dbt, [FromForm] string conn, [FromForm] string filterSchemaNameTableName = null, [FromForm] string databaseName = null)
    {
        return await Entry(dbt, conn, databaseName, async dk => await dk.GetColumn(filterSchemaNameTableName, databaseName, false));
    }

    /// <summary>
    /// 设置表注释
    /// </summary>
    /// <param name="dbt">数据库类型</param>
    /// <param name="conn">连接字符串</param>
    /// <param name="tableName">表名</param>
    /// <param name="tableComment">表注释</param>
    /// <param name="schemaName">模式名</param>
    /// <param name="databaseName">数据库名</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ResultVM> TableCommentPost([FromForm] DBTypes dbt, [FromForm] string conn, [FromForm] string tableName, [FromForm] string tableComment, [FromForm] string schemaName = null, [FromForm] string databaseName = null)
    {
        return await Entry(dbt, conn, databaseName, async dk => await dk.SetTableComment(tableName, tableComment, schemaName, databaseName));
    }

    /// <summary>
    /// 设置列注释
    /// </summary>
    /// <param name="dbt">数据库类型</param>
    /// <param name="conn">连接字符串</param>
    /// <param name="tableName">表名</param>
    /// <param name="columnName">列名</param>
    /// <param name="columnComment">列注释</param>
    /// <param name="schemaName">模式名</param>
    /// <param name="databaseName">数据库名</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ResultVM> ColumnCommentPost([FromForm] DBTypes dbt, [FromForm] string conn, [FromForm] string tableName, [FromForm] string columnName, [FromForm] string columnComment, [FromForm] string schemaName = null, [FromForm] string databaseName = null)
    {
        return await Entry(dbt, conn, databaseName, async dk => await dk.SetColumnComment(tableName, columnName, columnComment, schemaName, databaseName));
    }

    /// <summary>
    /// 执行脚本
    /// </summary>
    /// <param name="dbt">数据库类型</param>
    /// <param name="conn">连接字符串</param>
    /// <param name="sql">脚本</param>
    /// <param name="databaseName">数据库名</param>
    /// <param name="openTransaction">开启事物</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ResultVM> ExecuteSqlPost([FromForm] DBTypes dbt, [FromForm] string conn, [FromForm] string sql, [FromForm] string databaseName = null, [FromForm] bool openTransaction = true)
    {
        return await Entry(dbt, conn, databaseName, async dk => await dk.ExecuteSql(sql, openTransaction));
    }
}

#endif