namespace Netnr.TencentAI.Model
{
    /// <summary>
    /// 请求公共参数
    /// </summary>
    public class BaseRequest
    {
        /// <summary>
        /// 【公共参数】应用标识（AppId）
        /// </summary>
        [Required]
        public int app_id { get; set; }

        /// <summary>
        /// 【公共参数】请求时间戳（秒级）
        /// </summary>
        [Required]
        public int time_stamp { get; set; }

        /// <summary>
        /// 【公共参数】非空且长度上限32字节 随机字符串
        /// </summary>
        [Required]
        public string nonce_str { get; set; }

        /// <summary>
        /// 【公共参数】非空且长度固定32字节 签名信息，详见接口鉴权
        /// </summary>
        [Required]
        public string sign { get; set; }
    }
}