namespace Netnr.TencentAI.Model
{
    /// <summary>
    /// 语音合成>语音合成（优图）
    /// </summary>
    public class Aai_TtaRequest : BaseRequest
    {
        /// <summary>
        /// utf8格式，最大300字节
        /// </summary>
        [Required]
        public string text { get; set; }

        /// <summary>
        /// 发音模型，默认为0,取值范围[0,2]，0-女生 1-女生纯英文 2-男生
        /// </summary>
        [Required]
        public int model_type { get; set; } = 0;

        /// <summary>
        /// 语速，默认为0，取值范围[-2,2]，-2:0.6倍速 -1:0.8倍速 0-正常速度 1:1.2倍速 2:1.5倍速
        /// </summary>
        [Required]
        public int speed { get; set; } = 0;
    }
}