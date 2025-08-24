using Infoseed.MessagingPortal.Group.Dto;
using Infoseed.MessagingPortal.Teams.Dto;
using System.Collections.Generic;

namespace Infoseed.MessagingPortal.Teams.Dto
{
    public class TeamsModel
    {
        public List<TeamsDtoModel> TeamsDtoModel { get; set; }
        public long total { get; set; }
        public int state { get; set; }
        public string message { get; set; }
    }
}
