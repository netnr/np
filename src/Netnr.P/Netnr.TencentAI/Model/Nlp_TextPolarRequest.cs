namespace Netnr.TencentAI.Model
{
    /// <summary>
    /// 语义解析>情感分析
    /// </summary>
    public class Nlp_TextPolarRequest : BaseRequest
    {
        /// <summary>
        /// UTF-8编码，非空且长度上限200字节
        /// </summary>
        [Required]
        public string text { get; set; }
    }
}