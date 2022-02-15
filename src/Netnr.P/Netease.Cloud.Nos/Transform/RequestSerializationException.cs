using System;
using System.Runtime.Serialization;

namespace Netease.Cloud.NOS.Transform
{
    [Serializable]
    internal class RequestSerializationException : InvalidOperationException, ISerializable
    {
        public RequestSerializationException()
        { }

        public RequestSerializationException(string message)
            : base(message)
        { }

        public RequestSerializationException(string message, Exception Exception)
            : base(message, Exception)
        { }

        protected RequestSerializationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
