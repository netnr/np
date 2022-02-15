using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Netease.Cloud.NOS
{
    [Serializable]
    public class ServiceException : Exception
    {
        /// <summary>
        ///  获取NOS服务器定义错误代码
        /// </summary>
        public virtual string ErrorCode { get; internal set; }

        /// <summary>
        /// 获取请求ID
        /// </summary>
        public virtual string RequestId { get; internal set; }

        /// <summary>
        /// 获取Resource
        /// </summary>
        public virtual string Resource { get; internal set; }

        /// <summary>
        /// 获取HTTP Status Code
        /// </summary>
        public virtual int StatusCode { get; internal set; }

        /// <summary>
        /// <see cref="ServiceException"/>构造函数
        /// </summary>
        public ServiceException() { }

        /// <summary>
        /// <see cref="ServiceException"/>构造函数
        /// </summary>
        /// <param name="message">错误信息</param>
        public ServiceException(string message)
            : base(message) { }

        /// <summary>
        /// <see cref="ServiceException"/>构造函数
        /// </summary>
        /// <param name="message">错误信息</param>
        /// <param name="exception">导致当前异常的异常</param>
        public ServiceException(string message, Exception exception)
            : base(message, exception)
        { }

        /// <summary>
        /// <see cref="ServiceException"/>构造函数
        /// </summary>
        /// <param name="info">序列化对象数据的对象</param>
        /// <param name="context">有关源和目标的上下文信息</param>
        protected ServiceException(SerializationInfo info, StreamingContext context)
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
