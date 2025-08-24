using System.Collections.Generic;

namespace Infoseed.MessagingPortal.BotAPI.Models.Dto
{
    public class InsertSheetRowDto
    {
        public string spreadsheetId { get; set; }
        public string sheetName { get; set; }
        public int tenantId { get; set; }
        public Dictionary<string, string> row {  get; set; }
    }
}
