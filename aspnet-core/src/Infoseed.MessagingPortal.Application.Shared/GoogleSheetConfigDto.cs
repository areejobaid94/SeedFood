

namespace Infoseed.MessagingPortal
{
    public class GoogleSheetConfigDto
    {
        public int Id { get; set; }
        public int? TenantId { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public bool? IsConnected { get; set; }
        public string GoogleEmail { get; set; }

    }
}
