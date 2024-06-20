#if Full || Core

using System;
using System.Diagnostics;
using System.Threading;

namespace Netnr;

/// <summary>雪花ID，生成53位长度的整数，兼容浏览器端，单机版</summary>
/// <remarks>
/// https://github.com/cnxiaoma/Snowflake53
/// 
/// Timestamp 41位的时间戳，约 69 年
/// Sequence 12位的序列号
/// </remarks>
public partial class Snowflake53To
{
    private static readonly object _genlock = new();
    private static long _lastTimestamp = -1;
    private static int _sequence = 0;

    private static readonly DateTimeOffset DefaultEpoch = new(2022, 6, 6, 0, 0, 0, TimeSpan.Zero);

    private static readonly TimeSpan TickDuration = TimeSpan.FromMilliseconds(1);

    /// <summary>
    /// 生成ID
    /// </summary>
    /// <returns></returns>
    public static long Id()
    {
        lock (_genlock)
        {
            var timestamp = GetTimestamp();

            if (timestamp == _lastTimestamp)
            {
                _sequence++;

                if (_sequence > 4095)
                {
                    // Wait until next millisecond
                    while (timestamp <= _lastTimestamp)
                    {
                        timestamp = GetTimestamp();
                        Thread.SpinWait(1000);
                    }

                    _sequence = 0;
                    _lastTimestamp = timestamp;
                }
            }
            else
            {
                _sequence = 0;
                _lastTimestamp = timestamp;
            }

            unchecked
            {
                return (timestamp << 12) + _sequence;
            }
        }
    }

    /// <summary>
    /// 尝试分析
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static DateTime? Parse(long id)
    {
        var timestampPart = id >> 12;

        if (timestampPart == 0)
        {
            return null;
        }
        else
        {
            var timeDiff = TimeSpan.FromTicks(TickDuration.Ticks * timestampPart);
            var time = DefaultEpoch + timeDiff;

            return time.LocalDateTime;
        }
    }

    /// <summary>
    /// 根据时间获取ID，用于范围查询
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public static long GetId(DateTime time)
    {
        time = time.ToUniversalTime();
        var t = (long)(time - DefaultEpoch).TotalMilliseconds;
        return t << 12;
    }

    private static long GetTimestamp()
    {
        var timeDiff = DateTimeOffset.UtcNow - DefaultEpoch;
        return (long)(timeDiff.TotalMilliseconds / TickDuration.TotalMilliseconds);
    }
}

#endif