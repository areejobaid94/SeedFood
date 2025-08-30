using Abp.Application.Services.Dto;
using DocumentFormat.OpenXml.Drawing.Charts;
using Framework.Data;
using Infoseed.MessagingPortal.Areas.Dtos;
using Infoseed.MessagingPortal.Asset.Dto;
using Infoseed.MessagingPortal.Authorization.Users.Dto;
using Infoseed.MessagingPortal.Billings.Dtos;
using Infoseed.MessagingPortal.Booking.Dtos;
using Infoseed.MessagingPortal.BotFlow.Dtos;
using Infoseed.MessagingPortal.CaptionBot.Dtos;
using Infoseed.MessagingPortal.Configuration.Tenants.Dto;
using Infoseed.MessagingPortal.Contacts.Dtos;
using Infoseed.MessagingPortal.Currencies.Dtos;
using Infoseed.MessagingPortal.DashboardUI.Dto;
using Infoseed.MessagingPortal.DeliveryCost.Dto;
using Infoseed.MessagingPortal.Departments.Dto;
using Infoseed.MessagingPortal.Evaluations.Dtos;
using Infoseed.MessagingPortal.ExtraOrderDetails.Dtos;
using Infoseed.MessagingPortal.General.Dto;
using Infoseed.MessagingPortal.Group.Dto;
using Infoseed.MessagingPortal.Groups.Dto;
using Infoseed.MessagingPortal.ItemAdditionsCategorys.Dtos;
using Infoseed.MessagingPortal.Items.Dtos;
using Infoseed.MessagingPortal.LiveChat.Dto;
using Infoseed.MessagingPortal.Location.Dto;
using Infoseed.MessagingPortal.Loyalty.Dto;
using Infoseed.MessagingPortal.Menus;
using Infoseed.MessagingPortal.Menus.Dtos;
using Infoseed.MessagingPortal.MultiTenancy.Dto;
using Infoseed.MessagingPortal.OrderDetails.Dtos;
using Infoseed.MessagingPortal.Orders;
using Infoseed.MessagingPortal.Orders.Dtos;
using Infoseed.MessagingPortal.SealingReuest.Dto;
using Infoseed.MessagingPortal.Specifications.Dtos;
using Infoseed.MessagingPortal.Teams.Dto;
using Infoseed.MessagingPortal.TemplateMessages.Dtos;
using Infoseed.MessagingPortal.Tenants.Dashboard;
using Infoseed.MessagingPortal.Tenants.Dashboard.Dto;
using Infoseed.MessagingPortal.TenantServices.Dtos;
using Infoseed.MessagingPortal.WhatsApp.Dto;
using InfoSeedAzureFunction.AppFunEntities;
using NPOI.HSSF.Record.Chart;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Security.Policy;
using System.Text.Json;

namespace Infoseed.MessagingPortal
{
    public class DataReaderMapper
    {

