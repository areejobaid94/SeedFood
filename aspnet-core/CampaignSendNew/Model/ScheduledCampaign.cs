using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampaignSendNew.Model
{
    public class ScheduledCampaign
    {
        public long Id { get; set; }
        public long campaignId { get; set; }
        public string campaignName { get; set; }
        public string templateName { get; set; }
        public long templateId { get; set; }
        public bool IsExternal { get; set; }
        public bool IsSent { get; set; }
        public DateTime SendTime { get; set; }
        public DateTime CreatedDate { get; set; }
        public int TenantId { get; set; }
        public long UserId { get; set; }
        public string JopName { get; set; }
        public List<ListContactToCampin> contacts { get; set; }
    }
}