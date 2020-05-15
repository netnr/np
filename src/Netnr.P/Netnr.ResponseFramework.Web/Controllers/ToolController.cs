using System;
using System.Data;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Netnr.DataKit.Application;
using Netnr.ResponseFramework.Data;
using Netnr.ResponseFramework.Domain;

namespace Netnr.ResponseFramework.Web.Controllers
{
    /// <summary>
    /// 工具
    /// </summary>
    [Filters.FilterConfigs.AllowCors]
    [Filters.FilterConfigs.IsAdmin]
    [Route("[controller]/[action]")]
    public class ToolController : Controller
    {
        public ContextBase db;
        public ToolController(ContextBase cb)
        {
            db = cb;
        }

        #region 表管理

        /// <summary>
        /// 表管理
        /// </summary>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult TableManage()
        {
            if (db.Database.IsInMemory())
            {
                return Content("不支持内存数据库，请切换其它数据库");
            }

            return Redirect("/lib/dk/index.html?jsonp=/lib/dk/extension/netnrf.js");
        }

        /// <summary>
        /// 查询表配置虚拟表名
        /// </summary>
        /// <returns></returns>
        [HttpGet]
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
        public ActionResultVM SaveTableConfig(string rows, int buildType)
        {
            var vm = new ActionResultVM();

            var listMo = rows.ToEntitys<SysTableConfig>();
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
        /// 保存生成的代码
        /// </summary>
        /// <param name="name">视图名称</param>
        /// <param name="controller">控制器名称</param>
        /// <param name="view">视图内容</param>
        /// <param name="javascript">视图页面脚本内容</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResultVM SaveGenerateCode(string name, string controller, string view, string javascript)
        {
            var vm = new ActionResultVM();

            try
            {
                var rootg = "/upload/temp/.ignore/GenerateCode/";
                var rootc = GlobalTo.WebRootPath + rootg + "Controllers/";
                var rootv = GlobalTo.WebRootPath + rootg + "Views/" + name + "/";
                var rootj = GlobalTo.WebRootPath + rootg + "js/" + name.ToLower() + "/";

                if (!Directory.Exists(rootc))
                {
                    Directory.CreateDirectory(rootc);
                }
                if (!Directory.Exists(rootv))
                {
                    Directory.CreateDirectory(rootv);
                }
                if (!Directory.Exists(rootj))
                {
                    Directory.CreateDirectory(rootj);
                }

                Core.FileTo.WriteText(controller, rootc, name + ".cs", false);
                Core.FileTo.WriteText(view, rootv, name + ".cshtml", false);
                Core.FileTo.WriteText(javascript, rootj, name.ToLower() + ".js", false);

                vm.Set(ARTag.success);
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

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
                var CoverJson = false;

                vm = new Application.DataMirrorService().SaveAsJson(CoverJson);
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        #endregion

        #region 服务器信息

        /// <summary>
        /// 服务器信息
        /// </summary>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult ServerInfo()
        {
            return View();
        }

        /// <summary>
        /// 查询服务器信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ResponseCache(Duration = 10)]
        public ActionResultVM QueryServerInfo()
        {
            var vm = new ActionResultVM();

            try
            {
                vm.Data = new Fast.OSInfoTo();
                vm.Set(ARTag.success);
            }
            catch (Exception ex)
            {
                vm.Set(ex);
                Core.ConsoleTo.Log(ex);
            }

            return vm;
        }

        #endregion
    }
}