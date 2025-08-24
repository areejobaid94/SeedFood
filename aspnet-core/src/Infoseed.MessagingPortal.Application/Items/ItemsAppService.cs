using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infoseed.MessagingPortal.Items.Exporting;
using Infoseed.MessagingPortal.Items.Dtos;
using Infoseed.MessagingPortal.Dto;
using Abp.Application.Services.Dto;
using Microsoft.EntityFrameworkCore;
using Infoseed.MessagingPortal.Menus;
using Infoseed.MessagingPortal.MenuCategories;
using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Infoseed.MessagingPortal.ItemAdditions.Dtos;
using Infoseed.MessagingPortal.ItemAndAdditionsCategorys;
using Framework.Data;
using System.Text.Json;
using Newtonsoft.Json;
using Infoseed.MessagingPortal.ItemAdditionsCategorys.Dtos;
using Infoseed.MessagingPortal.InfoSeedParser;
using InfoSeedParser.ConfigrationFile;
using InfoSeedParser;
using InfoSeedParser.Interfaces;
using InfoSeedParser.Parsers;
using Infoseed.MessagingPortal.Loyalty;
using Infoseed.MessagingPortal.Booking.Dtos;
using Infoseed.MessagingPortal.Contacts.Dtos;
using Infoseed.MessagingPortal.General;

namespace Infoseed.MessagingPortal.Items
{
    public class ItemsAppService : MessagingPortalAppServiceBase, IItemsAppService
    {
        private readonly IRepository<Item, long> _itemRepository;
        private readonly IRepository<ItemAdditions.ItemAdditions, long> _itemAdditionsRepository;
        private readonly IItemsExcelExporter _itemsExcelExporter;
        private readonly IRepository<Menu, long> _lookup_menuRepository;
        private readonly IRepository<ItemCategory, long> _lookup_ItemCategoryRepository;
        private readonly IRepository<ItemAndAdditionsCategory, long> _itemAndAdditionsCategoryRepository;
        private readonly IItemParser _ItemParser;
        private ILoyaltyAppService _loyaltyAppService;
        private readonly IExcelExporterAppService _excelExporterAppServicer;

        public ItemsAppService(IRepository<Item, long> itemRepository, IItemsExcelExporter itemsExcelExporter, IRepository<Menu, long> lookup_menuRepository,
            IRepository<ItemCategory, long> lookup_ItemCategoryRepository, IRepository<ItemAdditions.ItemAdditions,
                long> itemAdditionsRepository, IRepository<ItemAndAdditionsCategory, long> itemAndAdditionsCategoryRepository,
            ILoyaltyAppService loyaltyAppService,IExcelExporterAppService excelExporterAppService)
        {
            _itemAdditionsRepository = itemAdditionsRepository;
            _lookup_menuRepository = lookup_menuRepository;
            _lookup_ItemCategoryRepository = lookup_ItemCategoryRepository;
            _itemRepository = itemRepository;
            _itemsExcelExporter = itemsExcelExporter;
            _itemAndAdditionsCategoryRepository = itemAndAdditionsCategoryRepository;
            _ItemParser=new ParserFactory().CreateParserItem(nameof(ItemExcelParser));
            _loyaltyAppService=loyaltyAppService;
            _excelExporterAppServicer = excelExporterAppService;
        }
        public ItemsAppService()
        {


        }
        public async Task<PagedResultDto<GetItemForViewDto>> GetAll(GetAllItemsInput input)
        {
            try
            {
                var filteredItems = _itemRepository.GetAll()
                        .Include(e => e.MenuFk)
                        .Include(e => e.ItemCategoryFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.ItemDescription.Contains(input.Filter) || e.Ingredients.Contains(input.Filter) || e.ItemName.Contains(input.Filter) || e.CategoryNames.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ItemDescriptionFilter), e => e.ItemDescription == input.ItemDescriptionFilter)

                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.ItemDescriptionEnglish.Contains(input.Filter) || e.Ingredients.Contains(input.Filter) || e.ItemNameEnglish.Contains(input.Filter) || e.CategoryNamesEnglish.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ItemDescriptionFilter), e => e.ItemDescriptionEnglish == input.ItemDescriptionFilter)


                        .WhereIf(!string.IsNullOrWhiteSpace(input.IngredientsFilter), e => e.Ingredients == input.IngredientsFilter)

