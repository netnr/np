namespace Netnr.TencentAI.Model
{
    /// <summary>
    /// 语音识别>长语音识别
    /// </summary>
    public class Aai_WxAsrLongRequest : BaseRequest
    {
        /// <summary>
        /// 语音压缩格式编码，PCM-1  WAV-2   AMR-3   SILK-4
        /// </summary>
        [Required]
        public int format { get; set; }

        /// <summary>
        /// 用户回调url，需用户提供，用于平台向用户通知识别结果
        /// </summary>
        [Required]
        public string callback_url { get; set; }

        /// <summary>
        /// 语音数据的Base64编码，原始音频大小上限5MB（时长上限15min）
        /// </summary>
        public string speech { get; set; }

        /// <summary>
        /// 音频下载地址，音频大小上限30MB（时长上限15min）
        /// </summary>
        public string speech_url { get; set; }
    }
}