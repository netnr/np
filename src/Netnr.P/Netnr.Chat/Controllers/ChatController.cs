using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Collections.Generic;
using Netnr.Chat.Application.ViewModel;
using Netnr.SharedFast;
using chs = Netnr.Chat.Application.ChatHubService;

namespace Netnr.Chat.Controllers
{
    /// <summary>
    /// 接口
    /// </summary>
    [Route("[controller]/[action]")]
    public class ChatController : Controller
    {
        readonly IHubContext<chs> hub;
        private readonly Data.ContextBase db;

        public ChatController(IHubContext<chs> _hub, Data.ContextBase _db)
        {
            hub = _hub;
            db = _db;
        }

        /// <summary>
        /// 推送消息到用户（好友才能推送）
        /// </summary>
        /// <param name="access_token">授权码</param>
        /// <param name="CmFromId">发送用户ID</param>
        /// <param name="CmFromDevice">发送者用户设备</param>
        /// <param name="CmFromSign">发送者用户标识</param>
        /// <param name="CmContent">发送内容</param>
        /// <param name="CmType">消息类型 MessageType 枚举</param>
        /// <param name="CmToIds">接收用户ID</param>
        /// <returns></returns>
        [HttpPost]
        public SharedResultVM PushMessageToUsers([FromQuery] string access_token, [FromForm] string CmFromId, [FromForm] string CmFromDevice, [FromForm] string CmFromSign, [FromForm] string CmContent, [FromForm] string CmType, [FromForm] List<string> CmToIds)
        {
            return SharedResultVM.Try(vm =>
            {
                var ua = chs.GetUserAuthInfo(HttpContext, access_token);
                if (ua == null || ua.UserId != CmFromId)
                {
                    vm.Set(SharedEnum.RTag.unauthorized);
                }
                else
                {
                    var pmax = GlobalTo.GetValue<int>("NetnrChat:BatchPushUserMax");
                    var rme = GlobalTo.GetValue("NetnrChat:ReceiveMessage");

                    var cm = new ChatMessageVM
                    {
                        CmFromId = CmFromId,
                        CmFromDevice = CmFromDevice,
                        CmFromSign = CmFromSign,
                        CmContent = CmContent,
                        CmType = CmType,
                        CmToIds = CmToIds
                    };

                    if (string.IsNullOrWhiteSpace(cm.CmFromId))
                    {
                        vm.Set(SharedEnum.RTag.lack);
                        vm.Msg = "发送用户ID不能为空";
                    }
                    else if (cm.CmContent == null)
                    {
                        vm.Set(SharedEnum.RTag.lack);
                        vm.Msg = "发送内容不能为空";
                    }
                    else if (!Enum.TryParse(cm.CmType, true, out MessageType mt))
                    {
                        vm.Set(SharedEnum.RTag.lack);
                        vm.Msg = "消息类型有误";
                    }
                    else if (cm.CmToIds == null || cm.CmToIds.Count == 0)
                    {
                        vm.Set(SharedEnum.RTag.lack);
                        vm.Msg = "接收用户ID不能为空";
                    }
                    else if (cm.CmToIds.Count > pmax)
                    {
                        vm.Set(SharedEnum.RTag.refuse);
                        vm.Msg = $"接收用户限制为最多{pmax}";
                    }
                    else
                    {
                        //发送用户好友
                        var listBuddy = db.NChatBuddy.Where(x => x.CuUserId == cm.CmFromId).Select(x => x.CbUserId).ToList();
                        var isBuddy = cm.CmToIds.Any(x => listBuddy.Contains(x));
                        //自己
                        if (CmToIds.Count == 1 && CmFromId == CmToIds.First())
                        {
                            isBuddy = true;
                        }
                        //非好友
                        if (!isBuddy)
                        {
                            vm.Set(SharedEnum.RTag.refuse);
                            vm.Msg = "不是好友关系不能发送消息";
                        }
                        else
                        {
                            cm.CmId = chs.NewMessageId();
                            cm.CmType = mt.ToString();
                            cm.CmTime = DateTime.Now;
                            cm.CmWhich = "User";

                            var ous = chs.FindUsers(cm.CmToIds, true);
                            var wcm = chs.WriteMessageForUser(cm);
                            var connids = new List<string>();
                            cm.CmToIds.ForEach(toid =>
                            {
                                var oc = ous.FirstOrDefault(x => x.UserId == toid);
                                //在线
                                var isOnline = oc != null;
                                if (isOnline)
                                {
                                    connids.AddRange(oc.Conns.Keys.ToList());
                                }

                                //消息状态
                                wcm.FirstOrDefault(x => x.CmuPullUserId == toid).CmuStatus = isOnline ? 2 : 1;
                            });

                            //写入消息
                            db.NChatMessageToUser.AddRange(wcm);
                            var num = db.SaveChanges();

                            //发送成功，返回消息ID
                            if (num > 0)
                            {
                                //在线用户直接推送
                                if (connids.Count > 0)
                                {
                                    hub.Clients.Clients(connids).SendAsync(rme, cm);
                                }

                                vm.Data = cm.CmId;
                                vm.Set(SharedEnum.RTag.success);
                            }
                            else
                            {
                                vm.Set(SharedEnum.RTag.fail);
                            }
                        }
                    }
                }

                return vm;
            });
        }

