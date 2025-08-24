using System;
using System.Collections.Generic;
using System.Text;

namespace InfoSeedAzureFunction.AppFunEntities
{
    public class ConversationMeasurements
    {
        public string ErrorMsg { get; set; }
        public int TotalOfAllContact { get; set; }
        public int TotalOfSendMessages { get; set; }
        public int TotalOfClose { get; set; }
        public int TotalOfOrders { get; set; }
        public double TotalOfRating { get; set; }
        public int Bandel { get; set; }
        public int RemainingConversation { get; set; }

        public int TotalFreeConversationWA { get; set; }
        public int TotalUsageFreeConversationWA { get; set; }
        public decimal TotalUsageFreeUIWA { get; set; }
        public decimal TotalUsageFreeBIWA { get; set; }
        public decimal TotalUsageFreeConversation { get; set; }
        public decimal TotalUIConversation { get; set; }
        public decimal TotalUsageUIConversation { get; set; }
        public decimal TotalBIConversation { get; set; }
        public decimal TotalUsageBIConversation { get; set; }
        public decimal TotalMarketingBIConversation { get; set; }
        public decimal TotalUsageMarketingBIConversation { get; set; }
        public decimal TotalUtilityBIConversation { get; set; }
        public decimal TotalUsageUtilityBIConversation { get; set; }
        public int RemainingConversationWA { get; set; }


        public int TenantId { get; set; }
        public int FromFacebook { get; set; }
        public int OutFacebook { get; set; }
        public int UserInitiated { get; set; }
        public int BusinessInitiated { get; set; }
        public int FreeBundle { get; set; }
        public int PaidBundle { get; set; }
    }
}
