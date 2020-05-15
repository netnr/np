using Netnr.TencentAI.Model;
using System.ComponentModel;

namespace Netnr.TencentAI.fcgi_bin
{
    /// <summary>
    /// 
    /// </summary>
    public class Image
    {
        /// <summary>
        /// 图片识别>多标签识别
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Description("图片识别>多标签识别")]
        public static string Image_Tag(Image_TagRequest request)
        {
            var uri = "https://api.ai.qq.com/fcgi-bin/image/image_tag";
            return Aid.Request(request, uri);
        }

        /// <summary>
        /// 图片识别>模糊图片检测
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Description("图片识别>模糊图片检测")]
        public static string Image_Fuzzy(Image_FuzzyRequest request)
        {
            var uri = "https://api.ai.qq.com/fcgi-bin/image/image_fuzzy";
            return Aid.Request(request, uri);
        }

        /// <summary>
        /// 图片识别>美食图片识别
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Description("图片识别>美食图片识别")]
        public static string Image_Food(Image_FoodRequest request)
        {
            var uri = "https://api.ai.qq.com/fcgi-bin/image/image_food";
            return Aid.Request(request, uri);
        }

        /// <summary>
        /// 敏感信息审核>暴恐识别
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Description("敏感信息审核>暴恐识别")]
        public static string Image_Terrorism(Image_TerrorismRequest request)
        {
            var uri = "https://api.ai.qq.com/fcgi-bin/image/image_terrorism";
            return Aid.Request(request, uri);
        }
    }
}