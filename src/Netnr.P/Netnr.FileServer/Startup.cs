using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.FileProviders;
using Netnr.SharedFast;

namespace Netnr.FileServer
{
    /// <summary>
    /// 
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="configuration">配置信息</param>
        /// <param name="env">环境信息</param>
        public Startup(IConfiguration configuration, IHostEnvironment env)
        {
            GlobalTo.Configuration = configuration;
            GlobalTo.HostEnvironment = env;
        }

        /// This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

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
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "FileServer API",
                    Description = string.Join(" &nbsp; ", new List<string>
                    {
                        "<b>Source</b>：<a target='_blank' href='https://github.com/netnr/np'>https://github.com/netnr/np</a>",
                        "<b>Blog</b>：<a target='_blank' href='https://www.netnr.com'>https://www.netnr.com</a>",
                        $"文件上传大小限制：<b>{GlobalTo.GetValue<int>("StaticResource:MaxSize")}</b> MB",
                        "管理员默认密码：<b>nr</b>"
                    })
                });

                c.IncludeXmlComments(AppContext.BaseDirectory + GetType().Namespace + ".xml", true);
            });

            //配置上传文件大小限制（详细信息：FormOptions）
            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = GlobalTo.GetValue<int>("StaticResource:MaxSize") * 1024 * 1024;
            });
        }

        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //初始化库
            using var db = new SQLite.SQLiteConnection(Application.FileServerService.SQLiteConn);
            db.CreateTable<Model.SysApp>();
            db.CreateTable<Model.FileRecord>();

            //配置swagger
            app.UseSwagger().UseSwaggerUI(c =>
            {
                c.DocumentTitle = "FileServer API";
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "FileServer API");
                c.InjectStylesheet("/Home/SwaggerCustomStyle");
            });

            //静态资源允许跨域
            app.UseStaticFiles(new StaticFileOptions()
            {
                OnPrepareResponse = (x) =>
                {
                    x.Context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                },
                ServeUnknownFileTypes = true
            });

            //目录浏览&&公开访问
            if (GlobalTo.GetValue<bool>("Safe:EnableDirectoryBrowsing") && GlobalTo.GetValue<bool>("Safe:PublicAccess"))
            {
                string vrootdir = GlobalTo.GetValue("StaticResource:RootDir");
                string prootdir = Application.FileServerService.StaticVrPathAsPhysicalPath(vrootdir);
                if (!System.IO.Directory.Exists(prootdir))
                {
                    System.IO.Directory.CreateDirectory(prootdir);
                }
                app.UseFileServer(new FileServerOptions()
                {
                    FileProvider = new PhysicalFileProvider(prootdir),
                    //目录浏览链接
                    RequestPath = new PathString(vrootdir),
                    EnableDirectoryBrowsing = true,
                    EnableDefaultFiles = false
                });
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
