using System.Linq;
using Abp.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using Infoseed.MessagingPortal.Editions;
using Infoseed.MessagingPortal.EntityFrameworkCore;
using System;

namespace Infoseed.MessagingPortal.Migrations.Seed.Tenants
{
    public class DefaultTenantBuilder
    {
        private readonly MessagingPortalDbContext _context;

        public DefaultTenantBuilder(MessagingPortalDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateDefaultTenant();
        }

        private void CreateDefaultTenant()
        {
            //Default tenant

           var defaultTenant =  _context.Tenants.IgnoreQueryFilters().FirstOrDefault(t => t.TenancyName == MultiTenancy.Tenant.DefaultTenantName);
            if (defaultTenant == null)
            {
                defaultTenant = new MultiTenancy.Tenant(AbpTenantBase.DefaultTenantName, AbpTenantBase.DefaultTenantName);

                var defaultEdition = _context.Editions.IgnoreQueryFilters().FirstOrDefault(e => e.Name == EditionManager.DefaultEditionName);
                if (defaultEdition != null)
                {
                    defaultTenant.EditionId = defaultEdition.Id;
                }

                defaultTenant.SmoochAPIKeyID = "app_5fe3b5cb056dd4000cef39dc";
                defaultTenant.SmoochAppID = "5fe3b59df87d5a000caa209a";
                defaultTenant.SmoochSecretKey = "g1d6eB17kf4vOMzeFFuen85ZHKAwNrBloNv3B1WPkWxIcOnnrpGyhayD38zRkgIl-SWu75S-iGLZN9RjvNC82Q";


                _context.Tenants.Add(defaultTenant);
                _context.SaveChanges();
            }
        }
    }
}
