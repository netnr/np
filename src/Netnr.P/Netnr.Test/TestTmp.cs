using Xunit;

namespace Netnr.Test
{
    public class TestTmp
    {
        [Fact]
        public async Task Tmp()
        {
            await Task.Delay(0);

            Debug.WriteLine("tmp");
        }

        [Fact]
        public async Task ClearCMS()
        {
            await Task.Delay(1000 * 5);

            var connOption = new DbKitConnectionOption
            {
                ConnectionString = "Server=local.host,1433;uid=sa;pwd=Abc1230...;database=xops",
                ConnectionType = DBTypes.SQLServer
            };
            var dbKit = connOption.CreateDbInstance();
            await dbKit.SqlExecuteNonQuery("truncate table cms_content;truncate table cms_comment;truncate table cms_collect");
        }

        [Fact]
        public async Task MountContentToCollect()
        {
            var connOption = new DbKitConnectionOption
            {
                ConnectionString = "Server=local.host,1433;uid=sa;pwd=Abc1230...;database=xops",
                ConnectionType = DBTypes.SQLServer
            };
            var dbKit = connOption.CreateDbInstance();
            var edsContent = await dbKit.SqlExecuteDataSet("select * from cms_content");
            var dtContent = edsContent.Datas.Tables[0];

            var edsCollect = await dbKit.SqlExecuteDataSet("select * from cms_collect where 1=2");
            var dtCollect = edsCollect.Datas.Tables[0];
            dtCollect.TableName = "cms_collect";
            var now = DateTime.Now;

            foreach (DataRow dr in dtContent.Rows)
            {
                var newdr = dtCollect.NewRow();
                newdr["collect_id"] = Snowflake53To.Id();
                newdr["collect_code"] = newdr["collect_id"];
                newdr["create_time"] = now;
                newdr["user_id"] = 148981413900288;

                newdr["content_id"] = dr["content_id"];
                newdr["content_category"] = dr["content_category"];

                newdr["collect_pid"] = 0; // 根据类别挂载父级
                newdr["collect_name"] = dr["content_title"];
                newdr["collect_order"] = 0;
                newdr["collect_directory"] = 0;
                newdr["collect_private"] = 0;

                newdr["collect_remark"] = "batch";

                dtCollect.Rows.Add(newdr.ItemArray);
            }
            var num = await dbKit.BulkCopy(dtCollect);
            Debug.WriteLine(num);
        }

        [Fact]
        public async Task DocToCollect()
        {
            var connOption2 = new DbKitConnectionOption
            {
                ConnectionString = "Server=local.host,1433;uid=sa;pwd=Abc1230...;database=netnr",
                ConnectionType = DBTypes.SQLServer
            };
            var dbKit2 = connOption2.CreateDbInstance();

            var connOption = new DbKitConnectionOption
            {
                ConnectionString = "Server=local.host,1433;uid=sa;pwd=Abc1230...;database=xops",
                ConnectionType = DBTypes.SQLServer
            };
            var dbKit = connOption.CreateDbInstance();

            var edo = await dbKit2.SqlExecuteDataOnly("select * from DocSet order by DsCreateTime asc;select * from DocSetDetail order by DsdOrder asc");
            var dtDoc = edo.Datas.Tables[0];
            var dtDocDetail = edo.Datas.Tables[1];

            var eds = await dbKit.SqlExecuteDataSet("select * from cms_collect where 1=2;select * from cms_content where 1=2");
            var dtCollect = eds.Datas.Tables[0];
            dtCollect.TableName = "cms_collect";

            var dtContent = eds.Datas.Tables[1];
            dtContent.TableName = "cms_content";

            var now = DateTime.Now;

            foreach (DataRow dr in dtDoc.Rows)
            {
                var newdr = dtCollect.NewRow();

                newdr["collect_id"] = Snowflake53To.Id();
                newdr["collect_code"] = dr["DsCode"];
                newdr["create_time"] = now;
                newdr["user_id"] = dr["Uid"].ToString() == "1" ? 148981413900288 : dr["Uid"];

                newdr["collect_pid"] = 0;
                newdr["collect_name"] = dr["DsName"];
                newdr["collect_order"] = 0;
                newdr["collect_directory"] = 1;

                newdr["content_id"] = 0;

                newdr["collect_private"] = dr["DsOpen"].ToString() == "1" ? 0 : 1;
                newdr["collect_share_code"] = dr["Spare1"];

                newdr["collect_remark"] = dr["DsRemark"];

                dtCollect.Rows.Add(newdr.ItemArray);
            }

            ConvertDocSetDetail(dtDocDetail, dtDoc, ref dtCollect, ref dtContent);

            var num = await dbKit.BulkCopy(dtCollect);
            num += await dbKit.BulkCopy(dtContent);

            Debug.WriteLine(num);
        }

