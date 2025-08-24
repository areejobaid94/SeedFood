using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.TenantServices.Dtos;
using Infoseed.MessagingPortal.Dto;
using System.Collections.Generic;

namespace Infoseed.MessagingPortal.TenantServices
{
    public interface ITenantServicesAppService : IApplicationService
    {
        IList<TenantServiceModalDto> GetTenatServices(int tenantIdentifer);
        Task<PagedResultDto<GetTenantServiceForViewDto>> GetAll(GetAllTenantServicesInput input);

        Task<GetTenantServiceForViewDto> GetTenantServiceForView(int id);

        Task<GetTenantServiceForEditOutput> GetTenantServiceForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditTenantServiceDto input);

        Task Delete(EntityDto input);

        Task<FileDto> GetTenantServicesToExcel(GetAllTenantServicesForExcelInput input);

        Task<PagedResultDto<TenantServiceInfoSeedServiceLookupTableDto>> GetAllInfoSeedServiceForLookupTable(GetAllForLookupTableInput input);

    }
}