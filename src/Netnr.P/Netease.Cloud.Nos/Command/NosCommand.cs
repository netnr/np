using System;
using System.Collections.Generic;
using System.IO;

using Netease.Cloud.NOS.Util;
using Netease.Cloud.NOS.Service;
using Netease.Cloud.NOS.Transform;

namespace Netease.Cloud.NOS.Commands
{
    internal abstract class NosCommand
    {
        protected ExecutionContext ExecutionContext { get; set; }

        private IServiceClient serviceClient { get; set; }

        private Uri Endpoint { get; set; }

        protected virtual HttpMethod Method
        {
            get { return HttpMethod.GET; }
        }

        protected virtual string Bucket
        {
            get { return null; }
        }

        protected virtual string Key
        {
            get { return null; }
        }

        protected virtual IDictionary<string, string> Headers
        {
            get { return new Dictionary<string, string>(); }
        }

        protected virtual IDictionary<string, string> Parameters
        {
            get { return new Dictionary<string, string>(); }
        }

        protected virtual Stream Content
        {
            get { return null; }
        }

        protected virtual bool LeaveRequestOpen
        {
            get { return false; }
        }

        protected NosCommand(IServiceClient client, Uri endpoint, ExecutionContext context)
        {
            this.serviceClient = client;
            this.Endpoint = endpoint;
            this.ExecutionContext = context;
        }

        public ServiceReponse Execute()
        {
            var request = CreateRequest();
            try
            {
                return serviceClient.Send(request, ExecutionContext);
            }
            finally
            {
                if (!LeaveRequestOpen)
                    request.Dispose();
            }
        }

        private ServiceRequest CreateRequest()
        {
            var config = NosUtils.GetClientConfiguration(serviceClient);
            var request = new ServiceRequest
            {
                Method = Method,
                Endpoint = NosUtils.MakeEndpoint(Endpoint, Bucket, config),
                ResourcePath = NosUtils.MakeResourcePath(Endpoint, Bucket, Key)
            };

            foreach (var p in Parameters)
                request.Parameters.Add(p.Key, p.Value);

            var adjustedTime = DateTime.UtcNow.AddSeconds(config.TickOffset);
            request.Headers[Util.Headers.DATE] = DateUtils.FormatRfc822Date(adjustedTime);

            foreach (var h in Headers)
                request.Headers.Add(h.Key, h.Value);

            request.Content = Content;

            return request;
        }
    }

    internal abstract class NosCommand<T> : NosCommand
    {
        private readonly IDeserializer<ServiceReponse, T> _deserializer;
        protected virtual bool LeaveResponseOpen { get { return false; } }

        protected NosCommand(IServiceClient serviceClient, Uri endpoint, ExecutionContext executionContext, IDeserializer<ServiceReponse, T> deserializer)
            : base(serviceClient, endpoint, executionContext)
        {
            _deserializer = deserializer;
            ExecutionContext.Command = this;
        }

        public new T Execute()
        {
            var response = base.Execute();
            return DeserializeResponse(response);
        }

        public T DeserializeResponse(ServiceReponse response)
        {
            try
            {
                return _deserializer.Deserialize(response);
            }
            catch (ResponseDeserializationException e)
            {
                throw ExceptionFactory.CreateInvalidResponseException(e);
            }
            finally
            {
                if (!LeaveResponseOpen)
                    response.Dispose();
            }
        }
    }
}
