using BotService.Interfaces;
using BotService.Models;
using BotService.Models.API;
using Framework.Data;
using Infoseed.MessagingPortal;
using Infoseed.MessagingPortal.Configuration.Tenants.Dto;
using Infoseed.MessagingPortal.Orders;
using Infoseed.MessagingPortal.Orders.Dtos;
using Infoseed.MessagingPortal.SocketIOClient;
using Infoseed.MessagingPortal.Web.Models.Sunshine;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.VisualBasic;
using Nancy.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Device.Location;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GoogleMapModel = Infoseed.MessagingPortal.Web.Models.Sunshine.GoogleMapModel;
using Item = BotService.Models.API.Item;
using JsonSerializer = System.Text.Json.JsonSerializer;
using Order = Infoseed.MessagingPortal.Orders.Order;
using TenantModel = Infoseed.MessagingPortal.Web.Models.Sunshine.TenantModel;
using WorkModel = Infoseed.MessagingPortal.Web.Models.Sunshine.WorkModel;

namespace BotService.Services
{
    public class BotApis : IBotApis

    {
        private static readonly object CurrentOrder = new object();
        private readonly IDocumentClient _IDocumentClient;
        public BotApis(IDocumentClient IDocumentClient)
        {
            _IDocumentClient=IDocumentClient;
        }

        #region public
        public List<string> GetBranchesWithPage(int? TenantID, string local, int pageNumber, int pageSize)
        {

            List<string> vs = new List<string>();
            var list = GetBranchesList(TenantID);

            if (list.Count > 8)
            {
                var values = list.Skip(pageSize * pageNumber).Take(pageSize).ToList();

                if (pageNumber >= 1)
                {
                    var values2 = list.Skip(pageSize * pageNumber).Take(pageSize).ToList();

                    if (local == "ar")
                    {
                        values2.Add(new Area
                        {
                            AreaName = "العودة",
                            AreaCoordinate = ""
                        });
                    }
                    else
                    {
                        values2.Add(new Area
                        {
                            AreaNameEnglish = "Back",
                            AreaCoordinate = ""
                        });
                    }
                    if (list.Count - (values2.Count - 1) - (pageNumber*8)  > 0)
                    {
                        if (local == "ar")
                        {
                            values2.Add(new Area
                            {
                                AreaName = "اخرى",
                                AreaCoordinate = ""
                            });
                        }
                        else
                        {
                            values2.Add(new Area
                            {
                                AreaNameEnglish = "Others",
                                AreaCoordinate = ""
                            });
                        }
                    }
                    foreach (var item in values2)
                    {

                        if (local == "ar")
                        {
                            vs.Add(item.AreaName);
                        }
                        else
                        {
                            vs.Add(item.AreaNameEnglish);
                        }
                    }
                    return vs;
                }
                else
                {
                    var values2 = list.Skip(pageSize * pageNumber).Take(pageSize).ToList();

                    if ((list.Count - values2.Count) > 0)
                    {
                        if (local == "ar")
                        {
                            values2.Add(new Area
                            {
                                AreaName = "اخرى",
                                AreaCoordinate = ""
                            });
                        }
                        else
                        {
                            values2.Add(new Area
                            {
                                AreaNameEnglish = "Others",
                                AreaCoordinate = ""
                            });
                        }
                    }
                    foreach (var item in values2)
                    {

                        if (local == "ar")
                        {
                            vs.Add(item.AreaName);
                        }
                        else
                        {
                            vs.Add(item.AreaNameEnglish);
                        }
                    }
                    return vs;
                }
            }
            else
            {

                foreach (var item in list)
                {

                    if (local == "ar")
                    {

                        vs.Add(item.AreaName);
                    }
                    else
                    {
                        vs.Add(item.AreaNameEnglish);
                    }
                }

                return vs;
            }

        }
        public Area GetBranch(int? TenantID, string AreaName, string local)
        {

            var list = GetBranchesList(TenantID);

            var area = list.Where(x => (x.AreaName).Contains(AreaName)).FirstOrDefault();

            if (local == "ar")
            {

                area = list.Where(x => (x.AreaName).Contains(AreaName)).FirstOrDefault();
            }
            else
            {
                area = list.Where(x => (x.AreaNameEnglish).Contains(AreaName)).FirstOrDefault();
            }


            if (area == null)
            {
                Area area1 = new Area();
                area1.Id = 0;
                return area1;
            }


            return area;

        }

        public LocationInfoModel GetlocationUserModel( SendLocationUserModel input)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            try
            {
                if (IsvalidLatLong(input.query.Split(",")[0], input.query.Split(",")[1]))
                {


                    TenantModel tenant = GetTenantById(input.tenantID).Result;

                    input.isOrderOffer=tenant.isOrderOffer;
                    if (tenant.DeliveryCostType == DeliveryCostType.PerKiloMeter)
                    {
                        LocationInfoModel getLocationInfoModel = getLocationInfoPerKiloMeter(input.tenantID, input.query);


                        return getLocationInfoModel;
                    }
                    else
                    {
                        LocationInfoModel infoLocation = new LocationInfoModel();
                        string result = "-1";
                        string Country = "";
                        string City = "";
                        string Area = "";
                        string Distric = "";
                        string Route = "";

                        if (input.isChangeLocation)
                        {
                            Country = input.address.Split(" - ")[4].Replace("'", "").Trim();
                            City = input.address.Split(" - ")[3].Replace("'", "").Trim();
                            Area = input.address.Split(" - ")[2].Replace("'", "").Trim();
                            Distric = input.address.Split(" - ")[1].Replace("'", "").Trim();
                            Route = input.address.Split(" - ")[0].Replace("'", "").Trim();
                        }
                        else
                        {
                            var rez = GetLocation(input.query);
                            Country = rez.Country.Replace("'", "").Trim();
                            City = rez.City.Replace("'", "").Trim();
                            Area = rez.Area.Replace("'", "").Trim();
                            Distric = rez.Distric.Replace("'", "").Trim();
                            Route = rez.Route.Replace("'", "").Trim();
                        }




                        try
                        {


                            decimal Longitude = decimal.Parse(input.query.Split(",")[0]);
                            decimal Latitude = decimal.Parse(input.query.Split(",")[1]);


                            using (var connection = new SqlConnection(connString))
                            using (var command = connection.CreateCommand())
                            {
                                command.CommandType = CommandType.StoredProcedure;
                                command.CommandText = "dbo.GetDeliveryCost";

                                command.Parameters.AddWithValue("@Longitude", Longitude);
                                command.Parameters.AddWithValue("@Latitude", Latitude);
                                command.Parameters.AddWithValue("@City", City);
                                command.Parameters.AddWithValue("@Area", Area);
                                command.Parameters.AddWithValue("@Distric", Distric);
                                command.Parameters.AddWithValue("@TenantId", input.tenantID);

                                System.Data.SqlClient.SqlParameter returnValue = command.Parameters.Add("@DeliveryCost", SqlDbType.NVarChar);
                                returnValue.Direction = ParameterDirection.ReturnValue;

                                connection.Open();
                                command.ExecuteNonQuery();

                                result = returnValue.Value.ToString();


                            }

                            var spilt = result.Split(",");



                            if (spilt[0] == "-1" || spilt[0] == "")
                            {

                                infoLocation.Country = Country;
                                infoLocation.City = City;
                                infoLocation.Area = Area;
                                infoLocation.Distric = Distric;

                                infoLocation.Address = Route + " - " + Distric + " - " + Area + " - " + City + " - " + Country;
                                infoLocation.DeliveryCostAfter = -1;
                                infoLocation.DeliveryCostBefor = -1;
                                infoLocation.LocationId = 0;

                                return infoLocation;


                            }
                            else
                            {

                                try
                                {

                                    var locationList = GetAllLocationInfoModel();
                                    var cost = decimal.Parse(spilt[0]);
                                    var add = Route + " - " + Distric + " - " + Area + " - " + City + " - " + Country;

                                    var cityModel = locationList.Where(x => x.LocationNameEn == City).FirstOrDefault();
                                    var areaModel = locationList.Where(x => x.LocationNameEn == Area).FirstOrDefault();
                                    var districModel = locationList.Where(x => x.LocationNameEn == Distric).FirstOrDefault();

                                    var areaname = GetBranchesList(input.tenantID).Where(x => x.Id == int.Parse(spilt[1])).FirstOrDefault();

                                    var cityName = "";
                                    var areaName = "";
                                    var districName = "";

                                    if (cityModel != null)
                                        cityName = cityModel.LocationName;
                                    if (areaModel != null)
                                        areaName = areaModel.LocationName;
                                    if (districModel != null)
                                        districName = districModel.LocationName;



                                    infoLocation.Country = Country;
                                    infoLocation.City = City;
                                    infoLocation.Area = Area;
                                    infoLocation.Distric = Distric;

                                    infoLocation.Address = add;


                                    infoLocation.LocationId = int.Parse(spilt[1]);
                                    infoLocation.LocationAreaName=areaname.AreaName;


                                    OrderOfferFun(input.tenantID, input.isOrderOffer, input.OrderTotal, infoLocation, cityName, areaName, districName, cost);



                                    //if (areaname.IsRestaurantsTypeAll)
                                    //{
                                    //    infoLocation.LocationId = 0;

                                    //}

                                    if (areaname != null)
                                    {
                                        if (input.local == "ar")
                                        {
                                            infoLocation.LocationAreaName = areaname.AreaName;
                                        }
                                        else
                                        {

                                            if (areaname.AreaNameEnglish == null)
                                            {

                                                infoLocation.LocationAreaName = areaname.AreaName;
                                            }
                                            else
                                            {
                                                infoLocation.LocationAreaName = areaname.AreaNameEnglish;
                                            }

                                        }

                                    }
                                    infoLocation.DeliveryCostAfter = cost;
                                    if (infoLocation.DeliveryCostBefor == null)
                                    {
                                        infoLocation.DeliveryCostBefor = 0;
                                    }



                                    return infoLocation;
                                }
                                catch
                                {
                                    infoLocation.Country = Country;
                                    infoLocation.City = City;
                                    infoLocation.Area = Area;
                                    infoLocation.Distric = Distric;

                                    infoLocation.Address = Route + " - " + Distric + " - " + Area + " - " + City + " - " + Country;
                                    infoLocation.DeliveryCostAfter = -1;
                                    infoLocation.DeliveryCostBefor = -1;
                                    infoLocation.LocationId = 0;

                                    return infoLocation;


                                }


                            }


                        }
                        catch (Exception)
                        {
                            infoLocation.Country = Country;
                            infoLocation.City = City;
                            infoLocation.Area = Area;
                            infoLocation.Distric = Distric;

                            infoLocation.Address = Route + " - " + Distric + " - " + Area + " - " + City + " - " + Country;
                            infoLocation.DeliveryCostAfter = -1;
                            infoLocation.DeliveryCostBefor = -1;
                            infoLocation.LocationId = 0;

                            return infoLocation;

                        }
                    }
                }
                else
                {
                    LocationInfoModel infoLocation = new LocationInfoModel();
                    infoLocation.Country = "";
                    infoLocation.City = "";
                    infoLocation.Area = "";
                    infoLocation.Distric = "";
                    infoLocation.Address = "";
                    infoLocation.DeliveryCostAfter = -1;
                    infoLocation.DeliveryCostBefor = -1;
                    infoLocation.LocationId = 0;

                    return infoLocation;
                }
            }
            catch (Exception)
            {
                LocationInfoModel infoLocation = new LocationInfoModel();

                infoLocation.Country = "";
                infoLocation.City = "";
                infoLocation.Area = "";
                infoLocation.Distric = "";

                infoLocation.Address = "";
                infoLocation.DeliveryCostAfter = -1;
                infoLocation.DeliveryCostBefor = -1;
                infoLocation.LocationId = 0;

                return infoLocation;
            }



        }

        public string AddMenuContcatKey(MenuContcatKeyModel model)
        {
            try
            {
                var cap = model.Value;
                var url = extractURL(model.Value);
                model.Value=url;
                model.KeyMenu= RandomString(10);
                addMenuContactKey(model);

                var ret = cap.Replace(url, url.Split("?")[0]+"?"+model.KeyMenu);
                return ret;
            }
            catch
            {
                throw;
            }
        }

        public OrderAndDetailsModel GetOrderAndDetails( SendOrderAndDetailModel input)
        {

            TenantModel tenant = GetTenantById(input.TenantID).Result;

            input.isOrderOffer=tenant.isOrderOffer;

            if (input.TypeChoes=="Delivery")
            {
                input.LocationId=input.LocationId;

            }

            var order = getOrderExtraDetails(input.TenantID, input.ContactId);

            List<OrderDetail> OrderDetailList = new List<OrderDetail>();

            List<ExtraOrderDetails> getOrderDetailExtraList = new List<ExtraOrderDetails>();

            if (!string.IsNullOrEmpty(order.OrderDetailDtoJson))
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                OrderDetailList = System.Text.Json.JsonSerializer.Deserialize<List<OrderDetail>>(order.OrderDetailDtoJson, options);
            }
            foreach (var item in OrderDetailList)
            {
                if (item.extraOrderDetailsDtos != null)
                    getOrderDetailExtraList.AddRange(item.extraOrderDetailsDtos);


            }
            //var order = GetOrder(input.TenantID, input.ContactId);
            // var OrderDetailList = GetOrderDetail(input.TenantID, int.Parse(order.Id.ToString()));
            // var getOrderDetailExtraList = GetOrderDetailExtra(input.TenantID);


            // var itemList = GetItem(input.TenantID);

            var orderEffor = GetOrderOffer(input.TenantID);

            var areaEffor = orderEffor.Where(x => x.isAvailable == true && x.isPersentageDiscount == true).FirstOrDefault();
            bool isDiscount = false;
            decimal Discount = 0;

