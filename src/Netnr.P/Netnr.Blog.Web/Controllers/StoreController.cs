using Netease.Cloud.NOS;
using Netnr.SharedFast;

namespace Netnr.Blog.Web.Controllers
{
    /// <summary>
    /// 存储
    /// </summary>
    [Apps.FilterConfigs.IsAdmin]
    public class StoreController : Controller
    {
        /// <summary>
        /// 存储首页
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }

        #region 163yun.com NOS 对象存储

        /// <summary>
        /// 秘钥
        /// </summary>
        public class AccessNOS
        {
            public static string AccessKeyId => GlobalTo.GetValue("ApiKey:AccessNOS:AccessKeyId");
            public static string AccessKeySecret => GlobalTo.GetValue("ApiKey:AccessNOS:AccessKeySecret");
            public static string EndPoint => GlobalTo.GetValue("ApiKey:AccessNOS:Endpoint");
        }

        /// <summary>
        /// NOS 对象存储
        /// </summary>
        /// <returns></returns>
        public IActionResult NENos()
        {
            return View();
        }

        /// <summary>
        /// NEAPI
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public string NEAPI([FromRoute] string id)
        {
            string bucket = Request.Form["bucket"].ToString();
            string result = "fail";

            var nosClient = new NosClient(AccessNOS.EndPoint, AccessNOS.AccessKeyId, AccessNOS.AccessKeySecret, new ClientConfiguration()
            {
                MaxErrorRetry = 2,
                MaxConnections = 200,
                ConnectionTimeout = 50000
            });

            switch (id?.ToLower())
            {
                case "ak":
                    {
                        result = new
                        {
                            AccessKey = AccessNOS.AccessKeyId,
                            SecretKey = AccessNOS.AccessKeySecret
                        }.ToJson();
                    }
                    break;

                #region 列举文件
                case "list":
                    var listObjectsRequest = new ListObjectsRequest(bucket)
                    {
                        MaxKeys = 1000,
                        Prefix = Request.Form["keywords"].ToString()
                    };
                    try
                    {
                        ObjectListing objList = nosClient.ListObjects(listObjectsRequest);
                        result = objList.ObjectSummarise.ToJson();
                    }
                    catch (Exception ex)
                    {
                        Apps.FilterConfigs.LogWrite(HttpContext, ex);
                        result = "[]";
                    }
                    break;
                #endregion

                #region 存在文件
                case "exists":
                    try
                    {
                        string key = Request.Form["key"].ToString();
                        bool b = nosClient.DoesObjectExist(bucket, key);
                        result = b ? "1" : "0";
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        result = "error";
                    }
                    break;
                #endregion

                #region 删除文件
                case "del":
                    try
                    {
                        var keys = Request.Form["key"].ToString().Split(',').ToList();
                        if (keys.Count == 1)
                        {
                            nosClient.DeleteObject(bucket, keys[0]);
                        }
                        else if (keys.Count > 1)
                        {
                            nosClient.DeleteObjects(bucket, keys, false);
                        }
                        result = "success";
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        result = "fail";
                    }
                    break;
                    #endregion
            }

            return result;
        }

        #endregion
    }
}