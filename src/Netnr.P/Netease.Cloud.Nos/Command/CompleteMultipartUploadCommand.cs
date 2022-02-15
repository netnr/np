using System;
using System.IO;
using System.Collections.Generic;

using Netease.Cloud.NOS.Transform;
using Netease.Cloud.NOS.Service;
using Netease.Cloud.NOS.Util;
using Netease.Cloud.NOS.Model;

namespace Netease.Cloud.NOS.Commands
{
    internal class CompleteMultipartUploadCommand : NosCommand<CompleteMultipartUploadResult>
    {
        private readonly CompleteMultipartUploadRequest _completeMultipartUploadRequest;

        protected override string Bucket
        {
            get
            {
                return _completeMultipartUploadRequest.Bucket;
            }
        }

        protected override string Key
        {
            get
            {
                return _completeMultipartUploadRequest.Key;
            }
        }

        protected override HttpMethod Method
        {
            get { return HttpMethod.POST; }
        }


        protected override IDictionary<string, string> Parameters
        {
            get
            {
                var parameters = base.Parameters;
                parameters[RequestParameters.PARAMETER_UPLOADID] = _completeMultipartUploadRequest.UploadId;
                return parameters;
            }
        }

        protected override Stream Content
        {
            get
            {
                return SerializerFactory.GetFactory().CreateCompleteUploadRequestSerializer()
                    .Serialize(_completeMultipartUploadRequest);
            }
        }

        protected override IDictionary<string, string> Headers
        {
            get
            {
                var headers = new Dictionary<string, string>();
                headers[Util.Headers.CONTENT_LENGTH] = Content.Length.ToString();
                string content = NosUtils.XmlStreamToMd5String(Content);
                headers[Util.Headers.CONTENT_MD5] = NosUtils.GetMD5FromXmlString(content);
                _completeMultipartUploadRequest.Populate(headers);
                return headers;
            }
        }

        private CompleteMultipartUploadCommand(IServiceClient serviceClient, Uri endpoint, ExecutionContext executionContext,
                                 IDeserializer<ServiceReponse, CompleteMultipartUploadResult> deserializeMethod,
                                 CompleteMultipartUploadRequest completeMultipartUploadRequest)
            : base(serviceClient, endpoint, executionContext, deserializeMethod)
        {
            _completeMultipartUploadRequest = completeMultipartUploadRequest;
        }

        public static CompleteMultipartUploadCommand Create(IServiceClient serviceClient, Uri endpoint, ExecutionContext executionContext,
                                  CompleteMultipartUploadRequest completeMultipartUploadRequest)
        {
            NosUtils.CheckBucketName(completeMultipartUploadRequest.Bucket);
            NosUtils.CheckObjectName(completeMultipartUploadRequest.Key);

            if (string.IsNullOrEmpty(completeMultipartUploadRequest.UploadId))
                throw new ArgumentException("The uploadId of part should be specified.");

            return new CompleteMultipartUploadCommand(serviceClient, endpoint, executionContext,
                                               DeserializerFactory.GetFactory().CreateCompleteUploadResultDeserializer(),
                                               completeMultipartUploadRequest);

        }
    }
}
