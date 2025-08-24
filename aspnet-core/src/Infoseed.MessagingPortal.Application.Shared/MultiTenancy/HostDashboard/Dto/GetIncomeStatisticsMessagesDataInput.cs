using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.MultiTenancy.HostDashboard.Dto
{
    public  class GetIncomeStatisticsMessagesDataInput : DashboardInputBase
    {
        public DateMessages IncomeStatisticsDateHostInterval { get; set; }
       
    }
}
