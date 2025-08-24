using Abp.Application.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Careem_Express
{
    public interface ICareemExpressService : IApplicationService
    {
        Task<CreateDeliveryResponse> CreateDelivery(CreateDeliveryDTO dto);
        Task<string> CancelDelivery(string deliveryId, string cancellationReason);
        Task<GetDeliveryResponse> GetDelivery(string deliveryId);
        Task<TrackDeliveryResponse> TrackDelivery(string deliveryId);





    }
}
