using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Loyalty.Dto
{
    public class LoyaltyModel
    {
        public long Id { get; set; }

        public bool IsLoyalityPoint { get; set; }
        public decimal CustomerPoints { get; set; }
        public decimal CustomerCurrencyValue { get; set; }

        public decimal ItemsPoints { get; set; }
        public decimal ItemsCurrencyValue { get; set; }
        public bool IsOverrideUpdatedPrice { get; set; }
        public bool IsLatest { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public long CreatedBy { get; set; }
        public int TenantId { get; set; }


        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; } = DateTime.Now;


        public string OrderType { get; set; }

        public int LoyaltyExpiration { get; set; }
    }
}
