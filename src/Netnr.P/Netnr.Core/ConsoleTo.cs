using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;
using System.Linq;

namespace Netnr.Core
{
    /// <summary>
    /// 输出
    /// </summary>
    public class ConsoleTo
    {
        /// <summary>
        /// 缓存
        /// </summary>
        public static ConcurrentQueue<string> CurrentCacheLog { get; set; } = new ConcurrentQueue<string>();

        /// <summary>
        /// 写入标记
        /// </summary>
        static readonly object WriteMark = new();

        /// <summary>
        /// 写入错误信息
        /// </summary>
        /// <param name="ex"></param>
        public static void Log(Exception ex)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"====日志记录时间：{DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine(ex.ToJson());
            Log(sb);
        }

        /// <summary>
        /// 写入消息
        /// </summary>
        /// <param name="msg"></param>
        public static void Log(object msg)
        {
            string txt;

            try
            {
                txt = msg.GetType().Name switch
                {
                    "Enum" or "Byte" or "Char" or "String" or "StringBuilder" or "Boolean" or "UInt16" or "Int16" or "Int32" or "Int64" or "Single" or "Double" or "Decimal" => msg.ToString(),
                    _ => msg.ToJson(),
                };
            }
            catch (Exception)
            {
                txt = msg.ToString();
            }

            CurrentCacheLog.Enqueue(txt);

            if (Monitor.TryEnter(WriteMark))
            {
                SaveLog();
                Monitor.Exit(WriteMark);
            }
        }

        /// <summary>
        /// 保存日志
        /// </summary>
        private static void SaveLog()
        {
        wmark:
            var sblog = new StringBuilder();
            while (CurrentCacheLog.TryDequeue(out string log))
            {
                sblog.AppendLine(log);
            }
            if (sblog != null)
            {
                var now = DateTime.Now;
                var filename = $"console_{now:yyyyMMdd}.log";
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", now.Year.ToString(), filename);

                FileTo.WriteText(sblog.ToString(), path);
            }

            if (!CurrentCacheLog.IsEmpty)
            {
                goto wmark;
            }
        }

        /// <summary>
        /// 调用菜单
        /// </summary>
        /// <param name="ctype"></param>
        public static void InvokeMenu(Type ctype)
        {
            var cms = ctype.GetMethods().ToList();
            var mm = cms.First().Module;
            cms = cms.Where(x => x.Module == mm).ToList();

            Console.WriteLine(Environment.NewLine);
            var prevGroupName = string.Empty;
            for (int i = 0; i < cms.Count; i++)
            {
                var mi = cms[i];
                var disps = mi.CustomAttributes.LastOrDefault()?.NamedArguments;
                var dispName = disps.FirstOrDefault(x => x.MemberName == "Name").TypedValue.Value.ToString();
                var dispGroupName = disps.FirstOrDefault(x => x.MemberName == "GroupName").TypedValue.Value.ToString();
                var nl = string.Empty;
                if (i > 0 && dispGroupName != prevGroupName)
                {
                    nl = "\n";
                }

                if (!string.IsNullOrWhiteSpace(dispName))
                {
                    Console.WriteLine($"{nl}{i,5}. {mi.Name} -> {dispName}");
                }

                prevGroupName = dispGroupName;
            }

            bool isMenumNum;
            do
            {
                Console.Write("\n请输入数字：");
                isMenumNum = int.TryParse(Console.ReadLine(), out int num) && num >= 0 && num < cms.Count;
                if (isMenumNum)
                {
                    cms[num].Invoke(ctype, null);

                    InvokeMenu(ctype);
                }
            } while (!isMenumNum);
        }
    }
}