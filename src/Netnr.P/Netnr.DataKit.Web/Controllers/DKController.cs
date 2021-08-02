using Microsoft.AspNetCore.Mvc;

namespace Netnr.DataKit.Web.Controllers
{
    /// <summary>
    /// Netnr.DataKit API
    /// </summary>
    [Route("[controller]/[action]")]
    [ResponseCache(Duration = 2)]
    [Apps.FilterConfigs.AllowCors]
    public class DKController : DKControllerTo
    {

    }
}