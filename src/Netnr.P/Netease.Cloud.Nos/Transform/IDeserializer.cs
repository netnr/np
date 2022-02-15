using System;

namespace Netease.Cloud.NOS.Transform
{
    internal interface IDeserializer<in Tin, out Tout>
    {
        Tout Deserialize(Tin xmlStream);
    }
}
