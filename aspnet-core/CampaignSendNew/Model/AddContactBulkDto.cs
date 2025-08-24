using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampaignSendNew.Model
{
    public class AddContactBulkDto
    {
        public string UserId { get; set; }
        public string DisplayName { get; set; }
        public string PhoneNumber { get; set; }
        public int? TenantId { get; set; }
        public string Group { get; set; }
        public string GroupName { get; set; }
        public Dictionary<string, string> variables { get; set; }
        public int customeropt { get; set; } = 0;
    }
}
