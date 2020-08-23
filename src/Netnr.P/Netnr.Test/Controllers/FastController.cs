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
            Test1();

            return Ok();
        }

        public void Test1()
        {
            
        }
    }
}
