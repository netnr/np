using Netnr.Core;
using Netnr.SharedFast;

namespace Netnr.Guff.Application
{
    public class BuildService
    {
        /// <summary>
        /// 链接
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string AutoLink(string path = "")
        {
            var isbh = CacheTo.Get(GlobalTo.GetValue("Common:BuildHtmlKey")) as bool? ?? false;
            var hp = isbh ? "" : "/home";
            return hp + path;
        }
    }
}
