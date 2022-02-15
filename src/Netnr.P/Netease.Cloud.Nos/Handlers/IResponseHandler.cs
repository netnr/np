using System;

using Netease.Cloud.NOS.Service;

namespace Netease.Cloud.NOS.Handlers
{
    internal interface IResponseHandler
    {
        void Handle(ServiceReponse response);
    }
}