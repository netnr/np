using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Netnr.Core;
using System;

namespace Netnr.Test.Controllers
{
    /// <summary>
    /// Netnr.Core
    /// </summary>
    public class CoreController : Controller
    {
        public IActionResult Index()
        {
            Test1();

            return Ok();
        }

        public void Test1()
        {
            Parallel.For(0, 9999, i =>
            {
                var msg = $"索引：{i}，任务{Task.CurrentId}";
                ConsoleTo.Log(msg);
            });

            Console.WriteLine(ConsoleTo.CurrentCacheLog.Count);
        }
    }
}
