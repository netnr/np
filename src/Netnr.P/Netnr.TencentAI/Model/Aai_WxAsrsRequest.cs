namespace Netnr.TencentAI.Model
{
    /// <summary>
    /// 语音识别>流式版(WeChat AI)
    /// </summary>
    public class Aai_WxAsrsRequest : BaseRequest
    {
        /// <summary>
        /// 语音压缩格式编码，PCM-1  WAV-2   AMR-3   SILK-4  SPEEX-5 MP3-8
        /// </summary>
        public int format { get; set; }

        /// <summary>
        /// 语音采样率编码，16KHz-16000
        /// </summary>
        public int rate { get; set; } = 16000;

        /// <summary>
        /// 音频采样位数，16位-16
        /// </summary>
        public int bits { get; set; } = 16;

        /// <summary>
        /// 语音分片所在语音流的偏移量（字节）
        /// </summary>
        public int seq { get; set; } = 0;

        /// <summary>
        /// 语音分片长度（字节）
        /// </summary>
        public int len { get; set; }

        /// <summary>
        /// 是否结束分片标识，0-中间分片 1-结束分片
        /// </summary>
        public int end { get; set; } = 1;

        /// <summary>
        /// 语音唯一标识，非空且长度上限64B，同一应用内每段语音流标识需唯一，同一段语音流内的语音分片标识需一致 
        /// </summary>
        public string speech_id { get; set; }

        /// <summary>
        /// 语音分片数据的Base64编码，非空且时长上限90s，建议时长200-300ms
        /// </summary>
        public string speech_chunk { get; set; }

        /// <summary>
        /// 是否获取中间识别结果，0-不获取    1-获取
        /// </summary>
        public int cont_res { get; set; }
    }
}