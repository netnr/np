using SQLite;
using System;
using Netnr.FileServer.Model;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Microsoft.VisualBasic.CompilerServices;
using System.ComponentModel;

namespace Netnr.FileServer.Application
{
    /// <summary>
    /// 数据库操作
    /// </summary>
    public class FileServerService
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        public static string SQLiteConn = GlobalTo.Configuration.GetConnectionString("SQLite").Replace("~", GlobalTo.ContentRootPath);

        /// <summary>
        /// 创建App
        /// </summary>
        /// <param name="owner">用户，唯一，文件夹名</param>
        /// <returns></returns>
        public static ActionResultVM CreateApp(string owner)
        {
            var vm = new ActionResultVM();

            try
            {
                using var db = new SQLiteConnection(SQLiteConn);

                var mo = new SysApp()
                {
                    AppId = Core.UniqueTo.LongId().ToString(),
                    AppKey = Core.UniqueTo.LongId().ToString() + Core.UniqueTo.LongId().ToString(),
                    CreateTime = DateTime.Now,
                    Owner = owner,
                    Token = NewToken(),
                    TokenExpireTime = DateTime.Now.AddMinutes(GlobalTo.GetValue<int>("Safe:TokenExpired")),
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
        public static ActionResultVM GetAppList(int pageNumber = 1, int pageSize = 20)
        {
            var vm = new ActionResultVM();

            try
            {
                using var db = new SQLiteConnection(SQLiteConn);
                var sk = db.Table<SysApp>().OrderByDescending(x => x.CreateTime).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

                vm.Data = sk;
                vm.Set(ARTag.success);
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
        public static ActionResultVM GetAppInfo(string AppId, string AppKey)
        {
            var vm = new ActionResultVM();

            try
            {
                using var db = new SQLiteConnection(SQLiteConn);
                var sk = db.Table<SysApp>().FirstOrDefault(x => x.AppId == AppId && x.AppKey == AppKey);

                if (sk == null)
                {
                    vm.Set(ARTag.invalid);
                }
                else
                {
                    vm.Data = sk;
                    vm.Set(ARTag.success);
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
        public static ActionResultVM GetToken(string AppId, string AppKey)
        {
            var vm = new ActionResultVM();

            try
            {
                using var db = new SQLiteConnection(SQLiteConn);
                var sk = db.Table<SysApp>().FirstOrDefault(x => x.AppId == AppId && x.AppKey == AppKey);
                if (sk != null)
                {
                    sk.Token = NewToken();
                    sk.TokenExpireTime = DateTime.Now.AddMinutes(GlobalTo.GetValue<int>("Safe:TokenExpired"));

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
                    vm.Set(ARTag.unauthorized);
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        /// <summary>
        /// 创建FixToken
        /// </summary>
        /// <param name="AppId">分配的应用ID</param>
        /// <param name="AppKey">分配的应用密钥</param>
        /// <param name="Name">名称</param>
        /// <param name="AuthMethod">授权接口</param>
        /// <returns></returns>
        public static ActionResultVM CreateFixToken(string AppId, string AppKey, string Name, string AuthMethod)
        {
            var vm = new ActionResultVM();

            try
            {
                using var db = new SQLiteConnection(SQLiteConn);
                var sk = db.Table<SysApp>().FirstOrDefault(x => x.AppId == AppId && x.AppKey == AppKey);
                if (sk != null)
                {
                    var listmo = new List<FixTokenJson>();
                    if (!string.IsNullOrWhiteSpace(sk.FixToken))
                    {
                        listmo = sk.FixToken.ToEntitys<FixTokenJson>();
                    }

                    var fixToken = NewToken(true);
                    listmo.Add(new FixTokenJson()
                    {
                        Name = Name,
                        Token = fixToken,
                        AuthMethod = AuthMethod,
                        CreateTime = DateTime.Now
                    });
                    sk.FixToken = listmo.ToJson();

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
                    vm.Set(ARTag.unauthorized);
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        /// <summary>
        /// 删除FixToken
        /// </summary>
        /// <param name="AppId">分配的应用ID</param>
        /// <param name="AppKey">分配的应用密钥</param>
        /// <param name="FixToken">固定Token</param>
        /// <returns></returns>
        public static ActionResultVM DelFixToken(string AppId, string AppKey, string FixToken)
        {
            var vm = new ActionResultVM();

            try
            {
                using var db = new SQLiteConnection(SQLiteConn);
                var sk = db.Table<SysApp>().FirstOrDefault(x => x.AppId == AppId && x.AppKey == AppKey);
                if (sk != null)
                {
                    var listmo = sk.FixToken.ToEntitys<FixTokenJson>();
                    if (listmo.Any(x => x.Token == FixToken))
                    {
                        listmo.Remove(listmo.FirstOrDefault(x => x.Token.Contains(FixToken)));
                        sk.FixToken = listmo.ToJson();
                        int num = db.Update(sk);
                        vm.Set(num > 0);
                    }
                    else
                    {
                        vm.Set(ARTag.invalid);
                        vm.Msg = "FixToken 无效";
                    }
                }
                else
                {
                    vm.Set(ARTag.unauthorized);
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
        /// <param name="IsFixToken">是固定Token，默认否</param>
        /// <returns></returns>
        public static string NewToken(bool IsFixToken = false)
        {
            return (IsFixToken ? GlobalTo.GetValue("Safe:FixTokenPrefix") : "") + (Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N")).ToUpper();
        }

        /// <summary>
        /// 验证Token
        /// </summary>
        /// <param name="token"></param>
        /// <param name="MethodName">方法名</param>
        /// <returns></returns>
        public static ActionResultVM ValidToken(string token, string MethodName)
        {
            var vm = new ActionResultVM();

            try
            {
                if (string.IsNullOrWhiteSpace(token))
                {
                    vm.Set(ARTag.unauthorized);
                }
                else
                {
                    using var db = new SQLiteConnection(SQLiteConn);

                    //FixToken
                    if (token.StartsWith(GlobalTo.GetValue("Safe:FixTokenPrefix")))
                    {
                        var sk = db.Table<SysApp>().FirstOrDefault(x => x.FixToken.Contains(token));

                        if (sk != null)
                        {
                            var mo = sk.FixToken.ToEntitys<FixTokenJson>().FirstOrDefault(x => x.Token == token);
                            if (mo.AuthMethod.Split(',').Contains(MethodName))
                            {
                                vm.Set(ARTag.success);

                                mo.Owner = sk.Owner;
                                vm.Data = mo;
                            }
                            else
                            {
                                vm.Set(ARTag.unauthorized);
                                vm.Msg = "FixToken 无权访问该接口";
                            }
                        }
                        else
                        {
                            vm.Set(ARTag.unauthorized);
                            vm.Msg = "token 无效或已过期";
                        }
                    }
                    else
                    {
                        var sk = db.Table<SysApp>().FirstOrDefault(x => x.Token == token);

                        if (sk?.TokenExpireTime > DateTime.Now)
                        {
                            vm.Set(ARTag.success);

                            vm.Data = new FixTokenJson()
                            {
                                Owner = sk.Owner
                            };
                        }
                        else
                        {
                            vm.Set(ARTag.unauthorized);
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
        /// 验证FixToken
        /// </summary>
        /// <param name="fixToken"></param>
        /// <returns></returns>
        public static ActionResultVM ValidFixToken(string fixToken)
        {
            var vm = new ActionResultVM();

            try
            {
                if (string.IsNullOrWhiteSpace(fixToken))
                {
                    vm.Set(ARTag.unauthorized);
                }
                else
                {
                    using var db = new SQLiteConnection(SQLiteConn);
                    var sk = db.Table<SysApp>().FirstOrDefault(x => x.FixToken.Contains(fixToken));

                    if (sk != null)
                    {
                        vm.Set(ARTag.success);

                        var mo = sk.FixToken.ToEntitys<FixTokenJson>().FirstOrDefault(x => x.Token == fixToken);
                        mo.Owner = sk.Owner;
                        vm.Data = mo;
                    }
                    else
                    {
                        vm.Set(ARTag.unauthorized);
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
        public static ActionResultVM InsertFile(FileRecord model)
        {
            var vm = new ActionResultVM();

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
        public static ActionResultVM InsertFile(List<FileRecord> list)
        {
            var vm = new ActionResultVM();

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
        public static ActionResultVM QueryFile(string owner, string IdOrPath)
        {
            var vm = new ActionResultVM();

            try
            {
                using var db = new SQLiteConnection(SQLiteConn);

                var fr = db.Table<FileRecord>().FirstOrDefault(x => x.Id == IdOrPath || x.Path == IdOrPath);
                if (fr == null)
                {
                    vm.Set(ARTag.lack);
                }
                else if (fr.OwnerUser != owner)
                {
                    vm.Set(ARTag.unauthorized);
                }
                else
                {
                    vm.Set(ARTag.success);
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
        public static ActionResultVM UpdateFile(FileRecord model)
        {
            var vm = new ActionResultVM();

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
        public static ActionResultVM DeleteFile(string owner, string IdOrPath)
        {
            var vm = new ActionResultVM();

            try
            {
                using var db = new SQLiteConnection(SQLiteConn);

                var fr = db.Table<FileRecord>().FirstOrDefault(x => x.Id == IdOrPath || x.Path == IdOrPath);
                if (fr == null)
                {
                    vm.Set(ARTag.lack);
                }
                else if (fr.OwnerUser != owner)
                {
                    vm.Set(ARTag.unauthorized);
                }
                else
                {
                    var fp = Fast.PathTo.Combine(GlobalTo.WebRootPath, fr.Path);
                    if (System.IO.File.Exists(fp))
                    {
                        System.IO.File.Delete(fp);
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
    }
}