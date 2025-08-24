using System.Collections.Generic;
using System.Linq;
using Abp.Localization;
using Microsoft.EntityFrameworkCore;
using Infoseed.MessagingPortal.EntityFrameworkCore;
using Infoseed.MessagingPortal.TemplateMessagePurposes;

namespace Infoseed.MessagingPortal.Migrations.Seed.Host
{
    public class DefaultTemplateMessagePurposesCreator
    {
        public static List<TemplateMessagePurpose> InitialTemplateMessagePurposes => GetInitialTemplateMessagePurposes();

        private readonly MessagingPortalDbContext _context;

        private static List<TemplateMessagePurpose> GetInitialTemplateMessagePurposes()
        {
            var tenantId = MessagingPortalConsts.MultiTenancyEnabled ? null : (int?)1;
            return new List<TemplateMessagePurpose>
            {
                new TemplateMessagePurpose(){ Purpose = "General"}
            };
        }

        public DefaultTemplateMessagePurposesCreator(MessagingPortalDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateTemplateMessagePurposes();
        }

        private void CreateTemplateMessagePurposes()
        {
            foreach (var purpose in InitialTemplateMessagePurposes)
            {
                AddTemplateMessagePurposeIfNotExists(purpose);
            }
        }

        private void AddTemplateMessagePurposeIfNotExists(TemplateMessagePurpose purpose)
        {
            if (_context.TemplateMessagePurposes.IgnoreQueryFilters().Any(l => l.Purpose == purpose.Purpose))
            {
                return;
            }

            _context.TemplateMessagePurposes.Add(purpose);

            _context.SaveChanges();
        }
    }
}