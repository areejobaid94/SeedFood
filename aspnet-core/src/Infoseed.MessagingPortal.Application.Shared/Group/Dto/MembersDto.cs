
using System.Collections.Generic;

namespace Infoseed.MessagingPortal.Group.Dto
{
    public class MembersDto
    {
        public int id { get; set; }
        public string phoneNumber { get; set; }
        public string displayName { get; set; }
        public int failedId { get; set; }
        public bool isFailed { get; set; }
        public Dictionary<string, string> variables { get; set; }
        public int customeropt { get; set; }
    }
}
