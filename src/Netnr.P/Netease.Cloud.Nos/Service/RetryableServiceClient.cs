using System;
using System.Net;
using System.IO;
using System.Threading;

using Netease.Cloud.NOS.Util;

namespace Netease.Cloud.NOS.Service
{
    internal class RetryableServiceClient : IServiceClient
    {
        #region Fields and Properties

        private const int DEFAULT_MAX_RETRY_TIMES = 3;
        private const int DEFAULT_RETRY_PAUSE_SCALE = 300;

        private readonly IServiceClient _innerClient;

        public delegate TResult NosFunc<T, TResult>(T t);

        public NosFunc<Exception, bool> ShouldRetryCallback { get; set; }

        public int MaxRetryTimes { get; set; }

        #endregion

        #region Constructors

        public RetryableServiceClient(IServiceClient innerClient)
        {
            _innerClient = innerClient;
            MaxRetryTimes = DEFAULT_MAX_RETRY_TIMES;
        }

        #endregion

        internal IServiceClient InnerServiceClient()
        {
            return _innerClient;
        }

        public ServiceReponse Send(ServiceRequest serviceRequest, ExecutionContext executionContext)
        {
            return SendImpl(serviceRequest, executionContext, 0);
        }

        private ServiceReponse SendImpl(ServiceRequest serviceRequest, ExecutionContext executionContext, int retryTimes)
        {
            long originalContentPosition = -1;
            try
            {
                if (serviceRequest.Content != null && serviceRequest.Content.CanSeek)
                    originalContentPosition = serviceRequest.Content.Position;
                return _innerClient.Send(serviceRequest, executionContext);
            }
            catch (Exception ex)
            {
                if (ShouldRetry(serviceRequest, ex, retryTimes))
                {
                    if (serviceRequest.Content != null && (originalContentPosition >= 0 && serviceRequest.Content.CanSeek))
                        serviceRequest.Content.Seek(originalContentPosition, SeekOrigin.Begin);

                    Pause(retryTimes);

                    return SendImpl(serviceRequest, executionContext, ++retryTimes);
                }

                throw;
            }
        }

        private bool ShouldRetry(ServiceRequest serviceRequest, Exception exception, int retryTimes)
        {
            if (retryTimes > MaxRetryTimes || serviceRequest.IsRequestRepeatable)
                return false;

            var webException = exception as WebException;
            if (webException != null)
            {
                var httpWebResponse = webException.Response as HttpWebResponse;
                if (httpWebResponse != null &&
                    (httpWebResponse.StatusCode == HttpStatusCode.ServiceUnavailable ||
                    httpWebResponse.StatusCode == HttpStatusCode.InternalServerError))
                    return true;
            }

            if (ShouldRetryCallback != null && ShouldRetryCallback(exception))
                return true;

            return false;
        }

        private static void Pause(int retryTimes)
        {
            var delay = (int)Math.Pow(2, retryTimes) * DEFAULT_RETRY_PAUSE_SCALE;
            Thread.Sleep(delay);
        }
    }
}