        #region Order
        public static OrderDto MapOrder(IDataReader dataReader)
        {
            try
            {
                OrderDto Order = new OrderDto();

                Order.HtmlPrint = SqlDataHelper.GetValue<string>(dataReader, "HtmlPrint");
                Order.AfterDeliveryCost = SqlDataHelper.GetValue<decimal?>(dataReader, "AfterDeliveryCost");
                Order.DeliveryCost = SqlDataHelper.GetValue<decimal?>(dataReader, "DeliveryCost");
                Order.AreaId = SqlDataHelper.GetValue<long>(dataReader, "AreaId");
                Order.OrderType = (OrderTypeEunm)SqlDataHelper.GetValue<int>(dataReader, "OrderType");
                Order.OrderStatus = (OrderStatusEunm)SqlDataHelper.GetValue<int>(dataReader, "OrderStatus");
                Order.BranchId = SqlDataHelper.GetValue<long?>(dataReader, "BranchId");
                Order.OrderNumber = (SqlDataHelper.GetValue<long>(dataReader, "OrderNumber"));
                Order.ContactId = SqlDataHelper.GetValue<int?>(dataReader, "ContactId");
                Order.OrderTime = SqlDataHelper.GetValue<string>(dataReader, "OrderTime");
                Order.OrderRemarks = SqlDataHelper.GetValue<string>(dataReader, "OrderRemarks");
                Order.CreationTime = SqlDataHelper.GetValue<DateTime>(dataReader, "CreationTime");
                Order.DeletionTime = SqlDataHelper.GetValue<DateTime?>(dataReader, "DeletionTime");
                Order.LastModificationTime = SqlDataHelper.GetValue<DateTime?>(dataReader, "LastModificationTime");
                Order.AgentId = SqlDataHelper.GetValue<long>(dataReader, "AgentId");
                Order.AgentIds = SqlDataHelper.GetValue<string>(dataReader, "AgentIds");
                Order.IsLockByAgent = SqlDataHelper.GetValue<bool>(dataReader, "IsLockByAgent");
                Order.LockByAgentName = SqlDataHelper.GetValue<string>(dataReader, "LockByAgentName");
                Order.Total = SqlDataHelper.GetValue<decimal>(dataReader, "Total");
                Order.TotalPoints = SqlDataHelper.GetValue<decimal>(dataReader, "TotalPoints");
                Order.TotalCreditPoints = SqlDataHelper.GetValue<decimal>(dataReader, "TotalCreditPoints");
                Order.Address = SqlDataHelper.GetValue<string>(dataReader, "Address");
                Order.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");
                Order.BranchAreaId = SqlDataHelper.GetValue<int>(dataReader, "BranchAreaId");
                Order.TenantId = SqlDataHelper.GetValue<int>(dataReader, "TenantId");
                Order.BranchAreaName = SqlDataHelper.GetValue<string>(dataReader, "BranchAreaName");
                Order.RestaurantName = SqlDataHelper.GetValue<string>(dataReader, "RestaurantName");
                Order.FromLocationDescribation = SqlDataHelper.GetValue<string>(dataReader, "FromLocationDescribation");
                Order.OrderDescribation = SqlDataHelper.GetValue<string>(dataReader, "OrderDescribation");
                Order.ToLocationDescribation = SqlDataHelper.GetValue<string>(dataReader, "ToLocationDescribation");
                Order.IsSpecialRequest = SqlDataHelper.GetValue<bool>(dataReader, "IsSpecialRequest");
                Order.SpecialRequestText = SqlDataHelper.GetValue<string>(dataReader, "SpecialRequestText");
                Order.IsPreOrder = SqlDataHelper.GetValue<bool>(dataReader, "IsPreOrder");
                Order.SelectDay = SqlDataHelper.GetValue<string>(dataReader, "SelectDay");
                Order.SelectTime = SqlDataHelper.GetValue<string>(dataReader, "SelectTime");
                Order.BuyType = SqlDataHelper.GetValue<string>(dataReader, "BuyType");
                Order.StringTotal = (Math.Round(SqlDataHelper.GetValue<decimal>(dataReader, "Total") * 100) / 100).ToString("N2");
                Order.OrderLocal = SqlDataHelper.GetValue<string>(dataReader, "OrderLocal");
                Order.ActionTime = DateTime.Now;
                Order.LastModificationTime = DateTime.Now;
                Order.OrderDetailsCareem = SqlDataHelper.GetValue<string>(dataReader, "OrderDetailsCareem");
                Order.DeliveryEstimation = SqlDataHelper.GetValue<DateTime?>(dataReader, "DeliveryEstimation");
                int? statusInt = SqlDataHelper.GetValue<int?>(dataReader, "ZeedlyOrderStatus");
                Order.ZeedlyOrderStatus = statusInt.HasValue
                    ? (ZeedlyOrderStatus)statusInt.Value
                    : default(ZeedlyOrderStatus);
                Order.IsZeedlyOrder = SqlDataHelper.GetValue<bool>(dataReader, "IsZeedlyOrder");
                return Order;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public static GetOrderForViewDto MapOrders(IDataReader dataReader)
        {
            GetOrderForViewDto entity = new GetOrderForViewDto();
            OrderDto Order = new OrderDto();


            Order.HtmlPrint = SqlDataHelper.GetValue<string>(dataReader, "HtmlPrint");
            Order.AfterDeliveryCost = SqlDataHelper.GetValue<decimal?>(dataReader, "AfterDeliveryCost");
            Order.DeliveryCost = SqlDataHelper.GetValue<decimal?>(dataReader, "DeliveryCost");
            Order.AreaId = SqlDataHelper.GetValue<long>(dataReader, "AreaId");
            Order.OrderType = (OrderTypeEunm)SqlDataHelper.GetValue<int>(dataReader, "OrderType");
            Order.OrderStatus = (OrderStatusEunm)SqlDataHelper.GetValue<int>(dataReader, "OrderStatus");
            Order.BranchId = SqlDataHelper.GetValue<long?>(dataReader, "BranchId");
            Order.OrderNumber = (SqlDataHelper.GetValue<long>(dataReader, "OrderNumber"));
            Order.ContactId = SqlDataHelper.GetValue<int?>(dataReader, "ContactId");

            try
            {
                Order.OrderTime = SqlDataHelper.GetValue<string>(dataReader, "OrderTime");
                Order.CreationTime = SqlDataHelper.GetValue<DateTime>(dataReader, "CreationTime");
                Order.DeletionTime = SqlDataHelper.GetValue<DateTime?>(dataReader, "DeletionTime");
                Order.LastModificationTime = SqlDataHelper.GetValue<DateTime?>(dataReader, "LastModificationTime");
            }
            catch
            {

            }



            Order.OrderRemarks = SqlDataHelper.GetValue<string>(dataReader, "OrderRemarks");

            Order.AgentId = SqlDataHelper.GetValue<long>(dataReader, "AgentId");
            Order.AgentIds = SqlDataHelper.GetValue<string>(dataReader, "AgentIds");
            Order.IsLockByAgent = SqlDataHelper.GetValue<bool>(dataReader, "IsLockByAgent");
            Order.LockByAgentName = SqlDataHelper.GetValue<string>(dataReader, "LockByAgentName");
            Order.Total = SqlDataHelper.GetValue<decimal>(dataReader, "Total");
            Order.Tax = SqlDataHelper.GetValue<decimal?>(dataReader, "Tax") ?? 0;

            Order.TotalPoints = SqlDataHelper.GetValue<decimal>(dataReader, "TotalPoints");
            Order.Address = SqlDataHelper.GetValue<string>(dataReader, "Address");
            Order.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");
            Order.BranchAreaId = SqlDataHelper.GetValue<int>(dataReader, "BranchAreaId");
            Order.BranchAreaName = SqlDataHelper.GetValue<string>(dataReader, "BranchAreaName");
            Order.RestaurantName = SqlDataHelper.GetValue<string>(dataReader, "RestaurantName");
            Order.FromLocationDescribation = SqlDataHelper.GetValue<string>(dataReader, "FromLocationDescribation");
            Order.OrderDescribation = SqlDataHelper.GetValue<string>(dataReader, "OrderDescribation");
            Order.ToLocationDescribation = SqlDataHelper.GetValue<string>(dataReader, "ToLocationDescribation");
            Order.IsSpecialRequest = SqlDataHelper.GetValue<bool>(dataReader, "IsSpecialRequest");
            Order.SpecialRequestText = SqlDataHelper.GetValue<string>(dataReader, "SpecialRequestText");
            Order.IsPreOrder = SqlDataHelper.GetValue<bool>(dataReader, "IsPreOrder");
            Order.SelectDay = SqlDataHelper.GetValue<string>(dataReader, "SelectDay");
            Order.SelectTime = SqlDataHelper.GetValue<string>(dataReader, "SelectTime");
            Order.BuyType = SqlDataHelper.GetValue<string>(dataReader, "BuyType");
            Order.StringTotal = (Math.Round(SqlDataHelper.GetValue<decimal>(dataReader, "Total") * 100) / 100).ToString("N2");

            Order.IsItemOffer = SqlDataHelper.GetValue<bool>(dataReader, "IsItemOffer");
            Order.IsDeliveryOffer = SqlDataHelper.GetValue<bool>(dataReader, "IsDeliveryOffer");
            Order.ItemOffer = SqlDataHelper.GetValue<decimal?>(dataReader, "ItemOffer");
            Order.DeliveryOffer = SqlDataHelper.GetValue<decimal?>(dataReader, "DeliveryOffer");
            // Order.ActionTime=DateTime.Now;

            entity.Order = Order;

            entity.TotalPoints = SqlDataHelper.GetValue<decimal>(dataReader, "TotalPoints");
            entity.IsConversationExpired = SqlDataHelper.GetValue<bool>(dataReader, "IsConversationExpired");
            entity.IsLockedByAgent = SqlDataHelper.GetValue<bool>(dataReader, "IsLockedByAgent");
            entity.CustomerCustomerName = SqlDataHelper.GetValue<string>(dataReader, "DisplayName");
            entity.CustomerMobile = SqlDataHelper.GetValue<string>(dataReader, "PhoneNumber");
            entity.CustomerID = SqlDataHelper.GetValue<string>(dataReader, "CustomerID");
            entity.LastMessageDate = DateTime.Now;
            entity.AreahName = SqlDataHelper.GetValue<string>(dataReader, "AreahName");
            entity.AreaCoordinatetowTak = SqlDataHelper.GetValue<string>(dataReader, "AreaCoordinatetowTak");
            entity.AreahNametowTak = SqlDataHelper.GetValue<string>(dataReader, "AreahNametowTak");
            entity.OrderNumber = (SqlDataHelper.GetValue<long>(dataReader, "OrderNumber")).ToString();
            entity.CreatTime = SqlDataHelper.GetValue<string>(dataReader, "CreatTime");
            entity.OrderStatusName = Order.OrderStatus.ToString();
            entity.OrderTypeName = ((OrderTypeEunm)SqlDataHelper.GetValue<int>(dataReader, "OrderType")).ToString();
            entity.DeliveryChangeDeliveryServiceProvider = SqlDataHelper.GetValue<string>(dataReader, "DeliveryChangeDeliveryServiceProvider");
            entity.CreatDate = SqlDataHelper.GetValue<string>(dataReader, "CreatDate");
            entity.DeliveryCost = SqlDataHelper.GetValue<decimal>(dataReader, "DeliveryCost");
            entity.BranchAreaName = SqlDataHelper.GetValue<string>(dataReader, "BranchAreaName");
            entity.AreaCoordinatetow = entity.BranchAreaName;
            entity.AreahNametow = Order.RestaurantName;
            entity.DeliveryEstimation = SqlDataHelper.GetValue<string>(dataReader, "DeliveryEstimation");
            entity.OrderDetailsCareem = SqlDataHelper.GetValue<string>(dataReader, "OrderDetailsCareem");
            entity.IsZeedlyOrder = SqlDataHelper.GetValue<bool?>(dataReader, "IsZeedlyOrder");
            entity.ZeedlyOrderStatus = SqlDataHelper.GetValue<int?>(dataReader, "ZeedlyOrderStatus");
            try
            {
                if (SqlDataHelper.GetValue<DateTime>(dataReader, "ActionTime") != null)
                {
                    DateTime? ActionTime = SqlDataHelper.GetValue<DateTime>(dataReader, "ActionTime");
                    ActionTime = ActionTime.Value.AddHours(AppSettingsModel.AddHour);
                    if (ActionTime.HasValue && ActionTime.Value.Year < 1400)
                    {
                        ActionTime = null;
                    }
                    if (ActionTime.HasValue)
                    {
                        entity.ActionTime = ActionTime.Value.ToString("MM/dd hh:mm tt");
                    }
                }

            }
            catch
            {


            }

            //else
            //{
            //    DateTime ActionTime = SqlDataHelper.GetValue<DateTime>(dataReader, "ActionTime");

            //    entity.ActionTime = DateTime.Now.ToString("MM/dd hh:mm tt");
            //}
            try
            {
                entity.StreetName = SqlDataHelper.GetValue<string>(dataReader, "StreetName");
                entity.BuildingNumber = SqlDataHelper.GetValue<string>(dataReader, "BuildingNumber");
                entity.FloorNo = SqlDataHelper.GetValue<string>(dataReader, "FloorNo");

                entity.ApartmentNumber = SqlDataHelper.GetValue<string>(dataReader, "ApartmentNumber");


            }
            catch
            {


            }

            entity.LoyaltyRemainingdays = new LoyaltyRemainingdaysDto() { CreatedDate = DateTime.Now };

            return entity;

        }

        public static OrderStatusSummaryDto MapOrderStatistics(IDataReader dataReader)
        {
            return new OrderStatusSummaryDto
            {
                TotalOrders = SqlDataHelper.GetValue<int>(dataReader, "TotalOrders"),
                Pending = SqlDataHelper.GetValue<int>(dataReader, "Pending"),
                Done = SqlDataHelper.GetValue<int>(dataReader, "Done"),
                Deleted = SqlDataHelper.GetValue<int>(dataReader, "Deleted"),
                Canceled = SqlDataHelper.GetValue<int>(dataReader, "Canceled"),
                PreOrder = SqlDataHelper.GetValue<int>(dataReader, "PreOrder")
            };
        }
        public static GetOrderForViewDto MapLoyaltyRemainingdays(IDataReader dataReader)
        {
            GetOrderForViewDto entity = new GetOrderForViewDto();
            LoyaltyRemainingdaysDto loyaltyRemainingdaysDto = new LoyaltyRemainingdaysDto();


            loyaltyRemainingdaysDto.Points = SqlDataHelper.GetValue<decimal>(dataReader, "Points");
            loyaltyRemainingdaysDto.CreatedDate = SqlDataHelper.GetValue<DateTime>(dataReader, "CreatedDate");
            loyaltyRemainingdaysDto.CreatedDate = loyaltyRemainingdaysDto.CreatedDate.AddHours(AppSettingsModel.AddHour);
            loyaltyRemainingdaysDto.LoyaltyExpiration = SqlDataHelper.GetValue<int>(dataReader, "LoyaltyExpiration");
            loyaltyRemainingdaysDto.OrderNumber = SqlDataHelper.GetValue<long>(dataReader, "OrderNumber");
            loyaltyRemainingdaysDto.Total = SqlDataHelper.GetValue<decimal>(dataReader, "Total");
            loyaltyRemainingdaysDto.Remainingdays = SqlDataHelper.GetValue<int>(dataReader, "Remainingdays");
            loyaltyRemainingdaysDto.OrderType = SqlDataHelper.GetValue<int>(dataReader, "OrderType");

            entity.LoyaltyRemainingdays = loyaltyRemainingdaysDto;




            return entity;

        }
        public static OrderDto MapOrdersDto(IDataReader dataReader)
        {
            OrderDto Order = new OrderDto();


            Order.HtmlPrint = SqlDataHelper.GetValue<string>(dataReader, "HtmlPrint");
            Order.AfterDeliveryCost = SqlDataHelper.GetValue<decimal?>(dataReader, "AfterDeliveryCost");
            Order.DeliveryCost = SqlDataHelper.GetValue<decimal?>(dataReader, "DeliveryCost");
            Order.AreaId = SqlDataHelper.GetValue<long>(dataReader, "AreaId");
            Order.OrderType = (OrderTypeEunm)SqlDataHelper.GetValue<int>(dataReader, "OrderType");
            Order.OrderStatus = (OrderStatusEunm)SqlDataHelper.GetValue<int>(dataReader, "OrderStatus");
            Order.BranchId = SqlDataHelper.GetValue<long?>(dataReader, "BranchId");
            Order.OrderNumber = (SqlDataHelper.GetValue<long>(dataReader, "OrderNumber"));
            Order.ContactId = SqlDataHelper.GetValue<int?>(dataReader, "ContactId");
            Order.OrderTime = SqlDataHelper.GetValue<string>(dataReader, "OrderTime");
            Order.OrderRemarks = SqlDataHelper.GetValue<string>(dataReader, "OrderRemarks");
            Order.CreationTime = SqlDataHelper.GetValue<DateTime>(dataReader, "CreationTime");
            Order.DeletionTime = SqlDataHelper.GetValue<DateTime?>(dataReader, "DeletionTime");
            Order.LastModificationTime = SqlDataHelper.GetValue<DateTime?>(dataReader, "LastModificationTime");
            Order.AgentId = SqlDataHelper.GetValue<long>(dataReader, "AgentId");
            Order.AgentIds = SqlDataHelper.GetValue<string>(dataReader, "AgentIds");
            Order.IsLockByAgent = SqlDataHelper.GetValue<bool>(dataReader, "IsLockByAgent");
            Order.LockByAgentName = SqlDataHelper.GetValue<string>(dataReader, "LockByAgentName");
            Order.Total = SqlDataHelper.GetValue<decimal>(dataReader, "Total");
            Order.TotalPoints = SqlDataHelper.GetValue<decimal>(dataReader, "TotalPoints");
            Order.Address = SqlDataHelper.GetValue<string>(dataReader, "Address");
            Order.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");
            Order.BranchAreaId = SqlDataHelper.GetValue<int>(dataReader, "BranchAreaId");
            Order.BranchAreaName = SqlDataHelper.GetValue<string>(dataReader, "BranchAreaName");
            Order.RestaurantName = SqlDataHelper.GetValue<string>(dataReader, "RestaurantName");
            Order.FromLocationDescribation = SqlDataHelper.GetValue<string>(dataReader, "FromLocationDescribation");
            Order.OrderDescribation = SqlDataHelper.GetValue<string>(dataReader, "OrderDescribation");
            Order.ToLocationDescribation = SqlDataHelper.GetValue<string>(dataReader, "ToLocationDescribation");
            Order.IsSpecialRequest = SqlDataHelper.GetValue<bool>(dataReader, "IsSpecialRequest");
            Order.SpecialRequestText = SqlDataHelper.GetValue<string>(dataReader, "SpecialRequestText");
            Order.IsPreOrder = SqlDataHelper.GetValue<bool>(dataReader, "IsPreOrder");
            Order.SelectDay = SqlDataHelper.GetValue<string>(dataReader, "SelectDay");
            Order.SelectTime = SqlDataHelper.GetValue<string>(dataReader, "SelectTime");
            Order.BuyType = SqlDataHelper.GetValue<string>(dataReader, "BuyType");
            Order.StringTotal = (Math.Round(SqlDataHelper.GetValue<decimal>(dataReader, "Total") * 100) / 100).ToString("N2");
            Order.ActionTime = DateTime.Now;


            return Order;

        }
        public static OrderStatusHistoryDto MapOrderStatusHistory(IDataReader dataReader)
        {
            OrderStatusHistoryDto orderStatus = new OrderStatusHistoryDto();

            orderStatus.DoneCount = SqlDataHelper.GetValue<decimal>(dataReader, "@DoneCount");
            orderStatus.CancelOrDeleteCount = SqlDataHelper.GetValue<decimal>(dataReader, "@CancelOrDeleteCount");
            return orderStatus;

        }
        public static GetOrderDetailForViewDto MapOrderDetail(IDataReader dataReader)
        {
            GetOrderDetailForViewDto entity = new GetOrderDetailForViewDto();
            OrderDetailDto OrderDetail = new OrderDetailDto();

            OrderDetail.TotalLoyaltyPoints = SqlDataHelper.GetValue<decimal>(dataReader, "TotalLoyaltyPoints");
            OrderDetail.UnitPoints = SqlDataHelper.GetValue<decimal>(dataReader, "UnitPoints");

            OrderDetail.Quantity = SqlDataHelper.GetValue<string>(dataReader, "Quantity");
            OrderDetail.UnitPrice = SqlDataHelper.GetValue<decimal>(dataReader, "UnitPrice");
            OrderDetail.Total = SqlDataHelper.GetValue<decimal>(dataReader, "Total");
            OrderDetail.Discount = SqlDataHelper.GetValue<decimal>(dataReader, "Discount");
            OrderDetail.CreationTime = SqlDataHelper.GetValue<DateTime>(dataReader, "CreationTime");
            OrderDetail.OrderId = SqlDataHelper.GetValue<long>(dataReader, "OrderId");
            OrderDetail.ItemId = SqlDataHelper.GetValue<long>(dataReader, "ItemId");
            OrderDetail.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");
            OrderDetail.ItemNote = SqlDataHelper.GetValue<string>(dataReader, "Note");

            entity.OrderDetail = OrderDetail;

            entity.ItemName = SqlDataHelper.GetValue<string>(dataReader, "ItemName");
            entity.ItemNameEnglish = SqlDataHelper.GetValue<string>(dataReader, "ItemNameEnglish");
            entity.SKU = SqlDataHelper.GetValue<string>(dataReader, "SKU");



            return entity;
        }
        public static ExtraOrderDetailsDto MapMenuOrderExtraDetails(IDataReader dataReader)
        {
            ExtraOrderDetailsDto orderExtraDetail = new ExtraOrderDetailsDto();

            orderExtraDetail.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");
            orderExtraDetail.OrderDetailId = SqlDataHelper.GetValue<long>(dataReader, "OrderDetailId");
            orderExtraDetail.Name = SqlDataHelper.GetValue<string>(dataReader, "Name");
            orderExtraDetail.NameEnglish = SqlDataHelper.GetValue<string>(dataReader, "NameEnglish");
            orderExtraDetail.TenantId = SqlDataHelper.GetValue<int>(dataReader, "TenantId");
            orderExtraDetail.Total = SqlDataHelper.GetValue<decimal>(dataReader, "Total");
            orderExtraDetail.UnitPrice = SqlDataHelper.GetValue<decimal>(dataReader, "UnitPrice");
            orderExtraDetail.SpecificationUniqueId = SqlDataHelper.GetValue<long>(dataReader, "SpecificationUniqueId");
            orderExtraDetail.SpecificationName = SqlDataHelper.GetValue<string>(dataReader, "SpecificationName");
            orderExtraDetail.SpecificationNameEnglish = SqlDataHelper.GetValue<string>(dataReader, "SpecificationNameEnglish");
            orderExtraDetail.UnitPoints = SqlDataHelper.GetValue<decimal>(dataReader, "UnitPoints");
            orderExtraDetail.Quantity = SqlDataHelper.GetValue<int>(dataReader, "Quantity");
            orderExtraDetail.TotalLoyaltyPoints = SqlDataHelper.GetValue<decimal>(dataReader, "TotalLoyaltyPoints");
            orderExtraDetail.TypeExtraDetails = SqlDataHelper.GetValue<int>(dataReader, "TypeExtraDetails");
            orderExtraDetail.SpecificationId = SqlDataHelper.GetValue<int>(dataReader, "SpecificationId");
            orderExtraDetail.SpecificationChoiceId = SqlDataHelper.GetValue<int>(dataReader, "SpecificationChoiceId");



            return orderExtraDetail;
        }


        public static OrderDto MapOrderExtraDetails(IDataReader dataReader)
        {
            OrderDto orderDto = new OrderDto();
            orderDto.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");
            orderDto.OrderNumber = SqlDataHelper.GetValue<long>(dataReader, "OrderNumber");
            orderDto.ContactId = SqlDataHelper.GetValue<long>(dataReader, "ContactId");
            orderDto.Total = SqlDataHelper.GetValue<decimal>(dataReader, "Total");
            orderDto.TotalPoints = SqlDataHelper.GetValue<decimal>(dataReader, "TotalPoints");

            orderDto.CreationTime = SqlDataHelper.GetValue<DateTime>(dataReader, "CreationTime");
            orderDto.OrderTime = SqlDataHelper.GetValue<DateTime>(dataReader, "OrderTime");
            orderDto.OrderDetailDtoJson = SqlDataHelper.GetValue<string>(dataReader, "OrderDetailDtoJson");

            return orderDto;
        }

        public static OrderDto MapOrderDetialsExtraDetails(IDataReader dataReader)
        {
            OrderDto orderDto = new OrderDto();
            orderDto.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");
            orderDto.OrderNumber = SqlDataHelper.GetValue<long>(dataReader, "OrderNumber");
            orderDto.ContactId = SqlDataHelper.GetValue<long>(dataReader, "ContactId");
            orderDto.Total = SqlDataHelper.GetValue<decimal>(dataReader, "Total");
            orderDto.TotalPoints = SqlDataHelper.GetValue<decimal>(dataReader, "TotalPoints");

            orderDto.CreationTime = SqlDataHelper.GetValue<DateTime>(dataReader, "CreationTime");
            orderDto.OrderTime = SqlDataHelper.GetValue<DateTime>(dataReader, "OrderTime");
            orderDto.TenantId = SqlDataHelper.GetValue<int>(dataReader, "TenantId");
            orderDto.OrderDetailDtoJson = SqlDataHelper.GetValue<string>(dataReader, "OrderDetailDtoJson");
            orderDto.CaptionJson = SqlDataHelper.GetValue<string>(dataReader, "CaptionJson");
            orderDto.OrderOfferJson = SqlDataHelper.GetValue<string>(dataReader, "OrderOfferJson");

            return orderDto;
        }


        public static OrderDashModel MapGetOrderAllDash(IDataReader dataReader)
        {
            try
            {
                OrderDashModel model = new OrderDashModel();
                model.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");
                model.AgentId = SqlDataHelper.GetValue<long>(dataReader, "AgentId");
                model.CreationTime = SqlDataHelper.GetValue<DateTime>(dataReader, "CreationTime");
                model.ActionTime = SqlDataHelper.GetValue<DateTime>(dataReader, "ActionTime");
                model.OrderStatus = SqlDataHelper.GetValue<int>(dataReader, "OrderStatus");
                model.UserName = SqlDataHelper.GetValue<string>(dataReader, "UserName");
                model.EmailAddress = SqlDataHelper.GetValue<string>(dataReader, "EmailAddress");
                model.Count_OrderStatus_2 = SqlDataHelper.GetValue<int>(dataReader, "Count_OrderStatus_2");
                model.Count_OrderStatus_3 = SqlDataHelper.GetValue<int>(dataReader, "Count_OrderStatus_3");
                model.Avg_ActionTime_Minutes = SqlDataHelper.GetValue<int>(dataReader, "Avg_ActionTime_Minutes");
                model.Avg_ActionTime = SqlDataHelper.GetValue<string>(dataReader, "Avg_ActionTime");
                model.totalOrders = SqlDataHelper.GetValue<long>(dataReader, "totalOrders");

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static OrderStatisticsModel MapOrderGetStatistics(IDataReader dataReader)
        {
            try
            {
                OrderStatisticsModel model = new OrderStatisticsModel();
                model.TotalOrder = SqlDataHelper.GetValue<long>(dataReader, "TotalOrder") ?? (long?)0;
                model.TotalOrderPending = SqlDataHelper.GetValue<long>(dataReader, "TotalOrderPending") ?? (long?)0;
                model.TotalOrderCompleted = SqlDataHelper.GetValue<long>(dataReader, "TotalOrderCompleted") ?? (long?)0;
                model.TotalOrderDeleted = SqlDataHelper.GetValue<long>(dataReader, "TotalOrderDeleted") ?? (long?)0;
                model.TotalOrderCanceled = SqlDataHelper.GetValue<long>(dataReader, "TotalOrderCanceled") ?? (long?)0;
                model.TotalOrderPreOrder = SqlDataHelper.GetValue<long>(dataReader, "TotalOrderPreOrder") ?? (long?)0;
                model.TotalRevenue = SqlDataHelper.GetValue<decimal>(dataReader, "TotalRevenue") ?? (decimal?)0;
                model.RewardPoints = SqlDataHelper.GetValue<decimal>(dataReader, "RewardPoints") ?? (decimal?)0;
                model.RedeemedPoints = SqlDataHelper.GetValue<decimal>(dataReader, "RedeemedPoints") ?? (decimal?)0;
                model.TotalTakeaway = SqlDataHelper.GetValue<long>(dataReader, "TotalTakeaway") ?? (long?)0;
                model.TotalDelivery = SqlDataHelper.GetValue<long>(dataReader, "TotalDelivery") ?? (long?)0;
                model.DeliveryCost = SqlDataHelper.GetValue<decimal>(dataReader, "DeliveryCost") ?? (decimal?)0;

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Evaluations
        public static GetEvaluationForViewDto MapEvaluations(IDataReader dataReader)
        {
            GetEvaluationForViewDto entity = new GetEvaluationForViewDto();
            EvaluationDto evaluation = new EvaluationDto();


            evaluation.OrderNumber = SqlDataHelper.GetValue<long>(dataReader, "OrderNumber");
            evaluation.CreationTime = SqlDataHelper.GetValue<DateTime>(dataReader, "CreationTime");
            evaluation.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");
            evaluation.TenantId = SqlDataHelper.GetValue<int>(dataReader, "TenantId");
            evaluation.ContactName = SqlDataHelper.GetValue<string>(dataReader, "ContactName");
            evaluation.EvaluationsText = SqlDataHelper.GetValue<string>(dataReader, "EvaluationsText");
            evaluation.OrderId = SqlDataHelper.GetValue<int>(dataReader, "OrderId");
            evaluation.PhoneNumber = SqlDataHelper.GetValue<string>(dataReader, "PhoneNumber");
            evaluation.EvaluationsReat = SqlDataHelper.GetValue<string>(dataReader, "EvaluationsReat");

            entity.CreatTime = (SqlDataHelper.GetValue<DateTime>(dataReader, "CreationTime")).ToString("hh:mm tt");
            entity.CreatDate = (SqlDataHelper.GetValue<DateTime>(dataReader, "CreationTime")).ToString("MM/dd/yyyy");
            entity.UserId = evaluation.TenantId + "_" + evaluation.PhoneNumber;

            entity.evaluation = evaluation;

            return entity;

        }
        #endregion

        #region Seeling Request 
        public static SellingRequestDto ConvertToSellingRequestDto(IDataReader dataReader)
        {
            SellingRequestDto sellingRequestDto = new SellingRequestDto();


            sellingRequestDto.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");

            sellingRequestDto.ContactInfo = SqlDataHelper.GetValue<string>(dataReader, "ContactInfo");
            sellingRequestDto.TenantId = SqlDataHelper.GetValue<int>(dataReader, "TenantId");
            sellingRequestDto.Price = SqlDataHelper.GetValue<decimal>(dataReader, "Price");
            sellingRequestDto.RequestDescription = SqlDataHelper.GetValue<string>(dataReader, "RequestDescription");
            sellingRequestDto.CreatedBy = SqlDataHelper.GetValue<string>(dataReader, "CreatedBy");
            sellingRequestDto.CreatedOn = SqlDataHelper.GetValue<DateTime>(dataReader, "CreatedOn");
            sellingRequestDto.CreatedOn = sellingRequestDto.CreatedOn.AddHours(AppSettingsModel.AddHour);
            sellingRequestDto.ModifiedOn = SqlDataHelper.GetValue<DateTime>(dataReader, "ModifiedOn");
            sellingRequestDto.ModifiedOn = SqlDataHelper.GetValue<string>(dataReader, "ModifiedOn");
            sellingRequestDto.SellingStatusId = SqlDataHelper.GetValue<int>(dataReader, "SellingStatusId");
            sellingRequestDto.UserId = SqlDataHelper.GetValue<int>(dataReader, "UserId");
            sellingRequestDto.ContactId = SqlDataHelper.GetValue<int>(dataReader, "ContactId");

            try
            {
                sellingRequestDto.DepartmentId = SqlDataHelper.GetValue<int>(dataReader, "DepartmentId");
            }
            catch
            {
                sellingRequestDto.DepartmentId = 0;
            }

            try
            {
                sellingRequestDto.AreaId = SqlDataHelper.GetValue<long>(dataReader, "AreaId");
            }
            catch
            {
                sellingRequestDto.AreaId = 0;
            }

            try
            {
                sellingRequestDto.IsRequestForm = SqlDataHelper.GetValue<bool>(dataReader, "IsRequestForm");
            }
            catch
            {
                sellingRequestDto.IsRequestForm = false;
            }


            sellingRequestDto.PhoneNumber = SqlDataHelper.GetValue<int>(dataReader, "PhoneNumber");
            var SellingRequestDetails = SqlDataHelper.GetValue<string>(dataReader, "SellingRequestDetails");
            var RequestDescription = SqlDataHelper.GetValue<string>(dataReader, "RequestDescription");


            if (!string.IsNullOrEmpty(SellingRequestDetails))
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                sellingRequestDto.lstSellingRequestDetailsDto = System.Text.Json.JsonSerializer.Deserialize<List<SellingRequestDetailsDto>>(SellingRequestDetails, options);
            }
            if (!string.IsNullOrEmpty(RequestDescription) && sellingRequestDto.IsRequestForm)
            {
                var options = new JsonSerializerOptions { WriteIndented = true };

                sellingRequestDto.RequestForm = System.Text.Json.JsonSerializer.Deserialize<SellingRequestFormModel>(RequestDescription, options);
            }
            else
                sellingRequestDto.RequestDescription = RequestDescription;


            var SginUpRequestDescription = SqlDataHelper.GetValue<string>(dataReader, "SginUpRequestDescription");
            try
            {
                sellingRequestDto.IsSginUpForm = SqlDataHelper.GetValue<bool>(dataReader, "IsSginUpForm");
                if (!string.IsNullOrEmpty(SginUpRequestDescription) && sellingRequestDto.IsRequestForm)
                {
                    var options = new JsonSerializerOptions { WriteIndented = true };

                    sellingRequestDto.SginUpRequest = System.Text.Json.JsonSerializer.Deserialize<SginUpRequestModel>(SginUpRequestDescription, options);
                }
                else
                    sellingRequestDto.SginUpRequest = SginUpRequestDescription;
            }
            catch
            {
                sellingRequestDto.IsSginUpForm = false;
                sellingRequestDto.SginUpRequest = new SginUpRequestModel();
            }

            return sellingRequestDto;
        }
        public static Status StatusGet(IDataReader dataReader)
        {
            Status status = new Status();
            status.TenantId = SqlDataHelper.GetValue<int>(dataReader, "TenantId");
            status.LiveChatStatus = SqlDataHelper.GetValue<int>(dataReader, "LiveChatStatus");
            status.ContactId = SqlDataHelper.GetValue<int>(dataReader, "ContactId");
            status.phoneNumber = SqlDataHelper.GetValue<string>(dataReader, "phoneNumber");
            status.CategoryType = SqlDataHelper.GetValue<string>(dataReader, "CategoryType");

            return status;
        }




        #endregion

        #region Asset 


        public static AssetDto ConvertToAssetDto(IDataReader dataReader)
        {
            AssetDto assetDto = new AssetDto();
            assetDto.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");
            assetDto.AssetNameAr = SqlDataHelper.GetValue<string>(dataReader, "AssetNameAr");
            assetDto.TenantId = SqlDataHelper.GetValue<int>(dataReader, "TenantId");
            assetDto.AssetNameEn = SqlDataHelper.GetValue<decimal>(dataReader, "AssetNameEn");


            assetDto.AssetDescriptionAr = SqlDataHelper.GetValue<string>(dataReader, "AssetDescriptionAr");
            assetDto.AssetDescriptionEn = SqlDataHelper.GetValue<string>(dataReader, "AssetDescriptionEn");

            assetDto.AssetLevelOneId = SqlDataHelper.GetValue<long>(dataReader, "AssetLevelOneId");
            assetDto.AssetLevelTwoId = SqlDataHelper.GetValue<long>(dataReader, "AssetLevelTwoId");
            assetDto.AssetLevelThreeId = SqlDataHelper.GetValue<long?>(dataReader, "AssetLevelThreeId");
            assetDto.AssetLevelFourId = SqlDataHelper.GetValue<long?>(dataReader, "AssetLevelFourId");

            assetDto.AssetTypeId = SqlDataHelper.GetValue<int>(dataReader, "AssetTypeId");

            assetDto.CreatedBy = SqlDataHelper.GetValue<string>(dataReader, "CreatedBy");
            assetDto.CreatedOn = SqlDataHelper.GetValue<DateTime>(dataReader, "CreatedOn");
            assetDto.ModifiedOn = SqlDataHelper.GetValue<DateTime>(dataReader, "ModifiedOn");
            assetDto.IsOffer = SqlDataHelper.GetValue<bool>(dataReader, "IsOffer");

            var attachments = SqlDataHelper.GetValue<string>(dataReader, "AssetAttachment");


            if (!string.IsNullOrEmpty(attachments))
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                assetDto.lstAssetAttachmentDto = System.Text.Json.JsonSerializer.Deserialize<List<AssetAttachmentDto>>(attachments, options);
            }
            return assetDto;
        }

        public static LevelsEntity ConvertToLevelsEntity(IDataReader dataReader)
        {
            LevelsEntity levelsEntity = new LevelsEntity();

            var AssetLevelOne = SqlDataHelper.GetValue<string>(dataReader, "AssetLevelOne");
            var AssetLevelTwo = SqlDataHelper.GetValue<string>(dataReader, "AssetLevelTwo");
            var AssetLevelThree = SqlDataHelper.GetValue<string>(dataReader, "AssetLevelThree");
            var AssetLevelFour = SqlDataHelper.GetValue<string>(dataReader, "AssetLevelFour");

            if (!string.IsNullOrEmpty(AssetLevelOne))
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                levelsEntity.lstAssetLevelOneDto = System.Text.Json.JsonSerializer.Deserialize<List<AssetLevelOneDto>>(AssetLevelOne, options);

            }
            if (!string.IsNullOrEmpty(AssetLevelTwo))
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                levelsEntity.lstAssetLevelTwoDto = System.Text.Json.JsonSerializer.Deserialize<List<AssetLevelTwoDto>>(AssetLevelTwo, options);
            }
            if (!string.IsNullOrEmpty(AssetLevelThree))
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                levelsEntity.lstAssetLevelThreeDto = System.Text.Json.JsonSerializer.Deserialize<List<AssetLevelThreeDto>>(AssetLevelThree, options);
            }
            if (!string.IsNullOrEmpty(AssetLevelFour))
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                levelsEntity.lstAssetLevelFourDto = System.Text.Json.JsonSerializer.Deserialize<List<AssetLevelFourDto>>(AssetLevelFour, options);
            }
            return levelsEntity;
        }
        public static AssetLevelTwoDto ConvertMgMotorsGetAsset(IDataReader dataReader)
        {
            AssetLevelTwoDto levelsEntity = new AssetLevelTwoDto();

            levelsEntity.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");
            levelsEntity.LevelOneId = SqlDataHelper.GetValue<long>(dataReader, "LevelOneId");
            levelsEntity.LevelTwoNameAr = SqlDataHelper.GetValue<string>(dataReader, "LevelTwoNameAr");
            levelsEntity.LevelTwoNameEn = SqlDataHelper.GetValue<string>(dataReader, "LevelTwoNameEn");
            levelsEntity.TenantId = SqlDataHelper.GetValue<int>(dataReader, "TenantId");
            return levelsEntity;
        }



        #endregion

        #region Deliver Cost 

        public List<DeliveryCostDetailsDto> lstDeliveryCostDetailsDto { get; set; }

        public static DeliveryCostDto ConvertToDeliveryCostDto(IDataReader dataReader)
        {
            DeliveryCostDto deliveryCostDto = new DeliveryCostDto();
            deliveryCostDto.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");
            deliveryCostDto.TenantId = SqlDataHelper.GetValue<int>(dataReader, "TenantId");
            deliveryCostDto.DeliveryCostJson = SqlDataHelper.GetValue<string>(dataReader, "DeliveryCostJson");

            deliveryCostDto.AreaIds = SqlDataHelper.GetValue<string>(dataReader, "AreaIds");
            deliveryCostDto.CreatedBy = SqlDataHelper.GetValue<string>(dataReader, "CreatedBy");
            deliveryCostDto.CreatedOn = SqlDataHelper.GetValue<DateTime>(dataReader, "CreatedOn");
            deliveryCostDto.ModifiedOn = SqlDataHelper.GetValue<DateTime>(dataReader, "ModifiedOn");
            deliveryCostDto.ModifiedOn = SqlDataHelper.GetValue<string>(dataReader, "ModifiedOn");

            deliveryCostDto.AboveValue = SqlDataHelper.GetValue<decimal>(dataReader, "AboveValue");
            deliveryCostDto.AreaNames = SqlDataHelper.GetValue<string>(dataReader, "AreaNames");

            if (!string.IsNullOrEmpty(deliveryCostDto.DeliveryCostJson))
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                deliveryCostDto.lstDeliveryCostDetailsDto = System.Text.Json.JsonSerializer.Deserialize<List<DeliveryCostDetailsDto>>(deliveryCostDto.DeliveryCostJson, options);
            }
            return deliveryCostDto;
        }

