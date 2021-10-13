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
    /// <param name="DatabaseName">数据库名</param>
    /// <returns></returns>
    [HttpGet]
    public SharedResultVM GetTable(SharedEnum.TypeDB? tdb, string conn, string DatabaseName = null)
    {
        return DataKitTo.GetTable(tdb, conn, DatabaseName);
    }

    /// <summary>
    /// 获取列
    /// </summary>
    /// <param name="tdb">数据库类型</param>
    /// <param name="conn">连接字符串</param>
    /// <param name="filterTableName">过滤表名，英文逗号分隔，为空时默认所有表</param>
    /// <param name="DatabaseName">数据库名</param>
    /// <returns></returns>
    [HttpPost]
    public SharedResultVM GetColumn([FromForm] SharedEnum.TypeDB? tdb, [FromForm] string conn, [FromForm] string filterTableName = "", [FromForm] string DatabaseName = null)
    {
        return DataKitTo.GetColumn(tdb, conn, filterTableName, DatabaseName);
    }

    /// <summary>
    /// 设置表注释
    /// </summary>
    /// <param name="tdb">数据库类型</param>
    /// <param name="conn">连接字符串</param>
    /// <param name="TableName">表名</param>
    /// <param name="TableComment">表注释</param>
    /// <param name="DatabaseName">数据库名</param>
    /// <returns></returns>
    [HttpPost]
    public SharedResultVM SetTableComment([FromForm] SharedEnum.TypeDB? tdb, [FromForm] string conn, [FromForm] string TableName, [FromForm] string TableComment, [FromForm] string DatabaseName = null)
    {
        return DataKitTo.SetTableComment(tdb, conn, TableName, TableComment, DatabaseName);
    }

    /// <summary>
    /// 设置列注释
    /// </summary>
    /// <param name="tdb">数据库类型</param>
    /// <param name="conn">连接字符串</param>
    /// <param name="TableName">表名</param>
    /// <param name="ColumnName">列名</param>
    /// <param name="ColumnComment">列注释</param>
    /// <param name="DatabaseName">数据库名</param>
    /// <returns></returns>
    [HttpPost]
    public SharedResultVM SetColumnComment([FromForm] SharedEnum.TypeDB? tdb, [FromForm] string conn, [FromForm] string TableName, [FromForm] string ColumnName, [FromForm] string ColumnComment, [FromForm] string DatabaseName = null)
    {
        return DataKitTo.SetColumnComment(tdb, conn, TableName, ColumnName, ColumnComment, DatabaseName);
    }

    /// <summary>
    /// 执行脚本
    /// </summary>
    /// <param name="tdb">数据库类型</param>
    /// <param name="conn">连接字符串</param>
    /// <param name="sql">脚本</param>
    /// <param name="DatabaseName">数据库名</param>
    /// <returns></returns>
    [HttpPost]
    public SharedResultVM ExecuteSql([FromForm] SharedEnum.TypeDB? tdb, [FromForm] string conn, [FromForm] string sql, [FromForm] string DatabaseName = null)
    {
        return DataKitTo.ExecuteSql(tdb, conn, sql, DatabaseName);
    }

    /// <summary>
    /// 查询数据
    /// </summary>
    /// <param name="tdb">数据库类型</param>
    /// <param name="conn">连接字符串</param>
    /// <param name="TableName">表名</param>
    /// <param name="page">页码</param>
    /// <param name="rows">页量</param>
    /// <param name="sort">排序字段</param>
    /// <param name="order">排序方式</param>
    /// <param name="listFieldName">查询列，默认为 *</param>
    /// <param name="whereSql">条件</param>
    /// <param name="DatabaseName">数据库名</param>
    /// <returns></returns>
    [HttpGet]
    public SharedResultVM GetData(SharedEnum.TypeDB? tdb, string conn, string TableName, int page, int rows, string sort, string order, string listFieldName, string whereSql, string DatabaseName = null)
    {
        return DataKitTo.GetData(tdb, conn, TableName, page, rows, sort, order, listFieldName, whereSql, DatabaseName);
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