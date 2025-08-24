using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Infoseed.MessagingPortal.Areas;
using Infoseed.MessagingPortal.Authorization;
using Infoseed.MessagingPortal.DeliveryChanges.Dtos;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.DeliveryChanges
{
	[AbpAuthorize(AppPermissions.Pages_DeliveryChanges)]
    public class DeliveryChangesAppService : MessagingPortalAppServiceBase, IDeliveryChangesAppService
    {
		 private readonly IRepository<DeliveryChange, long> _deliveryChangeRepository;
		 private readonly IRepository<Area,long> _lookup_areaRepository;
		 

		  public DeliveryChangesAppService(IRepository<DeliveryChange, long> deliveryChangeRepository , IRepository<Area, long> lookup_areaRepository) 
		  {
			_deliveryChangeRepository = deliveryChangeRepository;
			_lookup_areaRepository = lookup_areaRepository;
		
		  }

		 public async Task<PagedResultDto<GetDeliveryChangeForViewDto>> GetAll(GetAllDeliveryChangesInput input)
         {
			
			var filteredDeliveryChanges = _deliveryChangeRepository.GetAll()
						.Include( e => e.AreaFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.DeliveryServiceProvider.Contains(input.Filter))
						.WhereIf(input.MinChargesFilter != null, e => e.Charges >= input.MinChargesFilter)
						.WhereIf(input.MaxChargesFilter != null, e => e.Charges <= input.MaxChargesFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.DeliveryServiceProviderFilter),  e => e.DeliveryServiceProvider == input.DeliveryServiceProviderFilter)
						.WhereIf(input.MinCreationTimeFilter != null, e => e.CreationTime >= input.MinCreationTimeFilter)
						.WhereIf(input.MaxCreationTimeFilter != null, e => e.CreationTime <= input.MaxCreationTimeFilter)
						.WhereIf(input.MinDeletionTimeFilter != null, e => e.DeletionTime >= input.MinDeletionTimeFilter)
						.WhereIf(input.MaxDeletionTimeFilter != null, e => e.DeletionTime <= input.MaxDeletionTimeFilter)
						.WhereIf(input.MinLastModificationTimeFilter != null, e => e.LastModificationTime >= input.MinLastModificationTimeFilter)
						.WhereIf(input.MaxLastModificationTimeFilter != null, e => e.LastModificationTime <= input.MaxLastModificationTimeFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.AreaAreaNameFilter), e => e.AreaFk != null && e.AreaFk.AreaName == input.AreaAreaNameFilter);

			var pagedAndFilteredDeliveryChanges = filteredDeliveryChanges
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var deliveryChanges = from o in pagedAndFilteredDeliveryChanges
                         join o1 in _lookup_areaRepository.GetAll() on o.AreaId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         select new GetDeliveryChangeForViewDto() {
							DeliveryChange = new DeliveryChangeDto
							{
                                Charges = o.Charges,
                                DeliveryServiceProvider = o.DeliveryServiceProvider,
                                CreationTime = o.CreationTime,
                                DeletionTime = o.DeletionTime,
                                LastModificationTime = o.LastModificationTime,
                                Id = o.Id
							},
                         	AreaAreaName = s1 == null || s1.AreaName == null ? "" : s1.AreaName.ToString()
						};

            var totalCount = await filteredDeliveryChanges.CountAsync();

            return new PagedResultDto<GetDeliveryChangeForViewDto>(
                totalCount,
                await deliveryChanges.ToListAsync()
            );
         }
		 
		 public async Task<GetDeliveryChangeForViewDto> GetDeliveryChangeForView(long id)
         {
            var deliveryChange = await _deliveryChangeRepository.GetAsync(id);

            var output = new GetDeliveryChangeForViewDto { DeliveryChange = ObjectMapper.Map<DeliveryChangeDto>(deliveryChange) };

		    //if (output.DeliveryChange.AreaId != null)
      //      {
                var _lookupArea = await _lookup_areaRepository.FirstOrDefaultAsync((long)output.DeliveryChange.AreaId);
                output.AreaAreaName = _lookupArea?.AreaName?.ToString();
            //}
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_DeliveryChanges_Edit)]
		 public async Task<GetDeliveryChangeForEditOutput> GetDeliveryChangeForEdit(EntityDto<long> input)
         {
            var deliveryChange = await _deliveryChangeRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetDeliveryChangeForEditOutput {DeliveryChange = ObjectMapper.Map<CreateOrEditDeliveryChangeDto>(deliveryChange)};

		    //if (output.DeliveryChange.AreaId != null)
      //      {
                var _lookupArea = await _lookup_areaRepository.FirstOrDefaultAsync((long)output.DeliveryChange.AreaId);
                output.AreaAreaName = _lookupArea?.AreaName?.ToString();
           // }
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditDeliveryChangeDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_DeliveryChanges_Create)]
		 protected virtual async Task Create(CreateOrEditDeliveryChangeDto input)
         {
            var deliveryChange = ObjectMapper.Map<DeliveryChange>(input);

			
			if (AbpSession.TenantId != null)
			{
				deliveryChange.TenantId = (int?) AbpSession.TenantId;
			}
		

            await _deliveryChangeRepository.InsertAsync(deliveryChange);
         }

		 [AbpAuthorize(AppPermissions.Pages_DeliveryChanges_Edit)]
		 protected virtual async Task Update(CreateOrEditDeliveryChangeDto input)
         {
            var deliveryChange = await _deliveryChangeRepository.FirstOrDefaultAsync((long)input.Id);
             ObjectMapper.Map(input, deliveryChange);
         }

		 [AbpAuthorize(AppPermissions.Pages_DeliveryChanges_Delete)]
         public async Task Delete(EntityDto<long> input)
         {
            await _deliveryChangeRepository.DeleteAsync(input.Id);
         } 
			[AbpAuthorize(AppPermissions.Pages_DeliveryChanges)]
			public async Task<List<DeliveryChangeAreaLookupTableDto>> GetAllAreaForTableDropdown()
			{
				return await _lookup_areaRepository.GetAll()
					.Select(area => new DeliveryChangeAreaLookupTableDto
					{
						Id = area.Id,
						DisplayName = area == null || area.AreaName == null ? "" : area.AreaName.ToString()
					}).ToListAsync();
			}
							
    }
}