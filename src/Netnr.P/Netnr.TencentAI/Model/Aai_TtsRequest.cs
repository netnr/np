namespace Netnr.TencentAI.Model
{
    /// <summary>
    /// 语音合成>语音合成（AI Lab）
    /// </summary>
    public class Aai_TtsRequest : BaseRequest
    {
        /// <summary>
        /// 语音发音人编码，1-普通话男声 5-静琪女声 6-欢馨女声 7-碧萱女声
        /// </summary>
        [Required]
        public int speaker { get; set; } = 1;

        /// <summary>
        /// 合成语音格式编码，1-PCM 2-WAV 3-MP3
        /// </summary>
        [Required]
        public int format { get; set; } = 2;

        /// <summary>
        /// 合成语音音量，取值范围[-10, 10]，如-10表示音量相对默认值小10dB，0表示默认音量，10表示音量相对默认值大10dB
        /// </summary>
        [Required]
        public int volume { get; set; } = 0;

        /// <summary>
        /// 合成语音语速，取值范围[50, 200]，默认100
        /// </summary>
        [Required]
        public int speed { get; set; } = 100;

        /// <summary>
        /// UTF-8编码，非空且长度上限150字节
        /// </summary>
        [Required]
        public string text { get; set; }

        /// <summary>
        /// 合成语音降低/升高半音个数，即改变音高，默认0
        /// </summary>
        [Required]
        public int aht { get; set; } = 0;

        /// <summary>
        /// 控制频谱翘曲的程度，改变说话人的音色，取值范围[0, 100]，默认58
        /// </summary>
        [Required]
        public int apc { get; set; } = 58;
    }
}