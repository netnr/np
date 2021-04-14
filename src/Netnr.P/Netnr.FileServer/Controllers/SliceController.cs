using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Netnr.SharedFast;
using System;
using System.IO;
using System.Linq;

namespace Netnr.FileServer.Controllers
{
    /// <summary>
    /// 切片
    /// </summary>
    [Route("[controller]/[action]")]
    [Apps.FilterConfigs.AllowCors]
    public class SliceController : Controller
    {
        /// <summary>
        /// 配置
        /// </summary>
        public class Config
        {
            /// <summary>
            /// 根目录
            /// </summary>
            public const string RootDir = "/static";

            /// <summary>
            /// 转为物理路径
            /// </summary>
            /// <param name="path"></param>
            /// <returns></returns>
            public static string AsPhysicalPath(string path)
            {
                return Core.PathTo.Combine(GlobalTo.WebRootPath, path);
            }
        }

        /// <summary>
        /// 分片上传
        /// </summary>
        /// <param name="chunk"></param>
        /// <param name="chunks"></param>
        /// <param name="sole"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public SharedResultVM Upload([FromForm] int chunk, [FromForm] int chunks, [FromForm] string sole, [FromForm] FormFile file)
        {
            var vm = new SharedResultVM();
            try
            {
                if (file == null)
                {
                    vm.Set(SharedEnum.RTag.lack);
                    vm.Msg = "未获取到上传文件对象";
                }
                else
                {
                    var partDirV = $"{Config.RootDir}/part";
                    var partDirP = Config.AsPhysicalPath(partDirV);
                    var uploadDirV = $"{Config.RootDir}/upload";
                    var uploadDirP = Config.AsPhysicalPath(uploadDirV);

                    //分片上传
                    if (chunks > 1)
                    {
                        if (!Directory.Exists(partDirP))
                        {
                            Directory.CreateDirectory(partDirP);
                        }

                        if (!Directory.Exists(uploadDirP))
                        {
                            Directory.CreateDirectory(uploadDirP);
                        }

                        //存入分片临时目录（格式：Id_片索引.文件格式后缀）
                        var ext = Path.GetExtension(file.FileName);
                        var partName = $"{sole}_{chunk}{ext}";

                        using (Stream fileStream = new FileStream(partDirP + partName, FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        var ckey = $"upload-{sole}";
                        var ci = Core.CacheTo.Get(ckey) as int? ?? 0;

                        ci++;
                        Core.CacheTo.Set(ckey, ci);

                        //合并片
                        if (ci == chunks)
                        {
                            //删除超过20个小时的片（处理异常造成的碎片）
                            var partAll = Directory.GetFiles(partDirP);
                            var now = DateTime.Now;
                            foreach (var pf in partAll)
                            {
                                if (new FileInfo(pf).CreationTime.AddHours(20) < now)
                                {
                                    System.IO.File.Delete(pf);
                                }
                            }

                            var partFiles = Directory.GetFiles(partDirP, sole + "_*");

                            var finalPath = Path.Combine(uploadDirP, file.FileName);
                            if (System.IO.File.Exists(finalPath))
                            {
                                //删除分块
                                partFiles.ToList().ForEach(x => System.IO.File.Delete(x));

                                vm.Set(SharedEnum.RTag.exist);
                                vm.Msg = "文件已经存在";
                            }
                            else
                            {
                                using (var fs = new FileStream(finalPath, FileMode.Create))
                                {
                                    //排一下序，保证从0-N Write
                                    var po = partFiles.OrderBy(x => x.Length).ThenBy(x => x);
                                    foreach (var part in po)
                                    {
                                        var bytes = System.IO.File.ReadAllBytes(part);
                                        fs.Write(bytes, 0, bytes.Length);
                                        bytes = null;
                                        System.IO.File.Delete(part);//删除分块
                                    }

                                    vm.Data = uploadDirV + file.FileName;
                                    vm.Set(SharedEnum.RTag.success);
                                }
                            }
                        }
                        else
                        {
                            vm.Set(SharedEnum.RTag.success);
                            vm.Data = partDirV + partName;
                            vm.Msg = "part success";
                        }
                    }
                    else
                    {
                        //直接上传
                        if (System.IO.File.Exists(uploadDirP + file.FileName))
                        {
                            vm.Set(SharedEnum.RTag.exist);
                            vm.Msg = "文件已经存在";
                        }
                        else
                        {
                            using (Stream fileStream = new FileStream(uploadDirP + file.FileName, FileMode.Create))
                            {
                                file.CopyTo(fileStream);
                            }

                            vm.Data = uploadDirV + file.FileName;
                            vm.Set(SharedEnum.RTag.success);
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

        public SharedResultVM FileList()
        {
            var vm = new SharedResultVM();
            try
            {

            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        public SharedResultVM FileDelete()
        {
            var vm = new SharedResultVM();
            try
            {

            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        public SharedResultVM VideoInfo()
        {
            var vm = new SharedResultVM();
            try
            {

            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        public SharedResultVM Sliced()
        {
            var vm = new SharedResultVM();
            try
            {

            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        public SharedResultVM VideoList()
        {
            var vm = new SharedResultVM();
            try
            {

            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        public SharedResultVM VideoDelete()
        {
            var vm = new SharedResultVM();
            try
            {

            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }
    }
}
