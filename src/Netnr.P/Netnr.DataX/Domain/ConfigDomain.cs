namespace Netnr.DataX.Domain
{
    /// <summary>
    /// 配置
    /// </summary>
    public class ConfigDomain
    {
        public string Version { get; set; }
        public string Console_Encoding { get; set; }
        public string MapingMatchPattern { get; set; }
        public List<SharedDataKit.TransferVM.ConnectionInfo> ListConnectionInfo { get; set; }
    }
}
