using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using Azure.Storage.Blobs.Models;
using Framework.Data;
using Infoseed.MessagingPortal.Areas;
using Infoseed.MessagingPortal.Asset.Dto;
using Infoseed.MessagingPortal.CaptionBot;
using Infoseed.MessagingPortal.Configuration.Tenants.Dto;
using Infoseed.MessagingPortal.Dto;
using Infoseed.MessagingPortal.ItemAdditions.Dtos;
using Infoseed.MessagingPortal.ItemAdditionsCategorys;
using Infoseed.MessagingPortal.ItemAdditionsCategorys.Dtos;
using Infoseed.MessagingPortal.ItemAndAdditionsCategorys;
using Infoseed.MessagingPortal.ItemAndAdditionsCategorys.Dtos;
using Infoseed.MessagingPortal.Items;
using Infoseed.MessagingPortal.Items.Dtos;
using Infoseed.MessagingPortal.ItemSpecificationsDetails;
using Infoseed.MessagingPortal.Loyalty;
using Infoseed.MessagingPortal.MenuCategories;
using Infoseed.MessagingPortal.MenuCategories.Dtos;
using Infoseed.MessagingPortal.Menus.Dtos;
using Infoseed.MessagingPortal.Menus.Exporting;
using Infoseed.MessagingPortal.Specifications.Dtos;
using InfoSeedParser;
using InfoSeedParser.Interfaces;
using InfoSeedParser.Parsers;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Menus
{
    public class MenusAppService : MessagingPortalAppServiceBase, IMenusAppService
    {
        private readonly IRepository<Menu, long> _menuRepository;
        private readonly IMenusExcelExporter _menusExcelExporter;
        private readonly IRepository<ItemCategory, long> _lookup_menuCategoryRepository;
        private readonly IRepository<Item, long> _itemRepository;
        private readonly IRepository<ItemAdditions.ItemAdditions, long> _itemAdditionsRepository;
        private readonly IRepository<ItemAdditionsCategory, long> _itemAdditionsCategoryRepository;
        private readonly IMenuParser _MenuParser;
        private readonly IRepository<ItemAndAdditionsCategory, long> _itemAndAdditionsCategoryRepository;
        private readonly IRepository<ItemSpecifications.ItemSpecification, long> _ItemSpecificationsRepository;
        private readonly IRepository<SpecificationChoices.SpecificationChoice, long> _SpecificationChoicesRepository;
        private readonly IRepository<Specifications.Specification, long> _SpecificationsRepository;
        private ILoyaltyAppService _loyaltyAppService;
        private readonly IDocumentClient _IDocumentClient;
        public MenusAppService(IRepository<Menu, long> menuRepository, IMenusExcelExporter menusExcelExporter, IRepository<ItemCategory, long> lookup_menuCategoryRepository, IRepository<Item, long> itemRepository, IRepository<ItemAdditions.ItemAdditions, long> itemAdditionsRepository, IRepository<ItemAdditionsCategory, long> itemAdditionsCategoryRepository, IRepository<ItemAndAdditionsCategory, long> itemAndAdditionsCategoryRepository, IRepository<ItemSpecifications.ItemSpecification, long> ItemSpecificationsRepository, IRepository<SpecificationChoices.SpecificationChoice, long> SpecificationChoicesRepository, IRepository<Specifications.Specification, long> SpecificationsRepository, ILoyaltyAppService loyaltyAppService, IDocumentClient IDocumentClient)
        {
            _itemAdditionsRepository = itemAdditionsRepository;
            _itemRepository = itemRepository;
            _menuRepository = menuRepository;
            _menusExcelExporter = menusExcelExporter;
            _lookup_menuCategoryRepository = lookup_menuCategoryRepository;
            _itemAdditionsCategoryRepository = itemAdditionsCategoryRepository;
            _itemAndAdditionsCategoryRepository = itemAndAdditionsCategoryRepository;
            _ItemSpecificationsRepository = ItemSpecificationsRepository;
            _SpecificationChoicesRepository = SpecificationChoicesRepository;
            _SpecificationsRepository = SpecificationsRepository;
            _MenuParser = new ParserFactory().CreateParser(nameof(MenuExcelParser));
            _IDocumentClient= IDocumentClient;
            _loyaltyAppService =loyaltyAppService;
        }
        public MenusAppService()
        {

        }
        public long CreateMenuReminderMessage(MenuReminderMessages messages)
        {
            return createMenuReminderMessage(messages);
        }
        public void UpdateMenuReminderMessage(long id)
        {
            updateMenuReminderMessage(id);
        }
        public List<MenuReminderMessages> GetMenuReminderMessage()
        {
            try
            {
                var SP_Name = Constants.Menu.SP_MenuReminderMessagesGet;
                List<MenuReminderMessages> messages = new List<MenuReminderMessages>();
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                    //new SqlParameter("@TenantId", tenantId) 
                };
                messages = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), MapMenuReminderMessage, AppSettingsModel.ConnectionStrings).ToList();

                return messages;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private MenuReminderMessages MapMenuReminderMessage(IDataReader dataReader)
        {
            MenuReminderMessages booking = new MenuReminderMessages
            {
                Id = SqlDataHelper.GetValue<long>(dataReader, "Id"),
                IsActive = SqlDataHelper.GetValue<bool>(dataReader, "IsActive"),
                ContactId = SqlDataHelper.GetValue<int>(dataReader, "ContactId"),
                CreationDate = SqlDataHelper.GetValue<DateTime>(dataReader, "CreationDate"),
                PhoneNumber = SqlDataHelper.GetValue<string>(dataReader, "PhoneNumber"),
                TenantId = SqlDataHelper.GetValue<int>(dataReader, "TenantId"),
            };

            return booking;
        }
        private long createMenuReminderMessage(MenuReminderMessages messages)
        {
            try
            {
                var SP_Name = Constants.Menu.SP_MenuReminderMessageAdd;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                    new System.Data.SqlClient.SqlParameter("@ContactId",messages.ContactId)
                   ,new System.Data.SqlClient.SqlParameter("@CreationDate",messages.CreationDate)
                   ,new System.Data.SqlClient.SqlParameter("@IsActive",messages.IsActive)

                };


                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.BigInt,
                    ParameterName = "@Id",
                    Direction = ParameterDirection.Output
                };

                sqlParameters.Add(OutputParameter);
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(),AppSettingsModel.ConnectionStrings);

                return (long)OutputParameter.Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void updateMenuReminderMessage(long id)
        {
            try
            {
                var SP_Name = Constants.Menu.SP_MenuReminderMessagesUpdate;
                List<MenuReminderMessages> messages = new List<MenuReminderMessages>();
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                    new System.Data.SqlClient.SqlParameter("@Id", id)
                };
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public MenusEntity GetMenus(int pageNumber , int PageSize,int? TenantId = null)
        {
            return getMenus(pageNumber, PageSize, TenantId);
        }
        public bool Delete(long input)
        {
            return deleteMenu(input);
        }
        //public async Task<GetMenuForViewDto> GetMenuForView(long id)
        //{
        //    try
        //    {
        //        var menu = await _menuRepository.GetAsync(id);

        //        var output = new GetMenuForViewDto { Menu = ObjectMapper.Map<MenuDto>(menu) };

        //        return output;
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }
            
        //}
        public async Task<GetMenuForEditOutput> GetMenuForEdit(EntityDto<long> input)
        {
            try
            {
                var menu = await _menuRepository.FirstOrDefaultAsync(input.Id);
                var output = new GetMenuForEditOutput { Menu = ObjectMapper.Map<CreateOrEditMenuDto>(menu) };
                return output;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }
        public async Task<long> CreateOrEdit(CreateOrEditMenuDto input)
        {
            try
            {
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
        protected virtual async Task<long> Create(CreateOrEditMenuDto input)
        {
            try
            {
                var menu = ObjectMapper.Map<Menu>(input);
                menu.LanguageBotId = 1;
                if (AbpSession.TenantId != null)
                {
                    menu.TenantId = (int?)AbpSession.TenantId;
                }
                var entityId = await _menuRepository.InsertAndGetIdAsync(menu);

                return entityId;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        protected virtual async Task Update(CreateOrEditMenuDto input)
        {
            try
            {
                var menu = await _menuRepository.FirstOrDefaultAsync((long)input.Id);
                menu.LanguageBotId = 1;
                ObjectMapper.Map(input, menu);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        //public async void deleteItemAndAdditionsCategorys(long input)
        //{
        //    await _itemAndAdditionsCategoryRepository.DeleteAsync(input);
        //}

        public async Task<long> CreateOrEditItemAndAdditionsCategorys(CreateOrEditItemAndAdditionsCategoryDto input)
        {
            try
            {
                if (input.AdditionsAndItemId == 0)
                {

                    ItemAndAdditionsCategory itemAndAdditionsCategory = new ItemAndAdditionsCategory();

                    itemAndAdditionsCategory.ItemId = input.ItemId;
                    itemAndAdditionsCategory.SpecificationId = input.SpecificationId;
                    itemAndAdditionsCategory.AdditionsCategorysId = input.AdditionsCategorysId;
                    itemAndAdditionsCategory.MenuType = input.MenuType;
                    itemAndAdditionsCategory.TenantId = (int?)AbpSession.TenantId;


                    var entityIds = await _itemAndAdditionsCategoryRepository.InsertAndGetIdAsync(itemAndAdditionsCategory);


                    return entityIds;
                }
                else
                {
                    ItemAndAdditionsCategory itemAndAdditionsCategory = new ItemAndAdditionsCategory();

                    itemAndAdditionsCategory.Id = input.AdditionsAndItemId;
                    itemAndAdditionsCategory.ItemId = input.ItemId;
                    itemAndAdditionsCategory.SpecificationId = input.SpecificationId;
                    itemAndAdditionsCategory.AdditionsCategorysId = input.AdditionsCategorysId;
                    itemAndAdditionsCategory.MenuType = input.MenuType;
                    itemAndAdditionsCategory.TenantId = (int?)AbpSession.TenantId;




                    await _itemAndAdditionsCategoryRepository.UpdateAsync(itemAndAdditionsCategory);

                    return itemAndAdditionsCategory.Id;
                }


            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }

        //protected virtual async Task<long> Create(CreateOrEditItemAdditionDto input)
        //{
        //    try
        //    {
        //        var ItemAdditions = new ItemAdditions.ItemAdditions();

        //        ItemAdditions.ItemId = input.ItemId;
        //        ItemAdditions.Name = input.Name;
        //        ItemAdditions.NameEnglish = input.NameEnglish;
        //        ItemAdditions.Price = input.Price;
        //        ItemAdditions.TenantId = (int?)AbpSession.TenantId;
        //        ItemAdditions.ItemAdditionsCategoryId = input.ItemAdditionsCategoryId;
        //        ItemAdditions.ItemId = null;
        //        ItemAdditions.MenuType = input.MenuType;
        //        ItemAdditions.ImageUri = input.ImageUri;

        //        var entityId = await _itemAdditionsRepository.InsertAndGetIdAsync(ItemAdditions);

        //        return entityId;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}



        public async Task<FileDto> GetMenusToExcel(GetAllMenusForExcelInput input)
        {
            try
            {
                var filteredMenus = _menuRepository.GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.MenuDescription.Contains(input.Filter) || e.MenuDescription.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.MenuDescriptionEnglish.Contains(input.Filter) || e.MenuDescriptionEnglish.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.MenuNameFilter), e => e.MenuName == input.MenuNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.MenuDescriptionFilter), e => e.MenuDescription == input.MenuDescriptionFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.MenuNameFilter), e => e.MenuNameEnglish == input.MenuNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.MenuDescriptionFilter), e => e.MenuDescriptionEnglish == input.MenuDescriptionFilter)
                        .WhereIf(input.MinEffectiveTimeFromFilter != null, e => e.EffectiveTimeFrom >= input.MinEffectiveTimeFromFilter)
                        .WhereIf(input.MaxEffectiveTimeFromFilter != null, e => e.EffectiveTimeFrom <= input.MaxEffectiveTimeFromFilter)
                        .WhereIf(input.MinEffectiveTimeToFilter != null, e => e.EffectiveTimeTo >= input.MinEffectiveTimeToFilter)
                        .WhereIf(input.MaxEffectiveTimeToFilter != null, e => e.EffectiveTimeTo <= input.MaxEffectiveTimeToFilter)
                        .WhereIf(input.MinTaxFilter != null, e => e.Tax >= input.MinTaxFilter)
                        .WhereIf(input.MaxTaxFilter != null, e => e.Tax <= input.MaxTaxFilter)
                        .WhereIf(input.PriorityFilter != 0, e => e.Priority >= input.PriorityFilter);

                var query = (from o in filteredMenus

                select new GetMenuForViewDto()
                {
                    Menu = new MenuDto
                    {
                        MenuName = o.MenuName,
                        MenuDescription = o.MenuDescription,
                        MenuNameEnglish = o.MenuNameEnglish,
                        MenuDescriptionEnglish = o.MenuDescriptionEnglish,
                        EffectiveTimeFrom = o.EffectiveTimeFrom,
                        EffectiveTimeTo = o.EffectiveTimeTo,
                        Tax = o.Tax,
                        Priority = o.Priority,
                        Id = o.Id
                    },
                });


                var menuListDtos = await query.ToListAsync();

                return _menusExcelExporter.ExportToFile(menuListDtos);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }
        //public MenuAndItemAndCategorys GetAllWithTenantID(int? TenantID, int? ContactId)
        //{
        //    try
        //    {
        //        MenuAndItemAndCategorys menuAndItemAndCategorys = new MenuAndItemAndCategorys();
        //        List<ListMenu> listMenus = new List<ListMenu>();

        //        var ListMenu = GetAllMenuWithTenantID(TenantID).OrderBy(x => x.Priority).ToList();

        //        foreach (var item in ListMenu)
        //        {


        //            listMenus.Add(new ListMenu
        //            {
        //                Id = item.Id,
        //                ImageBgUri = item.ImageBgUri,
        //                ImageUri = item.ImageUri,
        //                MenuName = item.MenuName,
        //                MenuDescription = item.MenuDescription,
        //                MenuNameEnglish = item.MenuNameEnglish,
        //                MenuDescriptionEnglish = item.MenuDescriptionEnglish,
        //                Priority = item.Priority,
        //                RestaurantsType = item.RestaurantsType,
        //                listItems = new List<ListItem>()

        //            });

        //        }
        //        foreach (var item in listMenus)
        //        {
        //            var listItem = GetAllItemWithTenantID(TenantID, item.Id);

        //            foreach (var itemlist in listItem)
        //            {
        //                var Categorie = GetAllitemCategoriesWithTenantID(TenantID, itemlist.Id);

        //                var ListItemAddition = GetAllItemAdditionWithTenantID(TenantID, itemlist.Id);

        //                item.listItems.Add(new ListItem
        //                {
        //                    itemAdditionDtos = ListItemAddition,

        //                    Id = itemlist.Id,
        //                    Priority = itemlist.Priority,
        //                    ImageUri = itemlist.ImageUri,
        //                    IsInService = itemlist.IsInService,
        //                    ItemDescription = itemlist.ItemDescription,
        //                    ItemDescriptionEnglish = itemlist.ItemDescriptionEnglish,
        //                    ItemName = itemlist.ItemName,
        //                    ItemNameEnglish = itemlist.ItemNameEnglish,
        //                    Price = itemlist.Price,
        //                    SKU = itemlist.SKU,

        //                    categorysModle = new CategorysModle
        //                    {
        //                        Id = Categorie.Id,
        //                        Name = Categorie.Name,
        //                        NameEnglish = Categorie.NameEnglish,
        //                        bgImag = item.ImageBgUri,
        //                        logoImag = item.ImageUri,

        //                    }

        //                });

        //            }

        //        }

        //        menuAndItemAndCategorys.listMenus = listMenus;
        //        menuAndItemAndCategorys.ContactId = ContactId;
        //        return menuAndItemAndCategorys;
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }
            
        //}
        //public MenuDataModel GetMenuData(int menuId)
        //{
        //    try
        //    {
        //        MenuDataModel menuDataModel = new MenuDataModel();


        //        var Menu = GetAllMenuWithTenantID(AbpSession.TenantId).Where(x => x.Id == menuId).FirstOrDefault();


        //        ListMenu MenuData = new ListMenu
        //        {
        //            Id = Menu.Id,
        //            ImageUri = Menu.ImageUri,
        //            MenuName = Menu.MenuName,
        //            MenuDescription = Menu.MenuDescription,
        //            MenuDescriptionEnglish = Menu.MenuDescriptionEnglish,
        //            MenuNameEnglish = Menu.MenuNameEnglish,
        //            Priority = Menu.Priority,
        //            listItems = new List<ListItem>()

        //        };



        //        var listItem = GetAllItemWithTenantID(AbpSession.TenantId, MenuData.Id);

        //        foreach (var itemlist in listItem)
        //        {
        //            var Categorie = GetAllitemCategoriesWithTenantID(AbpSession.TenantId, itemlist.Id);

        //            var ListItemAddition = GetAllItemAdditionWithTenantID(AbpSession.TenantId, itemlist.Id);

        //            MenuData.listItems.Add(new ListItem
        //            {
        //                itemAdditionDtos = ListItemAddition,
        //                Id = itemlist.Id,
        //                Priority = itemlist.Priority,
        //                ImageUri = itemlist.ImageUri,
        //                IsInService = itemlist.IsInService,
        //                ItemDescription = itemlist.ItemDescription,
        //                ItemDescriptionEnglish = itemlist.ItemDescriptionEnglish,
        //                ItemName = itemlist.ItemName,
        //                ItemNameEnglish = itemlist.ItemNameEnglish,
        //                Price = itemlist.Price,
        //                SKU = itemlist.SKU,
        //                categorysModle = new CategorysModle
        //                {
        //                    Id = Categorie.Id,
        //                    Name = Categorie.Name,
        //                    NameEnglish = Categorie.NameEnglish
        //                }

        //            });

        //        }



        //        menuDataModel.MenuData = MenuData;
        //        return menuDataModel;
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }
            
        //}
        public async Task<List<GetItemAdditionsCategorysModel>> GetAddOnsCategorys()
        {
            try
            {

                var CategoryList = GetItemAdditionsCategory((int?)AbpSession.TenantId).ToList();
                return CategoryList.OrderBy(p => p.categoryPriority).ToList();

            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }
        public ItemCategoryEntity GetItemAdditionsCategories( int pageNumber = 0, int pageSize = 10)
        {
            return getItemAdditionsCategories(pageNumber,pageSize);
        }
        public ItemCategoryEntity GetSpecificationsCategory(int pageNumber = 0, int pageSize = 10)
        {
            return getSpecificationsCategory(pageNumber, pageSize);
        }
        public async Task<List<ItemAdditionDto>> GetItemAdditionChoicesModel(long ItemAdditionsCategoryId)
        {

            var model = GetItemAdditionChoicesById((int?)AbpSession.TenantId, ItemAdditionsCategoryId);

            foreach (var item in model)
            {
                var GetSpecificationsLoyaltyLog = _loyaltyAppService.GetAdditionsLoyaltyLog(item.Id);

                item.LoyaltyPoints =_loyaltyAppService.ConvertPriceToPoints(item.price, item.LoyaltyPoints, GetSpecificationsLoyaltyLog.OriginalLoyaltyPoints, GetSpecificationsLoyaltyLog.LoyaltyDefinitionId);

                //item.LoyaltyDefinitionId=_loyaltyAppService.GetAll().Id;
                //item.CreatedBy=AbpSession.UserId.Value;

            }

            return model;
        }
        public void SaveSetting(long menuId, WorkModel workModel)
        {
            try
            {

                workModel.StartDateFri = checkValidValue(workModel.StartDateFri);
                workModel.StartDateSat = checkValidValue(workModel.StartDateSat);
                workModel.StartDateSun = checkValidValue(workModel.StartDateSun);
                workModel.StartDateMon = checkValidValue(workModel.StartDateMon);
                workModel.StartDateTues = checkValidValue(workModel.StartDateTues);
                workModel.StartDateWed = checkValidValue(workModel.StartDateWed);
                workModel.StartDateThurs = checkValidValue(workModel.StartDateThurs);




                workModel.EndDateFri = checkValidValue(workModel.EndDateFri);
                workModel.EndDateSat = checkValidValue(workModel.EndDateSat);
                workModel.EndDateSun = checkValidValue(workModel.EndDateSun);
                workModel.EndDateMon = checkValidValue(workModel.EndDateMon);
                workModel.EndDateTues = checkValidValue(workModel.EndDateTues);
                workModel.EndDateWed = checkValidValue(workModel.EndDateWed);
                workModel.EndDateThurs = checkValidValue(workModel.EndDateThurs);

                workModel.StartDateFriSP = checkValidValue(workModel.StartDateFriSP);
                workModel.StartDateSatSP = checkValidValue(workModel.StartDateSatSP);
                workModel.StartDateSunSP = checkValidValue(workModel.StartDateSunSP);
                workModel.StartDateMonSP = checkValidValue(workModel.StartDateMonSP);
                workModel.StartDateTuesSP = checkValidValue(workModel.StartDateTuesSP);
                workModel.StartDateWedSP = checkValidValue(workModel.StartDateWedSP);
                workModel.StartDateThursSP = checkValidValue(workModel.StartDateThursSP);

                workModel.EndDateFriSP = checkValidValue(workModel.EndDateFriSP);
                workModel.EndDateSatSP = checkValidValue(workModel.EndDateSatSP);
                workModel.EndDateSunSP = checkValidValue(workModel.EndDateSunSP);
                workModel.EndDateMonSP = checkValidValue(workModel.EndDateMonSP);
                workModel.EndDateTuesSP = checkValidValue(workModel.EndDateTuesSP);
                workModel.EndDateWedSP = checkValidValue(workModel.EndDateWedSP);
                workModel.EndDateThursSP = checkValidValue(workModel.EndDateThursSP);

                MenuSettingUpdate(menuId, JsonConvert.SerializeObject(workModel, Formatting.Indented));
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }
        public WorkModel GetMenuSetting(long menuId)
        {
            return getMenuSetting(menuId);
        }


        public async Task<List<SpecificationChoicesDto>> GetSpecificationChoicesModel(long SpecificationId)
        {

            var model = GetSpecificationChoicesById((int?)AbpSession.TenantId, SpecificationId);

            foreach (var item in model)
            {
                var GetSpecificationsLoyaltyLog = _loyaltyAppService.GetSpecificationsLoyaltyLog(item.Id);

                item.LoyaltyPoints =_loyaltyAppService.ConvertPriceToPoints(item.Price, item.LoyaltyPoints, GetSpecificationsLoyaltyLog.OriginalLoyaltyPoints, GetSpecificationsLoyaltyLog.LoyaltyDefinitionId);

            }
            return model;
        }


        public async Task<List<GetSpecificationsCategorysModel>> GetSpecificationsCategorys()
        {
            var CategoryList = GetSpecificationsCategory((int?)AbpSession.TenantId).ToList();
            return CategoryList.OrderBy(p => p.categoryPriority).ToList();       
        }


        private async Task SendToRestaurantsBot(int id)
        {
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, AppSettingsModel.BotApi+"api/RestaurantsChatBot/DeleteCache?TenantId="+id);
                request.Headers.Add("accept", "text/plain");
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();


            }
            catch
            {

            }




        }
        public async Task UpdateMenuImages(string image, string imageBG)
        {
            try
            {
                var SP_Name = Constants.Menu.SP_MenuImagesUpdate;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                     new System.Data.SqlClient.SqlParameter("@TenantId",AbpSession.TenantId.Value)
                    ,new System.Data.SqlClient.SqlParameter("@Image",image)
                    ,new System.Data.SqlClient.SqlParameter("@ImageBG",imageBG)
                };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);
                var itemsCollection = new DocumentCosmoseDB<Infoseed.MessagingPortal.MultiTenancy.TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);

                Infoseed.MessagingPortal.MultiTenancy.TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.TenantId == AbpSession.GetTenantId());
                if(image!=null &&image!="")
                {
                    tenant.Image=image;
                }
                if (imageBG!=null &&imageBG!="")
                {
                    tenant.ImageBg=imageBG;
                }

                await itemsCollection.UpdateItemAsync(tenant._self, tenant);
                SendToRestaurantsBot(AbpSession.TenantId.Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //string connString = AppSettingsModel.ConnectionStrings;
            //using (SqlConnection connection = new SqlConnection(connString))
            //    try
            //    {

            //        using (SqlCommand command = connection.CreateCommand())
            //        {
            //            command.CommandText = "UPDATE AbpTenants SET Image = @Image, ImageBg = @ImageBg  Where Id = @Id";

            //            command.Parameters.AddWithValue("@Id", AbpSession.TenantId);
            //            command.Parameters.AddWithValue("@Image", imag);
            //            command.Parameters.AddWithValue("@ImageBg", imagBg);
            //            connection.Open();
            //            command.ExecuteNonQuery();
            //            connection.Close();
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        throw ex;

            //    }
        }


        public List<CategorysInItemModel> GetCategorysInItem(int menuId)
        {

            try
            {
                List<CategorysInItemModel> categorysInItemModel = new List<CategorysInItemModel>();

                var CategorieList = GetAllitemCategories(AbpSession.TenantId, menuId);
                var listitem232 = GetAllItemWithTenantID(AbpSession.TenantId, menuId).OrderBy(x => x.Priority).ToList();

                foreach (var Categorie in CategorieList)
                {

                    var listitem = listitem232.Where(x => x.ItemCategoryId == Categorie.Id).ToList();




                    foreach (var item in listitem)
                    {

                        var xx = GetAdditionsCategorysForitemList(int.Parse(item.Id.ToString()));

                        var dd = GetSpecificationsAndChoisessList(int.Parse(item.Id.ToString()));
                        item.additionsCategorysListModels = xx;
                        item.ItemSpecifications = dd;

                    }

                    CategorysInItemModel categorysInItemModel1 = new CategorysInItemModel();
                    if (Categorie != null)
                    {


                        categorysInItemModel1.categoryId = Categorie.Id;
                        categorysInItemModel1.categoryName = Categorie.Name;
                        categorysInItemModel1.categoryNameEnglish = Categorie.NameEnglish;
                        categorysInItemModel1.categoryId = Categorie.Id;


                    }


                    if (listitem != null && listitem.Count() > 0)
                    {

                        categorysInItemModel1.listItemInCategories = listitem;


                    }


                    categorysInItemModel.Add(categorysInItemModel1);

                }
                categorysInItemModel.OrderBy(x => x.menuPriority).ToList();
                return categorysInItemModel;
            }
            catch (Exception ex)
            {

                throw ex;
            }



        }
        public RType[] GetRType(int? TenantID)
        {
            try
            {
                List<RType> rType = new List<RType>();

                var ListArea = GetAreasList(TenantID.ToString());
                
                rType.Add(new RType
                {

                    Id = 0,
                    Name = "All"

                });
                foreach (var item in ListArea)
                {
                    rType.Add(new RType
                    {

                        Id = int.Parse(item.Id.ToString()),
                        Name = item.AreaName + "-" + item.AreaCoordinate

                    });

                }
                return rType.ToArray();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }

        public AdditionsCategorysListModel[] GetAdditionsCategorysList()
        {
            try
            {
                List<AdditionsCategorysListModel> rType = new List<AdditionsCategorysListModel>();


                var addCatList = _itemAdditionsCategoryRepository.GetAll().ToList();

                foreach (var add in addCatList)
                {
                    rType.Add(new AdditionsCategorysListModel
                    {
                        Id = int.Parse(add.Id.ToString()),
                        Name = add.Name,
                        NameEnglish = add.NameEnglish

                    });
                }
                return rType.ToArray();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public List<AdditionsCategorysListModel> GetAdditionsCategorysForitemList(int itemId)
        {
            try
            {
                List<AdditionsCategorysListModel> rType = new List<AdditionsCategorysListModel>();

                var CatAndAdd = _itemAndAdditionsCategoryRepository.GetAll().ToList();


                var CatAndAddList = CatAndAdd.Where(x => x.ItemId == itemId).ToList();

                var addCatList = _itemAdditionsCategoryRepository.GetAll().ToList();

                foreach (var item in CatAndAddList)
                {
                    var xx = addCatList.Where(x => x.Id == item.AdditionsCategorysId).ToList();

                    foreach (var add in xx)
                    {
                        rType.Add(new AdditionsCategorysListModel
                        {

                            AdditionsAndItemId = int.Parse(item.Id.ToString()),
                            Id = int.Parse(add.Id.ToString()),
                            Name = add.Name,
                            NameEnglish = add.NameEnglish

                        });



                    }

                }


                return rType;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public List<ItemSpecification> GetSpecificationsAndChoisessList(int itemId)
        {
            try
            {
                List<ItemSpecification> getItemAdditionsCategorysModels = new List<ItemSpecification>();

                var itemSpecificationsList = GetItemSpecifications((int?)AbpSession.TenantId);

                var itemSpecificationsListWhittemID = itemSpecificationsList.Where(x => x.ItemId == itemId).ToList();



                var CategoryList = GetSpecifications((int?)AbpSession.TenantId).ToList();


                var add = GetSpecificationChoices((int?)AbpSession.TenantId);


                foreach (var item in itemSpecificationsListWhittemID)
                {
                    var xx = CategoryList.Where(x => x.Id == item.SpecificationId).ToList();

                    var addlist = add.Where(x => x.SpecificationId == item.SpecificationId);
                    List<SpecificationChoice> itemAdditionDtos = new List<SpecificationChoice>();
                    foreach (var adds in addlist)
                    {
                        adds.LanguageBotId = 1;
                        itemAdditionDtos.Add(new SpecificationChoice
                        {

                            Id = adds.Id,
                            SpecificationChoiceDescription = adds.SpecificationChoiceDescription,
                            SpecificationChoiceDescriptionEnglish = adds.SpecificationChoiceDescriptionEnglish,
                            Price = adds.Price,
                            SpecificationId = adds.SpecificationId,
                            SKU = adds.SKU,
                            LanguageBotId = adds.LanguageBotId

                        });


                    }


                    foreach (var xxitem in xx)
                    {
                        getItemAdditionsCategorysModels.Add(new ItemSpecification
                        {


                            Id = xxitem.Id,
                            ItemSpecificationId = item.Id,
                            TenantId = int.Parse(xxitem.TenantId.ToString()),
                            SpecificationDescription = xxitem.SpecificationDescription,
                            SpecificationDescriptionEnglish = xxitem.SpecificationDescriptionEnglish,
                            Priority = xxitem.Priority,
                            IsMultipleSelection = xxitem.IsMultipleSelection,
                            MaxSelectNumber = xxitem.MaxSelectNumber,
                            IsRequired = item.IsRequired,
                            SpecificationChoices = itemAdditionDtos



                        });
                    }
                }

                return getItemAdditionsCategorysModels.OrderBy(p => p.Priority).ToList();
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public LanguageBot[] GetLanguageBot()
        {
            var list = LanguageBot();

            return list.ToArray();
        }
        public Task CopyeMenu(int MenuId)
        {
            try
            {
                var connString = AppSettingsModel.ConnectionStrings;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@menuId",MenuId)
                };
                SqlDataHelper.ExecuteNoneQuery("dbo.CopyMenu",sqlParameters.ToArray(),connString);
            }
            catch (Exception ex)
            {
                throw ex;

            }

            return Task.CompletedTask;
        }

        public void UpdateItemAdditionDetailsListForView(GetItemAdditionDetailsModel[] input)
        {
            try
            {
                var list = GetItemAdditionDetailsList((int)AbpSession.TenantId);



                foreach (var item in input)
                {
                    var itemAdditionDetailsModel = list.Where(x => x.Id == item.Id).FirstOrDefault();

                    itemAdditionDetailsModel.IsInService = item.IsInServes;

                    UpdateItemAdditionDetails(itemAdditionDetailsModel);


                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public void UpdateItemSpecificationsListForView(GetItemAdditionDetailsModel[] input)
        {
            try
            {
                var list = GetItemSpecificationsDetailList((int)AbpSession.TenantId);
                ItemSpecificationsDetail itemSpecificationsDetail = new ItemSpecificationsDetail();


                foreach (var item in input)
                {
                    var itemAdditionDetailsModel = list.Where(x => x.Id == item.Id).FirstOrDefault();

                    itemAdditionDetailsModel.IsInService = item.IsInServes;




                    UpdateItemSpecificationsDetails(itemAdditionDetailsModel);


                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<GetItemAdditionDetailsModel> GetItemAdditionDetailsListForView(int menuId)
        {
            try
            {
                List<GetItemAdditionDetailsModel> getItemAdditionDetailsModelslist = new List<GetItemAdditionDetailsModel>();
                var DetialAddList = GetItemAdditionDetailsList((int)AbpSession.TenantId);

                var add = _itemAdditionsRepository.GetAll().ToList();


                foreach (var det in DetialAddList)
                {

                    var xxx = add.Where(x => x.Id == det.ItemAdditionId).FirstOrDefault();


                    if (xxx != null)
                    {

                        if (det.MenuType == menuId)
                        {

                            getItemAdditionDetailsModelslist.Add(new GetItemAdditionDetailsModel
                            {

                                Id = det.Id,
                                Name = xxx.Name,
                                NameEnglish = xxx.NameEnglish,
                                IsInServes = det.IsInService

                            });
                        }

                    }

                }


                return getItemAdditionDetailsModelslist;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            

        }

        public List<GetItemAdditionDetailsModel> GetItemSpecificationsListForView(int menuId)
        {
            try
            {
                List<GetItemAdditionDetailsModel> getItemAdditionDetailsModelslist = new List<GetItemAdditionDetailsModel>();
                var DetialAddList = GetItemSpecificationsDetailList((int)AbpSession.TenantId);

                var add = GetSpecificationChoices((int)AbpSession.TenantId);


                foreach (var det in DetialAddList)
                {

                    var xxx = add.Where(x => x.Id == det.SpecificationChoicesId).FirstOrDefault();


                    if (xxx != null)
                    {

                        if (det.MenuType == menuId)
                        {

                            getItemAdditionDetailsModelslist.Add(new GetItemAdditionDetailsModel
                            {

                                Id = int.Parse(det.Id.ToString()),
                                Name = xxx.SpecificationChoiceDescription,
                                NameEnglish = xxx.SpecificationChoiceDescriptionEnglish,
                                IsInServes = det.IsInService

                            });
                        }

                    }
                }


                return getItemAdditionDetailsModelslist;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            

        }

        public List<MenuEntity> GetMenusWithDetails(int TenantID, int MenuType)
        {
            return getMenusWithDetails(TenantID, MenuType);
        }


        public List<CreateOrEditMenuCategoryDto> GetCategorysByMenuID(long menuID)
        {
            return GetCategorys(menuID);
        }

        public long AddSubCatogeory(CreateOrEditMenuSubCategoryDto createOrEditMenuSubCategoryDto)
        {
            return AddSubCatogeories(createOrEditMenuSubCategoryDto);
        }

        public void UpdateSubCatogeory(CreateOrEditMenuSubCategoryDto createOrEditMenuSubCategoryDto)
        {
            UpdateSubCatogeories(createOrEditMenuSubCategoryDto);

        }

        public bool DeleteSubCatogeory(long subCategoryID)
        {
           return deleteSubCategory(subCategoryID);

        }

        public MenuContcatKeyModel MenuContactKeyAddAndGet(MenuContcatKeyModel model)
        {

            var rez = addMenuContactKey(model);
            return rez;
        }
        //public async Task<string> UploadExcelFileAsync([FromForm] UploadFileModel file)
        //{
        //    try
        //    {
        //        if (file == null || file.FormFile.Length == 0)
        //            return "NotFound";

        //        //var filePath = Path.GetTempFileName();
        //        //var path = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot\\UploadFile\\",
        //        //            file.FormFile.FileName);

        //        //using (var stream = new FileStream("wwwroot\\UploadFile\\" + file.FormFile.FileName, FileMode.Create))
        //        //{
        //        //    await file.FormFile.CopyToAsync(stream);
        //        //}

        //        //var _filePath = "wwwroot\\UploadFile\\" + file.FormFile.FileName;

        //        var formFile = file.FormFile;

        //        byte[] fileData = null;
        //        using (var ms = new MemoryStream())
        //        {
        //            formFile.CopyTo(ms);
        //            fileData = ms.ToArray();


        //        }

        //        var MenuList = _MenuParser.Parse(new ParseConfig()
        //        {
        //            config = new ConfigrationExcelFile(),
        //            FileData = fileData,
        //            FileName = formFile.FileName,
        //            Parser = nameof(MenuExcelParser)
        //        });

        //        foreach (var menu in MenuList.Menu)
        //        {
        //            Menu InsertMenu = new Menu
        //            {
        //                MenuName = menu.MenuName,
        //                MenuDescription = menu.MenuDescription,

        //                MenuNameEnglish = menu.MenuNameEnglish,
        //                MenuDescriptionEnglish = menu.MenuDescriptionEnglish,
        //                RestaurantsType = menu.RestaurantsType,
        //                Priority = menu.Priority,
        //                TenantId = AbpSession.TenantId

        //            };

        //            var MenuEntityId = await _menuRepository.InsertAndGetIdAsync(InsertMenu);

        //            var CategoryList = MenuList.Category.Where(x => x.Id == menu.Id).ToList();

        //            foreach (var itemCategory in CategoryList)
        //            {
        //                ItemCategory InsertitemCategory = new ItemCategory
        //                {

        //                    Name = itemCategory.Name,
        //                    NameEnglish = itemCategory.NameEnglish,
        //                    MenuType = itemCategory.MenuType,
        //                    Priority = itemCategory.Priority,
        //                    IsDeleted = false,
        //                    TenantId = AbpSession.TenantId

        //                };

        //                var itemCategoryEntityId = await _lookup_menuCategoryRepository.InsertAndGetIdAsync(InsertitemCategory);


        //                var itemsList = MenuList.Item.Where(x => x.MenuId == menu.Id).ToList();

        //                foreach (var item in itemsList)
        //                {

        //                    Item item1 = new Item
        //                    {

        //                        SKU = item.SKU,
        //                        IsInService = item.IsInService,
        //                        CategoryNames = item.CategoryNames,
        //                        CategoryNamesEnglish = item.CategoryNamesEnglish,
        //                        ItemDescription = item.ItemDescription,
        //                        ItemDescriptionEnglish = item.ItemDescriptionEnglish,
        //                        ItemName = item.ItemName,
        //                        ItemNameEnglish = item.ItemNameEnglish,
        //                        ItemCategoryId = itemCategoryEntityId,
        //                        MenuId = MenuEntityId,
        //                        ImageUri = item.ImageUri,
        //                        Priority = item.Priority,
        //                        CreationTime = DateTime.Now,
        //                        DeletionTime = DateTime.Now,
        //                        LastModificationTime = DateTime.Now,
        //                        MenuType = item.MenuType,
        //                        Price = item.Price,
        //                        TenantId = AbpSession.TenantId



        //                    };

        //                    var itemEntityId = await _itemRepository.InsertAndGetIdAsync(item1);

        //                    var itemsAddationList = MenuList.itemAdditionDtos.Where(x => x.itemId == item.Id).ToList();

        //                    foreach (var itemsAddation in itemsAddationList)
        //                    {
        //                        ItemAdditions.ItemAdditions itemAdditions = new ItemAdditions.ItemAdditions
        //                        {
        //                            ItemId = itemEntityId,
        //                            Name = itemsAddation.Name,
        //                            NameEnglish = itemsAddation.NameEnglish,
        //                            MenuType = itemsAddation.MenuType,
        //                            Price = itemsAddation.price,
        //                            SKU = itemsAddation.SKU,
        //                            TenantId = AbpSession.TenantId


        //                        };
        //                        var itemsAddationEntityId = await _itemAdditionsRepository.InsertAndGetIdAsync(itemAdditions);

        //                    }
        //                }
        //            }
        //        }



        //        //try
        //        //{
        //        //    // Check if file exists with its full path    
        //        //    if (System.IO.File.Exists("wwwroot\\UploadFile\\" + file.FormFile.FileName))
        //        //    {
        //        //        // If file found, delete it    
        //        //        System.IO.File.Delete("wwwroot\\UploadFile\\" + file.FormFile.FileName);
        //        //        Console.WriteLine("File deleted.");
        //        //    }
        //        //    else Console.WriteLine("File not found");
        //        //}
        //        //catch (IOException ioExp)
        //        //{
        //        //    Console.WriteLine(ioExp.Message);
        //        //    return "NotFound";
        //        //}



        //        return "ok";
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }

        //}





        //public async Task CopyeMenu2(int MenuId, int TenantID)
        //{

        //    try
        //    {
        //        var connString = AppSettingsModel.ConnectionStrings;
        //        var sqlParameters = new List<SqlParameter> {
        //                    new SqlParameter("@menuId",MenuId),
        //                    new SqlParameter("@tenantNewId",TenantID)
        //             };
        //        SqlDataHelper.ExecuteNoneQuery(
        //                  "dbo.CopyMenuWithTeneantID",
        //                  sqlParameters.ToArray(),
        //                  connString);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;

        //    }


        //}



        //private List<ItemAdditionDetailsModel> GetItemAdditionDetailsList(int TenantID, int MenuType)
        //{
        //    try
        //    {
        //        //TenantID = "31";
        //        string connString = AppSettingsModel.ConnectionStrings;
        //        string query = "select * from [dbo].[ItemAdditionDetails] where TenantID=" + TenantID + " and MenuType=" + MenuType;


        //        SqlConnection conn = new SqlConnection(connString);
        //        SqlCommand cmd = new SqlCommand(query, conn);
        //        conn.Open();

        //        // create the DataSet 
        //        DataSet dataSet = new DataSet();

        //        // create data adapter
        //        SqlDataAdapter da = new SqlDataAdapter(cmd);
        //        // this will query your database and return the result to your datatable
        //        da.Fill(dataSet);

        //        List<ItemAdditionDetailsModel> itemAdditionDetails = new List<ItemAdditionDetailsModel>();

        //        for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
        //        {

        //            itemAdditionDetails.Add(new ItemAdditionDetailsModel
        //            {
        //                Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
        //                CopiedFromId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["CopiedFromId"]),
        //                ItemAdditionId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemAdditionId"]),
        //                ItemId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemId"]),
        //                MenuType = Convert.ToInt32(dataSet.Tables[0].Rows[i]["MenuType"]),
        //                TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
        //                IsInService = Convert.ToBoolean(dataSet.Tables[0].Rows[i]["IsInService"]),

        //            });



        //        }

        //        conn.Close();
        //        da.Dispose();

        //        return itemAdditionDetails;
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }

        //}







        #region private Methods


        private MenusEntity getMenus(int pageNumber, int pageSize, int? tenantId = null)
        {
            try
            {
                if (tenantId == null)
                {
                    tenantId = AbpSession.TenantId.Value;
                }
                MenusEntity menus = new MenusEntity();
                var SP_Name = Constants.Menu.SP_MenusGet;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@TenantId", AbpSession.TenantId.Value),
                    new System.Data.SqlClient.SqlParameter("@PageNumber", pageNumber),
                    new System.Data.SqlClient.SqlParameter("@PageSize", pageSize)
                };

                var OutputParameter = new System.Data.SqlClient.SqlParameter();
                OutputParameter.SqlDbType = SqlDbType.BigInt;
                OutputParameter.ParameterName = "@TotalCount";
                OutputParameter.Direction = ParameterDirection.Output;
                sqlParameters.Add(OutputParameter);

                menus.lstMenu = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapMenu, AppSettingsModel.ConnectionStrings).ToList();
                menus.TotalCount = Convert.ToInt32(OutputParameter.Value);


                return menus;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<ItemSpecificationsDto> GetItemSpecifications(int? TenantID)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[ItemSpecifications] where TenantId=" + TenantID;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<ItemSpecificationsDto> itemSpecifications = new List<ItemSpecificationsDto>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                itemSpecifications.Add(new ItemSpecificationsDto
                {
                    Id = int.Parse(dataSet.Tables[0].Rows[i]["Id"].ToString()),
                    ItemId = int.Parse(dataSet.Tables[0].Rows[i]["ItemId"].ToString()),
                    IsRequired = bool.Parse(dataSet.Tables[0].Rows[i]["IsRequired"].ToString()),
                    SpecificationId = int.Parse(dataSet.Tables[0].Rows[i]["SpecificationId"].ToString()),
                    //MaxSelectNumber = int.Parse(dataSet.Tables[0].Rows[i]["MaxSelectNumber"].ToString()),
                    TenantId = int.Parse(dataSet.Tables[0].Rows[i]["TenantId"].ToString())

                });
            }

            conn.Close();
            da.Dispose();

            return itemSpecifications;

        }
        private List<SpecificationChoicesDto> GetSpecificationChoices(int? TenantID)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[SpecificationChoices] where TenantId=" + TenantID;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<SpecificationChoicesDto> specificationChoices = new List<SpecificationChoicesDto>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                //decimal LoyaltyPoints = 0;
                //var IsOverrideLoyaltyPoints = bool.Parse(dataSet.Tables[0].Rows[i]["IsOverrideLoyaltyPoints"].ToString());
                //if (!IsOverrideLoyaltyPoints)
                //{
                //    var Price = decimal.Parse(dataSet.Tables[0].Rows[i]["Price"].ToString());
                //    LoyaltyPoints = _loyaltyAppService.ConvertPriceToPoint(Price);
                //}


                specificationChoices.Add(new SpecificationChoicesDto
                {
                    Id = int.Parse(dataSet.Tables[0].Rows[i]["Id"].ToString()),
                    LanguageBotId = int.Parse(dataSet.Tables[0].Rows[i]["LanguageBotId"].ToString()),
                    SpecificationChoiceDescription = dataSet.Tables[0].Rows[i]["SpecificationChoiceDescription"].ToString(),
                    SpecificationChoiceDescriptionEnglish = dataSet.Tables[0].Rows[i]["SpecificationChoiceDescriptionEnglish"].ToString(),
                    Price = decimal.Parse(dataSet.Tables[0].Rows[i]["Price"].ToString()),
                    SKU = dataSet.Tables[0].Rows[i]["SKU"].ToString() ?? "",
                    TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"].ToString()),
                    SpecificationId = int.Parse(dataSet.Tables[0].Rows[i]["SpecificationId"].ToString()),
                    LoyaltyPoints=decimal.Parse(dataSet.Tables[0].Rows[i]["LoyaltyPoints"].ToString()),
                    OriginalLoyaltyPoints=decimal.Parse(dataSet.Tables[0].Rows[i]["OriginalLoyaltyPoints"].ToString()),
                    IsOverrideLoyaltyPoints = bool.Parse(dataSet.Tables[0].Rows[i]["IsOverrideLoyaltyPoints"].ToString()),
                });
            }

            conn.Close();
            da.Dispose();

            return specificationChoices;

        }
        private List<SpecificationChoicesDto> GetSpecificationChoicesById(int? TenantID, long SpecificationId)
        {

           // var LoyaltyDefinitionId = _loyaltyAppService.GetAll().Id;

            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[SpecificationChoices] where TenantId=" + TenantID +" and SpecificationId = " + SpecificationId + " and  IsDeleted = 0";


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<SpecificationChoicesDto> specificationChoices = new List<SpecificationChoicesDto>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                //decimal LoyaltyPoints = 0;
                //var IsOverrideLoyaltyPoints = bool.Parse(dataSet.Tables[0].Rows[i]["IsOverrideLoyaltyPoints"].ToString());
                //if (!IsOverrideLoyaltyPoints)
                //{
                //    var Price = decimal.Parse(dataSet.Tables[0].Rows[i]["Price"].ToString());
                //     LoyaltyPoints = _loyaltyAppService.ConvertPriceToPoint(Price);
                //}
              

                

                specificationChoices.Add(new SpecificationChoicesDto
                {
                    Id = int.Parse(dataSet.Tables[0].Rows[i]["Id"].ToString()),
                    LanguageBotId = 1,
                    SpecificationChoiceDescription = dataSet.Tables[0].Rows[i]["SpecificationChoiceDescription"].ToString(),
                    SpecificationChoiceDescriptionEnglish = dataSet.Tables[0].Rows[i]["SpecificationChoiceDescriptionEnglish"].ToString(),
                    Price = decimal.Parse(dataSet.Tables[0].Rows[i]["Price"].ToString()),
                    SKU = dataSet.Tables[0].Rows[i]["SKU"].ToString() ?? "",
                    TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"].ToString()),
                    SpecificationId = int.Parse(dataSet.Tables[0].Rows[i]["SpecificationId"].ToString()),
                    LoyaltyPoints=decimal.Parse(dataSet.Tables[0].Rows[i]["LoyaltyPoints"].ToString()),
                    OriginalLoyaltyPoints=decimal.Parse(dataSet.Tables[0].Rows[i]["OriginalLoyaltyPoints"].ToString()),
                    IsOverrideLoyaltyPoints = bool.Parse(dataSet.Tables[0].Rows[i]["IsOverrideLoyaltyPoints"].ToString()),
                   // LoyaltyDefinitionId=LoyaltyDefinitionId
                });
            }

            conn.Close();
            da.Dispose();

            return specificationChoices;

        }
        private List<ItemAdditionDto> GetItemAdditionChoicesById(int? TenantID, long ItemAdditionsCategoryId)
        {
           // var LoyaltyDefinitionId=_loyaltyAppService.GetAll().Id;
            try
            {
                string connString = AppSettingsModel.ConnectionStrings;
                string query = "select * from [dbo].[ItemAdditions] where TenantId=" + TenantID +" and ItemAdditionsCategoryId = " + ItemAdditionsCategoryId + " and  IsDeleted = 0";


                SqlConnection conn = new SqlConnection(connString);
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                // create the DataSet 
                DataSet dataSet = new DataSet();

                // create data adapter
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                // this will query your database and return the result to your datatable
                da.Fill(dataSet);

                List<ItemAdditionDto> specificationChoices = new List<ItemAdditionDto>();

                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {

                  
                    var IsOverrideLoyaltyPoints = bool.Parse(dataSet.Tables[0].Rows[i]["IsOverrideLoyaltyPoints"].ToString());
                    var OriginalLoyaltyPoints = decimal.Parse(dataSet.Tables[0].Rows[i]["OriginalLoyaltyPoints"].ToString());
                    decimal LoyaltyPoints = decimal.Parse(dataSet.Tables[0].Rows[i]["LoyaltyPoints"].ToString());


                    try
                    {
                        var Price = decimal.Parse(dataSet.Tables[0].Rows[i]["Price"].ToString());

                        //if (!IsOverrideLoyaltyPoints)
                        //{                           
                        //    LoyaltyPoints = _loyaltyAppService.ConvertPriceToPoints(Price, LoyaltyPoints, OriginalLoyaltyPoints, LoyaltyDefinitionId);
                        //}



                        specificationChoices.Add(new ItemAdditionDto
                        {
                            Id = int.Parse(dataSet.Tables[0].Rows[i]["Id"].ToString()),
                            LanguageBotId = 1,

                            Name = dataSet.Tables[0].Rows[i]["Name"].ToString(),
                            NameEnglish = dataSet.Tables[0].Rows[i]["NameEnglish"].ToString(),
                            price = Price,
                            SKU = dataSet.Tables[0].Rows[i]["SKU"].ToString() ?? "",
                            TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"].ToString()),
                            ItemAdditionsCategoryId = int.Parse(dataSet.Tables[0].Rows[i]["ItemAdditionsCategoryId"].ToString()),
                            ImageUri= dataSet.Tables[0].Rows[i]["ImageUri"].ToString() ?? "",
                            LoyaltyPoints=LoyaltyPoints,
                            OriginalLoyaltyPoints=OriginalLoyaltyPoints,
                            IsOverrideLoyaltyPoints = IsOverrideLoyaltyPoints,
                           // LoyaltyDefinitionId=LoyaltyDefinitionId
                        });
                    }
                    catch
                    {

                        //if (!IsOverrideLoyaltyPoints)
                        //{
                        //    LoyaltyPoints = _loyaltyAppService.ConvertPriceToPoints(0, LoyaltyPoints, OriginalLoyaltyPoints, LoyaltyDefinitionId);
                        //}


                        specificationChoices.Add(new ItemAdditionDto
                        {
                            Id = int.Parse(dataSet.Tables[0].Rows[i]["Id"].ToString()),
                            LanguageBotId = 1,

                            Name = dataSet.Tables[0].Rows[i]["Name"].ToString(),
                            NameEnglish = dataSet.Tables[0].Rows[i]["NameEnglish"].ToString(),
                            price = 0,
                            SKU = dataSet.Tables[0].Rows[i]["SKU"].ToString() ?? "",
                            TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"].ToString()),
                            ItemAdditionsCategoryId = int.Parse(dataSet.Tables[0].Rows[i]["ItemAdditionsCategoryId"].ToString()),
                            ImageUri= dataSet.Tables[0].Rows[i]["ImageUri"].ToString() ?? "",
                            LoyaltyPoints=LoyaltyPoints,
                            //LoyaltyDefinitionId=LoyaltyDefinitionId,
                            OriginalLoyaltyPoints=OriginalLoyaltyPoints,
                            IsOverrideLoyaltyPoints = IsOverrideLoyaltyPoints,
                        });

                    }
                   
                }

                conn.Close();
                da.Dispose();

                return specificationChoices;

            }
            catch(Exception ex)
            {

                throw ex;
            }
           

        }

        private List<SpecificationsDto> GetSpecifications(int? TenantID)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[Specifications] where TenantId=" + TenantID + "and IsDeleted=" + 0;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<SpecificationsDto> specifications = new List<SpecificationsDto>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                specifications.Add(new SpecificationsDto
                {
                    Id = int.Parse(dataSet.Tables[0].Rows[i]["Id"].ToString()),
                    LanguageBotId = int.Parse(dataSet.Tables[0].Rows[i]["LanguageBotId"].ToString()),
                    Priority = int.Parse(dataSet.Tables[0].Rows[i]["Priority"].ToString()),
                    MaxSelectNumber = int.Parse(dataSet.Tables[0].Rows[i]["MaxSelectNumber"].ToString()),
                    TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"].ToString()),
                    IsMultipleSelection = bool.Parse(dataSet.Tables[0].Rows[i]["IsMultipleSelection"].ToString()),
                    SpecificationDescription = dataSet.Tables[0].Rows[i]["SpecificationDescription"].ToString(),
                    SpecificationDescriptionEnglish = dataSet.Tables[0].Rows[i]["SpecificationDescriptionEnglish"].ToString()

                });
            }

            conn.Close();
            da.Dispose();

            return specifications;

        }

        private ItemCategoryEntity getItemAdditionsCategories( int pageNumber=0, int pageSize = 10)
        {
            try
            {
                ItemCategoryEntity data = new ItemCategoryEntity();

                var SP_Name = Constants.ItemAdditions.SP_ItemAdditionsCategorysGet;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                    new System.Data.SqlClient.SqlParameter("@PageSize",pageSize)
                   ,new System.Data.SqlClient.SqlParameter("@PageNumber",pageNumber)
                   ,new System.Data.SqlClient.SqlParameter("@TenantId",AbpSession.TenantId.Value)
                };
                var OutputParameter = new System.Data.SqlClient.SqlParameter();
                OutputParameter.SqlDbType = SqlDbType.BigInt;
                OutputParameter.ParameterName = "@TotalCount";
                OutputParameter.Direction = ParameterDirection.Output;

                sqlParameters.Add(OutputParameter);
                data.lstItemAdditionsCategory = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapItemAdditionsCategories, AppSettingsModel.ConnectionStrings).ToList();


                data.TotalCount = Convert.ToInt32(OutputParameter.Value);
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private ItemCategoryEntity getSpecificationsCategory(int pageNumber = 0, int pageSize = 10)
        {
            try
            {
                ItemCategoryEntity data = new ItemCategoryEntity();

                var SP_Name = Constants.Menu.SP_SpecificationsCategoryGet;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                    new System.Data.SqlClient.SqlParameter("@PageSize",pageSize)
                   ,new System.Data.SqlClient.SqlParameter("@PageNumber",pageNumber)
                   ,new System.Data.SqlClient.SqlParameter("@TenantId",AbpSession.TenantId.Value)
                };
                var OutputParameter = new System.Data.SqlClient.SqlParameter();
                OutputParameter.SqlDbType = SqlDbType.BigInt;
                OutputParameter.ParameterName = "@TotalCount";
                OutputParameter.Direction = ParameterDirection.Output;

                sqlParameters.Add(OutputParameter);
                data.lstSpecificationsCategory = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapSpecificationsCategories, AppSettingsModel.ConnectionStrings).ToList();


                data.TotalCount = Convert.ToInt32(OutputParameter.Value);
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private List<GetItemAdditionsCategorysModel> GetItemAdditionsCategory(int? TenantID)
        {

            try
            {
                string connString = AppSettingsModel.ConnectionStrings;
                string query = "select * from [dbo].[ItemAdditionsCategorys] where TenantId=" + TenantID ;


                SqlConnection conn = new SqlConnection(connString);
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                // create the DataSet 
                DataSet dataSet = new DataSet();

                // create data adapter
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                // this will query your database and return the result to your datatable
                da.Fill(dataSet);

                List<GetItemAdditionsCategorysModel> specifications = new List<GetItemAdditionsCategorysModel>();

                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {

                    specifications.Add(new GetItemAdditionsCategorysModel
                    {

                        categoryId = int.Parse(dataSet.Tables[0].Rows[i]["Id"].ToString()),
                        IsCondiments =  bool.Parse(dataSet.Tables[0].Rows[i]["IsCondiments"].ToString()),
                        IsCrispy =  bool.Parse(dataSet.Tables[0].Rows[i]["IsCrispy"].ToString()),
                        IsDeserts =  bool.Parse(dataSet.Tables[0].Rows[i]["IsDeserts"].ToString()),
                        categoryName = dataSet.Tables[0].Rows[i]["Name"].ToString(),
                        categoryNameEnglish = dataSet.Tables[0].Rows[i]["NameEnglish"].ToString(),
                        categoryPriority = int.Parse(dataSet.Tables[0].Rows[i]["Priority"].ToString()),
                        IsNon =!bool.Parse(dataSet.Tables[0].Rows[i]["IsCondiments"].ToString()) && !bool.Parse(dataSet.Tables[0].Rows[i]["IsCrispy"].ToString()) && !bool.Parse(dataSet.Tables[0].Rows[i]["IsDeserts"].ToString()),

                    });
                }

                conn.Close();
                da.Dispose();

                return specifications;
            }
            catch (Exception ex)
            {
                throw ex;

            }
           

        }
        private List<GetSpecificationsCategorysModel> GetSpecificationsCategory(int? TenantID)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[Specifications] where TenantId=" + TenantID +"and IsDeleted=" + 0;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<GetSpecificationsCategorysModel> specifications = new List<GetSpecificationsCategorysModel>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                specifications.Add(new GetSpecificationsCategorysModel
                {

                    categoryId = int.Parse(dataSet.Tables[0].Rows[i]["Id"].ToString()),
                    categoryName = dataSet.Tables[0].Rows[i]["SpecificationDescription"].ToString(),
                    categoryNameEnglish = dataSet.Tables[0].Rows[i]["SpecificationDescriptionEnglish"].ToString(),
                    categoryPriority = int.Parse(dataSet.Tables[0].Rows[i]["Priority"].ToString()),
                    IsMultipleSelection = bool.Parse(dataSet.Tables[0].Rows[i]["IsMultipleSelection"].ToString()),
                    IsRequired = true,
                   // listSpecificationChoices =new List<SpecificationChoicesDto>() { new SpecificationChoicesDto { } },
                    MaxSelectNumber = int.Parse(dataSet.Tables[0].Rows[i]["MaxSelectNumber"].ToString())
 

                });
            }

            conn.Close();
            da.Dispose();

            return specifications;

        }
        //private List<MenuDto> GetAllMenuWithTenantID(int? TenantID)
        //{
        //    string connString = AppSettingsModel.ConnectionStrings;
        //    string query = "select * from [dbo].[Menus] where TenantID=" + TenantID;


        //    SqlConnection conn = new SqlConnection(connString);
        //    SqlCommand cmd = new SqlCommand(query, conn);
        //    conn.Open();

        //    // create the DataSet 
        //    DataSet dataSet = new DataSet();

        //    // create data adapter
        //    SqlDataAdapter da = new SqlDataAdapter(cmd);
        //    // this will query your database and return the result to your datatable
        //    da.Fill(dataSet);

        //    List<MenuDto> menuDtos = new List<MenuDto>();

        //    for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
        //    {

        //        menuDtos.Add(new MenuDto
        //        {
        //            Id = long.Parse(dataSet.Tables[0].Rows[i]["Id"].ToString()),
        //            ImageUri = dataSet.Tables[0].Rows[i]["ImageUri"].ToString(),
        //            MenuName = dataSet.Tables[0].Rows[i]["MenuName"].ToString(),
        //            MenuDescription = dataSet.Tables[0].Rows[i]["MenuDescription"].ToString(),
        //            MenuNameEnglish = dataSet.Tables[0].Rows[i]["MenuNameEnglish"].ToString(),
        //            MenuDescriptionEnglish = dataSet.Tables[0].Rows[i]["MenuDescriptionEnglish"].ToString(),
        //            Priority = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Priority"]),
        //            RestaurantsType = (RestaurantsTypeEunm)Convert.ToInt32(dataSet.Tables[0].Rows[i]["RestaurantsType"]),
        //            LanguageBotId = int.Parse(dataSet.Tables[0].Rows[i]["LanguageBotId"].ToString()),
        //        });
        //    }

        //    conn.Close();
        //    da.Dispose();

        //    return menuDtos;

        //}

        private List<ItemDto> GetAllItemWithTenantID(int? TenantID, long menuID)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[Items] where TenantID=" + TenantID + " and MenuId=" + menuID;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<ItemDto> itemDtos = new List<ItemDto>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                var IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString());

                if (!IsDeleted)
                {
                    try
                    {
                        itemDtos.Add(new ItemDto
                        {
                            ItemCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemCategoryId"].ToString()),
                            Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"].ToString()),
                            CategoryNames = dataSet.Tables[0].Rows[i]["CategoryNames"].ToString(),
                            CategoryNamesEnglish = dataSet.Tables[0].Rows[i]["CategoryNamesEnglish"].ToString(),
                            IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString()),
                            SKU = dataSet.Tables[0].Rows[i]["SKU"].ToString(),
                            ImageUri = dataSet.Tables[0].Rows[i]["ImageUri"].ToString(),
                            IsInService = bool.Parse(dataSet.Tables[0].Rows[i]["IsInService"].ToString()),
                            ItemDescription = dataSet.Tables[0].Rows[i]["ItemDescription"].ToString(),
                            ItemDescriptionEnglish = dataSet.Tables[0].Rows[i]["ItemDescriptionEnglish"].ToString(),
                            ItemName = dataSet.Tables[0].Rows[i]["ItemName"].ToString(),
                            ItemNameEnglish = dataSet.Tables[0].Rows[i]["ItemNameEnglish"].ToString(),
                            Price = decimal.Parse(dataSet.Tables[0].Rows[i]["Price"].ToString()),
                            Priority = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Priority"].ToString()),
                            OldPrice = decimal.Parse(dataSet.Tables[0].Rows[i]["OldPrice"].ToString() ?? "0"),
                            Ingredients = dataSet.Tables[0].Rows[i]["Ingredients"].ToString() ?? "",

                        });
                    }
                    catch(Exception)
                    {

                        itemDtos.Add(new ItemDto
                        {
                            ItemCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemCategoryId"]),
                            Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                            CategoryNames = dataSet.Tables[0].Rows[i]["CategoryNames"].ToString(),
                            CategoryNamesEnglish = dataSet.Tables[0].Rows[i]["CategoryNamesEnglish"].ToString(),
                            IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString()),
                            SKU = dataSet.Tables[0].Rows[i]["SKU"].ToString(),
                            ImageUri = dataSet.Tables[0].Rows[i]["ImageUri"].ToString(),
                            IsInService = bool.Parse(dataSet.Tables[0].Rows[i]["IsInService"].ToString()),
                            ItemDescription = dataSet.Tables[0].Rows[i]["ItemDescription"].ToString(),
                            ItemDescriptionEnglish = dataSet.Tables[0].Rows[i]["ItemDescriptionEnglish"].ToString(),
                            ItemName = dataSet.Tables[0].Rows[i]["ItemName"].ToString(),
                            ItemNameEnglish = dataSet.Tables[0].Rows[i]["ItemNameEnglish"].ToString(),
                            Price = decimal.Parse(dataSet.Tables[0].Rows[i]["Price"].ToString()),
                            Priority = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Priority"])

                        });

                    }


                }

            }

            conn.Close();
            da.Dispose();

            return itemDtos;

        }

        //private MenuCategoryDto GetAllitemCategoriesWithTenantID(int? TenantID, long Id)
        //{
        //    string connString = AppSettingsModel.ConnectionStrings;
        //    string query = "select * from [dbo].[ItemCategorys] where TenantID=" + TenantID + " and Id=" + Id;


        //    SqlConnection conn = new SqlConnection(connString);
        //    SqlCommand cmd = new SqlCommand(query, conn);
        //    conn.Open();

        //    // create the DataSet 
        //    DataSet dataSet = new DataSet();

        //    // create data adapter
        //    SqlDataAdapter da = new SqlDataAdapter(cmd);
        //    // this will query your database and return the result to your datatable
        //    da.Fill(dataSet);

        //    MenuCategoryDto itemCategories = new MenuCategoryDto();

        //    for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
        //    {
        //        var IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString());

        //        if (!IsDeleted)
        //        {

        //            itemCategories = new MenuCategoryDto
        //            {
        //                Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
        //                Name = dataSet.Tables[0].Rows[i]["Name"].ToString(),
        //                NameEnglish = dataSet.Tables[0].Rows[i]["NameEnglish"].ToString(),
        //                IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString()),

        //                bgImag = dataSet.Tables[0].Rows[i]["bgImag"].ToString(),
        //                logoImag = dataSet.Tables[0].Rows[i]["logoImag"].ToString()
        //            };


        //        }


        //    }

        //    conn.Close();
        //    da.Dispose();

        //    return itemCategories;

        //}

        private List<MenuCategoryDto> GetAllitemCategories(int? TenantID, int menuId)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[ItemCategorys] where TenantID=" + TenantID + " and MenuId = " + menuId;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<MenuCategoryDto> itemCategories = new List<MenuCategoryDto>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                var IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString());

                if (!IsDeleted)
                {

                    itemCategories.Add(new MenuCategoryDto
                    {
                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                        Name = dataSet.Tables[0].Rows[i]["Name"].ToString(),
                        NameEnglish = dataSet.Tables[0].Rows[i]["NameEnglish"].ToString(),
                        IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString()),
                        MenuType = Convert.ToInt32(dataSet.Tables[0].Rows[i]["MenuType"]),
                        Priority = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Priority"]),
                        MenuId = Convert.ToInt64(dataSet.Tables[0].Rows[i]["MenuId"]),
                        logoImag = dataSet.Tables[0].Rows[i]["logoImag"].ToString(),
                        bgImag = dataSet.Tables[0].Rows[i]["bgImag"].ToString(),

                    });


                }


            }

            conn.Close();
            da.Dispose();

            return itemCategories;

        }

        //private List<ItemAdditionDto> GetAllItemAdditionWithTenantID(int? tenantId, long id)
        //{
        //    string connString = AppSettingsModel.ConnectionStrings;
        //    string query = "select * from [dbo].[ItemAdditions] where TenantId=" + tenantId + " and ItemId=" + id;


        //    SqlConnection conn = new SqlConnection(connString);
        //    SqlCommand cmd = new SqlCommand(query, conn);
        //    conn.Open();

        //    // create the DataSet 
        //    DataSet dataSet = new DataSet();

        //    // create data adapter
        //    SqlDataAdapter da = new SqlDataAdapter(cmd);
        //    // this will query your database and return the result to your datatable
        //    da.Fill(dataSet);

        //    List<ItemAdditionDto> itemAdditionDtos = new List<ItemAdditionDto>();

        //    for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
        //    {

        //        itemAdditionDtos.Add(new ItemAdditionDto
        //        {
        //            Id = long.Parse(dataSet.Tables[0].Rows[i]["Id"].ToString()),
        //            Name = dataSet.Tables[0].Rows[i]["Name"].ToString(),
        //            NameEnglish = dataSet.Tables[0].Rows[i]["NameEnglish"].ToString(),
        //            price = decimal.Parse(dataSet.Tables[0].Rows[i]["Price"].ToString()),
        //            itemId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["itemId"]),
        //            SKU = dataSet.Tables[0].Rows[i]["SKU"].ToString(),


        //        });
        //    }

        //    conn.Close();
        //    da.Dispose();

        //    return itemAdditionDtos;
        //}

        private List<LanguageBot> LanguageBot()
        {

            try
            {
                string connString = AppSettingsModel.ConnectionStrings;
                string query = "select * from [dbo].[LanguageBot] ";


                SqlConnection conn = new SqlConnection(connString);
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                // create the DataSet 
                DataSet dataSet = new DataSet();

                // create data adapter
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                // this will query your database and return the result to your datatable
                da.Fill(dataSet);

                List<LanguageBot> languageBots = new List<LanguageBot>();

                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {

                    languageBots.Add(new LanguageBot
                    {
                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                        ISO = dataSet.Tables[0].Rows[i]["ISO"].ToString(),
                        Name = dataSet.Tables[0].Rows[i]["Name"].ToString(),
                        TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),

                    });
                }

                conn.Close();
                da.Dispose();

                return languageBots;

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        private List<Area> GetAreasList(string TenantID)
        {
            try
            {
                //TenantID = "31";
                var tenantID = int.Parse(TenantID);
                string connString = AppSettingsModel.ConnectionStrings;
                string query = "select * from [dbo].[Areas] where TenantID=" + tenantID;


                SqlConnection conn = new SqlConnection(connString);
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                // create the DataSet 
                DataSet dataSet = new DataSet();

                // create data adapter
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                // this will query your database and return the result to your datatable
                da.Fill(dataSet);

                List<Area> branches = new List<Area>();

                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {
                    var IsAvailableBranch = bool.Parse(dataSet.Tables[0].Rows[i]["IsAvailableBranch"].ToString());

                    if (IsAvailableBranch)
                    {
                        branches.Add(new Area
                        {
                            Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                            AreaCoordinate = dataSet.Tables[0].Rows[i]["AreaCoordinate"].ToString() ?? "",
                            AreaName = dataSet.Tables[0].Rows[i]["AreaName"].ToString() ?? "",
                            AreaCoordinateEnglish = dataSet.Tables[0].Rows[i]["AreaCoordinateEnglish"].ToString() ?? "",
                            AreaNameEnglish = dataSet.Tables[0].Rows[i]["AreaNameEnglish"].ToString() ?? "",
                            TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
                            BranchID = dataSet.Tables[0].Rows[i]["BranchID"].ToString(),
                            UserId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["UserId"]),
                            RestaurantsType = Convert.ToInt32(dataSet.Tables[0].Rows[i]["RestaurantsType"]),
                            IsAssginToAllUser = bool.Parse(dataSet.Tables[0].Rows[i]["IsAssginToAllUser"].ToString()),
                            IsAvailableBranch = bool.Parse(dataSet.Tables[0].Rows[i]["IsAvailableBranch"].ToString()),
                            IsRestaurantsTypeAll = bool.Parse(dataSet.Tables[0].Rows[i]["IsRestaurantsTypeAll"].ToString()),


                        });

                    }

                }

                conn.Close();
                da.Dispose();

                return branches;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        private void UpdateItemAdditionDetails(ItemAdditionDetailsModel input)
        {
            try
            {
                string connString = AppSettingsModel.ConnectionStrings;
                using (SqlConnection connection = new SqlConnection(connString))
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "UPDATE ItemAdditionDetails SET"

                       + "  IsInService = @IsInService "
                       + "  Where Id = @Id ";

                    command.Parameters.AddWithValue("@Id", input.Id);
                    //  command.Parameters.AddWithValue("@ItemAdditionId", input.ItemAdditionId);
                    //  command.Parameters.AddWithValue("@ItemId", input.ItemId);
                    //  command.Parameters.AddWithValue("@MenuType", input.MenuType);
                    //  command.Parameters.AddWithValue("@TenantId", input.TenantId);
                    command.Parameters.AddWithValue("@IsInService", input.IsInService);
                    //  command.Parameters.AddWithValue("@CopiedFromId", input.CopiedFromId);

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        private void UpdateItemSpecificationsDetails(ItemSpecificationsDetail input)
        {
            try
            {
                string connString = AppSettingsModel.ConnectionStrings;
                using (SqlConnection connection = new SqlConnection(connString))
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "UPDATE ItemSpecificationsDetail SET"

                       + "  IsInService = @IsInService "
                       + "  Where Id = @Id ";

                    command.Parameters.AddWithValue("@Id", input.Id);
                    //  command.Parameters.AddWithValue("@ItemAdditionId", input.ItemAdditionId);
                    //  command.Parameters.AddWithValue("@ItemId", input.ItemId);
                    //  command.Parameters.AddWithValue("@MenuType", input.MenuType);
                    //  command.Parameters.AddWithValue("@TenantId", input.TenantId);
                    //command.Parameters.AddWithValue("@CopiedFromId", 0);
                    command.Parameters.AddWithValue("@IsInService", input.IsInService);
                    //  command.Parameters.AddWithValue("@CopiedFromId", input.CopiedFromId);

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        private List<ItemAdditionDetailsModel> GetItemAdditionDetailsList(int TenantID)
        {
            try
            {
                //TenantID = "31";
                string connString = AppSettingsModel.ConnectionStrings;
                string query = "select * from [dbo].[ItemAdditionDetails] where TenantID=" + TenantID;


                SqlConnection conn = new SqlConnection(connString);
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                // create the DataSet 
                DataSet dataSet = new DataSet();

                // create data adapter
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                // this will query your database and return the result to your datatable
                da.Fill(dataSet);

                List<ItemAdditionDetailsModel> itemAdditionDetails = new List<ItemAdditionDetailsModel>();

                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {

                    itemAdditionDetails.Add(new ItemAdditionDetailsModel
                    {
                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                        //CopiedFromId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["CopiedFromId"]),
                        ItemAdditionId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemAdditionId"]),
                        //  ItemId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemId"]),
                        MenuType = Convert.ToInt32(dataSet.Tables[0].Rows[i]["MenuType"]),
                        TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
                        IsInService = Convert.ToBoolean(dataSet.Tables[0].Rows[i]["IsInService"]),

                    });



                }

                conn.Close();
                da.Dispose();

                return itemAdditionDetails;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        private List<ItemSpecificationsDetail> GetItemSpecificationsDetailList(int TenantID)
        {
            try
            {
                //TenantID = "31";
                string connString = AppSettingsModel.ConnectionStrings;
                string query = "select * from [dbo].[ItemSpecificationsDetail] where TenantID=" + TenantID;


                SqlConnection conn = new SqlConnection(connString);
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                // create the DataSet 
                DataSet dataSet = new DataSet();

                // create data adapter
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                // this will query your database and return the result to your datatable
                da.Fill(dataSet);

                List<ItemSpecificationsDetail> itemAdditionDetails = new List<ItemSpecificationsDetail>();

                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {

                    itemAdditionDetails.Add(new ItemSpecificationsDetail
                    {
                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                        //CopiedFromId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["CopiedFromId"]),
                        SpecificationChoicesId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["SpecificationChoicesId"]),
                        //  ItemId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemId"]),
                        MenuType = Convert.ToInt32(dataSet.Tables[0].Rows[i]["MenuType"]),
                        TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
                        IsInService = Convert.ToBoolean(dataSet.Tables[0].Rows[i]["IsInService"]),

                    });



                }

                conn.Close();
                da.Dispose();

                return itemAdditionDetails;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        private List<CreateOrEditMenuCategoryDto> GetCategorys(long menuID)
        {

            try
            {
                List<CreateOrEditMenuCategoryDto> lstCategorysInItemModel = new List<CreateOrEditMenuCategoryDto>();
                var SP_Name = "[dbo].[CategorysByMenuIDGet]";

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {

             new System.Data.SqlClient.SqlParameter("@TenantId",(int?)AbpSession.TenantId)
            ,new System.Data.SqlClient.SqlParameter("@MenuID",menuID)
            };




                lstCategorysInItemModel = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),
                 MapCategorys, AppSettingsModel.ConnectionStrings).ToList();

                return lstCategorysInItemModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private CreateOrEditMenuCategoryDto MapCategorys(IDataReader dataReader)
        {
            try
            {
                CreateOrEditMenuCategoryDto entity = new CreateOrEditMenuCategoryDto();
                entity.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");
                //entity.te = SqlDataHelper.GetValue<int>(dataReader, "TenantId");
                entity.MenuId = SqlDataHelper.GetValue<long>(dataReader, "MenuType");
                entity.Name = SqlDataHelper.GetValue<string>(dataReader, "Name");
                entity.NameEnglish = SqlDataHelper.GetValue<string>(dataReader, "NameEnglish");
                entity.logoImag = SqlDataHelper.GetValue<string>(dataReader, "logoImag");
                entity.bgImag = SqlDataHelper.GetValue<string>(dataReader, "bgImag");
                entity.Priority = SqlDataHelper.GetValue<string>(dataReader, "Priority");
                if (!string.IsNullOrEmpty(SqlDataHelper.GetValue<string>(dataReader, "ItemSubCategories")))
                {
                    entity.lstMenuSubCategoryDto = JsonConvert.DeserializeObject<List<CreateOrEditMenuSubCategoryDto>>(SqlDataHelper.GetValue<string>(dataReader, "ItemSubCategories"));


                }
                return entity;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }
        private long AddSubCatogeories(CreateOrEditMenuSubCategoryDto createOrEditMenuSubCategoryDto)
        {
            try
            {
                var SP_Name = "[dbo].[ItemSubCategoriesAdd]";
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
             new System.Data.SqlClient.SqlParameter("@ItemCategoryId",createOrEditMenuSubCategoryDto.ItemCategoryId)
             ,new System.Data.SqlClient.SqlParameter("@Priority",createOrEditMenuSubCategoryDto.Priority)
             ,new System.Data.SqlClient.SqlParameter("@LanguageBotId",createOrEditMenuSubCategoryDto.LanguageBotId)
             ,new System.Data.SqlClient.SqlParameter("@Name",createOrEditMenuSubCategoryDto.Name)
             ,new System.Data.SqlClient.SqlParameter("@NameEnglish",createOrEditMenuSubCategoryDto.NameEnglish)
             ,new System.Data.SqlClient.SqlParameter("@bgImag",createOrEditMenuSubCategoryDto.bgImag)
             ,new System.Data.SqlClient.SqlParameter("@logoImag",createOrEditMenuSubCategoryDto.logoImag)
             ,new System.Data.SqlClient.SqlParameter("@MenuType",createOrEditMenuSubCategoryDto.MenuType)
             ,new System.Data.SqlClient.SqlParameter("@MenuId",createOrEditMenuSubCategoryDto.MenuId)
             ,new System.Data.SqlClient.SqlParameter("@TenantId",(int?)AbpSession.TenantId)
             ,new System.Data.SqlClient.SqlParameter("@IsNew",createOrEditMenuSubCategoryDto.IsNew)
             ,new System.Data.SqlClient.SqlParameter("@Price",createOrEditMenuSubCategoryDto.Price)
            };

                System.Data.SqlClient.SqlParameter OutsqlParameter = new System.Data.SqlClient.SqlParameter();
                OutsqlParameter.ParameterName = "@Id";
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
        private void UpdateSubCatogeories(CreateOrEditMenuSubCategoryDto createOrEditMenuSubCategoryDto)
        {



            try
            {
                var SP_Name = "[dbo].[ItemSubCategoriesUpdate]";

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
             new System.Data.SqlClient.SqlParameter("@ItemCategoryId",createOrEditMenuSubCategoryDto.ItemCategoryId)
             ,new System.Data.SqlClient.SqlParameter("@Priority",createOrEditMenuSubCategoryDto.Priority)
             ,new System.Data.SqlClient.SqlParameter("@LanguageBotId",createOrEditMenuSubCategoryDto.LanguageBotId)
             ,new System.Data.SqlClient.SqlParameter("@Name",createOrEditMenuSubCategoryDto.Name)
             ,new System.Data.SqlClient.SqlParameter("@NameEnglish",createOrEditMenuSubCategoryDto.NameEnglish)
             ,new System.Data.SqlClient.SqlParameter("@bgImag",createOrEditMenuSubCategoryDto.bgImag)
             ,new System.Data.SqlClient.SqlParameter("@logoImag",createOrEditMenuSubCategoryDto.logoImag)
             ,new System.Data.SqlClient.SqlParameter("@Id",createOrEditMenuSubCategoryDto.Id)
             ,new System.Data.SqlClient.SqlParameter("@IsNew",createOrEditMenuSubCategoryDto.IsNew)
             ,new System.Data.SqlClient.SqlParameter("@Price",createOrEditMenuSubCategoryDto.Price)

            };
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private bool deleteSubCategory(long subCategoryID)
        {
            try
            {
                var SP_Name = Constants.ItemSubCategory.SP_ItemSubCategoryDelete;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@SubCategoryId",subCategoryID),
                    new System.Data.SqlClient.SqlParameter("@TenantId",AbpSession.TenantId.Value),
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
        private bool deleteMenu(long menuId)
        {
            try
            {
                var SP_Name = Constants.Menu.SP_MenuDelete;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@MenuId",menuId),
                    new System.Data.SqlClient.SqlParameter("@TenantId",AbpSession.TenantId.Value)
                };
                var OutputParameter = new System.Data.SqlClient.SqlParameter();
                OutputParameter.SqlDbType = SqlDbType.Bit;
                OutputParameter.ParameterName = "@Result";
                OutputParameter.Direction = ParameterDirection.Output;
                sqlParameters.Add(OutputParameter);

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);
                return (bool)OutputParameter.Value;
            }
            catch(Exception ex)
            {
                throw ex;

            }

        }
        private List<MenuEntity> getMenusWithDetails(int TenantID, int MenuType)
        {
            List<MenuEntity> lstMenuDetailsModel = new List<MenuEntity>();
            try
            {

                var SP_Name = "[dbo].[MenuCategorySubCategoryGet]";

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {

             new System.Data.SqlClient.SqlParameter("@TenantId",TenantID)
            ,new System.Data.SqlClient.SqlParameter("@MenuType",MenuType)
            };




                lstMenuDetailsModel = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),
                 MapMenu, AppSettingsModel.ConnectionStrings).ToList();

                return lstMenuDetailsModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private MenuEntity MapMenu(IDataReader dataReader)
        {
            try
            {
                MenuEntity entity = new MenuEntity();
                entity.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");
                //entity.te = SqlDataHelper.GetValue<int>(dataReader, "TenantId");
                entity.MenuName = SqlDataHelper.GetValue<string>(dataReader, "MenuName");
                entity.MenuNameEnglish = SqlDataHelper.GetValue<string>(dataReader, "MenuNameEnglish");
                entity.ItemCategories = SqlDataHelper.GetValue<string>(dataReader, "ItemCategories");
                entity.ImageUri = SqlDataHelper.GetValue<string>(dataReader, "ImageUri");
                entity.Priority = SqlDataHelper.GetValue<string>(dataReader, "Priority");
                entity.SettingJson = SqlDataHelper.GetValue<string>(dataReader, "SettingJson");
                entity.MenuTypeId = SqlDataHelper.GetValue<int>(dataReader, "MenuTypeId");
                if (!string.IsNullOrEmpty(SqlDataHelper.GetValue<string>(dataReader, "ItemCategories")))
                {

                    var options = new JsonSerializerOptions { WriteIndented = true };
                    entity.CategoryEntity = System.Text.Json.JsonSerializer.Deserialize<List<CategoryEntity>>(entity.ItemCategories, options);

                }



                return entity;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }
        private void MenuSettingUpdate(long menuId, string menuSettingJson)
        {
            try
            {
                var SP_Name = "[dbo].[MenuSettingUpdate]";

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
            new System.Data.SqlClient.SqlParameter("@SettingJson",menuSettingJson)
           ,new System.Data.SqlClient.SqlParameter("@MenuId",menuId)
            };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private WorkModel getMenuSetting(long menuId)
        {
            WorkModel entity = new WorkModel();
            try
            {
                var SP_Name = "[dbo].[MenuSettingGet]";

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@MenuId",menuId)
                };

                entity = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), MapMenuSetting, AppSettingsModel.ConnectionStrings).FirstOrDefault();


                if(entity != null)
                {
                    entity.StartDateFri = getValidValue(entity.StartDateFri);
                    entity.StartDateSat = getValidValue(entity.StartDateSat);
                    entity.StartDateSun = getValidValue(entity.StartDateSun);
                    entity.StartDateMon = getValidValue(entity.StartDateMon);
                    entity.StartDateTues =getValidValue(entity.StartDateTues);
                    entity.StartDateWed = getValidValue(entity.StartDateWed);
                    entity.StartDateThurs = getValidValue(entity.StartDateThurs);


                    entity.EndDateFri = getValidValue(entity.EndDateFri);
                    entity.EndDateSat = getValidValue(entity.EndDateSat);
                    entity.EndDateSun = getValidValue(entity.EndDateSun);
                    entity.EndDateMon = getValidValue(entity.EndDateMon);
                    entity.EndDateTues =getValidValue(entity.EndDateTues);
                    entity.EndDateWed = getValidValue(entity.EndDateWed);
                    entity.EndDateThurs = getValidValue(entity.EndDateThurs);

                    entity.StartDateFriSP = getValidValue(entity.StartDateFriSP);
                    entity.StartDateSatSP = getValidValue(entity.StartDateSatSP);
                    entity.StartDateSunSP = getValidValue(entity.StartDateSunSP);
                    entity.StartDateMonSP = getValidValue(entity.StartDateMonSP);
                    entity.StartDateTuesSP =getValidValue(entity.StartDateTuesSP);
                    entity.StartDateWedSP = getValidValue(entity.StartDateWedSP);
                    entity.StartDateThursSP = getValidValue(entity.StartDateThursSP);

                    entity.EndDateFriSP = getValidValue(entity.EndDateFriSP);
                    entity.EndDateSatSP = getValidValue(entity.EndDateSatSP);
                    entity.EndDateSunSP = getValidValue(entity.EndDateSunSP);
                    entity.EndDateMonSP = getValidValue(entity.EndDateMonSP);
                    entity.EndDateTuesSP =getValidValue(entity.EndDateTuesSP);
                    entity.EndDateWedSP = getValidValue(entity.EndDateWedSP);
                    entity.EndDateThursSP = getValidValue(entity.EndDateThursSP);
                }

                return entity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private WorkModel MapMenuSetting(IDataReader dataReader)
        {
            try
            {
                WorkModel entity = new WorkModel();

                if (!string.IsNullOrEmpty(SqlDataHelper.GetValue<string>(dataReader, "SettingJson")))
                {

                    var options = new JsonSerializerOptions { WriteIndented = true };
                    entity = System.Text.Json.JsonSerializer.Deserialize<WorkModel>(SqlDataHelper.GetValue<string>(dataReader, "SettingJson"), options);

                }


                return entity;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }
        private MenuContcatKeyModel  MenuContcatKey(IDataReader dataReader)
        {
            try
            {
                MenuContcatKeyModel entity = new MenuContcatKeyModel();
                entity.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");
                entity.KeyMenu = SqlDataHelper.GetValue<string>(dataReader, "KeyMenu");
                entity.Value = SqlDataHelper.GetValue<string>(dataReader, "Value");
                //entity.CreationTime = SqlDataHelper.GetValue<DateTime>(dataReader, "CreationTime");
                entity.ContactID = SqlDataHelper.GetValue<int>(dataReader, "ContactID");

                return entity;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        private MenuContcatKeyModel  MenuContcatKeyNew(IDataReader dataReader)
        {
            try
            {
                MenuContcatKeyModel entity = new MenuContcatKeyModel();
                entity.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");
                entity.KeyMenu = SqlDataHelper.GetValue<string>(dataReader, "KeyMenu");
                entity.Value = SqlDataHelper.GetValue<string>(dataReader, "Value");
                //entity.CreationTime = DateTime.UtcNow; // Set CreationTime to the current UTC time
                //entity.CreationTime = SqlDataHelper.GetValue<DateTime>(dataReader, "CreationTime");
                entity.ContactID = SqlDataHelper.GetValue<int>(dataReader, "ContactID");

                return entity;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        private string checkValidValue(dynamic value)
        {
            string result = null;
            try
            {
               DateTime dateTime= DateTime.Parse(value.ToString());
                dateTime= dateTime.AddHours(AppSettingsModel.AddHour);
                result = dateTime.ToString("HH:mm");
                return result;
            }
            catch (Exception ex)
            {
                return result;

                throw ex;
            }
        }
        private string getValidValue(dynamic value)
        {
            string result = null;
            try
            {
                result = value.ToString();
                return result;
            }
            catch (Exception ex)
            {
                return result;
                throw ex;
            }
        }
        public MenuContcatKeyModel MenuContactKeyAdd(MenuContcatKeyModel model)
        {

            var rez = addMenuContactKey(model);
            return rez;
        }


        public MenuContcatKeyModel MenuContactKeyGet(MenuContcatKeyModel model)
        {

            var rez = getMenuContactKey(model);
            return rez;
        }
        public MenuContcatKeyModel MenuContactKeyGetNew(MenuContcatKeyModel model)
        {

            var rez = getMenuContactKeyNew(model);
            return rez;
        }

        private MenuContcatKeyModel addMenuContactKey(MenuContcatKeyModel model)
        {
            try
            {
                var SP_Name = Constants.MenuContactKey.SP_MenuContactKeyAdd;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>(){
                    new System.Data.SqlClient.SqlParameter("@KeyMenu", model.KeyMenu),
                   new System.Data.SqlClient.SqlParameter("@Value", model.Value),
                   new System.Data.SqlClient.SqlParameter("@ContactID", model.ContactID)
                    };

                MenuContcatKeyModel mo = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), MenuContcatKey, AppSettingsModel.ConnectionStrings).FirstOrDefault();
              
                return mo;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private MenuContcatKeyModel getMenuContactKey(MenuContcatKeyModel model)
        {
            try
            {
                var SP_Name = Constants.MenuContactKey.SP_MenuContactKeyGet;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>(){
                    new System.Data.SqlClient.SqlParameter("@KeyMenu", model.KeyMenu)
                    };

                MenuContcatKeyModel mo = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), MenuContcatKey, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                return mo;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private MenuContcatKeyModel getMenuContactKeyNew(MenuContcatKeyModel model)
        {
            try
            {
                var SP_Name = Constants.MenuContactKey.SP_MenuContactKeyGet;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>(){
                    new System.Data.SqlClient.SqlParameter("@KeyMenu", model.KeyMenu)
                    };

                MenuContcatKeyModel mo = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), MenuContcatKeyNew, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                return mo;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


    }
}