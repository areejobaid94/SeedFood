using BotService.Models;
using BotService.Models.API;
using Infoseed.MessagingPortal.Web.Models.Sunshine;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace BotService.Interfaces
{
    public interface IBotApis
    {
        List<string> GetBranchesWithPage(int? TenantID, string local, int pageNumber, int pageSize);
        Area GetBranch(int? TenantID, string AreaName, string local);
        LocationInfoModel GetlocationUserModel( SendLocationUserModel input);
        string AddMenuContcatKey(MenuContcatKeyModel model);
        OrderAndDetailsModel GetOrderAndDetails(SendOrderAndDetailModel input);
        void DeleteOrderDraft(int tenantID, long orderId);
        CancelOrderModel UpdateCancelOrder(int? TenantID, string OrderNumber, int ContactId, string CanatCancelOrderText);
        string UpdateOrder(UpdateOrderModel updateOrderModel);
        List<Caption> GetAllCaption(int TenantID, string local);

        void UpdateCustomer(CustomerModel customer);
        CustomerModel GetCustomer(string id);
    }
}
