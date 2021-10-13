namespace Netnr.DataX.Application
{
    public class TextEncodingConversionService
    {
        public static void Run()
        {
            try
            {
                var dp = Environment.CurrentDirectory.TrimEnd('/').TrimEnd('\\');
                var rootPath = DXService.ConsoleReadPath("请输入文件或文件夹：", 0, dp);

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
                        DXService.Log($"ERROR：{ex.Message}");
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

        /// <summary>
        /// 编码转换
        /// </summary>
        /// <param name="path"></param>
        /// <param name="newec"></param>
        public static void ConvertEncoding(string path, Encoding newec)
        {
            var ff = GetFileEncoding(path);
            if (ff.BodyName != newec.BodyName)
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }
            DXService.Log($"{ff.BodyName} => {newec.BodyName} , {path}");
            Console.ForegroundColor = ConsoleColor.White;

            var txt = File.ReadAllText(path, ff);
            File.WriteAllText(path, txt, newec);

            DXService.Log($"Done!\n");
        }

        /// <summary>
        /// 获取文件内容编码
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Encoding GetFileEncoding(string path)
        {
            var contentWithUTF8 = File.ReadAllText(path, Encoding.UTF8);
            var contentWithGBK = File.ReadAllText(path, Encoding.GetEncoding("GBK"));
            if (contentWithUTF8.Length < contentWithGBK.Length)
            {
                return Encoding.UTF8;
            }
            else if (contentWithUTF8.Length == contentWithGBK.Length)
            {
                using var reader = new StreamReader(path, true);
                reader.Peek();
                return reader.CurrentEncoding;
            }
            else
            {
                return Encoding.GetEncoding("GBK");
            }
        }
    }
}
