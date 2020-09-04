using Newtonsoft.Json.Linq;
using System.IO;

namespace Netnr.ScriptService.Application
{
    public class NavService
    {
        private static JArray _nav;

        public static JArray Nav
        {
            get
            {
                if (_nav == null)
                {
                    var fullPath = Path.Combine(GlobalTo.WebRootPath, "db/nav.json");
                    _nav = Core.FileTo.ReadText(fullPath).ToJArray();
                }
                return _nav;
            }
        }

        /// <summary>
        /// 返回一项
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static JToken Find(string action)
        {
            foreach (var gi in Nav)
            {
                foreach (JToken jt in gi["items"])
                {
                    if (jt["url"].ToString().TrimStart('/') == action)
                    {
                        return jt;
                    }
                }
            }

            return null;
        }
    }
}
