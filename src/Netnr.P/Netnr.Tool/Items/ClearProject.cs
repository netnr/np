using System;
using System.IO;

namespace Netnr.Tool.Items
{
    public class ClearProject
    {
        public static void Run()
        {
            try
            {
                var rootPath = string.Empty;

                do
                {
                    var dp = Environment.CurrentDirectory.TrimEnd('/').TrimEnd('\\');
                    Console.Write("请输入根目录（默认 " + dp + "）：");
                    rootPath = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(rootPath))
                    {
                        rootPath = dp;
                    }
                } while (!Directory.Exists(rootPath));

                DirectoryTree(rootPath);

                Console.WriteLine("Complete");
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR：" + ex.Message);
            }
        }

        private static void DirectoryTree(string path)
        {
            string[] strFiles = Directory.GetFiles(path, "*.csproj");
            if (strFiles.Length > 0)
            {
                var bin = Path.Combine(path, "bin");
                var obj = Path.Combine(path, "obj");
                if (Directory.Exists(bin))
                {
                    try
                    {
                        Directory.Delete(bin, true);
                        Console.WriteLine("Delete：" + bin);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Delete Error：" + ex.Message);
                    }
                }
                if (Directory.Exists(obj))
                {
                    try
                    {
                        Directory.Delete(obj, true);
                        Console.WriteLine("Delete：" + obj);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Delete Error：" + ex.Message);
                    }
                }
            }
            else
            {
                foreach (string item in Directory.GetDirectories(path))
                {
                    DirectoryTree(item);
                }
            }
        }
    }
}
