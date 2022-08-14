namespace Netnr.Blog.Application.Services
{
    /// <summary>
    /// 微信公众号
    /// </summary>
    public class WeixinMPService
    {
        /// <summary>
        /// 消息回复
        /// </summary>
        /// <param name="postData"></param>
        /// <returns></returns>
        public static string MessageReply(string postData)
        {
            var doc = postData.DeXml();

            var receiveUserName = doc.GetValue("/xml/ToUserName"); //开发者微信号
            var receiveOpenId = doc.GetValue("/xml/FromUserName"); //发送者OpenId

            var receiveMsgType = doc.GetValue("/xml/MsgType"); //消息类型

            var outType = "text";
            var outContent = $"{AppTo.GetValue("Common:ChineseName")} \n{AppTo.GetValue("Common:Domain")}";

            switch (receiveMsgType)
            {
                case "text":
                    {
                        var receiveContent = doc.GetValue("/xml/Content");
                        if (new[] { "彩票", "lottery", "cp" }.Contains(receiveContent))
                        {
                            outContent = $"彩票 \nhttps://ss.netnr.com/lottery";
                        }
                        else if (new[] { "时间", "sj", "time", "date" }.Contains(receiveContent))
                        {
                            outContent = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}";
                        }
                        else if (new[] { "表情", "emoji" }.Contains(receiveContent))
                        {
                            outContent = $"表情 \nhttps://ss.netnr.com/emoji";
                        }
                        else if (new[] { "源码", "git" }.Contains(receiveContent))
                        {
                            outContent = $"https://github.com/netnr";
                        }
                    }
                    break;
            }

            var result =
$@"
<xml>
  <ToUserName><![CDATA[{receiveOpenId}]]></ToUserName>
  <FromUserName><![CDATA[{receiveUserName}]]></FromUserName>
  <CreateTime>{DateTime.Now.ToTimestamp()}</CreateTime>
  <MsgType><![CDATA[{outType}]]></MsgType>
  <Content><![CDATA[{outContent}]]></Content>
</xml>
";
            return result;
        }
    }
}
