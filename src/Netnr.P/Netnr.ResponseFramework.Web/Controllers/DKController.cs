using Microsoft.AspNetCore.Mvc;
using Netnr.ResponseFramework.Data;
using Netnr.SharedFast;

namespace Netnr.ResponseFramework.Web.Controllers
{
    /// <summary>
    /// 数据库工具及代码构建
    /// </summary>
    [Route("[controller]/[action]")]
    [ResponseCache(Duration = 2)]
    [Apps.FilterConfigs.IsAdmin]
    public class DKController : DKControllerTo
    {
        public ContextBase db;
        public DKController(ContextBase cb)
        {
            db = cb;
            
            var conn = SharedDbContext.FactoryTo.GetConn().Replace("Filename=", "Data Source=");

            Core.CacheTo.Set("CDB", new Tuple<SharedEnum.TypeDB, string>(GlobalTo.TDB, conn));
        }

        /// <summary>
        /// 表管理
        /// </summary>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 查询表配置虚拟表名
        /// </summary>
        /// <returns></returns>
        [HttpGet]
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