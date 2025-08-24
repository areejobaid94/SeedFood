using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.BranchAreas.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Infoseed.MessagingPortal.BranchAreas
{
	public interface IBranchAreasAppService : IApplicationService 
    {
        Task<PagedResultDto<GetBranchAreaForViewDto>> GetAll(GetAllBranchAreasInput input);

        Task<GetBranchAreaForViewDto> GetBranchAreaForView(long id);

		Task<GetBranchAreaForEditOutput> GetBranchAreaForEdit(EntityDto<long> input);

		Task CreateOrEdit(CreateOrEditBranchAreaDto input);

		Task Delete(EntityDto<long> input);

		
		Task<List<BranchAreaAreaLookupTableDto>> GetAllAreaForTableDropdown();
		
		Task<List<BranchAreaBranchLookupTableDto>> GetAllBranchForTableDropdown();
		
    }
}