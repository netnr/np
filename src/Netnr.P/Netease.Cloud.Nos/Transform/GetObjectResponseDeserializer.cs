using System;

using Netease.Cloud.NOS.Model;
using Netease.Cloud.NOS.Service;

namespace Netease.Cloud.NOS.Transform
{
    internal class GetObjectResponseDeserializer : ResponseDeserializer<NosObject, NosObject>
    {
        private readonly GetObjectRequest _getObjectRequest;

        public GetObjectResponseDeserializer(GetObjectRequest getObjectRequest)
            : base(null)
        {
            _getObjectRequest = getObjectRequest;
        }

        public override NosObject Deserialize(ServiceReponse xmlStream)
        {
            return new NosObject(_getObjectRequest.Key)
            {
                Bucket = _getObjectRequest.Bucket,
                Content = xmlStream.Content,
                objectMetadata = DeserializerFactory.GetFactory()
                    .CreateGetObjectMetadataResultDeserializer().Deserialize(xmlStream)
            };
        }
    }
}