        public static LocationAddressDto ConvertToLocationAddressDto(IDataReader dataReader)
        {
            LocationAddressDto locationAddressDto = new LocationAddressDto();


            locationAddressDto.LocationId = SqlDataHelper.GetValue<long>(dataReader, "Id");
            locationAddressDto.AreaId = SqlDataHelper.GetValue<int>(dataReader, "BranchAreaId");
            locationAddressDto.LocationName = SqlDataHelper.GetValue<string>(dataReader, "LocationName");
            locationAddressDto.DeliveryCostBefor = SqlDataHelper.GetValue<decimal>(dataReader, "DeliveryCost");

            //  locationAddressDto.HasSubDistrict = SqlDataHelper.GetValue<bool>(dataReader, "HasSubDistrict");

            return locationAddressDto;
        }

        #endregion

        #region Contacts 
        public static ContactDto MapContact(IDataReader dataReader)
        {
            ContactDto contactDto = new ContactDto();
            contactDto.Id = SqlDataHelper.GetValue<int>(dataReader, "Id");
            contactDto.UserId = SqlDataHelper.GetValue<string>(dataReader, "UserId");
            contactDto.PhoneNumber = SqlDataHelper.GetValue<string>(dataReader, "PhoneNumber");
            contactDto.Description = SqlDataHelper.GetValue<string>(dataReader, "Description");
            contactDto.EmailAddress = SqlDataHelper.GetValue<string>(dataReader, "EmailAddress");
            contactDto.Website = SqlDataHelper.GetValue<string>(dataReader, "Website");
            contactDto.DisplayName = SqlDataHelper.GetValue<string>(dataReader, "DisplayName");
            contactDto.ContactDisplayName = SqlDataHelper.GetValue<string>(dataReader, "ContactDisplayName");
            contactDto.TakeAwayOrder = SqlDataHelper.GetValue<int>(dataReader, "TakeAwayOrder");
            contactDto.DeliveryOrder = SqlDataHelper.GetValue<int>(dataReader, "DeliveryOrder");
            contactDto.loyalityPoint = SqlDataHelper.GetValue<int>(dataReader, "loyalityPoint");
            contactDto.StreetName = SqlDataHelper.GetValue<string>(dataReader, "StreetName");
            contactDto.BuildingNumber = SqlDataHelper.GetValue<string>(dataReader, "BuildingNumber");
            contactDto.FloorNo = SqlDataHelper.GetValue<string>(dataReader, "FloorNo");
            contactDto.ApartmentNumber = SqlDataHelper.GetValue<string>(dataReader, "ApartmentNumber");
            contactDto.TenantId = SqlDataHelper.GetValue<int>(dataReader, "TenantId");
            contactDto.IsBlock = SqlDataHelper.GetValue<bool>(dataReader, "IsBlock");

            contactDto.CustomerOPT = SqlDataHelper.GetValue<int>(dataReader, "CustomerOPT");
            contactDto.Branch = SqlDataHelper.GetValue<string>(dataReader, "Branch");
            contactDto.TotalLiveChat = SqlDataHelper.GetValue<int>(dataReader, "TotalLiveChat");
            contactDto.TotalOrder = SqlDataHelper.GetValue<int>(dataReader, "TotalOrders");
            contactDto.TotalRequest = SqlDataHelper.GetValue<int>(dataReader, "TotalRequest");
            contactDto.OrderType = SqlDataHelper.GetValue<int>(dataReader, "OrderType");
            contactDto.CreationTime = SqlDataHelper.GetValue<DateTime>(dataReader, "CreationTime");

            try
            {
                contactDto.Group = SqlDataHelper.GetValue<string>(dataReader, "Group");
                contactDto.GroupName = SqlDataHelper.GetValue<string>(dataReader, "GroupName");
            }
            catch
            {
                contactDto.Group = "0";
                contactDto.GroupName = "";

            }

            try
            {
                contactDto.channel = SqlDataHelper.GetValue<string>(dataReader, "channel");
            }
            catch
            {
                contactDto.channel = "";

            }
            return contactDto;
        }

