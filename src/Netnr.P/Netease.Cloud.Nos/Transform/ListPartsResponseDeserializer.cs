using System;
using System.IO;

using Netease.Cloud.NOS.Model;
using Netease.Cloud.NOS.Model.XmlModel;
using Netease.Cloud.NOS.Service;
using Netease.Cloud.NOS.Util;

namespace Netease.Cloud.NOS.Transform
{
    internal class ListPartsResponseDeserializer : ResponseDeserializer<PartListing, ListPartsResponseModel>
    {
        public ListPartsResponseDeserializer(IDeserializer<Stream, ListPartsResponseModel> contentDeserializer)
            : base(contentDeserializer)
        { }

        public override PartListing Deserialize(ServiceReponse xmlStream)
        {
            var listPartResult = ContentDeserializer.Deserialize(xmlStream.Content);

            var partListing = new PartListing
            {
                Bucket = listPartResult.Bucket,
                Key = listPartResult.Key,
                MaxParts = listPartResult.MaxParts,
                NextPartNumberMarker = listPartResult.NextPartNumberMarker.Length == 0 ?
                    0 : Convert.ToInt32(listPartResult.NextPartNumberMarker),
                PartNumberMarker = listPartResult.PartNumberMarker,
                UploadId = listPartResult.UploadId,
                IsTruncated = listPartResult.IsTruncated,
                Owner = listPartResult.Owner != null ?
                                new Owner(listPartResult.Owner.Id, listPartResult.Owner.DisplayName) : null,
                StorageClass = listPartResult.StorageClass,
            };

            if (listPartResult.PartResults != null)
            {
                foreach (var partResult in listPartResult.PartResults)
                {
                    var part = new Part
                    {
                        ETag = partResult.ETag != null ? NosUtils.TrimQuotes(partResult.ETag) : string.Empty,
                        LastModified = partResult.LastModified,
                        PartNumber = partResult.PartNumber,
                        Size = partResult.Size
                    };
                    partListing.AddPart(part);
                }
            }

            return partListing;
        }
    }
}
