namespace BotService.Models.API
{
    public class UserTokenModel
    {
        public long Id { get; set; }

        public long UserId { get; set; }

        public string DeviceId { get; set; }
        public string Token { get; set; }
        public int TenantId { get; set; }
    }
}
