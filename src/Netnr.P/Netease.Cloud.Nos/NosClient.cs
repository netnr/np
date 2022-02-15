using System;
using System.IO;
using System.Net;
using System.Globalization;
using System.Collections.Generic;

using Netease.Cloud.NOS.Authentication;
using Netease.Cloud.NOS.Service;
using Netease.Cloud.NOS.Commands;
using Netease.Cloud.NOS.Handlers;
using Netease.Cloud.NOS.Model;
using Netease.Cloud.NOS.Util;

namespace Netease.Cloud.NOS
{
    public class NosClient : INos
    {
        #region Fields & Properties

        private volatile Uri _endpoint;
        private readonly ICredentialsFactory _credentialsFactory;
        private readonly IServiceClient _serviceClient;

        public void SetEndpoint(Uri endpoint)
        {
            _endpoint = endpoint;
        }

        #endregion

        #region Constructors

        /// <summary>
        ///  配置构造一个<see cref="NOSClient"/>实例
        /// </summary>
        /// <param name="endpoint">/param>
        /// <param name="credentialsFactory">Credentials工厂实例</param>
        /// <param name="clientConfiguration">客户端配置</param>
        public NosClient(string endpoint, ICredentialsFactory credentialsFactory, ClientConfiguration clientConfiguration)
        {
            if (endpoint == null)
                throw new ArgumentException("The endpoint is null or empty.", "endpoint");

            if (endpoint.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("The enpoint do not include transport protocols.");

            if (clientConfiguration != null)
            {
                if (!endpoint.ToString().StartsWith("http") && clientConfiguration.Protocol.Equals(Protocol.Https))
                    endpoint = "https://" + endpoint.Trim();

                if (!endpoint.ToString().StartsWith("http") && clientConfiguration.Protocol.Equals(Protocol.Http))
                    endpoint = "http://" + endpoint.Trim();
            }
            else
            {
                endpoint = "http://" + endpoint.Trim();
            }

            if (!endpoint.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                && !endpoint.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Endpoint not supported Protocal", "endpoint");

            if (credentialsFactory == null)
                throw new ArgumentException("The Credentials is null or empty.", "credentialsFactory");

            _endpoint = new Uri(endpoint);
            _credentialsFactory = credentialsFactory;
            _serviceClient = ServiceClientFactory.CreateSreviceClient(clientConfiguration ?? new ClientConfiguration());
        }

        /// <summary>
        ///  配置构造一个<see cref="NOSClient"/>实例
        /// </summary>
        /// <param name="endpoint">NOS访问地址</param>
        /// <param name="accessKeyId">NOS的访问ID</param>
        /// <param name="accessKeySecret">NOS访问密钥</param>
        /// <param name="clientConfiguration">客户端配置</param>
        public NosClient(string endpoint, string accessKeyId, string accessKeySecret, ClientConfiguration clientConfiguration)
            : this(endpoint, new DefaultCredentialsFactory(new DefaultCredentials(accessKeyId, accessKeySecret)), clientConfiguration)
        { }


        /// <summary>
        /// 配置构造一个<see cref="NOSClient"/>实例
        /// </summary>
        /// <param name="endpoint">NOS访问地址</param>
        /// <param name="accessKeyId">NOS的访问ID</param>
        /// <param name="accessKeySecret">NOS访问密钥</param>
        public NosClient(string endpoint, string accessKeyId, string accessKeySecret)
            : this(endpoint, accessKeyId, accessKeySecret, null)
        { }

        #endregion

        #region object Operations

        public void DeleteObject(string bucket, string key)
        {
            var command = DeleteObjectCommand.Create(_serviceClient, _endpoint,
                                                    CreateContext(HttpMethod.DELETE, bucket, key),
                                                    bucket, key);
            command.Execute();
        }

        public void DeleteObject(DeleteObjectRequest deleteObjectRequest)
        {
            var command = DeleteObjectCommand.Create(_serviceClient, _endpoint,
                                                    CreateContext(HttpMethod.DELETE, deleteObjectRequest.Bucket, deleteObjectRequest.Key),
                                                    deleteObjectRequest.Bucket, deleteObjectRequest.Key);
            command.Execute();
        }

        public DeleteObjectsResult DeleteObjects(string bucket, IList<string> keys, bool quiet)
        {
            var deleteObjectsRequest = new DeleteObjectsRequest(bucket, keys, quiet);
            return DeleteObjects(deleteObjectsRequest);
        }

        public DeleteObjectsResult DeleteObjects(DeleteObjectsRequest deleteObjectsRequest)
        {
            ThrowIfNullRequest(deleteObjectsRequest);

            var command = DeleteObjectsCommand.Create(_serviceClient, _endpoint,
                                                        CreateContext(HttpMethod.POST, deleteObjectsRequest.Bucket, null),
                                                        deleteObjectsRequest);
            return command.Execute();
        }


        public ObjectMetadata GetObjectMetadata(string bucket, string key)
        {
            var getObjectMetadataRequest = new GetObjectMetadataRequest(bucket, key);
            return GetObjectMetadata(getObjectMetadataRequest);                
        }

        public ObjectMetadata GetObjectMetadata(GetObjectMetadataRequest getObjectMetadataRequest)
        {
            ThrowIfNullRequest(getObjectMetadataRequest);

            var command = GetObjectMetadataCommand.Create(_serviceClient, _endpoint,
                                                     CreateContext(HttpMethod.HEAD, getObjectMetadataRequest.Bucket, getObjectMetadataRequest.Key),
                                                     getObjectMetadataRequest);
            return command.Execute();    
        }

        public ObjectListing ListObjects(string bucket)
        {
            var listObjectsRequest = new ListObjectsRequest(bucket);
            return ListObjects(listObjectsRequest);
        }

        public ObjectListing ListObjects(ListObjectsRequest listObjectsRequest)
        {
            ThrowIfNullRequest(listObjectsRequest);

            var command = ListObjectsCommand.Create(_serviceClient, _endpoint,
                                                    CreateContext(HttpMethod.GET, listObjectsRequest.Bucket, null),
                                                    listObjectsRequest);
            return command.Execute();
        }

        public NosObject GetObject(string bucket, string key)
        {
            var getObjectRequest = new GetObjectRequest(bucket, key);
            return GetObject(getObjectRequest);
        }

        public NosObject GetObject(GetObjectRequest getObjectRequest)
        {
            ThrowIfNullRequest(getObjectRequest);

            var command = GetObjectCommand.Create(_serviceClient, _endpoint,
                                             CreateContext(HttpMethod.GET, getObjectRequest.Bucket, getObjectRequest.Key),
                                             getObjectRequest);
            return command.Execute();
        }

        public ObjectMetadata GetObject(GetObjectRequest getObjectRequest, string fileToDownload)
        {
            var getObjectResult = GetObject(getObjectRequest);
            using (var requestStream = getObjectResult.Content)
            {
                using (FileStream fs = File.Open(fileToDownload, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
                {
                    fs.Seek(0, SeekOrigin.Begin);
                    fs.SetLength(0); //清空文件
                    int length = 4 * 1024;
                    byte[] buf = new byte[length];
                    do
                    {
                        length = requestStream.Read(buf, 0, length);
                        fs.Write(buf, 0, length);
                    } while (length != 0);
                    fs.Close();
                }
            }

            return getObjectResult.objectMetadata;
        }

        public ObjectMetadata GetObject(string bucket, string key, string fileToDownload)
        {
            var getObjectRequest = new GetObjectRequest(bucket, key);
            return GetObject(getObjectRequest, fileToDownload);
        }

        public PutObjectResult PutObject(string bucketName, string key, Stream content)
        {
            return PutObject(bucketName, key, content, null);
        }

        public PutObjectResult PutObject(string bucket, string key, Stream content, ObjectMetadata objectMetadata)
        {
            objectMetadata = objectMetadata ?? new ObjectMetadata();
            SetContentTypeIfNull(key, null, ref objectMetadata);

            var cmd = PutObjectCommand.Create(_serviceClient, _endpoint,
                                             CreateContext(HttpMethod.PUT, bucket, key),
                                             bucket, key, content, objectMetadata);
            return cmd.Execute();
        }

        public PutObjectResult PutObject(string bucketName, string key, string fileToUpload)
        {
            return PutObject(bucketName, key, fileToUpload, null);
        }

        public PutObjectResult PutObject(string bucketName, string key, string fileToUpload, ObjectMetadata objectMetadata)
        {
            if (!File.Exists(fileToUpload) || Directory.Exists(fileToUpload))
                throw new ArgumentException(String.Format("Invalid file path {0}.", fileToUpload));

            objectMetadata = objectMetadata ?? new ObjectMetadata();
            SetContentTypeIfNull(key, fileToUpload, ref objectMetadata);

            PutObjectResult result;
            using (Stream content = File.OpenRead(fileToUpload))
            {
                result = PutObject(bucketName, key, content, objectMetadata);
            }
            return result;
        }

        public void CopyObject(string sourceBucket, string sourceKey, string destinationBucket, string destinationKey)
        {
            var copyObjectRequest = new CopyObjectRequest(sourceBucket, sourceKey, destinationBucket, destinationKey);
            CopyObject(copyObjectRequest);
        }

        public void CopyObject(CopyObjectRequest copyObjectRequest)
        {
            ThrowIfNullRequest(copyObjectRequest);

            var command = CopyObjectCommand.Create(_serviceClient, _endpoint,
                                                    CreateContext(HttpMethod.PUT, copyObjectRequest.DestinationBucket, copyObjectRequest.DestinationKey),
                                                    copyObjectRequest);
            command.Execute();
        }

        public void MoveObject(string sourceBucket, string sourceKey, string destinationBucket, string destinationKey)
        {
            var moveObjectRequest = new MoveObjectRequest(sourceBucket, sourceKey, destinationBucket, destinationKey);
            MoveObject(moveObjectRequest);
        }

        public void MoveObject(MoveObjectRequest moveObjectResquest)
        {
            ThrowIfNullRequest(moveObjectResquest);

            var command = MoveObjectCommand.Create(_serviceClient, _endpoint,
                                                    CreateContext(HttpMethod.PUT, moveObjectResquest.DestinationBucket, moveObjectResquest.DestinationKey),
                                                    moveObjectResquest);
            command.Execute();
        }

        public bool DoesObjectExist(string bucket, string key)
        {
            try
            {
                var getObjectMetadataRequest = new GetObjectMetadataRequest(bucket, key);
                var command = GetObjectMetadataCommand.Create(_serviceClient, _endpoint,
                                                  CreateContext(HttpMethod.HEAD, bucket, key),
                                                  getObjectMetadataRequest);

                command.Execute();
            }
            catch (NosException e)
            {
                if (e.ErrorCode == "NoSuchBucket" || e.ErrorCode == "NoSuchKey")
                {
                    return false;
                }
                throw;
            }
            catch (WebException ex)
            {
                HttpWebResponse errorResponse = ex.Response as HttpWebResponse;
                if (errorResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    return false;
                }
                throw;
            }
            return true;
        }

        #endregion

        #region Multipart Operations

        public InitiateMultipartUploadResult InitiateMultipartUpload(InitiateMultipartUploadRequest initiateMultipartUploadRequest)
        {
            ThrowIfNullRequest(initiateMultipartUploadRequest);
            var command = InitiateMultipartUploadCommand.Create(_serviceClient, _endpoint,
                                                           CreateContext(HttpMethod.POST, initiateMultipartUploadRequest.Bucket, initiateMultipartUploadRequest.Key),
                                                           initiateMultipartUploadRequest);
            return command.Execute();
        }

        public UploadPartResult UploadPart(UploadPartRequest uploadPartRequest)
        {
            ThrowIfNullRequest(uploadPartRequest);
            var command = UploadPartCommand.Create(_serviceClient, _endpoint,
                                              CreateContext(HttpMethod.PUT, uploadPartRequest.Bucket, uploadPartRequest.Key),
                                              uploadPartRequest);
            return command.Execute();
        }

        public CompleteMultipartUploadResult CompleteMultipartUpload(CompleteMultipartUploadRequest completeMultipartUploadRequest)
        {
            ThrowIfNullRequest(completeMultipartUploadRequest);
            var command = CompleteMultipartUploadCommand.Create(_serviceClient, _endpoint,
                                                           CreateContext(HttpMethod.POST, completeMultipartUploadRequest.Bucket, completeMultipartUploadRequest.Key),
                                                           completeMultipartUploadRequest);
            return command.Execute();
        }

        public void AbortMultipartUpload(AbortMultipartUploadRequest abortMultipartUploadRequest)
        {
            ThrowIfNullRequest(abortMultipartUploadRequest);
            var command = AbortMultipartUploadCommand.Create(_serviceClient, _endpoint,
                                                        CreateContext(HttpMethod.DELETE, abortMultipartUploadRequest.Bucket, abortMultipartUploadRequest.Key),
                                                        abortMultipartUploadRequest);
            command.Execute();
        }

        public MultipartUploadListing ListMultipartUploads(ListMultipartUploadsRequest listMultipartUploadsRequest)
        {
            ThrowIfNullRequest(listMultipartUploadsRequest);
            var command = ListMultipartUploadsCommand.Create(_serviceClient, _endpoint,
                                                        CreateContext(HttpMethod.GET, listMultipartUploadsRequest.Bucket, null),
                                                        listMultipartUploadsRequest);
            return command.Execute();
        }

        public PartListing ListParts(ListPartsRequest listPartsRequest)
        {
            ThrowIfNullRequest(listPartsRequest);
            var cmd = ListPartsCommand.Create(_serviceClient, _endpoint,
                                             CreateContext(HttpMethod.GET, listPartsRequest.Bucket, listPartsRequest.Key),
                                             listPartsRequest);
            return cmd.Execute();
        }

        #endregion

        #region Private Methods

        private ExecutionContext CreateContext(HttpMethod method, string bucket, string key)
        {
            var contextBuilder = new ExecutionContextBuilder
            {
                Bucket = bucket,
                Key = key,
                Method = method,
                Credentials = _credentialsFactory.GetCredentials()
            };
            contextBuilder.ResponseHandlers.Add(new ErrorResponseHandler());
            return contextBuilder.Build();
        }

        virtual protected void ThrowIfNullRequest<TRequestType>(TRequestType request)
        {
            if (request == null)
                throw new ArgumentNullException("request");
        }

        private static void SetContentTypeIfNull(string key, string fileName, ref ObjectMetadata metadata)
        {
            if (metadata.ContentType == null)
            {
                metadata.ContentType = FileContentType.GetMimeType(key, fileName);
            }
        }

        #endregion

    }
}
