using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

using Netease.Cloud.NOS.Service;
using Netease.Cloud.NOS.Model;
using Netease.Cloud.NOS.Model.XmlModel;

namespace Netease.Cloud.NOS.Transform
{
    internal abstract class DeserializerFactory
    {
        public static DeserializerFactory GetFactory()
        {
            return GetFactory(null);
        }

        public static DeserializerFactory GetFactory(string contentType)
        {
            if (contentType == null)
                contentType = "text/xml";

            if (contentType.Contains("xml"))
                return new XmlDeserializerFactory();

            return null;
        }

        public abstract IDeserializer<Stream, T> CreateContentDeserializer<T>();

        public IDeserializer<ServiceReponse, ErrorResult> CreateErrorResultDeserializer()
        {
            return new SimpleResponseDeserializer<ErrorResult>(CreateContentDeserializer<ErrorResult>());
        }        

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public IDeserializer<ServiceReponse, NosObject> CreateGetObjectResultDeserializer(GetObjectRequest request)
        {
            return new GetObjectResponseDeserializer(request);
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public IDeserializer<ServiceReponse, PutObjectResult> CreatePutObjectReusltDeserializer()
        {
            return new PutObjectResponseDeserializer();
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public IDeserializer<ServiceReponse, ObjectMetadata> CreateGetObjectMetadataResultDeserializer()
        {
            return new GetObjectMetadataResponseDeserializer();
        }

        public IDeserializer<ServiceReponse, DeleteObjectsResult> CreateDeleteObjectsResultDeserializer()
        {
            return new DeleteObjectsResultDeserializer(CreateContentDeserializer<DeleteObjectsResult>());
        }

        internal IDeserializer<ServiceReponse, ObjectListing> CreateListObjectsResultDeserializer()
        {
            return new ListObjectsResponseDeserializer(CreateContentDeserializer<ListObjectsResponseModel>());
        }

        public IDeserializer<ServiceReponse, InitiateMultipartUploadResult> CreateInitiateMultipartUploadResultDeserializer()
        {
            return new InitiateMultipartUploadResultDeserializer(CreateContentDeserializer<InitiateMultipartResponseModel>());
        }

        public IDeserializer<ServiceReponse, UploadPartResult> CreateUploadPartResultDeserializer(int partNumber)
        {
            return new UploadPartResponseDeserializer(partNumber);
        }

        internal IDeserializer<ServiceReponse, CompleteMultipartUploadResult> CreateCompleteUploadResultDeserializer()
        {
            return new CompleteMultipartUploadResultDeserializer(CreateContentDeserializer<CompleteMultipartUploadResponseModel>());
        }

        public IDeserializer<ServiceReponse, PartListing> CreateListPartsResultDeserializer()
        {
            return new ListPartsResponseDeserializer(CreateContentDeserializer<ListPartsResponseModel>());
        }

        public IDeserializer<ServiceReponse, MultipartUploadListing> CreateListMultipartUploadsResultDeserializer()
        {
            return new ListMultipartUploadsResponseDeserializer(CreateContentDeserializer<ListMultipartUploadsResponseModel>());
        }
    }

    internal class XmlDeserializerFactory : DeserializerFactory
    {
        public override IDeserializer<Stream, T> CreateContentDeserializer<T>()
        {
            return new XmlStreamDeserializer<T>();
        }
    }
}
