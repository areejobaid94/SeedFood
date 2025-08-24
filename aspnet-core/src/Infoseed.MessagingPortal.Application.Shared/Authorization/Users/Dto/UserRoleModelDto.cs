using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Authorization.Users.Dto
{
    public class UserRoleModelDto
    {
        public long Id { get; set; }
        public int RoleId { get; set; }
        public long UserId { get; set; }
    }
}
