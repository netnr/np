namespace Netnr.TencentAI.Model
{
    /// <summary>
    /// 机器翻译>图片翻译
    /// </summary>
    public class Nlp_ImageTranslateRequest : BaseRequest
    {
        /// <summary>
        /// 原始图片的base64编码数据（原图大小上限1MB）
        /// </summary>
        [Required]
        public string image { get; set; }

        /// <summary>
        /// 一次请求ID（尽可能唯一，长度上限64字节）
        /// </summary>
        [Required]
        public string session_id { get; set; } = System.Guid.NewGuid().ToString("N");

        /// <summary>
        /// 识别类型，取值word/doc （word-单词识别，doc-文档识别）
        /// </summary>
        [Required]
        public string scene { get; set; }

        /// <summary>
        /// 源语言缩写    中文-zh   英文-en   日文-jp   韩文-kr   自动识别（中英互译）-auto
        /// </summary>
        [Required]
        public string source { get; set; }

        /// <summary>
        /// 目标语言缩写，en-zh    zh-en,jp,kr    jp-zh   kr-zh
        /// </summary>
        [Required]
        public string target { get; set; }
    }
}