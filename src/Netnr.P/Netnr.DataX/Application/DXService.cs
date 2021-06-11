using System;
using System.IO;
using System.Collections.Generic;
using Netnr.Core;
using Netnr.DataX.Domain;

namespace Netnr.DataX.Application
{
    /// <summary>
    /// 辅助类
    /// </summary>
    public partial class DXService
    {
        /// <summary>
        /// 配置名
        /// </summary>
        public static string ConfigName = null;

        /// <summary>
        /// 配置列表
        /// </summary>
        /// <returns></returns>
        public static List<string> GetConfigList()
        {
            var list = new List<string>();

            var co = new ConfigObj();
            var cfgFiles = Directory.GetFiles(co.DXPath, "config*.json");

            foreach (var path in cfgFiles)
            {
                list.Add(Path.GetFileName(path));
            }

            return list;
        }

        /// <summary>
        /// 日志
        /// </summary>
        /// <param name="msg"></param>
        public static void Log(string msg)
        {
            Console.WriteLine(msg);
            ConsoleTo.Log(msg);
        }

        /// <summary>
        /// 模糊匹配
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public static bool FuzzyMatch(string s1, string s2)
        {
            s1 = s1.Replace("-", "").Replace("_", "").Replace(" ", "").ToLower();
            s2 = s2.Replace("-", "").Replace("_", "").Replace(" ", "").ToLower();
            return s1 == s2;
        }
    }
}
