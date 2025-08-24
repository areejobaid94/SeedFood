using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Infoseed.MessagingPortal.CaptionBot;
using Infoseed.MessagingPortal.ExtraOrderDetails;
using Infoseed.MessagingPortal.ExtraOrderDetails.Dtos;
using Infoseed.MessagingPortal.Items;
using Infoseed.MessagingPortal.Items.Dtos;
using Infoseed.MessagingPortal.OrderDetails.Dtos;
using Infoseed.MessagingPortal.Orders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.OrderDetails
{
    //[AbpAuthorize(AppPermissions.Pages_OrderDetails)]
    public class OrderDetailsAppService : MessagingPortalAppServiceBase, IOrderDetailsAppService
    {
		 private readonly IRepository<OrderDetail, long> _orderDetailRepository;
		 private readonly IRepository<Order,long> _lookup_orderRepository;
		 private readonly IRepository<Item, long> _lookup_ItemRepository;
        private readonly IRepository<ExtraOrderDetail, long> _extraOrderDetailRepository;

        public OrderDetailsAppService(IRepository<OrderDetail, long> orderDetailRepository , IRepository<Order, long> lookup_orderRepository, IRepository<Item, long> lookup_ItemRepository, IRepository<ExtraOrderDetail, long> extraOrderDetailRepository) 
		  {
			_orderDetailRepository = orderDetailRepository;
			_lookup_orderRepository = lookup_orderRepository;
            _lookup_ItemRepository = lookup_ItemRepository;
            _extraOrderDetailRepository = extraOrderDetailRepository;


          }

		 public async Task<PagedResultDto<GetOrderDetailForViewDto>> GetAll(GetAllOrderDetailsInput input)
         {
			
			var filteredOrderDetails = _orderDetailRepository.GetAll()
						.Include( e => e.OrderFk)
						.Include( e => e.ItemsFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false )
						.WhereIf(input.MinQuantityFilter != null, e => e.Quantity >= input.MinQuantityFilter)
						.WhereIf(input.MaxQuantityFilter != null, e => e.Quantity <= input.MaxQuantityFilter)
						.WhereIf(input.MinUnitPriceFilter != null, e => e.UnitPrice >= input.MinUnitPriceFilter)
						.WhereIf(input.MaxUnitPriceFilter != null, e => e.UnitPrice <= input.MaxUnitPriceFilter)
						.WhereIf(input.MinTotalFilter != null, e => e.Total >= input.MinTotalFilter)
						.WhereIf(input.MaxTotalFilter != null, e => e.Total <= input.MaxTotalFilter)
						.WhereIf(input.MinDiscountFilter != null, e => e.Discount >= input.MinDiscountFilter)
						.WhereIf(input.MaxDiscountFilter != null, e => e.Discount <= input.MaxDiscountFilter)
						.WhereIf(input.MinTotalAfterDiscuntFilter != null, e => e.TotalAfterDiscunt >= input.MinTotalAfterDiscuntFilter)
						.WhereIf(input.MaxTotalAfterDiscuntFilter != null, e => e.TotalAfterDiscunt <= input.MaxTotalAfterDiscuntFilter)
						.WhereIf(input.MinTaxFilter != null, e => e.Tax >= input.MinTaxFilter)
						.WhereIf(input.MaxTaxFilter != null, e => e.Tax <= input.MaxTaxFilter)
						.WhereIf(input.MinTotalAfterTaxFilter != null, e => e.TotalAfterTax >= input.MinTotalAfterTaxFilter)
						.WhereIf(input.MaxTotalAfterTaxFilter != null, e => e.TotalAfterTax <= input.MaxTotalAfterTaxFilter)
						.WhereIf(input.MinCreationTimeFilter != null, e => e.CreationTime >= input.MinCreationTimeFilter)
						.WhereIf(input.MaxCreationTimeFilter != null, e => e.CreationTime <= input.MaxCreationTimeFilter)
						.WhereIf(input.MinDeletionTimeFilter != null, e => e.DeletionTime >= input.MinDeletionTimeFilter)
						.WhereIf(input.MaxDeletionTimeFilter != null, e => e.DeletionTime <= input.MaxDeletionTimeFilter)
						.WhereIf(input.MinLastModificationTimeFilter != null, e => e.LastModificationTime >= input.MinLastModificationTimeFilter)
						.WhereIf(input.MaxLastModificationTimeFilter != null, e => e.LastModificationTime <= input.MaxLastModificationTimeFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.OrderOrderRemarksFilter), e => e.OrderFk != null && e.OrderFk.OrderRemarks == input.OrderOrderRemarksFilter)
                         .WhereIf(!string.IsNullOrWhiteSpace(input.MenuMenuNameFilter), e => e.ItemsFk != null && e.ItemsFk.ItemName  == input.MenuMenuNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.MenuMenuNameFilter), e => e.ItemsFk != null && e.ItemsFk.ItemDescriptionEnglish == input.MenuMenuNameFilter);

			var pagedAndFilteredOrderDetails = filteredOrderDetails
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var orderDetails = from o in pagedAndFilteredOrderDetails
                         join o1 in _lookup_orderRepository.GetAll() on o.OrderId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         join o2 in _lookup_ItemRepository.GetAll() on o.ItemId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()
                         
                         select new GetOrderDetailForViewDto() {
							OrderDetail = new OrderDetailDto
							{
                                Quantity = o.Quantity,
                                UnitPrice = o.UnitPrice,
                                Total = o.Total,
                                Discount = o.Discount,
                                TotalAfterDiscunt = o.TotalAfterDiscunt,
                                Tax = o.Tax,
                                TotalAfterTax = o.TotalAfterTax,
                                CreationTime = o.CreationTime,
                                DeletionTime = o.DeletionTime,
                                LastModificationTime = o.LastModificationTime,
                                Id = o.Id
							},
                             OrderOrderRemarks = s1 == null || s1.OrderRemarks == null ? "" : s1.OrderRemarks.ToString(),
                              ItemName  = s2 == null || s2.ItemName  == null ? "" : s2.ItemName .ToString(),
                              ItemNameEnglish = s2 == null || s2.ItemNameEnglish == null ? "" : s2.ItemNameEnglish.ToString(),
                             SKU = s2 == null || s2.SKU == null ? "" : s2.SKU,
                         };

            var totalCount = await filteredOrderDetails.CountAsync();

            return new PagedResultDto<GetOrderDetailForViewDto>(
                totalCount,
                await orderDetails.ToListAsync()
            );
         }
		 
		 public async Task<GetOrderDetailForViewDto> GetOrderDetailForView(long id)
         {
            var orderDetail = await _orderDetailRepository.GetAsync(id);

            var output = new GetOrderDetailForViewDto { OrderDetail = ObjectMapper.Map<OrderDetailDto>(orderDetail) };

            if (output.OrderDetail.OrderId != null)
            {
                var _lookupOrder = await _lookup_orderRepository.FirstOrDefaultAsync((long)output.OrderDetail.OrderId);
                output.OrderOrderRemarks = _lookupOrder?.OrderRemarks?.ToString();
            }

            if (output.OrderDetail.ItemId != null)
            {
                var _lookupMenu = await _lookup_ItemRepository.FirstOrDefaultAsync((long)output.OrderDetail.ItemId);
                output.ItemName  = _lookupMenu?.ItemName ?.ToString();
                output.ItemNameEnglish = _lookupMenu?.ItemNameEnglish?.ToString();
            }
			
            return output;
         }
		 
		 //[AbpAuthorize(AppPermissions.Pages_OrderDetails_Edit)]
		 public async Task<GetOrderDetailForEditOutput> GetOrderDetailForEdit(EntityDto<long> input)
         {
            var orderDetail = await _orderDetailRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetOrderDetailForEditOutput {OrderDetail = ObjectMapper.Map<CreateOrEditOrderDetailDto>(orderDetail)};

            if (output.OrderDetail.OrderId != null)
            {
                var _lookupOrder = await _lookup_orderRepository.FirstOrDefaultAsync((long)output.OrderDetail.OrderId);
                output.OrderOrderRemarks = _lookupOrder?.OrderRemarks?.ToString();
            }

            if (output.OrderDetail.itemId != null)
            {
                var _lookupMenu = await _lookup_ItemRepository.FirstOrDefaultAsync((long)output.OrderDetail.itemId);
                output.ItemName  = _lookupMenu?.ItemName ?.ToString();
                output.ItemNameEnglish = _lookupMenu?.ItemNameEnglish?.ToString();
            }
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditOrderDetailDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 //[AbpAuthorize(AppPermissions.Pages_OrderDetails_Create)]
		 protected virtual async Task Create(CreateOrEditOrderDetailDto input)
         {
            var orderDetail = ObjectMapper.Map<OrderDetail>(input);

			
			if (AbpSession.TenantId != null)
			{
				orderDetail.TenantId = (int?) AbpSession.TenantId;
			}
		

            await _orderDetailRepository.InsertAsync(orderDetail);
         }

		 //[AbpAuthorize(AppPermissions.Pages_OrderDetails_Edit)]
		 protected virtual async Task Update(CreateOrEditOrderDetailDto input)
         {
            var orderDetail = await _orderDetailRepository.FirstOrDefaultAsync((long)input.Id);
             ObjectMapper.Map(input, orderDetail);
         }

		 //[AbpAuthorize(AppPermissions.Pages_OrderDetails_Delete)]
         public async Task Delete(EntityDto<long> input)
         {
            await _orderDetailRepository.DeleteAsync(input.Id);
         } 

		//[AbpAuthorize(AppPermissions.Pages_OrderDetails)]
         public async Task<PagedResultDto<OrderDetailOrderLookupTableDto>> GetAllOrderForLookupTable(Dtos.GetAllForLookupTableInput input)
         {
            var query = _lookup_orderRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.OrderRemarks != null && e.OrderRemarks.Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var orderList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<OrderDetailOrderLookupTableDto>();
            foreach (var order in orderList)
            {
                lookupTableDtoList.Add(new OrderDetailOrderLookupTableDto
                {
                    Id = order.Id,
                    DisplayName = order.OrderRemarks?.ToString()
                });
            }

            return new PagedResultDto<OrderDetailOrderLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );


        }

		//[AbpAuthorize(AppPermissions.Pages_OrderDetails)]
         public async Task<PagedResultDto<OrderDetailMenuLookupTableDto>> GetAllMenuForLookupTable(Dtos.GetAllForLookupTableInput input)
         {
             var query = _lookup_ItemRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.ItemName  != null && e.ItemName .Contains(input.Filter)

                );

            var totalCount = await query.CountAsync();

            var menuList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<OrderDetailMenuLookupTableDto>();
			foreach(var menu in menuList){
				lookupTableDtoList.Add(new OrderDetailMenuLookupTableDto
				{
					Id = menu.Id,
					 DisplayName  = menu.ItemName ?.ToString(),
                     DisplayNameEnglish = menu.ItemName ?.ToString()
                });
			}

            return new PagedResultDto<OrderDetailMenuLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }



        public async Task<PagedResultDto<GetOrderDetailForViewDto>> GetAllDetail(int orderId)
        {
            GetAllOrderDetailsInput getAllOrderDetailsInput = new GetAllOrderDetailsInput();
            var filteredOrderDetails = _orderDetailRepository.GetAll()
                        .Include(e => e.OrderFk)
                        .Include(e => e.ItemsFk);

            var pagedAndFilteredOrderDetails = filteredOrderDetails
                        .OrderBy(getAllOrderDetailsInput.Sorting ?? "id asc")
                        .PageBy(getAllOrderDetailsInput);

            var orderDetails = from o in pagedAndFilteredOrderDetails
                               join o1 in _lookup_orderRepository.GetAll() on o.OrderId equals o1.Id into j1
                               from s1 in j1.DefaultIfEmpty()

                               join o2 in _lookup_ItemRepository.GetAll() on o.ItemId equals o2.Id into j2
                               from s2 in j2.DefaultIfEmpty()

                               select new GetOrderDetailForViewDto()
                               {
                                   OrderDetail = new OrderDetailDto
                                   {
                                       Quantity = o.Quantity,
                                       UnitPrice = o.UnitPrice,
                                       Total = o.UnitPrice * o.Quantity,
                                       Discount = o.Discount,
                                       TotalAfterDiscunt = o.TotalAfterDiscunt,
                                       Tax = o.Tax,
                                       TotalAfterTax = o.TotalAfterTax,
                                       CreationTime = o.CreationTime,
                                       DeletionTime = o.DeletionTime,
                                       LastModificationTime = o.LastModificationTime,
                                        OrderId= o.OrderId,
                                         ItemId= o.ItemId,
                                       Id = o.Id
                                   },
                                   OrderOrderRemarks = s1 == null || s1.OrderRemarks == null ? "" : s1.OrderRemarks.ToString(),
                                    ItemName  = s2 == null || s2.ItemName  == null ? "" : s2.ItemName .ToString(),
                                    ItemNameEnglish = s2 == null || s2.ItemNameEnglish == null ? "" : s2.ItemNameEnglish.ToString(),
                                   SKU = s2 == null || s2.SKU == null ? "" : s2.SKU,
                                   OrderStringToPrint= GetOrderDetailString(AbpSession.TenantId, int.Parse(o.OrderId.ToString()))
                               };


            var orderDetailsList = orderDetails.Where(e => e.OrderDetail.OrderId == orderId);

            

            foreach (var item in orderDetailsList)
            {
                var ex = _extraOrderDetailRepository.GetAll().Where(x => x.OrderDetailId == item.OrderDetail.Id).ToList();

              

                foreach(var itemex in ex)
                {
                    item.OrderDetail.extraOrderDetailsDtos.Add(new ExtraOrderDetails.Dtos.ExtraOrderDetailsDto
                    {
                         Id=0,
                          Name= itemex.Name,
                           OrderDetailId= itemex.OrderDetailId,
                            Quantity= itemex.Quantity,
                             TenantId= itemex.TenantId,
                              Total= itemex.Total,
                               UnitPrice= itemex.UnitPrice,

                    });

                }
                    

                

            }
            


            var totalCount = await orderDetailsList.CountAsync();

            return new PagedResultDto<GetOrderDetailForViewDto>(
                totalCount,
                await orderDetailsList.ToListAsync()
            );
        }


        public string GetOrderDetailString(int? TenantID, int? OrderId)
        {

            var ord = GetOrderS(TenantID, OrderId);

            var captionQuantityText = GetCaptionFormat("BackEnd_Text_Quantity", "ar-JO", TenantID, "", "", 0);//العدد :
            var captionAddtionText = GetCaptionFormat("BackEnd_Text_Addtion", "ar-JO", TenantID, "", "", 0);//الاضافات
            var captionTotalText = GetCaptionFormat("BackEnd_Text_Total", "ar-JO", TenantID, "", "", 0);//المجموع:       
            var captionTotalOfAllText = GetCaptionFormat("BackEnd_Text_TotalOfAll", "ar-JO", TenantID, "", "", 0);//السعر الكلي للصنف: 

            var OrderDetailList = GetOrderDetail(TenantID, OrderId);
            var listString = "-------------------------- \r\n\r\n";
            decimal? total = 0;

            foreach (var OrderD in OrderDetailList)
            {
                var getOrderDetailExtra = GetOrderDetailExtra(TenantID, OrderD.Id);


                var item = GetItem(TenantID, OrderD.ItemId);
                listString = listString + item.ItemName  + "\r\n";
                listString = listString + captionQuantityText + OrderD.Quantity + "\r\n";


                if (getOrderDetailExtra.Count > 0)
                {
                    listString = listString + captionAddtionText + "\r\n";

                }

                foreach (var ext in getOrderDetailExtra)
                {

                    listString = listString + ext.Name + "\r\n";
                    listString = listString + captionQuantityText + ext.Quantity + "\r\n";

                }



                listString = listString + captionTotalOfAllText + OrderD.Total + "\r\n\r\n";




                total = total + OrderD.Total;
            }
            listString = listString + "-------------------------- \r\n\r\n";
            listString = listString + captionTotalText + ord.Total;

            return listString;
        }
        private List<OrderDetailDto> GetOrderDetail(int? TenantID, int? OrderId)
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
                    Total = decimal.Parse(dataSet.Tables[0].Rows[i]["Total"].ToString()),
                    UnitPrice = decimal.Parse(dataSet.Tables[0].Rows[i]["UnitPrice"].ToString()),



                });
            }

            conn.Close();
            da.Dispose();

            return orderDetailDtos;
        }
        private Order GetOrderS(int? TenantID, int? OrderId)
        {

            //var x = GetContactId("962779746365", "28");


            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[Orders] where TenantID=" + TenantID + "and id=" + OrderId;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            Order order = new Order();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                var IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString());
                if (!IsDeleted)
                {

                    order = new Order
                    {
                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                        OrderNumber = Convert.ToInt32(dataSet.Tables[0].Rows[i]["OrderNumber"]),
                        Total = decimal.Parse(dataSet.Tables[0].Rows[i]["Total"].ToString()),
                        ContactId = int.Parse(dataSet.Tables[0].Rows[i]["ContactId"].ToString())

                    };

                }

            }

            conn.Close();
            da.Dispose();

            return order;

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
                    Total = decimal.Parse(dataSet.Tables[0].Rows[i]["Total"].ToString()),
                    UnitPrice = decimal.Parse(dataSet.Tables[0].Rows[i]["UnitPrice"].ToString()),



                });
            }

            conn.Close();
            da.Dispose();

            return orderDetailDtos;
        }
        private ItemDto GetItem(int? TenantID, long? itemID)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[Items] where TenantID=" + TenantID + "and id=" + itemID;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            ItemDto itemDtos = new ItemDto();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                var IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString());

                if (!IsDeleted)
                {
                    itemDtos = new ItemDto
                    {
                        ItemCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemCategoryId"]),
                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                        CategoryNames  = dataSet.Tables[0].Rows[i]["CategoryNames "].ToString(),
                         CategoryNamesEnglish = dataSet.Tables[0].Rows[i]["CategoryNamesEnglish"].ToString(),
                        IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString()),
                        ImageUri = dataSet.Tables[0].Rows[i]["ImageUri"].ToString(),
                        IsInService = bool.Parse(dataSet.Tables[0].Rows[i]["IsInService"].ToString()),
                         ItemDescription  = dataSet.Tables[0].Rows[i]["ItemDescription "].ToString(),
                         ItemDescriptionEnglish = dataSet.Tables[0].Rows[i]["ItemDescriptionEnglish"].ToString(),
                        ItemName  = dataSet.Tables[0].Rows[i]["ItemName "].ToString(),
                         ItemNameEnglish = dataSet.Tables[0].Rows[i]["ItemNameEnglish"].ToString(),
                        Price = decimal.Parse(dataSet.Tables[0].Rows[i]["Price"].ToString()),
                        Priority = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Priority"])

                    };

                }

            }

            conn.Close();
            da.Dispose();

            return itemDtos;

        }
        private string GetCaptionFormat(string key, string lang, int? TenantID, string parm1, string parm2, int parm3)
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
                var text = string.Format(caption.Text, TenantID.ToString(), parm1.Trim(), parm2.Trim(), parm3);

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
                string query = "select * from [dbo].[LanguageBot] where ISO ='" + lang + "'";


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


    }
}