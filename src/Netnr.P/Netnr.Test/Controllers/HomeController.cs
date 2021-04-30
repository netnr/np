using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Netnr.Test.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var ass = System.Reflection.Assembly.GetExecutingAssembly();
            var listController = ass.ExportedTypes.Where(x => x.BaseType?.FullName == "Microsoft.AspNetCore.Mvc.Controller").ToList();
            var dicTree = new Dictionary<string, List<string>>();
            listController.ForEach(c =>
            {
                var mis = c.GetMethods().Where(x => x.Module.Name == "Netnr.Test.dll").ToList();
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
