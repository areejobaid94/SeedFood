namespace Infoseed.MessagingPortal.OrderDetails.Dtos
{
    public class GetOrderDetailForViewDto
    {
		public OrderDetailDto OrderDetail { get; set; }

        public string OrderStringToPrint { get; set; }

        public string OrderOrderRemarks { get; set;}

		public string ItemName { get; set;}
        public string ItemNameEnglish { get; set; }

        public string SKU { get; set; }

        public string BarCode { get; set; }
        public string BarcodeImg { get; set; }
        public bool IsBarcodeImg { get; set; }
        public string  ItemImageUrl { get; set; }

    }
}