using System;
using System.Collections.Generic;
using System.IO;

namespace Netnr.Core
{
    /// <summary>
    /// 输出
    /// </summary>
    public class ConsoleTo
    {
        /// <summary>
        /// 写入错误信息
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="isFull">是否全部信息，默认false</param>
        public static void Log(Exception ex, bool isFull = false)
        {
            var msg = ExceptionGet(ex, isFull);
            Log(msg);
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
                switch (msg.GetType().Name)
                {
                    case "Enum":
                    case "Byte":
                    case "Char":
                    case "String":
                    case "Boolean":
                    case "UInt16":
                    case "Int16":
                    case "Int32":
                    case "Int64":
                    case "Single":
                    case "Double":
                    case "Decimal":
                        txt = msg.ToString();
                        break;
                    default:
                        txt = msg.ToJson();
                        break;
                }
            }
            catch (Exception)
            {
                txt = msg.ToString();
            }

            var dt = DateTime.Now;
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs/" + dt.ToString("yyyyMM"));
            FileTo.WriteText(txt, path, "console_" + dt.ToString("yyyyMMdd") + ".log");
        }

        /// <summary>
        /// 获取异常信息
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="isFull">是否包含堆栈所有信息，默认 false</param>
        /// <returns></returns>
        private static string ExceptionGet(Exception ex, bool isFull = false)
        {
            var en = Environment.NewLine;
            var st = ex.StackTrace;
            if (!isFull)
            {
                st = st.Replace(en, "^").Split('^')[0];
            }

            string msg = string.Join(en, new List<string>()
            {
                $"====日志记录时间：{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}",
                $"消息内容：{ex.Message}",
                $"引发异常的方法：{st}{en}"
            });

            if (ex.InnerException != null)
            {
                msg += ExceptionGet(ex.InnerException, isFull);
            }

            return msg;
        }
    }
}