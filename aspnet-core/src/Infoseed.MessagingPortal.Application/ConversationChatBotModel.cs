using Framework.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal
{
    public class ConversationChatBotModel
    {
        public string MicrosoftBotId { get; set; }

        public string SunshineConversationId { get; set; }
        public string appID { get; set; }
        public string userId { get; set; }
        public string BotId { get; set; }

        public DateTime LastUpdate { get; set; }
        public InfoSeedContainerItemTypes ItemType { get; set; }

    }
}
