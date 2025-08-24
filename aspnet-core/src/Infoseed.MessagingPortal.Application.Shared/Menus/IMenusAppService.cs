using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.Menus.Dtos;
using Infoseed.MessagingPortal.Dto;
using System.Collections.Generic;
using Infoseed.MessagingPortal.MenuCategories.Dtos;
using Infoseed.MessagingPortal.Configuration.Tenants.Dto;
using Infoseed.MessagingPortal.ItemAdditionsCategorys.Dtos;
using Infoseed.MessagingPortal.Orders.Dtos;

namespace Infoseed.MessagingPortal.Menus
{
    public interface IMenusAppService : IApplicationService 
    {
		MenusEntity GetMenus(int pageNumber, int PageSize,int? tenantId = null);
        //Task<GetMenuForViewDto> GetMenuForView(long id);
		Task<GetMenuForEditOutput> GetMenuForEdit(EntityDto<long> input);

		Task<long> CreateOrEdit(CreateOrEditMenuDto input);

		bool Delete(long input);

		Task<FileDto> GetMenusToExcel(GetAllMenusForExcelInput input);

		//MenuAndItemAndCategorys GetAllWithTenantID(int? TenantID, int? ContactId);
		//MenuDataModel GetMenuData(int menuId);
		//List<CategorysInItemModel> GetCategorysInItem(int menuId);
		RType[] GetRType(int? TenantID);

		List<CreateOrEditMenuCategoryDto> GetCategorysByMenuID(long menuID);
		long AddSubCatogeory(CreateOrEditMenuSubCategoryDto createOrEditMenuSubCategoryDto);
		void UpdateSubCatogeory(CreateOrEditMenuSubCategoryDto createOrEditMenuSubCategoryDto);
		bool DeleteSubCatogeory(long subCategoryID);
		//List<CategorysInItemModel> GetCategorysInItemWithTenantId(int TenantId);
		List<MenuEntity> GetMenusWithDetails(int TenantID, int MenuType);
		void SaveSetting(long menuId, WorkModel workModel);
		WorkModel GetMenuSetting(long menuId);
		ItemCategoryEntity GetItemAdditionsCategories(int pageNumber = 0 ,int pageSize = 10);
		ItemCategoryEntity GetSpecificationsCategory(int pageNumber = 0, int pageSize = 10);
        MenuContcatKeyModel MenuContactKeyAdd(MenuContcatKeyModel model);
        MenuContcatKeyModel MenuContactKeyGet(MenuContcatKeyModel model);
        MenuContcatKeyModel MenuContactKeyGetNew(MenuContcatKeyModel model);

        long CreateMenuReminderMessage(MenuReminderMessages messages);
		void UpdateMenuReminderMessage(long id);

        List<MenuReminderMessages> GetMenuReminderMessage();

    }
}