using System;
using System.Xml.Serialization;

namespace Netease.Cloud.NOS
{
    [XmlRoot("Error")]
    public class ErrorResult
    {
        [XmlElement("Code")]
        public string Code { get; set; }

        [XmlElement("Message")]
        public string Message { get; set; }

        [XmlElement("Resource")]
        public string Resource { get; set; }

        [XmlElement("RequestId")]
        public string RequestId { get; set; }
    }
}
