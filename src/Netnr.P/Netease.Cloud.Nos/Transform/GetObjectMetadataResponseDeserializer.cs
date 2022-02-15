using System;
using System.Globalization;

using Netease.Cloud.NOS.Model;
using Netease.Cloud.NOS.Service;
using Netease.Cloud.NOS.Util;

namespace Netease.Cloud.NOS.Transform
{
    internal class GetObjectMetadataResponseDeserializer : ResponseDeserializer<ObjectMetadata, ObjectMetadata>
    {
        public GetObjectMetadataResponseDeserializer()
            : base(null)
        { }

        public override ObjectMetadata Deserialize(ServiceReponse xmlStream)
        {
            var metadata = new ObjectMetadata();
            foreach (var header in xmlStream.Headers)
            {
                if (header.Key.StartsWith(Headers.NOS_USER_METADATA_PREFIX, false, CultureInfo.InvariantCulture))
                {
                    metadata.UserMetadata.Add(header.Key.Substring(Headers.NOS_USER_METADATA_PREFIX.Length),
                                              header.Value);
                }
                else if (string.Equals(header.Key, Headers.CONTENT_LENGTH, StringComparison.InvariantCultureIgnoreCase))
                {
                    metadata.ContentLength = long.Parse(header.Value, CultureInfo.InvariantCulture);
                }
                else if (string.Equals(header.Key, Headers.ETAG, StringComparison.InvariantCultureIgnoreCase))
                {
                    metadata.ETag = NosUtils.TrimQuotes(header.Value);
                }
                else if (string.Equals(header.Key, Headers.LAST_MODIFIED, StringComparison.InvariantCultureIgnoreCase))
                {
                    metadata.LastModified = header.Value;
                }
                else
                {
                    metadata.AddHeader(header.Key, header.Value);
                }
            }
            return metadata;
        }
    }
}
