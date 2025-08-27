namespace Infoseed.MessagingPortal.Web.Models
{
    public class MerchantOrder
    {
        public string CustomerPhone { get; set; } //must contain country code 
        public string CustomerName { get; set; }
        public string CustomerLocation { get; set; }
        public string OrderNumber { get; set; }
        public string OrderPrice { get; set; }
        public string Payment { get; set; }
        public string PrepTime { get; set; }
        public string DeliveryAmount { get; set; }
        public string TotalAmount { get; set; }
        public bool IsCreate { get; set; }
        public bool IsMerchant { get; set; }
    }
}
