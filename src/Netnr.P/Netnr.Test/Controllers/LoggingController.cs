using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Netnr.SharedLogging;

namespace Netnr.Test.Controllers
{
    public class LoggingController : Controller
    {
        public LoggingController()
        {
            LoggingTo.OptionsDbPart = LoggingTo.DBPartType.Month;
            LoggingTo.OptionsCacheWriteCount = 99;
        }

        public IActionResult Index()
        {
            var vm = new SharedResultVM();

            try
            {
                vm.Data = Query();
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return Content(vm.ToJson());
        }

        public void Add()
        {
            var now = DateTime.Now;

            var listLog = new List<LoggingModel>();
            for (int i = 0; i < 5000; i++)
            {
                var lm = new LoggingModel()
                {
                    LogAction = "/",
                    LogCreateTime = now.AddDays(i),
                    LogContent = $"日志内容{i},{now}"
                };
                listLog.Add(lm);
            }

            LoggingTo.Add(listLog);
        }

        public LoggingResultVM Query()
        {
            var now = DateTime.Now;

            var vm = LoggingTo.Query(now, now.AddYears(30));
            return vm;
        }
    }
}
