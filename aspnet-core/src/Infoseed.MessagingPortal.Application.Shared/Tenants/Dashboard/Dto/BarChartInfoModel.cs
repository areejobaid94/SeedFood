using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Tenants.Dashboard.Dto
{
   public  class BarChartInfoModel
    {
        public List<string> labels { get; set; }
        public List<int> data { get; set; }
        public List<string> backgroundColor { get; set; }
    }
}
