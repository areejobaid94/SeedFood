using System.Collections.Generic;
using System.Linq;
using Abp.Localization;
using Microsoft.EntityFrameworkCore;
using Infoseed.MessagingPortal.EntityFrameworkCore;
using Infoseed.MessagingPortal.ServiceStatuses;
using System;

namespace Infoseed.MessagingPortal.Migrations.Seed.Host
{
    public class DefaultServiceStatusesCreator
    {
        public static List<ServiceStatus> InitialServiceStatuss => GetInitialServiceStatuss();

        private readonly MessagingPortalDbContext _context;

        private static List<ServiceStatus> GetInitialServiceStatuss()
        {
            var tenantId = MessagingPortalConsts.MultiTenancyEnabled ? null : (int?)1;
            return new List<ServiceStatus>
            {
                new ServiceStatus("Active", true,DateTime.Now),
                new ServiceStatus("Inactive", true,DateTime.Now)
            };
        }

        public DefaultServiceStatusesCreator(MessagingPortalDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateServiceStatuss();
        }

        private void CreateServiceStatuss()
        {
            foreach (var ServiceStatus in InitialServiceStatuss)
            {
                AddServiceStatusIfNotExists(ServiceStatus);
            }
        }

        private void AddServiceStatusIfNotExists(ServiceStatus ServiceStatus)
        {
            if (_context.ServiceStatuses.IgnoreQueryFilters().Any(l => l.ServiceStatusName == ServiceStatus.ServiceStatusName))
            {
                return;
            }

            _context.ServiceStatuses.Add(ServiceStatus);

            _context.SaveChanges();
        }
    }
}