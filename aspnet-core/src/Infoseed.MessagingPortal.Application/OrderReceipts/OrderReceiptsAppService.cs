using Infoseed.MessagingPortal.Orders;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infoseed.MessagingPortal.OrderReceipts.Dtos;
using Infoseed.MessagingPortal.Dto;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Infoseed.MessagingPortal.OrderReceipts
{
	//[AbpAuthorize(AppPermissions.Pages_OrderReceipts)]
    public class OrderReceiptsAppService : MessagingPortalAppServiceBase, IOrderReceiptsAppService
    {
		 private readonly IRepository<OrderReceipt, long> _orderReceiptRepository;
		 private readonly IRepository<Order,long> _lookup_orderRepository;
		 

		  public OrderReceiptsAppService(IRepository<OrderReceipt, long> orderReceiptRepository , IRepository<Order, long> lookup_orderRepository) 
		  {
			_orderReceiptRepository = orderReceiptRepository;
			_lookup_orderRepository = lookup_orderRepository;
		
		  }

		 public async Task<PagedResultDto<GetOrderReceiptForViewDto>> GetAll(GetAllOrderReceiptsInput input)
         {
			
			var filteredOrderReceipts = _orderReceiptRepository.GetAll()
						.Include( e => e.OrderFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false )
						.WhereIf(input.MinOrderTimeFilter != null, e => e.OrderTime >= input.MinOrderTimeFilter)
						.WhereIf(input.MaxOrderTimeFilter != null, e => e.OrderTime <= input.MaxOrderTimeFilter)
						.WhereIf(input.MinOrderAmountFilter != null, e => e.OrderAmount >= input.MinOrderAmountFilter)
						.WhereIf(input.MaxOrderAmountFilter != null, e => e.OrderAmount <= input.MaxOrderAmountFilter)
						.WhereIf(input.MinOrderDiscountFilter != null, e => e.OrderDiscount >= input.MinOrderDiscountFilter)
						.WhereIf(input.MaxOrderDiscountFilter != null, e => e.OrderDiscount <= input.MaxOrderDiscountFilter)
						.WhereIf(input.MinTotalAfterDiscuntFilter != null, e => e.TotalAfterDiscunt >= input.MinTotalAfterDiscuntFilter)
						.WhereIf(input.MaxTotalAfterDiscuntFilter != null, e => e.TotalAfterDiscunt <= input.MaxTotalAfterDiscuntFilter)
						.WhereIf(input.IsCashReceivedFilter.HasValue && input.IsCashReceivedFilter > -1,  e => (input.IsCashReceivedFilter == 1 && e.IsCashReceived) || (input.IsCashReceivedFilter == 0 && !e.IsCashReceived) )
						.WhereIf(input.MinCreationTimeFilter != null, e => e.CreationTime >= input.MinCreationTimeFilter)
						.WhereIf(input.MaxCreationTimeFilter != null, e => e.CreationTime <= input.MaxCreationTimeFilter)
						.WhereIf(input.MinDeletionTimeFilter != null, e => e.DeletionTime >= input.MinDeletionTimeFilter)
						.WhereIf(input.MaxDeletionTimeFilter != null, e => e.DeletionTime <= input.MaxDeletionTimeFilter)
						.WhereIf(input.MinLastModificationTimeFilter != null, e => e.LastModificationTime >= input.MinLastModificationTimeFilter)
						.WhereIf(input.MaxLastModificationTimeFilter != null, e => e.LastModificationTime <= input.MaxLastModificationTimeFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.OrderOrderRemarksFilter), e => e.OrderFk != null && e.OrderFk.OrderRemarks == input.OrderOrderRemarksFilter);

			var pagedAndFilteredOrderReceipts = filteredOrderReceipts
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var orderReceipts = from o in pagedAndFilteredOrderReceipts
                         join o1 in _lookup_orderRepository.GetAll() on o.OrderId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         select new GetOrderReceiptForViewDto() {
							OrderReceipt = new OrderReceiptDto
							{
                                OrderTime = o.OrderTime,
                                OrderAmount = o.OrderAmount,
                                OrderDiscount = o.OrderDiscount,
                                TotalAfterDiscunt = o.TotalAfterDiscunt,
                                IsCashReceived = o.IsCashReceived,
                                CreationTime = o.CreationTime,
                                DeletionTime = o.DeletionTime,
                                LastModificationTime = o.LastModificationTime,
                                Id = o.Id
							},
                         	OrderOrderRemarks = s1 == null || s1.OrderRemarks == null ? "" : s1.OrderRemarks.ToString()
						};

            var totalCount = await filteredOrderReceipts.CountAsync();

            return new PagedResultDto<GetOrderReceiptForViewDto>(
                totalCount,
                await orderReceipts.ToListAsync()
            );
         }
		 
		 public async Task<GetOrderReceiptForViewDto> GetOrderReceiptForView(long id)
         {
            var orderReceipt = await _orderReceiptRepository.GetAsync(id);

            var output = new GetOrderReceiptForViewDto { OrderReceipt = ObjectMapper.Map<OrderReceiptDto>(orderReceipt) };

		    if (output.OrderReceipt.OrderId != null)
            {
                var _lookupOrder = await _lookup_orderRepository.FirstOrDefaultAsync((long)output.OrderReceipt.OrderId);
                output.OrderOrderRemarks = _lookupOrder?.OrderRemarks?.ToString();
            }
			
            return output;
         }
		 
		 //[AbpAuthorize(AppPermissions.Pages_OrderReceipts_Edit)]
		 public async Task<GetOrderReceiptForEditOutput> GetOrderReceiptForEdit(EntityDto<long> input)
         {
            var orderReceipt = await _orderReceiptRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetOrderReceiptForEditOutput {OrderReceipt = ObjectMapper.Map<CreateOrEditOrderReceiptDto>(orderReceipt)};

		    if (output.OrderReceipt.OrderId != null)
            {
                var _lookupOrder = await _lookup_orderRepository.FirstOrDefaultAsync((long)output.OrderReceipt.OrderId);
                output.OrderOrderRemarks = _lookupOrder?.OrderRemarks?.ToString();
            }
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditOrderReceiptDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 //[AbpAuthorize(AppPermissions.Pages_OrderReceipts_Create)]
		 protected virtual async Task Create(CreateOrEditOrderReceiptDto input)
         {
            var orderReceipt = ObjectMapper.Map<OrderReceipt>(input);

			
			if (AbpSession.TenantId != null)
			{
				orderReceipt.TenantId = (int?) AbpSession.TenantId;
			}
		

            await _orderReceiptRepository.InsertAsync(orderReceipt);
         }

		 //[AbpAuthorize(AppPermissions.Pages_OrderReceipts_Edit)]
		 protected virtual async Task Update(CreateOrEditOrderReceiptDto input)
         {
            var orderReceipt = await _orderReceiptRepository.FirstOrDefaultAsync((long)input.Id);
             ObjectMapper.Map(input, orderReceipt);
         }

		 //[AbpAuthorize(AppPermissions.Pages_OrderReceipts_Delete)]
         public async Task Delete(EntityDto<long> input)
         {
            await _orderReceiptRepository.DeleteAsync(input.Id);
         } 

		//[AbpAuthorize(AppPermissions.Pages_OrderReceipts)]
         public async Task<PagedResultDto<OrderReceiptOrderLookupTableDto>> GetAllOrderForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_orderRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.OrderRemarks != null && e.OrderRemarks.Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var orderList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<OrderReceiptOrderLookupTableDto>();
			foreach(var order in orderList){
				lookupTableDtoList.Add(new OrderReceiptOrderLookupTableDto
				{
					Id = order.Id,
					DisplayName = order.OrderRemarks?.ToString()
				});
			}

            return new PagedResultDto<OrderReceiptOrderLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }
    }
}