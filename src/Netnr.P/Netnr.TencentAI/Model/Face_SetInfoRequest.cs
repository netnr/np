namespace Netnr.TencentAI.Model
{
    /// <summary>
    /// 人脸与人体识别>人脸搜索>设置信息
    /// </summary>
    public class Face_SetInfoRequest : BaseRequest
    {
        /// <summary>
        /// 需要设置的个体（Person）ID
        /// </summary>
        [Required]
        public string person_id { get; set; }

        /// <summary>
        /// 新的名字
        /// </summary>
        public string person_name { get; set; }

        /// <summary>
        /// 备注信息
        /// </summary>
        public string tag { get; set; }
    }
}