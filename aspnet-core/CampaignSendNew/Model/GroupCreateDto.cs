using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampaignSendNew.Model
{
    public class GroupCreateDto
    {
        public long id { get; set; }
        public string groupName { get; set; }
        public List<MembersDto> failedAdd { get; set; }
        public int failedCount { get; set; } = 0;
        public int successCount { get; set; }
        public List<phoneNumbers> NUmberList { get; set; }
        public string message { get; set; }
        public int state { get; set; }
    }
    public class phoneNumbers
    {
        public string NUmber { get; set; }
    }
}
