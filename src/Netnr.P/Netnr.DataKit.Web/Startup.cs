using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Netnr.SharedFast;
using Newtonsoft.Json.Converters;

namespace Netnr.DataKit.Web
{
    public class Startup
    {
        public Startup(IHostEnvironment env)
        {
            GlobalTo.HostEnvironment = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddControllers().AddNewtonsoftJson(options =>
            {
                //Action原样输出JSON
                options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
                //日期格式化
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss.fff";

                //swagger枚举显示名称
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
            });
            //swagger枚举显示名称
            services.AddSwaggerGenNewtonsoftSupport();

            //配置swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Netnr.DataKit API",
                    Description = string.Join(" &nbsp; ", new List<string>
                    {
                        "<b>Source</b>：<a target='_blank' href='https://github.com/netnr/np'>https://github.com/netnr/np</a>",
                        "<b>Blog</b>：<a target='_blank' href='https://www.netnr.com'>https://www.netnr.com</a>"
                    })
                });

                c.IncludeXmlComments(AppContext.BaseDirectory + "Netnr.DataKit.Web.xml", true);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //配置swagger
            app.UseSwagger().UseSwaggerUI(c =>
            {
                c.DocumentTitle = "Netnr.DataKit API";
                c.SwaggerEndpoint("/swagger/v1/swagger.json", c.DocumentTitle);
                c.InjectStylesheet("/Home/SwaggerCustomStyle");
            });

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
