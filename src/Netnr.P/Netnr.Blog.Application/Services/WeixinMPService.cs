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
            var outContent = $"{AppTo.GetValue("ProgramParameters:ChineseName")} \n{AppTo.GetValue("ProgramParameters:Domain")}";

            switch (receiveMsgType)
            {
                case "text":
                    {
                        var receiveContent = doc.GetValue("/xml/Content");
                        switch (receiveContent)
                        {
                            case "彩票":
                            case "cp":
                            case "lottery":
                                outContent = $"彩票 \nhttps://ss.netnr.com/lottery";
                                break;
                            case "时间":
                            case "sj":
                            case "time":
                            case "date":
                                outContent = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}";
                                break;
                            case "表情":
                            case "emoji":
                                outContent = $"表情 \nhttps://ss.netnr.com/emoji";
                                break;
                            case "源码":
                            case "git":
                                outContent = "https://github.com/netnr";
                                break;
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
