using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Netnr.DataKit.Application;
using Netnr.ResponseFramework.Data;

namespace Netnr.ResponseFramework.Web.Controllers
{
    /// <summary>
    /// 数据库工具及代码构建
    /// </summary>
    [Route("[controller]/[action]")]
    [Filters.FilterConfigs.AllowCors]
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
        [Filters.FilterConfigs.IsAdmin]
        public IActionResult Index()
        {
            if (db.Database.IsInMemory())
            {
                return Content("不支持内存数据库，请切换其它数据库");
            }

            var uinfo = Application.CommonService.GetLoginUserInfo(HttpContext);
            ViewData["token"] = Application.CommonService.TokenMake(uinfo);

            if (Enum.TryParse(GlobalTo.GetValue("TypeDB"), true, out TypeDB tdb))
            {
                ViewData["dbi"] = (int)tdb;
            }

            return View();
        }

        #region DK API

        /// <summary>
        /// 处理连接信息
        /// </summary>
        /// <param name="tdb"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        private ActionResultVM ConnCheck(ref TypeDB? tdb, ref string conn)
        {
            var vm = new ActionResultVM();
            if (db.Database.IsInMemory())
            {
                vm.Set(ARTag.refuse);
                vm.Msg = "不支持内存数据库，请切换其它数据库";
            }
            else
            {
                //没指定连接字符串时取后台连接信息
                if (string.IsNullOrWhiteSpace(conn))
                {
                    tdb = ContextBase.TDB;
                    conn = db.Database.GetDbConnection().ConnectionString;
                    if (db.Database.IsSqlite())
                    {
                        conn = conn.Replace("Filename=", "Data Source=");
                    }
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
        [Filters.FilterConfigs.IsAdmin]
        public ActionResultVM GetTable(TypeDB? tdb, string conn)
        {
            var vm = ConnCheck(ref tdb, ref conn);
            if (vm.Code == 0)
            {
                vm = new DataKitUseService().GetTable(tdb, conn);
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
        [Filters.FilterConfigs.IsAdmin]
        public ActionResultVM GetColumn([FromForm]TypeDB? tdb, [FromForm]string conn, [FromForm]string filterTableName = "")
        {
            var vm = ConnCheck(ref tdb, ref conn);
            if (vm.Code == 0)
            {
                vm = new DataKitUseService().GetColumn(tdb, conn, filterTableName);
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
        [Filters.FilterConfigs.IsAdmin]
        public ActionResultVM SetTableComment(TypeDB? tdb, string conn, string TableName, string TableComment)
        {
            var vm = ConnCheck(ref tdb, ref conn);
            if (vm.Code == 0)
            {
                vm = new DataKitUseService().SetTableComment(tdb, conn, TableName, TableComment);
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
        [Filters.FilterConfigs.IsAdmin]
        public ActionResultVM SetColumnComment(TypeDB? tdb, string conn, string TableName, string FieldName, string FieldComment)
        {
            var vm = ConnCheck(ref tdb, ref conn);
            if (vm.Code == 0)
            {
                vm = new DataKitUseService().SetColumnComment(tdb, conn, TableName, FieldName, FieldComment);
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
        /// <returns></returns>
        [HttpGet]
        [HttpOptions]
        [Filters.FilterConfigs.IsAdmin]
        public ActionResultVM GetData(TypeDB? tdb, string conn, string TableName, int page, int rows, string sort, string order, string listFieldName)
        {
            var vm = ConnCheck(ref tdb, ref conn);
            if (vm.Code == 0)
            {
                vm = new DataKitUseService().GetData(tdb, conn, TableName, page, rows, sort, order, listFieldName);
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
        [Filters.FilterConfigs.IsAdmin]
        public ActionResultVM QueryTableConfig()
        {
            var vm = new ActionResultVM();

            var query = from a in db.SysTableConfig
                        group a by a.TableName into g
                        select g.Key;

            vm.Data = query.ToList();
            vm.Set(ARTag.success);

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
        [Filters.FilterConfigs.IsAdmin]
        public ActionResultVM SaveTableConfig(string rows, int buildType)
        {
            var vm = new ActionResultVM();

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
            vm.Set(ARTag.success);

            return vm;
        }

        /// <summary>
        /// 根据JSON数据重置数据库
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResultVM ResetDataBaseForJson()
        {
            var vm = new Application.DataMirrorService().AddForJson();

            return vm;
        }

        /// <summary>
        /// 数据库备份数据为JSON
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResultVM BackupDataBaseAsJson()
        {
            var vm = new ActionResultVM();

            try
            {
                //是否覆盖JSON文件，默认不覆盖，避免线上重置功能被破坏
                var CoverJson = true;

                vm = new Application.DataMirrorService().SaveAsJson(CoverJson);
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }
    }
}