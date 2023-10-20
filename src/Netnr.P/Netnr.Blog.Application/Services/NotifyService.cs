namespace Netnr.Blog.Application.Services
{
    /// <summary>
    /// 通知服务
    /// </summary>
    public class NotifyService
    {
        /// <summary>
        /// 斗鱼 开播提醒
        /// </summary>
        /// <returns></returns>
        public static void DouyuRoomOnline()
        {
            var thread = new Thread(async () =>
            {
                while (AppTo.GetValue<bool>("Notify:enable"))
                {
                    var now = DateTime.Now;

                    var roomIds = AppTo.GetValue("Notify:DouyuRoomOnline").Split(',');
                    foreach (var roomId in roomIds)
                    {
                        try
                        {
                            var url = $"http://open.douyucdn.cn/api/RoomApi/room/{roomId}";
                            var hc = new HttpClient();
                            hc.DefaultRequestHeaders.UserAgent.TryParseAdd("netnr");
                            hc.DefaultRequestHeaders.Referrer = new Uri("https://douyu.com/{id}");
                            var result = await hc.GetStringAsync(url);
                            var json = result.DeJson();

                            if (json.GetValue<int>("error") == 0)
                            {
                                json = json.GetProperty("data");

                                var ckey = $"douyu-room-{roomId}";
                                var cval = CacheTo.Get<DateTime?>(ckey);

                                var start_time = json.GetValue("start_time");
                                var owner_name = json.GetValue("owner_name");
                                var room_name = json.GetValue("room_name");

                                //上线
                                var online = json.GetValue<int>("online");
                                if (online > 0)
                                {
                                    if (cval.HasValue)
                                    {
                                        CacheTo.Set(ckey, cval);
                                    }
                                    else
                                    {
                                        await PushService.PushWeChat($"[Live] {owner_name}", $"标题: {room_name}\r\n时间: {start_time}\r\nhttps://api.netnr.eu.org/douyu/{roomId}");
                                        CacheTo.Set(ckey, now);
                                    }
                                }
                                else
                                {
                                    if (cval.HasValue)
                                    {
                                        await PushService.PushWeChat($"[End] {owner_name}", $"标题: {room_name}\r\n时间: {start_time}\r\nhttps://api.netnr.eu.org/douyu/{roomId}");
                                    }
                                    CacheTo.Remove(ckey);
                                }
                            }
                            else
                            {
                                ConsoleTo.WriteCard(nameof(DouyuRoomOnline), result);
                            }
                        }
                        catch (Exception ex)
                        {
                            ConsoleTo.LogError(ex, "Notify");
                        }

                        await Task.Delay(1000 * 5);
                    }

                    //间隔分钟
                    var intervalMinute = Enumerable.Range(0, 7).Contains(now.Hour) ? 5 : 1;
                    await Task.Delay(1000 * 60 * intervalMinute);
                }
            })
            {
                IsBackground = true
            };
            thread.Start();
            GC.KeepAlive(thread);
        }
    }
}
