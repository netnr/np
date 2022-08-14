using SQLite;
using Netnr.FileServer.Domain;

namespace Netnr.FileServer.Application
{
    /// <summary>
    /// 数据库操作及辅助方法
    /// </summary>
    public class FileServerService
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        public static string SQLiteConn { get; set; } = AppTo.Configuration.GetConnectionString("SQLite").Replace("~", AppTo.ContentRootPath);

        /// <summary>
        /// 创建App
        /// </summary>
        /// <param name="owner">用户，唯一，文件夹名</param>
        /// <returns></returns>
        public static ResultVM CreateApp(string owner)
        {
            var vm = new ResultVM();

            try
            {
                using var db = new SQLiteConnection(SQLiteConn);

                var mo = new SysApp()
                {
                    AppId = UniqueTo.LongId().ToString(),
                    AppKey = UniqueTo.LongId().ToString() + UniqueTo.LongId().ToString(),
                    CreateTime = DateTime.Now,
                    Owner = owner,
                    Token = NewToken(),
                    TokenExpireTime = DateTime.Now.AddMinutes(AppTo.GetValue<int>("Safe:TokenExpired")),
                    Remark = "通过接口创建"
                };

                int num = db.Insert(mo);

                vm.Set(num > 0);
                vm.Data = mo;
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        /// <summary>
        /// 获取App列表
        /// </summary>
        /// <param name="pageNumber">页码，默认1</param>
        /// <param name="pageSize">页量，默认20</param>
        /// <returns></returns>
        public static ResultVM GetAppList(int pageNumber = 1, int pageSize = 20)
        {
            var vm = new ResultVM();

            try
            {
                using var db = new SQLiteConnection(SQLiteConn);
                var sk = db.Table<SysApp>().OrderByDescending(x => x.CreateTime).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

                vm.Data = sk;

                vm.Set(EnumTo.RTag.success);
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        /// <summary>
        /// 获取App信息
        /// </summary>
        /// <param name="AppId">分配的应用ID</param>
        /// <param name="AppKey">分配的应用密钥</param>
        /// <returns></returns>
        public static ResultVM GetAppInfo(string AppId, string AppKey)
        {
            var vm = new ResultVM();

            try
            {
                using var db = new SQLiteConnection(SQLiteConn);
                var sk = db.Table<SysApp>().FirstOrDefault(x => x.AppId == AppId && x.AppKey == AppKey);

                if (sk == null)
                {
                    vm.Set(EnumTo.RTag.invalid);
                }
                else
                {
                    vm.Data = sk;
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
        /// 获取Token
        /// </summary>
        /// <param name="AppId">分配的应用ID</param>
        /// <param name="AppKey">分配的应用密钥</param>
        /// <returns></returns>
        public static ResultVM GetToken(string AppId, string AppKey)
        {
            var vm = new ResultVM();

            try
            {
                using var db = new SQLiteConnection(SQLiteConn);
                var sk = db.Table<SysApp>().FirstOrDefault(x => x.AppId == AppId && x.AppKey == AppKey);
                if (sk != null)
                {
                    sk.Token = NewToken();
                    sk.TokenExpireTime = DateTime.Now.AddMinutes(AppTo.GetValue<int>("Safe:TokenExpired"));

                    int num = db.Update(sk);
                    vm.Data = new
                    {
                        sk.Token,
                        sk.TokenExpireTime
                    };
                    vm.Set(num > 0);
                }
                else
                {
                    vm.Set(EnumTo.RTag.unauthorized);
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        /// <summary>
        /// 创建固定 Token
        /// </summary>
        /// <param name="AppId">分配的应用ID</param>
        /// <param name="AppKey">分配的应用密钥</param>
        /// <param name="Name">名称</param>
        /// <param name="AuthMethod">授权接口</param>
        /// <returns></returns>
        public static ResultVM CreateFixedToken(string AppId, string AppKey, string Name, string AuthMethod)
        {
            var vm = new ResultVM();

            try
            {
                using var db = new SQLiteConnection(SQLiteConn);
                var sk = db.Table<SysApp>().FirstOrDefault(x => x.AppId == AppId && x.AppKey == AppKey);
                if (sk != null)
                {
                    var listmo = new List<FixedTokenJson>();
                    if (!string.IsNullOrWhiteSpace(sk.FixedToken))
                    {
                        listmo = sk.FixedToken.DeJson<List<FixedTokenJson>>();
                    }

                    var fixToken = NewToken(true);
                    listmo.Add(new FixedTokenJson()
                    {
                        Name = Name,
                        Token = fixToken,
                        AuthMethod = AuthMethod,
                        CreateTime = DateTime.Now
                    });
                    sk.FixedToken = listmo.ToJson();

                    int num = db.Update(sk);
                    vm.Data = new
                    {
                        Token = fixToken,
                        AuthMethod
                    };
                    vm.Set(num > 0);
                }
                else
                {
                    vm.Set(EnumTo.RTag.unauthorized);
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        /// <summary>
        /// 删除固定Token
        /// </summary>
        /// <param name="AppId">分配的应用ID</param>
        /// <param name="AppKey">分配的应用密钥</param>
        /// <param name="FixedToken">固定Token</param>
        /// <returns></returns>
        public static ResultVM DelFixedToken(string AppId, string AppKey, string FixedToken)
        {
            var vm = new ResultVM();

            try
            {
                using var db = new SQLiteConnection(SQLiteConn);
                var sk = db.Table<SysApp>().FirstOrDefault(x => x.AppId == AppId && x.AppKey == AppKey);
                if (sk != null)
                {
                    var listmo = sk.FixedToken.DeJson<List<FixedTokenJson>>();
                    if (listmo.Any(x => x.Token == FixedToken))
                    {
                        listmo.Remove(listmo.FirstOrDefault(x => x.Token.Contains(FixedToken)));
                        sk.FixedToken = listmo.ToJson();
                        int num = db.Update(sk);
                        vm.Set(num > 0);
                    }
                    else
                    {
                        vm.Set(EnumTo.RTag.invalid);
                        vm.Msg = "FixedToken 无效";
                    }
                }
                else
                {
                    vm.Set(EnumTo.RTag.unauthorized);
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        /// <summary>
        /// 验证Token
        /// </summary>
        /// <param name="token"></param>
        /// <param name="MethodName">方法名</param>
        /// <returns></returns>
        public static ResultVM ValidToken(string token, string MethodName)
        {
            var vm = new ResultVM();

            try
            {
                if (string.IsNullOrWhiteSpace(token))
                {
                    vm.Set(EnumTo.RTag.unauthorized);
                }
                else
                {
                    using var db = new SQLiteConnection(SQLiteConn);

                    //FixedToken
                    if (token.StartsWith(AppTo.GetValue("Safe:FixedTokenPrefix")))
                    {
                        var sk = db.Table<SysApp>().FirstOrDefault(x => x.FixedToken.Contains(token));

                        if (sk != null)
                        {
                            var mo = sk.FixedToken.DeJson<List<FixedTokenJson>>().FirstOrDefault(x => x.Token == token);
                            if (mo.AuthMethod.Split(',').Contains(MethodName))
                            {
                                vm.Set(EnumTo.RTag.success);

                                mo.Owner = sk.Owner;
                                vm.Data = mo;
                            }
                            else
                            {
                                vm.Set(EnumTo.RTag.unauthorized);
                                vm.Msg = "FixedToken 无权访问该接口";
                            }
                        }
                        else
                        {
                            vm.Set(EnumTo.RTag.unauthorized);
                            vm.Msg = "token 无效或已过期";
                        }
                    }
                    else
                    {
                        var sk = db.Table<SysApp>().FirstOrDefault(x => x.Token == token);

                        if (sk?.TokenExpireTime > DateTime.Now)
                        {
                            vm.Set(EnumTo.RTag.success);

                            vm.Data = new FixedTokenJson()
                            {
                                Owner = sk.Owner
                            };
                        }
                        else
                        {
                            vm.Set(EnumTo.RTag.unauthorized);
                            vm.Msg = "token 无效或已过期";
                        }
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
        /// 验证FixedToken
        /// </summary>
        /// <param name="fixedToken"></param>
        /// <returns></returns>
        public static ResultVM ValidFixedToken(string fixedToken)
        {
            var vm = new ResultVM();

            try
            {
                if (string.IsNullOrWhiteSpace(fixedToken))
                {
                    vm.Set(EnumTo.RTag.unauthorized);
                }
                else
                {
                    using var db = new SQLiteConnection(SQLiteConn);
                    var sk = db.Table<SysApp>().FirstOrDefault(x => x.FixedToken.Contains(fixedToken));

                    if (sk != null)
                    {
                        vm.Set(EnumTo.RTag.success);

                        var mo = sk.FixedToken.DeJson<List<FixedTokenJson>>().FirstOrDefault(x => x.Token == fixedToken);
                        mo.Owner = sk.Owner;
                        vm.Data = mo;
                    }
                    else
                    {
                        vm.Set(EnumTo.RTag.unauthorized);
                        vm.Msg = "token 无效或已过期";
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
        /// 新增
        /// </summary>
        /// <param name="model"></param>
        public static ResultVM InsertFile(FileRecord model)
        {
            var vm = new ResultVM();

            try
            {
                using var db = new SQLiteConnection(SQLiteConn);
                int num = db.Insert(model);

                vm.Set(num > 0);
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="list"></param>
        public static ResultVM InsertFile(List<FileRecord> list)
        {
            var vm = new ResultVM();

            try
            {
                using var db = new SQLiteConnection(SQLiteConn);
                int num = db.InsertAll(list);

                vm.Set(num > 0);
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="owner">所属</param>
        /// <param name="IdOrPath">文件ID或路径</param>
        public static ResultVM QueryFile(string owner, string IdOrPath)
        {
            var vm = new ResultVM();

            try
            {
                using var db = new SQLiteConnection(SQLiteConn);

                var fr = db.Table<FileRecord>().FirstOrDefault(x => x.Id == IdOrPath || x.Path == IdOrPath);
                if (fr == null)
                {
                    vm.Set(EnumTo.RTag.lack);
                }
                else if (fr.OwnerUser != owner)
                {
                    vm.Set(EnumTo.RTag.unauthorized);
                }
                else
                {
                    vm.Set(EnumTo.RTag.success);
                    vm.Data = fr;
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="model">实体</param>
        public static ResultVM UpdateFile(FileRecord model)
        {
            var vm = new ResultVM();

            try
            {
                using var db = new SQLiteConnection(SQLiteConn);

                int num = db.Update(model);
                vm.Set(num > 0);
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="owner">所属</param>
        /// <param name="IdOrPath">文件ID或路径</param>
        public static ResultVM DeleteFile(string owner, string IdOrPath)
        {
            var vm = new ResultVM();

            try
            {
                using var db = new SQLiteConnection(SQLiteConn);

                var fr = db.Table<FileRecord>().FirstOrDefault(x => x.Id == IdOrPath || x.Path == IdOrPath);
                if (fr == null)
                {
                    vm.Set(EnumTo.RTag.lack);
                }
                else if (fr.OwnerUser != owner)
                {
                    vm.Set(EnumTo.RTag.unauthorized);
                }
                else
                {
                    var fp = Path.Combine(AppTo.WebRootPath, fr.Path);
                    if (File.Exists(fp))
                    {
                        File.Delete(fp);
                    }

                    int num = db.Delete(fr);

                    vm.Set(num > 0);
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        /// <summary>
        /// 创建 Token
        /// </summary>
        /// <param name="IsFixedToken">是固定Token，默认否</param>
        /// <returns></returns>
        public static string NewToken(bool IsFixedToken = false)
        {
            return (IsFixedToken ? AppTo.GetValue("Safe:FixedTokenPrefix") : "") + (Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N")).ToUpper();
        }

        /// <summary>
        /// 静态资源虚拟路径转为物理路径
        /// </summary>
        /// <param name="VrDir">虚拟路径</param>
        /// <returns></returns>
        public static string StaticVrPathAsPhysicalPath(string VrDir)
        {
            return PathTo.Combine(AppTo.GetValue<bool>("Safe:PublicAccess") ? AppTo.WebRootPath : AppTo.ContentRootPath, VrDir);
        }

    }
}