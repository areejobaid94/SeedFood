using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Web.Models.Sunshine
{
    public class TemplateMessagesModel
    {
        public string TenantId { get; set; }
        public string Id { get; set; }
        public string TemplateMessageName { get; set; }

        public string TemplateMessagePurposeId { get; set; }

        public string MessageText { get; set; }
    }
}
