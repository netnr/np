namespace Netnr.TencentAI.Model
{
    /// <summary>
    /// 人脸与人体识别>人脸对比
    /// </summary>
    public class Face_FaceCompareRequest : BaseRequest
    {
        /// <summary>
        /// 原始图片的base64编码数据（原图大小上限1MB，支持JPG、PNG、BMP格式）待对比人脸图片A
        /// </summary>
        [Required]
        public string image_a { get; set; }

        /// <summary>
        /// 原始图片的base64编码数据（原图大小上限1MB，支持JPG、PNG、BMP格式）待对比人脸图片B
        /// </summary>
        [Required]
        public string image_b { get; set; }
    }
}