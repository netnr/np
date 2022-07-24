using System.Xml;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Filters;
using Netnr.Blog.Data;

namespace Netnr.Blog.Web.Apps
{
    /// <summary>
    /// 过滤器
    /// </summary>
    public class FilterConfigs
    {
        private static Dictionary<string, string> _dicDescription;

        /// <summary>
        /// 根据生成的注释文件XML获取Action的注释
        /// </summary>
        public static Dictionary<string, string> DicDescription
        {
            get
            {
                if (_dicDescription == null)
                {
                    var ass = System.Reflection.Assembly.GetExecutingAssembly();
                    var listController = ass.ExportedTypes.Where(x => x.BaseType?.FullName == "Microsoft.AspNetCore.Mvc.Controller").ToList();

                    //载入xml注释
                    var cp = AppContext.BaseDirectory + ass.FullName.Split(',').FirstOrDefault() + ".xml";
                    XmlDocument xmldoc = new();
                    xmldoc.Load(cp);
                    var xns = xmldoc.DocumentElement.SelectSingleNode("members").SelectNodes("member");
                    var listMember = new List<XmlNode>();
                    for (int i = 0; i < xns.Count; i++)
                    {
                        listMember.Add(xns[i]);
                    }

                    var dic = new Dictionary<string, string>();
                    foreach (var conll in listController)
                    {
                        var methods = conll.GetMethods();
                        foreach (var item in methods)
                        {
                            if (item.DeclaringType == conll)
                            {
                                string remark = "未备注说明";

                                //方法完整命名空间及名称
                                var cname = "M:" + conll.FullName + "." + item.Name;
                                //方法参数
                                var cparam = item.GetParameters();
                                if (cparam.Length > 0)
                                {
                                    var listParam = new List<string>();
                                    foreach (var par in cparam)
                                    {
                                        listParam.Add(par.ParameterType.FullName);
                                    }
                                    cname += "(" + string.Join(",", listParam) + ")";
                                }

                                var xnm = listMember.FirstOrDefault(x => x.Attributes["name"].Value.ToString() == cname);
                                if (xnm != null)
                                {
                                    remark = xnm.SelectSingleNode("summary").InnerText.ToString().Trim();
                                }

                                var action = (conll.Name.Replace("Controller", "/") + item.Name).ToLower();
                                if (!dic.ContainsKey(action))
                                {
                                    dic.Add(action, remark);
                                }
                            }
                        }
                    }
                    _dicDescription = dic;
                }

                return _dicDescription;
            }
            set
            {
                _dicDescription = value;
            }
        }

        /// <summary>
        /// 【过滤器】全局过滤器
        /// </summary>
        public class GlobalFilter : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext context)
            {
                //日志

                var hc = context.HttpContext;

                if (string.IsNullOrWhiteSpace(hc.Request.Query["__nolog"].ToString()))
                {
                    string controller = context.RouteData.Values["controller"].ToString().ToLower();
                    string action = context.RouteData.Values["action"].ToString().ToLower();

                    //日志保存
                    var mo = LogBuild(context.HttpContext);
                    mo.LogAction = controller + "/" + action;
                    if (DicDescription.ContainsKey(mo.LogAction))
                    {
                        mo.LogContent = DicDescription[mo.LogAction];
                    }

                    LoggingTo.Add(mo);
                }

