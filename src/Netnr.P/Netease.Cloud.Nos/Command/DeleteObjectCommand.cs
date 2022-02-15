using System;
using System.Collections.Generic;

using Netease.Cloud.NOS.Service;
using Netease.Cloud.NOS.Util;

namespace Netease.Cloud.NOS.Commands
{
    internal class DeleteObjectCommand : NosCommand
    {
        private readonly string _bucket;
        private readonly string _key;

        protected override string Bucket
        {
            get
            {
                return _bucket;
            }
        }

        protected override string Key
        {
            get
            {
                return _key;
            }
        }

        protected override HttpMethod Method
        {
            get
            {
                return HttpMethod.DELETE;
            }
        }

        private DeleteObjectCommand(IServiceClient serviceClient, Uri endpoint, ExecutionContext executionContext, string bucket, string key)
            : base(serviceClient, endpoint, executionContext)
        {
            NosUtils.CheckBucketName(bucket);
            NosUtils.CheckObjectName(key);

            _bucket = bucket;
            _key = key;
        }

        public static DeleteObjectCommand Create(IServiceClient serviceClient, Uri endpoint, ExecutionContext executionContext, string bucket, string key)
        {
            return new DeleteObjectCommand(serviceClient, endpoint, executionContext, bucket, key);
        }
    }
}
