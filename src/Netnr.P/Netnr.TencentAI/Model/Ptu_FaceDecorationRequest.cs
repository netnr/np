namespace Netnr.TencentAI.Model
{
    /// <summary>
    /// 图片特效>人脸变妆
    /// </summary>
    public class Ptu_FaceDecorationRequest : BaseRequest
    {
        /// <summary>
        /// 变妆编码
        /// </summary>
        [Required]
        public int decoration { get; set; }

        /// <summary>
        /// 原始图片的base64编码数据（原图大小上限500KB）
        /// </summary>
        [Required]
        public string image { get; set; }
    }
}