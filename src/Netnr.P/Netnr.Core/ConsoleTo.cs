using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Netnr;

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
    static int WriteN = 0;

    /// <summary>
    /// 写入错误信息
    /// </summary>
    /// <param name="ex"></param>
    /// <param name="tag">标记</param>
    public static void Log(Exception ex, string tag = null)
    {
        if (tag != null)
        {
            Title(tag, ex);
        }

        var msg = $"\r\n## Exception {tag} {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}\r\n{ex.Message}\r\n### StackTrace\r\n{ex.StackTrace}";
        Log(msg);
    }

    /// <summary>
    /// 写入消息
    /// </summary>
    /// <param name="msg"></param>
    public static void Log(string msg)
    {
        CurrentCacheLog.Enqueue(msg);

        if (WriteN == 0)
        {
            Interlocked.Exchange(ref WriteN, 1);
            ThreadPool.QueueUserWorkItem(_ => SaveLog());
        }
    }

    /// <summary>
    /// 保存日志
    /// </summary>
    private static void SaveLog()
    {
        Thread.Sleep(1000);

        var now = DateTime.Now;
        var filename = $"console_{now:yyyyMMdd}.log";

        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", now.Year.ToString());
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
    }

    /// <summary>
    /// 输出标题
    /// </summary>
    /// <param name="title">标题</param>
    /// <param name="contentArray">内容（可选）</param>
    public static void Title(string title, params object[] contentArray)
    {
        Console.WriteLine("");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"------- [{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {title}\r\n");
        Console.ForegroundColor = ConsoleColor.White;

        foreach (var content in contentArray)
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
                    throw new Exception("dot pass");
                }
            }
            catch (Exception)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                await Console.Out.WriteLineAsync("Invalid entry, please try again");
                Console.ForegroundColor = ConsoleColor.White;
            }
        } while (!ifPass);
    }
}