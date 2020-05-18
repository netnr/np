using System;
using Microsoft.AspNetCore.Mvc;

namespace Netnr.ResponseFramework.Web.Controllers
{
    /// <summary>
    /// 工具
    /// </summary>
    [Route("[controller]/[action]")]
    public class ToolController : Controller
    {
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