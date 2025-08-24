using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Tenants.Dashboard.Dto
{
    public class UserPerformanceTicketGenarecModel
    {
        public List<UserPerformanceTicketModel> userPerformanceTicketModel { get; set; }
        public long totalTickets { get; set; }
    }
    public class UserPerformanceTicketModel
    {
        public long AgentId { get; set; }
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public decimal TotalOpen { get; set; }
        public decimal TotalClose { get; set; }

        public decimal TotalPending { get; set; }
        public long AvgTimeMinutes { get; set; }
        public string Avg_ActionTime { get; set; }  

    }
}
