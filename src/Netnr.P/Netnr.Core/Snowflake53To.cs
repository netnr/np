using System;
using System.Diagnostics;

namespace Netnr;

/// <summary>雪花ID，生成53位长度的整数，兼容浏览器端，单机版</summary>
/// <remarks>
/// https://github.com/cnxiaoma/Snowflake53
/// 
/// Timestamp 41位的时间戳，约 69 年
/// Sequence 12位的序列号
/// </remarks>
public class Snowflake53To
{
    /// <summary>
    /// 
    /// </summary>
    public class StopwatchTimeSource
    {
        /// <summary>
        /// 跑表
        /// </summary>
        public static readonly Stopwatch Sw = Stopwatch.StartNew();

        /// <summary>
        /// 
        /// </summary>
        public DateTimeOffset Epoch { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        protected TimeSpan Offset { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public TimeSpan TickDuration { get; private set; }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="epoch"></param>
        /// <param name="tickDuration"></param>
        public StopwatchTimeSource(DateTimeOffset epoch, TimeSpan tickDuration)
        {
            Epoch = epoch;
            Offset = (DateTimeOffset.UtcNow - Epoch);
            TickDuration = tickDuration;
        }

        /// <summary>
        /// 取嘀嗒
        /// </summary>
        /// <returns></returns>
        public long GetTicks() => (Offset.Ticks + Sw.ElapsedTicks) / TickDuration.Ticks;
    }

    /// <summary>
    /// 生成
    /// </summary>
    public class IdGenerator
    {
        /// <summary>
        /// 首次使用设置，默认 2022-06-06
        /// </summary>
        public static DateTime DefaultEpoch = new(2022, 6, 6, 0, 0, 0, DateTimeKind.Utc);

        private static readonly StopwatchTimeSource timesource = new(DefaultEpoch, TimeSpan.FromMilliseconds(1));

        private int _sequence;
        private long _lastgen = -1;

        private readonly object _genlock = new();

        /// <summary>
        /// Create
        /// </summary>
        /// <returns></returns>
        public long CreateId()
        {
            lock (_genlock)
            {
                var ticks = timesource.GetTicks();
                var timestamp = ticks & ((1L << 41) - 1);

                if (timestamp == _lastgen)
                {
                    _sequence++;
                }
                else
                {
                    _sequence = 0;
                    _lastgen = timestamp;
                }

                unchecked
                {
                    return (timestamp << 12) + _sequence;
                }
            }
        }
    }


    #region 实例

    private static IdGenerator IdGen { get; set; }

    /// <summary>
    /// 实例对象
    /// </summary>
    public static IdGenerator IdGenInstance
    {
        get
        {
            IdGen ??= new IdGenerator();
            return IdGen;
        }
        set
        {
            IdGen = value;
        }
    }

    /// <summary>
    /// 新ID
    /// </summary>
    /// <returns></returns>
    public static long Id() => IdGenInstance.CreateId();

    #endregion
}