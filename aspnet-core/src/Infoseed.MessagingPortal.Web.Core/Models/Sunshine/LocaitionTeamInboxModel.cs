using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Web.Models.Sunshine
{
    public class LocaitionTeamInboxModel
    {
        public string userId { get; set; }
        public Location location { get; set; }
        public Coordinates coordinates { get; set; }
    }
}
