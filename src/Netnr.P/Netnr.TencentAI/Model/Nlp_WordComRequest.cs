namespace Netnr.TencentAI.Model
{
    /// <summary>
    /// 语义解析>意图成分
    /// </summary>
    public class Nlp_WordComRequest : BaseRequest
    {
        /// <summary>
        /// UTF-8编码，非空且长度上限100字节
        /// </summary>
        [Required]
        public string text { get; set; }
    }
}