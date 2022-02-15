using System;
using System.Xml.Serialization;

namespace Netease.Cloud.NOS.Model.XmlModel
{
    [XmlRoot("Delete")]
    public class DeleteObjectsRequestModel
    {
        [XmlElement("Quiet")]
        public bool Quiet { get; set; }

        [XmlElement("Object")]
        public ObjectToDel[] Keys { get; set; }

        [XmlRoot("Object")]
        public class ObjectToDel
        {
            [XmlElement("Key")]
            public string Key { get; set; }
        }
    }
}
