using System;

namespace Infoseed.MessagingPortal.Authorization.Users.Dto
{
    public class UserLoginAttemptDto
    {
        public string TenancyName { get; set; }

        public string UserNameOrEmail { get; set; }

        public string ClientIpAddress { get; set; }

        public string ClientName { get; set; }

        public string BrowserInfo { get; set; }

        public string Result { get; set; }

        public DateTime CreationTime { get; set; }

        public string formattedDate { get; set; }
        public string formattedTime { get; set; }
    }
}
