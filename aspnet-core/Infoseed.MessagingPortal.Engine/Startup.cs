using Abp.AspNetCore;
using Abp.AspNetCore.Mvc.Antiforgery;
using Abp.Castle.Logging.Log4Net;
using Abp.Extensions;
using Abp.PlugIns;
using Castle.Facilities.Logging;
using Framework.Integration.Implementation;
using Framework.Integration.Interfaces;
using Infoseed.MessagingPortal.Areas;
using Infoseed.MessagingPortal.Asset;
using Infoseed.MessagingPortal.Authorization.Users;
using Infoseed.MessagingPortal.CaptionBot;
using Infoseed.MessagingPortal.Configuration;
using Infoseed.MessagingPortal.DeliveryCost;
using Infoseed.MessagingPortal.Engine.Models.Firebase;
using Infoseed.MessagingPortal.EntityFrameworkCore;
using Infoseed.MessagingPortal.Evaluations;
using Infoseed.MessagingPortal.Identity;
using Infoseed.MessagingPortal.Items;
using Infoseed.MessagingPortal.LiveChat;
using Infoseed.MessagingPortal.Loyalty;
using Infoseed.MessagingPortal.MenuCategories;
using Infoseed.MessagingPortal.Menus;
using Infoseed.MessagingPortal.Orders;
using Infoseed.MessagingPortal.SealingReuest;
using Infoseed.MessagingPortal.SellingRequest;
using Infoseed.MessagingPortal.SocketIOClient;
using Infoseed.MessagingPortal.Web.Models;
using Infoseed.MessagingPortal.Web.Sunshine;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
//using AutoWrapper;
using Microsoft.OpenApi.Models;
using Owl.reCAPTCHA;
using System;
using System.IO;
using System.Linq;

namespace Infoseed.MessagingPortal.Engine
{
    public class Startup
    {

        private const string DefaultCorsPolicyName = "localhost";

