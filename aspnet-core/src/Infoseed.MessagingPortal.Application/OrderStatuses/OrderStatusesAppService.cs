

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infoseed.MessagingPortal.OrderStatuses.Dtos;
using Infoseed.MessagingPortal.Dto;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Infoseed.MessagingPortal.OrderStatuses
{
	//[AbpAuthorize(AppPermissions.Pages_OrderStatuses)]
    public class OrderStatusesAppService : MessagingPortalAppServiceBase, IOrderStatusesAppService
    {
		 private readonly IRepository<OrderStatus, long> _orderStatusRepository;
		 

		  public OrderStatusesAppService(IRepository<OrderStatus, long> orderStatusRepository ) 
		  {
			_orderStatusRepository = orderStatusRepository;
			
		  }

		 public async Task<PagedResultDto<GetOrderStatusForViewDto>> GetAll(GetAllOrderStatusesInput input)
         {
			
			var filteredOrderStatuses = _orderStatusRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Name.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter),  e => e.Name == input.NameFilter);

			var pagedAndFilteredOrderStatuses = filteredOrderStatuses
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var orderStatuses = from o in pagedAndFilteredOrderStatuses
                         select new GetOrderStatusForViewDto() {
							OrderStatus = new OrderStatusDto
							{
                                Name = o.Name,
                                Id = o.Id
							}
						};

            var totalCount = await filteredOrderStatuses.CountAsync();

            return new PagedResultDto<GetOrderStatusForViewDto>(
                totalCount,
                await orderStatuses.ToListAsync()
            );
         }
		 
		 public async Task<GetOrderStatusForViewDto> GetOrderStatusForView(long id)
         {
            var orderStatus = await _orderStatusRepository.GetAsync(id);

            var output = new GetOrderStatusForViewDto { OrderStatus = ObjectMapper.Map<OrderStatusDto>(orderStatus) };
			
            return output;
         }
		 
		 //[AbpAuthorize(AppPermissions.Pages_OrderStatuses_Edit)]
		 public async Task<GetOrderStatusForEditOutput> GetOrderStatusForEdit(EntityDto<long> input)
         {
            var orderStatus = await _orderStatusRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetOrderStatusForEditOutput {OrderStatus = ObjectMapper.Map<CreateOrEditOrderStatusDto>(orderStatus)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditOrderStatusDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 //[AbpAuthorize(AppPermissions.Pages_OrderStatuses_Create)]
		 protected virtual async Task Create(CreateOrEditOrderStatusDto input)
         {
            var orderStatus = ObjectMapper.Map<OrderStatus>(input);

			
			if (AbpSession.TenantId != null)
			{
				orderStatus.TenantId = (int?) AbpSession.TenantId;
			}
		

            await _orderStatusRepository.InsertAsync(orderStatus);
         }

		 //[AbpAuthorize(AppPermissions.Pages_OrderStatuses_Edit)]
		 protected virtual async Task Update(CreateOrEditOrderStatusDto input)
         {
            var orderStatus = await _orderStatusRepository.FirstOrDefaultAsync((long)input.Id);
             ObjectMapper.Map(input, orderStatus);
         }

		 //[AbpAuthorize(AppPermissions.Pages_OrderStatuses_Delete)]
         public async Task Delete(EntityDto<long> input)
         {
            await _orderStatusRepository.DeleteAsync(input.Id);
         } 
    }
}