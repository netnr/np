namespace Netnr.DataX.Application
{
    public class ProjectCleanupService
    {
        public static void Run()
        {
            try
            {
                var dp = Environment.CurrentDirectory.TrimEnd('/').TrimEnd('\\');
                var rootPath = DXService.ConsoleReadPath("请输入目录：", 2, dp);

                DirectoryTree(rootPath);

                DXService.Log("Done");
            }
            catch (Exception ex)
            {
                DXService.Log($"ERROR：{ex.Message}");
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
