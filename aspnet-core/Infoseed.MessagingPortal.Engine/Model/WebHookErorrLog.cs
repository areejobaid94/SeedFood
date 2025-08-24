namespace Infoseed.MessagingPortal.Engine.Model
{
    public class WebHookErorrLog
    {
        public long Id { get; set; }

        public int TenantId { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public string Details { get; set; }

        public string JsonString { get; set; }

    }
}
