namespace Infoseed.MessagingPortal.Contacts.Dtos
{
    public class ContactCampaignDto
    {
        public long Id { get; set; }
        public string PhoneNumber { get; set; }
        public string TemplateName { get; set; }
        public string CampaignName { get; set; }
        public bool? IsSent { get; set; }
        public bool? IsDelivered { get; set; }
        public bool? IsRead { get; set; }
        public bool? IsFailed { get; set; }
        public bool? IsReplied { get; set; }
        public bool? IsHanged { get; set; }

    }
    
}
