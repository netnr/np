using System;
using System.Net;
using System.Diagnostics;

namespace Netease.Cloud.NOS.Service
{
    internal static class ServiceClientFactory
    {
        static ServiceClientFactory()
        {
            //ServicePointManager.Expect100Continue = true;
            //ServicePointManager.DefaultConnectionLimit = ClientConfiguration.ConnectionLimit;
        }

        public static IServiceClient CreateSreviceClient(ClientConfiguration clientConfiguration)
        {
            Debug.Assert(clientConfiguration != null);

            var retryableServiceClient = new RetryableServiceClient(ServiceClient.Create(clientConfiguration))
            {
                MaxRetryTimes = clientConfiguration.MaxErrorRetry
            };

            return retryableServiceClient;
        }
    }
}
