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
    public class ItemExcelParser : IItemParser
    {
        public ParseMenuModel Parse(ParseConfig parserOptions)
        {
            MenuExcelDictionaryModel qnAExcelDictionaryModel = new MenuExcelDictionaryModel();

            var FileName = Path.GetFileNameWithoutExtension(parserOptions.FileName);
            var Data = ExcelReader.ParseExcelDictionaryByte(parserOptions.FileData);
            qnAExcelDictionaryModel.Data = Data;
            var resalt = ParseTheList(qnAExcelDictionaryModel, parserOptions.config2 as ItmeConfigrationExcelFile, FileName);
            return resalt;
        }

        private ParseMenuModel ParseTheList(MenuExcelDictionaryModel menu, ItmeConfigrationExcelFile confg, string FileName)
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
            catch (Exception)
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

        ParseMenuModel GetTheItem(KeyValuePair<int, Dictionary<string, string>> kvp, ItmeConfigrationExcelFile confg, string FileName)
        {
            try
            {
                string item_Id_ColumnID, item_Name_ColumnID, item_Price_ColumnID;
                GetValueFunchation(kvp, confg, out item_Id_ColumnID, out item_Name_ColumnID, out item_Price_ColumnID);


                List<ItemDto> Item = new List<ItemDto>();


                Item.Add(new ItemDto
                {
                    Id = int.Parse(item_Id_ColumnID),           
                    ItemName = item_Name_ColumnID,                 
                    Price = !string.IsNullOrEmpty(item_Price_ColumnID) ? decimal.Parse(item_Price_ColumnID) : 0         

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

        private static void GetValueFunchation(KeyValuePair<int, Dictionary<string, string>> kvp, ItmeConfigrationExcelFile confg, out string item_Id_ColumnID, out string item_Name_ColumnID, out string item_Price_ColumnID)
        {
            try
            {
                item_Id_ColumnID = kvp.Value.ToList().Where(x => x.Key.ToLower().Contains(confg.item_Id_ColumnID.ToLower())).FirstOrDefault().Value;
                item_Name_ColumnID = kvp.Value.ToList().Where(x => x.Key.ToLower().Contains(confg.item_Name_ColumnID.ToLower())).FirstOrDefault().Value;
                item_Price_ColumnID = kvp.Value.ToList().Where(x => x.Key.ToLower().Contains(confg.item_Price_ColumnID.ToLower())).FirstOrDefault().Value;
              


                if (string.IsNullOrEmpty(item_Id_ColumnID))
                    item_Id_ColumnID = "";
                if (string.IsNullOrEmpty(item_Name_ColumnID))
                    item_Name_ColumnID = "";
                if (string.IsNullOrEmpty(item_Price_ColumnID))
                    item_Price_ColumnID = "";
            

            }
            catch (Exception )
            {


            }


            item_Id_ColumnID = kvp.Value.ToList().Where(x => x.Key.ToLower().Contains(confg.item_Id_ColumnID.ToLower())).FirstOrDefault().Value;
            item_Name_ColumnID = kvp.Value.ToList().Where(x => x.Key.ToLower().Contains(confg.item_Name_ColumnID.ToLower())).FirstOrDefault().Value;
            item_Price_ColumnID = kvp.Value.ToList().Where(x => x.Key.ToLower().Contains(confg.item_Price_ColumnID.ToLower())).FirstOrDefault().Value;

            if (string.IsNullOrEmpty(item_Id_ColumnID))
                item_Id_ColumnID = "";
            if (string.IsNullOrEmpty(item_Name_ColumnID))
                item_Name_ColumnID = "";
            if (string.IsNullOrEmpty(item_Price_ColumnID))
                item_Price_ColumnID = "";

        }

    }
}
