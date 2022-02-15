using System;
using System.Xml.Serialization;

namespace Netease.Cloud.NOS.Model.XmlModel
{
    [XmlRoot("ListPartsResult")]
    public class ListPartsResponseModel
    {
        [XmlElement("Bucket")]
        public string Bucket { get; set; }

        [XmlElement("Key")]
        public string Key { get; set; }

        [XmlElement("UploadId")]
        public string UploadId { get; set; }

        [XmlElement("Owner")]
        public Owner Owner { get; set; }

        [XmlElement("StorageClass")]
        public string StorageClass { get; set; }

        [XmlElement("PartNumberMarker")]
        public int PartNumberMarker { get; set; }

        [XmlElement("NextPartNumberMarker")]
        public String NextPartNumberMarker { get; set; }

        [XmlElement("MaxParts")]
        public int MaxParts { get; set; }

        [XmlElement("IsTruncated")]
        public bool IsTruncated { get; set; }

        [XmlElement("Part")]
        public PartResult[] PartResults { get; set; }


        [XmlRoot("Part")]
        public class PartResult
        {
            [XmlElement("PartNumber")]
            public int PartNumber { get; set; }

            [XmlElement("LastModified")]
            public string LastModified { get; set; }

            //[XmlElement("LastModified")]
            //public string LastModified2
            //{
            //    get { return LastModified.ToString("yyyy-MM-dd HH:mm:ss"); }
            //    set { LastModified = DateTime.Parse(value); }
            //}

            [XmlElement("ETag")]
            public string ETag { get; set; }

            [XmlElement("Size")]
            public long Size { get; set; }
        }
    }
}
