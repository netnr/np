using System.Collections.Concurrent;
using System.Net.Http;

namespace Netnr.Blog.Web.Controllers
{
    /// <summary>
    /// 脚本服务 ScriptService
    /// </summary>
    public class SSController : Controller
    {
        /// <summary>
        /// 首页
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 在线壁纸
        /// </summary>
        /// <returns></returns>
        public IActionResult WallPaper()
        {
            return View();
        }

        /// <summary>
        /// 彩票查询
        /// </summary>
        /// <returns></returns>
        public IActionResult Lottery()
        {
            return View();
        }

        /// <summary>
        /// 备案信息查询
        /// </summary>
        /// <returns></returns>
        public IActionResult ICP()
        {
            return View();
        }

        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <returns></returns>
        public IActionResult QrCode()
        {
            return View();
        }

        /// <summary>
        /// 身份证号码查询
        /// </summary>
        /// <returns></returns>
        public IActionResult IdCard()
        {
            return View();
        }

        /// <summary>
        /// Nginx配置文件格式化
        /// </summary>
        /// <returns></returns>
        public IActionResult Nginx()
        {
            return View();
        }

        /// <summary>
        /// 加密转码
        /// </summary>
        /// <returns></returns>
        public IActionResult Code()
        {
            return View();
        }

        /// <summary>
        /// JSON转
        /// </summary>
        /// <returns></returns>
        public IActionResult JsonTo()
        {
            return View();
        }

        /// <summary>
        /// vscode
        /// </summary>
        /// <returns></returns>
        public IActionResult VSCode()
        {
            return View();
        }

        /// <summary>
        /// 文本对比
        /// </summary>
        /// <returns></returns>
        public IActionResult Diff()
        {
            return View();
        }

        /// <summary>
        /// 快递查询
        /// </summary>
        /// <returns></returns>
        public IActionResult Express()
        {
            return View();
        }

        /// <summary>
        /// IP
        /// </summary>
        /// <returns></returns>
        public IActionResult IP()
        {
            return View();
        }

        /// <summary>
        /// 特殊符号
        /// </summary>
        /// <returns></returns>
        public IActionResult Symbol()
        {
            return View();
        }

        /// <summary>
        /// Emoji
        /// </summary>
        /// <returns></returns>
        public IActionResult Emoji()
        {
            return View();
        }

        /// <summary>
        /// Base64
        /// </summary>
        /// <returns></returns>
        public IActionResult Base64()
        {
            return View();
        }

        /// <summary>
        /// 留言
        /// </summary>
        /// <returns></returns>
        public IActionResult Message()
        {
            return View();
        }

        /// <summary>
        /// 行政区划
        /// </summary>
        /// <returns></returns>
        public IActionResult Zoning()
        {
            return View();
        }

        /// <summary>
        /// 网络测速
        /// </summary>
        /// <returns></returns>
        public IActionResult SpeedTest()
        {
            return View();
        }

        /// <summary>
        /// UserAgent
        /// </summary>
        /// <returns></returns>
        public IActionResult UserAgent()
        {
            return View();
        }

        /// <summary>
        /// RMB
        /// </summary>
        /// <returns></returns>
        public IActionResult RMB()
        {
            return View();
        }

        /// <summary>
        /// Ping云服务
        /// </summary>
        /// <returns></returns>
        public IActionResult PingCloud()
        {
            return View();
        }

        /// <summary>
        /// 数据字典
        /// </summary>
        /// <returns></returns>
        public IActionResult DataDict()
        {
            return View();
        }

        /// <summary>
        /// 免费
        /// </summary>
        /// <returns></returns>
        public IActionResult Free()
        {
            return View();
        }

        /// <summary>
        /// 在线电视
        /// </summary>
        /// <returns></returns>
        public IActionResult TV()
        {
            return View();
        }

        /// <summary>
        /// 体重指标
        /// </summary>
        /// <returns></returns>
        public IActionResult BMI()
        {
            return View();
        }

        /// <summary>
        /// 新华字典
        /// </summary>
        /// <returns></returns>
        public IActionResult Zidian()
        {
            return View();
        }

        /// <summary>
        /// 聊天
        /// </summary>
        /// <returns></returns>
        public IActionResult Chat()
        {
            return View();
        }

        /// <summary>
        /// ZeroTier Web Manager
        /// </summary>
        /// <returns></returns>
        public IActionResult ZeroTier()
        {
            return View();
        }

        /// <summary>
        /// 生成密钥
        /// </summary>
        /// <returns></returns>
        public IActionResult RandomCode()
        {
            return View();
        }

        /// <summary>
        /// 随机匹配
        /// </summary>
        /// <returns></returns>
        public IActionResult RandomMatch()
        {
            return View();
        }

