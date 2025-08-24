using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Tenants.Dashboard.Dto
{
    public class UserPerformanceBookingGenarecModel
    {
            public List<UserPerformanceBookingModel> userPerformanceBookingModel { get; set; }
            public long TotalBooking { get; set; }
        
        public class UserPerformanceBookingModel
        {
            public long AgentId { get; set; }
            public string UserName { get; set; }
            public string EmailAddress { get; set; }
            public decimal TotalBooked { get; set; }
            public decimal TotalConfirmed { get; set; }
            public decimal TotalCancelled { get; set; }
            public decimal TotalDeleted { get; set; }
            public decimal TotalPending { get; set; }

        }
    }
}
