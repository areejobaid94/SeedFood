using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.WhatsApp.Dto
{
    public class KeyWordModel
    {
        public long id { get; set; }
        public int tenantId { get; set; }
        public string action { get; set; }
        public long actionId { get; set; }

        public int KeyWordType { get; set; } //1- Fuzzy match   2 -Exact matching 3- Contains
        public int FuzzyMatch { get; set; }
        public long KeyUse { get; set; }

        public string triggersBot { get; set; }
        public long triggersBotId { get; set; }
        public string buttonText { get; set; }
    }
}