        /// <summary>
        /// 用户消息回执
        /// </summary>
        /// <param name="access_token">授权码</param>
        /// <param name="CmId">消息ID</param>
        /// <param name="UserId">接收用户ID</param>
        /// <param name="CmStatus">消息状态</param>
        /// <returns></returns>
        [HttpGet]
        public SharedResultVM UserMessageReceipt(string access_token, string CmId, string UserId, int CmStatus)
        {
            return SharedResultVM.Try(vm =>
            {
                var ua = chs.GetUserAuthInfo(HttpContext, access_token);
                if (ua == null || ua.UserId != UserId)
                {
                    vm.Set(SharedEnum.RTag.unauthorized);
                }
                else
                {
                    //写入库
                    var mb = db.NChatMessageToUser.Find(CmId);
                    if (mb == null)
                    {
                        vm.Set(SharedEnum.RTag.invalid);
                        vm.Msg = "消息ID无效";
                    }
                    else if (mb.CmuPullUserId != UserId)
                    {
                        vm.Set(SharedEnum.RTag.unauthorized);
                    }
                    else
                    {
                        mb.CmuStatus = CmStatus;
                        var num = db.SaveChanges();

                        vm.Set(num > 0);
                    }
                }

                return vm;
            });
        }

        /// <summary>
        /// 推送消息到群组
        /// </summary>
        /// <param name="access_token">授权码</param>
        /// <param name="CmFromId">发送用户ID</param>
        /// <param name="CmFromDevice">发送者用户设备</param>
        /// <param name="CmFromSign">发送者用户标识</param>
        /// <param name="CmContent">发送内容</param>
        /// <param name="CmType">消息类型 MessageType 枚举</param>
        /// <param name="CmToIds">接收群组ID</param>
        /// <returns></returns>
        [HttpPost]
        public SharedResultVM PushMessageToGroups([FromQuery] string access_token, [FromForm] string CmFromId, [FromForm] string CmFromDevice, [FromForm] string CmFromSign, [FromForm] string CmContent, [FromForm] string CmType, [FromForm] List<string> CmToIds)
        {
            return SharedResultVM.Try(vm =>
            {
                var ua = chs.GetUserAuthInfo(HttpContext, access_token);
                if (ua == null || ua.UserId != CmFromId)
                {
                    vm.Set(SharedEnum.RTag.unauthorized);
                }
                else
                {
                    var pmax = GlobalTo.GetValue<int>("NetnrChat:BatchPushGroupMax");
                    var rme = GlobalTo.GetValue("NetnrChat:ReceiveMessage");

                    var cm = new ChatMessageVM
                    {
                        CmFromId = CmFromId,
                        CmFromDevice = CmFromDevice,
                        CmFromSign = CmFromSign,
                        CmContent = CmContent,
                        CmType = CmType,
                        CmToIds = CmToIds
                    };

                    if (string.IsNullOrWhiteSpace(cm.CmFromId))
                    {
                        vm.Set(SharedEnum.RTag.lack);
                        vm.Msg = "发送用户ID不能为空";
                    }
                    else if (cm.CmContent == null)
                    {
                        vm.Set(SharedEnum.RTag.lack);
                        vm.Msg = "发送内容不能为空";
                    }
                    else if (!Enum.TryParse(cm.CmType, true, out MessageType mt))
                    {
                        vm.Set(SharedEnum.RTag.lack);
                        vm.Msg = "消息类型有误";
                    }
                    else if (cm.CmToIds == null || cm.CmToIds.Count == 0)
                    {
                        vm.Set(SharedEnum.RTag.lack);
                        vm.Msg = "接收群组ID不能为空";
                    }
                    else if (cm.CmToIds.Count > pmax)
                    {
                        vm.Set(SharedEnum.RTag.refuse);
                        vm.Msg = $"接收组限制为最多{pmax}";
                    }
                    else
                    {
                        //群组及成员
                        var gms = db.NChatGroupMember.Where(x => cm.CmToIds.Contains(x.CgId)).Select(x => x.CgId + ":" + x.CuUserId).ToList();
                        //是成员
                        var isMember = cm.CmToIds.Any(x => gms.Contains(x + ":" + CmFromId));
                        if (!isMember)
                        {
                            vm.Set(SharedEnum.RTag.refuse);
                            vm.Msg = "非群组成员不能发送消息";
                        }
                        else
                        {
                            cm.CmId = chs.NewMessageId();
                            cm.CmType = mt.ToString();
                            cm.CmTime = DateTime.Now;
                            cm.CmWhich = "Group";

                            //发送消息
                            var connids = new List<string>();
                            gms.ForEach(gm =>
                            {
                                var uid = gm.Split(':').Last();
                                if (chs.OnlineUser1.ContainsKey(uid))
                                {
                                    connids.AddRange(chs.OnlineUser1[uid].Conns.Keys.ToList());
                                }
                            });

                            //写入消息
                            var wcm = chs.WriteMessageForGroup(cm);
                            db.NChatMessageToGroup.AddRange(wcm);

                            var num = db.SaveChanges();
                            //发送成功，返回消息ID
                            if (num > 0)
                            {
                                //在线用户直接推送
                                if (connids.Count > 0)
                                {
                                    hub.Clients.Clients(connids).SendAsync(rme, cm);
                                }

                                vm.Data = cm.CmId;
                                vm.Set(SharedEnum.RTag.success);
                            }
                            else
                            {
                                vm.Set(SharedEnum.RTag.fail);
                            }
                        }
                    }
                }

                return vm;
            });
        }

