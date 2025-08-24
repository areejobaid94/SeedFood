namespace Infoseed.MessagingPortal.Contacts.Dtos
{
    public class ContactsCampaignFilter
    {
        public int pageNumber { get; set; } = 0;
        public int pageSize { get; set; } = 50;
        public string phone { get; set; } = null;
        public long? templateId { get; set; } = null;
        public long? campaignId { get; set; } = null;
        public int statusId { get; set; } = 0; // isSent >> 1 , isDelivered >>2 ,isRead >>3 , isFailed>> 4 ,isHanged >> 5
    }
}
