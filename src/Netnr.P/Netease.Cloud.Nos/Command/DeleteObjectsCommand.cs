using System;
using System.IO;
using System.Collections.Generic;

using Netease.Cloud.NOS.Transform;
using Netease.Cloud.NOS.Service;
using Netease.Cloud.NOS.Util;
using Netease.Cloud.NOS.Model;

namespace Netease.Cloud.NOS.Commands
{
    internal class DeleteObjectsCommand : NosCommand<DeleteObjectsResult>
    {
        private readonly DeleteObjectsRequest _deleteObjectsRequest;

        protected override string Bucket
        {
            get { return _deleteObjectsRequest.Bucket; }
        }

        protected override HttpMethod Method
        {
            get { return HttpMethod.POST; }
        }

        protected override IDictionary<string, string> Parameters
        {
            get
            {
                var parameters = new Dictionary<string, string>();
                parameters[RequestParameters.PARAMETER_DELETE] = null;
                return parameters;
            }
        }

        protected override Stream Content
        {
            get
            {
                return SerializerFactory.GetFactory().CreateDeleteObjectsRequestSerializer().Serialize(_deleteObjectsRequest);
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
                return headers;
            }
        }

        private DeleteObjectsCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                IDeserializer<ServiceReponse, DeleteObjectsResult> deserializeMethod,
                                DeleteObjectsRequest deleteObjectsRequest)
            : base(client, endpoint, context, deserializeMethod)
        {
            _deleteObjectsRequest = deleteObjectsRequest;
        }

        public static DeleteObjectsCommand Create(IServiceClient client, Uri endpoint, ExecutionContext context,
                                                  DeleteObjectsRequest deleteObjectsRequest)
        {
            NosUtils.CheckBucketName(deleteObjectsRequest.Bucket);
            return new DeleteObjectsCommand(client, endpoint, context,
                                            DeserializerFactory.GetFactory().CreateDeleteObjectsResultDeserializer(),
                                            deleteObjectsRequest);
        }

    }
}
