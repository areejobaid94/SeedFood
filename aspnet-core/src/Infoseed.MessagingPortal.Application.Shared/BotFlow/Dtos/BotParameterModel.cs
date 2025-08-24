using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.BotFlow.Dtos
{
    public class BotParameterModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Format { get; set; }
        public bool IsDeleted { get; set; } = false;
        public int TenantId { get; set; }

    }
}
