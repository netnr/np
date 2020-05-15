namespace Netnr.TencentAI.Model
{
    /// <summary>
    /// OCR>身份证OCR
    /// </summary>
    public class Ocr_IdCardOcrRequest : BaseRequest
    {
        /// <summary>
        /// 原始图片的base64编码数据（原图大小上限1MB，支持JPG、PNG、BMP格式）
        /// </summary>
        [Required]
        public string image { get; set; }

        /// <summary>
        /// 身份证图片类型，0-正面，1-反面
        /// </summary>
        [Required]
        public int card_type { get; set; }
    }
}