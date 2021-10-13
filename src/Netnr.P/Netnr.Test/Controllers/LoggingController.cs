using Netnr.SharedLogging;
using System.Collections.Concurrent;

namespace Netnr.Test.Controllers
{
    /// <summary>
    /// 日志
    /// </summary>
    [Route("[controller]/[action]")]
    public class LoggingController : Controller
    {
        public LoggingController()
        {
            LoggingTo.OptionsDbPart = LoggingTo.DBPartType.Year;
        }

        [HttpGet]
        public SharedResultVM Query()
        {
            var vm = new SharedResultVM();

            try
            {
                var now = DateTime.Now;
                vm.Data = LoggingTo.Query(now, now.AddYears(30));
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        [HttpGet]
        public SharedResultVM Add()
        {
            var vm = new SharedResultVM();

            var now = DateTime.Now;
            var ct = new SharedApp.ClientTo(HttpContext);

            var listLog = new ConcurrentBag<LoggingModel>();
            Parallel.For(0, 999, i =>
            {
                var lm = new LoggingModel()
                {
                    LogAction = "/",
                    LogCreateTime = now.AddHours(i),
                    LogContent = $"日志内容{i},{now}",
                    LogUserAgent = ct.UserAgent,
                    LogIp = ct.IPv4,
                    LogReferer = ct.Referer
                };
                listLog.Add(lm);
            });

            LoggingTo.Add(listLog);

            vm.Data = listLog.Count;

            return vm;
        }

        /// <summary>
        /// 删除存储目录
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public SharedResultVM DeleteStorageDir()
        {
            var vm = new SharedResultVM();

            try
            {
                vm.Log.Add(LoggingTo.OptionsDbRoot);
                System.IO.Directory.Delete(LoggingTo.OptionsDbRoot, true);

                vm.Set(SharedEnum.RTag.success);
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }
    }
}
