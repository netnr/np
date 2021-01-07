using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Netnr.BuildSwagger.Models.Serverless;

namespace Netnr.BuildSwagger.Controllers.Serverless
{
    /// <summary>
    /// AI 人工智能
    /// </summary>
    public class AIController : Controller
    {
        /// <summary>
        /// OCR 通用文字识别(百度接口,50000次/天免费,用自己申请的授权信息更稳定不受限制)
        /// </summary>
        /// <param name="file">待识别的图片文件（二选一）</param>
        /// <param name="url">远程图片地址（二选一）</param>
        /// <param name="APP_ID">百度AI接口：APP_ID（可选，用自己申请的授权信息更稳定不受限制）</param>
        /// <param name="API_KEY">百度AI接口：API_KEY（可选，用自己申请的授权信息更稳定不受限制）</param>
        /// <param name="SECRET_KEY">百度AI接口：SECRET_KEY（可选，用自己申请的授权信息更稳定不受限制）</param>
        /// <returns></returns>
        [HttpPost, Route("/aip/ocr")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")]
        public PublicResult Aip_ocr(IFormFile file, [FromForm] string url, [FromForm] string APP_ID, [FromForm] string API_KEY, [FromForm] string SECRET_KEY)
        {
            return new PublicResult();
        }

        /// <summary>
        /// 内容分词解析(nodejieba 组件)
        /// </summary>
        /// <param name="lang">语言，默认 zh-cn</param>
        /// <param name="ctype">命令类型，0：分词（默认，可不传）；1：关键词</param>
        /// <param name="content">内容</param>
        /// <returns></returns>
        [HttpPost, Route("/analysis"), Consumes("application/x-www-form-urlencoded")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")]
        public PublicResult Analysis([FromForm] string lang, [FromForm] int ctype, [FromForm] string content = "结过婚的和尚未结过婚的")
        {
            return new PublicResult();
        }
    }
}
