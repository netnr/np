using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Netnr.AliyunSMS.Controllers
{
    /// <summary>
    /// 发送短信
    /// </summary>
    [Route("[controller]/[action]")]
    [Filters.FilterConfigs.AllowCors]
    public class SmsController : Controller
    {
        /// <summary>
        /// 批量发送短信
        /// </summary>
        /// <param name="dicAddQuery">参数项</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResultVM SendBatchSms(Dictionary<string, string> dicAddQuery)
        {
            var vm = Application.SmsService.SendBatchSms(dicAddQuery);
            return vm;
        }
    }
}