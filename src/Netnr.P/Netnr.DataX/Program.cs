using System.Reflection;
using Netnr.Core;

namespace Netnr.DataX
{
    class Program
    {
        static void Main()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            Console.OutputEncoding = Encoding.UTF8;
            Console.Title = MethodBase.GetCurrentMethod().DeclaringType.Namespace + "  v0.0.1";
            Console.CancelKeyPress += (s, e) => Environment.Exit(0);

            ConsoleTo.InvokeMenu(typeof(Application.MenuService));
        }
    }
}
