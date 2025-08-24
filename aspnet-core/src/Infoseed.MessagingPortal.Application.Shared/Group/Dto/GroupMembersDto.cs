using System.Collections.Generic;

namespace Infoseed.MessagingPortal.Group.Dto
{
    public class GroupMembersDto
    {
        public GroupDtoModel groupDtoModel { get; set; }
        public List<MembersDto> membersDto { get; set; }
        public int totalCount { get; set; }
    }
}
