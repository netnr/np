using System;
using System.Xml.Serialization;

namespace Netease.Cloud.NOS.Model.XmlModel
{
    [XmlRoot("InitiateMultipartUploadResult")]
    public class InitiateMultipartResponseModel
    {
        [XmlElement("Bucket")]
        public string Bucket { get; set; }

        [XmlElement("Key")]
        public string Key { get; set; }

        [XmlElement("UploadId")]
        public string UploadId { get; set; }
    }
}
