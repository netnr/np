#if Full || Core

using System;
using System.Linq;

namespace Netnr;

/// <summary>
/// 生成随机字符
/// </summary>
public partial class RandomTo
{
    /// <summary>
    /// 获取实例，多次调用时请赋值变量
    /// </summary>
    public static Random Instance
    {
        get
        {
            Random random = new(Guid.NewGuid().GetHashCode());
            return random;
        }
    }

    /// <summary>
    /// 随机字符 验证码
    /// </summary>
    /// <param name="length">长度 默认4个字符</param>
    /// <param name="chars">自定义随机的字符源</param>
    /// <returns></returns>
    public static string NewString(int length = 4, string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789")
    {
        var random = Instance;
        var result = new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        return result;
    }

    /// <summary>
    /// 随机字符 纯数字
    /// </summary>
    /// <param name="length">长度 默认4</param>
    /// <param name="chars">自定义随机的字符源，默认 0-9</param>
    /// <returns></returns>
    public static string NewNumber(int length = 4, string chars = "0123456789") => NewString(length, chars);
}

#endif