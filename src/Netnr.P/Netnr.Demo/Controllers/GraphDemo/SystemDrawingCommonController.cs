using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.Versioning;

namespace Netnr.Demo.Controllers.GraphDemo;

[SupportedOSPlatform("windows")]
[Route("/GraphDemo/[controller]/[action]")]
public class SystemDrawingCommonController(IWebHostEnvironment host) : Controller
{
    public IWebHostEnvironment env = host;

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
            Console.WriteLine(imgPath);
            byte[] bytes = ResizeBin(imgPath, 200, 0, "width");
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
    /// <param name="code">随机码</param>
    [ApiExplorerSettings(IgnoreApi = true)]
    public byte[] CreateImg(string code)
    {
        Random random = new();

        //为验证码插入空格
        for (int i = 0; i < 2; i++)
        {
            code = code.Insert(random.Next(code.Length - 1), " ");
        }

        //验证码颜色集合  
        Color[] colors = [Color.LightBlue, Color.LightCoral, Color.LightGreen, Color.LightPink, Color.LightSkyBlue, Color.LightSteelBlue, Color.LightSalmon];

        //定义图像的大小，生成图像的实例  
        using Bitmap Img = new(code.Length * 22, 38);
        using Graphics g = Graphics.FromImage(Img);

        //背景设为白色
        g.Clear(Color.White);

        //在随机位置画背景点
        for (int i = 0; i < 200; i++)
        {
            int x = random.Next(Img.Width);
            int y = random.Next(Img.Height);
            g.DrawRectangle(new Pen(colors[random.Next(colors.Length - 1)], 0), x, y, 1, 1);
        }

        //验证码绘制
        for (int i = 0; i < code.Length; i++)
        {
            Font f = new(FontFamily.GenericSerif, 24, (FontStyle.Italic | FontStyle.Bold));//字体  
            Brush b = new SolidBrush(colors[random.Next(colors.Length - 1)]);//颜色  

            //控制验证码不在同一高度
            int ii = random.Next(15) * (random.Next(1) % 2 == 0 ? -1 : 1) + 8;
            g.DrawString(code.Substring(i, 1), f, b, (i * 20), ii);//绘制一个验证字符
        }

        using MemoryStream ms = new();
        Img.Save(ms, ImageFormat.Jpeg);

        return ms.ToArray();
    }

    /// <summary>
    /// 生成缩略图
    /// </summary>
    /// <param name="imgPath">原图片地址</param>
    /// <param name="width">缩略图宽度</param>
    /// <param name="height">缩略图高度</param>
    /// <param name="model">生成缩略的模式: wh|width|height|cut </param>
    [ApiExplorerSettings(IgnoreApi = true)]
    public byte[] ResizeBin(string imgPath, int width, int height, string model)
    {
        using Image ImgBox = Image.FromFile(imgPath);

        int minWidth = width;      //缩略图的宽度
        int minHeight = height;    //缩略图的高度

        int x = 0;
        int y = 0;

        int oldWidth = ImgBox.Width;    //原始图片的宽度
        int oldHeight = ImgBox.Height;  //原始图片的高度

        switch (model.ToLower())
        {
            case "wh":      //指定高宽缩放,可能变形
                break;
            case "width":       //指定宽度,高度按照比例缩放
                minHeight = ImgBox.Height * width / ImgBox.Width;
                break;
            case "height":       //指定高度,宽度按照等比例缩放
                minWidth = ImgBox.Width * height / ImgBox.Height;
                break;
            case "cut":
                if (ImgBox.Width / (double)ImgBox.Height > minWidth / (double)minHeight)
                {
                    oldHeight = ImgBox.Height;
                    oldWidth = ImgBox.Height * minWidth / minHeight;
                    y = 0;
                    x = (ImgBox.Width - oldWidth) / 2;
                }
                else
                {
                    oldWidth = ImgBox.Width;
                    oldHeight = oldWidth * height / minWidth;
                    x = 0;
                    y = (ImgBox.Height - oldHeight) / 2;
                }
                break;
            default:
                break;
        }

        //新建一个bmp图片
        using Image bitmap = new Bitmap(minWidth, minHeight);

        //新建一个画板
        using Graphics graphic = Graphics.FromImage(bitmap);

        //设置高质量查值法
        graphic.InterpolationMode = InterpolationMode.High;

        //设置高质量，低速度呈现平滑程度
        graphic.SmoothingMode = SmoothingMode.HighQuality;

        //清空画布并以透明背景色填充
        graphic.Clear(Color.Transparent);

        //在指定位置并且按指定大小绘制原图片的指定部分
        graphic.DrawImage(ImgBox, new Rectangle(0, 0, minWidth, minHeight), new Rectangle(x, y, oldWidth, oldHeight), GraphicsUnit.Pixel);

        using var ms = new MemoryStream();
        bitmap.Save(ms, ImgBox.RawFormat);
        return ms.ToArray();
    }

    /// <summary>
    /// 水印（文字）
    /// </summary>
    /// <param name="imgPath"></param>
    /// <param name="text"></param>
    [ApiExplorerSettings(IgnoreApi = true)]
    public byte[] WatermarkForTextBin(string imgPath, string text)
    {
        using Image image = Image.FromFile(imgPath);

        //新建一个画板
        using Graphics graphic = Graphics.FromImage(image);
        graphic.DrawImage(image, 0, 0, image.Width, image.Height);

        //设置字体
        var fn = OperatingSystem.IsWindows() ? "Microsoft Yahei" : "DejaVu Sans Mono";
        Font f = new(fn, 32);

        //设置字体颜色
        using Brush b = new SolidBrush(Color.DeepPink);

        graphic.DrawString(text, f, b, image.Width * .72f, image.Height * .86f);

        using var ms = new MemoryStream();
        image.Save(ms, image.RawFormat);
        return ms.ToArray();
    }
}