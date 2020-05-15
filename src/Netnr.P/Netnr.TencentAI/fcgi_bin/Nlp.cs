using Netnr.TencentAI.Model;
using System.ComponentModel;

namespace Netnr.TencentAI.fcgi_bin
{
    /// <summary>
    /// 
    /// </summary>
    public class Nlp
    {
        /// <summary>
        /// 智能闲聊>智能闲聊
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Description("智能闲聊>智能闲聊")]
        public static string Nlp_TextChat(Nlp_TextChatRequest request)
        {
            var uri = "https://api.ai.qq.com/fcgi-bin/nlp/nlp_textchat";
            return Aid.Request(request, uri, "GET");
        }

        /// <summary>
        /// 机器翻译>文本翻译（AI Lab）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Description("机器翻译>文本翻译（AI Lab）")]
        public static string Nlp_TextTrans(Nlp_TextTransRequest request)
        {
            var uri = "https://api.ai.qq.com/fcgi-bin/nlp/nlp_texttrans";
            return Aid.Request(request, uri, "GET");
        }

        /// <summary>
        /// 机器翻译>文本翻译（翻译君）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Description("机器翻译>文本翻译（翻译君）")]
        public static string Nlp_TextTranslate(Nlp_TextTranslateRequest request)
        {
            var uri = "https://api.ai.qq.com/fcgi-bin/nlp/nlp_texttranslate";
            return Aid.Request(request, uri, "GET");
        }

        /// <summary>
        /// 机器翻译>语音翻译
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Description("机器翻译>语音翻译")]
        public static string Nlp_SpeechTranslate(Nlp_SpeechTranslateRequest request)
        {
            var uri = "https://api.ai.qq.com/fcgi-bin/nlp/nlp_speechtranslate";
            return Aid.Request(request, uri);
        }

        /// <summary>
        /// 机器翻译>图片翻译
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Description("机器翻译>图片翻译")]
        public static string Nlp_ImageTranslate(Nlp_ImageTranslateRequest request)
        {
            var uri = "https://api.ai.qq.com/fcgi-bin/nlp/nlp_imagetranslate";
            return Aid.Request(request, uri);
        }

        /// <summary>
        /// 机器翻译>语种识别
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Description("机器翻译>语种识别")]
        public static string Nlp_TextDetect(Nlp_TextDetectRequest request)
        {
            var uri = "https://api.ai.qq.com/fcgi-bin/nlp/nlp_textdetect";
            return Aid.Request(request, uri);
        }

        /// <summary>
        /// 基础文本分析>分词
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Description("基础文本分析>分词")]
        public static string Nlp_WordSeg(Nlp_WordSegRequest request)
        {
            var uri = "https://api.ai.qq.com/fcgi-bin/nlp/nlp_wordseg";
            return Aid.Request(request, uri, "GET", "GBK");
        }

        /// <summary>
        /// 基础文本分析>词性
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Description("基础文本分析>词性")]
        public static string Nlp_WordPos(Nlp_WordPosRequest request)
        {
            var uri = "https://api.ai.qq.com/fcgi-bin/nlp/nlp_wordpos";
            return Aid.Request(request, uri, "GET", "GBK");
        }

        /// <summary>
        /// 基础文本分析>专有名词
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Description("基础文本分析>专有名词")]
        public static string Nlp_WordNer(Nlp_WordNerRequest request)
        {
            var uri = "https://api.ai.qq.com/fcgi-bin/nlp/nlp_wordner";
            return Aid.Request(request, uri, "GET", "GBK");
        }

        /// <summary>
        /// 基础文本分析>同义词
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Description("基础文本分析>同义词")]
        public static string Nlp_WordSyn(Nlp_WordSynRequest request)
        {
            var uri = "https://api.ai.qq.com/fcgi-bin/nlp/nlp_wordsyn";
            return Aid.Request(request, uri, "GET", "GBK");
        }

        /// <summary>
        /// 语义解析>意图成分
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Description("语义解析>意图成分")]
        public static string Nlp_WordCom(Nlp_WordComRequest request)
        {
            var uri = "https://api.ai.qq.com/fcgi-bin/nlp/nlp_wordcom";
            return Aid.Request(request, uri, "GET");
        }

        /// <summary>
        /// 语义解析>情感分析
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Description("语义解析>情感分析")]
        public static string Nlp_TextPolar(Nlp_TextPolarRequest request)
        {
            var uri = "https://api.ai.qq.com/fcgi-bin/nlp/nlp_textpolar";
            return Aid.Request(request, uri, "GET");
        }
    }
}