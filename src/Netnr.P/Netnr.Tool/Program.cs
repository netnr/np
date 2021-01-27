using System;
using Netnr.Tool.Items;
using System.Reflection;
using Netnr.Core;
using System.Linq;
using System.ComponentModel;

namespace Netnr.Tool
{
    class Program
    {
        static void Main()
        {
            Console.Title = MethodBase.GetCurrentMethod().DeclaringType.Namespace + "  v0.0.1";
            Console.CancelKeyPress += new ConsoleCancelEventHandler(CloseApp);

            MenuStart();
        }

        public static void CloseApp(object sender, ConsoleCancelEventArgs args)
        {
            Environment.Exit(0);
        }

        /// <summary>
        /// 启动菜单
        /// </summary>
        public static void MenuStart()
        {
            var mitype = typeof(MenuItems);
            var mis = mitype.GetMethods();
            var mlen = mis.Length - 4;
            Console.WriteLine(Environment.NewLine + "  ======================= Netnr.Tool =======================");
            for (int i = 0; i < mlen; i++)
            {
                var mi = mis[i];
                var desc = mi.CustomAttributes.First().ConstructorArguments.First().Value;
                Console.WriteLine($"  {i}） {desc}");
            }
            Console.WriteLine(Environment.NewLine);

            bool isMenumNum;
            do
            {
                Console.Write("Enter the serial number：");
                isMenumNum = int.TryParse(Console.ReadLine(), out int num) && num >= 0 && num < mlen;
                if (isMenumNum)
                {
                    mis[num].Invoke(mitype, null);

                    MenuStart();
                }
            } while (!isMenumNum);
        }

        /// <summary>
        /// 菜单项
        /// </summary>
        public class MenuItems
        {
            [Description("Exit")]
            public static void Mi_Exit()
            {
                Environment.Exit(0);
            }

            [Description("Git Pull (All projects must exist in a .git folder)")]
            public static void Mi_GitPull()
            {
                GitPull.Run();
            }

            [Description("System Status (Json or View)")]
            public static void Mi_SystemStatus()
            {
                Console.Write($"Please choose [1/J]Json or [2/V]View(default 1):");
                var format = Console.ReadLine()?.ToLower();
                var ss = new SystemStatusTo();
                Console.WriteLine(Environment.NewLine);
                if (format == "2" || format == "v")
                {
                    Console.WriteLine(ss.ToView());
                }
                else
                {
                    Console.WriteLine(ss.ToJson());
                }
            }

            [Description("Project cleanup (bin and obj)")]
            public static void Mi_ProjectCleanup()
            {
                ProjectCleanup.Run();
            }

            [Description("Project safe copy (Handle key and cleanup)")]
            public static void Mi_ProjectSafeCopy()
            {
                ProjectSafeCopy.Run();
            }

            [Description("DES encrypt or decode (Database connection string)")]
            public static void Mi_DESEncryptOrDecrypt()
            {
                DESEncryptOrDecrypt.Run();
            }

            [Description("Text encoding conversion (Please backup first)")]
            public static void Mi_TextEncodingConversion()
            {
                TextEncodingConversion.Run();
            }
        }
    }
}
