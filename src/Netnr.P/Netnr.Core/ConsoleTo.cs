using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Netnr;

/// <summary>
/// 输出
/// </summary>
public partial class ConsoleTo
{
    /// <summary>
    /// 缓存
    /// </summary>
    public static ConcurrentQueue<string> CurrentCacheLog { get; set; } = new ConcurrentQueue<string>();

    /// <summary>
    /// 写入标记
    /// </summary>
    static int WriteN = 0;

    /// <summary>
    /// 写入日志
    /// </summary>
    /// <param name="msg"></param>
    public static void Log(string msg)
    {
        CurrentCacheLog.Enqueue(msg);

        if (WriteN == 0)
        {
            //保存日志
            Interlocked.Exchange(ref WriteN, 1);

            Task.Run(() =>
            {
                Thread.Sleep(1500);

                var now = DateTime.Now;
                var filename = $"console_{now:yyyyMMdd}.log";

                var path = Path.Combine(AppContext.BaseDirectory, "logs", now.Year.ToString());
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                var fullPath = Path.Combine(path, filename);

                do
                {
                    var sblog = new StringBuilder();
                    while (CurrentCacheLog.TryDequeue(out string log) && sblog.Length < 1024 * 1024 * 10)
                    {
                        sblog.AppendLine(log);
                    }

                    if (sblog != null)
                    {
                        //流写入
                        using var fs = File.Open(fullPath, File.Exists(fullPath) ? FileMode.Append : FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                        using var sw = new StreamWriter(fs);
                        sw.WriteLine(sblog);
                        sw.Flush();
                        sw.Close();
                    }
                } while (!CurrentCacheLog.IsEmpty);

                Interlocked.Exchange(ref WriteN, 0);
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
        var list = new List<string> {
            "",
            $"------- [{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {tag ?? nameof(Exception)}",
            $"\r\n{ex.Message}\r\n## StackTrace\r\n{ex.StackTrace}"
        };
        Console.WriteLine(list[0]);
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(list[1]);
        Console.ResetColor();
        Console.WriteLine(list[2]);

        Log(string.Join(Environment.NewLine, list));
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
    /// 输出标签
    /// </summary>
    /// <param name="name">标题</param>
    public static void LogTag(string name)
    {
        var list = new List<string> { "", $"------- {name}" };
        Console.WriteLine(list[0]);
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(list[1]);
        Console.ResetColor();

        Log(string.Join(Environment.NewLine, list));
    }

    /// <summary>
    /// 输出卡片
    /// </summary>
    /// <param name="title">标题</param>
    /// <param name="content">内容（可选）</param>
    public static void WriteCard(string title, object content = null)
    {
        Console.WriteLine("");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"------- [{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {title}\r\n");
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