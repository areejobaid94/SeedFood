namespace Infoseed.MessagingPortal.BotAPI.Models.Utrac
{
    public class CancelUtracOrderModel
    {
        public int TenantID { get; set; }
        public int ContactId { get; set; }

        public int OrderId { get; set; }

        public string PhoneNumber { get; set; }
    }
}
