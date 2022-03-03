namespace Netnr.FileServer.Model
{
    /// <summary>
    /// 固定Token JSON实体
    /// </summary>
    public class FixedTokenJson
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Token
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// 授权方法名
        /// </summary>
        public string AuthMethod { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 所属用户
        /// </summary>
        [JsonIgnore]
        public string Owner { get; set; }
    }
}
