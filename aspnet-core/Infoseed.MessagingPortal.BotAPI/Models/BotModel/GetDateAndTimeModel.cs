using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.BotAPI.Models.BotModel
{
    public class GetDateAndTimeModel
    {
        public List<string> Days { get; set; }
        public List<string> Times { get; set; }
    }
}
