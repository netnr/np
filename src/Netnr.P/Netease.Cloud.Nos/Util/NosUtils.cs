using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web;
using System.Linq;
using System.Threading.Tasks;

using Netease.Cloud.NOS.Service;

namespace Netease.Cloud.NOS.Util
{
    public static class NosUtils
    {
        /// <summary>
        /// 对象名长度最大限制， 1000
        /// </summary>
        public const int OBJECT_NAME_LENGTH_LIMIT = 1000;

        /// <summary>
        /// 一次最多删除的文件数，1000
        /// </summary>
        public const int OBJECTS_UPPER_LIMIT = 1000;

        /// <summary>
        /// 分片个数限制,10000
        /// </summary>
        public const int PART_NUMBER_UPPER_LIMIT = 10000;

        /// <summary>
        /// 最多返回的文件数,1000
        /// </summary>
        public const int MAX_RETURNED_KEYS = 1000;

        /// <summary>
        /// 默认返回的文件数
        /// </summary>
        public const int DEFAULT_RETURNED_KEYS = 100;

        /// <summary>
        /// 默认和最大返回的分块数
        /// </summary>
        public const int DEFAULT_MAX_PARTS = 1000;

        /// <summary>
        /// 默认和最大返回的分块上传事件数
        /// </summary>
        public const int DEFAULT_MAX_UPLOADS = 1000;

        /// <summary>
        /// 最大分片大小,100M
        /// </summary>
        public const long MAX_PART_SIZE = 100 * 1024 * 1024;

        internal static void CheckCredentials(string accessKeyId, string accessKeySecret)
        {
            if (string.IsNullOrEmpty(accessKeyId))
                throw new ArgumentException("Your accessKeyId is null or empty", "accessKeyId");
            if (string.IsNullOrEmpty(accessKeySecret))
                throw new ArgumentException("Your accessKeySecret is null or empty", "accessKeySecret");
        }

        internal static void CheckBucketName(string bucket)
        {
            if (string.IsNullOrEmpty(bucket))
                throw new ArgumentException("Bucket name cannot be null.");
            if (!bucket.ToLower().Equals(bucket))
                throw new ArgumentException("Bucket name should not contain uppercase characters.");
            if (bucket.Contains("_"))
                throw new ArgumentException("Bucket name should not contain '_'.");
            if (bucket.Contains("!") || bucket.Contains("@") || bucket.Contains("#"))
                throw new ArgumentException("Bucket name contains illegal characters.");
            if (bucket.Length < 3 || bucket.Length > 63)
                throw new ArgumentException("Bucket name should be between 3 and 63 characters long.");
            if (bucket.EndsWith("-") || bucket.EndsWith("."))
                throw new ArgumentException("Bucket name should not end with '-' or '.'.");
            if (bucket.Contains(".."))
                throw new ArgumentException("Bucket name should not contain two adjacent periods.");
            if (bucket.Contains("-.") || bucket.Contains("._"))
                throw new ArgumentException("Bucket name should not contain dashes next to periods.");
        }

        internal static void CheckObjectName(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Object name connot be null.");

            var byteCount = Encoding.GetEncoding("utf-8").GetByteCount(key);
            if (byteCount > OBJECT_NAME_LENGTH_LIMIT)
                throw new ArgumentException("Bucket name should be between 1 and 1000 bytes long.");

            if (key.StartsWith("/") || key.StartsWith("\\"))
                throw new ArgumentException("Object name should not be starts with '\' or '//'");
        }

        internal static string UrlEncodeKey(string key)
        {
            const char separator = '/';
            var segments = key.Split(separator);

            var encodedKey = new StringBuilder();
            encodedKey.Append(HttpUtils.EncodeUri(segments[0], "utf-8"));
            for (var i = 1; i < segments.Length; i++)
                encodedKey.Append(separator).Append(HttpUtils.EncodeUri(segments[i], "utf-8"));

            return encodedKey.ToString();
        }

        internal static ClientConfiguration GetClientConfiguration(IServiceClient serviceClient)
        {
            var outerClient = (RetryableServiceClient)serviceClient;
            var innerClient = (ServiceClient)outerClient.InnerServiceClient();
            return innerClient.ClientConfiguration;
        }

        internal static string MakeResourcePath(Uri endpoint, string bucket, string key)
        {
            String resourcePath = (key == null) ? string.Empty : key;

            if (IsIp(endpoint))
            {
                resourcePath = bucket + "/" + resourcePath.Replace("/","%2F");
            }
            return UrlEncodeKey(resourcePath);
        }

        internal static Uri MakeEndpoint(Uri endpoint, string bucket, ClientConfiguration clientConfiguration)
        {
            return new Uri(endpoint.Scheme + "://"
                            + ((bucket != null && !clientConfiguration.IsSubdomain && !IsIp(endpoint))
                            ? (bucket + "." + endpoint.Host) : endpoint.Host)
                            + ((endpoint.Port != 80) ? (":" + endpoint.Port) : ""));

        }

        private static bool IsIp(Uri endpoint)
        {
            return Regex.IsMatch(endpoint.Host, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }

        public static string TrimQuotes(string eTag)
        {
            return eTag != null ? eTag.Trim('\"') : null;
        }

        internal static bool IsPartNumberInRange(int? partNumber)
        {
            return (partNumber.HasValue && partNumber > 0
                && partNumber <= NosUtils.PART_NUMBER_UPPER_LIMIT);
        }

        internal static bool IsPartSizeInRange(long partSize)
        {
            return (Convert.ToString(partSize) != "" && partSize >= 0 && partSize <= NosUtils.MAX_PART_SIZE);
        }

        internal static string XmlStreamToMd5String(Stream stream)
        {
            stream.Position = 0;
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            string content = reader.ReadToEnd();
            content = content.Replace("\r", "");
            return content;
        }

        public static string GetMD5FromXmlString(string myString)
        {
            MD5 md5 = MD5.Create();
            byte[] fromData = System.Text.Encoding.UTF8.GetBytes(myString);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = "";

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += System.Convert.ToString(targetData[i], 16).PadLeft(2, '0');
            }

            return byte2String;
        }

        public static string GetMD5FromStream(Stream content)
        {
            try
            {
                long pos = content.Position;
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(content);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                content.Seek(pos, SeekOrigin.Begin);
                return sb.ToString();                
            }
            catch (Exception ex)
            {
                throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
            }
        }

        public static string GetMD5FromStreamGet(Stream content)
        {
            try
            {
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(content);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();                
            }
            catch (Exception ex)
            {
                throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
            }
        }
    }
}
