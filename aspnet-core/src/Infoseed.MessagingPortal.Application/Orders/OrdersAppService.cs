using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using BarcodeLib;
using Framework.Data;
using Infoseed.MessagingPortal.Areas;
using Infoseed.MessagingPortal.Branches;
using Infoseed.MessagingPortal.CaptionBot;
using Infoseed.MessagingPortal.Contacts;
using Infoseed.MessagingPortal.DashboardUI;
using Infoseed.MessagingPortal.DashboardUI.Dto;
using Infoseed.MessagingPortal.Dto;
using Infoseed.MessagingPortal.ExtraOrderDetails.Dtos;
using Infoseed.MessagingPortal.General;
using Infoseed.MessagingPortal.Items;
using Infoseed.MessagingPortal.Loyalty;
using Infoseed.MessagingPortal.Loyalty.Dto;
using Infoseed.MessagingPortal.MultiTenancy;
using Infoseed.MessagingPortal.OrderDetails.Dtos;
using Infoseed.MessagingPortal.Orders.Dtos;
using Infoseed.MessagingPortal.Orders.Exporting;
using Infoseed.MessagingPortal.SocketIOClient;
using Infoseed.MessagingPortal.Tenants.Contacts;
using Infoseed.MessagingPortal.WhatsApp;
using Infoseed.MessagingPortal.WhatsApp.Dto;
using Microsoft.Azure.Documents;
using Nancy.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using System.Transactions;
using static Infoseed.MessagingPortal.DashboardUI.Dto.DashboardEnums;

namespace Infoseed.MessagingPortal.Orders
{
    public class OrdersAppService : MessagingPortalAppServiceBase, IOrdersAppService
    {
        private readonly IOrderExcelExporter _orderExcelExporter;
        private IAreasAppService _areasAppService;
        private readonly IRepository<Order, long> _orderRepository;
        private readonly IRepository<Branch, long> _lookup_branchRepository;
        private IRepository<Contact> _lookup_customerRepository;
        private readonly IRepository<Area, long> _lookup_areaRepository;
        private readonly IDocumentClient _IDocumentClient;
        private ILoyaltyAppService _ILoyaltyAppService;
        private IGeneralAppService _generalAppService;
        private readonly IDashboardUIAppService _dashboardUIAppService;

        public OrdersAppService(
            IAreasAppService areasAppService,
            IRepository<Order, long> orderRepository,
            IRepository<Branch, long> lookup_branchRepository,
            IRepository<Contact> lookup_customerRepository,
            IRepository<Area, long> lookup_areaRepository,
            IOrderExcelExporter orderExcelExporter,
            IDocumentClient iDocumentClient,
            ILoyaltyAppService ILoyaltyAppService,
            IGeneralAppService generalAppService,
            IDashboardUIAppService dashboardUIAppService
        )
        {
            _lookup_areaRepository = lookup_areaRepository;
            _orderExcelExporter = orderExcelExporter;
            _orderRepository = orderRepository;
            _lookup_branchRepository = lookup_branchRepository;
            _lookup_customerRepository = lookup_customerRepository;
            _areasAppService = areasAppService;
            _IDocumentClient = iDocumentClient;
            _ILoyaltyAppService = ILoyaltyAppService;
            _generalAppService = generalAppService;
            _dashboardUIAppService = dashboardUIAppService;
        }

        public OrdersAppService()
        {

        }
        public OrderStatusHistoryDto GetOrderStatusHistory(long orderId)
        {
            return getOrderStatusHistory(orderId);
        }
        public OrderDto GetOrderById(long orderId)
        {
            return getOrderById(orderId);
        }

