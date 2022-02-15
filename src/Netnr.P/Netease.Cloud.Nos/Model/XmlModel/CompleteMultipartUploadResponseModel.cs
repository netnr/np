using System;
using System.Xml.Serialization;

namespace Netease.Cloud.NOS.Model.XmlModel
{
    [XmlRoot("CompleteMultipartUploadResult")]
    public class CompleteMultipartUploadResponseModel
    {
        [XmlElement("Location")]
        public string Location { get; set; }

        [XmlElement("Bucket")]
        public string Bucket { get; set; }

        [XmlElement("Key")]
        public string Key { get; set; }

        [XmlElement("ETag")]
        public string ETag { get; set; }
    }
}
