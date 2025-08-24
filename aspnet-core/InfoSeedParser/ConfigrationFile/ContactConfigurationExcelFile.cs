using System;
using System.Collections.Generic;
using System.Text;

namespace InfoSeedParser.ConfigrationFile
{
    public class ContactConfigurationExcelFile
    {
        public string contact_PhoneNumber_ColumnID = "A";
        public string contact_Name_ColumnID = "B";
        public string contact_Var1_ColumnID = "C";
        public string contact_Var2_ColumnID = "D";
        public string contact_Var3_ColumnID = "E";
        public string contact_Var4_ColumnID = "F";
        public string contact_Var5_ColumnID = "G";
        public bool SkipFirstRowInExcel = true;
    }
}
