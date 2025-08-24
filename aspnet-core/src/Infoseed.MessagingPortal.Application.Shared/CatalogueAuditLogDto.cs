using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal
{
    public class CatalogueAuditLogDto
    {
        public int TenantId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
    }
}
