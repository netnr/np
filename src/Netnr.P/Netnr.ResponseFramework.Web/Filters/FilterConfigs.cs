namespace Netnr.ResponseFramework.Web.Filters
{
    /// <summary>
    /// 过滤器
    /// </summary>
    public class FilterConfigs
    {
        /// <summary>
        /// 全局访问过滤器
        /// </summary>
        public class GlobalActionAttribute : ActionFilterAttribute
        {
            readonly Stopwatch swAsync = new();

            public override void OnActionExecuting(ActionExecutingContext context)
            {
                swAsync.Restart();

                var hc = context.HttpContext;
                string controller = context.RouteData.Values["controller"].ToString().ToLower();
                string action = context.RouteData.Values["action"].ToString().ToLower();
                var ca = "/" + controller + "/" + action;

                var uinfo = IdentityService.Get(hc);

                //角色有权限访问配置的菜单
                if (uinfo != null && !CommonService.QueryMenuIsAuth(uinfo.RoleId, ca))
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
                var uinfo = IdentityService.Get(hc);

                //日志记录，设置“__nolog”参数可忽略日志记录，为压力测试等环境考虑（即一些不需要记录请求日志的需求）
                if (AppTo.GetValue<bool>("logs:enable") && string.IsNullOrWhiteSpace(hc.Request.Query["__nolog"].ToString()))
                {
                    try
                    {
                        swAsync.Stop();

                        //客户端信息
                        var ci = new ClientInfoTo(hc);

                        //日志保存
                        var mo = new SysLog()
                        {
                            SuName = uinfo?.UserName,
                            SuNickname = uinfo?.Nickname,
                            LogAction = ca,
                            LogUrl = url,
                            LogIp = ci.IP,
                            LogUserAgent = ci.Headers.UserAgent,
                            LogCreateTime = DateTime.Now,
                            LogGroup = 1,
                            LogLevel = "I",
                            LogRemark = $"请求耗时：{swAsync.ElapsedMilliseconds}毫秒"
                        };

                        if (LoggingService.DicDescription.TryGetValue(ca, out string value))
                        {
                            mo.LogContent = value;
                        }

                        //记录查询SQL
                        if (context.Result is ObjectResult { Value: QueryDataOutputVM ovm })
                        {
                            mo.LogContent += Environment.NewLine + ovm.QuerySql;
                        }

                        #region 分批写入日志

                        LoggingService.CurrentCacheLog.Enqueue(mo);

                        //分批写入满足的条件：缓存的日志数量
                        int cacheLogCount = AppTo.GetValue<int>("logs:CacheWriteCount");
                        //分批写入满足的条件：缓存的时长，单位秒
                        int cacheLogTime = AppTo.GetValue<int>("logs:CacheWriteSecond");

                        //上次写入的时间
                        var cacheLogWriteKey = "Logs-Write";
                        var cacheLogWrite = CacheTo.Get<DateTime?>(cacheLogWriteKey);
                        if (cacheLogWrite == null)
                        {
                            CacheTo.Set(cacheLogWriteKey, cacheLogWrite = DateTime.Now);
                        }

                        if (LoggingService.CurrentCacheLog.Count > cacheLogCount || DateTime.Now.ToTimestamp() - cacheLogWrite.Value.ToTimestamp() > cacheLogTime)
                        {
                            //异步写入日志
                            Task.Run(() =>
                            {
                                try
                                {
                                    //写入日志前
                                    var listMo = new List<SysLog>();

                                    while (LoggingService.CurrentCacheLog.TryDequeue(out SysLog deobj))
                                    {
                                        deobj.LogId = Guid.NewGuid().ToLongString();

                                        LoggingTo.UserAgentParser(deobj.LogUserAgent, out string browserName, out string systemName, out bool isBot);
                                        deobj.LogBrowserName = browserName;
                                        deobj.LogSystemName = systemName;
                                        if (isBot)
                                        {
                                            deobj.LogGroup = 2;
                                        }

                                        listMo.Add(deobj);
                                    }

                                    using var db = ContextBaseFactory.CreateDbContext();
                                    db.SysLog.AddRange(listMo);
                                    db.SaveChanges();

                                    //更新时间
                                    CacheTo.Set(cacheLogWriteKey, cacheLogWrite = DateTime.Now);
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
        [AttributeUsage(AttributeTargets.All)]
        public class IsAdmin : Attribute, IActionFilter
        {
            public void OnActionExecuted(ActionExecutedContext context)
            {

            }

            public void OnActionExecuting(ActionExecutingContext context)
            {
                if (!IdentityService.IsAdmin(context.HttpContext))
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