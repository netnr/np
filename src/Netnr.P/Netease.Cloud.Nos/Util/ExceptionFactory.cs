using System;

namespace Netease.Cloud.NOS.Util
{
    internal static class ExceptionFactory
    {
        public static NosException CreateException(int statusCode, string errorCode, string message, string resource, string requestId, Exception exception)
        {
            var ex = exception != null ?
                new NosException(message, exception) :
                new NosException(message);

            ex.ErrorCode = errorCode;
            ex.Resource = resource;
            ex.RequestId = requestId;
            ex.StatusCode = statusCode;

            return ex;
        }

        public static NosException CreateException(int statusCode,string errorCode, string message, string resource, string requestId)
        {
            return CreateException(statusCode, errorCode, message, resource, requestId, null);
        }

        public static Exception CreateInvalidResponseException(Exception exception)
        {
            throw new InvalidOperationException("Exception Invalid Response", exception);
        }
    }
}
