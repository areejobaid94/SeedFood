using Abp.AspNetCore;
using Abp.AspNetCore.Mvc.Antiforgery;
using Abp.Castle.Logging.Log4Net;
using Abp.Extensions;
using Abp.PlugIns;
using AutoMapper;
using Castle.Facilities.Logging;
using Framework.Integration.Implementation;
using Framework.Integration.Interfaces;
using Framework.Payment.Implementation.Zoho;
using Framework.Payment.Interfaces.Zoho;
using Infoseed.MessagingPortal.Areas;
using Infoseed.MessagingPortal.Asset;
using Infoseed.MessagingPortal.Authorization.Users;
using Infoseed.MessagingPortal.booking;
using Infoseed.MessagingPortal.Booking;
using Infoseed.MessagingPortal.BotAPI.Controllers;
using Infoseed.MessagingPortal.BotAPI.Interfaces;
using Infoseed.MessagingPortal.BotAPI.Models.Firebase;
using Infoseed.MessagingPortal.CaptionBot;
using Infoseed.MessagingPortal.Configuration;
using Infoseed.MessagingPortal.Configuration.Tenants;
using Infoseed.MessagingPortal.Customers;
using Infoseed.MessagingPortal.DeliveryCost;
using Infoseed.MessagingPortal.Departments;
using Infoseed.MessagingPortal.EntityFrameworkCore;
using Infoseed.MessagingPortal.Evaluations;
using Infoseed.MessagingPortal.General;
using Infoseed.MessagingPortal.Identity;
using Infoseed.MessagingPortal.Items;
using Infoseed.MessagingPortal.LiveChat;
using Infoseed.MessagingPortal.MenuCategories;
using Infoseed.MessagingPortal.Menus;
using Infoseed.MessagingPortal.Orders;
using Infoseed.MessagingPortal.SealingReuest;
using Infoseed.MessagingPortal.SellingRequest;
using Infoseed.MessagingPortal.SocketIOClient;
using Infoseed.MessagingPortal.Web.Models;
using Infoseed.MessagingPortal.Web.Sunshine;
using Infoseed.MessagingPortal.WhatsApp;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Owl.reCAPTCHA;
using System;
using System.IO;
using System.Linq;

