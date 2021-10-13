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
        public SharedResultVM ToCmd_1()
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

            foreach (var item in vm.Log)
            {
                Console.WriteLine(item);
            }            

            return vm;
        }

        /// <summary>
        /// CmdTo
        /// </summary>
        /// <param name="args">参数</param>
        /// <param name="fileName">应用程序</param>
        /// <returns></returns>
        [HttpGet]
        public SharedResultVM ToCmd_2(string args = @"-i E:\package\video\cspk.mp4", string fileName = "ffmpeg")
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
        public IActionResult ToSystemStatus(string type = "json")
        {
            var ss = new SystemStatusTo();
            return Content(type == "json" ? ss.ToJson(true) : ss.ToView());
        }

        /// <summary>
        /// 加密/解码
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        [HttpGet]
        public SharedResultVM ToCalc(string txt = "123 abc", string key = "321", string iv = "")
        {
            return SharedResultVM.Try(vm =>
            {
                vm.Log.Add($"encoding：{CalcTo.encoding}");
                vm.Log.Add($"txt：{txt}，Key：{key}，IV：{iv}");
                vm.Log.Add(string.Empty);
                vm.Log.Add($"AES Encrypt：{CalcTo.AESEncrypt(txt, key)}");
                vm.Log.Add($"AES Decrypt：{CalcTo.AESDecrypt(CalcTo.AESEncrypt(txt, key), key)}");
                vm.Log.Add(string.Empty);
                vm.Log.Add($"DES Encrypt：{CalcTo.DESEncrypt(txt, key)}");
                vm.Log.Add($"DES Decrypt：{CalcTo.DESDecrypt(CalcTo.DESEncrypt(txt, key), key)}");
                vm.Log.Add(string.Empty);
                vm.Log.Add($"MD5：{CalcTo.MD5(txt)}");
                vm.Log.Add(string.Empty);
                vm.Log.Add($"SHA_1：{CalcTo.SHA_1(txt)}");
                vm.Log.Add($"SHA_256：{CalcTo.SHA_256(txt)}");
                vm.Log.Add($"SHA_384：{CalcTo.SHA_384(txt)}");
                vm.Log.Add($"SHA_512：{CalcTo.SHA_512(txt)}");
                vm.Log.Add(string.Empty);
                vm.Log.Add($"HMAC_SHA1：{CalcTo.HMAC_SHA1(txt, key)}");
                vm.Log.Add($"HMAC_SHA256：{CalcTo.HMAC_SHA256(txt, key)}");
                vm.Log.Add($"HMAC_SHA384：{CalcTo.HMAC_SHA384(txt, key)}");
                vm.Log.Add($"HMAC_SHA512：{CalcTo.HMAC_SHA512(txt, key)}");
                vm.Log.Add($"HMAC_MD5：{CalcTo.HMAC_MD5(txt, key)}");

                return vm;
            });
        }

        /// <summary>
        /// 下载
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public SharedResultVM ToHttp_DownloadSave(string url = "https://img01.sogoucdn.com/app/a/100540022/2021053117531272442865.png", string path = @"D:\tmp\abc.png")
        {
            var vm = new SharedResultVM();

            HttpTo.DownloadSave(HttpTo.HWRequest(url), path);

            vm.Set(SharedEnum.RTag.success);

            return vm;
        }
    }
}
