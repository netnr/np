using System;
using System.IO;
using System.Text;

namespace Netnr.Core
{
    /// <summary>
    /// 输出
    /// </summary>
    public class ConsoleTo
    {
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

            var now = DateTime.Now;
            var filename = $"console_{now:yyyyMMdd}.log";
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", now.Year.ToString(), filename);
            FileTo.WriteText(txt, path);
        }

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

    }
}