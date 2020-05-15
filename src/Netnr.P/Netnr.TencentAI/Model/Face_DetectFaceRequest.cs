namespace Netnr.TencentAI.Model
{
    /// <summary>
    /// 人脸与人体识别>人脸检测与分析
    /// </summary>
    public class Face_DetectFaceRequest : BaseRequest
    {
        /// <summary>
        /// 原始图片的base64编码数据（原图大小上限1MB，支持JPG、PNG、BMP格式）
        /// </summary>
        [Required]
        public string image { get; set; }

        /// <summary>
        /// 检测模式，0-正常，1-大脸模式（默认1）
        /// </summary>
        [Required]
        public int mode { get; set; } = 1;
    }
}