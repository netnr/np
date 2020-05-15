namespace Netnr.TencentAI.Model
{
    /// <summary>
    /// 人脸与人体识别>人脸搜索>获取信息
    /// </summary>
    public class Face_GetInfoRequest : BaseRequest
    {
        /// <summary>
        /// 需要查询的个体（Person）ID
        /// </summary>
        [Required]
        public string person_id { get; set; }
    }
}