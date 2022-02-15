using System;
using System.Runtime.Serialization;

namespace Netease.Cloud.NOS.Transform
{
    [Serializable]
    internal class ResponseDeserializationException : InvalidOperationException, ISerializable
    {
        public ResponseDeserializationException()
        { }

        public ResponseDeserializationException(string message)
            : base(message)
        { }

        public ResponseDeserializationException(string message, Exception exception)
            : base(message, exception)
        { }

        protected ResponseDeserializationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
