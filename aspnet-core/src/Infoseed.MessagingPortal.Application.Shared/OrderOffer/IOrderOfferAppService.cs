using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.OrderOffer.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.OrderOffer
{
   public  interface IOrderOfferAppService : IApplicationService
    {
		Task<PagedResultDto<GetOrderOfferForViewDto>> GetAll(GetAllOrderOfferInput input);

		Task<GetOrderOfferForViewDto> GetOrderOfferForView(long id);

		Task<GetOrderOfferForEditOutput> GetOrderOfferForEdit(EntityDto<long> input);
	
		Task CreateOrEdit(CreateOrEditOrderOfferDto input);
		
		Task DeleteOrderOffer(EntityDto<long> input);
		
	}
}
