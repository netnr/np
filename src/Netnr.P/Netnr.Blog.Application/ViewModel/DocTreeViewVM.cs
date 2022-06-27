namespace Netnr.Blog.Application.ViewModel
{
    /// <summary>
    /// 文档树形结构视图
    /// </summary>
    public class DocTreeVM
    {
        /// <summary>
        /// 文档页ID
        /// </summary>
        public string DsdId { get; set; }
        /// <summary>
        /// 父ID
        /// </summary>
        public string DsdPid { get; set; }
        /// <summary>
        /// 文档主码
        /// </summary>
        public string DsCode { get; set; }
        /// <summary>
        /// 文档页标题
        /// </summary>
        public string DsdTitle { get; set; }
        /// <summary>
        /// 文档页排序
        /// </summary>
        public int? DsdOrder { get; set; }
        /// <summary>
        /// 是目录
        /// </summary>
        public bool IsCatalog { get; set; }
    }
}