        public OrderDto GetOrderByNumber(long orderNumber, int tenantId)
        {
            return getDeliveryEstimationByOrderNumber(orderNumber, tenantId);
        }
        public void UpdateOrderStatus(OrderDto order)
        {
            updateOrderStatus(order);
        }
        private void updateOrderStatus(OrderDto order)
        {
            try
            {
                var SP_Name = Constants.Order.SP_OrderStatusUpdate;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                      new System.Data.SqlClient.SqlParameter("@OrderId",order.Id),
                      new System.Data.SqlClient.SqlParameter("@OrderStatus",order.OrderStatus),
                      new System.Data.SqlClient.SqlParameter("@OrderRemarks",order.OrderRemarks),
                      new System.Data.SqlClient.SqlParameter("@IsLockByAgent",order.IsLockByAgent),
                      new System.Data.SqlClient.SqlParameter("@AgentId",order.AgentId),
                      new System.Data.SqlClient.SqlParameter("@LockByAgentName",order.LockByAgentName),
                      new System.Data.SqlClient.SqlParameter("@ActionTime",order.ActionTime),
                };
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private OrderDto getDeliveryEstimationByOrderNumber (long orderNumber, int tenantId)
        {
            try
            {
                var result = new OrderDto();
                string connectionString = AppSettingsModel.ConnectionStrings;

                using (var connection = new SqlConnection(connectionString))
                {
                    using (var command = new SqlCommand("dbo.GetOrderByNumber", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@OrderNumber", orderNumber);
                        command.Parameters.AddWithValue("@TenantId", tenantId);

                        connection.Open();

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                result.DeliveryEstimation = reader["DeliveryEstimation"] as string;
                                result.Total = reader.GetDecimal(reader.GetOrdinal("Total"));
                                result.ContactId = reader.GetInt32(reader.GetOrdinal("ContactId"));
                            }
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private OrderDto getOrderById(long orderId)
        {
            try
            {
                OrderDto order = new OrderDto();
                var SP_Name = Constants.Order.SP_OrderByIdGet;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                      new System.Data.SqlClient.SqlParameter("@OrderId",orderId)
                };
                order = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapOrder, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                return order;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public bool CreateOrderStatusHistory(long orderId, int orderStatusId, int? TenantId = null)
        {
            return createOrderStatusHistory(orderId, orderStatusId, TenantId);
        }
        private bool createOrderStatusHistory(long orderId, int orderStatusId, int? TenantId = null)
        {

            if (TenantId == null)
            {

                TenantId = AbpSession.TenantId.Value;
            }


            var CreatedBy = 0;
            if (AbpSession.UserId != null)
            {

                CreatedBy = (int)AbpSession.UserId.Value;
            }
            try
            {
                var SP_Name = Constants.Order.SP_OrderStatusHistoryAdd;
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
        //public bool CreateOrderStatusZeedlyHistory(long orderId, int orderStatusId, int? TenantId = null)
        //{
        //    return createOrderStatusHistory(orderId, orderStatusId, TenantId);
        //}
        //private bool createOrderStatusZeedlyHistory(long orderId, int orderStatusId, int? tenantId = null)
        //{
        //    if (tenantId == null)
        //    {
        //        tenantId = AbpSession.TenantId;
        //    }
        //    int createdBy = AbpSession.UserId != null ? (int)AbpSession.UserId.Value : 0;

        //    try
        //    {
        //        string spName = Constants.Order.SP_OrderStatusHistoryAddFromZeedly;

        //        var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
        //       {
        //           new System.Data.SqlClient.SqlParameter("@TenantId", tenantId ?? (object)DBNull.Value),
        //           new System.Data.SqlClient.SqlParameter("@OrderId", orderId),
        //           new System.Data.SqlClient.SqlParameter("@OrderStatusIdZeedly", orderStatusId), 
        //           new System.Data.SqlClient.SqlParameter("@CreatedDate", DateTime.UtcNow),
        //           new System.Data.SqlClient.SqlParameter("@CreatedBy", createdBy)
        //       };

        //        var outputParameter = new System.Data.SqlClient.SqlParameter
        //        {
        //            SqlDbType = System.Data.SqlDbType.Bit,
        //            ParameterName = "@Result",
        //            Direction = System.Data.ParameterDirection.Output
        //        };

        //        sqlParameters.Add(outputParameter);

        //        SqlDataHelper.ExecuteNoneQuery(spName, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

        //        return (bool)outputParameter.Value;
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        //private bool CreateOrderStatusZeedlyHistory(long orderId, int? orderStatusId, int? tenantId = null)
        //{
        //    if (tenantId == null)
        //    {
        //        tenantId = AbpSession.TenantId;
        //    }

        //    var createdBy = 0;
        //    if (AbpSession.UserId != null)
        //    {
        //        createdBy = (int)AbpSession.UserId.Value;
        //    }

        //    try
        //    {
        //        var spName = Constants.Order.SP_OrderStatusHistoryAddFromZeedly; // خلي اسم SP الخاص بالجدول الجديد
        //        var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
        //{
        //    new System.Data.SqlClient.SqlParameter("@TenantId", tenantId ?? (object)DBNull.Value),
        //    new System.Data.SqlClient.SqlParameter("@OrderId", orderId),
        //    new System.Data.SqlClient.SqlParameter("@OrderStatusId", orderStatusId ?? (object)DBNull.Value),
        //    new System.Data.SqlClient.SqlParameter("@CreatedDate", DateTime.UtcNow),
        //    new System.Data.SqlClient.SqlParameter("@CreatedBy", createdBy)
        //};

        //        var outputParameter = new System.Data.SqlClient.SqlParameter
        //        {
        //            SqlDbType = System.Data.SqlDbType.Bit,
        //            ParameterName = "@Result",
        //            Direction = System.Data.ParameterDirection.Output
        //        };

        //        sqlParameters.Add(outputParameter);

        //        SqlDataHelper.ExecuteNoneQuery(spName, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

        //        return (bool)outputParameter.Value;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw; // نعيد الرمي من غير ex علشان ما نضيع الـ StackTrace
        //    }
        //}



        private bool getOrderStatusValidation(long orderId, int orderStatusId)
        {
            try
            {
                var SP_Name = Constants.Order.SP_OrderStatusHistoryValidationGet;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                     new System.Data.SqlClient.SqlParameter("@TenantId",AbpSession.TenantId.Value)
                    ,new System.Data.SqlClient.SqlParameter("@OrderId",orderId)
                    ,new System.Data.SqlClient.SqlParameter("@OrderStatusId",orderStatusId)

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
        public OrderEntity GetAllByContactId(int contactId, int? tenantId, int? pageNumber, int? pageSize)
        {

            if (tenantId == null)
            {
                tenantId = AbpSession.TenantId;
            }

            return GetALLOrdersByContactId(contactId, tenantId, pageNumber, pageSize);
        }

        public OrderEntity GetAllLoyaltyRemainingdays(int contactId, int? tenantId, int? pageNumber, int? pageSize)
        {
            if (tenantId == null)
            {
                tenantId = AbpSession.TenantId;
            }
            return FGetAllLoyaltyRemainingdays(contactId, tenantId, pageNumber, pageSize);
        }

        public async Task<PagedResultDto<GetOrderForViewDto>> GetAll(GetAllOrdersInput input)
        {
            if (input.Filter == "-1")
            {
                input.Filter = "";
            }



            int totalCount = 0;
            int? orderStatus = null;
            string sorting = null;
            string PhoneNumber = null;

            if (!string.IsNullOrEmpty(input.Filter))
            {
                orderStatus = int.Parse(input.Filter);
            }
            if (!string.IsNullOrEmpty(input.Sorting))
            {
                sorting = input.Sorting;
            }
            if (!string.IsNullOrEmpty(input.OrderRemarksFilter))
            {
                PhoneNumber = input.OrderRemarksFilter;
            }


            List<GetOrderForViewDto> itemes = GetALLOrders(input.SkipCount, input.MaxResultCount, input.IsArchived, out totalCount, orderStatus, sorting, PhoneNumber, input.UserIdFilter);

            return new PagedResultDto<GetOrderForViewDto>(totalCount, itemes);
        }



        public string GetOrderDetailsForBot(long orderId, int lang, string resourceIds,bool isOrderOffer, long areaId)
        {

            var result = getOrderDetailsForBot(orderId, lang, resourceIds, isOrderOffer,  areaId);

            return JsonConvert.SerializeObject(result);
        }
        public async Task<long> CreateNewOrder(string orderJson)
        {

            return await createNewOrder(orderJson);
        }
        public async Task<long> PostNewOrder(string orderJson)
        {

            return await postNewOrder(orderJson);
        }
        public async Task<long> CreateOrderDetails(string orderDetailsJson)
        {

            return await createOrderDetails(orderDetailsJson);
        }

        public async Task<long> CreateOrderDetailsExtra(string orderDetailsExtraJson)

        {

            return await createOrderDetailsExtra(orderDetailsExtraJson);
        }
        public async Task<long> CreateOrderDetailsSpecifications(string orderDetailsSpecificationsJson)
        {

            return await createOrderDetailsSpecifications(orderDetailsSpecificationsJson);
        }

        public async Task<FileDto> GetOrderToExcel(GetAllOrdersInput input)
        {



            int totalcount = 0;
            List<GetOrderForViewDto> itemes = GetALLOrders(input.SkipCount, input.MaxResultCount, input.IsArchived, out totalcount, null, null, null, null);

            return _orderExcelExporter.ExportToFile(itemes);
        }

        public async Task<GetOrderForViewDto> GetOrderForView(long id)
        {
            var order = await _orderRepository.GetAsync(id);

            var output = new GetOrderForViewDto { Order = ObjectMapper.Map<OrderDto>(order) };

            if (output.Order.BranchId != null)
            {
                var _lookupBranch = await _lookup_branchRepository.FirstOrDefaultAsync((long)output.Order.BranchId);
                output.BranchName = _lookupBranch?.Name?.ToString();
            }

            if (output.Order.ContactId != null)
            {
                var _lookupCustomer = await _lookup_customerRepository.FirstOrDefaultAsync((int)output.Order.ContactId);
                output.CustomerCustomerName = _lookupCustomer?.DisplayName?.ToString();
            }

            return output;
        }

        public async Task<GetOrderForEditOutput> GetOrderForEdit(EntityDto<long> input)
        {
            var order = await _orderRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetOrderForEditOutput { Order = ObjectMapper.Map<CreateOrEditOrderDto>(order) };


            if (output.Order.BranchId != null)
            {
                var _lookupBranch = await _lookup_branchRepository.FirstOrDefaultAsync((long)output.Order.BranchId);
                output.BranchName = _lookupBranch?.Name?.ToString();
            }

            if (output.Order.CustomerId != null)
            {
                var _lookupCustomer = await _lookup_customerRepository.FirstOrDefaultAsync((int)output.Order.CustomerId);
                output.CustomerCustomerName = _lookupCustomer?.DisplayName?.ToString();
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditOrderDto input)
        {
            if (input.Id == null)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }

        protected virtual async Task Create(CreateOrEditOrderDto input)
        {
            var order = ObjectMapper.Map<Order>(input);


            if (AbpSession.TenantId != null)
            {
                order.TenantId = (int?)AbpSession.TenantId;
            }


            await _orderRepository.InsertAsync(order);
        }

        protected virtual async Task Update(CreateOrEditOrderDto input)
        {
            var order = await _orderRepository.FirstOrDefaultAsync((long)input.Id);
            ObjectMapper.Map(input, order);
        }

        public async Task Delete(EntityDto<long> input)
        {
            await _orderRepository.DeleteAsync(input.Id);
        }

        public async Task<List<GetOrderDetailForViewDto>> GetAllDetail(long orderId)
        {
            List<GetOrderDetailForViewDto> orderDetailsList = getOrderDetails(orderId);
            foreach (var item in orderDetailsList)
            {
                var exList2 = GetMenuOrderDetailExtraForDetails(AbpSession.TenantId, item.OrderDetail.Id);
                List<CategoryExtraOrderDetailsDto> lstCategoryExtraOrderDetailsDto = new List<CategoryExtraOrderDetailsDto>();
                foreach (var itemCat in exList2)
                {
                    CategoryExtraOrderDetailsDto objCategoryExtraOrderDetailsDto = new CategoryExtraOrderDetailsDto();
                    objCategoryExtraOrderDetailsDto.lstExtraOrderDetailsDto = new List<ExtraOrderDetailsDto>();
                    foreach (var itemex in itemCat.lstExtraOrderDetailsDto)
                    {

                        if (AbpSession.TenantId == 34)
                        {
                            objCategoryExtraOrderDetailsDto.lstExtraOrderDetailsDto.Add(new ExtraOrderDetailsDto
                            {
                                Id = 0,
                                Name = itemex.Name,
                                NameEnglish = itemex.NameEnglish,
                                OrderDetailId = itemex.OrderDetailId,
                                Quantity = itemex.Quantity,
                                TenantId = itemex.TenantId,
                                Total = 0,// itemex.Total,
                                UnitPrice = 0,//itemex.UnitPrice,
                                UnitPoints = 0,
                                TotalLoyaltyPoints = 0

                            });
                        }
                        else
                        {
                            if (itemex.Total == 0)
                            {

                                objCategoryExtraOrderDetailsDto.lstExtraOrderDetailsDto.Add(new ExtraOrderDetailsDto
                                {
                                    Id = 0,
                                    Name = itemex.Name,
                                    NameEnglish=itemex.NameEnglish,
                                    OrderDetailId = itemex.OrderDetailId,
                                    Quantity = itemex.Quantity,
                                    TenantId = itemex.TenantId,
                                    Total = itemex.UnitPrice * itemex.Quantity,
                                    UnitPrice = itemex.UnitPrice,
                                    SpecificationNameEnglish = itemex.SpecificationNameEnglish,
                                    SpecificationName = itemex.SpecificationName,
                                    SpecificationUniqueId = itemex.SpecificationUniqueId,
                                    UnitPoints = itemex.UnitPoints,
                                    TotalLoyaltyPoints = itemex.UnitPoints * itemex.Quantity * item.OrderDetail.Quantity,

                                });
                            }
                            else
                            {

                                objCategoryExtraOrderDetailsDto.lstExtraOrderDetailsDto.Add(new ExtraOrderDetailsDto
                                {
                                    Id = 0,
                                    Name = itemex.Name,
                                    NameEnglish = itemex.NameEnglish,
                                    OrderDetailId = itemex.OrderDetailId,
                                    Quantity = itemex.Quantity,
                                    TenantId = itemex.TenantId,
                                    Total = itemex.Quantity * itemex.UnitPrice * item.OrderDetail.Quantity,
                                    UnitPrice = itemex.UnitPrice,
                                    SpecificationNameEnglish = itemex.SpecificationNameEnglish,
                                    SpecificationName = itemex.SpecificationName,
                                    SpecificationUniqueId = itemex.SpecificationUniqueId,
                                    UnitPoints = itemex.UnitPoints,
                                    TotalLoyaltyPoints = itemex.Quantity * itemex.UnitPoints * item.OrderDetail.Quantity,
                                });
                            }

                        }

                    }
                    objCategoryExtraOrderDetailsDto.SpecificationUniqueId = itemCat.SpecificationUniqueId;
                    objCategoryExtraOrderDetailsDto.SpecificationName = itemCat.SpecificationName;
                    objCategoryExtraOrderDetailsDto.SpecificationNameEnglish = itemCat.SpecificationNameEnglish;
                    lstCategoryExtraOrderDetailsDto.Add(objCategoryExtraOrderDetailsDto);

                }

                item.OrderDetail.lstCategoryExtraOrderDetailsDto = lstCategoryExtraOrderDetailsDto;

                try
                {
                    var it = GetItemID(item.OrderDetail.ItemId);
                    if (it.Id != 0)
                    {
                        if (!string.IsNullOrEmpty(it.Barcode))
                        {
                            item.BarCode = it.Barcode;
                            item.SKU = it.Barcode.ToString();
                            if (it.BarcodeImg == null || it.BarcodeImg == "")
                            {
                                var urrlImg = tBarCode(it.Barcode.ToString(), it);

                                item.BarcodeImg = urrlImg;
                                item.IsBarcodeImg = true;
                            }
                            else
                            {
                                item.BarcodeImg = it.BarcodeImg;
                                item.IsBarcodeImg = true;

                            }
                            item.IsBarcodeImg = true;

                        }
                        else
                        {
                            item.IsBarcodeImg = false;
                        }
                    }
                    else
                    {
                        item.IsBarcodeImg = false;
                    }
                }
                catch
                {


                }
            }
            return orderDetailsList.ToList();

        }

        //for ordering menu sys

        public async Task<List<GetOrderDetailForViewDto>> GetOrderDetailsForMenu(int tenantId, long orderId)
        {
            List<GetOrderDetailForViewDto> orderDetailsList = getOrderDetails(orderId);
            foreach (var item in orderDetailsList)
            {
                //var exList2 = GetOrderDetailExtraForDetails(tenantId, item.OrderDetail.Id);

                var exList2 = GetMenuOrderDetailExtraForDetails(tenantId, item.OrderDetail.Id);
                List<CategoryExtraOrderDetailsDto> lstCategoryExtraOrderDetailsDto = new List<CategoryExtraOrderDetailsDto>();
                foreach (var itemCat in exList2)
                {
                    CategoryExtraOrderDetailsDto objCategoryExtraOrderDetailsDto = new CategoryExtraOrderDetailsDto();
                    objCategoryExtraOrderDetailsDto.lstExtraOrderDetailsDto = new List<ExtraOrderDetailsDto>();
                    foreach (var itemex in itemCat.lstExtraOrderDetailsDto)
                    {

                        if ((int?)AbpSession.TenantId == 34)
                        {
                            objCategoryExtraOrderDetailsDto.lstExtraOrderDetailsDto.Add(new ExtraOrderDetailsDto
                            {
                                Id = 0,
                                Name = itemex.Name,
                                NameEnglish = itemex.NameEnglish,
                                OrderDetailId = itemex.OrderDetailId,
                                Quantity = 0,//itemex.Quantity,
                                TenantId = itemex.TenantId,
                                Total = 0,// itemex.Total,
                                UnitPrice = 0,//itemex.UnitPrice,,
                                TotalLoyaltyPoints = 0,
                                UnitPoints = 0,
                                TypeExtraDetails = itemex.TypeExtraDetails,
                                SpecificationId = itemex.SpecificationId,
                                SpecificationChoiceId = itemex.SpecificationChoiceId

                            });
                            item.OrderDetail.TotalLoyaltyPoints = (item.OrderDetail.Quantity * itemex.UnitPoints).HasValue ? item.OrderDetail.TotalLoyaltyPoints : 0;
                            item.OrderDetail.Total = item.OrderDetail.Quantity * itemex.UnitPrice;
                            item.OrderDetail.UnitPrice = itemex.UnitPrice;
                            item.OrderDetail.UnitPoints = itemex.UnitPoints.HasValue ? item.OrderDetail.UnitPoints : 0;
                        }
                        else
                        {
                            if (itemex.Total == 0)
                            {

                                objCategoryExtraOrderDetailsDto.lstExtraOrderDetailsDto.Add(new ExtraOrderDetailsDto
                                {
                                    Id = 0,
                                    Name = itemex.Name,
                                    NameEnglish = itemex.NameEnglish,
                                    OrderDetailId = itemex.OrderDetailId,
                                    Quantity = itemex.Quantity,
                                    TenantId = itemex.TenantId,
                                    Total = itemex.UnitPrice * itemex.Quantity,
                                    UnitPrice = itemex.UnitPrice,
                                    TotalLoyaltyPoints = itemex.UnitPoints * itemex.Quantity,
                                    UnitPoints = itemex.UnitPoints,
                                    SpecificationNameEnglish = itemex.SpecificationNameEnglish,
                                    SpecificationName = itemex.SpecificationName,
                                    SpecificationUniqueId = itemex.SpecificationUniqueId,
                                    TypeExtraDetails = itemex.TypeExtraDetails,
                                    SpecificationId = itemex.SpecificationId,
                                    SpecificationChoiceId = itemex.SpecificationChoiceId
                                });
                            }
                            else
                            {

                                objCategoryExtraOrderDetailsDto.lstExtraOrderDetailsDto.Add(new ExtraOrderDetailsDto
                                {
                                    Id = 0,
                                    Name = itemex.Name,
                                    NameEnglish=itemex.NameEnglish,
                                    OrderDetailId = itemex.OrderDetailId,
                                    Quantity = itemex.Quantity,
                                    TenantId = itemex.TenantId,
                                    Total = itemex.Total,
                                    UnitPrice = itemex.UnitPrice,
                                    TotalLoyaltyPoints = itemex.UnitPoints * itemex.Quantity,
                                    UnitPoints = itemex.UnitPoints,
                                    SpecificationNameEnglish = itemex.SpecificationNameEnglish,
                                    SpecificationName = itemex.SpecificationName,
                                    SpecificationUniqueId = itemex.SpecificationUniqueId,
                                    TypeExtraDetails = itemex.TypeExtraDetails,
                                    SpecificationId = itemex.SpecificationId,
                                    SpecificationChoiceId = itemex.SpecificationChoiceId
                                });
                            }

                        }

                    }
                    objCategoryExtraOrderDetailsDto.SpecificationUniqueId = itemCat.SpecificationUniqueId;
                    objCategoryExtraOrderDetailsDto.SpecificationName = itemCat.SpecificationName;
                    objCategoryExtraOrderDetailsDto.SpecificationNameEnglish = itemCat.SpecificationNameEnglish;
                    lstCategoryExtraOrderDetailsDto.Add(objCategoryExtraOrderDetailsDto);

                }

                item.OrderDetail.lstCategoryExtraOrderDetailsDto = lstCategoryExtraOrderDetailsDto;

                try
                {
                    var it = GetItemID(item.OrderDetail.ItemId);

                    item.ItemImageUrl = it.ImageUri;
                    if (it.Id != 0)
                    {
                        if (!string.IsNullOrEmpty(it.Barcode))
                        {
                            item.BarCode = it.Barcode;
                            item.SKU = it.Barcode.ToString();
                            if (it.BarcodeImg == null || it.BarcodeImg == "")
                            {
                                var urrlImg = tBarCode(it.Barcode.ToString(), it);

                                item.BarcodeImg = urrlImg;
                                item.IsBarcodeImg = true;
                            }
                            else
                            {
                                item.BarcodeImg = it.BarcodeImg;
                                item.IsBarcodeImg = true;

                            }
                            item.IsBarcodeImg = true;

                        }
                        else
                        {
                            item.IsBarcodeImg = false;
                        }

                    }
                    else
                    {
                        item.IsBarcodeImg = false;
                    }
                }
                catch
                {


                }
            }
            return orderDetailsList.ToList();
        }


        public async Task<List<GetOrderDetailForViewDto>> GetAllDetailForMenu(int tenantId, long orderId)
        {
            
            List<GetOrderDetailForViewDto> orderDetailsList = getOrderDetails(orderId);

            foreach (var item in orderDetailsList)
            {
                var exList2 = GetMenuOrderDetailExtraForDetails(tenantId, item.OrderDetail.Id);
                List<CategoryExtraOrderDetailsDto> lstCategoryExtraOrderDetailsDto = new List<CategoryExtraOrderDetailsDto>();
                foreach (var itemCat in exList2)
                {
                    CategoryExtraOrderDetailsDto objCategoryExtraOrderDetailsDto = new CategoryExtraOrderDetailsDto();
                    objCategoryExtraOrderDetailsDto.lstExtraOrderDetailsDto = new List<ExtraOrderDetailsDto>();
                    foreach (var itemex in itemCat.lstExtraOrderDetailsDto)
                    {

                        if ((int?)AbpSession.TenantId == 34)
                        {
                            objCategoryExtraOrderDetailsDto.lstExtraOrderDetailsDto.Add(new ExtraOrderDetailsDto
                            {
                                Id = 0,
                                NameEnglish = itemex.NameEnglish,
                                Name = itemex.Name,
                                OrderDetailId = itemex.OrderDetailId,
                                Quantity = 0,//itemex.Quantity,
                                TenantId = itemex.TenantId,
                                Total = 0,// itemex.Total,
                                UnitPrice = 0,//itemex.UnitPrice,,


                            });
                            item.OrderDetail.Total = item.OrderDetail.Quantity * itemex.UnitPrice;
                            item.OrderDetail.UnitPrice = itemex.UnitPrice;
                        }
                        else
                        {
                            if (itemex.Total == 0)
                            {

                                objCategoryExtraOrderDetailsDto.lstExtraOrderDetailsDto.Add(new ExtraOrderDetailsDto
                                {
                                    Id = 0,
                                    Name = itemex.Name,
                                    NameEnglish = itemex.NameEnglish,
                                    OrderDetailId = itemex.OrderDetailId,
                                    Quantity = itemex.Quantity,
                                    TenantId = itemex.TenantId,
                                    Total = itemex.UnitPrice * itemex.Quantity,
                                    UnitPrice = itemex.UnitPrice,
                                    SpecificationNameEnglish = itemex.SpecificationNameEnglish,
                                    SpecificationName = itemex.SpecificationName,
                                    SpecificationUniqueId = itemex.SpecificationUniqueId,

                                });
                            }
                            else
                            {

                                objCategoryExtraOrderDetailsDto.lstExtraOrderDetailsDto.Add(new ExtraOrderDetailsDto
                                {
                                    Id = 0,
                                    Name = itemex.Name,
                                    NameEnglish = itemex.NameEnglish,
                                    OrderDetailId = itemex.OrderDetailId,
                                    Quantity = itemex.Quantity,
                                    TenantId = itemex.TenantId,
                                    Total = itemex.Total,
                                    UnitPrice = itemex.UnitPrice,
                                    SpecificationNameEnglish = itemex.SpecificationNameEnglish,
                                    SpecificationName = itemex.SpecificationName,
                                    SpecificationUniqueId = itemex.SpecificationUniqueId,

                                });
                            }

                        }

                    }
                    objCategoryExtraOrderDetailsDto.SpecificationUniqueId = itemCat.SpecificationUniqueId;
                    objCategoryExtraOrderDetailsDto.SpecificationName = itemCat.SpecificationName;
                    objCategoryExtraOrderDetailsDto.SpecificationNameEnglish = itemCat.SpecificationNameEnglish;
                    lstCategoryExtraOrderDetailsDto.Add(objCategoryExtraOrderDetailsDto);

                }

                item.OrderDetail.lstCategoryExtraOrderDetailsDto = lstCategoryExtraOrderDetailsDto;

                try
                {
                    var it = GetItemID(item.OrderDetail.ItemId);

                    item.ItemImageUrl = it.ImageUri;
                    if (it.Id != 0)
                    {
                        if (!string.IsNullOrEmpty(it.Barcode))
                        {
                            item.BarCode = it.Barcode;
                            item.SKU = it.Barcode.ToString();
                            if (it.BarcodeImg == null || it.BarcodeImg == "")
                            {
                                var urrlImg = tBarCode(it.Barcode.ToString(), it);

                                item.BarcodeImg = urrlImg;
                                item.IsBarcodeImg = true;
                            }
                            else
                            {
                                item.BarcodeImg = it.BarcodeImg;
                                item.IsBarcodeImg = true;

                            }
                            item.IsBarcodeImg = true;

                        }
                        else
                        {
                            item.IsBarcodeImg = false;
                        }
                    }
                    else
                    {
                        item.IsBarcodeImg = false;
                    }
                }
                catch
                {


                }


            }
            return orderDetailsList.ToList();

        }


        public string tBarCode(string Code, Item item)
        {
            // Create an instance of the API
            Barcode barcodLib = new Barcode();


            int imageWidth = 290;  // barcode image width
            int imageHeight = 120; //barcode image height
            System.Drawing.Color foreColor = System.Drawing.Color.Black; // Color to print barcode
            System.Drawing.Color backColor = System.Drawing.Color.White; //background color

            //only numbers are allowed in UPCA type
            string NumericString = Code;

            //type upca
            Image barcodeImage = null;

            if (Code.Length >= 12)
            {
                barcodeImage = barcodLib.Encode(TYPE.EAN13, NumericString, foreColor, backColor, imageWidth, imageHeight);

            }
            else if (Code.Length > 8)
            {

                barcodeImage = barcodLib.Encode(TYPE.UPCA, NumericString, foreColor, backColor, imageWidth, imageHeight);
            }
            else if (Code.Length >= 0)
            {

                barcodeImage = barcodLib.Encode(TYPE.EAN8, NumericString, foreColor, backColor, imageWidth, imageHeight);
            }


            // Store image in some path with the desired format
            //note: you must have permission to save file in the specified path
            //barcodeImage.Save(@"D:\Barcode.png", ImageFormat.Png);

            var url = GetImageURL(barcodeImage).Result;
            item.BarcodeImg = url;
            updateItemBarCode(item);
            return url;
        }


        private async Task<string> GetImageURL(Image model)
        {
            var url = "";

            byte[] fileData = null;

            using (var ms = new MemoryStream())
            {
                model.Save(ms, ImageFormat.Png);
                fileData = ms.ToArray();
            }



            AzureBlobProvider azureBlobProvider = new AzureBlobProvider();
            AttachmentContent attachmentContent = new AttachmentContent()
            {
                Content = fileData,
                Extension = ".png",//Path.GetExtension(formFile.FileName),
                MimeType = "image/png",//model.GetType().ToString(),

            };

            url = await azureBlobProvider.Save(attachmentContent);



            return url;
        }

        private void updateItemBarCode(Item item)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            using (SqlConnection connection = new SqlConnection(connString))
            using (SqlCommand command = connection.CreateCommand())
            {


                command.CommandText = "UPDATE Items SET "
                    + "BarcodeImg = @BarcodeImg "
                    + " Where Id = @Id";

                command.Parameters.AddWithValue("@Id", item.Id);
                command.Parameters.AddWithValue("@BarcodeImg", item.BarcodeImg);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        public List<OrderDetailDto> GetOrderDetail(int? TenantID, int? OrderId)
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

            List<OrderDetailDto> orderDetailDtos = new List<OrderDetailDto>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                orderDetailDtos.Add(new OrderDetailDto
                {
                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                    Discount = decimal.Parse(dataSet.Tables[0].Rows[i]["Discount"].ToString()),
                    ItemId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemId"]),
                    OrderId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["OrderId"]),
                    Quantity = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Quantity"]),
                    //TotalAfterDiscunt = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TotalAfterDiscunt"]),
                    Total = decimal.Parse(dataSet.Tables[0].Rows[i]["Total"].ToString()),
                    UnitPrice = decimal.Parse(dataSet.Tables[0].Rows[i]["UnitPrice"].ToString()),



                });
            }

            conn.Close();
            da.Dispose();

            return orderDetailDtos;
        }
        public async Task Lock(EntityDto<long> input, int agentId, string agentName, string stringTotall)
        {
            var order = await _orderRepository.FirstOrDefaultAsync((long)input.Id);

            order.IsLockByAgent = true;
            order.AgentId = agentId;
            order.LockByAgentName = agentName;

            await _orderRepository.UpdateAsync(order);


            var area = _lookup_areaRepository.GetAll().Where(x => x.Id == order.AreaId).FirstOrDefault();


            var orderStatusName = Enum.GetName(typeof(OrderStatusEunm), order.orderStatus);
            var orderTypeName = Enum.GetName(typeof(OrderTypeEunm), order.OrderType);

            GetOrderForViewDto getOrderForViewDto = new GetOrderForViewDto();
            var GetOrderMap = ObjectMapper.Map(order, getOrderForViewDto.Order);
            GetOrderMap.StringTotal = stringTotall;
            getOrderForViewDto.Order = GetOrderMap;
            getOrderForViewDto.OrderStatusName = orderStatusName;
            getOrderForViewDto.OrderTypeName = orderTypeName;
            getOrderForViewDto.StringdeliveryCost = order.Total.ToString();
            getOrderForViewDto.TenantId = AbpSession.TenantId;
            getOrderForViewDto.IsLockedByAgent = true;
            if (area != null)
            {
                getOrderForViewDto.AreahName = area.AreaName + " (" + area.AreaCoordinate.ToString() + ")";
            }

            // await _hub.Clients.All.SendAsync("brodCastAgentOrder", getOrderForViewDto);
            SocketIOManager.SendOrder(getOrderForViewDto, order.TenantId.Value);

        }
        public async Task UnLock(EntityDto<long> input, string stringTotall)
        {
            var order = await _orderRepository.FirstOrDefaultAsync((long)input.Id);
            //order.orderStatus = OrderStatusEunm.Pending;
            order.IsLockByAgent = false;
            order.AgentId = 0;
            order.LockByAgentName = null;
            await _orderRepository.UpdateAsync(order);

            var area = _lookup_areaRepository.GetAll().Where(x => x.Id == order.AreaId).FirstOrDefault();

            var orderStatusName = Enum.GetName(typeof(OrderStatusEunm), order.orderStatus);
            var orderTypeName = Enum.GetName(typeof(OrderTypeEunm), order.OrderType);


            GetOrderForViewDto getOrderForViewDto = new GetOrderForViewDto();
            var GetOrderMap = ObjectMapper.Map(order, getOrderForViewDto.Order);
            GetOrderMap.StringTotal = stringTotall;
            getOrderForViewDto.Order = GetOrderMap;
            getOrderForViewDto.OrderStatusName = orderStatusName;
            getOrderForViewDto.OrderTypeName = orderTypeName;
            getOrderForViewDto.TenantId = AbpSession.TenantId;
            getOrderForViewDto.IsLockedByAgent = true;

            if (area != null)
            {
                getOrderForViewDto.AreahName = area.AreaName + " (" + area.AreaCoordinate.ToString() + ")";
            }

            // await _hub.Clients.All.SendAsync("brodCastAgentOrder", getOrderForViewDto);
            SocketIOManager.SendOrder(getOrderForViewDto, order.TenantId.Value);

        }
        public async Task<bool> DeleteOrder(EntityDto<long> input, string stringTotall, int agentId, string agentName)
        {
            try
            {
                if (getOrderStatusValidation(input.Id, (int)OrderStatusEunm.Deleted))
                {
                    var order = getOrderById(input.Id);
                    var tenant = _generalAppService.GetTenantById(order.TenantId);
                    var user = GetCustomerAzuer(order.ContactId.ToString());
                    var area = _areasAppService.GetAreaById((int)order.AreaId, order.TenantId);
                    var orderStatusName = Enum.GetName(typeof(OrderStatusEunm), (int)OrderStatusEunm.Deleted);
                    var orderTypeName = Enum.GetName(typeof(OrderTypeEunm), order.OrderType);

                    using (TransactionScope transactionScope = new TransactionScope())
                    {
                        if (!string.IsNullOrEmpty(tenant.AccessToken))
                        {
                            order.OrderStatus = OrderStatusEunm.Deleted;
                            order.IsLockByAgent = true;
                            order.AgentId = agentId;
                            order.LockByAgentName = agentName;
                            order.ActionTime = DateTime.UtcNow;

                            updateOrderStatus(order);
                            _dashboardUIAppService.CreateDashboardNumber(new DashboardNumbers
                            {
                                TenantId = order.TenantId,
                                TypeId = (int)DashboardTypeEnum.Order,
                                StatusId = (int)OrderStatusEunm.Deleted,
                                StatusName = orderStatusName
                            });


                            createOrderStatusHistory(input.Id, (int)OrderStatusEunm.Deleted);
                            ContactLoyaltyTransactionModel contactLoyalty = new ContactLoyaltyTransactionModel();
                            decimal total = 0;
                            if (order.Total > 0)
                            {
                                decimal DeliveryCost = order.DeliveryCost.HasValue ? order.DeliveryCost.Value : 0;
                                total = (order.Total - DeliveryCost);
                                if (total < 0)
                                {
                                    total = 0;
                                }
                            }
                            else
                            {
                                total = 0;
                            }

                            //var CardPoints = _ILoyaltyAppService.ConvertCustomerPriceToPoint(total, user.TenantId);
                            //var loyaltyModel = _ILoyaltyAppService.GetAll();
                            //long loyalityDefId = _ILoyaltyAppService.GetAll().Id;
                            //var DateNow = Convert.ToDateTime(DateTime.Now.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                            //var DateStart = Convert.ToDateTime(loyaltyModel.StartDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                            //var DateEnd = Convert.ToDateTime(loyaltyModel.EndDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));

                            //if (loyaltyModel.IsLoyalityPoint && loyalityDefId != 0 && loyaltyModel.OrderType.Contains(((int)order.OrderType).ToString())&& DateStart <= DateNow && DateEnd >= DateNow)
                            //{
                            //    contactLoyalty.Points = order.TotalPoints;
                            //    contactLoyalty.CreditPoints = CardPoints;
                            //    contactLoyalty.ContactId = int.Parse(user.ContactID);
                            //    contactLoyalty.OrderId = order.Id;
                            //    contactLoyalty.LoyaltyDefinitionId = loyalityDefId;
                            //    contactLoyalty.CreatedBy = (long)AbpSession.UserId;
                            //    contactLoyalty.TransactionTypeId = 2;
                            //    _ILoyaltyAppService.CreateContactLoyaltyTransaction(contactLoyalty);
                            //}

                            if (order.TotalPoints > 0 || order.TotalCreditPoints > 0)
                            {    // [dbo].[ContactLoyaltyTransactionAdd]
                                _ILoyaltyAppService.UpdateContactLoyaltyTransaction(new ContactLoyaltyTransactionModel
                                {
                                    OrderId = order.Id,
                                    ContactId = order.ContactId.Value,
                                    CreatedBy = (long)AbpSession.UserId,
                                    TransactionTypeId = (int)LoyaltyTransactionType.DeleteCancelOrder,
                                });

                            }

                            transactionScope.Complete();
                        }
                    }

                    var captionDeleteOrderText = GetCaptionFormat("BackEnd_Text_DeleteOrder", "ar-JO", order.TenantId, "", "");//تم الغاء الطلب من قبل المطعم  ...

                    if (order.OrderLocal == "ar" || order.OrderLocal == null)
                    {
                        captionDeleteOrderText = GetCaptionFormat("BackEnd_Text_DeleteOrder", "ar-JO", order.TenantId, "", "");//تم الغاء الطلب من قبل المطعم  ...
                    }
                    else
                    {
                        captionDeleteOrderText = "the Order has been canceled ";// GetCaptionFormat("BackEnd_Text_DeleteOrder", "ar-JO", order.TenantId, "", "");//تم الغاء الطلب من قبل المطعم  ...

                    }

                    SunshinePostMsgBotModel.Content message = new SunshinePostMsgBotModel.Content
                    {
                        text = captionDeleteOrderText,
                        type = "text",
                        agentName = order.LockByAgentName,
                        agentId = order.AgentId.ToString()
                    };

                    PostWhatsAppMessageModel postWhatsAppMessageModel = new PostWhatsAppMessageModel();
                    postWhatsAppMessageModel.type = "text";
                    postWhatsAppMessageModel.text = new PostWhatsAppMessageModel.Text();
                    postWhatsAppMessageModel.text.body = captionDeleteOrderText;
                    postWhatsAppMessageModel.to = user.phoneNumber;



                    var result = await new WhatsAppAppService().postToFB(postWhatsAppMessageModel, tenant.D360Key, tenant.AccessToken, tenant.IsD360Dialog);
                    if (result)
                    {
                        var CustomerChat = UpdateCustomerChat(user.TenantId, message, user.userId, user.SunshineConversationId);
                        user.customerChat = CustomerChat;
                        SocketIOManager.SendContact(user, (int)user.TenantId);
                    }




                    GetOrderForViewDto getOrderForViewDto = new GetOrderForViewDto();
                    var GetOrderMap = ObjectMapper.Map(order, getOrderForViewDto.Order);
                    GetOrderMap.StringTotal = stringTotall;
                    getOrderForViewDto.Order = GetOrderMap;
                    getOrderForViewDto.OrderStatusName = orderStatusName;
                    getOrderForViewDto.OrderTypeName = orderTypeName;
                    getOrderForViewDto.CustomerCustomerName = user.displayName;
                    getOrderForViewDto.TenantId = AbpSession.TenantId;
                    getOrderForViewDto.ActionTime = DateTime.Now.AddHours(AppSettingsModel.AddHour).ToString("hh:mm tt");
                    if (area != null)
                    {
                        getOrderForViewDto.AreahName = area.AreaName;
                    }

                    SocketIOManager.SendOrder(getOrderForViewDto, order.TenantId);
                    return true;

                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<bool> DoneOrder(EntityDto<long> input, string stringTotall, int agentId, string agentName)
        {
            try
            {


                if (getOrderStatusValidation(input.Id, (int)OrderStatusEunm.Done))
                {

                    var order = getOrderById(input.Id);
                    var tenant = _generalAppService.GetTenantById(order.TenantId);
                    var user = GetCustomerAzuer(order.ContactId.ToString());
                    var area = _areasAppService.GetAreaById((int)order.AreaId, order.TenantId);
                    var orderStatusName = Enum.GetName(typeof(OrderStatusEunm), (int)OrderStatusEunm.Done);
                    var orderTypeName = Enum.GetName(typeof(OrderTypeEunm), (int)order.OrderType);
                    var setting = _areasAppService.GetMenuSetting(order.AreaId.Value);
                    string captionDoneOrderText = "طلبكم قيد التجهيز ...";

                    using (TransactionScope transactionScope = new TransactionScope())
                    {
                        if (!string.IsNullOrEmpty(tenant.AccessToken))
                        {

                            order.OrderStatus = OrderStatusEunm.Done;
                            order.OrderRemarks = "";
                            order.IsLockByAgent = true;
                            order.AgentId = agentId;
                            order.LockByAgentName = agentName;
                            order.ActionTime = DateTime.UtcNow;

                            bool isValid = createOrderStatusHistory(input.Id, (int)OrderStatusEunm.Done);
                            updateOrderStatus(order);
                            _dashboardUIAppService.CreateDashboardNumber(new DashboardNumbers
                            {
                                TenantId = order.TenantId,
                                TypeId = (int)DashboardTypeEnum.Order,
                                StatusId = (int)OrderStatusEunm.Done,
                                StatusName = orderStatusName
                            });
                            ContactLoyaltyTransactionModel contactLoyalty = new ContactLoyaltyTransactionModel();
                            decimal total = 0;
                            if (order.Total > 0)
                            {
                                decimal DeliveryCost = order.DeliveryCost.HasValue ? order.DeliveryCost.Value : 0;
                                total = order.Total - DeliveryCost;
                                if (total < 0)
                                {
                                    total = 0;
                                }
                            }
                            else
                            {
                                total = 0;
                            }
                            var loyaltyModel = _ILoyaltyAppService.GetAll();
                            //var CardPoints = _ILoyaltyAppService.ConvertCustomerPriceToPoint(total, order.TenantId);
                            //long loyalityDefId = _ILoyaltyAppService.GetAll(50).Id;

                            //var DateNow = Convert.ToDateTime(DateTime.Now.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                            //var DateStart = Convert.ToDateTime(loyaltyModel.StartDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                            //var DateEnd = Convert.ToDateTime(loyaltyModel.EndDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));

                            //if (loyaltyModel.IsLoyalityPoint && loyalityDefId != 0 && loyaltyModel.OrderType.Contains(((int)order.OrderType).ToString())&& DateStart <= DateNow && DateEnd >= DateNow)
                            //{
                            //    contactLoyalty.CreditPoints = order.TotalPoints;
                            //    contactLoyalty.Points = CardPoints;
                            //    contactLoyalty.ContactId = (int)order.ContactId;
                            //    contactLoyalty.OrderId = order.Id;
                            //    contactLoyalty.LoyaltyDefinitionId = loyalityDefId;
                            //    contactLoyalty.CreatedBy = (long)AbpSession.UserId;
                            //    contactLoyalty.TransactionTypeId = 1;
                            //    _ILoyaltyAppService.CreateContactLoyaltyTransaction(contactLoyalty);
                            //}
                            //var x = 0;
                            //var r = 3 / x;

                            if (order.TotalCreditPoints > 0 || order.TotalPoints > 0)
                            {    // [dbo].[ContactLoyaltyTransactionAdd]
                                _ILoyaltyAppService.CreateContactLoyaltyTransaction(new ContactLoyaltyTransactionModel
                                {
                                    OrderId = order.Id,
                                    LoyaltyDefinitionId = loyaltyModel.Id,
                                    ContactId = order.ContactId.Value,
                                    Points = order.TotalPoints,
                                    CreditPoints = order.TotalCreditPoints,
                                    CreatedBy = (long)AbpSession.UserId,
                                    Year = DateTime.UtcNow.Year,
                                    TransactionTypeId = (int)LoyaltyTransactionType.MakeOrder,
                                }); ; ;

                            }

                        }
                        transactionScope.Complete();
                    }

                    if (setting != null)
                    {
                        if ((order.OrderLocal == "ar" || order.OrderLocal == null) && setting.WorkTextAR != null)
                        {
                            captionDoneOrderText = setting.WorkTextAR;
                        }
                        if (order.OrderLocal == "en" && setting.WorkTextEN != null)
                        {
                            captionDoneOrderText = setting.WorkTextEN;
                        }
                    }

                    SunshinePostMsgBotModel.Content message = new SunshinePostMsgBotModel.Content
                    {
                        text = captionDoneOrderText,
                        type = "text",
                        agentName = order.LockByAgentName,
                        agentId = order.AgentId.ToString()
                    };

                    PostWhatsAppMessageModel postWhatsAppMessageModel = new PostWhatsAppMessageModel();
                    postWhatsAppMessageModel.type = "text";
                    postWhatsAppMessageModel.to = user.phoneNumber;

                    postWhatsAppMessageModel.text = new PostWhatsAppMessageModel.Text();
                    postWhatsAppMessageModel.text.body = captionDoneOrderText;



                    var result = await new WhatsAppAppService().postToFB(postWhatsAppMessageModel, tenant.D360Key, tenant.AccessToken, tenant.IsD360Dialog);
                    if (result)
                    {
                        var CustomerChat = UpdateCustomerChat(user.TenantId, message, user.userId, user.SunshineConversationId);
                        user.customerChat = CustomerChat;
                        SocketIOManager.SendContact(user, (int)user.TenantId);
                    }



                    GetOrderForViewDto getOrderForViewDto = new GetOrderForViewDto();
                    var GetOrderMap = ObjectMapper.Map(order, getOrderForViewDto.Order);
                    GetOrderMap.StringTotal = stringTotall;
                    getOrderForViewDto.Order = GetOrderMap;
                    getOrderForViewDto.OrderStatusName = orderStatusName;
                    getOrderForViewDto.OrderTypeName = orderTypeName;
                    getOrderForViewDto.CustomerCustomerName = user.displayName;
                    getOrderForViewDto.TenantId = AbpSession.TenantId;
                    getOrderForViewDto.ActionTime = DateTime.Now.AddHours(AppSettingsModel.AddHour).ToString("hh:mm tt");

                    if (area != null)
                    {
                        getOrderForViewDto.AreahName = area.AreaName;
                    }
                    SocketIOManager.SendOrder(getOrderForViewDto, order.TenantId);
                    return true;

                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        //public async Task<bool> AcceptOrder(EntityDto<long> input, string stringTotall, int agentId, string agentName)
        //{
        //    try
        //    {
        //        if (getOrderStatusValidation(input.Id, (int)ZeedlyOrderStatus.Accepted))
        //        {

        //            var order = getOrderById(input.Id);
        //            var tenant = _generalAppService.GetTenantById(order.TenantId);
        //            var user = GetCustomerAzuer(order.ContactId.ToString());
        //            var area = _areasAppService.GetAreaById((int)order.AreaId, order.TenantId);
        //            var orderStatusName = Enum.GetName(typeof(ZeedlyOrderStatus), (int)ZeedlyOrderStatus.Accepted);
        //            var orderTypeName = Enum.GetName(typeof(ZeedlyOrderStatus), (int)order.OrderType);
        //            var setting = _areasAppService.GetMenuSetting(order.AreaId.Value);
        //            string captionDoneOrderText = "طلبكم قيد التجهيز ...";

        //            using (TransactionScope transactionScope = new TransactionScope())
        //            {
        //                if (!string.IsNullOrEmpty(tenant.AccessToken))
        //                {

        //                    //order.ZeedlyOrderStatus = ZeedlyOrderStatus.Accepted;
        //                    order.OrderRemarks = "";
        //                    order.IsLockByAgent = true;
        //                    order.AgentId = agentId;
        //                    order.LockByAgentName = agentName;
        //                    order.ActionTime = DateTime.UtcNow;

        //                    bool isValid = createOrderStatusZeedlyHistory(input.Id, (int)ZeedlyOrderStatus.Accepted);
        //                    updateOrderStatus(order);
        //                    _dashboardUIAppService.CreateDashboardNumber(new DashboardNumbers
        //                    {
        //                        TenantId = order.TenantId,
        //                        TypeId = (int)DashboardTypeEnum.Order,
        //                        StatusId = (int)ZeedlyOrderStatus.Accepted,
        //                        StatusName = orderStatusName
        //                    });
        //                    ContactLoyaltyTransactionModel contactLoyalty = new ContactLoyaltyTransactionModel();
        //                    decimal total = 0;
        //                    if (order.Total > 0)
        //                    {
        //                        decimal DeliveryCost = order.DeliveryCost.HasValue ? order.DeliveryCost.Value : 0;
        //                        total = order.Total - DeliveryCost;
        //                        if (total < 0)
        //                        {
        //                            total = 0;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        total = 0;
        //                    }
        //                    var loyaltyModel = _ILoyaltyAppService.GetAll();
        //                    //var CardPoints = _ILoyaltyAppService.ConvertCustomerPriceToPoint(total, order.TenantId);
        //                    //long loyalityDefId = _ILoyaltyAppService.GetAll(50).Id;

        //                    //var DateNow = Convert.ToDateTime(DateTime.Now.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
        //                    //var DateStart = Convert.ToDateTime(loyaltyModel.StartDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
        //                    //var DateEnd = Convert.ToDateTime(loyaltyModel.EndDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));

        //                    //if (loyaltyModel.IsLoyalityPoint && loyalityDefId != 0 && loyaltyModel.OrderType.Contains(((int)order.OrderType).ToString())&& DateStart <= DateNow && DateEnd >= DateNow)
        //                    //{
        //                    //    contactLoyalty.CreditPoints = order.TotalPoints;
        //                    //    contactLoyalty.Points = CardPoints;
        //                    //    contactLoyalty.ContactId = (int)order.ContactId;
        //                    //    contactLoyalty.OrderId = order.Id;
        //                    //    contactLoyalty.LoyaltyDefinitionId = loyalityDefId;
        //                    //    contactLoyalty.CreatedBy = (long)AbpSession.UserId;
        //                    //    contactLoyalty.TransactionTypeId = 1;
        //                    //    _ILoyaltyAppService.CreateContactLoyaltyTransaction(contactLoyalty);
        //                    //}
        //                    //var x = 0;
        //                    //var r = 3 / x;

        //                    if (order.TotalCreditPoints > 0 || order.TotalPoints > 0)
        //                    {    // [dbo].[ContactLoyaltyTransactionAdd]
        //                        _ILoyaltyAppService.CreateContactLoyaltyTransaction(new ContactLoyaltyTransactionModel
        //                        {
        //                            OrderId = order.Id,
        //                            LoyaltyDefinitionId = loyaltyModel.Id,
        //                            ContactId = order.ContactId.Value,
        //                            Points = order.TotalPoints,
        //                            CreditPoints = order.TotalCreditPoints,
        //                            CreatedBy = (long)AbpSession.UserId,
        //                            Year = DateTime.UtcNow.Year,
        //                            TransactionTypeId = (int)LoyaltyTransactionType.MakeOrder,
        //                        }); ; ;

        //                    }

        //                }
        //                transactionScope.Complete();
        //            }

        //            if (setting != null)
        //            {
        //                if ((order.OrderLocal == "ar" || order.OrderLocal == null) && setting.WorkTextAR != null)
        //                {
        //                    captionDoneOrderText = setting.WorkTextAR;
        //                }
        //                if (order.OrderLocal == "en" && setting.WorkTextEN != null)
        //                {
        //                    captionDoneOrderText = setting.WorkTextEN;
        //                }
        //            }

        //            SunshinePostMsgBotModel.Content message = new SunshinePostMsgBotModel.Content
        //            {
        //                text = captionDoneOrderText,
        //                type = "text",
        //                agentName = order.LockByAgentName,
        //                agentId = order.AgentId.ToString()
        //            };

        //            PostWhatsAppMessageModel postWhatsAppMessageModel = new PostWhatsAppMessageModel();
        //            postWhatsAppMessageModel.type = "text";
        //            postWhatsAppMessageModel.to = user.phoneNumber;

        //            postWhatsAppMessageModel.text = new PostWhatsAppMessageModel.Text();
        //            postWhatsAppMessageModel.text.body = captionDoneOrderText;



        //            var result = await new WhatsAppAppService().postToFB(postWhatsAppMessageModel, tenant.D360Key, tenant.AccessToken, tenant.IsD360Dialog);
        //            if (result)
        //            {
        //                var CustomerChat = UpdateCustomerChat(user.TenantId, message, user.userId, user.SunshineConversationId);
        //                user.customerChat = CustomerChat;
        //                SocketIOManager.SendContact(user, (int)user.TenantId);
        //            }



        //            GetOrderForViewDto getOrderForViewDto = new GetOrderForViewDto();
        //            var GetOrderMap = ObjectMapper.Map(order, getOrderForViewDto.Order);
        //            GetOrderMap.StringTotal = stringTotall;
        //            getOrderForViewDto.Order = GetOrderMap;
        //            getOrderForViewDto.OrderStatusName = orderStatusName;
        //            getOrderForViewDto.OrderTypeName = orderTypeName;
        //            getOrderForViewDto.CustomerCustomerName = user.displayName;
        //            getOrderForViewDto.TenantId = AbpSession.TenantId;
        //            getOrderForViewDto.ActionTime = DateTime.Now.AddHours(AppSettingsModel.AddHour).ToString("hh:mm tt");

        //            if (area != null)
        //            {
        //                getOrderForViewDto.AreahName = area.AreaName;
        //            }
        //            SocketIOManager.SendOrder(getOrderForViewDto, order.TenantId);
        //            return true;

        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }
        //}
      
        public async Task CloseOrder(EntityDto<long> input, string stringTotall)
        {
            var order = await _orderRepository.FirstOrDefaultAsync((long)input.Id);
            //order.orderStatus = OrderStatusEunm.Delete;
            order.OrderRemarks = "CancelByAgent";
            await _orderRepository.UpdateAsync(order);

            var area = _lookup_areaRepository.GetAll().Where(x => x.Id == order.AreaId).FirstOrDefault();



            var orderStatusName = Enum.GetName(typeof(OrderStatusEunm), order.orderStatus);
            var orderTypeName = Enum.GetName(typeof(OrderTypeEunm), order.OrderType);


            GetOrderForViewDto getOrderForViewDto = new GetOrderForViewDto();
            var GetOrderMap = ObjectMapper.Map(order, getOrderForViewDto.Order);
            GetOrderMap.StringTotal = stringTotall;
            getOrderForViewDto.Order = GetOrderMap;
            getOrderForViewDto.OrderStatusName = orderStatusName;
            getOrderForViewDto.OrderTypeName = orderTypeName;
            getOrderForViewDto.TenantId = AbpSession.TenantId;
            getOrderForViewDto.ActionTime = order.ActionTime?.ToString("hh:mm tt");
            if (area != null)
            {
                getOrderForViewDto.AreahName = area.AreaName + " (" + area.AreaCoordinate.ToString() + ")";
            }

            //  await _hub.Clients.All.SendAsync("brodCastAgentOrder", getOrderForViewDto);
            SocketIOManager.SendOrder(getOrderForViewDto, order.TenantId.Value);

        }
        public async Task DeleteForEver(EntityDto<long> input)
        {
            var order = await _orderRepository.FirstOrDefaultAsync((long)input.Id);



            var orderDetailsDrft = GetOrderDetail2(AbpSession.TenantId, order.Id);

            foreach (var orderDetai in orderDetailsDrft)
            {
                var GetOrderDetailExtraDraft = GetOrderDetailExtra(AbpSession.TenantId, orderDetai.Id);

                foreach (var ExtraOrde in GetOrderDetailExtraDraft)
                {

                    DeleteExtraOrderDetail(ExtraOrde.Id);
                }


                DeleteOrderDetails(orderDetai.Id);
            }



            DeleteOrder(order.Id);

        }
        public async Task DeleteAllForEver()
        {
            var contactList = _lookup_customerRepository.GetAll().ToList();
            var orderDrift = GetOrderList(AbpSession.TenantId).Result;



            foreach (var cont in contactList)
            {

                var orderDrift2 = orderDrift.Where(x => x.ContactId == cont.Id).ToList();


                foreach (var order in orderDrift2)
                {
                    var orderDetailsDrft = GetOrderDetail2(AbpSession.TenantId, order.Id);

                    foreach (var orderDetai in orderDetailsDrft)
                    {
                        var GetOrderDetailExtraDraft = GetOrderDetailExtra(AbpSession.TenantId, orderDetai.Id);

                        foreach (var ExtraOrde in GetOrderDetailExtraDraft)
                        {

                            DeleteExtraOrderDetail(ExtraOrde.Id);
                        }


                        DeleteOrderDetails(orderDetai.Id);
                    }



                    DeleteOrder(order.Id);
                }

            }




        }
        public async Task<int> GetPendingCount()
        {

            var filteredOrders = _orderRepository.GetAll().ToList();

            var count = filteredOrders.Where(x => x.orderStatus == OrderStatusEunm.Pending).ToList().Count();
            var count2 = filteredOrders.Where(x => x.orderStatus == OrderStatusEunm.Pre_Order).ToList().Count();


            return count + count2;
        }



        public OrderDto GetOrderExtraDetails(int TenantID, long ContactId)
        {
            return getOrderExtraDetails(TenantID, ContactId);
        }
        public long UpdateOrder(string order, long orderid, int tenantID)
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

        public OrderSoket UpdateOrderSoket(string order, long orderid, int tenantID)
        {
            try
            {
                OrderSoket orderDto = new OrderSoket();
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

                orderDto = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.OrderSoket, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                return orderDto;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public string updateOrderZeedlyStatus(long orderNumber, int tenantId, int zeedlyStatus) 
        {
            try
            {
                string connectionString = AppSettingsModel.ConnectionStrings;
                using (var connection = new SqlConnection(connectionString))
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "dbo.UpdateOrderZeedlyStatus";

                        command.Parameters.AddWithValue("@TenantId", tenantId);
                        command.Parameters.AddWithValue("@OrderNumber", orderNumber);
                        command.Parameters.AddWithValue("@ZeedlyOrderStatus", zeedlyStatus);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    return "order zeedly status updated successfully";
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public void updateOrderETA(long orderNumber, int tenantId, string eta)
        {
            try
            {
                string connectionString = AppSettingsModel.ConnectionStrings;
                using (var connection = new SqlConnection(connectionString))
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "dbo.updateOrderETA";

                        command.Parameters.AddWithValue("@TenantId", tenantId);
                        command.Parameters.AddWithValue("@OrderNumber", orderNumber);
                        command.Parameters.AddWithValue("@ETA", eta);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        #region check apis
        //public int GetuserArea(int? UserID)
        //{

        //    string connString = AppSettingsModel.ConnectionStrings;
        //    string query = "select * from [dbo].[AbpUsers] where Id=" + UserID;

        //    SqlConnection conn = new SqlConnection(connString);
        //    SqlCommand cmd = new SqlCommand(query, conn);
        //    conn.Open();

        //    // create the DataSet 
        //    DataSet dataSet = new DataSet();

        //    // create data adapter
        //    SqlDataAdapter da = new SqlDataAdapter(cmd);
        //    // this will query your database and return the result to your datatable
        //    da.Fill(dataSet);

        //    List<Area> branches = new List<Area>();



        //    int Id = 0;
        //    for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
        //    {
        //        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["AreaId"]);

        //    }

        //    conn.Close();
        //    da.Dispose();

        //    return Id;
        //}
        //private CustomerChat GetLastMessage(string userId)
        //{
        //    if (string.IsNullOrEmpty(userId))
        //        return null;
        //    var CustomerChatCollection = new DocumentCosmoseDB<CustomerChat>(CollectionTypes.ItemsCollection, _IDocumentClient);
        //    var chatConversation = CustomerChatCollection.GetItemsAsync(a => a.ItemType == InfoSeedContainerItemTypes.ConversationItem && a.userId == userId, null, int.MaxValue, 1).Result;

        //    return chatConversation.Item1.LastOrDefault();
        //}

        //private async Task<List<CustomerModel>> GetCustomers(int? tenantId)
        //{

        //    var pageNumber = 0;
        //    var pageSize = int.MaxValue;
        //    var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
        //    var customers = await itemsCollection.GetItemsAsync(a => a.TenantId == tenantId && a.ItemType == InfoSeedContainerItemTypes.CustomerItem, null, pageSize, pageNumber);

        //    return customers.Item1.ToList();
        //}
        #endregion
        #region private

        private CustomerChat UpdateCustomerChat(int? tenantId, SunshinePostMsgBotModel.Content model, string userId, string conversationID)
        {
            var itemsCollection = new DocumentCosmoseDB<CustomerChat>(CollectionTypes.ItemsCollection, _IDocumentClient);

            // Create your new conversation instance
            var CustomerChat = new CustomerChat()
            {
                messageId = Guid.NewGuid().ToString(),
                userId = userId,
                SunshineConversationId = conversationID,
                text = model.text,
                type = model.type,
                fileName = model.fileName,
                CreateDate = DateTime.Now,
                status = 1,
                sender = Tenants.Contacts.MessageSenderType.TeamInbox,
                mediaUrl = model.mediaUrl,
                agentName = model.agentName,
                agentId = model.agentId,
                TenantId = tenantId,

            };
            var result = itemsCollection.CreateItemAsync(CustomerChat).Result;

            return CustomerChat;
        }

        private List<ExtraOrderDetailsDto> GetOrderDetailExtra(int? TenantID, long? OrderDetailId)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[ExtraOrderDetail] where TenantID=" + TenantID + " and OrderDetailId=" + OrderDetailId;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<ExtraOrderDetailsDto> orderDetailDtos = new List<ExtraOrderDetailsDto>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                orderDetailDtos.Add(new ExtraOrderDetailsDto
                {
                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                    OrderDetailId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["OrderDetailId"]),
                    TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
                    Quantity = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Quantity"]),
                    Name = dataSet.Tables[0].Rows[i]["Name"].ToString(),
                    NameEnglish = dataSet.Tables[0].Rows[i]["NameEnglish"].ToString(),
                    Total = decimal.Parse(dataSet.Tables[0].Rows[i]["Total"].ToString()),
                    UnitPrice = decimal.Parse(dataSet.Tables[0].Rows[i]["UnitPrice"].ToString()),
                    SpecificationUniqueId = int.Parse(dataSet.Tables[0].Rows[i]["SpecificationUniqueId"].ToString()),
                    SpecificationName = dataSet.Tables[0].Rows[i]["SpecificationName"].ToString(),
                    SpecificationNameEnglish = dataSet.Tables[0].Rows[i]["SpecificationNameEnglish"].ToString(),



                });
            }

            conn.Close();
            da.Dispose();

            return orderDetailDtos;
        }
        private List<CategoryExtraOrderDetailsDto> GetMenuOrderDetailExtraForDetails(int? TenantID, long? OrderDetailId)
        {

            List<ExtraOrderDetailsDto> orderDetailDtos = new List<ExtraOrderDetailsDto>();

            try
            {
                var SP_Name = Constants.Order.SP_OrderMenuDetailExtraDetailsGet;

                // var SP_Name = "[dbo].[OrderMenuDetailExtraDetailsGet]";
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                 new System.Data.SqlClient.SqlParameter("@OrderDetailId",OrderDetailId),
                   new System.Data.SqlClient.SqlParameter("@TenantID",TenantID)
                };
                orderDetailDtos = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),
                   DataReaderMapper.MapMenuOrderExtraDetails, AppSettingsModel.ConnectionStrings).ToList();


                List<CategoryExtraOrderDetailsDto> lstCategoryExtraOrderDetailsDto = new List<CategoryExtraOrderDetailsDto>();

                var obj = orderDetailDtos.GroupBy(x => x.SpecificationUniqueId).ToList();

                foreach (var item in obj)
                {

                    CategoryExtraOrderDetailsDto categoryExtraOrderDetailsDto = new CategoryExtraOrderDetailsDto();
                    categoryExtraOrderDetailsDto.SpecificationName = item.FirstOrDefault().SpecificationName;
                    categoryExtraOrderDetailsDto.SpecificationNameEnglish = item.FirstOrDefault().SpecificationNameEnglish;
                    categoryExtraOrderDetailsDto.SpecificationUniqueId = item.FirstOrDefault().SpecificationUniqueId;
                    categoryExtraOrderDetailsDto.lstExtraOrderDetailsDto = item.ToList();
                    lstCategoryExtraOrderDetailsDto.Add(categoryExtraOrderDetailsDto);
                }



                return lstCategoryExtraOrderDetailsDto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // done with SP
        //private List<CategoryExtraOrderDetailsDto> GetOrderDetailExtraForDetails(int? TenantID, long? OrderDetailId)
        //{
        //    string connString = AppSettingsModel.ConnectionStrings;
        //    string query = "select * from [dbo].[ExtraOrderDetail] where TenantID=" + TenantID + " and OrderDetailId=" + OrderDetailId;


        //    SqlConnection conn = new SqlConnection(connString);
        //    SqlCommand cmd = new SqlCommand(query, conn);
        //    conn.Open();

        //    // create the DataSet 
        //    DataSet dataSet = new DataSet();

        //    // create data adapter
        //    SqlDataAdapter da = new SqlDataAdapter(cmd);
        //    // this will query your database and return the result to your datatable
        //    da.Fill(dataSet);

        //    List<ExtraOrderDetailsDto> orderDetailDtos = new List<ExtraOrderDetailsDto>();

        //    for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
        //    {

        //        orderDetailDtos.Add(new ExtraOrderDetailsDto
        //        {
        //            Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
        //            OrderDetailId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["OrderDetailId"]),
        //            TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
        //            Quantity = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Quantity"]),
        //            Name = dataSet.Tables[0].Rows[i]["Name"].ToString(),
        //            Total = decimal.Parse(dataSet.Tables[0].Rows[i]["Total"].ToString()),
        //            UnitPrice = decimal.Parse(dataSet.Tables[0].Rows[i]["UnitPrice"].ToString()),
        //            SpecificationUniqueId = int.Parse(dataSet.Tables[0].Rows[i]["SpecificationUniqueId"].ToString()),
        //            SpecificationName = dataSet.Tables[0].Rows[i]["SpecificationName"].ToString(),
        //            SpecificationNameEnglish = dataSet.Tables[0].Rows[i]["SpecificationNameEnglish"].ToString(),
        //            // UnitPoints= decimal.Parse(dataSet.Tables[0].Rows[i]["UnitPoints"].ToString()),
        //            // TotalLoyaltyPoints= decimal.Parse(dataSet.Tables[0].Rows[i]["TotalLoyaltyPoints"].ToString()),
        //            //TypeExtraDetails = int.Parse(dataSet.Tables[0].Rows[i]["TypeExtraDetails"].ToString()),
        //            //SpecificationId = int.Parse(dataSet.Tables[0].Rows[i]["SpecificationId"].ToString()),
        //            //SpecificationChoiceId = int.Parse(dataSet.Tables[0].Rows[i]["SpecificationChoiceId"].ToString()),



        //        });
        //    }

        //    conn.Close();
        //    da.Dispose();

        //    List<CategoryExtraOrderDetailsDto> lstCategoryExtraOrderDetailsDto = new List<CategoryExtraOrderDetailsDto>();

        //    var obj  = orderDetailDtos.GroupBy(x => x.SpecificationUniqueId).ToList();


        //    foreach (var item in obj)
        //    {

        //        CategoryExtraOrderDetailsDto categoryExtraOrderDetailsDto = new CategoryExtraOrderDetailsDto();
        //        categoryExtraOrderDetailsDto.SpecificationName = item.FirstOrDefault().SpecificationName;
        //        categoryExtraOrderDetailsDto.SpecificationNameEnglish = item.FirstOrDefault().SpecificationNameEnglish;
        //        categoryExtraOrderDetailsDto.SpecificationUniqueId = item.FirstOrDefault().SpecificationUniqueId;
        //        categoryExtraOrderDetailsDto.lstExtraOrderDetailsDto = item.ToList();
        //        lstCategoryExtraOrderDetailsDto.Add(categoryExtraOrderDetailsDto);
        //    }



        //    return lstCategoryExtraOrderDetailsDto;
        //}


        //private Contact GetCustomer(int? CustomerId)
        //{
        //    string connString = AppSettingsModel.ConnectionStrings;
        //    string query = "select * from [dbo].[Contacts] where Id=" + CustomerId;


        //    SqlConnection conn = new SqlConnection(connString);
        //    SqlCommand cmd = new SqlCommand(query, conn);
        //    conn.Open();

        //    // create the DataSet 
        //    DataSet dataSet = new DataSet();

        //    // create data adapter
        //    SqlDataAdapter da = new SqlDataAdapter(cmd);
        //    // this will query your database and return the result to your datatable
        //    da.Fill(dataSet);

        //    Contact contact = new Contact();

        //    for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
        //    {

        //        contact = new Contact
        //        {
        //            Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
        //            UserId = dataSet.Tables[0].Rows[i]["UserId"].ToString(),
        //            TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
        //            PhoneNumber = dataSet.Tables[0].Rows[i]["PhoneNumber"].ToString()
        //        };
        //    }

        //    conn.Close();
        //    da.Dispose();

        //    return contact;
        //}
        private CustomerModel GetCustomerAzuer(string ContactID)
        {
            string result = string.Empty;
            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.ContactID == ContactID);
            if (customerResult.IsCompletedSuccessfully)
            {
                return customerResult.Result;

            }


            return null;
        }
        //private async Task<TenantModel> GetTenantByAppId(string id)
        //{
        //    if (string.IsNullOrEmpty(id))
        //        return null;
        //    var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);

        //    TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.SmoochAppID == id);
        //    return tenant;
        //}

        private async Task<TenantModel> GetTenantById()
        {

            var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);

            TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.TenantId == (int)AbpSession.TenantId);
            return tenant;
        }



        private List<LocationInfoModelDto> GetAllLocationInfoModel()
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

                List<LocationInfoModelDto> locationInfoModel = new List<LocationInfoModelDto>();

                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {

                    locationInfoModel.Add(new LocationInfoModelDto
                    {
                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                        LocationName = dataSet.Tables[0].Rows[i]["LocationName"].ToString(),
                        LocationNameEn = dataSet.Tables[0].Rows[i]["LocationNameEn"].ToString().TrimEnd(),

                    });
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
        private async Task<List<Order>> GetOrderList(int? TenantID)
        {

            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[Orders] where TenantID=" + TenantID;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<Order> order = new List<Order>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                order.Add(new Order
                {
                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                    OrderNumber = Convert.ToInt32(dataSet.Tables[0].Rows[i]["OrderNumber"]),
                    Total = decimal.Parse(dataSet.Tables[0].Rows[i]["Total"].ToString()),
                    ContactId = int.Parse(dataSet.Tables[0].Rows[i]["ContactId"].ToString()),
                    CreationTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["CreationTime"].ToString()),
                    OrderTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["OrderTime"].ToString()),
                    orderStatus = (OrderStatusEunm)Enum.Parse(typeof(OrderStatusEunm), dataSet.Tables[0].Rows[i]["orderStatus"].ToString(), true)
                });



            }

            conn.Close();
            da.Dispose();

            return order;

        }
        private List<OrderDetailDto> GetOrderDetail2(int? TenantID, long? OrderId)
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

            List<OrderDetailDto> orderDetailDtos = new List<OrderDetailDto>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                orderDetailDtos.Add(new OrderDetailDto
                {
                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                    Discount = decimal.Parse(dataSet.Tables[0].Rows[i]["Discount"].ToString()),
                    ItemId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemId"]),
                    OrderId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["OrderId"]),
                    Quantity = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Quantity"]),
                    //TotalAfterDiscunt = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TotalAfterDiscunt"]),
                    Total = decimal.Parse(dataSet.Tables[0].Rows[i]["Total"].ToString()),
                    UnitPrice = decimal.Parse(dataSet.Tables[0].Rows[i]["UnitPrice"].ToString()),



                });
            }

            conn.Close();
            da.Dispose();

            return orderDetailDtos;
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
        private string GetCaptionFormat(string key, string lang, int? TenantID, string parm1, string parm2)
        {
            var langg = GetLanguageBot(TenantID, lang);
            var testR = GetTextResource(TenantID, key);
            var caption = GetAllCaption(TenantID, langg.Id, testR.Id);

            if (caption != null && testR.Category == "Welcome_Text")
            {
                var text = string.Format(caption.Text, parm1.Trim());

                return text.Replace("\\r\\n", "\r\n");
            }
            if (caption != null && testR.Category == "PlaceOrder_Text")
            {
                var text = string.Format(caption.Text, TenantID.ToString(), parm1.Trim());

                return text.Replace("\\r\\n", "\r\n");
            }

            if (caption != null && testR.Category == "CancelOrder_SeccesMassege")
            {
                var text = string.Format(caption.Text, parm1.ToString(), parm2.Trim());

                return text.Replace("\\r\\n", "\r\n");
            }

            if (caption != null && testR.Category == "NextBranch_Text")
            {
                var text = string.Format(caption.Text, parm1.ToString());

                return text.Replace("\\r\\n", "\r\n");
            }
            if (caption != null && testR.Category == "Select_Text")
            {
                var text = string.Format(caption.Text, parm1.ToString());

                return text.Replace("\\r\\n", "\r\n");
            }

            if (caption != null)
            {

                return caption.Text.Replace("\\r\\n", "\r\n");
            }

            return "";
        }
        private Caption GetAllCaption(int? TenantID, int langId, int testRId)
        {

            try
            {
                string connString = AppSettingsModel.ConnectionStrings;
                string query = "select * from [dbo].[Caption] where TenantID=" + TenantID + "and LanguageBotId =  " + langId + " and TextResourceId=" + testRId;


                SqlConnection conn = new SqlConnection(connString);
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                // create the DataSet 
                DataSet dataSet = new DataSet();

                // create data adapter
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                // this will query your database and return the result to your datatable
                da.Fill(dataSet);

                Caption captions = new Caption();

                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {

                    captions = new Caption
                    {
                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                        Text = dataSet.Tables[0].Rows[i]["Text"].ToString(),
                        TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
                        LanguageBotId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["LanguageBotId"]),
                        TextResourceId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TextResourceId"]),

                    };
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
        private LanguageBot GetLanguageBot(int? TenantID, string lang)
        {

            try
            {
                string connString = AppSettingsModel.ConnectionStrings;
                string query = "select * from [dbo].[LanguageBot] where  ISO ='" + lang + "'";


                SqlConnection conn = new SqlConnection(connString);
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                // create the DataSet 
                DataSet dataSet = new DataSet();

                // create data adapter
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                // this will query your database and return the result to your datatable
                da.Fill(dataSet);

                LanguageBot languageBots = new LanguageBot();

                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {

                    languageBots = new LanguageBot
                    {
                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                        ISO = dataSet.Tables[0].Rows[i]["ISO"].ToString(),
                        Name = dataSet.Tables[0].Rows[i]["Name"].ToString(),
                        TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),

                    };
                }

                conn.Close();
                da.Dispose();

                return languageBots;

            }
            catch
            {
                return null;

            }

        }
        private TextResource GetTextResource(int? TenantID, string key)
        {

            try
            {
                string connString = AppSettingsModel.ConnectionStrings;
                string query = "select * from [dbo].[TextResource] where Category = '" + key + "'";


                SqlConnection conn = new SqlConnection(connString);
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                // create the DataSet 
                DataSet dataSet = new DataSet();

                // create data adapter
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                // this will query your database and return the result to your datatable
                da.Fill(dataSet);

                TextResource textResources = new TextResource();

                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {

                    textResources = new TextResource
                    {
                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                        Key = dataSet.Tables[0].Rows[i]["Key"].ToString(),
                        Category = dataSet.Tables[0].Rows[i]["Category"].ToString(),
                        TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),

                    };
                }

                conn.Close();
                da.Dispose();

                return textResources;

            }
            catch (Exception )
            {
                return null;

            }

        }

        public List<Area> GetAreas(string TenantID)
        {
            //TenantID = "31";
            var tenantID = int.Parse(TenantID);
            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[Areas] where TenantID=" + tenantID;


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

                branches.Add(new Area
                {
                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                    AreaCoordinate = dataSet.Tables[0].Rows[i]["AreaCoordinate"].ToString(),
                    AreaName = dataSet.Tables[0].Rows[i]["AreaName"].ToString(),
                    TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
                    BranchID = dataSet.Tables[0].Rows[i]["BranchID"].ToString(),
                    UserId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["UserId"]),



                });
            }

            conn.Close();
            da.Dispose();

            return branches;
        }


        public Item GetItemID(long? id)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[Items] where id= " + id;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            Item itemDtos = new Item();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                try
                {
                    itemDtos = new Item
                    {
                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                        //CategoryNames = dataSet.Tables[0].Rows[i]["CategoryNames"].ToString(),
                        //CategoryNamesEnglish = dataSet.Tables[0].Rows[i]["CategoryNamesEnglish"].ToString(),
                        //CreationTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["CreationTime"]),
                        //DeletionTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["DeletionTime"]),
                        ImageUri = dataSet.Tables[0].Rows[i]["ImageUri"].ToString(),
                        //Ingredients = dataSet.Tables[0].Rows[i]["Ingredients"].ToString(),
                        //IsInService = bool.Parse(dataSet.Tables[0].Rows[i]["IsInService"].ToString()),
                        //ItemDescription = dataSet.Tables[0].Rows[i]["ItemDescription"].ToString(),
                        //ItemDescriptionEnglish = dataSet.Tables[0].Rows[i]["ItemDescriptionEnglish"].ToString(),
                        //ItemName = dataSet.Tables[0].Rows[i]["ItemName"].ToString(),
                        //ItemNameEnglish = dataSet.Tables[0].Rows[i]["ItemNameEnglish"].ToString(),
                        ////LastModificationTime = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Priority"]),
                        //Price = decimal.Parse(dataSet.Tables[0].Rows[i]["Price"].ToString()),
                        //Priority = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Priority"]),
                        //SKU = dataSet.Tables[0].Rows[i]["SKU"].ToString(),
                        //MenuType = Convert.ToInt32(dataSet.Tables[0].Rows[i]["MenuType"]),
                        Barcode = dataSet.Tables[0].Rows[i]["Barcode"].ToString() ?? "",
                        BarcodeImg = dataSet.Tables[0].Rows[i]["BarcodeImg"].ToString()


                    };
                }
                catch (Exception )
                {

                }

            }

            conn.Close();
            da.Dispose();

            return itemDtos;

        }


        private List<GetOrderDetailForViewDto> getOrderDetails(long orderId)
        {
            List<GetOrderDetailForViewDto> lstGetOrderDetailForViewDto = new List<GetOrderDetailForViewDto>();

            try
            {

                var SP_Name = "[dbo].[OrderDetailGet]";


                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
             new System.Data.SqlClient.SqlParameter("@OrderId",orderId)

            };


                lstGetOrderDetailForViewDto = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),
                 DataReaderMapper.MapOrderDetail, AppSettingsModel.ConnectionStrings).ToList();

                return lstGetOrderDetailForViewDto;
            }
            catch (Exception ex)
            {
                throw ex;
            }





        }

