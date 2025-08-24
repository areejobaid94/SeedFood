using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.TargetReach.Dto
{
    public class TargetReachEntity
    {
          public long Id { get; set; }
            public string CustomerName { get; set; }
            public string PhoneNumber { get; set; }
            public int TenantId { get; set; }
            public long CreatedBy { get; set; }
            public DateTime CreatedOn { get; set; }
            public int ContactId { get; set; }
            public long TemplateId { get; set; }
            public int LanguageId { get; set; }
            public string Message { get; set; }
            public string TemplateName { get; set; }
            public Guid GuidId { get; set; }
            public string UserId { get; set; }
        
    }
}
