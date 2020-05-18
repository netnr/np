using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Netnr.ResponseFramework.Web.Filters
{
    /// <summary>
    /// 过滤器
    /// 
    /// 能 实现一个过滤器接口，要么是同步版本的，要么是异步版本的，鱼和熊掌不可兼得 。
    /// 如果你需要在接口中执行异步工作，那么就去实现异步接口。否则应该实现同步版本的接口。
    /// 框架会首先检查是不是实现了异步接口，如果实现了异步接口，那么将调用它。
    /// 不然则调用同步接口的方法。如果一个类中实现了两个接口，那么只有异步方法会被调用。
    /// 最后，不管 action 是同步的还是异步的，过滤器的同步或是异步是独立于 action 的
    /// </summary>
    public class FilterConfigs
    {
        /// <summary>
        /// 全局错误处理
        /// </summary>
        public class ErrorActionFilter : IExceptionFilter
        {
            public void OnException(ExceptionContext context)
            {
                Core.ConsoleTo.Log(context.Exception);
            }
        }

        private static Dictionary<string, string> _dicDescription;

        public static Dictionary<string, string> DicDescription
        {
            get
            {
                if (_dicDescription == null)
                {
                    var ass = System.Reflection.Assembly.GetExecutingAssembly();
                    var listController = ass.ExportedTypes.Where(x => x.BaseType.FullName == "Microsoft.AspNetCore.Mvc.Controller").ToList();

                    //载入xml注释
                    var cp = AppContext.BaseDirectory + ass.FullName.Split(',').FirstOrDefault() + ".xml";
                    XmlDocument xmldoc = new XmlDocument();
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
        /// 全局访问过滤器
        /// </summary>
        public class GlobalActionAttribute : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext context)
            {
                var hc = context.HttpContext;

                //日志记录，设置“__nolog”参数可忽略日志记录，为压力测试等环境考虑（即一些不需要记录请求日志的需求）

                if (GlobalTo.GetValue<bool>("logs:enable") && string.IsNullOrWhiteSpace(hc.Request.Query["__nolog"].ToString()))
                {
                    string controller = context.RouteData.Values["controller"].ToString().ToLower();
                    string action = context.RouteData.Values["action"].ToString().ToLower();
                    string url = hc.Request.Path.ToString() + hc.Request.QueryString.Value;

                    try
                    {
                        //客户端信息
                        var ct = new Fast.ClientTo(hc);

                        //用户信息
                        var userinfo = Application.CommonService.GetLoginUserInfo(hc);

                        //日志保存
                        var mo = new Domain.SysLog()
                        {
                            SuName = userinfo.UserName,
                            SuNickname = userinfo.Nickname,
                            LogAction = controller + "/" + action,
                            LogUrl = url,
                            LogIp = ct.IPv4,
                            LogUserAgent = ct.UserAgent,
                            LogCreateTime = DateTime.Now,
                            LogGroup = 1,
                            LogLevel = "I"
                        };

                        mo.LogContent = DicDescription[mo.LogAction.ToLower()];

                        #region 分批写入日志

                        //分批写入满足的条件：缓存的日志数量
                        int cacheLogCount = GlobalTo.GetValue<int>("logs:CacheWriteCount");
                        //分批写入满足的条件：缓存的时长，单位秒
                        int cacheLogTime = GlobalTo.GetValue<int>("logs:CacheWriteSecond");

                        //日志记录
                        var cacheLogsKey = "Global_Logs";
                        //上次写入的时间
                        var cacheLogWriteKey = "Global_Logs_Write";

                        if (!(Core.CacheTo.Get(cacheLogsKey) is List<Domain.SysLog> cacheLogs))
                        {
                            cacheLogs = new List<Domain.SysLog>();
                        }
                        cacheLogs.Add(mo);

                        var cacheLogWrite = Core.CacheTo.Get(cacheLogWriteKey) as DateTime?;
                        if (!cacheLogWrite.HasValue)
                        {
                            cacheLogWrite = DateTime.Now;
                        }

                        if (cacheLogs?.Count > cacheLogCount || DateTime.Now.ToTimestamp() - cacheLogWrite.Value.ToTimestamp() > cacheLogTime)
                        {
                            //异步写入日志
                            System.Threading.ThreadPool.QueueUserWorkItem(_ =>
                            {
                                try
                                {
                                    var ipto = new IPAreaTo();
                                    //写入日志前
                                    foreach (var log in cacheLogs)
                                    {
                                        log.LogId = Core.UniqueTo.LongId().ToString();
                                        //log.LogArea = ipto.Parse(log.LogIp);
                                        log.LogArea = ipto.Parse("23.99.110.186");

                                        var uato = new UserAgentTo(log.LogUserAgent);
                                        log.LogBrowserName = uato.BrowserName + " " + uato.BrowserVersion;
                                        log.LogSystemName = uato.SystemName + " " + uato.SystemVersion;
                                        if (uato.IsBot)
                                        {
                                            log.LogGroup = 2;
                                        }
                                    }

                                    using var db = new Data.ContextBase(Data.ContextBase.DCOB().Options);
                                    db.SysLog.AddRange(cacheLogs);
                                    db.SaveChanges();

                                    //清空数据及更新时间
                                    cacheLogs = null;
                                    cacheLogWrite = DateTime.Now;

                                    Core.CacheTo.Set(cacheLogsKey, cacheLogs);
                                    Core.CacheTo.Set(cacheLogWriteKey, cacheLogWrite);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("写入日志出错：" + ex.Message);
                                }
                            });
                        }
                        else
                        {
                            Core.CacheTo.Set(cacheLogsKey, cacheLogs);
                            Core.CacheTo.Set(cacheLogWriteKey, cacheLogWrite);
                        }

                        #endregion
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("写入日志出错：" + ex.Message);
                    }
                }

                base.OnActionExecuting(context);
            }
        }

        /// <summary>
        /// 允许跨域
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
        /// 是管理员
        /// </summary>
        public class IsAdmin : Attribute, IActionFilter
        {
            public void OnActionExecuted(ActionExecutedContext context)
            {

            }

            public void OnActionExecuting(ActionExecutingContext context)
            {
                var an = GlobalTo.GetValue("AdminName");

                bool isv;
                //cookie 授权已登录
                if (context.HttpContext.User.Identity.IsAuthenticated)
                {
                    var uinfo = Application.CommonService.GetLoginUserInfo(context.HttpContext);
                    isv = uinfo.UserName == an;
                }
                else
                {
                    var token = context.HttpContext.Request.Headers["Authorization"].ToString();
                    var mo = Application.CommonService.TokenValid(token);
                    isv = mo?.UserName == an;
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
    }
}