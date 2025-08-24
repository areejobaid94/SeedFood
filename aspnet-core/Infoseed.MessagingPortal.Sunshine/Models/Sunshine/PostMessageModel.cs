using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Sunshine.Models.Sunshine
{
    public class PostMessageModel
    {

        public string UserId { get; set; }
        public string Text { get; set; }
        public string AgentName { get; set; }
        public string AgentId { get; set; }
    }
}
