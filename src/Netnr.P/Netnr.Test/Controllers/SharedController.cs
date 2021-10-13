using DocumentFormat.OpenXml.Packaging;
using Microsoft.Data.Sqlite;
using Netnr.Core;
using Netnr.SharedAdo;
using Netnr.SharedCompile;
using Netnr.SharedDataKit;
using System.Text.Json;

namespace Netnr.Test.Controllers
{
    /// <summary>
    /// Netnr.Shared
    /// </summary>
    [Route("[controller]/[action]")]
    public class SharedController : Controller
    {
        /// <summary>
        /// User-Agent
        /// </summary>
        /// <param name="ua">User-Agent</param>
        /// <param name="loop">循环</param>
        /// <returns></returns>
        [HttpGet]
        public SharedResultVM To_UserAgent(string ua = "", int loop = 1)
        {
            var vm = new SharedResultVM();

            if (string.IsNullOrWhiteSpace(ua))
            {
                ua = Request.HttpContext.Request.Headers["User-Agent"].ToString();
            }

            Parallel.For(0, loop, i =>
            {
                _ = new SharedUserAgent.UserAgentTo(ua);
            });

            var uainfo = new SharedUserAgent.UserAgentTo(ua);
            vm.Data = uainfo;

            return vm;
        }

        /// <summary>
        /// 动态编译并执行
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public SharedResultVM To_Compile()
        {
            return SharedResultVM.Try(vm =>
            {
                var code = @"
using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class DynamicCompile
{
    public void Main()
    {
        Console.WriteLine(DateTime.Now);
        Console.WriteLine(Environment.OSVersion);
        Console.WriteLine(Environment.SystemDirectory);
        Console.WriteLine(Environment.Version);
        Console.WriteLine(RuntimeInformation.FrameworkDescription);
        Console.WriteLine(RuntimeInformation.OSDescription);
    }
}
";
                vm.Msg = code;
                var ce = CompileTo.Executing(code, "System.Runtime.InteropServices.RuntimeInformation.dll".Split(",")).Split(Environment.NewLine);
                foreach (var item in ce)
                {
                    vm.Log.Add(item);
                }

                return vm;
            });
        }
    }
}
