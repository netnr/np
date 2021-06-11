using Microsoft.AspNetCore.Mvc;
using Netnr.SharedDataKit;

namespace Netnr.DataKit.Web.Controllers
{
    /// <summary>
    /// Netnr.DataKit API
    /// </summary>
    [Route("[controller]/[action]")]
    [ResponseCache(Duration = 2)]
    [Apps.FilterConfigs.AllowCors]
    public class DKController : Controller
    {
        /// <summary>
        /// 获取所有表名及注释
        /// </summary>
        /// <param name="tdb">数据库类型</param>
        /// <param name="conn">连接字符串</param>
        /// <returns></returns>
        [HttpGet]
        public SharedResultVM GetTable(SharedEnum.TypeDB? tdb, string conn)
        {
            var vm = DataKitTo.GetTable(tdb, conn);
            return vm;
        }

        /// <summary>
        /// 获取所有列
        /// </summary>
        /// <param name="tdb">数据库类型</param>
        /// <param name="conn">连接字符串</param>
        /// <param name="filterTableName">过滤表名，英文逗号分隔，为空时默认所有表</param>
        /// <returns></returns>
        [HttpPost]
        public SharedResultVM GetColumn([FromForm] SharedEnum.TypeDB? tdb, [FromForm] string conn, [FromForm] string filterTableName = "")
        {
            var vm = DataKitTo.GetColumn(tdb, conn, filterTableName);
            return vm;
        }

        /// <summary>
        /// 设置表注释
        /// </summary>
        /// <param name="tdb">数据库类型</param>
        /// <param name="conn">连接字符串</param>
        /// <param name="TableName">表名</param>
        /// <param name="TableComment">表注释</param>
        /// <returns></returns>
        [HttpGet]
        public SharedResultVM SetTableComment(SharedEnum.TypeDB? tdb, string conn, string TableName, string TableComment)
        {
            var vm = DataKitTo.SetTableComment(tdb, conn, TableName, TableComment);
            return vm;
        }

        /// <summary>
        /// 设置列注释
        /// </summary>
        /// <param name="tdb">数据库类型</param>
        /// <param name="conn">连接字符串</param>
        /// <param name="TableName">表名</param>
        /// <param name="FieldName">列名</param>
        /// <param name="FieldComment">列注释</param>
        /// <returns></returns>
        [HttpGet]
        public SharedResultVM SetColumnComment(SharedEnum.TypeDB? tdb, string conn, string TableName, string FieldName, string FieldComment)
        {
            var vm = DataKitTo.SetColumnComment(tdb, conn, TableName, FieldName, FieldComment);
            return vm;
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
        /// <returns></returns>
        [HttpGet]
        public SharedResultVM GetData(SharedEnum.TypeDB? tdb, string conn, string TableName, int page, int rows, string sort, string order, string listFieldName, string whereSql)
        {
            var vm = DataKitTo.GetData(tdb, conn, TableName, page, rows, sort, order, listFieldName, whereSql);
            return vm;
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
            var vm = DataKitTo.GetDEI(tdb, conn);
            return vm;
        }
    }
}