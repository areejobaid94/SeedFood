
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InfoSeedAzureFunction
{
    public class ConversationChatModel
    {
        public int TenantId { get; set; }
        public string MicrosoftBotId { get; set; }

        public string SunshineConversationId { get; set; }
        public string D360Key { get; set; }
        public string appID { get; set; }
        public string userId { get; set; }
        public string BotId { get; set; }
        public string watermark { get; set; }
        public DateTime LastUpdate { get; set; }
        public ContainerItemTypes ItemType { get; set; }


        public string id { get; set; }
        public string _rid { get; set; }
        public string _self { get; set; }
        public string _etag { get; set; }
        public string _attachments { get; set; }
        public long _ts { get; set; }

    }
}
