using Netnr.Core;
using Netnr.SharedFast;
using Newtonsoft.Json.Linq;

namespace Netnr.Blog.Application
{
    /// <summary>
    /// 推送服务
    /// </summary>
    public class PushService
    {
        /// <summary>
        /// 获取 access_token
        /// </summary>
        /// <returns></returns>
        private static string Get_access_token()
        {
            var atkey = "test_access_token";
            if (CacheTo.Get(atkey) is not JObject ato)
            {
                var appID = GlobalTo.GetValue("ApiKey:TestWeChatMP:AppID");
                var appsecret = GlobalTo.GetValue("ApiKey:TestWeChatMP:AppSecret");
                var access_token_url = $"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={appID}&secret={appsecret}";
                Console.WriteLine(access_token_url);
                var access_token_result = HttpTo.Get(access_token_url);
                Console.WriteLine(access_token_result);
                ato = access_token_result.ToJObject();
                CacheTo.Set(atkey, ato, 3600, false);
            }

            var access_token = ato["access_token"].ToString();
            return access_token;
        }

        /// <summary>
        /// 推送测试公众号
        /// https://mp.weixin.qq.com/debug/cgi-bin/sandbox?t=sandbox/login
        /// </summary>
        /// <param name="title"></param>
        /// <param name="msg"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static SharedResultVM Push(string title = "", string msg = "", string url = "")
        {
            return SharedResultVM.Try(vm =>
            {
                var access_token = Get_access_token();

                var template_id = GlobalTo.GetValue("ApiKey:TestWeChatMP:Template_Id");
                var tousers = GlobalTo.GetValue("ApiKey:TestWeChatMP:ToUser");

                var now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                vm.Set(SharedEnum.RTag.success);
                tousers.Split(',').ToList().ForEach(touser =>
                {
                    var post_data = new
                    {
                        touser,
                        template_id,
                        url,
                        data = new
                        {
                            title = new { value = title, color = "#000" },
                            time = new { value = now, color = "#f00" },
                            msg = new { value = msg, color = "#173177" }
                        }
                    };
                    var post_url = $"https://api.weixin.qq.com/cgi-bin/message/template/send?access_token={access_token}";
                    var post_result = HttpTo.Post(post_url, post_data.ToJson());
                    Console.WriteLine(post_result);
                    if (post_result.ToJObject()["errcode"].ToString() == "0")
                    {
                        vm.Log.Add($"Push successfully {touser}");
                    }
                    else
                    {
                        vm.Log.Add($"Push failed {touser}");
                        vm.Set(SharedEnum.RTag.fail);
                    }
                });

                return vm;
            });
        }

        /// <summary>
        /// 推送（异步）
        /// </summary>
        /// <param name="title"></param>
        /// <param name="msg"></param>
        /// <param name="url"></param>
        public static void PushAsync(string title = "", string msg = "", string url = "")
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                Push(title, msg, url);
            });
        }

        /// <summary>
        /// 推送菜单（管理员）
        /// </summary>
        /// <param name="json"></param>
        /// <param name="access_token"></param>
        /// <returns></returns>
        public static SharedResultVM PushMenu(string json = "", string access_token = "")
        {
            return SharedResultVM.Try(vm =>
            {
                if (string.IsNullOrWhiteSpace(access_token))
                {
                    access_token = Get_access_token();
                }

                if (string.IsNullOrWhiteSpace(json))
                {
                    json = new
                    {
                        button = new[]
                        {
                            new {name="NET牛人",type="view",url="https://www.netnr.com" },
                            new {name="留言",type="view",url="https://ss.netnr.com/message" }
                        }
                    }.ToJson();
                }
                var post_url = $"https://api.weixin.qq.com/cgi-bin/menu/create?access_token={access_token}";
                var post_result = HttpTo.Post(post_url, json);
                vm.Set(SharedEnum.RTag.success);
                vm.Data = post_result;

                return vm;
            });
        }
    }
}
