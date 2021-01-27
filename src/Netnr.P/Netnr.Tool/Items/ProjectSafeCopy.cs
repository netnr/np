using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Netnr.Tool.Items
{
    public class ProjectSafeCopy
    {
        public static void Run()
        {
            try
            {
                var sourcePath = string.Empty;
                var targetPath = string.Empty;

                do
                {
                    var dp = Path.Combine(Environment.CurrentDirectory, "npp");
                    Console.Write($"Enter the source directory (default {dp}):");
                    sourcePath = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(sourcePath))
                    {
                        sourcePath = dp;
                    }
                } while (!Directory.Exists(sourcePath));

                {
                    var dp = Path.Combine(Environment.CurrentDirectory, "np");
                    Console.Write($"Enter the target directory (default {dp}):");
                    targetPath = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(targetPath))
                    {
                        targetPath = dp;
                    }
                }

                var ignoreForder = "bin,obj,PublishProfiles,node_modules,packages,.git,.svg,.vs,.config,.vercel".Split(',').ToList();
                Console.WriteLine($"Folder ignored :{ignoreForder.ToJson()}");

                Core.FileTo.CopyDirectory(sourcePath, targetPath, ignoreForder);

                Console.WriteLine($"Copy completed{Environment.NewLine}");

                //需要处理的项目名称
                var listEp = "Netnr.Blog.Web".ToLower().Split(",").ToList();
                //需要处理的键名
                var listEk = "APPID,Key,Secret,ClientID,Token,AK,SK,Token,Password".ToLower().Split(',').ToList();

                var filesPath = Directory.GetFiles(targetPath, "appsettings.json", SearchOption.AllDirectories);
                for (int i = 0; i < filesPath.Length; i++)
                {
                    var filePath = filesPath[i];
                    if (!listEp.Any(x => filePath.ToLower().Contains(x)))
                    {
                        continue;
                    }

                    var txt = Core.FileTo.ReadText(filePath);
                    bool isFound = false;

                    string pattern = @"\""(\S+)\"": \""(\S+)\""";
                    var ntxt = Regex.Replace(txt, pattern, rr =>
                    {
                        if (listEk.Any(x => rr.Groups[1].Value.ToLower().Contains(x)))
                        {
                            if (!isFound)
                            {
                                Console.WriteLine($"{Environment.NewLine}Handle key:{filePath}");
                            }
                            isFound = true;

                            var key = rr.Groups[2].Value;
                            var hlen = Math.Min(10, Math.Max(1, Convert.ToInt32(Math.Floor(key.Length * 0.4))));
                            var rlen = Math.Min(20, key.Length - hlen);
                            var rstr = string.Empty;
                            while (rlen-- > 0)
                            {
                                rstr += "X";
                            }
                            var nkey = key[0..hlen] + rstr;
                            var nv = rr.Value.Replace(key, nkey);
                            Console.WriteLine(nv);

                            return nv;
                        }
                        else
                        {
                            return rr.Value;
                        }
                    }, RegexOptions.Singleline);

                    if (txt != ntxt)
                    {
                        Core.FileTo.WriteText(ntxt, filePath, false);
                    }
                }

                Console.WriteLine("Done");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
