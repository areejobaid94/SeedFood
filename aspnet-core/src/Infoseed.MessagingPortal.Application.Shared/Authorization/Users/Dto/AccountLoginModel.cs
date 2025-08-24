using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Authorization.Users.Dto
{
    public class AccountLoginModel
    {
        public bool IsLogOut { get; set; }  
 
        public long UserId { get; set; }

        public int TenantId { get; set; }


    }
}
