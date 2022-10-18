namespace Netnr.ResponseFramework.Web.Controllers
{
    /// <summary>
    /// Netnr.DataKit API （管理员）
    /// </summary>
    [Route("[controller]/[action]")]
    [FilterConfigs.IsAdmin]
    public class DKController : DataKitController
    {

    }
}