        /// <summary>
        /// PS
        /// </summary>
        /// <returns></returns>
        public IActionResult PS()
        {
            return View();
        }

        /// <summary>
        /// PDF
        /// </summary>
        /// <returns></returns>
        public IActionResult PDF()
        {
            return View();
        }

        /// <summary>
        /// 查看Office文档
        /// </summary>
        /// <returns></returns>
        public IActionResult OfficeView()
        {
            return View();
        }

        /// <summary>
        /// 画笔
        /// </summary>
        /// <returns></returns>
        public IActionResult Brush()
        {
            return View();
        }

        /// <summary>
        /// OCR 识别图片内容
        /// </summary>
        /// <returns></returns>
        public IActionResult OCR()
        {
            return View();
        }

        /// <summary>
        /// NSFW 图片审查
        /// </summary>
        /// <returns></returns>
        public IActionResult NSFW()
        {
            return View();
        }

        /// <summary>
        /// 自然语言处理
        /// </summary>
        /// <returns></returns>
        public IActionResult NLP()
        {
            return View();
        }

        /// <summary>
        /// JS、CSS
        /// </summary>
        /// <returns></returns>
        public IActionResult JsCss()
        {
            return View();
        }

        /// <summary>
        /// 格式化
        /// </summary>
        /// <returns></returns>
        public IActionResult Formatter()
        {
            return View();
        }

        /// <summary>
        /// PowerDesigner
        /// </summary>
        /// <returns></returns>
        public IActionResult PDM()
        {
            return View();
        }

        /// <summary>
        /// Swagger
        /// </summary>
        /// <returns></returns>
        public IActionResult SwaggerTo()
        {
            return View();
        }

        /// <summary>
        /// SVG优化
        /// </summary>
        /// <returns></returns>
        public IActionResult Svgo()
        {
            return View();
        }

        /// <summary>
        /// 身份图标
        /// </summary>
        /// <returns></returns>
        public IActionResult Identicon()
        {
            return View();
        }

        /// <summary>
        /// Git 存储
        /// </summary>
        /// <returns></returns>
        public IActionResult GitStorage()
        {
            return View();
        }

        /// <summary>
        /// 流程图
        /// </summary>
        /// <returns></returns>
        public IActionResult BPMN()
        {
            return View();
        }

        /// <summary>
        /// 流程图
        /// </summary>
        /// <returns></returns>
        public IActionResult Graph()
        {
            return View();
        }

        /// <summary>
        /// 脑图
        /// </summary>
        /// <returns></returns>
        public IActionResult Mind()
        {
            return View();
        }

        #region 生成脚本服务

        /// <summary>
        /// 【管理员】构建静态文件
        /// </summary>
        /// <returns></returns>
        public async Task<ResultVM> Build()
        {
            var vm = new ResultVM();

            //是管理员 或 带参数
            if (IdentityService.IsAdmin(HttpContext) || Environment.GetCommandLineArgs().Contains("--admin"))
            {
                vm = await BuildHtml<SSController>();
            }
            else
            {
                vm.Set(EnumTo.RTag.unauthorized);
            }

            return vm;
        }

        /// <summary>
        /// 根据控制器构建静态页面
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private async Task<ResultVM> BuildHtml<T>() where T : Controller
        {
            var vm = new ResultVM();

            try
            {
                ConsoleTo.Title("Build HTML");

                AppContext.SetSwitch("Netnr.BuildHtml", true);

                //反射 action
                var type = typeof(T);
                var rtype = typeof(IActionResult);
                var methods = type.GetMethods().Where(x => x.DeclaringType == type && x.ReturnType == rtype).ToList();
                var ctrlName = type.Name.Replace("Controller", "").ToLower();

                //访问前缀
                var urlPrefix = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/{ctrlName}/";

                var cbs = new ConcurrentBag<string>();
                var hc = new HttpClient();
                //请求
                await Parallel.ForEachAsync(methods, async (mh, token) =>
                {
                    Console.WriteLine(mh.Name);

                    cbs.Add(mh.Name);

                    var html = await hc.GetStringAsync(urlPrefix + mh.Name, token);
                    var savePath = $"{AppTo.WebRootPath}/{mh.Name.ToLower()}.html";

                    await System.IO.File.WriteAllTextAsync(savePath, html, token);
                });
                vm.Log.AddRange(cbs);
                Console.WriteLine("\r\nDone!");

                vm.Set(EnumTo.RTag.success);
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }
            finally
            {
                AppContext.SetSwitch("Netnr.BuildHtml", false);
            }

            return vm;
        }

        #endregion
    }
}
