namespace Netnr.TencentAI.Sample
{
    public class Aai
    {
        /// <summary>
        /// 敏感信息审核>音频鉴黄/敏感词检测
        /// </summary>
        /// <returns></returns>
        public static string Aai_EvilAudio()
        {
            Model.Aai_EvilAudioRequest mo = new Model.Aai_EvilAudioRequest
            {
                speech_url = "http://xmdx.sc.chinaz.com/files/download/sound1/201811/10786.wav"
            };

            string result = fcgi_bin.Aai.Aai_EvilAudio(mo);
            return result;
            //return "待测试，报签名错误，暂时没找到原因";
        }

        /// <summary>
        /// 语音识别>语音识别-echo版
        /// </summary>
        /// <returns></returns>
        public static string Aai_Asr()
        {
            return "待测试";
        }

        /// <summary>
        /// 语音识别>语音识别-流式版（AI Lab）
        /// </summary>
        /// <returns></returns>
        public static string Aai_Asrs()
        {
            return "待测试";
        }

        /// <summary>
        /// 语音识别>流式版(WeChat AI)
        /// </summary>
        /// <returns></returns>
        public static string Aai_WxAsrs()
        {
            return "待测试";
        }

        /// <summary>
        /// 语音识别>长语音识别
        /// </summary>
        /// <returns></returns>
        public static string Aai_WxAsrLong()
        {
            return "待测试";
        }

        /// <summary>
        /// 语音识别>关键词检索
        /// </summary>
        /// <returns></returns>
        public static string Aai_DetectKeyWord()
        {
            return "待测试";
        }

        /// <summary>
        /// 语音合成>语音合成（AI Lab）
        /// </summary>
        /// <returns></returns>
        public static string Aai_Tts()
        {
            Model.Aai_TtsRequest mo = new Model.Aai_TtsRequest
            {
                text = "Hi，我是netnr"
            };

            string result = fcgi_bin.Aai.Aai_Tts(mo);
            return result;
        }

        /// <summary>
        /// 语音合成>语音合成（优图）
        /// </summary>
        /// <returns></returns>
        public static string Aai_Tta()
        {
            Model.Aai_TtaRequest mo = new Model.Aai_TtaRequest
            {
                text = "Hi，我是netnr"
            };

            string result = fcgi_bin.Aai.Aai_Tta(mo);
            return result;
        }
    }
}