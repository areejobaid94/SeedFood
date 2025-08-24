using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Infoseed.MessagingPortal.EntityFrameworkCore;

namespace Infoseed.MessagingPortal.HealthChecks
{
    public class MessagingPortalDbContextHealthCheck : IHealthCheck
    {
        private readonly DatabaseCheckHelper _checkHelper;

        public MessagingPortalDbContextHealthCheck(DatabaseCheckHelper checkHelper)
        {
            _checkHelper = checkHelper;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            if (_checkHelper.Exist("db"))
            {
                return Task.FromResult(HealthCheckResult.Healthy("MessagingPortalDbContext connected to database."));
            }

            return Task.FromResult(HealthCheckResult.Unhealthy("MessagingPortalDbContext could not connect to database"));
        }
    }
}
