namespace Netnr.TencentAI.Model
{
    /// <summary>
    /// OCR>行驶证/驾驶证OCR
    /// </summary>
    public class Ocr_DriverLicenseOcrRequest : BaseRequest
    {
        /// <summary>
        /// 原始图片的base64编码数据（原图大小上限1MB，支持JPG、PNG、BMP格式）
        /// </summary>
        [Required]
        public string image { get; set; }

        /// <summary>
        /// 识别类型，0-行驶证识别，1-驾驶证识别
        /// </summary>
        [Required]
        public int type { get; set; }
    }
}