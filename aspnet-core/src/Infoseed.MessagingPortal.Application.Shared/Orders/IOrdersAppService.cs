using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.Orders.Dtos;
using System.Collections.Generic;
using Infoseed.MessagingPortal.OrderDetails.Dtos;
using Infoseed.MessagingPortal.Dto;

namespace Infoseed.MessagingPortal.Orders
{
    public interface IOrdersAppService : IApplicationService 
    {
		OrderDto GetOrderById(long orderId);
		void UpdateOrderStatus(OrderDto order);
        OrderStatusHistoryDto GetOrderStatusHistory(long orderId);
		bool CreateOrderStatusHistory(long orderId, int orderStatusId, int? TenantId = null);
        Task<PagedResultDto<GetOrderForViewDto>> GetAll(GetAllOrdersInput input);
		OrderEntity GetAllByContactId(int contactId, int? tenantId, int? pageNumber, int? pageSize);
		Task<GetOrderForViewDto> GetOrderForView(long id);

		Task<GetOrderForEditOutput> GetOrderForEdit(EntityDto<long> input);

		Task Delete(EntityDto<long> input);

		Task<FileDto> GetOrderToExcel(GetAllOrdersInput input);
		Task CreateOrEdit(CreateOrEditOrderDto input);
		Task Lock(EntityDto<long> input, int agentId, string agentName, string stringTotall);
		Task UnLock(EntityDto<long> input, string stringTotall);
        Task<bool> DeleteOrder(EntityDto<long> input, string stringTotall, int agentId, string agentName);
        Task<bool> DoneOrder(EntityDto<long> input, string stringTotall, int agentId, string agentName);
		Task CloseOrder(EntityDto<long> input, string stringTotall);
		Task DeleteForEver(EntityDto<long> input);
		Task DeleteAllForEver();
		Task<List<GetOrderDetailForViewDto>> GetAllDetail(long orderId);
		List<OrderDetailDto> GetOrderDetail(int? TenantID, int? OrderId);
		OrderDto GetOrderExtraDetails(int TenantID, long ContactId);
		Task<long> CreateNewOrder(string orderJson);
		Task<long> PostNewOrder(string orderJson);
		Task<long> CreateOrderDetails(string orderDetailsJson);
		Task<long> CreateOrderDetailsExtra(string orderDetailsExtraJson);
		Task<long> CreateOrderDetailsSpecifications(string orderDetailsSpecificationsJson);
		Task<List<GetOrderDetailForViewDto>> GetAllDetailForMenu(int tenantId, long orderId);
        Task<List<GetOrderDetailForViewDto>> GetOrderDetailsForMenu(int tenantId, long orderId);

        long UpdateOrder(string order, long orderid,int tenantID);
        OrderSoket UpdateOrderSoket(string order, long orderid, int tenantID);

        string GetOrderDetailsForBot(long orderId, int lang, string resourceIds, bool isOrderOffer,long areaId);

		OrderEntity GetAllLoyaltyRemainingdays(int contactId, int? tenantId, int? pageNumber, int? pageSize);
        OrderDto GetOrderByNumber(long orderNumber, int tenantId);
		string updateOrderZeedlyStatus(long orderId, int tenantId, int zeedlyStatus);
		void updateOrderETA(long orderNumber, int tenantId, string eta);


    }
}