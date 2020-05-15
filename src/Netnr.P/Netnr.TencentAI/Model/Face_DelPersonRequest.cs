namespace Netnr.TencentAI.Model
{
    /// <summary>
    /// 人脸与人体识别>人脸搜索>删除个体
    /// </summary>
    public class Face_DelPersonRequest : BaseRequest
    {
        /// <summary>
        /// 需要删除的个体（Person）ID
        /// </summary>
        [Required]
        public string person_id { get; set; }
    }
}