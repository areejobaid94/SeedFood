namespace Infoseed.MessagingPortal.BotAPI.Models.Utrac
{
    public class SubmitUtracOrderModel
    {
        public int TenantID { get; set; }
        public int ContactId { get; set; }

        public string PhoneNumber { get; set; }
        public string BuildingNumber { get; set; }
        public string ApartmentNumber { get; set; }
    }
}