        /// <summary>
        /// 获取用户未读消息数量
        /// </summary>
        /// <param name="access_token">授权码</param>
        /// <param name="UserId">用户ID</param>
        /// <returns></returns>
        [HttpGet]
        public SharedResultVM GetUserUnreadMessageCount(string access_token, string UserId)
        {
            return SharedResultVM.Try(vm =>
            {
                var ua = chs.GetUserAuthInfo(HttpContext, access_token);
                if (ua == null || ua.UserId != UserId)
                {
                    vm.Set(SharedEnum.RTag.unauthorized);
                }
                else
                {
                    int num = db.NChatMessageToUser.Where(x => x.CmuPullUserId == UserId && x.CmuStatus >= 1 && x.CmuStatus <= 3).Count();
                    vm.Data = num;
                    vm.Set(SharedEnum.RTag.success);
                }

                return vm;
            });
        }

        /// <summary>
        /// 获取群组未读消息数量
        /// </summary>
        /// <param name="access_token">授权码</param>
        /// <param name="UserId">用户ID</param>
        /// <returns></returns>
        [HttpGet]
        public SharedResultVM GetGroupUnreadMessageCount(string access_token, string UserId)
        {
            return SharedResultVM.Try(vm =>
            {
                var ua = chs.GetUserAuthInfo(HttpContext, access_token);
                if (ua == null || ua.UserId != UserId)
                {
                    vm.Set(SharedEnum.RTag.unauthorized);
                }
                else
                {
                    //库里面的消息
                    var query1 = from a in db.NChatGroup
                                 join b1 in db.NChatMessageGroupPull on a.CgOwnerId equals b1.CuUserId into bg
                                 from b in bg.DefaultIfEmpty()
                                 where a.CgOwnerId == UserId
                                 select new
                                 {
                                     a.CgId,
                                     b.GpUpdateTime
                                 };

                    var query = from a in query1
                                join b1 in db.NChatMessageToGroup on a.CgId equals b1.CmgPullGroupId into bg
                                from b in bg.DefaultIfEmpty()
                                where b.CmgCreateTime > a.GpUpdateTime
                                group a by new { a.CgId, a.GpUpdateTime } into g
                                select new
                                {
                                    GroupId = g.Key.CgId,
                                    UpdateTime = g.Key.GpUpdateTime,
                                    Count = g.Count()
                                };

                    var list = query.ToList();
                    var gk = new Dictionary<string, int>();

                    foreach (var gi in list)
                    {
                        gk.Add(gi.GroupId, gi.Count);
                    }

                    vm.Data = new
                    {
                        Count = gk.Values.Sum(),
                        Detail = gk
                    };
                    vm.Set(SharedEnum.RTag.success);
                }

                return vm;
            });
        }

