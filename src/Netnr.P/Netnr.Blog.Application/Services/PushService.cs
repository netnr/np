namespace Netnr.Blog.Application.Services
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
            var ato = CacheTo.Get<JsonElement?>(atkey);
            if (ato == null)
            {
                var corpid = AppTo.GetValue("ApiKey:EWeixinApp:CorpId");
                var corpsecret = AppTo.GetValue("ApiKey:EWeixinApp:CorpSecret");

                var access_token_url = $"https://qyapi.weixin.qq.com/cgi-bin/gettoken?corpid={corpid}&corpsecret={corpsecret}";
                var access_token_result = HttpTo.Get(access_token_url);
                ato = access_token_result.DeJson();

                CacheTo.Set(atkey, ato, 3600, false);
            }

            var access_token = ato.GetValue("access_token");
            return access_token;
        }

        /// <summary>
        /// 发送应用消息
        /// https://developer.work.weixin.qq.com/document/path/90236
        /// </summary>
        /// <param name="content"></param>
        /// <param name="msgtype">默认 text,可选 markdown</param>
        /// <returns></returns>
        public static ResultVM SendAppMessage(string content = "", string msgtype = "text") => ResultVM.Try(vm =>
        {
            if (AppTo.GetValue<bool>("ApiKey:EWeixinApp:enable"))
            {
                vm.Set(EnumTo.RTag.success);

                var access_token = GetAccessToken();
                var touser = AppTo.GetValue("ApiKey:EWeixinApp:ToUser");
                var agentid = AppTo.GetValue<int>("ApiKey:EWeixinApp:AgentId");

                var post_data = new JsonObject
                {
                    ["touser"] = touser,
                    ["msgtype"] = msgtype,
                    ["agentid"] = agentid,
                    [msgtype] = new JsonObject
                    {
                        ["content"] = content
                    }
                };

                var post_url = $"https://qyapi.weixin.qq.com/cgi-bin/message/send?access_token={access_token}";
                var post_result = HttpTo.Post(post_url, post_data.ToJson());
                Console.WriteLine($"\r\n------- WeChat message push {DateTime.Now:yyyy-MM-dd HH:mm:ss}\r\n{post_result.ToJson(true)}");

                if (post_result.DeJson().GetValue("errcode") == "0")
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

        /// <summary>
        /// 推送（异步）
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content"></param>
        public async static Task<ResultVM> PushAsync(string title, string content = "")
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

            return await Task.Run(() => SendAppMessage(string.Join("\r\n", list)));
        }
    }
}
