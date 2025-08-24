using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.OrderOffer.Dtos
{
   public class OrderOfferDate 
    {
        public OrderOfferDate(DateTime start, DateTime end)
        {
            OrderOfferDateStart = start;
            OrderOfferDateEnd = end;
        }




        public dynamic OrderOfferDateStart { get; set; }
        public dynamic OrderOfferDateEnd { get; set; }

    }
}
