using System;
using Netnr.Tool.Items;
using System.Reflection;
using System.Collections.Generic;
using Netnr.Core;
using System.Text.RegularExpressions;
using System.Linq;

namespace Netnr.Tool
{
    class Program
    {
        static void Main()
        {
            Console.Title = MethodBase.GetCurrentMethod().DeclaringType.Namespace + "  v0.0.1";
            Console.WriteLine(Environment.NewLine);
            Console.CancelKeyPress += new ConsoleCancelEventHandler(CloseApp);

            Test1();

            Menu();
        }

        public static void Test1()
        {

        }

        public static void Menu()
        {
            var dic = new Dictionary<int, string>()
            {
                { 0,"Exit"},
                { 1,"Git Pull (All projects must exist in a .git folder)"},
                { 2,"SystemStatusTo (Json)"},
                { 3,"SystemStatusTo (View)"},
                { 4,"Project cleanup (bin and obj)"},
                { 5,"Project safe copy (Handle key and cleanup)"}
            };
            foreach (var key in dic.Keys)
            {
                Console.WriteLine($"  {key}） " + dic[key]);
            }
            Console.WriteLine(Environment.NewLine);


            bool isnum = false;
            do
            {
                Console.Write("Enter the serial number：");
                if (int.TryParse(Console.ReadLine(), out int num))
                {
                    isnum = true;
                }
                if (isnum)
                {
                    if (num == 0)
                    {
                        break;
                    }

                    if (dic.ContainsKey(num))
                    {
                        Switch(num);
                    }
                    else
                    {
                        isnum = false;
                        Console.WriteLine("Serial number error");
                    }
                }
            } while (!isnum);
        }

        public static void Switch(int num)
        {
            Console.WriteLine(Environment.NewLine);
            switch (num)
            {
                case 1:
                    GitPull.Run();
                    break;
                case 2:
                    Console.WriteLine(new SystemStatusTo().ToJson());
                    break;
                case 3:
                    Console.WriteLine(new SystemStatusTo().ToView());
                    break;
                case 4:
                    ProjectCleanup.Run();
                    break;
                case 5:
                    ProjectSafeCopy.Run();
                    break;
                default:
                    Console.WriteLine("Invalid");
                    break;
            }
            Console.WriteLine(Environment.NewLine);
            Menu();
        }

        public static void CloseApp(object sender, ConsoleCancelEventArgs args)
        {
            Environment.Exit(0);
        }
    }
}
