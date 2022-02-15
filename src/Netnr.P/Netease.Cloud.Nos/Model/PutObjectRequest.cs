using System;
using System.Collections.Generic;
using System.IO;

using Netease.Cloud.NOS.Util;

namespace Netease.Cloud.NOS
{
    public class PutObjectRequest
    {
        /// <summary>
        /// 获取或设置<see cref="NosObject"/>所在<see cref="Bucket"/>的名称
        /// </summary>
        public string Bucket { get; private set; }

        /// <summary>
        /// 获取或设置需要上传的<see cref="NosObject"/>的Key
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// 获取或设置上传文件的路径
        /// </summary>
        public string FileToUpload { get; private set; }

        /// <summary>
        /// 获取或设置上传对象的内容
        /// </summary>
        public Stream Content { get; set; }

        /// <summary>
        /// 获取或设置对象的元信息
        /// </summary>
        public ObjectMetadata ObjectMetadata { get; set; }

        /// <summary>
        /// 获取或设置对象的存储级别
        /// </summary>
        public string StorageClass { get; set; }

        /// <summary>
        /// 构造一个流式上传的<see cref="PutObjectRequest"/>实例，带对象元数据.
        /// </summary>
        /// <param name="bucket">桶名</param>
        /// <param name="key">对象名</param>
        /// <param name="content">流内容</param>
        /// <param name="objectMetadata">对象元数据</param>
        public PutObjectRequest(string bucket, string key, Stream content, ObjectMetadata objectMetadata)
        {
            NosUtils.CheckBucketName(bucket);
            NosUtils.CheckObjectName(key);

            this.Bucket = bucket;
            this.Key = key;
            this.Content = content;
            this.ObjectMetadata = objectMetadata;
        }

        /// <summary>
        /// 构造一个流式上传的<see cref="PutObjectRequest"/>实例.
        /// </summary>
        /// <param name="bucket">桶名</param>
        /// <param name="key">对象名</param>
        /// <param name="content">流内容</param>
        public PutObjectRequest(string bucket, string key, Stream content)
            : this(bucket, key, content, null)
        { }

        /// <summary>
        /// 构造一个上传文件的<see cref="PutObjectRequest"/>实例，带对象元数据
        /// </summary>
        /// <param name="bucket">桶名</param>
        /// <param name="key">对象名</param>
        /// <param name="fileToUpload">上传的文件</param>
        /// <param name="objectMetadata">对象元数据</param>
        public PutObjectRequest(string bucket, string key, string fileToUpload, ObjectMetadata objectMetadata)
        {
            NosUtils.CheckBucketName(bucket);
            NosUtils.CheckObjectName(key);

            this.Bucket = bucket;
            this.Key = key;
            this.FileToUpload = fileToUpload;
            this.ObjectMetadata = objectMetadata;
        }

        /// <summary>
        /// 构造一个上传文件的<see cref="PutObjectRequest"/>实例
        /// </summary>
        /// <param name="bucket">桶名</param>
        /// <param name="key">对象名</param>
        /// <param name="fileToUpload">上传的文件</param>
        public PutObjectRequest(string bucket, string key, string fileToUpload)
            : this(bucket, key, fileToUpload, null)
        { }
    }
}
