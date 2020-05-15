namespace Netnr.TencentAI.Model
{
    /// <summary>
    /// 敏感信息审核>音频鉴黄/敏感词检测
    /// </summary>
    public class Aai_EvilAudioRequest : BaseRequest
    {
        /// <summary>
        /// 非空且长度上限64B，同一应用内每段语音流标识需唯一
        /// </summary>
        [Required]
        public string speech_id { get; set; } = System.Guid.NewGuid().ToString("N");

        /// <summary>
        /// 音频URL，非空且长度上限512B，建议音频时长不超过3分钟
        /// </summary>
        [Required]
        public string speech_url { get; set; }

        /// <summary>
        /// 是否开通音频鉴黄（0-不开通，1-开通，不传默认开通），两种检测类型至少开通一种
        /// </summary>
        [Required]
        public int porn_detect { get; set; } = 1;

        /// <summary>
        /// 是否开通敏感词检测（0-不开通，1-开通，不传默认开通），两种检测类型至少开通一种
        /// </summary>
        [Required]
        public int keyword_detect { get; set; } = 1;
    }
}