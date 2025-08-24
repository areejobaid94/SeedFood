using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampaignSendNew.Model
{
    public class GroupSetQueueDBModel
    {
        public int idrow { get; set; }
        public long rowId { get; set; } = 0;
        public int tenantId { get; set; }
        public long groupId { get; set; }
        public string contactJson { get; set; }
        public DateTime createdDate { get; set; }
        public bool IsExternalContact { get; set; }
        public bool IsCreated { get; set; }

        public int ItemType { get; set; }
    }
}
