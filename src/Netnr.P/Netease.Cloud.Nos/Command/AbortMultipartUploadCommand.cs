using System;
using System.Collections.Generic;

using Netease.Cloud.NOS.Model;
using Netease.Cloud.NOS.Service;
using Netease.Cloud.NOS.Util;

namespace Netease.Cloud.NOS.Commands
{
    internal class AbortMultipartUploadCommand : NosCommand
    {
        private readonly AbortMultipartUploadRequest _abortMultipartUploadRequest;

        protected override HttpMethod Method
        {
            get { return HttpMethod.DELETE; }
        }

        protected override string Bucket
        {
            get
            {
                return _abortMultipartUploadRequest.Bucket;
            }
        }

        protected override string Key
        {
            get
            {
                return _abortMultipartUploadRequest.Key;
            }
        }

        protected override IDictionary<string, string> Parameters
        {
            get
            {
                var parameters = base.Parameters;
                parameters[RequestParameters.PARAMETER_UPLOADID] = _abortMultipartUploadRequest.UploadId;
                return parameters;
            }
        }

        private AbortMultipartUploadCommand(IServiceClient serviceClient, Uri endpoint, ExecutionContext executionContext,
                                            AbortMultipartUploadRequest abortMultipartUploadRequest)
            : base(serviceClient, endpoint, executionContext)
        {
            _abortMultipartUploadRequest = abortMultipartUploadRequest;
        }


        public static AbortMultipartUploadCommand Create(IServiceClient serviceClient, Uri endpoint, ExecutionContext executionContext,
                                                 AbortMultipartUploadRequest abortMultipartUploadRequest)
        {
            NosUtils.CheckBucketName(abortMultipartUploadRequest.Bucket);
            NosUtils.CheckObjectName(abortMultipartUploadRequest.Key);

            if (string.IsNullOrEmpty(abortMultipartUploadRequest.UploadId))
                throw new ArgumentException("The uploadId of part should be specified.");

            return new AbortMultipartUploadCommand(serviceClient, endpoint, executionContext, abortMultipartUploadRequest);
        }
    }
}
