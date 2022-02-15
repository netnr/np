using Netnr.SharedDataKit;
using Newtonsoft.Json.Linq;

namespace Netnr.DataX.Application
{
    public class SilenceService
    {
        /// <summary>
        /// 运行
        /// </summary>
        /// <param name="tasks"></param>
        public static void Run(List<string> tasks)
        {
            DXService.Log($"\n开始静默执行任务({tasks.Count} 个): {string.Join(" ", tasks)}\n");

            var ci = new Domain.ConfigInit();
            var co = ci.ConfigObj;
            var cs = ci.Silence;

            for (int ti = 0; ti < tasks.Count; ti++)
            {
                var taskName = tasks[ti];

                if (cs.ContainsKey(taskName))
                {
                    try
                    {
                        var taskJson = cs[taskName] as JObject;
                        //方法
                        var methods = taskJson.Properties().Select(p => p.Name).ToList();

                        DXService.Log($"开始 {taskName} 任务，进度 {ti + 1}/{tasks.Count}\n");

                        for (int mi = 0; mi < methods.Count; mi++)
                        {
                            var methodName = methods[mi];
                            DXService.Log($"开始 {methodName} 方法，进度 {mi + 1}/{methods.Count}\n");

                            //参数
                            var parameters = taskJson[methodName];

                            switch (methodName)
                            {
                                case "ExportDatabase":
                                    {
                                        var mo = parameters.ToJson().ToEntity<TransferVM.ExportDatabase>();
                                        mo.ZipPath = DXService.ParsePathVar(mo.ZipPath);

                                        //引用连接
                                        if (mo.ReadConnectionInfo == null && !string.IsNullOrWhiteSpace(mo.RefReadConnectionInfo))
                                        {
                                            mo.ReadConnectionInfo = co.ListConnectionInfo.FirstOrDefault(x => x.ConnectionRemark == mo.RefReadConnectionInfo);
                                        }

                                        DataKit.ExportDatabase(mo, le => DXService.Log(le.NewItems[0].ToString()));
                                    }
                                    break;
                                case "ExportDataTable":
                                    {
                                        var mo = parameters.ToJson().ToEntity<TransferVM.ExportDataTable>();
                                        mo.ZipPath = DXService.ParsePathVar(mo.ZipPath);

                                        //引用连接
                                        if (mo.ReadConnectionInfo == null && !string.IsNullOrWhiteSpace(mo.RefReadConnectionInfo))
                                        {
                                            mo.ReadConnectionInfo = co.ListConnectionInfo.FirstOrDefault(x => x.ConnectionRemark == mo.RefReadConnectionInfo);
                                        }

                                        DataKit.ExportDataTable(mo, le => DXService.Log(le.NewItems[0].ToString()));
                                    }
                                    break;
                                case "MigrateDatabase":
                                    {
                                        var mo = parameters.ToJson().ToEntity<TransferVM.MigrateDatabase>();

                                        //引用连接
                                        if (mo.ReadConnectionInfo == null && !string.IsNullOrWhiteSpace(mo.RefReadConnectionInfo))
                                        {
                                            mo.ReadConnectionInfo = co.ListConnectionInfo.FirstOrDefault(x => x.ConnectionRemark == mo.RefReadConnectionInfo);
                                        }
                                        if (mo.WriteConnectionInfo == null && !string.IsNullOrWhiteSpace(mo.RefWriteConnectionInfo))
                                        {
                                            mo.WriteConnectionInfo = co.ListConnectionInfo.FirstOrDefault(x => x.ConnectionRemark == mo.RefWriteConnectionInfo);
                                        }

                                        DataKit.MigrateDataTable(mo.AsMigrateDataTable(), le => DXService.Log(le.NewItems[0].ToString()));
                                    }
                                    break;
                                case "MigrateDataTable":
                                    {
                                        var mo = parameters.ToJson().ToEntity<TransferVM.MigrateDataTable>();

                                        //引用连接
                                        if (mo.ReadConnectionInfo == null && !string.IsNullOrWhiteSpace(mo.RefReadConnectionInfo))
                                        {
                                            mo.ReadConnectionInfo = co.ListConnectionInfo.FirstOrDefault(x => x.ConnectionRemark == mo.RefReadConnectionInfo);
                                        }
                                        if (mo.WriteConnectionInfo == null && !string.IsNullOrWhiteSpace(mo.RefWriteConnectionInfo))
                                        {
                                            mo.WriteConnectionInfo = co.ListConnectionInfo.FirstOrDefault(x => x.ConnectionRemark == mo.RefWriteConnectionInfo);
                                        }

                                        DataKit.MigrateDataTable(mo, le => DXService.Log(le.NewItems[0].ToString()));
                                    }
                                    break;
                                case "ImportDatabase":
                                    {
                                        var mo = parameters.ToJson().ToEntity<TransferVM.ImportDatabase>();
                                        mo.ZipPath = DXService.ParsePathVar(mo.ZipPath);

                                        //引用连接
                                        if (mo.WriteConnectionInfo == null && !string.IsNullOrWhiteSpace(mo.RefWriteConnectionInfo))
                                        {
                                            mo.WriteConnectionInfo = co.ListConnectionInfo.FirstOrDefault(x => x.ConnectionRemark == mo.RefWriteConnectionInfo);
                                        }

                                        DataKit.ImportDatabase(mo, le => DXService.Log(le.NewItems[0].ToString()));
                                    }
                                    break;
                                default:
                                    DXService.Log($"not support {methodName}\n");
                                    break;
                            }
                        }

                        DXService.Log($"\n完成 {taskName} 任务\n");
                    }
                    catch (Exception ex)
                    {
                        DXService.Log($"任务（{taskName}）出错");
                        DXService.Log(ex.ToJson());
                    }
                }
                else
                {
                    DXService.Log($"无效任务（{taskName}）");
                }
            }

            DXService.Log($"\n任务全部完成\n");
        }
    }
}
