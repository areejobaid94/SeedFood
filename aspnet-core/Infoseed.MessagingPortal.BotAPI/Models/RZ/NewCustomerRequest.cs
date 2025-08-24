using DocumentFormat.OpenXml.Presentation;

namespace Infoseed.MessagingPortal.BotAPI.Models.RZ
{
    public class NewCustomerRequest
    {
        public string Lang { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public CustomerData[] Data { get; set; }
    }

    public class CustomerData
    {
        public string ArabicName { get; set; }
        public string EnglishName { get; set; }
        public string Mobile { get; set; }
        public string AddressDesc { get; set; }
    }

    public class NewCustomerResponse
    {
        public string message { get; set; }
        public int Status { get; set; }
    }
}
