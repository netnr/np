using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Netnr.Guff.Controllers
{
    public class BuildController : Controller
    {
        /// <summary>
        /// 构建静态文件
        /// </summary>
        /// <returns></returns>
        public ActionResultVM Index()
        {
            var vm = new ActionResultVM();

            //设置是构建访问
            var cacheKey = GlobalTo.GetValue("Common:BuildHtmlKey");
            Core.CacheTo.Set(cacheKey, true);

            try
            {
                var urlPrefix = $"{Request.Scheme}://{Request.Host}/home/";
                var path = GlobalTo.WebRootPath + "/";

                //反射action
                var type = typeof(HomeController);
                var methods = type.GetMethods().Where(x => x.DeclaringType == type).ToList();

                //并行请求
                Parallel.ForEach(methods, mh =>
                {
                    string html = Core.HttpTo.Get(urlPrefix + mh.Name);
                    Core.FileTo.WriteText(html, path + mh.Name.ToLower() + ".html", false);
                });

                vm.Set(ARTag.success);
                vm.Data = "Count：" + methods.Count;
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            Core.CacheTo.Remove(cacheKey);

            return vm;
        }
    }
}
