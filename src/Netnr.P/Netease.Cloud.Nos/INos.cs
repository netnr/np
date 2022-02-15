using System;
using System.Collections.Generic;
using System.IO;

using Netease.Cloud.NOS.Model;

namespace Netease.Cloud.NOS
{
    public interface INos
    {
        /// <summary>
        /// 删除指定<see cref="NosObject"/>
        /// </summary>
        /// <param name="bucket"><see cref="Bucket"/>的名称</param>
        /// <param name="key">对象的名称</param>
        void DeleteObject(string bucket, string key);

        /// <summary>
        /// 删除指定<see cref="NosObject"/>
        /// </summary>
        /// <param name="deleteObjectRequest"><see cref="DeleteObjectRequest"/>实例</param>
        void DeleteObject(DeleteObjectRequest deleteObjectRequest);

        /// <summary>
        /// 删除多个<see cref="NosObject"/>
        /// </summary>
        /// <param name="bucket"><see cref="Bucket"/>的名称</param>
        /// <param name="keys">需删除的对象列表</param>
        /// <param name="quiet">删除模式（true表示静默模式，false表示详细模式）</param>
        /// <returns></returns>
        DeleteObjectsResult DeleteObjects(string bucket, IList<string> keys, bool quiet);

        /// <summary>
        /// 删除多个<see cref="NosObject"/>
        /// </summary>
        /// <param name="deleteObjectsRequest"><see cref="DeleteObjectsRequest"/>实例</param>
        /// <returns><see cref="DeleteObjectsResult"/>实例</returns>
        DeleteObjectsResult DeleteObjects(DeleteObjectsRequest deleteObjectsRequest);

        /// <summary>
        /// 从指定的<see cref="Bucket" />中下载指定的<see cref="OssObject" />。
        /// </summary>
        /// <param name="bucket"><see cref="Bucket"/>的名称</param>
        /// <param name="key">需要下载的对象名称</param>
        /// <returns><see cref="NosObject"/>实例</returns>
        NosObject GetObject(string bucket, string key);

        /// <summary>
        /// 下载指定的对象到指定的文件
        /// </summary>
        /// <param name="bucket">桶名</param>
        /// <param name="key">对象名</param>
        /// <param name="fireToDownload">对象下载到的文件</param>
        /// <returns><see cref="NosObject实例"/></returns>
        ObjectMetadata GetObject(string bucket, string key, string fireToDownload);

        /// <summary>
        /// 下载满足请求参数<see cref="GetObjectRequest"/>的<see cref="OssObject" />
        /// </summary>
        /// <param name="getObjectRequest"><see cref="GetObjectRequest"/>实例</param>
        /// <returns><see cref="NosObject"/>实例</returns>
        NosObject GetObject(GetObjectRequest getObjectRequest);

        /// <summary>
        /// 下载指定对象到指定文件
        /// </summary>
        /// <param name="getObjectRequest"></param>
        /// <param name="fileToDownload"></param>
        /// <returns></returns>
        ObjectMetadata GetObject(GetObjectRequest getObjectRequest, string fileToDownload);

        /// <summary>
        /// 获取<see cref="OssObject" />的元信息。
        /// </summary>
        /// <param name="backet"><see cref="Bucket"/>的名称</param>
        /// <param name="key">>需要获取元信息的对象名称</param>
        /// <returns><see cref="ObjectMetadata"/>实例</returns>
        ObjectMetadata GetObjectMetadata(string backet, string key);

        /// <summary>
        /// 获取<see cref="OssObject" />的元信息。
        /// </summary>
        /// <param name="getObjectMetadataRequest"><see cref="GetObjectMetadataRequest"/>实例</param>
        /// <returns><see cref="ObjectMetadata"/>实例</returns>
        /// <returns></returns>
        ObjectMetadata GetObjectMetadata(GetObjectMetadataRequest getObjectMetadataRequest);

        /// <summary>
        /// 上传<see cref="OssObject" />
        /// </summary>
        /// <param name="bucket">see cref="Bucket"/>的名称</param>
        /// <param name="key">需上传的对象名称</param>
        /// <param name="content">需上传的对象的内容</param>
        /// <returns><see cref="PutObjectResult"/>实例</returns>
        PutObjectResult PutObject(string bucket, string key, Stream content);

        /// <summary>
        /// 上传<see cref="OssObject" />
        /// </summary>
        /// <param name="bucket">see cref="Bucket"/>的名称</param>
        /// <param name="key">需上传的对象名称</param>
        /// <param name="content">需上传的对象的内容</param>
        /// <param name="objectMetadata">需上传的对象的元信息</param>
        /// <returns><see cref="PutObjectResult"/>实例</returns>
        PutObjectResult PutObject(string bucket, string key, Stream content, ObjectMetadata objectMetadata);

        /// <summary>
        /// 上传<see cref="OssObject" />(文件)
        /// </summary>
        /// <param name="bucket">see cref="Bucket"/>的名称</param>
        /// <param name="key">需上传的对象名称</param>
        /// <param name="fileToUpload">需上传的文件路径</param>
        /// <returns><see cref="PutObjectResult"/>实例</returns>
        PutObjectResult PutObject(string bucket, string key, string fileToUpload);

