using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.Linq;
using Netnr.Core;

namespace Netnr.Test.Controllers
{
    /// <summary>
    /// Netnr.Core
    /// </summary>
    [Route("[controller]/[action]")]
    public class CoreController : Controller
    {
        /// <summary>
        /// CmdTo
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public SharedResultVM CmdTo_1()
        {
            var vm = new SharedResultVM();

            var listCmd = new List<string>();
            if (CmdTo.IsWindows)
            {
                listCmd = new List<string>
                {
                    "dir",
                    "getmac",
                    "ver",
                    "PowerShell \"Get-Counter '\\Processor(_Total)\\% Processor Time'\"",
                    "wmic os get LastBootUpTime /value",
                    "wmic cpu get Name /value"
                };
            }
            else
            {
                listCmd = new List<string>
                {
                    "ls",
                    "ip addr",
                    "free -m",
                    "df -h",
                    "vmstat 1 2",
                    "cat /proc/uptime"
                };
            }

            listCmd.ForEach(cmd =>
            {
                vm.Log.Add($"\n$$$ {cmd}\n");
                var cr = CmdTo.Execute(cmd);
                vm.Log.Add(cr.CrOutput);
            });

            vm.Log.ForEach(x => Console.WriteLine(x));

            return vm;
        }

        /// <summary>
        /// CmdTo
        /// </summary>
        /// <param name="args">参数</param>
        /// <param name="fileName">应用程序</param>
        /// <returns></returns>
        [HttpGet]
        public SharedResultVM CmdTo_2(string args = @"-i E:\package\video\cspk.mp4", string fileName = "ffmpeg")
        {
            var vm = new SharedResultVM();

            CmdTo.Execute(args, fileName, (process, cr) =>
            {
                process.ErrorDataReceived += (sender, output) =>
                {
                    vm.Log.Add(output.Data);
                    Console.WriteLine(output.Data);
                };

                process.Start();//启动线程
                process.BeginErrorReadLine();//开始异步读取
                process.WaitForExit();//阻塞等待进程结束
                process.Close();//关闭进程
            });

            return vm;
        }

        /// <summary>
        /// 系统状态
        /// </summary>
        /// <param name="type">json 或 view</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult SystemStatusTo(string type = "json")
        {
            var ss = new SystemStatusTo();
            return Content(type == "json" ? ss.ToJson(true) : ss.ToView());
        }

    }
}
