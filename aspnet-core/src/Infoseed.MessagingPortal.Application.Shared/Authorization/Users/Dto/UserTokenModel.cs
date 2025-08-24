using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Authorization.Users.Dto
{
    public class UserTokenModel
    {
        public long Id { get; set; }
        public long UserId { get; set; } = 0;
        public string DeviceId { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public int TenantId { get; set; } = 0;
    }
}
