

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infoseed.MessagingPortal.MenuItemStatuses.Exporting;
using Infoseed.MessagingPortal.MenuItemStatuses.Dtos;
using Infoseed.MessagingPortal.Dto;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Infoseed.MessagingPortal.MenuItemStatuses
{
	[AbpAuthorize(AppPermissions.Pages_MenuItemStatuses)]
    public class MenuItemStatusesAppService : MessagingPortalAppServiceBase, IMenuItemStatusesAppService
    {
		 private readonly IRepository<MenuItemStatus, long> _menuItemStatusRepository;
		 private readonly IMenuItemStatusesExcelExporter _menuItemStatusesExcelExporter;
		 

		  public MenuItemStatusesAppService(IRepository<MenuItemStatus, long> menuItemStatusRepository, IMenuItemStatusesExcelExporter menuItemStatusesExcelExporter ) 
		  {
			_menuItemStatusRepository = menuItemStatusRepository;
			_menuItemStatusesExcelExporter = menuItemStatusesExcelExporter;
			
		  }

		 public async Task<PagedResultDto<GetMenuItemStatusForViewDto>> GetAll(GetAllMenuItemStatusesInput input)
         {
			
			var filteredMenuItemStatuses = _menuItemStatusRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Name.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter),  e => e.Name == input.NameFilter);

			var pagedAndFilteredMenuItemStatuses = filteredMenuItemStatuses
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var menuItemStatuses = from o in pagedAndFilteredMenuItemStatuses
                         select new GetMenuItemStatusForViewDto() {
							MenuItemStatus = new MenuItemStatusDto
							{
                                Name = o.Name,
                                Id = o.Id
							}
						};

            var totalCount = await filteredMenuItemStatuses.CountAsync();

            return new PagedResultDto<GetMenuItemStatusForViewDto>(
                totalCount,
                await menuItemStatuses.ToListAsync()
            );
         }
		 
		 public async Task<GetMenuItemStatusForViewDto> GetMenuItemStatusForView(long id)
         {
            var menuItemStatus = await _menuItemStatusRepository.GetAsync(id);

            var output = new GetMenuItemStatusForViewDto { MenuItemStatus = ObjectMapper.Map<MenuItemStatusDto>(menuItemStatus) };
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_MenuItemStatuses_Edit)]
		 public async Task<GetMenuItemStatusForEditOutput> GetMenuItemStatusForEdit(EntityDto<long> input)
         {
            var menuItemStatus = await _menuItemStatusRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetMenuItemStatusForEditOutput {MenuItemStatus = ObjectMapper.Map<CreateOrEditMenuItemStatusDto>(menuItemStatus)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditMenuItemStatusDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_MenuItemStatuses_Create)]
		 protected virtual async Task Create(CreateOrEditMenuItemStatusDto input)
         {
            var menuItemStatus = ObjectMapper.Map<MenuItemStatus>(input);

			

            await _menuItemStatusRepository.InsertAsync(menuItemStatus);
         }

		 [AbpAuthorize(AppPermissions.Pages_MenuItemStatuses_Edit)]
		 protected virtual async Task Update(CreateOrEditMenuItemStatusDto input)
         {
            var menuItemStatus = await _menuItemStatusRepository.FirstOrDefaultAsync((long)input.Id);
             ObjectMapper.Map(input, menuItemStatus);
         }

		 [AbpAuthorize(AppPermissions.Pages_MenuItemStatuses_Delete)]
         public async Task Delete(EntityDto<long> input)
         {
            await _menuItemStatusRepository.DeleteAsync(input.Id);
         } 

		public async Task<FileDto> GetMenuItemStatusesToExcel(GetAllMenuItemStatusesForExcelInput input)
         {
			
			var filteredMenuItemStatuses = _menuItemStatusRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Name.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter),  e => e.Name == input.NameFilter);

			var query = (from o in filteredMenuItemStatuses
                         select new GetMenuItemStatusForViewDto() { 
							MenuItemStatus = new MenuItemStatusDto
							{
                                Name = o.Name,
                                Id = o.Id
							}
						 });


            var menuItemStatusListDtos = await query.ToListAsync();

            return _menuItemStatusesExcelExporter.ExportToFile(menuItemStatusListDtos);
         }


    }
}