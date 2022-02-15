using System;
using System.Collections.Generic;
using System.Globalization;

using Netease.Cloud.NOS.Transform;
using Netease.Cloud.NOS.Service;
using Netease.Cloud.NOS.Util;
using Netease.Cloud.NOS.Model;

namespace Netease.Cloud.NOS.Commands
{
    internal class ListObjectsCommand : NosCommand<ObjectListing>
    {
        private readonly ListObjectsRequest _listObjectsRequest;

        protected override string Bucket
        {
            get { return _listObjectsRequest.Bucket; }
        }

        protected override HttpMethod Method
        {
            get { return HttpMethod.GET; }
        }

        protected override IDictionary<string, string> Parameters
        {
            get
            {
                var parameters = base.Parameters;
                Populate(_listObjectsRequest, parameters);
                return parameters;
            }
        }

        private ListObjectsCommand(IServiceClient serviceClient, Uri endpoint, ExecutionContext ExecutionContext,
                                   IDeserializer<ServiceReponse, ObjectListing> deserializer,
                                   ListObjectsRequest listObjectsRequest)
            : base(serviceClient, endpoint, ExecutionContext, deserializer)
        {
            _listObjectsRequest = listObjectsRequest;
        }

        public static ListObjectsCommand Create(IServiceClient serviceClient, Uri endpoint, ExecutionContext ExecutionContext,
                                                ListObjectsRequest listObjectsRequest)
        {
            return new ListObjectsCommand(serviceClient, endpoint, ExecutionContext,
                                          DeserializerFactory.GetFactory().CreateListObjectsResultDeserializer(),
                                          listObjectsRequest);
        }

        private static void Populate(ListObjectsRequest listObjectsRequest, IDictionary<string, string> parameters)
        {
            if (listObjectsRequest.Prefix != null)
            {
                parameters[RequestParameters.PREFIX] = listObjectsRequest.Prefix;
            }

            if (listObjectsRequest.Marker != null)
            {
                parameters[RequestParameters.MARKER] = listObjectsRequest.Marker;
            }

            if (listObjectsRequest.Delimiter != null)
            {
                parameters[RequestParameters.DELIMITER] = listObjectsRequest.Delimiter;
            }

            if (listObjectsRequest.MaxKeys >= 0 && listObjectsRequest.MaxKeys <= NosUtils.MAX_RETURNED_KEYS)
            {
                parameters[RequestParameters.MAX_KEYS] = Convert.ToString(listObjectsRequest.MaxKeys);
            }
            else
                throw new ArgumentException("The max-keys is not valid.");
        }
    }
}
