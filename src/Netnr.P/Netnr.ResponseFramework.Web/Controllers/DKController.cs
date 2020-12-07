using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Netnr.ResponseFramework.Data;
using Netnr.SharedFast;
using Netnr.SharedDataKit.Applications;

namespace Netnr.ResponseFramework.Web.Controllers
{
    /// <summary>
    /// 数据库工具及代码构建
    /// </summary>
    [ResponseCache(Duration = 3)]
    [Route("[controller]/[action]")]
    public class DKController : Controller
    {
        public ContextBase db;
        public DKController(ContextBase cb)
        {
            db = cb;
        }

        /// <summary>
        /// 表管理
        /// </summary>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Apps.FilterConfigs.IsAdmin]
        public IActionResult Index()
        {
            ViewData["tdb"] = GlobalTo.TDB;

            return View();
        }

        #region DK API

        /// <summary>
        /// 处理连接信息
        /// </summary>
        /// <param name="tdb"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        private SharedResultVM ConnCheck(ref SharedEnum.TypeDB? tdb, ref string conn)
        {
            var vm = new SharedResultVM();

            //没指定连接字符串时取后台连接信息
            if (string.IsNullOrWhiteSpace(conn))
            {
                tdb = GlobalTo.TDB;
                conn = db.Database.GetDbConnection().ConnectionString;
                if (db.Database.IsSqlite())
                {
                    conn = conn.Replace("Filename=", "Data Source=");
                }
            }

            return vm;
        }

        /// <summary>
        /// 获取所有表名及注释
        /// </summary>
        /// <param name="tdb">数据库类型（0：MySQL，1：SQLite，2：Oracle，3：SQLServer，4：PostgreSQL）</param>
        /// <param name="conn">连接字符串</param>
        /// <returns></returns>
        [HttpGet]
        [HttpOptions]
        [Apps.FilterConfigs.IsAdmin]
        public SharedResultVM GetTable(SharedEnum.TypeDB? tdb, string conn)
        {
            var vm = ConnCheck(ref tdb, ref conn);
            if (vm.Code == 0)
            {
                vm = DataKitService.GetTable(tdb, conn);
            }
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
        [Apps.FilterConfigs.IsAdmin]
        public SharedResultVM GetColumn([FromForm] SharedEnum.TypeDB? tdb, [FromForm] string conn, [FromForm] string filterTableName = "")
        {
            var vm = ConnCheck(ref tdb, ref conn);
            if (vm.Code == 0)
            {
                vm = DataKitService.GetColumn(tdb, conn, filterTableName);
            }
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
        [Apps.FilterConfigs.IsAdmin]
        public SharedResultVM SetTableComment(SharedEnum.TypeDB? tdb, string conn, string TableName, string TableComment)
        {
            var vm = ConnCheck(ref tdb, ref conn);
            if (vm.Code == 0)
            {
                vm = DataKitService.SetTableComment(tdb, conn, TableName, TableComment);
            }
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
        [Apps.FilterConfigs.IsAdmin]
        public SharedResultVM SetColumnComment(SharedEnum.TypeDB? tdb, string conn, string TableName, string FieldName, string FieldComment)
        {
            var vm = ConnCheck(ref tdb, ref conn);
            if (vm.Code == 0)
            {
                vm = DataKitService.SetColumnComment(tdb, conn, TableName, FieldName, FieldComment);
            }
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
        [Apps.FilterConfigs.IsAdmin]
        public SharedResultVM GetData(SharedEnum.TypeDB? tdb, string conn, string TableName, int page, int rows, string sort, string order, string listFieldName, string whereSql)
        {
            var vm = ConnCheck(ref tdb, ref conn);
            if (vm.Code == 0)
            {
                vm = DataKitService.GetData(tdb, conn, TableName, page, rows, sort, order, listFieldName, whereSql);
            }
            return vm;
        }

        /// <summary>
        /// 查询数据库环境信息
        /// </summary>
        /// <param name="tdb">数据库类型（0：MySQL，1：SQLite，2：Oracle，3：SQLServer，4：PostgreSQL）</param>
        /// <param name="conn">连接字符串</param>
        /// <returns></returns>
        [HttpGet]
        public SharedResultVM GetDEI(SharedEnum.TypeDB? tdb, string conn)
        {
            var vm = ConnCheck(ref tdb, ref conn);
            if (vm.Code == 0)
            {
                vm = DataKitService.GetDEI(tdb, conn);
            }
            return vm;
        }

        #endregion

        /// <summary>
        /// 查询表配置虚拟表名
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HttpOptions]
        [Apps.FilterConfigs.IsAdmin]
        public SharedResultVM QueryTableConfig()
        {
            var vm = new SharedResultVM();

            var query = from a in db.SysTableConfig
                        group a by a.TableName into g
                        select g.Key;

            vm.Data = query.ToList();
            vm.Set(SharedEnum.RTag.success);

            return vm;
        }

        /// <summary>
        /// 保存表配置信息
        /// </summary>
        /// <param name="rows">表配置数据行</param>
        /// <param name="buildType">1：追加，2：覆盖</param>
        /// <returns></returns>
        [HttpPost]
        [HttpOptions]
        [Apps.FilterConfigs.IsAdmin]
        public SharedResultVM SaveTableConfig(string rows, int buildType)
        {
            var vm = new SharedResultVM();

            var listMo = rows.ToEntitys<Domain.SysTableConfig>();
            var hasTableName = listMo.Select(x => x.TableName).ToList();

            switch (buildType)
            {
                //追加
                case 1:
                    {
                        var listField = db.SysTableConfig
                            .Where(x => hasTableName.Contains(x.TableName))
                            .Select(x => new { x.TableName, x.ColField }).ToList();
                        for (int i = listMo.Count - 1; i >= 0; i--)
                        {
                            if (listField.Any(x => x.TableName == listMo[i].TableName && x.ColField == listMo[i].ColField))
                            {
                                listMo.RemoveAt(i);
                            }
                        }
                    }
                    break;

                //覆盖
                case 2:
                    {
                        var delstc = db.SysTableConfig.Where(x => hasTableName.Contains(x.TableName)).ToList();
                        db.SysTableConfig.RemoveRange(delstc);
                    }
                    break;
            }

            if (listMo.Count > 0)
            {
                //主键统一用程序生成GUID覆盖
                listMo.ForEach(x => x.Id = Guid.NewGuid().ToString());
                db.SysTableConfig.AddRange(listMo);
            }

            vm.Data = db.SaveChanges();
            vm.Set(SharedEnum.RTag.success);

            return vm;
        }
    }
}