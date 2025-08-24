namespace Infoseed.MessagingPortal.Engine.Model
{
    public class InvoiceResponse
    {
        public int status { get; set; }
        public bool success { get; set; }
        public InvoiceData data { get; set; }
    }
    public class InvoiceData
    {
        public string url { get; set; }
    }
}
