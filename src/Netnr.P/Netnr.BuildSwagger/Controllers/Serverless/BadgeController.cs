using Microsoft.AspNetCore.Mvc;

namespace Netnr.BuildSwagger.Controllers.Serverless
{
    /// <summary>
    /// Badge 徽章
    /// </summary>
    public class BadgeController : Controller
    {
        /// <summary>
        /// 获取 NuGet 发布包的最新版本
        /// </summary>
        /// <param name="package">发布包名</param>
        /// <returns></returns>
        [HttpGet, Route("/badge/nuget/v/{package}.svg")]
        public FileResult nuget_v(string package)
        {
            return null;
        }

        /// <summary>
        /// 获取 NPM 发布包的最新版本
        /// </summary>
        /// <param name="package">发布包名</param>
        /// <returns></returns>
        [HttpGet, Route("/badge/npm/v/{package}.svg")]
        public FileResult npm_v(string package)
        {
            return null;
        }
    }
}
