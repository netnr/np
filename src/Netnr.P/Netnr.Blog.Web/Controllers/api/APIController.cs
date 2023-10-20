using System.Net;
using System.Globalization;
using SkiaSharp;
using SkiaSharp.QrCode;
using System.Net.Http;

namespace Netnr.Blog.Web.Controllers
{
    /// <summary>
    /// 公共接口
    /// </summary>
    [Route("api/v1/[action]")]
    public partial class APIController : ControllerBase
    {
        /// <summary>
        /// 登录用户个人信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ResultVM UserInfo()
        {
            var vm = new ResultVM();

            var uinfo = IdentityService.Get(HttpContext);
            if (uinfo != null)
            {
                vm.Data = uinfo;
                vm.Set(RCodeTypes.success);
            }
            else
            {
                vm.Set(RCodeTypes.unauthorized);
            }

            return vm;
        }

        /// <summary>
        /// 推送消息
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="touser">可选</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResultVM> PushAsync([FromForm] string title = "", [FromForm] string content = "", [FromForm] string touser = "@all")
        {
            return await PushService.PushWeChat(title, content, touser);
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="content">消息主体</param>
        /// <param name="toMails">接收邮箱，多个逗号分割</param>
        /// <param name="token">授权码</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResultVM> SendEmail([FromForm] string title, [FromForm] string content, [FromForm] string toMails, [FromForm] string token)
        {
            var vm = new ResultVM();
            if (!string.IsNullOrWhiteSpace(token) && token.Length > 5 && AppTo.GetValue("Common:GlobalKey").Contains(token))
            {
                vm = await PushService.SendEmail(title, content, toMails);
            }
            else
            {
                vm.Set(RCodeTypes.unauthorized);
            }

            return vm;
        }

        /// <summary>
        /// 上传检测
        /// </summary>
        /// <param name="file">文件</param>
        /// <param name="content">文件内容</param>
        /// <param name="ext"></param>
        /// <param name="subdir">输出完整物理路径，用于存储</param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        internal static async Task<ResultVM> UploadCheck(IFormFile file, byte[] content, string ext, string subdir)
        {
            var vm = new ResultVM();

            if (file != null)
            {
                ext = Path.GetExtension(file.FileName);
            }

            if (file == null || ParsingTo.IsRiskExtension(file.FileName))
            {
                vm.Set(RCodeTypes.refuse);
                vm.Msg = "File extension not supported";
            }
            else
            {
                var now = DateTime.Now;
                string filename = now.ToString("MMddHHmmss") + RandomTo.NewNumber(3) + ext;

                if (!string.IsNullOrWhiteSpace(subdir) && !ParsingTo.IsLinkPath(subdir))
                {
                    vm.Set(RCodeTypes.failure);
                    vm.Msg = "subdir 仅为字母、数字";
                }
                else
                {
                    //虚拟路径
                    var vpath = CommonService.UrlRelativePath(null, subdir, now.ToString("yyyy"));

                    //物理路径
                    var ppath = ParsingTo.Combine(AppTo.WebRootPath, vpath);
                    //创建物理目录
                    if (!Directory.Exists(ppath))
                    {
                        Directory.CreateDirectory(ppath);
                    }

                    using var fs = new FileStream(ParsingTo.Combine(ppath, filename), FileMode.CreateNew);
                    if (file != null)
                    {
                        await file.CopyToAsync(fs);
                    }
                    else
                    {
                        await fs.WriteAsync(content);
                    }
                    fs.Flush();
                    fs.Close();

                    //输出
                    vm.Data = ParsingTo.Combine(vpath, filename);

                    vm.Set(RCodeTypes.success);
                }
            }

            return vm;
        }

        /// <summary>
        /// 上传（文件）
        /// </summary>
        /// <param name="file">文件</param>
        /// <param name="subdir">可选，默认 /static，自定义子路径，如：/static/draw</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResultVM> Upload(IFormFile file, [FromForm] string subdir = "/static")
        {
            var vm = new ResultVM();

            try
            {
                if (AppTo.GetValue<bool?>("DisableDatabaseWrite") != true && !HttpContext.User.Identity.IsAuthenticated)
                {
                    vm.Set(RCodeTypes.unauthorized);
                }
                else if (file == null)
                {
                    vm.Set(RCodeTypes.failure);
                }
                else
                {
                    vm = await UploadCheck(file, null, "", subdir);
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        /// <summary>
        /// 上传（Base64）
        /// </summary>
        /// <param name="content">内容 Base64 编码</param>
        /// <param name="ext">后缀，以点“.”开头，如：.txt</param>
        /// <param name="subdir">可选，默认 /static，自定义子路径，如：/static/draw</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResultVM> UploadBase64([FromForm] string content, [FromForm] string ext, [FromForm] string subdir = "/static")
        {
            var vm = new ResultVM();

            try
            {
                if (AppTo.GetValue<bool?>("DisableDatabaseWrite") != true && !HttpContext.User.Identity.IsAuthenticated)
                {
                    vm.Set(RCodeTypes.unauthorized);
                }
                else if (content == null)
                {
                    vm.Set(RCodeTypes.failure);
                }
                else
                {
                    //删除 “,” 前面的字符串
                    content = content[(content.IndexOf(",") + 1)..].Trim();
                    var bs = Convert.FromBase64String(content);

                    vm = await UploadCheck(null, bs, ext, subdir);
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        /// <summary>
        /// 上传（文本）
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="ext">后缀，以点“.”开头，如：.txt</param>
        /// <param name="subdir">可选，默认 /static，自定义子路径，如：/static/draw</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResultVM> UploadText([FromForm] string content, [FromForm] string ext, [FromForm] string subdir = "/static")
        {
            var vm = new ResultVM();

            try
            {
                if (AppTo.GetValue<bool?>("DisableDatabaseWrite") != true && !HttpContext.User.Identity.IsAuthenticated)
                {
                    vm.Set(RCodeTypes.unauthorized);
                }
                else if (content == null)
                {
                    vm.Set(RCodeTypes.failure);
                }
                else
                {
                    var bs = Encoding.UTF8.GetBytes(content);
                    vm = await UploadCheck(null, bs, ext, subdir);
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        /// <summary>
        /// 获取时钟(UTC),默认东8区,中国,自定义时区:东1~12区、西-1~-12区
        /// </summary>
        /// <param name="timezone">东1 ~ 12区、西-1 ~ -12区</param>
        /// <returns></returns>
        [HttpGet, Route(""), Route("{timezone}")]
        public ResultVM Clock([FromRoute] int? timezone = 8)
        {
            var vm = new ResultVM();
            try
            {
                var ci = new ClientInfoTo(HttpContext);
                var client_ip = ci.IP;

                timezone ??= 8;
                timezone = Math.Min(12, timezone.Value);
                timezone = Math.Max(-12, timezone.Value);

                var utc_datetime = DateTime.UtcNow;
                var unixtime = utc_datetime.ToTimestamp(true);
                var datetime = utc_datetime.AddHours(timezone.Value);
                var day_of_week = (int)datetime.DayOfWeek;
                var day_of_year = datetime.DayOfYear;

                var week_number = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(datetime, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

                vm.Data = new
                {
                    //周数
                    week_number,
                    //UTC 时间
                    utc_datetime,
                    //时间戳
                    unixtime,
                    //天数
                    day_of_year,
                    //星期
                    day_of_week,
                    //时间
                    datetime,
                    //时区
                    time_zone = timezone,
                    //IP
                    client_ip,
                };
                vm.Set(RCodeTypes.success);
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        /// <summary>
        /// 获取IP
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string IP() => new ClientInfoTo(HttpContext).IP;

        /// <summary>
        /// 生成GUID
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string GUID() => Guid.NewGuid().ToString();

        /// <summary>
        /// 生成GUID,默认返回一条
        /// </summary>
        /// <param name="count">自定义条数（限制1-99，1条为字符串，多条为数组JSON）</param>
        /// <returns></returns>
        [HttpGet, Route("{count}")]
        public List<string> GUID([FromRoute] int count = 9)
        {
            count = Math.Max(1, count);
            count = Math.Min(99, count);

            var list = new List<string>();
            for (int i = 0; i < count; i++)
            {
                list.Add(Guid.NewGuid().ToString());
            }

            return list;
        }

        /// <summary>
        /// 生成雪花ID,默认返回一条
        /// </summary>
        /// <param name="count">自定义条数（限制1-99）</param>
        /// <returns></returns>
        [HttpGet, Route("{count}")]
        public List<long> SnowflakeId([FromRoute] int count = 1)
        {
            count = Math.Max(1, count);
            count = Math.Min(99, count);

            var list = new List<long>();
            for (int i = 0; i < count; i++)
            {
                list.Add(Snowflake53To.Id());
            }

            return list;
        }

        /// <summary>
        /// 百度AI参数检查
        /// </summary>
        /// <param name="file"></param>
        /// <param name="API_KEY"></param>
        /// <param name="SECRET_KEY"></param>
        /// <returns></returns>
        private static byte[] AipCheck(IFormFile file, ref string API_KEY, ref string SECRET_KEY)
        {
            if (string.IsNullOrWhiteSpace(API_KEY) && string.IsNullOrWhiteSpace(SECRET_KEY))
            {
                API_KEY = AppTo.GetValue("ApiKey:Aip:API_KEY");
                SECRET_KEY = AppTo.GetValue("ApiKey:Aip:SECRET_KEY");
            }

            byte[] bytes = null;
            if (file != null)
            {
                using var ms = new MemoryStream();
                file.CopyTo(ms);
                bytes = ms.ToArray();
            }
            return bytes;
        }

        /// <summary>
        /// OCR 通用文字识别 50,000次/天
        /// </summary>
        /// <param name="file">待识别的图片文件（二选一）</param>
        /// <param name="url">远程图片地址（二选一）</param>
        /// <param name="API_KEY">百度AI接口：API_KEY（可选，用自己申请的授权信息更稳定不受限制）</param>
        /// <param name="SECRET_KEY">百度AI接口：SECRET_KEY（可选，用自己申请的授权信息更稳定不受限制）</param>
        /// <returns></returns>
        [HttpPost]
        public ResultVM OCR(IFormFile file, [FromForm] string url, [FromForm] string API_KEY, [FromForm] string SECRET_KEY)
        {
            var vm = new ResultVM();
            try
            {
                var bytes = AipCheck(file, ref API_KEY, ref SECRET_KEY);
                var ocr = new Baidu.Aip.Ocr.Ocr(API_KEY, SECRET_KEY);

                if (bytes != null)
                {
                    vm.Data = ocr.GeneralBasic(bytes).ToString().DeJson();
                    vm.Set(RCodeTypes.success);
                }
                else if (!string.IsNullOrWhiteSpace(url))
                {
                    vm.Data = ocr.GeneralBasicUrl(url).ToString().DeJson();
                    vm.Set(RCodeTypes.success);
                }
                else
                {
                    vm.Set(RCodeTypes.failure);
                }

                vm.Log.Add("通用文字识别(百度接口,50000次/天免费,用自己申请的授权信息更稳定不受限制)");
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        private Dictionary<string, string> DictOCRType { get; set; } = new Dictionary<string, string>
        {
            { "General", "通用文字识别（标准含位置版） 500次/天" },
            { "AccurateBasic", "通用文字识别（高精度版） 500次/天" },
            { "Accurate", "通用文字识别（高精度含位置版） 50次/天" },
            { "WebImage", "网络图片文字识别 500次/天" },
            { "Numbers", "数字识别 200次/天" },
            { "Handwriting", "手写文字识别 50次/天" },
            { "Idcard", "身份证识别 500次/天" },
            { "Bankcard", "银行卡识别 500次/天" },
            { "BusinessLicense", "营业执照识别 200次/天" },
            { "VatInvoice", "增值税发票识别 500次/天" },
            { "TrainTicket", "火车票识别 50次/天" },
            { "TaxiReceipt", "出租车票识别 50次/天" },
            { "Receipt", "通用票据识别 200次/天" },
            { "VehicleLicense", "行驶证识别 200次/天" },
            { "DrivingLicense", "驾驶证识别 200次/天" },
            { "LicensePlate", "车牌识别 200次/天" },
            { "Seal", "印章识别 100次/天" }
        };

        /// <summary>
        /// OCR 支持的子类型
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ResultVM OCRType()
        {
            var vm = new ResultVM
            {
                Data = DictOCRType
            };
            vm.Set(RCodeTypes.success);

            return vm;
        }

        /// <summary>
        /// OCR
        /// </summary>
        /// <param name="type">类别</param>
        /// <param name="file">待识别的图片文件（二选一）</param>
        /// <param name="side">身份证边，头像：avatar</param>
        /// <param name="API_KEY">百度AI接口：API_KEY（可选，用自己申请的授权信息更稳定不受限制）</param>
        /// <param name="SECRET_KEY">百度AI接口：SECRET_KEY（可选，用自己申请的授权信息更稳定不受限制）</param>
        /// <returns></returns>
        [HttpPost, Route("{type}")]
        public ResultVM OCR([FromRoute] string type, IFormFile file, [FromForm] string side, [FromForm] string API_KEY, [FromForm] string SECRET_KEY)
        {
            var vm = new ResultVM();
            try
            {
                var bytes = AipCheck(file, ref API_KEY, ref SECRET_KEY);
                var ocr = new Baidu.Aip.Ocr.Ocr(API_KEY, SECRET_KEY);

                switch (type?.ToLower())
                {
                    case "general":
                        vm.Log.Add("通用文字识别（标准含位置版） 500次/天");
                        vm.Data = ocr.General(bytes);
                        vm.Set(RCodeTypes.success);
                        break;
                    case "accuratebasic":
                        vm.Log.Add("通用文字识别（高精度版） 500次/天");
                        vm.Data = ocr.AccurateBasic(bytes);
                        vm.Set(RCodeTypes.success);
                        break;
                    case "accurate":
                        vm.Log.Add("通用文字识别（高精度含位置版） 50次/天");
                        vm.Data = ocr.Accurate(bytes);
                        vm.Set(RCodeTypes.success);
                        break;
                    case "numbers":
                        vm.Log.Add("数字识别 200次/天");
                        vm.Data = ocr.Numbers(bytes);
                        vm.Set(RCodeTypes.success);
                        break;
                    case "webimage":
                        vm.Log.Add("网络图片文字识别 500次/天");
                        vm.Data = ocr.WebImage(bytes);
                        vm.Set(RCodeTypes.success);
                        break;
                    case "handwriting":
                        vm.Log.Add("手写文字识别 50次/天");
                        vm.Data = ocr.Handwriting(bytes);
                        vm.Set(RCodeTypes.success);
                        break;
                    case "idcard":
                        vm.Log.Add("身份证识别 500次/天");
                        vm.Data = ocr.Idcard(bytes, side == "avatar" ? "front" : "back");
                        vm.Set(RCodeTypes.success);
                        break;
                    case "bankcard":
                        vm.Log.Add("银行卡识别 500次/天");
                        vm.Data = ocr.Bankcard(bytes);
                        vm.Set(RCodeTypes.success);
                        break;
                    case "businesslicense":
                        vm.Log.Add("营业执照识别 200次/天");
                        vm.Data = ocr.BusinessLicense(bytes);
                        vm.Set(RCodeTypes.success);
                        break;
                    case "vatinvoice":
                        vm.Log.Add("增值税发票识别 500次/天");
                        vm.Data = ocr.VatInvoice(bytes);
                        vm.Set(RCodeTypes.success);
                        break;
                    case "trainticket":
                        vm.Log.Add("火车票识别 50次/天");
                        vm.Data = ocr.TrainTicket(bytes);
                        vm.Set(RCodeTypes.success);
                        break;
                    case "taxireceipt":
                        vm.Log.Add("出租车票识别 50次/天");
                        vm.Data = ocr.TaxiReceipt(bytes);
                        vm.Set(RCodeTypes.success);
                        break;
                    case "receipt":
                        vm.Log.Add("通用票据识别 200次/天");
                        vm.Data = ocr.Receipt(bytes);
                        vm.Set(RCodeTypes.success);
                        break;
                    case "vehiclelicense":
                        vm.Log.Add("行驶证识别 200次/天");
                        vm.Data = ocr.VehicleLicense(bytes);
                        vm.Set(RCodeTypes.success);
                        break;
                    case "drivinglicense":
                        vm.Log.Add("驾驶证识别 200次/天");
                        vm.Data = ocr.DrivingLicense(bytes);
                        vm.Set(RCodeTypes.success);
                        break;
                    case "licenseplate":
                        vm.Log.Add("车牌识别 200次/天");
                        vm.Data = ocr.LicensePlate(bytes);
                        vm.Set(RCodeTypes.success);
                        break;
                    case "seal":
                        vm.Log.Add("印章识别 100次/天");
                        vm.Data = ocr.Seal(bytes);
                        vm.Set(RCodeTypes.success);
                        break;
                    default:
                        vm.Set(RCodeTypes.failure);
                        break;
                }

                if (vm.Code == 200)
                {
                    vm.Data = vm.Data.ToString().DeJson();
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        /// <summary>
        /// NLP 支持的子类型
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ResultVM NLPType()
        {
            var vm = new ResultVM
            {
                Data = new Dictionary<string, string>
                {
                    { "CommentTag", "评论观点抽取" },
                    { "SentimentClassify", "情感倾向分析" },
                    { "Keyword", "文章标签" },
                    { "Ecnet", "文本纠错" },
                    { "NewsSummary", "新闻摘要" }
                }
            };

            vm.Set(RCodeTypes.success);

            return vm;
        }

        /// <summary>
        /// NLP
        /// </summary>
        /// <param name="type">类别</param>
        /// <param name="title">标题（文章标签 必填）</param>
        /// <param name="content">内容</param>
        /// <param name="maxSummaryLen">最大长度（新闻摘要 必填）</param>
        /// <param name="API_KEY">百度AI接口：API_KEY（可选，用自己申请的授权信息更稳定不受限制）</param>
        /// <param name="SECRET_KEY">百度AI接口：SECRET_KEY（可选，用自己申请的授权信息更稳定不受限制）</param>
        /// <returns></returns>
        [HttpPost, Route("{type}")]
        public ResultVM NLP([FromRoute] string type, [FromForm] string title, [FromForm] string content, [FromForm] int maxSummaryLen, [FromForm] string API_KEY, [FromForm] string SECRET_KEY)
        {
            var vm = new ResultVM();
            try
            {
                var bytes = AipCheck(null, ref API_KEY, ref SECRET_KEY);
                var nlp = new Baidu.Aip.Nlp.Nlp(API_KEY, SECRET_KEY);

                if (string.IsNullOrWhiteSpace(content))
                {
                    vm.Set(RCodeTypes.failure);
                    vm.Msg = "content 不能为空";
                }
                else
                {
                    switch (type?.ToLower())
                    {
                        case "commenttag":
                            vm.Log.Add("评论观点抽取");
                            vm.Data = nlp.CommentTag(content);
                            vm.Set(RCodeTypes.success);
                            break;
                        case "sentimentclassify":
                            vm.Log.Add("情感倾向分析");
                            vm.Data = nlp.SentimentClassify(content);
                            vm.Set(RCodeTypes.success);
                            break;
                        case "keyword":
                            {
                                vm.Log.Add("文章标签");
                                if (string.IsNullOrWhiteSpace(title))
                                {
                                    vm.Set(RCodeTypes.failure);
                                    vm.Msg = "title 不能为空";
                                }
                                else
                                {
                                    vm.Data = nlp.Keyword(title, content);
                                    vm.Set(RCodeTypes.success);
                                }
                            }
                            break;
                        case "ecnet":
                            vm.Log.Add("文本纠错");
                            vm.Data = nlp.Ecnet(content);
                            vm.Set(RCodeTypes.success);
                            break;
                        case "newssummary":
                            {
                                vm.Log.Add("新闻摘要");
                                if (maxSummaryLen <= 0)
                                {
                                    vm.Log.Add("maxSummaryLen 请设置摘要长度");
                                    maxSummaryLen = 300;
                                }
                                vm.Data = nlp.NewsSummary(content, maxSummaryLen);
                            }
                            break;
                    }
                }

                if (vm.Code == 200)
                {
                    vm.Data = vm.Data.ToString().DeJson();
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        /// <summary>
        /// 生成占位图,默认200x200，SVG placeholder image generator
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult SVGPIG()
        {
            return SVGPIG("200x200");
        }

        /// <summary>
        /// 生成占位图,默认200x200，SVG placeholder image generator
        /// </summary>
        /// <param name="wh">自定义宽高，如 500x309</param>
        /// <returns></returns>
        [HttpGet, Route("{wh}")]
        [ResponseCache(Duration = 3600)]
        public IActionResult SVGPIG([FromRoute] string wh)
        {
            var w = 200;
            var h = 200;
            if (!string.IsNullOrWhiteSpace(wh))
            {
                try
                {
                    var sc = "x";
                    if (wh.Contains('*'))
                    {
                        sc = "*";
                    }
                    else if (wh.Contains('_'))
                    {
                        sc = "_";
                    }
                    var whs = wh.Split(sc);

                    w = Convert.ToInt32(whs.First());
                    h = Convert.ToInt32(whs.Last());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            var txt = $"{w}x{h}";

            var xml = $@"<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 {w} {h}' width='{w}' height='{h}'>
    <rect width='{w}' height='{h}' fill='#eee'></rect>
    <text x='50%' y='50%' dominant-baseline='middle' text-anchor='middle' font-family='monospace' font-size='26px' fill='#aaa'>{txt}</text>
</svg>";

            return File(Encoding.Default.GetBytes(xml), "image/svg+xml;charset=utf-8");
        }

        /// <summary>
        /// 生成二维码（带 logo）
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="icon">带 logo 图标</param>
        /// <param name="size">大小，默认 300 </param>
        /// <param name="fill">填充颜色</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult QRCode([FromForm] string text, IFormFile icon, [FromForm] int size = 300, [FromForm] string fill = "000000")
        {
            size = Math.Max(30, size);
            size = Math.Min(99999, size);

            var qr = new QRCodeGenerator().CreateQrCode(text, ECCLevel.H);
            var info = new SKImageInfo(size, size);
            var surface = SKSurface.Create(info);

            if (icon == null)
            {
                surface.Canvas.Render(qr, info.Width, info.Height, SKColor.Parse("FFFFFF"), SKColor.Parse(fill));
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
                surface.Canvas.Render(qr, info.Width, info.Height, SKColor.Parse("FFFFFF"), SKColor.Parse(fill), icond);
            }

            using var data = surface.Snapshot().Encode(SKEncodedImageFormat.Png, 100);
            using var ms = new MemoryStream();
            data.SaveTo(ms);
            var bytes = ms.ToArray();

            return File(bytes, "image/png");
        }

        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="size">大小，默认 300 </param>
        /// <param name="fill">填充颜色</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult QRCode(string text, int size = 300, string fill = "000000")
        {
            return QRCode(text, null, size, fill);
        }

        /// <summary>
        /// 代理
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="charset">编码，默认utf-8，如：gb2312</param>
        /// <returns></returns>
        [HttpGet]
        [HttpPost]
        [HttpPut]
        [HttpHead]
        [HttpPatch]
        [HttpDelete]
        public IActionResult Proxy(string url, string charset = "utf-8")
        {
            var outCode = 500;
            var outContent = string.Empty;
            var outContentType = "application/json; charset=utf-8";

            try
            {
                var ci = new ClientInfoTo(HttpContext);
                Console.WriteLine($"PROXY | {Request.Method} | {ci.IP} | {url} | {ci.Headers.UserAgent}");

                if (AppTo.GetValue<bool?>("DisableDatabaseWrite") != true)
                {
                    return BadRequest("Refuse");
                }

                var keywordBlacklist = new List<string> { ".m3u8", ".mpd", ".m4v" };
                if (string.IsNullOrWhiteSpace(url) || url.Length < 3)
                {
                    return Ok("Url Invalid");
                }
                else if (keywordBlacklist.Any(url.Contains))
                {
                    return BadRequest($"The keyword '{string.Join(" ", keywordBlacklist)}' was blacklisted by the operator of this proxy.");
                }
                else
                {
                    if (!url.Contains("://"))
                    {
                        url = "http://" + url;
                    }

                    var ct = Request.Headers["Content-Type"].ToString();

                    byte[] data = null;

                    if (Request.Method != "GET")
                    {
                        try
                        {
                            if (ct.StartsWith("application/json"))
                            {
                                using var ms = new MemoryStream();
                                Request.Body.CopyToAsync(ms).Wait();
                                data = ms.ToArray();
                            }
                            else if (ct.StartsWith("application/x-www-form-urlencoded") || ct.StartsWith("multipart/form-data"))
                            {
                                var sb = new StringBuilder();

                                foreach (var key in Request.Form.Keys)
                                {
                                    sb.Append($"&{key}={Request.Form[key].ToString().ToUrlEncode()}");
                                }

                                data = Encoding.GetEncoding(charset).GetBytes(sb.Remove(0, 1).ToString());
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }

                    var hwr = HttpTo.HWRequest(url, Request.Method, data);
                    //hwr.Proxy = new WebProxy("127.0.0.1", 1081);

                    var skipHeader = new List<string> { "Content-Length", "Content-Type", "Referer", "Cookie", "Host", "origin" };
                    foreach (var key in Request.Headers.Keys)
                    {
                        if (!skipHeader.Contains(key))
                        {
                            try
                            {
                                hwr.Headers[key] = Request.Headers[key].ToString();
                            }
                            catch (Exception)
                            {

                            }
                        }
                    }

                    var stream = HttpTo.Stream(hwr, out HttpWebResponse response, charset);
                    if (!string.IsNullOrEmpty(response.ContentType))
                    {
                        outContentType = response.ContentType;
                    }

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return File(stream.BaseStream, outContentType, Path.GetFileName(url));
                    }
                    else
                    {
                        outContent = stream.ReadToEnd();
                        outCode = (int)response.StatusCode;
                    }
                }
            }
            catch (WebException ex)
            {
                var response = (HttpWebResponse)ex.Response;
                if (response == null)
                {
                    outContent = ex.Message;
                }
                else
                {
                    outCode = (int)response.StatusCode;
                    outContent = response.Headers[HttpRequestHeader.ContentType];
                }
            }
            catch (Exception ex)
            {
                outContent = new
                {
                    code = -1,
                    msg = ex
                }.ToJson();
            }

            return new ContentResult
            {
                StatusCode = outCode,
                Content = outContent,
                ContentType = outContentType
            };
        }

        /// <summary>
        /// 死链检测
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> Link(string url)
        {
            try
            {
                var hc = new HttpClient();
                var resp = await hc.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

                var data = new JsonObject
                {
                    ["ok"] = resp.IsSuccessStatusCode,
                    ["status"] = (int)resp.StatusCode,
                    ["statusText"] = resp.ReasonPhrase,
                    ["url"] = url
                };

                var header = new JsonObject();
                resp.Headers.ForEach(h =>
                {
                    if (h.Value.Count() > 1)
                    {
                        var values = new JsonArray();
                        h.Value.ForEach(v => values.Add(v));
                        header[h.Key.ToLower()] = values;
                    }
                    else
                    {
                        header[h.Key.ToLower()] = h.Value.First();
                    }
                });
                data["header"] = header;

                return new ContentResult
                {
                    StatusCode = (int)resp.StatusCode,
                    Content = data.ToJson(),
                    ContentType = "application/json; charset=utf-8"
                };
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// 执行命令行
        /// </summary>
        /// <param name="arguments">参数（必填）</param>
        /// <param name="fileName">文件（可选，默认 cmd.exe 或 bash ）</param>
        /// <returns></returns>
        [HttpGet]
        [FilterConfigs.IsAdmin]
        public ResultVM CommandLine(string arguments, string fileName)
        {
            var vm = new ResultVM();
            try
            {
                if (AppTo.GetValue<bool?>("DisableDatabaseWrite") == true)
                {
                    var cr = CmdTo.Execute(arguments, fileName);
                    vm.Data = cr.CrOutput?.Split(Environment.NewLine);

                    vm.Log.AddRange(cr.CrError?.Split(Environment.NewLine));

                    vm.Set(RCodeTypes.success);
                }
                else
                {
                    vm.Set(RCodeTypes.refuse);
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

    }
}