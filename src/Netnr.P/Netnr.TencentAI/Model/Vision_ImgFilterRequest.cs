namespace Netnr.TencentAI.Model
{
    /// <summary>
    /// 图片特效>图片滤镜（AI Lab）,更适合风景图片
    /// </summary>
    public class Vision_ImgFilterRequest : BaseRequest
    {
        /// <summary>
        /// 滤镜特效编码
        /// </summary>
        [Required]
        public int filter { get; set; }

        /// <summary>
        /// 原始图片的base64编码数据（原图大小上限1MB）
        /// </summary>
        [Required]
        public string image { get; set; }

        /// <summary>
        /// 尽可能唯一，长度上限64字节,一次请求ID
        /// </summary>
        [Required]
        public string session_id { get; set; } = System.Guid.NewGuid().ToString("N");
    }
}