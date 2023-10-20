using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Netnr;

/// <summary>
/// 解析
/// </summary>
public partial class ParsingTo
{
    /// <summary>
    /// 路径结合，默认 / 拼接
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public static string Combine(params string[] args)
    {
        var path = string.Empty;
        foreach (var arg in args)
        {
            if (!string.IsNullOrWhiteSpace(arg))
            {
                if (path == string.Empty)
                {
                    path = arg.Trim();
                }
                else
                {
                    var tsarg = arg.Trim().Trim('.').TrimStart('/');
                    path += (path.EndsWith("/") || path.EndsWith("\\")) ? tsarg : '/' + tsarg;
                }
            }
        }
        return path;
    }

    /// <summary>
    /// 根据时间段判断拆分维度
    /// </summary>
    /// <param name="time1"></param>
    /// <param name="time2"></param>
    /// <returns></returns>
    public static string SplitDimension(DateTime? time1 = null, DateTime? time2 = null)
    {
        if (!time1.HasValue)
        {
            return "Months"; // 默认按月
        }

        var endTime = time2 ?? DateTime.Now;
        var timeDiff = endTime - time1.Value;

        if (timeDiff.TotalDays > 365)
        {
            return "Years";
        }
        else if (timeDiff.TotalDays > 30)
        {
            return "Months";
        }
        else if (timeDiff.TotalDays >= 1)
        {
            return "Days";
        }
        else if (timeDiff.TotalHours >= 1)
        {
            return "Hours";
        }
        else
        {
            return "Minutes";
        }
    }

    /// <summary>
    /// 按字节截取字符串
    /// </summary>
    /// <param name="content"></param>
    /// <param name="byteSize">字节大小</param>
    /// <param name="encoding">编码，默认 UTF8</param>
    /// <returns></returns>
    public static List<string> SplitBySize(string content, int byteSize, Encoding encoding = null)
    {
        var parts = new List<string>();
        var sb = new StringBuilder();
        int currentBytes = 0;

        foreach (char c in content)
        {
            int charBytes = (encoding ?? Encoding.UTF8).GetByteCount(c.ToString());

            if (currentBytes + charBytes > byteSize)
            {
                parts.Add(sb.ToString());
                sb.Clear();
                currentBytes = 0;
            }

            sb.Append(c);
            currentBytes += charBytes;
        }

        if (sb.Length > 0)
        {
            parts.Add(sb.ToString());
        }

        return parts;
    }

    /// <summary>
    /// 是邮件地址
    /// </summary>
    /// <param name="txt"></param>
    /// <returns></returns>
    public static bool IsMail(string txt)
    {
        if (string.IsNullOrWhiteSpace(txt))
        {
            return false;
        }
        else
        {
            return Regex.IsMatch(txt, @"\w[-\w.+]*@([A-Za-z0-9][-A-Za-z0-9]+\.)+[A-Za-z]{2,14}", RegexOptions.Compiled);
        }
    }

    /// <summary>
    /// 是合法链接路径（数字、字母、下划线）；可为多级路径，如：abc/xyz ；为空时返回不合法
    /// </summary>
    /// <param name="txt"></param>
    /// <returns></returns>
    public static bool IsLinkPath(string txt)
    {
        if (string.IsNullOrWhiteSpace(txt))
        {
            return false;
        }
        else
        {
            return !Regex.IsMatch(txt.Replace("/", ""), @"\W", RegexOptions.Compiled);
        }
    }

    /// <summary>
    /// 危险替换：仅保留 字母、数字或下划线
    /// </summary>
    /// <param name="txt"></param>
    /// <returns></returns>
    public static string ReplaceDanger(string txt)
    {
        var result = Regex.Replace(txt, "[^a-zA-Z0-9_]+", "", RegexOptions.Compiled);
        return result;
    }

    /// <summary>
    /// 危险检测：非字母、数字或下划线
    /// </summary>
    /// <param name="txt"></param>
    /// <returns></returns>
    public static bool IsDanger(string txt) => Regex.IsMatch(txt, "[^a-zA-Z0-9_]+", RegexOptions.Compiled);