        public static ContactsInterestedOfModel MapContactInterestedOf(IDataReader dataReader)
        {
            ContactsInterestedOfModel InterestedOf = new ContactsInterestedOfModel();
            InterestedOf.LevelOneNameAr = SqlDataHelper.GetValue<string>(dataReader, "LevelOneNameAr");
            InterestedOf.LevelOneNameEn = SqlDataHelper.GetValue<string>(dataReader, "LevelOneNameEn");
            InterestedOf.LevelTwoNameAr = SqlDataHelper.GetValue<string>(dataReader, "LevelTwoNameAr");
            InterestedOf.LevelTwoNameEn = SqlDataHelper.GetValue<string>(dataReader, "LevelTwoNameEn");
            InterestedOf.LevelThreeNameAr = SqlDataHelper.GetValue<string>(dataReader, "LevelThreeNameAr");
            InterestedOf.LevelThreeNameEn = SqlDataHelper.GetValue<string>(dataReader, "LevelThreeNameEn");
            return InterestedOf;
        }
        public static ContactCampaignDto MapContactCampaign(IDataReader dataReader)
        {
            try
            {
                ContactCampaignDto contacts = new ContactCampaignDto
                {
                    Id = SqlDataHelper.GetValue<long>(dataReader, "Id"),
                    PhoneNumber = SqlDataHelper.GetValue<string>(dataReader, "PhoneNumber"),
                    TemplateName = SqlDataHelper.GetValue<string>(dataReader, "TemplateName"),
                    CampaignName = SqlDataHelper.GetValue<string>(dataReader, "CampaignName"),
                    IsSent = SqlDataHelper.GetValue<bool>(dataReader, "IsSent"),
                    IsDelivered = SqlDataHelper.GetValue<bool>(dataReader, "IsDelivered"),
                    IsRead = SqlDataHelper.GetValue<bool>(dataReader, "IsRead"),
                    IsFailed = SqlDataHelper.GetValue<bool>(dataReader, "IsFailed"),
                    IsReplied = SqlDataHelper.GetValue<bool>(dataReader, "IsReplied"),
                    IsHanged = SqlDataHelper.GetValue<bool>(dataReader, "IsHanged")
                };
                return contacts;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public static ContactStatisticsModel MapContactGetStatistics(IDataReader dataReader)
        {
            try
            {
                ContactStatisticsModel model = new ContactStatisticsModel();
                model.TotalContact = SqlDataHelper.GetValue<long>(dataReader, "TotalContact") ?? (long?)0;
                model.TotalContactOptIn = SqlDataHelper.GetValue<long>(dataReader, "TotalContactOptIn") ?? (long?)0;
                model.TotalContactOptOut = SqlDataHelper.GetValue<long>(dataReader, "TotalContactOptOut") ?? (long?)0;
                model.TotalContactNeutral = SqlDataHelper.GetValue<long>(dataReader, "TotalContactNeutral") ?? (long?)0;

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region ItemCategory
        public static CategorysInItemModel ConvertToCategorysInItemModel(IDataReader dataReader)
        {
            CategorysInItemModel categorysInItemModel = new CategorysInItemModel();
            categorysInItemModel.categoryId = SqlDataHelper.GetValue<long>(dataReader, "Id");
            categorysInItemModel.categoryName = SqlDataHelper.GetValue<string>(dataReader, "Name");
            categorysInItemModel.categoryNameEnglish = SqlDataHelper.GetValue<string>(dataReader, "NameEnglish");

            string ItemsJson = SqlDataHelper.GetValue<string>(dataReader, "ItemsJson");
            if (!string.IsNullOrEmpty(ItemsJson))
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                categorysInItemModel.listItemInCategories = System.Text.Json.JsonSerializer.Deserialize<List<ItemDto>>(ItemsJson, options);
            }
            return categorysInItemModel;
        }

        #endregion

        #region Live Chat
        public static CustomerLiveChatModel ConvertToGetCustomerLiveChatForViewDto(IDataReader dataReader)
        {
            CustomerLiveChatModel customerLiveChat = new CustomerLiveChatModel();
            //customerLiveChat.id = SqlDataHelper.GetValue<long>(dataReader, "id");

            try
            {
                customerLiveChat.TenantId = SqlDataHelper.GetValue<int>(dataReader, "TenantId");

                customerLiveChat.IdLiveChat = SqlDataHelper.GetValue<long>(dataReader, "Id");
                customerLiveChat.userId = SqlDataHelper.GetValue<string>(dataReader, "userId");

                try
                {
                    customerLiveChat.TeamsIds = SqlDataHelper.GetValue<string>(dataReader, "TeamsIds");
                }
                catch
                {


                }
                customerLiveChat.phoneNumber = SqlDataHelper.GetValue<string>(dataReader, "phoneNumber");
                customerLiveChat.displayName = SqlDataHelper.GetValue<string>(dataReader, "displayName");
                customerLiveChat.requestedLiveChatTime = SqlDataHelper.GetValue<DateTime>(dataReader, "requestedLiveChatTime");

                try
                {
                    customerLiveChat.ContactCreationDate = SqlDataHelper.GetValue<DateTime>(dataReader, "ContactCreationDate");
                }
                catch
                {
                    customerLiveChat.ContactCreationDate = null;
                }

                try
                {
                    customerLiveChat.IsNote = SqlDataHelper.GetValue<DateTime>(dataReader, "IsNote");
                }
                catch
                {
                    customerLiveChat.IsNote = false;
                }

                try
                {
                    customerLiveChat.NumberNote = SqlDataHelper.GetValue<DateTime>(dataReader, "NumberNote");
                }
                catch
                {
                    customerLiveChat.NumberNote = 0;
                }

                customerLiveChat.requestedLiveChatTime = customerLiveChat.requestedLiveChatTime.Value.AddHours(AppSettingsModel.AddHour);

                if (customerLiveChat.requestedLiveChatTime.HasValue && customerLiveChat.requestedLiveChatTime.Value.Year < 1400)
                {
                    customerLiveChat.requestedLiveChatTime = DateTime.UtcNow;
                }


                customerLiveChat.IsliveChat = SqlDataHelper.GetValue<bool>(dataReader, "IsliveChat");
                customerLiveChat.LiveChatStatus = SqlDataHelper.GetValue<int>(dataReader, "LiveChatStatus");
                customerLiveChat.LockedByAgentName = SqlDataHelper.GetValue<string>(dataReader, "AgentName");
                customerLiveChat.Department = SqlDataHelper.GetValue<string>(dataReader, "Department");
                customerLiveChat.Department = customerLiveChat.Department.Replace("-\"\"", "");



                if (customerLiveChat.lastNotificationsData.HasValue && customerLiveChat.lastNotificationsData.Value.Year < 1400)
                {
                    customerLiveChat.lastNotificationsData = DateTime.UtcNow;
                }
                if (customerLiveChat.CreateDate.HasValue && customerLiveChat.CreateDate.Value.Year < 1400)
                {
                    customerLiveChat.CreateDate = DateTime.UtcNow;
                }
                if (customerLiveChat.ModifyDate.HasValue && customerLiveChat.ModifyDate.Value.Year < 1400)
                {
                    customerLiveChat.ModifyDate = DateTime.UtcNow;
                }
                if (customerLiveChat.LastMessageData.HasValue && customerLiveChat.LastMessageData.Value.Year < 1400)
                {
                    customerLiveChat.LastMessageData = DateTime.UtcNow;
                }
                if (customerLiveChat.LastConversationStartDateTime.HasValue && customerLiveChat.LastConversationStartDateTime.Value.Year < 1400)
                {
                    customerLiveChat.LastConversationStartDateTime = DateTime.UtcNow;
                }
                try
                {
                    customerLiveChat.UserIds = SqlDataHelper.GetValue<string>(dataReader, "UserIds");


                }
                catch
                {

                }

                try
                {
                    customerLiveChat.AssignedToUserId = SqlDataHelper.GetValue<long>(dataReader, "AssignedToUserId");


                }
                catch
                {

                }

                try
                {
                    customerLiveChat.AssignedToUserName = SqlDataHelper.GetValue<string>(dataReader, "AssignedToUserName");


                }
                catch
                {

                }

                //00000000
                try
                {


                    customerLiveChat.creation_timestamp = SqlDataHelper.GetValue<int>(dataReader, "CreationTimestamp");


                }
                catch
                {

                }

                try
                {
                    customerLiveChat.expiration_timestamp = SqlDataHelper.GetValue<int>(dataReader, "ExpirationTimestamp");


                }
                catch
                {

                }

                customerLiveChat.TicketSummary = SqlDataHelper.GetValue<string>(dataReader, "TicketSummary");
                try
                {
                    customerLiveChat.OpenTimeTicket = SqlDataHelper.GetValue<DateTime>(dataReader, "OpenTimeTicket");
                    if (customerLiveChat.OpenTimeTicket.HasValue && customerLiveChat.OpenTimeTicket.Value.Year < 1400)
                    {
                        customerLiveChat.OpenTimeTicket = DateTime.UtcNow;
                    }


                }
                catch
                {

                }
                try
                {
                    customerLiveChat.CloseTimeTicket = SqlDataHelper.GetValue<DateTime>(dataReader, "CloseTimeTicket");
                    if (customerLiveChat.CloseTimeTicket.HasValue && customerLiveChat.CloseTimeTicket.Value.Year < 1400)
                    {
                        customerLiveChat.CloseTimeTicket = DateTime.UtcNow;
                    }


                }
                catch
                {

                }

                customerLiveChat.ResolutionTicket = DataReaderMapper.CalculateResolution(customerLiveChat.OpenTimeTicket, customerLiveChat.CloseTimeTicket);


                try
                {
                    customerLiveChat.IsOpen = SqlDataHelper.GetValue<bool>(dataReader, "IsOpen");
                    customerLiveChat.agentId = SqlDataHelper.GetValue<int>(dataReader, "AgentId");
                }
                catch
                {
                    customerLiveChat.IsOpen = false;
                }


                try
                {
                    customerLiveChat.OpenTime = SqlDataHelper.GetValue<DateTime>(dataReader, "OpenTime");
                    customerLiveChat.OpenTime = customerLiveChat.OpenTime.Value.AddHours(AppSettingsModel.AddHour);
                    if (customerLiveChat.OpenTime.HasValue && customerLiveChat.OpenTime.Value.Year < 1400)
                    {
                        customerLiveChat.OpenTime = DateTime.UtcNow;
                    }
                }
                catch
                {
                    //customerLiveChat.OpenTime = false;
                }

                try
                {
                    customerLiveChat.CloseTime = SqlDataHelper.GetValue<DateTime>(dataReader, "CloseTime");


                    customerLiveChat.CloseTime = customerLiveChat.CloseTime.Value.AddHours(AppSettingsModel.AddHour);
                    if (customerLiveChat.CloseTime.HasValue && customerLiveChat.CloseTime.Value.Year < 1400)
                    {
                        customerLiveChat.CloseTime = DateTime.UtcNow;
                    }
                }
                catch
                {
                    //customerLiveChat.CloseTime = false;
                }

                //////////tickit
                try
                {
                    customerLiveChat.OpenTimeTicket = SqlDataHelper.GetValue<DateTime>(dataReader, "OpenTimeTicket");
                    customerLiveChat.OpenTimeTicket = customerLiveChat.OpenTimeTicket.Value.AddHours(AppSettingsModel.AddHour);
                    if (customerLiveChat.OpenTimeTicket.HasValue && customerLiveChat.OpenTimeTicket.Value.Year < 1400)
                    {
                        customerLiveChat.OpenTimeTicket = DateTime.UtcNow;
                    }
                }
                catch
                {
                    //customerLiveChat.OpenTime = false;
                }

                try
                {
                    customerLiveChat.CloseTimeTicket = SqlDataHelper.GetValue<DateTime>(dataReader, "CloseTimeTicket");
                    customerLiveChat.CloseTimeTicket = customerLiveChat.CloseTimeTicket.Value.AddHours(AppSettingsModel.AddHour);
                    if (customerLiveChat.CloseTimeTicket.HasValue && customerLiveChat.CloseTimeTicket.Value.Year < 1400)
                    {
                        customerLiveChat.CloseTimeTicket = DateTime.UtcNow;
                    }
                }
                catch
                {
                    //customerLiveChat.CloseTime = false;
                }


                try
                {
                    customerLiveChat.ConversationsCount = SqlDataHelper.GetValue<int>(dataReader, "ConversationsCount");
                }
                catch
                {
                    customerLiveChat.ConversationsCount = 0;
                }

                try
                {


                    customerLiveChat.IsConversationExpired = SqlDataHelper.GetValue<bool>(dataReader, "IsConversationExpired");
                }
                catch
                {
                    customerLiveChat.IsConversationExpired = false;
                }

                try
                {
                    customerLiveChat.ActionTime = SqlDataHelper.GetValue<DateTime>(dataReader, "ActionTime");
                    if (customerLiveChat.ActionTime.HasValue && customerLiveChat.ActionTime.Value.Year < 1400)
                    {
                        customerLiveChat.ActionTime = DateTime.UtcNow;
                    }
                }
                catch
                {
                }
                customerLiveChat.CategoryType = SqlDataHelper.GetValue<string>(dataReader, "CategoryType");
                if (customerLiveChat.IdLiveChat > 0 && customerLiveChat.CategoryType == "")
                {
                    customerLiveChat.CategoryType = "Ticket";
                }
                if (customerLiveChat.CategoryType == "Request")
                {
                    if (customerLiveChat.ActionTime != null && customerLiveChat.requestedLiveChatTime != null)
                    {
                        customerLiveChat.ActionTime = customerLiveChat.ActionTime.Value.AddHours(AppSettingsModel.AddHour);
                        customerLiveChat.DurationTime = (int)Math.Ceiling((customerLiveChat.ActionTime.Value - customerLiveChat.requestedLiveChatTime.Value).TotalMinutes);
                    }
                    //customerLiveChat.DurationTime = 1;
                }
                else
                {
                    if (customerLiveChat.CloseTime != null && customerLiveChat.OpenTime != null)
                    {
                        customerLiveChat.DurationTime = (int)Math.Ceiling((customerLiveChat.CloseTime.Value - customerLiveChat.OpenTime.Value).TotalMinutes);
                    }

                    if (customerLiveChat.CloseTimeTicket != null && customerLiveChat.OpenTimeTicket != null)
                    {
                        customerLiveChat.DurationTime = (int)Math.Ceiling((customerLiveChat.CloseTimeTicket.Value - customerLiveChat.OpenTimeTicket.Value).TotalMinutes);
                    }
                    if (customerLiveChat.OpenTimeTicket != null && customerLiveChat.requestedLiveChatTime != null)
                    {
                        customerLiveChat.timeToOpen = (int)Math.Ceiling((customerLiveChat.OpenTimeTicket.Value - customerLiveChat.requestedLiveChatTime.Value).TotalMinutes);
                    }
                }


                if (customerLiveChat.LiveChatStatus == 0)
                {

                    customerLiveChat.LiveChatStatusName = "Pending";
                }
                if (customerLiveChat.LiveChatStatus == 1)
                {

                    customerLiveChat.LiveChatStatusName = "Pending";
                }
                if (customerLiveChat.LiveChatStatus == 2)
                {

                    customerLiveChat.LiveChatStatusName = "Open";
                }
                if (customerLiveChat.LiveChatStatus == 3)
                {

                    customerLiveChat.LiveChatStatusName = "Done";
                }
                if (customerLiveChat.LiveChatStatus == 4) { customerLiveChat.LiveChatStatusName = "Confirm"; }
                if (customerLiveChat.LiveChatStatus == 5) { customerLiveChat.LiveChatStatusName = "Reject"; }
                if (customerLiveChat.CategoryType == "Request")
                {
                    customerLiveChat.RequestDescription = SqlDataHelper.GetValue<string>(dataReader, "RequestDescription");
                    //customerLiveChat.ContactInfo = SqlDataHelper.GetValue<string>(dataReader, "ContactInfo");
                    //customerLiveChat.IsRequestForm = SqlDataHelper.GetValue<bool>(dataReader, "IsRequestForm") ?? (bool?)false;
                    //customerLiveChat.AreaId = SqlDataHelper.GetValue<long>(dataReader, "AreaId") ?? (long?)0;

                    customerLiveChat.ContactId = SqlDataHelper.GetValue<int>(dataReader, "ContactId") ?? (int?)0;
                    //customerLiveChat.lstSellingRequestDetailsDto = SqlDataHelper.GetValue<string>(dataReader, "RequestDescription");
                    customerLiveChat.Department = customerLiveChat.RequestDescription;

                }
                if (customerLiveChat.LiveChatStatus == 6) { customerLiveChat.LiveChatStatusName = "Assigned"; }
                //if (customerLiveChat.expiration_timestamp != 0)
                //{
                //    var diff = customerLiveChat.expiration_timestamp - customerLiveChat.creation_timestamp;


                //    var offsetcreation = DateTimeOffset.FromUnixTimeSeconds(customerLiveChat.creation_timestamp);
                //    DateTime creationDate = offsetcreation.UtcDateTime;

                //    var offsetexpiration = DateTimeOffset.FromUnixTimeSeconds(customerLiveChat.expiration_timestamp);
                //    DateTime expirationDate = offsetexpiration.UtcDateTime;


                //    TimeSpan timediff = expirationDate - creationDate;
                //    int totalHoursforuser = (int)(timediff.TotalHours);


                //    if (DateTime.UtcNow <= expirationDate)
                //    {
                //        customerLiveChat.IsConversationExpired = false;
                //    }
                //    else
                //    {

                //        customerLiveChat.IsConversationExpired = true;
                //    }


                //}

                //if (customerLiveChat.expiration_timestamp==0 || customerLiveChat.creation_timestamp==0)
                //{

                //    customerLiveChat.IsConversationExpired = true;

                //}










            }
            catch
            {
                customerLiveChat.IdLiveChat = SqlDataHelper.GetValue<long>(dataReader, "Id");
                customerLiveChat.userId = SqlDataHelper.GetValue<string>(dataReader, "userId");

                try
                {
                    customerLiveChat.TeamsIds = SqlDataHelper.GetValue<string>(dataReader, "TeamsIds");


                }
                catch
                {


                }
                customerLiveChat.phoneNumber = SqlDataHelper.GetValue<string>(dataReader, "phoneNumber");
                customerLiveChat.displayName = SqlDataHelper.GetValue<string>(dataReader, "displayName");
                customerLiveChat.requestedLiveChatTime = SqlDataHelper.GetValue<DateTime>(dataReader, "requestedLiveChatTime");
                if (customerLiveChat.requestedLiveChatTime.HasValue && customerLiveChat.requestedLiveChatTime.Value.Year < 1400)
                {
                    customerLiveChat.requestedLiveChatTime = DateTime.UtcNow;
                }
                customerLiveChat.IsliveChat = SqlDataHelper.GetValue<bool>(dataReader, "IsliveChat");
                customerLiveChat.LiveChatStatus = SqlDataHelper.GetValue<int>(dataReader, "LiveChatStatus");
                customerLiveChat.LockedByAgentName = SqlDataHelper.GetValue<string>(dataReader, "AgentName");
                customerLiveChat.AssignedToUserId = SqlDataHelper.GetValue<long>(dataReader, "AssignedToUserId");
                customerLiveChat.AssignedToUserName = SqlDataHelper.GetValue<string>(dataReader, "AssignedToUserName");

                if (customerLiveChat.LiveChatStatus == 0)
                {

                    customerLiveChat.LiveChatStatusName = "Pending";
                }
                if (customerLiveChat.LiveChatStatus == 1)
                {

                    customerLiveChat.LiveChatStatusName = "Pending";
                }
                if (customerLiveChat.LiveChatStatus == 2)
                {

                    customerLiveChat.LiveChatStatusName = "Open";
                }
                if (customerLiveChat.LiveChatStatus == 3)
                {

                    customerLiveChat.LiveChatStatusName = "Done";
                }
                if (customerLiveChat.LiveChatStatus == 4)
                {
                    customerLiveChat.LiveChatStatusName = "Confirm";
                }
                if (customerLiveChat.LiveChatStatus == 5)
                {
                    customerLiveChat.LiveChatStatusName = "Reject";
                }
                customerLiveChat.CategoryType = SqlDataHelper.GetValue<string>(dataReader, "CategoryType");

                if (customerLiveChat.CategoryType == "Request")
                {
                    customerLiveChat.RequestDescription = SqlDataHelper.GetValue<string>(dataReader, "RequestDescription");
                    //customerLiveChat.ContactInfo = SqlDataHelper.GetValue<string>(dataReader, "ContactInfo");
                    //customerLiveChat.IsRequestForm = SqlDataHelper.GetValue<bool>(dataReader, "IsRequestForm");
                    //customerLiveChat.AreaId = SqlDataHelper.GetValue<long>(dataReader, "AreaId") ?? (long?)0;
                    customerLiveChat.ContactId = SqlDataHelper.GetValue<int>(dataReader, "ContactId") ?? (int?)0;
                }
            }

            //customerLiveChat.agentId = SqlDataHelper.GetValue<int>(dataReader, "AgentId");
            return customerLiveChat;
        }


        public static TickitDashModel MapGetTicketsAllDash(IDataReader dataReader)
        {
            try
            {
                TickitDashModel model = new TickitDashModel();
                model.AgentId = SqlDataHelper.GetValue<long>(dataReader, "AgentId") ?? (long?)0;
                model.TotalOpen = SqlDataHelper.GetValue<int>(dataReader, "TotalOpen") ?? (int?)0;
                model.TotalClose = SqlDataHelper.GetValue<int>(dataReader, "TotalClose") ?? (int?)0;
                model.TotalPending = SqlDataHelper.GetValue<int>(dataReader, "TotalPending") ?? (int?)0;
                model.AvgTimeMinutes = SqlDataHelper.GetValue<long>(dataReader, "AvgTimeMinutes") ?? (long?)0;

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static ExportToExcelHost MapTenantStatistics(IDataReader dataReader)
        {
            try
            {
                ExportToExcelHost model = new ExportToExcelHost();

                if (dataReader.Read())
                {
                    model.TotalTickets = SqlDataHelper.GetValue<long>(dataReader, "TotalTickets") ?? 0;
                    model.TotalPending = SqlDataHelper.GetValue<long>(dataReader, "TotalPending") ?? 0;
                    model.TotalOpened = SqlDataHelper.GetValue<long>(dataReader, "TotalOpened") ?? 0;
                    model.TotalClosed = SqlDataHelper.GetValue<long>(dataReader, "TotalClosed") ?? 0;
                    model.TotalExpired = SqlDataHelper.GetValue<long>(dataReader, "TotalExpired") ?? 0;
                    model.LastClosedTicketDate = SqlDataHelper.GetValue<DateTime>(dataReader, "LastClosedTicketDate");
                }

                dataReader.NextResult();
                if (dataReader.Read())
                {
                    model.LastMonthTotalTickets = SqlDataHelper.GetValue<long>(dataReader, "TotalTickets") ?? 0;
                    model.LastMonthTotalPending = SqlDataHelper.GetValue<long>(dataReader, "TotalPending") ?? 0;
                    model.LastMonthTotalOpened = SqlDataHelper.GetValue<long>(dataReader, "TotalOpened") ?? 0;
                    model.LastMonthTotalClosed = SqlDataHelper.GetValue<long>(dataReader, "TotalClosed") ?? 0;
                    model.LastMonthTotalExpired = SqlDataHelper.GetValue<long>(dataReader, "TotalExpired") ?? 0;
                    model.LastMonthLastClosedTicketDate = SqlDataHelper.GetValue<DateTime>(dataReader, "LastClosedTicketDate");
                }


                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static TicketsStatisticsModel MapTicketsGetStatistics(IDataReader dataReader)
        {
            try
            {
                TicketsStatisticsModel model = new TicketsStatisticsModel();
                model.TotalTickets = SqlDataHelper.GetValue<long>(dataReader, "TotalTickets") ?? (long?)0;
                model.TotalPending = SqlDataHelper.GetValue<long>(dataReader, "TotalPending") ?? (long?)0;
                model.TotalOpened = SqlDataHelper.GetValue<long>(dataReader, "TotalOpened") ?? (long?)0;
                model.TotalClosed = SqlDataHelper.GetValue<long>(dataReader, "TotalClosed") ?? (long?)0;
                model.TotalExpired = SqlDataHelper.GetValue<long>(dataReader, "TotalExpired") ?? (long?)0;
                //model.AvgResolutionTime = SqlDataHelper.GetValue<decimal>(dataReader, "AvgResolutionTime") ?? (decimal?)0;
                model.LastClosedTicketDate = SqlDataHelper.GetValue<DateTime>(dataReader, "LastClosedTicketDate");

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static TicketsStatisticsModel MapTicketsGetStatistics2(IDataReader dataReader)
        {
            try
            {
                TicketsStatisticsModel model = new TicketsStatisticsModel();
                model.TotalTickets = SqlDataHelper.GetValue<long>(dataReader, "TotalTickets") ?? (long?)0;
                model.TotalPending = SqlDataHelper.GetValue<long>(dataReader, "TotalPending") ?? (long?)0;
                model.TotalOpened = SqlDataHelper.GetValue<long>(dataReader, "TotalOpened") ?? (long?)0;
                model.TotalClosed = SqlDataHelper.GetValue<long>(dataReader, "TotalClosed") ?? (long?)0;
                model.TotalExpired = SqlDataHelper.GetValue<long>(dataReader, "TotalExpired") ?? (long?)0;
                //model.AvgResolutionTime = SqlDataHelper.GetValue<decimal>(dataReader, "AvgResolutionTime") ?? (decimal?)0;

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static string CalculateResolution(DateTime? openTime, DateTime? closeTime)
        {
            if (openTime == null || closeTime == null)
            {
                return "";
            }

            TimeSpan resolutionTime = closeTime.Value - openTime.Value;

            if (resolutionTime.TotalSeconds < 0)
            {
                return "";
            }

            if (resolutionTime.Days > 0)
            {
                return $"{resolutionTime.Days} days, {(int)resolutionTime.TotalHours % 24} hours, {resolutionTime.Minutes} minutes";
            }
            else if (resolutionTime.TotalHours == 0 && resolutionTime.Days == 0 && resolutionTime.Minutes == 0)
            {
                return $"{resolutionTime.Seconds} Seconds";
            }
            else if (resolutionTime.TotalHours == 0 && resolutionTime.Days == 0 && resolutionTime.Minutes > 0)
            {
                return $"{resolutionTime.TotalMinutes} Minutes";
            }
            else if (resolutionTime.TotalHours > 0)
            {
                return $"{(int)resolutionTime.TotalHours} hours, {resolutionTime.Minutes} minutes";
            }
            else
            {
                return $"{resolutionTime.Minutes} minutes";
            }
        }


        public static LiveChatModel ConvertLiveChatDto(IDataReader dataReader)
        {
            LiveChatModel chat = new LiveChatModel();

            chat.UserId = SqlDataHelper.GetValue<string>(dataReader, "userId");
            // Handle NULL values safely
            chat.RequestedLiveChatTime = dataReader["requestedLiveChatTime"] != DBNull.Value
                ? Convert.ToDateTime(dataReader["requestedLiveChatTime"])
                : DateTime.UtcNow; // Default value if NULL

            return chat;
        }

        #endregion

        #region Permissions
        public static string ConvertToGetPermissions(IDataReader dataReader)
        {

            return SqlDataHelper.GetValue<string>(dataReader, "Name");
        }
        #endregion

        #region Tenant 
        public static TenantInfoForOrdaringSystemDto ConvertTenantInformationDto(IDataReader dataReader)
        {
            TenantInfoForOrdaringSystemDto tenantInfoDto = new TenantInfoForOrdaringSystemDto();
            tenantInfoDto.Id = SqlDataHelper.GetValue<int>(dataReader, "Id");
            tenantInfoDto.PhoneNumber = SqlDataHelper.GetValue<string>(dataReader, "PhoneNumber");
            tenantInfoDto.Name = SqlDataHelper.GetValue<string>(dataReader, "Name");
            tenantInfoDto.LogoImag = SqlDataHelper.GetValue<string>(dataReader, "Image");
            tenantInfoDto.BgImag = SqlDataHelper.GetValue<string>(dataReader, "ImageBg");
            tenantInfoDto.CurrencyCode = SqlDataHelper.GetValue<string>(dataReader, "CurrencyCode");
            tenantInfoDto.IsApplyLoyalty = SqlDataHelper.GetValue<bool>(dataReader, "IsApplyLoyalty") != null ? SqlDataHelper.GetValue<bool>(dataReader, "IsApplyLoyalty") : false;

            tenantInfoDto.OriginalLoyaltyPoints = SqlDataHelper.GetValue<decimal>(dataReader, "OriginalLoyaltyPoints") != null ? SqlDataHelper.GetValue<decimal>(dataReader, "OriginalLoyaltyPoints") : 0;
            tenantInfoDto.DisplayName = SqlDataHelper.GetValue<string>(dataReader, "DisplayName");

            tenantInfoDto.OrderType = SqlDataHelper.GetValue<string>(dataReader, "OrderType");

            tenantInfoDto.CurrencyCode = SqlDataHelper.GetValue<string>(dataReader, "CurrencyCode");

            tenantInfoDto.StartDate = SqlDataHelper.GetValue<DateTime>(dataReader, "StartDate") != null ? SqlDataHelper.GetValue<DateTime>(dataReader, "StartDate") : DateTime.MinValue;
            tenantInfoDto.EndDate = SqlDataHelper.GetValue<DateTime>(dataReader, "EndDate") != null ? SqlDataHelper.GetValue<DateTime>(dataReader, "EndDate") : DateTime.MinValue;

            return tenantInfoDto;
        }

        public static ExportToExcelHost GetExportToExcelHostLastDay(IDataReader dataReader)
        {
            ExportToExcelHost model = new ExportToExcelHost();

            try
            {
                model.TenantName = SqlDataHelper.GetValue<string>(dataReader, "TenantName");
                model.Name = SqlDataHelper.GetValue<string>(dataReader, "Name");
                model.TenancyName = SqlDataHelper.GetValue<string>(dataReader, "TenancyName");
                model.IsActive = SqlDataHelper.GetValue<bool>(dataReader, "IsActive");
                model.Integration = SqlDataHelper.GetValue<bool?>(dataReader, "Integration");
                model.CreatedBy = SqlDataHelper.GetValue<string>(dataReader, "CreatedBy");
                model.CreationTime = SqlDataHelper.GetValue<DateTime>(dataReader, "CreationTime");
                model.InvoiceId = SqlDataHelper.GetValue<string>(dataReader, "InvoiceId");
                model.PhoneNumber = SqlDataHelper.GetValue<string>(dataReader, "PhoneNumber");
                model.DomainName = SqlDataHelper.GetValue<string>(dataReader, "DomainName");
                model.CustomerName = SqlDataHelper.GetValue<string>(dataReader, "CustomerName");

                model.TotalTickets = SqlDataHelper.GetValue<long>(dataReader, "TotalTickets");
                model.TotalPending = SqlDataHelper.GetValue<long>(dataReader, "TotalPending");
                model.TotalOpened = SqlDataHelper.GetValue<long>(dataReader, "TotalOpened");
                model.TotalClosed = SqlDataHelper.GetValue<long>(dataReader, "TotalClosed");
                model.TotalExpired = SqlDataHelper.GetValue<long>(dataReader, "TotalExpired");
                model.AvgResolutionTime = SqlDataHelper.GetValue<decimal>(dataReader, "AvgResolutionTime");
                model.LastClosedTicketDate = SqlDataHelper.GetValue<DateTime>(dataReader, "LastClosedTicketDate");

                model.LastMonthTotalTickets = SqlDataHelper.GetValue<long>(dataReader, "LastMonthTotalTickets");
                model.LastMonthTotalPending = SqlDataHelper.GetValue<long>(dataReader, "LastMonthTotalPending");
                model.LastMonthTotalOpened = SqlDataHelper.GetValue<long>(dataReader, "LastMonthTotalOpened");
                model.LastMonthTotalClosed = SqlDataHelper.GetValue<long>(dataReader, "LastMonthTotalClosed");
                model.LastMonthTotalExpired = SqlDataHelper.GetValue<long>(dataReader, "LastMonthTotalExpired");
                model.LastMonthLastClosedTicketDate = SqlDataHelper.GetValue<DateTime>(dataReader, "LastMonthLastClosedTicketDate");

                model.WalletBalance = (decimal?)Convert.ToDecimal(SqlDataHelper.GetValue<double?>(dataReader, "WalletBalance"));

                model.TotalOrder = SqlDataHelper.GetValue<long>(dataReader, "TotalOrders");
                model.TotalOrderPending = SqlDataHelper.GetValue<long>(dataReader, "PendingOrders");
                model.DoneOrders = SqlDataHelper.GetValue<long>(dataReader, "DoneOrders");
                model.TotalOrderDeleted = SqlDataHelper.GetValue<long>(dataReader, "DeletedOrders");
                model.TotalOrderCanceled = SqlDataHelper.GetValue<long>(dataReader, "CanceledOrders");
                model.TotalOrderPreOrder = SqlDataHelper.GetValue<long>(dataReader, "PreOrders");

                model.LastMonthTotalOrders = SqlDataHelper.GetValue<long>(dataReader, "LastMonthTotalOrders");
                model.LastMonthPendingOrders = SqlDataHelper.GetValue<long>(dataReader, "LastMonthPendingOrders");
                model.LastMonthDoneOrders = SqlDataHelper.GetValue<long>(dataReader, "LastMonthDoneOrders");
                model.LastMonthDeletedOrders = SqlDataHelper.GetValue<long>(dataReader, "LastMonthDeletedOrders");
                model.LastMonthCanceledOrders = SqlDataHelper.GetValue<long>(dataReader, "LastMonthCanceledOrders");
                model.LastMonthPreOrders = SqlDataHelper.GetValue<long>(dataReader, "LastMonthPreOrders");

                return model;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public static TenantsModel MapTenant(IDataReader dataReader)
        {
            try
            {
                TenantsModel tenant = new TenantsModel();
                tenant.TenantId = SqlDataHelper.GetValue<int>(dataReader, "Id");
                tenant.PhoneNumber = SqlDataHelper.GetValue<string>(dataReader, "PhoneNumber");
                tenant.AccessToken = SqlDataHelper.GetValue<string>(dataReader, "AccessToken");
                tenant.IsBotActive = SqlDataHelper.GetValue<bool>(dataReader, "IsBotActive");
                tenant.WhatsAppAccountID = SqlDataHelper.GetValue<string>(dataReader, "WhatsAppAccountID");

                tenant.IsLoyalityPoint = SqlDataHelper.GetValue<bool>(dataReader, "IsLoyalityPoint");
                tenant.D360Key = SqlDataHelper.GetValue<string>(dataReader, "D360Key");
                tenant.ZohoCustomerId = SqlDataHelper.GetValue<string>(dataReader, "ZohoCustomerId");
                tenant.UnAvailableBookingDates = SqlDataHelper.GetValue<string>(dataReader, "UnAvailableBookingDates");
                tenant.UnAvailableBookingDates = SqlDataHelper.GetValue<string>(dataReader, "UnAvailableBookingDates");
                tenant.ConfirmRequestText = SqlDataHelper.GetValue<string>(dataReader, "ConfirmRequestText");
                tenant.RejectRequestText = SqlDataHelper.GetValue<string>(dataReader, "RejectRequestText");

                try
                {
                    tenant.IsReplyAfterHumanHandOver = SqlDataHelper.GetValue<bool>(dataReader, "IsReplyAfterHumanHandOver");
                }
                catch
                {
                    tenant.IsReplyAfterHumanHandOver = true;
                }

                try
                {
                    tenant.CautionDays = SqlDataHelper.GetValue<int>(dataReader, "CautionDays");
                    tenant.WarningDays = SqlDataHelper.GetValue<int>(dataReader, "WarningDays");
                }
                catch
                {

                }
                try
                {
                    tenant.IsD360Dialog = SqlDataHelper.GetValue<bool>(dataReader, "IsD360Dialog");

                }
                catch
                {

                }

                return tenant;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public static TenantsModel MapTenantGet(IDataReader dataReader)
        {
            try
            {
                TenantsModel tenant = new TenantsModel();

                tenant.DomainName = SqlDataHelper.GetValue<string>(dataReader, "TenancyName");
                tenant.CustomerName = SqlDataHelper.GetValue<string>(dataReader, "CommercialName");
                tenant.IsActive = SqlDataHelper.GetValue<bool>(dataReader, "IsActive");
                tenant.PhoneNumber = SqlDataHelper.GetValue<string>(dataReader, "PhoneNumber");
                tenant.CreationTime = SqlDataHelper.GetValue<DateTime>(dataReader, "CreationTime");

                try
                {
                    tenant.IsReplyAfterHumanHandOver = SqlDataHelper.GetValue<bool>(dataReader, "IsReplyAfterHumanHandOver");
                }
                catch
                {
                    tenant.IsReplyAfterHumanHandOver = true;  // default value
                }

                return tenant;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public static TenantsToExcelDto MapTenantsInfo(IDataReader dataReader)
        {
            try
            {
                TenantsToExcelDto tenant = new TenantsToExcelDto();

                tenant.TenantId = SqlDataHelper.GetValue<int>(dataReader, "Id");
                tenant.TenantName = SqlDataHelper.GetValue<string>(dataReader, "Name");
                tenant.PhoneNumber = SqlDataHelper.GetValue<string>(dataReader, "PhoneNumber");
                tenant.TotalFreeConversation = SqlDataHelper.GetValue<decimal>(dataReader, "TotalFreeConversationWA");
                tenant.RemainingFreeConversation = SqlDataHelper.GetValue<decimal>(dataReader, "RemainingFreeConversation");
                tenant.TotalUIConversation = SqlDataHelper.GetValue<decimal>(dataReader, "TotalUIConversation");
                tenant.RemainingUIConversation = SqlDataHelper.GetValue<decimal>(dataReader, "RemainingUIConversation");
                tenant.TotalBIConversation = SqlDataHelper.GetValue<decimal>(dataReader, "TotalBIConversation");
                tenant.RemainingBIConversation = SqlDataHelper.GetValue<decimal>(dataReader, "RemainingBIConversation");

                return tenant;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public static TenantEditDto MapSettingsTenantHost(IDataReader dataReader)
        {
            try
            {
                TenantEditDto tenant = new TenantEditDto();

                tenant.TimeZoneId = SqlDataHelper.GetValue<string>(dataReader, "TimeZoneId");
                tenant.ZohoCustomerId = SqlDataHelper.GetValue<string>(dataReader, "ZohoCustomerId");
                tenant.CautionDays = SqlDataHelper.GetValue<int>(dataReader, "CautionDays");
                tenant.WarningDays = SqlDataHelper.GetValue<int>(dataReader, "WarningDays");
                tenant.IsDelivery = SqlDataHelper.GetValue<bool>(dataReader, "IsDelivery");
                tenant.IsPickup = SqlDataHelper.GetValue<bool>(dataReader, "IsPickup");
                tenant.IsPreOrder = SqlDataHelper.GetValue<bool>(dataReader, "IsPreOrder");
                tenant.Currency = SqlDataHelper.GetValue<string>(dataReader, "CurrencyName");
                tenant.CurrencyCode = SqlDataHelper.GetValue<string>(dataReader, "CurrencyCode");

                try
                {
                    tenant.ClientIpAddress = SqlDataHelper.GetValue<string>(dataReader, "ClientIpAddress");

                }
                catch
                {
                    tenant.ClientIpAddress = null;

                }


                return tenant;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public static HostTenantListDto MapHostTenantsInfo(IDataReader dataReader)
        {
            try
            {
                HostTenantListDto tenant = new HostTenantListDto();
                tenant.Id = SqlDataHelper.GetValue<int>(dataReader, "Id");
                tenant.Name = SqlDataHelper.GetValue<string>(dataReader, "Name");
                tenant.TenancyName = SqlDataHelper.GetValue<string>(dataReader, "TenancyName");
                tenant.IsActive = SqlDataHelper.GetValue<bool>(dataReader, "IsActive");
                tenant.CreatedBy = SqlDataHelper.GetValue<string>(dataReader, "CreatedBy");
                tenant.CreationTime = SqlDataHelper.GetValue<DateTime>(dataReader, "CreationTime");
                tenant.InvoiceId = SqlDataHelper.GetValue<string>(dataReader, "InvoiceId");
                tenant.Integration = SqlDataHelper.GetValue<string>(dataReader, "Integration");
                tenant.CustomerName = SqlDataHelper.GetValue<string>(dataReader, "CommercialName");
                tenant.PhoneNumber = SqlDataHelper.GetValue<string>(dataReader, "PhoneNumber");



                try
                {

                    tenant.PhoneNumber = SqlDataHelper.GetValue<string>(dataReader, "PhoneNumber");

                }
                catch
                {


                }

                return tenant;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static ConversationMeasurements MapTenantConversationMeasurements(IDataReader dataReader)
        {
            try
            {
                ConversationMeasurements conversationMeasurements = new ConversationMeasurements
                {
                    TenantId = SqlDataHelper.GetValue<int>(dataReader, "TenantId"),
                    TotalUIConversation = SqlDataHelper.GetValue<decimal>(dataReader, "TotalUIConversation"),
                    TotalUsageUIConversation = SqlDataHelper.GetValue<decimal>(dataReader, "TotalUsageUIConversation"),
                    TotalBIConversation = SqlDataHelper.GetValue<decimal>(dataReader, "TotalBIConversation"),
                    TotalUsageBIConversation = SqlDataHelper.GetValue<decimal>(dataReader, "TotalUsageBIConversation"),
                    TotalMarketingBIConversation = SqlDataHelper.GetValue<decimal>(dataReader, "TotalMarketingBIConversation"),
                    TotalUsageMarketingBIConversation = SqlDataHelper.GetValue<decimal>(dataReader, "TotalUsageMarketingBIConversation"),
                    TotalUtilityBIConversation = SqlDataHelper.GetValue<decimal>(dataReader, "TotalUtilityBIConversation"),
                    TotalUsageUtilityBIConversation = SqlDataHelper.GetValue<decimal>(dataReader, "TotalUsageUtilityBIConversation")
                };

                return conversationMeasurements;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static TenantInfoDto MapGetTenantInfo(IDataReader dataReader)
        {
            TenantInfoDto model = new TenantInfoDto();
            model.TenantId = SqlDataHelper.GetValue<int>(dataReader, "Id");
            model.AccessToken = SqlDataHelper.GetValue<string>(dataReader, "AccessToken");
            model.D360Key = SqlDataHelper.GetValue<string>(dataReader, "D360Key");

            return model;
        }
        #endregion

        #region WhatsApp
        public static MessageTemplateModel MapTemplate(IDataReader dataReader)
        {
            MessageTemplateModel _MessageTemplateModel = new MessageTemplateModel();
            _MessageTemplateModel.name = SqlDataHelper.GetValue<string>(dataReader, "Name");
            _MessageTemplateModel.language = SqlDataHelper.GetValue<string>(dataReader, "Language");
            _MessageTemplateModel.category = SqlDataHelper.GetValue<string>(dataReader, "Category");
            _MessageTemplateModel.sub_category = SqlDataHelper.GetValue<string>(dataReader, "sub_category");

            var components = SqlDataHelper.GetValue<string>(dataReader, "Components");
            var options = new JsonSerializerOptions { WriteIndented = true };

            _MessageTemplateModel.components = JsonSerializer.Deserialize<List<WhatsAppComponentModel>>(components, options);
            _MessageTemplateModel.id = SqlDataHelper.GetValue<string>(dataReader, "WhatsAppTemplateId");
            _MessageTemplateModel.LocalTemplateId = SqlDataHelper.GetValue<long>(dataReader, "Id");
            _MessageTemplateModel.mediaType = SqlDataHelper.GetValue<string>(dataReader, "MediaType");
            _MessageTemplateModel.mediaLink = SqlDataHelper.GetValue<string>(dataReader, "MediaLink");
            _MessageTemplateModel.isDeleted = SqlDataHelper.GetValue<bool>(dataReader, "IsDeleted");


            if (_MessageTemplateModel.category == "AUTHENTICATION")
            {

                _MessageTemplateModel.VariableCount = 1;
            }
            else
            {
                _MessageTemplateModel.VariableCount = SqlDataHelper.GetValue<int>(dataReader, "VariableCount");
            }



            _MessageTemplateModel.BtnOneActionId = SqlDataHelper.GetValue<long>(dataReader, "BtnOneActionId");
            _MessageTemplateModel.BtnTwoActionId = SqlDataHelper.GetValue<long>(dataReader, "BtnTwoActionId");
            _MessageTemplateModel.BtnThreeActionId = SqlDataHelper.GetValue<long>(dataReader, "BtnThreeActionId");

            return _MessageTemplateModel;
        }
        public static WhatsAppCampaignModel MapCampaign(IDataReader dataReader)
        {
            try
            {
                WhatsAppCampaignModel _WhatsAppCampaignModel = new WhatsAppCampaignModel();
                _WhatsAppCampaignModel.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");
                _WhatsAppCampaignModel.Title = SqlDataHelper.GetValue<string>(dataReader, "Title");
                _WhatsAppCampaignModel.Language = SqlDataHelper.GetValue<string>(dataReader, "Language");
                _WhatsAppCampaignModel.Sent = SqlDataHelper.GetValue<long>(dataReader, "NumberOfSent");
                _WhatsAppCampaignModel.Read = SqlDataHelper.GetValue<long>(dataReader, "NumberOfRead");
                _WhatsAppCampaignModel.Delivered = SqlDataHelper.GetValue<long>(dataReader, "NumberOfDelivered");
                _WhatsAppCampaignModel.Failed = SqlDataHelper.GetValue<long>(dataReader, "NumberOfFailed");
                _WhatsAppCampaignModel.Replied = SqlDataHelper.GetValue<long>(dataReader, "NumberOfReplied");
                _WhatsAppCampaignModel.Hanged = SqlDataHelper.GetValue<long>(dataReader, "NumberOfHanged");
                _WhatsAppCampaignModel.TemplateId = SqlDataHelper.GetValue<long>(dataReader, "TemplateId");
                _WhatsAppCampaignModel.Status = SqlDataHelper.GetValue<int>(dataReader, "Status");
                _WhatsAppCampaignModel.SentTime = SqlDataHelper.GetValue<DateTime>(dataReader, "SentTime");
                _WhatsAppCampaignModel.CreatedDate = SqlDataHelper.GetValue<DateTime>(dataReader, "CreatedDate");

                try
                {
                    _WhatsAppCampaignModel.Type = SqlDataHelper.GetValue<int>(dataReader, "Type");

                }
                catch
                {
                    _WhatsAppCampaignModel.Type = 2;
                }


                try
                {
                    _WhatsAppCampaignModel.TemplateName = SqlDataHelper.GetValue<string>(dataReader, "TemplateName");

                    _WhatsAppCampaignModel.UserName = SqlDataHelper.GetValue<string>(dataReader, "UserName");
                }
                catch
                {

                }


                _WhatsAppCampaignModel.TotalNumbers = _WhatsAppCampaignModel.Sent + _WhatsAppCampaignModel.Failed;
                if (_WhatsAppCampaignModel.TotalNumbers > 0)
                {
                    _WhatsAppCampaignModel.SentPercentage = ((float)_WhatsAppCampaignModel.Sent / _WhatsAppCampaignModel.TotalNumbers) * 100;
                    _WhatsAppCampaignModel.DeliveredPercentage = ((float)_WhatsAppCampaignModel.Delivered / _WhatsAppCampaignModel.TotalNumbers) * 100;
                    _WhatsAppCampaignModel.ReadPercentage = ((float)_WhatsAppCampaignModel.Read / _WhatsAppCampaignModel.TotalNumbers) * 100;
                    _WhatsAppCampaignModel.RepliedPercentage = ((float)_WhatsAppCampaignModel.Replied / _WhatsAppCampaignModel.TotalNumbers) * 100;
                    _WhatsAppCampaignModel.HangedPercentage = ((float)_WhatsAppCampaignModel.Hanged / _WhatsAppCampaignModel.TotalNumbers) * 100;
                    _WhatsAppCampaignModel.FailedPercentage = ((float)_WhatsAppCampaignModel.Failed / _WhatsAppCampaignModel.TotalNumbers) * 100;
                }


                if (_WhatsAppCampaignModel.CreatedDate != null)
                {
                    _WhatsAppCampaignModel.CreatedDate = _WhatsAppCampaignModel.CreatedDate.Value.AddHours(AppSettingsModel.AddHour);
                }
                if (_WhatsAppCampaignModel.SentTime != null)
                {
                    _WhatsAppCampaignModel.SentTime = _WhatsAppCampaignModel.SentTime.Value.AddHours(AppSettingsModel.AddHour);
                }

                return _WhatsAppCampaignModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public static WhatsAppCampaignHistoryModel MapCampaignHistory(IDataReader dataReader)
        {
            try
            {
                WhatsAppCampaignHistoryModel _WhatsAppCampaignHistoryModel = new WhatsAppCampaignHistoryModel();

                _WhatsAppCampaignHistoryModel.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");
                _WhatsAppCampaignHistoryModel.CampaignId = SqlDataHelper.GetValue<long>(dataReader, "CampaignId");
                _WhatsAppCampaignHistoryModel.SentByUserId = SqlDataHelper.GetValue<long>(dataReader, "SentByUserId");
                _WhatsAppCampaignHistoryModel.SentTime = SqlDataHelper.GetValue<DateTime>(dataReader, "SentTime");
                if (_WhatsAppCampaignHistoryModel.SentTime != null)
                {
                    _WhatsAppCampaignHistoryModel.SentTime = _WhatsAppCampaignHistoryModel.SentTime.AddHours(AppSettingsModel.AddHour);
                }
                _WhatsAppCampaignHistoryModel.CampaignType = SqlDataHelper.GetValue<int>(dataReader, "CampaignType");
                _WhatsAppCampaignHistoryModel.TemplateId = SqlDataHelper.GetValue<long>(dataReader, "TemplateId");
                _WhatsAppCampaignHistoryModel.TenantId = SqlDataHelper.GetValue<int>(dataReader, "TenantId");
                _WhatsAppCampaignHistoryModel.UserName = SqlDataHelper.GetValue<string>(dataReader, "Name");
                _WhatsAppCampaignHistoryModel.Sent = SqlDataHelper.GetValue<long>(dataReader, "NumberOfSent");
                _WhatsAppCampaignHistoryModel.Read = SqlDataHelper.GetValue<long>(dataReader, "NumberOfRead");
                _WhatsAppCampaignHistoryModel.Delivered = SqlDataHelper.GetValue<long>(dataReader, "NumberOfDelivered");
                _WhatsAppCampaignHistoryModel.Failed = SqlDataHelper.GetValue<long>(dataReader, "NumberOfFailed");

                return _WhatsAppCampaignHistoryModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public static WhatsAppFreeMessageModel MapWhatsAppFreeMessage(IDataReader dataReader)
        {
            try
            {
                WhatsAppFreeMessageModel _WhatsAppFreeMessageModel = new WhatsAppFreeMessageModel();
                _WhatsAppFreeMessageModel.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");
                _WhatsAppFreeMessageModel.FreeMessage = SqlDataHelper.GetValue<string>(dataReader, "FreeMessage");
                _WhatsAppFreeMessageModel.FreeMessageType = SqlDataHelper.GetValue<string>(dataReader, "FreeMessageType");
                _WhatsAppFreeMessageModel.CampaingStatusId = SqlDataHelper.GetValue<int>(dataReader, "Status");
                _WhatsAppFreeMessageModel.Sent = SqlDataHelper.GetValue<long>(dataReader, "NumberOfSent");
                _WhatsAppFreeMessageModel.Read = SqlDataHelper.GetValue<long>(dataReader, "NumberOfRead");
                _WhatsAppFreeMessageModel.Delivered = SqlDataHelper.GetValue<long>(dataReader, "NumberOfDelivered");
                _WhatsAppFreeMessageModel.Failed = SqlDataHelper.GetValue<long>(dataReader, "NumberOfFailed");
                _WhatsAppFreeMessageModel.SentTime = SqlDataHelper.GetValue<DateTime>(dataReader, "SentTime");
                if (_WhatsAppFreeMessageModel.SentTime != null)
                {
                    _WhatsAppFreeMessageModel.SentTime = _WhatsAppFreeMessageModel.SentTime.Value.AddHours(AppSettingsModel.AddHour);
                }
                _WhatsAppFreeMessageModel.TenantId = SqlDataHelper.GetValue<int>(dataReader, "TenantId");
                return _WhatsAppFreeMessageModel;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public static WhatsAppContactsDto MapFilterContacts(IDataReader dataReader)
        {
            WhatsAppContactsDto contacts = new WhatsAppContactsDto();
            contacts.Id = SqlDataHelper.GetValue<int>(dataReader, "id");
            contacts.ContactName = SqlDataHelper.GetValue<string>(dataReader, "DisplayName");
            contacts.PhoneNumber = SqlDataHelper.GetValue<string>(dataReader, "PhoneNumber");
            contacts.CustomerOPT = SqlDataHelper.GetValue<string>(dataReader, "CustomerOPT");

            return contacts;
        }
        public static ListContactToCampin MapNewFilterContacts(IDataReader dataReader)
        {
            ListContactToCampin contacts = new ListContactToCampin();
            contacts.Id = SqlDataHelper.GetValue<int>(dataReader, "id");
            contacts.ContactName = SqlDataHelper.GetValue<string>(dataReader, "DisplayName");
            contacts.PhoneNumber = SqlDataHelper.GetValue<string>(dataReader, "PhoneNumber");
            contacts.CustomerOPT = SqlDataHelper.GetValue<int>(dataReader, "CustomerOPT");

            return contacts;
        }
        public static ListContactToCampin MaContactsFromGrouyps(IDataReader dataReader)
        {
            ListContactToCampin contacts = new ListContactToCampin();
            contacts.Id = SqlDataHelper.GetValue<int>(dataReader, "id");
            contacts.ContactName = SqlDataHelper.GetValue<string>(dataReader, "DisplayName");
            contacts.PhoneNumber = SqlDataHelper.GetValue<string>(dataReader, "PhoneNumber");
            contacts.CustomerOPT = SqlDataHelper.GetValue<int>(dataReader, "CustomerOPT");
            string jsonString = SqlDataHelper.GetValue<string>(dataReader, "Variables");
            if (!string.IsNullOrWhiteSpace(jsonString))
            {
                try
                {
                    contacts.variables = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString);
                }
                catch (JsonException)
                {
                    contacts.variables = new Dictionary<string, string>();
                }
            }

            return contacts;
        }
        public static ListContactToCampin MapnewExternalContacts(IDataReader dataReader)
        {
            ListContactToCampin contacts = new ListContactToCampin();
            contacts.Id = SqlDataHelper.GetValue<int>(dataReader, "Id");
            //contacts.CustomerOPT = SqlDataHelper.GetValue<int>(dataReader, "CustomerOPT");
            contacts.ContactName = SqlDataHelper.GetValue<string>(dataReader, "ContactName");
            contacts.PhoneNumber = SqlDataHelper.GetValue<string>(dataReader, "PhoneNumber");

            return contacts;
        }
        public static DailylimitCount MapDailylimitCount(IDataReader dataReader)
        {
            DailylimitCount model = new DailylimitCount();
            try
            {
                model.BIDailyLimit = SqlDataHelper.GetValue<int>(dataReader, "BIDailyLimit");
            }
            catch
            {
                model.BIDailyLimit = 0;
            }
            try
            {
                model.DailyLimit = SqlDataHelper.GetValue<int>(dataReader, "DailyLimit");

            }
            catch
            {
                model.DailyLimit = 0;
            }



            return model;
        }
        public static WhatsAppContactsDto MapExternalContacts(IDataReader dataReader)
        {
            WhatsAppContactsDto contacts = new WhatsAppContactsDto();
            contacts.Id = SqlDataHelper.GetValue<int>(dataReader, "Id");
            //contacts.CustomerOPT = SqlDataHelper.GetValue<int>(dataReader, "CustomerOPT");
            contacts.ContactName = SqlDataHelper.GetValue<string>(dataReader, "ContactName");
            contacts.PhoneNumber = SqlDataHelper.GetValue<string>(dataReader, "PhoneNumber");

            return contacts;
        }
        public static ListContactToCampin MapExternalContactsForTable(IDataReader dataReader)
        {
            ListContactToCampin contacts = new ListContactToCampin();
            contacts.Id = SqlDataHelper.GetValue<int>(dataReader, "Id");
            //contacts.CustomerOPT = SqlDataHelper.GetValue<int>(dataReader, "CustomerOPT");
            contacts.ContactName = SqlDataHelper.GetValue<string>(dataReader, "ContactName");
            contacts.PhoneNumber = SqlDataHelper.GetValue<string>(dataReader, "PhoneNumber");

            return contacts;
        }
        public static WhatsAppScheduledCampaign MapScheduledCampaign(IDataReader dataReader)
        {
            WhatsAppScheduledCampaign _WhatsAppScheduledCampaign = new WhatsAppScheduledCampaign();
            _WhatsAppScheduledCampaign.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");
            _WhatsAppScheduledCampaign.CampaignId = SqlDataHelper.GetValue<long>(dataReader, "CampaignId");
            _WhatsAppScheduledCampaign.TemplateId = SqlDataHelper.GetValue<long>(dataReader, "TemplateId");
            _WhatsAppScheduledCampaign.SendDateTime = SqlDataHelper.GetValue<DateTime>(dataReader, "SendDateTime");
            if (_WhatsAppScheduledCampaign.SendDateTime != null)
            {
                _WhatsAppScheduledCampaign.SendDateTime = _WhatsAppScheduledCampaign.SendDateTime.AddHours(AppSettingsModel.AddHour);
            }
            _WhatsAppScheduledCampaign.StatusId = SqlDataHelper.GetValue<int>(dataReader, "StatusId");
            _WhatsAppScheduledCampaign.IsActive = SqlDataHelper.GetValue<bool>(dataReader, "IsActive");
            _WhatsAppScheduledCampaign.IsRecurrence = SqlDataHelper.GetValue<bool>(dataReader, "IsRecurrence");
            _WhatsAppScheduledCampaign.IsLatest = SqlDataHelper.GetValue<bool>(dataReader, "IsLatest");
            _WhatsAppScheduledCampaign.IsFreeMessage = SqlDataHelper.GetValue<bool>(dataReader, "IsFreeMessage");
            _WhatsAppScheduledCampaign.CreatedDate = SqlDataHelper.GetValue<DateTime>(dataReader, "CreatedDate");
            if (_WhatsAppScheduledCampaign.CreatedDate != null)
            {
                _WhatsAppScheduledCampaign.CreatedDate = _WhatsAppScheduledCampaign.CreatedDate.AddHours(AppSettingsModel.AddHour);
            }
            _WhatsAppScheduledCampaign.CreatedByUserId = SqlDataHelper.GetValue<long>(dataReader, "CreatedByUserId");
            _WhatsAppScheduledCampaign.ContactsJson = SqlDataHelper.GetValue<string>(dataReader, "ContactsJson");
            return _WhatsAppScheduledCampaign;
        }
        public static GetAllDashboard MapDashboardStatistic(IDataReader dataReader)
        {
            GetAllDashboard GetAllDashboard = new GetAllDashboard();
            GetAllDashboard.TotalOfAllContact = SqlDataHelper.GetValue<int>(dataReader, "TotalOfAllContact");
            GetAllDashboard.TotalOfOrders = SqlDataHelper.GetValue<int>(dataReader, "TotalOfOrders");
            GetAllDashboard.Bandel = SqlDataHelper.GetValue<int>(dataReader, "ConversationBundle");
            GetAllDashboard.RemainingConversation = SqlDataHelper.GetValue<int>(dataReader, "RemainingConversation");
            GetAllDashboard.TotalOfRating = SqlDataHelper.GetValue<double>(dataReader, "TotalOfRating");

            GetAllDashboard.TotalFreeConversationWA = SqlDataHelper.GetValue<decimal>(dataReader, "TotalFreeConversationWA");
            GetAllDashboard.TotalUsageFreeConversationWA = SqlDataHelper.GetValue<int>(dataReader, "TotalUsageFreeConversationWA");
            GetAllDashboard.TotalUsageFreeUIWA = SqlDataHelper.GetValue<int>(dataReader, "TotalUsageFreeUIWA");
            GetAllDashboard.TotalUsageFreeBIWA = SqlDataHelper.GetValue<int>(dataReader, "TotalUsageFreeBIWA");

            GetAllDashboard.TotalUsagePaidConversationWA = SqlDataHelper.GetValue<int>(dataReader, "TotalUsagePaidConversationWA");
            GetAllDashboard.TotalUsagePaidUIWA = SqlDataHelper.GetValue<int>(dataReader, "TotalUsagePaidUIWA");
            GetAllDashboard.TotalUsagePaidBIWA = SqlDataHelper.GetValue<int>(dataReader, "TotalUsagePaidBIWA");
            GetAllDashboard.TotalUsageFreeEntry = SqlDataHelper.GetValue<int>(dataReader, "TotalUsageFreeEntry");

            GetAllDashboard.TotalUsageFreeConversation = SqlDataHelper.GetValue<decimal>(dataReader, "TotalUsageFreeConversation");
            GetAllDashboard.TotalUIConversation = SqlDataHelper.GetValue<decimal>(dataReader, "TotalUIConversation");
            GetAllDashboard.TotalUsageUIConversation = SqlDataHelper.GetValue<decimal>(dataReader, "TotalUsageUIConversation");
            GetAllDashboard.TotalBIConversation = SqlDataHelper.GetValue<decimal>(dataReader, "TotalBIConversation");
            GetAllDashboard.TotalUsageBIConversation = SqlDataHelper.GetValue<decimal>(dataReader, "TotalUsageBIConversation");

            //GetAllDashboard.RemainingConversationWA = SqlDataHelper.GetValue<int>(dataReader, "RemainingConversationWA");
            return GetAllDashboard;
        }

        public static CampaignDashModel MapCampaignGetAll(IDataReader dataReader)
        {
            try
            {
                CampaignDashModel model = new CampaignDashModel();
                model.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");
                model.Title = SqlDataHelper.GetValue<string>(dataReader, "Title");

                return model;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public static CampaignStatisticsModel MapCampaignGetStatistics(IDataReader dataReader)
        {
            try
            {
                CampaignStatisticsModel model = new CampaignStatisticsModel();
                model.TotalContact = SqlDataHelper.GetValue<long>(dataReader, "TotalContact") ?? (long?)0;
                model.TotalDelivered = SqlDataHelper.GetValue<long>(dataReader, "TotalDelivered") ?? (long?)0;
                model.TotalRead = SqlDataHelper.GetValue<long>(dataReader, "TotalRead") ?? (long?)0;
                model.TotalSent = SqlDataHelper.GetValue<long>(dataReader, "TotalSent") ?? (long?)0;
                model.TotalReplied = SqlDataHelper.GetValue<long>(dataReader, "TotalReplied") ?? (long?)0;
                model.TotalFailed = SqlDataHelper.GetValue<long>(dataReader, "TotalFailed") ?? (long?)0;

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static string MapTemplatesCategory(IDataReader dataReader)
        {
            string category = "";
            category = SqlDataHelper.GetValue<string>(dataReader, "Category");

            return category;
        }
        public static TemplatesInfo MapTemplatesIslamicInfo(IDataReader dataReader)
        {
            TemplatesInfo templatesInfo = new TemplatesInfo();
            templatesInfo.templatId = SqlDataHelper.GetValue<long>(dataReader, "Id");
            templatesInfo.category = SqlDataHelper.GetValue<string>(dataReader, "Category");

            return templatesInfo;
        }
        #endregion

        #region Conersation Session
        public static ConversationSessionModel MapConversationSession(IDataReader dataReader)
        {
            try
            {
                ConversationSessionModel _ConversationSessionModel = new ConversationSessionModel();
                // _ConversationSessionModel.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");
                _ConversationSessionModel.PhoneNumber = SqlDataHelper.GetValue<string>(dataReader, "PhoneNumber");
                _ConversationSessionModel.ConversationId = SqlDataHelper.GetValue<string>(dataReader, "ConversationId");
                _ConversationSessionModel.ConversationDateTime = SqlDataHelper.GetValue<string>(dataReader, "ConversationDateTime");
                _ConversationSessionModel.InitiatedBy = SqlDataHelper.GetValue<string>(dataReader, "InitiatedBy");
                _ConversationSessionModel.TenantId = SqlDataHelper.GetValue<string>(dataReader, "TenantId");
                _ConversationSessionModel.ContactId = SqlDataHelper.GetValue<string>(dataReader, "ContactId");


                return _ConversationSessionModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        #endregion

        #region Loyalty
        public static LoyaltyModel ConvertToLoyaltyDto(IDataReader dataReader)
        {
            LoyaltyModel loyalty = new LoyaltyModel();
            loyalty.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");


            loyalty.CustomerPoints = SqlDataHelper.GetValue<decimal>(dataReader, "CustomerPoints");
            loyalty.CustomerCurrencyValue = SqlDataHelper.GetValue<decimal>(dataReader, "CustomerCurrencyValue");
            loyalty.ItemsPoints = SqlDataHelper.GetValue<decimal>(dataReader, "ItemsPoints");
            loyalty.ItemsCurrencyValue = SqlDataHelper.GetValue<decimal>(dataReader, "ItemsCurrencyValue");
            loyalty.IsOverrideUpdatedPrice = SqlDataHelper.GetValue<bool>(dataReader, "IsOverrideUpdatedPrice");
            loyalty.CreatedDate = SqlDataHelper.GetValue<DateTime>(dataReader, "CreatedDate");
            loyalty.CreatedBy = SqlDataHelper.GetValue<long>(dataReader, "CreatedBy");
            loyalty.TenantId = SqlDataHelper.GetValue<int>(dataReader, "TenantId");
            loyalty.IsLatest = SqlDataHelper.GetValue<bool>(dataReader, "IsLatest");
            loyalty.IsLoyalityPoint = SqlDataHelper.GetValue<bool>(dataReader, "IsLoyalityPoint");
            loyalty.StartDate = SqlDataHelper.GetValue<DateTime>(dataReader, "StartDate");
            if (loyalty.StartDate != null)
            {
                loyalty.StartDate = loyalty.StartDate.AddHours(AppSettingsModel.AddHour);
            }
            loyalty.EndDate = SqlDataHelper.GetValue<DateTime>(dataReader, "EndDate");
            if (loyalty.EndDate != null)
            {
                loyalty.EndDate = loyalty.EndDate.AddHours(AppSettingsModel.AddHour);
            }
            loyalty.OrderType = SqlDataHelper.GetValue<string>(dataReader, "OrderType");
            loyalty.LoyaltyExpiration = SqlDataHelper.GetValue<int>(dataReader, "LoyaltyExpiration");
            return loyalty;
        }


        public static ItemsLoyaltyLogModel ConvertToItemLoyaltyDto(IDataReader dataReader)
        {
            ItemsLoyaltyLogModel loyalty = new ItemsLoyaltyLogModel();
            loyalty.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");
            loyalty.CreatedDate = SqlDataHelper.GetValue<DateTime>(dataReader, "CreatedDate");
            loyalty.CreatedBy = SqlDataHelper.GetValue<long>(dataReader, "CreatedBy");
            loyalty.TenantId = SqlDataHelper.GetValue<int>(dataReader, "TenantId");
            loyalty.IsLatest = SqlDataHelper.GetValue<bool>(dataReader, "IsLatest");

            loyalty.LoyaltyPoints = SqlDataHelper.GetValue<decimal>(dataReader, "LoyaltyPoints");
            loyalty.OriginalLoyaltyPoints = SqlDataHelper.GetValue<decimal>(dataReader, "OriginalLoyaltyPoints");

            loyalty.IsOverrideLoyaltyPoints = SqlDataHelper.GetValue<bool>(dataReader, "IsOverrideLoyaltyPoints");



            loyalty.LoyaltyDefinitionId = SqlDataHelper.GetValue<long>(dataReader, "LoyaltyDefinitionId");
            loyalty.ItemId = SqlDataHelper.GetValue<long>(dataReader, "ItemId");


            return loyalty;
        }


        public static SpecificationsLoyaltyLogModel ConvertToSpecificationLoyaltyDto(IDataReader dataReader)
        {
            SpecificationsLoyaltyLogModel loyalty = new SpecificationsLoyaltyLogModel();
            loyalty.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");
            loyalty.CreatedDate = SqlDataHelper.GetValue<DateTime>(dataReader, "CreatedDate");
            loyalty.CreatedBy = SqlDataHelper.GetValue<long>(dataReader, "CreatedBy");
            loyalty.TenantId = SqlDataHelper.GetValue<int>(dataReader, "TenantId");
            loyalty.IsLatest = SqlDataHelper.GetValue<bool>(dataReader, "IsLatest");

            loyalty.LoyaltyPoints = SqlDataHelper.GetValue<decimal>(dataReader, "LoyaltyPoints");
            loyalty.OriginalLoyaltyPoints = SqlDataHelper.GetValue<decimal>(dataReader, "OriginalLoyaltyPoints");

            loyalty.IsOverrideLoyaltyPoints = SqlDataHelper.GetValue<bool>(dataReader, "IsOverrideLoyaltyPoints");



            loyalty.LoyaltyDefinitionId = SqlDataHelper.GetValue<long>(dataReader, "LoyaltyDefinitionId");
            loyalty.SpecificationChoicesId = SqlDataHelper.GetValue<long>(dataReader, "SpecificationChoicesId");
            loyalty.SpecificationId = SqlDataHelper.GetValue<long>(dataReader, "SpecificationId");


            return loyalty;
        }

        public static AdditionsLoyaltyLogModel ConvertToAdditionsLoyaltyDto(IDataReader dataReader)
        {
            AdditionsLoyaltyLogModel loyalty = new AdditionsLoyaltyLogModel();
            loyalty.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");
            loyalty.CreatedDate = SqlDataHelper.GetValue<DateTime>(dataReader, "CreatedDate");
            loyalty.CreatedBy = SqlDataHelper.GetValue<long>(dataReader, "CreatedBy");
            loyalty.TenantId = SqlDataHelper.GetValue<int>(dataReader, "TenantId");
            loyalty.IsLatest = SqlDataHelper.GetValue<bool>(dataReader, "IsLatest");

            loyalty.LoyaltyPoints = SqlDataHelper.GetValue<decimal>(dataReader, "LoyaltyPoints");
            loyalty.OriginalLoyaltyPoints = SqlDataHelper.GetValue<decimal>(dataReader, "OriginalLoyaltyPoints");

            loyalty.IsOverrideLoyaltyPoints = SqlDataHelper.GetValue<bool>(dataReader, "IsOverrideLoyaltyPoints");



            loyalty.LoyaltyDefinitionId = SqlDataHelper.GetValue<long>(dataReader, "LoyaltyDefinitionId");
            loyalty.ItemAdditionsCategoryId = SqlDataHelper.GetValue<long>(dataReader, "ItemAdditionsCategoryId");
            loyalty.ItemAdditionsId = SqlDataHelper.GetValue<long>(dataReader, "ItemAdditionsId");


            return loyalty;
        }
        #endregion

        #region Location
        public static LocationInfoModelDto MapMenuLocation(IDataReader dataReader)
        {
            try
            {
                LocationInfoModelDto entity = new LocationInfoModelDto();

                entity.Id = SqlDataHelper.GetValue<int>(dataReader, "Id");
                entity.LocationId = SqlDataHelper.GetValue<int>(dataReader, "LocationId");
                entity.DeliveryCost = SqlDataHelper.GetValue<decimal>(dataReader, "DeliveryCost");
                entity.AreaId = SqlDataHelper.GetValue<long>(dataReader, "BranchAreaId");

                return entity;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public static LocationModel MapLocation(IDataReader dataReader)
        {

            LocationModel location = new LocationModel();

            location.LocationId = SqlDataHelper.GetValue<int>(dataReader, "LocationId");
            location.LocationName = SqlDataHelper.GetValue<string>(dataReader, "LocationName");
            location.LocationNameEn = SqlDataHelper.GetValue<string>(dataReader, "LocationNameEn");
            location.LevelId = SqlDataHelper.GetValue<int>(dataReader, "LevelId");
            location.ParentId = SqlDataHelper.GetValue<int>(dataReader, "ParentId");
            location.GoogleURL = SqlDataHelper.GetValue<string>(dataReader, "GoogleURL");
            location.TenantId = SqlDataHelper.GetValue<int>(dataReader, "TenantId");
            location.DeliveryCostId = SqlDataHelper.GetValue<int>(dataReader, "DeliveryCostId");
            location.DeliveryCost = SqlDataHelper.GetValue<decimal>(dataReader, "DeliveryCost");
            location.BranchAreaId = SqlDataHelper.GetValue<int>(dataReader, "BranchAreaId");
            location.HasSubDistrict = SqlDataHelper.GetValue<bool>(dataReader, "HasSubDistrict");
            location.IsAvailable = SqlDataHelper.GetValue<bool>(dataReader, "IsAvailable");
            location.CityId = SqlDataHelper.GetValue<int>(dataReader, "CityId");
            location.AreaId = SqlDataHelper.GetValue<int>(dataReader, "AreaId");
            location.CityName = SqlDataHelper.GetValue<string>(dataReader, "CityName");
            location.AreaName = SqlDataHelper.GetValue<string>(dataReader, "AreaName");


            return location;

        }
        public static SubDistrictModel MapSubDistric(IDataReader dataReader)
        {
            SubDistrictModel subDistrict = new SubDistrictModel();
            subDistrict.Id = SqlDataHelper.GetValue<int>(dataReader, "Id");
            subDistrict.TenantId = SqlDataHelper.GetValue<int>(dataReader, "TenantId");
            subDistrict.LocationDeliveryCostId = SqlDataHelper.GetValue<int>(dataReader, "LocationDeliveryCostId");
            subDistrict.DeliveryCost = SqlDataHelper.GetValue<decimal>(dataReader, "DeliveryCost");
            subDistrict.Latitude = SqlDataHelper.GetValue<decimal>(dataReader, "Latitude");
            subDistrict.Longitude = SqlDataHelper.GetValue<decimal>(dataReader, "Longitude");
            subDistrict.LocationName = SqlDataHelper.GetValue<string>(dataReader, "LocationName");
            subDistrict.LocationNameEn = SqlDataHelper.GetValue<string>(dataReader, "LocationNameEn");
            subDistrict.LongitudeAndLatitude = SqlDataHelper.GetValue<string>(dataReader, "LongitudeAndLatitude");

            return subDistrict;
        }
        #endregion

        #region Area
        public static AreaDto MapAreaPSQL(IDataReader dataReader)
        {
            try
            {
                AreaDto area = new AreaDto();

                // Map the values
                area.Id = SqlDataHelper.GetValue<long>(dataReader, "id");
                area.AreaName = SqlDataHelper.GetValue<string>(dataReader, "areaname");
                area.AreaNameEnglish = SqlDataHelper.GetValue<string>(dataReader, "areanameenglish");
                area.AreaCoordinate = SqlDataHelper.GetValue<string>(dataReader, "areacoordinate");
                area.AreaCoordinateEnglish = SqlDataHelper.GetValue<string>(dataReader, "areacoordinateenglish");
                area.IsAssginToAllUser = SqlDataHelper.GetValue<bool>(dataReader, "isassgintoalluser");
                area.IsRestaurantsTypeAll = SqlDataHelper.GetValue<bool>(dataReader, "isrestaurantstypeall");
                area.Latitude = SqlDataHelper.GetValue<double>(dataReader, "latitude");
                area.Longitude = SqlDataHelper.GetValue<double>(dataReader, "longitude");
                area.SettingJson = SqlDataHelper.GetValue<string>(dataReader, "settingjson");
                area.BranchID = SqlDataHelper.GetValue<string>(dataReader, "branchid");
                area.UserId = SqlDataHelper.GetValue<int>(dataReader, "userid");
                area.RestaurantsType = SqlDataHelper.GetValue<int>(dataReader, "restaurantstype");
                area.IsAvailableBranch = SqlDataHelper.GetValue<bool>(dataReader, "isavailablebranch");
                area.UserIds = SqlDataHelper.GetValue<string>(dataReader, "userids");
                area.Url = SqlDataHelper.GetValue<string>(dataReader, "url");
                area.TotalCount = SqlDataHelper.GetValue<long>(dataReader, "total_count");

                return area;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public static AreaDto MapArea(IDataReader dataReader)
        {
            AreaDto area = new AreaDto();
            area.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");
            area.AreaName = SqlDataHelper.GetValue<string>(dataReader, "AreaName");
            area.AreaNameEnglish = SqlDataHelper.GetValue<string>(dataReader, "AreaNameEnglish");
            area.AreaCoordinate = SqlDataHelper.GetValue<string>(dataReader, "AreaCoordinate");
            area.AreaCoordinateEnglish = SqlDataHelper.GetValue<string>(dataReader, "AreaCoordinateEnglish");
            area.IsAssginToAllUser = SqlDataHelper.GetValue<bool>(dataReader, "IsAssginToAllUser");
            area.IsRestaurantsTypeAll = SqlDataHelper.GetValue<bool>(dataReader, "IsRestaurantsTypeAll");
            area.Latitude = SqlDataHelper.GetValue<double>(dataReader, "Latitude");
            area.Longitude = SqlDataHelper.GetValue<double>(dataReader, "Longitude");
            area.SettingJson = SqlDataHelper.GetValue<string>(dataReader, "SettingJson");
            area.BranchID = SqlDataHelper.GetValue<string>(dataReader, "BranchID");
            area.UserId = SqlDataHelper.GetValue<int>(dataReader, "UserId");
            area.RestaurantsType = SqlDataHelper.GetValue<int>(dataReader, "RestaurantsType");
            area.IsAvailableBranch = SqlDataHelper.GetValue<bool>(dataReader, "IsAvailableBranch");
            area.UserIds = SqlDataHelper.GetValue<string>(dataReader, "UserIds");
            area.Url = SqlDataHelper.GetValue<string>(dataReader, "URL");

            return area;
        }


        public static UserTicketsModel MapUserTickets(IDataReader dataReader)
        {
            try
            {
                UserTicketsModel entity = new UserTicketsModel();

                entity.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");

                try
                {
                    entity.TicketsOpened = SqlDataHelper.GetValue<int>(dataReader, "TicketsOpened");

                }
                catch
                {

                    entity.TicketsOpened = 0;
                }
                try
                {
                    entity.MaximumTickets = SqlDataHelper.GetValue<int>(dataReader, "MaximumTickets");

                }
                catch
                {

                    entity.MaximumTickets = 0;
                }



                return entity;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }


        public static WorkModel MapBranchSetting(IDataReader dataReader)
        {
            try
            {
                WorkModel entity = new WorkModel();

                if (!string.IsNullOrEmpty(SqlDataHelper.GetValue<string>(dataReader, "SettingJson")))
                {

                    var options = new JsonSerializerOptions { WriteIndented = true };
                    entity = System.Text.Json.JsonSerializer.Deserialize<WorkModel>(SqlDataHelper.GetValue<string>(dataReader, "SettingJson"), options);

                }
                return entity;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public static WorkModel MapBranchSettingPSQL(IDataReader dataReader)
        {
            try
            {
                WorkModel entity = new WorkModel();

                if (!string.IsNullOrEmpty(SqlDataHelper.GetValue<string>(dataReader, "setting_json")))
                {

                    var options = new JsonSerializerOptions { WriteIndented = true };
                    entity = System.Text.Json.JsonSerializer.Deserialize<WorkModel>(SqlDataHelper.GetValue<string>(dataReader, "SettingJson"), options);

                }
                return entity;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public static BranchsModel MapBranchGeeAll(IDataReader dataReader)
        {
            try
            {
                BranchsModel model = new BranchsModel();
                model.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");
                model.AreaName = SqlDataHelper.GetValue<string>(dataReader, "AreaName");
                model.AreaNameEnglish = SqlDataHelper.GetValue<string>(dataReader, "AreaNameEnglish");


                return model;
            }
            catch (Exception)
            {
                throw;
            }

        }

        #endregion

        #region  locationsPinned
        public static LocationsPinned ConvertToLocationsPinnedDto(IDataReader dataReader)
        {
            LocationsPinned entity = new LocationsPinned();


            entity.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");
            entity.NameAR = SqlDataHelper.GetValue<string>(dataReader, "NameAR");
            entity.NameEN = SqlDataHelper.GetValue<string>(dataReader, "NameEN");
            entity.URL = SqlDataHelper.GetValue<string>(dataReader, "URL");
            entity.TenantId = SqlDataHelper.GetValue<int>(dataReader, "TenantId");

            entity.Latitude = SqlDataHelper.GetValue<float>(dataReader, "Latitude");
            entity.Longitude = SqlDataHelper.GetValue<float>(dataReader, "Longitude");


            return entity;
        }

        public static GetLocationInfoTowModel ConvertToLocationsP(IDataReader dataReader)
        {
            GetLocationInfoTowModel entity = new GetLocationInfoTowModel();


            entity.DeliveryCostAfter = SqlDataHelper.GetValue<decimal>(dataReader, "DeliveryCost");
            entity.DeliveryCostBefor = SqlDataHelper.GetValue<decimal>(dataReader, "DeliveryCost");

            entity.LocationId = SqlDataHelper.GetValue<int>(dataReader, "LocationId");

            entity.AreaId = SqlDataHelper.GetValue<int>(dataReader, "BranchAreaId");

            return entity;
        }


        #endregion

        #region Menu
        public static MenuDto MapMenu(IDataReader dataReader)
        {
            MenuDto menu = new MenuDto();
            menu.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");
            menu.MenuName = SqlDataHelper.GetValue<string>(dataReader, "MenuName");
            menu.MenuNameEnglish = SqlDataHelper.GetValue<string>(dataReader, "MenuNameEnglish");
            menu.MenuDescription = SqlDataHelper.GetValue<string>(dataReader, "MenuDescription");
            menu.MenuDescriptionEnglish = SqlDataHelper.GetValue<string>(dataReader, "MenuDescriptionEnglish");
            menu.EffectiveTimeFrom = SqlDataHelper.GetValue<DateTime>(dataReader, "EffectiveTimeFrom");
            menu.EffectiveTimeTo = SqlDataHelper.GetValue<DateTime>(dataReader, "EffectiveTimeTo");
            menu.Tax = SqlDataHelper.GetValue<int>(dataReader, "Tax");
            menu.ImageUri = SqlDataHelper.GetValue<string>(dataReader, "ImageUri");
            menu.Priority = SqlDataHelper.GetValue<int>(dataReader, "Priority");

            menu.RestaurantsTypeId = SqlDataHelper.GetValue<int>(dataReader, "RestaurantsType");
            menu.MenuTypeId = SqlDataHelper.GetValue<int>(dataReader, "MenuTypeId");

            menu.LanguageBotId = SqlDataHelper.GetValue<int>(dataReader, "LanguageBotId");
            menu.ImageBgUri = SqlDataHelper.GetValue<string>(dataReader, "ImageBgUri");
            menu.AreaIds = SqlDataHelper.GetValue<string>(dataReader, "AreaIds");

            return menu;
        }
        public static GetItemAdditionsCategorysModel MapItemAdditionsCategories(IDataReader dataReader)
        {
            GetItemAdditionsCategorysModel _ItemAdditionsCategorysModel = new GetItemAdditionsCategorysModel();

            _ItemAdditionsCategorysModel.categoryId = SqlDataHelper.GetValue<int>(dataReader, "Id");
            _ItemAdditionsCategorysModel.IsCondiments = SqlDataHelper.GetValue<bool>(dataReader, "IsCondiments");
            _ItemAdditionsCategorysModel.IsCrispy = SqlDataHelper.GetValue<bool>(dataReader, "IsCrispy");
            _ItemAdditionsCategorysModel.IsDeserts = SqlDataHelper.GetValue<bool>(dataReader, "IsDeserts");
            _ItemAdditionsCategorysModel.categoryName = SqlDataHelper.GetValue<string>(dataReader, "Name");
            _ItemAdditionsCategorysModel.categoryNameEnglish = SqlDataHelper.GetValue<string>(dataReader, "NameEnglish");
            _ItemAdditionsCategorysModel.categoryPriority = SqlDataHelper.GetValue<string>(dataReader, "Priority");
            _ItemAdditionsCategorysModel.IsNon = !SqlDataHelper.GetValue<bool>(dataReader, "IsCondiments");

            return _ItemAdditionsCategorysModel;
        }
        public static GetSpecificationsCategorysModel MapSpecificationsCategories(IDataReader dataReader)
        {
            GetSpecificationsCategorysModel _SpecificationsCategorysModel = new GetSpecificationsCategorysModel();

            _SpecificationsCategorysModel.categoryId = SqlDataHelper.GetValue<int>(dataReader, "Id");
            _SpecificationsCategorysModel.categoryName = SqlDataHelper.GetValue<string>(dataReader, "SpecificationDescription");
            _SpecificationsCategorysModel.categoryNameEnglish = SqlDataHelper.GetValue<string>(dataReader, "SpecificationDescriptionEnglish");
            _SpecificationsCategorysModel.categoryPriority = SqlDataHelper.GetValue<string>(dataReader, "Priority");
            _SpecificationsCategorysModel.IsMultipleSelection = SqlDataHelper.GetValue<bool>(dataReader, "IsMultipleSelection");
            _SpecificationsCategorysModel.IsRequired = true;
            _SpecificationsCategorysModel.MaxSelectNumber = SqlDataHelper.GetValue<int>(dataReader, "MaxSelectNumber");
            //_SpecificationsCategorysModel.IsRequired
            return _SpecificationsCategorysModel;
        }



        #endregion

        #region Item
        public ItemImagesModel MapItemImages(IDataReader dataReader)
        {
            ItemImagesModel itemImages = new ItemImagesModel
            {
                Id = SqlDataHelper.GetValue<long>(dataReader, "Id"),
                ItemId = SqlDataHelper.GetValue<long>(dataReader, "ItemId"),
                ImageUrl = SqlDataHelper.GetValue<string>(dataReader, "ImageUrl"),
                TenantId = SqlDataHelper.GetValue<int>(dataReader, "TenantId"),
                IsMainImage = SqlDataHelper.GetValue<bool>(dataReader, "IsMainImage")
            };
            return itemImages;
        }
        #endregion

        #region Captions



        public static CaptionDto ConvertCaptionDto(IDataReader dataReader)
        {
            CaptionDto captionDto = new CaptionDto();

            captionDto.TenantId = SqlDataHelper.GetValue<int>(dataReader, "TenantId");
            captionDto.Id = SqlDataHelper.GetValue<int>(dataReader, "Id");
            captionDto.Text = SqlDataHelper.GetValue<string>(dataReader, "Text");
            captionDto.LanguageBotId = SqlDataHelper.GetValue<int>(dataReader, "LanguageBotId");
            captionDto.TextResourceId = SqlDataHelper.GetValue<int>(dataReader, "TextResourceId");
            captionDto.HeaderText = SqlDataHelper.GetValue<string>(dataReader, "HeaderText");


            return captionDto;
        }



        #endregion

        #region Currency
        public static CurrencyDto MapCurrency(IDataReader dataReader)
        {
            CurrencyDto Currency = new CurrencyDto();


            Currency.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");
            Currency.CurrencyName = SqlDataHelper.GetValue<string>(dataReader, "CurrencyName");
            Currency.ISOName = SqlDataHelper.GetValue<string>(dataReader, "ISOName");


            return Currency;
        }
        #endregion

        #region Bot
        public static BotTemplatesModel MapBotTemplates(IDataReader dataReader)
        {
            BotTemplatesModel BotTemplate = new BotTemplatesModel();


            BotTemplate.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");
            BotTemplate.GuidId = SqlDataHelper.GetValue<Guid>(dataReader, "GuidId");
            BotTemplate.TemplateName = SqlDataHelper.GetValue<string>(dataReader, "TemplateName");
            BotTemplate.TemplateNumber = SqlDataHelper.GetValue<int>(dataReader, "TemplateNumber");
            BotTemplate.IsDeleted = SqlDataHelper.GetValue<bool>(dataReader, "IsDeleted");


            return BotTemplate;
        }

        public static BotReservedWordsModel MapBotReservedWords(IDataReader dataReader)
        {
            BotReservedWordsModel model = new BotReservedWordsModel
            {
                Id = SqlDataHelper.GetValue<long>(dataReader, "Id"),
                ButtonText = SqlDataHelper.GetValue<string>(dataReader, "ButtonText"),
                Action = SqlDataHelper.GetValue<string>(dataReader, "Action"),
                TenantId = SqlDataHelper.GetValue<int>(dataReader, "TenantId"),
                TriggersBot = SqlDataHelper.GetValue<string>(dataReader, "TriggersBot"),
                ActionId = SqlDataHelper.GetValue<long>(dataReader, "ActionId"),
                ActionAr = SqlDataHelper.GetValue<string>(dataReader, "ActionAr"),
                ActionEn = SqlDataHelper.GetValue<string>(dataReader, "ActionEn"),
            };


            return model;
        }
        public static ActionsModel MapActions(IDataReader dataReader)
        {
            ActionsModel model = new ActionsModel
            {
                Id = SqlDataHelper.GetValue<long>(dataReader, "Id"),
                ActionAr = SqlDataHelper.GetValue<string>(dataReader, "ActionAr"),
                ActionEn = SqlDataHelper.GetValue<string>(dataReader, "ActionEn"),
            };


            return model;
        }
        public static KeyWordModel MapKeyWordModel(IDataReader dataReader)
        {
            KeyWordModel model = new KeyWordModel();

            model.id = SqlDataHelper.GetValue<long>(dataReader, "Id");
            model.tenantId = SqlDataHelper.GetValue<int>(dataReader, "TenantId");
            model.action = SqlDataHelper.GetValue<string>(dataReader, "Action");
            model.actionId = SqlDataHelper.GetValue<long>(dataReader, "ActionId");
            model.triggersBot = SqlDataHelper.GetValue<string>(dataReader, "TriggersBot");
            model.triggersBotId = SqlDataHelper.GetValue<long>(dataReader, "TriggersBotId");
            model.buttonText = SqlDataHelper.GetValue<string>(dataReader, "ButtonText");

            try
            {
                model.KeyUse = SqlDataHelper.GetValue<long>(dataReader, "KeyUse");

            }
            catch
            {
                model.KeyUse = 0;

            }


            try
            {
                model.KeyWordType = SqlDataHelper.GetValue<int>(dataReader, "KeyWordType");

            }
            catch
            {


            }

            try
            {
                model.FuzzyMatch = SqlDataHelper.GetValue<int>(dataReader, "FuzzyMatch");

            }
            catch
            {
                model.FuzzyMatch = 0;

            }

            return model;
        }
        #endregion

        #region Departments
        public static DepartmentModel MapDepartment(IDataReader dataReader)
        {
            DepartmentModel department = new DepartmentModel();


            department.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");
            department.DepartmentId = SqlDataHelper.GetValue<int>(dataReader, "DepartmentId");
            department.TenantId = SqlDataHelper.GetValue<int>(dataReader, "TenantId");
            department.Name = SqlDataHelper.GetValue<string>(dataReader, "Name");
            department.NameEn = SqlDataHelper.GetValue<string>(dataReader, "NameEn");
            department.UserIds = SqlDataHelper.GetValue<string>(dataReader, "UserIds");
            department.UserNames = SqlDataHelper.GetValue<string>(dataReader, "UserNames");
            department.CreatedBy = SqlDataHelper.GetValue<long>(dataReader, "CreatedBy");
            department.CreatedDate = SqlDataHelper.GetValue<DateTime>(dataReader, "CreatedDate");
            department.ModifiedBy = SqlDataHelper.GetValue<long>(dataReader, "ModifiedBy");
            department.ModifiedDate = SqlDataHelper.GetValue<DateTime>(dataReader, "ModifiedDate");
            department.ModifiedDate = SqlDataHelper.GetValue<DateTime>(dataReader, "SentTime");
            if (department.ModifiedDate != null)
            {
                department.ModifiedDate = department.ModifiedDate.Value.AddHours(AppSettingsModel.AddHour);
            }
            department.IsLiveChat = SqlDataHelper.GetValue<bool>(dataReader, "IsLiveChat");
            department.IsSellingRequest = SqlDataHelper.GetValue<bool>(dataReader, "IsSellingRequest");

            return department;
        }
        #endregion


        #region Billing
        public static BillingDto MapBilling(IDataReader dataReader)
        {
            BillingDto billingDto = new BillingDto();
            billingDto.Id = SqlDataHelper.GetValue<int>(dataReader, "Id");
            billingDto.BillingID = SqlDataHelper.GetValue<string>(dataReader, "BillingID");
            billingDto.BillingDate = SqlDataHelper.GetValue<DateTime>(dataReader, "BillingDate");
            billingDto.DueDate = SqlDataHelper.GetValue<DateTime>(dataReader, "DueDate");

            billingDto.CurrencyId = SqlDataHelper.GetValue<int>(dataReader, "CurrencyId");

            billingDto.Status = SqlDataHelper.GetValue<string>(dataReader, "Status");

            billingDto.CustomerId = SqlDataHelper.GetValue<string>(dataReader, "CustomerId");
            billingDto.InvoiceJson = SqlDataHelper.GetValue<string>(dataReader, "InvoiceJson");
            billingDto.TotalAmount = SqlDataHelper.GetValue<decimal>(dataReader, "TotalAmount");

            return billingDto;
        }
        #endregion

        #region Booking
        public static BookingModel MapBooking(IDataReader dataReader)
        {
            BookingModel booking = new BookingModel();

            booking.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");
            booking.CustomerName = SqlDataHelper.GetValue<string>(dataReader, "CustomerName");
            booking.PhoneNumber = SqlDataHelper.GetValue<string>(dataReader, "PhoneNumber");
            booking.BookingDateTime = SqlDataHelper.GetValue<DateTime>(dataReader, "BookingDateTime").AddHours(AppSettingsModel.AddHour);
            booking.BookingDateTimeString = booking.BookingDateTime.ToString();
            booking.StatusId = SqlDataHelper.GetValue<int>(dataReader, "StatusId");
            booking.BookingNumber = SqlDataHelper.GetValue<int>(dataReader, "BookingNumber");
            booking.AreaId = SqlDataHelper.GetValue<int>(dataReader, "AreaId");
            booking.AreaName = SqlDataHelper.GetValue<string>(dataReader, "AreaName");
            booking.TenantId = SqlDataHelper.GetValue<int>(dataReader, "TenantId");
            booking.CreatedBy = SqlDataHelper.GetValue<long>(dataReader, "CreatedBy");
            booking.CreatedOn = SqlDataHelper.GetValue<DateTime>(dataReader, "CreatedOn");
            booking.ContactId = SqlDataHelper.GetValue<int>(dataReader, "ContactId");
            booking.LanguageId = SqlDataHelper.GetValue<int>(dataReader, "LanguageId");
            booking.BookingTypeId = SqlDataHelper.GetValue<int>(dataReader, "BookingTypeId");
            booking.CustomerId = SqlDataHelper.GetValue<string>(dataReader, "CustomerId");
            booking.DeletionReasonId = SqlDataHelper.GetValue<int>(dataReader, "DeletionReasonId");
            booking.Note = SqlDataHelper.GetValue<string>(dataReader, "Note");
            booking.UserName = SqlDataHelper.GetValue<string>(dataReader, "UserName");
            booking.IsNew = SqlDataHelper.GetValue<bool>(dataReader, "IsNew");
            booking.UserId = SqlDataHelper.GetValue<bool>(dataReader, "UserId");


            return booking;
        }

        public static BookingOffDays MapBookingOffDays(IDataReader dataReader)
        {
            BookingOffDays bookingOffDays = new BookingOffDays();
            bookingOffDays.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");
            bookingOffDays.Day = SqlDataHelper.GetValue<string>(dataReader, "Day");
            bookingOffDays.StartTime = SqlDataHelper.GetValue<string>(dataReader, "StartTime");
            bookingOffDays.EndTime = SqlDataHelper.GetValue<string>(dataReader, "EndTime");
            bookingOffDays.TenantId = SqlDataHelper.GetValue<int>(dataReader, "TenantId");
            bookingOffDays.UserId = SqlDataHelper.GetValue<long>(dataReader, "UserId");
            bookingOffDays.IsOffDayBooking = SqlDataHelper.GetValue<bool>(dataReader, "IsOffDay");
            return bookingOffDays;
        }


        public static BookingDashModel MapGetBookingAllDash(IDataReader dataReader)
        {
            try
            {
                BookingDashModel model = new BookingDashModel();
                model.UserId = SqlDataHelper.GetValue<long>(dataReader, "UserId");
                model.TotalBooked = SqlDataHelper.GetValue<int>(dataReader, "TotalBooked");
                model.TotalConfirmed = SqlDataHelper.GetValue<int>(dataReader, "TotalConfirmed");
                model.TotalCancelled = SqlDataHelper.GetValue<int>(dataReader, "TotalCancelled");
                model.TotalDeleted = SqlDataHelper.GetValue<int>(dataReader, "TotalDeleted");
                model.TotalPending = SqlDataHelper.GetValue<int>(dataReader, "TotalPending");
                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static BookingStatisticsModel MapBookingGetStatistics(IDataReader dataReader)
        {
            try
            {
                BookingStatisticsModel model = new BookingStatisticsModel();
                model.TotalAppointments = SqlDataHelper.GetValue<long>(dataReader, "TotalAppointments") ?? (long?)0;
                model.TotalBooked = SqlDataHelper.GetValue<long>(dataReader, "TotalBooked") ?? (long?)0;
                model.TotalConfirmed = SqlDataHelper.GetValue<long>(dataReader, "TotalConfirmed") ?? (long?)0;
                model.TotalCancelled = SqlDataHelper.GetValue<long>(dataReader, "TotalCancelled") ?? (long?)0;
                model.TotalPending = SqlDataHelper.GetValue<long>(dataReader, "TotalPending") ?? (long?)0;

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Template Message

        public static CreateOrEditTemplateMessageDto MapTemplateMessage(IDataReader dataReader)
        {
            CreateOrEditTemplateMessageDto templateMessage = new CreateOrEditTemplateMessageDto();

            templateMessage.Id = SqlDataHelper.GetValue<int>(dataReader, "Id");
            templateMessage.TemplateMessageName = SqlDataHelper.GetValue<string>(dataReader, "TemplateMessageName");
            templateMessage.MessageCreationDate = SqlDataHelper.GetValue<DateTime>(dataReader, "MessageCreationDate");
            templateMessage.MessageText = SqlDataHelper.GetValue<string>(dataReader, "MessageText");
            templateMessage.TemplateMessagePurposeId = SqlDataHelper.GetValue<int>(dataReader, "TemplateMessagePurposeId");

            return templateMessage;
        }

        #endregion

        #region Role

        public static RoleModelDto ConvertToRoleDto(IDataReader dataReader)
        {
            RoleModelDto role = new RoleModelDto();
            role.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");
            role.Name = SqlDataHelper.GetValue<string>(dataReader, "Name");

            return role;
        }


        public static UserRoleModelDto ConvertToUserRoleDto(IDataReader dataReader)
        {
            UserRoleModelDto userroles = new UserRoleModelDto();
            userroles.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");
            userroles.RoleId = SqlDataHelper.GetValue<int>(dataReader, "RoleId");
            userroles.UserId = SqlDataHelper.GetValue<long>(dataReader, "UserId");
            return userroles;
        }
        #endregion

        #region Dashboard
        public static DashboardNumbersEntity MapDashboardNumbers(IDataReader dataReader)
        {
            ContactDashboard contact = new ContactDashboard
            {
                TotalContact = SqlDataHelper.GetValue<int>(dataReader, "TotalContact"),
                TotalContactOptIn = SqlDataHelper.GetValue<int>(dataReader, "TotalContactOptIn"),
                TotalContactOptOut = SqlDataHelper.GetValue<int>(dataReader, "TotalContactOptOut"),
                TotalContactNeutral = SqlDataHelper.GetValue<int>(dataReader, "TotalContactNeutral"),
            };

            OrderDashboard order = new OrderDashboard
            {
                TotalOrder = SqlDataHelper.GetValue<int>(dataReader, "TotalOrder"),
                TotalOrderDone = SqlDataHelper.GetValue<int>(dataReader, "TotalOrderDone"),
                TotalOrderCancel = SqlDataHelper.GetValue<int>(dataReader, "TotalOrderCancel"),
                TotalOrderDelete = SqlDataHelper.GetValue<int>(dataReader, "TotalOrderDelete"),
                TotalOrderPending = SqlDataHelper.GetValue<int>(dataReader, "TotalOrderPending"),
                TotalOrderPreOrder = SqlDataHelper.GetValue<int>(dataReader, "TotalOrderPreOrder")
            };

            LiveChatDashboard liveChat = new LiveChatDashboard
            {
                TotalLiveChat = SqlDataHelper.GetValue<int>(dataReader, "TotalLiveChat"),
                TotalLiveChatPending = SqlDataHelper.GetValue<int>(dataReader, "TotalLiveChatPending"),
                TotalLiveChatOpen = SqlDataHelper.GetValue<int>(dataReader, "TotalLiveChatOpen"),
                TotalLiveChatClose = SqlDataHelper.GetValue<int>(dataReader, "TotalLiveChatClose"),
            };

            RequestDashboard request = new RequestDashboard
            {
                TotalRequest = SqlDataHelper.GetValue<int>(dataReader, "TotalRequest"),
                TotalRequestPending = SqlDataHelper.GetValue<int>(dataReader, "TotalRequestPending"),
                TotalRequestOpen = SqlDataHelper.GetValue<int>(dataReader, "TotalRequestOpen"),
                TotalRequestClose = SqlDataHelper.GetValue<int>(dataReader, "TotalRequestClose"),
            };

            CampaignDashboard campaign = new CampaignDashboard
            {
                TotalCampaign = SqlDataHelper.GetValue<int>(dataReader, "TotalCampaign"),
                TotalCampaignSent = SqlDataHelper.GetValue<int>(dataReader, "TotalCampaignSent"),
                TotalCampaignDelivered = SqlDataHelper.GetValue<int>(dataReader, "TotalCampaignDelivered"),
                TotalCampaignRead = SqlDataHelper.GetValue<int>(dataReader, "TotalCampaignRead"),
            };


            DashboardNumbersEntity dashboardNumbersEntity = new DashboardNumbersEntity
            {
                Order = order,
                Request = request,
                LiveChat = liveChat,
                Contact = contact,
                Campaign = campaign,
            };
            return dashboardNumbersEntity;
        }

        public static TenantModelDash MapTenantForDash(IDataReader dataReader)
        {
            try
            {
                TenantModelDash tenant = new TenantModelDash();
                tenant.TenantId = SqlDataHelper.GetValue<int>(dataReader, "Id");
                tenant.AccessToken = SqlDataHelper.GetValue<string>(dataReader, "AccessToken");
                tenant.Name = SqlDataHelper.GetValue<string>(dataReader, "Name");
                tenant.TenancyName = SqlDataHelper.GetValue<string>(dataReader, "TenancyName");
                tenant.WhatsAppAccountID = SqlDataHelper.GetValue<string>(dataReader, "WhatsAppAccountID");
                tenant.PhoneNumber = SqlDataHelper.GetValue<string>(dataReader, "PhoneNumber");
                return tenant;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static string MapTenantForZohoCustomerId(IDataReader dataReader)
        {
            try
            {
                string ZohoCustomerId;
                ZohoCustomerId = SqlDataHelper.GetValue<string>(dataReader, "ZohoCustomerId");

                return ZohoCustomerId;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static CountryCodeModel MapCountryCode(IDataReader dataReader)
        {
            try
            {
                CountryCodeModel model = new CountryCodeModel();
                model.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");
                model.Country = SqlDataHelper.GetValue<string>(dataReader, "Country");
                model.Region = SqlDataHelper.GetValue<string>(dataReader, "Region");
                model.CountryCallingCode = SqlDataHelper.GetValue<string>(dataReader, "CountryCallingCode");
                model.Currency = SqlDataHelper.GetValue<string>(dataReader, "Currency");
                model.MarketingPrice = SqlDataHelper.GetValue<float>(dataReader, "MarketingPrice");
                model.UtilityPrice = SqlDataHelper.GetValue<float>(dataReader, "UtilityPrice");
                model.AuthenticationPrice = SqlDataHelper.GetValue<float>(dataReader, "AuthenticationPrice");
                model.ServicePrice = SqlDataHelper.GetValue<float>(dataReader, "ServicePrice");

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static BestSellingModel MapBestSellingItems(IDataReader dataReader)
        {
            try
            {
                BestSellingModel model = new BestSellingModel();
                model.ItemName = SqlDataHelper.GetValue<string>(dataReader, "ItemName");
                model.ItemNameEnglish = SqlDataHelper.GetValue<string>(dataReader, "ItemNameEnglish");
                model.TotalQuantity = SqlDataHelper.GetValue<int>(dataReader, "TotalQuantity");

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static UsageDetailsModel MapGetUsageDetails(IDataReader dataReader)
        {
            try
            {
                UsageDetailsModel model = new UsageDetailsModel();
                model.transactionIdS = SqlDataHelper.GetValue<string>(dataReader, "TransactionIds");
                model.CategoryType = SqlDataHelper.GetValue<string>(dataReader, "CategoryType");
                model.TransactionDate = SqlDataHelper.GetValue<DateTime>(dataReader, "TransactionDate");
                model.TotalQuantity = SqlDataHelper.GetValue<int>(dataReader, "TotalQuantity") ?? (int?)0;
                model.TotalTransaction = SqlDataHelper.GetValue<decimal>(dataReader, "TotalTransaction") ?? (decimal?)0;
                model.TotalRemaining = SqlDataHelper.GetValue<decimal>(dataReader, "TotalRemaining") ?? (decimal?)0;
                model.DoneBy = SqlDataHelper.GetValue<string>(dataReader, "DoneBy");
                model.TemplateName = SqlDataHelper.GetValue<string>(dataReader, "TemplateName");
                model.CampaignName = SqlDataHelper.GetValue<string>(dataReader, "CampaignName");
                model.CampaignId = SqlDataHelper.GetValue<long>(dataReader, "CampaignId") ?? (long?)0;
                model.Country = SqlDataHelper.GetValue<string>(dataReader, "Country");

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static UsageStatisticsModel MapGetUsageStatistics(IDataReader dataReader)
        {
            try
            {
                UsageStatisticsModel model = new UsageStatisticsModel();
                model.TotalDelivered = SqlDataHelper.GetValue<int>(dataReader, "TotalDelivered");
                model.TotalRead = SqlDataHelper.GetValue<int>(dataReader, "TotalRead");
                model.TotalSent = SqlDataHelper.GetValue<int>(dataReader, "TotalSent");
                model.TotalReplied = SqlDataHelper.GetValue<int>(dataReader, "TotalReplied");
                model.TotalFailed = SqlDataHelper.GetValue<int>(dataReader, "TotalFailed");

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion


        #region BotFlows 
        public static GetBotModelFlowForViewDto ConvertBotFlowsDto(IDataReader dataReader)
        {
            GetBotModelFlowForViewDto getBotModelFlowForViewDto = new GetBotModelFlowForViewDto();

            getBotModelFlowForViewDto.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");
            getBotModelFlowForViewDto.isPublished = SqlDataHelper.GetValue<bool>(dataReader, "IsPublished");
            getBotModelFlowForViewDto.TenantId = SqlDataHelper.GetValue<int>(dataReader, "TenantId");
            getBotModelFlowForViewDto.CreatedDate = SqlDataHelper.GetValue<DateTime>(dataReader, "CreatedDate");
            getBotModelFlowForViewDto.CreatedUserId = SqlDataHelper.GetValue<long>(dataReader, "CreatedUserId");
            getBotModelFlowForViewDto.CreatedUserName = SqlDataHelper.GetValue<string>(dataReader, "CreatedUserName");

            getBotModelFlowForViewDto.ModifiedDate = SqlDataHelper.GetValue<DateTime>(dataReader, "ModifiedDate") ?? (DateTime?)null;
            getBotModelFlowForViewDto.ModifiedUserId = SqlDataHelper.GetValue<long>(dataReader, "ModifiedUserId");
            getBotModelFlowForViewDto.ModifiedUserName = SqlDataHelper.GetValue<string>(dataReader, "ModifiedUserName");
            getBotModelFlowForViewDto.FlowName = SqlDataHelper.GetValue<string>(dataReader, "FlowName");
            getBotModelFlowForViewDto.StatusId = SqlDataHelper.GetValue<int>(dataReader, "StatusId");

            try
            {
                getBotModelFlowForViewDto.BotChannel = SqlDataHelper.GetValue<string>(dataReader, "BotChannel");
            }
            catch
            {
                getBotModelFlowForViewDto.BotChannel = "watsapp";

            }

            if (string.IsNullOrEmpty(getBotModelFlowForViewDto.BotChannel))
            {


                getBotModelFlowForViewDto.BotChannel = "watsapp";
            }

            var getBotFlowForViewDto = SqlDataHelper.GetValue<string>(dataReader, "JsonModel");
            if (!string.IsNullOrEmpty(getBotFlowForViewDto))
            {
                var options = new JsonSerializerOptions { WriteIndented = true };

                getBotModelFlowForViewDto.getBotFlowForViewDto = System.Text.Json.JsonSerializer.Deserialize<GetBotFlowForViewDto[]>(getBotFlowForViewDto, options);
            }
            else
                getBotModelFlowForViewDto.getBotFlowForViewDto = null;

            return getBotModelFlowForViewDto;
        }
        public static long ConvertBotFlowsByTempIdDto(IDataReader dataReader)
        {
            var Id = SqlDataHelper.GetValue<string>(dataReader, "WhatsAppTemplateId");

            return long.Parse(Id);

        }
        public static BotParameterModel ConvertBotParametrsDto(IDataReader dataReader)
        {
            BotParameterModel botParameterModel = new BotParameterModel();

            botParameterModel.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");
            botParameterModel.Name = SqlDataHelper.GetValue<string>(dataReader, "Name");
            botParameterModel.Format = SqlDataHelper.GetValue<string>(dataReader, "Format");
            botParameterModel.IsDeleted = SqlDataHelper.GetValue<bool>(dataReader, "IsDeleted");
            botParameterModel.TenantId = SqlDataHelper.GetValue<int>(dataReader, "TenantId");

            return botParameterModel;
        }
        #endregion

        #region OrderSoket
        public static OrderSoket OrderSoket(IDataReader dataReader)
        {
            try
            {
                OrderSoket Order = new OrderSoket();

                Order.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");
                Order.CreatorUserId = SqlDataHelper.GetValue<long>(dataReader, "CreatorUserId");
                Order.LastModifierUserId = SqlDataHelper.GetValue<long>(dataReader, "LastModifierUserId");

                Order.IsDeleted = SqlDataHelper.GetValue<bool>(dataReader, "IsDeleted");
                Order.DeleterUserId = SqlDataHelper.GetValue<long>(dataReader, "DeleterUserId");
                Order.TenantId = SqlDataHelper.GetValue<int>(dataReader, "TenantId");
                Order.OrderTime = SqlDataHelper.GetValue<DateTime>(dataReader, "OrderTime") ?? (DateTime?)null;
                Order.OrderRemarks = SqlDataHelper.GetValue<string>(dataReader, "OrderRemarks");
                Order.OrderNumber = SqlDataHelper.GetValue<long>(dataReader, "OrderNumber");

                Order.CreationTime = SqlDataHelper.GetValue<DateTime>(dataReader, "CreationTime") ?? (DateTime?)null;
                Order.DeletionTime = SqlDataHelper.GetValue<DateTime>(dataReader, "DeletionTime") ?? (DateTime?)null;
                Order.LastModificationTime = SqlDataHelper.GetValue<DateTime>(dataReader, "LastModificationTime") ?? (DateTime?)null;

                Order.BranchId = SqlDataHelper.GetValue<long>(dataReader, "BranchId");
                Order.ContactId = SqlDataHelper.GetValue<int>(dataReader, "ContactId");
                Order.AgentId = SqlDataHelper.GetValue<long>(dataReader, "AgentId");
                Order.IsLockByAgent = SqlDataHelper.GetValue<bool>(dataReader, "IsLockByAgent");
                Order.LockByAgentName = SqlDataHelper.GetValue<string>(dataReader, "LockByAgentName");
                Order.Total = SqlDataHelper.GetValue<decimal>(dataReader, "Total") ?? (decimal?)0;

                Order.OrderStatus = (OrderStatusEunm)SqlDataHelper.GetValue<int>(dataReader, "OrderStatus");
                Order.OrderType = (OrderTypeEunm)SqlDataHelper.GetValue<int>(dataReader, "OrderType");

                Order.Address = SqlDataHelper.GetValue<string>(dataReader, "Address");
                Order.AreaId = SqlDataHelper.GetValue<long>(dataReader, "AreaId");
                Order.DeliveryCost = SqlDataHelper.GetValue<decimal>(dataReader, "DeliveryCost") ?? (decimal?)0;

                Order.IsEvaluation = SqlDataHelper.GetValue<bool>(dataReader, "IsEvaluation");
                Order.IsBranchArea = SqlDataHelper.GetValue<bool>(dataReader, "IsBranchArea");

                Order.BranchAreaId = SqlDataHelper.GetValue<int>(dataReader, "BranchAreaId");
                Order.BranchAreaName = SqlDataHelper.GetValue<string>(dataReader, "BranchAreaName");

                Order.LocationID = SqlDataHelper.GetValue<int>(dataReader, "LocationID");
                Order.FromLocationID = SqlDataHelper.GetValue<int>(dataReader, "FromLocationID");
                Order.ToLocationID = SqlDataHelper.GetValue<int>(dataReader, "ToLocationID");

                Order.FromLocationDescribation = SqlDataHelper.GetValue<string>(dataReader, "FromLocationDescribation");
                Order.FromLongitudeLatitude = SqlDataHelper.GetValue<string>(dataReader, "FromLongitudeLatitude");
                Order.OrderDescribation = SqlDataHelper.GetValue<string>(dataReader, "OrderDescribation");
                Order.ToLocationDescribation = SqlDataHelper.GetValue<string>(dataReader, "ToLocationDescribation");
                Order.ToLongitudeLatitude = SqlDataHelper.GetValue<string>(dataReader, "ToLongitudeLatitude");

                Order.AfterDeliveryCost = SqlDataHelper.GetValue<decimal>(dataReader, "AfterDeliveryCost") ?? (decimal?)0;
                Order.IsSpecialRequest = SqlDataHelper.GetValue<bool>(dataReader, "IsSpecialRequest");
                Order.SpecialRequestText = SqlDataHelper.GetValue<string>(dataReader, "SpecialRequestText");
                Order.IsPreOrder = SqlDataHelper.GetValue<bool>(dataReader, "IsPreOrder");

                Order.SelectDay = SqlDataHelper.GetValue<string>(dataReader, "SelectDay");
                Order.SelectTime = SqlDataHelper.GetValue<string>(dataReader, "SelectTime");
                Order.RestaurantName = SqlDataHelper.GetValue<string>(dataReader, "RestaurantName");
                Order.HtmlPrint = SqlDataHelper.GetValue<string>(dataReader, "HtmlPrint");
                Order.BuyType = SqlDataHelper.GetValue<string>(dataReader, "BuyType");
                Order.OrderLocal = SqlDataHelper.GetValue<string>(dataReader, "OrderLocal");

                Order.IsArchived = SqlDataHelper.GetValue<bool>(dataReader, "IsArchived");

                Order.StreetName = SqlDataHelper.GetValue<string>(dataReader, "StreetName");
                Order.BuildingNumber = SqlDataHelper.GetValue<string>(dataReader, "BuildingNumber");
                Order.FloorNo = SqlDataHelper.GetValue<string>(dataReader, "FloorNo");
                Order.ApartmentNumber = SqlDataHelper.GetValue<string>(dataReader, "ApartmentNumber");

                Order.ActionTime = SqlDataHelper.GetValue<DateTime>(dataReader, "ActionTime") ?? (DateTime?)null;
                Order.AgentIds = SqlDataHelper.GetValue<string>(dataReader, "AgentIds");

                Order.TotalPoints = SqlDataHelper.GetValue<decimal>(dataReader, "TotalPoints") ?? (decimal?)0;
                Order.TotalCreditPoints = SqlDataHelper.GetValue<decimal>(dataReader, "TotalCreditPoints") ?? (decimal?)0;

                Order.IsItemOffer = SqlDataHelper.GetValue<bool>(dataReader, "IsItemOffer");
                Order.ItemOffer = SqlDataHelper.GetValue<decimal>(dataReader, "ItemOffer") ?? (decimal?)0;
                Order.IsDeliveryOffer = SqlDataHelper.GetValue<bool>(dataReader, "IsDeliveryOffer");
                Order.DeliveryOffer = SqlDataHelper.GetValue<decimal>(dataReader, "DeliveryOffer") ?? (decimal?)0 ?? (decimal?)0;

                return Order;
            }
            catch (Exception ex)
            {
                return new OrderSoket();
            }


        }
        #endregion


        #region JoPetrolBot 
        public static JoPetrolPhoneModel ConvertJoPetrolPhoneDto(IDataReader dataReader)
        {
            JoPetrolPhoneModel joPetrolPhoneModel = new JoPetrolPhoneModel();
            joPetrolPhoneModel.Id = SqlDataHelper.GetValue<int>(dataReader, "Id");
            joPetrolPhoneModel.PhoneNumber = SqlDataHelper.GetValue<string>(dataReader, "PhoneNumber");

            return joPetrolPhoneModel;
        }
        public static string JoPetrolRequestDto(IDataReader dataReader)
        {
            string joPetrolPhoneModel;
            joPetrolPhoneModel = SqlDataHelper.GetValue<int>(dataReader, "RequestDescription");

            return joPetrolPhoneModel;
        }
        #endregion


        #region Wallet
        public static WalletModel MapWallet(IDataReader dataReader)
        {
            try
            {
                WalletModel model = new WalletModel();
                model.WalletId = SqlDataHelper.GetValue<long>(dataReader, "WalletId");
                model.TenantId = SqlDataHelper.GetValue<int>(dataReader, "TenantId");
                model.TotalAmount = SqlDataHelper.GetValue<decimal>(dataReader, "TotalAmount");
                model.OnHold = SqlDataHelper.GetValue<decimal>(dataReader, "OnHold");
                model.DepositDate = SqlDataHelper.GetValue<DateTime>(dataReader, "DepositDate");

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region User
        public static UsersDashModel MapUserInfo(IDataReader dataReader)
        {
            try
            {
                UsersDashModel model = new UsersDashModel();
                model.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");
                model.Name = SqlDataHelper.GetValue<string>(dataReader, "Name");
                model.UserName = SqlDataHelper.GetValue<string>(dataReader, "UserName");
                model.EmailAddress = SqlDataHelper.GetValue<string>(dataReader, "EmailAddress");


                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion


        #region TransactionForWallet
        public static TransactionModel MapTransactionInfo(IDataReader dataReader)
        {
            try
            {
                TransactionModel model = new TransactionModel();
                model.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");
                model.DoneBy = SqlDataHelper.GetValue<string>(dataReader, "DoneBy");
                model.TotalTransaction = SqlDataHelper.GetValue<decimal>(dataReader, "TotalTransaction");
                model.TransactionDate = SqlDataHelper.GetValue<DateTime>(dataReader, "TransactionDate");
                model.CategoryType = SqlDataHelper.GetValue<string>(dataReader, "CategoryType");
                model.TotalRemaining = SqlDataHelper.GetValue<decimal>(dataReader, "TotalRemaining");
                model.WalletId = SqlDataHelper.GetValue<long>(dataReader, "WalletId");
                model.Country = SqlDataHelper.GetValue<string>(dataReader, "Country");
                model.TenantId = SqlDataHelper.GetValue<int>(dataReader, "TenantId");
                model.invoiceId = SqlDataHelper.GetValue<string>(dataReader, "invoiceId");
                model.invoiceUrl = SqlDataHelper.GetValue<string>(dataReader, "invoiceUrl");
                model.IsPayed = SqlDataHelper.GetValue<bool>(dataReader, "IsPayed");


                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Country 
        public static CountryCodeModel MapCountry(IDataReader dataReader)
        {
            try
            {
                CountryCodeModel model = new CountryCodeModel();
                model.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");
                model.Country = SqlDataHelper.GetValue<string>(dataReader, "Country");
                model.Region = SqlDataHelper.GetValue<string>(dataReader, "Region");
                model.CountryCallingCode = SqlDataHelper.GetValue<string>(dataReader, "CountryCallingCode");
                model.Currency = SqlDataHelper.GetValue<string>(dataReader, "Currency");
                model.MarketingPrice = SqlDataHelper.GetValue<float>(dataReader, "MarketingPrice");
                model.UtilityPrice = SqlDataHelper.GetValue<float>(dataReader, "UtilityPrice");
                model.AuthenticationPrice = SqlDataHelper.GetValue<float>(dataReader, "AuthenticationPrice");
                model.ServicePrice = SqlDataHelper.GetValue<float>(dataReader, "ServicePrice");


                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
        #region Groups
        public static GroupDtoModel MapGroups(IDataReader dataReader)
        {
            //  Id, TenantId, GroupName, CreationDate, ModificationDate, IsDeleted
            GroupDtoModel groupDtoModel = new GroupDtoModel();
            groupDtoModel.id = SqlDataHelper.GetValue<long>(dataReader, "Id");
            groupDtoModel.tenantId = SqlDataHelper.GetValue<int>(dataReader, "TenantId");
            groupDtoModel.groupName = SqlDataHelper.GetValue<string>(dataReader, "GroupName");
            groupDtoModel.creationDate = SqlDataHelper.GetValue<DateTime>(dataReader, "CreationDate");
            groupDtoModel.modificationDate = SqlDataHelper.GetValue<DateTime>(dataReader, "ModificationDate");
            groupDtoModel.IsDeleted = SqlDataHelper.GetValue<bool>(dataReader, "IsDeleted");
            groupDtoModel.totalNumber = SqlDataHelper.GetValue<int>(dataReader, "TotalNumbar");
            groupDtoModel.OnHoldCount = SqlDataHelper.GetValue<int>(dataReader, "OnHoldCount");
            groupDtoModel.FailedCount = SqlDataHelper.GetValue<int>(dataReader, "FailedCount");
            groupDtoModel.TotolForPrograss = SqlDataHelper.GetValue<int>(dataReader, "TotolForPrograss");
            groupDtoModel.CreatorUserName = SqlDataHelper.GetValue<string>(dataReader, "CreatorUserName");
            groupDtoModel.AllUnsubscribed = SqlDataHelper.GetValue<bool>(dataReader, "AllUnsubscribed");

            return groupDtoModel;
        }
        public static GroupPhoneNumberLog MapGroupsLog(IDataReader dataReader)
        {
            GroupPhoneNumberLog groupDtoModel = new GroupPhoneNumberLog();
            groupDtoModel.displayName = SqlDataHelper.GetValue<long>(dataReader, "displayName");
            groupDtoModel.PhoneNumber = SqlDataHelper.GetValue<int>(dataReader, "PhoneNumber");
            groupDtoModel.FailedStatusId = SqlDataHelper.GetValue<string>(dataReader, "FailedStatusId");

            try
            {
                groupDtoModel.Reason = "This number is in another group ( " + SqlDataHelper.GetValue<string>(dataReader, "Reason") + " )"; ;
            }
            catch
            {

                groupDtoModel.Reason = "This number is in another group";
            }

            switch (groupDtoModel.FailedStatusId)
            {
                case 1:
                    groupDtoModel.FailureMessage = "Error while Adding";
                    break;
                case 2:
                    groupDtoModel.FailureMessage = "The number already exists in the group";
                    break;
                case 3:
                    groupDtoModel.FailureMessage = "Phone number validation failed";
                    break;
                case 4:
                    groupDtoModel.FailureMessage = groupDtoModel.Reason;// "This number is in another group";
                    break;
                case 5:
                    groupDtoModel.FailureMessage = "This number is Deleted";
                    break;
                case 6:
                    groupDtoModel.FailureMessage = "This number is Unsubscribe";
                    break;
            }
            groupDtoModel.SetDate = SqlDataHelper.GetValue<DateTime>(dataReader, "SetDate");

            return groupDtoModel;
        }
        public static MembersDto MapMembers(IDataReader dataReader)
        {
            MembersDto groupDtoModel = new MembersDto();

            groupDtoModel.id = SqlDataHelper.GetValue<int>(dataReader, "Id");
            groupDtoModel.phoneNumber = SqlDataHelper.GetValue<string>(dataReader, "PhoneNumber");
            groupDtoModel.displayName = SqlDataHelper.GetValue<string>(dataReader, "DisplayName");
            string jsonString = SqlDataHelper.GetValue<string>(dataReader, "Variables");
            groupDtoModel.customeropt = SqlDataHelper.GetValue<int>(dataReader, "customeropt");

            if (!string.IsNullOrWhiteSpace(jsonString))
            {
                try
                {
                    groupDtoModel.variables = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString);
                }
                catch (JsonException)
                {
                    groupDtoModel.variables = new Dictionary<string, string>();
                }
            }

            return groupDtoModel;
        }
        public static ListContactToCampin MapMembersForCamp(IDataReader dataReader)
        {
            ListContactToCampin model = new ListContactToCampin();

            model.Id = SqlDataHelper.GetValue<int>(dataReader, "Id");
            model.PhoneNumber = SqlDataHelper.GetValue<string>(dataReader, "PhoneNumber");
            model.ContactName = SqlDataHelper.GetValue<string>(dataReader, "DisplayName");
            model.CustomerOPT = SqlDataHelper.GetValue<int>(dataReader, "CustomerOPT");

            return model;
        }
        public static MembersDto MapMembersUpdate(IDataReader dataReader)
        {
            MembersDto groupDtoModel = new MembersDto();

            groupDtoModel.id = SqlDataHelper.GetValue<int>(dataReader, "id");
            groupDtoModel.phoneNumber = SqlDataHelper.GetValue<string>(dataReader, "phoneNumber");
            groupDtoModel.displayName = SqlDataHelper.GetValue<string>(dataReader, "displayName");
            groupDtoModel.failedId = SqlDataHelper.GetValue<int>(dataReader, "failedId");
            //groupDtoModel.isChecked = false;

            return groupDtoModel;
        }
        #endregion




        #region Teams
        public static TeamsDtoModel MapTeams(IDataReader dataReader)
        {
            //  Id, TenantId, GroupName, CreationDate, ModificationDate, IsDeleted
            TeamsDtoModel TeamsDtoModel = new TeamsDtoModel();
            TeamsDtoModel.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");
            TeamsDtoModel.TenantId = SqlDataHelper.GetValue<int>(dataReader, "TenantId");
            TeamsDtoModel.TeamsName = SqlDataHelper.GetValue<string>(dataReader, "TeamsName");
            TeamsDtoModel.CreationDate = SqlDataHelper.GetValue<DateTime>(dataReader, "CreationDate");
            TeamsDtoModel.ModificationDate = SqlDataHelper.GetValue<DateTime>(dataReader, "ModificationDate");
            TeamsDtoModel.IsDeleted = SqlDataHelper.GetValue<bool>(dataReader, "IsDeleted");
            TeamsDtoModel.TotalNumber = SqlDataHelper.GetValue<int>(dataReader, "TotalNumber");
            TeamsDtoModel.OnHoldCount = SqlDataHelper.GetValue<int>(dataReader, "OnHoldCount");
            TeamsDtoModel.FailedCount = SqlDataHelper.GetValue<int>(dataReader, "FailedCount");
            TeamsDtoModel.TotolForPrograss = SqlDataHelper.GetValue<int>(dataReader, "TotolForPrograss");
            TeamsDtoModel.UserIds = SqlDataHelper.GetValue<string>(dataReader, "UserIds");
            return TeamsDtoModel;
        }

        #endregion 
    }
}