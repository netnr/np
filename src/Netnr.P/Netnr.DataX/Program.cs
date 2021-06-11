using System;
using System.Reflection;
using Netnr.Core;

namespace Netnr.DataX
{
    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Title = MethodBase.GetCurrentMethod().DeclaringType.Namespace + "  v0.0.1";
            Console.CancelKeyPress += (s, e) => Environment.Exit(0);

            ConsoleTo.InvokeMenu(typeof(Application.MenuService));
        }
    }
}
