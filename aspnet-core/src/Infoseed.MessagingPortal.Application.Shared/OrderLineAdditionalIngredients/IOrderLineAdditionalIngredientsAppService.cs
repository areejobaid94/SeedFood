using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.OrderLineAdditionalIngredients.Dtos;
using Infoseed.MessagingPortal.Dto;


namespace Infoseed.MessagingPortal.OrderLineAdditionalIngredients
{
    public interface IOrderLineAdditionalIngredientsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetOrderLineAdditionalIngredientForViewDto>> GetAll(GetAllOrderLineAdditionalIngredientsInput input);

        Task<GetOrderLineAdditionalIngredientForViewDto> GetOrderLineAdditionalIngredientForView(long id);

		Task<GetOrderLineAdditionalIngredientForEditOutput> GetOrderLineAdditionalIngredientForEdit(EntityDto<long> input);

		Task CreateOrEdit(CreateOrEditOrderLineAdditionalIngredientDto input);

		Task Delete(EntityDto<long> input);

		
		Task<PagedResultDto<OrderLineAdditionalIngredientOrderLookupTableDto>> GetAllOrderForLookupTable(GetAllForLookupTableInput input);
		
    }
}