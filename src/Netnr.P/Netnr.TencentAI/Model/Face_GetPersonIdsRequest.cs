namespace Netnr.TencentAI.Model
{
    /// <summary>
    /// 人脸与人体识别>人脸搜索>获取个体列表
    /// </summary>
    public class Face_GetPersonIdsRequest : BaseRequest
    {
        /// <summary>
        /// 组（Group）ID
        /// </summary>
        [Required]
        public string group_id { get; set; }
    }
}