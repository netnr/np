namespace Netnr.TencentAI.Model
{
    /// <summary>
    /// 人脸与人体识别>多人脸检测
    /// </summary>
    public class Face_DetectMultiFaceRequest : BaseRequest
    {
        /// <summary>
        /// 原始图片的base64编码数据（原图大小上限1MB）
        /// </summary>
        [Required]
        public string image { get; set; }
    }
}