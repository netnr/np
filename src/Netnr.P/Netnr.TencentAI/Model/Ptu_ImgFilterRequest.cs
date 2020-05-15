namespace Netnr.TencentAI.Model
{
    /// <summary>
    /// 图片特效>图片滤镜（天天P图）,更适合人物图片
    /// </summary>
    public class Ptu_ImgFilterRequest : BaseRequest
    {
        /// <summary>
        /// 滤镜特效编码
        /// </summary>
        [Required]
        public int filter { get; set; }

        /// <summary>
        /// 原始图片的base64编码数据（原图大小上限500KB）
        /// </summary>
        [Required]
        public string image { get; set; }
    }
}