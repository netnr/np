using Netnr.SharedFast;

namespace Netnr.Blog.Web.Controllers
{
    /// <summary>
    /// 对象存储
    /// </summary>
    [Apps.FilterConfigs.IsAdmin]
    public class StorageController : Controller
    {
        /// <summary>
        /// 腾讯对象存储
        /// </summary>
        /// <returns></returns>
        public IActionResult Tencent()
        {
            return View();
        }

        /// <summary>
        /// 秘钥
        /// </summary>
        public class AccessCOS
        {
            public string Bucket => GlobalTo.GetValue("ApiKey:AccessCOS:Bucket");
            public string AppId => GlobalTo.GetValue("ApiKey:AccessCOS:AppId");
            public string Region => GlobalTo.GetValue("ApiKey:AccessCOS:Region");
            public string SecretId => GlobalTo.GetValue("ApiKey:AccessCOS:SecretId");
            public string SecretKey => GlobalTo.GetValue("ApiKey:AccessCOS:SecretKey");
        }

        [HttpGet, Apps.FilterConfigs.IsAdmin]
        public AccessCOS TencentAPI()
        {
            return new AccessCOS();
        }

        /// <summary>
        /// 网易对象存储
        /// </summary>
        /// <returns></returns>
        public IActionResult NetEasy()
        {
            return View();
        }

        /// <summary>
        /// 秘钥
        /// </summary>
        public class AccessNOS
        {
            public string Bucket => GlobalTo.GetValue("ApiKey:AccessNOS:Bucket");
            public string AccessKeyId => GlobalTo.GetValue("ApiKey:AccessNOS:AccessKeyId");
            public string AccessKeySecret => GlobalTo.GetValue("ApiKey:AccessNOS:AccessKeySecret");
            public string Endpoint => GlobalTo.GetValue("ApiKey:AccessNOS:Endpoint");
        }

        [HttpGet, Apps.FilterConfigs.IsAdmin]
        public AccessNOS NetEasyAPI()
        {
            return new AccessNOS();
        }

        /// <summary>
        /// 七牛对象存储
        /// </summary>
        /// <returns></returns>
        public IActionResult Qiniu()
        {
            ViewData["DateUnix"] = DateTime.Now.AddHours(1).ToTimestamp();
            return View();
        }

        /// <summary>
        /// 秘钥
        /// </summary>
        public class AccessKODO
        {
            public string Bucket => GlobalTo.GetValue("ApiKey:AccessKODO:Bucket");
            public string AccessKey => GlobalTo.GetValue("ApiKey:AccessKODO:AccessKey");
            public string SecretKey => GlobalTo.GetValue("ApiKey:AccessKODO:SecretKey");
        }

        [HttpGet, Apps.FilterConfigs.IsAdmin]
        public AccessKODO QiniuAPI()
        {
            return new AccessKODO();
        }
    }
}