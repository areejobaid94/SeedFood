using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.WhatsApp.Dto
{
    public class BotReservedWordsEntity
    {
        public List<BotReservedWordsModel> lstBotReservedWordsModel { get; set; }
        public int TotalCount { get; set; }
    }
}
