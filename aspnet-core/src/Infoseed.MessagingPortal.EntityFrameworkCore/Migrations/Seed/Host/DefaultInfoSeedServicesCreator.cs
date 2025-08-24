using System.Collections.Generic;
using System.Linq;
using Abp.Localization;
using Microsoft.EntityFrameworkCore;
using Infoseed.MessagingPortal.EntityFrameworkCore;
using Infoseed.MessagingPortal.InfoSeedServices;
using System;

namespace Infoseed.MessagingPortal.Migrations.Seed.Host
{
    public class DefaultInfoSeedServicesCreator
    {
        public static List<InfoSeedService> InitialInfoSeedServices => GetInitialInfoSeedServices();

        private readonly MessagingPortalDbContext _context;

        private static List<InfoSeedService> GetInitialInfoSeedServices()
        {
            var tenantId = MessagingPortalConsts.MultiTenancyEnabled ? null : (int?)1;
            return new List<InfoSeedService>
            {
                new InfoSeedService(1,"Sign up fees",DateTime.Now,DateTime.Now,"Used for setting up the Team inbox for a client",1,1,1,1,"001",0,0),
                new InfoSeedService(1,"Platform subscription fees",DateTime.Now,DateTime.Now,"Charged on quarterly bases in advance",1,1,2,1,"002",0,0),
                new InfoSeedService(1,"Conversation fees per month",DateTime.Now,DateTime.Now,"Based on how many conversations (customers) chatted per month",2,1,3,1,"003",0,0),
                new InfoSeedService(1,"Chatbot fees per message",DateTime.Now,DateTime.Now,"Based on how many conversations (customers) chatted per month",2,1,3,1,"004",0,0),
                new InfoSeedService(1,"Transaction related Charges",DateTime.Now,DateTime.Now,"Based on how many conversations (customers) chatted per month",2,1,3,1,"005",0,0),
            };
        }

        public DefaultInfoSeedServicesCreator(MessagingPortalDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateInfoSeedServices();
        }

        private void CreateInfoSeedServices()
        {
            foreach (var InfoSeedService in InitialInfoSeedServices)
            {
                AddInfoSeedServiceIfNotExists(InfoSeedService);
            }
        }

        private void AddInfoSeedServiceIfNotExists(InfoSeedService InfoSeedService)
        {
            if (_context.InfoSeedServices.IgnoreQueryFilters().Any(l => l.ServiceName == InfoSeedService.ServiceName))
            {
                return;
            }

            var fre = _context.ServiceFrquencies.First(p => p.ServiceFrequencyName == "Monthly");
            var serviceType = _context.ServiceTypes.First(p => p.ServicetypeName == "Advance");
            var status = _context.ServiceStatuses.First(p => p.ServiceStatusName == "Active");

            InfoSeedService.ServiceFrquencyId = fre.Id;
            InfoSeedService.ServiceStatusId = status.Id;
            InfoSeedService.ServiceTypeId = serviceType.Id;

            _context.InfoSeedServices.Add(InfoSeedService);

            _context.SaveChanges();
        }
    }
}