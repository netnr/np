namespace Netnr.TencentAI.Model
{
    /// <summary>
    /// 图片识别>看图说话
    /// </summary>
    public class Vision_ImgToTextRequest : BaseRequest
    {
        /// <summary>
        /// 原始图片的base64编码数据（原图大小上限1MB）
        /// </summary>
        [Required]
        public string image { get; set; }

        /// <summary>
        /// 尽可能唯一，长度上限64字节，一次请求ID
        /// </summary>
        [Required]
        public string session_id { get; set; } = System.Guid.NewGuid().ToString("N");
    }
}