using System;
using System.Collections.Generic;
using System.Text;

namespace InfoSeedParser.ConfigrationFile
{
    public class ItmeConfigrationExcelFile
    {
        public string item_Id_ColumnID = "A";// { get; set; }
        public string item_Name_ColumnID = "B";// { get; set; }
        public string item_Price_ColumnID = "C";// { get; set; }

        public bool SkipFirstRowInExcel = true;// { get; set; }
    }
}
