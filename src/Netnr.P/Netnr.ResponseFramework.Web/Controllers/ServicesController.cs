using Microsoft.AspNetCore.Mvc;
using Netnr.ResponseFramework.Data;
using Netnr.SharedUserAgent;
using Netnr.SharedApp;
using Netnr.SharedFast;
using Netnr.Core;
using System;

namespace Netnr.ResponseFramework.Web.Controllers
{
    /// <summary>
    /// 服务
    /// </summary>
    [Route("[controller]/[action]")]
    public class ServicesController : Controller
    {
        public ContextBase db;
        public ServicesController(ContextBase cb)
        {
            db = cb;
        }

        /// <summary>
        /// 服务项
        /// </summary>
        public enum ServiceItem
        {
            /// <summary>
            /// 重置数据库（读取JSON文件）
            /// </summary>
            DatabaseReset,

            /// <summary>
            /// 备份数据库（写入JSON文件）
            /// </summary>
            DatabaseBackup,

            /// <summary>
            /// 清理临时目录
            /// </summary>
            ClearTmp
        }

        /// <summary>
        /// 服务
        /// </summary>
        /// <param name="ti">服务项</param>
        /// <returns></returns>
        [HttpGet]
        public SharedResultVM Index(ServiceItem ti)
        {
            var vm = new SharedResultVM();

            var ck = "Global_ServiceDo";
            var msg = CacheTo.Get(ck)?.ToString();
            if (!string.IsNullOrWhiteSpace(msg))
            {
                vm.Set(SharedEnum.RTag.refuse);
                vm.Msg = msg;
            }
            else
            {
                CacheTo.Set(ck, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} 正在执行 {ti}", 600, false);

                switch (ti)
                {
                    case ServiceItem.DatabaseReset:
                        {
                            if (new UserAgentTo(new ClientTo(HttpContext).UserAgent).IsBot)
                            {
                                vm.Set(SharedEnum.RTag.refuse);
                                vm.Msg = "are you human？";
                            }
                            else
                            {
                                var jsonPath = PathTo.Combine(GlobalTo.ContentRootPath, "db/data.json");
                                vm = ContextBase.ImportDataBase(jsonPath);
                            }
                        }
                        break;

                    case ServiceItem.DatabaseBackup:
                        {
                            //是否覆盖JSON文件，默认不覆盖，避免线上重置功能被破坏
                            var CoverJson = false;

                            var jsonPath = PathTo.Combine(GlobalTo.ContentRootPath, "db/data.json");
                            vm = ContextBase.ExportDataBase(CoverJson ? jsonPath : null);
                        }
                        break;

                    case ServiceItem.ClearTmp:
                        {
                            vm = Application.TaskService.ClearTmp();
                        }
                        break;
                }

                CacheTo.Remove(ck);
            }

            return vm;
        }
    }
}