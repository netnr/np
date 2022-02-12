using System.Reflection;
using Netnr.Core;

namespace Netnr.DataX
{
    class Program
    {
        static void Main()
        {
            SharedReady.ReadyTo.EncodingReg();
            SharedReady.ReadyTo.LegacyTimestamp();

            Console.OutputEncoding = CmdTo.IsWindows ? Encoding.Unicode : Encoding.UTF8;

            var ci = new Domain.ConfigInit();
            var co = ci.ConfigObj;
            var cs = ci.Silence;

            if (co.Console_Encoding == "UTF-8")
            {
                Console.OutputEncoding = Encoding.UTF8;
            }
            else if (co.Console_Encoding == "Unicode")
            {
                Console.OutputEncoding = Encoding.Unicode;
            }

            //参数模式（静默）
            var args = Environment.GetCommandLineArgs().ToList();
            var tasks = args.Where(x => x.StartsWith("Task_")).ToList();
            if (tasks.Count > 0)
            {
                Application.SilenceService.Run(tasks);
            }
            else
            {
                Console.Title = $"{MethodBase.GetCurrentMethod().DeclaringType.Namespace}  v{co.Version}";

                //Ctrl C Exit  
                Console.CancelKeyPress += (s, e) => Environment.Exit(1);

                ConsoleTo.InvokeMenu(typeof(Application.MenuService));
            }
        }
    }
}
