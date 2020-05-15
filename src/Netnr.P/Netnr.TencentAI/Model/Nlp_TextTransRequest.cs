namespace Netnr.TencentAI.Model
{
    /// <summary>
    /// 机器翻译>文本翻译（AI Lab）
    /// </summary>
    public class Nlp_TextTransRequest : BaseRequest
    {
        /// <summary>
        /// 翻译类型，默认为0，
        /// 0	自动识别（中英文互转）
        /// 1	中文翻译成英文
        /// 2	英文翻译成中文
        /// 3	中文翻译成西班牙文
        /// 4	西班牙文翻译成中文
        /// 5	中文翻译成法文
        /// 6	法文翻译成中文
        /// 7	英文翻译成越南语
        /// 8	越南语翻译成英文
        /// 9	中文翻译成粤语
        /// 10	粤语翻译成中文
        /// 11	中文翻译成韩文
        /// 13	英文翻译成德语
        /// 14	德语翻译成英文
        /// 15	中文翻译成日文
        /// 16	日文翻译成中文
        /// </summary>
        [Required]
        public int type { get; set; } = 0;

        /// <summary>
        /// UTF-8编码，非空且长度上限1024字节
        /// </summary>
        [Required]
        public string text { get; set; }
    }
}