using System.Reflection;

namespace Netnr.DataX
{
    class Program
    {
        static void Main()
        {
            ReadyTo.EncodingReg();
            ReadyTo.LegacyTimestamp();

            //git bash
            if (Console.Out.Encoding.CodePage == 65001)
            {
                Console.OutputEncoding = Encoding.UTF8;
            }
            else
            {
                Console.OutputEncoding = GlobalTo.IsWindows ? Encoding.Unicode : Encoding.UTF8;
            }

            //参数模式（静默）
            if (GlobalTo.IsStartWithArgs)
            {
                MenuSilenceService.Run();
            }
            else
            {
                Console.Title = $"{ConfigInit.ShortName}({MethodBase.GetCurrentMethod().DeclaringType.Namespace}) v{ConfigInit.Version}";
                DXService.InvokeMenu(typeof(MenuMainService));
            }
        }
    }
}