        private static void ConvertDocSetDetail(DataTable dtDetail, DataTable dtDoc, ref DataTable dtOutCollect, ref DataTable dtOutContent, string startPid = null)
        {
            startPid ??= Guid.Empty.ToString();

            var now = DateTime.Now;
            var drs = dtDetail.AsEnumerable().Where(x => x["DsdPid"].ToString() == startPid);
            foreach (DataRow dr in drs)
            {
                var newdr = dtOutCollect.NewRow();

                var dsCode = dr["DsCode"].ToString();
                var dsdId = dr["DsdId"].ToString();
                var dsdPid = dr["DsdPid"].ToString();

                newdr["collect_id"] = Snowflake53To.Id();
                newdr["collect_code"] = dsdId;
                newdr["create_time"] = now;
                newdr["user_id"] = dr["Uid"].ToString() == "1" ? 148981413900288 : dr["Uid"];

                var topicRow = dtOutCollect.AsEnumerable().FirstOrDefault(x => x["collect_code"].ToString() == dsCode);
                newdr["collect_private"] = topicRow == null ? 0 : topicRow["collect_private"];

                var pdr = dtOutCollect.AsEnumerable().FirstOrDefault(x => x["collect_code"].ToString() == dsdPid);
                if (pdr != null)
                {
                    newdr["collect_pid"] = pdr["collect_id"];
                }
                else
                {
                    if (topicRow == null)
                    {
                        continue;
                    }
                    newdr["collect_pid"] = topicRow["collect_id"];
                }

                newdr["collect_name"] = dr["DsdTitle"];
                newdr["collect_order"] = dr["DsdOrder"];

                var isDirectory = dr["DsdContentMd"] == DBNull.Value;
                newdr["collect_directory"] = isDirectory ? 1 : 0;
                if (isDirectory)
                {
                    newdr["content_id"] = 0;
                }
                else
                {
                    newdr["content_id"] = Snowflake53To.Id();
                    newdr["content_category"] = "blog";

                    var newdrContent = dtOutContent.NewRow();
                    newdrContent["content_id"] = newdr["content_id"];
                    newdrContent["content_code"] = dsdId;
                    newdrContent["create_time"] = now;
                    newdrContent["update_time"] = now;
                    newdrContent["audit_time"] = now;
                    newdrContent["user_id"] = newdr["user_id"];
                    newdrContent["content_status"] = 3;
                    newdrContent["content_category"] = "blog";
                    newdrContent["content_title"] = newdr["collect_name"];
                    newdrContent["content_type"] = "markdown";
                    newdrContent["content_body1"] = dr["DsdContentMd"];
                    newdrContent["content_body2"] = dr["DsdContentHtml"];
                    newdrContent["content_order"] = 0;
                    newdrContent["content_visit_count"] = 0;
                    newdrContent["content_comment_count"] = 0;
                    newdrContent["content_like_count"] = 0;
                    newdrContent["content_disable_comment"] = 0;
                    newdrContent["content_private"] = 0;

                    dtOutContent.Rows.Add(newdrContent.ItemArray);
                }

                newdr["collect_private"] = 0;
                dtOutCollect.Rows.Add(newdr.ItemArray);

                if (isDirectory)
                {
                    ConvertDocSetDetail(dtDetail, dtDoc, ref dtOutCollect, ref dtOutContent, dsdId);
                }
            }
        }
    }
}