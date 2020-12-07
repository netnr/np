using System;
using Microsoft.AspNetCore.Mvc;
using Netnr.Core;

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
        public SharedResultVM QueryServerInfo()
        {
            var vm = new SharedResultVM();

            try
            {
                var ss = new SystemStatusTo();
                vm.Log.Add(ss);
                vm.Data = ss.ToView();
                vm.Set(SharedEnum.RTag.success);
            }
            catch (Exception ex)
            {
                vm.Set(ex);
                ConsoleTo.Log(ex);
            }

            return vm;
        }

        #endregion

        #region 退出

        /// <summary>
        /// 退出应用
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public void Exit()
        {
            //Environment.Exit(0);
            //Process.GetCurrentProcess().Kill();
        }

        #endregion
    }
}