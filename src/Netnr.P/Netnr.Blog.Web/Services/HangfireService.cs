using Hangfire.Dashboard;
using Hangfire.InMemory;

namespace Netnr.Blog.Web.Services
{
    public class HangfireService
    {
        /// <summary>
        /// 初始化服务
        /// </summary>
        /// <param name="builder"></param>
        public static void InitServer(WebApplicationBuilder builder)
        {
            // Add Hangfire services.
            builder.Services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings(setting => setting.DefaultJsonSerializerSettings())
                .UseInMemoryStorage(new InMemoryStorageOptions
                {
                    MaxExpirationTime = TimeSpan.FromHours(1),
                    StringComparer = StringComparer.OrdinalIgnoreCase,
                }));

            // Add the processing server as IHostedService
            builder.Services.AddHangfireServer();
        }

        /// <summary>
        /// 授权
        /// </summary>
        public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
        {
            public bool Authorize(DashboardContext context)
            {
                var httpContext = context.GetHttpContext();

                return IdentityService.IsAdmin(httpContext);
            }
        }

        /// <summary>
        /// 初始化管理
        /// </summary>
        public static void InitManager(WebApplication app)
        {
            if (AppTo.GetValue<bool?>("ProgramParameters:DisableDatabaseWrite") != true && AppTo.GetValue<bool?>("ProgramParameters:DisableCrontab") != true)
            {
                //Hangfire
                app.UseHangfireDashboard("/api/hangfire", options: new DashboardOptions
                {
                    FaviconPath = "/favicon.ico",
                    DisplayStorageConnectionString = false,
                    StatsPollingInterval = 5000,
                    DashboardTitle = "Crontab",
                    Authorization = new[] { new HangfireAuthorizationFilter() }
                });

                //清理任务
                //JobStorage.Current.GetConnection().GetRecurringJobs().ForEach(x =>
                //{
                //    RecurringJob.RemoveIfExists(x.Id);
                //});

                try
                {
                    //不主动执行
                    RecurringJob.AddOrUpdate("ExecuteBackupData", () => WorkService.ExecuteBackupData(), Cron.Never, DefaultReJobOptions());

                    //每3天执行一次
                    RecurringJob.AddOrUpdate("ExecuteBackupToGit", () => WorkService.ExecuteBackupToGit(), "16 16 */3 * *", DefaultReJobOptions());

                    //6小时执行一次
                    RecurringJob.AddOrUpdate("ExecuteOperationRecord", () => WorkService.ExecuteOperationRecord(), "0 */6 * * *", DefaultReJobOptions());

                    //斗鱼房间在线状态
                    if (!string.IsNullOrWhiteSpace(AppTo.GetValue("ProgramParameters:CrontabDouyuRooms")))
                    {
                        RecurringJob.AddOrUpdate("ExecuteDouyuRoomOnlineStatus", () => WorkService.ExecuteDouyuRoomOnlineStatus(), "*/2 * * * *", DefaultReJobOptions());
                    }
                }
                catch (Exception ex)
                {
                    ConsoleTo.LogError(ex);
                }
            }
        }

        public static RecurringJobOptions DefaultReJobOptions()
        {
            return new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.Local
            };
        }

        public static string ConvertSecondsToCronExpression(int seconds)
        {
            if (seconds < 60)
            {
                return $"*/{seconds} * * * * *";
            }
            else
            {
                int minutes = (int)Math.Round((double)seconds / 60, MidpointRounding.AwayFromZero);
                return $"*/{minutes} * * * *";
            }
        }
    }
}
