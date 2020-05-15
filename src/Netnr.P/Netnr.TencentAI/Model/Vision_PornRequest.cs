namespace Netnr.TencentAI.Model
{
    /// <summary>
    /// 敏感信息审核>图片鉴黄
    /// </summary>
    public class Vision_PornRequest : BaseRequest
    {
        /// <summary>
        /// 原始图片的base64编码数据（原图大小上限1MB，支持JPG、PNG、BMP格式），image和image_url必须至少提供一个
        /// </summary>
        public string image { get; set; }

        /// <summary>
        /// 如果image和image_url都提供，仅支持image_url，image和image_url必须至少提供一个
        /// </summary>
        public string image_url { get; set; }
    }
}