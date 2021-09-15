using Microsoft.AspNetCore.Http.Features;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Netnr.Core;
using Netnr.SharedFast;

namespace Netnr.Chat
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostEnvironment env)
        {
            GlobalTo.Configuration = configuration;
            GlobalTo.HostEnvironment = env;

            //����ע��
            GlobalTo.EncodingReg();
        }

        //����swagger
        public string ver = "v1";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                //cookie�洢���û�ͬ�⣬ŷ���±�׼�����ҹرգ������û�ûͬ���޷�д��
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddControllersWithViews();

            services.AddControllers().AddNewtonsoftJson(options =>
            {
                //Actionԭ�����JSON
                options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
                //���ڸ�ʽ��
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss.fff";
            });

            //����swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(ver, new OpenApiInfo
                {
                    Title = GlobalTo.HostEnvironment.ApplicationName
                });
                //ע��
                c.IncludeXmlComments(AppContext.BaseDirectory + GetType().Namespace + ".xml", true);
            });

            //��Ȩ������Ϣ
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();

            //session
            services.AddSession();

            //�����ϴ��ļ���С���ƣ���ϸ��Ϣ��FormOptions��
            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = GlobalTo.GetValue<int>("StaticResource:MaxSize") * 1024 * 1024;
            });

            //���ݿ����ӳ�
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

            //���ݿⲻ�����򴴽��������󷵻�true
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

            //����swagger
            app.UseSwagger().UseSwaggerUI(c =>
            {
                c.DocumentTitle = GlobalTo.HostEnvironment.ApplicationName;
                c.SwaggerEndpoint($"{ver}/swagger.json", c.DocumentTitle);
            });

            app.UseStaticFiles();

            app.UseRouting();

            //��Ȩ����
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
