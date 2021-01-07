using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Netnr.BuildSwagger.Models.Serverless;

namespace Netnr.BuildSwagger.Controllers.Serverless
{
    /// <summary>
    /// Bed 图床
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")]
    public class BedController : Controller
    {
        /// <summary>
        /// 聚合图床(获取存储商列表)
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("/bed")]
        public PublicResult Bed()
        {
            return new PublicResult();
        }

        /// <summary>
        /// 聚合图床(上传)
        /// </summary>
        /// <param name="url">存储商接口URL</param>
        /// <param name="file">文件</param>
        /// <returns></returns>
        [HttpPost, Route("/bed")]
        public PublicResult Bed([FromForm] string url, IFormFile file)
        {
            return new PublicResult();
        }
    }
}
