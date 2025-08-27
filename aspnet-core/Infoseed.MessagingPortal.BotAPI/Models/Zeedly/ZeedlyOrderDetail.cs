using System.Collections.Generic;

namespace Infoseed.MessagingPortal.BotAPI.Models.Zeedly
{
    public class ZeedlyOrderDetail
    {
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }
        public int ItemId { get; set; }
        public string ProductId { get; set; }
        public string Name { get; set; }
        public string Retailer_Id { get; set; }
        public string Description { get; set; }
        public string Availability { get; set; }
        public string Image_Url { get; set; }
        public string Url { get; set; }
    }
}
