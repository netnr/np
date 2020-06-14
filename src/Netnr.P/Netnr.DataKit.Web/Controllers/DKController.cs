using Microsoft.AspNetCore.Mvc;
using Netnr.DataKit.Application;

namespace Netnr.DataKit.Web.Controllers
{
    /// <summary>
    /// Netnr.DataKit API
    /// </summary>
    [Route("[controller]/[action]")]
    [ResponseCache(Duration = 3)]
    [Filters.FilterConfigs.AllowCors]
    public class DKController : Controller
    {
        /// <summary>
        /// 获取所有表名及注释
        /// </summary>
        /// <param name="tdb">数据库类型（0：MySQL，1：SQLite，2：Oracle，3：SQLServer，4：PostgreSQL）</param>
        /// <param name="conn">连接字符串</param>
        /// <returns></returns>
        [HttpGet]
        [HttpOptions]
        public ActionResultVM GetTable(TypeDB? tdb, string conn)
        {
            var vm = new DataKitUseService().GetTable(tdb, conn);
            return vm;
        }

        /// <summary>
        /// 获取所有列
        /// </summary>
        /// <param name="tdb">数据库类型（0：MySQL，1：SQLite，2：Oracle，3：SQLServer，4：PostgreSQL）</param>
        /// <param name="conn">连接字符串</param>
        /// <param name="filterTableName">过滤表名，英文逗号分隔，为空时默认所有表</param>
        /// <returns></returns>
        [HttpPost]
        [HttpOptions]
        public ActionResultVM GetColumn([FromForm] TypeDB? tdb, [FromForm] string conn, [FromForm] string filterTableName = "")
        {
            var vm = new DataKitUseService().GetColumn(tdb, conn, filterTableName);
            return vm;
        }

        /// <summary>
        /// 设置表注释
        /// </summary>
        /// <param name="tdb">数据库类型（0：MySQL，1：SQLite，2：Oracle，3：SQLServer，4：PostgreSQL）</param>
        /// <param name="conn">连接字符串</param>
        /// <param name="TableName">表名</param>
        /// <param name="TableComment">表注释</param>
        /// <returns></returns>
        [HttpGet]
        [HttpOptions]
        public ActionResultVM SetTableComment(TypeDB? tdb, string conn, string TableName, string TableComment)
        {
            var vm = new DataKitUseService().SetTableComment(tdb, conn, TableName, TableComment);
            return vm;
        }

        /// <summary>
        /// 设置列注释
        /// </summary>
        /// <param name="tdb">数据库类型（0：MySQL，1：SQLite，2：Oracle，3：SQLServer，4：PostgreSQL）</param>
        /// <param name="conn">连接字符串</param>
        /// <param name="TableName">表名</param>
        /// <param name="FieldName">列名</param>
        /// <param name="FieldComment">列注释</param>
        /// <returns></returns>
        [HttpGet]
        [HttpOptions]
        public ActionResultVM SetColumnComment(TypeDB? tdb, string conn, string TableName, string FieldName, string FieldComment)
        {
            var vm = new DataKitUseService().SetColumnComment(tdb, conn, TableName, FieldName, FieldComment);
            return vm;
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="tdb">数据库类型（0：MySQL，1：SQLite，2：Oracle，3：SQLServer，4：PostgreSQL）</param>
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
        [HttpOptions]
        public ActionResultVM GetData(TypeDB? tdb, string conn, string TableName, int page, int rows, string sort, string order, string listFieldName, string whereSql)
        {
            var vm = new DataKitUseService().GetData(tdb, conn, TableName, page, rows, sort, order, listFieldName, whereSql);
            return vm;
        }
    }
}