using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.BotFlow.Dtos
{
    public class GetBotModelFlowForViewDto
    {
        public long Id { get; set; }
        public bool isPublished { get; set; }
        public int TenantId { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public long CreatedUserId { get; set; }
        public string CreatedUserName { get; set; }
        public DateTime? ModifiedDate { get; set; } = DateTime.UtcNow;
        public long ModifiedUserId { get; set; }
        public string ModifiedUserName { get; set; }
        public string FlowName { get; set; }
        public int StatusId { get; set; }
        public string BotChannel { get; set; }
        public GetBotFlowForViewDto[] getBotFlowForViewDto { get; set; }

    }
}
