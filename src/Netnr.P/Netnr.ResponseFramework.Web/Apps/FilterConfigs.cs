using System.Xml;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc.Filters;
using Netnr.SharedUserAgent;
using Netnr.SharedIpArea;
using Netnr.SharedFast;
using Netnr.SharedApp;

namespace Netnr.ResponseFramework.Web.Apps
{
    /// <summary>
    /// 过滤器
    /// </summary>
    public class FilterConfigs
    {
        private static Dictionary<string, string> _dicDescription;
        /// <summary>
        /// 提取 XML 注释为字典
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

                                var action = "/" + (conll.Name.Replace("Controller", "/") + item.Name).ToLower();
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
        /// 当前缓存日志
        /// </summary>
        public static ConcurrentQueue<Domain.SysLog> CurrentCacheLog { get; set; } = new ConcurrentQueue<Domain.SysLog>();

        /// <summary>
        /// 全局访问过滤器
        /// </summary>
        public class GlobalActionAttribute : ActionFilterAttribute
        {
            readonly Stopwatch swAsync = new();

            public override void OnActionExecuting(ActionExecutingContext context)
            {
                swAsync.Reset();
                swAsync.Start();

                var hc = context.HttpContext;
                string controller = context.RouteData.Values["controller"].ToString().ToLower();
                string action = context.RouteData.Values["action"].ToString().ToLower();
                var ca = "/" + controller + "/" + action;

                //用户信息
                var userinfo = LoginService.GetLoginUserInfo(hc);

                //角色有权限访问配置的菜单
                if (!Application.CommonService.QueryMenuIsAuth(userinfo.RoleId, ca))
                {
                    context.Result = new ContentResult()
                    {
                        Content = "unauthorized",
                        StatusCode = 401
                    };
                }

                base.OnActionExecuting(context);
            }

            public override void OnResultExecuted(ResultExecutedContext context)
            {
                var hc = context.HttpContext;
                string controller = context.RouteData.Values["controller"].ToString().ToLower();
                string action = context.RouteData.Values["action"].ToString().ToLower();
                var ca = "/" + controller + "/" + action;
                string url = hc.Request.Path.ToString() + hc.Request.QueryString.Value;

                //用户信息
                var userinfo = LoginService.GetLoginUserInfo(hc);

                //日志记录，设置“__nolog”参数可忽略日志记录，为压力测试等环境考虑（即一些不需要记录请求日志的需求）
                if (GlobalTo.GetValue<bool>("logs:enable") && string.IsNullOrWhiteSpace(hc.Request.Query["__nolog"].ToString()))
                {
                    try
                    {
                        swAsync.Stop();

                        //客户端信息
                        var ct = new ClientTo(hc);

                        //日志保存
                        var mo = new Domain.SysLog()
                        {
                            SuName = userinfo.UserName,
                            SuNickname = userinfo.Nickname,
                            LogAction = ca,
                            LogUrl = url,
                            LogIp = ct.IPv4,
                            LogUserAgent = ct.UserAgent,
                            LogCreateTime = DateTime.Now,
                            LogGroup = 1,
                            LogLevel = "I",
                            LogRemark = $"请求耗时：{swAsync.ElapsedMilliseconds}毫秒"
                        };

                        if (DicDescription.ContainsKey(ca))
                        {
                            mo.LogContent = DicDescription[ca];
                        }

                        //记录查询SQL
                        if ((context.Result as ObjectResult)?.Value is QueryDataOutputVM ovm)
                        {
                            mo.LogContent += Environment.NewLine + ovm.QuerySql;
                        }

                        #region 分批写入日志

                        CurrentCacheLog.Enqueue(mo);

                        //分批写入满足的条件：缓存的日志数量
                        int cacheLogCount = GlobalTo.GetValue<int>("logs:CacheWriteCount");
                        //分批写入满足的条件：缓存的时长，单位秒
                        int cacheLogTime = GlobalTo.GetValue<int>("logs:CacheWriteSecond");

                        //上次写入的时间
                        var cacheLogWriteKey = "Global_Logs_Write";
                        var cacheLogWrite = Core.CacheTo.Get(cacheLogWriteKey) as DateTime?;
                        if (!cacheLogWrite.HasValue)
                        {
                            Core.CacheTo.Set(cacheLogWriteKey, cacheLogWrite = DateTime.Now);
                        }

                        if (CurrentCacheLog.Count > cacheLogCount || DateTime.Now.ToTimestamp() - cacheLogWrite.Value.ToTimestamp() > cacheLogTime)
                        {
                            //异步写入日志
                            System.Threading.ThreadPool.QueueUserWorkItem(_ =>
                            {
                                try
                                {
                                    var ipto = new IpAreaTo();

                                    //写入日志前
                                    var listMo = new List<Domain.SysLog>();

                                    while (CurrentCacheLog.TryDequeue(out Domain.SysLog deobj))
                                    {
                                        deobj.LogId = Core.UniqueTo.LongId().ToString();
                                        deobj.LogArea = ipto.Parse(deobj.LogIp);

                                        var uato = new UserAgentTo(deobj.LogUserAgent);
                                        deobj.LogBrowserName = uato.BrowserName + " " + uato.BrowserVersion;
                                        deobj.LogSystemName = uato.SystemName + " " + uato.SystemVersion;
                                        if (uato.IsBot)
                                        {
                                            deobj.LogGroup = 2;
                                        }

                                        listMo.Add(deobj);
                                    }

                                    using var db = Data.ContextBaseFactory.CreateDbContext();
                                    db.SysLog.AddRange(listMo);
                                    db.SaveChanges();

                                    //更新时间
                                    Core.CacheTo.Set(cacheLogWriteKey, cacheLogWrite = DateTime.Now);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("写入日志出错：" + ex.Message);
                                }
                            });
                        }

                        #endregion
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("写入日志出错：" + ex.Message);
                    }
                }

                base.OnResultExecuted(context);
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
                    var uinfo = LoginService.GetLoginUserInfo(context.HttpContext);
                    isv = uinfo.UserName == an;
                }
                else
                {
                    var token = context.HttpContext.Request.Headers["Authorization"].ToString();
                    var mo = LoginService.TokenValid(token);
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