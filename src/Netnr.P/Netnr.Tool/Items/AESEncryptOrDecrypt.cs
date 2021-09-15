namespace Netnr.Tool.Items
{
    public class AESEncryptOrDecrypt
    {
        public static void Run()
        {
            try
            {
                string ed = string.Empty;
                string content = string.Empty;
                string password = string.Empty;

                Console.Write($"请选择 [1/E]Encrypt 或 [2/D]Decrypt(默认 1)：");
                ed = Console.ReadLine();
                Console.Write($"请输入内容：");
                content = Console.ReadLine();
                Console.Write($"请输入密码：");
                password = Console.ReadLine();

                string result = string.Empty;
                result = (ed?.ToLower()) switch
                {
                    "2" or "e" => Core.CalcTo.AESDecrypt(content, password),
                    _ => Core.CalcTo.AESEncrypt(content, password),
                };
                Console.WriteLine(Environment.NewLine + result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR：{ex.Message}");
            }
        }
    }
}
