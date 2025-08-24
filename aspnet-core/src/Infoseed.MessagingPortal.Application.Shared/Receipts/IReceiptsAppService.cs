using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.Receipts.Dtos;
using Infoseed.MessagingPortal.Dto;

namespace Infoseed.MessagingPortal.Receipts
{
    public interface IReceiptsAppService : IApplicationService
    {
        Task<PagedResultDto<GetReceiptForViewDto>> GetAll(GetAllReceiptsInput input);

        Task<GetReceiptForViewDto> GetReceiptForView(int id);

        Task<GetReceiptForEditOutput> GetReceiptForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditReceiptDto input);

        Task Delete(EntityDto input);

        Task<FileDto> GetReceiptsToExcel(GetAllReceiptsForExcelInput input);

        Task<PagedResultDto<ReceiptBankLookupTableDto>> GetAllBankForLookupTable(GetAllForLookupTableInput input);

        Task<PagedResultDto<ReceiptPaymentMethodLookupTableDto>> GetAllPaymentMethodForLookupTable(GetAllForLookupTableInput input);

    }
}