using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.OrderDetails.Dtos;
using Infoseed.MessagingPortal.Dto;


namespace Infoseed.MessagingPortal.OrderDetails
{
    public interface IOrderDetailsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetOrderDetailForViewDto>> GetAll(GetAllOrderDetailsInput input);

        Task<GetOrderDetailForViewDto> GetOrderDetailForView(long id);

		Task<GetOrderDetailForEditOutput> GetOrderDetailForEdit(EntityDto<long> input);

		Task CreateOrEdit(CreateOrEditOrderDetailDto input);

		Task Delete(EntityDto<long> input);

		
		Task<PagedResultDto<OrderDetailOrderLookupTableDto>> GetAllOrderForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<OrderDetailMenuLookupTableDto>> GetAllMenuForLookupTable(GetAllForLookupTableInput input);
		
    }
}