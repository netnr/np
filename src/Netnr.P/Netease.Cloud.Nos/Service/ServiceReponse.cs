using System;
using System.Net;
using System.Diagnostics;

namespace Netease.Cloud.NOS.Service
{
    internal abstract class ServiceReponse : ServiceMessage, IDisposable    
    {
        /// <summary>
        /// 获取响应状态码
        /// </summary>
        public abstract HttpStatusCode HttpStatusCode { get; }

        /// <summary>
        /// 获取异常信息
        /// </summary>
        public abstract Exception Failure { get; }

        /// <summary>
        /// 请求是否成功
        /// </summary>
        /// <returns>布尔值</returns>
        public virtual bool IsSuccessful()
        {
            return ((int)HttpStatusCode / 100 == (int)HttpStatusCode.OK / 100) || ((int)HttpStatusCode / 100 == 3);
        }

        public virtual void EnsureSuccessful()
        {
            if (!IsSuccessful())
            {
                if (Content != null)
                {
                    Content.Dispose();
                }

                Debug.Assert(Failure != null);
                throw Failure;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        { }
    }
}
