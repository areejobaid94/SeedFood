using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.DeliveryChanges.Dtos;
using Infoseed.MessagingPortal.Dto;
using System.Collections.Generic;


namespace Infoseed.MessagingPortal.DeliveryChanges
{
    public interface IDeliveryChangesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetDeliveryChangeForViewDto>> GetAll(GetAllDeliveryChangesInput input);

        Task<GetDeliveryChangeForViewDto> GetDeliveryChangeForView(long id);

		Task<GetDeliveryChangeForEditOutput> GetDeliveryChangeForEdit(EntityDto<long> input);

		Task CreateOrEdit(CreateOrEditDeliveryChangeDto input);

		Task Delete(EntityDto<long> input);

		
		Task<List<DeliveryChangeAreaLookupTableDto>> GetAllAreaForTableDropdown();
		
    }
}