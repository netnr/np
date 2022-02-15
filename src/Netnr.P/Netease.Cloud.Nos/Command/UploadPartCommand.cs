using System;
using System.IO;
using System.Collections.Generic;

using Netease.Cloud.NOS.Transform;
using Netease.Cloud.NOS.Service;
using Netease.Cloud.NOS.Util;
using Netease.Cloud.NOS.Model;

namespace Netease.Cloud.NOS.Commands
{
    internal class UploadPartCommand : NosCommand<UploadPartResult>
    {
        private readonly UploadPartRequest _uploadPartRequest;

        protected override HttpMethod Method
        {
            get { return HttpMethod.PUT; }
        }

        protected override string Bucket
        {
            get { return _uploadPartRequest.Bucket; }
        }

        protected override string Key
        {
            get { return _uploadPartRequest.Key; }
        }

        protected override IDictionary<string, string> Parameters
        {
            get
            {
                var parameters = base.Parameters;
                parameters[RequestParameters.PARAMETER_PARTNUMBER] = _uploadPartRequest.PartNumber.ToString();
                parameters[RequestParameters.PARAMETER_UPLOADID] = _uploadPartRequest.UploadId;
                return parameters;
            }
        }

        protected override IDictionary<string, string> Headers
        {
            get
            {
                var headers = base.Headers;
                headers[Util.Headers.CONTENT_LENGTH] = _uploadPartRequest.PartSize.ToString();
                return headers;
            }
        }

        protected override Stream Content
        {
            get { return _uploadPartRequest.Content; }
        }

        protected override bool LeaveRequestOpen
        {
            get { return true; }
        }

        private UploadPartCommand(IServiceClient serviceClient, Uri endpoint, ExecutionContext executionContext,
                                 IDeserializer<ServiceReponse, UploadPartResult> deserializer,
                                                UploadPartRequest uploadPartRequest)
            : base(serviceClient, endpoint, executionContext, deserializer)
        {
            _uploadPartRequest = uploadPartRequest;
        }

        public static UploadPartCommand Create(IServiceClient serviceClient, Uri endpoint, ExecutionContext executionContext,
                                               UploadPartRequest uploadPartRequest)
        {
            NosUtils.CheckBucketName(uploadPartRequest.Bucket);
            NosUtils.CheckObjectName(uploadPartRequest.Key);

            if (string.IsNullOrEmpty(uploadPartRequest.UploadId))
                throw new ArgumentException("The uploadId of part should be specified");
            if (!uploadPartRequest.PartNumber.HasValue)
                throw new ArgumentException("The partNumber of part should be specified");
            if (Convert.ToString(uploadPartRequest.PartSize) == "")
                throw new ArgumentException(" The partSize of part should be specified");
            if (uploadPartRequest.Content == null)
                throw new ArgumentException("The content of part should be specified");

            if (!NosUtils.IsPartSizeInRange(uploadPartRequest.PartSize))
                throw new ArgumentException("The partSize not live in valid range");
            if (!NosUtils.IsPartNumberInRange(uploadPartRequest.PartNumber))
                throw new ArgumentException("The partNumber not live in valid range");

            return new UploadPartCommand(serviceClient, endpoint, executionContext,
                                        DeserializerFactory.GetFactory().CreateUploadPartResultDeserializer(uploadPartRequest.PartNumber.Value),
                                        uploadPartRequest);
        }
    }
}
