using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Sunshine.Models
{
    public class ConversationChatModel
    {
        public string MicrosoftBotId { get; set; }
        public string SunshineConversationId { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
