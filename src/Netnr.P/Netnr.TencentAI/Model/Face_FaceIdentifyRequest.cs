namespace Netnr.TencentAI.Model
{
    /// <summary>
    /// 人脸与人体识别>人脸搜索>人脸搜索
    /// </summary>
    public class Face_FaceIdentifyRequest : BaseRequest
    {
        /// <summary>
        /// 原始图片的base64编码数据（原图大小上限1MB，支持JPG、PNG、BMP格式）
        /// </summary>
        [Required]
        public string image { get; set; }

        /// <summary>
        /// 候选人组ID（个体创建时设定）
        /// </summary>
        [Required]
        public string group_id { get; set; }

        /// <summary>
        /// 返回的候选人个数（默认9个）,取值[1-10]
        /// </summary>
        [Required]
        public int topn { get; set; } = 9;
    }
}