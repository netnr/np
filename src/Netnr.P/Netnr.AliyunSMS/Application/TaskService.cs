using FluentScheduler;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;

namespace Netnr.AliyunSMS.Application
{
    /// <summary>
    /// 定时任务
    /// </summary>
    public class TaskService
    {
        /// <summary>
        /// 任务组件
        /// </summary>
        public class TaskComponent
        {
            /// <summary>
            /// 任务注册
            /// </summary>
            public class Reg : Registry
            {
                /// <summary>
                /// 构造
                /// </summary>
                public Reg()
                {
                    int ws = GlobalTo.GetValue<int>("Task:WaitSeconds");
                    Schedule<SmsNotSent>().ToRunEvery(ws).Seconds();
                }
            }

            /// <summary>
            /// 检测未发送短信
            /// </summary>
            public class SmsNotSent : IJob
            {
                void IJob.Execute()
                {
                    //启用任务
                    if (GlobalTo.GetValue<bool>("Task:Enable"))
                    {
                        SmsNotSent();
                    }
                }
            }
        }

        /// <summary>
        /// 检测未发送短信
        /// </summary>
        /// <returns></returns>
        public static ActionResultVM SmsNotSent()
        {
            var vm = new ActionResultVM();
            var listPrimaryKey = new List<string>();

            try
            {
                string connectionString = string.Empty;

                //查询SQL
                var querySql = GlobalTo.GetValue("Task:QuerySql");

                DataTable dt = null;

                //数据源连接字符串
                if (Enum.TryParse(GlobalTo.GetValue("TypeDB"), true, out TypeDB TDB))
                {
                    connectionString = GlobalTo.Configuration.GetConnectionString(TDB.ToString());
                    switch (TDB)
                    {
                        case TypeDB.MySQL:
                            dt = new Data.MySQL.MySQLHelper(connectionString).Query(querySql).Tables[0];
                            break;
                        case TypeDB.SQLite:
                            dt = new Data.SQLite.SQLiteHelper(connectionString).Query(querySql).Tables[0];
                            break;
                        case TypeDB.Oracle:
                            dt = new Data.Oracle.OracleHelper(connectionString).Query(querySql).Tables[0];
                            break;
                        case TypeDB.SQLServer:
                            dt = new Data.SQLServer.SQLServerHelper(connectionString).Query(querySql).Tables[0];
                            break;
                        case TypeDB.PostgreSQL:
                            dt = new Data.PostgreSQL.PostgreSQLHelper(connectionString).Query(querySql).Tables[0];
                            break;
                    }
                }

                if (dt.Rows.Count > 0)
                {
                    var listPhoneNumber = new List<string>();
                    var listSignName = new List<string>();
                    var TemplateCode = string.Empty;
                    var listTemplateParamJson = new List<string>();

                    //表列名
                    var colPrimaryKey = GlobalTo.GetValue("Task:TableColumn:PrimaryKey");
                    var colPhoneNumber = GlobalTo.GetValue("Task:TableColumn:PhoneNumber");
                    var colSignName = GlobalTo.GetValue("Task:TableColumn:SignName");
                    var colTemplateCode = GlobalTo.GetValue("Task:TableColumn:TemplateCode");
                    var colTemplateParam = GlobalTo.GetValue("Task:TableColumn:TemplateParam");

                    foreach (DataRow dr in dt.Rows)
                    {
                        listPrimaryKey.Add(dr[colPrimaryKey].ToString());

                        listPhoneNumber.Add(dr[colPhoneNumber].ToString());
                        listSignName.Add(dr[colSignName].ToString());
                        if (string.IsNullOrWhiteSpace(TemplateCode))
                        {
                            TemplateCode = dr[colTemplateCode].ToString();
                        }
                        listTemplateParamJson.Add(dr[colTemplateParam].ToString());
                    }

                    //发送参数
                    var dicAddQuery = new Dictionary<string, string>
                    {
                        { "PhoneNumberJson", listPhoneNumber.ToJson() },
                        { "SignNameJson", listSignName.ToJson() },
                        { "TemplateCode", TemplateCode },
                        { "TemplateParamJson", listTemplateParamJson.ToJson() }
                    };

                    //调用发送方法
                    vm = SmsService.SendBatchSms(dicAddQuery);

                    //发送成功
                    if (vm.Code == 200)
                    {
                        //修改状态
                        var updateSql = GlobalTo.GetValue("Task:UpdateSql");
                        updateSql = string.Format(updateSql, "'" + string.Join("','", listPrimaryKey) + "'");

                        int num = 0;

                        switch (TDB)
                        {
                            case TypeDB.MySQL:
                                num = new Data.MySQL.MySQLHelper(connectionString).ExecuteNonQuery(updateSql);
                                break;
                            case TypeDB.SQLite:
                                num = new Data.SQLite.SQLiteHelper(connectionString).ExecuteNonQuery(updateSql);
                                break;
                            case TypeDB.Oracle:
                                num = new Data.Oracle.OracleHelper(connectionString).ExecuteNonQuery(updateSql);
                                break;
                            case TypeDB.SQLServer:
                                num = new Data.SQLServer.SQLServerHelper(connectionString).ExecuteNonQuery(updateSql);
                                break;
                            case TypeDB.PostgreSQL:
                                num = new Data.PostgreSQL.PostgreSQLHelper(connectionString).ExecuteNonQuery(updateSql);
                                break;
                        }

                        if (num <= 0)
                        {
                            vm.Msg += "（修改发送状态失败）";
                        }
                    }
                }
                else
                {
                    vm.Set(ARTag.lack);
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            //日志记录
            if (GlobalTo.GetValue<bool>("Logs:Enable"))
            {
                Core.ConsoleTo.Log(new { vm, listPrimaryKey }.ToJson() + Environment.NewLine);
            }

            return vm;
        }
    }
}
