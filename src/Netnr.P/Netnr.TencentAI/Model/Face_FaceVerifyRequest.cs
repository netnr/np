namespace Netnr.TencentAI.Model
{
    /// <summary>
    /// 人脸与人体识别>人脸验证
    /// </summary>
    public class Face_FaceVerifyRequest : BaseRequest
    {
        /// <summary>
        /// 原始图片的base64编码数据（原图大小上限1MB，支持JPG、PNG、BMP格式）
        /// </summary>
        [Required]
        public string image { get; set; }

        /// <summary>
        /// 待验证的个体（Person）ID
        /// </summary>
        [Required]
        public string person_id { get; set; }
    }
}