using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Web.Models.BotModel
{
    public class GetOrderAndDetailModel
    {
        public int TenantID { get; set; }
        public int ContactId { get; set; }
        public string lang { get; set; }

        public string MenuType { get; set; }

        public string captionQuantityText { get; set; }
        public string captionAddtionText { get; set; }
        public string captionTotalText { get; set; }
        public string captionTotalOfAllText { get; set; }
    }
}
