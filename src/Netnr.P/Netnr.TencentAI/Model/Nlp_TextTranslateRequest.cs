namespace Netnr.TencentAI.Model
{
    /// <summary>
    /// 机器翻译>文本翻译（翻译君）
    /// </summary>
    public class Nlp_TextTranslateRequest : BaseRequest
    {
        /// <summary>
        /// UTF-8编码，非空且长度上限1024字节
        /// </summary>
        [Required]
        public string text { get; set; }

        /// <summary>
        /// 源语言缩写
        /// 中文	zh
        /// 英文	en
        /// 日文	jp
        /// 韩文	kr
        /// 法文	fr
        /// 西班牙文	es
        /// 意大利文	it
        /// 德文	de
        /// 土耳其文	tr
        /// 俄文	ru
        /// 葡萄牙文	pt
        /// 越南文	vi
        /// 印度尼西亚文	id
        /// 马来西亚文	ms
        /// 泰文	th
        /// 自动识别（中英互译）	auto
        /// </summary>
        [Required]
        public string source { get; set; } = "auto";

        /// <summary>
        /// 目标语言缩写
        /// </summary>
        [Required]
        public string target { get; set; }
    }
}