            if (areaEffor != null && input.isOrderOffer)
            {
                if (order.Total >= areaEffor.FeesStart && order.Total <= areaEffor.FeesEnd)
                {


                    var DateNow = Convert.ToDateTime(DateTime.UtcNow.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                    var DateStart = Convert.ToDateTime(areaEffor.OrderOfferDateStart.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                    var DateEnd = Convert.ToDateTime(areaEffor.OrderOfferDateEnd.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));

                    if (DateStart <= DateNow && DateEnd >= DateNow)
                    {
                        var timeNow = Convert.ToDateTime(DateTime.UtcNow.AddHours(AppSettingsModel.AddHour).ToString("HH:mm:ss", CultureInfo.InvariantCulture));
                        var timeStart = Convert.ToDateTime(areaEffor.OrderOfferStart.AddHours(AppSettingsModel.AddHour).ToString("HH:mm:ss ", CultureInfo.InvariantCulture));
                        var timeEnd = Convert.ToDateTime(areaEffor.OrderOfferEnd.AddHours(AppSettingsModel.AddHour).ToString("HH:mm:ss ", CultureInfo.InvariantCulture));

                        if ((timeStart <= timeNow && timeNow <= timeEnd))
                        {

                            if (areaEffor.isBranchDiscount)
                            {
                                if (areaEffor.BranchesIds.Contains(input.LocationId.ToString()))
                                {
                                    order.Total = order.Total - (order.Total * areaEffor.NewFees) / 100;
                                    isDiscount = true;
                                    Discount = areaEffor.NewFees;
                                }
                            }
                            else
                            {
                                order.Total = order.Total - (order.Total * areaEffor.NewFees) / 100;
                                isDiscount = true;
                                Discount = areaEffor.NewFees;
                            }


                        }



                    }
                }
            }
            if (input.LocationInfo==null)
                input.LocationInfo=new LocationInfoModel() { DeliveryCostAfter=0, DeliveryCostBefor=0, LocationId=input.LocationId };

            var after = input.LocationInfo.DeliveryCostAfter;
            decimal? cost = 0;
            if (input.isOrderOffer)
            {
                if (string.IsNullOrEmpty(input.LocationInfo.City))
                {
                    input.LocationInfo.City="NotFound";
                }
                if (string.IsNullOrEmpty(input.LocationInfo.Area))
                {
                    input.LocationInfo.Area="NotFound";
                }
                if (string.IsNullOrEmpty(input.LocationInfo.Distric))
                {
                    input.LocationInfo.Distric="NotFound";
                }

                var locationList = GetAllLocationInfoModel();

                var cityModel = locationList.Where(x => x.LocationNameEn == input.LocationInfo.City).FirstOrDefault();
                var areaModel = locationList.Where(x => x.LocationNameEn == input.LocationInfo.Area).FirstOrDefault();
                var districModel = locationList.Where(x => x.LocationNameEn == input.LocationInfo.Distric).FirstOrDefault();

                var cityName = "NotFound";
                var areaName = "NotFound";
                var districName = "NotFound";

                if (cityModel != null)
                    cityName = cityModel.LocationName;
                if (areaModel != null)
                    areaName = areaModel.LocationName;
                if (districModel != null)
                    districName = districModel.LocationName;



                OrderOfferFun(input.TenantID, input.isOrderOffer, order.Total, input.LocationInfo, cityName, areaName, districName, cost.Value);




                if (input.LocationInfo.isOrderOfferCost)
                {
                    cost =input.LocationInfo.DeliveryCostBefor;
                    input.LocationInfo.DeliveryCostAfter=input.LocationInfo.DeliveryCostBefor;
                    input.LocationInfo.DeliveryCostBefor=after;

                    input.deliveryCostBefor=input.LocationInfo.DeliveryCostBefor;
                    input.deliveryCostAfter=input.LocationInfo.DeliveryCostAfter;

                    input.DeliveryCostText=input.DeliveryCostTextTow.Replace("{0}", cost.ToString());


                    cost=input.deliveryCostAfter;

                    //  order.Total=order.Total+input.LocationInfo.DeliveryCostAfter.Value;
                }
                else
                {
                    input.LocationInfo.DeliveryCostAfter=after;
                    cost=input.deliveryCostAfter;
                }



            }
            else
            {
                cost= input.Cost;

                //  order.Total=order.Total+cost.Value;
            }


            // input.DeliveryCostText=input.DeliveryCostTextTow.Replace("{0}", cost.ToString());

            var OrderString = GetOrderDetailString(input.TenantID, input.lang, order.Total, order.TotalPoints, input.captionQuantityText, input.captionAddtionText, input.captionTotalText, input.captionTotalOfAllText, OrderDetailList, getOrderDetailExtraList, Discount, isDiscount, input.TypeChoes, input.DeliveryCostText, cost);


            //if (input.LocationInfo.isOrderOfferCost)
            //{
            //    order.Total=order.Total+input.LocationInfo.DeliveryCostAfter.Value;
            //}
            OrderAndDetailsModel orderAndDetailsModel = new OrderAndDetailsModel
            {
                order = order,
                DetailText = OrderString,
                orderId = int.Parse(order.Id.ToString()),
                total = order.Total,
                Discount = Discount,
                IsDiscount = isDiscount,
                GetLocationInfo=input.LocationInfo,
                IsItemOffer=isDiscount,
                ItemOffer=Discount,
                IsDeliveryOffer=input.LocationInfo.isOrderOfferCost,
                DeliveryOffer=after

            };



            return orderAndDetailsModel;
        }

        public void DeleteOrderDraft(int tenantID, long orderId)
        {

            var orderDetail = GetOrderDetail(tenantID, orderId);
            var OrderDetailExtraList = GetOrderDetailExtra(tenantID);

            foreach (var deteal in orderDetail)
            {

                var GetOrderDetailExtraDraft = OrderDetailExtraList.Where(x => x.OrderDetailId == deteal.Id).ToList();

                foreach (var ExtraOrde in GetOrderDetailExtraDraft)
                {

                    DeleteExtraOrderDetail(ExtraOrde.Id);
                }

                DeleteOrderDetails(deteal.Id);

            }

            DeleteOrder(orderId);

        }

