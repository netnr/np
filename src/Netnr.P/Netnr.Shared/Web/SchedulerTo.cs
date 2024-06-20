#if Full || Web

using System.Text.RegularExpressions;

namespace Netnr;

/// <summary>
/// FluentScheduler 调度器扩展
/// </summary>
public static partial class SchedulerTo
{
    /// <summary>
    /// 转换成 Cron ，用于兼容旧版本
    /// </summary>
    /// <param name="timming"></param>
    /// <returns></returns>
    public static string ToCronExpression(string timming)
    {
        Match mr;
        if ((mr = TimmingSeconds().Match(timming)).Success)
        {
            var second = int.Parse(mr.Groups[1].ToString());
            return ToCronExpression(second);
        }
        else if ((mr = TimmingMinutes().Match(timming)).Success)
        {
            var minute = int.Parse(mr.Groups[1].ToString());
            return $"*/{minute} * * * *";
        }
        else if ((mr = TimmingHours().Match(timming)).Success)
        {
            var hour = int.Parse(mr.Groups[1].ToString());
            var minute = int.Parse(mr.Groups[2].ToString());
            return $"{minute} */{hour} * * *";
        }
        else if ((mr = TimmingDays().Match(timming)).Success)
        {
            var day = int.Parse(mr.Groups[1].ToString());
            var hour = int.Parse(mr.Groups[2].ToString());
            var minute = int.Parse(mr.Groups[3].ToString());
            return $"{minute} {hour} */{day} * *";
        }
        else if ((mr = TimmingWeekdays().Match(timming)).Success)
        {
            //var weekday = int.Parse(mr.Groups[1].ToString());

            var hour = int.Parse(mr.Groups[2].ToString());
            var minute = int.Parse(mr.Groups[3].ToString());
            return $"{minute} {hour} * * 1-5";
        }
        else if ((mr = TimmingWeeks().Match(timming)).Success)
        {
            //var week = int.Parse(mr.Groups[1].ToString());

            var dayOfWeek = int.Parse(mr.Groups[2].ToString());
            var hour = int.Parse(mr.Groups[3].ToString());
            var minute = int.Parse(mr.Groups[4].ToString());
            return $"{minute} {hour} * * {dayOfWeek}";
        }
        else if ((mr = TimmingMonths().Match(timming)).Success)
        {
            var onDay = int.Parse(mr.Groups[2].ToString());
            var hour = int.Parse(mr.Groups[3].ToString());
            var minute = int.Parse(mr.Groups[4].ToString());
            if (onDay < 0)
            {
                return $"{minute} {hour} L-{Math.Abs(onDay)} * *";
            }
            else
            {
                return $"{minute} {hour} {onDay} * *";
            }
        }

        return null;
    }

    /// <summary>
    /// 秒转换成 Cron 表达式，大于60强制转换成分钟
    /// </summary>
    /// <param name="second"></param>
    /// <returns></returns>
    public static string ToCronExpression(int second)
    {
        if (second < 60)
        {
            return $"*/{second} * * * * *";
        }
        else
        {
            int minute = (int)Math.Round((double)second / 60, MidpointRounding.AwayFromZero);
            return $"*/{minute} * * * *";
        }
    }

    /// <summary>
    /// 时间 "10 Seconds", 每 10 秒钟一次
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex(@"^(\d+) Seconds$", RegexOptions.Singleline)]
    public static partial Regex TimmingSeconds();

    /// <summary>
    /// 时间 "10 Minutes", 每 10 分钟一次
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex(@"^(\d+) Minutes$", RegexOptions.Singleline)]
    public static partial Regex TimmingMinutes();

    /// <summary>
    /// 时间 "10 Hours At 10 Minutes", 每 10 小时的 10 秒
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex(@"^(\d+) Hours At (\d+) Minutes$", RegexOptions.Singleline)]
    public static partial Regex TimmingHours();

    /// <summary>
    /// 时间 "1 Days At 10:10", 每 1 天的 10:10
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex(@"^(\d+) Days At (\d+):(\d+)$", RegexOptions.Singleline)]
    public static partial Regex TimmingDays();

    /// <summary>
    /// 时间 "1 Weekdays At 10:10", 每 1 天（工作日，周一到周五）的 10:10
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex(@"^(\d+) Weekdays At (\d+):(\d+)$", RegexOptions.Singleline)]
    public static partial Regex TimmingWeekdays();

    /// <summary>
    /// 时间 "1 Weeks On [0-6] At 10:10", 每 1 周[周日-周六]的 10:10, 如 每周五10:10 "1 Weeks On 5 At 10:10"
    /// 1 Weeks 下周，0 Weeks 本周
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex(@"^(\d+) Weeks On (\d+) At (\d+):(\d+)$", RegexOptions.Singleline)]
    public static partial Regex TimmingWeeks();

    /// <summary>
    /// 时间 "1 Months On [-1|1-last] At 10:10" 每 1 月在[最后一天或第1天或第n天]的 10:10, 如 每月最后一天10:10 "1 Months On -1 At 10:10"
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex(@"^(\d+) Months On (.*?) At (\d+):(\d+)$", RegexOptions.Singleline)]
    public static partial Regex TimmingMonths();
}

#endif