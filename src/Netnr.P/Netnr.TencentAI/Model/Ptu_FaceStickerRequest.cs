namespace Netnr.TencentAI.Model
{
    /// <summary>
    /// 图片特效>大头贴
    /// </summary>
    public class Ptu_FaceStickerRequest : BaseRequest
    {
        /// <summary>
        /// 大头贴编码
        /// </summary>
        [Required]
        public int sticker { get; set; }

        /// <summary>
        /// 原始图片的base64编码数据（原图大小上限500KB）
        /// </summary>
        [Required]
        public string image { get; set; }
    }
}