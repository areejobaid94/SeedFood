using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Tenants.Dashboard.Dto
{
    public class ConversationMeasurementsModel
    {
        public int Id { get; set; }
        public int BusinessInitiatedCount { get; set; }

        public int UserInitiatedCount { get; set; }
        public int ReferralConversionCount { get; set; }

        public int TotalFreeConversation { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int TenantId { get; set; }
    }
}
