using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampaignSendNew.Model
{
    public class NewContactDto
    {
        public string DisplayName { get; set; }
        public string PhoneNumber { get; set; }    
        public string UserId { get; set; }
        public int? TenantId { get; set; }
        public DateTime? CreationTime { get; set; } = DateTime.MinValue;
        public string Group { get; set; }
        public string GroupName { get; set; }


    }
}
