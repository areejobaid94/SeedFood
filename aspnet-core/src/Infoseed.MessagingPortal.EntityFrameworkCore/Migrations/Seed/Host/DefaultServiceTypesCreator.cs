using System.Collections.Generic;
using System.Linq;
using Abp.Localization;
using Microsoft.EntityFrameworkCore;
using Infoseed.MessagingPortal.EntityFrameworkCore;
using Infoseed.MessagingPortal.ServiceTypes;
using System;

namespace Infoseed.MessagingPortal.Migrations.Seed.Host
{
    public class DefaultServiceTypesCreator
    {
        public static List<ServiceType> InitialServiceTypes => GetInitialServiceTypes();

        private readonly MessagingPortalDbContext _context;

        private static List<ServiceType> GetInitialServiceTypes()
        {
            var tenantId = MessagingPortalConsts.MultiTenancyEnabled ? null : (int?)1;
            return new List<ServiceType>
            {
                new ServiceType("Advance", true,DateTime.Now),
                new ServiceType("End of month", true,DateTime.Now),
                new ServiceType("Per scope", true,DateTime.Now)

            };
        }

        public DefaultServiceTypesCreator(MessagingPortalDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateServiceTypes();
        }

        private void CreateServiceTypes()
        {
            foreach (var ServiceType in InitialServiceTypes)
            {
                AddServiceTypeIfNotExists(ServiceType);
            }
        }

        private void AddServiceTypeIfNotExists(ServiceType ServiceType)
        {
            if (_context.ServiceTypes.IgnoreQueryFilters().Any(l => l.ServicetypeName == ServiceType.ServicetypeName))
            {
                return;
            }

            _context.ServiceTypes.Add(ServiceType);

            _context.SaveChanges();
        }
    }
}