using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.General.Dto
{
    public class BotTemplatesModel
    {
        public long Id { get; set; }
        public Guid GuidId { get; set; }
        public string TemplateName { get; set; }
        public int TemplateNumber { get; set; }
        public bool IsDeleted { get; set; }

    }
}
