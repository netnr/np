using Netnr.TencentAI.Model;
using System.ComponentModel;

namespace Netnr.TencentAI.fcgi_bin
{
    /// <summary>
    /// 辅助
    /// </summary>
    public class Aai
    {
        /// <summary>
        /// 敏感信息审核>音频鉴黄/敏感词检测
        /// </summary>
        /// <returns></returns>
        [Description("敏感信息审核>音频鉴黄/敏感词检测")]
        public static string Aai_EvilAudio(Aai_EvilAudioRequest request)
        {
            var uri = "https://api.ai.qq.com/fcgi-bin/aai/aai_evilaudio";
            return Aid.Request(request, uri, "GET");
        }

        /// <summary>
        /// 语音识别>语音识别-echo版
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Description("语音识别>语音识别-echo版")]
        public static string Aai_Asr(Aai_AsrRequest request)
        {
            var uri = "https://api.ai.qq.com/fcgi-bin/aai/aai_asr";
            return Aid.Request(request, uri);
        }

        /// <summary>
        /// 语音识别>语音识别-流式版（AI Lab）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Description("语音识别>语音识别-流式版（AI Lab）")]
        public static string Aai_Asrs(Aai_AsrsRequest request)
        {
            var uri = "https://api.ai.qq.com/fcgi-bin/aai/aai_asrs";
            return Aid.Request(request, uri);
        }

        /// <summary>
        /// 语音识别>流式版(WeChat AI)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Description("语音识别>流式版(WeChat AI)")]
        public static string Aai_WxAsrs(Aai_WxAsrsRequest request)
        {
            var uri = "https://api.ai.qq.com/fcgi-bin/aai/aai_wxasrs";
            return Aid.Request(request, uri);
        }

        /// <summary>
        /// 语音识别>长语音识别
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Description("语音识别>长语音识别")]
        public static string Aai_WxAsrLong(Aai_WxAsrLongRequest request)
        {
            var uri = "https://api.ai.qq.com/fcgi-bin/aai/aai_wxasrlong";
            return Aid.Request(request, uri);
        }

        /// <summary>
        /// 语音识别>关键词检索
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Description("语音识别>关键词检索")]
        public static string Aai_DetectKeyWord(Aai_DetectKeyWordRequest request)
        {
            var uri = "https://api.ai.qq.com/fcgi-bin/aai/aai_detectkeyword";
            return Aid.Request(request, uri);
        }

        /// <summary>
        /// 语音合成>语音合成（AI Lab）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Description("语音合成>语音合成（AI Lab）")]
        public static string Aai_Tts(Aai_TtsRequest request)
        {
            var uri = "https://api.ai.qq.com/fcgi-bin/aai/aai_tts";
            return Aid.Request(request, uri, "GET");
        }

        /// <summary>
        /// 语音合成>语音合成（优图）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Description("语音合成>语音合成（优图）")]
        public static string Aai_Tta(Aai_TtaRequest request)
        {
            var uri = "https://api.ai.qq.com/fcgi-bin/aai/aai_tta";
            return Aid.Request(request, uri, "GET");
        }
    }
}