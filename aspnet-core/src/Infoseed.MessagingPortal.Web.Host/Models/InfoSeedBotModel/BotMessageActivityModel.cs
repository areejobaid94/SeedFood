using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Web.Models.InfoSeedBotModel
{
    public class BotMessageActivityModel
    {
        public string Text { get; set; }
        public string Type { get; set; }
        public string Locale { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }

        public string TempName { get; set; }
        public string StepNumber { get; set; }
    }
}
