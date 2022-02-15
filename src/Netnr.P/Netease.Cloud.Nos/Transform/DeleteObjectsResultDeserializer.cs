using System;
using System.IO;

using Netease.Cloud.NOS.Model;
using Netease.Cloud.NOS.Service;

namespace Netease.Cloud.NOS.Transform
{
    internal class DeleteObjectsResultDeserializer : ResponseDeserializer<DeleteObjectsResult, DeleteObjectsResult>
    {
        public DeleteObjectsResultDeserializer(IDeserializer<Stream, DeleteObjectsResult> contentDeserializer)
            : base(contentDeserializer)
        { }

        public override DeleteObjectsResult Deserialize(ServiceReponse xmlStream)
        {

            if (int.Parse(xmlStream.Headers[Util.Headers.CONTENT_LENGTH]) == 0)
                return new DeleteObjectsResult();

            return ContentDeserializer.Deserialize(xmlStream.Content);
        }
    }
}
