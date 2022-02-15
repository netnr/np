using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace Netease.Cloud.NOS.Model.XmlModel
{
    [XmlRoot("ListMultipartUploadsResult")]
    public class ListMultipartUploadsResponseModel
    {
        [XmlElement("Bucket")]
        public string Bucket { get; set; }

        [XmlElement("KeyMarker")]
        public string KeyMarker { get; set; }

        [XmlElement("NextKeyMarker")]
        public string NextKeyMarker { get; set; }

        [XmlElement("Prefix")]
        public string Prefix { get; set; }

        [XmlElement("MaxUploads")]
        public int MaxUploads { get; set; }

        [XmlElement("IsTruncated")]
        public bool IsTruncated { get; set; }

        [XmlElement("Upload")]
        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        public Upload[] Uploads { get; set; }

        [XmlElement("CommonPrefixes")]
        public CommonPrefixs CommonPrefix { get; set; }

        [XmlRoot("Upload")]
        public class Upload
        {
            [XmlElement("Key")]
            public string Key { get; set; }

            [XmlElement("UploadId")]
            public string UploadId { get; set; }

            [XmlElement("StorageClass")]
            public string StorageClass { get; set; }

            [XmlIgnore]
            public DateTime Initiated { get; set; }

            [XmlElement("Initiated")]
            public string Initiated2
            {
                get { return Initiated.ToString("yyyy-MM-dd HH:mm:ss"); }
                set { Initiated = DateTime.Parse(value); }
            }

            [XmlElement("Owner")]
            public Owner Owner { get; set; }
        }

        [XmlRoot("CommonPrefixes")]
        public class CommonPrefixs
        {
            [XmlElement("Prefix")]
            [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
            public string[] Prefixs { get; set; }
        }
    }
}
