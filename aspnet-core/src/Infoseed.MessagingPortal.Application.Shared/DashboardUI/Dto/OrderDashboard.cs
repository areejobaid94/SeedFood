using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.DashboardUI.Dto
{
    public class OrderDashboard
    {
        public int TotalOrder { get; set; }
        public int TotalOrderPending { get; set; }
        public int TotalOrderDone { get; set; }
        public int TotalOrderDelete { get; set; }
        public int TotalOrderCancel { get; set; }
        public int TotalOrderPreOrder { get; set; }
    }
}