                        .WhereIf(!string.IsNullOrWhiteSpace(input.ItemNameFilter), e => e.ItemName == input.ItemNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ItemNameFilter), e => e.ItemDescriptionEnglish == input.ItemNameFilter)

                        .WhereIf(input.IsInServiceFilter.HasValue && input.IsInServiceFilter > -1, e => (input.IsInServiceFilter == 1 && e.IsInService) || (input.IsInServiceFilter == 0 && !e.IsInService))

                        .WhereIf(!string.IsNullOrWhiteSpace(input.CategoryNamesFilter), e => e.CategoryNames == input.CategoryNamesFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.CategoryNamesFilter), e => e.CategoryNamesEnglish == input.CategoryNamesFilter)

                        .WhereIf(input.MinCreationTimeFilter != null, e => e.CreationTime >= input.MinCreationTimeFilter)
                        .WhereIf(input.MaxCreationTimeFilter != null, e => e.CreationTime <= input.MaxCreationTimeFilter)
                        .WhereIf(input.MinDeletionTimeFilter != null, e => e.DeletionTime >= input.MinDeletionTimeFilter)
                        .WhereIf(input.MaxDeletionTimeFilter != null, e => e.DeletionTime <= input.MaxDeletionTimeFilter)
                        .WhereIf(input.MinPriceFilter != null, e => e.Price >= input.MinPriceFilter)
                        .WhereIf(input.PriorityFilter != 0, e => e.Priority >= input.MinPriceFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ImageUriFilter), e => e.ImageUri == input.ImageUriFilter)
                        .WhereIf(input.MinLastModificationTimeFilter != null, e => e.LastModificationTime >= input.MinLastModificationTimeFilter)
                        .WhereIf(input.MaxLastModificationTimeFilter != null, e => e.LastModificationTime <= input.MaxLastModificationTimeFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.MenuNameFilter), e => e.MenuFk != null && e.MenuFk.MenuDescription == input.MenuNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.CategoryNameFilter), e => e.ItemCategoryFk != null && e.ItemCategoryFk.Name == input.CategoryNameFilter);

                var pagedAndFilteredItem = filteredItems
                    .OrderBy(input.Sorting ?? "id asc")
                    .PageBy(input);


                var item = from o in pagedAndFilteredItem
                           join o1 in _lookup_menuRepository.GetAll() on o.MenuId equals o1.Id into j1
                           from s1 in j1.DefaultIfEmpty()

                           join o2 in _lookup_ItemCategoryRepository.GetAll() on o.ItemCategoryId equals o2.Id into j2
                           from s2 in j2.DefaultIfEmpty()

                           select new GetItemForViewDto()
                           {
                               Item = new ItemDto
                               {
                                   ItemDescription = o.ItemDescription,
                                   ItemDescriptionEnglish = o.ItemDescriptionEnglish,
                                   Ingredients = o.Ingredients,
                                   ItemName = o.ItemName,
                                   ItemNameEnglish = o.ItemNameEnglish,
                                   IsInService = o.IsInService,
                                   CategoryNames = o.CategoryNames,
                                   CategoryNamesEnglish = o.CategoryNamesEnglish,
                                   CreationTime = o.CreationTime,
                                   DeletionTime = o.DeletionTime,
                                   LastModificationTime = o.LastModificationTime,
                                   ImageUri = o.ImageUri,
                                   Price = o.Price,
                                   Priority = o.Priority,
                                   Id = o.Id,
                                   SKU = o.SKU,
                                   MenuType = o.MenuType
                               },
                               MenuName = s1 == null || s1.MenuName == null ? "" : s1.MenuName.ToString(),
                               MenuNameEnglish = s1 == null || s1.MenuNameEnglish == null ? "" : s1.MenuNameEnglish.ToString(),
                               CategoryName = s2 == null || s2.Name == null ? "" : s2.Name.ToString(),
                               CategoryNameEnglish = s2 == null || s2.NameEnglish == null ? "" : s2.NameEnglish.ToString()
                           };


                var totalCount = await filteredItems.CountAsync();

                return new PagedResultDto<GetItemForViewDto>(totalCount,await item.ToListAsync());
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<GetItemForViewDto> GetItemForView(long id)
        {
            try
            {
                var item = await _itemRepository.GetAsync(id);
                var output = new GetItemForViewDto { Item = ObjectMapper.Map<ItemDto>(item) };
                return output;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<GetItemForEditOutput> GetItemForEdit(EntityDto<long> input)
        {
            try
            {
                var item = await _itemRepository.FirstOrDefaultAsync(input.Id);

                var output = new GetItemForEditOutput { Item = ObjectMapper.Map<CreateOrEditItemDto>(item) };

                return output;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public async Task<long> CreateOrEdit(CreateOrEditItemDto input)
        {

            try
            {
                if (input.ItemDiscount != 0)
                {
                    input.OldPrice = input.Price - (input.Price * (input.ItemDiscount / 100));
                    
                    //var discount = decimal.Parse(input.Ingredients);

                    //var y = input.Price * (discount / 100);
                    //var d = input.Price - y;
                    //input.OldPrice = d;

                }
                else
                {
                    input.OldPrice = null;
                    input.Ingredients = null;

                }

                if (input.Id == null)
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

        public async Task<long> CreateOrEditItems(CreateOrEditItemDto input)
        {

            try
            {
                input.TenantId = AbpSession.TenantId.Value;
                input.CreatorUserId=AbpSession.UserId;

                var GetItemLoyaltyLogModel = _loyaltyAppService.GetItemLoyaltyLog(input.Id, input.TenantId);

                if (GetItemLoyaltyLogModel.LoyaltyDefinitionId==0)
                {

                    input.LoyaltyDefinitionId=_loyaltyAppService.GetAll().Id;
                    input.OriginalLoyaltyPoints=input.LoyaltyPoints;
                }
                else
                {
                    input.LoyaltyDefinitionId=GetItemLoyaltyLogModel.LoyaltyDefinitionId;
                    input.OriginalLoyaltyPoints=GetItemLoyaltyLogModel.OriginalLoyaltyPoints;
                }
                input.LoyaltyPoints =_loyaltyAppService.ConvertPriceToPoints(input.Price, input.LoyaltyPoints, input.OriginalLoyaltyPoints, input.LoyaltyDefinitionId.Value);
                input.LoyaltyDefinitionId=_loyaltyAppService.GetAll().Id;

                if (input.LoyaltyPoints!=input.OriginalLoyaltyPoints)
                {

                    input.IsOverrideLoyaltyPoints=true;
                    input.OriginalLoyaltyPoints=input.LoyaltyPoints;

                }

                if (input.ItemDiscount != 0)
                {
                    input.OldPrice = input.Price - (input.Price * (input.ItemDiscount / 100));
                }
                else
                {
                    if (AbpSession.TenantId.Value != 34)
                    {
                        input.OldPrice = null;
                        input.Ingredients = null;
                    }
                }

                string objspecifications = string.Empty;
                string objItemAndAdditionsCategory = string.Empty;
                input.TenantId = AbpSession.TenantId.Value;

                if (input.lstItemSpecificationsDto != null && input.lstItemSpecificationsDto.Count > 0)
                {
                    objspecifications = JsonConvert.SerializeObject(input.lstItemSpecificationsDto);
                }

                if (input.lstItemAndAdditionsCategoryDto != null && input.lstItemAndAdditionsCategoryDto.Count > 0)
                {
                    objItemAndAdditionsCategory = JsonConvert.SerializeObject(input.lstItemAndAdditionsCategoryDto);
                }

                if (input.Status_Code==null)
                {
                    input.Status_Code=2;
                }

                if (input.Id == null)
                {
                    input.LoyaltyPoints =_loyaltyAppService.ConvertPriceToPoints(input.Price, input.LoyaltyPoints, input.OriginalLoyaltyPoints, input.LoyaltyDefinitionId.Value);
                    foreach (var item in input.lstItemImages)
                    {
                        if (item.IsMainImage)
                        {
                            input.ImageUri = item.ImageUrl;
                        }
                    }
                    var entityId = createItems(JsonConvert.SerializeObject(input), objspecifications, objItemAndAdditionsCategory);
                    input.Id = entityId;
                    foreach (var item in input.lstItemImages)
                    {
                        item.ItemId = entityId;
                    }
                    CreateItemImages(input.lstItemImages, input.Id);
                    return entityId;
                }
                else
                {
                    if (input.lstItemImages != null)
                    {
                        foreach (var item in input.lstItemImages)
                        {
                            if (item.IsMainImage)
                            {
                                input.ImageUri = item.ImageUrl;
                            }
                        }
                    }
                    
                    CreateItemImages(input.lstItemImages,input.Id);
                    editItems(input.Id.Value,JsonConvert.SerializeObject(input), objspecifications, objItemAndAdditionsCategory);
                }
                return 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void CreateItemImages(List<ItemImagesModel> lstItemImages, long? itemId)
        {
            try
            {
                DeleteItemImages(itemId);
                var SP_Name = Constants.Item.SP_ItemImagesAdd;
                var sqlParameters = new List<SqlParameter>
                {
                    new SqlParameter("@ItemImagesJson",JsonConvert.SerializeObject(lstItemImages)),
                };
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        } 
        private void DeleteItemImages(long? itemId)
        {
            try
            {
                if (itemId != null)
                {
                    var SP_Name = Constants.Item.SP_ItemImagesDelete;
                    var sqlParameters = new List<SqlParameter> {
                        new SqlParameter("@ItemId",itemId)
                    };
                    SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        protected virtual async Task<long> Create(CreateOrEditItemDto input)
        {
            try
            {
                var item = ObjectMapper.Map<Item>(input);

                if (AbpSession.TenantId != null)
                {
                    item.TenantId = (int?)AbpSession.TenantId;
                }

                var entityId = await _itemRepository.InsertAndGetIdAsync(item);


                if (input.itemAdditionDtos != null)
                {
                    foreach (var add in input.itemAdditionDtos)
                    {

                        if (add.itemId == null || add.itemId == 0)
                        {
                            await CreateitemAddition(add, entityId);
                        }
                        else
                        {
                            await UpdateitemAddition(add);
                        }
                    }
                }

                return entityId;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        protected virtual async Task CreateitemAddition(ItemAdditionDto input, long itemId)
        {
            try
            {
                ItemAdditions.ItemAdditions item = new ItemAdditions.ItemAdditions();
                item.Name = input.Name;
                item.NameEnglish = input.NameEnglish;
                item.Price = input.price;
                item.SKU = input.SKU;
                item.ItemId = itemId;

                if (AbpSession.TenantId != null)
                {
                    item.TenantId = (int?)AbpSession.TenantId;
                }

                var entityId = await _itemAdditionsRepository.InsertAndGetIdAsync(item);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        protected virtual async Task Update(CreateOrEditItemDto input)
        {
            try
            {
                var item = await _itemRepository.FirstOrDefaultAsync((long)input.Id);

                ObjectMapper.Map(input, item);
                item.TenantId = AbpSession.TenantId;
                if (input.itemAdditionDtos!=null)
                {
                    foreach (var add in input.itemAdditionDtos)
                    {
                        if (add.Id == 0)
                        {
                            await CreateitemAddition(add, (long)input.Id);
                        }
                        else
                        {
                            await UpdateitemAddition(add);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }



        }
        protected virtual async Task UpdateitemAddition(ItemAdditionDto input)
        {
            try
            {
                var item = await _itemAdditionsRepository.FirstOrDefaultAsync((long)input.Id);

                item.Name = input.Name;
                item.NameEnglish = input.NameEnglish;
                item.Price = input.price;
                item.SKU = input.SKU;
                item.ItemId = input.itemId;

                await _itemAdditionsRepository.UpdateAsync(item);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public bool DeleteItem(long itemId)
        {
            return deleteItem(itemId);
        }
        public ItemDto GetItemInfoForBot(long id, int tenantID)
        {
            return getItemInfoForBot(id, tenantID);
        }
        public List<ItemDto> GetItemCTown(int TenantID, int menu, long ItemSubCategoryId, out int totalCount, int PageSize = 20, int PageNumber = 0, string Search = "", int IsSort = 0, int? OrderByPrice = null, int? OrderByDiscount = null, int IsDescOrder = 0,bool isvisible=false)
        {
            return getItemCTown(TenantID, menu, ItemSubCategoryId, out totalCount, PageSize, PageNumber, Search, IsSort, OrderByPrice, OrderByDiscount, IsDescOrder, isvisible);
        }
        public ItemDto GetItemById(long id, int? tenantID = null, bool isFromMenu = false)
        {
            try
            {
                int? TenantId = 0;

                if (tenantID.HasValue)
                {
                    TenantId = tenantID.Value;
                }
                else
                {
                    TenantId = (int?)AbpSession.TenantId;

                }

                if (!isFromMenu)
                {

                    return getItemsById(id, TenantId, isFromMenu);
                }
                else
                {

                    return getItemsById(id, TenantId, isFromMenu);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public async void deleteItemAndAdditionsCategorys(long input)
        {
            await _itemAndAdditionsCategoryRepository.DeleteAsync(input);
        }
        public async Task<FileDto> GetItemsToExcel(GetAllItemsForExcelInput input)
        {
            try
            {
                var filteredItems = _itemRepository.GetAll()
                         .Include(e => e.MenuFk)
                        .Include(e => e.ItemCategoryFk)
                        .WhereIf(true, e => e.IsDeleted == false)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.IngredientsFilter), e => e.Ingredients == input.IngredientsFilter)
                        .WhereIf(input.IsInServiceFilter.HasValue && input.IsInServiceFilter > -1, e => (input.IsInServiceFilter == 1 && e.IsInService) || (input.IsInServiceFilter == 0 && !e.IsInService))
                        .WhereIf(input.MinCreationTimeFilter != null, e => e.CreationTime >= input.MinCreationTimeFilter)
                        .WhereIf(input.MaxCreationTimeFilter != null, e => e.CreationTime <= input.MaxCreationTimeFilter)
                        .WhereIf(input.MinDeletionTimeFilter != null, e => e.DeletionTime >= input.MinDeletionTimeFilter)
                        .WhereIf(input.MaxDeletionTimeFilter != null, e => e.DeletionTime <= input.MaxDeletionTimeFilter)
                        .WhereIf(input.MinPriceFilter != null, e => e.Price >= input.MinPriceFilter)
                        .WhereIf(input.PriorityFilter != 0, e => e.Priority >= input.PriorityFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ImageUriFilter), e => e.ImageUri == input.ImageUriFilter)
                        .WhereIf(input.MinLastModificationTimeFilter != null, e => e.LastModificationTime >= input.MinLastModificationTimeFilter)
                        .WhereIf(input.MaxLastModificationTimeFilter != null, e => e.LastModificationTime <= input.MaxLastModificationTimeFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.MenuNameFilter), e => e.MenuFk != null && e.MenuFk.MenuName == input.MenuNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.CategoryNameFilter), e => e.ItemCategoryFk != null && e.ItemCategoryFk.Name == input.CategoryNameFilter);

                var query = (from o in filteredItems
                             join o1 in _lookup_menuRepository.GetAll() on o.MenuId equals o1.Id into j1
                             from s1 in j1.DefaultIfEmpty()

                             join o2 in _lookup_ItemCategoryRepository.GetAll() on o.ItemCategoryId equals o2.Id into j2
                             from s2 in j2.DefaultIfEmpty()
                             select new GetItemForViewDto()
                             {
                                 Item = new ItemDto
                                 {
                                     ItemDescription = o.ItemDescription,
                                     ItemDescriptionEnglish = o.ItemDescriptionEnglish,
                                     Ingredients = o.Ingredients,
                                     ItemName = o.ItemName,
                                     ItemNameEnglish = o.ItemNameEnglish,
                                     IsInService = o.IsInService,
                                     CategoryNames = o.CategoryNames,
                                     CategoryNamesEnglish = o.CategoryNamesEnglish,
                                     CreationTime = o.CreationTime,
                                     DeletionTime = o.DeletionTime,
                                     LastModificationTime = o.LastModificationTime,
                                     ImageUri = o.ImageUri,
                                     Price = o.Price,
                                     Priority = o.Priority,
                                     Id = o.Id,
                                     SKU = o.SKU,
                                     MenuType = o.MenuType
                                 },
                                 MenuName = s1 == null || s1.MenuName == null ? "" : s1.MenuName.ToString(),
                                 MenuNameEnglish = s1 == null || s1.MenuNameEnglish == null ? "" : s1.MenuNameEnglish.ToString(),
                                 CategoryName = s2 == null || s2.Name == null ? "" : s2.Name.ToString(),
                                 CategoryNameEnglish = s2 == null || s2.NameEnglish == null ? "" : s2.NameEnglish.ToString()
                             });


                var itemListDtos = await query.ToListAsync();

                return _itemsExcelExporter.ExportToFile(itemListDtos);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }


        public FileDto ExportItemsToExcel(long categoryId, long subCategoryId)
        {
            List<ItemDto> items = getItemByCategoryAndSubCategory(categoryId, subCategoryId);
            if (items != null)
            {
                return _excelExporterAppServicer.ExportItemsToExcel(items);
            }
            else
            {
                return new FileDto();
            }
        }

        
        [HttpPost("GetImageURL")]
        public async Task<string> GetImageURL([FromForm] GetImageURLModel model)
        {
            try
            {
                var url = "";
                if (model.FormFile != null)
                {
                    if (model.FormFile.Length > 0)
                    {
                        var formFile = model.FormFile;
                        long ContentLength = formFile.Length;
                        byte[] fileData = null;
                        using (var ms = new MemoryStream())
                        {
                            formFile.CopyTo(ms);
                            fileData = ms.ToArray();
                        }

                        AzureBlobProvider azureBlobProvider = new AzureBlobProvider();
                        AttachmentContent attachmentContent = new AttachmentContent()
                        {
                            Content = fileData,
                            Extension = Path.GetExtension(formFile.FileName),
                            MimeType = formFile.ContentType,

                        };

                        url = await azureBlobProvider.Save(attachmentContent);

                    }
                }

                return url;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public List<ItemDto> GetItemsByMenuID(long menuID, long itemCategoryId = 0, long itemSubCategoryId = 0, int pageNumber = 0, int pageSize = 20)
        {
            return GetAllItems(menuID, itemCategoryId, itemSubCategoryId, pageNumber, pageSize);
        }

        public List<ItemDto> GetItemsBySubGategory(int tenantID, int menuType, long itemSubCategoryId, int languageBotId, int pageNumber, int pageSize, out int totalCount, string Search = "")
        {
            return GetAllItemsBysubCategory(tenantID, menuType, itemSubCategoryId, languageBotId, pageNumber, pageSize, out totalCount,Search);
        }
        [Route("iItemUploadExcelFileAsync")]
        [HttpPost]
        public async Task<string> iItemUploadExcelFileAsync([FromForm] UploadFileModel file, int TenantId)
        {
            try
            {
                if (file == null || file.FormFile.Length == 0)
                    return "NotFound";

                var formFile = file.FormFile;

                byte[] fileData = null;
                using (var ms = new MemoryStream())
                {
                    formFile.CopyTo(ms);
                    fileData = ms.ToArray();
                }

                var MenuList = _ItemParser.Parse(new ParseConfig()
                {
                    config2 = new ItmeConfigrationExcelFile(),
                    FileData = fileData,
                    FileName = formFile.FileName,
                    Parser = nameof(ItemExcelParser)
                });

                var itemList = MenuList.Item;

                ItemUpdate(TenantId, JsonConvert.SerializeObject(itemList, Formatting.Indented));

                return "ok";
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public List<ItemDto> GetLoyaltyItems(int tenantID, int menuType, int languageBotId, int pageNumber, int pageSize, out int totalCount, string Search = "")
        {
            return GetAllLoyaltyItems(tenantID, menuType, languageBotId, pageNumber, pageSize, out totalCount , Search);
        }










        #region Private Methods


        private void ItemUpdate(int TenantId, string ItemUpdateJson)
        {
            try
            {
                var SP_Name = "[dbo].[ItemPricesUpdate]";

                var sqlParameters = new List<SqlParameter> {
                    new SqlParameter("@ItemJson",ItemUpdateJson)
                   ,new SqlParameter("@TenantId",TenantId)
                };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private bool deleteItem(long itemId)
        {
            try
            {
                var SP_Name = Constants.Item.SP_ItemDelete;
                var sqlParameters = new List<SqlParameter> {
                    new SqlParameter("@ItemId",itemId),
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
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private List<ItemDto> GetAllItems(long menuID, long itemCategoryId = 0, long itemSubCategoryId = 0, int pageNumber=0, int pageSize=20)
        {

            try
            {
                List<ItemDto> lstItemDto = new List<ItemDto>();
                var SP_Name = "[dbo].[ItemsByMenuIdGet]";
                var sqlParameters = new List<SqlParameter> {
                     new SqlParameter("@TenantId",(int?)AbpSession.TenantId)
                    ,new SqlParameter("@MenuID",menuID)
                    ,new SqlParameter("@ItemCategoryId",itemCategoryId)
                    ,new SqlParameter("@ItemSubCategoryId",itemSubCategoryId)
                    ,new SqlParameter("@PageNumber",pageNumber)
                    ,new SqlParameter("@PageSize",pageSize)
                };

                lstItemDto = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),
                MapItemsMenu, AppSettingsModel.ConnectionStrings).ToList();

                return lstItemDto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private List<ItemDto> GetAllLoyaltyItems(int tenantID, int menuType, int languageBotId, int pageNumber, int pageSize, out int totalCount, string Search = "")
        {

            try
            {
                if (pageSize == 0)
                {
                    pageSize = 50;
                }
                List<ItemDto> lstItemDto = new List<ItemDto>();
                var SP_Name = "[dbo].[ItemsByLoyaltyGet]";
                var sqlParameters = new List<SqlParameter> {
                    new SqlParameter("@TenantId",tenantID)
                    ,new SqlParameter("@MenuType",menuType)

                    ,new SqlParameter("@LanguageBotId",languageBotId)
                    ,new SqlParameter("@PageNumber",pageNumber)
                    ,new SqlParameter("@PageSize",pageSize)

                };
                if (Search != "" && Search != null && Search != "null")
                {
                    sqlParameters.Add(new SqlParameter("@Search", Search));
                }
                var OutputParameter = new SqlParameter();
                OutputParameter.SqlDbType = SqlDbType.BigInt;
                OutputParameter.ParameterName = "@TotalCount";
                OutputParameter.Direction = ParameterDirection.Output;

                sqlParameters.Add(OutputParameter);


                lstItemDto = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),
                MapItemsMenu, AppSettingsModel.ConnectionStrings).ToList();
                totalCount = Convert.ToInt32(OutputParameter.Value);

                return lstItemDto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private List<ItemDto> GetAllItemsBysubCategory(int tenantID, int menuType, long itemSubCategoryId, int languageBotId, int pageNumber, int pageSize, out int totalCount , string Search = "")
        {

            try
            {
                if (pageSize == 0)
                {
                    pageSize = 50;
                }
                List<ItemDto> lstItemDto = new List<ItemDto>();
                var SP_Name = "[dbo].[ItemsByItemSubCategoryIdGet]";
                var sqlParameters = new List<SqlParameter> {
                     new SqlParameter("@TenantId",tenantID)
                    ,new SqlParameter("@MenuType",menuType)
                    
                    ,new SqlParameter("@LanguageBotId",languageBotId)
                    ,new SqlParameter("@PageNumber",pageNumber)
                    ,new SqlParameter("@PageSize",pageSize)

                };
                if (Search != "" && Search != null && Search != "null")
                {
                    sqlParameters.Add(new SqlParameter("@Search", Search));
                }
                else
                {
                    sqlParameters.Add(new SqlParameter("@ItemSubCategoryId", itemSubCategoryId));
                }
                var OutputParameter = new SqlParameter();
                OutputParameter.SqlDbType = SqlDbType.BigInt;
                OutputParameter.ParameterName = "@TotalCount";
                OutputParameter.Direction = ParameterDirection.Output;

                sqlParameters.Add(OutputParameter);


                lstItemDto = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),MapItemsMenu, AppSettingsModel.ConnectionStrings).ToList();
                totalCount = Convert.ToInt32(OutputParameter.Value);

                return lstItemDto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private ItemDto getItemsById(long itemId, int? tenantID, bool isFromMenu = false)
        {

            try
            {
                ItemDto ItemDto = new ItemDto();
                var SP_Name = "[dbo].[ItemsByItemIdGet]";
                var sqlParameters = new List<SqlParameter> {
                    new SqlParameter("@TenantId",tenantID.Value)
                    ,new SqlParameter("@ItemId",itemId)
                };

                if (!isFromMenu)
                {
                    ItemDto = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),MapItems, AppSettingsModel.ConnectionStrings).FirstOrDefault();
                }
                else
                {
                    ItemDto = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),MapItemsMenu, AppSettingsModel.ConnectionStrings).FirstOrDefault();
                }


                return ItemDto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private ItemDto getItemInfoForBot(long itemId, int TenantId)
        {

            try
            {
                ItemDto ItemDto = new ItemDto();
                var SP_Name = "[dbo].[ItemInfoForBotGet]";
                var sqlParameters = new List<SqlParameter> {
             new SqlParameter("@TenantId",TenantId)
            ,new SqlParameter("@ItemId",itemId)


            };
                ItemDto = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),
                MapItemsMenu, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                return ItemDto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private List<ItemDto> getItemCTown(int TenantID, int menu, long ItemSubCategoryId, out int totalCount, int PageSize = 20, int PageNumber = 0, string Search = "", int IsSort = 0, int? OrderByPrice = null, int? OrderByDiscount = null, int IsDescOrder = 0, bool isvisible = false)
        {

            if (PageSize == 0)
            {
                PageSize = 20;
            }


            string connString = AppSettingsModel.ConnectionStrings;// "Server=tcp:info-seed-db-server-prod.database.windows.net,1433;Initial Catalog=InfoSeedDB-Prod;Persist Security Info=False;User ID=devadmin;Password=P@ssw0rd;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";


            if (IsSort == 1)//اقل  سعر
            {

                OrderByPrice = 1;
                IsDescOrder = 0;
            }
            if (IsSort == 2)//اعلى سعر
            {

                OrderByPrice = 1;
                IsDescOrder = 1;
            }


            if (IsSort == 3)//اقل  سعر خصم
            {

                OrderByDiscount = 1;
                IsDescOrder = 0;
            }
            if (IsSort == 4)//اعلى سعر خصم
            {

                OrderByDiscount = 1;
                IsDescOrder = 1;
            }

            var sqlParameters = new List<SqlParameter> {
                new SqlParameter("@MenuType", menu),
                new SqlParameter("@PageNumber", PageNumber),
                new SqlParameter("@PageSize", PageSize),
                new SqlParameter("@OrderByPrice", OrderByPrice),
                new SqlParameter("@OrderByDiscount", OrderByDiscount),
                new SqlParameter("@IsDescOrder", IsDescOrder),
                new SqlParameter("@Isvisible", isvisible)

            };



            if (Search != "" && Search != null && Search != "null")
            {
                sqlParameters.Add(new SqlParameter("@Search", Search));
            }

            if (ItemSubCategoryId == 342 || ItemSubCategoryId == 343 || ItemSubCategoryId == 346 || ItemSubCategoryId == 347 || ItemSubCategoryId == 344 || ItemSubCategoryId == 345)
            {
                sqlParameters.Add(new SqlParameter("@IsOffer", true));

            }
            else
            {
                sqlParameters.Add(new SqlParameter("@IsOffer", false));
                sqlParameters.Add(new SqlParameter("@ItemSubCategoryId", ItemSubCategoryId));
            }

            var OutputParameter = new SqlParameter();
            OutputParameter.SqlDbType = SqlDbType.BigInt;
            OutputParameter.ParameterName = "@TotalCount";
            OutputParameter.Direction = ParameterDirection.Output;

            sqlParameters.Add(OutputParameter);

            IList<ItemDto> result = SqlDataHelper.ExecuteReader("[ctownjo].[ItemCtownSearch]",
                                  sqlParameters.ToArray(),
                                  MapItemsMenu, connString);
            totalCount = Convert.ToInt32(OutputParameter.Value);

            return result.ToList();

        }
        private ItemDto MapItems(IDataReader dataReader)
        {

            try
            {
                ItemDto entity = new ItemDto();


                entity.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");
                entity.Qty = SqlDataHelper.GetValue<int>(dataReader, "Qty");
                entity.TenantId = SqlDataHelper.GetValue<int>(dataReader, "TenantId");
                entity.MenuId = SqlDataHelper.GetValue<long>(dataReader, "MenuId");
                entity.ItemName = SqlDataHelper.GetValue<string>(dataReader, "ItemName");
                entity.ItemNameEnglish = SqlDataHelper.GetValue<string>(dataReader, "ItemNameEnglish");
                entity.ItemDescription = SqlDataHelper.GetValue<string>(dataReader, "ItemDescription");
                entity.ItemDescriptionEnglish = SqlDataHelper.GetValue<string>(dataReader, "ItemDescriptionEnglish");
                entity.ItemSubCategoryId = SqlDataHelper.GetValue<long>(dataReader, "ItemSubCategoryId");
                entity.ItemCategoryId = SqlDataHelper.GetValue<long>(dataReader, "ItemCategoryId");
                entity.ImageUri = SqlDataHelper.GetValue<string>(dataReader, "ImageUri");
                entity.Priority = SqlDataHelper.GetValue<int>(dataReader, "Priority");
                entity.Price = SqlDataHelper.GetValue<decimal>(dataReader, "Price");
                entity.OldPrice = SqlDataHelper.GetValue<decimal>(dataReader, "OldPrice");
                entity.SKU = SqlDataHelper.GetValue<string>(dataReader, "SKU");
                entity.Size = SqlDataHelper.GetValue<string>(dataReader, "Size");


                entity.Status_Code = SqlDataHelper.GetValue<int>(dataReader, "Status_Code");
                entity.Discount = SqlDataHelper.GetValue<string>(dataReader, "Ingredients");
                entity.Ingredients = SqlDataHelper.GetValue<string>(dataReader, "Ingredients");
                entity.IsInService = SqlDataHelper.GetValue<bool>(dataReader, "IsInService");
                entity.CategoryNames = SqlDataHelper.GetValue<string>(dataReader, "CategoryName");
                entity.CategoryNamesEnglish = SqlDataHelper.GetValue<string>(dataReader, "CategoryNameEnglish");
                entity.CreationTime = SqlDataHelper.GetValue<DateTime>(dataReader, "CreationTime");
                entity.DateFrom = SqlDataHelper.GetValue<DateTime>(dataReader, "DateFrom") ?? (DateTime?)null;
                entity.DateTo = SqlDataHelper.GetValue<DateTime>(dataReader, "DateTo") ?? (DateTime?)null;
                entity.SubCategoryName= SqlDataHelper.GetValue<string>(dataReader, "Name");
                entity.SubCategoryNameEnglish= SqlDataHelper.GetValue<string>(dataReader, "NameEnglish");
                entity.IsLoyal = SqlDataHelper.GetValue<bool>(dataReader, "IsLoyal");
                entity.LoyaltyPoints = SqlDataHelper.GetValue<decimal>(dataReader, "LoyaltyPoints");
                //entity.OriginalLoyaltyPoints = SqlDataHelper.GetValue<decimal>(dataReader, "OriginalLoyaltyPoints");
                entity.IsOverrideLoyaltyPoints = SqlDataHelper.GetValue<bool>(dataReader, "IsOverrideLoyaltyPoints");
                entity.LoyaltyDefinitionId = SqlDataHelper.GetValue<long>(dataReader, "LoyaltyDefinitionId");
                entity.Barcode = SqlDataHelper.GetValue<string>(dataReader, "Barcode");


               entity.Status_Code= SqlDataHelper.GetValue<int>(dataReader, "Status_Code");
               entity.ItemDiscount= SqlDataHelper.GetValue<decimal>(dataReader, "ItemDiscount");
                   

              

                if (entity.CreationTime == DateTime.MinValue)
                {
                    entity.CreationTime = DateTime.Now;
                }

                entity.MenuType = SqlDataHelper.GetValue<int>(dataReader, "MenuType");


                entity.AreaIds = SqlDataHelper.GetValue<string>(dataReader, "AreaIds");
                entity.InServiceIds = SqlDataHelper.GetValue<string>(dataReader, "InServiceIds");

                entity.IsQuantitative = SqlDataHelper.GetValue<bool>(dataReader, "IsQuantitative");

                if (!string.IsNullOrEmpty(SqlDataHelper.GetValue<string>(dataReader, "Specifications")))
                {

                    var options = new JsonSerializerOptions { WriteIndented = true };
                    entity.ItemSpecifications = System.Text.Json.JsonSerializer.Deserialize<List<ItemSpecification>>(
                        SqlDataHelper.GetValue<string>(dataReader, "Specifications")

                        , options);
                }

                if (!string.IsNullOrEmpty(SqlDataHelper.GetValue<string>(dataReader, "additionsCategorysListModels")))
                {

                    List<ItemAdditionDto> itemAdditionDtos = new List<ItemAdditionDto>();
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    entity.additionsCategorysListModels = System.Text.Json.JsonSerializer.Deserialize<List<AdditionsCategorysListModel>>(
                        SqlDataHelper.GetValue<string>(dataReader, "additionsCategorysListModels")
                        , options);
                    foreach (var item in entity.additionsCategorysListModels)
                    {
                        if (item.ItemAdditionDto != null)
                            itemAdditionDtos.AddRange(item.ItemAdditionDto);

                    }
                    entity.itemAdditionDtos = itemAdditionDtos.ToArray();
                }

                if (!entity.IsLoyal)
                {

                    entity.LoyaltyPoints =_loyaltyAppService.ConvertPriceToPoints(entity.Price, entity.LoyaltyPoints, entity.OriginalLoyaltyPoints, 0);
                }
                else
                {
                    entity.LoyaltyPoints =_loyaltyAppService.ConvertPriceToPoints(entity.Price, entity.LoyaltyPoints, entity.OriginalLoyaltyPoints, entity.LoyaltyDefinitionId);
                }
                if (!string.IsNullOrEmpty(SqlDataHelper.GetValue<string>(dataReader, "ItemImages")))
                {

                    var options = new JsonSerializerOptions { WriteIndented = true };
                    entity.lstItemImages = System.Text.Json.JsonSerializer.Deserialize<List<ItemImagesModel>>(SqlDataHelper.GetValue<string>(dataReader, "ItemImages"), options);
                }


                return entity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private ItemDto MapItemsMenu(IDataReader dataReader)
        {

            try
            {
                ItemDto entity = new ItemDto();


                entity.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");
                entity.Qty = SqlDataHelper.GetValue<int>(dataReader, "Qty");
                entity.MenuId = SqlDataHelper.GetValue<long>(dataReader, "MenuId");
                entity.ItemName = SqlDataHelper.GetValue<string>(dataReader, "ItemName");
                entity.ItemNameEnglish = SqlDataHelper.GetValue<string>(dataReader, "ItemNameEnglish");
                entity.ItemDescription = SqlDataHelper.GetValue<string>(dataReader, "ItemDescription");
                entity.ItemDescriptionEnglish = SqlDataHelper.GetValue<string>(dataReader, "ItemDescriptionEnglish");
                entity.ItemSubCategoryId = SqlDataHelper.GetValue<long>(dataReader, "ItemSubCategoryId");
                entity.ItemCategoryId = SqlDataHelper.GetValue<long>(dataReader, "ItemCategoryId");
                entity.ImageUri = SqlDataHelper.GetValue<string>(dataReader, "ImageUri");
                entity.Priority = SqlDataHelper.GetValue<int>(dataReader, "Priority");
                entity.Price = SqlDataHelper.GetValue<decimal>(dataReader, "Price");
                entity.OldPrice = SqlDataHelper.GetValue<decimal>(dataReader, "OldPrice");
                entity.SKU = SqlDataHelper.GetValue<string>(dataReader, "SKU");
                entity.Size = SqlDataHelper.GetValue<string>(dataReader, "Size");
                entity.Status_Code = SqlDataHelper.GetValue<int>(dataReader, "Status_Code");
                entity.Discount = SqlDataHelper.GetValue<string>(dataReader, "Ingredients");
                entity.Ingredients = SqlDataHelper.GetValue<string>(dataReader, "Ingredients");
                entity.IsInService = SqlDataHelper.GetValue<bool>(dataReader, "IsInService");
                entity.CategoryNames = SqlDataHelper.GetValue<string>(dataReader, "CategoryName");
                entity.CategoryNamesEnglish = SqlDataHelper.GetValue<string>(dataReader, "CategoryNameEnglish");

                try
                {
                    entity.CreationTime = SqlDataHelper.GetValue<DateTime>(dataReader, "CreationTime");
                    entity.DateTo = SqlDataHelper.GetValue<DateTime>(dataReader, "DateTo") ?? (DateTime?)null;
                    entity.DateFrom = SqlDataHelper.GetValue<DateTime>(dataReader, "DateFrom") ?? (DateTime?)null;
                }
                catch
                {


                }

                entity.SubCategoryName= SqlDataHelper.GetValue<string>(dataReader, "Name");
                entity.SubCategoryNameEnglish= SqlDataHelper.GetValue<string>(dataReader, "NameEnglish");
                entity.IsLoyal = SqlDataHelper.GetValue<bool>(dataReader, "IsLoyal");
                entity.LoyaltyPoints = SqlDataHelper.GetValue<decimal>(dataReader, "LoyaltyPoints");
                entity.ItemDiscount = SqlDataHelper.GetValue<decimal>(dataReader, "ItemDiscount");

                entity.IsOverrideLoyaltyPoints = SqlDataHelper.GetValue<bool>(dataReader, "IsOverrideLoyaltyPoints");
                entity.LoyaltyDefinitionId = SqlDataHelper.GetValue<long>(dataReader, "LoyaltyDefinitionId");

                try
                {
                    entity.OriginalLoyaltyPoints = SqlDataHelper.GetValue<decimal>(dataReader, "OriginalLoyaltyPoints");
                }
                catch
                {
                    entity.OriginalLoyaltyPoints =0;

                }
                if (entity.CreationTime == DateTime.MinValue)
                {
                    entity.CreationTime = DateTime.Now;
                }

                entity.MenuType = SqlDataHelper.GetValue<int>(dataReader, "MenuType");


                entity.AreaIds = SqlDataHelper.GetValue<string>(dataReader, "AreaIds");
                entity.InServiceIds = SqlDataHelper.GetValue<string>(dataReader, "InServiceIds");


                entity.IsQuantitative = SqlDataHelper.GetValue<bool>(dataReader, "IsQuantitative");

                if (!string.IsNullOrEmpty(SqlDataHelper.GetValue<string>(dataReader, "Specifications")))
                {

                    var options = new JsonSerializerOptions { WriteIndented = true };
                    entity.ItemSpecifications = System.Text.Json.JsonSerializer.Deserialize<List<ItemSpecification>>(
                        SqlDataHelper.GetValue<string>(dataReader, "Specifications")

                        , options);
                }

                if (!string.IsNullOrEmpty(SqlDataHelper.GetValue<string>(dataReader, "additionsCategorysListModels")))
                {

                    List<ItemAdditionDto> itemAdditionDtos = new List<ItemAdditionDto>();
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    entity.additionsCategorysListModels = System.Text.Json.JsonSerializer.Deserialize<List<AdditionsCategorysListModel>>(
                        SqlDataHelper.GetValue<string>(dataReader, "additionsCategorysListModels")
                        , options);
                    foreach (var item in entity.additionsCategorysListModels)
                    {
                        if (item.ItemAdditionDto != null)
                            itemAdditionDtos.AddRange(item.ItemAdditionDto);

                    }
                    entity.itemAdditionDtos = itemAdditionDtos.ToArray();
                }

                if (!string.IsNullOrEmpty(SqlDataHelper.GetValue<string>(dataReader, "ItemImages")))
                {

                    var options = new JsonSerializerOptions { WriteIndented = true };
                    entity.lstItemImages = System.Text.Json.JsonSerializer.Deserialize<List<ItemImagesModel>>(SqlDataHelper.GetValue<string>(dataReader, "ItemImages"), options);
                }
                if (entity.lstItemImages==null)
                {
                    entity.lstItemImages=new List<ItemImagesModel>();

                    entity.lstItemImages.Add(new ItemImagesModel() { ImageUrl= entity.ImageUri, IsMainImage=true, ItemId=entity.Id, TenantId=entity.TenantId });

                }
        
                return entity;
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        private long createItems(string item, string specifications, string itemAndAdditionsCategory)
        {

            try
            {

                var SP_Name = "[dbo].[ItemAdd]";
                var sqlParameters = new List<SqlParameter> {
                     new SqlParameter("@ItemJson",item)
                     ,new SqlParameter("@CreatorUserId",AbpSession.UserId)
                     ,new SqlParameter("@SpecificationsJson",!string.IsNullOrWhiteSpace(specifications)?specifications: null)
                     ,new SqlParameter("@ItemAndAdditionsCategoryjson",!string.IsNullOrWhiteSpace(itemAndAdditionsCategory)?itemAndAdditionsCategory:null)

                };

                SqlParameter OutsqlParameter = new SqlParameter();
                OutsqlParameter.ParameterName = "@ItemId";
                OutsqlParameter.SqlDbType = System.Data.SqlDbType.BigInt;
                OutsqlParameter.Direction = System.Data.ParameterDirection.Output;

                sqlParameters.Add(OutsqlParameter);

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

                return (long)OutsqlParameter.Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private void editItems(long itemId, string item, string specifications, string itemAndAdditionsCategory)
        {

            try
            {

                var SP_Name = "[dbo].[ItemEdit]";
                var sqlParameters = new List<SqlParameter> {
                     new SqlParameter("@ItemId",itemId)
                     ,new SqlParameter("@ItemJson",item)
                     ,new SqlParameter("@CreatorUserId",AbpSession.UserId)
                     ,new SqlParameter("@SpecificationsJson",!string.IsNullOrWhiteSpace(specifications)?specifications: null)
                     ,new SqlParameter("@ItemAndAdditionsCategoryjson",!string.IsNullOrWhiteSpace(itemAndAdditionsCategory)?itemAndAdditionsCategory:null)

                };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private List<ItemDto> getItemByCategoryAndSubCategory(long categoryId, long subCategoryId)
        {
            try
            {
                List<ItemDto> items = new List<ItemDto>();
                var SP_Name = Constants.Item.SP_ItemByCategoryAndSubCategoryGet;
                var sqlParameters = new List<SqlParameter> {
                    new SqlParameter("@CategoryId", categoryId),
                    new SqlParameter("@SubCategoryId", subCategoryId)
                };

                items = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), MapItems, AppSettingsModel.ConnectionStrings).ToList();
                return items;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion
    }
}