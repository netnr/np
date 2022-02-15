using System.Xml.Serialization;
using System;

namespace Netease.Cloud.NOS.Model.XmlModel
{
    [XmlRoot("ListBucketResult")]
    public class ListObjectsResponseModel
    {
        private string _bucket;
        private string _prefix;
        private string _delimiter;
        private string _marker;
        private int _maxKeys;
        private string _nextMarker;
        private bool _isTruncated;

        private ListObjectsResultContents[] _contents;
        private ListObjectsResultCommonPrefixes[] _commonPrefixes;

        [XmlElement("Name")]
        public string Bucket
        {
            get
            {
                return _bucket;
            }
            set
            {
                _bucket = value;
            }
        }

        [XmlElement("Prefix")]
        public string Prefix
        {
            get
            {
                return _prefix;
            }
            set
            {
                _prefix = value;
            }
        }

        [XmlElement("Delimiter")]
        public string Delimiter
        {
            get
            {
                return _delimiter;
            }
            set
            {
                _delimiter = value;
            }
        }

        [XmlElement("Marker")]
        public string Marker
        {
            get
            {
                return _marker;
            }
            set
            {
                _marker = value;
            }
        }

        [XmlElement("MaxKeys")]
        public int MaxKeys
        {
            get
            {
                return _maxKeys;
            }
            set
            {
                _maxKeys = value;
            }
        }

        [XmlElement("NextMarker")]
        public string NextMarker
        {
            get
            {
                return _nextMarker;
            }
            set
            {
                _nextMarker = value;
            }
        }

        [XmlElement("IsTruncated")]
        public bool IsTruncated
        {
            get
            {
                return _isTruncated;
            }
            set
            {
                _isTruncated = value;
            }
        }

        [XmlElement("Contents")]
        public ListObjectsResultContents[] Contents
        {
            get
            {
                return _contents;
            }
            set
            {
                _contents = value;
            }
        }

        [XmlElement("CommonPrefixs")]
        public ListObjectsResultCommonPrefixes[] CommonPrefixs
        {
            get
            {
                return _commonPrefixes;
            }
            set
            {
                _commonPrefixes = value;
            }
        }
    }

    [XmlRoot("Contents")]
    public class ListObjectsResultContents
    {
        private string _key;
        private string _lastModified;
        private long _size;
        private string _etag;
        private string _storageClass;
        private Owner _owner;

        [XmlElement("Key")]
        public string Key
        {
            get
            {
                return _key;
            }
            set
            {
                _key = value;
            }
        }

        [XmlElement("LastModified")]
        public string LastModified
        {
            get
            {
                return _lastModified;
            }
            set
            {
                _lastModified = value;
            }
        }

        //[XmlElement("LastModified")]
        //public string LastModified2
        //{
        //    get { return LastModified.ToString("yyyy-MM-dd HH:mm:ss"); }
        //    set { LastModified = DateTime.Parse(value); }
        //}

        [XmlElement("Size")]
        public long Size
        {
            get
            {
                return _size;
            }
            set
            {
                _size = value;
            }
        }

        [XmlElement("ETag")]
        public string ETag
        {
            get
            {
                return _etag;
            }
            set
            {
                _etag = value;
            }
        }

        [XmlElement("StorageClass")]
        public string StorageClass
        {
            get
            {
                return _storageClass;
            }
            set
            {
                _storageClass = value;
            }
        }

        [XmlElement("Owner")]
        public Owner Owner
        {
            get
            {
                return _owner;
            }
            set
            {
                _owner = value;
            }
        }
    }

    [XmlRoot("CommonPrefixes")]
    public class ListObjectsResultCommonPrefixes
    {
        private string[] _prefix;

        [XmlElement("Prefix")]
        public string[] Prefix
        {
            get
            {
                return _prefix;
            }
            set
            {
                _prefix = value;
            }
        }
    }
}
