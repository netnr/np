using System;
using System.Collections.Generic;

using Netease.Cloud.NOS.Service;
using Netease.Cloud.NOS.Util;
using Netease.Cloud.NOS.Model;
using Netease.Cloud.NOS.Transform;

namespace Netease.Cloud.NOS.Commands
{
    internal class MoveObjectCommand : NosCommand
    {
        private readonly MoveObjectRequest _moveObjectRequest;

        protected override string Bucket
        {
            get
            {
                return _moveObjectRequest.DestinationBucket;
            }
        }

        protected override string Key
        {
            get
            {
                return _moveObjectRequest.DestinationKey;
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
                _moveObjectRequest.Populate(headers);
                return headers;
            }
        }

        private MoveObjectCommand(IServiceClient serviceClient, Uri endpoint, ExecutionContext executionContext,
                                MoveObjectRequest moveObjectRequest)
            : base(serviceClient, endpoint, executionContext)
        {
            _moveObjectRequest = moveObjectRequest;
        }

        public static MoveObjectCommand Create(IServiceClient serviceClient, Uri endpoint, ExecutionContext executionContext, MoveObjectRequest moveObjectRequest)
        {
            NosUtils.CheckBucketName(moveObjectRequest.DestinationBucket);
            NosUtils.CheckObjectName(moveObjectRequest.DestinationKey);
            NosUtils.CheckBucketName(moveObjectRequest.SourceBucket);
            NosUtils.CheckObjectName(moveObjectRequest.SourceKey);

            return new MoveObjectCommand(serviceClient, endpoint, executionContext, moveObjectRequest);
        }
    }
}
