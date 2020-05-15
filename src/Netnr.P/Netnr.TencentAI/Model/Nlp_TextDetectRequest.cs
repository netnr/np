namespace Netnr.TencentAI.Model
{
    /// <summary>
    /// 机器翻译>语种识别
    /// </summary>
    public class Nlp_TextDetectRequest : BaseRequest
    {
        /// <summary>
        /// UTF-8编码，非空且长度上限1024字节
        /// </summary>
        [Required]
        public string text { get; set; }

        /// <summary>
        /// 语言缩写，多种语言间用“|”分隔，中文-zh  英文-en   日文-jp   韩文-kr
        /// </summary>
        [Required]
        public string candidate_langs { get; set; }

        /// <summary>
        /// 是否强制从候选语言中选择（只对二选一有效），取值 0/1
        /// </summary>
        [Required]
        public int force { get; set; } = 0;
    }
}