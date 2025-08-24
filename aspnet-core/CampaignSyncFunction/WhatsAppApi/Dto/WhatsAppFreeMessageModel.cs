using System;
using System.Collections.Generic;
using System.Text;

namespace CampaignSyncFunction.WhatsAppApi.Dto
{
    public class WhatsAppFreeMessageModel
    {
        public long? Id { get; set; }
        public string FreeMessage { get; set; }
        public string FreeMessageType { get; set; }
        public int? TenantId { get; set; }
        public long UserId { get; set; }
        public long CampaignId { get; set; }
        public bool IsActive { get; set; }
        public bool IsLatest { get; set; }
        public bool IsRecurrence { get; set; }
        public DateTime? SendDateTime { get; set; }

    }
}
