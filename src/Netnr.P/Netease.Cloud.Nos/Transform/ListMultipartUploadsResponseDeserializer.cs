using System;
using System.IO;
using System.Globalization;

using Netease.Cloud.NOS.Model;
using Netease.Cloud.NOS.Model.XmlModel;
using Netease.Cloud.NOS.Service;
using Netease.Cloud.NOS.Util;

namespace Netease.Cloud.NOS.Transform
{
    internal class ListMultipartUploadsResponseDeserializer : ResponseDeserializer<MultipartUploadListing, ListMultipartUploadsResponseModel>
    {
        public ListMultipartUploadsResponseDeserializer(IDeserializer<Stream, ListMultipartUploadsResponseModel> contentDeserializer)
            : base(contentDeserializer)
        { }

        public override MultipartUploadListing Deserialize(ServiceReponse xmlStream)
        {
            var listMultipartUploadsResult = ContentDeserializer.Deserialize(xmlStream.Content);

            var uploadsList = new MultipartUploadListing(listMultipartUploadsResult.Bucket)
            {
                Bucket = listMultipartUploadsResult.Bucket,
                IsTruncated = listMultipartUploadsResult.IsTruncated,
                KeyMarker = listMultipartUploadsResult.KeyMarker,
                MaxUploads = listMultipartUploadsResult.MaxUploads,
                NextKeyMarker = listMultipartUploadsResult.NextKeyMarker,
                Prefix = listMultipartUploadsResult.Prefix,
            };

            if (listMultipartUploadsResult.CommonPrefix != null)
            {
                if (listMultipartUploadsResult.CommonPrefix.Prefixs != null)
                {
                    foreach (var prefix in listMultipartUploadsResult.CommonPrefix.Prefixs)
                    {
                        uploadsList.AddCommonPrefix(prefix);
                    }
                }
            }

            if (listMultipartUploadsResult.Uploads != null)
            {
                foreach (var uploadResult in listMultipartUploadsResult.Uploads)
                {
                    var upload = new MultipartUpload
                    {
                        Initiated = uploadResult.Initiated,
                        Key = uploadResult.Key,
                        UploadId = uploadResult.UploadId,
                        StorageClass = uploadResult.StorageClass
                    };
                    uploadsList.AddMultipartUpload(upload);
                }
            }

            return uploadsList;
        }
    }
}