        private readonly IConfigurationRoot _appConfiguration;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _hostingEnvironment = env;
            _appConfiguration = env.GetAppConfiguration();
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {

            // services.Configure<IISServerOptions>(options =>
            // {
            //     options.MaxRequestBodySize = 104857600; // Example: 100 MB
            // });

            // services.Configure<KestrelServerOptions>(options =>
            // {
            //     options.Limits.MaxRequestBodySize = 104857600; // Example: 100 MB
            // });
            services.AddSingleton<IConfiguration>(_appConfiguration);

            AppSettingsModel.ConnectionStrings = _appConfiguration.GetSection("ConnectionStrings").GetChildren().Where(x => x.Key == "Default").FirstOrDefault().Value;
            SettingsModel.ConnectionStrings = _appConfiguration.GetSection("ConnectionStrings").GetChildren().Where(x => x.Key == "Default").FirstOrDefault().Value;

            AppSettingsModel.BlobServiceConnectionStrings = _appConfiguration.GetSection("BlobService").GetChildren().Where(x => x.Key == "ConnectionString").FirstOrDefault().Value;
            SettingsModel.BlobServiceConnectionStrings = _appConfiguration.GetSection("BlobService").GetChildren().Where(x => x.Key == "ConnectionString").FirstOrDefault().Value;

            AppSettingsModel.GoogleMapsKey = _appConfiguration.GetSection("GoogleMapsKey").GetChildren().Where(x => x.Key == "KeyMap").FirstOrDefault().Value;
            SettingsModel.GoogleMapsKey = _appConfiguration.GetSection("GoogleMapsKey").GetChildren().Where(x => x.Key == "KeyMap").FirstOrDefault().Value;


            AppSettingsModel.TranslateKey = _appConfiguration.GetSection("TranslateKey").GetChildren().Where(x => x.Key == "KeyT").FirstOrDefault().Value;


            AppSettingsCoreModel.ConnectionStrings = _appConfiguration.GetSection("ConnectionStrings").GetChildren().Where(x => x.Key == "Default").FirstOrDefault().Value;

            AppSettingsCoreModel.BlobServiceConnectionStrings = _appConfiguration.GetSection("BlobService").GetChildren().Where(x => x.Key == "ConnectionString").FirstOrDefault().Value;

            AppSettingsCoreModel.GoogleMapsKey = _appConfiguration.GetSection("GoogleMapsKey").GetChildren().Where(x => x.Key == "KeyMap").FirstOrDefault().Value;


            AppSettingsModel.AddHour = int.Parse(_appConfiguration.GetSection("InfoSeedTime").GetChildren().Where(x => x.Key == "AddHour").FirstOrDefault().Value);
            AppSettingsModel.DivHour = int.Parse(_appConfiguration.GetSection("InfoSeedTime").GetChildren().Where(x => x.Key == "DivHour").FirstOrDefault().Value);



            SettingsModel.AzureWebJobsStorage = _appConfiguration.GetSection("AzureWebJobsStorage").GetChildren().Where(x => x.Key == "KeyA").FirstOrDefault().Value;
            AppSettingsCoreModel.AzureWebJobsStorage = _appConfiguration.GetSection("AzureWebJobsStorage").GetChildren().Where(x => x.Key == "KeyA").FirstOrDefault().Value;




            AppSettingsCoreModel.SocketIOURL = _appConfiguration.GetSection("SocketIO").GetChildren().Where(x => x.Key == "URL").FirstOrDefault().Value;
            AppSettingsCoreModel.SocketIOToken = _appConfiguration.GetSection("SocketIO").GetChildren().Where(x => x.Key == "Token").FirstOrDefault().Value;


            SettingsModel.MgKey=_appConfiguration.GetSection("MgMotor").GetChildren().Where(x => x.Key == "Key").FirstOrDefault().Value;
            SettingsModel.MgUrl=_appConfiguration.GetSection("MgMotor").GetChildren().Where(x => x.Key == "baseUrl").FirstOrDefault().Value;

            
           string cosmoEndPoin = _appConfiguration.GetSection("CosmosDBSettings").GetChildren().Where(x => x.Key == "EndPoint").FirstOrDefault().Value;
           string cosmoMasterKey = _appConfiguration.GetSection("CosmosDBSettings").GetChildren().Where(x => x.Key == "AuthKey").FirstOrDefault().Value;



            FcmNotificationSetting.SenderId = _appConfiguration.GetSection("FcmNotification").GetChildren().Where(x => x.Key == "SenderId").FirstOrDefault().Value;
            FcmNotificationSetting.ServerKey = _appConfiguration.GetSection("FcmNotification").GetChildren().Where(x => x.Key == "ServerKey").FirstOrDefault().Value;
            FcmNotificationSetting.webAddr = _appConfiguration.GetSection("FcmNotification").GetChildren().Where(x => x.Key == "webAddr").FirstOrDefault().Value;

            AppSettingsModel.BotApi = _appConfiguration.GetSection("Bot").GetChildren().Where(x => x.Key == "Api").FirstOrDefault().Value;

            AppSettingsModel.connectionStringMongoDB = _appConfiguration.GetSection("MongoDB").GetChildren().Where(x => x.Key == "connectionStringMongoDB").FirstOrDefault().Value;
            AppSettingsModel.databaseName = _appConfiguration.GetSection("MongoDB").GetChildren().Where(x => x.Key == "databaseName").FirstOrDefault().Value;
            AppSettingsModel.collectionName = _appConfiguration.GetSection("MongoDB").GetChildren().Where(x => x.Key == "collectionName").FirstOrDefault().Value;
            AppSettingsModel.urlSendCampaignProject = _appConfiguration.GetSection("MongoDB").GetChildren().Where(x => x.Key == "urlSendCampaignProject").FirstOrDefault().Value;


            services.AddSwaggerGen();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo() { Title = "MessagingPortal API", Version = "v1" });

            }).AddSwaggerGenNewtonsoftSupport();

            services.AddSignalR();

            services.AddCors(options =>
            {
                options.AddPolicy(DefaultCorsPolicyName, builder =>
                {
                    //App:CorsOrigins in appsettings.json can contain more than one address with splitted by comma.
                    builder
                        .WithOrigins(
                            // App:CorsOrigins in appsettings.json can contain more than one address separated by comma.
                            _appConfiguration["App:CorsOrigins"]
                                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                                .Select(o => o.RemovePostFix("/"))
                                .ToArray()
                        )
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            IdentityRegistrar.Register(services);
            AuthConfigurer.Configure(services, _appConfiguration);


            //Recaptcha
            services.AddreCAPTCHAV3(x =>
            {
                x.SiteKey = _appConfiguration["Recaptcha:SiteKey"];
                x.SiteSecret = _appConfiguration["Recaptcha:SecretKey"];
            });


       

            services.AddScoped<IDBService, CosmosDBService>();
            services.AddScoped<IMenusAppService, MenusAppService>();
            services.AddScoped<IItemsAppService, ItemsAppService>();
            services.AddScoped<IAreasAppService, AreasAppService>();
            services.AddScoped<IUserAppService, UserAppService>();
            services.AddScoped<ISellingRequestAppService, SellingRequestAppService>();
            services.AddScoped<IOrdersAppService, OrdersAppService>();
            services.AddScoped<IDeliveryCostAppService, DeliveryCostAppService>();
            services.AddScoped<IAssetAppService, AssetAppService>();
            services.AddScoped<IMenuCategoriesAppService, ItemCategoryAppService>();
            services.AddScoped<ILiveChatAppService, LiveChatAppService>();
            services.AddScoped<IEvaluationsAppService, EvaluationsAppService>();
            services.AddScoped<ICaptionBotAppService, CaptionBotAppService>();
            services.AddScoped<ILoyaltyAppService, LoyaltyAppService>();

          
            
            services.AddScoped<IContactsAPI, ContactsAPI>();
            services.AddControllersWithViews(options =>
            {
                options.Filters.Add(new AbpAutoValidateAntiforgeryTokenAttribute());
            }).AddNewtonsoftJson();

            services.AddApplicationInsightsTelemetry(_appConfiguration["ApplicationInsights:InstrumentationKey"]);



            services.AddSingleton<IDocumentClient>(x => new  DocumentClient(new Uri(cosmoEndPoin), cosmoMasterKey, new ConnectionPolicy { EnableEndpointDiscovery = false }));


            SocketIOManager.Init();
         return   services.AddAbp<MessagingPortalEngineModule>(options =>
            {
                //Configure Log4Net logging
                options.IocManager.IocContainer.AddFacility<LoggingFacility>(
                    f => f.UseAbpLog4Net().WithConfig(_hostingEnvironment.IsDevelopment()
                            ? "log4net.config"
                            : "log4net.Production.config")
                );

                options.PlugInSources.AddFolder(Path.Combine(_hostingEnvironment.WebRootPath, "Plugins"), SearchOption.AllDirectories);
            });

        }





        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {

            app.UseAbp(options =>
            {
                options.UseAbpRequestLocalization = false; //used below: UseAbpRequestLocalization
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {

                app.UseStatusCodePagesWithRedirects("~/Error?statusCode={0}");
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseSentryTracing();
            app.UseCors(DefaultCorsPolicyName); //Enable CORS!

            app.UseAuthentication();

            if (bool.Parse(_appConfiguration["IdentityServer:IsEnabled"]))
            {
                app.UseIdentityServer();
            }

            app.UseAuthorization();

            using (var scope = app.ApplicationServices.CreateScope())
            {
                if (scope.ServiceProvider.GetService<DatabaseCheckHelper>().Exist(_appConfiguration["ConnectionStrings:Default"]))
                {
                    app.UseAbpRequestLocalization();
                }
            }


   
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });

            if (bool.Parse(_appConfiguration["HealthChecks:HealthChecksEnabled"]))
            {
                if (bool.Parse(_appConfiguration["HealthChecks:HealthChecksUI:HealthChecksUIEnabled"]))
                {
                    app.UseHealthChecksUI();
                }
            }
          
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My Test1 Api v1");

            });

            
        }

    }

}
