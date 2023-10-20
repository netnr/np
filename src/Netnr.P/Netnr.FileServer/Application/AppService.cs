using LiteDB;
using System.Threading.Tasks;

namespace Netnr.FileServer.Application
{
    /// <summary>
    /// App
    /// </summary>
    public class AppService
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        public static string LiteDBConnectionString { get; set; } = AppTo.Configuration.GetConnectionString("LiteDB").Replace("~", AppTo.ContentRootPath);

        public static LiteDatabase LDB { get; set; } = new LiteDatabase(LiteDBConnectionString);

        public static ILiteCollection<BaseApp> CollSysApp { get; set; } = LDB.GetCollection<BaseApp>(nameof(BaseApp).ToLower());

        public static ILiteCollection<BaseFile> CollFileRecord { get; set; } = LDB.GetCollection<BaseFile>(nameof(BaseFile).ToLower());

        /// <summary>
        /// 验证Token
        /// </summary>
        /// <param name="token"></param>
        /// <param name="methodName">方法名</param>
        /// <returns></returns>
        public static ResultVM ValidToken(string token, string methodName)
        {
            var vm = new ResultVM();

            try
            {
                if (string.IsNullOrWhiteSpace(token))
                {
                    vm.Set(RCodeTypes.unauthorized);
                }
                else
                {
                    //FixedToken
                    if (token.StartsWith(AppTo.GetValue("Safe:FixedTokenPrefix")))
                    {
                        var model = CollSysApp.FindOne(x => x.AppFixedToken.Contains(token));

                        if (model != null)
                        {
                            var fixedTokenJson = model.AppFixedToken.DeJson<List<FixedTokenJson>>().FirstOrDefault(x => x.Token == token);
                            if (fixedTokenJson.AuthMethod.Split(',').Contains(methodName))
                            {
                                vm.Set(RCodeTypes.success);

                                fixedTokenJson.Owner = model.AppOwner;
                                vm.Data = fixedTokenJson;
                            }
                            else
                            {
                                vm.Set(RCodeTypes.unauthorized);
                                vm.Msg = "FixedToken 无权访问该接口";
                            }
                        }
                        else
                        {
                            vm.Set(RCodeTypes.unauthorized);
                            vm.Msg = "token 无效或已过期";
                        }
                    }
                    else
                    {
                        var model = CollSysApp.FindOne(x => x.AppToken == token);

                        if (model?.AppTokenExpireTime > DateTime.Now)
                        {
                            vm.Set(RCodeTypes.success);

                            vm.Data = new FixedTokenJson()
                            {
                                Owner = model.AppOwner
                            };
                        }
                        else
                        {
                            vm.Set(RCodeTypes.unauthorized);
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
        /// 创建 Token
        /// </summary>
        /// <param name="isFixedToken">是固定Token，默认否</param>
        /// <returns></returns>
        public static string NewToken(bool isFixedToken = false)
        {
            return (isFixedToken ? AppTo.GetValue("Safe:FixedTokenPrefix") : "") + RandomTo.NewString(64);
        }

        /// <summary>
        /// 静态资源虚拟路径转为物理路径
        /// </summary>
        /// <param name="vpath">虚拟路径</param>
        /// <returns></returns>
        public static string StaticVrPathAsPhysicalPath(string vpath)
        {
            return ParsingTo.Combine(AppTo.GetValue<bool>("Safe:PublicAccess") ? AppTo.WebRootPath : AppTo.ContentRootPath, vpath);
        }

        /// <summary>
        /// 禁用扩展名
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static bool IsDisableExtension(string filename)
        {
            var risk = false;
            if (filename.EndsWith("."))
            {
                risk = true;
            }
            else
            {
                var disableExtension = AppTo.GetValue("StaticResource:DisableExtension");
                var ext = Path.GetExtension(filename).ToLower();
                if (ext.Length > 1 && !string.IsNullOrWhiteSpace(disableExtension) && disableExtension.ToLower().Split(' ').Contains(ext))
                {
                    risk = true;
                }
            }

            return risk;
        }

        /// <summary>
        /// 清理临时目录
        /// </summary>
        /// <param name="keepTime">保留时间，单位：分钟</param>
        /// <returns></returns>
        public static ResultVM ClearTmp(int keepTime)
        {
            var vm = new ResultVM();

            try
            {
                var listDel = new List<string>();

                //删除临时文件
                var tmpdir = StaticVrPathAsPhysicalPath(AppTo.GetValue("StaticResource:TmpDir"));
                if (Directory.Exists(tmpdir))
                {
                    if (keepTime > 0)
                    {
                        var now = DateTime.Now;
                        Directory.EnumerateFiles(tmpdir).ForEach(item =>
                        {
                            var fi = new FileInfo(item);
                            if ((now - fi.CreationTime).TotalMinutes > keepTime)
                            {
                                fi.Delete();
                                listDel.Add(item);
                            }
                        });

                        Directory.EnumerateDirectories(tmpdir).ForEach(item =>
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

                vm.Set(RCodeTypes.success);
                vm.Data = listDel;
            }
            catch (Exception ex)
            {
                vm.Set(ex);
                ConsoleTo.LogError(ex);
            }

            return vm;
        }
    }
}