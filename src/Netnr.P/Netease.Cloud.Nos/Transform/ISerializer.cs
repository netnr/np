using System;

namespace Netease.Cloud.NOS.Transform
{
    internal interface ISerializer<in TIn, out TOut>
    {
        TOut Serialize(TIn input);
    }
}
