using Abp.Application.Services;
using Infoseed.MessagingPortal.UTracOrder.Dto;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.UTracOrder
{
    public interface IUTrackOrderAppService : IApplicationService
    {
        Task<UTracCreateOrderResponseModel> CreateUTracOrder(UTracCreateOrderModel model);
        Task<UTracPriceResponseModel> GetUTracPriceList(string integrator_id);
        Task<UTracOrderResponseModel> GetUTracOrderList(UTracSearchEntity model);
        Task<UTracCancelOrderResponseModel> CancelUtracOrder(UTracCancelOrderModel model);
    }
}
