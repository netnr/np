using System;
using System.Collections.Generic;
using System.Globalization;

using Netease.Cloud.NOS.Transform;
using Netease.Cloud.NOS.Service;
using Netease.Cloud.NOS.Util;
using Netease.Cloud.NOS.Model;

namespace Netease.Cloud.NOS.Commands
{
    internal class ListMultipartUploadsCommand : NosCommand<MultipartUploadListing>
    {
        private readonly ListMultipartUploadsRequest _listMultipartUploadsRequest;

        protected override HttpMethod Method
        {
            get { return HttpMethod.GET; }
        }

        protected override string Bucket
        {
            get { return _listMultipartUploadsRequest.Bucket; }
        }

        protected override IDictionary<string, string> Parameters
        {
            get
            {
                var parameters = base.Parameters;
                Populate(_listMultipartUploadsRequest, parameters);
                return parameters;
            }
        }

        private ListMultipartUploadsCommand(IServiceClient serviceClient, Uri endpoint, ExecutionContext executionContext,
                                            IDeserializer<ServiceReponse, MultipartUploadListing> deserializeMethod,
                                            ListMultipartUploadsRequest listMultipartUploadsRequest)
            : base(serviceClient, endpoint, executionContext, deserializeMethod)
        {
            NosUtils.CheckBucketName(listMultipartUploadsRequest.Bucket);

            _listMultipartUploadsRequest = listMultipartUploadsRequest;
        }

        public static ListMultipartUploadsCommand Create(IServiceClient serviceClient, Uri endpoint, ExecutionContext executionContext,
                                                         ListMultipartUploadsRequest listMultipartUploadsRequest)
        {
            return new ListMultipartUploadsCommand(serviceClient, endpoint, executionContext,
                                                   DeserializerFactory.GetFactory().CreateListMultipartUploadsResultDeserializer(),
                                                   listMultipartUploadsRequest);
        }

        private static void Populate(ListMultipartUploadsRequest listMultipartUploadsRequest,
                                    IDictionary<string, string> parameters)
        {
            parameters[RequestParameters.PARAMETER_UPLOADS] = null;

            if (listMultipartUploadsRequest.KeyMarker != null)
            {
                parameters[RequestParameters.KEY_MARKER] = listMultipartUploadsRequest.KeyMarker;
            }

            if (listMultipartUploadsRequest.MaxUploads >= 0 &listMultipartUploadsRequest.MaxUploads<=NosUtils.DEFAULT_MAX_UPLOADS)
            {
                parameters[RequestParameters.MAX_UPLOADS] = listMultipartUploadsRequest.MaxUploads.ToString();
            }
            else
                throw new ArgumentException("The max_parts is not valid");

            if (listMultipartUploadsRequest.Prefix != null)
            {
                parameters[RequestParameters.PREFIX] = listMultipartUploadsRequest.Prefix;
            }
        }

    }
}
