namespace Infoseed.MessagingPortal.ReceiptDetails.Dtos
{
    public class GetReceiptDetailForViewDto
    {
        public ReceiptDetailDto ReceiptDetail { get; set; }

        public string ReceiptReceiptNumber { get; set; }

        public string AccountBillingBillID { get; set; }

    }
}