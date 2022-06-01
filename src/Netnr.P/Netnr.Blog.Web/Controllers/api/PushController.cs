using Newtonsoft.Json.Linq;
using Netnr.Core;
using Netnr.SharedFast;

namespace Netnr.Blog.Web.Controllers.api
{
    public partial class APIController : ControllerBase
    {
        /// <summary>
        /// 推送测试公众号
        /// https://mp.weixin.qq.com/debug/cgi-bin/sandbox?t=sandbox/login
        /// </summary>
        /// <param name="title"></param>
        /// <param name="msg"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        [HttpPost]
        [Apps.FilterConfigs.AllowCors]
        public SharedResultVM Push([FromForm] string title = "", [FromForm] string msg = "", [FromForm] string url = "")
        {
            return Application.PushService.Push(title, msg, url);
        }

        /// <summary>
        /// 推送菜单（管理员）
        /// </summary>
        /// <param name="json"></param>
        /// <param name="access_token"></param>
        /// <returns></returns>
        [HttpPost]
        [Apps.FilterConfigs.IsAdmin]
        public SharedResultVM PushMenu([FromForm] string json = "", [FromForm] string access_token = "")
        {
            return Application.PushService.PushMenu(json, access_token);
        }
    }
}