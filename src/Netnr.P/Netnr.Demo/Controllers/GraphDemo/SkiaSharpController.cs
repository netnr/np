﻿using SkiaSharp;
using SkiaSharp.QrCode;

namespace Netnr.Demo.Controllers.GraphDemo;

[Route("/GraphDemo/[controller]/[action]")]
public class SkiaSharpController(IWebHostEnvironment host) : Controller
{
    public IWebHostEnvironment env = host;

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
    /// 缩略图
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    [HttpGet]
    public IActionResult Resize(string path = "images/netnr_avatar.jpg")
    {
        try
        {
            var imgPath = Path.Combine(env.WebRootPath, path);
            byte[] bytes = ResizeBin(imgPath, 200, null);
            return File(bytes, "image/jpeg");
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
    /// <param name="text"></param>
    /// <returns></returns>
    [HttpGet]
    public IActionResult Watermark(string text = "netnr")
    {
        try
        {
            var imgPath = Path.Combine(env.WebRootPath, "images/netnr_avatar.jpg");
            byte[] bytes = WatermarkForTextBin(imgPath, text);
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

        //为验证码插入空格
        for (int i = 0; i < 2; i++)
        {
            code = code.Insert(random.Next(code.Length - 1), " ");
        }

        //验证码颜色集合  
        SKColor[] colors = [SKColors.LightBlue, SKColors.LightCoral, SKColors.LightGreen, SKColors.LightPink, SKColors.LightSkyBlue, SKColors.LightSteelBlue, SKColors.LightSalmon];

        //旋转角度
        int randAngle = 40;

        using SKBitmap bitmap = new(code.Length * 22, 38);
        using SKCanvas canvas = new(bitmap);
        //背景设为白色
        canvas.Clear(SKColors.White);

        //在随机位置画背景点
        for (int i = 0; i < 200; i++)
        {
            int x = random.Next(0, bitmap.Width);
            int y = random.Next(0, bitmap.Height);

            var paint = new SKPaint() { Color = colors[random.Next(colors.Length)] };
            canvas.DrawRect(new SKRect(x, y, x + 2, y + 2), paint);
        }

        //验证码绘制
        for (int i = 0; i < code.Length; i++)
        {
            //角度
            float angle = random.Next(-randAngle, randAngle);

            //不同高度
            int ii = random.Next(20) * (random.Next(1) % 2 == 0 ? -1 : 1) + 20;

            SKPoint point = new(18, 20);

            canvas.Translate(point);
            canvas.RotateDegrees(angle);

            var textPaint = new SKPaint()
            {
                TextAlign = SKTextAlign.Center,
                Color = colors[random.Next(colors.Length)],
                TextSize = 28,
                IsAntialias = true,
                FakeBoldText = true
            };

            canvas.DrawText(code.Substring(i, 1), new SKPoint(0, ii), textPaint);
            canvas.RotateDegrees(-angle);
            canvas.Translate(0, -point.Y);
        }

        canvas.Translate(-4, 0);

        using var image = SKImage.FromBitmap(bitmap);
        using var ms = new MemoryStream();
        image.Encode(SKEncodedImageFormat.Jpeg, 90).SaveTo(ms);

        return ms.ToArray();
    }

    /// <summary>
    /// 缩略图
    /// </summary>
    /// <param name="imgPath"></param>
    /// <param name="width">宽度，null 根据高度自适应</param>
    /// <param name="height">高度，null 根据宽度自适应</param>
    /// <param name="quality">质量，1-100，默认 90</param>
    [HttpGet]
    public byte[] ResizeBin(string imgPath, int? width = null, int? height = null, int quality = 90)
    {
        SKBitmap bitmap = SKBitmap.Decode(imgPath);

        //缩略大小
        double scale = 1;
        if (height.HasValue)
        {
            scale = (double)height.Value / bitmap.Height;
        }
        if (width.HasValue)
        {
            scale = (double)width.Value / bitmap.Width;
        }
        if (!width.HasValue)
        {
            width = Convert.ToInt32(bitmap.Width * scale);
        }
        if (!height.HasValue)
        {
            height = Convert.ToInt32(bitmap.Height * scale);
        }

        //调整大小
        var scaled = bitmap.Resize(new SKImageInfo(width.Value, height.Value), SKFilterQuality.High);

        using SKImage image = SKImage.FromBitmap(scaled);

        var ext = Path.GetExtension(imgPath).TrimStart('.').ToLower();
        if (ext == "jpg")
        {
            ext = "Jpeg";
        }
        var eif = ext.DeEnum<SKEncodedImageFormat>();
        var data = image.Encode(eif, quality);

        return data.ToArray();
    }

    /// <summary>
    /// 水印
    /// </summary>
    /// <param name="imgPath"></param>
    /// <param name="text">文字</param>
    /// <param name="paint">绘画信息</param>
    /// <param name="point">位置</param>
    private static byte[] WatermarkForTextBin(string imgPath, string text, Action<SKPaint> paint = null, Action<SKPoint> point = null)
    {
        SKBitmap bitmap = SKBitmap.Decode(imgPath);

        using SKCanvas canvas = new(bitmap);

        var textPaint = new SKPaint()
        {
            IsAntialias = true,
            FakeBoldText = true,
            TextAlign = SKTextAlign.Right,
            TextSize = 48,
            Color = SKColors.DeepPink
        };

        paint?.Invoke(textPaint);

        var skp = new SKPoint(bitmap.Width * .95f, bitmap.Height * .95f);

        point?.Invoke(skp);

        canvas.DrawText(text, skp, textPaint);

        canvas.Flush();

        using SKImage image = SKImage.FromBitmap(bitmap);

        var ext = Path.GetExtension(imgPath).TrimStart('.').ToLower();
        if (ext == "jpg")
        {
            ext = "Jpeg";
        }
        var eif = ext.DeEnum<SKEncodedImageFormat>();
        var data = image.Encode(eif, 100);

        return data.ToArray();
    }

    /// <summary>
    /// 生成二维码（SkiaSharp.QrCode）
    /// </summary>
    /// <param name="text">文本</param>
    /// <param name="icon">带 logo 图标</param>
    /// <param name="size">大小，默认 200 </param>
    /// <returns></returns>
    [HttpPost]
    public IActionResult QrCode([FromForm] string text, IFormFile icon, [FromForm] int size = 200)
    {
        size = Math.Max(30, size);
        size = Math.Min(99999, size);

        var qr = new QRCodeGenerator().CreateQrCode(text, ECCLevel.H);
        var info = new SKImageInfo(size, size);
        var surface = SKSurface.Create(info);

        if (icon == null)
        {
            surface.Canvas.Render(qr, info.Width, info.Height, SKColor.Parse("FFFFFF"), SKColor.Parse("000000"));
        }
        else
        {
            var fms = new MemoryStream();
            icon.CopyTo(fms);
            var icond = new SkiaSharp.QrCode.Models.IconData
            {
                Icon = SKBitmap.Decode(fms.ToArray()),
                IconSizePercent = 20,
            };
            surface.Canvas.Render(qr, info.Width, info.Height, SKColor.Parse("FFFFFF"), SKColor.Parse("000000"), icond);
        }

        using var data = surface.Snapshot().Encode(SKEncodedImageFormat.Png, 100);
        using var ms = new MemoryStream();
        data.SaveTo(ms);
        var bytes = ms.ToArray();

        return File(bytes, "image/png");
    }
}