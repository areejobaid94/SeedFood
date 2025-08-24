using Microsoft.Extensions.DependencyInjection;
using Infoseed.MessagingPortal.HealthChecks;

namespace Infoseed.MessagingPortal.Web.HealthCheck
{
    public static class AbpZeroHealthCheck
    {
        public static IHealthChecksBuilder AddAbpZeroHealthCheck(this IServiceCollection services)
        {
            var builder = services.AddHealthChecks();
            builder.AddCheck<MessagingPortalDbContextHealthCheck>("Database Connection");
            builder.AddCheck<MessagingPortalDbContextUsersHealthCheck>("Database Connection with user check");
            builder.AddCheck<CacheHealthCheck>("Cache");

            // add your custom health checks here
            // builder.AddCheck<MyCustomHealthCheck>("my health check");

            return builder;
        }
    }
}
