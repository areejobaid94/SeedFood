using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.OrderReceipts.Dtos;
using Infoseed.MessagingPortal.Dto;


namespace Infoseed.MessagingPortal.OrderReceipts
{
    public interface IOrderReceiptsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetOrderReceiptForViewDto>> GetAll(GetAllOrderReceiptsInput input);

        Task<GetOrderReceiptForViewDto> GetOrderReceiptForView(long id);

		Task<GetOrderReceiptForEditOutput> GetOrderReceiptForEdit(EntityDto<long> input);

		Task CreateOrEdit(CreateOrEditOrderReceiptDto input);

		Task Delete(EntityDto<long> input);

		
		Task<PagedResultDto<OrderReceiptOrderLookupTableDto>> GetAllOrderForLookupTable(GetAllForLookupTableInput input);
		
    }
}