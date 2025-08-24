using System;
using System.Collections.Generic;
using System.Text;

namespace CampaignSyncFunction.AppFunEntities
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

        public int? TotalFreeConversationWA { get; set; }
        public int? TotalUsageFreeConversationWA { get; set; }
        public int? TotalUsageFreeUIWA { get; set; }
        public int? TotalUsageFreeBIWA { get; set; }
        public int? TotalUsageFreeConversation { get; set; }
        public int? TotalUIConversation { get; set; }
        public int? TotalUsageUIConversation { get; set; }
        public int? TotalBIConversation { get; set; }
        public int? TotalUsageBIConversation { get; set; }
        public int? RemainingConversationWA { get; set; }



        public int FromFacebook { get; set; }
        public int OutFacebook { get; set; }
        public int UserInitiated { get; set; }
        public int BusinessInitiated { get; set; }
        public int FreeBundle { get; set; }
        public int PaidBundle { get; set; }
    }
}
