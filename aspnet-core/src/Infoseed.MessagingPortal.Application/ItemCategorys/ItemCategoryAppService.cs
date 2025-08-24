using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infoseed.MessagingPortal.MenuCategories.Exporting;
using Infoseed.MessagingPortal.MenuCategories.Dtos;
using Infoseed.MessagingPortal.Dto;
using Abp.Application.Services.Dto;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using System.Data;
using Infoseed.MessagingPortal.Menus;
using Infoseed.MessagingPortal.Items.Dtos;
using Infoseed.MessagingPortal.ItemAdditions.Dtos;
using Infoseed.MessagingPortal.Menus.Dtos;
using Framework.Data;

namespace Infoseed.MessagingPortal.MenuCategories
{
	//[AbpAuthorize(AppPermissions.Pages_MenuCategories)]
	public class ItemCategoryAppService : MessagingPortalAppServiceBase, IMenuCategoriesAppService
	{
		private readonly IRepository<ItemCategory, long> _menuCategoryRepository;
		private readonly IMenuCategoriesExcelExporter _menuCategoriesExcelExporter;


		public ItemCategoryAppService()
		{
			

		}

		public ItemCategoryAppService(IRepository<ItemCategory, long> menuCategoryRepository, IMenuCategoriesExcelExporter menuCategoriesExcelExporter)
		{
			_menuCategoryRepository = menuCategoryRepository;
			_menuCategoriesExcelExporter = menuCategoriesExcelExporter;

		}

		public async Task<PagedResultDto<GetMenuCategoryForViewDto>> GetAll(GetAllMenuCategoriesInput input)
		{

			var filteredMenuCategories = _menuCategoryRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Name.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter), e => e.Name == input.NameFilter);


			var pagedAndFilteredMenuCategories = filteredMenuCategories
				.OrderBy(input.Sorting ?? "id asc")
				.PageBy(input);

			var menuCategories = from o in pagedAndFilteredMenuCategories
								 select new GetMenuCategoryForViewDto() {
									 MenuCategory = new MenuCategoryDto
									 {
										 Name = o.Name,
										 NameEnglish = o.NameEnglish,
										 Id = o.Id,
										 IsDeleted = o.IsDeleted,
										 MenuType = o.MenuType,
										 Priority = o.Priority,
										 logoImag = o.logoImag,
										 bgImag = o.bgImag,
										 MenuId = o.MenuId
									 }
								 };

			var totalCount = await filteredMenuCategories.CountAsync();

			return new PagedResultDto<GetMenuCategoryForViewDto>(
				totalCount,
				await menuCategories.ToListAsync()
			);
		}

		public async Task<GetMenuCategoryForViewDto> GetItemCategoryForView(long id)
		{
			var menuCategory = await _menuCategoryRepository.GetAsync(id);

			var output = new GetMenuCategoryForViewDto { MenuCategory = ObjectMapper.Map<MenuCategoryDto>(menuCategory) };

			return output;
		}

		//[AbpAuthorize(AppPermissions.Pages_MenuCategories_Edit)]
		public async Task<GetMenuCategoryForEditOutput> GetItemCategoryForEdit(EntityDto<long> input)
		{
			var menuCategory = await _menuCategoryRepository.FirstOrDefaultAsync(input.Id);

			var output = new GetMenuCategoryForEditOutput { MenuCategory = ObjectMapper.Map<CreateOrEditMenuCategoryDto>(menuCategory) };

			return output;
		}

		public async Task<long> CreateOrEdit(CreateOrEditMenuCategoryDto input)
		{
            try
            {
				if (input.Id == null || input.Id == 0)
				{
					return await Create(input);
				}
				else
				{
					await Update(input);
				}
				return 0;
			}
            catch (Exception ex)
            {

                throw ex;
            }
			

		}

		//[AbpAuthorize(AppPermissions.Pages_MenuCategories_Create)]
		protected virtual async Task<long> Create(CreateOrEditMenuCategoryDto input)
		{
			var menuCategory = ObjectMapper.Map<ItemCategory>(input);


			if (AbpSession.TenantId != null)
			{
				menuCategory.TenantId = (int?)AbpSession.TenantId;
			}


			var entityId = await _menuCategoryRepository.InsertAndGetIdAsync(menuCategory);
			return entityId;

		}

		//[AbpAuthorize(AppPermissions.Pages_MenuCategories_Edit)]
		protected virtual async Task Update(CreateOrEditMenuCategoryDto input)
		{
			var menuCategory = await _menuCategoryRepository.FirstOrDefaultAsync((long)input.Id);
			ObjectMapper.Map(input, menuCategory);
		}

		//[AbpAuthorize(AppPermissions.Pages_MenuCategories_Delete)]
		public bool Delete(long input)
		{
            var SP_Name = Constants.ItemCategory.SP_ItemCategoryDelete;

            var sqlParameters = new List<SqlParameter> {
                    new SqlParameter("@CategoryId",input),
                    new SqlParameter("@TenantId",AbpSession.TenantId.Value),
            };
            var OutputParameter = new System.Data.SqlClient.SqlParameter();
            OutputParameter.SqlDbType = SqlDbType.Bit;
            OutputParameter.ParameterName = "@Result";
            OutputParameter.Direction = ParameterDirection.Output;
            sqlParameters.Add(OutputParameter);

            SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);
            return (bool)OutputParameter.Value;

        }
		public async Task<FileDto> GetItemCategoryToExcel(GetAllMenuCategoriesForExcelInput input)
		{

			var filteredMenuCategories = _menuCategoryRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Name.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter), e => e.Name == input.NameFilter);

			var query = (from o in filteredMenuCategories
						 select new GetMenuCategoryForViewDto() {
							 MenuCategory = new MenuCategoryDto
							 {
								 Name = o.Name,
								 NameEnglish = o.NameEnglish,
								 Id = o.Id
							 }
						 });


			var menuCategoryListDtos = await query.ToListAsync();

			return _menuCategoriesExcelExporter.ExportToFile(menuCategoryListDtos);
		}

		public List<CategorysInItemModel> GetCategoryWithItem(int tenantId, int menuType)

		{

			return getCategoryWithItem(tenantId, menuType);
}


		private List<CategorysInItemModel> getCategoryWithItem(int tenantId, int menuType)
		{

			try
			{
				List<CategorysInItemModel> categorysInItemModels = new List<CategorysInItemModel>();
				var SP_Name = Constants.ItemCategory.SP_ItemWithCategoryGet;

				var sqlParameters = new List<SqlParameter>
				{
				new SqlParameter("@TenantId",tenantId)
			   ,new SqlParameter("@MenuType",menuType)


			};

				
				categorysInItemModels = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),
				 DataReaderMapper.ConvertToCategorysInItemModel, AppSettingsModel.ConnectionStrings).ToList();


				return categorysInItemModels;
			}
			catch (Exception ex)
			{
				throw ex;
			}


		//return categorysInItemModels;
		}


	}
}