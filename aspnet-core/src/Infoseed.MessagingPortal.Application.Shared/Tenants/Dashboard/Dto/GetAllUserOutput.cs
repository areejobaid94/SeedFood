using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Tenants.Dashboard.Dto
{
    public class GetAllUserOutput
    {
        public int? FirstTenantId { get; set; }
        public string FirstUserId { get; set; }

        public List<UserModel> UserModel { get; set; }
    }
}
