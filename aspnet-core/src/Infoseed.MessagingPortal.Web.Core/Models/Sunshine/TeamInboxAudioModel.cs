using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Web.Models.Sunshine
{
    public class TeamInboxAudioModel
    {
        public string Type { get; set; }
        public string UserID { get; set; }     
        public string agentName { get; set; }
        public string agentId { get; set; }
        public IFormFile FormFile { get; set; }
    }
}
