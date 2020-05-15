namespace Netnr.TencentAI.Model
{
    /// <summary>
    /// 语音识别>语音识别-echo版
    /// </summary>
    public class Aai_AsrRequest : BaseRequest
    {
        /// <summary>
        /// 语音压缩格式编码，PCM-1  WAV-2   AMR-3   SILK-4
        /// </summary>
        [Required]
        public int format { get; set; }

        /// <summary>
        /// 语音数据的Base64编码，非空且长度上限8MB （时长上限15s）
        /// </summary>
        [Required]
        public string speech { get; set; }

        /// <summary>
        /// 语音采样率编码，默认即16KHz，8KHz-8000  16KHz-16000
        /// </summary>
        [Required]
        public int rate { get; set; } = 16000;
    }
}