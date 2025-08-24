using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Teams.Dto
{
    public class TeamsCreateDto
    {
        public long id { get; set; }
        public string groupName { get; set; }
        public List<TeamsDto> failedAdd { get; set; }
        public int failedCount { get; set; }
        public int successCount { get; set; }
        public string message { get; set; }
        public int state { get; set; }
    }
}
