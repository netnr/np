using Netnr.Core;
using Netnr.DataX.Domain;
using Netnr.SharedDataKit;
using System.Text.RegularExpressions;

namespace Netnr.DataX.Application
{
    /// <summary>
    /// 辅助类
    /// </summary>
    public partial class DXService
    {
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
        /// 相似匹配
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public static bool SimilarMatch(string s1, string s2)
        {
            s1 = s1.Replace("-", "").Replace("_", "").Replace(" ", "").ToLower();
            s2 = s2.Replace("-", "").Replace("_", "").Replace(" ", "").ToLower();
            return s1 == s2;
        }

        /// <summary>
        /// 提示符号
        /// </summary>
        /// <param name="tip"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static string TipSymbol(string tip, string symbol = ": ")
        {
            return $"\n{tip.TrimEnd('：').TrimEnd(':')}{symbol}";
        }

        /// <summary>
        /// 输入数据库
        /// </summary>
        /// <param name="model"></param>
        public static TransferVM.ConnectionInfo ConsoleReadDatabase(ConfigDomain model, string tip = "请选择数据库")
        {
            var mo = new TransferVM.ConnectionInfo();

        Flag1:
            var cacheKey = "tmp_Database_Conns";
            var tmpDbConns = CacheTo.Get(cacheKey) as List<TransferVM.ConnectionInfo> ?? new List<TransferVM.ConnectionInfo>();
            var allDbConns = model.ListConnectionInfo.Concat(tmpDbConns).ToList();

            Console.WriteLine($"\n{0,5}. 输入数据库连接信息");
            for (int i = 0; i < allDbConns.Count; i++)
            {
                var obj = allDbConns[i];
                Console.WriteLine($"{i + 1,5}. {obj.ConnectionRemark} -> {obj.ConnectionType} => {obj.ConnectionString}");
            }
            Console.Write(TipSymbol(tip));

            var rdi = int.TryParse(Console.ReadLine().Trim(), out int ed);

            if (rdi && ed == 0)
            {
            Flag2:
                Console.Write(TipSymbol("数据库连接信息（MySQL => Conn）"));
                var tcs = Console.ReadLine().Trim().Split(" => ");
                var tctype = tcs[0].Split(" -> ").Last();
                if (Enum.TryParse(tctype, true, out SharedEnum.TypeDB tdb))
                {
                    mo.ConnectionType = tdb;
                    mo.ConnectionString = tcs[1];
                    mo.ConnectionRemark = $"[TMP] {DateTime.Now:yyyy-MM-dd HH:mm:ss} {tdb}";
                    if (mo.ConnectionString.Length < 10)
                    {
                        Log("连接字符串无效");
                        goto Flag2;
                    }

                    mo.ConnectionString = SharedAdo.DbHelper.SqlConnPreCheck(mo.ConnectionType, mo.ConnectionString);

                    tmpDbConns.Add(mo);
                    CacheTo.Set(cacheKey, tmpDbConns);
                }
                else
                {
                    Log("无效数据库类型");
                    goto Flag2;
                }
            }
            else if (ed > 0 && ed <= allDbConns.Count)
            {
                mo = allDbConns[ed - 1];
            }
            else
            {
                Log($"无效选择，请重新选择");
                goto Flag1;
            }

            Log($"\n已选择 {mo.ConnectionType} => {mo.ConnectionString}\n");

            return mo;
        }

        /// <summary>
        /// 输入文件（夹）
        /// </summary>
        /// <param name="tip">提示文字</param>
        /// <param name="type">默认（0：都可以；1：文件；2：文件夹）</param>
        /// <param name="dv">默认文件（夹）</param>
        /// <param name="mustExist">是否必须存在</param>
        public static string ConsoleReadPath(string tip, int type = 1, string dv = null, bool mustExist = true)
        {
        Flag1:
            var dtip = string.IsNullOrWhiteSpace(dv) ? TipSymbol(tip) : TipSymbol(tip, $"(default: {dv})");
            Console.Write(dtip);
            var path = Console.ReadLine().Trim();
            if (!string.IsNullOrWhiteSpace(dv) && string.IsNullOrWhiteSpace(path))
            {
                path = dv;
            }
            else if (mustExist)
            {
                if ((type == 1 && !File.Exists(path)) || (type == 2 && !Directory.Exists(path)) || (type == 0 && !File.Exists(path) && !Directory.Exists(path)))
                {
                    Log($"{path} 无效文件（夹）");
                    goto Flag1;
                }
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
                Console.WriteLine($"{j + 1,5}. {items[j]}");
            }
            Console.Write(TipSymbol(tip, $"(default: {dv}. {items[dv - 1]}): "));

            var ii = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(ii))
            {
                Log($"\nSelected {dv}. {items[dv - 1]}\n");
                return dv;
            }
            else
            {
                _ = int.TryParse(ii, out int i);

                if (i > 0 && i <= items.Count)
                {
                    Log($"\nSelected {i}. {items[i - 1]}\n");
                    return i;
                }
                else
                {
                    Log($"{ii} invalid");
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
        public static string NewFileName(object prefix, string ext)
        {
            return $"{prefix}_{DateTime.Now:yyyyMMdd_HHmmss}{ext}";
        }

        /// <summary>
        /// 解析路径变量
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ParsePathVar(string str)
        {
            string pattern = @"({\w+})";
            var now = DateTime.Now;

            var ci = new ConfigInit();
            var co = ci.ConfigObj;

            var path = new Regex(pattern).Replace(str, o =>
            {
                var format = o.Groups[1].Value[1..^1];
                return DateTime.Now.ToString(format);
            }).Replace("~", ci.DXHub);

            return path;
        }
    }
}
