using System;
using System.IO;

using Netease.Cloud.NOS.Service;

namespace Netease.Cloud.NOS.Transform
{
    internal class SimpleResponseDeserializer<T> : ResponseDeserializer<T, T>
    {
        public SimpleResponseDeserializer(IDeserializer<Stream, T> contentDeserializer)
            : base(contentDeserializer)
        { }

        public override T Deserialize(ServiceReponse xmlStream)
        {
            using (xmlStream.Content)
            {
                return ContentDeserializer.Deserialize(xmlStream.Content);
            }
        }
    }
}
