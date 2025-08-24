using Abp.AspNetCore;
using Abp.AspNetCore.Mvc.Antiforgery;
using Abp.AspNetZeroCore.Web.Authentication.JwtBearer;
using Abp.Castle.Logging.Log4Net;
using Abp.Extensions;
using Abp.PlugIns;
using Castle.Facilities.Logging;
using Framework.Payment.Implementation.Zoho;
using Framework.Payment.Interfaces.Zoho;
using GraphQL.Server;
using GraphQL.Server.Ui.Playground;
using HealthChecks.UI.Client;
using IdentityServer4.Configuration;
using Infoseed.MessagingPortal.Areas;
using Infoseed.MessagingPortal.Asset;
using Infoseed.MessagingPortal.Authorization.Users;
using Infoseed.MessagingPortal.booking;
using Infoseed.MessagingPortal.Booking;
using Infoseed.MessagingPortal.Configuration;
using Infoseed.MessagingPortal.Configure;
using Infoseed.MessagingPortal.Contacts;
using Infoseed.MessagingPortal.DashboardUI;
using Infoseed.MessagingPortal.DeliveryCost;
using Infoseed.MessagingPortal.Departments;
using Infoseed.MessagingPortal.EntityFrameworkCore;
using Infoseed.MessagingPortal.Facebook;
using Infoseed.MessagingPortal.FacebookDTO;
using Infoseed.MessagingPortal.General;
using Infoseed.MessagingPortal.Identity;
using Infoseed.MessagingPortal.ItemAdditions;
using Infoseed.MessagingPortal.Items;
using Infoseed.MessagingPortal.LiveChat;
using Infoseed.MessagingPortal.LiveChat.Exporting;
using Infoseed.MessagingPortal.Location;
using Infoseed.MessagingPortal.MenuCategories;
using Infoseed.MessagingPortal.Menus;
using Infoseed.MessagingPortal.Orders;
using Infoseed.MessagingPortal.Schemas;
using Infoseed.MessagingPortal.SealingReuest;
using Infoseed.MessagingPortal.SellingRequest;
using Infoseed.MessagingPortal.SocketIOClient;
using Infoseed.MessagingPortal.Web.Common;
using Infoseed.MessagingPortal.Web.HealthCheck;
using Infoseed.MessagingPortal.Web.Host;
using Infoseed.MessagingPortal.Web.Host.Gateway;
using Infoseed.MessagingPortal.Web.IdentityServer;
using Infoseed.MessagingPortal.Web.Models;
using Infoseed.MessagingPortal.Web.Models.Firebase;
using Infoseed.MessagingPortal.Web.Sunshine;
using Infoseed.MessagingPortal.Web.Swagger;
using Infoseed.MessagingPortal.WhatsApp;
using Infoseed.MessagingPortal.Zoho;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
//using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Owl.reCAPTCHA;
using Sentry.AspNetCore;
using Sentry.Extensibility;
using SocketIOClient.Processors;
using Stripe;
using System;
//using System.Collections.Generic;
//using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using HealthChecksUISettings = HealthChecks.UI.Configuration.Settings;
using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;

