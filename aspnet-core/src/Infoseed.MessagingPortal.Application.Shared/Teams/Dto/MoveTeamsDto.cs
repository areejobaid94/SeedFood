using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Teams.Dto
{
    public class MoveTeamsDto
    {
        public long OldGroupId { get; set; }
        public long NewGroupId { get; set; }
        public List<TeamsDto> membersDto { get; set; }
    }
}
