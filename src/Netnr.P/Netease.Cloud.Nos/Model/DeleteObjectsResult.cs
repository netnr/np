using System;
using System.Text;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Globalization;

using Netease.Cloud.NOS.Util;

namespace Netease.Cloud.NOS
{
    /// <summary>
    /// Delete Objects 的请求结果
    /// </summary>
    [XmlRoot("DeleteResult")]
    public class DeleteObjectsResult
    {
        private DeletedObject[] _keys;
        private ErrorObject[] _error;

        /// <summary>
        /// Deleted部分的解析和获取
        /// </summary>
        [XmlElement("Deleted")]
        public DeletedObject[] Keys
        {
            get
            {
                return _keys;
            }
            set
            {
                this._keys = value;
            }
        }

        /// <summary>
        /// Error部分的解析和获取
        /// </summary>
        [XmlElement("Error")]
        public ErrorObject[] Error
        {
            get
            {
                return _error;
            }
            set
            {
                this._error = value;
            }
        }

        /// <summary>
        /// EncodingType值的解析和获取
        /// </summary>
        //[XmlElement("EncodingType")]
        //public string EncodingType { get; set; }

        internal DeleteObjectsResult()
        {
        }

        /// <summary>
        /// Deleted部分的解析和获取
        /// </summary>
        [XmlRoot("Deleted")]
        public class DeletedObject
        {
            /// <summary>
            /// Deleted Key的解析和获取
            /// </summary>
            [XmlElement("Key")]
            public string Key { get; set; }
        }

        [XmlRoot("Error")]
        public class ErrorObject
        {
            [XmlElement("Key")]
            public string Key { get; set; }

            [XmlElement("Code")]
            public string Code { get; set; }

            [XmlElement("Message")]
            public string Message { get; set; }
        }
    }
}
