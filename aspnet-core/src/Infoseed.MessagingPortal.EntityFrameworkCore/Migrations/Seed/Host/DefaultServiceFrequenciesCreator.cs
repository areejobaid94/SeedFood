using System.Collections.Generic;
using System.Linq;
using Abp.Localization;
using Microsoft.EntityFrameworkCore;
using Infoseed.MessagingPortal.EntityFrameworkCore;
using Infoseed.MessagingPortal.ServiceFrequencies;
using System;

namespace Infoseed.MessagingPortal.Migrations.Seed.Host
{
    public class DefaultServiceFrequenciesCreator
    {
        public static List<ServiceFrquency> InitialServiceFrequencies => GetInitialServiceFrequencies();

        private readonly MessagingPortalDbContext _context;

        private static List<ServiceFrquency> GetInitialServiceFrequencies()
        {
            var tenantId = MessagingPortalConsts.MultiTenancyEnabled ? null : (int?)1;
            return new List<ServiceFrquency>
            {
                new ServiceFrquency("One time payment", true,DateTime.Now),
                new ServiceFrquency("Quarterly", true,DateTime.Now),
                new ServiceFrquency("Monthly", true,DateTime.Now)

            };
        }
        private static List<string> GetGeenratedServiceId()
        {
            List<string> vs = new List<string>();
            return vs;
        }

        public DefaultServiceFrequenciesCreator(MessagingPortalDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateServiceFrequencys();
        }

        private void CreateServiceFrequencys()
        {
            foreach (var ServiceFrequency in InitialServiceFrequencies)
            {
                AddServiceFrequencyIfNotExists(ServiceFrequency);
            }
        }

        private void AddServiceFrequencyIfNotExists(ServiceFrquency ServiceFrequency)
        {
            if (_context.ServiceFrquencies.IgnoreQueryFilters().Any(l => l.ServiceFrequencyName == ServiceFrequency.ServiceFrequencyName))
            {
                return;
            }

            _context.ServiceFrquencies.Add(ServiceFrequency);

            _context.SaveChanges();
        }
    }
}