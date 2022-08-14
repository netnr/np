using Microsoft.AspNetCore.Mvc;
using NetVips;

namespace Netnr.GraphDemo.Controllers;

[Route("[controller]/[action]")]
public class NetVipsController : Controller
{
    public IWebHostEnvironment env;
    public NetVipsController(IWebHostEnvironment host)
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
    /// <param name="code"></param>
    /// <returns></returns>
    [HttpGet]
    public byte[] CreateImg(string code)
    {
        var random = new Random();

        var textLayer = Image.Black(code.Length * 22, 38);

        for (int i = 0; i < code.Length; i++)
        {
            var letter = Image.Text(code.Substring(i, 1), dpi: 300);
            var image = letter.Gravity(Enums.CompassDirection.Centre, letter.Width + 40, letter.Height + 40);

            // random scale and rotate
            using var similarity = image.Similarity(scale: random.NextDouble() / 2 + 0.8, angle: random.Next(0, 40));

            // random colour
            var colour = Enumerable.Range(1, 3).Select(i => random.Next(0, 255)).ToArray();
            using var ifthenelse = similarity.Ifthenelse(colour, 0, blend: true);

            // tag as 9-bit srgb
            using var srgb = ifthenelse.Copy(interpretation: Enums.Interpretation.Srgb);
            using var cast = srgb.Cast(Enums.BandFormat.Uchar);

            // position at our write position in the image
            using var embed = cast.Embed(i * 60, 0, image.Width + i * 60, image.Height);

            using var add = textLayer + embed;
            textLayer = add.Cast(Enums.BandFormat.Uchar);
        }

        // make an alpha for the text layer: just a mono version of the image, but scaled
        // up so the letters themselves are not transparent
        using var mono = textLayer.Colourspace(Enums.Interpretation.Bw);
        using var alpha = mono.Cast(Enums.BandFormat.Uchar);
        textLayer = textLayer.Bandjoin(alpha);

        //  make a white background with random speckles
        using var speckles = Image.Gaussnoise(textLayer.Width, textLayer.Height, mean: 400, sigma: 200);
        using var background = Enumerable.Range(1, 2).Aggregate(speckles, (a, b) =>
        {
            var speckles2 = Image.Gaussnoise(textLayer.Width, textLayer.Height, mean: 400, sigma: 200);
            var join = a.Bandjoin(speckles2);
            var srgb = join.Copy(interpretation: Enums.Interpretation.Srgb);
            return srgb.Cast(Enums.BandFormat.Uchar);
        });

        // composite the text over the background
        var final = background.Composite(textLayer, Enums.BlendMode.Over);
        return final.WriteToBuffer(".jpg[Q=95]");
    }

    /// <summary>
    /// 智能剪裁
    /// </summary>
    /// <param name="file">图片</param>
    /// <param name="w_h">宽_高，例 300_200</param>
    /// <param name="format">格式</param>
    /// <returns></returns>
    [HttpPost]
    public FileResult SmartCrop(IFormFile file, [FromForm] string w_h = "61_100", [FromForm] string format = "png")
    {
        var w_hArr = w_h.Split('_');
        var w = int.Parse(w_hArr[0]);
        var h = int.Parse(w_hArr[1]);

        var formatString = $".{format}[Q=95]";
        var contentType = $"image/{format}";

        if (file != null)
        {
            using var ms = new MemoryStream();
            file.CopyTo(ms);

            using var image = Image.ThumbnailBuffer(ms.ToArray(), width: w, height: h, crop: Enums.Interesting.Attention);
            return File(image.WriteToBuffer(formatString), contentType);
        }
        else
        {
            var fileName = Path.Combine(env.WebRootPath, "netnr_avatar.jpg");
            using var image = Image.Thumbnail(fileName, width: w, height: h, crop: Enums.Interesting.Attention);
            return File(image.WriteToBuffer(formatString), contentType);
        }
    }
}