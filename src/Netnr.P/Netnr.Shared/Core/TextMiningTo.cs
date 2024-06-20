#if Full || Core

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Netnr;

/// <summary>
/// 互联网时代的社会语言学：基于SNS的文本数据挖掘
/// http://www.matrix67.com/blog/archives/5044
/// https://www.cnblogs.com/cliffhuang/p/3334219.html
/// </summary>
public partial class TextMiningTo
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// 构造
    /// </remarks>
    /// <param name="value"></param>
    /// <param name="positionOnRight"></param>
    public struct CharPos(char value, bool positionOnRight)
    {
        /// <summary>
        /// 
        /// </summary>
        public char ThisChar = value;
        /// <summary>
        /// 
        /// </summary>
        public bool PositionOnRight = positionOnRight;
    }

    /// <summary>
    /// 英文汉字
    /// </summary>
    public readonly static Regex RegSplit_CN_EN = new(@"\W+", RegexOptions.Compiled | RegexOptions.Multiline);
    /// <summary>
    /// 汉字
    /// </summary>
    public readonly static Regex RegSplit_CN = new(@"\W+|[a-zA-Z0-9]+", RegexOptions.Compiled | RegexOptions.Multiline);

    /// <summary>
    /// 词组最大长度
    /// </summary>
    public int OptionsMaxWordLength { get; set; } = 10;
    /// <summary>
    /// 最小频率
    /// </summary>
    public int OptionsMinFreq { get; set; } = 2;
    /// <summary>
    /// 
    /// </summary>
    public double OptionsPSvPThreshold { get; set; } = 10;
    /// <summary>
    /// 
    /// </summary>
    public double OptionsEntropyThreshold { get; set; } = 1.0;
    /// <summary>
    /// 分割正则
    /// </summary>
    public Regex OptionsRegSplit { get; set; } = RegSplit_CN_EN;
    /// <summary>
    /// 结果
    /// </summary>
    public HashSet<string> Result { get; set; } = [];
    /// <summary>
    /// 穷举
    /// </summary>
    public Dictionary<string, int> DictItems { get; set; } = [];

    private readonly Dictionary<string, Dictionary<CharPos, int>> words = [];
    private readonly Dictionary<string, double> ps = [];
    private int total;

    private void WordInfoEntropy(string word, out double leftEntropy, out double rightEntropy)
    {
        leftEntropy = rightEntropy = 0;
        double totalL = 0, totalR = 0;
        foreach (var pair in words[word])
        {
            if (pair.Key.PositionOnRight)
            {
                totalR += pair.Value;
            }
            else
            {
                totalL += pair.Value;
            }
        }

        if (totalL <= 0)
        {
            leftEntropy = double.MaxValue;
        }

        if (totalR <= 0)
        {
            rightEntropy = double.MaxValue;
        }

        foreach (var pair in words[word])
        {
            var p = pair.Value / (pair.Key.PositionOnRight ? totalR : totalL);
            double entropy = (pair.Key.PositionOnRight ? ref rightEntropy : ref leftEntropy);
            entropy -= p * Math.Log(p);
        }
    }

    /// <summary>
    /// 从字符串
    /// </summary>
    /// <param name="rows"></param>
    public void FromString(IEnumerable<string> rows)
    {
        foreach (var row in rows)
        {
            total += AddParagraph(row);
        }

        Process();
    }

    /// <summary>
    /// 从文本
    /// </summary>
    /// <param name="filePath">完整文件路径</param>
    /// <param name="encoding">编码，默认 UTF-8</param>
    public void FromFile(string filePath, Encoding encoding = null)
    {
        encoding ??= Encoding.UTF8;
        using (var sr = new StreamReader(filePath, encoding))
        {
            string row;
            while ((row = sr.ReadLine()) != null)
            {
                total += AddParagraph(row);
            }
        }

        Process();
    }

    /// <summary>
    /// 按词频获取前 N 项
    /// </summary>
    /// <param name="top">默认 50</param>
    /// <returns></returns>
    public IEnumerable<KeyValuePair<string, int>> TopItems(int top = 50)
    {
        var result = DictItems
            .Where(x => x.Key.Length > 1)
            .OrderByDescending(x => x.Value)
            .Take(top);
        return result;
    }

    /// <summary>
    /// 开始处理
    /// </summary>
    private void Process()
    {
        FinalizeParagraph();

        foreach (var pair in ps)
        {
            if (pair.Key.Length < 2 || pair.Key.Length > OptionsMaxWordLength)
            {
                continue;
            }

            var p = 0.0;
            for (var i = 1; i < pair.Key.Length; ++i)
            {
                var t = ps[pair.Key.Substring(0, i)] * ps[pair.Key.Substring(i)];
                p = Math.Max(p, t);
            }

            if (DictItems[pair.Key] >= OptionsMinFreq && pair.Value / p > OptionsPSvPThreshold)
            {
                words.Add(pair.Key, []);
            }
        }

        foreach (var cword in DictItems.Keys)
        {
            var wl = cword.Length > 1 ? cword.Substring(1) : "";
            var wr = cword.Length > 1 ? cword.Substring(0, cword.Length - 1) : "";
            var wc = cword.Length > 2 ? cword.Substring(1, cword.Length - 2) : "";
            var frq = DictItems[cword];

            if (words.TryGetValue(wl, out var left))
            {
                left.TryGetValue(new CharPos(cword[0], false), out var leftVal);
                left[new CharPos(cword[0], false)] = leftVal + frq;
            }

            if (words.TryGetValue(wr, out var right))
            {
                right.TryGetValue(new CharPos(cword[cword.Length - 1], true), out var rightVal);
                right[new CharPos(cword[cword.Length - 1], true)] = rightVal + frq;
            }

            if (words.TryGetValue(wc, out var center))
            {
                center.TryGetValue(new CharPos(cword[0], false), out var centerVal);
                center[new CharPos(cword[0], false)] = centerVal + frq;

                center.TryGetValue(new CharPos(cword[cword.Length - 1], true), out var centerVal2);
                center[new CharPos(cword[cword.Length - 1], true)] = centerVal2 + frq;
            }
        }

        foreach (var word in words.Keys)
        {
            WordInfoEntropy(word, out var leftEntropy, out var rightEntropy);
            if (leftEntropy < OptionsEntropyThreshold || rightEntropy < OptionsEntropyThreshold)
            {
                continue;
            }

            Result.Add(word);
        }
    }

    private int AddParagraph(string paragraph)
    {
        var incr_total = 0;
        foreach (var sentence in OptionsRegSplit.Split(paragraph))
        {
            if (sentence.Length < 2)
            {
                continue;
            }

            for (var i = 0; i < sentence.Length; ++i)
            {
                for (var j = 1; j <= OptionsMaxWordLength + 2 && i + j - 1 < sentence.Length; ++j)
                {
                    var word = sentence.Substring(i, j);
                    DictItems.TryGetValue(word, out var freqVal);
                    DictItems[word] = freqVal + 1;
                    ++incr_total;
                }
            }
        }

        return incr_total;
    }

    private void FinalizeParagraph()
    {
        foreach (var key in DictItems.Keys)
        {
            ps[key] = (double)DictItems[key] / total;
        }
    }
}

#endif