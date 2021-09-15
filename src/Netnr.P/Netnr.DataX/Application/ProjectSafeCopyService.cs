using System.Text.RegularExpressions;

namespace Netnr.DataX.Application
{
    public class ProjectSafeCopyService
    {
        public static void Run()
        {
            try
            {
                var sourceDp = Path.Combine(Environment.CurrentDirectory, "npp");
                var sourcePath = DXService.ConsoleReadPath("请输入源目录：", 2, sourceDp);

                var targetDp = Path.Combine(Environment.CurrentDirectory, "np");
                var targetPath = DXService.ConsoleReadPath("请输入目标目录：", 2, targetDp);

                var ignoreForder = "bin,obj,Properties,PublishProfiles,node_modules,packages,.git,.svg,.vs,.config,.vercel".Split(',').ToList();
                DXService.Log($"Ignored folders\n{ignoreForder.ToJson(true)}");

                Core.FileTo.CopyDirectory(sourcePath, targetPath, ignoreForder);

                DXService.Log($"Copy completed!\n");

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

                    var txt = Core.FileTo.ReadText(filePath)
                        .Replace("https://www.nentr.com", "https://www.nentr.eu.org")
                        .Replace("https://s1.nentr.com", "https://s1.nentr.eu.org");
                    bool isFound = false;

                    string pattern = @"\""(\S+)\"": \""(\S+)\""";
                    var ntxt = Regex.Replace(txt, pattern, rr =>
                    {
                        if (listEk.Any(x => rr.Groups[1].Value.Replace("_", "").ToLower().Contains(x)))
                        {
                            if (!isFound)
                            {
                                DXService.Log($"\n处理关键信息：{filePath}");
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
                            DXService.Log(nv);

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

                DXService.Log("\nDone!");
            }
            catch (Exception ex)
            {
                DXService.Log($"ERROR：{ex.Message}");
            }
        }
    }
}
