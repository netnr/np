using Netnr.FileServer.Application;
using Netnr.FileServer.Model;
using Netnr.Core;
using Netnr.SharedFast;
using Microsoft.AspNetCore.StaticFiles;

namespace Netnr.FileServer.Controllers
{
    /// <summary>
    /// API接口
    /// </summary>
    [Route("[controller]/[action]")]
    [Apps.FilterConfigs.AllowCors]
    public class APIController : ControllerBase
    {
        /// <summary>
        /// 【管理】创建App
        /// </summary>
        /// <param name="password">密码，必填</param>
        /// <param name="owner">用户，唯一，文件夹名，推荐小写字母</param>
        /// <returns></returns>
        [HttpGet]
        public SharedResultVM CreateApp(string password, string owner)
        {
            var vm = new SharedResultVM();

            try
            {
                if (!ParsingTo.IsLinkPath(owner))
                {
                    vm.Msg = "owner 必填，仅为字母、数字";
                }
                else if (owner.ToLower() == GlobalTo.GetValue("StaticResource:TmpDir").ToLower())
                {
                    vm.Msg = "owner 与临时目录冲突";
                }
                else if (string.IsNullOrWhiteSpace(password) || password != GlobalTo.GetValue("Safe:AdminPassword"))
                {
                    vm.Set(SharedEnum.RTag.unauthorized);
                    vm.Msg = "密码错误或已关闭管理接口";
                }
                else
                {
                    vm = FileServerService.CreateApp(owner);
                    if (vm.Code == -1 && vm.Msg.Contains("UNIQUE"))
                    {
                        vm.Set(SharedEnum.RTag.exist);
                        vm.Msg = "owner 用户已经存在";
                    }
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
                ConsoleTo.Log(ex);
            }

            return vm;
        }

        /// <summary>
        /// 【管理】获取App列表
        /// </summary>
        /// <param name="password">密码</param>
        /// <param name="pageNumber">页码，默认1</param>
        /// <param name="pageSize">页量，默认20</param>
        /// <returns></returns>
        [HttpGet]
        public SharedResultVM GetAppList(string password, int pageNumber = 1, int pageSize = 20)
        {
            var vm = new SharedResultVM();

            try
            {
                if (string.IsNullOrWhiteSpace(password) || password != GlobalTo.GetValue("Safe:AdminPassword"))
                {
                    vm.Set(SharedEnum.RTag.unauthorized);
                    vm.Msg = "密码错误或已关闭管理接口";
                }
                else
                {
                    vm = FileServerService.GetAppList(pageNumber, pageSize);
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
                ConsoleTo.Log(ex);
            }

            return vm;
        }

        /// <summary>
        /// 获取App信息
        /// </summary>
        /// <param name="AppId">分配的应用ID</param>
        /// <param name="AppKey">分配的应用密钥</param>
        /// <returns></returns>
        [HttpGet]
        public SharedResultVM GetAppInfo(string AppId, string AppKey)
        {
            var vm = new SharedResultVM();

            try
            {
                vm = FileServerService.GetAppInfo(AppId, AppKey);
            }
            catch (Exception ex)
            {
                vm.Set(ex);
                ConsoleTo.Log(ex);
            }

            return vm;
        }

        /// <summary>
        /// 【管理】清空数据库和上传文件
        /// </summary>
        /// <param name="password">密码，必填</param>
        /// <returns></returns>
        [HttpGet]
        public SharedResultVM ResetAll(string password)
        {
            var vm = new SharedResultVM();

            try
            {
                if (string.IsNullOrWhiteSpace(password) || password != GlobalTo.GetValue("Safe:AdminPassword"))
                {
                    vm.Set(SharedEnum.RTag.unauthorized);
                    vm.Msg = "密码错误或已关闭管理接口";
                }
                else
                {
                    //清空数据库
                    using var db = new SQLite.SQLiteConnection(FileServerService.SQLiteConn);
                    db.DeleteAll<SysApp>();
                    db.DeleteAll<FileRecord>();

                    //删除上传文件
                    var rootdir = FileServerService.StaticVrPathAsPhysicalPath(GlobalTo.GetValue("StaticResource:RootDir"));
                    if (Directory.Exists(rootdir))
                    {
                        Directory.Delete(rootdir, true);
                    }

                    vm.Set(SharedEnum.RTag.success);
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
                ConsoleTo.Log(ex);
            }

            return vm;
        }

        /// <summary>
        /// 【管理】清理临时目录
        /// </summary>
        /// <param name="password">密码，必填</param>
        /// <param name="keepTime">保留最近文件，超过时间被清理，0为全部清理，单位：分</param>
        /// <returns></returns>
        [HttpGet]
        public SharedResultVM ClearTmp(string password, int keepTime = 30)
        {
            var vm = new SharedResultVM();

            try
            {
                if (string.IsNullOrWhiteSpace(password) || password != GlobalTo.GetValue("Safe:AdminPassword"))
                {
                    vm.Set(SharedEnum.RTag.unauthorized);
                    vm.Msg = "密码错误或已关闭管理接口";
                }
                else
                {
                    var listDel = new List<string>();

                    //删除临时文件
                    var tmpdir = FileServerService.StaticVrPathAsPhysicalPath(GlobalTo.GetValue("StaticResource:TmpDir"));
                    if (Directory.Exists(tmpdir))
                    {
                        if (keepTime > 0)
                        {
                            var now = DateTime.Now;
                            Directory.GetFiles(tmpdir).ToList().ForEach(item =>
                            {
                                var fi = new FileInfo(item);
                                if ((now - fi.CreationTime).TotalMinutes > keepTime)
                                {
                                    fi.Delete();
                                    listDel.Add(item);
                                }
                            });

                            Directory.GetDirectories(tmpdir).ToList().ForEach(item =>
                            {
                                var di = new DirectoryInfo(item);
                                if ((now - di.CreationTime).TotalMinutes > keepTime)
                                {
                                    di.Delete(true);
                                    listDel.Add(item);
                                }
                            });
                        }
                        else
                        {
                            listDel = Directory.GetFileSystemEntries(tmpdir).ToList();

                            Directory.Delete(tmpdir, true);
                            Directory.CreateDirectory(tmpdir);
                        }
                    }

                    vm.Set(SharedEnum.RTag.success);
                    vm.Data = listDel;
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
                ConsoleTo.Log(ex);
            }

            return vm;
        }

        /// <summary>
        /// 获取Token
        /// </summary>
        /// <param name="AppId">分配的应用ID</param>
        /// <param name="AppKey">分配的应用密钥</param>
        /// <returns></returns>
        [HttpGet, ResponseCache(Duration = 30)]
        public SharedResultVM GetToken(string AppId, string AppKey)
        {
            var vm = new SharedResultVM();

            try
            {
                if (string.IsNullOrWhiteSpace(AppId) || string.IsNullOrWhiteSpace(AppKey))
                {
                    vm.Set(SharedEnum.RTag.lack);
                    vm.Msg = "参数缺失";
                }
                else
                {
                    if (CacheTo.Get(AppKey) is not SharedResultVM cvm)
                    {
                        vm = FileServerService.GetToken(AppId, AppKey);

                        if (vm.Code == 200)
                        {
                            //Token缓存
                            CacheTo.Set(AppKey, vm, GlobalTo.GetValue<int>("Safe:TokenCache"), false);
                        }
                    }
                    else
                    {
                        vm = cvm;
                    }
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
                ConsoleTo.Log(ex);
            }

            return vm;
        }

        /// <summary>
        /// 创建FixedToken
        /// </summary>
        /// <param name="AppId">分配的应用ID</param>
        /// <param name="AppKey">分配的应用密钥</param>
        /// <param name="Name">名称</param>
        /// <param name="AuthMethod">授权接口名，多个用逗号分割，区分大小写</param>
        /// <returns></returns>
        [HttpGet]
        public SharedResultVM CreateFixedToken(string AppId, string AppKey, string Name, string AuthMethod)
        {
            var vm = new SharedResultVM();

            try
            {
                if (string.IsNullOrWhiteSpace(AppId) || string.IsNullOrWhiteSpace(AppKey) || string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(AuthMethod))
                {
                    vm.Set(SharedEnum.RTag.lack);
                    vm.Msg = "参数缺失";
                }
                else
                {
                    var listAm = AuthMethodList().Data as List<string>;
                    if (!AuthMethod.Split(',').ToList().All(x => listAm.Contains(x)))
                    {
                        vm.Set(SharedEnum.RTag.invalid);
                        vm.Msg = "AuthMethod 存在无效接口名";
                    }
                    else
                    {
                        vm = FileServerService.CreateFixedToken(AppId, AppKey, Name, AuthMethod);
                    }
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
                ConsoleTo.Log(ex);
            }

            return vm;
        }

        /// <summary>
        /// 删除FixedToken
        /// </summary>
        /// <param name="AppId">分配的应用ID</param>
        /// <param name="AppKey">分配的应用密钥</param>
        /// <param name="FixedToken">固定 Token</param>
        /// <returns></returns>
        [HttpGet]
        public SharedResultVM DelFixedToken(string AppId, string AppKey, string FixedToken)
        {
            var vm = new SharedResultVM();

            try
            {
                if (string.IsNullOrWhiteSpace(AppId) || string.IsNullOrWhiteSpace(AppKey) || string.IsNullOrWhiteSpace(FixedToken))
                {
                    vm.Set(SharedEnum.RTag.lack);
                    vm.Msg = "参数缺失";
                }
                else
                {
                    vm = FileServerService.DelFixedToken(AppId, AppKey, FixedToken);
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
                ConsoleTo.Log(ex);
            }

            return vm;
        }

        /// <summary>
        /// 授权方法列表（即需要传Token验证的方法名）
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public SharedResultVM AuthMethodList()
        {
            var vm = new SharedResultVM();

            try
            {
                var listA = new List<string>();

                var type = typeof(APIController);
                var methods = type.GetMethods().Where(x => x.DeclaringType == type).ToList();
                foreach (var method in methods)
                {
                    if (method.GetParameters().Any(x => x.Name.Equals("token", StringComparison.OrdinalIgnoreCase)))
                    {
                        listA.Add(method.Name);
                    }
                }

                vm.Set(SharedEnum.RTag.success);
                vm.Data = listA;
            }
            catch (Exception ex)
            {
                vm.Set(ex);
                ConsoleTo.Log(ex);
            }

            return vm;
        }

        /// <summary>
        /// 上传文件，文件流
        /// </summary>
        /// <param name="files">文件流</param>
        /// <param name="token">token，授权验证，必填</param>
        /// <param name="subdir">子目录，可选</param>
        /// <returns></returns>
        [HttpPost, HttpOptions]
        public SharedResultVM Upload(IFormFileCollection files, [FromForm] string token, [FromForm] string subdir)
        {
            var vm = new SharedResultVM();

            var mn = RouteData.Values["Action"].ToString();
            try
            {
                if (files == null || files.Count < 1)
                {
                    vm.Msg = "未找到上传文件";
                }
                else if (!string.IsNullOrWhiteSpace(subdir) && !ParsingTo.IsLinkPath(subdir))
                {
                    vm.Msg = "subdir 仅为字母、数字";
                }
                else
                {
                    //验证token
                    var vt = FileServerService.ValidToken(token, mn);
                    if (vt.Code != 200)
                    {
                        vm = vt;
                    }
                    else
                    {
                        var vtjson = vt.Data as FixedTokenJson;
                        var now = DateTime.Now;

                        var listFr = new List<FileRecord>();

                        //虚拟路径
                        var vpath = PathTo.Combine(GlobalTo.GetValue("StaticResource:RootDir"), vtjson.Owner, subdir, now.ToString("yyyy'/'MM'/'dd"));

                        //物理路径
                        var ppath = FileServerService.StaticVrPathAsPhysicalPath(vpath);
                        if (!Directory.Exists(ppath))
                        {
                            Directory.CreateDirectory(ppath);
                        }

                        foreach (var file in files)
                        {
                            var fr = new FileRecord()
                            {
                                Id = now.ToTimestamp() + RandomTo.NumCode(),
                                OwnerUser = vtjson.Owner,
                                Name = Path.GetFileName(file.FileName),
                                Size = file.Length,
                                Type = file.ContentType,
                                CreateTime = now
                            };

                            var filename = fr.Id + Path.GetExtension(file.FileName);

                            using (var fs = new FileStream(PathTo.Combine(ppath, filename), FileMode.CreateNew))
                            {
                                file.CopyTo(fs);
                                fs.Flush();
                            }

                            fr.Path = PathTo.Combine(vpath, filename);
                            listFr.Add(fr);
                        }

                        vm = FileServerService.InsertFile(listFr);
                        if (vm.Code == 200)
                        {
                            if (listFr.Count == 1)
                            {
                                vm.Data = listFr.First();
                            }
                            else
                            {
                                vm.Data = listFr;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
                ConsoleTo.Log(ex);
            }

            return vm;
        }

        /// <summary>
        /// 分块上传
        /// </summary>
        /// <param name="file">分片文件</param>
        /// <param name="token">token，授权验证，必填</param>
        /// <param name="ts">唯一标识</param>
        /// <param name="subdir">子目录，可选</param>
        /// <param name="chunk">当前分片索引</param>
        /// <param name="chunks">总分片数</param>
        /// <returns></returns>
        [HttpPost, HttpOptions]
        public SharedResultVM UploadChunk(IFormFile file, [FromForm] string token, [FromForm] string subdir, [FromForm] string ts, [FromForm] int chunk, [FromForm] int chunks)
        {
            var vm = new SharedResultVM();

            var mn = RouteData.Values["Action"].ToString();
            try
            {
                if (file == null)
                {
                    vm.Msg = "未找到上传文件";
                }
                else if (!ParsingTo.IsLinkPath(ts))
                {
                    vm.Msg = "ts 仅为字母、数字";
                }
                else if (!string.IsNullOrWhiteSpace(subdir) && !ParsingTo.IsLinkPath(subdir))
                {
                    vm.Msg = "subdir 仅为字母、数字";
                }
                else if (file.Length * (chunks - 1) > GlobalTo.GetValue<int>("StaticResource:MaxSize") * 1024 * 1024)
                {
                    vm.Msg = "文件大小超出限制";
                }
                else
                {
                    var vtkey = "vt-" + token;
                    if (CacheTo.Get(vtkey) is not SharedResultVM vt)
                    {
                        vt = FileServerService.ValidToken(token, mn);
                        //缓存 Token 验证 30 分钟
                        CacheTo.Set(vtkey, vt, 1800, false);
                    }

                    if (vt.Code != 200)
                    {
                        vm = vt;
                    }
                    else
                    {
                        //分片
                        if (chunks > 0)
                        {
                            //分片临时虚拟目录
                            var chunkDir = PathTo.Combine(GlobalTo.GetValue("StaticResource:TmpDir"), ts);

                            var ext = Path.GetExtension(file.FileName);
                            //存入分片临时目录（格式：Id_片索引.文件格式后缀）
                            var chunkName = $"{ts}_{chunk}.{ext}";

                            //保存分片物理路径
                            var chunkPPath = FileServerService.StaticVrPathAsPhysicalPath(chunkDir);
                            if (!Directory.Exists(chunkPPath))
                            {
                                Directory.CreateDirectory(chunkPPath);
                            }

                            using (var fs = new FileStream(PathTo.Combine(chunkPPath, chunkName), FileMode.CreateNew))
                            {
                                file.CopyTo(fs);
                                fs.Flush();
                            }

                            //记录已上传的分片总数
                            var ckkey = $"chunk-{ts}";
                            var ci = CacheTo.Get(ckkey) as int? ?? 0;
                            ci++;
                            CacheTo.Set(ckkey, ci);

                            //所有片已上传完，合并片
                            if (ci == chunks)
                            {
                                var now = DateTime.Now;

                                var vtjson = vt.Data as FixedTokenJson;

                                //虚拟路径
                                var vpath = PathTo.Combine(GlobalTo.GetValue("StaticResource:RootDir"), vtjson.Owner, subdir, now.ToString("yyyy'/'MM'/'dd"));

                                //物理路径
                                var ppath = FileServerService.StaticVrPathAsPhysicalPath(vpath);
                                if (!Directory.Exists(ppath))
                                {
                                    Directory.CreateDirectory(ppath);
                                }

                                var fr = new FileRecord()
                                {
                                    Id = now.ToTimestamp() + RandomTo.NumCode(),
                                    OwnerUser = vtjson.Owner,
                                    Name = Path.GetFileName(file.FileName),
                                    Size = file.Length,
                                    Type = file.ContentType,
                                    CreateTime = now
                                };
                                var filename = fr.Id + Path.GetExtension(file.FileName);

                                using var fs = new FileStream(PathTo.Combine(ppath, filename), FileMode.Create);
                                //排序从 0-N Write
                                var po = Directory.GetFiles(chunkPPath).OrderBy(x => x.Length).ThenBy(x => x).ToList();
                                foreach (var part in po)
                                {
                                    var bytes = System.IO.File.ReadAllBytes(part);
                                    fs.Write(bytes, 0, bytes.Length);
                                    bytes = null;
                                }
                                //删除分块文件夹
                                Directory.Delete(chunkPPath, true);

                                fr.Path = PathTo.Combine(vpath, filename);

                                vm = FileServerService.InsertFile(fr);
                                if (vm.Code == 200)
                                {
                                    vm.Data = fr;
                                }
                            }
                            else
                            {
                                vm.Code = 201;
                                vm.Data = PathTo.Combine(chunkDir, chunkName);
                                vm.Msg = "chunk success";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
                ConsoleTo.Log(ex);
            }

            return vm;
        }

        /// <summary>
        /// 查看文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet, ResponseCache(Duration = 30)]
        public ActionResult View(string path, string token)
        {
            var vm = new SharedResultVM();

            var mn = RouteData.Values["Action"].ToString();
            try
            {
                var isAuth = false;
                //公开访问
                if (GlobalTo.GetValue<bool>("Safe:PublicAccess"))
                {
                    isAuth = true;
                }
                else
                {
                    isAuth = FileServerService.ValidToken(token, mn).Code == 200;
                }

                if (isAuth)
                {
                    var ppath = FileServerService.StaticVrPathAsPhysicalPath(path);
                    if (System.IO.File.Exists(ppath))
                    {
                        new FileExtensionContentTypeProvider().TryGetContentType(path, out string contentType);
                        return PhysicalFile(ppath, contentType ?? "application/octet-stream");
                    }
                    else
                    {
                        vm.Set(SharedEnum.RTag.lack);
                    }
                }
                else
                {
                    vm.Set(SharedEnum.RTag.unauthorized);
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
                ConsoleTo.Log(ex);
            }

            return Content(vm.ToJson());
        }

        /// <summary>
        /// 复制文件
        /// </summary>
        /// <param name="token">token，授权验证，必填</param>
        /// <param name="path">文件路径或文件ID，必填</param>
        /// <returns></returns>
        [HttpGet]
        public SharedResultVM Copy(string token, string path)
        {
            var vm = new SharedResultVM();

            var mn = RouteData.Values["Action"].ToString();
            try
            {
                //验证token
                var vt = FileServerService.ValidToken(token, mn);
                if (vt.Code != 200)
                {
                    vm = vt;
                }
                else
                {
                    var vtjson = vt.Data as FixedTokenJson;

                    var qf = FileServerService.QueryFile(vtjson.Owner, path);
                    if (qf.Code != 200)
                    {
                        vm = qf;
                    }
                    else
                    {
                        //要复制的文件信息
                        var nowfile = qf.Data as FileRecord;
                        //要复制的文件虚拟路径
                        var vpath = nowfile.Path;

                        var now = DateTime.Now;
                        var fr = new FileRecord()
                        {
                            Id = now.ToTimestamp() + RandomTo.NumCode(),
                            OwnerUser = nowfile.OwnerUser,
                            Name = nowfile.Name,
                            Size = nowfile.Size,
                            Type = nowfile.Type,
                            CreateTime = DateTime.Now
                        };

                        //要复制的物理路径
                        var ppath = FileServerService.StaticVrPathAsPhysicalPath(vpath);
                        fr.Path = vpath.Replace(Path.GetFileNameWithoutExtension(vpath), fr.Id);
                        //复制的新物理路径
                        var newppath = FileServerService.StaticVrPathAsPhysicalPath(fr.Path);

                        System.IO.File.Copy(ppath, newppath);

                        vm = FileServerService.InsertFile(new List<FileRecord> { fr });
                        if (vm.Code == 200)
                        {
                            vm.Data = fr;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
                ConsoleTo.Log(ex);
            }

            return vm;
        }

        /// <summary>
        /// 覆盖文件，文件流
        /// </summary>
        /// <param name="file">文件流</param>
        /// <param name="token">token，授权验证，必填</param>
        /// <param name="path">文件路径或文件ID，必填</param>
        /// <returns></returns>
        [HttpPost, HttpOptions]
        public SharedResultVM Cover(IFormFile file, [FromForm] string token, [FromForm] string path)
        {
            var vm = new SharedResultVM();

            var mn = RouteData.Values["Action"].ToString();
            try
            {
                if (file == null)
                {
                    vm.Msg = "没找到上传的文件";
                }
                else
                {
                    //验证token
                    var vt = FileServerService.ValidToken(token, mn);
                    if (vt.Code != 200)
                    {
                        vm = vt;
                    }
                    else
                    {
                        var vtjson = vt.Data as FixedTokenJson;

                        var qf = FileServerService.QueryFile(vtjson.Owner, path);

                        if (qf.Code != 200)
                        {
                            vm = qf;
                        }
                        else
                        {
                            var nowfile = qf.Data as FileRecord;
                            var ppath = FileServerService.StaticVrPathAsPhysicalPath(nowfile.Path);

                            if (System.IO.File.Exists(ppath))
                            {
                                System.IO.File.Delete(ppath);

                                using (var fs = new FileStream(ppath, FileMode.CreateNew))
                                {
                                    file.CopyTo(fs);
                                    fs.Flush();
                                }

                                //更新信息
                                nowfile.Name = file.Name;
                                nowfile.Size = file.Length;
                                nowfile.Type = file.ContentType;
                                nowfile.CreateTime = DateTime.Now;

                                vm = FileServerService.UpdateFile(nowfile);
                            }
                            else
                            {
                                vm.Set(SharedEnum.RTag.invalid);
                                vm.Msg = "文件路径无效";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
                ConsoleTo.Log(ex);
            }

            return vm;
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="token">token，授权验证，必填</param>
        /// <param name="path">文件路径或文件ID，必填</param>
        /// <returns></returns>
        [HttpGet]
        public SharedResultVM Delete(string token, string path)
        {
            var vm = new SharedResultVM();

            var mn = RouteData.Values["Action"].ToString();
            try
            {
                //验证token
                var vt = FileServerService.ValidToken(token, mn);
                if (vt.Code != 200)
                {
                    vm = vt;
                }
                else
                {
                    var vtjson = vt.Data as FixedTokenJson;

                    vm = FileServerService.DeleteFile(vtjson.Owner, path);
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
                ConsoleTo.Log(ex);
            }

            return vm;
        }

        /// <summary>
        /// 上传临时文件，不记录数据库
        /// </summary>
        /// <param name="file">文件流</param>
        /// <returns></returns>
        [HttpPost, HttpOptions]
        public SharedResultVM UploadTmp(IFormFile file)
        {
            var vm = new SharedResultVM();

            try
            {
                if (!GlobalTo.GetValue<bool>("Safe:EnableUploadTmp"))
                {
                    vm.Set(SharedEnum.RTag.refuse);
                    vm.Msg = "该接口已关闭";
                }
                else
                {
                    if (file == null)
                    {
                        vm.Set(SharedEnum.RTag.lack);
                        vm.Msg = "未找到上传文件";
                    }
                    else
                    {
                        var vpath = GlobalTo.GetValue("StaticResource:TmpDir");
                        var ppath = FileServerService.StaticVrPathAsPhysicalPath(vpath);
                        if (!Directory.Exists(ppath))
                        {
                            Directory.CreateDirectory(ppath);
                        }

                        var filename = DateTime.Now.ToTimestamp() + RandomTo.NumCode() + Path.GetExtension(file.FileName);

                        using (var fs = new FileStream(PathTo.Combine(ppath, filename), FileMode.CreateNew))
                        {
                            file.CopyTo(fs);
                            fs.Flush();
                        }

                        vm.Set(SharedEnum.RTag.success);
                        vm.Data = PathTo.Combine(vpath, filename);
                    }
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
                ConsoleTo.Log(ex);
            }

            return vm;
        }
    }
}
