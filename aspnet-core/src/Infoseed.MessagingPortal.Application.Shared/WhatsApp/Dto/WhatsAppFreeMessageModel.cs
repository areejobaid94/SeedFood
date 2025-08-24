using System;
using System.Collections.Generic;
using System.Text;
using static Infoseed.MessagingPortal.WhatsApp.Dto.WhatsAppEnum;

namespace Infoseed.MessagingPortal.WhatsApp.Dto
{
    public class WhatsAppFreeMessageModel
    {
        public long? Id { get; set; }
        public string FreeMessage { get; set; }
        public string FreeMessageType { get; set; }

        public WhatsAppCampaignStatusEnum WhatsAppCampaignStatus { get; set; }
        public int CampaingStatusId
        {
            get { return (int)this.WhatsAppCampaignStatus; }
            set { this.WhatsAppCampaignStatus = (WhatsAppCampaignStatusEnum)value; }
        }

        public int? TenantId { get; set; }
        public string UserId { get; set; }
        public long Sent { get; set; }

        public long Delivered { get; set; }

        public long Read { get; set; }
        public long Failed { get; set; }
        public DateTime? SentTime { get; set; } = DateTime.UtcNow;
        public bool? IsActive { get; set; }
    }
}
