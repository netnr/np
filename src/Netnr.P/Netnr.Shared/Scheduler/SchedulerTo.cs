#if Full || Scheduler

using System.Text.RegularExpressions;

namespace Netnr;

/// <summary>
/// FluentScheduler 调度器扩展
/// </summary>
public static partial class SchedulerTo
{
    /// <summary>
    /// 设置任务定时
    /// </summary>
    /// <param name="schedule"></param>
    /// <param name="timming"></param>
    /// <returns></returns>
    public static bool TimmingSet(this Schedule schedule, string timming)
    {
        Match mr;
        if ((mr = TimmingSeconds().Match(timming)).Success)
        {
            var second = int.Parse(mr.Groups[1].ToString());
            schedule.ToRunEvery(second).Seconds();
        }
        else if ((mr = TimmingMinutes().Match(timming)).Success)
        {
            var minute = int.Parse(mr.Groups[1].ToString());
            schedule.ToRunEvery(minute).Minutes();
        }
        else if ((mr = TimmingHours().Match(timming)).Success)
        {
            var hour = int.Parse(mr.Groups[1].ToString());
            var minute = int.Parse(mr.Groups[2].ToString());
            schedule.ToRunEvery(hour).Hours().At(minute);
        }
        else if ((mr = TimmingDays().Match(timming)).Success)
        {
            var day = int.Parse(mr.Groups[1].ToString());
            var hour = int.Parse(mr.Groups[2].ToString());
            var minute = int.Parse(mr.Groups[3].ToString());
            schedule.ToRunEvery(day).Days().At(hour, minute);
        }
        else if ((mr = TimmingWeekdays().Match(timming)).Success)
        {
            var weekday = int.Parse(mr.Groups[1].ToString());
            var hour = int.Parse(mr.Groups[2].ToString());
            var minute = int.Parse(mr.Groups[3].ToString());
            schedule.ToRunEvery(weekday).Weekdays().At(hour, minute);
        }
        else if ((mr = TimmingWeeks().Match(timming)).Success)
        {
            var week = int.Parse(mr.Groups[1].ToString());
            var onDayOfWeek = (DayOfWeek)int.Parse(mr.Groups[2].ToString());
            var hour = int.Parse(mr.Groups[3].ToString());
            var minute = int.Parse(mr.Groups[4].ToString());
            schedule.ToRunEvery(week).Weeks().On(onDayOfWeek).At(hour, minute);
        }
        else if ((mr = TimmingMonths().Match(timming)).Success)
        {
            var month = int.Parse(mr.Groups[1].ToString());
            var onDay = int.Parse(mr.Groups[2].ToString());
            var hour = int.Parse(mr.Groups[3].ToString());
            var minute = int.Parse(mr.Groups[4].ToString());
            if (onDay == -1)
            {
                schedule.ToRunEvery(month).Months().OnTheLastDay().At(hour, minute);
            }
            else
            {
                schedule.ToRunEvery(month).Months().On(onDay).At(hour, minute);
            }
        }

        return mr.Success;
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