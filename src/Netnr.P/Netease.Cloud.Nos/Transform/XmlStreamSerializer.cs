using System;
using System.IO;
using System.Xml.Serialization;

namespace Netease.Cloud.NOS.Transform
{
    internal class XmlStreamSerializer<TRequest> : ISerializer<TRequest, Stream>
    {
        private static readonly XmlSerializer XmlSerializer = new XmlSerializer(typeof(TRequest));

        public Stream Serialize(TRequest requestObject)
        {
            MemoryStream stream = null;
            try
            {
                stream = new MemoryStream();
                var namespaces = new XmlSerializerNamespaces();
                namespaces.Add(string.Empty, string.Empty);
                XmlSerializer.Serialize(stream, requestObject, namespaces);
                stream.Seek(0, SeekOrigin.Begin);
                return stream;
            }
            catch (InvalidOperationException ex)
            {
                if (stream != null)
                    stream.Close();

                throw new RequestSerializationException(ex.Message, ex);
            }
        }
    }
}
