using Microsoft.AspNetCore.Mvc;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Netnr.GraphDemo.Controllers;

[Route("[controller]/[action]")]
public class SixLaborsImageSharpDrawingController : Controller
{
    /// <summary>
    /// 验证码
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public IActionResult Captcha(string code = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                code = Guid.NewGuid().ToString("N")[..4].ToUpper();
            }

            byte[] bytes = CreateImg(code);
            return File(bytes, "image/jpeg");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// 生成图片验证码
    /// </summary>
    /// <param name="code">随机码</param>
    [HttpGet]
    public byte[] CreateImg(string code)
    {
        var random = new Random();

        //为验证码插入空格
        for (int i = 0; i < 2; i++)
        {
            code = code.Insert(random.Next(code.Length - 1), " ");
        }

        //验证码颜色集合  
        Color[] Colors = { Color.LightBlue, Color.LightCoral, Color.LightGreen, Color.LightPink, Color.LightSkyBlue, Color.LightSteelBlue, Color.LightSalmon };

        var image = new Image<Rgba32>(code.Length * 22, 38);
        var font = SystemFonts.CreateFont(SystemFonts.Families.First().Name, 30, FontStyle.BoldItalic);

        image.Mutate(ctx =>
        {
            //背景设为白色
            ctx.Fill(Color.White);

            //在随机位置画背景点  
            for (int i = 0; i < 200; i++)
            {
                var pen = new Pen(Colors[random.Next(Colors.Length)], 1);

                var p1 = new PointF(random.Next(image.Width), random.Next(image.Height));
                var p2 = new PointF(p1.X + 2f, p1.Y + 2f);

                ctx.DrawLines(pen, p1, p2);
            }

            //验证码绘制
            for (int i = 0; i < code.Length; i++)
            {
                //控制验证码不在同一高度
                int ii = random.Next(15) * (random.Next(1) % 2 == 0 ? -1 : 1) + 12;
                ctx.DrawText(code.Substring(i, 1), font, Colors[random.Next(Colors.Length)], new PointF(20 * i + 10, ii));
            }
        });

        MemoryStream ms = new();
        image.SaveAsJpeg(ms);
        return ms.ToArray();
    }
}
