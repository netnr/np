using System;
using Microsoft.AspNetCore.Mvc;

namespace Netnr.Blog.Web.Controllers
{
    /// <summary>
    /// 测试
    /// </summary>
    public class TestController : Controller
    {
        /// <summary>
        /// 起始页
        /// </summary>
        /// <returns></returns>
        public ActionResultVM Index()
        {
            var vm = new ActionResultVM();


            try
            {
                //TO DO

            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }
    }
}