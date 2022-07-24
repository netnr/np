using Netease.Cloud.NOS;
using Newtonsoft.Json.Linq;
using Qiniu.Storage;
using Qiniu.Util;
using System.Security.Cryptography;

namespace Netnr.Blog.Web.Controllers
{
    /// <summary>
    /// 对象存储
    /// </summary>
    [Apps.FilterConfigs.IsAdmin]
    public class StorageController : Controller
    {
        /// <summary>
        /// 七牛云 对象存储 KODO
        /// </summary>
        /// <returns></returns>
        public IActionResult Qiniu()
        {
            return View("/Views/Storage/_PartialStorage.cshtml");
        }

        /// <summary>
        /// 网易数帆 对象存储 NOS
        /// </summary>
        /// <returns></returns>
        public IActionResult NetEase()
        {
            return View("/Views/Storage/_PartialStorage.cshtml");
        }

        /// <summary>
        /// 腾讯云 对象存储 COS
        /// </summary>
        /// <returns></returns>
        public IActionResult Tencent()
        {
            return View("/Views/Storage/_PartialStorage.cshtml");
        }

        /// <summary>
        /// 对象存储接口
        /// </summary>
        /// <param name="id">服务商</param>
        /// <param name="sid">命令</param>
        /// <returns></returns>
        public IActionResult API([FromRoute] string id, [FromRoute] string sid)
        {
            //参数
            var bucket = Request.Query["Bucket"].ToString();
            var endpoint = Request.Query["Endpoint"].ToString();
            var prefix = Request.Query["Prefix"].ToString();
            var marker = Request.Query["Marker"].ToString();
            if (!int.TryParse(Request.Query["Limit"], out var limit))
            {
                limit = 1000;
            }
            var delimiter = Request.Query["Delimiter"].ToString();
            var key = Request.Query["key"].ToString();
            var keys = Request.Query["keys"].ToString();
            var newKey = Request.Query["newKey"].ToString();

            var secretId = GlobalTo.GetValue($"StorageKey:{id}:SecretId");
            var secretKey = GlobalTo.GetValue($"StorageKey:{id}:SecretKey");

            try
            {
                switch (id?.ToLower())
                {
                    case "key":
                        {
                            var result = System.IO.File.ReadAllText(GlobalTo.ContentRootPath + "/appsettings.json").ToJObject()["StorageKey"].ToJson();
                            return Content(result);
                        }
                    case "qiniu":
                        {
                            // 设置存储区域
                            Config config = new();
                            config.Zone = Zone.ZONE_CN_East;
                            Mac mac = new(secretId, secretKey);
                            BucketManager bucketManager = new(mac, config);

                            switch (sid)
                            {
                                case "upload-token":
                                    {
                                        PutPolicy putPolicy = new();
                                        putPolicy.Scope = bucket;
                                        putPolicy.SetExpires(3600 * 7);
                                        string token = Auth.CreateUploadToken(mac, putPolicy.ToJsonString());
                                        return Content(token);
                                    }
                                case "list":
                                    {
                                        var result = bucketManager.ListFiles(bucket, prefix, marker, limit, delimiter);
                                        return Content(result.ToJson());
                                    }
                                case "delete":
                                    {
                                        var result = new List<object>();
                                        keys.ToString().Split(',').ToList().ForEach(x =>
                                        {
                                            var r = bucketManager.Delete(bucket, x);
                                            result.Add(new
                                            {
                                                Key = x,
                                                Result = r
                                            });
                                        });
                                        return Content(result.ToJson());
                                    }
                                case "move":
                                    {
                                        var result = bucketManager.Move(bucket, key, bucket, newKey);
                                        return Content(result.ToJson());
                                    }
                            }
                        }
                        break;
                    case "netease":
                        {
                            var nosClient = new NosClient(endpoint, secretId, secretKey, new ClientConfiguration()
                            {
                                MaxErrorRetry = 2,
                                MaxConnections = 200,
                                ConnectionTimeout = 50000
                            });

                            switch (sid)
                            {
                                case "upload-token":
                                    {
                                        var putPolicy = new JObject
                                        {
                                            ["Bucket"] = bucket,
                                            ["Object"] = key,
                                            ["Expires"] = DateTime.Now.AddHours(7).ToTimestamp() // Unix时间，单位：秒
                                        };
                                        var encodedPutPolicy = putPolicy.ToJson().ToBase64Encode();
                                        using var algorithm = new HMACSHA256();
                                        algorithm.Key = secretKey.ToByte();
                                        var encodedSign = Convert.ToBase64String(algorithm.ComputeHash(encodedPutPolicy.ToByte()));
                                        var result = $"UPLOAD {secretId}:{encodedSign}:{encodedPutPolicy}";
                                        return Content(result);
                                    }
                                case "list":
                                    {
                                        var result = nosClient.ListObjects(new ListObjectsRequest(bucket)
                                        {
                                            MaxKeys = limit,
                                            Prefix = prefix,
                                            Delimiter = delimiter
                                        });
                                        return Content(result.ToJson());
                                    }
                                case "delete":
                                    {
                                        keys.ToString().Split(',').ToList().ForEach(x =>
                                        {
                                            nosClient.DeleteObject(bucket, x);
                                        });
                                        return Ok();
                                    }
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(ex.Message);
            }

            return Ok();
        }
    }
}