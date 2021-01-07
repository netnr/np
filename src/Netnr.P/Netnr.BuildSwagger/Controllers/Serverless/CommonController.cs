using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using static Netnr.BuildSwagger.Models.Serverless;

namespace Netnr.BuildSwagger.Controllers.Serverless
{
    /// <summary>
    /// Common 常用
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")]
    public class CommonController : Controller
    {
        /// <summary>
        /// 获取时钟(UTC),默认东8区,中国,自定义时区:东1~12区、西-1~-12区
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("/clock")]
        public ClockResult Clock()
        {
            return new ClockResult();
        }

        /// <summary>
        /// 获取时钟(UTC),默认东8区,中国,自定义时区:东1~12区、西-1~-12区
        /// </summary>
        /// <param name="timezone">东1 ~ 12区、西-1 ~ -12区</param>
        /// <returns></returns>
        [HttpGet, Route("/clock/{timezone}")]
        public ClockResult Clock([FromRoute] int timezone)
        {
            return new ClockResult();
        }

        /// <summary>
        /// 获取IP
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("/ip")]
        public string Ip()
        {
            return null;
        }

        /// <summary>
        /// CSS压缩
        /// </summary>
        /// <param name="content">CSS内容</param>
        /// <param name="options">自定义配置，JSON配置字符串，参考：https://github.com/fmarcia/UglifyCSS </param>
        /// <returns></returns>
        [HttpPost, Route("/minify/css"), Consumes("application/x-www-form-urlencoded")]
        public PublicResult Minify_css([FromForm] string content, [FromForm] string options)
        {
            return null;
        }

        /// <summary>
        /// 生成占位图,默认200x200
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("/svg")]
        public FileResult Svg()
        {
            return null;
        }

        /// <summary>
        /// 生成占位图,默认200x200
        /// </summary>
        /// <param name="wh">自定义宽高，如 500x309</param>
        /// <returns></returns>
        [HttpGet, Route("/svg/{wh}")]
        public FileResult Svg(string wh)
        {
            return null;
        }

        /// <summary>
        /// 生成SVG验证码,默认返回一条
        /// </summary>
        /// <param name="size">验证码长度，默认4位</param>
        /// <param name="ignoreChars">验证码字符中排除，如：0o1i</param>
        /// <param name="noise">干扰线条的数量，默认1</param>
        /// <param name="color">验证码的字符是否有颜色，默认没有，如果设定了背景，则默认有</param>
        /// <param name="background">验证码图片背景颜色</param>
        /// <param name="width">验证码宽，默认150</param>
        /// <param name="height">验证码高，默认50</param>
        /// <param name="fontSize">验证码字体大小</param>
        /// <returns></returns>
        [HttpGet, Route("/captcha")]        
        public CaptchaResult Captcha(int? size, string ignoreChars, int? noise, bool color, string background, int? width, int? height, int? fontSize)
        {
            return new CaptchaResult();
        }

        /// <summary>
        /// 生成SVG验证码,默认返回一条
        /// </summary>
        /// <param name="count">自定义条数（限制1-99，1条为 Object，多条为 Array）</param>
        /// <param name="size">验证码长度，默认4位</param>
        /// <param name="ignoreChars">验证码字符中排除，如：0o1i</param>
        /// <param name="noise">干扰线条的数量，默认1</param>
        /// <param name="color">验证码的字符是否有颜色，默认没有，如果设定了背景，则默认有</param>
        /// <param name="background">验证码图片背景颜色</param>
        /// <param name="width">验证码宽，默认150</param>
        /// <param name="height">验证码高，默认50</param>
        /// <param name="fontSize">验证码字体大小</param>
        /// <returns></returns>
        [HttpGet, Route("/captcha/{count}")]
        public List<CaptchaResult> Captcha(int count, int? size, string ignoreChars, int? noise, bool color, string background, int? width, int? height, int? fontSize)
        {
            return new List<CaptchaResult> { new CaptchaResult() };
        }

        /// <summary>
        /// 生成UUID,默认返回一条
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("/uuid")]
        public string UUID()
        {
            return string.Empty;
        }

        /// <summary>
        /// 生成UUID,默认返回一条
        /// </summary>
        /// <param name="count">自定义条数（限制1-99，1条为字符串，多条为数组JSON）</param>
        /// <returns></returns>
        [HttpGet, Route("/uuid/{count}")]
        public List<string> UUID(int count)
        {
            return new List<string> { string.Empty };
        }
    }
}
