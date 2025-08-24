using System.Collections.Generic;
using Infoseed.MessagingPortal.TenantServices.Dtos;
using Infoseed.MessagingPortal.Dto;

namespace Infoseed.MessagingPortal.TenantServices.Exporting
{
    public interface ITenantServicesExcelExporter
    {
        FileDto ExportToFile(List<GetTenantServiceForViewDto> tenantServices);
    }
}