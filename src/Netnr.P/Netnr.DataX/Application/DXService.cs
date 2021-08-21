using Netnr.Core;
using Netnr.DataX.Domain;

namespace Netnr.DataX.Application
{
    /// <summary>
    /// 辅助类
    /// </summary>
    public partial class DXService
    {
        /// <summary>
        /// 配置列表
        /// </summary>
        /// <returns></returns>
        public static List<string> GetConfigLists()
        {
            var list = new List<string>();

            var co = new ConfigObj();
            var cfgFiles = Directory.GetFiles(co.DXPath, "config*.json");

            foreach (var path in cfgFiles)
            {
                list.Add(Path.GetFileName(path));
            }

            return list;
        }

        /// <summary>
        /// 日志
        /// </summary>
        /// <param name="msg"></param>
        public static void Log(string msg)
        {
            Console.WriteLine(msg);
            ConsoleTo.Log(msg);
        }

        /// <summary>
        /// 模糊匹配
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public static bool FuzzyMatch(string s1, string s2)
        {
            s1 = s1.Replace("-", "").Replace("_", "").Replace(" ", "").ToLower();
            s2 = s2.Replace("-", "").Replace("_", "").Replace(" ", "").ToLower();
            return s1 == s2;
        }

        /// <summary>
        /// 输入数据库
        /// </summary>
        /// <param name="co"></param>
        public static DbConnObj ConsoleReadDatabase(ConfigObj co, string tip = "请选择库：")
        {
            var mo = new DbConnObj();

        Flag1:
            Console.WriteLine($"\n0. 输入数据库连接信息");
            for (int i = 0; i < co.DbConns.Count; i++)
            {
                var obj = co.DbConns[i];
                Console.WriteLine($"{i + 1}. {obj.Remark} => {obj.TDB} => {obj.Conn}");
            }
            Console.Write(tip);

            _ = int.TryParse(Console.ReadLine().Trim(), out int ed);

            if (ed == 0)
            {
            Flag2:
                Console.Write($"数据库连接信息（MySQL => Conn）：");
                var tc = Console.ReadLine().Trim().Split(" => ");
                if (Enum.TryParse(tc[0], true, out SharedEnum.TypeDB tdb))
                {
                    mo.TDB = tdb;
                    mo.Conn = tc[1];
                    if (mo.Conn.Length < 10)
                    {
                        Log("连接字符串无效");
                        goto Flag2;
                    }
                }
                else
                {
                    Log("无效数据库类型");
                    goto Flag2;
                }
            }
            else if (ed > 0 && ed <= co.DbConns.Count)
            {
                mo = co.DbConns[ed - 1];
            }
            else
            {
                Log($"无效 {ed}");
                goto Flag1;
            }

            Log($"已选择 {mo.TDB} => {mo.Conn}");

            return mo;
        }

        /// <summary>
        /// 输入文件（夹）
        /// </summary>
        /// <param name="tip">提示文字</param>
        /// <param name="type">默认（0：都可以；1：文件；2：文件夹）</param>
        /// <param name="dv">默认文件（夹）</param>
        public static string ConsoleReadPath(string tip, int type = 1, string dv = null)
        {
        Flag1:
            var dtip = "\n";
            if (!string.IsNullOrWhiteSpace(dv))
            {
                dtip = $"{tip.TrimEnd('：').TrimEnd(':')}（默认 {dv}）：";
            }
            else
            {
                dtip += tip;
            }

            Console.Write(dtip);
            var path = Console.ReadLine().Trim();
            if (!string.IsNullOrWhiteSpace(dv) && string.IsNullOrWhiteSpace(path))
            {
                path = dv;
            }
            else if ((type == 1 && !File.Exists(path)) || (type == 2 && !Directory.Exists(path)) || (type == 0 && !File.Exists(path) && !Directory.Exists(path)))
            {
                Log($"{path} 无效文件（夹）");
                goto Flag1;
            }

            return path;
        }

        /// <summary>
        /// 输入选择项
        /// </summary>
        /// <param name="tip">提示文字</param>
        /// <param name="items">项</param>
        /// <param name="dv">默认（从 1 开始）</param>
        public static int ConsoleReadItem(string tip, IList<string> items, int dv = 1)
        {
        Flag1:
            Console.WriteLine("");
            for (int j = 0; j < items.Count; j++)
            {
                Console.WriteLine($"{j + 1}. {items[j]}");
            }
            Console.Write($"{tip}（默认：{dv}. {items[dv - 1]}）：");

            var ii = Console.ReadLine().Trim();
            if (string.IsNullOrWhiteSpace(ii))
            {
                Log($"已选择 {dv}. {items[dv - 1]}");
                return dv;
            }
            else
            {
                _ = int.TryParse(ii, out int i);

                if (i > 0 && i <= items.Count)
                {
                    Log($"已选择 {i}. {items[i - 1]}");
                    return i;
                }
                else
                {
                    Log($"{ii} 无效");
                    goto Flag1;
                }
            }
        }

        /// <summary>
        /// 新建文件名
        /// </summary>
        /// <param name="prefix">前缀</param>
        /// <param name="ext">后缀</param>
        /// <returns></returns>
        public static string NewFileName(string prefix, string ext)
        {
            return $"{prefix}_{DateTime.Now:yyyyMMdd_HHmmss}{ext}";
        }
    }
}
