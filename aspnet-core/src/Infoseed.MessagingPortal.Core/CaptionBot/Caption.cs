using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Infoseed.MessagingPortal.CaptionBot
{
    [Table("Caption")]
    public class Caption : FullAuditedEntity<long>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public int Id { get; set; }
        public string Text { get; set; }
        public int LanguageBotId { get; set; }
        public LanguageBot  LanguageBot { get; set; }
        public int TextResourceId { get; set; }
        public TextResource TextResource { get; set; }
        public string HeaderText { get; set; }
    }
}
