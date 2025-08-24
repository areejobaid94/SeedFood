using System.Globalization;

namespace Infoseed.MassagingPort.OrderingMenu.Pages.Model
{


    public class OrderingMenuLoyaltyModel
    {
        public long Id { get; set; }

        public bool IsLoyalityPoint { get; set; }
        public decimal CustomerPoints { get; set; }
        public decimal CustomerCurrencyValue { get; set; }

        public decimal ItemsPoints { get; set; }
        public decimal ItemsCurrencyValue { get; set; }
        public bool IsOverrideUpdatedPrice { get; set; }
        public bool IsLatest { get; set; }

        public DateTime CreatedDate { get; set; }
        public long CreatedBy { get; set; }
        public int TenantId { get; set; }
    }
}
