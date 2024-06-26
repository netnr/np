﻿using Netnr.Demo.Controllers.GraphDemo;
using System.Drawing.Text;
using System.Runtime.Versioning;

namespace Netnr.Demo.Controllers
{
    public class GraphController(IWebHostEnvironment host) : Controller
    {
        public IWebHostEnvironment env = host;

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 测试验证码
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [SupportedOSPlatform("windows")]
        public IActionResult CaptchaTest([FromRoute] int id)
        {
            var num = id;
            num = Math.Min(num, 66);
            num = Math.Max(num, 1);

            var code = Guid.NewGuid().ToString("N")[..4];
            var c1 = new ImageSharpController();
            var c2 = new NetVipsController(env);
            var c3 = new SkiaSharpController(env);
            var c4 = new MagickNETController(env);
            var c5 = new SystemDrawingCommonController(env);

            var dicout = new Dictionary<string, object> { { "Loop", num } };

            var sw = new Stopwatch();

            sw.Start();
            try
            {
                for (int i = 0; i < num; i++)
                {
                    c1.CreateImg(code);
                }
                dicout.Add("ImageSharp", sw.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                dicout.Add("ImageSharp", ex.Message);
            }

            sw.Restart();
            try
            {
                for (int i = 0; i < num; i++)
                {
                    c2.CreateImg(code);
                }
                dicout.Add("NetVips", sw.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                dicout.Add("NetVips", ex.Message);
            }

            sw.Restart();
            try
            {
                for (int i = 0; i < num; i++)
                {
                    c3.CreateImg(code);
                }
                dicout.Add("SkiaSharp", sw.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                dicout.Add("SkiaSharp", ex.Message);
            }

            sw.Restart();
            try
            {
                for (int i = 0; i < num; i++)
                {
                    c4.Captcha(code);
                }
                dicout.Add("Magick.NET", sw.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                dicout.Add("Magick.NET", ex.Message);
            }

            sw.Restart();
            try
            {
                for (int i = 0; i < num; i++)
                {
                    c5.Captcha(code);
                }
                dicout.Add("System.Drawing.Common", sw.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                dicout.Add("System.Drawing.Common", ex.Message);
            }

            return Json(dicout);
        }

        /// <summary>
        /// 获取系统已安装字体列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [SupportedOSPlatform("windows")]
        public IActionResult FontFamilyGet()
        {
            var installedFonts = new InstalledFontCollection();
            return Content(installedFonts.Families.ToJson());
        }
    }
}