        /// <summary>
        /// 获取用户历史消息
        /// </summary>
        /// <param name="access_token">授权码</param>
        /// <param name="UserId">用户ID</param>
        /// <param name="page">页码</param>
        /// <param name="size">页量</param>
        /// <returns></returns>
        [HttpGet]
        public SharedResultVM GetHistoryUserMessage(string access_token, string UserId, int page, int size)
        {
            return SharedResultVM.Try(vm =>
            {
                var ua = chs.GetUserAuthInfo(HttpContext, access_token);
                if (ua == null || ua.UserId != UserId)
                {
                    vm.Set(SharedEnum.RTag.unauthorized);
                }
                else
                {
                    var begin = (page - 1) * size;

                    var list = db.NChatMessageToUser.Where(x => x.CmuPullUserId == UserId)
                                .OrderByDescending(x => x.CmuCreateTime)
                                .Skip(begin).Take(size)
                                .ToList();

                    vm.Data = chs.WriteMessageForUserReverse(list);
                    vm.Set(SharedEnum.RTag.success);
                }

                return vm;
            });
        }

        /// <summary>
        /// 获取群组历史消息
        /// </summary>
        /// <param name="access_token">授权码</param>
        /// <param name="GroupId">群组ID</param>
        /// <param name="UserId">用户ID</param>
        /// <param name="page">页码</param>
        /// <param name="size">页量</param>
        /// <returns></returns>
        [HttpGet]
        public SharedResultVM GetHistoryGroupMessage(string access_token, string GroupId, string UserId, int page, int size)
        {
            return SharedResultVM.Try(vm =>
            {
                var ua = chs.GetUserAuthInfo(HttpContext, access_token);
                if (ua == null || ua.UserId != UserId)
                {
                    vm.Set(SharedEnum.RTag.unauthorized);
                }
                else
                {
                    var isMember = db.NChatGroupMember.Any(x => x.CgId == GroupId && x.CuUserId == UserId);
                    if (isMember)
                    {
                        var begin = (page - 1) * size;

                        var list = db.NChatMessageToGroup.Where(x => x.CmgPullGroupId == GroupId)
                                    .OrderByDescending(x => x.CmgCreateTime)
                                    .Skip(begin).Take(size)
                                    .ToList();

                        vm.Data = chs.WriteMessageForGroupReverse(list);
                        vm.Set(SharedEnum.RTag.success);
                    }
                    else
                    {
                        vm.Set(SharedEnum.RTag.refuse);
                    }
                }

                return vm;
            });
        }

