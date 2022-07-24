using Netnr.Core;
using Newtonsoft.Json.Linq;

namespace Netnr.Blog.Application
{
    /// <summary>
    /// 企业微信推送
    /// </summary>
    public class PushService
    {
        /// <summary>
        /// 获取 access_token
        /// https://developer.work.weixin.qq.com/document/path/91039
        /// </summary>
        /// <returns></returns>
        public static string GetAccessToken()
        {
            var atkey = "e_access_token";
            if (CacheTo.Get(atkey) is not JObject ato)
            {
                var corpid = GlobalTo.GetValue("ApiKey:EWeChatApp:CorpID");
                var corpsecret = GlobalTo.GetValue("ApiKey:EWeChatApp:CorpSecret");
                var access_token_url = $"https://qyapi.weixin.qq.com/cgi-bin/gettoken?corpid={corpid}&corpsecret={corpsecret}";
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
        /// 发送应用消息
        /// https://developer.work.weixin.qq.com/document/path/90236
        /// </summary>
        /// <param name="content"></param>
        /// <param name="msgtype">默认 text,可选 markdown</param>
        /// <returns></returns>
        public static ResultVM SendAppMessage(string content = "", string msgtype = "text")
        {
            return ResultVM.Try(vm =>
            {
                if (GlobalTo.GetValue<bool>("ApiKey:EWeChatApp:enable"))
                {
                    vm.Set(EnumTo.RTag.success);

                    var access_token = GetAccessToken();
                    var touser = GlobalTo.GetValue("ApiKey:EWeChatApp:ToUser");
                    var agentid = GlobalTo.GetValue<int>("ApiKey:EWeChatApp:AgentID");

                    var post_data = new JObject
                    {
                        ["touser"] = touser,
                        ["msgtype"] = msgtype,
                        ["agentid"] = agentid,
                        [msgtype] = new JObject
                        {
                            ["content"] = content
                        }
                    };

                    var post_url = $"https://qyapi.weixin.qq.com/cgi-bin/message/send?access_token={access_token}";
                    var post_result = HttpTo.Post(post_url, post_data.ToJson());
                    Console.WriteLine(post_result);

                    if (post_result.ToJObject()["errcode"].ToString() == "0")
                    {
                        vm.Log.Add($"successfully");
                    }
                    else
                    {
                        vm.Log.Add($"failed {touser}");
                        vm.Set(EnumTo.RTag.fail);
                    }
                }
                else
                {
                    vm.Set(EnumTo.RTag.refuse);
                    vm.Msg = "未启用";
                }

                return vm;
            });
        }

        /// <summary>
        /// 推送（异步）
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content"></param>
        public async static void PushAsync(string title, string content = "")
        {
            var list = new List<string>();
            if (!string.IsNullOrWhiteSpace(title))
            {
                list.Add(title);
            }
            list.Add($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            if (!string.IsNullOrWhiteSpace(content))
            {
                list.Add("\r\n" + content);
            }

            await Task.Run(() => SendAppMessage(string.Join("\r\n", list)));
        }
    }
}
