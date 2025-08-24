using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Tenants.Dashboard.Dto
{
    public class UserPerformanceOrderGenarecModel
    {
        public List<UserPerformanceOrderModel> userPerformanceOrderModel {  get; set; }
        public long totalOrders { get; set; }

    }
    public class UserPerformanceOrderModel
    {
        public long Id {  get; set; }
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public decimal CountDone { get; set; }
        public decimal CountDelete { get; set; }
        public int Avg_ActionTime_Minutes { get; set; }
        public string Avg_ActionTime { get; set; }
        //public UsersDashModel usersDashModel { get; set; }
        //public OrderDashModel urderDashModel { get; set; }
    }
}
