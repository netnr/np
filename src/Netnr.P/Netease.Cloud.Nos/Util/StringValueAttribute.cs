using System;

namespace Netease.Cloud.NOS.Util
{
    internal sealed class StringValueAttribute : Attribute
    {
        public string Value { get; private set; }

        public StringValueAttribute(string value)
        {
            this.Value = value;
        }
    }
}