    /// <summary>
    /// 风险扩展名检测
    /// </summary>
    /// <param name="filename"></param>
    /// <returns></returns>
    public static bool IsRiskExtension(string filename)
    {
        var risk = false;
        if (filename.EndsWith("."))
        {
            risk = true;
        }
        else
        {
            var ext = Path.GetExtension(filename).ToLower();
            if (ext.Length > 1)
            {
                risk = RiskExtensions.Contains(ext);
            }
        }

        return risk;
    }

    /// <summary>
    /// 风险扩展名
    /// </summary>
    private static readonly List<string> RiskExtensions = new()
    {
        ".exe",
        ".bat",
        ".sh",
        ".php",
        ".php3",
        ".asa",
        ".asp",
        ".aspx",
        ".js",
        ".jse",
        ".jsp",
        ".jspx",
        ".dll",
        ".so",
        ".jar",
        ".war",
        ".ear",
        ".ps1",
        ".psm1",
        ".pl",
        ".pm",
        ".py",
        ".pyc",
        ".pyo",
        ".rb"
    };

    /// <summary>
    /// JS安全拼接
    /// </summary>
    /// <param name="txt">内容</param>
    /// <returns></returns>
    public static string JsSafeJoin(string txt)
    {
        if (string.IsNullOrWhiteSpace(txt))
        {
            return txt;
        }
        return txt.Replace("'", "").Replace("\"", "");
    }

    /// <summary>
    /// 可视化 字节数
    /// </summary>
    /// <param name="size">字节大小</param>
    /// <param name="keep">保留</param>
    /// <param name="rate"></param>
    /// <param name="space">间隔</param>
    /// <returns></returns>
    public static string FormatByte(double size, int keep = 2, int rate = 1024, string space = "")
    {
        if (Math.Abs(size) < rate)
        {
            return $"{Math.Round(size, keep)}{space}B";
        }

        string[] units = rate == 1000
            ? new[] { "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" }
            : new[] { "KiB", "MiB", "GiB", "TiB", "PiB", "EiB", "ZiB", "YiB" };

        var u = -1;
        var r = Math.Pow(10, keep);

        do
        {
            size /= rate;
            ++u;
        } while (Math.Round(Math.Abs(size) * r) / r >= rate && u < units.Length - 1);

        var result = $"{Math.Round(size, keep)}{space}{units[u]}";
        return result;
    }

    /// <summary>
    /// 可视化 计数 1、1.2K、3M
    /// </summary>
    /// <param name="num"></param>
    /// <param name="keep"></param>
    /// <param name="rate"></param>
    /// <returns></returns>
    public static string FormatCount(double num, int keep = 1, int rate = 1024)
    {
        if (Math.Abs(num) < rate)
        {
            return $"{Math.Round(num, keep)}";
        }

        string[] units = new[] { "K", "M", "G", "T", "P", "E", "Z", "Y" };

        var u = -1;
        var r = Math.Pow(10, keep);

        do
        {
            num /= rate;
            ++u;
        } while (Math.Round(Math.Abs(num) * r) / r >= rate && u < units.Length - 1);

        var result = $"{Math.Round(num, keep)}{units[u]}";
        return result;
    }

    /// <summary>
    /// 可视化 时间 好久前
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public static string FormatTimeAgo(DateTime time)
    {
        TimeSpan timeDiff = DateTime.Now - time;

        if (timeDiff.TotalSeconds < 60)
        {
            return $"{(int)timeDiff.TotalSeconds}秒前";
        }
        else if (timeDiff.TotalMinutes < 60)
        {
            return $"{(int)timeDiff.TotalMinutes}分钟前";
        }
        else if (timeDiff.TotalHours < 24)
        {
            return $"{(int)timeDiff.TotalHours}小时{timeDiff.Minutes}分钟前";
        }
        else if (timeDiff.TotalDays < 30)
        {
            return $"{(int)timeDiff.TotalDays}天{timeDiff.Hours}小时{timeDiff.Minutes}分钟前";
        }
        else
        {
            return time.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }

    /// <summary>
    /// 可视化 毫秒
    /// </summary>
    /// <param name="ms">毫秒</param>
    /// <param name="format">格式化</param>
    /// <returns></returns>
    public static string FormatMilliseconds(double ms, string format = @"hh\:mm\:ss\.fff")
    {
        TimeSpan time = TimeSpan.FromMilliseconds(ms);
        return time.ToString(format);
    }
}