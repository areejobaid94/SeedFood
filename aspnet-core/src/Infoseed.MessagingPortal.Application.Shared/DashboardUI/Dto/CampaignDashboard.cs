using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.DashboardUI.Dto
{
    public class CampaignDashboard
    {
        public int TotalCampaign { get; set; }
        public int TotalCampaignSent { get; set; }
        public int TotalCampaignDelivered { get; set; }
        public int TotalCampaignRead { get; set; }
    }
}
