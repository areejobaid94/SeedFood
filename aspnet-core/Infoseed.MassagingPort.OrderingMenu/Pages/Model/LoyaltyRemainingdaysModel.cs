namespace Infoseed.MassagingPort.OrderingMenu.Pages.Model
{
    public class LoyaltyRemainingdaysModel
    {       
        public decimal Points { get; set; }
        public DateTime CreatedDate { get; set; }
        public int LoyaltyExpiration { get; set; }
        public int OrderType { get; set; }
        public long OrderNumber { get; set; }
        public decimal Total { get; set; }
        public int Remainingdays { get; set; }
      
    }
}
