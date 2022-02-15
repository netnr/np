using System;
using System.Collections.Generic;

using Netease.Cloud.NOS.Service;
using Netease.Cloud.NOS.Util;
using Netease.Cloud.NOS.Model;
using Netease.Cloud.NOS.Transform;

namespace Netease.Cloud.NOS.Commands
{
    internal class GetObjectCommand : NosCommand<NosObject>
    {
        private readonly GetObjectRequest _getObjectRequest;

        protected override string Bucket
        {
            get { return _getObjectRequest.Bucket; }
        }

        protected override string Key
        {
            get { return _getObjectRequest.Key; }
        }

        protected override IDictionary<string, string> Headers
        {
            get
            {
                var headers = new Dictionary<string, string>();
                _getObjectRequest.Populate(headers);
                return headers;
            }
        }

        protected override IDictionary<string, string> Parameters
        {
            get
            {
                var parameters = base.Parameters;
                return parameters;
            }
        }

        protected override bool LeaveResponseOpen
        {
            get { return true; }
        }

        private GetObjectCommand(IServiceClient serviceClient, Uri endpoint, ExecutionContext executionContext,
                                 IDeserializer<ServiceReponse, NosObject> deserializer,
                                 GetObjectRequest getObjectRequest)
            : base(serviceClient, endpoint, executionContext, deserializer)
        {
            _getObjectRequest = getObjectRequest;
        }

        public static GetObjectCommand Create(IServiceClient serviceClient, Uri endpoint, ExecutionContext executionContext,
                                              GetObjectRequest getObjectRequest)
        {
            NosUtils.CheckBucketName(getObjectRequest.Bucket);
            NosUtils.CheckObjectName(getObjectRequest.Key);

            return new GetObjectCommand(serviceClient, endpoint, executionContext,
                                 DeserializerFactory.GetFactory().CreateGetObjectResultDeserializer(getObjectRequest),
                                 getObjectRequest);
        }
    }
}
