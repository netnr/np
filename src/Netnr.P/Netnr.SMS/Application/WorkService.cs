namespace Netnr.SMS.Application
{
    /// <summary>
    /// 作业
    /// </summary>
    public class WorkService
    {
        /// <summary>
        /// 作业
        /// </summary>
        /// <returns></returns>
        public static SharedResultVM WorkAliyun()
        {
            return SharedResultVM.Try(vm =>
            {
                var ci = new Domain.ConfigInit();
                if (ci.GetValue<bool>("Work:Enable"))
                {
                    //主键
                    var listPrimaryKey = new List<string>();
                    //数据库类型
                    var tdb = ci.GetValue<SharedEnum.TypeDB>("TypeDB");
                    //连接字符串
                    var connectionString = ci.GetValue($"ConnectionStrings:{tdb}");
                    //查询SQL
                    var querySql = ci.GetValue("Work:QuerySql");
                    //查询结果
                    var db = SharedAdo.DbHelper.Init(tdb, connectionString);
                    var dt = db.SqlExecuteReader(querySql).Item1.Tables[0];

                    if (dt.Rows.Count > 0)
                    {
                        var listPhoneNumber = new List<string>();
                        var listSignName = new List<string>();
                        var TemplateCode = string.Empty;
                        var listTemplateParamJson = new List<string>();

                        //表列名
                        var colPrimaryKey = ci.GetValue("Work:TableColumn:PrimaryKey");
                        var colPhoneNumber = ci.GetValue("Work:TableColumn:PhoneNumber");
                        var colSignName = ci.GetValue("Work:TableColumn:SignName");
                        var colTemplateCode = ci.GetValue("Work:TableColumn:TemplateCode");
                        var colTemplateParam = ci.GetValue("Work:TableColumn:TemplateParam");

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
                        vm = AliyunService.SendBatchSms(dicAddQuery);

                        //发送成功
                        if (vm.Code == 200)
                        {
                            //修改状态
                            var updateSql = ci.GetValue("Work:UpdateSql");
                            updateSql = string.Format(updateSql, "'" + string.Join("','", listPrimaryKey) + "'");

                            var num = db.SqlExecuteNonQuery(updateSql);
                            if (num <= 0)
                            {
                                vm.Msg += "（修改发送状态失败）";
                            }
                        }
                    }
                    else
                    {
                        vm.Set(SharedEnum.RTag.lack);
                        vm.Msg = "咣";
                    }
                }
                else
                {
                    vm.Set(SharedEnum.RTag.refuse);
                    vm.Msg = "已暂停作业";
                }

                return vm;
            });
        }
    }
}
