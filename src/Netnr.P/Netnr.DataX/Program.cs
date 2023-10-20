using System.Reflection;

namespace Netnr.DataX
{
    class Program
    {
        static async Task Main()
        {
            BaseTo.ReadyEncoding();
            BaseTo.ReadyLegacyTimestamp();

            //参数模式（静默）
            if (BaseTo.IsCmdArgs)
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
