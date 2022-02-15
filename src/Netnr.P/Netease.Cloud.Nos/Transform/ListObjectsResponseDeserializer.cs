using System;
using System.IO;

using Netease.Cloud.NOS.Model;
using Netease.Cloud.NOS.Model.XmlModel;
using Netease.Cloud.NOS.Service;
using Netease.Cloud.NOS.Util;

namespace Netease.Cloud.NOS.Transform
{
    internal class ListObjectsResponseDeserializer : ResponseDeserializer<ObjectListing, ListObjectsResponseModel>
    {
        public ListObjectsResponseDeserializer(IDeserializer<Stream, ListObjectsResponseModel> contentDeserializer)
            : base(contentDeserializer)
        { }

        public override ObjectListing Deserialize(ServiceReponse xmlStream)
        {
            var listObjectsResult = ContentDeserializer.Deserialize(xmlStream.Content);

            var objectList = new ObjectListing(listObjectsResult.Bucket)
            {
                Delimiter = listObjectsResult.Delimiter,
                IsTruncated = listObjectsResult.IsTruncated,
                Marker = listObjectsResult.Marker,
                MaxKeys = listObjectsResult.MaxKeys,
                NextMarker = listObjectsResult.NextMarker,
                Prefix = listObjectsResult.Prefix,
            };

            if (listObjectsResult.Contents != null)
            {
                foreach (var contents in listObjectsResult.Contents)
                {
                    var summary = new NosObjectSummary
                    {
                        Bucket = listObjectsResult.Bucket,
                        Key = contents.Key,
                        LastModified = contents.LastModified,
                        ETag = contents.ETag != null ? NosUtils.TrimQuotes(contents.ETag) : string.Empty,
                        Size = contents.Size,
                        StorageClass = contents.StorageClass,
                        Owner = contents.Owner != null ?
                                new Owner(contents.Owner.Id, contents.Owner.DisplayName) : null
                    };

                    objectList.AddObjectSummary(summary);
                }
            }

            if (listObjectsResult.CommonPrefixs != null)
            {
                foreach (var commonPrefixes in listObjectsResult.CommonPrefixs)
                {
                    if (commonPrefixes.Prefix != null)
                    {
                        foreach (var prefix in commonPrefixes.Prefix)
                        {
                            objectList.AddCommonPrefix(Decode(prefix, "url"));
                        }
                    }
                }
            }
            return objectList;
        }
    }
}
