using ImageMagick;

namespace Netnr.Demo.Controllers.GraphDemo;

[Route("/GraphDemo/[controller]/[action]")]
public class MagickNETController : Controller
{
    public IWebHostEnvironment env;
    public MagickNETController(IWebHostEnvironment host)
    {
        env = host;
    }

    /// <summary>
    /// 验证码
    /// </summary>
    /// <param name="code"></param>
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

            using var image = new MagickImage(new MagickColor("#ffffff"), 132, 38);
            //噪点
            image.AddNoise(NoiseType.Uniform, 50f, Channels.RGB);

            MagickColor[] colors = { MagickColors.LightBlue, MagickColors.LightCoral, MagickColors.LightGreen, MagickColors.LightPink, MagickColors.LightSkyBlue, MagickColors.LightSteelBlue, MagickColors.LightSalmon };

            var random = new Random();
            for (int i = 0; i < 4; i++)
            {
                //随机角度
                var randomAngle = random.Next(-60, 60);
                //随机X
                int randomX = i * 29 + 10;
                //随机Y
                int randomY = random.Next(25, 35);
                //随机颜色
                var randomC = colors[random.Next(colors.Length)];

                new Drawables()
                    .FontPointSize(30)
                    .TextAlignment(TextAlignment.Left)
                    .FillColor(randomC)
                    .StrokeColor(randomC)
                    .Text(randomX, randomY, code[i].ToString())
                    .Draw(image);
            }

            using var ms = new MemoryStream();
            image.Write(ms, MagickFormat.Jpeg);

            return File(ms.ToArray(), "image/jpeg");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// 水印
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public IActionResult Watermark()
    {
        try
        {
            //读取图片
            using var image = new MagickImage(Path.Combine(env.WebRootPath, "images/netnr_avatar.jpg"));

            //水印图片
            using var watermark = new MagickImage(Path.Combine(env.WebRootPath, "favicon.ico"));

            // 水印更透明
            //watermark.Evaluate(Channels.Alpha, EvaluateOperator.Divide, 4);
            // 指定位置水印
            image.Composite(watermark, 230, 60, CompositeOperator.Over);

            // 保存
            using var ms = new MemoryStream();
            image.Write(ms);

            return File(ms.ToArray(), "image/jpeg");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// 读取图片 exif 信息
    /// </summary>
    /// <param name="file">上传</param>
    /// <returns></returns>
    [HttpPost]
    public IActionResult ReadExifData(IFormFile file = null)
    {
        try
        {
            using var image = new MagickImage();
            if (file != null)
            {
                image.Read(file.OpenReadStream());
            }
            else
            {
                image.Read(Path.Combine(env.WebRootPath, "netnr_avatar.jpg"));
            }

            var profile = image.GetExifProfile();
            if (profile == null)
            {
                return BadRequest("Image does not contain exif information.");
            }
            else
            {
                return Json(profile);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return BadRequest(ex.Message);
        }
    }
}
