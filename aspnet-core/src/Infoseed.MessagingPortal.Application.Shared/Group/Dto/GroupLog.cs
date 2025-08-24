using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Group.Dto
{
    public class GroupLog
    {
        public long Id { get; set; }
        public long GroupId { get; set; }
        public long totalCount { get; set; }
        public List<GroupPhoneNumberLog> members { get; set; }

    }
    public class GroupPhoneNumberLog
    {
        public string displayName { get; set; }
        public string PhoneNumber { get; set; }
        public int FailedStatusId { get; set; }
        public string FailureMessage { get; set; }
        public DateTime SetDate { get; set; } = DateTime.UtcNow;
        public string Reason { get; set; } ="";
    }
}
