using System;

namespace Netease.Cloud.NOS.Service
{
    internal interface IServiceClient
    {
        /// <summary>
        /// 发送一个请求
        /// </summary>
        /// <param name="request"><see cref="ServiceRequest"/>实例</param>
        /// <param name="context"><see cref="ExectionContext"/>实例</param>
        /// <returns><see cref="ServiceResponse"/>实例</returns>
        ServiceReponse Send(ServiceRequest request, ExecutionContext context);
    }
}
