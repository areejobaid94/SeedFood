namespace CampaignSyncFunction.WhatsAppApi.Dto
{
    public class CaptionDto 
    {
        public int? TenantId { get; set; }
        public int Id { get; set; }
        public string Text { get; set; }
        public int LanguageBotId { get; set; }
        public int TextResourceId { get; set; }
        public string HeaderText { get; set; }

    }
}
