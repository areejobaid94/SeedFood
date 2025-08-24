using Abp.Application.Services;
using Infoseed.MessagingPortal.General.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.General
{
    public interface ICosmosAppService : IApplicationService
    {
        Task<TenantsModel> GetTenant(int tenantId);
    }
}
