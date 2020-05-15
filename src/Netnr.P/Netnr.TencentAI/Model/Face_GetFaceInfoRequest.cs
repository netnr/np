namespace Netnr.TencentAI.Model
{
    /// <summary>
    /// 人脸与人体识别>人脸搜索>获取人脸信息
    /// </summary>
    public class Face_GetFaceInfoRequest : BaseRequest
    {
        /// <summary>
        /// 人脸（Face） ID
        /// </summary>
        [Required]
        public string face_id { get; set; }
    }
}