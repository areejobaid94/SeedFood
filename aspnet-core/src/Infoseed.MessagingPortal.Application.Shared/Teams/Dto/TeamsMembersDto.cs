using Infoseed.MessagingPortal.Authorization.Users.Dto;
using Infoseed.MessagingPortal.Teams.Dto;
using System.Collections.Generic;

namespace Infoseed.MessagingPortal.Teams.Dto
{
    public class TeamsMembersDto
    {
        public TeamsDtoModel teamsModel { get; set; }
       // public List<UserListDto> membersDto { get; set; }
        public int totalCount { get; set; }

        public bool isCreate { get; set; }
    }
}
