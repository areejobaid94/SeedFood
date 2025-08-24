using Abp.Application.Services;
using Infoseed.MessagingPortal.Contacts.Dtos;
using Infoseed.MessagingPortal.TenantServices.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.TenantServicesInfo
{
    public interface ITenantServicesInfoAppService : IApplicationService
    {
     TenantInfoForOrdaringSystemDto GetTenantsById(int tenantId, int contactId);

     ContactDto GetContactbyId(int id);
    }
}
