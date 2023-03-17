namespace Netnr.Blog.Application.Services
{
    /// <summary>
    /// 脚本服务
    /// </summary>
    public class ScriptService
    {
        public static JsonElement.ArrayEnumerator NavArray
        {
            get
            {
                var fullPath = Path.Combine(AppTo.WebRootPath, "file/data-nav.json");
                return File.ReadAllText(fullPath).DeJson().EnumerateArray();
            }
        }

        /// <summary>
        /// 返回一项
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static JsonElement? Find(string action)
        {
            foreach (var objGroup in NavArray)
            {
                foreach (var objItem in objGroup.GetProperty("items").EnumerateArray())
                {
                    if (objItem.GetValue("url").TrimStart('/') == action)
                    {
                        return objItem;
                    }
                }
            }

            return null;
        }
    }
}
