using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Concurrent;

namespace Netnr;

/// <summary>
/// 缓存
/// </summary>
public class CacheTo
{
    private static Stopwatch Sw { get; set; } = new Stopwatch();
    private static ConcurrentDictionary<string, ValueTuple<object, long, int, bool>> CacheDictionary_ { get; set; }

    /// <summary>
    /// key, 数据, 跑表开始时间（毫秒）, 有效时间（秒），相对过期
    /// </summary>
    private static ConcurrentDictionary<string, ValueTuple<object, long, int, bool>> CacheDictionary
    {
        get
        {
            if (CacheDictionary_ == null)
            {
                Sw.Start();
                CacheDictionary_ = new ConcurrentDictionary<string, (object, long, int, bool)>();

                //定时清理
                Task.Run(() =>
                {
                    while (true)
                    {
                        Thread.Sleep(10000);

                        CacheDictionary.Keys.ToList().ForEach(key =>
                        {
                            if (CacheDictionary.TryGetValue(key, out (object data, long startElapsed, int second, bool sliding) value))
                            {
                                //过期
                                if (value.second != 0 && (Sw.ElapsedMilliseconds - value.startElapsed) > value.second * 1000)
                                {
                                    CacheDictionary.TryRemove(key, out _);
                                }
                            }
                        });
                    }
                });
            }
            return CacheDictionary_;
        }
    }

    /// <summary>
    /// 设置
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="data">值</param>
    /// <param name="second">过期时间，默认7200秒 </param>
    /// <param name="sliding">是否相对过期，默认是；否，则固定时间过期</param>
    public static void Set(string key, object data, int second = 7200, bool sliding = true)
    {
        CacheDictionary[key] = new(data, Sw.ElapsedMilliseconds, second, sliding);
    }

    /// <summary>
    /// 获取（没有返回默认值）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    public static T Get<T>(string key)
    {
        if (CacheDictionary.TryGetValue(key, out (object data, long startElapsed, int second, bool sliding) value))
        {
            //没过期
            if (value.second == 0 || (Sw.ElapsedMilliseconds - value.startElapsed) <= value.second * 1000)
            {
                //相对过期，刷新过期
                if (value.sliding)
                {
                    value.startElapsed = Sw.ElapsedMilliseconds;
                    CacheDictionary[key] = value;
                }

                return (T)value.data;
            }
            else
            {
                CacheDictionary.TryRemove(key, out _);
            }
        }
        return default;
    }

    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="key"></param>
    public static void Remove(string key) => CacheDictionary.TryRemove(key, out _);

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
    /// <returns></returns>
    public static bool Contains(string key)
    {
        if (CacheDictionary.TryGetValue(key, out (object data, long startElapsed, int second, bool sliding) value))
        {
            //没过期
            if (value.second == 0 || (Sw.ElapsedMilliseconds - value.startElapsed) <= value.second * 1000)
            {
                return true;
            }
        }
        return false;
    }
}
