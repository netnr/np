using System;
using System.Collections.Generic;

using Netease.Cloud.NOS.Authentication;
using Netease.Cloud.NOS.Handlers;
using Netease.Cloud.NOS.Commands;

namespace Netease.Cloud.NOS.Service
{
    internal class ExecutionContext
    {
        /// <summary>
        /// 响应处理程序
        /// </summary>
        private readonly IList<IResponseHandler> _responseHandlers = new List<IResponseHandler>();

        /// <summary>
        /// 获取或设置<see cref="ISigner"/>
        /// </summary>
        public ISigner Signer { get; set; }

        /// <summary>
        /// 获取或设置<see cref="ICredentials"/>
        /// </summary>
        public ICredentials Credentials { get; set; }

        /// <summary>
        /// 获取或设置<see cref="IResponseHandlers"/>列表
        /// </summary>
        public IList<IResponseHandler> ResponseHandlers
        {
            get { return _responseHandlers; }
        }

        /// <summary>
        /// 获取或设置<see cref="NosCommand"/>
        /// </summary>
        public NosCommand Command { get; set; }
    }
}
