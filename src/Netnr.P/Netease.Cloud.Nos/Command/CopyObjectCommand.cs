using System;
using System.Collections.Generic;

using Netease.Cloud.NOS.Service;
using Netease.Cloud.NOS.Util;
using Netease.Cloud.NOS.Model;
using Netease.Cloud.NOS.Transform;

namespace Netease.Cloud.NOS.Commands
{
    internal class CopyObjectCommand : NosCommand
    {
        private readonly CopyObjectRequest _copyObjectRequest;

        protected override string Bucket
        {
            get
            {
                return _copyObjectRequest.DestinationBucket;
            }
        }

        protected override string Key
        {
            get
            {
                return _copyObjectRequest.DestinationKey;
            }
        }

        protected override HttpMethod Method
        {
            get { return HttpMethod.PUT; }
        }

        protected override IDictionary<string, string> Headers
        {
            get
            {
                var headers = new Dictionary<string, string>();
                _copyObjectRequest.Populate(headers);
                return headers;
            }
        }

        private CopyObjectCommand(IServiceClient serviceClient, Uri endpoint, ExecutionContext executionContext,
                                CopyObjectRequest copyObjectRequest)
            : base(serviceClient, endpoint, executionContext)
        {
            _copyObjectRequest = copyObjectRequest;
        }

        public static CopyObjectCommand Create(IServiceClient serviceClient, Uri endpoint, ExecutionContext executionContext, CopyObjectRequest copyObjectRequest)
        {
            NosUtils.CheckBucketName(copyObjectRequest.DestinationBucket);
            NosUtils.CheckObjectName(copyObjectRequest.DestinationKey);
            NosUtils.CheckBucketName(copyObjectRequest.SourceBucket);
            NosUtils.CheckObjectName(copyObjectRequest.SourceKey);

            return new CopyObjectCommand(serviceClient, endpoint, executionContext, copyObjectRequest);
        }
    }
}
