using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Maintenance.Dtos
{
    public class CreateMaintenanceModel
    {
        public int ContactId { get; set; }
        public int TenantID { get; set; }

        public string BotLocal { get; set; }



        public bool isOrderOfferCost { get; set; }
        public string Address { get; set; }
        public string AddressEn { get; set; }
        public decimal DeliveryCostAfter { get; set; }
        public decimal DeliveryCostBefor { get; set; }


        public bool IsPreOrder { get; set; }
        public string SelectDay { get; set; }
        public string SelectTime { get; set; }

        public string LocationFrom { get; set; }


        public string CustomerName { get; set; }
        public string PhoneNumber1 { get; set; }
        public string PhoneNumber2 { get; set; }
        public string PhoneType { get; set; }
        public string Damage { get; set; }
    }
}
