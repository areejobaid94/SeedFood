using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infoseed.MessagingPortal.Configuration;
using Abp.AspNetCore;
using Abp.Runtime.Caching;
using Owl.reCAPTCHA;
using Infoseed.MessagingPortal.Web.Models;

namespace CampaignProject
{
    public class Startup
    {
        private readonly IConfigurationRoot _appConfiguration;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _appConfiguration = env.GetAppConfiguration();
            Configuration = configuration;
        }
    
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string cosmoEndPoin = _appConfiguration.GetSection("CosmosDBSettings").GetChildren().Where(x => x.Key == "EndPoint").FirstOrDefault().Value;
            string cosmoMasterKey = _appConfiguration.GetSection("CosmosDBSettings").GetChildren().Where(x => x.Key == "AuthKey").FirstOrDefault().Value;


            ////Recaptcha
            //services.AddreCAPTCHAV3(x =>
            //{
            //    x.SiteKey = _appConfiguration["Recaptcha:SiteKey"];
            //    x.SiteSecret = _appConfiguration["Recaptcha:SecretKey"];
            //});
            SettingsModel.ConnectionStrings = _appConfiguration.GetSection("ConnectionStrings").GetChildren().Where(x => x.Key == "Default").FirstOrDefault().Value;



            services.AddSingleton<IDocumentClient>(x => new DocumentClient(new Uri(cosmoEndPoin), cosmoMasterKey, new ConnectionPolicy { EnableEndpointDiscovery = false }));

            services.AddControllersWithViews();
            // Register Swagger services
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Your API",
                    Version = "v1"
                });
            });

            // Optional: Use Newtonsoft JSON for Swagger
            services.AddSwaggerGenNewtonsoftSupport();
         
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            // Enable Swagger middleware
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API v1");
                c.RoutePrefix = string.Empty; // Swagger UI at root
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
