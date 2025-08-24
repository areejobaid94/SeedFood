using System;

namespace Infoseed.MessagingPortal.Group.Dto
{
    public class GroupDtoModel
    {
        public long id { get; set; }
        public int tenantId { get; set; }
        public string groupName { get; set; }
        public DateTime? creationDate { get; set; } = DateTime.UtcNow;
        public DateTime? modificationDate { get; set; } = null;
        public bool IsDeleted { get; set; }
        public int totalNumber { get; set; } = 0;
        public int FailedCount { get; set; } = 0;
        public int OnHoldCount { get; set; } = 0;
        public int TotolForPrograss { get; set; } = 0;
        public string CreatorFullName { get; set; }
    }
}