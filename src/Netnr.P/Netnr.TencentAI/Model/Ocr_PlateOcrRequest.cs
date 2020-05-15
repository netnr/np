namespace Netnr.TencentAI.Model
{
    /// <summary>
    /// OCR>车牌OCR
    /// </summary>
    public class Ocr_PlateOcrRequest : BaseRequest
    {
        /// <summary>
        /// 原始图片的base64编码数据（原图大小上限1MB，支持JPG、PNG、BMP格式）,image和image_url必须至少提供一个
        /// </summary>
        public string image { get; set; }

        /// <summary>
        /// image和image_url都提供，仅支持image_url，image和image_url必须至少提供一个
        /// </summary>
        public string image_url { get; set; }
    }
}