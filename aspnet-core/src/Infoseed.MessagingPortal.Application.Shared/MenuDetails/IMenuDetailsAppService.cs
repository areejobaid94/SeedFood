using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.Dto;
using Infoseed.MessagingPortal.MenuDetails.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Infoseed.MessagingPortal.MenuDetails
{
	public interface IMenuDetailsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetMenuDetailForViewDto>> GetAll(GetAllMenuDetailsInput input);

        Task<GetMenuDetailForViewDto> GetMenuDetailForView(long id);

		Task<GetMenuDetailForEditOutput> GetMenuDetailForEdit(EntityDto<long> input);

		Task CreateOrEdit(CreateOrEditMenuDetailDto input);

		Task Delete(EntityDto<long> input);

		Task<FileDto> GetMenuDetailsToExcel(GetAllMenuDetailsForExcelInput input);

		
		Task<List<MenuDetailItemLookupTableDto>> GetAllItemForTableDropdown();
		
		Task<PagedResultDto<MenuDetailMenuLookupTableDto>> GetAllMenuForLookupTable(GetAllForLookupTableInput input);
		
		Task<List<MenuDetailMenuItemStatusLookupTableDto>> GetAllMenuItemStatusForTableDropdown();
		
    }
}