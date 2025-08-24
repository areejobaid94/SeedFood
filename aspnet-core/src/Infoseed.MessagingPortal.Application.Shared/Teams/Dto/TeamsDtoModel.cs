using System;

namespace Infoseed.MessagingPortal.Teams.Dto
{
    public class TeamsDtoModel
    {        
        public long Id { get; set; }
        public int TenantId { get; set; }
        public string TeamsName { get; set; }
        public DateTime? CreationDate { get; set; } = DateTime.UtcNow;
        public DateTime? ModificationDate { get; set; } = null;
        public bool IsDeleted { get; set; }
        public int TotalNumber { get; set; } = 0;
        public int FailedCount { get; set; } = 0;
        public int OnHoldCount { get; set; } = 0;

        public int TotolForPrograss { get; set; } = 0;
        public string UserIds { get; set; }

    }
}
