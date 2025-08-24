using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Infoseed.MessagingPortal.Areas;
using Infoseed.MessagingPortal.Authorization;
using Infoseed.MessagingPortal.BranchAreas.Dtos;
using Infoseed.MessagingPortal.Branches;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.BranchAreas
{
	[AbpAuthorize(AppPermissions.Pages_BranchAreas)]
    public class BranchAreasAppService : MessagingPortalAppServiceBase, IBranchAreasAppService
    {
		 private readonly IRepository<BranchArea, long> _branchAreaRepository;
		 private readonly IRepository<Area,long> _lookup_areaRepository;
		 private readonly IRepository<Branch,long> _lookup_branchRepository;
		 

		  public BranchAreasAppService(IRepository<BranchArea, long> branchAreaRepository , IRepository<Area, long> lookup_areaRepository, IRepository<Branch, long> lookup_branchRepository) 
		  {
			_branchAreaRepository = branchAreaRepository;
			_lookup_areaRepository = lookup_areaRepository;
		_lookup_branchRepository = lookup_branchRepository;
		
		  }

		 public async Task<PagedResultDto<GetBranchAreaForViewDto>> GetAll(GetAllBranchAreasInput input)
         {
            try
            {
				var filteredBranchAreas = _branchAreaRepository.GetAll()
						.Include(e => e.AreaFk)
						.Include(e => e.BranchFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false)
						.WhereIf(input.MinCreationTimeFilter != null, e => e.CreationTime >= input.MinCreationTimeFilter)
						.WhereIf(input.MaxCreationTimeFilter != null, e => e.CreationTime <= input.MaxCreationTimeFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.AreaAreaNameFilter), e => e.AreaFk != null && e.AreaFk.AreaName == input.AreaAreaNameFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.BranchNameFilter), e => e.BranchFk != null && e.BranchFk.Name == input.BranchNameFilter);

				var pagedAndFilteredBranchAreas = filteredBranchAreas
					.OrderBy(input.Sorting ?? "id asc")
					.PageBy(input);

				var branchAreas = from o in pagedAndFilteredBranchAreas
								  join o1 in _lookup_areaRepository.GetAll() on o.AreaId equals o1.Id into j1
								  from s1 in j1.DefaultIfEmpty()

								  join o2 in _lookup_branchRepository.GetAll() on o.BranchId equals o2.Id into j2
								  from s2 in j2.DefaultIfEmpty()

								  select new GetBranchAreaForViewDto()
								  {
									  BranchArea = new BranchAreaDto
									  {
										  CreationTime = o.CreationTime,
										  Id = o.Id
									  },
									  AreaAreaName = s1 == null || s1.AreaName == null ? "" : s1.AreaName.ToString(),
									  BranchName = s2 == null || s2.Name == null ? "" : s2.Name.ToString()
								  };

				var totalCount = await filteredBranchAreas.CountAsync();

				return new PagedResultDto<GetBranchAreaForViewDto>(
					totalCount,
					await branchAreas.ToListAsync()
				);
			}
            catch (Exception ex)
            {

                throw ex;
            }
			
         }
		 
		 public async Task<GetBranchAreaForViewDto> GetBranchAreaForView(long id)
         {
            try
            {
				var branchArea = await _branchAreaRepository.GetAsync(id);

				var output = new GetBranchAreaForViewDto { BranchArea = ObjectMapper.Map<BranchAreaDto>(branchArea) };

				//if (output.BranchArea.AreaId != null)
				//{
					var _lookupArea = await _lookup_areaRepository.FirstOrDefaultAsync((long)output.BranchArea.AreaId);
					output.AreaAreaName = _lookupArea?.AreaName?.ToString();
				//}

				//if (output.BranchArea.BranchId != null)
				//{
					var _lookupBranch = await _lookup_branchRepository.FirstOrDefaultAsync((long)output.BranchArea.BranchId);
					output.BranchName = _lookupBranch?.Name?.ToString();
				//}

				return output;
			}
            catch (Exception ex)
            {

                throw ex;
            }
            
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_BranchAreas_Edit)]
		 public async Task<GetBranchAreaForEditOutput> GetBranchAreaForEdit(EntityDto<long> input)
         {
            try
            {
				var branchArea = await _branchAreaRepository.FirstOrDefaultAsync(input.Id);

				var output = new GetBranchAreaForEditOutput { BranchArea = ObjectMapper.Map<CreateOrEditBranchAreaDto>(branchArea) };

				//if (output.BranchArea.AreaId != null)
				//{
					var _lookupArea = await _lookup_areaRepository.FirstOrDefaultAsync((long)output.BranchArea.AreaId);
					output.AreaAreaName = _lookupArea?.AreaName?.ToString();
				//}

				//if (output.BranchArea.BranchId != null)
				//{
					var _lookupBranch = await _lookup_branchRepository.FirstOrDefaultAsync((long)output.BranchArea.BranchId);
					output.BranchName = _lookupBranch?.Name?.ToString();
				//}

				return output;
			}
            catch (Exception ex)
            {

                throw ex;
            }
            
         }

		 public async Task CreateOrEdit(CreateOrEditBranchAreaDto input)
         {
            try
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
            catch (Exception ex)
            {

                throw ex;
            }
            
         }

		 [AbpAuthorize(AppPermissions.Pages_BranchAreas_Create)]
		 protected virtual async Task Create(CreateOrEditBranchAreaDto input)
         {
            try
            {
				var branchArea = ObjectMapper.Map<BranchArea>(input);


				if (AbpSession.TenantId != null)
				{
					branchArea.TenantId = (int?)AbpSession.TenantId;
				}


				await _branchAreaRepository.InsertAsync(branchArea);
			}
            catch (Exception ex)
            {

                throw ex;
            }
            
         }

		 [AbpAuthorize(AppPermissions.Pages_BranchAreas_Edit)]
		 protected virtual async Task Update(CreateOrEditBranchAreaDto input)
         {
            var branchArea = await _branchAreaRepository.FirstOrDefaultAsync((long)input.Id);
             ObjectMapper.Map(input, branchArea);
         }

		 [AbpAuthorize(AppPermissions.Pages_BranchAreas_Delete)]
         public async Task Delete(EntityDto<long> input)
         {
            await _branchAreaRepository.DeleteAsync(input.Id);
         } 
			[AbpAuthorize(AppPermissions.Pages_BranchAreas)]
			public async Task<List<BranchAreaAreaLookupTableDto>> GetAllAreaForTableDropdown()
			{
				try
				{
					return await _lookup_areaRepository.GetAll()
						.Select(area => new BranchAreaAreaLookupTableDto
						{
							Id = area.Id,
							DisplayName = area == null || area.AreaName == null ? "" : area.AreaName.ToString()
						}).ToListAsync();

				}
				catch (Exception)
				{

					throw;
				}
				
			}
							
			[AbpAuthorize(AppPermissions.Pages_BranchAreas)]
			public async Task<List<BranchAreaBranchLookupTableDto>> GetAllBranchForTableDropdown()
			{
				try
				{
					return await _lookup_branchRepository.GetAll()
						.Select(branch => new BranchAreaBranchLookupTableDto
						{
							Id = branch.Id,
							DisplayName = branch == null || branch.Name == null ? "" : branch.Name.ToString()
						}).ToListAsync();
				}
				catch (Exception)
				{

					throw;
				}
				
			}
							
    }
}