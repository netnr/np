using System.Reflection;

namespace Netnr.DataX
{
    class Program
    {
        static async Task Main()
        {
            BaseTo.ReadyEncoding();
            BaseTo.ReadyLegacyTimestamp();

            //git bash
            if (Console.Out.Encoding.CodePage == 65001)
            {
                Console.OutputEncoding = Encoding.UTF8;
            }
            else
            {
                Console.OutputEncoding = CmdTo.IsWindows ? Encoding.Unicode : Encoding.UTF8;
            }

            //参数模式（静默）
            if (BaseTo.IsWithArgs)
            {
                await DXService.RunOfSilence();
            }
            else
            {
                Console.Title = $"{ConfigInit.ShortName}({MethodBase.GetCurrentMethod().DeclaringType.Namespace}) v{BaseTo.Version}";
                await DXService.RunOfConsole();
            }
        }
    }
}
