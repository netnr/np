namespace Netnr.ResponseFramework.Domain.Models
{
    /// <summary>
    /// 表单组件参数项
    /// </summary>
    public class InvokeFormVM
    {
        /// <summary>
        /// 渲染视图名称，默认Default
        /// </summary>
        public string ViewName { get; set; } = "Default";
        /// <summary>
        /// 表配置表名（虚表）
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 多面板选项卡标题（有两个区域或以上时，英文逗号分隔）
        /// </summary>
        public string PanelTitle { get; set; }
        /// <summary>
        /// 单据标题
        /// </summary>
        public string InvoiceTitle { get; set; } = "单据标题";
        /// <summary>
        /// 模态框大小 1小 2中 3大
        /// </summary>
        public int ModalSize { get; set; } = 3;
        /// <summary>
        /// 模态框大小转换样式名
        /// </summary>
        public string MsClass
        {
            get
            {
                return ModalSize == 3 ? "modal-lg" : ModalSize == 1 ? "modal-sm" : "";
            }
        }
        /// <summary>
        /// 表单ID后缀，单页面多表单，需要累加
        /// </summary>
        public int Index { get; set; } = 1;
        /// <summary>
        /// 输出的数据
        /// </summary>
        public object Data { get; set; }
    }
}
