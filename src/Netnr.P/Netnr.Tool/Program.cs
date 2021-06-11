using System;
using System.Reflection;
using System.ComponentModel;
using Netnr.Core;
using Netnr.Tool.Items;

namespace Netnr.Tool
{
    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Title = MethodBase.GetCurrentMethod().DeclaringType.Namespace + "  v0.0.1";
            Console.CancelKeyPress += (s, e) => Environment.Exit(0);

            ConsoleTo.InvokeMenu(typeof(MenuItems));
        }

        /// <summary>
        /// 菜单项
        /// </summary>
        public class MenuItems
        {
            [Description("退出")]
            public static void Mi_Exit()
            {
                Environment.Exit(0);
            }

            [Description("Git Pull （有 .git 文件夹）")]
            public static void Mi_GitPull()
            {
                GitPull.Run();
            }

            [Description("系统状态 （Json 或 View）")]
            public static void Mi_SystemStatus()
            {
                Console.Write($"请选择 [1/J]Json 或 [2/V]View（默认 1）：");
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

            [Description("项目清理 （bin 、obj）")]
            public static void Mi_ProjectCleanup()
            {
                ProjectCleanup.Run();
            }

            [Description("项目安全拷贝 （替换密钥）")]
            public static void Mi_ProjectSafeCopy()
            {
                ProjectSafeCopy.Run();
            }

            [Description("AES 加密解密 （数据库连接字符串）")]
            public static void Mi_AESEncryptOrDecrypt()
            {
                AESEncryptOrDecrypt.Run();
            }

            [Description("文本编码转换 （请先备份）")]
            public static void Mi_TextEncodingConversion()
            {
                TextEncodingConversion.Run();
            }
        }
    }
}
