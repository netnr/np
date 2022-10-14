using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Concurrent;

namespace Netnr.Admin.Web.Services
{
    /// <summary>
    /// 日志
    /// </summary>
    public class LoggingService
    {
        static readonly Stopwatch sw = new();

        /// <summary>
        /// 当前缓存日志
        /// </summary>
        public static ConcurrentQueue<BaseLog> CurrentCacheLog { get; set; } = new ConcurrentQueue<BaseLog>();

        /// <summary>
        /// 构建
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static BaseLog Build(ResultExecutedContext context)
        {
            var hc = context.HttpContext;
            var uinfo = IdentityService.Get(hc);

            //IP
            var ip = hc.Connection.RemoteIpAddress.ToString();
            if (hc.Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                ip = hc.Request.Headers["X-Forwarded-For"].ToString();
            }

            //日志
            var model = new BaseLog
            {
                LogId = SnowflakeTo.Id(),
                CreateTime = DateTime.Now,
                LogUser = uinfo?.Account,
                LogType = 1,
                LogAction = $"/{context.RouteData.Values["controller"]}/{context.RouteData.Values["action"]}".ToLower(),
                LogUrl = $"{hc.Request.Path}{hc.Request.QueryString.Value}",
                LogIp = ip,
                LogUserAgent = hc.Request.Headers.UserAgent.ToString()
            };
            if (model.LogUrl.Length > 4000)
            {
                model.LogUrl = model.LogUrl.Substring(0, 4000);
            }

            var itemName = "LogSql";
            if (hc.Items.TryGetValue(itemName, out object logSql))
            {
                model.LogSql = logSql.ToString();
                hc.Items.Remove(itemName);
            }

            itemName = "LogContent";
            if (hc.Items.TryGetValue(itemName, out object logContent))
            {
                model.LogContent = logContent.ToString();
                hc.Items.Remove(itemName);
            }

            return model;
        }

        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="model"></param>
        public static void Write(BaseLog model)
        {
            //开启写入日志
            if (AppTo.GetValue<bool>("Common:WriteLog"))
            {
                //添加到队列
                CurrentCacheLog.Enqueue(model);

                if (sw.IsRunning == false)
                {
                    sw.Restart();
                }

                //分批写入满足的条件：缓存的日志数量
                int cacheLogCount = 999;
                //分批写入满足的条件：缓存的时长
                int cacheLogTime = 99 * 1000;

                if (CurrentCacheLog.Count > cacheLogCount || sw.ElapsedMilliseconds > cacheLogTime)
                {
                    sw.Restart();

                    //异步写入日志
                    ThreadPool.QueueUserWorkItem(async _ =>
                    {
                        try
                        {
                            var listModel = new List<BaseLog>();
                            while (CurrentCacheLog.TryDequeue(out BaseLog deobj))
                            {
                                listModel.Add(deobj);
                            }

                            using var db = ContextBaseFactory.CreateDbContext();
                            await db.BaseLog.AddRangeAsync(listModel);
                            var num = await db.SaveChangesAsync();

                            ConsoleTo.Title($"写入日志 {num} 条");
                        }
                        catch (Exception ex)
                        {
                            ConsoleTo.Title("写入日志错误", ex.ToJson(true));
                        }
                    });
                }
            }
        }
    }
}
