using System;
using System.IO;
using System.Collections.Generic;

using Netease.Cloud.NOS.Transform;
using Netease.Cloud.NOS.Service;
using Netease.Cloud.NOS.Util;
using Netease.Cloud.NOS.Model;

namespace Netease.Cloud.NOS.Commands
{
    class PutObjectCommand : NosCommand<PutObjectResult>
    {
        private readonly NosObject _nosObject;

        protected override string Bucket
        {
            get { return _nosObject.Bucket; }
        }

        protected override string Key
        {
            get
            {
                return _nosObject.Key;
            }
        }

        protected override HttpMethod Method
        {
            get { return HttpMethod.PUT; }
        }

        protected override Stream Content
        {
            get { return _nosObject.Content; }
        }

        protected override IDictionary<string, string> Headers
        {
            get
            {
                var headers = new Dictionary<string, string>();
                _nosObject.objectMetadata.Populate(headers);
                if (_nosObject.objectMetadata.ContentMD5 == null)
                {
                    headers[Util.Headers.CONTENT_MD5] = NosUtils.GetMD5FromStream(Content);
                    headers[Util.Headers.CONTENT_LENGTH] = Content.Length.ToString();
                }                                    
                return headers;
            }
        }

        protected override bool LeaveRequestOpen
        {
            get { return true; }
        }

        private PutObjectCommand(IServiceClient serviceClient, Uri endpoint, ExecutionContext executionContext,
                                IDeserializer<ServiceReponse, PutObjectResult> deserializer, NosObject nosObject)
            : base(serviceClient, endpoint, executionContext, deserializer)
        {
            _nosObject = nosObject;
        }

        public static PutObjectCommand Create(IServiceClient serviceClient, Uri endpoint, ExecutionContext executionContext,
                                              string bucket, string key,
                                              Stream content, ObjectMetadata objectMetadata)
        {
            NosUtils.CheckBucketName(bucket);
            NosUtils.CheckObjectName(key);

            if (content == null)
                throw new ArgumentNullException("The content is null or empty.");

            var nosObject = new NosObject(key)
            {
                Bucket = bucket,
                Content = content,
                objectMetadata = objectMetadata ?? new ObjectMetadata()
            };

            return new PutObjectCommand(serviceClient, endpoint, executionContext,
                                        DeserializerFactory.GetFactory().CreatePutObjectReusltDeserializer(),
                                        nosObject);
        }
    }
}
