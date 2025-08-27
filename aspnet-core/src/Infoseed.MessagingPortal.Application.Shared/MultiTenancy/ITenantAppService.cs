using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.Dto;
using Infoseed.MessagingPortal.MultiTenancy.Dto;
using Infoseed.MessagingPortal.Tenants.Dashboard.Dto;

namespace Infoseed.MessagingPortal.MultiTenancy
{
    public interface ITenantAppService : IApplicationService
    {
        Task<PagedResultDto<TenantListDto>> GetTenants(GetTenantsInput input);
        Task<PagedResultDto<HostTenantListDto>> GetHostTenants(int? pageSize = 10, int? pageNumber = 0, string filter = "");
        Task<string> GetTenantPhoneNumber(EntityDto input);
        Task CreateTenant(CreateTenantInput input);

        Task<TenantEditDto> GetTenantForEdit(EntityDto input);
        //List<ExportToExcelHost> ExportToExcelHost();
        List<ExportToExcelHost> ExportToExcelHost1();
        Task UpdateTenant(TenantEditDto input);

        Task DeleteTenant(EntityDto input);

        Task<GetTenantFeaturesEditOutput> GetTenantFeaturesForEdit(EntityDto input);

        Task UpdateTenantFeatures(UpdateTenantFeaturesInput input);

        Task ResetTenantSpecificFeatures(EntityDto input);

        Task UnlockTenantAdmin(EntityDto input);

        Task<TenantEditDto> GetTenantForEditPhone(int input);
        FileDto ExportTenantsToExcel(int month);
        FileDto ExportTenantsToExcelHost();

        Task<SettingsTenantHostModel> GetSettingsTenantHost(int Tenant);
        SettingsTenantHostModel UpdateSettingsTenantHost(SettingsTenantHostModel model);
        string GetTenantCatalogueLink(int tenantId);
        Task<CatalogueDto> GetCatalogue(int tenantId);
        Task<List<ProductItem>> GetCatalogueItems(int tenantId);
        bool AddCatalogueEditLog(CatalogueAuditLogDto model);





    }
}
