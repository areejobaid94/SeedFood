using Infoseed.MessagingPortal.InfoSeedParser;
using Infoseed.MessagingPortal.Items.Dtos;
using InfoSeedParser.ConfigrationFile;
using InfoSeedParser.Helpers;
using InfoSeedParser.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace InfoSeedParser.Parsers
{
    public  class MenuExcelParser: IMenuParser
    {

         ParseMenuModel IMenuParser.Parse(ParseConfig parserOptions)
        {
            MenuExcelDictionaryModel qnAExcelDictionaryModel = new MenuExcelDictionaryModel();
            var FileName = Path.GetFileNameWithoutExtension(parserOptions.FileName);
            var Data = ExcelReader.ParseExcelDictionaryByte(parserOptions.FileData);
            qnAExcelDictionaryModel.Data = Data;
            var resalt = ParseTheList(qnAExcelDictionaryModel, parserOptions.config as ConfigrationExcelFile, FileName);
            return resalt;
        }
        private ParseMenuModel ParseTheList(MenuExcelDictionaryModel menu, ConfigrationExcelFile confg, string FileName)
        {

            List<ParseMenuModel> qnADTOs = new List<ParseMenuModel>();

            try
            {
                var Skip = menu.Data.Skip(0);
                if (confg.SkipFirstRowInExcel)
                {
                    Skip = menu.Data.Skip(1);
                }

                foreach (var kvp in Skip)
                {
                    qnADTOs.Add(GetTheItem(kvp, confg, FileName));
                }

                List<ItemDto> noRepet = FixList(qnADTOs);

                ParseMenuModel parseMenuModel = new ParseMenuModel
                {
                    Item = noRepet

                };

                return parseMenuModel;
            }
            catch (Exception )
            {


            }
           

         

            return null;
        }

        private List<ItemDto> FixList(List<ParseMenuModel> qnADTOs)
        {
            List<ItemDto> Item = new List<ItemDto>();

            try
            {
                foreach (var item in qnADTOs)
                {
                    Item.Add(item.Item.FirstOrDefault());

                }
            }
            catch (Exception)
            {


            }
         

           



            return Item;


        }

        ParseMenuModel GetTheItem(KeyValuePair<int, Dictionary<string, string>> kvp, ConfigrationExcelFile confg, string FileName)
        {
            try
            {
                string item_numberColumnID, NameColumnID, SizeColumnID, BarcodeColumnID, PriceColumnID, translationColumnID, priorityColumnID, dateFromColumnID, dateToColumnID, oldPriceColumnID, status_CodeColumnID, qtyColumnID;
                GetValueFunchation(kvp, confg, out item_numberColumnID, out NameColumnID, out SizeColumnID, out BarcodeColumnID, out PriceColumnID, out translationColumnID, out priorityColumnID  , out dateFromColumnID, out dateToColumnID, out oldPriceColumnID, out status_CodeColumnID,out qtyColumnID);


                List<ItemDto> Item = new List<ItemDto>();


                Item.Add(new ItemDto
                {
                    //Id = int.Parse(ItemIDColumnID),
                    Priority = !string.IsNullOrEmpty(priorityColumnID)?int.Parse(priorityColumnID):0,
                    //MenuType = int.Parse(ItemMenuTypeColumnID),
                    // CategoryNames  = ItemCategoryNamesColumnID,
                    // CategoryNamesEnglish = ItemCategoryNamesColumnID,
                    CreationTime = DateTime.Now,
                    DeletionTime = DateTime.Now,
                    //   ImageUri = ItemImageUriColumnID,
                    IsInService = true,//Convert.ToBoolean(int.Parse(ItemIsInServiceColumnID)),
                                       //   ItemCategoryId = int.Parse(ItemCategoryIdColumnID),
                    ItemName = translationColumnID,
                    ItemNameEnglish = NameColumnID,
                    //ItemDescription = ItemNameColumnID,
                    // ItemDescriptionEnglish = ItemNameColumnID,
                    // MenuId = int.Parse(ItemMenuIdColumnID),
                    Price = !string.IsNullOrEmpty(PriceColumnID) ? decimal.Parse(PriceColumnID) : 0  ,
                    SKU = item_numberColumnID,
                    Size = SizeColumnID,
                    Barcode = BarcodeColumnID,
                    Ingredients = "",
                    LanguageBotId = 1,

                    LastModificationTime = DateTime.Now,

                    DateFrom= !string.IsNullOrEmpty(dateFromColumnID) ? DateTime.Parse(dateFromColumnID) : (DateTime?)null,
                    DateTo = !string.IsNullOrEmpty(dateToColumnID) ? DateTime.Parse(dateToColumnID) : (DateTime?)null ,
                     OldPrice = !string.IsNullOrEmpty(PriceColumnID) ? decimal.Parse(PriceColumnID) : (decimal?)null,
                      Status_Code = !string.IsNullOrEmpty(status_CodeColumnID) ? int.Parse(status_CodeColumnID) : 2,
                      Qty = !string.IsNullOrEmpty(qtyColumnID) ? int.Parse(qtyColumnID) : 20,

                });

                ParseMenuModel qnAModel = new ParseMenuModel()
                {

                    Menu = null,
                    Category = null,
                    Item = Item,
                    itemAdditionDtos = null
                };


                return qnAModel;
            }
            catch (Exception )
            {

                return null;
            }

          

        }

        private static void GetValueFunchation(KeyValuePair<int, Dictionary<string, string>> kvp, ConfigrationExcelFile confg, out string item_numberColumnID, out string NameColumnID, out string SizeColumnID, out string BarcodeColumnID, out string PriceColumnID, out string translationColumnID, out string priorityColumnID   , out string dateFromColumnID, out string dateToColumnID, out string oldPriceColumnID, out string status_CodeColumnID, out string qtyColumnID)
        {
            try
            {
                item_numberColumnID = kvp.Value.ToList().Where(x => x.Key.ToLower().Contains(confg.item_numberColumnID.ToLower())).FirstOrDefault().Value;
                NameColumnID = kvp.Value.ToList().Where(x => x.Key.ToLower().Contains(confg.NameColumnID.ToLower())).FirstOrDefault().Value;
                SizeColumnID = kvp.Value.ToList().Where(x => x.Key.ToLower().Contains(confg.SizeColumnID.ToLower())).FirstOrDefault().Value;
                BarcodeColumnID = kvp.Value.ToList().Where(x => x.Key.ToLower().Contains(confg.BarcodeColumnID.ToLower())).FirstOrDefault().Value;
                PriceColumnID = kvp.Value.ToList().Where(x => x.Key.ToLower().Contains(confg.PriceColumnID.ToLower())).FirstOrDefault().Value;
                translationColumnID = kvp.Value.ToList().Where(x => x.Key.ToLower().Contains(confg.translationColumnID.ToLower())).FirstOrDefault().Value;
                priorityColumnID = kvp.Value.ToList().Where(x => x.Key.ToLower().Contains(confg.priorityColumnID.ToLower())).FirstOrDefault().Value;


                dateFromColumnID = kvp.Value.ToList().Where(x => x.Key.ToLower().Contains(confg.dateFromColumnID.ToLower())).FirstOrDefault().Value;
                dateToColumnID = kvp.Value.ToList().Where(x => x.Key.ToLower().Contains(confg.dateToColumnID.ToLower())).FirstOrDefault().Value;
                oldPriceColumnID = kvp.Value.ToList().Where(x => x.Key.ToLower().Contains(confg.oldPriceColumnID.ToLower())).FirstOrDefault().Value;
                status_CodeColumnID = kvp.Value.ToList().Where(x => x.Key.ToLower().Contains(confg.status_CodeColumnID.ToLower())).FirstOrDefault().Value;
                qtyColumnID = kvp.Value.ToList().Where(x => x.Key.ToLower().Contains(confg.qtyColumnID.ToLower())).FirstOrDefault().Value;
                
                if (string.IsNullOrEmpty(item_numberColumnID))
                    item_numberColumnID = "";
                if (string.IsNullOrEmpty(NameColumnID))
                    NameColumnID = "";
                if (string.IsNullOrEmpty(SizeColumnID))
                    SizeColumnID = "";
                if (string.IsNullOrEmpty(BarcodeColumnID))
                    BarcodeColumnID = "";
                if (string.IsNullOrEmpty(PriceColumnID))
                    PriceColumnID = "";
                if (string.IsNullOrEmpty(translationColumnID))
                    translationColumnID = "";

                if (string.IsNullOrEmpty(priorityColumnID))
                    priorityColumnID = "";

                if (string.IsNullOrEmpty(dateFromColumnID))
                    dateFromColumnID = "";
                if (string.IsNullOrEmpty(dateToColumnID))
                    dateToColumnID = "";
                if (string.IsNullOrEmpty(oldPriceColumnID))
                    oldPriceColumnID = "";
                if (string.IsNullOrEmpty(status_CodeColumnID))
                    status_CodeColumnID = "";
                if (string.IsNullOrEmpty(qtyColumnID))
                    qtyColumnID = "";
                
            }
            catch(Exception )
            {


            }


            item_numberColumnID = kvp.Value.ToList().Where(x => x.Key.ToLower().Contains(confg.item_numberColumnID.ToLower())).FirstOrDefault().Value;
            NameColumnID = kvp.Value.ToList().Where(x => x.Key.ToLower().Contains(confg.NameColumnID.ToLower())).FirstOrDefault().Value;
            SizeColumnID = kvp.Value.ToList().Where(x => x.Key.ToLower().Contains(confg.SizeColumnID.ToLower())).FirstOrDefault().Value;
            BarcodeColumnID = kvp.Value.ToList().Where(x => x.Key.ToLower().Contains(confg.BarcodeColumnID.ToLower())).FirstOrDefault().Value;
            PriceColumnID = kvp.Value.ToList().Where(x => x.Key.ToLower().Contains(confg.PriceColumnID.ToLower())).FirstOrDefault().Value;
            translationColumnID = kvp.Value.ToList().Where(x => x.Key.ToLower().Contains(confg.translationColumnID.ToLower())).FirstOrDefault().Value;
            priorityColumnID = kvp.Value.ToList().Where(x => x.Key.ToLower().Contains(confg.priorityColumnID.ToLower())).FirstOrDefault().Value;


            dateFromColumnID = kvp.Value.ToList().Where(x => x.Key.ToLower().Contains(confg.dateFromColumnID.ToLower())).FirstOrDefault().Value;
            dateToColumnID = kvp.Value.ToList().Where(x => x.Key.ToLower().Contains(confg.dateToColumnID.ToLower())).FirstOrDefault().Value;
            oldPriceColumnID = kvp.Value.ToList().Where(x => x.Key.ToLower().Contains(confg.oldPriceColumnID.ToLower())).FirstOrDefault().Value;
            status_CodeColumnID = kvp.Value.ToList().Where(x => x.Key.ToLower().Contains(confg.status_CodeColumnID.ToLower())).FirstOrDefault().Value;
            qtyColumnID = kvp.Value.ToList().Where(x => x.Key.ToLower().Contains(confg.qtyColumnID.ToLower())).FirstOrDefault().Value;

            if (string.IsNullOrEmpty(item_numberColumnID))
                item_numberColumnID = "";
            if (string.IsNullOrEmpty(NameColumnID))
                NameColumnID = "";
            if (string.IsNullOrEmpty(SizeColumnID))
                SizeColumnID = "";
            if (string.IsNullOrEmpty(BarcodeColumnID))
                BarcodeColumnID = "";
            if (string.IsNullOrEmpty(PriceColumnID))
                PriceColumnID = "";
            if (string.IsNullOrEmpty(translationColumnID))
                translationColumnID = "";

            if (string.IsNullOrEmpty(priorityColumnID))
                priorityColumnID = "";

            if (string.IsNullOrEmpty(dateFromColumnID))
                dateFromColumnID = "";
            if (string.IsNullOrEmpty(dateToColumnID))
                dateToColumnID = "";
            if (string.IsNullOrEmpty(oldPriceColumnID))
                oldPriceColumnID = "";
            if (string.IsNullOrEmpty(status_CodeColumnID))
                status_CodeColumnID = "";
            if (string.IsNullOrEmpty(qtyColumnID))
                qtyColumnID = "";


        }



    }
}
