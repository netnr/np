﻿using Markdig;

namespace Netnr.Blog.Web.Controllers
{
    /// <summary>
    /// Doc
    /// </summary>
    public class DocController(ContextBase cb) : Controller
    {
        public ContextBase db = cb;

        /// <summary>
        /// Doc 新增表单
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public IActionResult Index()
        {
            return View("/Views/Doc/_PartialDocForm.cshtml");
        }

        /// <summary>
        /// Doc 列表
        /// </summary>
        /// <param name="k"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [ResponseCache(Duration = 10)]
        public async Task<IActionResult> Discover(string k, int page = 1)
        {
            var userId = IdentityService.Get(HttpContext)?.UserId ?? 0;

            var ps = await CommonService.DocQuery(k, 0, userId, page);
            ps.Route = Request.Path;
            return View("/Views/Doc/_PartialDocList.cshtml", ps);
        }

        /// <summary>
        /// Doc 用户
        /// </summary>
        /// <param name="id"></param>
        /// <param name="k"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [ActionName("User")]
        public async Task<IActionResult> Id([FromRoute] string id, string k, int page = 1)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Redirect("/doc");
            }

            int uid = Convert.ToInt32(id);

            var mu = await db.UserInfo.FindAsync(uid);
            if (mu == null)
            {
                return Content("Account is empty");
            }
            ViewData["Nickname"] = mu.Nickname;

            var userId = IdentityService.Get(HttpContext)?.UserId ?? 0;

