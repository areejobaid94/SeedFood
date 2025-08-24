

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infoseed.MessagingPortal.Branches.Dtos;
using Infoseed.MessagingPortal.Dto;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Infoseed.MessagingPortal.Branches
{
	//[AbpAuthorize(AppPermissions.Pages_Branches)]
    public class BranchesAppService : MessagingPortalAppServiceBase, IBranchesAppService
    {
		 private readonly IRepository<Branch, long> _branchRepository;
		 

		  public BranchesAppService(IRepository<Branch, long> branchRepository ) 
		  {
			_branchRepository = branchRepository;
			
		  }

		 public async Task<PagedResultDto<GetBranchForViewDto>> GetAll(GetAllBranchesInput input)
         {
            try
            {
                var filteredBranches = _branchRepository.GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Name.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter), e => e.Name == input.NameFilter);


                var pagedAndFilteredBranches = filteredBranches
                    .OrderBy(input.Sorting ?? "id asc")
                    .PageBy(input);

                var branches = from o in pagedAndFilteredBranches
                               select new GetBranchForViewDto()
                               {
                                   Branch = new BranchDto
                                   {
                                       RestaurantName = o.RestaurantName,
                                       Name = o.Name,
                                       Id = o.Id,
                                       DeliveryCost = o.DeliveryCost
                                   }
                               };

                var totalCount = await filteredBranches.CountAsync();

                return new PagedResultDto<GetBranchForViewDto>(
                    totalCount,
                    await branches.ToListAsync()
                );
            }
            catch (Exception ex)
            {

                throw ex;
            }
			
         }
		 

        public void testinfosed()
        {


            throw new Exception("test");
        }
		 public async Task<GetBranchForViewDto> GetBranchForView(long id)
         {
            try
            {
                var branch = await _branchRepository.GetAsync(id);

                var output = new GetBranchForViewDto { Branch = ObjectMapper.Map<BranchDto>(branch) };

                return output;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
         }
		 
		 //[AbpAuthorize(AppPermissions.Pages_Branches_Edit)]
		 public async Task<GetBranchForEditOutput> GetBranchForEdit(EntityDto<long> input)
         {
            try
            {
                var branch = await _branchRepository.FirstOrDefaultAsync(input.Id);

                var output = new GetBranchForEditOutput { Branch = ObjectMapper.Map<CreateOrEditBranchDto>(branch) };

                return output;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
         }

		 public async Task CreateOrEdit(CreateOrEditBranchDto input)
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

		 //[AbpAuthorize(AppPermissions.Pages_Branches_Create)]
		 protected virtual async Task Create(CreateOrEditBranchDto input)
         {
            try
            {
                var branch = ObjectMapper.Map<Branch>(input);


                if (AbpSession.TenantId != null)
                {
                    branch.TenantId = (int?)AbpSession.TenantId;
                }


                await _branchRepository.InsertAsync(branch);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
         }

		 //[AbpAuthorize(AppPermissions.Pages_Branches_Edit)]
		 protected virtual async Task Update(CreateOrEditBranchDto input)
         {
            try
            {
                var branch = await _branchRepository.FirstOrDefaultAsync((long)input.Id);
                ObjectMapper.Map(input, branch);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
         }

		 //[AbpAuthorize(AppPermissions.Pages_Branches_Delete)]
         public async Task Delete(EntityDto<long> input)
         {
            try
            {
                await _branchRepository.DeleteAsync(input.Id);

            }
            catch (Exception ex)
            {

                throw ex;
            }
         } 
    }
}