namespace Netnr.TencentAI.Model
{
    /// <summary>
    /// 人脸与人体识别>跨年龄人脸识别
    /// </summary>
    public class Face_DetectCrossAgeFaceRequest : BaseRequest
    {
        /// <summary>
        /// 原始图片的base64编码数据（原图大小上限1MB）
        /// </summary>
        [Required]
        public string source_image { get; set; }

        /// <summary>
        /// 原始图片的base64编码数据（原图大小上限1MB）
        /// </summary>
        [Required]
        public string target_image { get; set; }
    }
}
