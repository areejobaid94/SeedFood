using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Tenants.Dashboard.Dto
{
    public class TenantModelDash
    {
        public int TenantId { get; set; }
        public string AccessToken { get; set; }
        public string Name { get; set; }
        public string TenancyName { get; set; }
        public string WhatsAppAccountID { get; set; }
        public string PhoneNumber { get; set; }

    }
}
