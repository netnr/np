using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

using Netease.Cloud.NOS.Util;

namespace Netease.Cloud.NOS.Service
{
    internal class ServiceClientImpl : ServiceClient
    {
        #region Constructors

        public ServiceClientImpl(ClientConfiguration clientConfiguration)
            : base(clientConfiguration)
        { }

        #endregion

        #region ResponseImpl

        /// <summary>
        /// The response data of<see cref="ServiceClientImpl"/> requests.
        /// </summary>
        private class ResponseImpl : ServiceReponse
        {
            private HttpWebResponse _response;
            private readonly Exception _failure;
            private IDictionary<string, string> _headers;
            private bool _disposed;

            public override HttpStatusCode HttpStatusCode
            {
                get { return _response.StatusCode; }
            }

            public override Exception Failure
            {
                get { return _failure; }
            }

            public override IDictionary<string, string> Headers
            {
                get
                {
                    ThrowIfObjectDisposed();
                    return _headers ?? (_headers = GetResponseHeasers(_response));
                }
            }

            public override Stream Content
            {
                get
                {
                    ThrowIfObjectDisposed();

                    try
                    {
                        return (_response != null) ? _response.GetResponseStream() : null;
                    }
                    catch (ProtocolViolationException ex)
                    {
                        throw new InvalidOperationException(ex.Message, ex);
                    }
                }
            }

            public ResponseImpl(HttpWebResponse HttpWebResponse)
            {
                _response = HttpWebResponse;
            }

            public ResponseImpl(WebException failure)
            {
                var HttpWebResponse = failure.Response as HttpWebResponse;
                _failure = failure;
                _response = HttpWebResponse;
            }

            private static IDictionary<string, string> GetResponseHeasers(HttpWebResponse response)
            {
                var headers = response.Headers;
                var result = new Dictionary<string, string>(headers.Count);

                for (var i = 0; i < headers.Count; i++)
                {
                    var key = headers.Keys[i];
                    var value = headers.Get(key);
                    result.Add(key, HttpUtils.Reencode(value, "iso-8859-1", "utf-8"));
                }

                return result;
            }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
                if (_disposed)
                    return;


                if (disposing)
                {
                    if (_response != null)
                    {
                        _response.Close();
                        _response = null;
                    }
                    _disposed = true;
                }
            }

            private void ThrowIfObjectDisposed()
            {
                if (_disposed)
                    throw new ObjectDisposedException(GetType().Name);
            }
        }

        #endregion

        #region Implementations

        protected override ServiceReponse SendCore(ServiceRequest serviceRequest, ExecutionContext context)
        {
            var request = HttpFactory.CreateWebRequest(serviceRequest, ClientConfiguration);
            SetRequestContent(request, serviceRequest);
            try
            {
                var response = request.GetResponse() as HttpWebResponse;
                return new ResponseImpl(response);
            }
            catch (WebException ex)
            {
                return HandleException(ex);
            }

        }

        private static void SetRequestContent(HttpWebRequest httpWebRequest, ServiceRequest serviceRequest)
        {
            var data = serviceRequest.CreateRequestContent();

            if (data == null ||
                serviceRequest.Method != HttpMethod.PUT &&
                serviceRequest.Method != HttpMethod.POST)
            {
                return;
            }

            long userSetContentLength = -1;
            if (serviceRequest.Headers.ContainsKey(Headers.CONTENT_LENGTH))
                userSetContentLength = long.Parse(serviceRequest.Headers[Headers.CONTENT_LENGTH]);

            long streamLength = data.Length - data.Position;
            httpWebRequest.ContentLength = (userSetContentLength >= 0 && userSetContentLength <= streamLength) ? userSetContentLength : streamLength;        
            using (var requestStream = httpWebRequest.GetRequestStream())
            {
                IoUtils.WriteTo(data, requestStream, httpWebRequest.ContentLength);
            }
        }

        private static ServiceReponse HandleException(WebException ex)
        {
            var response = ex.Response as HttpWebResponse;
            if (response == null)
                throw ex;
            else
                return new ResponseImpl(ex);
        }

        #endregion
    }
}
