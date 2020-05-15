namespace Netnr.TencentAI.Model
{
    /// <summary>
    /// 机器翻译>语音翻译
    /// </summary>
    public class Nlp_SpeechTranslateRequest : BaseRequest
    {
        /// <summary>
        /// 语音压缩格式编码 3-AMR	4-SILK  6-PCM   8-MP3   9-AAC
        /// </summary>
        [Required]
        public int format { get; set; } = 3;

        /// <summary>
        /// 语音分片所在语音流的偏移量（字节）
        /// </summary>
        [Required]
        public int seq { get; set; } = 0;

        /// <summary>
        /// 是否结束分片标识，0-中间分片 1-结束分片
        /// </summary>
        [Required]
        public int end { get; set; } = 1;

        /// <summary>
        /// 非空且长度上限64B,语音唯一标识（同一应用内）
        /// </summary>
        [Required]
        public string session_id { get; set; } = System.Guid.NewGuid().ToString("N");

        /// <summary>
        /// 语音分片数据的Base64编码，非空且长度上限8MB
        /// </summary>
        [Required]
        public string speech_chunk { get; set; }

        /// <summary>
        /// 源语言缩写    中文-zh   英文-en   日文-jp   韩文-kr   自动识别（中英互译）-auto
        /// </summary>
        [Required]
        public string source { get; set; }

        /// <summary>
        /// 目标语言缩写，en-zh    zh-en,jp,kr    jp-zh   kr-zh
        /// </summary>
        [Required]
        public string target { get; set; }
    }
}