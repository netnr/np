namespace Netnr.DataKit.Controllers
{
    /// <summary>
    /// Netnr.DataKit API
    /// </summary>
    [Route("[controller]/[action]")]
    [ResponseCache(Duration = 1)]
    [Filters.FilterConfigs.AllowCors]
    public class DKController : DataKitController
    {

    }
}