using System;
using System.Collections.Generic;
using System.Diagnostics;

using Netease.Cloud.NOS.Handlers;
using Netease.Cloud.NOS.Util;

namespace Netease.Cloud.NOS.Service
{
    internal abstract class ServiceClient : IServiceClient
    {
        #region Fields and Properties

        private readonly ClientConfiguration _clientConfiguration;

        internal ClientConfiguration ClientConfiguration
        {
            get { return _clientConfiguration; }
        }

        #endregion

        #region Constructors

        protected ServiceClient(ClientConfiguration clientConfiguration)
        {
            this._clientConfiguration = clientConfiguration;
        }

        public static ServiceClient Create(ClientConfiguration clientConfiguration)
        {
            return new ServiceClientImpl(clientConfiguration);
        }

        #endregion

        public ServiceReponse Send(ServiceRequest request, ExecutionContext context)
        {
            SignRequest(request, context);
            var response = SendCore(request, context);
            HandleResponse(response, context.ResponseHandlers);
            return response;

        }

        private static void SignRequest(ServiceRequest request, ExecutionContext context)
        {
            if (context.Signer != null)
                context.Signer.Sign(request, context.Credentials);
        }

        protected abstract ServiceReponse SendCore(ServiceRequest request, ExecutionContext context);

        protected static void HandleResponse(ServiceReponse response, IEnumerable<IResponseHandler> responseHandlers)
        {
            foreach (var handler in responseHandlers)
                handler.Handle(response);
        }
    }
}