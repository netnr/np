using System;
using System.IO;

using Netease.Cloud.NOS.Model;
using Netease.Cloud.NOS.Model.XmlModel;

namespace Netease.Cloud.NOS.Transform
{
    internal abstract class SerializerFactory
    {
        public static SerializerFactory GetFactory(string contentType = null)
        {
            if (contentType == null || contentType.Contains("xml"))
            {
                return new XmlSerializerFactory();
            }
            return null;
        }

        protected abstract ISerializer<T, Stream> CreateContentSerializer<T>();

        internal class XmlSerializerFactory : SerializerFactory
        {
            protected override ISerializer<T, Stream> CreateContentSerializer<T>()
            {
                return new XmlStreamSerializer<T>();
            }
        }

        public ISerializer<DeleteObjectsRequest, Stream> CreateDeleteObjectsRequestSerializer()
        {
            return new DeleteObjectsRequestSerializer(CreateContentSerializer<DeleteObjectsRequestModel>());
        }

        public ISerializer<CompleteMultipartUploadRequest, Stream> CreateCompleteUploadRequestSerializer()
        {
            return new CompleteMultipartUploadRequestSerializer(CreateContentSerializer<CompleteMultipartUploadRequestModel>());
        }
    }
}
