using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Infoseed.MessagingPortal.Authorization;
using Infoseed.MessagingPortal.Dto;
using Infoseed.MessagingPortal.Items;
using Infoseed.MessagingPortal.MenuDetails.Dtos;
using Infoseed.MessagingPortal.MenuDetails.Exporting;
using Infoseed.MessagingPortal.MenuItemStatuses;
using Infoseed.MessagingPortal.Menus;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.MenuDetails
{
	[AbpAuthorize(AppPermissions.Pages_MenuDetails)]
    public class MenuDetailsAppService : MessagingPortalAppServiceBase, IMenuDetailsAppService
    {
		 private readonly IRepository<MenuDetail, long> _menuDetailRepository;
		 private readonly IMenuDetailsExcelExporter _menuDetailsExcelExporter;
		 private readonly IRepository<Item,long> _lookup_itemRepository;
		 private readonly IRepository<Menu,long> _lookup_menuRepository;
		 private readonly IRepository<MenuItemStatus,long> _lookup_menuItemStatusRepository;
		 

		  public MenuDetailsAppService(IRepository<MenuDetail, long> menuDetailRepository, IMenuDetailsExcelExporter menuDetailsExcelExporter , IRepository<Item, long> lookup_itemRepository, IRepository<Menu, long> lookup_menuRepository, IRepository<MenuItemStatus, long> lookup_menuItemStatusRepository) 
		  {
			_menuDetailRepository = menuDetailRepository;
			_menuDetailsExcelExporter = menuDetailsExcelExporter;
			_lookup_itemRepository = lookup_itemRepository;
		_lookup_menuRepository = lookup_menuRepository;
		_lookup_menuItemStatusRepository = lookup_menuItemStatusRepository;
		
		  }

		 public async Task<PagedResultDto<GetMenuDetailForViewDto>> GetAll(GetAllMenuDetailsInput input)
         {

			var filteredMenuDetails = _menuDetailRepository.GetAll()
						//.Include( e => e.ItemFk)
						//.Include( e => e.MenuFk)
						//.Include( e => e.MenuItemStatusFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Description.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter), e => e.Description == input.DescriptionFilter)
						.WhereIf(input.IsStandAloneFilter.HasValue && input.IsStandAloneFilter > -1, e => (input.IsStandAloneFilter == 1 && e.IsStandAlone) || (input.IsStandAloneFilter == 0 && !e.IsStandAlone))
						.WhereIf(input.MinPriceFilter != null, e => e.Price >= input.MinPriceFilter)
						.WhereIf(input.MaxPriceFilter != null, e => e.Price <= input.MaxPriceFilter);
						//.WhereIf(!string.IsNullOrWhiteSpace(input.ItemItemNameFilter), e => e.ItemFk != null && e.ItemFk.ItemName == input.ItemItemNameFilter)
						//.WhereIf(!string.IsNullOrWhiteSpace(input.MenuMenuNameFilter), e => e.MenuFk != null && e.MenuFk.MenuName == input.MenuMenuNameFilter)
						//.WhereIf(!string.IsNullOrWhiteSpace(input.MenuItemStatusNameFilter), e => e.MenuItemStatusFk != null && e.MenuItemStatusFk.Name == input.MenuItemStatusNameFilter);

			var pagedAndFilteredMenuDetails = filteredMenuDetails
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var menuDetails = from o in pagedAndFilteredMenuDetails
                         //join o1 in _lookup_itemRepository.GetAll() on o.ItemId equals o1.Id into j1
                         //from s1 in j1.DefaultIfEmpty()
                         
                         //join o2 in _lookup_menuRepository.GetAll() on o.MenuId equals o2.Id into j2
                         //from s2 in j2.DefaultIfEmpty()
                         
                         //join o3 in _lookup_menuItemStatusRepository.GetAll() on o.MenuItemStatusId equals o3.Id into j3
                         //from s3 in j3.DefaultIfEmpty()
                         
                         select new GetMenuDetailForViewDto() {
							MenuDetail = new MenuDetailDto
							{
                                Description = o.Description,
                                IsStandAlone = o.IsStandAlone,
                                Price = o.Price,
                                Id = o.Id
							},
                         	//ItemItemName = s1 == null || s1.ItemName == null ? "" : s1.ItemName.ToString(),
                         	//MenuMenuName = s2 == null || s2.MenuName == null ? "" : s2.MenuName.ToString(),
                         	//MenuItemStatusName = s3 == null || s3.Name == null ? "" : s3.Name.ToString()
						};

            var totalCount = await filteredMenuDetails.CountAsync();

            return new PagedResultDto<GetMenuDetailForViewDto>(
                totalCount,
                await menuDetails.ToListAsync()
            );
         }
		 
		 public async Task<GetMenuDetailForViewDto> GetMenuDetailForView(long id)
         {
            var menuDetail = await _menuDetailRepository.GetAsync(id);

            var output = new GetMenuDetailForViewDto { MenuDetail = ObjectMapper.Map<MenuDetailDto>(menuDetail) };

		    //if (output.MenuDetail.ItemId != null)
      //      {
                var _lookupItem = await _lookup_itemRepository.FirstOrDefaultAsync((long)output.MenuDetail.ItemId);
                output.ItemItemName  = _lookupItem?.ItemName ?.ToString();
				output.ItemItemNameEnglish = _lookupItem?.ItemNameEnglish?.ToString();
			//}

		 //   if (output.MenuDetail.MenuId != null)
   //         {
                var _lookupMenu = await _lookup_menuRepository.FirstOrDefaultAsync((long)output.MenuDetail.MenuId);
                output.MenuMenuName = _lookupMenu?.MenuName?.ToString();
				output.MenuMenuNameEnglish  = _lookupMenu?.MenuNameEnglish?.ToString();
			//}

		 //   if (output.MenuDetail.MenuItemStatusId != null)
   //         {
                var _lookupMenuItemStatus = await _lookup_menuItemStatusRepository.FirstOrDefaultAsync((long)output.MenuDetail.MenuItemStatusId);
                output.MenuItemStatusName = _lookupMenuItemStatus?.Name?.ToString();
            //}
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_MenuDetails_Edit)]
		 public async Task<GetMenuDetailForEditOutput> GetMenuDetailForEdit(EntityDto<long> input)
         {
            var menuDetail = await _menuDetailRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetMenuDetailForEditOutput {MenuDetail = ObjectMapper.Map<CreateOrEditMenuDetailDto>(menuDetail)};

		    //if (output.MenuDetail.ItemId != null)
      //      {
                var _lookupItem = await _lookup_itemRepository.FirstOrDefaultAsync((long)output.MenuDetail.ItemId);
                output.ItemItemNameEnglish = _lookupItem?.ItemNameEnglish?.ToString();
				output.ItemItemName = _lookupItem?.ItemName ?.ToString();
			//}

		 //   if (output.MenuDetail.MenuId != null)
   //         {
                var _lookupMenu = await _lookup_menuRepository.FirstOrDefaultAsync((long)output.MenuDetail.MenuId);
				output.MenuMenuName = _lookupMenu?.MenuName?.ToString();
				output.MenuMenuNameEnglish = _lookupMenu?.MenuNameEnglish?.ToString();
			//}

		    if (output.MenuDetail.MenuItemStatusId != null)
            {
                var _lookupMenuItemStatus = await _lookup_menuItemStatusRepository.FirstOrDefaultAsync((long)output.MenuDetail.MenuItemStatusId);
                output.MenuItemStatusName = _lookupMenuItemStatus?.Name?.ToString();
            }
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditMenuDetailDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_MenuDetails_Create)]
		 protected virtual async Task Create(CreateOrEditMenuDetailDto input)
         {
            var menuDetail = ObjectMapper.Map<MenuDetail>(input);

			
			if (AbpSession.TenantId != null)
			{
				menuDetail.TenantId = (int?) AbpSession.TenantId;
			}
		

            await _menuDetailRepository.InsertAsync(menuDetail);
         }

		 [AbpAuthorize(AppPermissions.Pages_MenuDetails_Edit)]
		 protected virtual async Task Update(CreateOrEditMenuDetailDto input)
         {
            var menuDetail = await _menuDetailRepository.FirstOrDefaultAsync((long)input.Id);
             ObjectMapper.Map(input, menuDetail);
         }

		 [AbpAuthorize(AppPermissions.Pages_MenuDetails_Delete)]
         public async Task Delete(EntityDto<long> input)
         {
            await _menuDetailRepository.DeleteAsync(input.Id);
         } 

		public async Task<FileDto> GetMenuDetailsToExcel(GetAllMenuDetailsForExcelInput input)
         {

			var filteredMenuDetails = _menuDetailRepository.GetAll()
						//.Include( e => e.ItemFk)
						//.Include( e => e.MenuFk)
						//.Include( e => e.MenuItemStatusFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Description.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter), e => e.Description == input.DescriptionFilter)
						.WhereIf(input.IsStandAloneFilter.HasValue && input.IsStandAloneFilter > -1, e => (input.IsStandAloneFilter == 1 && e.IsStandAlone) || (input.IsStandAloneFilter == 0 && !e.IsStandAlone))
						.WhereIf(input.MinPriceFilter != null, e => e.Price >= input.MinPriceFilter)
						.WhereIf(input.MaxPriceFilter != null, e => e.Price <= input.MaxPriceFilter);
						//.WhereIf(!string.IsNullOrWhiteSpace(input.ItemItemNameFilter), e => e.ItemFk != null && e.ItemFk.ItemName == input.ItemItemNameFilter)
						//.WhereIf(!string.IsNullOrWhiteSpace(input.MenuMenuNameFilter), e => e.MenuFk != null && e.MenuFk.MenuName == input.MenuMenuNameFilter)
						//.WhereIf(!string.IsNullOrWhiteSpace(input.MenuItemStatusNameFilter), e => e.MenuItemStatusFk != null && e.MenuItemStatusFk.Name == input.MenuItemStatusNameFilter);

			var query = (from o in filteredMenuDetails
                         //join o1 in _lookup_itemRepository.GetAll() on o.ItemId equals o1.Id into j1
                         //from s1 in j1.DefaultIfEmpty()
                         
                         //join o2 in _lookup_menuRepository.GetAll() on o.MenuId equals o2.Id into j2
                         //from s2 in j2.DefaultIfEmpty()
                         
                         //join o3 in _lookup_menuItemStatusRepository.GetAll() on o.MenuItemStatusId equals o3.Id into j3
                         //from s3 in j3.DefaultIfEmpty()
                         
                         select new GetMenuDetailForViewDto() { 
							MenuDetail = new MenuDetailDto
							{
                                Description = o.Description,
                                IsStandAlone = o.IsStandAlone,
                                Price = o.Price,
                                Id = o.Id
							},
                         	//ItemItemName = s1 == null || s1.ItemName == null ? "" : s1.ItemName.ToString(),
                         	//MenuMenuName = s2 == null || s2.MenuName == null ? "" : s2.MenuName.ToString(),
                         	//MenuItemStatusName = s3 == null || s3.Name == null ? "" : s3.Name.ToString()
						 });


            var menuDetailListDtos = await query.ToListAsync();

            return _menuDetailsExcelExporter.ExportToFile(menuDetailListDtos);
         }


			[AbpAuthorize(AppPermissions.Pages_MenuDetails)]
			public async Task<List<MenuDetailItemLookupTableDto>> GetAllItemForTableDropdown()
			{
				return await _lookup_itemRepository.GetAll()
					.Select(item => new MenuDetailItemLookupTableDto
					{
						Id = item.Id,
						 DisplayName  = item == null || item.ItemName  == null ? "" : item.ItemName .ToString(),
						 DisplayNameEnglish = item == null || item.ItemNameEnglish == null ? "" : item.ItemNameEnglish.ToString()
					}).ToListAsync();
			}
							

		[AbpAuthorize(AppPermissions.Pages_MenuDetails)]
         public async Task<PagedResultDto<MenuDetailMenuLookupTableDto>> GetAllMenuForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_menuRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.MenuName != null && e.MenuName.Contains(input.Filter)
				);

            var totalCount = await query.CountAsync();

            var menuList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<MenuDetailMenuLookupTableDto>();
			foreach(var menu in menuList){
				lookupTableDtoList.Add(new MenuDetailMenuLookupTableDto
				{
					Id = menu.Id,
					 DisplayName = menu.MenuName?.ToString(),
					 DisplayNameEnglish = menu.MenuNameEnglish?.ToString()
				});
			}

            return new PagedResultDto<MenuDetailMenuLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }
			[AbpAuthorize(AppPermissions.Pages_MenuDetails)]
			public async Task<List<MenuDetailMenuItemStatusLookupTableDto>> GetAllMenuItemStatusForTableDropdown()
			{
				return await _lookup_menuItemStatusRepository.GetAll()
					.Select(menuItemStatus => new MenuDetailMenuItemStatusLookupTableDto
					{
						Id = menuItemStatus.Id,
						DisplayName = menuItemStatus == null || menuItemStatus.Name == null ? "" : menuItemStatus.Name.ToString()
					}).ToListAsync();
			}
							
    }
}