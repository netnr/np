using Microsoft.AspNetCore.StaticFiles;
using System.Collections.Generic;

namespace Netnr.FileServer.Controllers
{
    /// <summary>
    /// API接口
    /// </summary>
    [Route("[controller]/[action]")]
    public class APIController : Controller
    {
        #region tmp

        /// <summary>
        /// 上传临时文件，不记录数据库
        /// </summary>
        /// <param name="file">文件流</param>
        /// <returns></returns>
        [HttpPost]
        public ResultVM UploadTmp(IFormFile file)
        {
            var vm = new ResultVM();

            if (!AppTo.GetValue<bool>("Safe:EnableUploadTmp"))
            {
                vm.Set(RCodeTypes.refuse);
                vm.Msg = "该接口已关闭";
            }
            else
            {
                if (file == null)
                {
                    vm.Set(RCodeTypes.failure);
                    vm.Msg = "未找到上传文件";
                }
                else if (AppService.IsDisableExtension(file.FileName))
                {
                    vm.Set(RCodeTypes.refuse);
                    vm.Msg = "File extension not supported";
                }
                else
                {
                    var vpath = AppTo.GetValue("StaticResource:TmpDir");
                    var ppath = AppService.StaticVrPathAsPhysicalPath(vpath);
                    if (!Directory.Exists(ppath))
                    {
                        Directory.CreateDirectory(ppath);
                    }

                    var filename = $"{RandomTo.NewString(4)}{Path.GetExtension(file.FileName)}".ToLower();

                    using (var fs = new FileStream(ParsingTo.Combine(ppath, filename), FileMode.CreateNew))
                    {
                        file.CopyTo(fs);
                        fs.Flush();
                    }

                    vm.Set(RCodeTypes.success);
                    vm.Data = ParsingTo.Combine(vpath, filename);
                    vm.Log.Add($"{AppTo.GetValue<int>("StaticResource:TmpExpire")} 分钟后失效");
                }
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
        public ResultVM SaClearTmpGet(string password, int keepTime = 30)
        {
            var vm = new ResultVM();

            if (string.IsNullOrWhiteSpace(password) || password != AppTo.GetValue("Safe:AdminPassword"))
            {
                vm.Set(RCodeTypes.unauthorized);
                vm.Msg = "密码错误或已关闭管理接口";
            }
            else
            {
                vm = AppService.ClearTmp(keepTime);
            }

            return vm;
        }

        #endregion

        /// <summary>
        /// 【管理】创建App
        /// </summary>
        /// <param name="password">密码，必填</param>
        /// <param name="owner">用户，唯一，文件夹名，推荐小写字母</param>
        /// <returns></returns>
        [HttpPost]
        public ResultVM SaAppPost([FromForm] string password, [FromForm] string owner)
        {
            var vm = new ResultVM();

            if (!ParsingTo.IsLinkPath(owner))
            {
                vm.Msg = "owner 必填，仅为字母、数字";
            }
            else if (owner.Equals(AppTo.GetValue("StaticResource:TmpDir"), StringComparison.OrdinalIgnoreCase))
            {
                vm.Msg = "owner 与临时目录冲突";
            }
            else if (string.IsNullOrWhiteSpace(password) || password != AppTo.GetValue("Safe:AdminPassword"))
            {
                vm.Set(RCodeTypes.unauthorized);
                vm.Msg = "密码错误或已关闭管理接口";
            }
            else
            {
                var model = new BaseApp()
                {
                    AppId = Snowflake53To.Id(),
                    AppKey = Guid.NewGuid().ToString("N").ToUpper(),
                    CreateTime = DateTime.Now,
                    AppOwner = owner,
                    AppToken = AppService.NewToken(),
                    AppTokenExpireTime = DateTime.Now.AddMinutes(AppTo.GetValue<int>("Safe:TokenExpired")),
                    AppRemark = "通过接口创建"
                };

                try
                {
                    AppService.CollSysApp.Insert(model);

                    vm.Data = model;
                    vm.Set(RCodeTypes.success);
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("unique", StringComparison.OrdinalIgnoreCase))
                    {
                        vm.Set(RCodeTypes.exist);
                        vm.Msg = "owner 用户已经存在";
                        vm.Data = ex.ToTree();
                    }
                }
            }

            return vm;
        }

        /// <summary>
        /// 【管理】获取App列表
        /// </summary>
        /// <param name="password">密码</param>
        /// <returns></returns>
        [HttpGet]
        public ResultVM SaAppGet(string password)
        {
            var vm = new ResultVM();

            if (string.IsNullOrWhiteSpace(password) || password != AppTo.GetValue("Safe:AdminPassword"))
            {
                vm.Set(RCodeTypes.unauthorized);
                vm.Msg = "密码错误或已关闭管理接口";
            }
            else
            {
                var result = AppService.CollSysApp.Query().OrderByDescending(x => x.CreateTime).ToList();

                vm.Data = result;
                vm.Set(RCodeTypes.success);
            }

            return vm;
        }

        /// <summary>
        /// 【管理】清空数据库和上传文件
        /// </summary>
        /// <param name="password">密码，必填</param>
        /// <returns></returns>
        [HttpGet]
        public ResultVM SaResetAllGet(string password)
        {
            var vm = new ResultVM();

            if (string.IsNullOrWhiteSpace(password) || password != AppTo.GetValue("Safe:AdminPassword"))
            {
                vm.Set(RCodeTypes.unauthorized);
                vm.Msg = "密码错误或已关闭管理接口";
            }
            else
            {
                //清空数据库
                AppService.CollSysApp.DeleteAll();
                AppService.CollFileRecord.DeleteAll();

                //删除上传文件
                var rootPath = AppService.StaticVrPathAsPhysicalPath(AppTo.GetValue("StaticResource:RootDir"));
                if (Directory.Exists(rootPath))
                {
                    Directory.Delete(rootPath, true);
                }

                vm.Set(RCodeTypes.success);
            }

            return vm;
        }

        /// <summary>
        /// 【管理】信息
        /// </summary>
        /// <param name="password">密码，必填</param>
        /// <returns></returns>
        [HttpGet]
        public ResultVM SaInfoGet(string password)
        {
            var vm = new ResultVM();

            if (string.IsNullOrWhiteSpace(password) || password != AppTo.GetValue("Safe:AdminPassword"))
            {
                vm.Set(RCodeTypes.unauthorized);
                vm.Msg = "密码错误或已关闭管理接口";
            }
            else
            {
                var ss = new SystemStatusTo();
                ss.RefreshAll().GetAwaiter().GetResult();
                vm.Data = new
                {
                    DatabaseDrive = AppService.LDB.GetType().Assembly.FullName,
                    DatabaseSource = "https://github.com/mbdavid/LiteDB",
                    SystemInfo = ss,
                };
            }

            return vm;
        }

        /// <summary>
        /// 获取Token
        /// </summary>
        /// <param name="appId">分配的应用ID</param>
        /// <param name="appKey">分配的应用密钥</param>
        /// <returns></returns>
        [HttpPost]
        public ResultVM TokenPost([FromForm] long appId, [FromForm] string appKey)
        {
            var vm = new ResultVM();

            if (appId < 1 || string.IsNullOrWhiteSpace(appKey))
            {
                vm.Set(RCodeTypes.failure);
                vm.Msg = "参数缺失";
            }
            else
            {
                var ckey = appKey;
                var gkey = appId.ToString();
                vm = CacheTo.Get<ResultVM>(ckey, gkey);
                if (vm == null)
                {
                    var model = AppService.CollSysApp.FindOne(x => x.AppId == appId && x.AppKey == appKey);
                    if (model != null)
                    {
                        model.AppToken = AppService.NewToken();
                        model.AppTokenExpireTime = DateTime.Now.AddMinutes(AppTo.GetValue<int>("Safe:TokenExpired"));

                        if (AppService.CollSysApp.Update(model))
                        {
                            vm.Data = new
                            {
                                model.AppToken,
                                model.AppTokenExpireTime
                            };
                            vm.Set(RCodeTypes.success);

                            //Token缓存
                            CacheTo.Set(ckey, vm, AppTo.GetValue<int>("Safe:TokenCache"), false, gkey);
                        }
                        else
                        {
                            vm.Set(RCodeTypes.failure);
                        }
                    }
                    else
                    {
                        vm.Set(RCodeTypes.unauthorized);
                    }
                }
            }

            return vm;
        }

        /// <summary>
        /// 创建FixedToken
        /// </summary>
        /// <param name="appId">分配的应用ID</param>
        /// <param name="appKey">分配的应用密钥</param>
        /// <param name="Name">名称</param>
        /// <param name="authMethod">授权接口名，多个用逗号分割，区分大小写</param>
        /// <returns></returns>
        [HttpPost]
        public ResultVM FixedTokenPost([FromForm] long appId, [FromForm] string appKey, [FromForm] string Name, [FromForm] string authMethod)
        {
            var vm = new ResultVM();

            if (appId < 1 || string.IsNullOrWhiteSpace(appKey) || string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(authMethod))
            {
                vm.Set(RCodeTypes.failure);
                vm.Msg = "参数缺失";
            }
            else
            {
                var listAm = AuthMethodGet().Data as List<string>;
                if (!authMethod.Split(',').ToList().All(listAm.Contains))
                {
                    vm.Set(RCodeTypes.failure);
                    vm.Msg = "AuthMethod 存在无效接口名";
                }
                else
                {
                    var sk = AppService.CollSysApp.FindOne(x => x.AppId == appId && x.AppKey == appKey);
                    if (sk != null)
                    {
                        var listmo = new List<FixedTokenJson>();
                        if (!string.IsNullOrWhiteSpace(sk.AppFixedToken))
                        {
                            listmo = sk.AppFixedToken.DeJson<List<FixedTokenJson>>();
                        }

                        var fixToken = AppService.NewToken(true);
                        listmo.Add(new FixedTokenJson()
                        {
                            Name = Name,
                            Token = fixToken,
                            AuthMethod = authMethod,
                            CreateTime = DateTime.Now
                        });
                        sk.AppFixedToken = listmo.ToJson();

                        if (AppService.CollSysApp.Update(sk))
                        {
                            vm.Data = new
                            {
                                Token = fixToken,
                                authMethod
                            };
                            vm.Set(RCodeTypes.success);
                        }
                        else
                        {
                            vm.Set(RCodeTypes.failure);
                        }
                    }
                    else
                    {
                        vm.Set(RCodeTypes.unauthorized);
                    }
                }
            }

            return vm;
        }

        /// <summary>
        /// 删除FixedToken
        /// </summary>
        /// <param name="appId">分配的应用ID</param>
        /// <param name="appKey">分配的应用密钥</param>
        /// <param name="fixedToken">固定 Token</param>
        /// <returns></returns>
        [HttpGet]
        public ResultVM FixedTokenDelete(long appId, string appKey, string fixedToken)
        {
            var vm = new ResultVM();

            if (appId < 1 || string.IsNullOrWhiteSpace(appKey) || string.IsNullOrWhiteSpace(fixedToken))
            {
                vm.Set(RCodeTypes.failure);
                vm.Msg = "参数缺失";
            }
            else
            {
                var model = AppService.CollSysApp.FindOne(x => x.AppId == appId && x.AppKey == appKey);
                if (model != null)
                {
                    var listmo = model.AppFixedToken.DeJson<List<FixedTokenJson>>();
                    if (listmo.Any(x => x.Token == fixedToken))
                    {
                        listmo.Remove(listmo.FirstOrDefault(x => x.Token.Contains(fixedToken)));
                        model.AppFixedToken = listmo.ToJson();

                        vm.Set(AppService.CollSysApp.Update(model));
                    }
                    else
                    {
                        vm.Set(RCodeTypes.failure);
                        vm.Msg = "FixedToken 无效";
                    }
                }
                else
                {
                    vm.Set(RCodeTypes.unauthorized);
                }
            }

            return vm;
        }

        /// <summary>
        /// 授权方法列表（即需要传Token验证的方法名）
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ResultVM AuthMethodGet()
        {
            var vm = new ResultVM();

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

            vm.Set(RCodeTypes.success);
            vm.Data = listA;

            return vm;
        }

        /// <summary>
        /// 上传文件，文件流
        /// </summary>
        /// <param name="files">文件流</param>
        /// <param name="token">token，授权验证，必填</param>
        /// <param name="subdir">子目录，可选</param>
        /// <param name="oname">原名保存，1原名存储，默认生成新ID</param>
        /// <returns></returns>
        [HttpPost]
        public ResultVM UploadPost(IFormFileCollection files, [FromForm] string token, [FromForm] string subdir, [FromForm] int oname = 0)
        {
            var vm = new ResultVM();

            var mn = RouteData.Values["Action"].ToString();

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
                var vt = AppService.ValidToken(token, mn);
                if (vt.Code != 200)
                {
                    vm = vt;
                }
                else
                {
                    var vtjson = vt.Data as FixedTokenJson;
                    var now = DateTime.Now;

                    var listFr = new List<BaseFile>();

                    //虚拟路径
                    var vpath = ParsingTo.Combine(AppTo.GetValue("StaticResource:RootDir"), vtjson.Owner, subdir, now.ToString("yyyy'/'MM"));

                    //物理路径
                    var ppath = AppService.StaticVrPathAsPhysicalPath(vpath);
                    if (!Directory.Exists(ppath))
                    {
                        Directory.CreateDirectory(ppath);
                    }

                    foreach (var file in files)
                    {
                        if (!AppService.IsDisableExtension(file.FileName))
                        {
                            var fr = new BaseFile()
                            {
                                Id = Snowflake53To.Id(),
                                OwnerUser = vtjson.Owner,
                                FileName = Path.GetFileName(file.FileName),
                                FileSize = file.Length,
                                ContentType = file.ContentType,
                                CreateTime = now
                            };

                            var filename = oname == 1 ? fr.FileName : fr.Id + Path.GetExtension(file.FileName);

                            using (var fs = new FileStream(ParsingTo.Combine(ppath, filename), FileMode.CreateNew))
                            {
                                file.CopyTo(fs);
                                fs.Flush();
                            }

                            fr.FilePath = ParsingTo.Combine(vpath, filename);
                            listFr.Add(fr);
                        }
                    }

                    var num = AppService.CollFileRecord.InsertBulk(listFr);
                    vm.Set(num > 0);
                    if (num > 0)
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
        /// <param name="oname">原名保存，1原名存储，默认生成新ID</param>
        /// <returns></returns>
        [HttpPost]
        public ResultVM UploadChunkPost(IFormFile file, [FromForm] string token, [FromForm] string subdir, [FromForm] string ts, [FromForm] int chunk, [FromForm] int chunks, [FromForm] int oname = 0)
        {
            var vm = new ResultVM();

            var mn = RouteData.Values["Action"].ToString();

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
            else if (file.Length * (chunks - 1) > AppTo.GetValue<int>("StaticResource:MaxSize") * 1024 * 1024)
            {
                vm.Msg = "文件大小超出限制";
            }
            else if (AppService.IsDisableExtension(file.FileName))
            {
                vm.Msg = "File extension not supported";
            }
            else
            {
                var vtkey = $"Upload-Token-{token}";
                var vt = CacheTo.Get<ResultVM>(vtkey);
                if (vt == null)
                {
                    vt = AppService.ValidToken(token, mn);
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
                        var chunkDir = ParsingTo.Combine(AppTo.GetValue("StaticResource:TmpDir"), ts);

                        var ext = Path.GetExtension(file.FileName);
                        //存入分片临时目录（格式：Id_片索引.文件格式后缀）
                        var chunkName = $"{ts}_{chunk}.{ext}";

                        //保存分片物理路径
                        var chunkPPath = AppService.StaticVrPathAsPhysicalPath(chunkDir);
                        if (!Directory.Exists(chunkPPath))
                        {
                            Directory.CreateDirectory(chunkPPath);
                        }

                        using (var fs = new FileStream(ParsingTo.Combine(chunkPPath, chunkName), FileMode.CreateNew))
                        {
                            file.CopyTo(fs);
                            fs.Flush();
                        }

                        //记录已上传的分片总数
                        var ckkey = $"Upload-Chunk-{ts}";
                        var ci = CacheTo.Get<int>(ckkey);
                        ci++;
                        CacheTo.Set(ckkey, ci);

                        //所有片已上传完，合并片
                        if (ci == chunks)
                        {
                            var now = DateTime.Now;

                            var vtjson = vt.Data as FixedTokenJson;

                            //虚拟路径
                            var vpath = ParsingTo.Combine(AppTo.GetValue("StaticResource:RootDir"), vtjson.Owner, subdir, now.ToString("yyyy'/'MM"));

                            //物理路径
                            var ppath = AppService.StaticVrPathAsPhysicalPath(vpath);
                            if (!Directory.Exists(ppath))
                            {
                                Directory.CreateDirectory(ppath);
                            }

                            var fr = new BaseFile()
                            {
                                Id = Snowflake53To.Id(),
                                OwnerUser = vtjson.Owner,
                                FileName = Path.GetFileName(file.FileName),
                                FileSize = file.Length,
                                ContentType = file.ContentType,
                                CreateTime = now
                            };
                            var filename = oname == 1 ? fr.FileName : fr.Id + Path.GetExtension(file.FileName);

                            using var fs = new FileStream(ParsingTo.Combine(ppath, filename), FileMode.Create);
                            //排序从 0-N Write
                            Directory.EnumerateFiles(chunkPPath).OrderBy(x => x.Length).ThenBy(x => x).ForEach(part =>
                            {
                                var bytes = System.IO.File.ReadAllBytes(part);
                                fs.Write(bytes, 0, bytes.Length);
                                bytes = null;
                            });
                            //删除分块文件夹
                            Directory.Delete(chunkPPath, true);

                            fr.FilePath = ParsingTo.Combine(vpath, filename);

                            AppService.CollFileRecord.Insert(fr);

                            vm.Data = fr;
                            vm.Set(RCodeTypes.success);
                        }
                        else
                        {
                            vm.Code = 201;
                            vm.Data = ParsingTo.Combine(chunkDir, chunkName);
                            vm.Msg = "chunk success";
                        }
                    }
                }
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
        [HttpPost]
        public ResultVM UploadCoverPost(IFormFile file, [FromForm] string token, [FromForm] string path)
        {
            var vm = new ResultVM();

            var mn = RouteData.Values["Action"].ToString();

            if (file == null)
            {
                vm.Msg = "没找到上传的文件";
            }
            else if (AppService.IsDisableExtension(file.FileName))
            {
                vm.Set(RCodeTypes.refuse);
                vm.Msg = "File extension not supported";
            }
            else
            {
                //验证token
                var vt = AppService.ValidToken(token, mn);
                if (vt.Code != 200)
                {
                    vm = vt;
                }
                else
                {
                    var fixedTokenJson = vt.Data as FixedTokenJson;

                    _ = long.TryParse(fixedTokenJson.Owner, out long id);
                    var model = AppService.CollFileRecord.FindOne(x => x.Id == id || x.FilePath == path);
                    if (model == null)
                    {
                        vm.Set(RCodeTypes.failure);
                    }
                    else if (model.OwnerUser != fixedTokenJson.Owner)
                    {
                        vm.Set(RCodeTypes.unauthorized);
                    }
                    else
                    {
                        var ppath = AppService.StaticVrPathAsPhysicalPath(model.FilePath);

                        if (System.IO.File.Exists(ppath))
                        {
                            System.IO.File.Delete(ppath);

                            using (var fs = new FileStream(ppath, FileMode.CreateNew))
                            {
                                file.CopyTo(fs);
                                fs.Flush();
                            }

                            //更新信息
                            model.FileName = file.Name;
                            model.FileSize = file.Length;
                            model.ContentType = file.ContentType;
                            model.CreateTime = DateTime.Now;

                            vm.Set(AppService.CollFileRecord.Update(model));
                        }
                        else
                        {
                            vm.Set(RCodeTypes.failure);
                            vm.Msg = "文件路径无效";
                        }
                    }
                }
            }

            return vm;
        }

        /// <summary>
        /// 查看文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        [ResponseCache(Duration = 600)]
        public IActionResult FileViewGet(string path, string token)
        {
            var vm = new ResultVM();

            var mn = RouteData.Values["Action"].ToString();

            bool isAuth;
            //公开访问
            if (AppTo.GetValue<bool>("Safe:PublicAccess"))
            {
                isAuth = true;
            }
            else
            {
                isAuth = AppService.ValidToken(token, mn).Code == 200;
            }

            if (isAuth)
            {
                var ppath = AppService.StaticVrPathAsPhysicalPath(path);
                if (System.IO.File.Exists(ppath))
                {
                    new FileExtensionContentTypeProvider().TryGetContentType(path, out string contentType);
                    return PhysicalFile(ppath, contentType ?? "application/octet-stream");
                }
                else
                {
                    vm.Set(RCodeTypes.failure);
                }
            }
            else
            {
                vm.Set(RCodeTypes.unauthorized);
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
        public ResultVM FileCopyGet(string token, string path)
        {
            var vm = new ResultVM();

            var mn = RouteData.Values["Action"].ToString();

            //验证token
            var vt = AppService.ValidToken(token, mn);
            if (vt.Code != 200)
            {
                vm = vt;
            }
            else
            {
                var fixedTokenJson = vt.Data as FixedTokenJson;

                _ = long.TryParse(fixedTokenJson.Owner, out long id);
                var model = AppService.CollFileRecord.FindOne(x => x.Id == id || x.FilePath == path);
                if (model == null)
                {
                    vm.Set(RCodeTypes.failure);
                }
                else if (model.OwnerUser != fixedTokenJson.Owner)
                {
                    vm.Set(RCodeTypes.unauthorized);
                }
                else
                {
                    //要复制的文件虚拟路径
                    var vpath = model.FilePath;

                    var newModel = new BaseFile()
                    {
                        Id = Snowflake53To.Id(),
                        OwnerUser = model.OwnerUser,
                        FileName = model.FileName,
                        FileSize = model.FileSize,
                        ContentType = model.ContentType,
                        CreateTime = DateTime.Now
                    };

                    //要复制的物理路径
                    var ppath = AppService.StaticVrPathAsPhysicalPath(vpath);
                    newModel.FilePath = vpath.Replace(Path.GetFileNameWithoutExtension(vpath), newModel.Id.ToString());
                    //复制的新物理路径
                    var newppath = AppService.StaticVrPathAsPhysicalPath(newModel.FilePath);

                    System.IO.File.Copy(ppath, newppath);

                    AppService.CollFileRecord.Insert(newModel);
                    vm.Data = newModel;
                    vm.Set(RCodeTypes.success);
                }
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
        public ResultVM FileDelete(string token, string path)
        {
            var mn = RouteData.Values["Action"].ToString();

            //验证token
            var vm = AppService.ValidToken(token, mn);
            if (vm.Code == 200)
            {
                var fixedTokenJson = vm.Data as FixedTokenJson;

                _ = long.TryParse(path, out long id);
                var model = AppService.CollFileRecord.FindOne(x => x.Id == id || x.FilePath == path);
                if (model == null)
                {
                    vm.Set(RCodeTypes.failure);
                }
                else if (model.OwnerUser != fixedTokenJson.Owner)
                {
                    vm.Set(RCodeTypes.unauthorized);
                }
                else
                {
                    var fp = Path.Combine(AppTo.WebRootPath, model.FilePath);
                    if (System.IO.File.Exists(fp))
                    {
                        System.IO.File.Delete(fp);
                    }

                    vm.Set(AppService.CollFileRecord.Delete(model.Id));
                }
            }

            return vm;
        }
    }
}