                base.OnActionExecuting(context);
            }
        }

        /// <summary>
        /// 日志 构建实体
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static LoggingModel LogBuild(HttpContext context)
        {
            string reqPath = context.Request.Path.ToString();
            string reqQueryString = context.Request.QueryString.ToString();

            //客户端信息
            var ct = new ClientTo(context);

            //用户信息
            var userinfo = LoginService.Get(context);

            //日志保存
            var mo = new LoggingModel()
            {
                LogApp = GlobalTo.GetValue("Common:EnglishName"),
                LogUid = userinfo?.UserName,
                LogNickname = userinfo?.Nickname,
                LogAction = reqPath,
                LogUrl = reqPath + reqQueryString,
                LogIp = ct.IPv4,
                LogReferer = ct.Referer,
                LogCreateTime = DateTime.Now,
                LogUserAgent = ct.UserAgent,
                LogGroup = "1",
                LogLevel = "I"
            };

            var ddk = reqPath.ToLower().TrimStart('/');
            if (DicDescription.ContainsKey(ddk))
            {
                mo.LogContent = DicDescription[ddk];
            }

            return mo;
        }

        /// <summary>
        /// 日志 写入异常
        /// </summary>
        /// <param name="context"></param>
        /// <param name="exception"></param>
        public static void LogWrite(HttpContext context, Exception exception)
        {
            var mo = LogBuild(context);

            mo.LogLevel = "E";
            mo.LogGroup = "-1";
            mo.LogContent = exception.Message;

            LoggingTo.Add(mo);
        }

        /// <summary>
        /// 【过滤器】需要授权访问
        /// </summary>
        public class LoginSignValid : IAuthorizationFilter
        {
            public void OnAuthorization(AuthorizationFilterContext context)
            {
                //验证登录标记是最新，不是则注销登录（即同一用户不允许同时在线，按缓存时间生效）
                if (context.HttpContext.User.Identity.IsAuthenticated && GlobalTo.GetValue<bool>("Common:SingleSignOn"))
                {
                    var uinfo = LoginService.Get(context.HttpContext);

                    string ServerSign = HelpFuncTo.GetLogonSign(uinfo.UserId);
                    if (uinfo.UserSign != ServerSign)
                    {
                        context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    }
                }
            }
        }

        /// <summary>
        /// 【注解】允许跨域
        /// </summary>
        public class AllowCors : Attribute, IActionFilter
        {
            public void OnActionExecuted(ActionExecutedContext context)
            {

            }

            public void OnActionExecuting(ActionExecutingContext context)
            {
                var res = context.HttpContext.Response;

                var origin = context.HttpContext.Request.Headers["Origin"].ToString();

                var dicAk = new Dictionary<string, string>
                {
                    { "Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS" },
                    { "Access-Control-Allow-Headers", "Accept, Authorization, Cache-Control, Content-Type, DNT, If-Modified-Since, Keep-Alive, Origin, User-Agent, X-Requested-With, Token, x-access-token" }
                };

                if (string.IsNullOrWhiteSpace(origin))
                {
                    dicAk.Add("Access-Control-Allow-Origin", "*");
                }
                else
                {
                    dicAk.Add("Access-Control-Allow-Origin", origin);
                    dicAk.Add("Access-Control-Allow-Credentials", "true");
                }

                foreach (var ak in dicAk.Keys)
                {
                    res.Headers.Remove(ak);
                    res.Headers.Add(ak, dicAk[ak]);
                }

                if (context.HttpContext.Request.Method == "OPTIONS")
                {
                    context.Result = new OkResult();
                }
            }
        }

        /// <summary>
        /// 【注解】是管理员
        /// </summary>
        public class IsAdmin : Attribute, IActionFilter
        {
            public void OnActionExecuted(ActionExecutedContext context)
            {

            }

            public void OnActionExecuting(ActionExecutingContext context)
            {
                bool isv = false;

                if (context.HttpContext.User.Identity.IsAuthenticated)
                {
                    var uinfo = LoginService.Get(context.HttpContext);
                    isv = uinfo.UserId == GlobalTo.GetValue<int>("Common:AdminId");
                }

                if (!isv)
                {
                    context.Result = new ContentResult()
                    {
                        Content = "unauthorized",
                        StatusCode = 401
                    };
                }
            }
        }

        /// <summary>
        /// 【注解】完善信息
        /// </summary>
        public class IsCompleteInfo : Attribute, IActionFilter
        {
            public void OnActionExecuted(ActionExecutedContext context)
            {

            }

            public void OnActionExecuting(ActionExecutingContext context)
            {
                var vm = LoginService.CompleteInfoValid(context.HttpContext);
                if (vm.Code != 200)
                {
                    context.Result = new RedirectResult("/home/completeinfo");
                }
            }
        }

        /// <summary>
        /// 【注解】有效授权（Cookie、Token）
        /// </summary>
        public class IsValidAuth : Attribute, IActionFilter
        {
            public void OnActionExecuted(ActionExecutedContext context)
            {

            }

            public void OnActionExecuting(ActionExecutingContext context)
            {
                var uinfo = LoginService.Get(context.HttpContext);

                if (uinfo.UserId == 0)
                {
                    context.Result = new ContentResult()
                    {
                        Content = "unauthorized",
                        StatusCode = 401
                    };
                }
            }
        }

        #region 辅助方法

        public class HelpFuncTo
        {
            /// <summary>
            /// 获取最新登录标记，用于对比本地，踢出下线
            /// </summary>
            /// <param name="UserId">登录的UserId</param>
            /// <param name="Cache">优先取缓存</param>
            /// <returns></returns>
            public static string GetLogonSign(int UserId, bool Cache = true)
            {
                string result = string.Empty;
                var usk = "UserSign_" + UserId;
                var us = Core.CacheTo.Get(usk) as string;
                if (Cache && !string.IsNullOrEmpty(us))
                {
                    result = us;
                }
                else
                {
                    using var db = ContextBaseFactory.CreateDbContext();
                    var uiMo = db.UserInfo.Find(UserId);
                    if (uiMo != null)
                    {
                        result = uiMo.UserSign;
                        Core.CacheTo.Set(usk, result, 5 * 60, false);
                    }
                }
                return result;
            }
        }

        #endregion
    }
}
