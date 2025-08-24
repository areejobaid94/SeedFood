using BotService.Controllers;
using BotService.Interfaces;
using BotService.Models;
using BotService.Services;
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
using Infoseed.MessagingPortal.SocketIOClient;
using Infoseed.MessagingPortal.Configuration;
using Infoseed.MessagingPortal;

namespace BotService
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            AppSettingsModel.ConnectionStrings = Configuration.GetSection("ConnectionStrings").GetChildren().Where(x => x.Key == "Default").FirstOrDefault().Value;
            AppSettingsModel.GoogleMapsKey= Configuration.GetSection("GoogleMapsKey").GetChildren().Where(x => x.Key == "KeyMap").FirstOrDefault().Value;
            AppSettingsModel.AddHour = int.Parse(Configuration.GetSection("InfoSeedTime").GetChildren().Where(x => x.Key == "AddHour").FirstOrDefault().Value);
            AppSettingsModel.DivHour = int.Parse(Configuration.GetSection("InfoSeedTime").GetChildren().Where(x => x.Key == "DivHour").FirstOrDefault().Value);

            FcmNotificationSetting.SenderId = Configuration.GetSection("FcmNotification").GetChildren().Where(x => x.Key == "SenderId").FirstOrDefault().Value;
            FcmNotificationSetting.ServerKey = Configuration.GetSection("FcmNotification").GetChildren().Where(x => x.Key == "ServerKey").FirstOrDefault().Value;
            FcmNotificationSetting.webAddr = Configuration.GetSection("FcmNotification").GetChildren().Where(x => x.Key == "webAddr").FirstOrDefault().Value;

            AppSettingsCoreModel.SocketIOURL = Configuration.GetSection("SocketIO").GetChildren().Where(x => x.Key == "URL").FirstOrDefault().Value;
            AppSettingsCoreModel.SocketIOToken = Configuration.GetSection("SocketIO").GetChildren().Where(x => x.Key == "Token").FirstOrDefault().Value;

            services.AddScoped<IBotApis, BotApis>();
            SocketIOManager.Init();

            string cosmoEndPoin = Configuration.GetSection("CosmosDBSettings").GetChildren().Where(x => x.Key == "EndPoint").FirstOrDefault().Value;
            string cosmoMasterKey = Configuration.GetSection("CosmosDBSettings").GetChildren().Where(x => x.Key == "AuthKey").FirstOrDefault().Value;
            services.AddSingleton<IDocumentClient>(x => new DocumentClient(new Uri(cosmoEndPoin), cosmoMasterKey, new ConnectionPolicy { EnableEndpointDiscovery = false }));

            services.AddControllersWithViews();
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
