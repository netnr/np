namespace Netnr.TencentAI.Model
{
    /// <summary>
    /// 人脸与人体识别>人脸搜索>删除人脸
    /// </summary>
    public class Face_DelFaceRequest : BaseRequest
    {
        /// <summary>
        /// 指定的个体（Person）ID
        /// </summary>
        [Required]
        public string person_id { get; set; }

        /// <summary>
        /// 需要删除的人脸（Face）ID（多个之间用“|”）
        /// </summary>
        [Required]
        public string face_ids { get; set; }
    }
}