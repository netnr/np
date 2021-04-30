using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;

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

                Monitor.Exit(WriteMark);
            }
        }
    }
}