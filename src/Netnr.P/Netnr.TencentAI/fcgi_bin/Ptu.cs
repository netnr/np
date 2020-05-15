using Netnr.TencentAI.Model;
using System.ComponentModel;

namespace Netnr.TencentAI.fcgi_bin
{
    /// <summary>
    /// 图片特效
    /// </summary>
    [Description("图片特效")]
    public class Ptu
    {
        /// <summary>
        /// 图片特效>图片滤镜（天天P图）,更适合人物图片
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Description("图片滤镜（天天P图）,更适合人物图片")]
        public static string Ptu_ImgFilter(Ptu_ImgFilterRequest request)
        {
            var uri = "https://api.ai.qq.com/fcgi-bin/ptu/ptu_imgfilter";
            return Aid.Request(request, uri);
        }

        /// <summary>
        /// 图片特效>人脸美妆
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Description("人脸美妆")]
        public static string Ptu_FaceCosmetic(Ptu_FaceCosmeticRequest request)
        {
            var uri = "https://api.ai.qq.com/fcgi-bin/ptu/ptu_facecosmetic";
            return Aid.Request(request, uri);
        }

        /// <summary>
        /// 图片特效>人脸变妆
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Description("人脸变妆")]
        public static string Ptu_FaceDecoration(Ptu_FaceDecorationRequest request)
        {
            var uri = "https://api.ai.qq.com/fcgi-bin/ptu/ptu_facedecoration";
            return Aid.Request(request, uri);
        }

        /// <summary>
        /// 图片特效>大头贴
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Description("大头贴")]
        public static string Ptu_FaceSticker(Ptu_FaceStickerRequest request)
        {
            var uri = "https://api.ai.qq.com/fcgi-bin/ptu/ptu_facesticker";
            return Aid.Request(request, uri);
        }

        /// <summary>
        /// 图片特效>颜龄检测
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Description("颜龄检测")]
        public static string Ptu_FaceAge(Ptu_FaceAgeRequest request)
        {
            var uri = "https://api.ai.qq.com/fcgi-bin/ptu/ptu_faceage";
            return Aid.Request(request, uri);
        }
    }
}
