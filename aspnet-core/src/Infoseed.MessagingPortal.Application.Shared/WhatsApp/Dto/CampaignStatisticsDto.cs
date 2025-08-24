namespace Infoseed.MessagingPortal.WhatsApp.Dto
{
    public class CampaignStatisticsDto
    {
        public string title { get; set; }
        public long TotalProcessing { get; set; }
        public long TotalDelivered { get; set; }
        public long TotalRead { get; set; }
        public long TotalNumbers { get; set; }

        public long TotalSent { get; set; }
        public long TotalReplied { get; set; }
        public long TotalFailed { get; set; }
        public double SentPercentage { get; set; }
        public double DeliveredPercentage { get; set; }
        public double ReadPercentage { get; set; }
        public double RepliedPercentage { get; set; }
        public double ProcessingPercentage { get; set; }
        public double FailedPercentage { get; set; }
    }
}
