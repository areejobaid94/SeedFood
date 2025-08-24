using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.DeliveryOrderDetails.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.DeliveryOrderDetails
{
    public interface IDeliveryOrderDetailsAppService : IApplicationService
    {
        Task<PagedResultDto<GetDeliveryOrderDetailsForViewDto>> GetAll(GetAllDeliveryOrderDetailsInput input);
        DeliveryOrderDetailsDto GetDeliveryOrderDetails(int OrderId);
        void AddDeliveryOrderDetails(DeliveryOrderDetailsDto deliveryLocationCost);
        void Delete(DeliveryOrderDetailsDto deliveryLocationCost);
    }
}
