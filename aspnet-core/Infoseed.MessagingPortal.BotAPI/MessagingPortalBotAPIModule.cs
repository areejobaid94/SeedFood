using System.Collections.Generic;
using Abp.AspNetZeroCore;
using Abp.AspNetZeroCore.Web.Authentication.External;
using Abp.AspNetZeroCore.Web.Authentication.External.Facebook;
using Abp.AspNetZeroCore.Web.Authentication.External.Google;
using Abp.AspNetZeroCore.Web.Authentication.External.Microsoft;
using Abp.AspNetZeroCore.Web.Authentication.External.OpenIdConnect;
using Abp.AspNetZeroCore.Web.Authentication.External.WsFederation;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Threading.BackgroundWorkers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Infoseed.MessagingPortal.Auditing;
using Infoseed.MessagingPortal.Configuration;
using Infoseed.MessagingPortal.EntityFrameworkCore;
using Infoseed.MessagingPortal.MultiTenancy;
using Abp.Runtime.Caching.Redis;
using Microsoft.Extensions.Hosting;
using Infoseed.MessagingPortal.Web;

namespace Infoseed.MessagingPortal.BotAPI
{
    [DependsOn(
        typeof(MessagingPortalWebCoreModule)
        ,typeof(AbpRedisCacheModule)
    )]
    public class MessagingPortalBotAPIModule : AbpModule
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public MessagingPortalBotAPIModule(
            IWebHostEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void PreInitialize()
        {
            Configuration.Modules.AbpWebCommon().MultiTenancy.DomainFormat = _appConfiguration["App:ServerRootAddress"] ?? "https://localhost:44301/";
            Configuration.Modules.AspNetZero().LicenseCode = _appConfiguration["AbpZeroLicenseCode"];

            Configuration.Auditing.IsEnabled = false;// Audai.Jmaanai : this code to stop Auditig

            if (!_env.IsDevelopment())
            {
                Configuration.Caching.UseRedis(options =>
            {
                options.ConnectionString = _appConfiguration["RedisCache:ConnectionString"];
                options.DatabaseId = _appConfiguration.GetValue<int>("RedisCache:DatabaseId");
            });
            }
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(MessagingPortalBotAPIModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            using (var scope = IocManager.CreateScope())
            {
                if (!scope.Resolve<DatabaseCheckHelper>().Exist(_appConfiguration["ConnectionStrings:Default"]))
                {
                    return;
                }
            }

            var workManager = IocManager.Resolve<IBackgroundWorkerManager>();
            if (IocManager.Resolve<IMultiTenancyConfig>().IsEnabled)
            {
                //workManager.Add(IocManager.Resolve<SubscriptionExpirationCheckWorker>());
                //workManager.Add(IocManager.Resolve<SubscriptionExpireEmailNotifierWorker>());
            }

            if (Configuration.Auditing.IsEnabled && ExpiredAuditLogDeleterWorker.IsEnabled)
            {
               // workManager.Add(IocManager.Resolve<ExpiredAuditLogDeleterWorker>());
            }

      
        }

    }
}