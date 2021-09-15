using System.Text;

namespace Netnr.Tool.Items
{
    public class TextEncodingConversion
    {
        public static void Run()
        {
            try
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                var rootPath = string.Empty;
                do
                {
                    var dp = Environment.CurrentDirectory.TrimEnd('/').TrimEnd('\\');
                    Console.Write($"请输入文件或文件夹路径（默认 {dp}）：");
                    rootPath = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(rootPath))
                    {
                        rootPath = dp;
                    }
                } while (!Directory.Exists(rootPath) && !File.Exists(rootPath));

                Encoding newec = new UTF8Encoding(false);
                var badec = false;
                do
                {
                    Console.Write($"设置新的编码（默认 {newec.BodyName})：");

                    try
                    {
                        var wec = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(wec))
                        {
                            newec = Encoding.GetEncoding(wec);
                        }
                        badec = false;
                    }
                    catch (Exception ex)
                    {
                        badec = true;
                        Console.WriteLine(ex);
                    }
                } while (badec);

                //file
                if (File.Exists(rootPath))
                {
                    ConvertEncoding(rootPath, newec);
                }
                else if (Directory.Exists(rootPath))
                {
                    var filterExtension = "*";
                    Console.Write($"设置文件格式，如：.md .js ，默认 {filterExtension}）：");
                    var nfe = Console.ReadLine();

                    var listFe = new List<string>();
                    if (!string.IsNullOrWhiteSpace(nfe) && nfe.Trim() != "*")
                    {
                        listFe = nfe.Split(' ').ToList().Select(x => x.Trim().ToLower()).ToList();
                    }

                    EachFolder(listFe, new DirectoryInfo(rootPath), newec);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR：{ex.Message}");
            }
        }

        public static void EachFolder(List<string> listFe, DirectoryInfo dir, Encoding newec)
        {
            foreach (FileInfo fi in dir.GetFiles())
            {
                if ((listFe.Count > 0 && listFe.Any(x => fi.Extension.ToLower() == x)) || listFe.Count == 0)
                {
                    ConvertEncoding(fi.FullName, newec);
                }
            }

            foreach (DirectoryInfo diSourceSubDir in dir.GetDirectories())
            {
                EachFolder(listFe, diSourceSubDir, newec);
            }
        }

        public static void ConvertEncoding(string path, Encoding newec)
        {
            var ff = GetFileEncoding(path);
            if (ff.BodyName != newec.BodyName)
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }
            Console.Write($"{ff.BodyName} => {newec.BodyName} , {path}");
            Console.ForegroundColor = ConsoleColor.White;

            var txt = File.ReadAllText(path, ff);
            File.WriteAllText(path, txt, newec);

            Console.Write($" Done {Environment.NewLine}");
        }

        public static Encoding GetFileEncoding(string path)
        {
            var contentWithUTF8 = File.ReadAllText(path, Encoding.UTF8);
            var contentWithGBK = File.ReadAllText(path, Encoding.GetEncoding("GBK"));
            if (contentWithUTF8.Length < contentWithGBK.Length)
                return Encoding.UTF8;
            else if (contentWithUTF8.Length == contentWithGBK.Length)
            {
                using (var reader = new StreamReader(path, true))
                {
                    reader.Peek();
                    return reader.CurrentEncoding;
                }
            }
            else
                return Encoding.GetEncoding("GBK");

        }
    }
}
