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
    /// <param name="tdb"></param>
    /// <param name="conn"></param>
    /// <param name="databaseName"></param>
    /// <param name="dkCall"></param>
    /// <returns></returns>
    private ResultVM Entry(EnumTo.TypeDB tdb, string conn, string databaseName, Func<DataKitTo, object> dkCall)
    {
        var vm = new ResultVM();

        try
        {
            var dk = DataKitTo.Init(tdb, conn, databaseName);
            if (dk != null)
            {
                //终止请求、终止执行
                HttpContext.RequestAborted.Register(() =>
                {
                    dk.db.CommandAbort();
                    Console.WriteLine("\rCancellation Requested\r");
                });

                vm.Data = dkCall(dk);
                vm.Set(EnumTo.RTag.success);
            }
            else
            {
                vm.Set(EnumTo.RTag.error);
            }
        }
        catch (Exception ex)
        {
            vm.Set(ex);
        }

        return vm;
    }

    /// <summary>
    /// 获取库名
    /// </summary>
    /// <param name="tdb">数据库类型</param>
    /// <param name="conn">连接字符串</param>
    /// <returns></returns>
    [HttpGet]
    public ResultVM GetDatabaseName(EnumTo.TypeDB tdb, string conn)
    {
        return Entry(tdb, conn, null, dk => dk.GetDatabaseName());
    }

    /// <summary>
    /// 获取库
    /// </summary>
    /// <param name="tdb">数据库类型</param>
    /// <param name="conn">连接字符串</param>
    /// <param name="filterDatabaseName">数据库名</param>
    /// <returns></returns>
    [HttpGet]
    public ResultVM GetDatabase(EnumTo.TypeDB tdb, string conn, string filterDatabaseName = null)
    {
        return Entry(tdb, conn, null, dk => dk.GetDatabase(filterDatabaseName));
    }

    /// <summary>
    /// 获取表
    /// </summary>
    /// <param name="tdb">数据库类型</param>
    /// <param name="conn">连接字符串</param>
    /// <param name="schemaName">模式名</param>
    /// <param name="databaseName">数据库名</param>
    /// <returns></returns>
    [HttpGet]
    public ResultVM GetTable(EnumTo.TypeDB tdb, string conn, string schemaName = null, string databaseName = null)
    {
        return Entry(tdb, conn, databaseName, dk => dk.GetTable(schemaName, databaseName));
    }

    /// <summary>
    /// 表DDL
    /// </summary>
    /// <param name="tdb">数据库类型</param>
    /// <param name="conn">连接字符串</param>
    /// <param name="tableName">表名</param>
    /// <param name="schemaName">模式名</param>
    /// <param name="databaseName">数据库名</param>
    /// <returns></returns>
    [HttpGet]
    public ResultVM GetTableDDL(EnumTo.TypeDB tdb, string conn, string tableName, string schemaName = null, string databaseName = null)
    {
        return Entry(tdb, conn, databaseName, dk => dk.GetTableDDL(tableName, schemaName, databaseName));
    }

    /// <summary>
    /// 获取列
    /// </summary>
    /// <param name="tdb">数据库类型</param>
    /// <param name="conn">连接字符串</param>
    /// <param name="filterSchemaNameTableName">过滤模式表名，逗号分隔</param>
    /// <param name="databaseName">数据库名</param>
    /// <returns></returns>
    [HttpPost]
    public ResultVM GetColumn([FromForm] EnumTo.TypeDB tdb, [FromForm] string conn, [FromForm] string filterSchemaNameTableName = null, [FromForm] string databaseName = null)
    {
        return Entry(tdb, conn, databaseName, dk => dk.GetColumn(filterSchemaNameTableName, databaseName));
    }

    /// <summary>
    /// 设置表注释
    /// </summary>
    /// <param name="tdb">数据库类型</param>
    /// <param name="conn">连接字符串</param>
    /// <param name="tableName">表名</param>
    /// <param name="tableComment">表注释</param>
    /// <param name="schemaName">模式名</param>
    /// <param name="databaseName">数据库名</param>
    /// <returns></returns>
    [HttpPost]
    public ResultVM SetTableComment([FromForm] EnumTo.TypeDB tdb, [FromForm] string conn, [FromForm] string tableName, [FromForm] string tableComment, [FromForm] string schemaName = null, [FromForm] string databaseName = null)
    {
        return Entry(tdb, conn, databaseName, dk => dk.SetTableComment(tableName, tableComment, schemaName, databaseName));
    }

    /// <summary>
    /// 设置列注释
    /// </summary>
    /// <param name="tdb">数据库类型</param>
    /// <param name="conn">连接字符串</param>
    /// <param name="tableName">表名</param>
    /// <param name="columnName">列名</param>
    /// <param name="columnComment">列注释</param>
    /// <param name="schemaName">模式名</param>
    /// <param name="databaseName">数据库名</param>
    /// <returns></returns>
    [HttpPost]
    public ResultVM SetColumnComment([FromForm] EnumTo.TypeDB tdb, [FromForm] string conn, [FromForm] string tableName, [FromForm] string columnName, [FromForm] string columnComment, [FromForm] string schemaName = null, [FromForm] string databaseName = null)
    {
        return Entry(tdb, conn, databaseName, dk => dk.SetColumnComment(tableName, columnName, columnComment, schemaName, databaseName));
    }

    /// <summary>
    /// 执行脚本
    /// </summary>
    /// <param name="tdb">数据库类型</param>
    /// <param name="conn">连接字符串</param>
    /// <param name="sql">脚本</param>
    /// <param name="databaseName">数据库名</param>
    /// <returns></returns>
    [HttpPost]
    public ResultVM ExecuteSql([FromForm] EnumTo.TypeDB tdb, [FromForm] string conn, [FromForm] string sql, [FromForm] string databaseName = null)
    {
        return Entry(tdb, conn, databaseName, dk => dk.ExecuteSql(sql));
    }
}

#endif