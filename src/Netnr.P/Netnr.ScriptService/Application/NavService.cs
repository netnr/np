using Newtonsoft.Json.Linq;

namespace Netnr.ScriptService.Application
{
    public class NavService
    {
        public static JArray Nav
        {
            get
            {
                var fullPath = Fast.PathTo.Combine(GlobalTo.WebRootPath, "db/nav.json");
                return Core.FileTo.ReadText(fullPath).ToJArray();
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
