namespace Netnr.TencentAI.Model
{
    /// <summary>
    /// 图片特效>颜龄检测
    /// </summary>
    public class Ptu_FaceAgeRequest : BaseRequest
    {
        /// <summary>
        /// 原始图片的base64编码数据（原图大小上限500KB）
        /// </summary>
        [Required]
        public string image { get; set; }
    }
}