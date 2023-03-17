using System;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Netnr.UAParser;

/// <summary>
/// 缓存
/// </summary>
internal class Caches
{
    /// <summary>
    /// 存储计时
    /// </summary>
    private static Stopwatch Sw { get; set; } = new Stopwatch();
    /// <summary>
    /// 过期计时
    /// </summary>
    private static Stopwatch ExpiredStopwatch { get; set; } = new Stopwatch();
    /// <summary>
    /// 过期计数
    /// </summary>
    private static int ExpiredCount { get; set; } = 0;
    /// <summary>
    /// 过期任务
    /// </summary>
    private static Task ExpiredTask { get; set; }

    /// <summary>
    /// 缓存规则，key, 跑表开始时间（毫秒）, 有效时间（秒），相对过期
    /// </summary>
    private static ConcurrentDictionary<string, ValueTuple<long, int, bool>> CacheRule { get; set; } = new();

    private static ConcurrentDictionary<string, object> CacheDictionary_ { get; set; }

    /// <summary>
    /// key, 数据
    /// </summary>
    private static ConcurrentDictionary<string, object> CacheDictionary
    {
        get
        {
            if (CacheDictionary_ == null)
            {
                Sw.Start();
                ExpiredStopwatch.Start();
                CacheDictionary_ = new();

                //定时清理
                ExpiredTask = Task.Run(() =>
                {
                    while (true)
                    {
                        //60s
                        Thread.Sleep(60000);

                        ExpiredStopwatch.Restart();
                        ExpiredCount = 0;

                        foreach (var key in CacheRule.Keys)
                        {
                            if (CacheRule.TryGetValue(key, out (long startElapsed, int second, bool sliding) ruleValue))
                            {
                                //过期
                                if (ruleValue.second != 0 && (Sw.ElapsedMilliseconds - ruleValue.startElapsed) > ruleValue.second * 1000)
                                {
                                    CacheRule.TryRemove(key, out _);
                                    CacheDictionary.TryRemove(key, out _);
                                }
                            }
                        }
                    }
                });
            }

            //重启清理任务
            if (ExpiredStopwatch.ElapsedMilliseconds > 90000 && ExpiredCount++ > 3)
            {
                try
                {
                    ExpiredTask.Start();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Cache Expired Task ERROR");
                    Console.WriteLine(ex);
                }
            }

            return CacheDictionary_;
        }
    }

    /// <summary>
    /// 设置
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="data">值</param>
    /// <param name="second">过期时间，默认7200秒，0不过期 </param>
    /// <param name="sliding">是否相对过期，默认是；否，则固定时间过期</param>
    /// <param name="group">分组</param>
    public static void Set(string key, object data, int second = 7200, bool sliding = true, string group = "Global")
    {
        var gkey = $"{group}-{key}";

        CacheRule[gkey] = new(Sw.ElapsedMilliseconds, second, sliding);
        CacheDictionary[gkey] = data;
    }

    /// <summary>
    /// 获取（没有返回默认值）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="group">分组</param>
    /// <returns></returns>
    public static T Get<T>(string key, string group = "Global")
    {
        var gkey = $"{group}-{key}";

        if (CacheRule.TryGetValue(gkey, out (long startElapsed, int second, bool sliding) ruleValue))
        {
            //没过期
            if (ruleValue.second == 0 || (Sw.ElapsedMilliseconds - ruleValue.startElapsed) <= ruleValue.second * 1000)
            {
                //相对过期，刷新过期
                if (ruleValue.sliding)
                {
                    ruleValue.startElapsed = Sw.ElapsedMilliseconds;
                    CacheRule[gkey] = ruleValue;
                }

                //返回数据
                if (CacheDictionary.TryGetValue(gkey, out object data))
                {
                    return (T)data;
                }
            }
            else
            {
                //删除
                CacheRule.TryRemove(gkey, out _);
                CacheDictionary.TryRemove(gkey, out _);
            }
        }
        return default;
    }

    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="key"></param>
    /// <param name="group">分组</param>
    public static void Remove(string key, string group = "Global")
    {
        var gkey = $"{group}-{key}";

        CacheRule.TryRemove(gkey, out _);
        CacheDictionary.TryRemove(gkey, out _);
    }

    /// <summary>
    /// 删除组
    /// </summary>
    /// <param name="group"></param>
    public static void RemoveGroup(string group)
    {
        var groupPrefix = $"{group}-";
        foreach (var key in CacheDictionary.Keys)
        {
            if (key.StartsWith(groupPrefix))
            {
                CacheRule.TryRemove(key, out _);
                CacheDictionary.TryRemove(key, out _);
            }
        }
    }

    /// <summary>
    /// 清空
    /// </summary>
    public static void RemoveAll()
    {
        Sw.Restart();
        CacheDictionary.Clear();
    }

    /// <summary>
    /// 包含
    /// </summary>
    /// <param name="key"></param>
    /// <param name="group">分组</param>
    /// <returns></returns>
    public static bool Contains(string key, string group = "Global")
    {
        var gkey = $"{group}-{key}";

        if (CacheRule.TryGetValue(gkey, out (long startElapsed, int second, bool sliding) ruleValue))
        {
            //没过期
            if (ruleValue.second == 0 || (Sw.ElapsedMilliseconds - ruleValue.startElapsed) <= ruleValue.second * 1000)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 所有 Keys
    /// </summary>
    public static ICollection<string> Keys
    {
        get
        {
            return CacheDictionary.Keys;
        }
    }
}
