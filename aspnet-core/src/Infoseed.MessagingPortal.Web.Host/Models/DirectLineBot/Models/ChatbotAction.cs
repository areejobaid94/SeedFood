using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Web.Models.DirectLineBot.Models
{
    public class ChatbotAction
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("type")]
        public string type { get; set; }
        [JsonProperty("data")]
        public string data { get; set; }
        [JsonProperty("title")]
        public string title { get; set; }
    }
}
