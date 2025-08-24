using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Web.Models.WhatsAppDialog
{
    public class D360PostAttachmentModel
    {
        public string Type { get; set; }
        public string To { get; set; }
        public string Text { get; set; }
        public string agentName { get; set; }
        public string agentId { get; set; }

        public int selectedLiveChatID { get; set; } = 0;
        public List<IFormFile> FormFile { get; set; }
    }
}
