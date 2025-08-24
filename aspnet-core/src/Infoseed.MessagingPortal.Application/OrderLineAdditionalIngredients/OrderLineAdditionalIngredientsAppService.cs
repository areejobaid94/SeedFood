using Infoseed.MessagingPortal.Orders;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infoseed.MessagingPortal.OrderLineAdditionalIngredients.Dtos;
using Infoseed.MessagingPortal.Dto;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Infoseed.MessagingPortal.OrderLineAdditionalIngredients
{
	//[AbpAuthorize(AppPermissions.Pages_OrderLineAdditionalIngredients)]
    public class OrderLineAdditionalIngredientsAppService : MessagingPortalAppServiceBase, IOrderLineAdditionalIngredientsAppService
    {
		 private readonly IRepository<OrderLineAdditionalIngredient, long> _orderLineAdditionalIngredientRepository;
		 private readonly IRepository<Order,long> _lookup_orderRepository;
		 

		  public OrderLineAdditionalIngredientsAppService(IRepository<OrderLineAdditionalIngredient, long> orderLineAdditionalIngredientRepository , IRepository<Order, long> lookup_orderRepository) 
		  {
			_orderLineAdditionalIngredientRepository = orderLineAdditionalIngredientRepository;
			_lookup_orderRepository = lookup_orderRepository;
		
		  }

		 public async Task<PagedResultDto<GetOrderLineAdditionalIngredientForViewDto>> GetAll(GetAllOrderLineAdditionalIngredientsInput input)
         {
			
			var filteredOrderLineAdditionalIngredients = _orderLineAdditionalIngredientRepository.GetAll()
						.Include( e => e.OrderFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Remarks.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.RemarksFilter),  e => e.Remarks == input.RemarksFilter)
						.WhereIf(input.MinTotalFilter != null, e => e.Total >= input.MinTotalFilter)
						.WhereIf(input.MaxTotalFilter != null, e => e.Total <= input.MaxTotalFilter)
						.WhereIf(input.MinQuantityFilter != null, e => e.Quantity >= input.MinQuantityFilter)
						.WhereIf(input.MaxQuantityFilter != null, e => e.Quantity <= input.MaxQuantityFilter)
						.WhereIf(input.MinUnitPriceFilter != null, e => e.UnitPrice >= input.MinUnitPriceFilter)
						.WhereIf(input.MaxUnitPriceFilter != null, e => e.UnitPrice <= input.MaxUnitPriceFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.OrderOrderRemarksFilter), e => e.OrderFk != null && e.OrderFk.OrderRemarks == input.OrderOrderRemarksFilter);

			var pagedAndFilteredOrderLineAdditionalIngredients = filteredOrderLineAdditionalIngredients
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var orderLineAdditionalIngredients = from o in pagedAndFilteredOrderLineAdditionalIngredients
                         join o1 in _lookup_orderRepository.GetAll() on o.OrderId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         select new GetOrderLineAdditionalIngredientForViewDto() {
							OrderLineAdditionalIngredient = new OrderLineAdditionalIngredientDto
							{
                                Remarks = o.Remarks,
                                Total = o.Total,
                                Quantity = o.Quantity,
                                UnitPrice = o.UnitPrice,
                                Id = o.Id
							},
                         	OrderOrderRemarks = s1 == null || s1.OrderRemarks == null ? "" : s1.OrderRemarks.ToString()
						};

            var totalCount = await filteredOrderLineAdditionalIngredients.CountAsync();

            return new PagedResultDto<GetOrderLineAdditionalIngredientForViewDto>(
                totalCount,
                await orderLineAdditionalIngredients.ToListAsync()
            );
         }
		 
		 public async Task<GetOrderLineAdditionalIngredientForViewDto> GetOrderLineAdditionalIngredientForView(long id)
         {
            var orderLineAdditionalIngredient = await _orderLineAdditionalIngredientRepository.GetAsync(id);

            var output = new GetOrderLineAdditionalIngredientForViewDto { OrderLineAdditionalIngredient = ObjectMapper.Map<OrderLineAdditionalIngredientDto>(orderLineAdditionalIngredient) };

		    if (output.OrderLineAdditionalIngredient.OrderId != null)
            {
                var _lookupOrder = await _lookup_orderRepository.FirstOrDefaultAsync((long)output.OrderLineAdditionalIngredient.OrderId);
                output.OrderOrderRemarks = _lookupOrder?.OrderRemarks?.ToString();
            }
			
            return output;
         }
		 
		 //[AbpAuthorize(AppPermissions.Pages_OrderLineAdditionalIngredients_Edit)]
		 public async Task<GetOrderLineAdditionalIngredientForEditOutput> GetOrderLineAdditionalIngredientForEdit(EntityDto<long> input)
         {
            var orderLineAdditionalIngredient = await _orderLineAdditionalIngredientRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetOrderLineAdditionalIngredientForEditOutput {OrderLineAdditionalIngredient = ObjectMapper.Map<CreateOrEditOrderLineAdditionalIngredientDto>(orderLineAdditionalIngredient)};

		    if (output.OrderLineAdditionalIngredient.OrderId != null)
            {
                var _lookupOrder = await _lookup_orderRepository.FirstOrDefaultAsync((long)output.OrderLineAdditionalIngredient.OrderId);
                output.OrderOrderRemarks = _lookupOrder?.OrderRemarks?.ToString();
            }
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditOrderLineAdditionalIngredientDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 //[AbpAuthorize(AppPermissions.Pages_OrderLineAdditionalIngredients_Create)]
		 protected virtual async Task Create(CreateOrEditOrderLineAdditionalIngredientDto input)
         {
            var orderLineAdditionalIngredient = ObjectMapper.Map<OrderLineAdditionalIngredient>(input);

			
			if (AbpSession.TenantId != null)
			{
				orderLineAdditionalIngredient.TenantId = (int?) AbpSession.TenantId;
			}
		

            await _orderLineAdditionalIngredientRepository.InsertAsync(orderLineAdditionalIngredient);
         }

		 //[AbpAuthorize(AppPermissions.Pages_OrderLineAdditionalIngredients_Edit)]
		 protected virtual async Task Update(CreateOrEditOrderLineAdditionalIngredientDto input)
         {
            var orderLineAdditionalIngredient = await _orderLineAdditionalIngredientRepository.FirstOrDefaultAsync((long)input.Id);
             ObjectMapper.Map(input, orderLineAdditionalIngredient);
         }

		 //[AbpAuthorize(AppPermissions.Pages_OrderLineAdditionalIngredients_Delete)]
         public async Task Delete(EntityDto<long> input)
         {
            await _orderLineAdditionalIngredientRepository.DeleteAsync(input.Id);
         } 

		//[AbpAuthorize(AppPermissions.Pages_OrderLineAdditionalIngredients)]
         public async Task<PagedResultDto<OrderLineAdditionalIngredientOrderLookupTableDto>> GetAllOrderForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_orderRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.OrderRemarks != null && e.OrderRemarks.Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var orderList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<OrderLineAdditionalIngredientOrderLookupTableDto>();
			foreach(var order in orderList){
				lookupTableDtoList.Add(new OrderLineAdditionalIngredientOrderLookupTableDto
				{
					Id = order.Id,
					DisplayName = order.OrderRemarks?.ToString()
				});
			}

            return new PagedResultDto<OrderLineAdditionalIngredientOrderLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }
    }
}