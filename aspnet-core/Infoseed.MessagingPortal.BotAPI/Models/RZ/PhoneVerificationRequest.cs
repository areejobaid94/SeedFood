namespace Infoseed.MessagingPortal.BotAPI.Models.RZ
{
    public class PhoneVerificationRequest
    {
        public string lang { get; set; }  
        public string mobile { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
    }
    public class ExistingCustomerResponse
    {
        public string clientName { get; set; }
        public string clientNumber { get; set; }
    }
    public class NonExistingCustomerResponse
    {
        public string mobileNo { get; set; }
    }

}
