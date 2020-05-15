namespace Netnr.TencentAI.Model
{
    /// <summary>
    /// 人脸与人体识别>人脸搜索>增加人脸
    /// </summary>
    public class Face_AddFaceRequest : BaseRequest
    {
        /// <summary>
        /// 指定的个体（Person）ID
        /// </summary>
        [Required]
        public string person_id { get; set; }

        /// <summary>
        /// 原始图片的base64编码数据（原图大小上限1MB，支持JPG、PNG、BMP格式，一次请求最多提交5个人脸）多个人脸图片之间用“|”分隔
        /// </summary>
        [Required]
        public string images { get; set; }

        /// <summary>
        /// 备注信息
        /// </summary>
        [Required]
        public string tag { get; set; }
    }
}