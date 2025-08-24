using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Infoseed.MessagingPortal.CaptionBot
{
    [Table("TextResource")]
    public class TextResource : FullAuditedEntity<long>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public int Id { get; set; }
        public string Key { get; set; }
        public string Category { get; set; }
    }
}
