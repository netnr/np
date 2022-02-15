using System;
using System.IO;

using Netease.Cloud.NOS.Model;
using Netease.Cloud.NOS.Model.XmlModel;
using Netease.Cloud.NOS.Service;
using Netease.Cloud.NOS.Util;

namespace Netease.Cloud.NOS.Transform
{
    internal class CompleteMultipartUploadResultDeserializer
        : ResponseDeserializer<CompleteMultipartUploadResult, CompleteMultipartUploadResponseModel>
    {
        public CompleteMultipartUploadResultDeserializer(IDeserializer<Stream, CompleteMultipartUploadResponseModel> contentDeserializer)
            : base(contentDeserializer)
        { }

        public override CompleteMultipartUploadResult Deserialize(ServiceReponse xmlStream)
        {
            var result = ContentDeserializer.Deserialize(xmlStream.Content);
            return new CompleteMultipartUploadResult
            {
                Bucket = result.Bucket,
                Key = result.Key,
                Location = result.Location,
                ETag = NosUtils.TrimQuotes(result.ETag)
            };
        }
    }
}
