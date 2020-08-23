using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Netnr.AliyunSMS
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
                    Title = "AliyunSMS API",
                    Version = "v1"
                });

                "AliyunSMS,Fast".Split(',').ToList().ForEach(x =>
                {
                    c.IncludeXmlComments(AppContext.BaseDirectory + "Netnr." + x + ".xml", true);
                });
            });

            //定时任务
            FluentScheduler.JobManager.Initialize(new Application.TaskService.TaskComponent.Reg());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //配置swagger
            app.UseSwagger().UseSwaggerUI(c =>
            {
                c.DocumentTitle = "AliyunSMS API";
                c.SwaggerEndpoint("/swagger/v1/swagger.json", c.DocumentTitle);
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();

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