            var ps = await CommonService.DocQuery(k, uid, userId, page);
            ps.Route = Request.Path;
            return View("/Views/Doc/_PartialDocList.cshtml", ps);
        }

        /// <summary>
        /// Doc Edit
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<IActionResult> Edit([FromRoute] string id)
        {
            var uinfo = IdentityService.Get(HttpContext);

            var mo = await db.DocSet.FirstOrDefaultAsync(x => x.DsCode == id && x.Uid == uinfo.UserId);
            if (mo != null)
            {
                return View("/Views/Doc/_PartialDocForm.cshtml", mo);
            }
            else
            {
                return Unauthorized("401");
            }
        }

        /// <summary>
        /// Doc Save
        /// </summary>
        /// <param name="mo"></param>
        /// <returns></returns>
        [Authorize, HttpPost]
        public async Task<IActionResult> Save([FromForm] DocSet mo)
        {
            var uinfo = IdentityService.Get(HttpContext);
            int num;

            //xss
            mo.DsName = CommonService.XSS(mo.DsName);

            //新增
            if (string.IsNullOrWhiteSpace(mo.DsCode))
            {
                mo.DsCode = Snowflake53To.Id().ToString();
                mo.Uid = uinfo.UserId;
                mo.DsStatus = 1;
                mo.DsCreateTime = DateTime.Now;

                await db.DocSet.AddAsync(mo);
                num = await db.SaveChangesAsync();

                //推送通知
                _ = PushService.PushWeChat("网站消息（Doc）", $"{mo.DsName}\r\n{mo.DsRemark}");
            }
            else
            {
                num = await db.DocSet.Where(x => x.DsCode == mo.DsCode && x.Uid == uinfo.UserId)
                    .ExecuteUpdateAsync(x => x
                    .SetProperty(p => p.DsName, mo.DsName)
                    .SetProperty(p => p.DsRemark, mo.DsRemark)
                    .SetProperty(p => p.DsOpen, mo.DsOpen)
                    .SetProperty(p => p.Spare1, mo.Spare1));
            }

            if (num > 0)
            {
                return Redirect($"/doc/user/{uinfo.UserId}");
            }
            else
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Doc Delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize, HttpGet]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            var uinfo = IdentityService.Get(HttpContext);

            var num = await db.DocSet.Where(x => x.DsCode == id && x.Uid == uinfo.UserId).ExecuteDeleteAsync();
            if (num > 0)
            {
                num += await db.DocSetDetail.Where(x => x.DsCode == id).ExecuteDeleteAsync();

                return Redirect($"/doc/user/{uinfo.UserId}");
            }

            return Unauthorized(401);
        }

        /// <summary>
        /// Doc 显示
        /// </summary>
        /// <param name="id">文档编号</param>
        /// <param name="sid">页ID或md文件名</param>
        /// <param name="code">分享码</param>
        /// <returns></returns>
        public async Task<IActionResult> Code([FromRoute] string id, [FromRoute] string sid, string code)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Redirect("/doc/discover");
            }

            var sharedCode = $"SharedCode_{id}";
            //有分享码
            if (!string.IsNullOrWhiteSpace(code))
            {
                Response.Cookies.Append(sharedCode, code);
            }
            else
            {
                code = Request.Cookies[sharedCode]?.ToString();
            }

            sid ??= "";

            //跳转带斜杠
            if (id.Length > 10 && sid == "" && !Request.Path.Value.EndsWith('/'))
            {
                return Redirect(Request.Path.Value + "/");
            }

            var baseUrl = string.Join("/", Request.Path.Value.Split('/').ToList().Take(4)) + "/";

            var uinfo = IdentityService.Get(HttpContext);

            var docModel = await db.DocSet.FindAsync(id);
            if (docModel == null)
            {
                return NotFound();
            }
            else if (docModel.DsOpen == 1 || docModel.Uid == uinfo?.UserId || (!string.IsNullOrWhiteSpace(docModel.Spare1) && docModel.Spare1 == code))
            {
                var ismd = sid.EndsWith(".md");
                var listSeo = new List<string>();

                //README
                if (sid.StartsWith("README") || string.IsNullOrWhiteSpace(sid))
                {
                    var ct = new List<string>
                    {
                        "## " + docModel.DsName,
                        string.IsNullOrWhiteSpace(docModel.DsRemark)? "" : $"#### {docModel.DsRemark}",
                        "",
                        docModel.DsCreateTime.Value.ToString("yyyy-MM-dd HH:mm:ss")
                    };
                    var ctmd = string.Join(Environment.NewLine, ct);
                    listSeo.Add(Markdown.ToHtml(ctmd));

                    if (ismd)
                    {
                        return Content(ctmd);
                    }
                }

                //左侧菜单
                if (sid.StartsWith("_sidebar") && sid.Length > 10)
                {
                    //文档集目录
                    var docTree = await db.DocSetDetail
                        .Where(x => x.DsCode == id)
                        .OrderBy(x => x.DsdOrder)
                        .Select(x => new DocTreeVM
                        {
                            DsdId = x.DsdId,
                            DsdPid = x.DsdPid,
                            DsCode = x.DsCode,
                            DsdTitle = x.DsdTitle,
                            DsdOrder = x.DsdOrder,
                            IsCatalog = x.DsdContentMd == null || x.DsdContentMd.Length == 0
                        }).ToListAsync();

                    var ct = TreeToMd(baseUrl, docTree, Guid.Empty.ToString());
                    var ctmd = string.Join(Environment.NewLine, ct);
                    listSeo.Add(Markdown.ToHtml(ctmd).Replace(".md\"", "\""));

                    if (ismd)
                    {
                        return Content(ctmd);
                    }
                }

                //一项
                if (sid.Length > 10 || sid.Length > 10)
                {
                    var mdid = sid.Replace(".md", "");

                    var detailModel = await db.DocSetDetail.FirstOrDefaultAsync(x => x.DsdId == mdid);

                    var uptime = "<span title='" + detailModel.DsdCreateTime.Value.ToString("yyyy-MM-dd HH:mm:ss") + "'><i class='fa fa-clock-o'></i> 创建于：" + detailModel.DsdCreateTime.Value.ToString("yyyy年MM月dd日") + "</span>";
                    if (detailModel.DsdUpdateTime != detailModel.DsdCreateTime)
                    {
                        uptime += " &nbsp; <span title='" + detailModel.DsdUpdateTime.Value.ToString("yyyy-MM-dd HH:mm:ss") + "'><i class='fa fa-clock-o'></i> 更新于：" + detailModel.DsdUpdateTime.Value.ToString("yyyy年MM月dd日") + "</span>";
                    }
                    uptime += " &nbsp; <span><i class='fa fa-sort-amount-asc'></i> 排序号：" + detailModel.DsdOrder + "</span>";
                    var mdtitle = string.Join(Environment.NewLine, new List<string>()
                    {
                        "<h2>"+detailModel.DsdTitle+"</h2>",
                        uptime,
                        "<hr/>",
                        "",
                        ""
                    });

                    listSeo.Add(mdtitle + detailModel.DsdContentHtml);

                    if (ismd)
                    {
                        return Content(mdtitle + detailModel.DsdContentMd);
                    }
                }

                //文档标题
                ViewData["Title"] = docModel.DsName;
                ViewData["DocSeo"] = string.Join(Environment.NewLine, listSeo);

                return View();
            }
            else
            {
                return Unauthorized(401);
            }
        }

        /// <summary>
        /// 树转 markdown
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="list"></param>
        /// <param name="pid"></param>
        /// <param name="listNo"></param>
        /// <returns></returns>
        private List<string> TreeToMd(string baseUrl, List<DocTreeVM> list, string pid, List<string> listNo = null)
        {
            var ct = new List<string>();

            listNo ??= [];

            var rdt = list.Where(x => x.DsdPid == pid).ToList();

            for (int i = 0; i < rdt.Count; i++)
            {
                //数据行
                var dr = rdt[i];
                var di = "";
                foreach (var item in listNo)
                {
                    di += "  ";
                }

                //序号
                var ino = string.Join(".", listNo);
                if (ino != "")
                {
                    ino += ".";
                }
                ino += (i + 1) + "、";

                if (dr.IsCatalog)
                {
                    ct.Add($"{di}- {ino}{dr.DsdTitle}");
                    ct.Add("");
                }
                else
                {
                    ct.Add($"{di}- [{ino}{dr.DsdTitle}]({baseUrl}{dr.DsdId}.md)");
                }

                var nrdt = list.Where(x => x.DsdPid == dr.DsdId).ToList();
                if (nrdt.Count > 0)
                {
                    listNo.Add((i + 1).ToString());
                    var sct = TreeToMd(baseUrl, list, dr.DsdId, listNo);
                    listNo.RemoveAt(listNo.Count - 1);
                    ct.AddRange(sct);
                }
            }

            return ct;
        }

        /// <summary>
        /// 一项（新增、编辑）
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public async Task<IActionResult> Item([FromRoute] string id, [FromRoute] string sid)
        {
            var uinfo = IdentityService.Get(HttpContext);

            if (!await db.DocSet.AnyAsync(x => x.DsCode == id && x.Uid == uinfo.UserId))
            {
                return Unauthorized("Not Authorized");
            }

            var mo = new DocSetDetail
            {
                DsCode = id
            };

            if (!string.IsNullOrWhiteSpace(sid))
            {
                mo = await db.DocSetDetail.FirstOrDefaultAsync(x => x.DsdId == sid);
            }

            return View(mo);
        }

        /// <summary>
        /// 一项（保存）
        /// </summary>
        /// <param name="mo"></param>
        /// <returns></returns>
        [Authorize, HttpPost]
        public async Task<ResultVM> ItemSave([FromForm] DocSetDetail mo)
        {
            var vm = IdentityService.CompleteInfoValid(HttpContext);
            if (vm.Code == 200)
            {
                var uinfo = IdentityService.Get(HttpContext);

                if (!await db.DocSet.AnyAsync(x => x.DsCode == mo.DsCode && x.Uid == uinfo.UserId))
                {
                    vm.Set(RCodeTypes.unauthorized);
                }
                else
                {
                    mo.DsdUpdateTime = DateTime.Now;
                    mo.Uid = uinfo.UserId;

                    if (string.IsNullOrWhiteSpace(mo.DsdPid))
                    {
                        mo.DsdPid = Guid.Empty.ToString();
                    }

                    if (!mo.DsdOrder.HasValue)
                    {
                        mo.DsdOrder = 99;
                    }

                    //xss
                    mo.DsdContentHtml = CommonService.XSS(mo.DsdContentHtml);

                    if (string.IsNullOrWhiteSpace(mo.DsdId))
                    {
                        mo.DsdId = Guid.NewGuid().ToLongString();
                        mo.DsdCreateTime = mo.DsdUpdateTime;

                        await db.DocSetDetail.AddAsync(mo);
                        int num = await db.SaveChangesAsync();

                        vm.Set(num > 0);
                        vm.Data = mo.DsdId;

                        //推送通知
                        _ = PushService.PushWeChat("网站消息（Doc-item）", $"{mo.DsdTitle}");
                    }
                    else
                    {
                        //修改
                        var num = await db.DocSetDetail.Where(x => x.DsdId == mo.DsdId && x.Uid == uinfo.UserId)
                            .ExecuteUpdateAsync(x => x
                            .SetProperty(p => p.DsdTitle, mo.DsdTitle)
                            .SetProperty(p => p.DsdPid, mo.DsdPid)
                            .SetProperty(p => p.DsdOrder, mo.DsdOrder)
                            .SetProperty(p => p.DsdContentMd, mo.DsdContentMd)
                            .SetProperty(p => p.DsdContentHtml, mo.DsdContentHtml));

                        vm.Set(num > 0);
                        vm.Data = mo.DsdId;
                    }
                }
            }

            return vm;
        }

        /// <summary>
        /// 一项（删除）
        /// </summary>
        /// <returns></returns>
        [Authorize, HttpGet]
        public async Task<IActionResult> ItemDelete([FromRoute] string id, [FromRoute] string sid)
        {
            var uinfo = IdentityService.Get(HttpContext);

            if (string.IsNullOrWhiteSpace(sid))
            {
                return BadRequest();
            }
            else if (!await db.DocSet.AnyAsync(x => x.DsCode == id && x.Uid == uinfo.UserId))
            {
                return Unauthorized("Not Authorized");
            }
            else
            {
                var num = await db.DocSetDetail.Where(x => x.DsdId == sid).ExecuteDeleteAsync();
                return Redirect($"/doc/code/{id}/");
            }
        }

        /// <summary>
        /// 目录
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public async Task<IActionResult> Catalog([FromRoute] string id)
        {
            var uinfo = IdentityService.Get(HttpContext);

            if (!await db.DocSet.AnyAsync(x => x.DsCode == id && x.Uid == uinfo.UserId))
            {
                return Unauthorized("Not Authorized");
            }

            return View();
        }

        /// <summary>
        /// 目录（保存：新增、修改、删除）
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        [Authorize, HttpPost]
        public async Task<ResultVM> SaveCatalog([FromRoute] string id, [FromForm] string rows)
        {
            var vm = new ResultVM();

            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    vm.Set(RCodeTypes.failure);
                }
                else
                {
                    var listTree = string.IsNullOrWhiteSpace(rows) ? [] : rows.DeJson<List<DocTreeVM>>();

                    var uinfo = IdentityService.Get(HttpContext);
                    if (!await db.DocSet.AnyAsync(x => x.DsCode == id && x.Uid == uinfo.UserId))
                    {
                        vm.Set(RCodeTypes.unauthorized);
                    }
                    else
                    {
                        var listDetail = await db.DocSetDetail.Where(x => x.DsCode == id).ToListAsync();

                        var listAdd = new List<DocSetDetail>();
                        var listMod = new List<DocSetDetail>();
                        var listDel = new List<DocSetDetail>();

                        listDetail.ForEach(item =>
                        {
                            var mo = listTree.FirstOrDefault(x => x.DsdId == item.DsdId);
                            if (mo == null)
                            {
                                listDel.Add(item);
                            }
                            else
                            {
                                item.DsdId = mo.DsdId;
                                item.DsdTitle = mo.DsdTitle;
                                item.DsdPid = mo.DsdPid;
                                item.DsdOrder = mo.DsdOrder;
                                listMod.Add(item);
                            }
                        });

                        var listKey = listDetail.Select(x => x.DsdId).ToList();
                        var listNew = listTree.Where(x => !listKey.Contains(x.DsdId));
                        var now = DateTime.Now;
                        foreach (var item in listNew)
                        {
                            listAdd.Add(new DocSetDetail
                            {
                                DsCode = id,
                                Uid = uinfo.UserId,
                                DsdId = item.DsdId,
                                DsdTitle = item.DsdTitle,
                                DsdPid = item.DsdPid,
                                DsdOrder = item.DsdOrder,
                                DsdContentMd = item.IsCatalog ? null : item.DsdTitle,
                                DsdContentHtml = item.IsCatalog ? null : item.DsdTitle,
                                DsdCreateTime = now,
                                DsdUpdateTime = now,
                            });
                        }

                        if (listAdd.Count > 0)
                        {
                            await db.DocSetDetail.AddRangeAsync(listAdd);
                        }
                        if (listMod.Count > 0)
                        {
                            db.DocSetDetail.UpdateRange(listMod);
                        }
                        if (listDel.Count > 0)
                        {
                            db.DocSetDetail.RemoveRange(listDel);
                        }

                        vm.Data = await db.SaveChangesAsync();
                        vm.Set(RCodeTypes.success);
                    }
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        /// <summary>
        /// 生成 Html
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult ToHtml([FromRoute] string id)
        {
            var uinfo = IdentityService.Get(HttpContext);

            var mo = db.DocSet.Find(id);
            if (mo != null && (mo.DsOpen == 1 || uinfo?.UserId == mo.Uid))
            {
                var list = db.DocSetDetail.Where(x => x.DsCode == id).OrderBy(x => x.DsdOrder).Select(x => new
                {
                    x.Uid,
                    x.DsdId,
                    x.DsdPid,
                    x.DsdTitle,
                    x.DsdOrder,
                    IsCatalog = string.IsNullOrEmpty(x.DsdContentMd),
                    x.DsdContentHtml
                }).ToList();

                var htmlbody = ListTreeEach(list, "DsdPid", "DsdId", [Guid.Empty.ToString()]);

                return Content(htmlbody);
            }
            else
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// 生成Html
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="pidField"></param>
        /// <param name="idField"></param>
        /// <param name="startPid"></param>
        /// <param name="listNo"></param>
        /// <param name="deep"></param>
        /// <returns></returns>
        private string ListTreeEach<T>(List<T> list, string pidField, string idField, List<string> startPid, List<int> listNo = null, int deep = 2)
        {
            StringBuilder sbTree = new();

            var rdt = list.Where(x => startPid.Contains(x.GetType().GetProperty(pidField).GetValue(x, null).ToString())).ToList();

            for (int i = 0; i < rdt.Count; i++)
            {
                var dr = rdt[i];
                var drgt = dr.GetType();

                //H标签
                var hn = deep;
                if (deep > 6)
                {
                    hn = 6;
                }

                //序号
                if (listNo == null)
                {
                    listNo = [i + 1];
                }
                else
                {
                    listNo.Add(i + 1);
                }

                //标题
                var title = drgt.GetProperty("DsdTitle").GetValue(dr, null).ToString();
                sbTree.AppendLine($"<h{hn}>{string.Join(".", listNo)}、{title}</h{hn}>");

                //是目录
                var iscatalog = Convert.ToBoolean(drgt.GetProperty("IsCatalog").GetValue(dr, null));
                if (!iscatalog)
                {
                    sbTree.AppendLine(drgt.GetProperty("DsdContentHtml").GetValue(dr, null).ToString());
                }

                var pis = drgt.GetProperties();

                var pi = pis.FirstOrDefault(x => x.Name == idField);
                startPid.Clear();
                var id = pi.GetValue(dr, null).ToString();
                startPid.Add(id);

                var nrdt = list.Where(x => x.GetType().GetProperty(pidField).GetValue(x, null).ToString() == id.ToString()).ToList();

                if (nrdt.Count > 0)
                {
                    string rs = ListTreeEach(list, pidField, idField, startPid, listNo, deep + 1);

                    //子数组源于递归
                    sbTree.AppendLine(rs);
                }

                while (listNo.Count > deep - 1)
                {
                    listNo.RemoveAt(listNo.Count - 1);
                }
            }

            return sbTree.ToString();
        }

        /// <summary>
        /// 目录树
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sid"></param>
        /// <returns></returns>
        [Authorize, HttpGet]
        public ResultVM MenuTree([FromRoute] string id, [FromRoute] string sid)
        {
            var vm = new ResultVM();

            try
            {
                var list = db.DocSetDetail.Where(x => x.DsCode == id).OrderBy(x => x.DsdOrder).Select(x => new
                {
                    x.DsdId,
                    x.DsdPid,
                    x.DsdTitle,
                    x.DsdOrder,
                    IsCatalog = x.DsdContentMd == null || x.DsdContentMd.Length == 0
                }).ToList();

                if (sid == "parent")
                {
                    vm.Data = list;
                    vm.Set(RCodeTypes.success);
                }
                else
                {
                    var listtree = TreeTo.ListToTree(list, "DsdPid", "DsdId", [Guid.Empty.ToString()]);
                    if (string.IsNullOrWhiteSpace(listtree))
                    {
                        vm.Set(RCodeTypes.failure);
                    }
                    else
                    {
                        vm.Data = listtree.DeJson();
                        vm.Set(RCodeTypes.success);
                    }
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
                LoggingService.Write(HttpContext, ex);
            }

            return vm;
        }

    }
}