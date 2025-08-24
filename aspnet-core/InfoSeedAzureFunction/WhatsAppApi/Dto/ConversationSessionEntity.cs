using System;
using System.Collections.Generic;
using System.Text;

namespace InfoSeedAzureFunction.WhatsAppApi.Dto
{
    public class ConversationSessionEntity
    {
        public List<ConversationSessionModel> conversationSessionModel { get; set; }
        public int TotalCount { get; set; }

    }
}
