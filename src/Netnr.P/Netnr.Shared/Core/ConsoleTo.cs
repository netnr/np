#if Full || Core

using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Netnr;

/// <summary>
/// 输出
/// </summary>
public partial class ConsoleTo
{
    /// <summary>
    /// 缓存
    /// </summary>
    public static ConcurrentQueue<string> CurrentCacheLog { get; set; } = [];

    /// <summary>
    /// 标记
    /// </summary>
    static int SafeMark = 0;

    /// <summary>
    /// 写入日志
    /// </summary>
    /// <param name="msg"></param>
    public static void Log(string msg)
    {
        CurrentCacheLog.Enqueue(msg);

        if (SafeMark == 0)
        {
            //保存日志
            Interlocked.Exchange(ref SafeMark, 1);

            Task.Run(() =>
            {
                Thread.Sleep(1500);

                var now = DateTime.Now;
                var userName = Environment.UserName.ToLower();
                var filename = $"out_{userName}_{now:yyyyMM}.log";
                var path = Path.Combine(AppContext.BaseDirectory, "logs");

                try
                {
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    var fullPath = Path.Combine(path, filename);

                    do
                    {
                        var sblog = new StringBuilder();
                        //10M
                        while (CurrentCacheLog.TryDequeue(out string log) && sblog.Length < 10485760)
                        {
                            sblog.AppendLine(log);
                        }

                        if (sblog.Length > 0)
                        {
                            //流写入
                            using var fs = File.Open(fullPath, File.Exists(fullPath) ? FileMode.Append : FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                            using var sw = new StreamWriter(fs);
                            sw.WriteLine(sblog);
                            sw.Flush();
                            sw.Close();
                        }
                    } while (!CurrentCacheLog.IsEmpty);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                Interlocked.Exchange(ref SafeMark, 0);
            });
        }
    }

    /// <summary>
    /// 写入日志，打印红色错误信息
    /// </summary>
    /// <param name="ex"></param>
    /// <param name="tag">标记，可选</param>
    public static void LogError(Exception ex, string tag = null)
    {
        var content = $"\r\n----- [{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {tag ?? nameof(Exception)}\r\n{ex.Message}\r\n## StackTrace\r\n{ex.StackTrace}";

        Console.Out.Flush();
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(content);
        Console.ResetColor();

        Log(content);
    }

    /// <summary>
    /// 写入日志，打印指定颜色
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="cc"></param>
    public static void LogColor(string msg, ConsoleColor? cc = null)
    {
        if (cc.HasValue)
        {
            Console.Out.Flush();
            Console.ForegroundColor = cc.Value;
            Console.WriteLine(msg);
            Console.ResetColor();
        }
        else
        {
            Console.WriteLine(msg);
        }

        Log(msg);
    }

    /// <summary>
    /// 写入卡片日志
    /// </summary>
    /// <param name="title">标题</param>
    /// <param name="content">内容（可选）</param>
    public static void LogCard(string title, string content = null)
    {
        var cardTitle = $"\r\n----- {title}\r\n";

        Console.Out.Flush();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(cardTitle);
        Console.ResetColor();

        if (content == null)
        {
            Log(cardTitle);
        }
        else
        {
            Console.WriteLine(content);
            Log(cardTitle + content);
        }
    }

    /// <summary>
    /// 输出卡片
    /// </summary>
    /// <param name="title">标题</param>
    /// <param name="content">内容（可选）</param>
    public static void WriteCard(string title, object content = null)
    {
        var cardTitle = $"\r\n----- [{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {title}\r\n";

        Console.Out.Flush();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(cardTitle);
        Console.ResetColor();

        if (content != null)
        {
            Console.WriteLine(content);
        }
    }

    /// <summary>
    /// 读取重试
    /// </summary>
    /// <param name="readAction">通过返回 True</param>
    /// <returns></returns>
    public static async Task ReadRetry(Func<Task<bool>> readAction)
    {
        bool ifPass = false;
        do
        {
            try
            {
                ifPass = await readAction.Invoke().ConfigureAwait(false);
                //通过
                if (!ifPass)
                {
                    throw new Exception("not pass");
                }
            }
            catch (Exception)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                await Console.Out.WriteLineAsync("Invalid entry, please try again");
                Console.ResetColor();
            }
        } while (!ifPass);
    }
}

#endif