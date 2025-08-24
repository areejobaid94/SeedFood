using Abp.Domain.Repositories;
using Infoseed.MessagingPortal.InfoSeedParser;
using Infoseed.MessagingPortal.ItemAdditions.Dtos;
using Infoseed.MessagingPortal.ItemAdditionsCategorys.Dtos;
using Infoseed.MessagingPortal.Items;
using Infoseed.MessagingPortal.Items.Dtos;
using Infoseed.MessagingPortal.Loyalty;
using Infoseed.MessagingPortal.MenuCategories;
using Infoseed.MessagingPortal.MenuCategories.Dtos;
using Infoseed.MessagingPortal.Menus;
using Infoseed.MessagingPortal.Web.Models.Ctown;
using Infoseed.MessagingPortal.Web.Models.Menu;
using InfoSeedParser;
using InfoSeedParser.ConfigrationFile;
using InfoSeedParser.Interfaces;
using InfoSeedParser.Parsers;
using LiteDB;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Infoseed.MessagingPortal.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CTownApiController : MessagingPortalControllerBase
    {//this is test ali
        private readonly IRepository<Items.Item, long> _lookup_ItemRepository;
        private readonly IMenuParser _MenuParser;
        private ILoyaltyAppService _loyaltyAppService;
        private readonly IItemsAppService _IItemsAppService;
        public CTownApiController(IRepository<Items.Item, long> lookup_ItemRepository, ILoyaltyAppService loyaltyAppService, IItemsAppService IItemsAppService)
        {
            _loyaltyAppService=loyaltyAppService;
            _lookup_ItemRepository = lookup_ItemRepository;
            _MenuParser = new ParserFactory().CreateParser(nameof(MenuExcelParser));
            _IItemsAppService=IItemsAppService;
        }
        [Route("GetCtownItemForEdit")]
        [HttpGet]

        public async Task<ItemDto> GetCtownItemForEdit(long input)
        {
           var item = _IItemsAppService.GetItemById(input);
            var model = _loyaltyAppService.GetItemLoyaltyLog(item.Id);
            if (item.IsLoyal)
            {
                item.LoyaltyPoints = _loyaltyAppService.ConvertPriceToPoints(item.Price, item.LoyaltyPoints, model.OriginalLoyaltyPoints, model.LoyaltyDefinitionId);

            }
            else
            {
                item.LoyaltyPoints = _loyaltyAppService.ConvertPriceToPoints(item.Price, item.LoyaltyPoints, item.OriginalLoyaltyPoints, 0);

            }
            return item;
        }

        [Route("GetCtownCatogeory")]
        [HttpGet]
        public List<CategorysInItemModel> GetCtownCatogeory(int? TenantID)
        {

            List<CategorysInItemModel> categorysInItemModel = new List<CategorysInItemModel>();

            var CategorieList = GetItemCategories(TenantID, 187, 0);


            var subCategorieList = GetSubItemCategories(TenantID, 187);


            foreach (var Categorie in CategorieList)
            {


                var ListSub = subCategorieList.Where(x => x.ItemCategoryId == Categorie.Id).ToList();

                List<SubCategorysInItemModel> subCategorysInItemModels = new List<SubCategorysInItemModel>();
                foreach (var subL in ListSub)
                {

                    //var count = GetItemCount(subL.Id);
                    subCategorysInItemModels.Add(new SubCategorysInItemModel
                        {
                            categoryId = Categorie.Id,
                            subcategoryId = subL.Id,
                            categoryName = subL.Name,
                            categoryNameEnglish = subL.NameEnglish,
                            menuId = 0,
                            menuPriority = subL.Priority,
                            listItemInCategories = new List<ItemDto>(),
                           //  itemCount= count

                    });

                    


                }


                categorysInItemModel.Add(new CategorysInItemModel
                {
                    menuPriority = Categorie.Priority,
                    menuId = 0,
                    categoryId = Categorie.Id,
                    categoryName = Categorie.Name,
                    categoryNameEnglish = Categorie.NameEnglish,
                    bgImg = Categorie.bgImag,
                    logImg = Categorie.logoImag,
                    isSubCategory = true,
                    subCategorysInItemModels = subCategorysInItemModels.OrderBy(x => x.menuPriority).ToList(),
                    listItemInCategories = new List<ItemDto>(),

                });



            }

            return categorysInItemModel.OrderBy(x => x.menuPriority).ToList();

        }


        [Route("GetCtownItem")]
        [HttpGet]
        public GetCtownItemModel GetCtownItem(int? TenantID,int ItemSubCategoryId, int PageSize = 10000, int PageNumber = 0, string Search = "")
        {

            //int? TenantID = 34;
            //int menu = 187;
            int IsSort = 0;
            int totalCount = 0;
            int areaId = 187;

            var listitem = _IItemsAppService.GetItemCTown(TenantID.Value, areaId, ItemSubCategoryId, out totalCount, PageSize, PageNumber, Search, IsSort,null,null,0,true);

            GetCtownItemModel getCtownItemModel = new GetCtownItemModel();


            getCtownItemModel.Total=totalCount;
            getCtownItemModel.ItemDtos=listitem;

            return getCtownItemModel;
        }


        [Route("UpdateCtownItem")]
        [HttpPost]
        public void UpdateCtownItem(ItemDto itemDto)
        {

            if (itemDto.OldPrice == null)
                itemDto.OldPrice = 0;

            if (itemDto.ImageUri == null)
                itemDto.ImageUri = "";

            if (itemDto.ItemName == null)
                itemDto.ItemName = "";

            if (itemDto.ItemNameEnglish == null)
                itemDto.ItemNameEnglish = "";

            if (itemDto.SKU == null)
                itemDto.SKU = "";

            if (itemDto.Size == null)
                itemDto.Size = "";


            CreateOrEditItemDto createOrEditItemDto = ConvertItemDTOToModel(itemDto);
            _IItemsAppService.CreateOrEditItems(createOrEditItemDto);
            
            //UpdateItem(itemDto);


        }


        [Route("syncImage")]
        [HttpPost]
        public void syncImage(int ItemSubCategoryId, bool replece)
        {

            StorageService _obj = new StorageService();
            var ListItem = GeAlltItemWs(187, ItemSubCategoryId);


            var items = ListItem.ToList();
                foreach (var obj in items)
                {


                     if (replece)
                     {
                        
                             var url = _obj.FindFileByName(obj.Barcode.ToString()).Result;
                             if (!string.IsNullOrWhiteSpace(url))
                             {
                              //   Console.WriteLine(url);
                                 obj.ImageUri = url;
                                 UpdateItemImage(obj);
                             }
                         
                 
                     }
                     else
                     {
                         if (!string.IsNullOrWhiteSpace(obj.Barcode.ToString()) && string.IsNullOrWhiteSpace(obj.ImageUri))
                         {
                             var url = _obj.FindFileByName(obj.Barcode.ToString()).Result;
                             if (!string.IsNullOrWhiteSpace(url))
                             {
                               //  Console.WriteLine(url);
                                 obj.ImageUri = url;
                                 UpdateItemImage(obj);
                             }
                         }
                 
                     }

                }

              

            
            Console.WriteLine("Done!");


        }


        [HttpPost("UploadImageCtown")]
        public async void UploadImageCtown([FromForm] UploadFileModel model)
        {
            StorageService _obj = new StorageService();

            if (model.FormFileList != null)
            {
                if (model.FormFileList.Count() > 0)
                {


                    foreach (var item in model.FormFileList)
                    {
                        if (item.Length > 0)
                        {
                            //var filePath = Path.GetTempFileName();
                            //var path = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot\\UploadFile\\",
                            //            item.FileName);

                            //using (var stream = new FileStream("wwwroot\\UploadFile\\" + item.FileName, FileMode.Create))
                            //{
                            //    await item.CopyToAsync(stream);
                            //}

                            var formFile = item;
                            long ContentLength = formFile.Length;
                            byte[] fileData = null;
                            using (var ms = new MemoryStream())
                            {
                                formFile.CopyTo(ms);
                                fileData = ms.ToArray();
                     

                            }
                            var urlss = _obj.UploadFile(fileData, item.FileName) ;




                            try
                            {
                                // Check if file exists with its full path    
                                //if (System.IO.File.Exists("wwwroot\\UploadFile\\" + item.FileName))
                                //{
                                //    // If file found, delete it    
                                //    System.IO.File.Delete("wwwroot\\UploadFile\\" + item.FileName);
                                //    Console.WriteLine("File deleted.");
                                //}
                                //else Console.WriteLine("File not found");
                            }
                            catch (IOException)
                            {
                               // Console.WriteLine(ioExp.Message);
                               // return "NotFound";
                            }






                        }


                    }

                }



            }

          //  return url;
        }


        [HttpPost("CreateOrEditItemCtown")]
        public async void CreateOrEditItemCtown([FromForm] CreateOrEditItemDto model)
        {
            if (!model.SKU.ToUpper().StartsWith("T") && model.Size.Contains("1 KG"))
            {
                model.IsQuantitative = true;
            }
            _ = _IItemsAppService.CreateOrEditItems(model);
        }

        [Route("TestCtown")]
        [HttpGet]
        public string TestCtown(int? TenantID, int FromItemCategoryId, int FromItemSubCategoryId, string FromNameCategoryE, bool isCreateSubCategory)
        {
            List<int> MinID = new List<int>();
            MinID.Add(928);
            //MinID.Add(930);
            //MinID.Add(931);
            MinID.Add(932);
            MinID.Add(933);
            List<int> MinTypeID = new List<int>();
            MinTypeID.Add(188);
           // MinTypeID.Add(189);
           // MinTypeID.Add(190);
            MinTypeID.Add(191);
            MinTypeID.Add(192);

            List<int> SubCatID = new List<int>();
            SubCatID.Add(374);
            SubCatID.Add(377);
            SubCatID.Add(378);
            //SubCatID.Add(237);
          //  SubCatID.Add(289);


           // var TenantID = 34;
           var  FromMenuType = 187;

            var listItem = GetItem(TenantID, FromMenuType, FromItemCategoryId, FromItemSubCategoryId);

            var LitstSub = GetSubCategory(TenantID, FromMenuType).Where(x => x.Id == FromItemSubCategoryId).FirstOrDefault();


            for (int i = 0; i <= 2;i++)
            {

               var MenuID = MinID[i];
                var ToMenuType = MinTypeID[i];
                var ToItemSubCategoryId = SubCatID[i];// 0;

                var catList = GetCategory(TenantID, ToMenuType).Where(x => x.NameEnglish == FromNameCategoryE).FirstOrDefault();

               var ToItemCategoryId = int.Parse(catList.Id.ToString());
                if (isCreateSubCategory)
                {
                   
                    LitstSub.MenuType = MinTypeID[i];
                    LitstSub.ItemCategoryId = ToItemCategoryId;
                    var idSub = CreateSubCategory(LitstSub);
                   ToItemSubCategoryId = idSub;
                }


                foreach (var item in listItem)
                {
                    item.ItemCategoryId = ToItemCategoryId;
                    item.ItemSubCategoryId = ToItemSubCategoryId;// idSub;
                    item.MenuId = MenuID;
                    item.MenuType = ToMenuType;
                    item.TenantId = TenantID.Value;
                    if (!item.SKU.ToUpper().StartsWith("T") && item.Size.Contains("1 KG"))
                    {
                        item.IsQuantitative = true;

                    }
                    CreateOrEditItemDto createOrEditItemDto = ConvertItemDTOToModel(item);
                    _IItemsAppService.CreateOrEditItems(createOrEditItemDto);
                    // CreateItem(item);
                }



            }



            return "hi hassan";
        }

        private static CreateOrEditItemDto ConvertItemDTOToModel(ItemDto item)
        {
            CreateOrEditItemDto createOrEditItemDto = new CreateOrEditItemDto();
            createOrEditItemDto.Id  =item.Id;

            createOrEditItemDto.CreatorUserId =item.CreatorUserId;
            createOrEditItemDto.Ingredients =item.Ingredients;


            createOrEditItemDto.ItemName =item.ItemName;
            createOrEditItemDto.ItemDescription =item.ItemDescription;


            createOrEditItemDto.ItemNameEnglish =item.ItemNameEnglish;
            createOrEditItemDto.ItemDescriptionEnglish =item.ItemDescriptionEnglish;

            createOrEditItemDto.CategoryNames =item.CategoryNames;

            createOrEditItemDto.CategoryNamesEnglish=item.CategoryNamesEnglish;



            createOrEditItemDto.IsInService=item.IsInService;



            createOrEditItemDto.CreationTime =item.CreationTime;


            createOrEditItemDto.DeletionTime =item.DeletionTime;


            createOrEditItemDto.LastModificationTime =item.LastModificationTime;

            createOrEditItemDto.Price =item.Price;
            createOrEditItemDto.OldPrice=item.OldPrice;

            createOrEditItemDto.ImageUri =item.ImageUri;

            createOrEditItemDto.Priority =item.Priority;

            createOrEditItemDto.MenuId=item.MenuId;

            createOrEditItemDto.ItemCategoryId =item.ItemCategoryId;
            createOrEditItemDto.ItemSubCategoryId =item.ItemSubCategoryId;
            createOrEditItemDto.SKU =item.SKU;

            createOrEditItemDto.itemAdditionDtos =item.itemAdditionDtos;


            createOrEditItemDto.MenuType =item.MenuType;
            createOrEditItemDto.LanguageBotId =item.LanguageBotId;

            createOrEditItemDto.Size =item.Size;

            createOrEditItemDto.Barcode =item.Barcode;
            createOrEditItemDto.TenantId =item.TenantId;

            createOrEditItemDto.lstItemAndAdditionsCategoryDto =item.lstItemAndAdditionsCategoryDto;
            createOrEditItemDto.lstItemSpecificationsDto =item.lstItemSpecificationsDto;
            createOrEditItemDto.AreaIds =item.AreaIds;

            createOrEditItemDto.InServiceIds =item.InServiceIds;
            createOrEditItemDto.IsQuantitative =item.IsQuantitative;

            createOrEditItemDto.IsLoyal =item.IsLoyal;
            createOrEditItemDto.LoyaltyPoints =item.LoyaltyPoints;
            createOrEditItemDto.OriginalLoyaltyPoints =item.OriginalLoyaltyPoints;
            createOrEditItemDto.IsOverrideLoyaltyPoints =item.IsOverrideLoyaltyPoints;
            createOrEditItemDto.LoyaltyDefinitionId =item.LoyaltyDefinitionId;


            createOrEditItemDto.Size=item.Size;

            createOrEditItemDto.OldPrice=item.OldPrice;

            createOrEditItemDto.DateFrom=item.DateFrom;

            createOrEditItemDto.DateTo =item.DateTo;
            createOrEditItemDto.Qty =item.Qty;
            createOrEditItemDto.Status_Code =item.Status_Code;

            return createOrEditItemDto;
        }

        [Route("CopyItem")]
        [HttpGet]
        public string CopyItem()
        {


            var item187 = GeAlltItem(187);
            var item188 = GeAlltItem(188);

            foreach(var item in item188)
            {

                var found = item187.Where(x => x.Barcode == item.Barcode).FirstOrDefault();

                if (found == null)
                {
                    var subCat=GetSubCategoryId(item.ItemSubCategoryId);

                    var subCat187 = GetAllSubCategory(34, subCat.NameEnglish).Where(x => x.MenuType == 187).FirstOrDefault();


                    item.MenuType = 187;
                    item.ItemCategoryId = subCat187.ItemCategoryId;
                    item.ItemSubCategoryId = subCat187.Id;
                    item.MenuId= 926;

                    CreateItemWithImage(item);

                }

            }




            return "hi hassan";
        }



        [Route("DeleteItemDoublicat")]
        [HttpGet]
        public string DeleteItemDoublicat(int? TenantID, int copyeFromMenuType, int ItemSubCategoryId)
        {


            //int TenantId = 34;

            var itemListOffer = GeAlltItemWs(copyeFromMenuType, ItemSubCategoryId);
            var itemListWithoutoffer = GeAlltItemWCtown(copyeFromMenuType, ItemSubCategoryId);



            foreach(var item in itemListOffer)
            {

                var found = itemListWithoutoffer.Where(x => x.Barcode == item.Barcode).FirstOrDefault();



                if(found!=null)
                {
                    Deleteitem(item.Id);

                }


            }
            

            return "hi hassan";
        }



        //[Route("UpdateBarCodeItemCtown")]
        //[HttpPost]
        //public async Task<string> UpdateBarCodeItemCtownAsync([FromForm] UploadFileModel file)
        //{
        //    int TenantId = 34;
        //    if (file == null || file.FormFile.Length == 0)
        //        return "NotFound";

        //    var filePath = Path.GetTempFileName();
        //    var path = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot\\UploadFile\\",
        //                file.FormFile.FileName);

        //    using (var stream = new FileStream("wwwroot\\UploadFile\\" + file.FormFile.FileName, FileMode.Create))
        //    {
        //        await file.FormFile.CopyToAsync(stream);
        //    }

        //    var _filePath = "wwwroot\\UploadFile\\" + file.FormFile.FileName;


        //    var MenuList = _MenuParser.Parse(new ParseConfig()
        //    {
        //        config = new ConfigrationExcelFile(),
        //        FilePath = _filePath,
        //        Parser = nameof(MenuExcelParser)
        //    });

        //    var itemList = MenuList.Item;


        //    List<BarCodeModel> barCodeModels = new List<BarCodeModel>();

        //    foreach(var item in itemList)
        //    {
        //        barCodeModels.Add(new BarCodeModel
        //        {
        //             item_number= item.SKU,
        //              Barcode= item.Barcode,
        //               priority=item.Priority,
        //        });

        //    }


        //   // var barcodeList = GetBarCode();

        //    var iscontune = false;

        //    var count = 0;

        //     foreach(var item in barCodeModels)
        //     {
        //        count++;
        //        iscontune = true;
        //        //if (item.item_number== "404")
        //        //{
        //        //    iscontune = true;
        //        //}
        //        if (iscontune)
        //        {
        //            updateItemPriority(item);

        //          // updateItemBarCode(item);

        //        }
         
        //     }



        //    return "hi hassan";
        //}


        [Route("CtownUploadExcelFile")]
        [HttpPost]
        public async Task<string> CtownUploadExcelFileAsync([FromForm] UploadFileModel file ,string ItemSubCategoryName,bool replece ,int TenantId)
        {
           //int TenantId = 34;
            if (file == null || file.FormFile.Length == 0)
                return "NotFound";

            //var filePath = Path.GetTempFileName();
            //var path = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot\\UploadFile\\",
            //            file.FormFile.FileName);

            //using (var stream = new FileStream("wwwroot\\UploadFile\\" + file.FormFile.FileName, FileMode.Create))
            //{
            //    await file.FormFile.CopyToAsync(stream);
            //}

            //var _filePath = "wwwroot\\UploadFile\\" + file.FormFile.FileName;



            var formFile = file.FormFile;
            
            byte[] fileData = null;
            using (var ms = new MemoryStream())
            {
                formFile.CopyTo(ms);
                fileData = ms.ToArray();


            }

            var MenuList = _MenuParser.Parse(new ParseConfig()
            {
                config = new ConfigrationExcelFile(),
                FileData = fileData,
                FileName=formFile.FileName,
                Parser = nameof(MenuExcelParser)
            });

            var itemList = MenuList.Item;

            List<int> MinID = new List<int>();
            List<int> MinTypeID = new List<int>();

            if (ItemSubCategoryName != "Biscuite And Cake")
            {
                MinID.Add(926);//187
                MinID.Add(928);//188               
                MinID.Add(930);//189
                MinID.Add(931);//190
                MinID.Add(932);//191
                MinID.Add(933);//192

                MinTypeID.Add(187);
                MinTypeID.Add(188);
                MinTypeID.Add(189);
                MinTypeID.Add(190);
                MinTypeID.Add(191);
                MinTypeID.Add(192);


            }
            else
            {
                MinID.Add(926);//187          
                MinID.Add(930);//189
                MinID.Add(931);//190
                MinID.Add(932);//191
                MinID.Add(933);//192
                MinID.Add(928);//188    

                MinTypeID.Add(187);
                MinTypeID.Add(189);
                MinTypeID.Add(190);
                MinTypeID.Add(191);
                MinTypeID.Add(192);
                MinTypeID.Add(188);

            }


            

          

            var LitstSub = GetAllSubCategory(TenantId, ItemSubCategoryName);
           
            if (replece)
            {
                foreach(var subitem in LitstSub)
                {

                    DeleteSubCategory(subitem.Id);
                }


            }

            


            for (int i = 0; i <= 5; i++)
            {
                

                foreach (var item in itemList)
                {                
                        item.ItemCategoryId = LitstSub[i].ItemCategoryId;
                        item.ItemSubCategoryId = LitstSub[i].Id;// idSub;
                        item.MenuId = MinID[i];
                        item.MenuType = LitstSub[i].MenuType;
                        item.TenantId = TenantId;
                        item.AreaIds=MinTypeID[i].ToString(); ;

                    //if (!item.SKU.ToUpper().StartsWith("T") && item.Size.Contains("1 KG"))
                    //{
                    //    item.IsQuantitative = true;

                    //}
                    CreateOrEditItemDto createOrEditItemDto = ConvertItemDTOToModel(item);
                     await _IItemsAppService.CreateOrEditItems(createOrEditItemDto);
                   // CreateItem(item , MinTypeID[i]);

                }
                try
                {
                    DeleteDuplicate(LitstSub[i].MenuType);
                }catch(Exception ex)
                {

                    throw   ex;
                }
                
            }






            //try
            //{
            //    // Check if file exists with its full path    
            //    //if (System.IO.File.Exists("wwwroot\\UploadFile\\" + file.FormFile.FileName))
            //    //{
            //    //    // If file found, delete it    
            //    //    System.IO.File.Delete("wwwroot\\UploadFile\\" + file.FormFile.FileName);
            //    //    Console.WriteLine("File deleted.");
            //    //}
            //    //else Console.WriteLine("File not found");
            //}
            //catch (IOException ioExp)
            //{
            //  //  Console.WriteLine(ioExp.Message);
            //    return "NotFound";
            //}



            return "ok";
        }

        [Route("SecriptTen")]
        [HttpGet]
        public void SecriptTen()
        {





        }

        private static void DeleteDuplicate(int branchCodeID)
        {

            string connetionString;
            string storedProcedureName = "CtownItemsDuplicateDelete";
            SqlConnection cnn;
             connetionString = AppSettingsModel.ConnectionStrings;
            //connetionString = "Server=tcp:info-seed-db-server-prod.database.windows.net,1433;Initial Catalog=InfoSeedDB-Prod;Persist Security Info=False;User ID=devadmin;Password=P@ssw0rd;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

            try
            {
                cnn = new SqlConnection(connetionString);
                SqlCommand command = new SqlCommand();
                command.Connection = cnn;
                command.CommandText = storedProcedureName;
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@BranchCodeID", branchCodeID));


                cnn.Open();

                command.ExecuteNonQuery();

                cnn.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
           
        }

        private List<ItemDto> GetItem(int? TenantID, int MenuType , int ItemCategoryId ,int ItemSubCategoryId)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[Items] where TenantID=" + TenantID + " and MenuType = " + MenuType + " and ItemCategoryId = "+ ItemCategoryId + " and ItemSubCategoryId ="+ ItemSubCategoryId;


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
                            ItemCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemCategoryId"]),
                            ItemSubCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemSubCategoryId"]),
                            Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                           // CategoryNames = dataSet.Tables[0].Rows[i]["CategoryNames"].ToString(),
                           // CategoryNamesEnglish = dataSet.Tables[0].Rows[i]["CategoryNamesEnglish"].ToString(),
                            IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString()),
                            ImageUri = dataSet.Tables[0].Rows[i]["ImageUri"].ToString() ?? "",
                            IsInService = bool.Parse(dataSet.Tables[0].Rows[i]["IsInService"].ToString()),
                            ItemDescription = dataSet.Tables[0].Rows[i]["ItemDescription"].ToString(),
                            ItemDescriptionEnglish = dataSet.Tables[0].Rows[i]["ItemDescriptionEnglish"].ToString(),
                            ItemName = dataSet.Tables[0].Rows[i]["ItemName"].ToString(),
                            ItemNameEnglish = dataSet.Tables[0].Rows[i]["ItemNameEnglish"].ToString(),
                            Price = decimal.Parse(dataSet.Tables[0].Rows[i]["Price"].ToString()),
                            Priority = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Priority"]),
                            MenuId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["MenuId"]),
                            Barcode = dataSet.Tables[0].Rows[i]["Barcode"].ToString(),
                            SKU = dataSet.Tables[0].Rows[i]["SKU"].ToString() ?? "",
                            Size = dataSet.Tables[0].Rows[i]["Size"].ToString() ?? "",
                            Qty = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Qty"] ?? 0),

                        });
                    }
                    catch
                    {

                        try
                        {
                            itemDtos.Add(new ItemDto
                            {
                                ItemCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemCategoryId"]),
                                ItemSubCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemSubCategoryId"]),
                                Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                              //  CategoryNames = dataSet.Tables[0].Rows[i]["CategoryNames"].ToString(),
                              //  CategoryNamesEnglish = dataSet.Tables[0].Rows[i]["CategoryNamesEnglish"].ToString(),
                                IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString()),
                                ImageUri = dataSet.Tables[0].Rows[i]["ImageUri"].ToString() ?? "",
                                IsInService = bool.Parse(dataSet.Tables[0].Rows[i]["IsInService"].ToString()),
                                ItemDescription = dataSet.Tables[0].Rows[i]["ItemDescription"].ToString(),
                                ItemDescriptionEnglish = dataSet.Tables[0].Rows[i]["ItemDescriptionEnglish"].ToString(),
                                ItemName = dataSet.Tables[0].Rows[i]["ItemName"].ToString(),
                                ItemNameEnglish = dataSet.Tables[0].Rows[i]["ItemNameEnglish"].ToString(),
                                Price = decimal.Parse(dataSet.Tables[0].Rows[i]["Price"].ToString()),
                                Priority = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Priority"]),
                                MenuId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["MenuId"]),
                                Barcode = dataSet.Tables[0].Rows[i]["Barcode"].ToString(),
                                SKU = dataSet.Tables[0].Rows[i]["SKU"].ToString() ?? "",
                               // Size = dataSet.Tables[0].Rows[i]["Size"].ToString() ?? "",
                                 Qty = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Qty"] ?? 0),

                            });

                        }
                        catch
                        {

                            try
                            {
                                itemDtos.Add(new ItemDto
                                {
                                    ItemCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemCategoryId"]),
                                    ItemSubCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemSubCategoryId"]),
                                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                                   // CategoryNames = dataSet.Tables[0].Rows[i]["CategoryNames"].ToString(),
                                  //  CategoryNamesEnglish = dataSet.Tables[0].Rows[i]["CategoryNamesEnglish"].ToString(),
                                    IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString()),
                                    // ImageUri = dataSet.Tables[0].Rows[i]["ImageUri"].ToString() ?? "",
                                    IsInService = bool.Parse(dataSet.Tables[0].Rows[i]["IsInService"].ToString()),
                                    ItemDescription = dataSet.Tables[0].Rows[i]["ItemDescription"].ToString(),
                                    ItemDescriptionEnglish = dataSet.Tables[0].Rows[i]["ItemDescriptionEnglish"].ToString(),
                                    ItemName = dataSet.Tables[0].Rows[i]["ItemName"].ToString(),
                                    ItemNameEnglish = dataSet.Tables[0].Rows[i]["ItemNameEnglish"].ToString(),
                                    Price = decimal.Parse(dataSet.Tables[0].Rows[i]["Price"].ToString()),
                                    Priority = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Priority"]),
                                    MenuId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["MenuId"]),
                                    Barcode = dataSet.Tables[0].Rows[i]["Barcode"].ToString(),
                                    SKU = dataSet.Tables[0].Rows[i]["SKU"].ToString() ?? "",
                                    Size = dataSet.Tables[0].Rows[i]["Size"].ToString() ?? "",
                                    Qty = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Qty"] ?? 0),

                                });

                            }
                            catch
                            {
                                itemDtos.Add(new ItemDto
                                {
                                    ItemCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemCategoryId"]),
                                    ItemSubCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemSubCategoryId"]),
                                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                                   // CategoryNames = dataSet.Tables[0].Rows[i]["CategoryNames"].ToString(),
                                  //  CategoryNamesEnglish = dataSet.Tables[0].Rows[i]["CategoryNamesEnglish"].ToString(),
                                    IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString()),
                                    // ImageUri = dataSet.Tables[0].Rows[i]["ImageUri"].ToString() ?? "",
                                    IsInService = bool.Parse(dataSet.Tables[0].Rows[i]["IsInService"].ToString()),
                                    ItemDescription = dataSet.Tables[0].Rows[i]["ItemDescription"].ToString(),
                                    ItemDescriptionEnglish = dataSet.Tables[0].Rows[i]["ItemDescriptionEnglish"].ToString(),
                                    ItemName = dataSet.Tables[0].Rows[i]["ItemName"].ToString(),
                                    ItemNameEnglish = dataSet.Tables[0].Rows[i]["ItemNameEnglish"].ToString(),
                                    Price = decimal.Parse(dataSet.Tables[0].Rows[i]["Price"].ToString()),
                                    Priority = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Priority"]),
                                    MenuId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["MenuId"]),
                                    Barcode = dataSet.Tables[0].Rows[i]["Barcode"].ToString(),
                                    SKU = dataSet.Tables[0].Rows[i]["SKU"].ToString() ?? "",
                                    //Size = dataSet.Tables[0].Rows[i]["Size"].ToString() ?? "",
                                    Qty = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Qty"] ?? 0),

                                });


                            }



                        }


                    }


                }

            }

            conn.Close();
            da.Dispose();

            return itemDtos;

        }


        private List<ItemDto> GeAlltItemWs(int MenuType, int ItemCategoryId)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[Items] where  MenuType= "+ MenuType + "and [ItemSubCategoryId]=" + ItemCategoryId;


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
                             TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
                            MenuType = Convert.ToInt32(dataSet.Tables[0].Rows[i]["MenuType"]),
                            ItemCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemCategoryId"]),
                            ItemSubCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemSubCategoryId"]),
                            Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                            // CategoryNames = dataSet.Tables[0].Rows[i]["CategoryNames"].ToString(),
                            // CategoryNamesEnglish = dataSet.Tables[0].Rows[i]["CategoryNamesEnglish"].ToString(),
                            IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString()),
                            ImageUri = dataSet.Tables[0].Rows[i]["ImageUri"].ToString() ?? "",
                            IsInService = bool.Parse(dataSet.Tables[0].Rows[i]["IsInService"].ToString()),
                            ItemDescription = dataSet.Tables[0].Rows[i]["ItemDescription"].ToString(),
                            ItemDescriptionEnglish = dataSet.Tables[0].Rows[i]["ItemDescriptionEnglish"].ToString(),
                            ItemName = dataSet.Tables[0].Rows[i]["ItemName"].ToString(),
                            ItemNameEnglish = dataSet.Tables[0].Rows[i]["ItemNameEnglish"].ToString(),
                            Price = decimal.Parse(dataSet.Tables[0].Rows[i]["Price"].ToString()),
                            Priority = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Priority"]),
                            MenuId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["MenuId"]),
                            Barcode =  dataSet.Tables[0].Rows[i]["Barcode"].ToString(),
                            SKU = dataSet.Tables[0].Rows[i]["SKU"].ToString() ?? "",
                            Size = dataSet.Tables[0].Rows[i]["Size"].ToString() ?? "",
                            Qty = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Qty"] ?? 0),

                        });
                    }
                    catch
                    {

                        try
                        {
                            itemDtos.Add(new ItemDto
                            {
                                ItemCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemCategoryId"]),
                                ItemSubCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemSubCategoryId"]),
                                Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                                //  CategoryNames = dataSet.Tables[0].Rows[i]["CategoryNames"].ToString(),
                                //  CategoryNamesEnglish = dataSet.Tables[0].Rows[i]["CategoryNamesEnglish"].ToString(),
                                IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString()),
                                ImageUri = dataSet.Tables[0].Rows[i]["ImageUri"].ToString() ?? "",
                                IsInService = bool.Parse(dataSet.Tables[0].Rows[i]["IsInService"].ToString()),
                                ItemDescription = dataSet.Tables[0].Rows[i]["ItemDescription"].ToString(),
                                ItemDescriptionEnglish = dataSet.Tables[0].Rows[i]["ItemDescriptionEnglish"].ToString(),
                                ItemName = dataSet.Tables[0].Rows[i]["ItemName"].ToString(),
                                ItemNameEnglish = dataSet.Tables[0].Rows[i]["ItemNameEnglish"].ToString(),
                                Price = decimal.Parse(dataSet.Tables[0].Rows[i]["Price"].ToString()),
                                Priority = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Priority"]),
                                MenuId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["MenuId"]),
                                Barcode = dataSet.Tables[0].Rows[i]["Barcode"].ToString(),
                                SKU = dataSet.Tables[0].Rows[i]["SKU"].ToString() ?? "",
                                // Size = dataSet.Tables[0].Rows[i]["Size"].ToString() ?? "",
                                Qty = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Qty"] ?? 0),

                            });

                        }
                        catch
                        {

                            try
                            {
                                itemDtos.Add(new ItemDto
                                {
                                    ItemCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemCategoryId"]),
                                    ItemSubCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemSubCategoryId"]),
                                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                                    // CategoryNames = dataSet.Tables[0].Rows[i]["CategoryNames"].ToString(),
                                    //  CategoryNamesEnglish = dataSet.Tables[0].Rows[i]["CategoryNamesEnglish"].ToString(),
                                    IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString()),
                                    // ImageUri = dataSet.Tables[0].Rows[i]["ImageUri"].ToString() ?? "",
                                    IsInService = bool.Parse(dataSet.Tables[0].Rows[i]["IsInService"].ToString()),
                                    ItemDescription = dataSet.Tables[0].Rows[i]["ItemDescription"].ToString(),
                                    ItemDescriptionEnglish = dataSet.Tables[0].Rows[i]["ItemDescriptionEnglish"].ToString(),
                                    ItemName = dataSet.Tables[0].Rows[i]["ItemName"].ToString(),
                                    ItemNameEnglish = dataSet.Tables[0].Rows[i]["ItemNameEnglish"].ToString(),
                                    Price = decimal.Parse(dataSet.Tables[0].Rows[i]["Price"].ToString()),
                                    Priority = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Priority"]),
                                    MenuId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["MenuId"]),
                                    Barcode = dataSet.Tables[0].Rows[i]["Barcode"].ToString(),
                                    SKU = dataSet.Tables[0].Rows[i]["SKU"].ToString() ?? "",
                                    Size = dataSet.Tables[0].Rows[i]["Size"].ToString() ?? "",
                                    Qty = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Qty"] ?? 0),

                                });

                            }
                            catch
                            {
                                itemDtos.Add(new ItemDto
                                {
                                    ItemCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemCategoryId"]),
                                    ItemSubCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemSubCategoryId"]),
                                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                                    // CategoryNames = dataSet.Tables[0].Rows[i]["CategoryNames"].ToString(),
                                    //  CategoryNamesEnglish = dataSet.Tables[0].Rows[i]["CategoryNamesEnglish"].ToString(),
                                    IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString()),
                                    // ImageUri = dataSet.Tables[0].Rows[i]["ImageUri"].ToString() ?? "",
                                    IsInService = bool.Parse(dataSet.Tables[0].Rows[i]["IsInService"].ToString()),
                                    ItemDescription = dataSet.Tables[0].Rows[i]["ItemDescription"].ToString(),
                                    ItemDescriptionEnglish = dataSet.Tables[0].Rows[i]["ItemDescriptionEnglish"].ToString(),
                                    ItemName = dataSet.Tables[0].Rows[i]["ItemName"].ToString(),
                                    ItemNameEnglish = dataSet.Tables[0].Rows[i]["ItemNameEnglish"].ToString(),
                                    Price = decimal.Parse(dataSet.Tables[0].Rows[i]["Price"].ToString()),
                                    Priority = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Priority"]),
                                    MenuId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["MenuId"]),
                                    Barcode = dataSet.Tables[0].Rows[i]["Barcode"].ToString(),
                                    SKU = dataSet.Tables[0].Rows[i]["SKU"].ToString() ?? "",
                                    //Size = dataSet.Tables[0].Rows[i]["Size"].ToString() ?? "",
                                    Qty = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Qty"] ?? 0),

                                });


                            }



                        }


                    }


                }

            }

            conn.Close();
            da.Dispose();

            return itemDtos;

        }


        private List<ItemDto> GeAlltItemWCtown(int MenuType, int ItemCategoryId)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[Items] where  MenuType= " + MenuType + "and [ItemSubCategoryId]<>" + ItemCategoryId;


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
                            TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
                            MenuType = Convert.ToInt32(dataSet.Tables[0].Rows[i]["MenuType"]),
                            ItemCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemCategoryId"]),
                            ItemSubCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemSubCategoryId"]),
                            Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                            // CategoryNames = dataSet.Tables[0].Rows[i]["CategoryNames"].ToString(),
                            // CategoryNamesEnglish = dataSet.Tables[0].Rows[i]["CategoryNamesEnglish"].ToString(),
                            IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString()),
                            ImageUri = dataSet.Tables[0].Rows[i]["ImageUri"].ToString() ?? "",
                            IsInService = bool.Parse(dataSet.Tables[0].Rows[i]["IsInService"].ToString()),
                            ItemDescription = dataSet.Tables[0].Rows[i]["ItemDescription"].ToString(),
                            ItemDescriptionEnglish = dataSet.Tables[0].Rows[i]["ItemDescriptionEnglish"].ToString(),
                            ItemName = dataSet.Tables[0].Rows[i]["ItemName"].ToString(),
                            ItemNameEnglish = dataSet.Tables[0].Rows[i]["ItemNameEnglish"].ToString(),
                            Price = decimal.Parse(dataSet.Tables[0].Rows[i]["Price"].ToString()),
                            Priority = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Priority"]),
                            MenuId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["MenuId"]),
                            Barcode = dataSet.Tables[0].Rows[i]["Barcode"].ToString(),
                            SKU = dataSet.Tables[0].Rows[i]["SKU"].ToString() ?? "",
                            Size = dataSet.Tables[0].Rows[i]["Size"].ToString() ?? "",
                            Qty = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Qty"] ?? 0),

                        });
                    }
                    catch
                    {

                        try
                        {
                            itemDtos.Add(new ItemDto
                            {
                                ItemCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemCategoryId"]),
                                ItemSubCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemSubCategoryId"]),
                                Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                                //  CategoryNames = dataSet.Tables[0].Rows[i]["CategoryNames"].ToString(),
                                //  CategoryNamesEnglish = dataSet.Tables[0].Rows[i]["CategoryNamesEnglish"].ToString(),
                                IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString()),
                                ImageUri = dataSet.Tables[0].Rows[i]["ImageUri"].ToString() ?? "",
                                IsInService = bool.Parse(dataSet.Tables[0].Rows[i]["IsInService"].ToString()),
                                ItemDescription = dataSet.Tables[0].Rows[i]["ItemDescription"].ToString(),
                                ItemDescriptionEnglish = dataSet.Tables[0].Rows[i]["ItemDescriptionEnglish"].ToString(),
                                ItemName = dataSet.Tables[0].Rows[i]["ItemName"].ToString(),
                                ItemNameEnglish = dataSet.Tables[0].Rows[i]["ItemNameEnglish"].ToString(),
                                Price = decimal.Parse(dataSet.Tables[0].Rows[i]["Price"].ToString()),
                                Priority = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Priority"]),
                                MenuId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["MenuId"]),
                                Barcode = dataSet.Tables[0].Rows[i]["Barcode"].ToString(),
                                SKU = dataSet.Tables[0].Rows[i]["SKU"].ToString() ?? "",
                                // Size = dataSet.Tables[0].Rows[i]["Size"].ToString() ?? "",
                                Qty = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Qty"] ?? 0),

                            });

                        }
                        catch
                        {

                            try
                            {
                                itemDtos.Add(new ItemDto
                                {
                                    ItemCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemCategoryId"]),
                                    ItemSubCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemSubCategoryId"]),
                                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                                    // CategoryNames = dataSet.Tables[0].Rows[i]["CategoryNames"].ToString(),
                                    //  CategoryNamesEnglish = dataSet.Tables[0].Rows[i]["CategoryNamesEnglish"].ToString(),
                                    IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString()),
                                    // ImageUri = dataSet.Tables[0].Rows[i]["ImageUri"].ToString() ?? "",
                                    IsInService = bool.Parse(dataSet.Tables[0].Rows[i]["IsInService"].ToString()),
                                    ItemDescription = dataSet.Tables[0].Rows[i]["ItemDescription"].ToString(),
                                    ItemDescriptionEnglish = dataSet.Tables[0].Rows[i]["ItemDescriptionEnglish"].ToString(),
                                    ItemName = dataSet.Tables[0].Rows[i]["ItemName"].ToString(),
                                    ItemNameEnglish = dataSet.Tables[0].Rows[i]["ItemNameEnglish"].ToString(),
                                    Price = decimal.Parse(dataSet.Tables[0].Rows[i]["Price"].ToString()),
                                    Priority = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Priority"]),
                                    MenuId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["MenuId"]),
                                    Barcode = dataSet.Tables[0].Rows[i]["Barcode"].ToString(),
                                    SKU = dataSet.Tables[0].Rows[i]["SKU"].ToString() ?? "",
                                    Size = dataSet.Tables[0].Rows[i]["Size"].ToString() ?? "",
                                    Qty = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Qty"] ?? 0),

                                });

                            }
                            catch
                            {
                                itemDtos.Add(new ItemDto
                                {
                                    ItemCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemCategoryId"]),
                                    ItemSubCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemSubCategoryId"]),
                                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                                    // CategoryNames = dataSet.Tables[0].Rows[i]["CategoryNames"].ToString(),
                                    //  CategoryNamesEnglish = dataSet.Tables[0].Rows[i]["CategoryNamesEnglish"].ToString(),
                                    IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString()),
                                    // ImageUri = dataSet.Tables[0].Rows[i]["ImageUri"].ToString() ?? "",
                                    IsInService = bool.Parse(dataSet.Tables[0].Rows[i]["IsInService"].ToString()),
                                    ItemDescription = dataSet.Tables[0].Rows[i]["ItemDescription"].ToString(),
                                    ItemDescriptionEnglish = dataSet.Tables[0].Rows[i]["ItemDescriptionEnglish"].ToString(),
                                    ItemName = dataSet.Tables[0].Rows[i]["ItemName"].ToString(),
                                    ItemNameEnglish = dataSet.Tables[0].Rows[i]["ItemNameEnglish"].ToString(),
                                    Price = decimal.Parse(dataSet.Tables[0].Rows[i]["Price"].ToString()),
                                    Priority = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Priority"]),
                                    MenuId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["MenuId"]),
                                    Barcode = dataSet.Tables[0].Rows[i]["Barcode"].ToString(),
                                    SKU = dataSet.Tables[0].Rows[i]["SKU"].ToString() ?? "",
                                    //Size = dataSet.Tables[0].Rows[i]["Size"].ToString() ?? "",
                                    Qty = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Qty"] ?? 0),

                                });


                            }



                        }


                    }


                }

            }

            conn.Close();
            da.Dispose();

            return itemDtos;

        }



        private List<ItemDto> GeAlltItem(int MenuType)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[Items] where  MenuType= " + MenuType ;


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
                            TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
                            MenuType = Convert.ToInt32(dataSet.Tables[0].Rows[i]["MenuType"]),
                            ItemCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemCategoryId"]),
                            ItemSubCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemSubCategoryId"]),
                            Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                            // CategoryNames = dataSet.Tables[0].Rows[i]["CategoryNames"].ToString(),
                            // CategoryNamesEnglish = dataSet.Tables[0].Rows[i]["CategoryNamesEnglish"].ToString(),
                            IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString()),
                            ImageUri = dataSet.Tables[0].Rows[i]["ImageUri"].ToString() ?? "",
                            IsInService = bool.Parse(dataSet.Tables[0].Rows[i]["IsInService"].ToString()),
                            ItemDescription = dataSet.Tables[0].Rows[i]["ItemDescription"].ToString(),
                            ItemDescriptionEnglish = dataSet.Tables[0].Rows[i]["ItemDescriptionEnglish"].ToString(),
                            ItemName = dataSet.Tables[0].Rows[i]["ItemName"].ToString(),
                            ItemNameEnglish = dataSet.Tables[0].Rows[i]["ItemNameEnglish"].ToString(),
                            Price = decimal.Parse(dataSet.Tables[0].Rows[i]["Price"].ToString()),
                            Priority = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Priority"]),
                            MenuId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["MenuId"]),
                            Barcode = dataSet.Tables[0].Rows[i]["Barcode"].ToString(),
                            SKU = dataSet.Tables[0].Rows[i]["SKU"].ToString() ?? "",
                            Size = dataSet.Tables[0].Rows[i]["Size"].ToString() ?? "",
                            Qty = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Qty"] ?? 0),

                        });
                    }
                    catch
                    {

                        try
                        {
                            itemDtos.Add(new ItemDto
                            {
                                ItemCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemCategoryId"]),
                                ItemSubCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemSubCategoryId"]),
                                Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                                //  CategoryNames = dataSet.Tables[0].Rows[i]["CategoryNames"].ToString(),
                                //  CategoryNamesEnglish = dataSet.Tables[0].Rows[i]["CategoryNamesEnglish"].ToString(),
                                IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString()),
                                ImageUri = dataSet.Tables[0].Rows[i]["ImageUri"].ToString() ?? "",
                                IsInService = bool.Parse(dataSet.Tables[0].Rows[i]["IsInService"].ToString()),
                                ItemDescription = dataSet.Tables[0].Rows[i]["ItemDescription"].ToString(),
                                ItemDescriptionEnglish = dataSet.Tables[0].Rows[i]["ItemDescriptionEnglish"].ToString(),
                                ItemName = dataSet.Tables[0].Rows[i]["ItemName"].ToString(),
                                ItemNameEnglish = dataSet.Tables[0].Rows[i]["ItemNameEnglish"].ToString(),
                                Price = decimal.Parse(dataSet.Tables[0].Rows[i]["Price"].ToString()),
                                Priority = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Priority"]),
                                MenuId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["MenuId"]),
                                Barcode = dataSet.Tables[0].Rows[i]["Barcode"].ToString(),
                                SKU = dataSet.Tables[0].Rows[i]["SKU"].ToString() ?? "",
                                // Size = dataSet.Tables[0].Rows[i]["Size"].ToString() ?? "",
                                Qty = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Qty"] ?? 0),

                            });

                        }
                        catch
                        {

                            try
                            {
                                itemDtos.Add(new ItemDto
                                {
                                    ItemCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemCategoryId"]),
                                    ItemSubCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemSubCategoryId"]),
                                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                                    // CategoryNames = dataSet.Tables[0].Rows[i]["CategoryNames"].ToString(),
                                    //  CategoryNamesEnglish = dataSet.Tables[0].Rows[i]["CategoryNamesEnglish"].ToString(),
                                    IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString()),
                                    // ImageUri = dataSet.Tables[0].Rows[i]["ImageUri"].ToString() ?? "",
                                    IsInService = bool.Parse(dataSet.Tables[0].Rows[i]["IsInService"].ToString()),
                                    ItemDescription = dataSet.Tables[0].Rows[i]["ItemDescription"].ToString(),
                                    ItemDescriptionEnglish = dataSet.Tables[0].Rows[i]["ItemDescriptionEnglish"].ToString(),
                                    ItemName = dataSet.Tables[0].Rows[i]["ItemName"].ToString(),
                                    ItemNameEnglish = dataSet.Tables[0].Rows[i]["ItemNameEnglish"].ToString(),
                                    Price = decimal.Parse(dataSet.Tables[0].Rows[i]["Price"].ToString()),
                                    Priority = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Priority"]),
                                    MenuId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["MenuId"]),
                                    Barcode = dataSet.Tables[0].Rows[i]["Barcode"].ToString(),
                                    SKU = dataSet.Tables[0].Rows[i]["SKU"].ToString() ?? "",
                                    Size = dataSet.Tables[0].Rows[i]["Size"].ToString() ?? "",
                                    Qty = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Qty"] ?? 0),

                                });

                            }
                            catch
                            {
                                itemDtos.Add(new ItemDto
                                {
                                    ItemCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemCategoryId"]),
                                    ItemSubCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemSubCategoryId"]),
                                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                                    // CategoryNames = dataSet.Tables[0].Rows[i]["CategoryNames"].ToString(),
                                    //  CategoryNamesEnglish = dataSet.Tables[0].Rows[i]["CategoryNamesEnglish"].ToString(),
                                    IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString()),
                                    // ImageUri = dataSet.Tables[0].Rows[i]["ImageUri"].ToString() ?? "",
                                    IsInService = bool.Parse(dataSet.Tables[0].Rows[i]["IsInService"].ToString()),
                                    ItemDescription = dataSet.Tables[0].Rows[i]["ItemDescription"].ToString(),
                                    ItemDescriptionEnglish = dataSet.Tables[0].Rows[i]["ItemDescriptionEnglish"].ToString(),
                                    ItemName = dataSet.Tables[0].Rows[i]["ItemName"].ToString(),
                                    ItemNameEnglish = dataSet.Tables[0].Rows[i]["ItemNameEnglish"].ToString(),
                                    Price = decimal.Parse(dataSet.Tables[0].Rows[i]["Price"].ToString()),
                                    Priority = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Priority"]),
                                    MenuId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["MenuId"]),
                                    Barcode = dataSet.Tables[0].Rows[i]["Barcode"].ToString(),
                                    SKU = dataSet.Tables[0].Rows[i]["SKU"].ToString() ?? "",
                                    //Size = dataSet.Tables[0].Rows[i]["Size"].ToString() ?? "",
                                    Qty = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Qty"] ?? 0),

                                });


                            }



                        }


                    }


                }

            }

            conn.Close();
            da.Dispose();

            return itemDtos;

        }

        private List<ItemCategory> GetCategory(int? TenantID, int MenuType )
        {
            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[ItemCategorys]  where TenantID=" + TenantID + " and MenuType = " + MenuType;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<ItemCategory> itemDtos = new List<ItemCategory>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                var IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString());

                if (!IsDeleted)
                {
                    itemDtos.Add(new ItemCategory
                    {
                        
                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                        Name = dataSet.Tables[0].Rows[i]["Name"].ToString(),
                        NameEnglish = dataSet.Tables[0].Rows[i]["NameEnglish"].ToString(),
                        MenuType = int.Parse(dataSet.Tables[0].Rows[i]["MenuType"].ToString()),
                       

                    });

                }

            }

            conn.Close();
            da.Dispose();

            return itemDtos;

        }


        private List<ItemSubCategory> GetSubCategory(int? TenantID, int MenuType)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[ItemSubCategories] where TenantID=" + TenantID + " and MenuType = " + MenuType;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<ItemSubCategory> itemDtos = new List<ItemSubCategory>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                var IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString());

                if (!IsDeleted)
                {
                    itemDtos.Add(new ItemSubCategory
                    {

                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                        TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
                        ItemCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemCategoryId"]),
                        Name = dataSet.Tables[0].Rows[i]["Name"].ToString(),
                        NameEnglish = dataSet.Tables[0].Rows[i]["NameEnglish"].ToString(),
                        MenuType = int.Parse(dataSet.Tables[0].Rows[i]["MenuType"].ToString()),


                    });

                }

            }

            conn.Close();
            da.Dispose();

            return itemDtos;

        }

        private ItemSubCategory GetSubCategoryId( long Id)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[ItemSubCategories] where  Id = " + Id;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            ItemSubCategory itemDtos = new ItemSubCategory();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                var IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString());

                if (!IsDeleted)
                {
                    itemDtos=new ItemSubCategory()
                    {

                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                        TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
                        ItemCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemCategoryId"]),
                        Name = dataSet.Tables[0].Rows[i]["Name"].ToString(),
                        NameEnglish = dataSet.Tables[0].Rows[i]["NameEnglish"].ToString(),
                        MenuType = int.Parse(dataSet.Tables[0].Rows[i]["MenuType"].ToString()),


                    };

                }

            }

            conn.Close();
            da.Dispose();

            return itemDtos;

        }

        private List<ItemSubCategory> GetAllSubCategory(int? TenantID, string SubCategoryName)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[ItemSubCategories] where TenantID=" + TenantID + " and NameEnglish = N'" + SubCategoryName+"'";// + "' and MenuType !=189 and  MenuType !=190 ";


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<ItemSubCategory> itemDtos = new List<ItemSubCategory>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                var IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString());

                if (!IsDeleted)
                {
                    itemDtos.Add(new ItemSubCategory
                    {

                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                        TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
                        ItemCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemCategoryId"]),
                        Name = dataSet.Tables[0].Rows[i]["Name"].ToString(),
                        NameEnglish = dataSet.Tables[0].Rows[i]["NameEnglish"].ToString(),
                        MenuType = int.Parse(dataSet.Tables[0].Rows[i]["MenuType"].ToString()),


                    });

                }

            }

            conn.Close();
            da.Dispose();

            return itemDtos;

        }


        private List<ItemSubCategory> GetAllSubCategoryE(int? TenantID, string SubCategoryNameE)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[ItemSubCategories] where TenantID=" + TenantID + " and [NameEnglish] = N'" + SubCategoryNameE + "' and (MenuType =188 or MenuType =189 or  MenuType =190 )";


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<ItemSubCategory> itemDtos = new List<ItemSubCategory>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                var IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString());

                if (!IsDeleted)
                {
                    itemDtos.Add(new ItemSubCategory
                    {

                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                        TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
                        ItemCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemCategoryId"]),
                        Name = dataSet.Tables[0].Rows[i]["Name"].ToString(),
                        NameEnglish = dataSet.Tables[0].Rows[i]["NameEnglish"].ToString(),
                        MenuType = int.Parse(dataSet.Tables[0].Rows[i]["MenuType"].ToString()),


                    });

                }

            }

            conn.Close();
            da.Dispose();

            return itemDtos;

        }
        private int CreateSubCategory(ItemSubCategory  itemSub)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            using (SqlConnection connection = new SqlConnection(connString))
                try
                {
                    int modified = 0;
                    using (SqlCommand command = connection.CreateCommand())
                    {

                        command.CommandText = "INSERT INTO ItemSubCategories "
                            + " (ItemCategoryId ,Name ,NameEnglish ,MenuType ,TenantId, IsDeleted) VALUES "
                            + " (@ItemCategoryId, @Name ,@NameEnglish ,@MenuType ,@TenantId, @IsDeleted) ;SELECT SCOPE_IDENTITY();";

                        command.Parameters.AddWithValue("@ItemCategoryId", itemSub.ItemCategoryId);
                        command.Parameters.AddWithValue("@Name", itemSub.Name);
                        command.Parameters.AddWithValue("@NameEnglish", itemSub.NameEnglish);
                        command.Parameters.AddWithValue("@MenuType", itemSub.MenuType);
                        command.Parameters.AddWithValue("@TenantId", itemSub.TenantId);
                        command.Parameters.AddWithValue("@IsDeleted", false);





                        connection.Open();
                        modified = Convert.ToInt32(command.ExecuteScalar());
                        if (connection.State == System.Data.ConnectionState.Open) connection.Close();


                        return modified;
                    }

                }
                catch (Exception )
                {

                    return  0; 
                }

        }

        private void CreateItem(ItemDto  item ,int AreaId =0)
        {
            if (!item.SKU.ToUpper().StartsWith("T") && item.Size.Contains("1 KG"))
            {
                item.IsQuantitative = true;

            }

       

           
            string connString = AppSettingsModel.ConnectionStrings;
            using (SqlConnection connection = new SqlConnection(connString))
                try
                {

                    if (item.DateFrom == null)
                    {
                        using (SqlCommand command = connection.CreateCommand())
                        {

                            command.CommandText = "INSERT INTO Items "
                                + " (ItemCategoryId ,ItemSubCategoryId ,ImageUri ,IsDeleted ,IsInService, ItemName, ItemNameEnglish ,Price,Priority,MenuId,Barcode,SKU,Size,Qty,MenuType,CreationTime,LanguageBotId,TenantId  ,Status_Code ,AreaIds ) VALUES "
                                + " (@ItemCategoryId ,@ItemSubCategoryId ,@ImageUri ,@IsDeleted,@IsInService, @ItemName ,@ItemNameEnglish,@Price,@Priority ,@MenuId,@Barcode,@SKU,@Size,@Qty,@MenuType,@CreationTime,@LanguageBotId,@TenantId  ,@Status_Code , @AreaIds ) ";

                            command.Parameters.AddWithValue("@ItemCategoryId", item.ItemCategoryId);
                            command.Parameters.AddWithValue("@ItemSubCategoryId", item.ItemSubCategoryId);
                            command.Parameters.AddWithValue("@ImageUri", "");
                            command.Parameters.AddWithValue("@IsDeleted", item.IsDeleted);
                            command.Parameters.AddWithValue("@IsInService", item.IsInService);
                            command.Parameters.AddWithValue("@ItemName", item.ItemName);
                            command.Parameters.AddWithValue("@ItemNameEnglish", item.ItemNameEnglish);

                            command.Parameters.AddWithValue("@Price", item.Price);
                            command.Parameters.AddWithValue("@Priority", item.Priority);
                            command.Parameters.AddWithValue("@MenuId", item.MenuId);
                            command.Parameters.AddWithValue("@Barcode", item.Barcode);
                            command.Parameters.AddWithValue("@SKU", item.SKU);
                            command.Parameters.AddWithValue("@Size", item.Size);
                            command.Parameters.AddWithValue("@MenuType", item.MenuType);

                            command.Parameters.AddWithValue("@CreationTime", DateTime.Now);
                            command.Parameters.AddWithValue("@LanguageBotId", 1);
                            command.Parameters.AddWithValue("@TenantId", item.TenantId);

                           // command.Parameters.AddWithValue("@DateFrom", item.DateFrom);
                           // command.Parameters.AddWithValue("@DateTo", item.DateTo);
                           // command.Parameters.AddWithValue("@OldPrice", item.OldPrice);
                            command.Parameters.AddWithValue("@Status_Code", item.Status_Code);
                            command.Parameters.AddWithValue("@Qty", item.Qty);


                            command.Parameters.AddWithValue("@AreaIds", AreaId);

                            connection.Open();
                            command.ExecuteNonQuery();
                            connection.Close();
                        }

                    }
                    else
                    {
                        using (SqlCommand command = connection.CreateCommand())
                        {

                            command.CommandText = "INSERT INTO Items "
                                + " (ItemCategoryId ,ItemSubCategoryId ,ImageUri ,IsDeleted ,IsInService, ItemName, ItemNameEnglish ,Price,Priority,MenuId,Barcode,SKU,Size,Qty,MenuType,CreationTime,LanguageBotId,TenantId  ,DateFrom,DateTo,OldPrice,Status_Code ) VALUES "
                                + " (@ItemCategoryId ,@ItemSubCategoryId ,@ImageUri ,@IsDeleted,@IsInService, @ItemName ,@ItemNameEnglish,@Price,@Priority ,@MenuId,@Barcode,@SKU,@Size,@Qty,@MenuType,@CreationTime,@LanguageBotId,@TenantId  ,@DateFrom,@DateTo,@OldPrice,@Status_Code ) ";

                            command.Parameters.AddWithValue("@ItemCategoryId", item.ItemCategoryId);
                            command.Parameters.AddWithValue("@ItemSubCategoryId", item.ItemSubCategoryId);
                            command.Parameters.AddWithValue("@ImageUri", "");
                            command.Parameters.AddWithValue("@IsDeleted", item.IsDeleted);
                            command.Parameters.AddWithValue("@IsInService", item.IsInService);
                            command.Parameters.AddWithValue("@ItemName", item.ItemName);
                            command.Parameters.AddWithValue("@ItemNameEnglish", item.ItemNameEnglish);

                            command.Parameters.AddWithValue("@Price", item.Price);
                            command.Parameters.AddWithValue("@Priority", item.Priority);
                            command.Parameters.AddWithValue("@MenuId", item.MenuId);
                            command.Parameters.AddWithValue("@Barcode", item.Barcode);
                            command.Parameters.AddWithValue("@SKU", item.SKU);
                            command.Parameters.AddWithValue("@Size", item.Size);
                            command.Parameters.AddWithValue("@MenuType", item.MenuType);

                            command.Parameters.AddWithValue("@CreationTime", DateTime.Now);
                            command.Parameters.AddWithValue("@LanguageBotId", 1);
                            command.Parameters.AddWithValue("@TenantId", item.TenantId);

                            command.Parameters.AddWithValue("@DateFrom", item.DateFrom);
                            command.Parameters.AddWithValue("@DateTo", item.DateTo);
                            command.Parameters.AddWithValue("@OldPrice", item.OldPrice);
                            command.Parameters.AddWithValue("@Status_Code", item.Status_Code);
                            command.Parameters.AddWithValue("@Qty", item.Qty);

                            connection.Open();
                            command.ExecuteNonQuery();
                            connection.Close();
                        }


                    }
                  
                }
                catch (Exception )
                {


                }

        }

        private void CreateItemWithImage(ItemDto item)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            using (SqlConnection connection = new SqlConnection(connString))
                try
                {

                    using (SqlCommand command = connection.CreateCommand())
                    {

                        command.CommandText = "INSERT INTO Items "
                            + " (ItemCategoryId ,ItemSubCategoryId ,ImageUri ,IsDeleted ,IsInService, ItemName, ItemNameEnglish ,Price,Priority,MenuId,Barcode,SKU,Size,Qty,MenuType,CreationTime,LanguageBotId,TenantId) VALUES "
                            + " (@ItemCategoryId ,@ItemSubCategoryId ,@ImageUri ,@IsDeleted,@IsInService, @ItemName ,@ItemNameEnglish,@Price,@Priority ,@MenuId,@Barcode,@SKU,@Size,@Qty,@MenuType,@CreationTime,@LanguageBotId,@TenantId) ";

                        command.Parameters.AddWithValue("@ItemCategoryId", item.ItemCategoryId);
                        command.Parameters.AddWithValue("@ItemSubCategoryId", item.ItemSubCategoryId);
                        command.Parameters.AddWithValue("@ImageUri", item.ImageUri);
                        command.Parameters.AddWithValue("@IsDeleted", item.IsDeleted);
                        command.Parameters.AddWithValue("@IsInService", item.IsInService);
                        command.Parameters.AddWithValue("@ItemName", item.ItemName);
                        command.Parameters.AddWithValue("@ItemNameEnglish", item.ItemNameEnglish);

                        command.Parameters.AddWithValue("@Price", item.Price);
                        command.Parameters.AddWithValue("@Priority", item.Priority);
                        command.Parameters.AddWithValue("@MenuId", item.MenuId);
                        command.Parameters.AddWithValue("@Barcode", item.Barcode);
                        command.Parameters.AddWithValue("@SKU", item.SKU);
                        command.Parameters.AddWithValue("@Size", item.Size);
                        command.Parameters.AddWithValue("@Qty", item.Qty);
                        command.Parameters.AddWithValue("@MenuType", item.MenuType);

                        command.Parameters.AddWithValue("@CreationTime", DateTime.Now);
                        command.Parameters.AddWithValue("@LanguageBotId", 1);
                        command.Parameters.AddWithValue("@TenantId", item.TenantId);

                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
                catch (Exception )
                {


                }

        }



        private void UpdateItemImage(ItemDto item)
        {


            string connString = AppSettingsModel.ConnectionStrings;
            using (SqlConnection connection = new SqlConnection(connString))
                try
                {

                    using (SqlCommand command = connection.CreateCommand())
                    {
                            command.CommandText = "UPDATE [dbo].[Items] SET "
                                 + "ImageUri = @ImageUri  "                              
                                 + " Where Barcode = @Barcode";
                    
                        command.Parameters.AddWithValue("@ImageUri", item.ImageUri);

                        command.Parameters.AddWithValue("@Barcode", item.Barcode);



                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
                catch (Exception )
                {


                }

        }


        private void UpdateItem(ItemDto item)
        {

            string connString = AppSettingsModel.ConnectionStrings;
            using (SqlConnection connection = new SqlConnection(connString))
                try
                {

                    using (SqlCommand command = connection.CreateCommand())
                    {


                        if(item.DateFrom ==null )
                        {

                            command.CommandText = "UPDATE [dbo].[Items] SET "


                                   + "DateFrom = Null , "
                                    + "DateTo = Null , "
                                    + "OldPrice = Null , "

                                 + "ImageUri = @ImageUri , "
                                 + "ItemName = @ItemName , "
                                 + "ItemNameEnglish = @ItemNameEnglish , "

                                  + "Size = @Size , "
                                   + "Qty = @Qty , "

                                 + "Price = @Price , "
                                 + "Priority = @Priority  "
                                 + " Where Barcode = @Barcode";

                        }
                        else
                        {
                            command.CommandText = "UPDATE [dbo].[Items] SET "

                                  + "DateFrom = @DateFrom , "
                                    + "DateTo = @DateTo , "                                
                                      + "Size = @Size , "
                                      + "Qty = @Qty , "

                                 + "ImageUri = @ImageUri , "
                                 + "ItemName = @ItemName , "
                                 + "ItemNameEnglish = @ItemNameEnglish , "
                                 + "Price = @Price , "
                                 + "OldPrice = @OldPrice , "
                                 + "Priority = @Priority  "
                                 + " Where Barcode = @Barcode";


                        }

                        command.Parameters.AddWithValue("@ImageUri", item.ImageUri);
                        command.Parameters.AddWithValue("@ItemName", item.ItemName);
                        command.Parameters.AddWithValue("@ItemNameEnglish", item.ItemNameEnglish);
                        command.Parameters.AddWithValue("@Price", item.Price);
                        command.Parameters.AddWithValue("@Priority", item.Priority);
                        command.Parameters.AddWithValue("@Barcode", item.Barcode);
                        command.Parameters.AddWithValue("@Size", item.ItemDescription);
                        command.Parameters.AddWithValue("@Qty", item.Qty);


                        if (item.DateFrom != null)
                        {
                            command.Parameters.AddWithValue("@DateFrom", item.DateFrom);
                            command.Parameters.AddWithValue("@DateTo", item.DateTo);
                            command.Parameters.AddWithValue("@OldPrice", item.OldPrice);

                        }



                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
                catch (Exception )
                {


                }

        }


        private List<BarCodeModel> GetBarCode()
        {
            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [ctownjo].[AllBarCode] " ;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<BarCodeModel> itemDtos = new List<BarCodeModel>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
              
                    itemDtos.Add(new BarCodeModel
                    {
                         item_number = dataSet.Tables[0].Rows[i]["item_number"].ToString(),
                         Barcode =  dataSet.Tables[0].Rows[i]["Barcode"].ToString()
                    });
                

            }

            conn.Close();
            da.Dispose();

            return itemDtos;

        }

        private void updateItemPriority(BarCodeModel barCodeModel)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            using (SqlConnection connection = new SqlConnection(connString))
            using (SqlCommand command = connection.CreateCommand())
            {


                command.CommandText = "UPDATE [dbo].[Items] SET "
                    + "Priority = @Priority "
                    + " Where SKU = @item_number";

                command.Parameters.AddWithValue("@Priority", barCodeModel.priority);
                command.Parameters.AddWithValue("@item_number", barCodeModel.item_number);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        private void updateItemBarCode(BarCodeModel barCodeModel)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            using (SqlConnection connection = new SqlConnection(connString))
            using (SqlCommand command = connection.CreateCommand())
            {


                command.CommandText = "UPDATE [dbo].[Items] SET "
                    + "Barcode = @Barcode "
                    + " Where SKU = @item_number";

                command.Parameters.AddWithValue("@Barcode", barCodeModel.Barcode);
                command.Parameters.AddWithValue("@item_number", barCodeModel.item_number);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }


        private void updateItemName(ItemDto  itemDto)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            using (SqlConnection connection = new SqlConnection(connString))
            using (SqlCommand command = connection.CreateCommand())
            {


                command.CommandText = "UPDATE [dbo].[Items] SET "
                    + "ItemName = @ItemName ,"
                    + "ItemNameEnglish = @ItemNameEnglish "
                    + " Where id = @Id";

                command.Parameters.AddWithValue("@Id", itemDto.Id);
                command.Parameters.AddWithValue("@ItemName", itemDto.ItemName);
                command.Parameters.AddWithValue("@ItemNameEnglish", itemDto.ItemNameEnglish);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        private List<MenuSubCategoryDto> GetSubItemCategories(int? TenantID, int MenuType)
        {
            string connString = AppSettingsModel.ConnectionStrings;
         //   string connString = "Server=tcp:info-seed-db-server-prod.database.windows.net,1433;Initial Catalog=InfoSeedDB-Prod;Persist Security Info=False;User ID=devadmin;Password=P@ssw0rd;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";//AppSettingsModel.ConnectionStrings; //
            string query = "select * from [dbo].[ItemSubCategories] where TenantId = " + TenantID + " and MenuType = " + MenuType;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<MenuSubCategoryDto> itemCategories = new List<MenuSubCategoryDto>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                var IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString());

                if (!IsDeleted)
                {

                    itemCategories.Add(new MenuSubCategoryDto
                    {
                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                        MenuType = Convert.ToInt32(dataSet.Tables[0].Rows[i]["MenuType"]),
                        TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
                        ItemCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemCategoryId"]),
                        Priority = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Priority"]),
                        Name = dataSet.Tables[0].Rows[i]["Name"].ToString(),
                        NameEnglish = dataSet.Tables[0].Rows[i]["NameEnglish"].ToString(),
                        // IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString()),
                        LanguageBotId = int.Parse(dataSet.Tables[0].Rows[i]["LanguageBotId"].ToString()),
                        // bgImag = dataSet.Tables[0].Rows[i]["bgImag"].ToString(),
                        // logoImag = dataSet.Tables[0].Rows[i]["logoImag"].ToString(),
                    });


                }


            }

            conn.Close();
            da.Dispose();

            return itemCategories;

        }

        private List<MenuCategoryDto> GetItemCategories(int? TenantID, int menu, int LanguageBotId)
        {

            string connString = AppSettingsModel.ConnectionStrings;
            //string connString = "Server=tcp:info-seed-db-server-prod.database.windows.net,1433;Initial Catalog=InfoSeedDB-Prod;Persist Security Info=False;User ID=devadmin;Password=P@ssw0rd;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            string query = "select * from [dbo].[ItemCategorys] where TenantID=" + TenantID + " and MenuType = " + menu;


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
                        LanguageBotId = int.Parse(dataSet.Tables[0].Rows[i]["LanguageBotId"].ToString()),


                        bgImag = dataSet.Tables[0].Rows[i]["bgImag"].ToString(),
                        logoImag = dataSet.Tables[0].Rows[i]["logoImag"].ToString(),
                        Priority = int.Parse(dataSet.Tables[0].Rows[i]["Priority"].ToString()),

                    });


                }


            }

            conn.Close();
            da.Dispose();

            return itemCategories;

        }


        private void DeleteSubCategory(int id)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            using (SqlConnection connection = new SqlConnection(connString))
                try
                {

                    using (SqlCommand command = connection.CreateCommand())
                    {

                        command.CommandText = "delete from [dbo].[Items] where [ItemSubCategoryId] = " + id;


                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
                catch (Exception )
                {


                }

        }


        private List<ItemDto> GetItemsPageList(int? TenantID, int menu, int ItemSubCategoryId, int PageSize = 20, int PageNumber = 0, string Search = "")
        {

            if (PageSize == 0)
            {
                PageSize = 20;
            }
            string connString = AppSettingsModel.ConnectionStrings;
               // string connString = "Server=tcp:info-seed-db-server-prod.database.windows.net,1433;Initial Catalog=InfoSeedDB-Prod;Persist Security Info=False;User ID=devadmin;Password=P@ssw0rd;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
                // string query = "select * from [dbo].[Items] where TenantID=" + TenantID + " and MenuType = " + menu+ " and ItemSubCategoryId = "+ ItemSubCategoryId+ " ORDER BY Priority  OFFSET "+ (PageNumber * PageSize) + " ROWS FETCH NEXT "+ PageSize+" ROWS ONLY";

                string query = "";

                if (Search != "" && Search != null)
                {
                    query = "SELECT * from  [dbo].[Items] where ( (Status_Code is null or Status_Code=2)and MenuType = " + menu + " and ( CONTAINS(ItemName,  N'" + Search.Replace(" ", " AND ") + " ')   Or   CONTAINS(ItemNameEnglish,  N'" + Search.Replace(" ", " AND ") + " ') ) )";//"SELECT * from  [dbo].[Items] where ( MenuType = " + menu + "  And   ItemName  LIKE N'" + Search + "%') ";

                }
                else
                {
                    // query = "select * from [dbo].[Items]  ORDER BY CASE WHEN MenuType = " + menu + " AND ItemSubCategoryId = " + ItemSubCategoryId + " and IsInService=1 THEN ItemName END DESC OFFSET " + (PageNumber * PageSize) + " ROWS FETCH NEXT " + PageSize + " ROWS ONLY";



                if (ItemSubCategoryId == 342 || ItemSubCategoryId == 343 || ItemSubCategoryId == 346 || ItemSubCategoryId == 347 || ItemSubCategoryId == 344 || ItemSubCategoryId == 345)
                {
                          query = "select * from [dbo].[Items] where (Status_Code is null or Status_Code=2) and MenuType = " + menu + " and DateFrom is not null and DateTo >=DATEADD(day, DATEDIFF(day,0,GETDATE())-1,0)    ORDER BY Priority  OFFSET " + (PageNumber * PageSize) + " ROWS FETCH NEXT " + PageSize + " ROWS ONLY";
                        // query = "select * from [dbo].[Items] where  MenuType = " + menu + " AND ItemSubCategoryId = " + ItemSubCategoryId + " and DateFrom is not null and ImageUri is not null  and DateTo >=DATEADD(day, DATEDIFF(day,0,GETDATE())-1,0)    ORDER BY Priority  OFFSET " + (PageNumber * PageSize) + " ROWS FETCH NEXT " + PageSize + " ROWS ONLY";
                }
                else
                {
                        query = "select * from [dbo].[Items] where  (Status_Code is null or Status_Code=2) and MenuType = " + menu + " AND ItemSubCategoryId = " + ItemSubCategoryId + "    ORDER BY Priority   OFFSET " + (PageNumber * PageSize) + " ROWS FETCH NEXT " + PageSize + " ROWS ONLY";
                }

                }
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
                    var _itemId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]);



                    var IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString());

                    if (!IsDeleted)
                    {



                        var itm = new ItemDto();
                        try
                        {
                            itm = new ItemDto
                            {
                                ItemSubCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemSubCategoryId"] ?? 0),
                                ItemCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemCategoryId"]),
                                Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                                CategoryNames = dataSet.Tables[0].Rows[i]["CategoryNames"].ToString() ?? "",
                                CategoryNamesEnglish = dataSet.Tables[0].Rows[i]["CategoryNamesEnglish"].ToString() ?? "",

                                ItemDescription = dataSet.Tables[0].Rows[i]["Size"].ToString(),//dataSet.Tables[0].Rows[i]["ItemDescription"].ToString(),
                                ItemDescriptionEnglish = dataSet.Tables[0].Rows[i]["Size"].ToString(),//dataSet.Tables[0].Rows[i]["ItemDescriptionEnglish"].ToString(),

                                ItemName = dataSet.Tables[0].Rows[i]["ItemName"].ToString(),
                                ItemNameEnglish = dataSet.Tables[0].Rows[i]["ItemNameEnglish"].ToString(),
                                IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString()),
                                //	CreationTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["CreationTime"]),
                                //DeletionTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["DeletionTime"]),
                                ImageUri = dataSet.Tables[0].Rows[i]["ImageUri"].ToString(),
                                SKU = dataSet.Tables[0].Rows[i]["SKU"].ToString(),
                                //	Ingredients = dataSet.Tables[0].Rows[i]["Ingredients"].ToString(),
                                IsInService = bool.Parse(dataSet.Tables[0].Rows[i]["IsInService"].ToString()),
                                //ItemDescription = dataSet.Tables[0].Rows[i]["ItemDescription"].ToString(),
                                //ItemName = dataSet.Tables[0].Rows[i]["ItemName"].ToString(),
                                //LastModificationTime = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Priority"]),
                                Price = decimal.Parse(dataSet.Tables[0].Rows[i]["Price"].ToString()),
                                Priority = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Priority"]),
                                MenuId = long.Parse(dataSet.Tables[0].Rows[i]["MenuId"].ToString()),
                                MenuType = Convert.ToInt32(dataSet.Tables[0].Rows[i]["MenuType"]),
                                LanguageBotId = int.Parse(dataSet.Tables[0].Rows[i]["LanguageBotId"].ToString()),
                                Qty = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Qty"] ?? 0),
                                Size = dataSet.Tables[0].Rows[i]["Size"].ToString() ?? "",
                                DateFrom = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["DateFrom"] ?? null),
                                DateTo = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["DateTo"] ?? null),
                                OldPrice = decimal.Parse(dataSet.Tables[0].Rows[i]["OldPrice"].ToString() ?? null),
                                Barcode = dataSet.Tables[0].Rows[i]["Barcode"].ToString() ?? "",
                                IsLoyal = bool.Parse(dataSet.Tables[0].Rows[i]["IsLoyal"].ToString()),
                                LoyaltyPoints=decimal.Parse(dataSet.Tables[0].Rows[i]["LoyaltyPoints"].ToString()),
                                OriginalLoyaltyPoints=decimal.Parse(dataSet.Tables[0].Rows[i]["OriginalLoyaltyPoints"].ToString()),
                                //LoyaltyDefinitionId=long.Parse(dataSet.Tables[0].Rows[i]["LoyaltyDefinitionId"].ToString()),
                                //Status_Code = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Status_Code"] ?? null),
                                //LastModifierDateC = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["LastModifierDateC"] ?? null),



                            };

                        }
                        catch (Exception)
                        {
                            try
                            {
                                itm = new ItemDto
                                {
                                    ItemSubCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemSubCategoryId"] ?? 0),
                                    ItemCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemCategoryId"]),
                                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                                    CategoryNames = dataSet.Tables[0].Rows[i]["CategoryNames"].ToString(),
                                    CategoryNamesEnglish = dataSet.Tables[0].Rows[i]["CategoryNamesEnglish"].ToString(),

                                    ItemDescription = dataSet.Tables[0].Rows[i]["Size"].ToString(),//dataSet.Tables[0].Rows[i]["ItemDescription"].ToString(),
                                    ItemDescriptionEnglish = dataSet.Tables[0].Rows[i]["Size"].ToString(),//dataSet.Tables[0].Rows[i]["ItemDescriptionEnglish"].ToString(),

                                    ItemName = dataSet.Tables[0].Rows[i]["ItemName"].ToString(),
                                    ItemNameEnglish = dataSet.Tables[0].Rows[i]["ItemNameEnglish"].ToString(),
                                    IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString()),
                                    //	CreationTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["CreationTime"]),
                                    //DeletionTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["DeletionTime"]),
                                    ImageUri = dataSet.Tables[0].Rows[i]["ImageUri"].ToString(),
                                    SKU = dataSet.Tables[0].Rows[i]["SKU"].ToString(),
                                    //	Ingredients = dataSet.Tables[0].Rows[i]["Ingredients"].ToString(),
                                    IsInService = bool.Parse(dataSet.Tables[0].Rows[i]["IsInService"].ToString()),
                                    //ItemDescription = dataSet.Tables[0].Rows[i]["ItemDescription"].ToString(),
                                    //ItemName = dataSet.Tables[0].Rows[i]["ItemName"].ToString(),
                                    //LastModificationTime = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Priority"]),
                                    Price = decimal.Parse(dataSet.Tables[0].Rows[i]["Price"].ToString()),
                                    Priority = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Priority"]),
                                    MenuId = long.Parse(dataSet.Tables[0].Rows[i]["MenuId"].ToString()),
                                    MenuType = Convert.ToInt32(dataSet.Tables[0].Rows[i]["MenuType"]),
                                    LanguageBotId = int.Parse(dataSet.Tables[0].Rows[i]["LanguageBotId"].ToString()),
                                    DateFrom = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["DateFrom"] ?? null),
                                    DateTo = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["DateTo"] ?? null),
                                    OldPrice = decimal.Parse(dataSet.Tables[0].Rows[i]["OldPrice"].ToString() ?? null),
                                    Barcode = dataSet.Tables[0].Rows[i]["Barcode"].ToString() ?? "",
                                    IsLoyal = bool.Parse(dataSet.Tables[0].Rows[i]["IsLoyal"].ToString()),
                                    LoyaltyPoints=decimal.Parse(dataSet.Tables[0].Rows[i]["LoyaltyPoints"].ToString()),
                                    OriginalLoyaltyPoints=decimal.Parse(dataSet.Tables[0].Rows[i]["OriginalLoyaltyPoints"].ToString()),
                                   // LoyaltyDefinitionId=long.Parse(dataSet.Tables[0].Rows[i]["LoyaltyDefinitionId"].ToString()),
                                    //Status_Code = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Status_Code"] ?? null),
                                    //LastModifierDateC = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["LastModifierDateC"] ?? null),
                                    //  Qty = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Qty"] ?? 0),
                                    // Size = dataSet.Tables[0].Rows[i]["Size"].ToString() ?? ""

                                };

                            }
                            catch
                            {

                                itm = new ItemDto
                                {
                                    ItemSubCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemSubCategoryId"] ?? 0),
                                    ItemCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemCategoryId"]),
                                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                                    CategoryNames = dataSet.Tables[0].Rows[i]["CategoryNames"].ToString(),
                                    CategoryNamesEnglish = dataSet.Tables[0].Rows[i]["CategoryNamesEnglish"].ToString(),

                                    ItemDescription = dataSet.Tables[0].Rows[i]["Size"].ToString(),//dataSet.Tables[0].Rows[i]["ItemDescription"].ToString(),
                                    ItemDescriptionEnglish = dataSet.Tables[0].Rows[i]["Size"].ToString(),//dataSet.Tables[0].Rows[i]["ItemDescriptionEnglish"].ToString(),
                                    ImageUri = dataSet.Tables[0].Rows[i]["ImageUri"].ToString(),
                                    ItemName = dataSet.Tables[0].Rows[i]["ItemName"].ToString(),
                                    ItemNameEnglish = dataSet.Tables[0].Rows[i]["ItemNameEnglish"].ToString(),
                                    IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString()),
                                    //	CreationTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["CreationTime"]),
                                    //DeletionTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["DeletionTime"]),
                                    // ImageUri = dataSet.Tables[0].Rows[i]["ImageUri"].ToString(),
                                    SKU = dataSet.Tables[0].Rows[i]["SKU"].ToString(),
                                    //	Ingredients = dataSet.Tables[0].Rows[i]["Ingredients"].ToString(),
                                    IsInService = bool.Parse(dataSet.Tables[0].Rows[i]["IsInService"].ToString()),
                                    //ItemDescription = dataSet.Tables[0].Rows[i]["ItemDescription"].ToString(),
                                    //ItemName = dataSet.Tables[0].Rows[i]["ItemName"].ToString(),
                                    //LastModificationTime = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Priority"]),
                                    Price = decimal.Parse(dataSet.Tables[0].Rows[i]["Price"].ToString()),
                                    Priority = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Priority"]),
                                    MenuId = long.Parse(dataSet.Tables[0].Rows[i]["MenuId"].ToString()),
                                    MenuType = Convert.ToInt32(dataSet.Tables[0].Rows[i]["MenuType"]),
                                    LanguageBotId = int.Parse(dataSet.Tables[0].Rows[i]["LanguageBotId"].ToString()),
                                    Barcode = dataSet.Tables[0].Rows[i]["Barcode"].ToString() ?? "",
                                    IsLoyal = bool.Parse(dataSet.Tables[0].Rows[i]["IsLoyal"].ToString()),
                                    LoyaltyPoints=decimal.Parse(dataSet.Tables[0].Rows[i]["LoyaltyPoints"].ToString()),
                                    OriginalLoyaltyPoints=decimal.Parse(dataSet.Tables[0].Rows[i]["OriginalLoyaltyPoints"].ToString()),
                                    //LoyaltyDefinitionId=long.Parse(dataSet.Tables[0].Rows[i]["LoyaltyDefinitionId"].ToString()),
                                    //Status_Code = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Status_Code"] ?? null),
                                    //LastModifierDateC = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["LastModifierDateC"] ?? null),
                                    //  Qty = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Qty"] ?? 0),
                                    // Size = dataSet.Tables[0].Rows[i]["Size"].ToString() ?? ""

                                };

                            }


                        }

                    itm.LoyaltyDefinitionId=_loyaltyAppService.GetItemLoyaltyLog(itm.Id, itm.TenantId).LoyaltyDefinitionId;
                    if (!itm.IsLoyal)
                    {

                        itm.LoyaltyPoints =_loyaltyAppService.ConvertPriceToPoints(itm.Price, itm.LoyaltyPoints, itm.OriginalLoyaltyPoints, 0);

                    }
                    else
                    {


                        itm.LoyaltyPoints =_loyaltyAppService.ConvertPriceToPoints(itm.Price, itm.LoyaltyPoints, itm.OriginalLoyaltyPoints, itm.LoyaltyDefinitionId);
                    }



                    itm.additionsCategorysListModels = new List<AdditionsCategorysListModel>();

                        List<ItemAdditionDto> itemAdditionDtos22 = new List<ItemAdditionDto>();
                        itm.itemAdditionDtos = itemAdditionDtos22.ToArray();

                        if (itm.OldPrice != 0 && itm.OldPrice != null)
                        {
                            if (itm.OldPrice != itm.Price)//&& itm.OldPrice > itm.Price
                        {
                            
                                //var datNow = DateTime.Now.AddHours(AppSettingsModel.AddHour);

                                if (itm.DateTo != null)
                                {


                                    var x1 = (itm.Price / itm.OldPrice);
                                    var x2 = 100 - (x1 * 100);
                                    var xFormat = String.Format("{0:0.##}", x2);
                                    itm.Discount = xFormat.ToString() + "%";
                                    itm.DiscountImg = "https://infoseedmediastorageprod.blob.core.windows.net/4408da24-aad7-4802-a1f1-46f5ad9e2074/sale.png";

                                }

                            }
                            else
                            {
                                itm.OldPrice = 0;
                            }


                        }
                        itm.ViewPrice = -1;
                        if (itm.ItemDescription.Contains("1 KG") && !itm.SKU.StartsWith('T'))
                        {
                            var itemAupdate = AddExtra(itm, TenantID.Value);

                            itm.ItemSpecifications = itemAupdate;
                            itm.ViewPrice = 0;
                        }


                        if (itm.DateTo != null)
                        {
                            if (itm.DiscountImg != null)
                            {
                                itemDtos.Add(itm);

                            }
                            else
                            {

                            }

                        }
                        else
                        {
                            itemDtos.Add(itm);
                        }

                  

                }



            }




                conn.Close();
                return itemDtos;

                

        }
        private List<ItemSpecification> AddExtra(ItemDto itm,int TenantID)
        {
            List<SpecificationChoice> Listchoices = new List<SpecificationChoice>();
            SpecificationChoice choices1 = new SpecificationChoice
            {
                TenantId = TenantID,
                Id = 1192,
                IsInService = true,
                LanguageBotId = 1,
                Price = (itm.Price * decimal.Parse("0.25")),
                SpecificationChoiceDescription = "0.25 kg",
                SpecificationChoiceDescriptionEnglish = "0.25 kg",
                SpecificationId = 292,
                SKU = ""

            };
            SpecificationChoice choices2 = new SpecificationChoice
            {
                TenantId = TenantID,
                Id = 1193,
                IsInService = true,
                LanguageBotId = 1,
                Price = (itm.Price * decimal.Parse("0.50")),
                SpecificationChoiceDescription = "0.50 kg",
                SpecificationChoiceDescriptionEnglish = "0.50 kg",
                SpecificationId = 292,
                SKU = ""

            };
            SpecificationChoice choices3 = new SpecificationChoice
            {
                TenantId = TenantID,
                Id = 1194,
                IsInService = true,
                LanguageBotId = 1,
                Price = (itm.Price * decimal.Parse("0.75")),
                SpecificationChoiceDescription = "0.75 kg",
                SpecificationChoiceDescriptionEnglish = "0.75 kg",
                SpecificationId = 292,
                SKU = ""

            };
            SpecificationChoice choices4 = new SpecificationChoice
            {
                TenantId = TenantID,
                Id = 1195,
                IsInService = true,
                LanguageBotId = 1,
                Price = (itm.Price * decimal.Parse("1.0")),
                SpecificationChoiceDescription = "1.00 kg",
                SpecificationChoiceDescriptionEnglish = "1.00 kg",
                SpecificationId = 292,
                SKU = ""

            };

            Listchoices.Add(choices1);
            Listchoices.Add(choices2);
            Listchoices.Add(choices3);
            Listchoices.Add(choices4);

            List<ItemSpecification> ItemSpecificationsList = new List<ItemSpecification>();

            ItemSpecification itemSpecification = new ItemSpecification
            {
                Id = 292,
                SpecificationDescription = "الرجاء اختيار الوزن ",
                IsMultipleSelection = false,
                IsRequired = true,
                SpecificationChoices = Listchoices,
                MaxSelectNumber = 0,
                // ItemSpecificationId = Convert.ToInt32(dataSetItemSpecification.Tables[1].Rows[j]["ItemSpecificationId"]),
                Priority = 0,
                SpecificationDescriptionEnglish = "Please select a weight",
                TenantId = TenantID,


            };


            ItemSpecificationsList.Add(itemSpecification);



            return ItemSpecificationsList;
        }
        private int GetItemCount(int subID)
        {
            string connString = AppSettingsModel.ConnectionStrings;
           // string connString = "Server=tcp:info-seed-db-server-prod.database.windows.net,1433;Initial Catalog=InfoSeedDB-Prod;Persist Security Info=False;User ID=devadmin;Password=P@ssw0rd;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";//AppSettingsModel.ConnectionStrings; //

            string query = "";
            if (subID == 343 || subID == 346 || subID == 347)
            {
                query = "SELECT COUNT(Id)  from [dbo].[Items] where ItemSubCategoryId = " + subID + " and DateFrom is not null";

            }
            else
            {
                query = "SELECT COUNT(Id)  from [dbo].[Items] where ItemSubCategoryId = " + subID;

            }





            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<MenuSubCategoryDto> itemCategories = new List<MenuSubCategoryDto>();
            var x = 0;
            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {



                x = int.Parse(dataSet.Tables[0].Rows[i]["Column1"].ToString());







            }

            conn.Close();
            da.Dispose();

            return x;

        }

        private void Deleteitem(long id)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            using (SqlConnection connection = new SqlConnection(connString))
                try
                {

                    using (SqlCommand command = connection.CreateCommand())
                    {

                        command.CommandText = "delete from [dbo].[Items] where id = " + id;


                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
                catch (Exception )
                {


                }

        }




        private List<ItemDto> GeAllTenant()
        {
            string connString = AppSettingsModel.ConnectionStrings;
            string query = "";// "select * from [dbo].[Items] where  MenuType= " + MenuType;


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

                    itemDtos.Add(new ItemDto
                    {
                        TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
                        MenuType = Convert.ToInt32(dataSet.Tables[0].Rows[i]["MenuType"]),
                        ItemCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemCategoryId"]),
                        ItemSubCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemSubCategoryId"]),
                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                        // CategoryNames = dataSet.Tables[0].Rows[i]["CategoryNames"].ToString(),
                        // CategoryNamesEnglish = dataSet.Tables[0].Rows[i]["CategoryNamesEnglish"].ToString(),
                        IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString()),
                        ImageUri = dataSet.Tables[0].Rows[i]["ImageUri"].ToString() ?? "",
                        IsInService = bool.Parse(dataSet.Tables[0].Rows[i]["IsInService"].ToString()),
                        ItemDescription = dataSet.Tables[0].Rows[i]["ItemDescription"].ToString(),
                        ItemDescriptionEnglish = dataSet.Tables[0].Rows[i]["ItemDescriptionEnglish"].ToString(),
                        ItemName = dataSet.Tables[0].Rows[i]["ItemName"].ToString(),
                        ItemNameEnglish = dataSet.Tables[0].Rows[i]["ItemNameEnglish"].ToString(),
                        Price = decimal.Parse(dataSet.Tables[0].Rows[i]["Price"].ToString()),
                        Priority = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Priority"]),
                        MenuId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["MenuId"]),
                        Barcode = dataSet.Tables[0].Rows[i]["Barcode"].ToString(),
                        SKU = dataSet.Tables[0].Rows[i]["SKU"].ToString() ?? "",
                        Size = dataSet.Tables[0].Rows[i]["Size"].ToString() ?? "",
                        Qty = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Qty"] ?? 0),

                    });


                }
                

            }

            conn.Close();
            da.Dispose();

            return itemDtos;

        }

    }
}
