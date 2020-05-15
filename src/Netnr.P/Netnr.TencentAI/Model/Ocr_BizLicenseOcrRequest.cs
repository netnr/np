namespace Netnr.TencentAI.Model
{
    /// <summary>
    /// OCR>营业执照OCR
    /// </summary>
    public class Ocr_BizLicenseOcrRequest : BaseRequest
    {
        /// <summary>
        /// 原始图片的base64编码数据（原图大小上限1MB，支持JPG、PNG、BMP格式）
        /// </summary>
        [Required]
        public string image { get; set; }
    }
}