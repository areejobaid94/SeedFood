using Abp.Domain.Repositories;
using Framework.Data;
using Infoseed.MessagingPortal.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Infoseed.MessagingPortal.Configuration.AppSettings;
namespace Infoseed.MessagingPortal.TenantInformation
{
    public class TenantInformationService : MessagingPortalAppServiceBase, ITenantInformation
    {

        private readonly IRepository<TenantInformation> _tenantInformation;

        public TenantInformationService(IRepository<TenantInformation> tenantInformation)
        {
            this._tenantInformation = tenantInformation;
        }

        public async Task CreateTenantInformationAsync(int tenantId, DateTime startDate, DateTime endDate)
        {           
                TenantInformation tenantInformation = new TenantInformation
                {
                    TenantId = tenantId,
                    StartDate = startDate,
                    EndDate = endDate

                };
                _tenantInformation.Insert(tenantInformation);
                      
        }

        public async Task DeleteAsync(int tenantId)
        {
            var tenantInformation = _tenantInformation.GetAllList().Where(x => x.TenantId == tenantId).FirstOrDefault();
            if (tenantInformation != null)
            {
                await _tenantInformation.DeleteAsync(tenantInformation.Id);
            }       
        }

        public async Task<TenantInformation> GetTenantInformationAsync(int? tenantId)
        {
            var tenantInformation = _tenantInformation.GetAllList().Where(x => x.TenantId == tenantId).FirstOrDefault();  
            return tenantInformation; 
        }

        public async Task UpdateTenantInformationAsync(int tenantId, DateTime startDate, DateTime endDate)
        {
            var tenantInformation = _tenantInformation.GetAllList().Where(x => x.TenantId == tenantId).FirstOrDefault();

            if (tenantInformation != null)
            {
                tenantInformation.EndDate = endDate;
                tenantInformation.StartDate = startDate;

                var list = _tenantInformation.UpdateAsync(tenantInformation);
            }           

        }
    }
}
