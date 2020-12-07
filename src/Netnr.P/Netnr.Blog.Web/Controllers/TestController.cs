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
        public SharedResultVM Index()
        {
            var vm = new SharedResultVM();

            try
            {
                //TO DO
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Core.ConsoleTo.Log(ex);
            }

            return vm;
        }
    }
}