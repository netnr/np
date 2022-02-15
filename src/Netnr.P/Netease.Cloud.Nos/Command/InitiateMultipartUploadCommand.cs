using System;
using System.IO;
using System.Collections.Generic;

using Netease.Cloud.NOS.Model;
using Netease.Cloud.NOS.Util;
using Netease.Cloud.NOS.Service;
using Netease.Cloud.NOS.Transform;

namespace Netease.Cloud.NOS.Commands
{
    internal class InitiateMultipartUploadCommand : NosCommand<InitiateMultipartUploadResult>
    {
        private readonly InitiateMultipartUploadRequest _initiateMultipartUploadRequest;

        protected override string Bucket
        {
            get { return _initiateMultipartUploadRequest.Bucket; }
        }

        protected override string Key
        {
            get { return _initiateMultipartUploadRequest.Key; }
        }

        protected override HttpMethod Method
        {
            get { return HttpMethod.POST; }
        }

        protected override IDictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { RequestParameters.PARAMETER_UPLOADS, null }
                };
            }
        }

        protected override Stream Content
        {
            get { return new MemoryStream(new byte[0]); }
        }

        protected override IDictionary<string, string> Headers
        {
            get
            {
                var headers = new Dictionary<string, string>();
                if (_initiateMultipartUploadRequest.ObjectMetadata != null)
                    _initiateMultipartUploadRequest.ObjectMetadata.Populate(headers);
                return headers;
            }
        }

        private InitiateMultipartUploadCommand(IServiceClient serviceClient, Uri endpoint, ExecutionContext executionContext,
                                               IDeserializer<ServiceReponse, InitiateMultipartUploadResult> deserializeMethod,
                                               InitiateMultipartUploadRequest initiateMultipartUploadRequest)
            : base(serviceClient, endpoint, executionContext, deserializeMethod)
        {
            _initiateMultipartUploadRequest = initiateMultipartUploadRequest;
        }

        public static InitiateMultipartUploadCommand Create(IServiceClient serviceClient, Uri endpoint, ExecutionContext executionContext,
                                                            InitiateMultipartUploadRequest initiateMultipartUploadRequest)
        {
            NosUtils.CheckBucketName(initiateMultipartUploadRequest.Bucket);
            NosUtils.CheckObjectName(initiateMultipartUploadRequest.Key);

            return new InitiateMultipartUploadCommand(serviceClient, endpoint, executionContext,
                                        DeserializerFactory.GetFactory().CreateInitiateMultipartUploadResultDeserializer(),
                                        initiateMultipartUploadRequest);
        }
    }
}
