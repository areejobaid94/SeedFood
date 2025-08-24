using System;
using System.Collections.Generic;
using System.Text;

namespace InfoSeedAzureFunction.Model
{
    public class WhatsAppEnums
    {
        public enum WhatsAppConversationTypeEnum
        {
            UNKNOWN,
            REGULAR,
            FREE_TIER,
            FREE_ENTRY_POINT
        }
        public enum WhatsAppConversationCategoryEnum
        {
            UNKNOWN,
            MARKETING,
            UTILITY,
            AUTHENTICATION,
            SERVICE
        }
    }
}
