using System;
using System.Diagnostics;

using Netease.Cloud.NOS.Service;

namespace Netease.Cloud.NOS.Handlers
{
    internal class ResponseHander : IResponseHandler
    {
        public virtual void Handle(ServiceReponse response)
        {
            Debug.Assert(response != null);
        }
    }
}
