using System;
using System.Collections.Generic;
using System.Text;

namespace InfoSeedParser.ConfigrationFile
{
    public class ConfigrationExcelFile
    {

        public string item_numberColumnID = "A";// { get; set; }
        public string NameColumnID = "B";// { get; set; }
        public string SizeColumnID = "C";// { get; set; }
        public string BarcodeColumnID = "D";// { get; set; }
        public string PriceColumnID = "E";// { get; set; }
        public string translationColumnID = "F";// { get; set; }
        public string priorityColumnID = "G";// { get; set; }


        public string dateFromColumnID = "H";// { get; set; }
        public string dateToColumnID = "I";// { get; set; }
        public string oldPriceColumnID = "J";// { get; set; }
        public string status_CodeColumnID = "K";// { get; set; }

        public string qtyColumnID = "L";// { get; set; }

        //public string MenuIDColumnID = "A";// { get; set; }
        //public string MenuNameColumnID = "B";// { get; set; }
        //public string MenuDescriptionColumnID = "C";// { get; set; }
        //public string RestaurantsTypeColumnID = "D";// { get; set; }
        //public string MenuTypeColumnID = "E";// { get; set; }
        //public string MenuPriorityColumnID = "F";// { get; set; }


        //public string CategoryIDColumnID = "G";// { get; set; }
        //public string CategoryNameColumnID = "H";// { get; set; }
        //public string CategoryMenuTypeColumnID = "I";// { get; set; }
        //public string CategoryPriorityColumnID = "J";// { get; set; }


        //public string ItemCategoryIdColumnID = "K";// { get; set; }
        //public string ItemCategoryNamesColumnID = "L";// { get; set; }
        //public string ItemMenuIdColumnID = "M";// { get; set; }
        //public string ItemIDColumnID = "N";// { get; set; }
        //public string ItemSkuColumnID = "O";// { get; set; }
        //public string ItemNameColumnID = "P";// { get; set; }
        //public string ItemDescriptionColumnID = "Q";// { get; set; }
        //public string ItemIsInServiceColumnID = "R";// { get; set; }
        //public string ItemPriorityColumnID = "S";// { get; set; }
        //public string ItemMenuTypeColumnID = "T";// { get; set; }
        //public string ItemImageUriColumnID = "U";// { get; set; }
        //public string ItemPriceColumnID = "V";// { get; set; }


        //public string AdditionItemIdColumnID = "W";// { get; set; }
        //public string AdditionIDColumnID = "X";// { get; set; }
        //public string AdditionSkuColumnID = "Y";// { get; set; }
        //public string AdditionMenuTypeColumnID = "Z";// { get; set; }
        //public string AdditionNameColumnID = "AA";// { get; set; }
        //public string AdditionPriceColumnID = "AB";// { get; set; }



        public bool SkipFirstRowInExcel = true;// { get; set; }



        ///// <summary> 
        /////  to know the column of  Questions  in Excel file
        ///// </summary>
        //public string QuestionColumnID { get; set; }
        ///// <summary>
        /////  to know the column of  Answer  in Excel file
        ///// </summary>
        //public string AnswerColumnID { get; set; }
        ///// <summary>
        /////  to know the column of  Prompts  in Excel file
        ///// </summary>
        //public string ContextColumnID { get; set; }
        ///// <summary>
        /////  to know the column of  Prompts  in Excel file
        ///// </summary>
        //public bool SkipFirstRowInExcel { get; set; }
        ///// <summary>
        /////  to know the column of  ID  in Excel file
        ///// </summary>
        //public Dictionary<string, string> MetadataColumnID { get; set; }


    }
}
