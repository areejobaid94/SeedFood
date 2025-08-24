using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.WhatsApp.Dto
{
    public class BotReservedWordsModel
    {
        public long Id { get; set; }
        public string ButtonText { get; set; }
        public string Action { get; set; }
        public int TenantId { get; set; }
        public string TriggersBot { get; set; }


        public long ActionId { get; set; }
        public string ActionAr { get; set; }
        public string ActionEn { get; set; }
    }
}