        /// <summary>
        /// 上传<see cref="OssObject" />(文件)
        /// </summary>
        /// <param name="bucket">see cref="Bucket"/>的名称</param>
        /// <param name="key">需上传的对象名称</param>
        /// <param name="fileToUpload">需上传的文件路径</param>
        /// <param name="objectMetadata">需上传的文件的元信息</param>
        /// <returns><see cref="PutObjectResult"/>实例</returns>
        PutObjectResult PutObject(string bucket, string key, string fileToUpload, ObjectMetadata objectMetadata);

        /// <summary>
        /// 列出指定<see cref="Bucket" />下<see cref="OssObject" />的列表
        /// </summary>
        /// <param name="bucket"><see cref="Bucket" />的名称。</param>
        /// <returns><see cref="OssObject" />的列表信息</returns>
        ObjectListing ListObjects(string bucket);

        /// <summary>
        /// 列出指定<see cref="Bucket" />下<see cref="OssObject" />的列表
        /// </summary>
        /// <param name="listObjectsRequest"><see cref="ListObjectsRequest"/>实例</param>
        /// <returns>对象列表信息</returns>
        ObjectListing ListObjects(ListObjectsRequest listObjectsRequest);

        /// <summary>
        /// 拷贝一个Object
        /// </summary>
        /// <param name="sourceBucket">源Object所在的Bucket的名称</param>
        /// <param name="sourceKey">源Object的Key</param>
        /// <param name="destinationBucket">目标Object所在的Bucket的名称</param>
        /// <param name="destinationKey">源Object的Key</param>
        void CopyObject(string sourceBucket, string sourceKey, string destinationBucket, string destinationKey);

        /// <summary>
        /// 拷贝一个Object
        /// </summary>
        /// <param name="copyObjectRequest"><see cref="CopyObjectRequest"/>实例</param>
        void CopyObject(CopyObjectRequest copyObjectRequest);

        /// <summary>
        /// 桶内move一个Object
        /// </summary>
        /// <param name="sourceBucket">源Object所在的Bucket的名称</param>
        /// <param name="sourceKey">源Object的Key</param>
        /// <param name="destinationBucket">目标Object所在的Bucket的名称</param>
        /// <param name="destinationKey">源Object的Key</param>
        void MoveObject(string sourceBucket, string sourceKey, string destinationBucket, string destinationKey);

        /// <summary>
        ///  桶内move一个Object
        /// </summary>
        /// <param name="moveObjectRequest"><see cref="MoveObjectRequest"/>实例</param>
        void MoveObject(MoveObjectRequest moveObjectResquest);

        /// <summary>
        /// 初始化分块上传
        /// </summary>
        /// <param name="initiateMultipartUploadRequest"><see cref="initiateMultipartUploadRequest"/>实例</param>
        /// <returns><see cref="InitiateMultipartUploadRequest"/>实例</returns>
        InitiateMultipartUploadResult InitiateMultipartUpload(InitiateMultipartUploadRequest initiateMultipartUploadRequest);

        /// <summary>
        /// 分块上传数据
        /// </summary>
        /// <param name="uploadPartRequest"><see cref="UploadPartRequest"/></param>
        /// <returns><see cref="UploadPartResult"/>实例</returns>
        UploadPartResult UploadPart(UploadPartRequest uploadPartRequest);

        /// <summary>
        /// 完成分块上传
        /// </summary>
        /// <param name="completeMultipartUploadRequest"><see cref="CompleteMultipartUploadRequest"/>实例</param>
        /// <returns><see cref="CompleteMultipartUploadResult" />实例</returns>        
        CompleteMultipartUploadResult CompleteMultipartUpload(CompleteMultipartUploadRequest completeMultipartUploadRequest);

        /// <summary>
        ///  取消分块上传并删除已上传的分块
        /// </summary>
        /// <param name="abortMultipartUploadRequest"><see cref="AbortMultipartUploadRequest"/></param>
        void AbortMultipartUpload(AbortMultipartUploadRequest abortMultipartUploadRequest);

        /// <summary>
        /// 列出已上传的分块
        /// </summary>
        /// <param name="listPartsRequest"><see cref=ListPartsRequest""/>实例</param>
        /// <returns><see cref="PartListing"/>实例</returns>
        PartListing ListParts(ListPartsRequest listPartsRequest);

        /// <summary>
        /// 列出所有执行中的分块上传事件
        /// </summary>
        /// <param name="listMultipartUploadsRequest"><see cref="ListMultipartUploadsRequest"/>实例</param>
        /// <returns><see cref="MultipartUploadListing"/></returns>
        MultipartUploadListing ListMultipartUploads(ListMultipartUploadsRequest listMultipartUploadsRequest);

        /// <summary>
        /// 判断对象是否存在
        /// </summary>
        /// <param name="bucket">桶名</param>
        /// <param name="key">对象名</param>
        /// <returns></returns>
        bool DoesObjectExist(string bucket, string key);
    }
}
