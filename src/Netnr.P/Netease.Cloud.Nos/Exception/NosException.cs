using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Netease.Cloud.NOS
{
    /// <summary>
    /// 该异常在访问失败时抛出
    /// </summary>
    [Serializable]
    public class NosException : ServiceException
    {
        /// <summary>
        /// <see cref="NosException"/>构造函数
        /// </summary>
        public NosException() { }

        /// <summary>
        /// <see cref="NosException"/>构造函数
        /// </summary>
        /// <param name="message">错误信息</param>
        public NosException(string message)
            : base(message) { }

        /// <summary>
        /// <see cref="NosException"/>构造函数
        /// </summary>
        /// <param name="message">错误信息</param>
        /// <param name="exception">导致当前异常的异常</param>
        public NosException(string message, Exception exception)
            : base(message, exception)
        { }

        /// <summary>
        /// <see cref="NosException"/>构造函数
        /// </summary>
        /// <param name="info">有关引发异常的序列化的对象数据</param>
        /// <param name="context">有关源和目标的上下文信息</param>
        protected NosException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        /// <summary>
        /// 重载<see cref="ISerializable.GetObjectData"/>方法
        /// </summary>
        /// <param name="info">有关引发异常的序列化的对象数据</param>
        /// <param name="context">有关源和目标的上下文信息</param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}
