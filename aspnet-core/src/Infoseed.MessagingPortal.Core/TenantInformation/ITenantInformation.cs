using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.TenantInformation
{
   public  interface ITenantInformation
    {
        Task<TenantInformation> GetTenantInformationAsync(int? tenantId);
        Task CreateTenantInformationAsync(int tenantId, DateTime startDate, DateTime endDate);
        Task UpdateTenantInformationAsync(int tenantId , DateTime startDate , DateTime endDate);

        Task DeleteAsync(int tenantId);
    }
}