namespace Infoseed.MessagingPortal.BotAPI
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

            //services.Configure<GatewayApiConfig>(Configuration.GetSection("GatewayApiConfig"));

            services.AddSingleton<IConfiguration>(_appConfiguration);
            Models.AppsettingsModel.AttacmentTypesAllowed = _appConfiguration.GetSection("AllowedAttacmentTypes").GetChildren().ToDictionary(x => x.Key, x => x.Value);
            Models.AppsettingsModel.AttacmentTypesAllowedM = _appConfiguration.GetSection("AllowedAttacmentTypesM").GetChildren().ToDictionary(x => x.Key, x => x.Value);



            AppSettingsModel.ConnectionStrings = _appConfiguration.GetSection("ConnectionStrings").GetChildren().Where(x => x.Key == "Default").FirstOrDefault().Value;
           // AppSettingsModel.EngineAPIURL = _appConfiguration.GetSection("ConnectionStrings").GetChildren().Where(x => x.Key == "EngineAPIURL").FirstOrDefault().Value;
            SettingsModel.ConnectionStrings = _appConfiguration.GetSection("ConnectionStrings").GetChildren().Where(x => x.Key == "Default").FirstOrDefault().Value;

            AppSettingsModel.BlobServiceConnectionStrings = _appConfiguration.GetSection("BlobService").GetChildren().Where(x => x.Key == "ConnectionString").FirstOrDefault().Value;
            SettingsModel.BlobServiceConnectionStrings = _appConfiguration.GetSection("BlobService").GetChildren().Where(x => x.Key == "ConnectionString").FirstOrDefault().Value;

            AppSettingsModel.GoogleMapsKey = _appConfiguration.GetSection("GoogleMapsKey").GetChildren().Where(x => x.Key == "KeyMap").FirstOrDefault().Value;
            SettingsModel.GoogleMapsKey = _appConfiguration.GetSection("GoogleMapsKey").GetChildren().Where(x => x.Key == "KeyMap").FirstOrDefault().Value;


            AppSettingsModel.TranslateKey = _appConfiguration.GetSection("TranslateKey").GetChildren().Where(x => x.Key == "KeyT").FirstOrDefault().Value;


            AppSettingsCoreModel.ConnectionStrings = _appConfiguration.GetSection("ConnectionStrings").GetChildren().Where(x => x.Key == "Default").FirstOrDefault().Value;

            AppSettingsCoreModel.BlobServiceConnectionStrings = _appConfiguration.GetSection("BlobService").GetChildren().Where(x => x.Key == "ConnectionString").FirstOrDefault().Value;

            AppSettingsCoreModel.StorageServiceURL = _appConfiguration.GetSection("BlobService").GetChildren().Where(x => x.Key == "URL").FirstOrDefault().Value;


            AppSettingsCoreModel.GoogleMapsKey = _appConfiguration.GetSection("GoogleMapsKey").GetChildren().Where(x => x.Key == "KeyMap").FirstOrDefault().Value;


            AppSettingsModel.AddHour = int.Parse(_appConfiguration.GetSection("InfoSeedTime").GetChildren().Where(x => x.Key == "AddHour").FirstOrDefault().Value);
            AppSettingsModel.DivHour = int.Parse(_appConfiguration.GetSection("InfoSeedTime").GetChildren().Where(x => x.Key == "DivHour").FirstOrDefault().Value);



            SettingsModel.AzureWebJobsStorage = _appConfiguration.GetSection("AzureWebJobsStorage").GetChildren().Where(x => x.Key == "KeyA").FirstOrDefault().Value;
            AppSettingsCoreModel.AzureWebJobsStorage = _appConfiguration.GetSection("AzureWebJobsStorage").GetChildren().Where(x => x.Key == "KeyA").FirstOrDefault().Value;

            FcmNotificationSetting.SenderId = _appConfiguration.GetSection("FcmNotification").GetChildren().Where(x => x.Key == "SenderId").FirstOrDefault().Value;
            FcmNotificationSetting.ServerKey = _appConfiguration.GetSection("FcmNotification").GetChildren().Where(x => x.Key == "ServerKey").FirstOrDefault().Value;
            FcmNotificationSetting.webAddr = _appConfiguration.GetSection("FcmNotification").GetChildren().Where(x => x.Key == "webAddr").FirstOrDefault().Value;



            Models.AppsettingsModel.StorageServiceConnectionStrings = _appConfiguration.GetSection("BlobService").GetChildren().Where(x => x.Key == "ConnectionString").FirstOrDefault().Value;
            Models.AppsettingsModel.StorageServiceURL = _appConfiguration.GetSection("BlobService").GetChildren().Where(x => x.Key == "URL").FirstOrDefault().Value;



            AppSettingsCoreModel.SocketIOURL = _appConfiguration.GetSection("SocketIO").GetChildren().Where(x => x.Key == "URL").FirstOrDefault().Value;
            AppSettingsCoreModel.SocketIOToken = _appConfiguration.GetSection("SocketIO").GetChildren().Where(x => x.Key == "Token").FirstOrDefault().Value;

            SettingsModel.MgKey=_appConfiguration.GetSection("MgMotor").GetChildren().Where(x => x.Key == "Key").FirstOrDefault().Value;
            SettingsModel.MgUrl=_appConfiguration.GetSection("MgMotor").GetChildren().Where(x => x.Key == "baseUrl").FirstOrDefault().Value;



            AppSettingsModel.connectionStringMongoDB = _appConfiguration.GetSection("MongoDB").GetChildren().Where(x => x.Key == "connectionStringMongoDB").FirstOrDefault().Value;
            AppSettingsModel.databaseName = _appConfiguration.GetSection("MongoDB").GetChildren().Where(x => x.Key == "databaseName").FirstOrDefault().Value;
            AppSettingsModel.collectionName = _appConfiguration.GetSection("MongoDB").GetChildren().Where(x => x.Key == "collectionName").FirstOrDefault().Value;
            AppSettingsModel.urlSendCampaignProject = _appConfiguration.GetSection("MongoDB").GetChildren().Where(x => x.Key == "urlSendCampaignProject").FirstOrDefault().Value;
             
            AppSettingsModel.googleSheetClientId = _appConfiguration.GetSection("GoogleAuth").GetChildren().Where(x => x.Key == "ClientId").FirstOrDefault().Value;
            AppSettingsModel.googleSheetClientSecret = _appConfiguration.GetSection("GoogleAuth").GetChildren().Where(x => x.Key == "ClientSecret").FirstOrDefault().Value;
            AppSettingsModel.googleSheetRedirectUri = _appConfiguration.GetSection("GoogleAuth").GetChildren().Where(x => x.Key == "RedirectUri").FirstOrDefault().Value;


            string cosmoEndPoin = _appConfiguration.GetSection("CosmosDBSettings").GetChildren().Where(x => x.Key == "EndPoint").FirstOrDefault().Value;
            string cosmoMasterKey = _appConfiguration.GetSection("CosmosDBSettings").GetChildren().Where(x => x.Key == "AuthKey").FirstOrDefault().Value;
           // AppSettingsModel.BotApi = _appConfiguration.GetSection("Bot").GetChildren().Where(x => x.Key == "Api").FirstOrDefault().Value;


            services.AddSwaggerGen();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo() { Title = "MessagingPortal API", Version = "v1" });
                ////   options.SwaggerDoc(name: "V1", new OpenApiInfo { Title = "My API", Version = "V1" });
                //options.DocInclusionPredicate((docName, description) => true);
                //options.ParameterFilter<SwaggerEnumParameterFilter>();
                //options.SchemaFilter<SwaggerEnumSchemaFilter>();
                //options.OperationFilter<SwaggerOperationIdFilter>();
                //options.OperationFilter<SwaggerOperationFilter>();
                //options.CustomDefaultSchemaIdSelector();
            }).AddSwaggerGenNewtonsoftSupport();

            services.AddSignalR();

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

            services.AddScoped<IContactsAPI, ContactsAPI>();
            services.AddScoped<ITicketsAPI, TicketsAPI>();
            services.AddScoped<IGeneralAppService, GeneralAppService>();
            services.AddScoped<IExcelExporterAppService, ExcelExporterAppService>();
            services.AddScoped<IDepartmentAppService, DepartmentAppService>();
            services.AddScoped<IBookingAppService, BookingAppService>();

            services.AddScoped<IInvoices, InvoicesApi>();
            services.AddScoped<ICaptionBotAppService, CaptionBotAppService>();


            services.AddScoped<IBotApis, NewBotAPIController>();

            //services.AddScoped<IBotFlows, BotFlowAPIController>();

            services.AddScoped<IWhatsAppMessageTemplateAppService, WhatsAppMessageTemplateAppService>();

            services.AddSingleton<IDocumentClient>(x => new DocumentClient(new Uri(cosmoEndPoin), cosmoMasterKey, new ConnectionPolicy { EnableEndpointDiscovery = false }));


            services.AddScoped<ICustomerBehaviourAppService, CustomerBehaviourAppService>();
            services.AddControllersWithViews(options =>
            {
                options.Filters.Add(new AbpAutoValidateAntiforgeryTokenAttribute());
            }).AddNewtonsoftJson();

            services.AddApplicationInsightsTelemetry(_appConfiguration["ApplicationInsights:InstrumentationKey"]);

            services.AddAutoMapper(typeof(InfoSeedBotAPIAutoMapping.AutoMapping));
            SocketIOManager.Init();


            return services.AddAbp<MessagingPortalBotAPIModule>(options =>
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

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        //public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        //{

        //    app.UseAbp(options =>
        //    {
        //        options.UseAbpRequestLocalization = false; //used below: UseAbpRequestLocalization
        //    });
        //    if (env.IsDevelopment())
        //    {
        //        app.UseDeveloperExceptionPage();
        //    }

        //    app.UseHttpsRedirection();

        //    app.UseRouting();

        //    app.UseAuthorization();
        //    app.UseApiResponseAndExceptionWrapper();

        //    app.UseEndpoints(endpoints =>
        //    {
        //        endpoints.MapControllers();
        //    });


        //    app.UseSwagger();
        //    app.UseSwaggerUI(c => {
        //        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My Test1 Api v1");
             
        //    });

        //    //app.UseSwagger();
        //    //// Enable middleware to serve swagger-ui assets (HTML, JS, CSS etc.)

        //    //app.UseSwaggerUI(options =>
        //    //{
        //    //    options.SwaggerEndpoint(_appConfiguration["App:SwaggerEndPoint"], "MessagingPortal API V1");
        //    //    options.IndexStream = () => Assembly.GetExecutingAssembly()
        //    //        .GetManifestResourceStream("Infoseed.MessagingPortal.Web.wwwroot.swagger.ui.index.html");
        //    //    options.InjectBaseUrl(_appConfiguration["App:ServerRootAddress"]);
        //    //});
        //}


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
                   // app.UseHealthChecksUI();
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