        private OrderDto getOrderExtraDetails(int tenantID, long contactId)
        {
            OrderDto orderDto = new OrderDto();

            try
            {

                var SP_Name = Constants.Order.SP_OrderExtraDetailsGet;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
             new System.Data.SqlClient.SqlParameter("@TenantID",tenantID)
            ,new System.Data.SqlClient.SqlParameter("@ContactId",contactId)

            };
                orderDto = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),
                DataReaderMapper.MapOrderExtraDetails, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                return orderDto;
            }
            catch (Exception ex)
            {
                throw ex;
            }





        }

        private OrderStatusHistoryDto getOrderStatusHistory(long orderId)
        {
            try
            {
                OrderStatusHistoryDto orderStatus = new OrderStatusHistoryDto();
                var SP_Name = Constants.Order.SP_OrderStatusHistoryGet;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                     new System.Data.SqlClient.SqlParameter("@OrderId",orderId)
                };

                System.Data.SqlClient.SqlParameter OutsqlParameter = new System.Data.SqlClient.SqlParameter();
                OutsqlParameter.ParameterName = "@DoneCount";
                OutsqlParameter.SqlDbType = SqlDbType.Int;
                OutsqlParameter.Direction = ParameterDirection.Output;
                sqlParameters.Add(OutsqlParameter);

                System.Data.SqlClient.SqlParameter OutsqlParameter2 = new System.Data.SqlClient.SqlParameter();
                OutsqlParameter2.ParameterName = "@CancelOrDeleteCount";
                OutsqlParameter2.SqlDbType = SqlDbType.Int;
                OutsqlParameter2.Direction = ParameterDirection.Output;
                sqlParameters.Add(OutsqlParameter2);

                SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapOrderStatusHistory, AppSettingsModel.ConnectionStrings).FirstOrDefault();
                orderStatus.DoneCount = Convert.ToInt32(OutsqlParameter.Value);
                orderStatus.CancelOrDeleteCount = Convert.ToInt32(OutsqlParameter2.Value);
                orderStatus.OrderId = orderId;

                return orderStatus;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private OrderEntity GetALLOrdersByContactId(int contactId, int? tenantId, int? pageNumber = 0, int? pageSize = 10)
        {

            try
            {
                OrderEntity _OrderEntity = new OrderEntity();

                List<GetOrderForViewDto> lstOrder = new List<GetOrderForViewDto>();
                var SP_Name = Constants.Order.SP_ContactOrdersGet;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                     new System.Data.SqlClient.SqlParameter("@PageNumber",pageNumber)
                    ,new System.Data.SqlClient.SqlParameter("@PageSize",pageSize)
                    ,new System.Data.SqlClient.SqlParameter("@TenantId",tenantId)
                    ,new System.Data.SqlClient.SqlParameter("@ContactId",contactId)
                };

                System.Data.SqlClient.SqlParameter OutsqlParameter = new System.Data.SqlClient.SqlParameter();
                OutsqlParameter.ParameterName = "@TotalCount";
                OutsqlParameter.SqlDbType = SqlDbType.Int;
                OutsqlParameter.Direction = ParameterDirection.Output;

                sqlParameters.Add(OutsqlParameter);
                lstOrder = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),
                 DataReaderMapper.MapOrders, AppSettingsModel.ConnectionStrings).ToList();
                _OrderEntity.lstOrder = lstOrder;

                _OrderEntity.TotalCount = (int)OutsqlParameter.Value;
                return _OrderEntity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private OrderEntity FGetAllLoyaltyRemainingdays(int contactId, int? tenantId, int? pageNumber = 0, int? pageSize = 10)
        {

            try
            {
                OrderEntity _OrderEntity = new OrderEntity();

                List<GetOrderForViewDto> lstOrder = new List<GetOrderForViewDto>();
                var SP_Name = Constants.Order.SP_LoyaltyRemainingdays;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                     new System.Data.SqlClient.SqlParameter("@PageNumber",pageNumber)
                    ,new System.Data.SqlClient.SqlParameter("@PageSize",pageSize)
                    ,new System.Data.SqlClient.SqlParameter("@TenantId",tenantId)
                    ,new System.Data.SqlClient.SqlParameter("@ContactId",contactId)
                };

                //System.Data.SqlClient.SqlParameter OutsqlParameter = new System.Data.SqlClient.SqlParameter();
                //OutsqlParameter.ParameterName = "@TotalCount";
                //OutsqlParameter.SqlDbType = SqlDbType.Int;
                //OutsqlParameter.Direction = ParameterDirection.Output;

                //sqlParameters.Add(OutsqlParameter);
                lstOrder = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),DataReaderMapper.MapLoyaltyRemainingdays, AppSettingsModel.ConnectionStrings).ToList();
                _OrderEntity.lstOrder = lstOrder;

                //_OrderEntity.TotalCount = (int)OutsqlParameter.Value;
                return _OrderEntity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private List<GetOrderForViewDto> GetALLOrders(int pageNumber, int pageSize, bool isArchived, out int totalCount, int? orderStatus, string sorting, string PhoneNumber = null, long? abpUserId = null)
        {

            try
            {
                List<GetOrderForViewDto> lstGetOrderForViewDto = new List<GetOrderForViewDto>();
                var SP_Name = Constants.Order.SP_OrdersGet;

                if (isArchived)
                {
                    SP_Name = Constants.Order.SP_OrdersArchivedGet;
                }
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                     new System.Data.SqlClient.SqlParameter("@PageNumber",pageNumber)
                    ,new System.Data.SqlClient.SqlParameter("@PageSize",pageSize)
                    ,new System.Data.SqlClient.SqlParameter("@TenantId",AbpSession.TenantId)
                    ,new System.Data.SqlClient.SqlParameter("@OrderStatus",orderStatus)

                    ,new System.Data.SqlClient.SqlParameter("@Sorting",sorting)
                    ,new System.Data.SqlClient.SqlParameter("@PhoneNumber",PhoneNumber)
                    ,new System.Data.SqlClient.SqlParameter("@AbpUserId",abpUserId),
                };



                System.Data.SqlClient.SqlParameter OutsqlParameter = new System.Data.SqlClient.SqlParameter();
                OutsqlParameter.ParameterName = "@TotalCount";
                OutsqlParameter.SqlDbType = System.Data.SqlDbType.Int;
                OutsqlParameter.Direction = System.Data.ParameterDirection.Output;

                sqlParameters.Add(OutsqlParameter);
                lstGetOrderForViewDto = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapOrders, AppSettingsModel.ConnectionStrings).ToList();
                totalCount = (int)OutsqlParameter.Value;
                return lstGetOrderForViewDto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<long> createNewOrder(string orderJson)
        {

            try
            {

                var SP_Name = Constants.Order.SP_OrderAdd;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                new System.Data.SqlClient.SqlParameter("@OrderJson",orderJson)

            };

                var OutputParameter = new System.Data.SqlClient.SqlParameter();
                OutputParameter.SqlDbType = System.Data.SqlDbType.BigInt;
                OutputParameter.ParameterName = "@OrderId";
                OutputParameter.Direction = System.Data.ParameterDirection.Output;


                sqlParameters.Add(OutputParameter);


                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(),
             AppSettingsModel.ConnectionStrings);

                return (long)OutputParameter.Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }





        }

        private async Task<long> postNewOrder(string orderJson)
        {

            try
            {

                var SP_Name = Constants.Order.SP_OrderDetailsExtraDetailsAdd;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
             new System.Data.SqlClient.SqlParameter("@OrderJson",orderJson)

            };

                var OutputParameter = new System.Data.SqlClient.SqlParameter();
                OutputParameter.SqlDbType = System.Data.SqlDbType.BigInt;
                OutputParameter.ParameterName = "@OrderId";
                OutputParameter.Direction = System.Data.ParameterDirection.Output;


                sqlParameters.Add(OutputParameter);


                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(),
                AppSettingsModel.ConnectionStrings);

                return (long)OutputParameter.Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }





        }

        private async Task<long> createOrderDetails(string orderJson)
        {

            try
            {

                var SP_Name = Constants.Order.SP_OrderDetailsAdd;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
             new System.Data.SqlClient.SqlParameter("@OrderDetailJson",orderJson)

            };

                var OutputParameter = new System.Data.SqlClient.SqlParameter();
                OutputParameter.SqlDbType = System.Data.SqlDbType.BigInt;
                OutputParameter.ParameterName = "@OrderDetailId";
                OutputParameter.Direction = System.Data.ParameterDirection.Output;


                sqlParameters.Add(OutputParameter);


                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(),
                     AppSettingsModel.ConnectionStrings);

                return (long)OutputParameter.Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }





        }



        private async Task<long> createOrderDetailsExtra(string orderJson)
        {

            try
            {



                var SP_Name = Constants.Order.SP_OrderDetailsExtraAdd;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
             new System.Data.SqlClient.SqlParameter("@OrderDetailExtraJson",orderJson)

            };

                var OutputParameter = new System.Data.SqlClient.SqlParameter();
                OutputParameter.SqlDbType = System.Data.SqlDbType.BigInt;
                OutputParameter.ParameterName = "@OrderDetailExtraId";
                OutputParameter.Direction = System.Data.ParameterDirection.Output;


                sqlParameters.Add(OutputParameter);


                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(),
                  AppSettingsModel.ConnectionStrings);

                return (long)OutputParameter.Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }





        }

        private async Task<long> createOrderDetailsSpecifications(string orderDetailsSpecificationsJson)

        {

            try
            {



                var SP_Name = Constants.Order.SP_OrderDetailsSpecificationAdd;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
             new System.Data.SqlClient.SqlParameter("@OrderDetailSpecificationJson ",orderDetailsSpecificationsJson)

            };

                var OutputParameter = new System.Data.SqlClient.SqlParameter();
                OutputParameter.SqlDbType = System.Data.SqlDbType.BigInt;
                OutputParameter.ParameterName = "@OrderDetailSpecificationId";
                OutputParameter.Direction = System.Data.ParameterDirection.Output;


                sqlParameters.Add(OutputParameter);


                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(),
                 AppSettingsModel.ConnectionStrings);

                return (long)OutputParameter.Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }





        }






        private OrderDto getOrderDetailsForBot(long orderId, int lang, string resourceIds, bool isOrderOffer, long areaId)
        {



            OrderDto orderDto = new OrderDto();

            try
            {

             var SP_Name = Constants.Order.SP_OrderDetailsExtraDetailsGet;
             var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
             new System.Data.SqlClient.SqlParameter("@OrderId",orderId),
             new System.Data.SqlClient.SqlParameter("@Lang",lang),
             new System.Data.SqlClient.SqlParameter("@ResourceIds",resourceIds),
             new System.Data.SqlClient.SqlParameter("@IsOrderOffer",isOrderOffer),
             new System.Data.SqlClient.SqlParameter("@AreaId",areaId)

            };
                orderDto = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),
                DataReaderMapper.MapOrderDetialsExtraDetails, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                return orderDto;
            }
            catch (Exception ex)
            {
                throw ex;
            }






        }



        #endregion






        //private void createContactLoyaltyTransaction(ContactLoyaltyTransactionModel contactLoyalty)
        //{
        //    try
        //    {
        //        int year = DateTime.UtcNow.Year;
        //        var SP_Name = Constants.Loyalty.SP_ContactLoyaltyTransactionAdd;

        //        var sqlParameters = new List<System.Data.SqlClient.SqlParameter>{
        //             new System.Data.SqlClient.SqlParameter("@ContactId",contactLoyalty.ContactId)
        //            ,new System.Data.SqlClient.SqlParameter("@Points",contactLoyalty.Points)
        //            ,new System.Data.SqlClient.SqlParameter("@OrderId",contactLoyalty.OrderId)
        //           // ,new SqlParameter("@CardPoints",contactLoyalty.CardPoints)
        //            ,new System.Data.SqlClient.SqlParameter("@CreatedBy",AbpSession.UserId)
        //            ,new System.Data.SqlClient.SqlParameter("@CreatedDate",DateTime.UtcNow)
        //            ,new System.Data.SqlClient.SqlParameter("@LoyaltyDefinitionId",contactLoyalty.LoyaltyDefinitionId)
        //            ,new System.Data.SqlClient.SqlParameter("@Year",year)
        //        };
        //        SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }
        //}
    }
}