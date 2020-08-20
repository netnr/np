using System;
using System.IO;
using System.Threading;
using ImageMagick;
using Microsoft.AspNetCore.Mvc;

namespace Netnr.CaptchaDemo.Controllers
{
    public class MagickNETController : Controller
    {
        public IActionResult Index()
        {
            try
            {
                string num = Guid.NewGuid().ToString("N").Substring(0, 4).ToUpper();
                byte[] bytes = CreateImg(num);
                return File(bytes, "image/jpeg");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }


        /// <summary>
        /// 生成图片验证码
        /// </summary>
        /// <param name="code">随机码</param>
        public byte[] CreateImg(string code)
        {
            Random random = new Random();

            //为验证码插入空格
            for (int i = 0; i < 2; i++)
            {
                code = code.Insert(random.Next(code.Length - 1), " ");
            }

            //验证码颜色集合  
            MagickColor[] colors = { MagickColors.LightBlue, MagickColors.LightCoral, MagickColors.LightGreen, MagickColors.LightPink, MagickColors.LightSkyBlue, MagickColors.LightSteelBlue, MagickColors.LightSalmon };

            using (var image = new MagickImage(MagickColors.White, code.Length * 22, 38))
            {
                //在随机位置画背景点
                for (int i = 0; i < 200; i++)
                {
                    int x = random.Next(0, image.Width);
                    int y = random.Next(0, image.Height);

                    new Drawables()
                        .FillColor(colors[random.Next(colors.Length - 1)])
                        .Rectangle(x, y, x + 1, y + 1).Draw(image);
                }

                //验证码绘制
                for (int i = 0; i < code.Length; i++)
                {
                    //不同高度
                    int ii = random.Next(15) * (random.Next(1) % 2 == 0 ? -1 : 1) + 35;

                    new Drawables()
                        .FontPointSize(28)
                        .FillColor(colors[random.Next(colors.Length - 1)])
                        .TextAlignment(TextAlignment.Center)
                        .Text(18 * i + 8, ii, code.Substring(i, 1))
                        .Draw(image);
                }

                image.Format = MagickFormat.Jpeg;

                return image.ToByteArray();
            }
        }
    }
}
