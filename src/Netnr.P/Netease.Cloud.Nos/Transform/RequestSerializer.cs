using System;
using System.IO;

namespace Netease.Cloud.NOS.Transform
{
    internal abstract class RequestSerializer<TRequest, TModel> : ISerializer<TRequest, Stream>
    {
        protected ISerializer<TModel, Stream> ContentSerializer { get; private set; }

        public RequestSerializer(ISerializer<TModel, Stream> contentSerializer)
        {
            ContentSerializer = contentSerializer;
        }

        public abstract Stream Serialize(TRequest request);
    }
}
