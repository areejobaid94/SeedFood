using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal
{
    public class InsertSheetRowDto
    {
        public string spreadsheetId { get; set; }
        public string sheetName { get; set; }
        public int tenantId { get; set; }
        public Dictionary<string, string> row { get; set; }
    }
}
