using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.OpenApi.Models;
using System.Linq;
using System.Collections.Generic;

namespace Netnr.ResponseFramework.Web
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

            IMvcBuilder builder = services.AddControllersWithViews(options =>
            {
                //注册全局错误过滤器
                options.Filters.Add(new Filters.FilterConfigs.ErrorActionFilter());

                //注册全局过滤器
                options.Filters.Add(new Filters.FilterConfigs.GlobalActionAttribute());
            });

#if DEBUG
            builder.AddRazorRuntimeCompilation();
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
                    Title = "RF API",
                    Description = string.Join(" &nbsp; ", new List<string>
                    {
                        "<b>Source</b>：<a target='_blank' href='https://github.com/netnr/np'>https://github.com/netnr/np</a>",
                        "<b>Blog</b>：<a target='_blank' href='https://www.netnr.com'>https://www.netnr.com</a>"
                    })
                });

                "ResponseFramework.Web,ResponseFramework.Application,Fast".Split(',').ToList().ForEach(x =>
                {
                    c.IncludeXmlComments(System.AppContext.BaseDirectory + "Netnr." + x + ".xml", true);
                });
            });

            //授权访问信息
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            {
                options.Cookie.Name = "netnrf_auth";
                options.LoginPath = "/account/login";
            });

            //session
            services.AddSession();

            //数据库连接池
            services.AddDbContextPool<Data.ContextBase>(options =>
            {
                Data.ContextBase.DCOB(options);
            }, 99);

            //定时任务
            FluentScheduler.JobManager.Initialize(new Application.TaskService.Reg());

            //配置上传文件大小限制（详细信息：FormOptions）
            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = GlobalTo.GetValue<int>("StaticResource:MaxSize") * 1024 * 1024;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, Data.ContextBase db)
        {
            //是开发环境
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //数据库不存在则创建，创建后返回true
            if (db.Database.EnsureCreated())
            {
                //调用重置数据库（实际开发中，你可能不需要，或只初始化一些表数据）
                new Controllers.DKController(db).ResetDataBaseForJson();
            }

            //配置swagger（生产环境不需要，把该代码移至 是开发环境 条件里面）
            app.UseSwagger().UseSwaggerUI(c =>
            {
                c.DocumentTitle = "RF API";
                c.SwaggerEndpoint("/swagger/v1/swagger.json", c.DocumentTitle);
                c.InjectStylesheet("/Home/SwaggerCustomStyle");
            });

            //默认起始页index.html
            DefaultFilesOptions options = new DefaultFilesOptions();
            options.DefaultFileNames.Add("index.html");
            app.UseDefaultFiles(options);

            //静态资源允许跨域
            app.UseStaticFiles(new StaticFileOptions()
            {
                OnPrepareResponse = (x) =>
                {
                    x.Context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                }
            });

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
            });
        }
    }
}