        /// <summary>
        /// 获取群组成员
        /// </summary>
        /// <param name="access_token">授权码</param>
        /// <param name="GroupId">群组ID</param>
        /// <param name="UserId">用户ID</param>
        /// <returns></returns>
        [HttpGet]
        public SharedResultVM GetGroupMember(string access_token, string GroupId, string UserId)
        {
            return SharedResultVM.Try(vm =>
            {
                var ua = chs.GetUserAuthInfo(HttpContext, access_token);
                if (ua == null || ua.UserId != UserId)
                {
                    vm.Set(SharedEnum.RTag.unauthorized);
                }
                else
                {
                    var isMember = db.NChatGroupMember.Any(x => x.CgId == GroupId && x.CuUserId == UserId);
                    if (isMember)
                    {
                        //返回群组成员用户
                        var query = from a in db.NChatGroupMember
                                    join b in db.NChatUser on a.CuUserId equals b.CuUserId
                                    where a.CgId == GroupId
                                    select b;

                        var list = query.ToList();
                        if (list.Count > 0)
                        {
                            vm.Data = list;
                            vm.Set(SharedEnum.RTag.success);
                        }
                        else
                        {
                            vm.Set(SharedEnum.RTag.invalid);
                        }
                    }
                    else
                    {
                        vm.Set(SharedEnum.RTag.refuse);
                    }
                }

                return vm;
            });
        }

