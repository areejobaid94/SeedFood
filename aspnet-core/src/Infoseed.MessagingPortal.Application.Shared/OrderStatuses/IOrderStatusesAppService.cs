using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.OrderStatuses.Dtos;
using Infoseed.MessagingPortal.Dto;


namespace Infoseed.MessagingPortal.OrderStatuses
{
    public interface IOrderStatusesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetOrderStatusForViewDto>> GetAll(GetAllOrderStatusesInput input);

        Task<GetOrderStatusForViewDto> GetOrderStatusForView(long id);

		Task<GetOrderStatusForEditOutput> GetOrderStatusForEdit(EntityDto<long> input);

		Task CreateOrEdit(CreateOrEditOrderStatusDto input);

		Task Delete(EntityDto<long> input);

		
    }
}