using System;
using System.Collections.Generic;

using Netease.Cloud.NOS.Service;
using Netease.Cloud.NOS.Util;
using Netease.Cloud.NOS.Model;
using Netease.Cloud.NOS.Transform;

namespace Netease.Cloud.NOS.Commands
{
    class GetObjectMetadataCommand : NosCommand<ObjectMetadata>
    {
        private readonly GetObjectMetadataRequest _getObjectMetadataRequest;

        protected override HttpMethod Method
        {
            get { return HttpMethod.HEAD; }
        }

        protected override string Bucket
        {
            get { return _getObjectMetadataRequest.Bucket; }
        }

        protected override string Key
        {
            get { return _getObjectMetadataRequest.Key; }
        }

        protected override IDictionary<string, string> Headers
        {
            get
            {
                var headers = new Dictionary<string, string>();
                _getObjectMetadataRequest.Populate(headers);
                return headers;
            }
        }

        private GetObjectMetadataCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                         IDeserializer<ServiceReponse, ObjectMetadata> deserializer,
                                         GetObjectMetadataRequest getObjectMetadataRequest)
            : base(client, endpoint, context, deserializer)
        {
            _getObjectMetadataRequest = getObjectMetadataRequest;
        }

        public static GetObjectMetadataCommand Create(IServiceClient client, Uri endpoint, ExecutionContext context,
                                                      GetObjectMetadataRequest getObjectMetadataRequest)
        {
            return new GetObjectMetadataCommand(client, endpoint, context,
                                                DeserializerFactory.GetFactory().CreateGetObjectMetadataResultDeserializer(),
                                                getObjectMetadataRequest);
        }
    }
}
