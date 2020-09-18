using System.Text.RegularExpressions;

namespace Netnr.Fast
{
    /// <summary>
    /// 解析
    /// </summary>
    public class ParsingTo
    {
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
                return Regex.IsMatch(txt, @"\w[-\w.+]*@([A-Za-z0-9][-A-Za-z0-9]+\.)+[A-Za-z]{2,14}");
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
                return !Regex.IsMatch(txt.Replace("/", ""), @"\W");
            }
        }

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
    }
}
