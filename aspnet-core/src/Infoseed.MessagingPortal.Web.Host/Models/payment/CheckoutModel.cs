using Infoseed.MessagingPortal.Web.Host.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Web.Models.payment
{
    public class CheckoutModel
    {

        public string CheckoutJsUrl { get; set; }

        public string MerchantId { get; set; }
        public string SessionId { get; set; }
        public string SessionVersion { get; set; }
        public CheckoutSessionModel SuccessIndicator { get; set; }
        public string OrderId { get; set; }
       
        public string Currency { get; set; }
    }
}
