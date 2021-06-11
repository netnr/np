using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Netnr.Tool.Items
{
    public class GitPull
    {
        public static void Run()
        {
            try
            {
                var rootPath = string.Empty;

                do
                {
                    var dp = Environment.CurrentDirectory.TrimEnd('/').TrimEnd('\\');
                    Console.Write($"请输入根目录(默认 {dp})：");
                    rootPath = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(rootPath))
                    {
                        rootPath = dp;
                    }
                } while (!Directory.Exists(rootPath));

                var dis = new DirectoryInfo(rootPath);
                var sdis = dis.GetDirectories().ToList();

                Console.WriteLine($"\n{sdis.Count} 个项目\n");

                int c1 = 0;
                int c2 = 0;
                Parallel.ForEach(sdis, sdi =>
                {
                    if (Directory.Exists(sdi.FullName + "/.git"))
                    {
                        var arg = $"git -C \"{sdi.FullName}\" pull --all";
                        var cr = Core.CmdTo.Execute(arg);
                        var rt = cr.CrOutput + cr.CrError;
                        Console.WriteLine($"【 {sdi.Name} 】\n{rt}");
                        c1++;
                    }
                    else
                    {
                        Console.WriteLine($"已跳过 \"{sdi.FullName}\" ，未找到 .git{Environment.NewLine}");
                        c2++;
                    }
                });

                Console.WriteLine($"完成! Pull：{c1} ，Skip：{c2}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR：{ex.Message}");
            }
        }
    }
}
