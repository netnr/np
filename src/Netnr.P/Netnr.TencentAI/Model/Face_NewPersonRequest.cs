namespace Netnr.TencentAI.Model
{
    /// <summary>
    /// 人脸与人体识别>人脸搜索>个体创建
    /// </summary>
    public class Face_NewPersonRequest : BaseRequest
    {
        /// <summary>
        /// 组（Group），一个组（Group）里面的个体（Person）总数上限为20000个，如果ID指定的组不存在，则会新建组并创建个体。
        /// </summary>
        [Required]
        public string group_ids { get; set; }

        /// <summary>
        /// 指定的个体（Person）ID
        /// </summary>
        [Required]
        public string person_id { get; set; }

        /// <summary>
        /// 个体图片,原始图片的base64编码数据（原图大小上限1MB，支持JPG、PNG、BMP格式）
        /// </summary>
        [Required]
        public string image { get; set; }

        /// <summary>
        /// 名字
        /// </summary>
        [Required]
        public string person_name { get; set; }

        /// <summary>
        /// 备注信息
        /// </summary>
        public string tag { get; set; }
    }
}
