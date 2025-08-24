using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Web.Models.WhatsAppDialog
{
    public class PostMessageD360Model
    {
        public string To { get; set; }
        public string Text { get; set; }
        public string Type { get; set; }
        public string AgentName { get; set; }
        public string AgentId { get; set; }
        public int selectedLiveChatID { get; set; } = 0;
    }
}
