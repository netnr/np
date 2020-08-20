using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Netnr.CaptchaDemo.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 测试
        /// </summary>
        /// <returns></returns>
        public IActionResult Test()
        {
            int.TryParse(RouteData.Values["id"]?.ToString(), out int num);
            num = Math.Min(num, 99);
            num = Math.Max(num, 1);

            var code = Guid.NewGuid().ToString("N").Substring(0, 4);
            var c1 = new SixLaborsImageSharpDrawingController();
            var c2 = new SystemDrawingCommonController();
            var c3 = new SkiaSharpController();
            var c4 = new MagickNETController();

            var dicout = new Dictionary<string, object>
            {
                { "Loop", num }
            };

            var sw = new Stopwatch();
            sw.Start();

            try
            {
                for (int i = 0; i < num; i++)
                {
                    c1.CreateImg(code);
                }
                dicout.Add("SixLabors.ImageSharp.Drawing", sw.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                dicout.Add("SixLabors.ImageSharp.Drawing", ex.Message);
            }

            sw.Restart();

            try
            {
                for (int i = 0; i < num; i++)
                {
                    c2.CreateImg(code);
                }
                dicout.Add("System.Drawing.Common", sw.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                dicout.Add("System.Drawing.Common", ex.Message);
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
                    c4.CreateImg(code);
                }
                dicout.Add("Magick.NET", sw.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                dicout.Add("Magick.NET", ex.Message);
            }

            return Json(dicout);
        }
    }
}
