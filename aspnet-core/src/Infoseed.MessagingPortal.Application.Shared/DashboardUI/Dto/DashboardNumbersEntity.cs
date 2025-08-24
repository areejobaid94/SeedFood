using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.DashboardUI.Dto
{
    public class DashboardNumbersEntity
    {
        public List<DashboardNumbers> DashboardNumbers { get; set; }
        public OrderDashboard Order { get; set; }
        public LiveChatDashboard LiveChat { get; set; }
        public RequestDashboard Request { get; set; }
        public ContactDashboard Contact { get; set; }
        public CampaignDashboard Campaign { get; set; }
    }
}
