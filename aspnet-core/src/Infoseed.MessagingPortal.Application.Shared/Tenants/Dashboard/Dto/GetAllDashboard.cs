using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Tenants.Dashboard.Dto
{
    public class GetAllDashboard
    {
        public string ErrorMsg { get; set; }
        public int TotalOfAllContact { get; set; }
        public int TotalOfSendMessages { get; set; }
        public int TotalOfClose { get; set; }
        public int TotalOfOrders { get; set; }
        public double TotalOfRating { get; set; }
        public int Bandel { get; set; }
        public int RemainingConversation { get; set; }

        public decimal TotalFreeConversationWA { get; set; }
        public int TotalUsageFreeConversationWA { get; set; }
        public int TotalUsageFreeUIWA { get; set; }
        public int TotalUsageFreeBIWA { get; set; }

        public int TotalUsagePaidConversationWA { get; set; }
        public int TotalUsagePaidUIWA { get; set; }
        public int TotalUsagePaidBIWA { get; set; }
        public int TotalUsageFreeEntry { get; set; }

        public decimal TotalUsageFreeConversation { get; set; }
        public decimal TotalUIConversation { get; set; }
        public decimal TotalUsageUIConversation { get; set; }
        public decimal TotalBIConversation { get; set; }
        public decimal TotalUsageBIConversation { get; set; }

        public decimal RemainingFreeConversation { get; set; }
        public decimal RemainingBIConversation { get; set; }
        public decimal RemainingUIConversation { get; set; }
        public decimal TotalRemainingBIConversation { get; set; }


        public int FromFacebook { get; set; }
        public int OutFacebook { get; set; }
        public int UserInitiated { get; set; }
        public int BusinessInitiated { get; set; }
        public int FreeBundle { get; set; }
        public int PaidBundle { get; set; }

    }
}
