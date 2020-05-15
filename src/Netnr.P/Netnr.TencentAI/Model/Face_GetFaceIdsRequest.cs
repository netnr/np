namespace Netnr.TencentAI.Model
{
    /// <summary>
    /// 人脸与人体识别>人脸搜索>获取人脸列表
    /// </summary>
    public class Face_GetFaceIdsRequest : BaseRequest
    {
        /// <summary>
        /// 个体（Person）ID
        /// </summary>
        [Required]
        public string person_id { get; set; }
    }
}