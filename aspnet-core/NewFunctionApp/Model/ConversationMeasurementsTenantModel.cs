using System;
using System.Collections.Generic;
using System.Text;

namespace NewFunctionApp
{
    public class ConversationMeasurementsTenantModel
    {
        public int Id { get; set; }
        public int? TenantId { get; set; }

        public int Year { get; set; }

        public int Month { get; set; }

        public int BusinessInitiatedCount { get; set; }
        public int UserInitiatedCount { get; set; }
        public int ReferralConversionCount { get; set; }
        public int TotalFreeConversation { get; set; }

        public DateTime LastUpdatedDate { get; set; }
        public DateTime CreationDate { get; set; }



        public int TotalFreeConversationWA { get; set; }
        public int TotalUsageFreeConversationWA { get; set; }
        public int TotalUsageFreeUIWA { get; set; }
        public int TotalUsageFreeBIWA { get; set; }

        public int TotalUsagePaidConversationWA { get; set; }
        public int TotalUsagePaidUIWA { get; set; }
        public int TotalUsagePaidBIWA { get; set; }

        public int TotalUsageFreeConversation { get; set; }
        public int TotalUIConversation { get; set; }
        public int TotalUsageUIConversation { get; set; }
        public int TotalBIConversation { get; set; }
        public int TotalUsageBIConversation { get; set; }

    }
}
