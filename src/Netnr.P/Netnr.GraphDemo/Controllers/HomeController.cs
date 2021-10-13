namespace Netnr.GraphDemo.Controllers;

public class HomeController : Controller
{
    public IWebHostEnvironment env;
    public HomeController(IWebHostEnvironment host)
    {
        env = host;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Captcha()
    {
        return View();
    }

    public IActionResult Resize()
    {
        return View();
    }

    public IActionResult Watermark()
    {
        return View();
    }

    /// <summary>
    /// 测试验证码
    /// </summary>
    /// <returns></returns>
    public IActionResult TestCaptcha()
    {
        _ = int.TryParse(RouteData.Values["id"]?.ToString(), out int num);
        num = Math.Min(num, 66);
        num = Math.Max(num, 1);

        var code = Guid.NewGuid().ToString("N").Substring(0, 4);
        var c1 = new SixLaborsImageSharpDrawingController();
        var c2 = new SystemDrawingCommonController(env);
        var c3 = new SkiaSharpController(env);

        var dicout = new Dictionary<string, object> { { "Loop", num } };

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

        return Json(dicout);
    }

    /// <summary>
    /// 测试缩略图
    /// </summary>
    /// <returns></returns>
    public IActionResult TestResize()
    {
        return Content("");
    }
}
