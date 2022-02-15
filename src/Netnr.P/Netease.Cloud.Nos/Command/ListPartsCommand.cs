using System;
using System.Collections.Generic;

using Netease.Cloud.NOS.Transform;
using Netease.Cloud.NOS.Service;
using Netease.Cloud.NOS.Util;
using Netease.Cloud.NOS.Model;

namespace Netease.Cloud.NOS.Commands
{
    internal class ListPartsCommand : NosCommand<PartListing>
    {
        private readonly ListPartsRequest _listPartsRequest;

        protected override HttpMethod Method
        {
            get { return HttpMethod.GET; }
        }

        protected override string Bucket
        {
            get { return _listPartsRequest.Bucket; }
        }

        protected override string Key
        {
            get { return _listPartsRequest.Key; }
        }

        protected override IDictionary<string, string> Parameters
        {
            get
            {
                var parameters = base.Parameters;
                Populate(_listPartsRequest, parameters);
                return parameters;
            }
        }

        private static void Populate(ListPartsRequest listPartsRequst, IDictionary<string, string> parameters)
        {
            parameters[RequestParameters.PARAMETER_UPLOADID] = listPartsRequst.UploadId;

            if (listPartsRequst.maxParts >= 0 && listPartsRequst.maxParts <= NosUtils.DEFAULT_MAX_PARTS)
                parameters[RequestParameters.MAX_PARTS] = listPartsRequst.maxParts.ToString();
            else
                throw new ArgumentException("The max_parts is not valid");

            if (listPartsRequst.PartNumberMarker != null)
                parameters[RequestParameters.PART_NUMBER_MARKER] = listPartsRequst.PartNumberMarker.ToString();
        }

        private ListPartsCommand(IServiceClient serviceClient, Uri endpoint, ExecutionContext executionContext,
                                 IDeserializer<ServiceReponse, PartListing> deserializeMethod,
                                ListPartsRequest listPartsRequest)
            : base(serviceClient, endpoint, executionContext, deserializeMethod)
        {
            NosUtils.CheckBucketName(listPartsRequest.Bucket);
            NosUtils.CheckObjectName(listPartsRequest.Key);

            if (string.IsNullOrEmpty(listPartsRequest.UploadId))
                throw new ArgumentException("The uploadId of part should be specified.");

            _listPartsRequest = listPartsRequest;
        }

        public static ListPartsCommand Create(IServiceClient serviceClient, Uri endpoint, ExecutionContext executionContext,
                                                ListPartsRequest listPartsRequest)
        {
            return new ListPartsCommand(serviceClient, endpoint, executionContext,
                                        DeserializerFactory.GetFactory().CreateListPartsResultDeserializer(),
                                        listPartsRequest);
        }
    }
}
