using Microsoft.AspNetCore.Mvc;

namespace Netnr.Test.Controllers
{
    /// <summary>
    /// Netnr.Fast || Netnr.Fast.Extend
    /// </summary>
    public class FastController : Controller
    {
        public IActionResult Index()
        {
            return Ok();
        }

        public IActionResult Test1()
        {
            var osi = new Fast.OSInfoTo();
            return Content(osi.ToView());
        }

        public IActionResult Test2()
        {
            var osi = new Fast.OSInfoTo();
            return Content(osi.ToJson());
        }
    }
}
