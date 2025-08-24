using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.Billings.Dtos;
using Infoseed.MessagingPortal.Dto;

namespace Infoseed.MessagingPortal.Billings
{
    public interface IBillingsAppService : IApplicationService
    {
        Task<PagedResultDto<GetBillingForViewDto>> GetAll(GetAllBillingsInput input);

        Task<GetBillingForViewDto> GetBillingForView(int id);

        Task<GetBillingForEditOutput> GetBillingForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditBillingDto input);

        Task Delete(EntityDto input);

        Task<FileDto> GetBillingsToExcel(GetAllBillingsForExcelInput input);

        Task<PagedResultDto<BillingCurrencyLookupTableDto>> GetAllCurrencyForLookupTable(GetAllForLookupTableInput input);

    }
}