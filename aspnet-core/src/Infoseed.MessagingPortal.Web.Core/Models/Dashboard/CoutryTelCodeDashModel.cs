namespace Infoseed.MessagingPortal.Web.Models.Dashboard
{
    public class CoutryTelCodeDashModel
    {
        public string Pfx { get; set; }
        public string Iso { get; set; }
        public decimal Rate { get; set; }

        public CoutryTelCodeDashModel(string pfx, string iso, decimal rate)
        {
            Pfx = pfx;
            Iso = iso;
            Rate = rate;
        }
    }
}
