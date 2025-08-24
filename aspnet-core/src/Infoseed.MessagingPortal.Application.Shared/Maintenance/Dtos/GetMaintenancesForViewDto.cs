using Infoseed.MessagingPortal.Orders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Maintenance.Dtos
{
   public  class GetMaintenancesForViewDto
    {
        public int Id { get; set; }
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

        public DateTime CreationTime { get; set; }
        public decimal DeliveryCost { get; set; }
        public string OrderLocal { get; set; }
        public int OrderStatus { get; set; }

        public virtual string StringTotal { get; set; }

        public string CreationTimeString { get; set; }
        public string CreationDateString { get; set; }

        public bool isLockByAgent { get; set; }
        public string LockByAgentName { get; set; }
        public string orderStatusName { get; set; }
        public int AgentId { get; set; }
        public string OrderRemarks { get; set; }


        public string UserId { get; set; }
        public string DisplayName { get; set; }
        public string phoneNumber { get; set; }

    }
}
