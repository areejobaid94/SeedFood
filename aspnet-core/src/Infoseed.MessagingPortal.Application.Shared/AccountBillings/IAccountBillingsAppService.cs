using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.AccountBillings.Dtos;
using Infoseed.MessagingPortal.Dto;

namespace Infoseed.MessagingPortal.AccountBillings
{
    public interface IAccountBillingsAppService : IApplicationService
    {
        Task<PagedResultDto<GetAccountBillingForViewDto>> GetAll(GetAllAccountBillingsInput input);

        Task<List<GetAccountBillingForViewDto>> GetAccountBillingForView(int id);

        Task<GetAccountBillingForEditOutput> GetAccountBillingForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditAccountBillingDto input);

        Task Delete(EntityDto input);

        Task<FileDto> GetAccountBillingsToExcel(GetAllAccountBillingsForExcelInput input);

        Task<PagedResultDto<AccountBillingInfoSeedServiceLookupTableDto>> GetAllInfoSeedServiceForLookupTable(GetAllForLookupTableInput input);

        Task<PagedResultDto<AccountBillingServiceTypeLookupTableDto>> GetAllServiceTypeForLookupTable(GetAllForLookupTableInput input);

        Task<PagedResultDto<AccountBillingCurrencyLookupTableDto>> GetAllCurrencyForLookupTable(GetAllForLookupTableInput input);
    }
}