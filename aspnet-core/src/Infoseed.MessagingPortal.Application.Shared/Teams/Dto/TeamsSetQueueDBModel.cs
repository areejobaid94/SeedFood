using System;

namespace Infoseed.MessagingPortal.Teams.Dto
{
    public class TeamsSetQueueDBModel
    {
        public long rowId { get; set; } = 0;
        public int tenantId { get; set; }
        public long groupId { get; set; }
        public string contactJson { get; set; }
        public DateTime createdDate { get; set; }
        public bool IsExternalContact { get; set; }
        public bool IsCreated { get; set; }
    }
}
