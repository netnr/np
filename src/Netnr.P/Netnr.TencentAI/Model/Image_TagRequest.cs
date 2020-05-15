namespace Netnr.TencentAI.Model
{
    /// <summary>
    /// 图片识别>多标签识别
    /// </summary>
    public class Image_TagRequest : BaseRequest
    {
        /// <summary>
        /// 原始图片的base64编码数据（原图大小上限1MB）
        /// </summary>
        [Required]
        public string image { get; set; }
    }
}