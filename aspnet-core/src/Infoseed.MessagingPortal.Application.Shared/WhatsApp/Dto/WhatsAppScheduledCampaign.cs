using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.WhatsApp.Dto
{
    public class WhatsAppScheduledCampaign
    {
        public long Id { get; set; }
        public long CampaignId { get; set; }
        public long TemplateId { get; set; }
        public DateTime SendDateTime { get; set; }
        public string SendTime { get; set; }
        public int StatusId { get; set; }
        public bool IsActive { get; set; }
        public bool IsRecurrence { get; set; }
        public bool IsLatest { get; set; }
        public bool IsFreeMessage { get; set; }
        public int TenantId { get; set; }
        public DateTime CreatedDate { get; set; }
        public long CreatedByUserId { get; set; }
        public string ContactsJson { get; set; }
    }
}
