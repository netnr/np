using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Netease.Cloud.NOS.Transform
{
    internal class XmlStreamDeserializer<T> : IDeserializer<Stream, T>
    {
        private static readonly XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

        public T Deserialize(Stream xmlStream)
        {
            using (xmlStream)
            {
                try
                {
                    return (T)xmlSerializer.Deserialize(xmlStream);
                }
                catch (XmlException e)
                {
                    throw new ResponseDeserializationException(e.Message, e);
                }
                catch (InvalidOperationException e)
                {
                    throw new ResponseDeserializationException(e.Message, e);
                }
            }
        }
    }
}
