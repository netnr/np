#if Full || DKController

using Microsoft.AspNetCore.Mvc;
using Netnr;
using Netnr.SharedDataKit;

/// <summary>
/// Netnr.DataKit API
/// </summary>
public class DKControllerTo : Controller
{
    /// <summary>
    /// 获取库名
    /// </summary>
    /// <param name="tdb">数据库类型</param>
    /// <param name="conn">连接字符串</param>
    /// <returns></returns>
    [HttpGet]
    public SharedResultVM GetdatabaseName(SharedEnum.TypeDB? tdb, string conn)
    {
        return DataKitTo.GetDatabaseName(tdb, conn);
    }

    /// <summary>
    /// 获取库
    /// </summary>
    /// <param name="tdb">数据库类型</param>
    /// <param name="conn">连接字符串</param>
    /// <returns></returns>
    [HttpGet]
    public SharedResultVM GetDatabase(SharedEnum.TypeDB? tdb, string conn)
    {
        return DataKitTo.GetDatabase(tdb, conn);
    }

    /// <summary>
    /// 获取表
    /// </summary>
    /// <param name="tdb">数据库类型</param>
    /// <param name="conn">连接字符串</param>
    /// <param name="databaseName">数据库名</param>
    /// <returns></returns>
    [HttpGet]
    public SharedResultVM GetTable(SharedEnum.TypeDB? tdb, string conn, string databaseName = null)
    {
        return DataKitTo.GetTable(tdb, conn, databaseName);
    }

    /// <summary>
    /// 获取列
    /// </summary>
    /// <param name="tdb">数据库类型</param>
    /// <param name="conn">连接字符串</param>
    /// <param name="filterTableName">过滤表名，英文逗号分隔，为空时默认所有表</param>
    /// <param name="databaseName">数据库名</param>
    /// <returns></returns>
    [HttpPost]
    public SharedResultVM GetColumn([FromForm] SharedEnum.TypeDB? tdb, [FromForm] string conn, [FromForm] string filterTableName = "", [FromForm] string databaseName = null)
    {
        return DataKitTo.GetColumn(tdb, conn, filterTableName, databaseName);
    }

    /// <summary>
    /// 设置表注释
    /// </summary>
    /// <param name="tdb">数据库类型</param>
    /// <param name="conn">连接字符串</param>
    /// <param name="tableName">表名</param>
    /// <param name="tableComment">表注释</param>
    /// <param name="databaseName">数据库名</param>
    /// <returns></returns>
    [HttpPost]
    public SharedResultVM SetTableComment([FromForm] SharedEnum.TypeDB? tdb, [FromForm] string conn, [FromForm] string tableName, [FromForm] string tableComment, [FromForm] string databaseName = null)
    {
        return DataKitTo.SetTableComment(tdb, conn, tableName, tableComment, databaseName);
    }

    /// <summary>
    /// 设置列注释
    /// </summary>
    /// <param name="tdb">数据库类型</param>
    /// <param name="conn">连接字符串</param>
    /// <param name="tableName">表名</param>
    /// <param name="columnName">列名</param>
    /// <param name="columnComment">列注释</param>
    /// <param name="databaseName">数据库名</param>
    /// <returns></returns>
    [HttpPost]
    public SharedResultVM SetColumnComment([FromForm] SharedEnum.TypeDB? tdb, [FromForm] string conn, [FromForm] string tableName, [FromForm] string columnName, [FromForm] string columnComment, [FromForm] string databaseName = null)
    {
        return DataKitTo.SetColumnComment(tdb, conn, tableName, columnName, columnComment, databaseName);
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
    public SharedResultVM ExecuteSql([FromForm] SharedEnum.TypeDB? tdb, [FromForm] string conn, [FromForm] string sql, [FromForm] string databaseName = null)
    {
        return DataKitTo.ExecuteSql(tdb, conn, sql, databaseName);
    }

    /// <summary>
    /// 查询数据
    /// </summary>
    /// <param name="tdb">数据库类型</param>
    /// <param name="conn">连接字符串</param>
    /// <param name="tableName">表名</param>
    /// <param name="page">页码</param>
    /// <param name="rows">页量</param>
    /// <param name="sort">排序字段</param>
    /// <param name="order">排序方式</param>
    /// <param name="listFieldName">查询列，默认为 *</param>
    /// <param name="whereSql">条件</param>
    /// <param name="databaseName">数据库名</param>
    /// <returns></returns>
    [HttpGet]
    public SharedResultVM GetData(SharedEnum.TypeDB? tdb, string conn, string tableName, int page, int rows, string sort, string order, string listFieldName, string whereSql, string databaseName = null)
    {
        return DataKitTo.GetData(tdb, conn, tableName, page, rows, sort, order, listFieldName, whereSql, databaseName);
    }

    /// <summary>
    /// 查询数据库环境信息
    /// </summary>
    /// <param name="tdb">数据库类型</param>
    /// <param name="conn">连接字符串</param>
    /// <returns></returns>
    [HttpGet]
    public SharedResultVM GetDEI(SharedEnum.TypeDB? tdb, string conn)
    {
        return DataKitTo.GetDEI(tdb, conn);
    }
}

#endif