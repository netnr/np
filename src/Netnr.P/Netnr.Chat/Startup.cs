using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Netnr.Core;
using Netnr.SharedFast;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Netnr.Chat
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostEnvironment env)
        {
            GlobalTo.Configuration = configuration;
            GlobalTo.HostEnvironment = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                //cookie存储需用户同意，欧盟新标准，暂且关闭，否则用户没同意无法写入
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            IMvcBuilder builder = services.AddControllersWithViews();

#if DEBUG
            builder.AddRazorRuntimeCompilation();
            //开发时：安装该包可以动态修改视图 cshtml 页面，无需重新运行项目
            //发布时：建议删除该包，会生成一堆“垃圾”
            //Install-Package Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation
#endif

            services.AddControllers().AddNewtonsoftJson(options =>
            {
                //Action原样输出JSON
                options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
                //日期格式化
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss.fff";
            });

            //配置swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Netnr API",
                    Version = "v1"
                });

                //注释
                "Chat".Split(',').ToList().ForEach(x =>
                {
                    c.IncludeXmlComments(AppContext.BaseDirectory + "Netnr." + x + ".xml", true);
                });
            });

            //授权访问信息
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();

            //session
            services.AddSession();

            //配置上传文件大小限制（详细信息：FormOptions）
            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = GlobalTo.GetValue<int>("StaticResource:MaxSize") * 1024 * 1024;
            });

            //数据库连接池
            services.AddDbContextPool<Data.ContextBase>(options =>
            {
                Data.ContextBaseFactory.CreateDbContextOptionsBuilder(options);
            }, 20);

            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, Data.ContextBase db)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //数据库不存在则创建，创建后返回true
            if (db.Database.EnsureCreated())
            {

            }
            if (!db.NChatUser.Any())
            {
                db.NChatUser.AddRange(new List<Domain.NChatUser>()
                {
                    new Domain.NChatUser()
                    {
                        CuUserId="5098247860344549548",
                        CuCreateTime=DateTime.Now,
                        CuPassword=CalcTo.MD5("netnr"),
                        CuStatus=1,
                        CuUserName="netnr",
                        CuUserNickname="netnr",
                        CuUserPhoto="favicon.ico"
                    },
                    new Domain.NChatUser()
                    {
                        CuUserId="5757526144712703761",
                        CuCreateTime=DateTime.Now,
                        CuPassword=CalcTo.MD5("123"),
                        CuStatus=1,
                        CuUserName="123",
                        CuUserNickname="123",
                        CuUserPhoto="favicon.ico"
                    }
                });
                db.SaveChanges();
            }

            //配置swagger
            app.UseSwagger().UseSwaggerUI(c =>
            {
                c.DocumentTitle = "Netnr Chat API";
                c.SwaggerEndpoint("/swagger/v1/swagger.json", c.DocumentTitle);
            });

            app.UseStaticFiles();

            app.UseRouting();

            //授权访问
            app.UseAuthentication();
            app.UseAuthorization();

            //session
            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapHub<Application.ChatHubService>("/chathub");
            });
        }
    }
}
