using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Web.Models.DirectLineBot.Models
{
    public class AttachmentContent
    {
        [JsonProperty("type")]
        public string type { get; set; }
        [JsonProperty("actions")]
        public ChatbotAction[] actions { get; set; }
    }
}
