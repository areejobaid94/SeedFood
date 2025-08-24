using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.ReceiptDetails.Dtos;
using Infoseed.MessagingPortal.Dto;

namespace Infoseed.MessagingPortal.ReceiptDetails
{
    public interface IReceiptDetailsAppService : IApplicationService
    {
        Task<PagedResultDto<GetReceiptDetailForViewDto>> GetAll(GetAllReceiptDetailsInput input);

        Task<GetReceiptDetailForViewDto> GetReceiptDetailForView(int id);

        Task<GetReceiptDetailForEditOutput> GetReceiptDetailForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditReceiptDetailDto input);

        Task Delete(EntityDto input);

        Task<FileDto> GetReceiptDetailsToExcel(GetAllReceiptDetailsForExcelInput input);

        Task<PagedResultDto<ReceiptDetailReceiptLookupTableDto>> GetAllReceiptForLookupTable(GetAllForLookupTableInput input);

        Task<PagedResultDto<ReceiptDetailAccountBillingLookupTableDto>> GetAllAccountBillingForLookupTable(GetAllForLookupTableInput input);

    }
}