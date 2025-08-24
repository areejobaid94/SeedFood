using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Group.Dto
{
    public class GroupSetQueueModel
    {
        public long rowId { get; set; }
        public int tenantId { get; set; }
        public long groupId { get; set; }
        public string functionName { get; set; }
        public string groupName { get; set; }

    }
}
