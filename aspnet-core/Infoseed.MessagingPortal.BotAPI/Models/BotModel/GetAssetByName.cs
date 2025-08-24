namespace Infoseed.MessagingPortal.BotAPI.Models.BotModel
{
    public class GetAssetByName
    {
        public int? TenantID { get; set; }
        public string? Brand { get; set; }
        public string? Brand2 { get; set; }
        public string? Local { get; set; }
        public bool? isOffer { get; set; }
        public string? PhoneNumber { get; set; }

        
    }
}
