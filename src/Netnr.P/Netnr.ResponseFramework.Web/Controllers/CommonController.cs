using Microsoft.AspNetCore.Authorization;

namespace Netnr.ResponseFramework.Web.Controllers
{
    /// <summary>
    /// 公共、常用查询
    /// </summary>
    [Authorize]
    [Route("[controller]/[action]")]
    public class CommonController : Controller
    {
        public ContextBase db;

        public CommonController(ContextBase cb)
        {
            db = cb;
        }

        /// <summary>
        /// 公共查询：菜单树
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet]
        [HttpPost]
        public string QueryMenu(string type)
        {
            string result = string.Empty;
            var listMenu = CommonService.QuerySysMenuList(x => x.SmStatus == 1, false);
            if (type != "all")
            {
                #region 根据登录用户查询角色配置的菜单
                var uinfo = IdentityService.Get(HttpContext);
                if (uinfo != null)
                {
                    var role = CommonService.QuerySysRoleEntity(x => x.SrId == uinfo.RoleId);
                    if (role != null)
                    {
                        var menuArray = role.SrMenus.Split(',').ToList();

                        listMenu = listMenu.Where(x => menuArray.Contains(x.SmId)).ToList();
                    }
                    else
                    {
                        listMenu = new List<SysMenu>();
                    }
                }
                else
                {
                    listMenu = new List<SysMenu>();
                }
                #endregion
            }

            #region 把实体转为JSON节点实体
            var listNode = new List<TreeNodeVM>();
            foreach (var item in listMenu)
            {
                listNode.Add(new TreeNodeVM()
                {
                    Id = item.SmId,
                    Pid = item.SmPid,
                    Text = item.SmName,
                    Ext1 = item.SmUrl,
                    Ext2 = item.SmIcon
                });
            }
            #endregion

            result = TreeTo.ListToTree(listNode, "Pid", "Id", new List<string> { Guid.Empty.ToString() });

            if (string.IsNullOrWhiteSpace(result))
            {
                result = "[]";
            }

            return result;
        }

        /// <summary>
        /// 公共查询：功能按钮树
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HttpPost]
        public string QueryButtonTree()
        {
            string result = string.Empty;
            var list = CommonService.QuerySysButtonList(x => x.SbStatus == 1);

            #region 把实体转为JSON节点实体
            var listNode = new List<TreeNodeVM>();
            foreach (var item in list)
            {
                listNode.Add(new TreeNodeVM()
                {
                    Id = item.SbId,
                    Pid = item.SbPid,
                    Text = item.SbBtnText,
                    Ext1 = item.SbBtnIcon,
                    Ext2 = item.SbBtnClass,
                    Ext3 = item.SbDescribe
                });
            }
            #endregion

            result = TreeTo.ListToTree(listNode, "Pid", "Id", new List<string> { Guid.Empty.ToString() });

            if (string.IsNullOrWhiteSpace(result))
            {
                result = "[]";
            }

            return result;
        }

        /// <summary>
        /// 公共查询：角色列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HttpPost]
        public List<ValueTextVM> QueryRole()
        {
            var query = from a in db.SysRole
                        where a.SrStatus == 1
                        orderby a.SrCreateTime
                        select new ValueTextVM
                        {
                            Value = a.SrId,
                            Text = a.SrName
                        };
            var list = query.ToList();
            return list;
        }

        /// <summary>
        /// 公共查询：查询数据字典的例子
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HttpPost]
        public List<ValueTextVM> QueryDictionaryDemo()
        {
            var list = db.SysDictionary.Where(x => x.SdType == "SysDictionary:SdType" && x.SdStatus == 1).Select(x => new ValueTextVM
            {
                Value = x.SdId,
                Text = x.SdValue
            }).ToList();

            return list;
        }

        /// <summary>
        /// 公共上传，支持同时上传多个
        /// </summary>
        /// <param name="files">文件</param>
        /// <param name="temp">temp=1,表示临时文件</param>
        /// <param name="subdir">自定义子目录，如：doc</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResultVM> Upload(IFormFileCollection files, int? temp, string subdir)
        {
            var vm = new ResultVM();

            try
            {
                if (files.Count > 0)
                {
                    var now = DateTime.Now;

                    //虚拟路径
                    var vpath = AppTo.GetValue("StaticResource:RootDir");
                    if (temp == 1)
                    {
                        vpath = AppTo.GetValue("StaticResource:TmpDir");
                    }
                    else
                    {
                        vpath = PathTo.Combine(vpath, subdir, now.ToString("yyyy'/'MM'"));
                    }

                    //物理路径
                    var ppath = PathTo.Combine(AppTo.WebRootPath, vpath);
                    if (!Directory.Exists(ppath))
                    {
                        Directory.CreateDirectory(ppath);
                    }

                    var listPath = new List<string>();
                    for (int i = 0; i < files.Count; i++)
                    {
                        var file = files[i];
                        var ext = Path.GetExtension(file.FileName);
                        var filename = now.ToString("ddHHmmss") + RandomTo.NewNumber() + ext;

                        using (var stream = new FileStream(PathTo.Combine(ppath, filename), FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        listPath.Add(PathTo.Combine(vpath, filename));
                    }

                    if (listPath.Count == 1)
                    {
                        vm.Data = listPath.FirstOrDefault();
                    }
                    else
                    {
                        vm.Data = listPath;
                    }
                    vm.Set(EnumTo.RTag.success);
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        /// <summary>
        /// 公共上传，富文本附件，限制大小2MB
        /// </summary>
        /// <param name="upload">一个文件</param>
        /// <returns></returns>
        [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit = 1024 * 1024 * 2)]
        public async Task<string> UploadRich(IFormFileCollection upload)
        {
            //调用通用的上传接口，富文本文件存放子目录：rich
            var vm = await Upload(upload, null, "rich");

            //返回富文本支持的接口信息
            if (vm.Code == 200)
            {
                var path = vm.Data.ToString();
                return new
                {
                    uploaded = 1,
                    fileName = path.Split('/').LastOrDefault(),
                    url = path
                }.ToJson();
            }
            else
            {
                return new
                {
                    uploaded = 0,
                    error = new
                    {
                        message = vm.Msg
                    }
                }.ToJson();
            }
        }
    }
}