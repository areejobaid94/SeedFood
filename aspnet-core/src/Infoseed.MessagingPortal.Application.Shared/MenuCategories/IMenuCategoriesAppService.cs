using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.MenuCategories.Dtos;
using Infoseed.MessagingPortal.Dto;
using System.Collections.Generic;
using Infoseed.MessagingPortal.Menus;

namespace Infoseed.MessagingPortal.MenuCategories
{
    public interface IMenuCategoriesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetMenuCategoryForViewDto>> GetAll(GetAllMenuCategoriesInput input);

        Task<GetMenuCategoryForViewDto> GetItemCategoryForView(long id);

		Task<GetMenuCategoryForEditOutput> GetItemCategoryForEdit(EntityDto<long> input);

        Task<long> CreateOrEdit(CreateOrEditMenuCategoryDto input);

		bool Delete(long input);

		Task<FileDto> GetItemCategoryToExcel(GetAllMenuCategoriesForExcelInput input);

        //List<MenuCategoryDto> GetAllWithTenantID(int? TenantID,int menu);

        List<CategorysInItemModel> GetCategoryWithItem(int tenantId, int menuType);
    }
}