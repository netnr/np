using Netnr.TencentAI.Model;
using System.ComponentModel;

namespace Netnr.TencentAI.fcgi_bin
{
    /// <summary>
    /// 
    /// </summary>
    public class Vision
    {
        /// <summary>
        /// 图片特效>图片滤镜（AI Lab）,更适合风景图片
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Description("图片特效>图片滤镜（AI Lab）,更适合风景图片")]
        public static string Vision_ImgFilter(Vision_ImgFilterRequest request)
        {
            var uri = "https://api.ai.qq.com/fcgi-bin/vision/vision_imgfilter";
            return Aid.Request(request, uri);
        }

        /// <summary>
        /// 图片识别>看图说话
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Description("图片识别>看图说话")]
        public static string Vision_ImgToText(Vision_ImgToTextRequest request)
        {
            var uri = "https://api.ai.qq.com/fcgi-bin/vision/vision_imgtotext";
            return Aid.Request(request, uri);
        }

        /// <summary>
        /// 图片识别>场景识别
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Description("图片识别>场景识别")]
        public static string Vision_Scener(Vision_ScenerRequest request)
        {
            var uri = "https://api.ai.qq.com/fcgi-bin/vision/vision_scener";
            return Aid.Request(request, uri);
        }

        /// <summary>
        /// 图片识别>物体识别
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Description("图片识别>物体识别")]
        public static string Vision_Objectr(Vision_ObjectrRequest request)
        {
            var uri = "https://api.ai.qq.com/fcgi-bin/vision/vision_objectr";
            return Aid.Request(request, uri);
        }

        /// <summary>
        /// 敏感信息审核>图片鉴黄
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Description("敏感信息审核>图片鉴黄")]
        public static string Vision_Porn(Vision_PornRequest request)
        {
            var uri = "https://api.ai.qq.com/fcgi-bin/vision/vision_porn";
            return Aid.Request(request, uri);
        }
    }
}