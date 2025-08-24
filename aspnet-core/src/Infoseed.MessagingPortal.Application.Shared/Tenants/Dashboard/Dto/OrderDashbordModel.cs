using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Tenants.Dashboard.Dto
{
    public class OrderDashbordModel
    {
        public List<OrderDashModel> orderDashModel { get; set; }
        public long TotalOrdrs { get; set; }

    }
}
