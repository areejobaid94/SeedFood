namespace CampaignProject.Models
{
    public class CampaignManager
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string PhoneNumber { get; set; }
        public string MassageId { get; set; }
        public string Status { get; set; }
        public int StatusCode { get; set; }
        public string FaildDetails { get; set; }
        public string DetailsJosn { get; set; }

        public int CampaignId { get; set; }

    }
}