namespace Infoseed.MessagingPortal.Web.Startup
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private const string DefaultCorsPolicyName = "localhost";

        private readonly IConfigurationRoot _appConfiguration;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _hostingEnvironment = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {

            //// Configure English culture (default)
            //var enCulture = new CultureInfo("en-US");
            //CultureInfo.DefaultThreadCurrentCulture = enCulture;
            //CultureInfo.DefaultThreadCurrentUICulture = enCulture;
            //services.AddSingleton(enCulture);

            //// Configure Arabic culture
            //var arCulture = new CultureInfo("ar");
            //services.AddSingleton(arCulture);

            //// Add localization support
            //services.AddLocalization(options => options.ResourcesPath = "Resources");


            // services.Configure<IISServerOptions>(options =>
            // {
            //     options.MaxRequestBodySize = 104857600; // Example: 100 MB
            // });

            // services.Configure<KestrelServerOptions>(options =>
            // {
            //     options.Limits.MaxRequestBodySize = 104857600; // Example: 100 MB
            // });

            services.Configure<GatewayApiConfig>(Configuration.GetSection("GatewayApiConfig"));

            services.AddSingleton<IConfiguration>(_appConfiguration);
            Models.AppsettingsModel.AttacmentTypesAllowed = _appConfiguration.GetSection("AllowedAttacmentTypes").GetChildren().ToDictionary(x => x.Key, x => x.Value);
            Models.AppsettingsModel.AttacmentTypesAllowedM = _appConfiguration.GetSection("AllowedAttacmentTypesM").GetChildren().ToDictionary(x => x.Key, x => x.Value);



            AppSettingsModel.ConnectionStrings = _appConfiguration.GetSection("ConnectionStrings").GetChildren().Where(x => x.Key == "Default").FirstOrDefault().Value;
            AppSettingsModel.EngineAPIURL = _appConfiguration.GetSection("ConnectionStrings").GetChildren().Where(x => x.Key == "EngineAPIURL").FirstOrDefault().Value;

            SettingsModel.ConnectionStrings = _appConfiguration.GetSection("ConnectionStrings").GetChildren().Where(x => x.Key == "Default").FirstOrDefault().Value;


            AppSettingsModel.BlobServiceConnectionStrings = _appConfiguration.GetSection("BlobService").GetChildren().Where(x => x.Key == "ConnectionString").FirstOrDefault().Value;
            SettingsModel.BlobServiceConnectionStrings = _appConfiguration.GetSection("BlobService").GetChildren().Where(x => x.Key == "ConnectionString").FirstOrDefault().Value;

            AppSettingsModel.GoogleMapsKey = _appConfiguration.GetSection("GoogleMapsKey").GetChildren().Where(x => x.Key == "KeyMap").FirstOrDefault().Value;
            SettingsModel.GoogleMapsKey = _appConfiguration.GetSection("GoogleMapsKey").GetChildren().Where(x => x.Key == "KeyMap").FirstOrDefault().Value;


            AppSettingsModel.TranslateKey = _appConfiguration.GetSection("TranslateKey").GetChildren().Where(x => x.Key == "KeyT").FirstOrDefault().Value;


            AppSettingsCoreModel.ConnectionStrings = _appConfiguration.GetSection("ConnectionStrings").GetChildren().Where(x => x.Key == "Default").FirstOrDefault().Value;


            AppSettingsCoreModel.StorageServiceURL = _appConfiguration.GetSection("BlobService").GetChildren().Where(x => x.Key == "URL").FirstOrDefault().Value;

            AppSettingsCoreModel.BlobServiceConnectionStrings = _appConfiguration.GetSection("BlobService").GetChildren().Where(x => x.Key == "ConnectionString").FirstOrDefault().Value;

            AppSettingsCoreModel.GoogleMapsKey = _appConfiguration.GetSection("GoogleMapsKey").GetChildren().Where(x => x.Key == "KeyMap").FirstOrDefault().Value;


            AppSettingsModel.AddHour = int.Parse(_appConfiguration.GetSection("InfoSeedTime").GetChildren().Where(x => x.Key == "AddHour").FirstOrDefault().Value);
            AppSettingsModel.DivHour = int.Parse(_appConfiguration.GetSection("InfoSeedTime").GetChildren().Where(x => x.Key == "DivHour").FirstOrDefault().Value);

            AppSettingsModel.BotApi = _appConfiguration.GetSection("Bot").GetChildren().Where(x => x.Key == "Api").FirstOrDefault().Value;


            SettingsModel.AzureWebJobsStorage = _appConfiguration.GetSection("AzureWebJobsStorage").GetChildren().Where(x => x.Key == "KeyA").FirstOrDefault().Value;
            AppSettingsCoreModel.AzureWebJobsStorage = _appConfiguration.GetSection("AzureWebJobsStorage").GetChildren().Where(x => x.Key == "KeyA").FirstOrDefault().Value;

            FcmNotificationSetting.SenderId= _appConfiguration.GetSection("FcmNotification").GetChildren().Where(x => x.Key == "SenderId").FirstOrDefault().Value;
            FcmNotificationSetting.ServerKey = _appConfiguration.GetSection("FcmNotification").GetChildren().Where(x => x.Key == "ServerKey").FirstOrDefault().Value;
            FcmNotificationSetting.webAddr = _appConfiguration.GetSection("FcmNotification").GetChildren().Where(x => x.Key == "webAddr").FirstOrDefault().Value;



            Models.AppsettingsModel.StorageServiceConnectionStrings = _appConfiguration.GetSection("BlobService").GetChildren().Where(x => x.Key == "ConnectionString").FirstOrDefault().Value;
            Models.AppsettingsModel.StorageServiceURL = _appConfiguration.GetSection("BlobService").GetChildren().Where(x => x.Key == "URL").FirstOrDefault().Value;



            AppSettingsCoreModel.SocketIOURL = _appConfiguration.GetSection("SocketIO").GetChildren().Where(x => x.Key == "URL").FirstOrDefault().Value;
            AppSettingsCoreModel.SocketIOToken = _appConfiguration.GetSection("SocketIO").GetChildren().Where(x => x.Key == "Token").FirstOrDefault().Value;


            AppSettingsModel.connectionStringMongoDB = _appConfiguration.GetSection("MongoDB").GetChildren().Where(x => x.Key == "connectionStringMongoDB").FirstOrDefault().Value;
            AppSettingsModel.databaseName = _appConfiguration.GetSection("MongoDB").GetChildren().Where(x => x.Key == "databaseName").FirstOrDefault().Value;
            AppSettingsModel.collectionName = _appConfiguration.GetSection("MongoDB").GetChildren().Where(x => x.Key == "collectionName").FirstOrDefault().Value;
            AppSettingsModel.urlSendCampaignProject = _appConfiguration.GetSection("MongoDB").GetChildren().Where(x => x.Key == "urlSendCampaignProject").FirstOrDefault().Value;

            AppSettingsModel.googleSheetClientId = _appConfiguration.GetSection("GoogleAuth").GetChildren().Where(x => x.Key == "ClientId").FirstOrDefault().Value;
            AppSettingsModel.googleSheetClientSecret = _appConfiguration.GetSection("GoogleAuth").GetChildren().Where(x => x.Key == "ClientSecret").FirstOrDefault().Value;
            AppSettingsModel.googleSheetRedirectUri = _appConfiguration.GetSection("GoogleAuth").GetChildren().Where(x => x.Key == "RedirectUri").FirstOrDefault().Value;



            AppSettingsModel.EngineApi = _appConfiguration.GetSection("engineapi").GetChildren().Where(x => x.Key == "Api").FirstOrDefault().Value;




            ////services.AddControllers();
            //services.AddTransient<INotificationService, NotificationService>();
            //services.AddHttpClient<FcmSender>();
            //services.AddHttpClient<ApnSender>();



            // Configure strongly typed settings objects
            //var appSettingsSection = _appConfiguration.GetSection("FcmNotification");
            //services.Configure<FcmNotificationSetting>(appSettingsSection);
            //services.Configure<FcmNotificationSetting>(_appConfiguration.GetSection("FcmNotification"));
            // Register the swagger generator
            //services.AddSwaggerGen(c => {
            //    c.SwaggerDoc(name: "V1", new OpenApiInfo { Title = "My API", Version = "V1" });
            //});


            //start Scheduler
            //var StartTime = _appConfiguration.GetSection("Schedulerjob").GetChildren().Where(x => x.Key == "StartTime").FirstOrDefault().Value;
            //var Hour = _appConfiguration.GetSection("Schedulerjob").GetChildren().Where(x => x.Key == "Hour").FirstOrDefault().Value;
            //SchedulerServices schedulerServices = new SchedulerServices();
            //schedulerServices.Strat(StartTime, Hour);


            // services.AddSingleton(apnSettings);
            // services.AddSingleton(appSettingsSection);
            //MVC
            services.AddControllersWithViews(options =>
            {
                options.Filters.Add(new AbpAutoValidateAntiforgeryTokenAttribute());
            }).AddNewtonsoftJson();

             services.AddSignalR();
            //services.AddSignalR().AddAzureSignalR(_appConfiguration.GetSection("AzureSignalR").GetChildren().First(p => p.Key == "ConnectionString").Value);

            //Configure CORS for angular2 UI
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

            if (bool.Parse(_appConfiguration["KestrelServer:IsEnabled"]))
            {
                ConfigureKestrel(services);
            }

            IdentityRegistrar.Register(services);
            AuthConfigurer.Configure(services, _appConfiguration);

            //Identity server
            if (bool.Parse(_appConfiguration["IdentityServer:IsEnabled"]))
            {
                IdentityServerRegistrar.Register(services, _appConfiguration, options =>
                     options.UserInteraction = new UserInteractionOptions()
                     {
                         LoginUrl = "/UI/Login",
                         LogoutUrl = "/UI/LogOut",
                         ErrorUrl = "/Error"
                     });
            }

            if (WebConsts.SwaggerUiEnabled)
            {
                //Swagger - Enable this line and the related lines in Configure method to enable swagger UI
                services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo() { Title = "MessagingPortal API", Version = "v1" });
                    //   options.SwaggerDoc(name: "V1", new OpenApiInfo { Title = "My API", Version = "V1" });
                    options.DocInclusionPredicate((docName, description) => true);
                    options.ParameterFilter<SwaggerEnumParameterFilter>();
                    options.SchemaFilter<SwaggerEnumSchemaFilter>();
                    options.OperationFilter<SwaggerOperationIdFilter>();
                    options.OperationFilter<SwaggerOperationFilter>();
                    options.CustomDefaultSchemaIdSelector();
                }).AddSwaggerGenNewtonsoftSupport();
            }

            //Recaptcha
            services.AddreCAPTCHAV3(x =>
            {
                x.SiteKey = _appConfiguration["Recaptcha:SiteKey"];
                x.SiteSecret = _appConfiguration["Recaptcha:SecretKey"];
            });

            services.AddScoped<IDBService, CosmosDBService>();
            services.AddScoped<IMenusAppService, MenusAppService>();
            services.AddScoped<IItemsAppService, ItemsAppService>();
            services.AddScoped<IItemAdditionAppService, ItemAdditionAppService>();
            services.AddScoped<IAreasAppService, AreasAppService>();
            services.AddScoped<IUserAppService, UserAppService>();
            services.AddScoped<ISellingRequestAppService, SellingRequestAppService>();
            services.AddScoped<IOrdersAppService, OrdersAppService>();
            services.AddScoped<IDeliveryCostAppService, DeliveryCostAppService>();
            services.AddScoped<IAssetAppService, AssetAppService>();
            services.AddScoped<IContactsAppService, ContactsAppService>();
            services.AddScoped<IMenuCategoriesAppService, ItemCategoryAppService>();
            services.AddScoped<ILiveChatAppService, LiveChatAppService>();
            services.AddScoped<ILiveChatExcelExporter, LivChatExcelExporter>();
            services.AddScoped<IWhatsAppMessageTemplateAppService, WhatsAppMessageTemplateAppService>();
            services.AddScoped<IWhatsAppConversationSessionAppService, WhatsAppConversationSessionAppService>();
            services.AddScoped<ICosmosAppService, CosmosAppService>();
            services.AddScoped<ILocationAppService, LocationAppService>();
            services.AddScoped<IDepartmentAppService, DepartmentAppService>();
            services.AddScoped<IBookingAppService, BookingAppService>();
            services.AddScoped<IDashboardUIAppService, DashboardUIAppService>();


            services.AddScoped<IFacebookAppService, FacebookAppService>();



            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 104857600; // 100 MB
            });
            services.AddControllers().AddJsonOptions(opts =>
            {
                opts.JsonSerializerOptions.MaxDepth = 64;
            });
            services.AddScoped<IZohoAppService, ZohoAppService>();
            
            services.AddScoped<IInvoices, InvoicesApi>();
            //services.AddSingleton<IUserFactory, MyUserFactory>();
            //services.AddTransient<ISentryEventProcessor, MyEventProcessor>();
            //services.AddScoped<ISentryEventExceptionProcessor, MyExceptionProcessor>();

            if (WebConsts.HangfireDashboardEnabled)
            {
                //Hangfire(Enable to use Hangfire instead of default job manager)
                //services.AddHangfire(config =>
                //{
                //    config.UseSqlServerStorage(_appConfiguration.GetConnectionString("Default"));
                //    using (var server = new BackgroundJobServer())
                //    {
                //        //var manager = new RecurringJobManager();
                //        //BillingGenerator
                //        // manager.AddOrUpdate<AccountBillingGenerator>("AccountBillingGenerator", m => m.Execute(AppSettingsModel.ConnectionStrings), Cron.Minutely());

                //        //manager.AddOrUpdate<BillingGenerator>("SealsUserUpdate", m => m.SealsUserUpdateSync(), Cron.Weekly());
                //        // manager.AddOrUpdate<BillingGenerator>("BillingGenerator", m => m.BillingUpdateSync(), Cron.Daily());

                //        // manager.AddOrUpdate<UpdateContactSync>("UpdateContactSync", m => m.UpdateConversationExpired(), Cron.Minutely);
                //       // manager.AddOrUpdate<TenantSyncService>("TenantSyncService", m => m.Sync(AppSettingsModel.ConnectionStrings), Cron.Minutely());
                //       // manager.AddOrUpdate<SessionTimeOutSync>("SessionTimeOutSync", m => m.SessionTimeOut(), Cron.Minutely());
                //       // manager.AddOrUpdate<EvaluationQuestion>("EvaluationQuestionSync", m => m.evaluationQuestion(), Cron.Minutely());

                //    }
                //});

            }

            if (WebConsts.GraphQL.Enabled)
            {
                services.AddAndConfigureGraphQL();
            }

            if (bool.Parse(_appConfiguration["HealthChecks:HealthChecksEnabled"]))
            {
                services.AddAbpZeroHealthCheck();

                var healthCheckUISection = _appConfiguration.GetSection("HealthChecks")?.GetSection("HealthChecksUI");

                if (bool.Parse(healthCheckUISection["HealthChecksUIEnabled"]))
                {
                    services.Configure<HealthChecksUISettings>(settings =>
                    {
                        healthCheckUISection.Bind(settings, c => c.BindNonPublicProperties = true);
                    });
                    services.AddHealthChecksUI()
                        .AddInMemoryStorage();
                }
            }

            //if (!_hostingEnvironment.IsDevelopment())
           // {
                 services.AddApplicationInsightsTelemetry(_appConfiguration["ApplicationInsights:InstrumentationKey"]);
            // }

            string cosmoEndPoin = _appConfiguration.GetSection("CosmosDBSettings").GetChildren().Where(x => x.Key == "EndPoint").FirstOrDefault().Value;
            string cosmoMasterKey = _appConfiguration.GetSection("CosmosDBSettings").GetChildren().Where(x => x.Key == "AuthKey").FirstOrDefault().Value;


            services.AddSingleton<IDocumentClient>(x => new DocumentClient(new Uri(cosmoEndPoin), cosmoMasterKey, new ConnectionPolicy { EnableEndpointDiscovery = false }));

            services.AddSingleton<GatewayApiClient, GatewayApiClient>();
            services.AddSingleton<NVPApiClient, NVPApiClient>();

            SocketIOManager.Init();

            //Configure Abp and Dependency Injection
            return services.AddAbp<MessagingPortalWebHostModule>(options =>
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
            //// Add other middleware here

            //// Enable localization
            //app.UseRequestLocalization(new RequestLocalizationOptions
            //{
            //    DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en-US"),
            //    SupportedCultures = new[] { new CultureInfo("en-US"), new CultureInfo("ar") },
            //    SupportedUICultures = new[] { new CultureInfo("en-US"), new CultureInfo("ar") }
            //});

            //app.UseHttpsRedirection();
           

            //Initializes ABP framework.
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
                //app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseRouting();

            app.UseSentryTracing();

            app.UseCors(DefaultCorsPolicyName); //Enable CORS!

            app.UseAuthentication();
            app.UseJwtTokenMiddleware();

            if (bool.Parse(_appConfiguration["IdentityServer:IsEnabled"]))
            {
                app.UseJwtTokenMiddleware("IdentityBearer");
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

            if (WebConsts.HangfireDashboardEnabled)
            {
                //Hangfire dashboard &server(Enable to use Hangfire instead of default job manager)
                //app.UseHangfireDashboard(WebConsts.HangfireDashboardEndPoint, new DashboardOptions
                //{
                //    Authorization = new[] { new AbpHangfireAuthorizationFilter(AppPermissions.Pages_Administration_HangfireDashboard) }
                //});
                //app.UseHangfireServer();
            }

            if (bool.Parse(_appConfiguration["Payment:Stripe:IsActive"]))
            {
                StripeConfiguration.ApiKey = _appConfiguration["Payment:Stripe:SecretKey"];
            }

            if (WebConsts.GraphQL.Enabled)
            {
                app.UseGraphQL<MainSchema>();
                if (WebConsts.GraphQL.PlaygroundEnabled)
                {
                    app.UseGraphQLPlayground(
                        new GraphQLPlaygroundOptions()); //to explorer API navigate https://*DOMAIN*/ui/playground
                }
            }


            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapHub<Models.Notifications>("infoseednotificationhub");
              //  endpoints.MapHub<AbpCommonHub>("/signalr");
              //  endpoints.MapHub<ChatHub>("/signalr-chat");
               //endpoints.MapHub<SignalR.TeamInboxHub>("/teaminbox");
             //   endpoints.MapHub<OrderHub>("/order");
               // endpoints.MapHub<LiveChatHub>("/liveChat");
               // endpoints.MapHub<SellingRequestHub>("/sellingRequest");
                //endpoints.MapHub<MaintenancesHub>("/maintenances");
               // endpoints.MapHub<EvaluationHub>("/evaluation");
              //  endpoints.MapHub<SignalR.TestSingalRhub>("/testsingalr");
                endpoints.MapControllerRoute("defaultWithArea", "{area}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

                if (bool.Parse(_appConfiguration["HealthChecks:HealthChecksEnabled"]))
                {
                    endpoints.MapHealthChecks("/health", new HealthCheckOptions()
                    {
                        Predicate = _ => true,
                        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                    });
                }
            });

            if (bool.Parse(_appConfiguration["HealthChecks:HealthChecksEnabled"]))
            {
                if (bool.Parse(_appConfiguration["HealthChecks:HealthChecksUI:HealthChecksUIEnabled"]))
                {
                    app.UseHealthChecksUI();
                }
            }
       //     app.UseAzureSignalR(
       //        (options) =>
       //        {
       //            options.MapHub<AbpCommonHub>("/signalr");
       //            options.MapHub<ChatHub>("/signalr-chat");
       //            options.MapHub<SignalR.TeamInboxHub>("/teaminbox");
       //            options.MapHub<OrderHub>("/order");
       //            options.MapHub<MaintenancesHub>("/maintenances");
       //            options.MapHub<EvaluationHub>("/evaluation");
       //            options.MapHub<LiveChatHub>("/liveChat");
       //            options.MapHub<SellingRequestHub>("/sellingRequest");

       //        }
       //);
            if (WebConsts.SwaggerUiEnabled)
            {
                // Enable middleware to serve generated Swagger as a JSON endpoint
                app.UseSwagger();
                // Enable middleware to serve swagger-ui assets (HTML, JS, CSS etc.)

                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint(_appConfiguration["App:SwaggerEndPoint"], "MessagingPortal API V1");
                    options.IndexStream = () => Assembly.GetExecutingAssembly()
                        .GetManifestResourceStream("Infoseed.MessagingPortal.Web.wwwroot.swagger.ui.index.html");
                    options.InjectBaseUrl(_appConfiguration["App:ServerRootAddress"]);
                }); //URL: /swagger
            }



        }

        private void ConfigureKestrel(IServiceCollection services)
        {
            services.Configure<Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServerOptions>(options =>
            {
                options.Listen(new System.Net.IPEndPoint(System.Net.IPAddress.Any, 443),
                    listenOptions =>
                    {
                        var certPassword = _appConfiguration.GetValue<string>("Kestrel:Certificates:Default:Password");
                        var certPath = _appConfiguration.GetValue<string>("Kestrel:Certificates:Default:Path");
                        var cert = new System.Security.Cryptography.X509Certificates.X509Certificate2(certPath, certPassword);
                        listenOptions.UseHttps(new HttpsConnectionAdapterOptions()
                        {
                            ServerCertificate = cert
                        });
                    });
            });
        }
    }
}