        public CancelOrderModel UpdateCancelOrder(int? TenantID, string OrderNumber, int ContactId, string CanatCancelOrderText)
        {
            CancelOrderModel cancelOrderModel = new CancelOrderModel();

            try
            {


                var OrderModel = GetOrderListWithContact(TenantID, ContactId, OrderNumber).Where(x => x.OrderNumber == long.Parse(OrderNumber)).FirstOrDefault();

                if (OrderModel != null)
                {
                    var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
                    TenantModel tenant = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.TenantId == TenantID).Result;

                    if (!createOrderStatusHistory(OrderModel.Id, (int)OrderStatusEunm.Canceled, TenantID))
                    {
                        cancelOrderModel.CancelOrder = true;
                        cancelOrderModel.WrongOrder = false;
                        cancelOrderModel.IsTrueOrder = true;
                        cancelOrderModel.TextCancelOrder = CanatCancelOrderText;
                        return cancelOrderModel;
                    }

                    if (tenant.IsCancelOrder)
                    {

                        TimeSpan timeSpan = DateTime.Now.AddHours(AppSettingsModel.AddHour) - OrderModel.CreationTime;
                        int totalMinutes = (int)Math.Ceiling(timeSpan.TotalMinutes);

                        if (totalMinutes >= tenant.CancelTime)
                        {

                            cancelOrderModel.CancelOrder = true;
                            cancelOrderModel.WrongOrder = false;
                            cancelOrderModel.IsTrueOrder = true;
                            cancelOrderModel.TextCancelOrder = CanatCancelOrderText;
                            return cancelOrderModel;

                        }
                    }

                    var x = UpdateOrderAfterCancel(OrderNumber, ContactId, OrderModel, TenantID);
                    cancelOrderModel.CancelOrder = false;
                    cancelOrderModel.WrongOrder = false;
                    cancelOrderModel.IsTrueOrder = true;




                    decimal total = 0;
                    if (OrderModel.Total > 0)
                    {
                        decimal DeliveryCost = OrderModel.DeliveryCost.HasValue ? OrderModel.DeliveryCost.Value : 0;
                        total = (decimal)(OrderModel.Total - DeliveryCost);
                    }
                    else
                    {
                        total = 0;
                    }
                    if (total < 0)
                    {
                        total = 0;
                    }




                    return cancelOrderModel;


                }
                else
                {

                    cancelOrderModel.CancelOrder = false;
                    cancelOrderModel.WrongOrder = true;
                    cancelOrderModel.IsTrueOrder = false;
                    return cancelOrderModel;

                }
            }
            catch
            {

                cancelOrderModel.CancelOrder = false;
                cancelOrderModel.WrongOrder = true;
                cancelOrderModel.IsTrueOrder = false;
                return cancelOrderModel;
            }


        }

        public  string UpdateOrder( UpdateOrderModel updateOrderModel)
        {


            if (updateOrderModel.BuyType == "No select")
                updateOrderModel.BuyType = "";

            var time = DateTime.Now;
            var timeAdd = time.AddHours(AppSettingsModel.AddHour);
            string connString = AppSettingsModel.ConnectionStrings;
            long number = 0;
            //var con = GetContact(updateOrderModel.ContactId);
            var con = getContactbyId(updateOrderModel.ContactId);

            lock (CurrentOrder)
            {
                number = UpateOrder(updateOrderModel.TenantID);

            }

            var captionTotalPointOfAllOrderText = "السعر الإجمالي لنقاط";

            if (updateOrderModel.BotLocal=="ar")
            {
                captionTotalPointOfAllOrderText="السعر الإجمالي لنقاط";
            }
            else
            {

                captionTotalPointOfAllOrderText="Total price for points";
            }


            if (updateOrderModel.TypeChoes == "Pickup")
            {

                var area = GetBranchesList(updateOrderModel.TenantID).Where(x => x.Id == updateOrderModel.BranchId).FirstOrDefault();
                BotService.Models.API.Order order = new BotService.Models.API.Order
                {

                    OrderLocal = updateOrderModel.BotLocal,
                    BranchId=updateOrderModel.BranchId,
                    HtmlPrint = "",
                    SpecialRequestText = updateOrderModel.SpecialRequest,
                    IsSpecialRequest= updateOrderModel.IsSpecialRequest,
                    AreaId = updateOrderModel.BranchId,
                    BranchAreaId= updateOrderModel.BranchId,
                    ContactId = updateOrderModel.ContactId,
                    OrderTime = timeAdd,
                    CreationTime = timeAdd,
                    Id = updateOrderModel.OrderId,
                    OrderNumber = number,
                    TenantId = updateOrderModel.TenantID,
                    orderStatus = OrderStatusEunm.Pending,
                    OrderType = OrderTypeEunm.Takeaway,
                    Total = updateOrderModel.OrderTotal,
                    IsDeleted = false,
                    AgentId = 0,
                    AgentIds = area.UserIds,
                    IsLockByAgent = false,
                    LockByAgentName="",
                    IsEvaluation =false,
                    Address="",
                    IsBranchArea=false,
                    BranchAreaName="",
                    LocationID=0,
                    FromLocationID=0,
                    ToLocationID=0,
                    FromLocationDescribation="",
                    FromLongitudeLatitude="",
                    OrderDescribation="",
                    AfterDeliveryCost=0,


                    IsPreOrder= updateOrderModel.IsPreOrder,
                    SelectDay= updateOrderModel.SelectDay,
                    SelectTime= updateOrderModel.SelectTime,
                    RestaurantName="",
                    BuyType="",
                    TotalPoints=updateOrderModel.loyalityPoint.Value,

                    IsItemOffer=updateOrderModel.IsItemOffer,
                    IsDeliveryOffer=updateOrderModel.IsDeliveryOffer,
                    ItemOffer=updateOrderModel.ItemOffer,
                    DeliveryOffer=updateOrderModel.DeliveryOffer


                };

                UpdateOrder(JsonConvert.SerializeObject(order), order.Id, order.TenantId);




                var ListString = "------------------ \r\n\r\n";

                ListString = ListString + updateOrderModel.captionOrderInfoText;
                ListString = ListString + updateOrderModel.captionOrderNumberText + number + "\r\n";
                ListString = ListString + updateOrderModel.aptionAreaNameText + updateOrderModel.BranchName + "\r\n";
                if (updateOrderModel.loyalityPoint.Value>0)
                {

                    ListString = ListString + captionTotalPointOfAllOrderText + updateOrderModel.loyalityPoint + "\r\n";

                }
                ListString = ListString + updateOrderModel.captionTotalOfAllOrderText + updateOrderModel.OrderTotal + "\r\n";
                ListString = ListString + "------------------ \r\n\r\n";
                ListString = ListString + updateOrderModel.captionEndOrderText;




                var orderStatusName = Enum.GetName(typeof(OrderStatusEunm), order.orderStatus);
                var orderTypeName = Enum.GetName(typeof(OrderTypeEunm), order.OrderType);

                GetOrderForViewDto getOrderForViewDto = new GetOrderForViewDto();
                // var GetOrderMap = _mapper.Map(order, getOrderForViewDto.Order);

                getOrderForViewDto.Order=new OrderDto()
                {
                    OrderTime =order.OrderTime,
                    OrderRemarks =order.OrderRemarks,
                    OrderNumber =order.OrderNumber,
                    CreationTime =order.CreationTime,
                    DeletionTime =order.DeletionTime,
                    LastModificationTime =order.LastModificationTime,
                    DeliveryChangeId =order.DeliveryChangeId,
                    BranchId =order.BranchId,
                    ContactId =order.ContactId,
                    OrderStatus = order.orderStatus,
                    OrderType =order.OrderType,
                    IsLockByAgent =order.IsLockByAgent,
                    AgentId  =order.AgentId,
                    LockByAgentName  =order.LockByAgentName,
                    Total  =order.Total,
                    TotalPoints  =order.TotalPoints,
                    StringTotal  =order.StringTotal,
                    Address  =order.Address,
                    AreaId  =order.AreaId,
                    DeliveryCost  =order.DeliveryCost,
                    AfterDeliveryCost  =order.AfterDeliveryCost,
                    BranchAreaId  =order.BranchAreaId,
                    BranchAreaName  =order.BranchAreaName,
                    FromLocationDescribation  =order.FromLocationDescribation,
                    ToLocationDescribation  =order.ToLocationDescribation,
                    OrderDescribation  =order.OrderDescribation,
                    IsSpecialRequest  =order.IsSpecialRequest,
                    SpecialRequestText  =order.SpecialRequestText,
                    SelectDay  =order.SelectDay,
                    SelectTime  =order.SelectTime,
                    IsPreOrder  =order.IsPreOrder,
                    RestaurantName  =order.RestaurantName,
                    HtmlPrint  =order.HtmlPrint,
                    BuyType  =order.BuyType,
                    ActionTime  =order.ActionTime,
                    AgentIds  =order.AgentIds,
                    OrderDetailDtoJson  =order.OrderDetailDtoJson,
                    TenantId  =order.TenantId,
                    CaptionJson  =order.CaptionJson,
                    OrderOfferJson  =order.OrderOfferJson,
                    IsItemOffer  =order.IsItemOffer,
                    ItemOffer  =order.ItemOffer,
                    IsDeliveryOffer  =order.IsDeliveryOffer,
                    DeliveryOffer  =order.DeliveryOffer,
                    OrderLocal  =order.OrderLocal
                };

                getOrderForViewDto.CustomerMobile = con.PhoneNumber;
                getOrderForViewDto.Order = getOrderForViewDto.Order;
                getOrderForViewDto.AreahName = updateOrderModel.BranchName;
                getOrderForViewDto.OrderStatusName = orderStatusName;
                getOrderForViewDto.OrderTypeName = orderTypeName;
                getOrderForViewDto.AgentIds = area.UserIds;
                getOrderForViewDto.CustomerID=con.UserId;
                getOrderForViewDto.IsAssginToAllUser = area.IsAssginToAllUser;
                getOrderForViewDto.IsAvailableBranch = area.IsAvailableBranch;
                getOrderForViewDto.TenantId = updateOrderModel.TenantID;
                getOrderForViewDto.CustomerCustomerName = con.DisplayName;
                getOrderForViewDto.CreatDate = getOrderForViewDto.Order.CreationTime.ToString("MM/dd/yyyy");
                getOrderForViewDto.CreatTime = getOrderForViewDto.Order.CreationTime.ToString("hh:mm tt");
                getOrderForViewDto.Order.StringTotal = (Math.Round(getOrderForViewDto.Order.Total * 100) / 100).ToString("N2");


                try
                {
                    var titl = "The Order Number: " + number.ToString();
                    var body = "Order Status :" + OrderTypeEunm.Takeaway.ToString() + " From :" + area.AreaName;
                    SendMobileNotification(order.TenantId, titl, body);
                }
                catch (Exception)
                {

                }



                SocketIOManager.SendOrder(getOrderForViewDto, updateOrderModel.TenantID);

                //delete bot conversation
                //  DeleteConversation(usertodelete.SunshineConversationId);

                con.TotalOrder = con.TotalOrder + 1;
                con.TakeAwayOrder = con.TakeAwayOrder + 1;

                //con.loyalityPoint=con.loyalityPoint+(int)updateOrderModel.loyalityPoint.Value;


                //   var CardPoints = _loyaltyAppService.ConvertCustomerPriceToPoint(updateOrderModel.OrderTotal, updateOrderModel.TenantID);
                // var Loyaltymodel = _loyaltyAppService.GetAll(updateOrderModel.TenantID);


                //var DateNow = Convert.ToDateTime(DateTime.Now.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                //var DateStart = Convert.ToDateTime(Loyaltymodel.StartDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                //var DateEnd = Convert.ToDateTime(Loyaltymodel.EndDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));



                //if (Loyaltymodel.IsLoyalityPoint && Loyaltymodel.Id != 0 && Loyaltymodel.OrderType.Contains(((int)order.OrderType).ToString())&& DateStart <= DateNow && DateEnd >= DateNow)
                //{
                //    ContactLoyaltyTransactionModel contactLoyalty = new ContactLoyaltyTransactionModel();
                //    contactLoyalty.ContactId = updateOrderModel.ContactId;
                //    contactLoyalty.LoyaltyDefinitionId =Loyaltymodel.Id;
                //    contactLoyalty.OrderId = order.Id;
                //    contactLoyalty.CreatedBy=updateOrderModel.ContactId;
                //    contactLoyalty.Points = CardPoints;
                //    contactLoyalty.CardPoints= updateOrderModel.loyalityPoint.Value;
                //    contactLoyalty.TransactionTypeId=1;
                //    _loyaltyAppService.CreateContactLoyaltyTransaction(contactLoyalty); // point

                //    //contactLoyalty.Points =-updateOrderModel.loyalityPoint.Value;
                //   // _loyaltyAppService.CreateContactLoyaltyTransaction(contactLoyalty); //subtract points

                //}







                // var contact = await _dbService.UpdateCustomerLocation(con);

                return ListString;

            }
            else
            {

                var area = new Area();
                try
                {
                    area = GetArea((int)updateOrderModel.MenuId);
                }
                catch
                {
                    area = null;

                }


                long agId = 0;
                if (area != null && area.Id != 0)
                {

                    if (area.UserId != 0)
                    {

                        agId = long.Parse(area.UserId.ToString());
                    }
                    else
                    {
                        area = GetArea(updateOrderModel.BranchId);
                        agId = updateOrderModel.BranchId;
                    }
                }
                else
                {
                    area = GetArea(updateOrderModel.BranchId);
                    agId = updateOrderModel.BranchId;
                }
                string AgentIds = area.UserIds;
                if (string.IsNullOrEmpty(AgentIds))
                    AgentIds = null;

                var BranchAreaName = updateOrderModel.BranchName;

                decimal totalWithBranchCost = 0;
                decimal Cost = 0;

                if (updateOrderModel.isOrderOfferCost)
                {
                    totalWithBranchCost = (updateOrderModel.OrderTotal + updateOrderModel.DeliveryCostBefor);
                    Cost = updateOrderModel.DeliveryCostBefor;
                }
                else
                {
                    totalWithBranchCost = (updateOrderModel.OrderTotal + updateOrderModel.DeliveryCostAfter);
                    Cost = updateOrderModel.DeliveryCostAfter;
                }




                BotService.Models.API.Order order = new BotService.Models.API.Order
                {
                    OrderLocal = updateOrderModel.BotLocal,
                    BuyType = updateOrderModel.BuyType,
                    SelectDay = updateOrderModel.SelectDay,
                    SelectTime = updateOrderModel.SelectTime,
                    IsPreOrder = updateOrderModel.IsPreOrder,

                    SpecialRequestText = updateOrderModel.SpecialRequest,
                    IsSpecialRequest = updateOrderModel.IsSpecialRequest,
                    AfterDeliveryCost = updateOrderModel.DeliveryCostAfter,
                    AreaId = updateOrderModel.BranchId,

                    BranchAreaName = BranchAreaName,
                    BranchAreaId = updateOrderModel.BranchId,
                    Address = updateOrderModel.Address,
                    BranchId = updateOrderModel.BranchId,
                    ContactId = updateOrderModel.ContactId,
                    OrderTime = timeAdd,
                    CreationTime = timeAdd,
                    Id = updateOrderModel.OrderId,
                    OrderNumber = number,
                    TenantId = updateOrderModel.TenantID,
                    orderStatus = OrderStatusEunm.Pending,
                    OrderType = OrderTypeEunm.Delivery,
                    Total = totalWithBranchCost,
                    IsDeleted = false,
                    AgentId = 0,
                    AgentIds = AgentIds,
                    LocationID = updateOrderModel.BranchId,
                    FromLocationDescribation = "https://maps.google.com/?q=" + updateOrderModel.LocationFrom.Replace(" ", ""),
                    HtmlPrint = "",

                    IsLockByAgent = false,
                    LockByAgentName="",
                    IsEvaluation =false,


                    IsBranchArea=false,
                    FromLocationID=0,
                    ToLocationID=0,
                    FromLongitudeLatitude="",
                    OrderDescribation="",
                    RestaurantName="",
                    TotalPoints=updateOrderModel.loyalityPoint.Value,
                    DeliveryCost= Cost,

                    IsItemOffer=updateOrderModel.IsItemOffer,
                    IsDeliveryOffer=updateOrderModel.IsDeliveryOffer,
                    ItemOffer=updateOrderModel.ItemOffer,
                    DeliveryOffer=updateOrderModel.DeliveryOffer

                };






                UpdateOrder(JsonConvert.SerializeObject(order), order.Id, order.TenantId);



                //var captionBranchCostText = GetCaptionFormat("BackEnd_Text_BranchCost", orderBotData.BotLocal, orderBotData.TenantID, "", "", 0);//قيمة التوصيل :
                //var captionFromLocationText = GetCaptionFormat("BackEnd_Text_FromLocation", orderBotData.BotLocal, orderBotData.TenantID, "", "", 0);//من الموقع :
                // UpdateDeliveryOrderDB(updateOrderModel, timeAdd, connString, number, BranchAreaName, area);



                var ListString = "------------------ \r\n\r\n";

                ListString = ListString + updateOrderModel.captionOrderInfoText;
                ListString = ListString + updateOrderModel.captionOrderNumberText + number + "\r\n";

                if (updateOrderModel.TenantID == 46)
                {
                    ListString = ListString + updateOrderModel.captionBranchCostText + ((int)Cost).ToString() + "\r\n";
                }
                else
                {
                    ListString = ListString + updateOrderModel.captionBranchCostText + Cost + "\r\n";
                }

                ListString = ListString + updateOrderModel.captionFromLocationText + updateOrderModel.Address + "\r\n";



                if (updateOrderModel.TenantID == 46)
                {
                    if (updateOrderModel.loyalityPoint.Value>0)
                    {
                        ListString = ListString + captionTotalPointOfAllOrderText + (updateOrderModel.loyalityPoint.Value).ToString() + "\r\n\r\n";

                    }
                    ListString = ListString + updateOrderModel.captionTotalOfAllOrderText + ((int)totalWithBranchCost).ToString() + "\r\n\r\n";
                }
                else
                {
                    if (updateOrderModel.loyalityPoint.Value>0)
                    {
                        ListString = ListString + captionTotalPointOfAllOrderText + updateOrderModel.loyalityPoint.Value.ToString() + "\r\n\r\n";

                    }
                    ListString = ListString + updateOrderModel.captionTotalOfAllOrderText + totalWithBranchCost + "\r\n\r\n";
                }



                ListString = ListString + "------------------ \r\n\r\n";
                ListString = ListString + updateOrderModel.captionEndOrderText;


                if (updateOrderModel.IsPreOrder)
                {
                    order.orderStatus = OrderStatusEunm.Pre_Order;
                }

                try
                {
                    var titl = "the order Number: " + number.ToString();
                    var body = "Order Status :" + order.orderStatus + " From :" + updateOrderModel.Address;

                    // await SendNotificationsAsync("fcm", updateOrderModel.TenantID.ToString(), titl, body);
                    SendMobileNotification(order.TenantId, titl, body);
                }
                catch
                {

                }


                var orderStatusName = Enum.GetName(typeof(OrderStatusEunm), order.orderStatus);
                var orderTypeName = Enum.GetName(typeof(OrderTypeEunm), order.OrderType);

                GetOrderForViewDto getOrderForViewDto = new GetOrderForViewDto();
                // var GetOrderMap = _mapper.Map(order, getOrderForViewDto.Order);

                getOrderForViewDto.Order=new OrderDto()
                {
                    OrderTime =order.OrderTime,
                    OrderRemarks =order.OrderRemarks,
                    OrderNumber =order.OrderNumber,
                    CreationTime =order.CreationTime,
                    DeletionTime =order.DeletionTime,
                    LastModificationTime =order.LastModificationTime,
                    DeliveryChangeId =order.DeliveryChangeId,
                    BranchId =order.BranchId,
                    ContactId =order.ContactId,
                    OrderStatus = order.orderStatus,
                    OrderType =order.OrderType,
                    IsLockByAgent =order.IsLockByAgent,
                    AgentId  =order.AgentId,
                    LockByAgentName  =order.LockByAgentName,
                    Total  =order.Total,
                    TotalPoints  =order.TotalPoints,
                    StringTotal  =order.StringTotal,
                    Address  =order.Address,
                    AreaId  =order.AreaId,
                    DeliveryCost  =order.DeliveryCost,
                    AfterDeliveryCost  =order.AfterDeliveryCost,
                    BranchAreaId  =order.BranchAreaId,
                    BranchAreaName  =order.BranchAreaName,
                    FromLocationDescribation  =order.FromLocationDescribation,
                    ToLocationDescribation  =order.ToLocationDescribation,
                    OrderDescribation  =order.OrderDescribation,
                    IsSpecialRequest  =order.IsSpecialRequest,
                    SpecialRequestText  =order.SpecialRequestText,
                    SelectDay  =order.SelectDay,
                    SelectTime  =order.SelectTime,
                    IsPreOrder  =order.IsPreOrder,
                    RestaurantName  =order.RestaurantName,
                    HtmlPrint  =order.HtmlPrint,
                    BuyType  =order.BuyType,
                    ActionTime  =order.ActionTime,
                    AgentIds  =order.AgentIds,
                    OrderDetailDtoJson  =order.OrderDetailDtoJson,
                    TenantId  =order.TenantId,
                    CaptionJson  =order.CaptionJson,
                    OrderOfferJson  =order.OrderOfferJson,
                    IsItemOffer  =order.IsItemOffer,
                    ItemOffer  =order.ItemOffer,
                    IsDeliveryOffer  =order.IsDeliveryOffer,
                    DeliveryOffer  =order.DeliveryOffer,
                    OrderLocal  =order.OrderLocal
                };
                getOrderForViewDto.CustomerMobile = con.PhoneNumber;
                getOrderForViewDto.Order = getOrderForViewDto.Order;
                getOrderForViewDto.OrderStatusName = orderStatusName;
                getOrderForViewDto.OrderTypeName = orderTypeName;
                getOrderForViewDto.AgentIds = AgentIds;
                getOrderForViewDto.CustomerID=con.UserId;
                getOrderForViewDto.BranchAreaName = BranchAreaName;
                //getOrderForViewDto.DeliveryCost = Cost;
                getOrderForViewDto.IsAssginToAllUser = true;// area.IsAssginToAllUser;
                getOrderForViewDto.IsAvailableBranch = true;// area.IsAvailableBranch;
                getOrderForViewDto.TenantId = updateOrderModel.TenantID;
                getOrderForViewDto.CustomerCustomerName = con.DisplayName;
                getOrderForViewDto.CreatDate = getOrderForViewDto.Order.CreationTime.ToString("MM/dd/yyyy");
                getOrderForViewDto.CreatTime = getOrderForViewDto.Order.CreationTime.ToString("hh:mm tt");

                if (updateOrderModel.isOrderOfferCost)
                {
                    getOrderForViewDto.DeliveryCost = updateOrderModel.DeliveryCostBefor;
                }
                else
                {
                    getOrderForViewDto.DeliveryCost = updateOrderModel.DeliveryCostAfter;
                }


                getOrderForViewDto.Order.StringTotal = (Math.Round(getOrderForViewDto.Order.Total * 100) / 100).ToString("N2");


                SocketIOManager.SendOrder(getOrderForViewDto, updateOrderModel.TenantID);


                con.TotalOrder = con.TotalOrder + 1;
                con.DeliveryOrder = con.DeliveryOrder + 1;
                con.Description = updateOrderModel.Address;
                con.Website = updateOrderModel.AddressEn;
                con.EmailAddress = updateOrderModel.LocationFrom;

                con.StreetName = updateOrderModel.StreetName;
                con.BuildingNumber = updateOrderModel.BuildingNumber;
                con.FloorNo = updateOrderModel.FloorNo;
                con.ApartmentNumber = updateOrderModel.ApartmentNumber;


                var contact = UpdateCustomerLocation(con).Result;


                contact.customerChat = null;
                return ListString;

            }



        }
        public List<Caption> GetAllCaption(int TenantID, string local)
        {
            int localID = 1;
            if (local == "en")
            {
                localID = 2;

            }
            else
            {
                localID = 1;
            }

            try
            {
                string connString = AppSettingsModel.ConnectionStrings;
                string query = "select * from [dbo].[Caption] where TenantID=" + TenantID + "and LanguageBotId =  " + localID;


                SqlConnection conn = new SqlConnection(connString);
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                // create the DataSet 
                DataSet dataSet = new DataSet();

                // create data adapter
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                // this will query your database and return the result to your datatable
                da.Fill(dataSet);

                List<Caption> captions = new List<Caption>();

                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {

                    captions.Add(new Caption
                    {
                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                        Text = dataSet.Tables[0].Rows[i]["Text"].ToString(),
                        TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
                        LanguageBotId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["LanguageBotId"]),
                        TextResourceId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TextResourceId"]),

                    });
                }

                conn.Close();
                da.Dispose();

                return captions;

            }
            catch
            {
                return null;

            }

        }

        public void UpdateCustomer(CustomerModel customer)
        {
            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem  && a.userId == customer.userId);//&& a.TenantId== TenantId
            var Result = itemsCollection.UpdateItemAsync(customer._self, customer).Result;
           
        }
        public  CustomerModel GetCustomer(string id)
        {
            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem  && a.userId == id);//&& a.TenantId== TenantId
            return customerResult.Result;
        }

        #endregion


        #region private
        private List<Area> GetBranchesList(int? TenantID)
        {
            //TenantID = "31";
            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[Areas] where TenantID=" + TenantID+" and  IsAvailableBranch = 1";


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<Area> branches = new List<Area>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                try
                {
                    branches.Add(new Area
                    {
                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                        AreaCoordinate = dataSet.Tables[0].Rows[i]["AreaCoordinate"].ToString() ?? "",
                        AreaName = dataSet.Tables[0].Rows[i]["AreaName"].ToString() ?? "",
                        AreaCoordinateEnglish = dataSet.Tables[0].Rows[i]["AreaCoordinateEnglish"].ToString() ?? "",
                        AreaNameEnglish = dataSet.Tables[0].Rows[i]["AreaNameEnglish"].ToString() ?? "",
                        TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
                        BranchID = dataSet.Tables[0].Rows[i]["BranchID"].ToString(),
                        UserId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["UserId"]),
                        RestaurantsType = Convert.ToInt32(dataSet.Tables[0].Rows[i]["RestaurantsType"]),
                        IsAssginToAllUser = bool.Parse(dataSet.Tables[0].Rows[i]["IsAssginToAllUser"].ToString()),
                        IsAvailableBranch = bool.Parse(dataSet.Tables[0].Rows[i]["IsAvailableBranch"].ToString()),
                        IsRestaurantsTypeAll = bool.Parse(dataSet.Tables[0].Rows[i]["IsRestaurantsTypeAll"].ToString()),
                        UserIds = (dataSet.Tables[0].Rows[i]["UserIds"].ToString()),
                        SettingJson = dataSet.Tables[0].Rows[i]["SettingJson"].ToString()


                    });
                }
                catch
                {
                    branches.Add(new Area
                    {
                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                        AreaCoordinate = dataSet.Tables[0].Rows[i]["AreaCoordinate"].ToString() ?? "",
                        AreaName = dataSet.Tables[0].Rows[i]["AreaName"].ToString() ?? "",
                        AreaCoordinateEnglish = dataSet.Tables[0].Rows[i]["AreaCoordinateEnglish"].ToString() ?? "",
                        AreaNameEnglish = dataSet.Tables[0].Rows[i]["AreaNameEnglish"].ToString() ?? "",
                        TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
                        BranchID = dataSet.Tables[0].Rows[i]["BranchID"].ToString(),
                        UserId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["UserId"]),
                        RestaurantsType = Convert.ToInt32(dataSet.Tables[0].Rows[i]["RestaurantsType"]),
                        IsAssginToAllUser = bool.Parse(dataSet.Tables[0].Rows[i]["IsAssginToAllUser"].ToString()),
                        IsAvailableBranch = bool.Parse(dataSet.Tables[0].Rows[i]["IsAvailableBranch"].ToString()),
                        IsRestaurantsTypeAll = bool.Parse(dataSet.Tables[0].Rows[i]["IsRestaurantsTypeAll"].ToString()),
                        UserIds = (dataSet.Tables[0].Rows[i]["UserIds"].ToString()),
                        //SettingJson = dataSet.Tables[0].Rows[i]["SettingJson"].ToString()


                    });
                }


            }

            conn.Close();
            da.Dispose();

            return branches;
        }
        private bool IsvalidLatLong(string latit, string longit)
        {
            double lon, lat;
            bool IsDoubleLat, IsDoubleLong, isValidLat, isValidLong;

            IsDoubleLat = double.TryParse(latit, out lat);
            IsDoubleLong = double.TryParse(longit, out lon);

            if (IsDoubleLat && IsDoubleLong)
            {
                if (lat < -90 || lat > 90)
                {
                    isValidLat = false;
                }
                else
                {
                    isValidLat = true;
                }
                if (lon < -180 || lon > 180)
                {
                    isValidLong = false;
                }
                else
                {
                    isValidLong = true;
                }
            }
            else
            {
                isValidLong = false;
                isValidLat = false;
            }

            return isValidLong && isValidLat;
        }

        private locationAddressModel GetLocation(string query)
        {
            try
            {
                var client = new HttpClient();
                string Key = AppSettingsModel.GoogleMapsKey;
                var url = $"https://maps.googleapis.com/maps/api/geocode/json?latlng={query}&key=" + Key;
                var response = client.GetAsync(url).Result;

                var result = response.Content.ReadAsStringAsync().Result;
                var resultO = Newtonsoft.Json.JsonConvert.DeserializeObject<GoogleMapModel>(result);



                ////
                locationAddressModel locationAddressModel = new locationAddressModel();
                var rez = resultO;

                foreach (var item in rez.results)
                {

                    //route
                    var route = item.types.Where(x => x == "street_address").FirstOrDefault();
                    if (route != null)
                    {

                        locationAddressModel.Route = item.formatted_address.Split(",")[0];
                        //break;
                    }
                    else
                    {
                        route = item.types.Where(x => x == "route").FirstOrDefault();
                        if (route != null)
                        {

                            locationAddressModel.Route = item.formatted_address.Split(",")[0];

                        }
                        else
                        {
                            //locationAddressModel.Route = "";
                        }
                    }

                    //Distric
                    var neighborhood = item.types.Where(x => x == "neighborhood").FirstOrDefault();
                    if (neighborhood != null)
                    {


                        locationAddressModel.Distric = item.address_components.FirstOrDefault().long_name;

                    }
                    else
                    {
                        neighborhood = item.types.Where(x => x == "administrative_area_level_2").FirstOrDefault();
                        if (neighborhood != null && locationAddressModel.Distric == null)
                        {
                            locationAddressModel.Distric = item.address_components.FirstOrDefault().long_name;


                        }
                        else
                        {
                            //locationAddressModel.Distric = "";

                        }


                    }


                    //Area
                    var sublocality_level_1 = item.types.Where(x => x == "sublocality_level_1").FirstOrDefault();
                    if (sublocality_level_1 != null)
                    {

                        locationAddressModel.Area = item.address_components.FirstOrDefault().long_name;

                    }
                    else
                    {
                        //locationAddressModel.Area = "";
                        ///

                    }


                    //city
                    var locality = item.types.Where(x => x == "locality").FirstOrDefault();
                    if (locality != null)
                    {

                        locationAddressModel.City = item.address_components.FirstOrDefault().long_name.Split(",")[0];

                    }
                    else
                    {


                        locality = item.types.Where(x => x == "administrative_area_level_1").FirstOrDefault();
                        if (locality != null)
                        {
                            if (item.address_components.FirstOrDefault().long_name.Split(",")[0] == "Jerash Governorate")
                            {
                                locationAddressModel.City = item.address_components.FirstOrDefault().long_name.Split(",")[0];
                            }


                        }
                        //locationAddressModel.City = "";
                    }


                    //Country
                    var country = item.types.Where(x => x == "country").FirstOrDefault();
                    if (country != null)
                    {

                        locationAddressModel.Country = item.address_components.FirstOrDefault().long_name;
                        //break;
                    }


                }

                if (locationAddressModel.Route == null)
                    locationAddressModel.Route = "";
                if (locationAddressModel.Distric == null)
                    locationAddressModel.Distric = "";
                if (locationAddressModel.Area == null)
                    locationAddressModel.Area = "";
                if (locationAddressModel.City == null)
                    locationAddressModel.City = "";
                if (locationAddressModel.Country == null)
                    locationAddressModel.Country = "";


                return locationAddressModel;

            }
            catch (Exception)
            {
                locationAddressModel locationAddressModel = new locationAddressModel();

                if (locationAddressModel.Route == null)
                    locationAddressModel.Route = "";
                if (locationAddressModel.Distric == null)
                    locationAddressModel.Distric = "";
                if (locationAddressModel.Area == null)
                    locationAddressModel.Area = "";
                if (locationAddressModel.City == null)
                    locationAddressModel.City = "";
                if (locationAddressModel.Country == null)
                    locationAddressModel.Country = "";


                return locationAddressModel;
            }



            //   return resultO;
        }
        private Area GetArea(int id)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[Areas] where Id=" + id;

            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            Area branches = new Area();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                branches = new Area
                {
                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                    AreaCoordinate = dataSet.Tables[0].Rows[i]["AreaCoordinate"].ToString(),
                    AreaName = dataSet.Tables[0].Rows[i]["AreaName"].ToString(),
                    TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
                    BranchID = dataSet.Tables[0].Rows[i]["BranchID"].ToString(),
                    UserId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["UserId"]),
                    RestaurantsType = Convert.ToInt32(dataSet.Tables[0].Rows[i]["RestaurantsType"]),
                    IsAssginToAllUser = bool.Parse(dataSet.Tables[0].Rows[i]["IsAssginToAllUser"].ToString()),
                    IsAvailableBranch = bool.Parse(dataSet.Tables[0].Rows[i]["IsAvailableBranch"].ToString()),
                    UserIds = dataSet.Tables[0].Rows[i]["UserIds"].ToString(),
                    IsRestaurantsTypeAll = bool.Parse(dataSet.Tables[0].Rows[i]["IsRestaurantsTypeAll"].ToString()),
                };
            }

            conn.Close();
            da.Dispose();

            return branches;
        }
        private Area getNearbyArea(int tenantID, double eLatitude, double eLongitude, string city, long areaId, out double distance)
        {


            distance = -1;

            bool isInAmman = !string.IsNullOrEmpty(city) && city.Trim().ToLower().Equals("amman");
            Area areaDto = new Area();
            var areas = GetBranchesList(tenantID);
            List<Area> lstAreaDto = new List<Area>();

            if (areas != null)
            {
                foreach (var item in areas)
                {
                    if (checkIsInService(item.SettingJson))
                    {
                        lstAreaDto.Add(item);

                    }
                }
            }

            if (!isInAmman && tenantID == 42)// Macdonalds
            {
                if (lstAreaDto != null)
                    lstAreaDto = lstAreaDto.Where(x => x.Id == areaId).ToList();
            }
            //int
            if (lstAreaDto != null)
            {
                foreach (var item in lstAreaDto)
                {

                    var sCoord = new GeoCoordinate(item.Latitude.Value, item.Longitude.Value);
                    var eCoord = new GeoCoordinate(eLatitude, eLongitude);
                    var currentDistance = sCoord.GetDistanceTo(eCoord);


                    if ((distance == -1 && !isInAmman) || (isInAmman && currentDistance < 5000 && distance == -1))
                    {
                        areaDto = new Area();
                        areaDto.Id = item.Id;
                        areaDto.AreaNameEnglish = item.AreaNameEnglish;
                        areaDto.AreaName = item.AreaName;
                        areaDto.AreaCoordinate = item.AreaCoordinate;
                        areaDto.AreaCoordinateEnglish = item.AreaCoordinateEnglish;
                        areaDto.IsRestaurantsTypeAll = item.IsRestaurantsTypeAll;
                        areaDto.IsAssginToAllUser = item.IsAssginToAllUser;
                        distance = currentDistance;

                    }
                    if ((distance > currentDistance && !isInAmman) || (isInAmman && currentDistance < 5000 && distance > currentDistance))
                    {
                        areaDto = new Area();
                        areaDto.Id = item.Id;
                        areaDto.AreaNameEnglish = item.AreaNameEnglish;
                        areaDto.AreaName = item.AreaName;
                        areaDto.AreaCoordinate = item.AreaCoordinate;
                        areaDto.AreaCoordinateEnglish = item.AreaCoordinateEnglish;
                        areaDto.IsRestaurantsTypeAll = item.IsRestaurantsTypeAll;
                        areaDto.IsAssginToAllUser = item.IsAssginToAllUser;
                        distance = currentDistance;

                    }

                }
            }

            return areaDto;
        }

        private bool checkIsInService(string workingHourSetting)
        {
            bool result = true;
            if (!string.IsNullOrEmpty(workingHourSetting))
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var workModel = System.Text.Json.JsonSerializer.Deserialize<WorkModel>(workingHourSetting, options);

                DateTime currentDateTime = DateTime.UtcNow.AddHours(AppSettingsModel.AddHour);
                DayOfWeek wk = currentDateTime.DayOfWeek;
                TimeSpan timeOfDay = currentDateTime.TimeOfDay;

                switch (wk)
                {
                    case DayOfWeek.Saturday:
                        if (workModel.IsWorkActiveSat)
                        {
                            var StartDateSat = getValidValue(workModel.StartDateSat);
                            var EndDateSat = getValidValue(workModel.EndDateSat);

                            var StartDateSatSP = getValidValue(workModel.StartDateSatSP);
                            var EndDateSatSP = getValidValue(workModel.EndDateSatSP);

                            if ((timeOfDay >= StartDateSat.TimeOfDay && timeOfDay <= EndDateSat.TimeOfDay) || (timeOfDay >= StartDateSatSP.TimeOfDay && timeOfDay <= EndDateSatSP.TimeOfDay))

                            {
                                result = true;
                            }
                            else
                            {
                                result = false;
                            }
                        }

                        break;
                    case DayOfWeek.Sunday:
                        if (workModel.IsWorkActiveSun)
                        {
                            var StartDate = getValidValue(workModel.StartDateSun);
                            var EndDate = getValidValue(workModel.EndDateSun);

                            var StartDateSP = getValidValue(workModel.StartDateSunSP);
                            var EndDateSP = getValidValue(workModel.EndDateSunSP);

                            if ((timeOfDay >= StartDate.TimeOfDay && timeOfDay <= EndDate.TimeOfDay) || (timeOfDay >= StartDateSP.TimeOfDay && timeOfDay <= EndDateSP.TimeOfDay))

                            {
                                result = true;
                            }
                            else
                            {

                                result = false;
                            }
                        }
                        break;
                    case DayOfWeek.Monday:

                        if (workModel.IsWorkActiveMon)
                        {
                            var StartDate = getValidValue(workModel.StartDateMon);
                            var EndDate = getValidValue(workModel.EndDateMon);

                            var StartDateSP = getValidValue(workModel.StartDateMonSP);
                            var EndDateSP = getValidValue(workModel.EndDateMonSP);

                            if ((timeOfDay >= StartDate.TimeOfDay && timeOfDay <= EndDate.TimeOfDay) || (timeOfDay >= StartDateSP.TimeOfDay && timeOfDay <= EndDateSP.TimeOfDay))

                            {
                                result = true;
                            }
                            else
                            {

                                result = false;
                            }
                        }

                        break;
                    case DayOfWeek.Tuesday:
                        if (workModel.IsWorkActiveTues)
                        {
                            var StartDate = getValidValue(workModel.StartDateTues);
                            var EndDate = getValidValue(workModel.EndDateTues);

                            var StartDateSP = getValidValue(workModel.StartDateTuesSP);
                            var EndDateSP = getValidValue(workModel.EndDateTuesSP);

                            if ((timeOfDay >= StartDate.TimeOfDay && timeOfDay <= EndDate.TimeOfDay) || (timeOfDay >= StartDateSP.TimeOfDay && timeOfDay <= EndDateSP.TimeOfDay))

                            {
                                result = true;
                            }
                            else
                            {

                                result = false;
                            }
                        }
                        break;
                    case DayOfWeek.Wednesday:
                        if (workModel.IsWorkActiveWed)
                        {
                            var StartDate = getValidValue(workModel.StartDateWed);
                            var EndDate = getValidValue(workModel.EndDateWed);

                            var StartDateSP = getValidValue(workModel.StartDateWedSP);
                            var EndDateSP = getValidValue(workModel.EndDateWedSP);

                            if ((timeOfDay >= StartDate.TimeOfDay && timeOfDay <= EndDate.TimeOfDay) || (timeOfDay >= StartDateSP.TimeOfDay && timeOfDay <= EndDateSP.TimeOfDay))

                            {
                                result = true;
                            }
                            else
                            {

                                result = false;
                            }
                        }
                        break;
                    case DayOfWeek.Thursday:
                        if (workModel.IsWorkActiveThurs)
                        {
                            var StartDate = getValidValue(workModel.StartDateThurs);
                            var EndDate = getValidValue(workModel.EndDateThurs);

                            var StartDateSP = getValidValue(workModel.StartDateThursSP);
                            var EndDateSP = getValidValue(workModel.EndDateThursSP);

                            if ((timeOfDay >= StartDate.TimeOfDay && timeOfDay <= EndDate.TimeOfDay) || (timeOfDay >= StartDateSP.TimeOfDay && timeOfDay <= EndDateSP.TimeOfDay))

                            {
                                result = true;
                            }
                            else
                            {

                                result = false;
                            }
                        }
                        break;
                    case DayOfWeek.Friday:
                        if (workModel.IsWorkActiveFri)
                        {
                            var StartDate = getValidValue(workModel.StartDateFri);
                            var EndDate = getValidValue(workModel.EndDateFri);

                            var StartDateSP = getValidValue(workModel.StartDateFriSP);
                            var EndDateSP = getValidValue(workModel.EndDateFriSP);

                            if ((timeOfDay >= StartDate.TimeOfDay && timeOfDay <= EndDate.TimeOfDay) || (timeOfDay >= StartDateSP.TimeOfDay && timeOfDay <= EndDateSP.TimeOfDay))

                            {
                                result = true;
                            }
                            else
                            {

                                result = false;
                            }
                        }
                        break;
                    default:

                        break;

                }


            }
            return result;

        }

        private DateTime getValidValue(dynamic value)
        {
            DateTime result = DateTime.MinValue;
            try
            {
                result = DateTime.Parse(value.ToString());
                return result;
            }
            catch (Exception)
            {
                return result;
                throw;
            }

        }

        private DeliveryCost getDeliveryCostByAreaId(int tenantId, long areaId)
        {
            try
            {
                DeliveryCost deliveryCostDto = new DeliveryCost();
                var SP_Name = "[dbo].[DeliveryCostByAreaIDGet]";

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                new System.Data.SqlClient.SqlParameter("@TenantId",tenantId)
               , new System.Data.SqlClient.SqlParameter("@AreaId",areaId)
            };

                deliveryCostDto = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),
                ConvertToDeliveryCost, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                return deliveryCostDto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<LocationInfoModel> GetAllLocationInfoModel()
        {

            try
            {
                string connString = AppSettingsModel.ConnectionStrings;
                string query = "select * from [dbo].[Locations] ";


                SqlConnection conn = new SqlConnection(connString);
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                // create the DataSet 
                DataSet dataSet = new DataSet();

                // create data adapter
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                // this will query your database and return the result to your datatable
                da.Fill(dataSet);

                List<LocationInfoModel> locationInfoModel = new List<LocationInfoModel>();

                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {
                    try
                    {
                        locationInfoModel.Add(new LocationInfoModel
                        {
                            Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                            LocationName = dataSet.Tables[0].Rows[i]["LocationName"].ToString(),
                            LocationNameEn = dataSet.Tables[0].Rows[i]["LocationNameEn"].ToString().TrimEnd(),
                            LevelId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["LevelId"].ToString()),
                            ParentId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ParentId"].ToString()),
                        });
                    }
                    catch
                    {
                        locationInfoModel.Add(new LocationInfoModel
                        {
                            Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                            LocationName = dataSet.Tables[0].Rows[i]["LocationName"].ToString(),
                            LocationNameEn = dataSet.Tables[0].Rows[i]["LocationNameEn"].ToString().TrimEnd(),
                            LevelId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["LevelId"].ToString()),
                        });

                    }


                }

                conn.Close();
                da.Dispose();

                return locationInfoModel;

            }
            catch
            {
                return null;

            }

        }

        private void OrderOfferFun(int tenantID, bool isOrderOffer, decimal OrderTotal, LocationInfoModel infoLocation, string ci, string ar, string dis, decimal costDistric)
        {
            if (isOrderOffer)
            {

                var orderEffor = GetOrderOffer(tenantID);

                if (infoLocation.LocationAreaName == null)
                    infoLocation.LocationAreaName = "";

                var item = orderEffor.Where(x => (x.BranchesIds.Contains(infoLocation.LocationId.ToString())&& x.isPersentageDiscount == false));//.FirstOrDefault(); x.Area.Contains(ci) || x.Area.Contains(ar) || x.Area.Contains(dis) &&


                foreach (var areaEffor in item)
                {


                    if (areaEffor != null)
                    {

                        if (!areaEffor.isPersentageDiscount)
                        {

                            if (OrderTotal >= areaEffor.FeesStart && OrderTotal <= areaEffor.FeesEnd)
                            {
                                var DateNow = Convert.ToDateTime(DateTime.UtcNow.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                                var DateStart = Convert.ToDateTime(areaEffor.OrderOfferDateStart.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                                var DateEnd = Convert.ToDateTime(areaEffor.OrderOfferDateEnd.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));

                                if (DateStart <= DateNow && DateEnd >= DateNow)
                                {
                                    var timeNow = Convert.ToDateTime(DateTime.UtcNow.AddHours(AppSettingsModel.AddHour).ToString("HH:mm:ss", CultureInfo.InvariantCulture));
                                    var timeStart = Convert.ToDateTime(areaEffor.OrderOfferStart.AddHours(AppSettingsModel.AddHour).ToString("HH:mm:ss ", CultureInfo.InvariantCulture));
                                    var timeEnd = Convert.ToDateTime(areaEffor.OrderOfferEnd.AddHours(AppSettingsModel.AddHour).ToString("HH:mm:ss ", CultureInfo.InvariantCulture));




                                    if ((timeStart <= timeNow && timeNow <= timeEnd))
                                    {


                                        infoLocation.DeliveryCostAfter = costDistric;
                                        infoLocation.DeliveryCostBefor = areaEffor.NewFees;
                                        infoLocation.isOrderOfferCost = true;


                                        break;

                                    }
                                    else
                                    {
                                        infoLocation.DeliveryCostAfter = costDistric;
                                        infoLocation.DeliveryCostBefor = 0;
                                        infoLocation.isOrderOfferCost = false;

                                    }


                                }
                                else
                                {
                                    infoLocation.DeliveryCostAfter = costDistric;
                                    infoLocation.DeliveryCostBefor = 0;
                                    infoLocation.isOrderOfferCost = false;

                                }


                            }
                            else
                            {
                                infoLocation.DeliveryCostAfter = costDistric;
                                infoLocation.DeliveryCostBefor = 0;
                                infoLocation.isOrderOfferCost = false;

                            }



                        }
                        else
                        {
                            infoLocation.DeliveryCostAfter = costDistric;
                            infoLocation.DeliveryCostBefor = 0;
                            infoLocation.isOrderOfferCost = false;

                        }



                    }
                    else
                    {
                        infoLocation.DeliveryCostAfter = costDistric;
                        infoLocation.DeliveryCostBefor = 0;
                        infoLocation.isOrderOfferCost = false;

                    }


                }


            }
            else
            {
                infoLocation.DeliveryCostAfter = costDistric;
                infoLocation.DeliveryCostBefor = 0;
                infoLocation.isOrderOfferCost = false;

            }

        }

        private List<OrderOffers> GetOrderOffer(int TenantID)
        {

            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[OrderOffer] where TenantID=" + TenantID;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<OrderOffers> order = new List<OrderOffers>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                var IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString());
                if (!IsDeleted)
                {


                    try
                    {
                        order.Add(new OrderOffers
                        {
                            Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                            Area = dataSet.Tables[0].Rows[i]["Area"].ToString(),
                            Cities = dataSet.Tables[0].Rows[i]["Cities"].ToString(),
                            FeesStart = decimal.Parse(dataSet.Tables[0].Rows[i]["FeesStart"].ToString()),
                            FeesEnd = decimal.Parse(dataSet.Tables[0].Rows[i]["FeesEnd"].ToString()),
                            NewFees = decimal.Parse(dataSet.Tables[0].Rows[i]["NewFees"].ToString()),
                            TenantId = int.Parse(dataSet.Tables[0].Rows[i]["TenantId"].ToString()),
                            isAvailable = bool.Parse(dataSet.Tables[0].Rows[i]["isAvailable"].ToString()),
                            OrderOfferDateEnd = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["OrderOfferDateEnd"].ToString()),
                            OrderOfferDateStart = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["OrderOfferDateStart"].ToString()),
                            OrderOfferEnd = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["OrderOfferEnd"].ToString()),//.AddHours(AppSettingsModel.AddHour),
                            OrderOfferStart = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["OrderOfferStart"].ToString()),//.AddHours(AppSettingsModel.AddHour),
                            isPersentageDiscount = bool.Parse(dataSet.Tables[0].Rows[i]["isPersentageDiscount"].ToString()),
                            isBranchDiscount = bool.Parse(dataSet.Tables[0].Rows[i]["isBranchDiscount"].ToString()),

                            BranchesName= dataSet.Tables[0].Rows[i]["BranchesName"].ToString(),
                            BranchesIds= dataSet.Tables[0].Rows[i]["BranchesIds"].ToString(),


                        });
                    }
                    catch
                    {
                        order.Add(new OrderOffers
                        {
                            Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                            Area = dataSet.Tables[0].Rows[i]["Area"].ToString(),
                            Cities = dataSet.Tables[0].Rows[i]["Cities"].ToString(),
                            FeesStart = decimal.Parse(dataSet.Tables[0].Rows[i]["FeesStart"].ToString()),
                            FeesEnd = decimal.Parse(dataSet.Tables[0].Rows[i]["FeesEnd"].ToString()),
                            NewFees = decimal.Parse(dataSet.Tables[0].Rows[i]["NewFees"].ToString()),
                            TenantId = int.Parse(dataSet.Tables[0].Rows[i]["TenantId"].ToString()),
                            isAvailable = bool.Parse(dataSet.Tables[0].Rows[i]["isAvailable"].ToString()),
                            OrderOfferDateEnd = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["OrderOfferDateEnd"].ToString()),
                            OrderOfferDateStart = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["OrderOfferDateStart"].ToString()),
                            OrderOfferEnd = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["OrderOfferEnd"].ToString()),//.AddHours(AppSettingsModel.AddHour),
                            OrderOfferStart = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["OrderOfferStart"].ToString()),//.AddHours(AppSettingsModel.AddHour),
                            isPersentageDiscount = bool.Parse(dataSet.Tables[0].Rows[i]["isPersentageDiscount"].ToString()),
                            isBranchDiscount = bool.Parse(dataSet.Tables[0].Rows[i]["isBranchDiscount"].ToString()),




                        });

                    }


                }

            }

            conn.Close();
            da.Dispose();

            return order;

        }
        private async Task<TenantModel> GetTenantById(int? id)
        {

            var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);

            TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.TenantId == id);
            return tenant;
        }

        private string extractURL(string myString)
        {
            // string myString = "test =) https://google.com/";
            Match url = Regex.Match(myString, @"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?");
            return url.ToString();
        }
        private string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var randomString = new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
            return randomString;
        }


        private MenuContcatKeyModel addMenuContactKey(MenuContcatKeyModel model)
        {
            try
            {
                var SP_Name = "[dbo].[MenuContactKeyAdd]";

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>(){
                    new System.Data.SqlClient.SqlParameter("@KeyMenu", model.KeyMenu),
                   new System.Data.SqlClient.SqlParameter("@Value", model.Value),
                   new System.Data.SqlClient.SqlParameter("@ContactID", model.ContactID)
                    };

                MenuContcatKeyModel mo = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), MenuContcatKey, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                return mo;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private string GetOrderDetailString(int TenantID, string lang, decimal? Total, decimal? TotalLoyaltyPoints, string captionQuantityText, string captionAddtionText, string captionTotalText, string captionTotalOfAllText, List<OrderDetail> OrderDetailList, List<ExtraOrderDetails> getOrderDetailExtraList, decimal discount, bool isdiscount, string TypeChoes = "", string DeliveryCostText = "", decimal? Cost = 0)
        {

            //var captionQuantityText = GetCaptionFormat("BackEnd_Text_Quantity", lang, TenantID, "", "", 0);//العدد :
            //var captionAddtionText = GetCaptionFormat("BackEnd_Text_Addtion", lang, TenantID, "", "", 0);//الاضافات
            //var captionTotalText = GetCaptionFormat("BackEnd_Text_Total", lang, TenantID, "", "", 0);//المجموع:       
            //var captionTotalOfAllText = GetCaptionFormat("BackEnd_Text_TotalOfAll", lang, TenantID, "", "", 0);//السعر الكلي للصنف: 

            var captionTotalPointsOfAllText = "السعر الكلي لصنف بالنقاط :";
            var captionTotalPointsText = "المجموع بالنقاط :";
            if (lang == "ar")
            {
                captionTotalPointsOfAllText = "السعر الكلي لصنف بالنقاط :";
                captionTotalPointsText = "المجموع بالنقاط :";
            }
            else
            {
                captionTotalPointsOfAllText = "Price per item in points:";
                captionTotalPointsText = "Total in points:";
            }




            var listString = "-------------------------- \r\n";
            listString = listString + "-------------------------- \r\n\r\n";
            decimal? total = 0;

            foreach (var OrderD in OrderDetailList)
            {

                var getOrderDetailExtra = getOrderDetailExtraList.Where(x => x.OrderDetailId == OrderD.Id).ToList();
                var item = GetItem(TenantID, long.Parse(OrderD.ItemId.ToString())).FirstOrDefault();// itemList.Where(x => x.Id == OrderD.ItemId).FirstOrDefault();

                if (item != null)
                {
                    if (lang == "ar")
                    {
                        listString = listString + "*" + item.ItemName.Trim() + "*" + "\r\n";
                    }
                    else
                    {
                        listString = listString + "*" + item.ItemNameEnglish.Trim() + "*" + "\r\n";
                    }

                    if (getOrderDetailExtra.Count > 0)
                    {
                        listString = listString + "*" + captionAddtionText.Trim() + "*" + "\r\n";

                    }
                }
                else
                {
                    if (lang == "ar")
                    {
                        if (OrderD.IsCrispy)
                        {
                            listString = listString + "*" + "كرسبي" + "*" + "\r\n";

                        }
                        else if (OrderD.IsDeserts)
                        {
                            listString = listString + "*" + "حلويات" + "*" + "\r\n";

                        }
                        else if (OrderD.IsCondiments)
                        {
                            listString = listString + "*" + "الصوص" + "*" + "\r\n";

                        }
                    }
                    else
                    {
                        if (OrderD.IsCrispy)
                        {
                            listString = listString + "*" + "Crispy" + "*" + "\r\n";

                        }
                        else if (OrderD.IsDeserts)
                        {
                            listString = listString + "*" + "Desserts" + "*" + "\r\n";

                        }
                        else if (OrderD.IsCondiments)
                        {
                            listString = listString + "*" + "Condiments" + "*" + "\r\n";
                        }

                    }

                    //if (getOrderDetailExtra.Count > 0)
                    //{
                    //    listString = listString + "*" + captionAddtionText.Trim() + "*" + "\r\n";

                    //}

                }


                foreach (var ext in getOrderDetailExtra)
                {
                    if (ext.Quantity > 1)
                    {
                        if (lang == "ar")
                        {
                            listString = listString + ext.Name + "  (" + ext.Quantity + ")" + "\r\n";
                        }
                        else
                        {
                            listString = listString + ext.NameEnglish + "  (" + ext.Quantity + ")" + "\r\n";

                        }

                    }
                    else
                    {
                        if (lang == "ar")
                        {

                            listString = listString + ext.Name + "\r\n";
                        }
                        else
                        {
                            listString = listString + ext.NameEnglish + "\r\n";
                        }


                    }

                    //listString = listString + captionQuantityText + ext.Quantity + "\r\n";

                }




                listString = listString + "\r\n" + "*" + captionQuantityText + OrderD.Quantity + "*" + "\r\n";
                if (TenantID == 46)
                {
                    if (OrderD.TotalLoyaltyPoints!=0)
                    {
                        listString = listString + captionTotalPointsOfAllText + ((int)OrderD.TotalLoyaltyPoints).ToString() + "\r\n\r\n";
                    }

                    listString = listString + captionTotalOfAllText + ((int)OrderD.Total).ToString() + "\r\n\r\n";
                }
                else
                {

                    if (OrderD.TotalLoyaltyPoints!=0)
                    {
                        listString = listString + captionTotalPointsOfAllText + OrderD.TotalLoyaltyPoints + "\r\n\r\n";
                    }
                    listString = listString + captionTotalOfAllText + OrderD.Total + "\r\n\r\n";
                }



                total = total + OrderD.Total;// + Cost;

                listString = listString + "-------------------------- \r\n";
            }


            listString = listString + "-------------------------- \r\n\r\n";

            if (TypeChoes == "Delivery")
            {
                listString = listString + DeliveryCostText + "\r\n";

                listString = listString + "-------------------------- \r\n\r\n";
            }

            if (TypeChoes == "Delivery")
            {
                if (TenantID == 46)
                {
                    if (TotalLoyaltyPoints!=0)
                    {
                        listString = listString + captionTotalPointsText + ((int)TotalLoyaltyPoints).ToString() + "\r\n";
                    }
                    listString = listString + captionTotalText + ((int)Total + Cost).ToString() + "\r\n";
                }
                else
                {
                    if (TotalLoyaltyPoints!=0)
                    {
                        listString = listString + captionTotalPointsText + ((int)TotalLoyaltyPoints).ToString() + "\r\n";
                    }
                    listString = listString + captionTotalText + (Total + Cost).ToString() + "\r\n";
                }
            }
            else
            {
                if (TenantID == 46)
                {
                    if (TotalLoyaltyPoints!=0)
                    {
                        listString = listString + captionTotalPointsText + ((int)TotalLoyaltyPoints).ToString() + "\r\n";
                    }
                    listString = listString + captionTotalText + ((int)Total).ToString() + "\r\n";
                }
                else
                {
                    if (TotalLoyaltyPoints!=0)
                    {
                        listString = listString + captionTotalPointsText + TotalLoyaltyPoints.ToString() + "\r\n";
                    }
                    listString = listString + captionTotalText + Total.ToString() + "\r\n";
                }

            }



            listString = listString + "-------------------------- \r\n";
            if (isdiscount)
            {
                if (lang == "ar")
                {

                    listString = listString + "بعد خصم :" + discount + " %" + "\r\n\r\n";
                }
                else
                {
                    listString = listString + "After Discount :" + discount + " %" + "\r\n\r\n";
                }


            }

            return listString;
        }


        private List<Item> GetItem(int? TenantID, long id)
        {

            List<Item> itemDtos = new List<Item>();
            Item item = getItemInfoForBot(id, TenantID.Value);
            itemDtos.Add(item);
            return itemDtos;
        }
        private Item getItemInfoForBot(long itemId, int TenantId)
        {

            try
            {
                Item ItemDto = new Item();
                var SP_Name = "[dbo].[ItemInfoForBotGet]";
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
             new System.Data.SqlClient.SqlParameter("@TenantId",TenantId)
            ,new System.Data.SqlClient.SqlParameter("@ItemId",itemId)


            };
                ItemDto = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),
                MapItemsMenu, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                return ItemDto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private BotService.Models.API.Order getOrderExtraDetails(int tenantID, long contactId)
        {
            BotService.Models.API.Order orderDto = new BotService.Models.API.Order();

            try
            {

                var SP_Name = "[dbo].[OrderExtraDetailsGet]";
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
             new System.Data.SqlClient.SqlParameter("@TenantID",tenantID)
            ,new System.Data.SqlClient.SqlParameter("@ContactId",contactId)

            };
                orderDto = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),
                MapOrderExtraDetails, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                return orderDto;
            }
            catch (Exception ex)
            {
                throw ex;
            }





        }

        private List<OrderDetail> GetOrderDetail(int? TenantID, long OrderId)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[OrderDetails] where TenantID=" + TenantID + " and OrderId=" + OrderId;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<OrderDetail> orderDetailDtos = new List<OrderDetail>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                orderDetailDtos.Add(new OrderDetail
                {
                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                    Discount = decimal.Parse(dataSet.Tables[0].Rows[i]["Discount"].ToString()),
                    ItemId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemId"]),
                    OrderId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["OrderId"]),
                    Quantity = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Quantity"]),
                    Total = decimal.Parse(dataSet.Tables[0].Rows[i]["Total"].ToString()),
                    UnitPrice = decimal.Parse(dataSet.Tables[0].Rows[i]["UnitPrice"].ToString()),



                });
            }

            conn.Close();
            da.Dispose();

            return orderDetailDtos;
        }
        private List<ExtraOrderDetails> GetOrderDetailExtra(int? TenantID)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[ExtraOrderDetail] where TenantID=" + TenantID;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<ExtraOrderDetails> orderDetailDtos = new List<ExtraOrderDetails>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                orderDetailDtos.Add(new ExtraOrderDetails
                {
                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                    OrderDetailId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["OrderDetailId"]),
                    TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
                    Quantity = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Quantity"]),
                    Name = dataSet.Tables[0].Rows[i]["Name"].ToString(),
                    NameEnglish = dataSet.Tables[0].Rows[i]["NameEnglish"].ToString(),
                    Total = decimal.Parse(dataSet.Tables[0].Rows[i]["Total"].ToString()),
                    UnitPrice = decimal.Parse(dataSet.Tables[0].Rows[i]["UnitPrice"].ToString()),



                });
            }

            conn.Close();
            da.Dispose();

            return orderDetailDtos;
        }

        private void DeleteExtraOrderDetail(long? id)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            using (SqlConnection connection = new SqlConnection(connString))
            using (SqlCommand command = connection.CreateCommand())
            {

                command.CommandText = "DELETE FROM ExtraOrderDetail   Where Id = @Id";
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }

        }
        private void DeleteOrder(long? id)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            using (SqlConnection connection = new SqlConnection(connString))
            using (SqlCommand command = connection.CreateCommand())
            {

                command.CommandText = "DELETE FROM Orders   Where Id = @Id";
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }

        }
        private void DeleteOrderDetails(long? id)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            using (SqlConnection connection = new SqlConnection(connString))
            using (SqlCommand command = connection.CreateCommand())
            {

                command.CommandText = "DELETE FROM OrderDetails   Where Id = @Id";
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }

        }

        private List<BotService.Models.API.Order> GetOrderListWithContact(int? TenantID, int ContactId, string OrderNumber)
        {

            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[Orders] where TenantID=" + TenantID + "and ContactId=" + ContactId +" and OrderNumber =" +OrderNumber +" and OrderStatus <> 4";


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<BotService.Models.API.Order> order = new List<BotService.Models.API.Order>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                var IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString());
                if (!IsDeleted)
                {

                    try
                    {

                        order.Add(new BotService.Models.API.Order
                        {
                            Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                            OrderNumber = Convert.ToInt32(dataSet.Tables[0].Rows[i]["OrderNumber"]),
                            Total = decimal.Parse(dataSet.Tables[0].Rows[i]["Total"].ToString()),
                            ContactId = int.Parse(dataSet.Tables[0].Rows[i]["ContactId"].ToString()),
                            CreationTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["CreationTime"].ToString()),
                            OrderTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["OrderTime"].ToString()),
                            AgentId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["AgentId"]),
                            AgentIds = dataSet.Tables[0].Rows[i]["AgentIds"].ToString(),
                            AreaId = int.Parse(dataSet.Tables[0].Rows[i]["ContactId"].ToString()),
                            TotalPoints = decimal.Parse(dataSet.Tables[0].Rows[i]["TotalPoints"].ToString()),
                            ///AfterDeliveryCost= decimal.Parse(dataSet.Tables[0].Rows[i]["AfterDeliveryCost"].ToString()),
                            DeliveryCost= decimal.Parse(dataSet.Tables[0].Rows[i]["DeliveryCost"].ToString()),
                            OrderType=(OrderTypeEunm)Convert.ToInt32(dataSet.Tables[0].Rows[i]["OrderType"]),

                        });

                    }
                    catch
                    {

                        order.Add(new BotService.Models.API.Order
                        {
                            Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                            OrderNumber = Convert.ToInt32(dataSet.Tables[0].Rows[i]["OrderNumber"]),
                            Total = decimal.Parse(dataSet.Tables[0].Rows[i]["Total"].ToString()),
                            ContactId = int.Parse(dataSet.Tables[0].Rows[i]["ContactId"].ToString()),
                            CreationTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["CreationTime"].ToString()),
                            OrderTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["OrderTime"].ToString()),
                            AgentIds = dataSet.Tables[0].Rows[i]["AgentIds"].ToString(),
                            AreaId = int.Parse(dataSet.Tables[0].Rows[i]["ContactId"].ToString()),
                            TotalPoints = decimal.Parse(dataSet.Tables[0].Rows[i]["TotalPoints"].ToString()),
                            //AfterDeliveryCost= decimal.Parse(dataSet.Tables[0].Rows[i]["AfterDeliveryCost"].ToString()),
                            OrderType=(OrderTypeEunm)Convert.ToInt32(dataSet.Tables[0].Rows[i]["OrderType"]),
                            DeliveryCost= 0,
                        });

                    }


                }

            }

            conn.Close();
            da.Dispose();

            return order;

        }

        private async Task UpdateOrderAfterCancel(string OrderNumber, int ContactId, BotService.Models.API.Order order, int? TenantID)
        {
            //var con = GetContact(ContactId);
            var con = getContactbyId(ContactId);

            string connString = AppSettingsModel.ConnectionStrings;
            using (SqlConnection connection = new SqlConnection(connString))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = "UPDATE Orders SET  OrderStatus = @OrI, OrderRemarks=@Rema  Where Id = @Id";
                command.Parameters.AddWithValue("@Id", order.Id);
                command.Parameters.AddWithValue("@OrI", OrderStatusEunm.Canceled);
                command.Parameters.AddWithValue("@Rema", "CancelByCustomer");
                // command.Parameters.AddWithValue("@ActionTime", DateTime.Now.AddHours(AppSettingsModel.AddHour));

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }

            order.OrderNumber = long.Parse(OrderNumber);
            order.ContactId = ContactId;
            order.orderStatus = OrderStatusEunm.Canceled;
            order.ActionTime =  DateTime.Now.AddHours(AppSettingsModel.AddHour);

            var orderStatusName = Enum.GetName(typeof(OrderStatusEunm), order.orderStatus);
            var orderTypeName = Enum.GetName(typeof(OrderTypeEunm), order.OrderType);
            GetOrderForViewDto getOrderForViewDto = new GetOrderForViewDto();

            //   var GetOrderMap = _mapper.Map(OrderModel, getOrderForViewDto.Order);

            getOrderForViewDto.Order=new OrderDto()
            {
                OrderTime =order.OrderTime,
                OrderRemarks =order.OrderRemarks,
                OrderNumber =order.OrderNumber,
                CreationTime =order.CreationTime,
                DeletionTime =order.DeletionTime,
                LastModificationTime =order.LastModificationTime,
                DeliveryChangeId =order.DeliveryChangeId,
                BranchId =order.BranchId,
                ContactId =order.ContactId,
                OrderStatus = order.orderStatus,
                OrderType =order.OrderType,
                IsLockByAgent =order.IsLockByAgent,
                AgentId  =order.AgentId,
                LockByAgentName  =order.LockByAgentName,
                Total  =order.Total,
                TotalPoints  =order.TotalPoints,
                StringTotal  =order.StringTotal,
                Address  =order.Address,
                AreaId  =order.AreaId,
                DeliveryCost  =order.DeliveryCost,
                AfterDeliveryCost  =order.AfterDeliveryCost,
                BranchAreaId  =order.BranchAreaId,
                BranchAreaName  =order.BranchAreaName,
                FromLocationDescribation  =order.FromLocationDescribation,
                ToLocationDescribation  =order.ToLocationDescribation,
                OrderDescribation  =order.OrderDescribation,
                IsSpecialRequest  =order.IsSpecialRequest,
                SpecialRequestText  =order.SpecialRequestText,
                SelectDay  =order.SelectDay,
                SelectTime  =order.SelectTime,
                IsPreOrder  =order.IsPreOrder,
                RestaurantName  =order.RestaurantName,
                HtmlPrint  =order.HtmlPrint,
                BuyType  =order.BuyType,
                ActionTime  =order.ActionTime,
                AgentIds  =order.AgentIds,
                OrderDetailDtoJson  =order.OrderDetailDtoJson,
                TenantId  =order.TenantId,
                CaptionJson  =order.CaptionJson,
                OrderOfferJson  =order.OrderOfferJson,
                IsItemOffer  =order.IsItemOffer,
                ItemOffer  =order.ItemOffer,
                IsDeliveryOffer  =order.IsDeliveryOffer,
                DeliveryOffer  =order.DeliveryOffer,
                OrderLocal  =order.OrderLocal
            };

            getOrderForViewDto.Order = getOrderForViewDto.Order;
            getOrderForViewDto.OrderStatusName = orderStatusName;
            getOrderForViewDto.AgentIds = order.AgentIds;
            getOrderForViewDto.OrderTypeName = orderTypeName;

            getOrderForViewDto.CustomerCustomerName = con.DisplayName;
            getOrderForViewDto.TenantId = TenantID;
            getOrderForViewDto.DeliveryChangeDeliveryServiceProvider = order.CreationTime.ToString("MM/dd hh:mm tt");


            getOrderForViewDto.IsAssginToAllUser = true;


            try
            {
                var area = GetArea(int.Parse(order.AreaId.ToString()));

                if (area != null)
                {
                    getOrderForViewDto.AreahName = area.AreaName;
                }

            }
            catch (Exception)
            {

                getOrderForViewDto.AreahName = "";
            }


            SocketIOManager.SendOrder(getOrderForViewDto, TenantID.HasValue ? TenantID.Value : 0);
            var titl = "The Order Number: " + OrderNumber;
            var body = "Order Status :" + OrderStatusEunm.Canceled;

            SendMobileCancelNotification(TenantID, titl, body);
        }

        private Contact getContactbyId(int id)
        {
            try
            {
                Contact contactDto = new Contact();
                var SP_Name = "[dbo].[ContactbyIdGet]";

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
               new System.Data.SqlClient.SqlParameter("@Id",id)
                 };

                contactDto = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),
                 MapContact, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                return contactDto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SendMobileCancelNotification(int? TenaentId, string title, string msg)
        {
            var result = "-1";
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(FcmNotificationSetting.webAddr);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Headers.Add(string.Format("Authorization: key={0}", FcmNotificationSetting.ServerKey));
            httpWebRequest.Headers.Add(string.Format("Sender: id={0}", FcmNotificationSetting.SenderId));
            httpWebRequest.Method = "POST";

            var payload = new
            {
                registration_ids = GetUserByTeneantId((int)TenaentId),
                data = new
                {

                    body = msg,
                    title = title,
                    sound = "cancel.caf",
                    apple = new
                    {
                        sound = "cancel.caf"
                    }
                },
                priority = "high",
                payload = new
                {
                    aps = new
                    {
                        sound = "cancel.caf"
                    }
                },
                android = new
                {
                    notification = new
                    {
                        channel_id = "high_importance_channel_cancel"
                    }
                },
                apns = new
                {

                    header = new
                    {
                        priority = "10"
                    },


                    payload = new
                    {
                        aps = new
                        {
                            sound = "cancel.caf"
                        }
                    }
                }
    ,
                notification = new
                {
                    body = msg,
                    content_available = true,
                    priority = "high",
                    title = title,
                    sound = "cancel.caf",
                    apns = new
                    {
                        payload = new
                        {
                            aps = new
                            {
                                sound = "cancel.caf"
                            }
                        }
                    }
                },




            };

            var serializer = new JavaScriptSerializer();
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = serializer.Serialize(payload);
                streamWriter.Write(json);
                streamWriter.Flush();
            }


            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }

        }

        private List<string> GetUserByTeneantId(int TenaentId, string userIds = null)
        {

            List<string> lstUserToken = new List<string>();
            var list = GetFuncation(TenaentId, userIds);
            if (list != null)
            {
                foreach (var item in list)
                {
                    lstUserToken.Add(item.Token);
                }
            }

            return lstUserToken;
        }

        private static IList<UserTokenModel> GetFuncation(int? tenantId, string userIds = null)
        {

            string spName = "[dbo].[UserTokenGet]";
            var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                new System.Data.SqlClient.SqlParameter("@TenantId",tenantId),
                new System.Data.SqlClient.SqlParameter("@UserIds",userIds)
            };


            IList<UserTokenModel> result = SqlDataHelper.ExecuteReader(spName, sqlParameters.ToArray(), MapUserToken, AppSettingsModel.ConnectionStrings);
            return result;
        }
        private bool createOrderStatusHistory(long orderId, int orderStatusId, int? TenantId = null)
        {




            var CreatedBy = 76;
           
            try
            {
                var SP_Name = "[dbo].[OrderStatusHistoryAdd]";
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                     new System.Data.SqlClient.SqlParameter("@TenantId",TenantId)
                    ,new System.Data.SqlClient.SqlParameter("@OrderId",orderId)
                    ,new System.Data.SqlClient.SqlParameter("@OrderStatusId",orderStatusId)
                    ,new System.Data.SqlClient.SqlParameter("@CreatedDate",DateTime.UtcNow)
                    ,new System.Data.SqlClient.SqlParameter("@CreatedBy",CreatedBy)

                };

                var OutputParameter = new System.Data.SqlClient.SqlParameter();
                OutputParameter.SqlDbType = System.Data.SqlDbType.Bit;
                OutputParameter.ParameterName = "@Result";
                OutputParameter.Direction = System.Data.ParameterDirection.Output;


                sqlParameters.Add(OutputParameter);


                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

                return (bool)OutputParameter.Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private long UpateOrder(int? tenantId)
        {
            string connString = AppSettingsModel.ConnectionStrings;

            //AbpSession.TenantId
            var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                            new System.Data.SqlClient.SqlParameter("@TenantId",tenantId)
                     };

            System.Data.SqlClient.SqlParameter output = new System.Data.SqlClient.SqlParameter();

            output.ParameterName = "@CurrentOrderNumber";
            output.DbType = DbType.Int64;
            output.Direction = ParameterDirection.Output;
            sqlParameters.Add(output);



            var result = SqlDataHelper.ExecuteNoneQuery(
                       "dbo.GetCurrentOrderNumber",
                       sqlParameters.ToArray(),
                       connString);

            return Int64.Parse(output.Value.ToString());
        }

        private long UpdateOrder(string order, long orderid, int tenantID)
        {
            try
            {

                var SP_Name = "[dbo].[OrderUpdate]";
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                 new System.Data.SqlClient.SqlParameter("@orderJson",order),
                 new System.Data.SqlClient.SqlParameter("@orderID",orderid),
                 new System.Data.SqlClient.SqlParameter("@tenantID",tenantID)

            };

                System.Data.SqlClient.SqlParameter OutsqlParameter = new System.Data.SqlClient.SqlParameter();
                OutsqlParameter.ParameterName = "@OrderNumber";
                OutsqlParameter.SqlDbType = System.Data.SqlDbType.BigInt;
                OutsqlParameter.Direction = System.Data.ParameterDirection.Output;

                sqlParameters.Add(OutsqlParameter);

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

                return (long)OutsqlParameter.Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private void SendMobileNotification(int? TenaentId, string title, string msg, bool islivechat = false, string userIds = null)
        {

            string namesoind = "sound.caf";
            string namesoind2 = "cancel.caf";

            if (islivechat)
            {
                namesoind= "livechat.caf";
                namesoind2= "livechat.caf";
            }
            var result = "-1";
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(FcmNotificationSetting.webAddr);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Headers.Add(string.Format("Authorization: key={0}", FcmNotificationSetting.ServerKey));
            httpWebRequest.Headers.Add(string.Format("Sender: id={0}", FcmNotificationSetting.SenderId));
            httpWebRequest.Method = "POST";

            var payload = new
            {
                registration_ids = GetUserByTeneantId((int)TenaentId, userIds),
                data = new
                {

                    body = msg,
                    title = title,
                    sound = namesoind,
                    apple = new
                    {
                        sound = namesoind2
                    }
                },
                priority = "high",
                payload = new
                {
                    aps = new
                    {
                        sound = namesoind
                    }
                },
                android = new
                {
                    notification = new
                    {
                        channel_id = "high_importance_channel_cancel"
                    }
                },
                apns = new
                {
                    header = new
                    {
                        priority = "10"
                    },


                    payload = new
                    {
                        aps = new
                        {
                            sound = namesoind
                        }
                    }
                }
                ,
                notification = new
                {
                    body = msg,
                    content_available = true,
                    priority = "high",
                    title = title,
                    sound = namesoind,
                    apns = new
                    {
                        payload = new
                        {
                            aps = new
                            {
                                sound = namesoind
                            }
                        }
                    }
                },




            };

            var serializer = new JavaScriptSerializer();
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = serializer.Serialize(payload);
                streamWriter.Write(json);
                streamWriter.Flush();
            }


            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }

        }
        public async Task<CustomerModel> UpdateCustomerLocation(Contact objContactDto)
        {
            // Contact user = new Contact();
            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == objContactDto.UserId);

            string result = string.Empty;
            if (customerResult.IsCompletedSuccessfully)
            {
                var customer = customerResult.Result;
                if (customer != null)
                {
                    //// update contact to database 
                    // UpdateContactInfo(objContactDto);
                    updateContactInfo(objContactDto);

                    customer.Description = objContactDto.Description;
                    await itemsCollection.UpdateItemAsync(customer._self, customer);

                    return customer;
                }
            }
            else
            {
                return null;
            }

            return null;


        }
        private void updateContactInfo(Contact contactDto)
        {
            try
            {
                //var contact = getContactbyId()
                var SP_Name = "[dbo].[ContactInfoUpdate]";
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                    new System.Data.SqlClient.SqlParameter("@ContactInfoJson",JsonConvert.SerializeObject(contactDto))

                };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Delivery cost Per KiloMeter
        private LocationInfoModel getLocationInfoPerKiloMeter(int tenantID, string query)
        {
            try
            {
                LocationInfoModel getLocationInfoModel = new LocationInfoModel();

                if (IsvalidLatLong(query.Split(",")[0], query.Split(",")[1]))
                {
                    double latitude = double.Parse(query.Split(",")[0]);
                    double longitude = double.Parse(query.Split(",")[1]);
                    var rez = GetLocation(query);


                    getLocationInfoModel.Country = rez.Country.Replace("'", "").Trim();
                    getLocationInfoModel.City = rez.City.Replace("'", "").Trim();
                    getLocationInfoModel.Area = rez.Area.Replace("'", "").Trim();
                    getLocationInfoModel.Distric = rez.Distric.Replace("'", "").Trim();
                    string Route = rez.Route.Replace("'", "").Trim();

                    getLocationInfoModel.DeliveryCostAfter = -1;
                    getLocationInfoModel.DeliveryCostBefor = -1;
                    getLocationInfoModel.LocationId = 0;
                    getLocationInfoModel.Address = Route + " - " + getLocationInfoModel.Distric + " - " + getLocationInfoModel.Area + " - " + getLocationInfoModel.City + " - " + getLocationInfoModel.Country;
                    double distance;
                    Area AreaDto = getNearbyArea(tenantID, latitude, longitude, null, 0, out distance);

                    if (AreaDto.Id > 0 && distance != -1)
                    {
                        DeliveryCost deliveryCostDto = getDeliveryCostByAreaId(tenantID, AreaDto.Id);

                        if (deliveryCostDto != null)
                        {

                            decimal value = -1;
                            if (deliveryCostDto.lstDeliveryCostDetailsDto != null)
                            {
                                distance = distance / 1000.00; // convert a meter to kilo-meter
                                DeliveryCostDetails deliveryCostDetailsDto = new DeliveryCostDetails();
                                foreach (var item in deliveryCostDto.lstDeliveryCostDetailsDto)
                                {
                                    if (deliveryCostDetailsDto.To <= item.To)
                                        deliveryCostDetailsDto = item;

                                    if ((double)item.From <= distance && distance <= (double)item.To)
                                    {
                                        value = item.Value;
                                        break;
                                    }

                                }
                                if (value == -1)
                                {

                                    // value = deliveryCostDto.AboveValue;

                                    value = (Math.Ceiling((decimal)distance - deliveryCostDetailsDto.To) * deliveryCostDto.AboveValue) + deliveryCostDetailsDto.Value;
                                }
                            }
                            getLocationInfoModel.DeliveryCostAfter = value;
                            getLocationInfoModel.DeliveryCostBefor = value;
                        }
                    }
                    getLocationInfoModel.LocationId = (int)AreaDto.Id;

                    //if (AreaDto.IsRestaurantsTypeAll)
                    //{
                    //    getLocationInfoModel.LocationId = 0;

                    //}
                    return getLocationInfoModel;
                }
                else
                {
                    return getLocationInfoModel;
                }

            }
            catch (Exception)
            {
                return new LocationInfoModel()
                {

                    DeliveryCostAfter = -1,
                    DeliveryCostBefor = -1,
                    LocationId = 0

                };

            }

        }
        #endregion

        #region Mapper
        private static DeliveryCost ConvertToDeliveryCost(IDataReader dataReader)
        {
            DeliveryCost deliveryCostDto = new DeliveryCost();
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
                deliveryCostDto.lstDeliveryCostDetailsDto = JsonSerializer.Deserialize<List<DeliveryCostDetails>>(deliveryCostDto.DeliveryCostJson, options);
            }
            return deliveryCostDto;
        }
        private MenuContcatKeyModel MenuContcatKey(IDataReader dataReader)
        {
            try
            {
                MenuContcatKeyModel entity = new MenuContcatKeyModel();
                entity.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");
                entity.KeyMenu = SqlDataHelper.GetValue<string>(dataReader, "KeyMenu");
                entity.Value = SqlDataHelper.GetValue<string>(dataReader, "Value");
                entity.CreationTime = SqlDataHelper.GetValue<DateTime>(dataReader, "CreationTime");
                entity.ContactID = SqlDataHelper.GetValue<int>(dataReader, "ContactID");

                return entity;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        private Item MapItemsMenu(IDataReader dataReader)
        {

            try
            {
                Item entity = new Item();


                entity.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");
                entity.Qty = SqlDataHelper.GetValue<int>(dataReader, "Qty");
                entity.MenuId = SqlDataHelper.GetValue<long>(dataReader, "MenuType");
                entity.ItemName = SqlDataHelper.GetValue<string>(dataReader, "ItemName");
                entity.ItemNameEnglish = SqlDataHelper.GetValue<string>(dataReader, "ItemNameEnglish");
                entity.ItemDescription = SqlDataHelper.GetValue<string>(dataReader, "ItemDescription");
                entity.ItemDescriptionEnglish = SqlDataHelper.GetValue<string>(dataReader, "ItemDescriptionEnglish");
                entity.ItemSubCategoryId = SqlDataHelper.GetValue<long>(dataReader, "ItemSubCategoryId");
                entity.ItemCategoryId = SqlDataHelper.GetValue<long>(dataReader, "ItemCategoryId");
                entity.ImageUri = SqlDataHelper.GetValue<string>(dataReader, "ImageUri");
                entity.Priority = SqlDataHelper.GetValue<int>(dataReader, "Priority");
                entity.Price = SqlDataHelper.GetValue<decimal>(dataReader, "Price");
                entity.OldPrice = SqlDataHelper.GetValue<decimal>(dataReader, "OldPrice");
                entity.SKU = SqlDataHelper.GetValue<string>(dataReader, "SKU");
                entity.Size = SqlDataHelper.GetValue<string>(dataReader, "Size");
                entity.Status_Code = SqlDataHelper.GetValue<int>(dataReader, "Status_Code");
                entity.Discount = SqlDataHelper.GetValue<string>(dataReader, "Ingredients");
                entity.Ingredients = SqlDataHelper.GetValue<string>(dataReader, "Ingredients");
                entity.IsInService = SqlDataHelper.GetValue<bool>(dataReader, "IsInService");
                entity.CategoryNames = SqlDataHelper.GetValue<string>(dataReader, "CategoryName");
                entity.CategoryNamesEnglish = SqlDataHelper.GetValue<string>(dataReader, "CategoryNameEnglish");
                entity.CreationTime = SqlDataHelper.GetValue<DateTime>(dataReader, "CreationTime");
                entity.DateTo = SqlDataHelper.GetValue<DateTime>(dataReader, "DateTo") ?? (DateTime?)null;
                entity.DateFrom = SqlDataHelper.GetValue<DateTime>(dataReader, "DateFrom") ?? (DateTime?)null;
                entity.SubCategoryName= SqlDataHelper.GetValue<string>(dataReader, "Name");
                entity.SubCategoryNameEnglish= SqlDataHelper.GetValue<string>(dataReader, "NameEnglish");
                entity.IsLoyal = SqlDataHelper.GetValue<bool>(dataReader, "IsLoyal");
                entity.LoyaltyPoints = SqlDataHelper.GetValue<decimal>(dataReader, "LoyaltyPoints");

                entity.IsOverrideLoyaltyPoints = SqlDataHelper.GetValue<bool>(dataReader, "IsOverrideLoyaltyPoints");
                entity.LoyaltyDefinitionId = SqlDataHelper.GetValue<long>(dataReader, "LoyaltyDefinitionId");

                try
                {
                    entity.OriginalLoyaltyPoints = SqlDataHelper.GetValue<decimal>(dataReader, "OriginalLoyaltyPoints");
                }
                catch
                {
                    entity.OriginalLoyaltyPoints =0;

                }
                if (entity.CreationTime == DateTime.MinValue)
                {
                    entity.CreationTime = DateTime.Now;
                }

                entity.MenuType = SqlDataHelper.GetValue<int>(dataReader, "MenuType");


                entity.AreaIds = SqlDataHelper.GetValue<string>(dataReader, "AreaIds");
                entity.IsQuantitative = SqlDataHelper.GetValue<bool>(dataReader, "IsQuantitative");

                if (!string.IsNullOrEmpty(SqlDataHelper.GetValue<string>(dataReader, "Specifications")))
                {

                    var options = new JsonSerializerOptions { WriteIndented = true };
                    entity.ItemSpecifications = System.Text.Json.JsonSerializer.Deserialize<List<ItemSpecification>>(
                        SqlDataHelper.GetValue<string>(dataReader, "Specifications")

                        , options);
                }

                if (!string.IsNullOrEmpty(SqlDataHelper.GetValue<string>(dataReader, "additionsCategorysListModels")))
                {

                    List<ItemAddition> itemAdditionDtos = new List<ItemAddition>();
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    entity.additionsCategorysListModels = System.Text.Json.JsonSerializer.Deserialize<List<AdditionsCategorysListModel>>(
                        SqlDataHelper.GetValue<string>(dataReader, "additionsCategorysListModels")
                        , options);
                    foreach (var item in entity.additionsCategorysListModels)
                    {
                        if (item.ItemAdditionDto != null)
                            itemAdditionDtos.AddRange(item.ItemAdditionDto);

                    }
                    entity.itemAdditionDtos = itemAdditionDtos.ToArray();
                }

                return entity;
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        public static BotService.Models.API.Order MapOrderExtraDetails(IDataReader dataReader)
        {
            BotService.Models.API.Order orderDto = new BotService.Models.API.Order();
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
        public static Contact MapContact(IDataReader dataReader)
        {
            Contact contactDto = new Contact();
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

            return contactDto;
        }
        private static UserTokenModel MapUserToken(IDataReader dataReader)
        {
            UserTokenModel model = new UserTokenModel
            {
                Id = SqlDataHelper.GetValue<long>(dataReader, "Id"),
                DeviceId = SqlDataHelper.GetValue<string>(dataReader, "DeviceId"),
                TenantId = SqlDataHelper.GetValue<int>(dataReader, "TenantId"),
                Token = SqlDataHelper.GetValue<string>(dataReader, "Token"),
                UserId = SqlDataHelper.GetValue<string>(dataReader, "UserId")


            };
            return model;
        }



        #endregion



    }
}
