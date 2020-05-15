namespace Netnr.TencentAI.Model
{
    /// <summary>
    /// 图片识别>物体识别
    /// </summary>
    public class Vision_ObjectrRequest:BaseRequest
    {
        /// <summary>
        /// 图片格式，1-JPG格式（image/jpeg
        /// </summary>
        [Required]
        public int format { get; set; } = 1;

        /// <summary>
        /// 返回结果个数（已按置信度倒排）,范围[1, 5]
        /// </summary>
        [Required]
        public int topk { get; set; } = 5;

        /// <summary>
        /// 原始图片的base64编码数据（解码后大小上限1MB）
        /// </summary>
        [Required]
        public string image { get; set; }
    }
}