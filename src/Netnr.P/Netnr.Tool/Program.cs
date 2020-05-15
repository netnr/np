using Netnr.Tool.Items;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Netnr.Tool
{
    class Program
    {
        static void Main()
        {
            Console.Title = MethodBase.GetCurrentMethod().DeclaringType.Namespace + "  v0.0.1";

            Console.WriteLine(Environment.NewLine);

            Menu();
        }

        public static void Menu()
        {
            var dic = new Dictionary<int, string>()
            {
                { 1,"Git Pull （拉取指定目录下的所有项目，检测 .git 文件夹是否存在）"},
                { 2,"OSInfoTo （获取系统信息）"},
                { 3,"Clear Project （清理项目 bin 、obj 目录）"}
            };
            foreach (var key in dic.Keys)
            {
                Console.WriteLine($"  {key}） " + dic[key]);
            }
            Console.WriteLine(Environment.NewLine);
            bool isnum = false;
            do
            {
                Console.Write("请选择功能，输入序号：");
                if (int.TryParse(Console.ReadLine(), out int num))
                {
                    isnum = true;
                }
                if (isnum)
                {
                    if (dic.ContainsKey(num))
                    {
                        Switch(num);
                    }
                    else
                    {
                        isnum = false;
                        Console.WriteLine("输入序号有误");
                    }
                }
            } while (!isnum);
        }

        public static void Switch(int num)
        {
            switch (num)
            {
                case 1:
                    GitPull.Run();
                    break;
                case 2:
                    Console.WriteLine(new OSInfoTo().ToJson());
                    break;
                case 3:
                    ClearProject.Run();
                    break;
                default:
                    Console.WriteLine("Invalid");
                    break;
            }
            Console.ReadKey();
        }

    }
}
