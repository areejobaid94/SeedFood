using System;
using System.Collections.Generic;

namespace WebHookStatusFun
{
    public class WhatsAppFunModel
    {
        public WhatsAppContactsDto whatsAppContactsDto { get; set; }
        public UTracOrderModel uTracOrderModel { get; set; }
        public List<TargetReachModel> targetReachModels{ get; set; }
         public BookingModel BookingModel { get; set; }
        public long templateId { get; set; }
        public long campaignId { get; set; }
        public Guid GuidId { get; set; }

        public int TenantId { get; set; }
        public string UserId { get; set; }
        public bool IsContact { get; set; }

        public int DailyLimit { get; set; }

        public string msg { get; set; }

        public string type { get; set; }
        public string Parameters { get; set; }

    }
}
