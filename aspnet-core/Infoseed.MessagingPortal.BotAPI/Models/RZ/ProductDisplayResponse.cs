namespace Infoseed.MessagingPortal.BotAPI.Models.RZ
{
    public class ProductDisplayResponse
    {
        public string itemCode { get; set; }
        public string itemDescription { get; set; }
        public string imagePath { get; set; }

        public int unitCode { get; set; }
        public string unitDescription { get; set; }
        public decimal itemBalance { get; set; }
        public decimal priceAmount { get; set; }
    }

}
