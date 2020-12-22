using System;

namespace Netnr.Tool.Items
{
    public class DESEncryptOrDecrypt
    {
        public static void Run()
        {
            try
            {
                string ed = string.Empty;
                string content = string.Empty;
                string password = string.Empty;

                Console.Write($"Please choose [1/E]Encrypt or [2/D]Decrypt(default 1):");
                ed = Console.ReadLine();
                Console.Write($"Please enter content:");
                content = Console.ReadLine();
                Console.Write($"Please enter password:");
                password = Console.ReadLine();

                string result = string.Empty;
                switch (ed?.ToLower())
                {
                    case "2":
                    case "e":
                        result = Core.CalcTo.DeDES(content, password);
                        break;
                    default:
                        result = Core.CalcTo.EnDES(content, password);
                        break;
                }

                Console.WriteLine(Environment.NewLine + result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR：{ex.Message}");
            }
        }
    }
}
