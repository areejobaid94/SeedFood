using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.MenuItemStatuses.Dtos;
using Infoseed.MessagingPortal.Dto;


namespace Infoseed.MessagingPortal.MenuItemStatuses
{
    public interface IMenuItemStatusesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetMenuItemStatusForViewDto>> GetAll(GetAllMenuItemStatusesInput input);

        Task<GetMenuItemStatusForViewDto> GetMenuItemStatusForView(long id);

		Task<GetMenuItemStatusForEditOutput> GetMenuItemStatusForEdit(EntityDto<long> input);

		Task CreateOrEdit(CreateOrEditMenuItemStatusDto input);

		Task Delete(EntityDto<long> input);

		Task<FileDto> GetMenuItemStatusesToExcel(GetAllMenuItemStatusesForExcelInput input);

		
    }
}