        /// <summary>
        /// 获取群组信息
        /// </summary>
        /// <param name="access_token">授权码</param>
        /// <param name="GroupId">群组ID</param>
        /// <param name="UserId">用户ID</param>
        /// <returns></returns>
        [HttpGet]
        public SharedResultVM GetGroupInfo(string access_token, string GroupId, string UserId)
        {
            return SharedResultVM.Try(vm =>
            {
                var ua = chs.GetUserAuthInfo(HttpContext, access_token);
                if (ua == null || ua.UserId != UserId)
                {
                    vm.Set(SharedEnum.RTag.unauthorized);
                }
                else
                {
                    var isMember = db.NChatGroupMember.Any(x => x.CgId == GroupId && x.CuUserId == UserId);
                    if (isMember)
                    {
                        //返回群组信息
                        var mo = db.NChatGroupMember.Find(GroupId);
                        if (mo != null)
                        {
                            vm.Data = mo;
                            vm.Set(SharedEnum.RTag.success);
                        }
                        else
                        {
                            vm.Set(SharedEnum.RTag.invalid);
                        }
                    }
                    else
                    {
                        vm.Set(SharedEnum.RTag.refuse);
                    }
                }

                return vm;
            });
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="access_token">授权码</param>
        /// <param name="UserId">用户ID</param>
        /// <returns></returns>
        [HttpGet]
        public SharedResultVM GetUserInfo(string access_token, string UserId)
        {
            return SharedResultVM.Try(vm =>
            {
                var ua = chs.GetUserAuthInfo(HttpContext, access_token);
                if (ua == null || ua.UserId != UserId)
                {
                    vm.Set(SharedEnum.RTag.unauthorized);
                }
                else
                {
                    var mo = db.NChatUser.Find(UserId);
                    if (mo != null)
                    {
                        vm.Data = mo;
                        vm.Set(SharedEnum.RTag.success);
                    }
                    else
                    {
                        vm.Set(SharedEnum.RTag.invalid);
                    }
                }

                return vm;
            });
        }

        /// <summary>
        /// 新建群组
        /// </summary>
        /// <param name="access_token">授权码</param>
        /// <param name="GroupName">群组名称</param>
        /// <param name="GroupUser">群组成员，默认第一个用户为管理员</param>
        /// <param name="GroupClassify">群组分类</param>
        /// <returns></returns>
        [HttpPost]
        public SharedResultVM GroupNew([FromQuery] string access_token, [FromForm] string GroupName, [FromForm] List<string> GroupUser, [FromForm] string GroupClassify)
        {
            return SharedResultVM.Try(vm =>
            {
                var ua = chs.GetUserAuthInfo(HttpContext, access_token);
                if (ua == null)
                {
                    vm.Set(SharedEnum.RTag.unauthorized);
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(GroupName))
                    {
                        vm.Set(SharedEnum.RTag.lack);
                        vm.Msg = "组名称不能为空";
                    }
                    else if (GroupUser.Count == 0)
                    {
                        vm.Set(SharedEnum.RTag.fail);
                        vm.Msg = "组用户至少有一个（默认第一个用户为管理员）";
                    }
                    else
                    {
                        //群组成员
                        var users = chs.FindUsers(GroupUser);
                        if (users.Count > 0)
                        {
                            var now = DateTime.Now;
                            var GroupId = Core.UniqueTo.LongId().ToString();

                            //组成员
                            var gms = new List<Domain.NChatGroupMember>();

                            //用户加入组
                            foreach (var user in users)
                            {
                                //添加组成员
                                gms.Add(new Domain.NChatGroupMember()
                                {
                                    CgmId = Core.UniqueTo.LongId().ToString(),
                                    CgId = GroupId,
                                    CuUserId = user.UserId,
                                    CgmCreateTime = now,
                                    CgmStatus = 1
                                });
                            }

                            //写入表
                            db.NChatGroup.Add(new Domain.NChatGroup()
                            {
                                CgId = GroupId,
                                CgName = GroupName,
                                CgOwnerId = users.FirstOrDefault().UserId,
                                CgCreateTime = DateTime.Now,
                                CcId = string.IsNullOrWhiteSpace(GroupClassify) ? "1" : GroupClassify,
                                CgStatus = 1
                            });
                            db.NChatGroupMember.AddRange(gms);
                            var num = db.SaveChanges();

                            //成功
                            if (num > 0)
                            {
                                vm.Data = GroupId;
                                vm.Set(SharedEnum.RTag.success);
                            }
                            else
                            {
                                vm.Set(SharedEnum.RTag.fail);
                            }
                        }
                        else
                        {
                            vm.Set(SharedEnum.RTag.invalid);
                            vm.Msg = "组用户无效";
                        }
                    }
                }

                return vm;
            });
        }

        /// <summary>
        /// 加入群组
        /// </summary>
        /// <param name="access_token">授权码</param>
        /// <param name="GroupId">群组ID</param>
        /// <param name="UserId">用户ID</param>
        /// <returns></returns>
        [HttpGet]
        public SharedResultVM GroupAddMember(string access_token, string GroupId, string UserId)
        {
            return SharedResultVM.Try(vm =>
            {

                return vm;
            });
        }

        /// <summary>
        /// 离开群组
        /// </summary>
        /// <param name="access_token">授权码</param>
        /// <param name="GroupId">群组ID</param>
        /// <param name="UserId">用户ID</param>
        /// <returns></returns>
        [HttpGet]
        public static SharedResultVM GroupDeleteMember(string access_token, string GroupId, string UserId)
        {
            return SharedResultVM.Try(vm =>
            {

                return vm;
            });
        }

        /// <summary>
        /// 删除群组
        /// </summary>
        /// <param name="access_token">授权码</param>
        /// <param name="GroupId">群组ID</param>
        /// <param name="UserId">用户ID</param>
        /// <returns></returns>
        [HttpGet]
        public static SharedResultVM GroupDelete(string access_token, string GroupId, string UserId)
        {
            return SharedResultVM.Try(vm =>
            {

                return vm;
            });
        }
    }
}