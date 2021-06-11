using System;
using System.IO;

namespace Netnr.Tool.Items
{
    public class ProjectCleanup
    {
        public static void Run()
        {
            try
            {
                var rootPath = string.Empty;

                do
                {
                    var dp = Environment.CurrentDirectory.TrimEnd('/').TrimEnd('\\');
                    Console.Write($"请输入根目录（默认 {dp}）：");
                    rootPath = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(rootPath))
                    {
                        rootPath = dp;
                    }
                } while (!Directory.Exists(rootPath));

                DirectoryTree(rootPath);

                Console.WriteLine("完成");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
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
                        Console.WriteLine("删除：" + bin);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
                if (Directory.Exists(obj))
                {
                    try
                    {
                        Directory.Delete(obj, true);
                        Console.WriteLine("删除：" + obj);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
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
