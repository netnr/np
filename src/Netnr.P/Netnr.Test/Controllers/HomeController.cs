using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Netnr.Test.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var cm = MethodBase.GetCurrentMethod();
            var listController = Assembly.GetExecutingAssembly().ExportedTypes.Where(x => x.BaseType == cm.DeclaringType.BaseType).ToList();
            var dicTree = new Dictionary<string, List<string>>();
            listController.ForEach(c =>
            {
                var mis = c.GetMethods().Where(x => x.Module == cm.Module).ToList();
                dicTree.Add(c.Name, mis.Select(x => x.Name).ToList());
            });

            return View(dicTree);
        }

        public IActionResult Swagger()
        {
            return Redirect("/swagger");
        }
    }
}
