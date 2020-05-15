namespace Netnr.Chat.Application.ViewModel
{
    public enum MessageType
    {
        /// <summary>
        /// 文本
        /// </summary>
        Text,
        /// <summary>
        /// 图片
        /// </summary>
        Image,
        /// <summary>
        /// 语音
        /// </summary>
        Voice,
        /// <summary>
        /// 视频
        /// </summary>
        Video,
        /// <summary>
        /// 链接
        /// </summary>
        Link,
        /// <summary>
        /// 新建组
        /// </summary>
        EventNewGroup,
        /// <summary>
        /// 加入组
        /// </summary>
        EventJoinGroup,
        /// <summary>
        /// 离开组
        /// </summary>
        EventLeaveGroup,
        /// <summary>
        /// 删除组
        /// </summary>
        EventDelGroup
    }
}
